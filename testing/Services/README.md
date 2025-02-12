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


### Unit test WebApi Endpoints
Following [Endpoint Attributes conventions](#endpoint-attributes-conventions), we can test the endpoint by creating a unit test class for the `Controller` class in a generic way, without the hustle and overhead of creating per-endpoint unit tests.
> **Important:** The goal of the endpoint unit tests is to validate the endpoint contract, not the business logic. The business logic should be tested in the business logic layer.
2 layers test the endpoint:
1. Static code analysis - Validates that all mandatory attributes are present and correctly configured
2. Unit tests - Validates that the endpoint is correctly configured and returns the expected result.

See the attached `asp.net webapi` project for examples of endpoint unit tests.

## `Controller` logical structure
As specified previously on this document, every endpoint out `WebApi` project exposes to the outside world, is pointed to a specific `Controller` method.
These method must follow one, and only one the following structure:
* An endpoint for reading data
* An endpoint to mutate domain entity
	* Create
	* Update
	* Delete

Regardsless of the functionlity the endpoint provides, it may return data. 
In this case, the returned data MUST strongly related to the functionlity AND the context the endpoint provides:
	
* ❌ An endpoint that updates user profile and return user orders
* ❌ An endpoint that gets user's order history returns product list
* ✅ An endpoint that adds item to cart return all the item in cart
* ✅ An endpoint that gets user's order history return paginated user order history


### The `Controller` conversation with the external world
`Controller` is the object created to implement a strict contract with the HTTP "world" outside our application. 
Any deviation from the contract causes expensive handling efforts. Therefore we embrace a zero tolerance approach towards it.

Deviations may be handled by the following ways:
* Generic error handling (such as 400 and 404 status codes)
* Explicit handling (such as 401 and 403 status code)

#### API Model, Domain Model and Database Model
At its core, the `WebApi` is mechanism to expose data efficiently to multiple clients using `HTTP` protocol.
The psaudo steps to perform this task is:
1. Parse the imcoming `Http Request` and extract the data required for the business logic
2. Trigger the suitable business logic function
3. Read/mutate using the persistency layer (database)
4. Reply with a ackknowledge response (with or without payload) to the requestee

This describes 3 [bounded contexts](https://martinfowler.com/bliki/BoundedContext.html):
- User/client context - Which managed by the user/client requirements
- Application context - Which managed by the business logic requirements (domain)
- Database context - Which managed by the persistency requirements and optimization needs

To have clear separation of concerns between these contexts, please follow these guidlines:
1. As all requests are stateless, contextual data is passed using http `headers` only (userid, token, etc)
2. Per request Data is passed to an endpoint using `query` parameters or `body` only.
3. Runtime/codebase-wise, when data is passed using the `query` parameters or request's `body` it is mapped FROM *`Model`* type to *`Domain`* type.

*`Model`* type represents user/client context object 
*`Domain`* type represents application context [`DTO`](https://en.wikipedia.org/wiki/Data_transfer_object).
In other words models types are used for the user/client to pass and recieve data to/from the endpoint, while domain types are used to perform query and mutations by the business logic
> Important: Event if in most cases the model and its matching domain object share the same properties (name and/or type) they should not be merged to same object as the consumption context effects the way these objects are categorised.

The last context is the database context. In this context we have 2 options:
* Using the same domain types as the business logic
	* This is used when using generic `ORM` and/or mapper (`ADO.NET`) which provide optimization over configuration to communicate with the database
* Using dedicated database models. 
	* This is done when we would have custom implementation of database communication

> Note: It is rarly required to use the second options. If you want to use custom database mapping implementation - it must be approved by the infrastructure team.


3. domain object the The input data is totally decoupled from the 
Simply put most of the time, the `Controller` methods receive a model object as input and return a model object as output.


To ease the effort of handling the contract, the `Controller` methods should perform the following tasks only:
* Validate the input data
* Validate user authorization
* Validate user permissions
* Call domain business logic
* Map incoming model to matching domain object
* Map outgoing domain object to matching model


The `Controller` must not handle any business logic. It should only be responsible for: