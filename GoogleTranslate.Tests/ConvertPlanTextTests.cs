using GoogleTranslate.Common.Impl;
using NUnit.Framework;

namespace GoogleTranslate.Tests;

public class ConvertPlanTextTests
{
    [TestCase("test \r\n content", "test [210] content")]
    [TestCase("test \r\n\r\n\r\n content", "test [220] content")]
    public void ConvertCode(string html, string clean)
    {
        var convert = new ConvertPlanText();
        var result = convert.Convert(html);

        Assert.AreEqual(result.Content, clean);
    }
}
