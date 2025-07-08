using System.Net.Http.Json;
using System.Runtime.ExceptionServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using CqrsService.Domain.Entities.ExamplePersonModule;
using CqrsService.Domain.Exceptions;
using CqrsService.Infrastructure.Configuration;
using CqrsService.Infrastructure.Options;
using CqrsService.Infrastructure.Provider.External.ReqRes.Models;
using CqrsService.Shared;
using Microsoft.Extensions.Options;

namespace CqrsService.Infrastructure.Provider.External.ReqRes;

public sealed class ReqResProvider : IReqResProvider
{
    private HttpClient _httpClient;
    private readonly ReqResOptions _reqResOptions;
    private readonly JsonSerializerOptions _serializerOptions = new() { PropertyNameCaseInsensitive = true, WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.Never };

    public ReqResProvider(HttpClient httpClient, IOptions<ReqResOptions> options)
    {
        _httpClient = httpClient;
        _reqResOptions = options.Value;
    }

    public async Task<TModel> CreateUserWithGenericRequestResponse<TModel>(TModel model) where TModel : class
    {
        try
        {
            var userEndpoint = $"{_reqResOptions.BaseUrl}{_reqResOptions.UserEndpoint}";
            var requestUri = new Uri(userEndpoint);

            using var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = requestUri,
                Content = JsonContent.Create(inputValue: model, options: _serializerOptions),
            };

            request.Headers.TryAddWithoutValidation("Header1", "Header1Value");

            var httpResponse = await _httpClient.SendAsync(request);

            if (!httpResponse.IsSuccessStatusCode)
            {
                var reponseContent = await httpResponse.Content.ReadAsStringAsync();
                var innerException = new System.Exception($"StatusCode: {httpResponse.StatusCode}, RequestUri: {requestUri}, ResponseContent: {reponseContent}");

                throw new ExternalSystemException(MessageContext.ReqResProviderCreateUserError.GetMessage(), innerException);
            }

            return await httpResponse.Content.ReadFromJsonAsync<TModel>(_serializerOptions);
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(ReqResProvider), methodName: nameof(CreateUserWithGenericRequestResponse));
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }

    public async Task<CreateExamplePersonResponse> CreateUserWithRequestResponseModel(CreateExamplePersonRequest model)
    {
        try
        {
            var userEndpoint = $"{_reqResOptions.BaseUrl}{_reqResOptions.UserEndpoint}";
            var requestUri = new Uri(userEndpoint);

            using var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = requestUri,
                Content = JsonContent.Create(inputValue: model, options: _serializerOptions),
            };

            var httpResponse = await _httpClient.SendAsync(request);

            if (!httpResponse.IsSuccessStatusCode)
            {
                var reponseContent = await httpResponse.Content.ReadAsStringAsync();
                var innerException = new System.Exception($"StatusCode: {httpResponse.StatusCode}, RequestUri: {requestUri}, ResponseContent: {reponseContent}");

                throw new ExternalSystemException(MessageContext.ReqResProviderCreateUserError.GetMessage(), innerException);
            }

            return await httpResponse.Content.ReadFromJsonAsync<CreateExamplePersonResponse>(_serializerOptions);
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(ReqResProvider), methodName: nameof(CreateUserWithRequestResponseModel));
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }

    public async Task<CreateExamplePersonResponse> CreateUserWithGenericRequest<TModel>(TModel model) where TModel : class
    {
        try
        {
            var userEndpoint = $"{_reqResOptions.BaseUrl}{_reqResOptions.UserEndpoint}";
            var requestUri = new Uri(userEndpoint);

            using var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = requestUri,
                Content = JsonContent.Create(inputValue: model, options: _serializerOptions),
            };

            var httpResponse = await _httpClient.SendAsync(request);

            if (!httpResponse.IsSuccessStatusCode)
            {
                var reponseContent = await httpResponse.Content.ReadAsStringAsync();
                var innerException = new System.Exception($"StatusCode: {httpResponse.StatusCode}, RequestUri: {requestUri}, ResponseContent: {reponseContent}");

                throw new ExternalSystemException(MessageContext.ReqResProviderCreateUserError.GetMessage(), innerException);
            }

            return await httpResponse.Content.ReadFromJsonAsync<CreateExamplePersonResponse>(_serializerOptions);
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(ReqResProvider), methodName: nameof(CreateUserWithGenericRequest));
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }

    public async Task<TModel> CreateUserWithGenericResponse<TModel>(ExamplePerson examplePerson) where TModel : class
    {
        try
        {
            var userEndpoint = $"{_reqResOptions.BaseUrl}{_reqResOptions.UserEndpoint}";
            var requestUri = new Uri(userEndpoint);

            var requestContent = new CreateExamplePersonRequest(examplePerson.FirstName, examplePerson.LastName);

            using var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = requestUri,
                Content = JsonContent.Create(inputValue: requestContent, options: _serializerOptions),
            };

            var httpResponse = await _httpClient.SendAsync(request);

            if (!httpResponse.IsSuccessStatusCode)
            {
                var reponseContent = await httpResponse.Content.ReadAsStringAsync();
                var innerException = new System.Exception($"StatusCode: {httpResponse.StatusCode}, RequestUri: {requestUri}, ResponseContent: {reponseContent}");

                throw new ExternalSystemException(MessageContext.ReqResProviderCreateUserError.GetMessage(), innerException);
            }

            return await httpResponse.Content.ReadFromJsonAsync<TModel>(_serializerOptions);
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(ReqResProvider), methodName: nameof(CreateUserWithGenericResponse));
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }
}