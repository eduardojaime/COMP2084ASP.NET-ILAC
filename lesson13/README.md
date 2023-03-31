# Instructions

### Demo 1 Testing default API controller
- Right click on the Controllers folder
    - Add Controller
    - Select API Controller - With Read/Write actions
    - Leave the default name as click add
    - Examine the following:
        - Class name has a decorator to specify the route
        - Methods have a decorator indicating the supported verb
    - Run application
- Open POSTMAN or Insomnia
    - Create a new request to test
        - GET to /api/values
    - Examine the following
        - Response Body
        - Status Codes
    - Create a new request
        - GET to /api/values/id
        - Verify in API Client
    - Create a new request
        - POST to /api/values
        - Add body as JSON
            - Provide value
        - Verify in API Client
    - Create a new request
        - DELETE to /api/values

### Demo 2 Scaffolding an API controller for Products
- Open your solution and in the /Controllers folder
    - Add Controller
    - Select API Controller with Entity Framework
    - Choose Product as our model
    - Name it ProductsApiController
- Examine the methods created by default
    - GET (all and single product)
    - POST
    - DELETE
    - PUT
- Open Insomnia or Postman and create request to tests them
- Verify in DB

### Demo 3 Using Swagger to automatically document our API
- Open a browser and navigate to https://learn.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger
- Install the following Nuget packages in our main proejct
    - Swashbuckle.AspNetCore
    - Accept additional packages to be added
- In Program.cs
    - Call  builder.Services.AddSwaggerGen() before building the app object
    - Call app.UseSwagger()
    - Call app.UseSwaggerUI() and set the endpoint for the swagger documentation
- Run your application
    - Navigate to /swagger to verify
- Run a few tests on Swagger