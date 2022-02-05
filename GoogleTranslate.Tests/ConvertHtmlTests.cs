using GoogleTranslate.Common;
using NUnit.Framework;

namespace GoogleTranslate.Tests
{
    public class ConvertHtmlTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("<pre>testpre</pre>  <code>codo sdad asd</code> Test <code>Code2</code>", " [120] Test [121] ")]
        [TestCase("<p><ul ><li><pre>testpre</pre></li> <li><em>(optional)</em> <code>export WINEDEBUG=fixme-all</code></li> <li>TRUE : <code>y Y t T 1</code></li></ul> </p>", " [120] optional [121] TRUE [122] ")]
        public void ConvertCode(string html, string clean)
        {
            var convert = new ConvertHtml();
            var result = convert.Convert(html);

            Assert.AreEqual(result.Content, clean);
        }
    }
}