using CommandLine;

namespace GoogleTranslate.Errors;

public class ErrorHandle
{
    private bool _hasError = false;

    public void HandleParseError(IEnumerable<Error> errors)
    {
        foreach (var error in errors)
        {
            _hasError = true;
        }
    }

    public bool HasError => _hasError;
}