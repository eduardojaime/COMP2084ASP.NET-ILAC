# Instructions

### Demo 1 Designing your Data (diagrams.net)
- Open a browser and navigate to https://app.diagrams.net/
    - Create a new diagram and store it in your device
- Think about:
    - What entities are involved in my program?
    - How do they relate with each other
    - What data attributes about them are relevant in our system?
- Draw an ER diagram accordingly:
    - See the one in this repository under ER-Diagram as a reference
    - Use the toolbox on the left to drag and drop entities and attributes on the drawing area
- Once you are done, validate your design

### Demo 2 Defining the Data Model
- Now it's time to convert our diagrams to code
- Open the solution using Visual Studio Code
- Add the following classes in the Models folder:
    - Category.cs
    - Product.cs
    - Cart.cs
    - OrderItem.cs
    - Order.cs
- Add the corresponding data attributes according to the diagram created in ### Demo 1
- Once you finished adding model classes, you'd need to register them in the application's data context
- Open /data/ApplicationDbContext.cs
    - Add a DbSet<T> attribute for each of the classes in the Models folder

### Demo 3 Configuring your application to connect to a database
- Create a new SQL Database
    - Somee.com > Free.NET Hosting includes a 30MB MSSQL 2016/2019/2022 database
    - Azure > register for free 12 months of Azure SQL Database 250 GB S0 instance with 10 database transaction units
- Retrieve the connection string for the database
- Open appsettings.json and replace the value of ConnectionStrings:DefaultConnection with your own connection string
- Open the Package Manager Console and run the following command
    - Add-Migrations InitialTables
- Open the /data/migrations folder and verify that a new cs file was created
    - Open this file
    - Scroll down and read through the code
    - A migration is a script to create tables in a SQL database
- If everything looks alright go back to the Package Manager Console and run the following command:
    - Update-Database
- Alternatively, connect to your database using MSSQL Management Studio or Azure Data Studio to verify that the corresponding tables were created in the database

### Demo 4 Protecting your connection string by adding them to a secret store
- Open the Package Manager Console
- Navigate to your project folder
    - Dir
    - cd \<folder>
- Run the following commands:
    - dotnet user-secrets init
    - dotnet user-secrets set "ConnectionStrings:DefaultConnection" "yourconnectionstringgoeshere"
- In appsettings.json
    - Replace the value in ConnectionStrings:DefaultConnection with \<secret>
- Run the application and verify it runs properly