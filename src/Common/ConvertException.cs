namespace GoogleTranslate.Common;

/// <summary>
/// Special exception for converting and converting html
/// </summary>
public class ConvertException : Exception
{
    public ConvertException()
    {
    }

    public ConvertException(string message) : base(message)
    {
    }
}