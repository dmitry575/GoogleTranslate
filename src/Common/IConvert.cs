using GoogleTranslate.Common.Models;

namespace GoogleTranslate.Common;

/// <summary>
/// Converting url for google translate
/// </summary>
public interface IConvert
{
    /// <summary>
    /// Converting html to another format, because Google translate begin translate html tags 
    /// </summary>
    /// <param name="content">Content in html format</param>
    ConvertResult Convert(string content);

    /// <summary>
    /// Unconverted text to clean string
    /// </summary>
    /// <param name="dirtyContent">Content with special tags</param>
    /// <param name="groups">Information what change by group of elements</param>
    /// <param name="tags">Information what tags need change</param>
    string UnConvert(string dirtyContent, Dictionary<int, string> groups, Dictionary<int, string> tags);
}