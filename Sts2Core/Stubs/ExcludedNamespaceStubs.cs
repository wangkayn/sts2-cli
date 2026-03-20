// Empty namespace stubs for excluded directories.
// These satisfy 'using' directives in included code.

namespace MegaCrit.Sts2.Core.Saves
{
    public class SaveManager { public static SaveManager Instance { get; } = new(); public PrefsSave PrefsSave { get; } = new(); public SettingsSave SettingsSave { get; } = new(); public bool SeenFtue(string id) => true; public ProfileSave ProfileSave { get; } = new(); public ProgressSave ProgressSave { get; } = new(); }
    public class PrefsSave { public FastModeType FastMode => FastModeType.Normal; }
    public class SettingsSave { public string Language => "eng"; }
    public class ProfileSave { }
    public class ProgressSave { }
    public class MegaCritSerializerContext { }
    public enum FastModeType { Normal, Fast, Instant }
    public class SerializableRun { }
    public class SerializableExtraPlayerFields { }
    public class ReadSaveResult<T> { }
    public enum ProgressState { InProgress, Completed }
    public class SerializablePlayerRngSet { }
}
namespace MegaCrit.Sts2.Core.Saves.Runs
{
    public class SerializablePlayer { }
    public class SerializableRng { }
    public class SerializableOdds { }
    public class SerializableRelicGrabBag { }
    public class ExtraRunFields { public static ExtraRunFields FromSerializable(ExtraRunFields f) => f ?? new(); }
    public class SerializableCard { }
    public class SerializableRunRngSet { }
    public class SerializableRoom { }
    public class SerializableModifier { }
    public class SerializableReward { }
    public class SerializableRelic { }
    public class SerializablePotion { }
    public class SerializablePlayerOddsSet { }
    public class SerializableMapPoint { }
    public class SerializableRunOddsSet { }
    public class SerializableRoomSet { }
    public class SerializableExtraRunFields { }
    public class SerializableExtraPlayerFields { }
    public class SerializableEnchantment { }
    public class SerializableActModel { }
    public class SerializableActMap { }
    public class SerializableMapDrawings { }
    public class SavedProperties { }
    public class ReadSaveResult<T> { }
    public enum ProgressState { InProgress, Completed }
    public interface ISaveSchema { }
    public enum SerializationCondition { SaveAlways, AlwaysSave, SaveIfNotTypeDefault, SaveIfNotCollectionEmptyOrNull }
    [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field)]
    public class SavedPropertyAttribute : System.Attribute
    {
        public SavedPropertyAttribute() { }
        public SavedPropertyAttribute(SerializationCondition c) { }
        public SavedPropertyAttribute(string key, SerializationCondition c = SerializationCondition.SaveAlways) { }
        public SavedPropertyAttribute(SerializationCondition c, int order) { }
    }
    [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field)]
    public class JsonSerializeConditionAttribute : System.Attribute { public JsonSerializeConditionAttribute(SerializationCondition c) { } }
}
namespace MegaCrit.Sts2.Core.Saves.Managers { }
namespace MegaCrit.Sts2.Core.Saves.MapDrawing { }
namespace MegaCrit.Sts2.Core.Saves.Migrations { }
namespace MegaCrit.Sts2.Core.Saves.Test { }
namespace MegaCrit.Sts2.Core.Saves.Validation { }
namespace MegaCrit.Sts2.Core.Assets
{
    public static class PreloadManager { public static AssetCache Cache { get; } = new(); }
    public class AssetCache
    {
        public Godot.PackedScene GetScene(string path) => new();
        public T? GetAsset<T>(string path) where T : class => null;
    }
    public class AtlasResourceLoader : Godot.ResourceFormatLoader { }
}
namespace MegaCrit.Sts2.Core.Timeline
{
    public class EpochData { }
    public class EpochModel { }
}
namespace MegaCrit.Sts2.Core.Timeline.Epochs
{
    public class EpochChainModel { }
}
namespace MegaCrit.Sts2.Core.Timeline.Stories { }
namespace MegaCrit.Sts2.Core.Platform
{
    public interface IPlatformService { }
    public static class PlatformServices { public static IPlatformService Instance { get; set; } = null!; }
    public enum PlatformType { Steam, None }
}
namespace MegaCrit.Sts2.Core.Platform.Null { }
namespace MegaCrit.Sts2.Core.Platform.Steam
{
    public enum SteamDisconnectionReason { None, Timeout, Kicked, Disconnected }
}
namespace MegaCrit.Sts2.Core.Achievements
{
    public class AchievementManager { public static AchievementManager Instance { get; } = new(); }
    public class Achievement { }
}
namespace MegaCrit.Sts2.Core.Audio.Debug { }
namespace MegaCrit.Sts2.Core.Debug
{
    public class DebugManager { }
}
namespace MegaCrit.Sts2.Core.Daily
{
    public class DailyManager { }
    public class TimeServerResult { }
}
namespace MegaCrit.Sts2.Core.DevConsole
{
    public class DevConsoleManager { }
}
namespace MegaCrit.Sts2.Core.DevConsole.ConsoleCommands { }
namespace MegaCrit.Sts2.Core.ControllerInput
{
    public enum ControllerMappingType { Default }
}
namespace MegaCrit.Sts2.Core.TextEffects { }
namespace MegaCrit.Sts2.Core.RichTextTags { }
// MegaCrit.Sts2.Core.Logging included from decompiled directory
namespace MegaCrit.Sts2.Core.Multiplayer.Transport
{
    public enum NetTransferMode { Unreliable, Reliable, ReliableOrdered }
    public class NetHost { }
    public class NetClient { }
    public interface INetHandler { }
    public enum SteamDisconnectionReason { None }
    public interface INetHostHandler { }
    public interface INetClientHandler { }
    public interface IClientConnectionInitializer { }
}
namespace MegaCrit.Sts2.Core.Leaderboard
{
    public class LeaderboardManager { }
}
namespace MegaCrit.Sts2.Core.Multiplayer.Transport.Steam { }
namespace MegaCrit.Sts2.Core.Multiplayer.Connection
{
    public interface IClientConnectionInitializer { }
}
namespace MegaCrit.Sts2.GameInfo { }
namespace MegaCrit.Sts2.GameInfo.Objects { }
namespace MegaCrit.Sts2.SourceGeneration
{
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Interface)]
    internal sealed class GenerateSubtypesAttribute : System.Attribute
    {
        public System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes DynamicallyAccessedMemberTypes { get; set; }
    }
}
// MegaCrit.Sts2.Core.Localization included from decompiled directory
namespace MegaCrit.Sts2.Core.Runs.Metrics { }
namespace System.Text.RegularExpressions.Generated { }
namespace System.IO.Hashing
{
    public sealed class XxHash64
    {
        public static byte[] Hash(ReadOnlySpan<byte> source) => new byte[8];
        public static byte[] Hash(byte[] source) => new byte[8];
    }
}
