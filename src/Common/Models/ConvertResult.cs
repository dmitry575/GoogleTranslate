namespace GoogleTranslate.Common.Models;

public class ConvertResult
{
    /// <summary>
    /// Split whole content on chunks for sending to google translate
    /// </summary>
    public List<string> Chunks { get; set; }
    
    /// <summary>
    /// Converts info, that replace 
    /// </summary>
    public Dictionary<int,string> Converts { get; set; }
}