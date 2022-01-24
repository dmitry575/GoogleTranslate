using System.Text;
using GoogleTranslate.Config;
using log4net;

namespace GoogleTranslate.Translate;

/// <summary>
/// Translate files via google translate
/// </summary>
public class GoogleTranslateFiles : IGoogleTranslate
{
    private static readonly ILog _logger = LogManager.GetLogger(typeof(GoogleTranslateFiles)); 

    private readonly Configuration _config;
    private readonly IFile _files;
    private readonly IConvertHtml _convert;
    private readonly IGoogleTranslateRequest _translate;

    private int _bytes = 0;
    private Dictionary<string,int> _filesSuccess = new Dictionary<string, int>();
    private Dictionary<string,int> _filesFailed = new Dictionary<string, int>();
    
    public GoogleTranslateFiles(Configuration config, IFile files, IConvertHtml convert, IGoogleTranslateRequest translate)
    {
        _config = config;
        _files = files;
        _convert = convert;
        _translate = translate;
    }

    public Task TranslateAsync()
    {
        var files = _files.GetFiles();

        foreach (var file in files)
        {
            var content = _files.GetContent(file);
            if (string.IsNullOrEmpty(content))
            {
                _logger.Error($"file content is empty: {file}");
                continue;
            }

            var (chunckes, infos) = _convert.GetConverts(content);
            var sb = new StringBuilder();
            foreach (var chunck in chunckes)
            {
                var translateText = await _translate.TranslateAsync(chunck);
                sb.Append(translateText);
            }

            var (res,translatedContent) = _convert.UnConvert(sb.ToString(), infos);

            if (res)
            {
                _files.SaveFiles(translatedContent);
                _filesSuccess.Add(file,translatedContent.Length);
            }
            else
            {
             _filesFailed.Add(file,1);   
            }
        }
    }

    public void PrintResult()
    {
        throw new NotImplementedException();
    }
}