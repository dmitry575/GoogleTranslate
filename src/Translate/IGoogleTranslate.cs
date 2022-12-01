namespace GoogleTranslate.Translate;

/// <summary>
/// Main interface for translating through Google Translate 
/// </summary>
public interface IGoogleTranslate
{
    /// <summary>
    /// Translate
    /// </summary>
    void Translate();

    /// <summary>
    /// Printing result
    /// </summary>
    void PrintResult();

    /// <summary>
    /// Translate text
    /// </summary>
    void TranslateText(string text);
}