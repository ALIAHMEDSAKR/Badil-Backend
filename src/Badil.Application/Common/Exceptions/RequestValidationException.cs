using FluentValidation.Results;

namespace Badil.Application.Common.Exceptions;

public class RequestValidationException  : Exception {
    public Dictionary<string , string[]> Errors { get; }

     public RequestValidationException(IEnumerable<ValidationFailure> failures)
        : base("One or more validation failures occurred.") {
            Errors = failures
                .GroupBy(f => f.PropertyName , f => f.ErrorMessage )
                .ToDictionary(g => g.Key , g => g.ToArray());
        }
}