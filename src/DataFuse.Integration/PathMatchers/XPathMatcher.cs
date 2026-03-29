using System.Text.RegularExpressions;
using DataFuse.Adapters.Abstraction;

namespace DataFuse.Integration.PathMatchers;

public partial class XPathMatcher : ISchemaPathMatcher
{
    [GeneratedRegex(@"=ancestor::(?'path'.*?)(/@|\[.*\]/@)", RegexOptions.Compiled)]
    private static partial Regex AncestorRegex();

    public bool IsMatch(string inputXPath, ISchemaPaths configuredXPaths)
    {
        if (inputXPath is null)
            return false;

        if (configuredXPaths.Paths.Any(x => inputXPath.Contains(x, StringComparison.OrdinalIgnoreCase)))
            return true;

        if (configuredXPaths.Paths.Any(x => inputXPath.Contains("ancestor::")
                && AncestorRegex().Matches(inputXPath).Cast<Match>()
                .Select(match => match.Groups["path"].Value).Distinct().Any(match => x.EndsWith(match))))
            return true;

        return false;
    }
}
