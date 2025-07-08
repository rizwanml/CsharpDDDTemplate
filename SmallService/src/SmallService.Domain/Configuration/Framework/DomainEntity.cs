namespace SmallService.Domain.Configuration.Framework;

/// <summary>
/// The domain entity base implementation providing self validating functionality
/// </summary>
public abstract class DomainEntity
{
    public string Id { get; init; }
    private List<ValidationError> ValidationErrors { get; set; }

    protected DomainEntity(string? id)
    {
        Id = id ?? Guid.NewGuid().ToString();
        ValidationErrors = new List<ValidationError>();
    }

    protected virtual bool HasBeenValidated { get; set; }

    public virtual bool IsValid()
    {
        if (ValidationErrors == null)
        {
            ValidationErrors = new List<ValidationError>();
        }

        if (string.IsNullOrWhiteSpace(Id))
        {
            AddValidationError(nameof(Id), "Id is invalid");
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