using System.Text.RegularExpressions;

namespace GoogleTranslate.Common.Impl;

public class ConvertTags
{
    /// <summary>
    /// Replace group
    /// </summary>
    /// <param name="translate"></param>
    /// <param name="groupPrefix"></param>
    /// <param name="tagsGroups"></param>
    /// <returns></returns>
    /// <exception cref="ConvertException"></exception>
    public static string GetUnGroup(string translate, string groupPrefix, Dictionary<int, string> tagsGroups)
    {
        var result = translate;
        foreach (var tagsGroup in tagsGroups)
        {
            var key = $"{groupPrefix}{tagsGroup.Key}";

            Regex regexTag = new Regex(@"\[\s*(" + key + @")\s*\]");
            if (regexTag.IsMatch(result))
            {
                result = regexTag.Replace(result, "[$1]");
            }

            if (!result.Contains("[" + key + "]"))
            {
                throw new ConvertException($"not found key [{key}] for {tagsGroup.Value}");
            }

            if (tagsGroup.Value.StartsWith("</"))
            {
                result = result.Replace(" [" + key + "]", tagsGroup.Value);
            }
            else
            {
                result = result.Replace("[" + key + "] ", tagsGroup.Value);
            }

            result = result.Replace("[" + key + "]", tagsGroup.Value);
        }

        return result;
    }

    /// <summary>
    /// Clean tags in the text
    /// </summary>
    /// <param name="translate">Dirty text</param>
    /// <param name="prefixTag">Prefix for tags</param>
    /// <param name="prefixGroupTag">Prefix fro group of tags</param>
    public static string CleanOnlyTags(string translate, string prefixTag, string prefixGroupTag)
    {
        string clean = translate;
        Regex regex = new Regex(@"\[\s*(" + prefixTag + @"[0-9]+)\s*\]");
        if (regex.IsMatch(clean))
        {
            clean = regex.Replace(clean, "[$1]");
        }

        Regex regexGroup = new Regex(@"\[\s*(" + prefixGroupTag + @"[0-9]+)\s*\]");
        if (regexGroup.IsMatch(clean))
        {
            clean = regexGroup.Replace(clean, "[$1]");
        }

        return clean;
    }
}
