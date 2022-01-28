namespace GoogleTranslate.Translate;

/// <summary>
/// Request to google translate
/// </summary>
public interface IGoogleTranslateRequest
{
    /// <summary>
    /// Translate text to another language
    /// </summary>
    /// <param name="text">Text witch need to translating</param>
    /// <param name="srcLang">From language</param>
    /// <param name="dstLang">To language</param>
    /// <returns></returns>
    Task<string> TranslateAsync(string text, string srcLang, string dstLang);
}