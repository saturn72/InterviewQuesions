# Web API

The Web API's role is to interact with the external world over HTTP protocol. 
It serves as the main gateway to our application and adheres to strict conventions for receiving requests and return responses.
These conventions utilize the HTTP request components (headers, body, query, etc.) and targeted URL to define the expected input and output.

## HTTP Request Components
Here are the basic standards we use for HTTP requests to Web APIs:

| Requirement | HTTP Verb to Use | Example |
|-------------|------------------|---------|
| Fetch data from the web API without performing any mutation | HTTP GET | Get user cart orders from an e-commerce site: [GET] /api/orders/ |
| Fetch data from the web API without performing any mutation and requires specific data fields | HTTP GET | Get user cart orders using pagination: [GET] /api/orders?pagesize=25&offset=10 |
| Mutation - **Create** domain entity | HTTP POST | New entity data is stored in the HTTP request's body: [POST] /api/order [body]={items:[{id:1, quantity:1},{id:2, quantity:1},{id:3, quantity:3}]} |
| Mutation - **Update** domain entity | HTTP PUT | Updated data is stored in the HTTP request's body: [PUT] /api/order/{order-id} [body]={items:[{id:2, quantity:1},{id:3, quantity:3}]}. The specific update logic is implemented in the business logic |
| Mutation - **Delete** domain entity | HTTP DELETE | Entity ID is mentioned explicitly in the URI: [DELETE] /api/order/{order-id} |


## Endpoint declaration 
`asp.net core` has several ways to declare an endpoint. The most common way is to use `Controller` class.
The `Controller` class method is the easiest way to test the endpoint, therfore should be used on most cases.
> Using other methods to declare endpoint is an exception, and must be approved explicitly by infrastructure team.

As an endpoint declaration, the `Controller` class is decorated with attributes that are used to define the route, HTTP verb, and other metadata.
The combination of these attributes defines the contract the `WebApi` commited. 
Testing the endpoint "commitment" is required to validate we did not break the contract with the `WebApi` consumers.
So these attributes are the fundemental parts of the `Controller` class unit testing.

### Endpoint Attributes conventions
* `[Route]` attribute is used to define the base route for the controller. 
	* It is recommended to use the `/api` prefix for all routes.]
	* `Template` should be explicit and in lowercase.

* `[Authorize]` attribute is used to define the controller/method as one that requires authentication
	* All controllers must be decorated with `Authorize` attribute by default

* `[AllowAnonymous]` attribute is used to define the controller/method as one that do not requires authentication
	* The controller methods that do not require authentication must be configured **per-method** 
	* Every method that is decorated with `[AllowAnonymous]` must be tested for it existance

* `[HttpMethod]` attribute is used to define the HTTP verb for the method
	* We use its derivatives (`[HttpGet]`, `[HttpPost]`, `[HttpPut]`, `[HttpDelete]`) to define the HTTP verb for the method as  mentioned in the [HTTP Request Components](#http-request-components)
	* The method name should be in the form of `{HttpVerb}{ActionName}`. For example, `GetOrdersAsync`, `CreateOrderAsync`, etc.
	* The method should be `async` and return `Task<IActionResult>`. 
	* The method should have a `FromQuery`, `FromBody` attribute to define the input parameters.
	* Each method must be decorated with **single** `[HttpMethod]` attribute only.
	* Route data is specified in the `[HttpMethod]` attribute. For example, `[HttpGet("{orderId}")]` will match the route `/api/order/{orderId}`

Here is an example of a simple `Controller` class:
```csharp
[Route("/api/order")]
[Authorize]
public class OrderController: ControllerBase
{
	[HttpGet]
	public Task<IActionResult> GetOrdersAsync([FromQuery]int? offset = 0, [FromQuery]int? pageSize = 15)
	{
		// do something async...
	}

	[HttpGet("{orderId}")]
	public Task<IActionResult> GetOrderByIdAsync(int orderId)
	{
		// do something async...
	}
	
	[HttpPost]
	public Task<IActionResult> CreateOrderByIdAsync([FromBody] OrderModel model)
	{
		// do something async...
	}
}

[Route("/api/store")]
[Authorize]
public class StoreController: ControllerBase
{
	[HttpGet]
	public Task<IActionResult> GetStoreInfoAsync()
	{
		// do something async...
	}

	[HttpGet("open-hours")]
	[AllowAnonymous]
	public Task<IActionResult> OpenHoursAsybc()
	{
		// do something async...
	}
}
```


## Unit test endpoints
Following [Endpoint Attributes conventions](#endpoint-attributes-conventions), we can test the endpoint by creating a unit test class for the `Controller` class in a generic way, without the hustle and overhead of creating per-endpoint unit tests.
> **Important:** The goal of the endpoint unit tests is to validate the endpoint contract, not the business logic. The business logic should be tested in the business logic layer.
2 layers test the endpoint:
1. Static code analysis - Validates that all mandatory attributes are present and correctly configured
2. Unit tests - Validates that the endpoint is correctly configured and returns the expected result.

See the attached `asp.net webapi` project for examples of endpoint unit tests.