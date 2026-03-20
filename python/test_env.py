#!/usr/bin/env python3
"""Quick test of the headless STS2 battle simulator."""

import subprocess
import json
import os

DOTNET = os.path.expanduser("~/.dotnet-arm64/dotnet")
PROJECT = os.path.join(os.path.dirname(__file__), "..", "Sts2Headless", "Sts2Headless.csproj")

def main():
    print("Starting Sts2Headless...")
    proc = subprocess.Popen(
        [DOTNET, "run", "--project", PROJECT, "--no-build"],
        stdin=subprocess.PIPE,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        text=True,
    )

    def send(cmd):
        line = json.dumps(cmd)
        proc.stdin.write(line + "\n")
        proc.stdin.flush()
        while True:
            resp = proc.stdout.readline().strip()
            if not resp:
                continue
            try:
                return json.loads(resp)
            except json.JSONDecodeError:
                print(f"  [non-json line]: {resp}")
                continue

    # Read ready signal
    ready = proc.stdout.readline().strip()
    print(f"Ready: {ready}")

    # Init combat
    state = send({"cmd": "init", "character": "Ironclad", "encounter": "CultistsNormal", "seed": "test123"})
    print(f"\n--- INITIAL STATE ---")
    print(f"Type: {state.get('type')}")
    if state.get("type") == "error":
        print(f"Error: {state.get('message')}")
        proc.terminate()
        return

    player = state["allies"][0]
    print(f"Player: {player['name']} HP={player['hp']}/{player['max_hp']}")
    print(f"Energy: {state['energy']}/{state['max_energy']}")
    print(f"Hand ({len(state['hand'])} cards):")
    for card in state["hand"]:
        print(f"  [{card['index']}] {card['name']} (cost={card['cost']}, type={card['type']})")
    print(f"Enemies ({len(state['enemies'])}):")
    for e in state["enemies"]:
        print(f"  [{e['index']}] {e['name']} HP={e['hp']}/{e['max_hp']} Block={e['block']}")
    print(f"Draw pile: {state['draw_pile_count']}, Discard: {state['discard_pile_count']}")

    # Get state again
    state2 = send({"cmd": "state"})
    assert state2["type"] == "state", f"Expected state, got {state2['type']}"
    assert len(state2["hand"]) == 5, f"Expected 5 cards in hand, got {len(state2['hand'])}"
    assert len(state2["enemies"]) == 2, f"Expected 2 enemies, got {len(state2['enemies'])}"
    print("\n--- STATE QUERY: OK ---")

    proc.stdin.close()
    proc.wait(timeout=5)
    stderr = proc.stderr.read()
    if stderr:
        print(f"\nStderr:\n{stderr}")

    print("\n=== ALL TESTS PASSED ===")

if __name__ == "__main__":
    main()
