﻿using System.Reflection;
using CommandLine;
using GoogleTranslate.Common.Impl;
using GoogleTranslate.Config;
using GoogleTranslate.Errors;
using GoogleTranslate.Translate;
using log4net.Config;

[assembly: XmlConfigurator(Watch = true, ConfigFile = "log4net.config")]

Console.WriteLine($"Start program translating files by Google Translate");
Console.WriteLine($"Version: {Assembly.GetExecutingAssembly().GetName().Version}");
Console.WriteLine("");

var errorHandle = new ErrorHandle();
var configuration = new Configuration();

Parser.Default.ParseArguments<Configuration>(args)
    .WithParsed(x => configuration = x)
    .WithNotParsed(errorHandle.HandleParseError);


if (!errorHandle.HasError)
{
    // without dependency injection
    var translate = new GoogleTranslateFiles(configuration,
        new GoogleTranslate.Common.Impl.File(),
        new ConvertFactory(),
        new GoogleTranslateRequest(configuration));

    translate.Translate();

    translate.PrintResult();
}
