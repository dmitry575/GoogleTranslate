using System.Text;
using GoogleTranslate.Common;
using GoogleTranslate.Common.Models;
using GoogleTranslate.Config;
using GoogleTranslate.Extensions;
using log4net;

namespace GoogleTranslate.Translate;

/// <summary>
/// Translate files via google translate
/// </summary>
public class GoogleTranslateFiles : IGoogleTranslate
{
    /// <summary>
    /// Max length of text witch can to send to google translate
    /// </summary>
    private const int MaxLengthChunk = 2000;

    /// <summary>
    /// How many times need to split text
    /// </summary>
    private const int SplitTextTimes = 10;

    /// <summary>
    /// If after translated html content get exception, try translate again but MaxLengthChunk divided by 2 
    /// </summary>
    private const int MaxLevel = 10;

    private static readonly ILog _logger = LogManager.GetLogger(typeof(GoogleTranslateFiles));

    /// <summary>
    /// Configuration of translation
    /// </summary>
    private readonly Configuration _config;

    /// <summary>
    /// Helping for working with files
    /// </summary>
    private readonly IFile _files;

    /// <summary>
    /// Helping for work with converting text, if it's html text or plan text
    /// </summary>
    private readonly IConvert _convertor;

    /// <summary>
    /// Sending request to google translate
    /// </summary>
    private readonly IGoogleTranslateRequest _translate;

    private int _bytes = 0;
    private readonly Dictionary<string, int> _filesSuccess = new Dictionary<string, int>();
    private readonly Dictionary<string, int> _filesFailed = new Dictionary<string, int>();

    private readonly object _lockObj = new object();

    public GoogleTranslateFiles(Configuration config, IFile files, IConvertFactory convertFactory, IGoogleTranslateRequest translate)
    {
        _config = config;
        _files = files;
        _convertor = convertFactory.Create(_config.IsHtml);
        _translate = translate;

        // checking thread configuration
        if (_config.Threads <= 0 || _config.Threads > 20)
        {
            _config.Threads = 1;
        }
    }

    public void Translate()
    {
        var files = _files.GetFiles(_config.SrcPath, _config.MaskFiles);

        _logger.Info($"Reading files from {_config.SrcPath} {_config.MaskFiles}");

        var tasks = new Task[Math.Min(_config.Threads, files.Count)];
        var countThreads = 0;
        var currentThread = 0;

        foreach (var file in files)
        {
            if (countThreads >= tasks.Length)
            {
                currentThread = Task.WaitAny(tasks);
                countThreads--;
            }

            tasks[currentThread++] = TranslateFileAsync(file);
            countThreads++;
        }

        Task.WaitAll(tasks.Where(x => x != null).ToArray());

        _logger.Info($"translated {_bytes} bytes");
    }

    private async Task TranslateFileAsync(string fileName)
    {
        using (LogicalThreadContext.Stacks["NDC"].Push($"Filename: {fileName}"))
        {
            try
            {
                _logger.Info($"starting translating file: {fileName}");

                var content = _files.GetContent(fileName);
                if (string.IsNullOrEmpty(content))
                {
                    _logger.Error($"file content is empty: {fileName}");
                    return;
                }


                var convertResult = _convertor.Convert(content);

                var contentTranslate = convertResult.Content;

                var translatedContent = await GetTranslateAsync(contentTranslate, convertResult);

                _files.SaveFiles(fileName, _config.GetDstPath(), _config.AdditionalExt, translatedContent);

                _logger.Info($"translated file of {fileName} saved");

                lock (_lockObj)
                {
                    _filesSuccess.Add(fileName, translatedContent.Length);
                    _bytes += translatedContent.Length;
                }
            }
            catch (Exception e)
            {
                _logger.Error($"translating file {fileName} failed, {e}");
                lock (_lockObj)
                {
                    _filesFailed.Add(fileName, 1);
                }
            }
        }
    }

    /// <summary>
    /// Translating text
    /// </summary>
    private async Task<string> GetTranslateAsync(string contentTranslate, ConvertResult convertResult, int maxChunkLength = MaxLengthChunk, int level = 1)
    {
        var sb = new StringBuilder();

        foreach (var chunk in contentTranslate.GetChunks(maxChunkLength))
        {
            var translateText = await _translate.TranslateAsync(chunk, _config.SrcLang, _config.DstLang);
            if (sb.Length > 0)
            {
                sb.Append(' ');
            }

            sb.Append(translateText);
        }

        string translatedContent;
        try
        {
            translatedContent = _convertor.UnConvert(sb.ToString(), convertResult.Groups, convertResult.Tags);
        }
        catch (ConvertException e)
        {
            _logger.Error($"get translated text failed, current max chunk: {maxChunkLength}, level:{level} : {e}");
            if (level > MaxLevel)
            {
                // throw exception to another handler of exception
                _logger.Error($"get translated text failed, too many attempts");
                throw;
            }

            return await GetTranslateAsync(contentTranslate, convertResult, maxChunkLength / SplitTextTimes, level + 1);
        }
        catch (Exception)
        {
            _logger.Error($"convertHtml: {contentTranslate}\r\n\r\ntranslate: {sb}\r\n\r\n");
            throw;
        }

        return translatedContent;
    }

    /// <summary>
    /// Print result of translating files
    /// </summary>
    public void PrintResult()
    {
        var count = 0;
        foreach (var success in _filesSuccess)
        {
            _logger.Info($"success: {success.Key} {success.Value} bytes");
            count++;
        }

        _logger.Info($"Total success {count}");

        count = 0;
        foreach (var failed in _filesFailed)
        {
            _logger.Info($"failed: {failed.Key} {failed.Value} bytes");
            count++;
        }

        _logger.Info($"Total failed {count}");
    }
}