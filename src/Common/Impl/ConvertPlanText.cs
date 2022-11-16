using System.Text.RegularExpressions;
using GoogleTranslate.Common.Models;

namespace GoogleTranslate.Common.Impl;

public sealed class ConvertPlanText : IConvert
{
    private const string PrefixTag = "21";
    private const string GroupPrefixTag = "22";

    private readonly char[] _listNumbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
    private readonly char[] _listSpaces = { ' ', '\r', '\n' };

    private readonly List<string> _strNotTranslate = new List<string> { "\r\n", "\n" };

    /// <summary>
    /// Clean double spaces
    /// </summary>
    private readonly Regex _regexSpace = new Regex("[ ]{2,}", RegexOptions.None);

    public ConvertResult Convert(string content)
    {
        var result = new ConvertResult();
        (var clean, result.Tags) = GetClean(content);

        // get groups of tags
        (clean, result.Groups) = GetGroup(clean);

        result.Content = clean;

        return result;
    }

    private (string clean, Dictionary<int, string> Tags) GetClean(string content)
    {
        var index = 0;
        var replaced = new Dictionary<int, string>();
        foreach (var str in _strNotTranslate)
        {

            content = Regex.Replace(content, str, match =>
            {
                replaced.Add(index, str);
                return $"[{PrefixTag}{index++}]";
            });

        }

        return (content, replaced);
    }

    /// <summary>
    /// Grouping new data
    /// </summary>
    private (string, Dictionary<int, string>) GetGroup(string clean)
    {
        var groupTags = new Dictionary<int, string>();
        clean = clean.Trim();

        // empty or space symbol
        const int EMPTY = 0;

        // current position int any tags
        const int INSERT_TAG = 1;

        // current position after close tag
        const int AFTER_TAG_EMPTY = 2;

        int status = 0;
        int cur = 0;
        int count = 0;
        int index = 0;

        for (int i = 0; i < clean.Length; i++)
        {
            switch (status)
            {
                case EMPTY:
                    if (clean[i] == '[')
                    {
                        if (clean.Substring(i + 1, 2) == PrefixTag)
                        {
                            cur = i;
                            status = INSERT_TAG;
                            count++;
                        }
                    }

                    break;

                case INSERT_TAG:
                    if (clean[i] == ']')
                    {
                        status = AFTER_TAG_EMPTY;
                        break;
                    }

                    if (!_listNumbers.Contains(clean[i]))
                    {
                        count = 0;
                        status = EMPTY;
                    }


                    break;

                case AFTER_TAG_EMPTY:
                    if (clean[i] == '[')
                    {
                        status = INSERT_TAG;
                        count++;
                        break;
                    }

                    if (!_listSpaces.Contains(clean[i]))
                    {
                        if (count > 1)
                        {
                            groupTags.Add(index++, clean.Substring(cur, i - cur));
                        }

                        count = 0;
                        status = EMPTY;
                    }

                    break;
            }
        }

        if (count > 0 && status == AFTER_TAG_EMPTY && cur > 0)
        {
            groupTags.Add(index, clean.Substring(cur, clean.Length - cur));
        }

        foreach (var key in groupTags.Keys.ToArray())
        {
            clean = clean.Replace(groupTags[key], $" [{GroupPrefixTag}{key}] ");
            groupTags[key] = _regexSpace.Replace(groupTags[key], " ");
        }

        clean = clean.Replace($".[{PrefixTag}", $". [{PrefixTag}");
        clean = clean.Replace($"![{PrefixTag}", $"! [{PrefixTag}");
        clean = clean.Replace($".[{PrefixTag}", $". [{PrefixTag}");
        clean = clean.Replace($"![{PrefixTag}", $"! [{PrefixTag}");

        clean = _regexSpace.Replace(clean, " ");

        // need for google
        clean = clean.Replace(" ,", ", ");

        return (clean, groupTags);
    }

    public string UnConvert(string dirtyContent, Dictionary<int, string> groups, Dictionary<int, string> tags)
    {
        return ReplaceTagToText(
            ConvertTags.GetUnGroup(
                ConvertTags.CleanOnlyTags(dirtyContent, PrefixTag, GroupPrefixTag),
                GroupPrefixTag,
                groups),
            tags);
    }

    private string ReplaceTagToText(string translate, Dictionary<int, string> tags)
    {
        var result = translate;
        foreach (var tag in tags)
        {
            var key = $"{PrefixTag}{tag.Key}";

            Regex regexTag = new Regex(@"\[\s*(" + key + @")\s*\]");
            if (regexTag.IsMatch(result))
            {
                result = regexTag.Replace(result, "[$1]");
            }


            if (!result.Contains("[" + key + "]"))
            {
                throw new ConvertException($"not found key {key} for {tag.Value}");
            }

            result = result.Replace("[" + key + "] ", tag.Value);
            result = result.Replace("[" + key + "]", tag.Value);
        }

        return result;
    }
}
