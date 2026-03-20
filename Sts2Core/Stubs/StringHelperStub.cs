// Replacement for StringHelper.cs that avoids GeneratedRegex issues
using System.Globalization;
using System.Text.RegularExpressions;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Saves;

namespace MegaCrit.Sts2.Core.Helpers;

public static class StringHelper
{
    private static readonly Regex _camelCaseRegex = new("([A-Za-z0-9]|\\G(?!^))([A-Z])");
    private static readonly Regex _snakeCaseRegex = new("(.*?)_([a-zA-Z0-9])");
    private static readonly Regex _whitespaceRegex = new("\\s+");
    private static readonly Regex _specialCharRegex = new("[^A-Z0-9_]");

    public static string SnakeCase(string txt) =>
        _camelCaseRegex.Replace(txt.Trim(), "$1_$2").ToLowerInvariant();

    public static string Slugify(string txt)
    {
        string text = _camelCaseRegex.Replace(txt.Trim(), "$1_$2");
        string input = _whitespaceRegex.Replace(text.ToUpperInvariant(), "_");
        return _specialCharRegex.Replace(input, "");
    }

    public static string Unslugify(string txt)
    {
        string text = _snakeCaseRegex.Replace(txt.Trim().ToLowerInvariant(), match =>
        {
            string g1 = match.Groups[1].ToString();
            string g2 = match.Groups[2].ToString();
            return g1 + g2.ToUpperInvariant();
        });
        return char.ToUpperInvariant(text[0]) + text.Substring(1);
    }

    public static string CompactText(string text) => text.Trim();

    public static int GetDeterministicHashCode(string str)
    {
        int num = 352654597;
        int num2 = num;
        for (int i = 0; i < str.Length; i += 2)
        {
            num = ((num << 5) + num) ^ str[i];
            if (i == str.Length - 1) break;
            num2 = ((num2 << 5) + num2) ^ str[i + 1];
        }
        return num + num2 * 1566083941;
    }

    public static string Radix(int value) => value.ToString("N0", CultureInfo.InvariantCulture);

    public static LocString RatioFormat(int numerator, int denominator) =>
        RatioFormat(numerator.ToString(), denominator.ToString());

    public static LocString RatioFormat(string numerator, string denominator)
    {
        LocString ls = new LocString("stats_screen", "RATIO_FORMAT");
        ls.Add("Numerator", numerator);
        ls.Add("Denominator", denominator);
        return ls;
    }

    public static string Capitalize(string input) =>
        char.ToUpperInvariant(input[0]) + input.Substring(1);

    public static string StripBbCode(this string text) =>
        Regex.Replace(text, "\\[(.*?)\\]", "");
}
