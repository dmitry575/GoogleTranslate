namespace GoogleTranslate.Translate;

public class GoogleTranslateRequest:IGoogleTranslateRequest
{
    public Task<string> TranslateAsync(string text, string srcLang, string dstLang)
    {
        throw new NotImplementedException();
    }
}