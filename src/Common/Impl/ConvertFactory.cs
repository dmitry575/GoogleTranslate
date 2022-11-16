namespace GoogleTranslate.Common.Impl;

public class ConvertFactory : IConvertFactory
{
    public IConvert Create(bool isHtml)
    {
        return isHtml ? new ConvertHtml() : new ConvertPlanText();
    }
}