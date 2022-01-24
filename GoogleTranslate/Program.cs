﻿// See https://aka.ms/new-console-template for more information

using System.Reflection;
using CommandLine;
using GoogleTranslate.Config;
using GoogleTranslate.Errors;
using GoogleTranslate.Translate;
using log4net.Config;

[assembly: XmlConfigurator(Watch = true, ConfigFile = "log4net.config")]

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

    var translate = new GoogleTranslateFiles(configuration);
    await translate.TranslateAsync();

    translate.PrintResult();
}
