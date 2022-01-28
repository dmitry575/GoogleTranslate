using GoogleTranslate.Common.Models;

namespace GoogleTranslate.Common;

/// <summary>
/// Converting url for google translate
/// </summary>
public interface IConvertHtml
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
    /// <param name="convertInfo">Information what change</param>
    (bool, string) UnConvert(string dirtyContent, Dictionary<int, string> convertInfo);
}