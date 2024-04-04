# Instructions

### Demo 1 Creating a new Unit Test project
- Open your solution
- Right click on solution name > Add new project
    - Select MSTest Test Project (.NET Core) in C# 
- Name it DotNetGrillTests
- Notice class that gets created and the decorators on top of the class and method declarations

### Demo 2 Adding Tests
- Since we will test classes in our other project, we need to import the corresponding namespace
    - Right click on project name > Add > Project Reference > Select DotNetGrill
    - using DotNetGrill.Controllers
- Remove UnitTest1.cs from the project
- Right click on project name > Add > New Item > Class
    - Make class public
    - Name it HomeControllerTest
    - Add [TestClass] decorator above the class definition
- Add a new method called IndexReturnsResult()
    - Add [TestMethod] decorator above the method definition
    - Every test method requires three steps:
        - Arrange data
        - Do something
        - Assert success or fail result
    - Create a new instance of the HomeController class
        - Pass null as logger parameter for now
    - Call Index method
    - Assert that result is not null
    - Run and examine
        - Click on Test > Run All Tests
        - Checkmarks are place on top of related methods in the controller
        - Under test menu at the top, click on Analyze Code Coverage 
- Add another test method for the Privacy action method
    - Create an instance of homecontroller
    - Call Privacy() and cast result to ViewResult object
    - Assert that view name equals 'Privacy'
- Run all tests again
    - It will fail since view name would be null
- Open /Controllers/HomeController.cs
    - Scroll down to the Privacy() method
    - Modify the return view() statement and explicitly pass the view name 'Privacy'
- Run tests again, and they should all pass now
        
### Demo 3 Setting up our ProductController test: InitializeTest method
- Right click on project name > Add > New Item > Class
    - Make class public
    - Name it ProductsControllerTest
    - Add [TestClass] decorator above the class definition
- Here we need to consider something:
    - ProductsController connects to the database to perform operations
    - When an instance of ProductController is created, an instance of dbcontext is passed to the constructor method via dependency injection
    - If we connect directly to the database we are no longer writing tests
    - The approach here is to use a mock object to simulate our db connection when testing
- Install Microsoft.EntityFrameworkCore.InMemory in our test project from the Nuget Package Manager window
- Set up the test class:
    - Declare the following variables
        - DbContext
        - A mock list of products
        - A controller variable to represent the controller we will test
    - Add a TestInitialize method that runs automatically before each unit test to streamline the setup. 
        - Use [TestInitialize] decorator.
        - Instantiate an in-memory db, similar to initialization in Startup.ca
            □ Instead of useSql, set options to useInMemoryDatabase
        - Create mock products, brands and categories
        - Add them to the corresponding context dbsets and save changes
        - Create an instance of the controller and pass it the dbcontext

### Demo 4 Creating Test Methods for the ProductsController class
- Create a new test method named IndexViewLoad()
    - Call Index() action method
    - Cast result as ViewResult object
    - Assert view name is Index
    - Run tests once > FAIL 
        - How can we fix this?
    - In ProductsController.Index() explicitly declare name of view to render
    - Run again > PASS
- Create new test method named IndexReturnsProductData()
    - Call index action method
    - Cast result as ViewResult object
    - Extract list of products from the view model
    - Compare mock list and model
    - Run once > FAIL
    - When running tests we need to make sure objects match exactly > Products are sorted differently
        - Place breakpoint and debug test to see why it fails
    - Make sure OrderBy is the same in the controller and test
    - Run again > PASS
- Think about two more test methods to add, and try it out