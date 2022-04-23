namespace GoogleTranslate.Common;

/// <summary>
/// Special exception for converting and deconverting html
/// </summary>
public class ConvertException : Exception
{
    public ConvertException()
    {
    }

    public ConvertException(string message)
        : base(message)
    {
    }
}