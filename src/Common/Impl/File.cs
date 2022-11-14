using System.Text;

namespace GoogleTranslate.Common.Impl;

public class File : IFile
{
    /// <inheritdoc />
    public List<string> GetFiles(string srcPath, string mask = "*.txt")
    {
        var isDir = (System.IO.File.GetAttributes(srcPath) & FileAttributes.Directory) == FileAttributes.Directory;
        
        // is file name
        if (!isDir)
        {
            return new List<string> { srcPath };
        }

        if (Directory.Exists(srcPath))
        {
            return Directory.GetFiles(srcPath, mask).ToList();
        }
        return new List<string>();
    }

    /// <inheritdoc />
    public void SaveFiles(string srcFileName, string dstPath, string additionalExt, string content)
    {
        var fInfo = new FileInfo(srcFileName);
        var sb = new StringBuilder();
        var fileResultName =
            sb.Append(fInfo.Name.Substring(0, fInfo.Name.Length - fInfo.Extension.Length))
                .Append(additionalExt)
                .Append(fInfo.Extension)
                .ToString();

        if (!Directory.Exists(dstPath))
        {
            Directory.CreateDirectory(dstPath);
        }

        System.IO.File.WriteAllText(Path.Combine(dstPath, fileResultName), content);
    }

    /// <inheritdoc />
    public string GetContent(string file)
    {
        return !System.IO.File.Exists(file) ? null : System.IO.File.ReadAllText(file);
    }
}