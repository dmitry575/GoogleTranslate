namespace GoogleTranslate.Common.Impl;

public class File: IFile
{
    public List<string> GetFiles(string srcPath, string mask = "*.txt")
    {
        return Directory.GetFiles(srcPath, mask).ToList();
    }

    public void SaveFiles(string srcFileName, string dstPath, string additionalExt, string content)
    {
        var fInfo = new FileInfo(srcFileName);
        var fileResultName =  fInfo.Name.Substring(0, fInfo.Name.Length - fInfo.Extension.Length)  + additionalExt  + fInfo.Extension;

        System.IO.File.WriteAllText(fileResultName, content)
    }

    public string? GetContent(string file)
    {
        if (!System.IO.File.Exists(file))
        {
            return null;
        }

        return System.IO.File.ReadAllText(file);
    }
}