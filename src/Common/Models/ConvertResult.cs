namespace GoogleTranslate.Common.Models;

/// <summary>
/// Result of converting text
/// </summary>
public class ConvertResult
{
    /// <summary>
    /// Split whole content on chunks for sending to google translate
    /// </summary>
    public string Content { get; set; }
    
    /// <summary>
    /// Converts info, that replace 
    /// </summary>
    public Dictionary<int,string> Tags { get; set; }
    
    /// <summary>
    /// Converts info, that replace 
    /// </summary>
    public Dictionary<int,string> Groups { get; set; }

}