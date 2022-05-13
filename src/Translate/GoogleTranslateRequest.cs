using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using GoogleTranslate.Config;
using Newtonsoft.Json;
using Polly;

namespace GoogleTranslate.Translate;

/// <summary>
/// Предложение
/// </summary>
public class GooglesSentence
{
    [JsonProperty("trans")] public string Trans { get; set; }
    [JsonProperty("orig")] public string Orig { get; set; }
    [JsonProperty("backend")] public string Backend { get; set; }
}

public class GoogleResult
{
    [JsonProperty("sentences")] public List<GooglesSentence> Sentenses { get; set; }
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

        string query = $"sl={HttpUtility.UrlEncode(srcLang)}&tl={HttpUtility.UrlEncode(dstLang)}&q={HttpUtility.UrlEncode(text)}";

        var response = await Policy.HandleResult<HttpResponseMessage>(x => x.StatusCode != HttpStatusCode.OK)
            .WaitAndRetryAsync(
                RetryCount,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(4, retryAttempt)))
            .ExecuteAsync(async () =>
                await PostRequest(url, query));

        if (response == null)
        {
            throw new Exception("failed request to " + url + ", " + query);
        }

        var responseStream = await response.Content.ReadAsStreamAsync();
        if (responseStream == null)
        {
            throw new Exception("response stream get request failed");
        }

        // reading json
        string json;
        using (var reader = new StreamReader(responseStream))
            json = await reader.ReadToEndAsync();

        var builder = new StringBuilder();

        if (string.IsNullOrEmpty(json)) return string.Empty;

        try
        {
            var result = JsonConvert.DeserializeObject<GoogleResult>(json);

            if (result is { Sentenses: { Count: > 0 } })
            {
                foreach (var sentence in result.Sentenses.Where(sentence => sentence != null && !string.IsNullOrEmpty(sentence.Trans)))
                {
                    if (string.IsNullOrEmpty(sentence.Trans))
                    {
                        continue;
                    }

                    if (builder.Length > 0)
                    {
                        builder.Append(' ');
                    }

                    var sen = sentence.Trans.Trim();

                    builder.Append(sen);
                    if (sentence.Orig.EndsWith(".") && !sen.EndsWith("."))
                    {
                        builder.Append('.');
                    }
                }
            }
        }
        catch (Exception e)
        {
            throw new Exception("failed parsing json: " + json + ", " + e.Message);
        }

        return builder.ToString();
    }

    /// <summary>
    /// Post request to Google Translate
    /// </summary>
    /// <param name="url">Url for request</param>
    /// <param name="query">Query</param>
    private async Task<HttpResponseMessage> PostRequest(string url, string query)
    {
        var httpClientHandler = new HttpClientHandler()
        {
            PreAuthenticate = false,
            UseDefaultCredentials = true,
        };

        if (!string.IsNullOrEmpty(_config.Proxy))
        {
            httpClientHandler.Proxy = new WebProxy(_config.Proxy);
        }

        using var client = new HttpClient(httpClientHandler);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
        client.DefaultRequestHeaders.Add("User-Agent", "AndroidTranslate/5.3.0.RC02.130475354-53000263 5.1 phone TRANSLATE_OPM5_TEST_1");
        client.DefaultRequestHeaders.Add("Accept-Language", "en_US");

        client.Timeout = TimeSpan.FromSeconds(300);

        var data = new StringContent(query, Encoding.UTF8, "application/x-www-form-urlencoded");

        return await client.PostAsync(url, data);
    }
}