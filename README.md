# GoogleTranslate
Files are translated throught Google Translate

# Example of usting
For example, we need to translate all html files in folder `files` and translate it from `ES` to `EN` and put translated files to new folder `Translated`. Comand line:

`-s es -d en -r ./files -o ./Translated -a .es -m *.html -l true`

#Full help information

-v, --verbose          Set output to verbose messages.
-s, --srcLang          Required. Set language will be translate from
-d, --dstLang          Required. Set language will be translate to
-p, --proxy            Http Proxy for request to google translate, i.e.: 192.168.1.1:3128
-t, --threads          Count of threads for translating files, default = 1, max = 20
-r, --srcPath          Required. Source path where files for translations
-o, --dstPath          Required. Output path for translated files
-a, --additionalExt    Required. Additional ext for translated files, for example: source file.txt, the result file_en.ru.txt, you need use "-a _en.ru"
-m, --maskFiles        Max of file for translating, default *.txt
-l, --html             Is it html files, if html turn on special converting of content before sending to Google.Translate
--help                 Display this help screen.

--version              Display version information.

#Build

If you wand get only one .exe file, run follow command

`dotnet publish src/GoogleTranslate.csproj -c Release -r win-x64 -o ./publish /p:PublishSingleFile=true /p:PublishTrimmed=true --self-contained`

Easy common command look like:

`dotnet publish src/GoogleTranslate.csproj -c Release`