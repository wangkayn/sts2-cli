using System.Text.Json;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Unlocks;

namespace Sts2Headless;

public class SimBridge
{
    private RunState? _runState;
    private CombatState? _combatState;

    private static bool _modelDbInitialized;

    private static void EnsureModelDbInitialized()
    {
        if (_modelDbInitialized) return;
        _modelDbInitialized = true;

        // Enable test mode to skip all UI/VFX/SFX
        MegaCrit.Sts2.Core.TestSupport.TestMode.IsOn = true;

        // Initialize ModelDb (register all 1601 model types)
        // Some model constructors may call Godot APIs that segfault.
        // Register each type individually, skipping failures.
        var subtypes = MegaCrit.Sts2.Core.Models.AbstractModelSubtypes.All;
        int registered = 0, failed = 0;
        for (int i = 0; i < subtypes.Count; i++)
        {
            try
            {
                ModelDb.Inject(subtypes[i]);
                registered++;
            }
            catch (Exception ex)
            {
                failed++;
                Console.Error.WriteLine($"[WARN] Failed to register {subtypes[i].Name}: {ex.GetType().Name}: {ex.Message}");
            }
        }
        Console.Error.WriteLine($"[INFO] ModelDb: {registered} registered, {failed} failed out of {subtypes.Count}");
    }

    public Dictionary<string, object?> InitCombat(string characterName, string encounterName, string? seed = null)
    {
        try
        {
            EnsureModelDbInitialized();

            Console.Error.WriteLine("[INFO] Creating player...");
            var player = CreatePlayer(characterName);
            Console.Error.Flush();
            if (player == null)
                return Error($"Unknown character: {characterName}");
            Console.Error.WriteLine("[INFO] Player created. Creating RunState...");

            _runState = RunState.CreateForTest(
                players: new[] { player },
                seed: seed ?? "headless_test"
            );

            var encounter = FindEncounter(encounterName);
            if (encounter == null)
                return Error($"Unknown encounter: {encounterName}. Available: {string.Join(", ", ModelDb.AllEncounters.Select(e => e.GetType().Name))}");

            var mutableEncounter = (EncounterModel)encounter.MutableClone();
            mutableEncounter.GenerateMonstersWithSlots(_runState);

            _combatState = new CombatState(
                mutableEncounter,
                _runState,
                _runState.Modifiers,
                _runState.MultiplayerScalingModel
            );

            // Add players to combat state
            foreach (var p in _runState.Players)
            {
                _combatState.AddPlayer(p);
            }

            // Add enemy creatures
            Console.Error.WriteLine($"[INFO] Encounter has {mutableEncounter.MonstersWithSlots?.Count ?? 0} monsters");
            if (mutableEncounter.MonstersWithSlots != null)
            {
                foreach (var pair in mutableEncounter.MonstersWithSlots)
                {
                    var monster = pair.Item1;
                    var slot = pair.Item2;
                    Console.Error.WriteLine($"[INFO] Adding monster: {monster?.GetType().Name ?? "null"} to slot: {slot ?? "null"}");
                    var creature = _combatState.CreateCreature(monster, CombatSide.Enemy, slot);
                    _combatState.AddCreature(creature);
                }
            }

            Console.Error.WriteLine($"[INFO] Creatures: {_combatState.Creatures?.Count() ?? 0}, Enemies: {_combatState.Enemies?.Count() ?? 0}, Allies: {_combatState.Allies?.Count() ?? 0}, Players: {_combatState.Players?.Count ?? 0}");
            CombatManager.Instance.SetUpCombat(_combatState);
            // Draw initial hand manually (bypass async StartCombatInternal which needs Godot nodes)
            var pcs = player.PlayerCombatState;
            int drawCount = CombatManager.baseHandDrawCount; // 5
            var drawPile = pcs.DrawPile;
            var hand = pcs.Hand;
            for (int i = 0; i < drawCount && drawPile.Cards.Count > 0; i++)
            {
                var card = drawPile.Cards[0];
                drawPile.RemoveInternal(card, silent: true);
                hand.AddInternal(card, silent: true);
            }
            // Set initial energy
            pcs.Energy = player.MaxEnergy;
            Console.Error.WriteLine($"[INFO] Combat started. Hand: {hand.Cards.Count} cards, Energy: {pcs.Energy}, DrawPile: {drawPile.Cards.Count}");

            return GetState();
        }
        catch (Exception ex)
        {
            var inner = ex;
            while (inner.InnerException != null) inner = inner.InnerException;
            return Error($"Init failed: {ex.GetType().Name}: {ex.Message}\nRoot cause: {inner.GetType().Name}: {inner.Message}\n{inner.StackTrace}");
        }
    }

    public Dictionary<string, object?> GetState()
    {
        if (_combatState == null || _runState == null)
            return Error("No combat in progress");

        try
        {
            var player = _runState.Players[0];
            var pcs = player.PlayerCombatState;

            var hand = pcs?.Hand?.Cards?.Select((c, i) => new Dictionary<string, object?>
            {
                ["index"] = i,
                ["id"] = c.Id.ToString(),
                ["name"] = c.GetType().Name,
                ["cost"] = c.EnergyCost?.GetResolved() ?? 0,
                ["type"] = c.Type.ToString(),
            }).ToList() ?? new List<Dictionary<string, object?>>();

            var enemies = _combatState.Enemies?
                .Where(e => e != null && e.IsAlive)
                .Select((e, i) => new Dictionary<string, object?>
                {
                    ["index"] = i,
                    ["name"] = e.Monster?.GetType().Name ?? "Unknown",
                    ["hp"] = e.CurrentHp,
                    ["max_hp"] = e.MaxHp,
                    ["block"] = e.Block,
                }).ToList() ?? new List<Dictionary<string, object?>>();

            var allies = _combatState.Allies?
                .Where(a => a != null && a.IsAlive)
                .Select(a => new Dictionary<string, object?>
                {
                    ["name"] = a.Player?.Character?.GetType().Name ?? "Player",
                    ["hp"] = a.CurrentHp,
                    ["max_hp"] = a.MaxHp,
                    ["block"] = a.Block,
                }).ToList() ?? new List<Dictionary<string, object?>>();

            return new Dictionary<string, object?>
            {
                ["type"] = "state",
                ["round"] = _combatState.RoundNumber,
                ["energy"] = pcs?.Energy ?? 0,
                ["max_energy"] = pcs?.MaxEnergy ?? 0,
                ["hand"] = hand,
                ["enemies"] = enemies,
                ["allies"] = allies,
                ["draw_pile_count"] = pcs?.DrawPile?.Cards?.Count ?? 0,
                ["discard_pile_count"] = pcs?.DiscardPile?.Cards?.Count ?? 0,
            };
        }
        catch (Exception ex)
        {
            return Error($"GetState failed: {ex.GetType().Name}: {ex.Message}");
        }
    }

    private Player? CreatePlayer(string characterName)
    {
        return characterName.ToLowerInvariant() switch
        {
            "ironclad" => Player.CreateForNewRun<Ironclad>(UnlockState.all, 1uL),
            "silent" => Player.CreateForNewRun<Silent>(UnlockState.all, 1uL),
            "defect" => Player.CreateForNewRun<Defect>(UnlockState.all, 1uL),
            "regent" => Player.CreateForNewRun<Regent>(UnlockState.all, 1uL),
            _ => null
        };
    }

    private EncounterModel? FindEncounter(string name)
    {
        return ModelDb.AllEncounters.FirstOrDefault(e =>
            e.GetType().Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    private static Dictionary<string, object?> Error(string message) =>
        new() { ["type"] = "error", ["message"] = message };
}
