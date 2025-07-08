Feature: Example Controller
	
Scenario: Create a person (V1)
	When I make a POST request to V1 of the endpoint with the following data
	"""
	{
		"FirstName": "firstName",
		"LastName": "lastName",
		"Age": 60
	}
	"""
	Then the response status code should be success
	And the response should be
	"""
	{
		"FirstName": "firstName",
		"LastName": "lastName",
		"Age": 60
	}
	"""
	And the database should contain the created person

Scenario: Create a person (V2)
	When I make a POST request to V2 of the endpoint with the following data
	"""
	{
		"FirstName": "firstName",
		"LastName": "lastName",
		"Age": 60
	}
	"""
	Then the response status code should be success
	And the response should be
	"""
	{
		"FirstName": "firstName",
		"LastName": "lastName",
		"Age": 60
	}
	"""
	And the database should contain the created person

Scenario: Get a person (V1)
	Given I have the following person in the database
	"""
	{
		"FirstName": "firstName",
		"LastName": "lastName",
		"Age": 60
	}
	"""
	When I make a GET request to V1 of "GetPersonById"	
	Then the response status code should be success
	And the response should be the above person

Scenario: Get all people (V1)
	Given I have 10 people in the database
	When I make a GET request to V1 of "GetAllPersons"	
	Then the response status code should be success
	And the response should be the people in the database

Scenario: Get a person by first and last name (V1)
	Given I have the following person in the database
	"""
	{
		"FirstName": "firstName",
		"LastName": "lastName",
		"Age": 60
	}
	"""
	When I make a GET request to V1 of "GetPersonByFirstNameAndLastName"	
	Then the response status code should be success
	And the response should be the above person

Scenario: Update a person (V1)
	Given I have the following person in the database
	"""
	{
		"FirstName": "firstName",
		"LastName": "lastName",
		"Age": 60
	}
	"""
	When I make a PUT request to V1 of the endpoint with the following data
	"""
	{
		"FirstName": "firstyfirst",
		"LastName": "lastylast",
		"Age": 234
	}
	"""
	Then the response status code should be success
	And the response should be the updated person
	And the database should contain the updated person

Scenario: Delete a person (V1)
	Given I have the following person in the database
	"""
	{
		"FirstName": "firstName",
		"LastName": "lastName",
		"Age": 60
	}
	"""
	When I make a DELETE request to V1 of the endpoint
	Then the response status code should be success
	And the response should be true
	And the database should no longer contain the person