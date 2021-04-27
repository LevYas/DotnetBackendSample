# Sample Web API application

[![Build status](https://ci.appveyor.com/api/projects/status/5ib0tcy7o0pyhpft?svg=true)](https://ci.appveyor.com/project/LevYas/dotnetbackendsample)

This is a backend application example (or template) that can show how to build a fully-functional Web API with users, roles, authentication, integration with another API, and a lot of tests.

Using this application, users can create and an account, log in, and store some food records to track sugar consumption. If the user does not provide the sugar amount, the app will fetch it from a nutrition data provider. Also, users can access API to check how much sugar they ate during the current day.

## The Project's structure
Backend is powered by ASP.NET 5 and written in C# with Nullable Reference Types feature. The Backend consists of three projects:

1. SugarCounter.Api - entry point, contains controllers, manages access rights
2. SugarCounter.DataAccess - repositories to provide data from the database
3. SugarCounter.Core - common interfaces and models

Also, there are three test projects:

   1. Unit tests - direct method calls with all dependencies mocked
   2. Integration tests - direct method calls with in-memory database
   3. Functional tests - calls via HTTP with a real database

At the moment, there are more than 160 tests in total.

### Technologies and approaches
#### SugarCounter.Api
- Used `RequestContext` class to save information about the current request, for example, the user who performs the request
- Implemented custom `AuthenticationHandler` to handle token-based authentication and fill the `RequestContext`
- Implemented custom `AuthorizationFilter` to handle authorization using attribute `AuthorizeFor` of controller methods, i.e. `[AuthorizeFor(UserRole.Supervisor, UserRole.Admin)]`
- Used `IHttpClientFactory` to manage the pooling and lifetime of underlying `HttpClientMessageHandler` instances of used `HttpClient`
- Actively used Data Annotations for data transfer objects' validation

Future plans
- Use [Refit](https://github.com/reactiveui/refit) to consume third-party REST APIs as live interfaces
- Try to use the Command pattern to see how it helps to separate routing logic from handling logic

#### SugarCounter.DataAccess
I considered three approaches to store database entity classes with the column type, restrictions, and index information:
1. Store them together in a shared place to provide easy access to the controller
    - \+ Easy to implement
    - \- Exposing EF Core implementation details outside of DataAccess layer
1. Store interfaces of entities in a shared place, store implementation with column information in the DataAccess assembly
    - \+ All the implementation details are in the DataAccess
    - \- Duplicated code in interfaces and implementations
    - \- A lot more type conversions, because generics are not working well with interfaces
1. Store classes, which contain properties or general logic in a shared place and configure them in DataAccess using fluent configuration
    - \- There are extra configuration classes in DataAccess
    - \+ All the implementation details are in the DataAccess
    - \+ There is no code duplication
    - \+ Works smoothly with generics

For now, I ended up with the third approach because it provides the separation of concerns I want with minimum efforts.

Future plans
- Unify repository interfaces and classes using the approach described [here](https://www.programmingwithwolfgang.com/repository-and-unit-of-work-pattern/)

#### SugarCounter.Core
Apart from the common interfaces and models, the assembly contains class `Res`, located in `Shared\Result.cs`. This is a class, used to encapsulate the evaluation result of a function, when the function can return either desired result or error information. This result then could be matched to different execution flows - success flow, which receives a valid result or error flow, which receives the information about the error:

    Res<UserInfo, CreateUserError> result = ...;

    return Match(result, onOk: u => new UserInfoDto(u),
        (CreateUserError.UserAlreadyExists, () => Conflict("User with this name is already created")),
        (CreateUserError.Unknown, () => Problem()));

One could convert the result using `Map`, and if the error and the final data types are the same, one can simply get the result:

    async Task<Res<UserInfo, ActionResult>> getUserOrError(int userId);

    public async Task<ActionResult> UpdateUser(int userId, UserEditsDto edits)
    {
        return await getUserOrError(userId)
            .ThenMap(userModel => tryUpdateUser(userModel, edits))
            .ThenGet();
    }

This class is inspired by concepts of Monad and Sum types from Functional programming.

#### Tests\Functional
- Used `WebApplicationFactory` to provide separate testing configuration for the server-under-tests, and to automatically create configured `HttpClient`
- Used WireMock.NET to mock responses from third-party APIs

## How to debug (on Windows)
1. Install Visual Studio 2019 16.8.3 or later (Community Edition is enough) with the ASP.NET Core and web development workload.
    As an alternative, Visual Studio Code could be used with C# for Visual Studio Code (latest version)
1. Install .NET 5 SDK
1. Install MS SQL Server 2019 (Express Edition is enough)
1. Execute script `Deploy\initDatabaseUser.cmd <your SQL Server instance name>` to create a dedicated user. This script relies on Windows Authentication in SQL Server
1. Open file `SugarCounter.sln`
1. Now you can browse the code and build the solution
1. To run the tests:
    1. open the "Test Explorer" (Menu: Test -> Test Explorer)
    1. Press the "Run All Tests" button

## How to build and test (on Windows or supported Linux distributives)
1. Install .NET 5 SDK
2. Install MS SQL Server 2019 (Express Edition is enough)

    2.1 if installed local, create a user with `Deploy\initDatabaseUser.sql` script

    2.2 if not local, set proper connection strings in `SugarCounter.Api\appsettings.json` and in `Tests\Functional\functionalTesting.json`
3. From the solution dir execute `dotnet build --configuration Release && dotnet test --configuration Release` or equivalent for your shell

## How to deploy (on Windows or supported Linux distributives)
1. Install all prerequisites, mentioned in the previous section, make changes in `SugarCounter.Api\appsettings.json` if needed
2. The program can work as a standalone user application, to run

   2.1. from code: inside the solution's folder execute command `dotnet run --project SugarCounter.Api --configuration Release` 

   2.2. from build result: call the executable file located at `SugarCounter.Api\bin\Release\net5.0\SugarCounter.Api.exe`

### Deploy considerations
- The app should operate behind reverse-proxy
- A standalone application requires logged in user to run, so for production usage, it's better to run the app as a service

## Troubleshooting
1. If all functional tests failed or app failed to start properly, check server address in connection strings. If you use another SQL server it could be different, i.e. `server=(local)`
2. If it didn't help, manually create DB login with credentials and rights to create and delete databases as mentioned in `Deploy\initDatabaseUser.sql` script
