# Sample Web API application

This is a backend application example (or template) that can show how to build a fully-functional Web API with users, roles, authentication, integration with another API, and a lot of tests.

Using this application, users can create and an account, log in, and store some food records to track sugar consumption. If the user does not provide the sugar amount, the app will fetch it from a nutrition data provider. Also, users can access API to check how much sugar they ate during the current day.

## The Project's structure
Backend is powered by ASP.NET Core and written in C#. The Backend consists of three projects:

1. SugarCounter.Api - entry point, contains controllers, manages access rights
2. SugarCounter.DataAccess - repositories to provide data from the database
3. SugarCounter.Core - common interfaces and models

Also, there are three test projects:

   1. Unit tests - direct method calls with all dependencies mocked
   2. Integration tests - direct method calls with in-memory database
   3. Functional test - calls via HTTP with a real database

At the moment, there are more than 160 tests in total.

## How to debug (on Windows)
1. Install Visual Studio 2019 16.4 or later (Community Edition is enough) with the ASP.NET Core and web development workload.
    As an alternative, Visual Studio Code could be used with C# for Visual Studio Code (latest version)
1. Install .NET Core 3.1 SDK
1. Install MS SQL Server 2019 (Express Edition is enough)
1. Install SQL Server Management Studio 18
1. Connect to the SQL server and execute script `DatabaseInitialization.sql` to create a dedicated user
1. Open file `SugarCounter.sln`
1. Now you can browse the code and build the solution
1. To run the tests:
    1. open the "Test Explorer" (Menu: Test -> Test Explorer)
    1. Press the "Run All Tests" button

## How to build and test (on Windows or supported Linux distributives)
1. Install .NET Core 3.1 SDK
2. Install MS SQL Server 2019 (Express Edition is enough)

    2.1 if installed local, create a user with `DatabaseInitialization.sql` script

    2.2 if not local, set proper connection strings in `SugarCounter.Api\appsettings.json` and in `Tests\Functional\functionalTesting.json`
3. From the solution dir execute `dotnet build --configuration Release && dotnet test --configuration Release` or equivalent for your shell

## How to deploy (on Windows or supported Linux distributives)
1. Install all prerequisites, mentioned in the previous section, make changes in `SugarCounter.Api\appsettings.json` if needed
2. The program can work as a standalone user application, to run

   2.1. from code: inside the solution's folder execute command `dotnet run --project SugarCounter.Api --configuration Release` 

   2.2. from build result: call the executable file located at `SugarCounter.Api\bin\Release\netcoreapp3.1\SugarCounter.Api.exe`

### Deploy considerations
- The app should operate behind reverse-proxy
- A standalone application requires logged in user to run, so for production usage, it's better to run the app as a service

## Troubleshooting
1. If all functional tests failed or app failed to start properly, check server address in connection strings. If you use another SQL server it could be different, i.e. `server=(local)`
2. If it didn't help, manually create DB login with credentials and rights to create and delete databases as mentioned in `DatabaseInitialization.sql` script
