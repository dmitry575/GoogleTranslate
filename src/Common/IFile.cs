namespace GoogleTranslate.Common;

/// <summary>
/// Common work with files
/// </summary>
public interface IFile
{
    /// <summary>
    /// Get all files name from directory
    /// </summary>
    /// <param name="srcPath">Source of path</param>
    /// <param name="mask">Mask for file</param>
    /// <returns></returns>
    List<string> GetFiles(string srcPath, string mask="*.txt");

    /// <summary>
    /// Save file to destination file
    /// </summary>
    /// <param name="srcFileName">Source file name from this name get file name for saving</param>
    /// <param name="dstPath">Path for saving files</param>
    /// <param name="additionalExt">Additional for name file</param>
    /// <param name="content">Content for saving</param>
    void SaveFiles(string srcFileName, string dstPath, string additionalExt, string content);
    
    /// <summary>
    /// Read data from file
    /// </summary>
    /// <param name="file">File name</param>
    string GetContent(string file);
}