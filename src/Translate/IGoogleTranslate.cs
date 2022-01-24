namespace GoogleTranslate.Translate;

public interface IGoogleTranslate
{
    /// <summary>
    /// Translate
    /// </summary>
    Task TranslateAsync();

    /// <summary>
    /// Printing result
    /// </summary>
    void PrintResult();
}