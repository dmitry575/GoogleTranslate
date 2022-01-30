using System.Net;
using System.Text;
using System.Web;
using GoogleTranslate.Config;
using Newtonsoft.Json;
using Polly;

namespace GoogleTranslate.Translate;

/// <summary>
/// Предложение
/// </summary>
public class GoogleSentense
{
    [JsonProperty("trans")] public string Trans { get; set; }
    [JsonProperty("orig")] public string Orig { get; set; }
    [JsonProperty("backend")] public string Backend { get; set; }
}

public class GoogelResult
{
    [JsonProperty("sentences")] public List<GoogleSentense> Sentenses { get; set; }
    [JsonProperty("src")] public string Src { get; set; }
}

public class GoogleTranslateRequest : IGoogleTranslateRequest
{
    private readonly Configuration _config;

    /// <summary>
    /// Count of repeat requests
    /// </summary>
    private const int RetryCount = 5;

    public GoogleTranslateRequest(Configuration config)
    {
        _config = config;
    }

    public async Task<string> TranslateAsync(string text, string srcLang, string dstLang)
    {
        string url = "https://translate.google.com/translate_a/single?client=at&dt=t&dt=ld&dt=qca&dt=rm&dt=bd&dj=1&hl=uk-RU&ie=UTF-8&oe=UTF-8&inputm=2&otf=2&iid=1dd3b944-fa62-4b55-b330-74909a99969e";
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        string query = string.Format("sl={0}&tl={1}&q={2}", HttpUtility.UrlEncode(srcLang), HttpUtility.UrlEncode(dstLang), HttpUtility.UrlEncode(text));

        Dictionary<string, string> options = new Dictionary<string, string>()
        {
            { "Accept", "text/json" },
            { "UserAgent", "AndroidTranslate/5.3.0.RC02.130475354-53000263 5.1 phone TRANSLATE_OPM5_TEST_1" },
            { "Accept-Language", "en_US" },
            { "Content-Type", "application/x-www-form-urlencoded" },
        };

        var response = await Policy.HandleResult<HttpWebResponse>(x => x.StatusCode != HttpStatusCode.OK)
            .WaitAndRetryAsync(
                RetryCount,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(4, retryAttempt)))
            .ExecuteAsync(async () =>
                await PostRequest(url, query));

        if (response == null)
        {
            throw new Exception("failed request to " + url + ", " + query);
        }

        Stream responseStream = response.GetResponseStream();
        if (responseStream == null)
            throw new Exception("request failed");

        string json;
        using (var reader = new StreamReader(responseStream))
        //---
            json = await reader.ReadToEndAsync();

        var transalteText = new StringBuilder();
        
        if (!string.IsNullOrEmpty(json))
        {
            try
            {
                GoogelResult result = JsonConvert.DeserializeObject<GoogelResult>(json);

                if (result is { Sentenses: { Count: > 0 } })
                {
                    foreach (GoogleSentense sentense in result.Sentenses)
                    {
                        if (sentense == null || string.IsNullOrEmpty(sentense.Trans))
                            continue;
                        //
                        if (transalteText.Length > 0) transalteText.Append(" ");
                        transalteText.Append(sentense.Trans.Trim());
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("failed parsing json: " + json + ", " + e.Message);
            }
        }


        return transalteText.ToString();
    }

    private async Task<HttpWebResponse> PostRequest(string url, string query)
    {
        var request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "POST";
        request.Accept = "text/json";
        request.UserAgent = "AndroidTranslate/5.3.0.RC02.130475354-53000263 5.1 phone TRANSLATE_OPM5_TEST_1";
        request.Timeout = 300 * 1000;
        request.Headers.Add("Accept-Language", "en_US");
        request.ContentType = "application/x-www-form-urlencoded";
        var queryBytes = Encoding.UTF8.GetBytes(query);
        request.ContentLength = queryBytes.Length;
        if (!string.IsNullOrEmpty(_config.Proxy))
        {
            request.Proxy = new WebProxy(_config.Proxy);
        }

        await using (var stream = request.GetRequestStream())
            await stream.WriteAsync(queryBytes, 0, queryBytes.Length);
        //--- вычитка данных 
        return (HttpWebResponse)await request.GetResponseAsync();
    }
}