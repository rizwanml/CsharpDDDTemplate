namespace CqrsService.Domain.Configuration.Framework;

/// <summary>
/// The domain value object base implementation providing self validating functionality
/// </summary>
public abstract record DomainValueObject
{
    private List<ValidationError> ValidationErrors { get; set; }

    protected DomainValueObject()
    {
        ValidationErrors = new List<ValidationError>();
    }

    protected virtual bool HasBeenValidated { get; set; }

    public virtual bool IsValid()
    {
        if (ValidationErrors == null)
        {
            ValidationErrors = new List<ValidationError>();
        }

        HasBeenValidated = true;
        return !ValidationErrors.Any();
    }

    public void AddValidationError(string propertyName, string message)
    {
        if (HasBeenValidated)
        {
            HasBeenValidated = false;
            ValidationErrors = new List<ValidationError>();
        }

        if (ValidationErrors == null)
        {
            ValidationErrors = new List<ValidationError>();
        }

        ValidationErrors.Add(new ValidationError(propertyName, message));
    }

    public List<ValidationError> GetValidationErrors()
    {
        return ValidationErrors;
    }
}