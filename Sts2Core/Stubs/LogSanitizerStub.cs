using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace MegaCrit.Sts2.Core.Logging;

public static class LogSanitizer
{
    private static readonly string _homeReplacement = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "%USERPROFILE%" : "~";
    private static readonly Regex _steamIdRegex = new("\\b76561\\d{12}\\b");

    public static string Sanitize(string text)
    {
        string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        if (!string.IsNullOrEmpty(folderPath))
        {
            text = text.Replace(folderPath, _homeReplacement);
            string alt = folderPath.Replace('\\', '/');
            if (alt != folderPath) text = text.Replace(alt, _homeReplacement);
        }
        text = _steamIdRegex.Replace(text, ReplaceSteamId);
        return text;
    }

    public static string ReplaceSteamId(Match m)
    {
        ulong id = ulong.Parse(m.Value);
        return "A" + IdAnonymizer.Anonymize(id);
    }
}
