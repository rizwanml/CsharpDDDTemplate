using System.Net;
using CqrsService.Infrastructure.Persistence.InMemory;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace CqrsService.Integration.Tests.StepDefinitions;

[Binding]
public class CommonStepDefinitions : Steps
{
    private const string BaseAddress = "http://localhost/";
    private ScenarioContext _scenarioContext;

    public HttpClient Client
    {
        get { return GetFromScenarioContextOrReturnNull<HttpClient>(nameof(Client)); }
        set { _scenarioContext[nameof(Client)] = value; }
    }

    public HttpResponseMessage? Response
    {
        get { return GetFromScenarioContextOrReturnNull<HttpResponseMessage>(nameof(Response)); }
        set { _scenarioContext[nameof(Response)] = value; }
    }

    public WebApplicationFactory<Program> Factory
    {
        get { return GetFromScenarioContextOrReturnNull<WebApplicationFactory<Program>>(nameof(Factory)); }
        private set { _scenarioContext[nameof(Factory)] = value; }
    }

    public IInMemoryPersistence Persistence
    {
        get { return GetFromScenarioContextOrReturnNull<IInMemoryPersistence>(nameof(Persistence)); }
        set { _scenarioContext[nameof(Persistence)] = value; }
    }

    private T? GetFromScenarioContextOrReturnNull<T>(string key)
    {
        try
        {
            return (T)_scenarioContext[key];
        }
        catch (KeyNotFoundException)
        {
            return default;
        }
    }

    public CommonStepDefinitions(WebApplicationFactory<Program> factory, ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
        Factory = Factory ?? factory;
        Persistence = Factory.Services.GetService<IInMemoryPersistence>();
        Client = Factory.CreateDefaultClient(new Uri(BaseAddress));
    }

    #pragma warning disable CA2000
    public WebApplicationFactory<Program> SetupServices(params Mock[] interfacesToMock)
    {
        if (interfacesToMock == null)
        {
            throw new ArgumentNullException(nameof(interfacesToMock));
        }

        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    foreach (var interfaceToMock in interfacesToMock)
                    {
                        var serviceType = interfaceToMock.GetType().GetGenericArguments().First();
                        services.AddSingleton(serviceType, interfaceToMock.Object);
                    }
                });
            });
    }

    [BeforeTestRun(Order = 1)]
    private async static Task SetLocalEnvironment()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Local");
    }

    [Then(@"the response status code should be success")]
    public void ThenTheResponseStatusCodeIsSuccess()
    {
        Assert.Equal(HttpStatusCode.OK, Response.StatusCode);
    }
}