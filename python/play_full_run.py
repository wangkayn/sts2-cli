#!/usr/bin/env python3
"""
Play a full STS2 run using the headless simulator with a random agent.

LLM mode (DeepSeek / any OpenAI-compatible):
  python3 play_full_run.py 1 Ironclad --llm deepseek --api-key sk-xxx
  python3 play_full_run.py 1 Ironclad --llm deepseek  # reads DEEPSEEK_API_KEY env var
"""

import json
import re
import subprocess
import sys
import random
import os
import urllib.request
import urllib.error
from game_log import GameLogger

def _find_dotnet():
    for p in [os.path.expanduser("~/.dotnet-arm64/dotnet"), os.path.expanduser("~/.dotnet/dotnet"), "dotnet"]:
        try:
            if subprocess.run([p, "--version"], capture_output=True, timeout=5).returncode == 0:
                return p
        except (FileNotFoundError, subprocess.TimeoutExpired):
            continue
    return "dotnet"

DOTNET = _find_dotnet()
PROJECT = os.path.join(os.path.dirname(os.path.dirname(os.path.abspath(__file__))),
                       "src", "Sts2Headless", "Sts2Headless.csproj")


# ─────────────────────────────────────────────────────────────────────────────
# LLM integration (OpenAI-compatible — works with DeepSeek, OpenAI, etc.)
# ─────────────────────────────────────────────────────────────────────────────

LLM_PROVIDERS = {
    "deepseek": {
        "url": "https://api.deepseek.com/v1/chat/completions",
        "model": "deepseek-chat",
        "env_key": "DEEPSEEK_API_KEY",
    },
    "openai": {
        "url": "https://api.openai.com/v1/chat/completions",
        "model": "gpt-4o",
        "env_key": "OPENAI_API_KEY",
    },
}

LLM_SYSTEM_PROMPT = """\
You are an expert Slay the Spire 2 player. Given the current game state, output ONLY a JSON array of actions for this entire decision point.

Rules:
- For combat: plan ALL card plays for this turn, then end_turn. Use the card_index values shown in the HAND list.
- For map/event/shop/rest/reward: typically one action.
- Output ONLY a JSON array, no explanation, no markdown.

Action formats:
  {"action":"play_card","args":{"card_index":0}}
  {"action":"play_card","args":{"card_index":0,"target_index":0}}
  {"action":"end_turn"}
  {"action":"select_map_node","args":{"col":3,"row":1}}
  {"action":"choose_option","args":{"option_index":0}}
  {"action":"select_card_reward","args":{"card_index":0}}
  {"action":"skip_card_reward"}
  {"action":"select_cards","args":{"indices":"0"}}
  {"action":"skip_select"}
  {"action":"buy_card","args":{"card_index":0}}
  {"action":"buy_relic","args":{"relic_index":0}}
  {"action":"buy_potion","args":{"potion_index":0}}
  {"action":"leave_room"}
"""


def _format_state_for_llm(state: dict) -> str:
    """Convert game state dict to a compact text prompt."""
    decision = state.get("decision", "")
    player = state.get("player", {})
    ctx = state.get("context", {})

    lines = [f"Decision: {decision}  Act {ctx.get('act')} Floor {ctx.get('floor')}"]
    lines.append(f"Player HP {player.get('hp')}/{player.get('max_hp')}  Block {player.get('block',0)}  Gold {player.get('gold')}  Energy {state.get('energy',0)}/{state.get('max_energy',0)}")

    for pw in (state.get("player_powers") or []):
        lines.append(f"  PlayerBuff: {pw.get('name')} x{pw.get('amount',1)}")

    if decision == "combat_play":
        energy = state.get("energy", 0)
        lines.append(f"Draw {state.get('draw_pile_count',0)}  Discard {state.get('discard_pile_count',0)}  Round {state.get('round',0)}")
        lines.append("ENEMIES:")
        for e in (state.get("enemies") or []):
            intents = []
            for i in (e.get("intents") or []):
                t = i.get("type","?")
                if i.get("damage"):
                    hits = i.get("hits",1)
                    intents.append(f"{t} {i['damage']}x{hits}" if hits>1 else f"{t} {i['damage']}")
                else:
                    intents.append(t)
            pows = ", ".join(f"{p['name']}({p.get('amount',1)})" for p in (e.get("powers") or []))
            lines.append(f"  [{e['index']}] {e['name']}  HP {e['hp']}/{e['max_hp']}  Block {e['block']}"
                         + (f"  → {', '.join(intents)}" if intents else "")
                         + (f"  [{pows}]" if pows else ""))
        lines.append(f"HAND (energy={energy}):")
        running_energy = energy
        for c in (state.get("hand") or []):
            cost = c.get("cost", 0)
            can_play = c.get("can_play", False)
            if not can_play:
                status = "CANNOT_PLAY"
            elif cost > running_energy:
                status = f"cost:{cost} INSUFFICIENT_ENERGY({running_energy} left)"
            else:
                status = f"cost:{cost} OK({running_energy} left)"
            stats = "  ".join(f"{k}:{v}" for k,v in (c.get("stats") or {}).items())
            kws = " ".join(c.get("keywords") or [])
            lines.append(f"  [{c['index']}] {c['name']}  {status}"
                         + (f"  ({stats})" if stats else "")
                         + (f"  [{kws}]" if kws else ""))
            if c.get("description"):
                lines.append(f"       {str(c['description'])[:100]}")
            if can_play and cost <= running_energy:
                running_energy -= cost  # simulate energy drain for planning
        deck_counts: dict = {}
        for c in (player.get("deck") or []):
            n = c.get("name","?") + ("+" if c.get("upgraded") else "")
            deck_counts[n] = deck_counts.get(n,0) + 1
        lines.append("DECK: " + "  ".join(f"{n}×{cnt}" for n,cnt in sorted(deck_counts.items())))
        lines.append("RELICS: " + "  ".join(r.get("name","?") for r in (player.get("relics") or [])))

    elif decision == "map_select":
        if ctx.get("boss"):
            lines.append(f"Boss: {ctx['boss'].get('name','?')}")
        lines.append("Choices:")
        for c in (state.get("choices") or []):
            nxt = ", ".join(f"{x.get('type','?')}" for x in (c.get("next") or []))
            lines.append(f"  col={c['col']} row={c['row']}  {c.get('type','?')}" + (f"  → {nxt}" if nxt else ""))

    elif decision == "event_choice":
        lines.append(f"Event: {state.get('event_name','?')}")
        if state.get("description"):
            desc = state["description"]
            if isinstance(desc, dict):
                desc = desc.get("zh") or desc.get("en") or ""
            lines.append(str(desc)[:300])
        lines.append("Options:")
        for o in (state.get("options") or []):
            locked = " [LOCKED]" if o.get("is_locked") else ""
            title = o.get("title","?")
            if isinstance(title, dict): title = title.get("zh") or title.get("en") or "?"
            lines.append(f"  [{o['index']}] {title}{locked}")

    elif decision == "rest_site":
        lines.append("Options:")
        for o in (state.get("options") or []):
            enabled = "" if o.get("is_enabled") else " [disabled]"
            name = o.get("name","?")
            if isinstance(name, dict): name = name.get("zh") or name.get("en") or o.get("option_id","?")
            heal = f" (+{o['heal_amount']} HP)" if o.get("heal_amount") else ""
            lines.append(f"  [{o['index']}] {name}{heal}{enabled}")

    elif decision == "card_reward":
        lines.append(f"Gold earned: {state.get('gold_earned',0)}")
        lines.append("Cards offered:")
        for c in (state.get("cards") or []):
            lines.append(f"  [{c['index']}] {c['name']}  {c.get('rarity','')}  {str(c.get('description',''))[:80]}")

    elif decision == "card_select":
        purpose = state.get("purpose","")
        lines.append(f"Purpose: {purpose}  (choose {state.get('min_select',1)}–{state.get('max_select',1)})")
        for c in (state.get("cards") or []):
            lines.append(f"  [{c['index']}] {c['name']}  {str(c.get('description',''))[:80]}")

    elif decision == "shop":
        lines.append("Cards:")
        for c in (state.get("cards") or []):
            if c.get("is_stocked"):
                lines.append(f"  [{c['index']}] {c['name']}  {c.get('cost')}g" + (" [SALE]" if c.get("on_sale") else ""))
        lines.append("Relics:")
        for r in (state.get("relics") or []):
            if r.get("is_stocked"):
                lines.append(f"  [{r['index']}] {r['name']}  {r.get('cost')}g")
        lines.append("Potions:")
        for p in (state.get("potions") or []):
            if p.get("is_stocked"):
                lines.append(f"  [{p['index']}] {p['name']}  {p.get('cost')}g")
        if state.get("card_removal_cost"):
            lines.append(f"Remove card: {state['card_removal_cost']}g")

    return "\n".join(lines)


def call_llm(state: dict, api_key: str, provider: str = "deepseek") -> list[dict]:
    """Call an OpenAI-compatible LLM and return a list of game actions."""
    import time
    cfg = LLM_PROVIDERS.get(provider, LLM_PROVIDERS["deepseek"])
    state_text = _format_state_for_llm(state)
    t0 = time.monotonic()

    payload = json.dumps({
        "model": cfg["model"],
        "messages": [
            {"role": "system", "content": LLM_SYSTEM_PROMPT},
            {"role": "user", "content": state_text},
        ],
        "max_tokens": 512,
        "temperature": 0.2,
    }).encode()

    req = urllib.request.Request(
        cfg["url"],
        data=payload,
        headers={
            "Content-Type": "application/json",
            "Authorization": f"Bearer {api_key}",
        },
    )
    with urllib.request.urlopen(req, timeout=30) as resp:
        body = json.loads(resp.read())

    elapsed = time.monotonic() - t0
    raw = body["choices"][0]["message"]["content"].strip()
    tokens_in = body.get("usage", {}).get("prompt_tokens", 0)
    tokens_out = body.get("usage", {}).get("completion_tokens", 0)
    print(f"  [LLM timing] {elapsed:.2f}s  in:{tokens_in} out:{tokens_out}")
    m = re.search(r'\[.*\]', raw, re.DOTALL)
    if not m:
        raise ValueError(f"LLM did not return JSON array:\n{raw}")
    return json.loads(m.group())


def play_run(seed: str, character: str = "Ironclad", verbose: bool = True, log: bool = True, llm_fn=None):
    """Play a complete run and return the result."""
    logger = GameLogger(character, seed, enabled=log)
    proc = subprocess.Popen(
        [DOTNET, "run", "--no-build", "--project", PROJECT],
        stdin=subprocess.PIPE,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE if not verbose else None,
        text=True,
        bufsize=1,
    )

    def read_json_line() -> dict:
        """Read a line from stdout, skipping non-JSON lines (build warnings etc.)"""
        while True:
            resp_line = proc.stdout.readline().strip()
            if not resp_line:
                raise RuntimeError("No response from simulator (EOF)")
            if resp_line.startswith("{"):
                return json.loads(resp_line)
            # Skip non-JSON lines (build warnings, etc.)
            if verbose:
                print(f"  [skip] {resp_line[:120]}")

    def send(cmd: dict) -> dict:
        line = json.dumps(cmd)
        if verbose:
            print(f"  > {line[:200]}")
        logger.log_action(cmd)
        proc.stdin.write(line + "\n")
        proc.stdin.flush()
        resp = read_json_line()
        logger.log_state(resp)
        if verbose:
            rtype = resp.get("type", "?")
            decision = resp.get("decision", "")
            if rtype == "decision":
                player = resp.get("player", {})
                hp = player.get("hp", "?")
                max_hp = player.get("max_hp", "?")
                gold = player.get("gold", "?")
                act = resp.get("act", "?")
                floor = resp.get("floor", "?")
                print(f"  < {rtype}/{decision} act={act} floor={floor} hp={hp}/{max_hp} gold={gold}")
            else:
                print(f"  < {json.dumps(resp)[:200]}")
        return resp

    try:
        # Read ready message (may need to skip build warnings)
        ready = read_json_line()
        if ready.get("type") != "ready":
            print(f"  Unexpected initial response: {ready}")
            return {"victory": False, "seed": seed, "error": "bad_init"}
        if verbose:
            print(f"Connected: {ready}")

        # Start run
        state = send({"cmd": "start_run", "character": character, "seed": seed})

        step = 0
        max_steps = 500  # Safety limit
        stuck_count = 0
        last_state_key = None

        while step < max_steps:
            step += 1

            if state.get("type") == "error":
                print(f"  ERROR: {state.get('message', 'unknown')}")
                if not llm_fn:
                    break
                # With LLM: try to recover by ending turn or leaving room
                decision_before = state.get("decision", decision)
                if decision_before == "combat_play":
                    state = send({"cmd": "action", "action": "end_turn"})
                else:
                    state = send({"cmd": "action", "action": "leave_room"})
                if state.get("type") == "error":
                    break

            decision = state.get("decision", "")

            # Stuck detection — use comprehensive state key
            hand_len = len(state.get("hand", []))
            enemy_hp = sum(e.get("hp", 0) for e in state.get("enemies", []))
            energy = state.get("energy", 0)
            state_key = f"{decision}:{state.get('round')}:{state.get('player',{}).get('hp')}:{hand_len}:{enemy_hp}:{energy}"
            if state_key == last_state_key:
                stuck_count += 1
                if stuck_count > 5:
                    print(f"  STUCK after {step} steps, forcing quit")
                    ctx = state.get("context", {})
                    return {"victory": False, "seed": seed, "steps": step,
                            "act": state.get("act") or ctx.get("act"),
                            "floor": state.get("floor") or ctx.get("floor"),
                            "hp": state.get("player", {}).get("hp"),
                            "max_hp": state.get("player", {}).get("max_hp")}
            else:
                stuck_count = 0
                last_state_key = state_key

            if decision == "game_over":
                victory = state.get("victory", False)
                player = state.get("player", {})
                ctx = state.get("context", {})
                act = state.get("act") or ctx.get("act")
                floor = state.get("floor") or ctx.get("floor")
                print(f"\n{'VICTORY' if victory else 'DEFEAT'} at act {act}, "
                      f"floor {floor} "
                      f"(HP: {player.get('hp')}/{player.get('max_hp')}, "
                      f"Gold: {player.get('gold')}, "
                      f"Deck: {player.get('deck_size')} cards)")
                return {
                    "victory": victory,
                    "seed": seed,
                    "steps": step,
                    "act": act,
                    "floor": floor,
                    "hp": player.get("hp"),
                    "max_hp": player.get("max_hp"),
                }

            # ── LLM decision path ─────────────────────────────────────────────
            if llm_fn and decision not in ("", "game_over"):
                # Fast-path: no playable cards → just end_turn, no LLM call needed
                if decision == "combat_play":
                    hand = state.get("hand") or []
                    energy = state.get("energy", 0)
                    playable = [c for c in hand if c.get("can_play") and c.get("cost", 0) <= energy]
                    if not playable:
                        state = send({"cmd": "action", "action": "end_turn"})
                        continue

                try:
                    actions = llm_fn(state)
                    if verbose:
                        print(f"  [LLM] {json.dumps(actions)[:300]}")

                    # Snapshot the initial hand so we can remap indices after each play.
                    # When a card is played it's removed from hand; remaining cards shift down.
                    # We track which original-index cards are still in hand as an ordered list.
                    initial_hand = list(state.get("hand") or [])
                    remaining = list(range(len(initial_hand)))  # original indices still in hand

                    llm_ok = True
                    for act in actions:
                        action_name = act.get("action", "")
                        cmd: dict = {"cmd": "action", "action": action_name}
                        args = dict(act.get("args") or {})

                        # Skip cards that were unplayable in the initial snapshot
                        if action_name == "play_card" and "card_index" in args:
                            orig_idx = args["card_index"]
                            orig_card = initial_hand[orig_idx] if orig_idx < len(initial_hand) else None
                            if orig_card and not orig_card.get("can_play", True):
                                if verbose:
                                    print(f"  [LLM] skipping unplayable card {orig_card.get('name')}")
                                continue
                            # Remap from original plan index to current hand position
                            if orig_idx in remaining:
                                args["card_index"] = remaining.index(orig_idx)
                            else:
                                continue  # already played, skip
                        if args:
                            cmd["args"] = args

                        state = send(cmd)
                        if state.get("type") == "error":
                            if verbose:
                                print(f"  [LLM] action failed: {state.get('message')} — recovering")
                            if decision == "combat_play":
                                state = send({"cmd": "action", "action": "end_turn"})
                            else:
                                state = send({"cmd": "action", "action": "leave_room"})
                            llm_ok = False
                            break

                        # After a successful play_card, remove that original index from remaining
                        if action_name == "play_card":
                            orig_idx = act.get("args", {}).get("card_index", 0)
                            if orig_idx in remaining:
                                remaining.remove(orig_idx)

                        if state.get("decision") != decision and action_name != "end_turn":
                            break  # decision changed mid-sequence (e.g. enemy died)
                    if llm_ok or state.get("type") != "error":
                        continue
                except Exception as e:
                    print(f"  [LLM error] {e} — falling back to rule-based")

            elif decision == "map_select":
                choices = state.get("choices", [])
                if not choices:
                    print("  No map choices available!")
                    break
                # Random selection
                choice = random.choice(choices)
                state = send({
                    "cmd": "action",
                    "action": "select_map_node",
                    "args": {"col": choice["col"], "row": choice["row"]}
                })

            elif decision == "combat_play":
                hand = state.get("hand", [])
                energy = state.get("energy", 0)
                enemies = state.get("enemies", [])

                # Simple strategy: play playable cards until out of energy
                playable = [c for c in hand if c.get("can_play", False)
                           and (c.get("cost", 0) <= energy)]

                if playable:
                    card = playable[0]
                    args = {"card_index": card["index"]}
                    # If card needs a target, pick first enemy
                    if card.get("target_type") == "AnyEnemy" and enemies:
                        args["target_index"] = 0
                    state = send({
                        "cmd": "action",
                        "action": "play_card",
                        "args": args
                    })
                else:
                    # End turn - retry a few times if we get "Not in play phase"
                    for retry in range(5):
                        state = send({
                            "cmd": "action",
                            "action": "end_turn"
                        })
                        if state.get("type") != "error":
                            break
                        import time
                        time.sleep(0.5)
                    if state.get("type") == "error":
                        # Try proceeding instead
                        state = send({"cmd": "action", "action": "proceed"})

            elif decision == "event_choice":
                options = state.get("options", [])
                if options:
                    # Pick first unlocked option
                    choice = next((o for o in options if not o.get("is_locked")), options[0])
                    state = send({
                        "cmd": "action",
                        "action": "choose_option",
                        "args": {"option_index": choice["index"]}
                    })
                    if state and state.get("type") == "error":
                        state = send({"cmd": "action", "action": "leave_room"})
                else:
                    state = send({"cmd": "action", "action": "leave_room"})

            elif decision == "rest_site":
                options = state.get("options", [])
                # Prefer heal (HEAL), then smith
                enabled = [o for o in options if o.get("is_enabled", True)]
                heal = next((o for o in enabled if o.get("option_id") == "HEAL"), None)
                choice = heal or (enabled[0] if enabled else None)
                if choice:
                    state = send({
                        "cmd": "action",
                        "action": "choose_option",
                        "args": {"option_index": choice["index"]}
                    })
                    if state and state.get("type") == "error":
                        state = send({"cmd": "action", "action": "leave_room"})
                else:
                    state = send({"cmd": "action", "action": "leave_room"})

            elif decision == "card_reward":
                # Pick the first card offered
                cards = state.get("cards", [])
                if cards:
                    state = send({
                        "cmd": "action",
                        "action": "select_card_reward",
                        "args": {"card_index": 0}
                    })
                else:
                    state = send({"cmd": "action", "action": "skip_card_reward"})

            elif decision == "bundle_select":
                state = send({"cmd": "action", "action": "select_bundle",
                             "args": {"bundle_index": 0}})

            elif decision == "card_select":
                # Auto-select first card
                cards = state.get("cards", [])
                if cards:
                    state = send({"cmd": "action", "action": "select_cards",
                                 "args": {"indices": "0"}})
                else:
                    state = send({"cmd": "action", "action": "skip_select"})

            elif decision == "shop":
                state = send({"cmd": "action", "action": "leave_room"})

            elif decision == "unknown":
                state = send({"cmd": "action", "action": "proceed"})

            else:
                state = send({"cmd": "action", "action": "proceed"})
                state = send({"cmd": "action", "action": "proceed"})

        print(f"  Reached max steps ({max_steps})")
        return {"victory": False, "seed": seed, "steps": step, "timeout": True}

    except Exception as e:
        print(f"  EXCEPTION: {e}")
        return {"victory": False, "seed": seed, "steps": step, "error": str(e)}

    finally:
        logger.close()
        if logger.path:
            print(f"  [log] Saved to {logger.path}")
        try:
            proc.stdin.write(json.dumps({"cmd": "quit"}) + "\n")
            proc.stdin.flush()
        except:
            pass
        try:
            proc.terminate()
            proc.wait(timeout=5)
        except:
            proc.kill()


def main():
    import argparse
    parser = argparse.ArgumentParser(description="Play STS2 headless runs")
    parser.add_argument("num_runs", nargs="?", type=int, default=5)
    parser.add_argument("character", nargs="?", default="Ironclad")
    parser.add_argument("--llm", metavar="PROVIDER", help="LLM provider: deepseek, openai")
    parser.add_argument("--api-key", metavar="KEY", help="API key (or set DEEPSEEK_API_KEY / OPENAI_API_KEY env var)")
    args = parser.parse_args()

    num_runs = args.num_runs
    character = args.character

    llm_fn = None
    if args.llm:
        provider = args.llm.lower()
        cfg = LLM_PROVIDERS.get(provider)
        if cfg is None:
            print(f"Unknown provider '{provider}'. Options: {list(LLM_PROVIDERS)}")
            sys.exit(1)
        api_key = args.api_key or os.environ.get(cfg["env_key"]) or os.environ.get("LLM_API_KEY")
        if not api_key:
            print(f"Set --api-key or {cfg['env_key']} env var")
            sys.exit(1)
        llm_fn = lambda state, _ak=api_key, _p=provider: call_llm(state, _ak, _p)
        print(f"LLM mode: {provider} ({cfg['model']})")

    print(f"Playing {num_runs} runs as {character}")
    print("=" * 60)

    results = []
    for i in range(num_runs):
        seed = f"run_{i+1}"
        print(f"\n--- Run {i+1}/{num_runs} (seed: {seed}) ---")
        result = play_run(seed, character, verbose=True, llm_fn=llm_fn)
        results.append(result)
        print()

    print("\n" + "=" * 60)
    print("SUMMARY")
    print("=" * 60)
    wins = sum(1 for r in results if r and r.get("victory"))
    completed = sum(1 for r in results if r and not r.get("timeout"))
    for i, r in enumerate(results):
        if r:
            status = "WIN" if r.get("victory") else ("TIMEOUT" if r.get("timeout") else "LOSS")
            print(f"  Run {i+1}: {status} | seed={r.get('seed')} steps={r.get('steps')} "
                  f"act={r.get('act')} floor={r.get('floor')}")
    print(f"\nWins: {wins}/{num_runs}, Completed: {completed}/{num_runs}")


if __name__ == "__main__":
    main()
