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

    private static readonly ILog _logger = LogManager.GetLogger(typeof(GoogleTranslateFiles));

    /// <summary>
    /// Configuration of transltion
    /// </summary>
    private readonly Configuration _config;

    /// <summary>
    /// Helping for working with files
    /// </summary>
    private readonly IFile _files;

    /// <summary>
    /// Helping for work with converting text, if it's html text
    /// </summary>
    private readonly IConvertHtml _convert;

    /// <summary>
    /// Sending request to google translate
    /// </summary>
    private readonly IGoogleTranslateRequest _translate;

    private int _bytes = 0;
    private Dictionary<string, int> _filesSuccess = new Dictionary<string, int>();
    private Dictionary<string, int> _filesFailed = new Dictionary<string, int>();

    private readonly object _lockObj = new object();

    public GoogleTranslateFiles(Configuration config, IFile files, IConvertHtml convert, IGoogleTranslateRequest translate)
    {
        _config = config;
        _files = files;
        _convert = convert;
        _translate = translate;

        // checking thread configuration
        if (_config.Threads <= 0 || _config.Threads > 20) _config.Threads = 1;
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
            if (countThreads >= tasks.Count())
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

    /// <summary>
    /// Translate one file
    /// </summary>
    /// <param name="fileName">Full file name</param>
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

                var contentTranslate = content;
                ConvertResult convertResult = new ConvertResult();

                if (_config.IsHtml)
                {
                    convertResult = _convert.Convert(content);
                    contentTranslate = convertResult.Content;

                }
                var sb = new StringBuilder();

                foreach (var chunk in contentTranslate.GetChunks(MaxLengthChunk))
                {
                    var translateText = await _translate.TranslateAsync(chunk, _config.SrcLang, _config.DstLang);
                    sb.Append(translateText);
                }

                string translatedContent;
                try
                {
                    translatedContent = GetTranslatedContent(sb.ToString(), convertResult);
                }
                catch (Exception)
                {
                    _logger.Error($"sourse: {content}\r\n\r\nconvert: {contentTranslate}\r\n\r\ntranslate: {sb}");
                    throw;
                }
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
    /// Get translated content after converting if needed
    /// </summary>
    /// <param name="v"></param>
    /// <param name="convertResult"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private string GetTranslatedContent(string translated, ConvertResult convertResult)
    {
        if (_config.IsHtml)
        {
            return _convert.UnConvert(translated, convertResult.Groups, convertResult.Tags);
        }
        else
        {
            return translated;
        }
    }

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