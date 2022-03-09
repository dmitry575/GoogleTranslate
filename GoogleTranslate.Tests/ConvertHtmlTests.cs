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

        [TestCase("<pre>testpre</pre>  <code>codo sdad asd</code> Test <code>Code2</code>", " [120] \r\nTest [121] \r\n")]
        [TestCase("<p><ul ><li><pre>testpre</pre></li> <li><em>(optional)</em> <code>export WINEDEBUG=fixme-all</code></li> <li>TRUE : <code>y Y t T 1</code></li></ul> </p>", " [120] \r\noptional [121] \r\nTRUE [122] \r\n")]
        [TestCase("<pre>testpre</pre>  <code>codo sdad asd</code> <p><i>Test</i></p> <code>Code2</code>", " [120] \r\nTest [121] \r\n")]
        [TestCase("<pre>testpre</pre>  <code>codo sdad asd</code> <p>\r\n\r\n<i>Test</i>\r\n\n</p> <code>Code2</code>", " [120] \r\nTest [121] \r\n")]


        public void ConvertCode(string html, string clean)
        {
            var convert = new ConvertHtml();
            var result = convert.Convert(html);

            Assert.AreEqual(result.Content, clean);
        }
    }
}