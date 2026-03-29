using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;

namespace DataFuse.Integration.Helpers.Xml;

public static partial class XmlSanitizer
{
    [GeneratedRegex("&(#)?([a-zA-Z0-9]*);")]
    private static partial Regex XmlEntityRegex();

    public static string Sanitize(string xml)
    {
        if (string.IsNullOrEmpty(xml))
            return xml;

        return XmlEntityRegex().Replace(xml, x =>
        {
            if (x.Groups[1].Value != "#")
                return x.Groups[0].Value;

            var strChar = x.Groups[2].Value;

            if (strChar.Contains('x'))
                strChar = strChar.Replace("x", string.Empty).Trim();

            if (!int.TryParse(strChar, NumberStyles.AllowHexSpecifier, null, out var intChar))
                return x.Groups[0].Value;

            return !XmlConvert.IsXmlChar((char)intChar)
                ? string.Empty
                : x.Groups[0].Value;
        });
    }
}
