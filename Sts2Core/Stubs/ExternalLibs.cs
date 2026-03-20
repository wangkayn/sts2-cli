// Stub external library types referenced by decompiled code

namespace Steamworks
{
    public struct CSteamID { public ulong m_SteamID; }
    public struct HSteamNetConnection { }
    public struct HSteamListenSocket { }
    public struct SteamNetworkingIdentity { }
    public struct SteamNetConnectionStatusChangedCallback_t { }
    public struct PublishedFileId_t { public ulong m_PublishedFileId; }
    public struct SteamAPICall_t { }
    public struct SteamLeaderboard_t { }
    public struct LeaderboardEntry_t { }
    public struct UserStatsReceived_t { }
    public struct GlobalStatsReceived_t { }
    public struct GameLobbyJoinRequested_t { }
    public struct ItemInstalled_t { }
    public struct InputHandle_t { }
    public struct InputDigitalActionHandle_t { }
    public struct InputAnalogActionHandle_t { }
    public struct InputActionSetHandle_t { }

    public enum EResult { k_EResultOK = 1 }
    public enum EInputActionOrigin { }
    public enum EChatRoomEnterResponse { }
    public enum ESteamInputType { }
    public enum ESteamAPIInitResult { k_ESteamAPIInitResult_OK }
    public enum ELeaderboardDataRequest { k_ELeaderboardDataRequestGlobal }

    public class Callback<T>
    {
        public static Callback<T> Create(Action<T> func) => new();
    }
    public class CallResult<T>
    {
        public void Set(SteamAPICall_t call, Action<T, bool> func) { }
    }
}

namespace SmartFormat
{
    public class SmartFormatter
    {
        public string Format(string format, params object[] args) => string.Format(format, args);
    }
}

namespace SmartFormat.Core.Extensions
{
    public interface IFormatter
    {
        string Name { get; set; }
        bool CanAutoDetect { get; set; }
        bool TryEvaluateFormat(IFormattingInfo formattingInfo);
    }
    public interface IFormattingInfo
    {
        object? CurrentValue { get; }
        string? FormatterOptions { get; }
        void Write(string text);
        SmartFormat.Core.Formatting.FormatDetails FormatDetails { get; }
    }
    public interface ISource
    {
        bool TryEvaluateSelector(ISelectorInfo selectorInfo);
    }
    public interface ISelectorInfo
    {
        object? CurrentValue { get; }
        string SelectorText { get; }
        int SelectorIndex { get; }
    }
}

namespace SmartFormat.Core.Parsing
{
    public class Format { }
    public class Selector { }
}

namespace Sentry
{
    public enum BreadcrumbLevel { Debug, Info, Warning, Error, Critical }
    public enum SentryLevel { Debug, Info, Warning, Error, Fatal }
    public class Scope { }
    public static class SentrySdk
    {
        public static void AddBreadcrumb(string message, string? category = null, string? type = null, IDictionary<string, string>? data = null, BreadcrumbLevel level = BreadcrumbLevel.Info) { }
        public static void CaptureException(Exception exception) { }
        public static void CaptureMessage(string message, SentryLevel level = SentryLevel.Info) { }
        public static void ConfigureScope(Action<Scope> configureScope) { }
    }
}

namespace HarmonyLib
{
    public class HarmonyLib { }
}

namespace Godot
{
    // ENet types used in multiplayer transport
    public class ENetConnection
    {
        public enum CompressionMode { None, RangeCoder, Fastlz, Zlib, Zstd }
        public Error CreateHost(int maxPeers, int maxChannels = 0, int inBandwidth = 0, int outBandwidth = 0) => Error.Ok;
        public Error CreateHostBound(string address, int port, int maxPeers, int maxChannels = 0, int inBandwidth = 0, int outBandwidth = 0) => Error.Ok;
        public ENetPacketPeer? ConnectToHost(string address, int port, int channels = 0, int data = 0) => null;
        public Godot.Collections.Array<ENetConnection.EventType> Service(int timeout = 0, object? @event = null) => new();
        public void Destroy() { }
        public enum EventType { None, Connect, Disconnect, Receive, Error }
    }

    public class ENetPacketPeer : GodotObject
    {
        public enum PeerState { Disconnected, Connecting, AcknowledgingConnect, ConnectionPending, ConnectionSucceeded, Connected, DisconnectLater, Disconnecting, AcknowledgingDisconnect, Zombie }
        public void PeerDisconnect(int data = 0) { }
        public Error Send(int channel, byte[] packet, int flags) => Error.Ok;
        public PeerState GetState() => PeerState.Disconnected;
    }
}

namespace SmartFormat.Extensions
{
    public class DefaultSource : SmartFormat.Core.Extensions.IFormatter
    {
        public string Name { get; set; } = "";
        public bool CanAutoDetect { get; set; }
        public bool TryEvaluateFormat(SmartFormat.Core.Extensions.IFormattingInfo formattingInfo) => false;
    }
    public class DefaultFormatter : SmartFormat.Core.Extensions.IFormatter
    {
        public string Name { get; set; } = "";
        public bool CanAutoDetect { get; set; }
        public bool TryEvaluateFormat(SmartFormat.Core.Extensions.IFormattingInfo formattingInfo) => false;
    }
    public class ListFormatter : SmartFormat.Core.Extensions.IFormatter
    {
        public string Name { get; set; } = "";
        public bool CanAutoDetect { get; set; }
        public bool TryEvaluateFormat(SmartFormat.Core.Extensions.IFormattingInfo formattingInfo) => false;
    }
    public class ReflectionSource : SmartFormat.Core.Extensions.IFormatter
    {
        public string Name { get; set; } = "";
        public bool CanAutoDetect { get; set; }
        public bool TryEvaluateFormat(SmartFormat.Core.Extensions.IFormattingInfo formattingInfo) => false;
    }
    public class DictionarySource : SmartFormat.Core.Extensions.IFormatter
    {
        public string Name { get; set; } = "";
        public bool CanAutoDetect { get; set; }
        public bool TryEvaluateFormat(SmartFormat.Core.Extensions.IFormattingInfo formattingInfo) => false;
    }
}

namespace SmartFormat.Core.Formatting
{
    public class FormatDetails { }
    public class FormattingException : Exception
    {
        public FormattingException() { }
        public FormattingException(string message) : base(message) { }
    }
}

// System.IO.Hashing moved to ExcludedNamespaceStubs.cs
