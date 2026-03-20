// Comprehensive stubs for the excluded Nodes layer.
// All N-type singletons return null (code uses ?. access).
// All UI/VFX types are empty shells to satisfy type references.
using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Combat;

namespace MegaCrit.Sts2.Core.Nodes
{
    public class NGame : Control
    {
        public static NGame? Instance => null;
        public static bool IsMainThread() => true;
        public NSceneContainer? RootSceneContainer => null;
        public Control? CurrentRunNode => null;
    }
    public class NRun : Control
    {
        public static NRun? Instance => null;
    }
    public class NSceneContainer : Control { }
    public class NOneTimeInitialization : Node { }
    public class NTransition : Control { }
}

namespace MegaCrit.Sts2.Core.Nodes.Combat
{
    public class NCombatRoom : Control
    {
        public static NCombatRoom? Instance => null;
        public Control? CombatVfxContainer => null;
        public NCreature? GetCreatureNode(Creature c) => null;
    }
    public class NCombatUi : Control { }
    public class NCreature : Node2D
    {
        public Vector2 VfxSpawnPosition => Vector2.Zero;
        public Vector2 GetBottomOfHitbox() => Vector2.Zero;
    }
    public class NCreatureVisuals : Node2D
    {
        public void SetVisible(bool v) { }
    }
    public class NPlayerHand : Control { }
    public class NCardPlayQueue : Control { }
    public class NCombatStartBanner : Control
    {
        public static NCombatStartBanner Create() => new();
    }
    public class NProceedButton : Control { }
}

namespace MegaCrit.Sts2.Core.Nodes.Cards
{
    public class NCard : Control { }
    public class NCardHolder : Control { }
}

namespace MegaCrit.Sts2.Core.Nodes.Audio
{
    public class NAudioManager : Node
    {
        public static NAudioManager? Instance => null;
        public void PlayOneShot(string sfx, float volume = 1f) { }
        public void PlayOneShot(string sfx, System.Collections.Generic.Dictionary<string, float> p, float volume = 1f) { }
        public void PlayLoop(string sfx, bool usesLoopParam = true) { }
        public void StopLoop(string sfx) { }
        public void SetParam(string sfx, string param, float value) { }
    }
    public class NRunMusicController : Node
    {
        public static NRunMusicController? Instance => null;
        public void PlayCustomMusic(string? bgm) { }
        public void UpdateTrack() { }
    }
}

namespace MegaCrit.Sts2.Core.Nodes.Rooms
{
    public class NCombatBackground : Control { }
    public class NMerchantButton : Control { }
    public class NTreasureRoom : Control { }
    public class NEventRoom : Control { }
    public class NRestSiteRoom : Control { }
    public class NMapRoom : Control { }
    public class NMerchantRoom : Control { }
    public enum CombatRoomMode { ActiveCombat, FinishedCombat, VisualOnly }
}

namespace MegaCrit.Sts2.Core.Nodes.CommonUi
{
    public class NGlobalUi : Control { }
    public class NTopBar : Control { }
    public class NControllerManager : Control
    {
        public static NControllerManager Instance { get; } = new();
        public bool IsUsingController => false;
    }
    public class NModalContainer : Control
    {
        public static NModalContainer? Instance => null;
        public void Add(Control c, bool showBackstop = true) { }
    }
    public class NInputManager : Control
    {
        public static NInputManager? Instance => null;
    }
    public class NCursorManager : Control { }
    public class NHotkeyManager : Control { }
    public enum CardPreviewStyle { Default, Full, Compact, HorizontalLayout }
}

namespace MegaCrit.Sts2.Core.Nodes.Screens
{
    public class NRunSubmenuStack : Control { }
    public class NCapstoneSubmenuStack : Control { }
    public class NRewardsScreen : Control { }
    public class ScreenStateTracker { }
}

namespace MegaCrit.Sts2.Core.Nodes.Screens.MainMenu
{
    public class NMainMenu : Control { }
    public class NMainMenuSubmenuStack : Control { }
}

namespace MegaCrit.Sts2.Core.Nodes.Screens.Map
{
    public class NMapScreen : Control
    {
        public static NMapScreen? Instance => null;
        public new class SignalName : Control.SignalName
        {
            public static readonly StringName Opened = "Opened";
        }
    }
    public enum DrawingMode { None, Draw, Erase }
}

namespace MegaCrit.Sts2.Core.Nodes.Screens.Overlays
{
    public class NOverlayStack : Control
    {
        public static NOverlayStack? Instance => null;
    }
}

namespace MegaCrit.Sts2.Core.Nodes.Screens.CardSelection
{
    public class NCardRewardSelectionScreen : Control { }
    public class NCardSelectionScreen : Control { }
}

namespace MegaCrit.Sts2.Core.Nodes.Screens.Capstones
{
    public class NCapstoneContainer : Control
    {
        public static NCapstoneContainer? Instance => null;
    }
}

namespace MegaCrit.Sts2.Core.Nodes.Screens.Timeline
{
    public class NEpochScreen : Control { }
    public enum EpochSlotState { Locked, Available, Completed }
}

namespace MegaCrit.Sts2.Core.Nodes.Screens.GameOverScreen
{
    public class NGameOverScreen : Control { }
}

namespace MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect
{
    public class NCharacterSelectScreen : Control { }
}

namespace MegaCrit.Sts2.Core.Nodes.Screens.CustomRun { }
namespace MegaCrit.Sts2.Core.Nodes.Screens.DailyRun { }
namespace MegaCrit.Sts2.Core.Nodes.Screens.FeedbackScreen
{
    public class FeedbackData { }
}
namespace MegaCrit.Sts2.Core.Nodes.Screens.InspectScreens { }
namespace MegaCrit.Sts2.Core.Nodes.Screens.ModdingScreen { }
namespace MegaCrit.Sts2.Core.Nodes.Screens.PotionLab { }
namespace MegaCrit.Sts2.Core.Nodes.Screens.ProfileScreen { }
namespace MegaCrit.Sts2.Core.Nodes.Screens.RelicCollection { }
namespace MegaCrit.Sts2.Core.Nodes.Screens.RunHistoryScreen { }
namespace MegaCrit.Sts2.Core.Nodes.Screens.Settings { }
namespace MegaCrit.Sts2.Core.Nodes.Screens.Shops { }
namespace MegaCrit.Sts2.Core.Nodes.Screens.StatsScreen { }
namespace MegaCrit.Sts2.Core.Nodes.Screens.TreasureRoomRelic { }
namespace MegaCrit.Sts2.Core.Nodes.Screens.Bestiary { }
namespace MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary { }

namespace MegaCrit.Sts2.Core.Nodes.Vfx
{
    public class NSpeechBubbleVfx : Node2D { }
    public class NSleepingVfx : Node2D { }
    public class NMonsterDeathVfx : Node2D { }
    public class NSovereignBladeVfx : Node2D { }
    public enum VfxColor { White, Red, Blue, Green, Yellow, Purple }
    // VFX scene path stubs
    public class NItemThrowVfx : Node2D { public const string scenePath = ""; }
    public class NSplashVfx : Node2D { public const string scenePath = ""; }
    public class NLiquidOverlayVfx : Node2D { public const string scenePath = ""; }
    public class NLargeMagicMissileVfx : Node2D { public const string scenePath = ""; }
    public class NBigSlashVfx : Node2D { public const string scenePath = ""; }
    public class NBigSlashImpactVfx : Node2D { public const string scenePath = ""; }
    public class NSmallMagicMissileVfx : Node2D { public const string scenePath = ""; }
    public class NGaseousImpactVfx : Node2D { public const string scenePath = ""; }
    public class NDaggerSprayFlurryVfx : Node2D { public const string scenePath = ""; }
    public class NDaggerSprayImpactVfx : Node2D { public const string scenePath = ""; }
    public class NFireBurstVfx : Node2D { public const string scenePath = ""; }
    public class NFireBurningVfx : Node2D { public const string scenePath = ""; }
    public class NPoisonImpactVfx : Node2D { public const string scenePath = ""; }
    public class NScratchVfx : Node2D { public const string scenePath = ""; }
    public class NSporeImpactVfx : Node2D { public const string scenePath = ""; }
    public class NShivThrowVfx : Node2D { public const string scenePath = ""; }
    public class NHyperbeamVfx : Node2D { public const string scenePath = ""; }
    public class NHyperbeamImpactVfx : Node2D { public const string scenePath = ""; }
    public class NMinionDiveBombVfx : Node2D { public const string scenePath = ""; }
    public class NSweepingBeamImpactVfx : Node2D { public const string scenePath = ""; }
    public class NSweepingBeamVfx : Node2D { public const string scenePath = ""; }
    public class NGoopyImpactVfx : Node2D { public const string scenePath = ""; }
    public class NWormyImpactVfx : Node2D { public const string scenePath = ""; }
    public class NHealNumVfx : Node2D
    {
        public static System.Collections.Generic.IEnumerable<string> AssetPaths => System.Array.Empty<string>();
    }
}

namespace MegaCrit.Sts2.Core.Nodes.Vfx.Ui
{
    public class NLowHpBorderVfx : Node2D { public const string scenePath = ""; }
    public class NGaseousScreenVfx : Node2D { }
}

namespace MegaCrit.Sts2.Core.Nodes.Vfx.Backgrounds
{
    public class NKaiserCrabBossBackground : Node2D { }
}

namespace MegaCrit.Sts2.Core.Nodes.Vfx.Utilities
{
    public class NParticlesContainer : Node2D { }
    public class LocalizedTexture : Godot.Resource { }
}

namespace MegaCrit.Sts2.Core.Nodes.Events
{
    public class NAncientEventLayout : Control { }
    public class NEventChoice : Control { }
}

namespace MegaCrit.Sts2.Core.Nodes.Events.Custom
{
    public class NFakeMerchant : Control { }
}

namespace MegaCrit.Sts2.Core.Nodes.Events.Custom.CrystalSphere { }

namespace MegaCrit.Sts2.Core.Nodes.GodotExtensions
{
    public class NClickableControl : Control
    {
        public new class SignalName : Control.SignalName
        {
            public static readonly StringName Released = "Released";
            public static readonly StringName Focused = "Focused";
        }
    }
    public class NButton : NClickableControl { }
    public class NScrollableContainer : Control { }
    public class NSlider : Control { }
}

namespace MegaCrit.Sts2.Core.Nodes.Animation { }
namespace MegaCrit.Sts2.Core.Nodes.Debug { }
namespace MegaCrit.Sts2.Core.Nodes.Debug.Multiplayer { }

namespace MegaCrit.Sts2.Core.Nodes.Vfx.Cards { }

namespace MegaCrit.Sts2.Core.Nodes.Cards.Holders { }

namespace MegaCrit.Sts2.Core.Nodes.Ftue
{
    public class NCombatRulesFtue : Control
    {
        public static NCombatRulesFtue Create() => new();
        public void Start() { }
    }
}

namespace MegaCrit.Sts2.Core.Nodes.HoverTips
{
    public class NHoverTipManager : Control { }
}

namespace MegaCrit.Sts2.Core.Nodes.Multiplayer { }
namespace MegaCrit.Sts2.Core.Nodes.Orbs { }

namespace MegaCrit.Sts2.Core.Nodes.Pooling
{
    public interface IPoolable { }
    public static class NodePool
    {
        public static void Free(IPoolable p) { }
    }
}

namespace MegaCrit.Sts2.Core.Nodes.Potions { }

namespace MegaCrit.Sts2.Core.Nodes.Reaction
{
    public class NReactionContainer : Control { }
    public class NReactionWheel : Control { }
}

namespace MegaCrit.Sts2.Core.Nodes.Relics
{
    public class NRelicFlashVfx : Control { }
}

namespace MegaCrit.Sts2.Core.Nodes.RestSite { }
namespace MegaCrit.Sts2.Core.Nodes.Rewards { }
namespace MegaCrit.Sts2.Core.Nodes.TreasureRooms { }

namespace MegaCrit.Sts2.Core.Multiplayer.Transport.ENet
{
    public class ENetTransport { }
}
