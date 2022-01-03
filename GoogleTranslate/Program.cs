// See https://aka.ms/new-console-template for more information

using System.Reflection;
using CommandLine;
using GoogleTranslate.Config;
using GoogleTranslate.Errors;

Console.WriteLine($"Start program translating files through the Google Translate");
Console.WriteLine($"Version: {Assembly.GetExecutingAssembly().GetName().Version}");
Console.WriteLine("");

var errorHandle = new ErrorHandle();
var configuration = new Configuration();
var arguments = Parser.Default.ParseArguments<Configuration>(args)
    .WithParsed(x => configuration = x)
    .WithNotParsed(errorHandle.HandleParseError);
    
    
if (!errorHandle.HasError)
{

    var checkUrls = new GoogleTranslate(configuration);
    await checkUrls.CheckAsync();

    checkUrls.PrintResult();
}