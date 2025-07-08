namespace CqrsService.Shared;

public static class StringExtensions
{
    public static string ForceTrailingCharacter(this string valueToCheck, char trailingCharacter)
    {
        if (trailingCharacter == '\0') return valueToCheck;
        if (string.IsNullOrWhiteSpace(valueToCheck)) return trailingCharacter.ToString();

        if (!valueToCheck.EndsWith(trailingCharacter))
        {
            valueToCheck += trailingCharacter;
        }

        return valueToCheck;
    }

    public static string StripLeadingCharacter(this string valueToCheck, char leadingCharacter)
    {
        if (string.IsNullOrWhiteSpace(valueToCheck) || leadingCharacter == '\0') return valueToCheck;

        return valueToCheck.StartsWith(leadingCharacter)
            ? valueToCheck.TrimStart(leadingCharacter)
            : valueToCheck;
    }
}