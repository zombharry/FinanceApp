using FluentValidation.Results;

namespace Auth.Api.Exceptions;

public class ValidationFailedException : Exception
{
    public IReadOnlyCollection<ValidationFailure> Errors { get; }

    public ValidationFailedException(IEnumerable<ValidationFailure> errors)
        : base("Validation failed.")
    {
        Errors = errors.ToList();
    }
}
