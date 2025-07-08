using System.ComponentModel;
using System.Reflection;

namespace CqrsService.Infrastructure.Configuration;

/// <summary>
/// Holds string messages for the infrastructure layer
/// </summary>
internal enum MessageContext
{
    [Description("A server error occurred while processing the request.")]
    ServerError,

    [Description("Error encountered while attempting to make a request to an external provider.")]
    ExternalProviderError,

    [Description("Error response received when sending the request to Req Res provider.")]
    ReqResProviderCreateUserError,
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