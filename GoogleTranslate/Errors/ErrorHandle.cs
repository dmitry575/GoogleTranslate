using CommandLine;
using log4net;

namespace GoogleTranslate.Errors;

public class ErrorHandle
{
    private static readonly ILog _logger = LogManager.GetLogger(typeof(ErrorHandle)); 
    
    private bool _hasError = false;

    public void HandleParseError(IEnumerable<Error> errors)
    {
        foreach (var error in errors)
        {
            _hasError = true;
            _logger.Error(error);
        }
    }

    public bool HasError => _hasError;
}