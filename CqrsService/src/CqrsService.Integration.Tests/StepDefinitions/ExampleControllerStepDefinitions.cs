using System.Net.Http.Json;
using System.Text.Json;
using CqrsService.API;
using CqrsService.Application.Commands;
using CqrsService.Domain.Entities.ExamplePersonModule;
using CqrsService.Infrastructure.Persistence.Entities;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CqrsService.Integration.Tests.StepDefinitions;

[Binding, Scope(Feature = "Example Controller")]
public class ExampleControllerStepDefinitions : CommonStepDefinitions
{
    private const string BASEV1URLFORAPI = "api/v1/ExamplePerson";
    private const string BASEV2URLFORAPI = "api/v2/ExamplePerson";

    private ExamplePerson _person;
    private UpdateExamplePersonCommand _updateCommand;
    List<ExamplePersonDbEntity> _people;

    public ExampleControllerStepDefinitions(WebApplicationFactory<Program> factory, ScenarioContext scenarioContext) :  base(factory, scenarioContext)
    {
            _people = new List<ExamplePersonDbEntity>();
        }

    [BeforeScenario]
    private async Task CleanupDatabase()
    {
            await Persistence.Delete<ExamplePersonDbEntity>(p => true);
        }

    [Given(@"I have the following person in the database")]
    public async Task GivenIHaveTheFollowingRecordInTheDatabase(string inputData)
    {
            var person = JsonSerializer.Deserialize<ExamplePerson>(inputData);
            var dbPerson = new ExamplePersonDbEntity(Guid.NewGuid().ToString(), person.FirstName, person.LastName, person.Age);
            dbPerson = await Persistence.Create(dbPerson);
            _people.Add(dbPerson);
        }

    [Given(@"I have (\d+) people in the database")]
    public async Task GivenIHaveTheFollowingRecordsInTheDatabase(int numberOfPeople)
    {
            for(int i = 0; i < numberOfPeople; i++)
            {
                var person = new ExamplePerson(firstName: "john" + i,
                                                    lastName: "smith" + i,
                                                    age: 123);
                var dbPerson = new ExamplePersonDbEntity(Guid.NewGuid().ToString(), person.FirstName, person.LastName, person.Age);
                dbPerson = await Persistence.Create(dbPerson);
                _people.Add(dbPerson);
            }
        }

    [When(@"I make a GET request to V(\d+) of ""(.*?)""")]
    public async Task WhenIMakeAGetRequestToEndpoint(int endpointVersion, string endpoint)
    {
            var person = _people.First();
            var url = GetVersionedEndpoint(endpointVersion);

            switch (endpoint)
            {
                case "GetPersonById":
                    url += $"/{endpoint}?id={person.Id}";
                    break;
                case "GetAllPersons":
                    url += $"/{endpoint}";
                    break;
                case "GetPersonByFirstNameAndLastName":
                    url += $"/{endpoint}?firstname={person.FirstName}&lastname={person.LastName}";
                    break;
            }
            Response = await Client.GetAsync(url);
        }

    [When(@"I make a POST request to V(\d+) of the endpoint with the following data")]
    public async Task WhenIMakeAPOSTRequestToEndpointWithData(int endpointVersion, string requestData)
    {
            var request = JsonSerializer.Deserialize<CreateExamplePersonCommand>(requestData);
            var endpoint = GetVersionedEndpoint(endpointVersion);
            Response = await Client.PostAsJsonAsync(endpoint, request);
        }

    [When(@"I make a PUT request to V(\d+) of the endpoint with the following data")]
    public async Task WhenIMakeAPUTRequestToEndpointWithData(int endpointVersion, string inputData)
    {
            var person = _people.First();
            var endpoint = GetVersionedEndpoint(endpointVersion);

            _updateCommand = JsonSerializer.Deserialize<UpdateExamplePersonCommand>(inputData);

            var updateRecord = new UpdateExamplePersonCommand(Id: person.Id,
                FirstName: _updateCommand.FirstName,
                LastName: _updateCommand.LastName,
                Age: _updateCommand.Age);

            Response = await Client.PutAsJsonAsync(endpoint, updateRecord);
        }

    [When(@"I make a DELETE request to V(\d+) of the endpoint")]
    public async Task WhenIMakeADELETERequestToEndpoint(int endpointVersion)
    {
            var person = _people.First();
            var endpoint = GetVersionedEndpoint(endpointVersion);
            var url = $"{endpoint}?id={person.Id}";

            Response = await Client.DeleteAsync(url);
        }

    [Then(@"the response should be")]
    public async Task ThenTheResponseDataShouldBeAsync(string expectedResponse)
    {
            _person = await Response.Content.ReadFromJsonAsync<ExamplePerson>();
            var expected = JsonSerializer.Deserialize<ExamplePerson>(expectedResponse);
            Assert.Equal(expected.FirstName, _person.FirstName);
            Assert.Equal(expected.LastName, _person.LastName);
            Assert.Equal(expected.Age, _person.Age);
        }

    [Then(@"the response should be the above person")]
    public async Task ThenTheResponseDataShouldBeTheAbovePersonAsync()
    {
            var expected = _people.First();
            var actual = await Response.Content.ReadFromJsonAsync<ExamplePerson>();
            Assert.Equal(expected.FirstName, actual.FirstName);
            Assert.Equal(expected.LastName, actual.LastName);
            Assert.Equal(expected.Age, actual.Age);
        }

    [Then(@"the response should be the updated person")]
    public async Task ThenTheResponseDataShouldBeTheUpdatedPersonAsync()
    {
            var expected = _people.First();
            var actual = await Response.Content.ReadFromJsonAsync<ExamplePerson>();

            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(_updateCommand.FirstName, actual.FirstName);
            Assert.Equal(_updateCommand.LastName, actual.LastName);
            Assert.Equal(_updateCommand.Age, actual.Age);
        }

    [Then(@"the database should contain the updated person")]
    public async Task ThenTheDatabaseShouldContainTheUpdatedPerson()
    {
            var expected = _people.First();
            var actual = await Persistence.QuerySingle<ExamplePersonDbEntity>(p => p.Id == expected.Id);

            Assert.NotNull(actual);

            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(_updateCommand.FirstName, actual.FirstName);
            Assert.Equal(_updateCommand.LastName, actual.LastName);
            Assert.Equal(_updateCommand.Age, actual.Age);
        }

    [Then(@"the database should no longer contain the person")]
    public async Task ThenTheDatabaseShouldNoLongerContainThePerson()
    {
            var person = _people.First();
            var dbPerson = await Persistence.QuerySingle<ExamplePersonDbEntity>(p => p.Id == person.Id);

            Assert.Null(dbPerson);
        }

    [Then(@"the response should be true")]
    public async Task ThenTheResponseShouldBeTrue()
    {
            var actual = await Response.Content.ReadFromJsonAsync<bool>();

            Assert.True(actual);
        }

    [Then(@"the response should be the people in the database")]
    public async Task ThenTheResponseDataShouldBeThePeopleInDBAsync()
    {
            var actualPeople = await Response.Content.ReadFromJsonAsync<List<ExamplePerson>>();

            foreach(var expected in _people)
            {
                var actual = actualPeople.FirstOrDefault(p => p.Id == expected.Id);

                Assert.NotNull(actual);

                Assert.Equal(expected.FirstName, actual.FirstName);
                Assert.Equal(expected.LastName, actual.LastName);
                Assert.Equal(expected.Age, actual.Age);
            }
        }

    [Then(@"the database should contain the created person")]
    public async Task ThenTheDatabaseShouldContainTheCreatedPerson()
    {
            var dbResult = await Persistence.QuerySingle<ExamplePersonDbEntity>(p => p.Id == _person.Id);

            Assert.NotNull(dbResult);

            Assert.Equal(_person.Id, dbResult.Id);
            Assert.Equal(_person.FirstName, dbResult.FirstName);
            Assert.Equal(_person.LastName, dbResult.LastName);
            Assert.Equal(_person.Age, dbResult.Age);
        }

    private string GetVersionedEndpoint(int endpointVersion)
    {
            var endpoint = string.Empty;

            switch (endpointVersion)
            {
                case 1:
                    endpoint = BASEV1URLFORAPI;
                    break;
                case 2:
                    endpoint = BASEV2URLFORAPI;
                    break;
                default:
                    throw new ArgumentException("Unsupported API endpoint version!");
            }

            return endpoint;
        }
}