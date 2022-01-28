using GoogleTranslate.Common.Models;

namespace GoogleTranslate.Common;

public class ConvertHtml : IConvertHtml
{
    public ConvertResult Convert(string content)
    {
        throw new NotImplementedException();
    }

    public (bool, string) UnConvert(string dirtyContent, Dictionary<int, string> convertInfo)
    {
        throw new NotImplementedException();
    }
}