using CommandLine;

namespace GoogleTranslate.Config;

/// <summary>
/// Configuration fo translating
/// </summary>
public class Configuration
{
    [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
    public bool Verbose { get; set; }

    [Option('s', "srcLang", Required = true, HelpText = "Set language will be translate from")]
    public string SrcLang { get; set; } = "en";

    [Option('d', "dstLang", Required = true, HelpText = "Set language will be translate to")]
    public string DstLang { get; set; } = "ru";

    [Option('p', "proxy", Required = false, HelpText = "Http Proxy for request to google translate, i.e.: 192.168.1.1:3128")]
    public string Proxy { get; set; }

    [Option('t', "threads", Required = false, HelpText = "Count of threads for translating files, default = 1, max = 20")]
    public int Threads { get; set; } = 1;

    [Option('r', "srcPath", Required = true, HelpText = "Source path where files for translations")]
    public string SrcPath { get; set; }

    [Option('o', "dstPath", Required = true, HelpText = "Output path for translated files")]
    public string DstPath { get; set; }

    [Option('a', "dstPath", Required = true, HelpText = "Additional ext for translated files, for example: source file.txt, the result file_en.ru.txt, you need use -a _en.ru")]
    public string AdditionalExt { get; set; }

    [Option('m', "maskFiles", Required = false, HelpText = "Max of file for translating, default *.txt")]
    public string MaskFiles { get; set; } = "*.txt";
    
    [Option('t', "html", Required = false, HelpText = "Is it html files, if html turn on special converting of content before sending to Google.Translate")]
    public bool IsHtml { get; set; };
}