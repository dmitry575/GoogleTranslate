using System.Text;
using GoogleTranslate.Common;
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
    private const int MaxLengthChunk = 3000;

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

    public GoogleTranslateFiles(Configuration config, IFile files, IConvertHtml convert, IGoogleTranslateRequest translate)
    {
        _config = config;
        _files = files;
        _convert = convert;
        _translate = translate;
    }

    public async Task TranslateAsync()
    {
        var files = _files.GetFiles(_config.SrcPath, _config.MaskFiles);

        foreach (var file in files)
        {
            await TranslateFile(file);
        }
    }

    /// <summary>
    /// Translate one file
    /// </summary>
    /// <param name="fileName">Full file name</param>
    private async Task TranslateFile(string fileName)
    {
        try
        {
            var content = _files.GetContent(fileName);
            if (string.IsNullOrEmpty(content))
            {
                _logger.Error($"file content is empty: {fileName}");
                return;
            }

            var convertResult = _convert.Convert(content);
            var sb = new StringBuilder();

            foreach (var chunck in convertResult.Content.GetChunks(MaxLengthChunk))
            {
                var translateText = await _translate.TranslateAsync(chunck, _config.SrcLang, _config.DstLang);
                sb.Append(translateText);
            }

            var translatedContent = _convert.UnConvert(sb.ToString(), convertResult.Groups, convertResult.Tags);

            _files.SaveFiles(fileName, _config.SrcPath, _config.AdditionalExt, translatedContent);
            _filesSuccess.Add(fileName, translatedContent.Length);
            _bytes += translatedContent.Length;
        }
        catch (Exception e)
        {
            _logger.Error($"translating file {fileName} failed, {e}");
            _filesFailed.Add(fileName, 1);
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