using System.Reflection;
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
    if (configuration.Service > 0) {

        // starting as service
        StartApp();
    }
    else
    {

        // without dependency injection
        var translate = new GoogleTranslateFiles(configuration,
            new GoogleTranslate.Common.Impl.File(),
            new ConvertFactory(),
            new GoogleTranslateRequest(configuration));

        translate.Translate();

        translate.PrintResult();
    }
}

internal void StartApp()
{
    var builder = WebApplication.CreateBuilder(null);

    // Add services to the container.
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



app.MapGet("/translate", (string text) =>
{
    // without dependency injection
    var translate = new GoogleTranslateFiles(configuration,
        new GoogleTranslate.Common.Impl.File(),
        new ConvertFactory(),
        new GoogleTranslateRequest(configuration));

    translate.Translate();

   

    return forecast;
})
.WithName("GetTranslate");

app.Run();
}