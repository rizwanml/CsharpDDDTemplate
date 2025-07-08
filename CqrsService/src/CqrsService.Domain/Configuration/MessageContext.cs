using System.ComponentModel;
using System.Reflection;

namespace CqrsService.Domain.Configuration;

/// <summary>
/// Holds string messages for the domain layer
/// </summary>
internal enum MessageContext
{
    [Description("A system error occurred while processing the request.")]
    SystemError,

    [Description("A validation error occurred while processing the request.")]
    ValidationError,

    [Description("An external system error occurred while processing the request.")]
    ExternalSystemError,

    [Description("This is an example of an error message to tell the caller what went wrong.")]
    ErrorExample
}

internal static class MessageContextDescription
{
    internal static string GetMessage(this MessageContext messageContext)
    {
        FieldInfo field = messageContext.GetType().GetField(messageContext.ToString());

        DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

        return attribute == null ? messageContext.ToString() : attribute.Description;
    }
}