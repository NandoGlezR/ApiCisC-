## Installing EF at the .NET development tool level
EF needs to be installed globally in order to make use of the following commands.
```cmd
dotnet tool install --global dotnet-ef
```

## Step to be able to execute the migrations.

To be able to add or initialize the first migration, we must position ourselves in the
project `Infraestructure.Persistences` o `Persistences` execute the following
command structure.
`dotnet ef migrations add <name_migrations> --startup-project <main_application>`
It is important to add this `--startup-project` since `persistences` is only a library project,
so it is not an executable and depends on adding another project.

`main_application`: is the API where the dependency injection is initialized and run.

```cmd
dotnet ef migrations add InitialCreate --startup-project ../WebApi
```

To be able to delete the database.
```cmd
dotnet ef database drop --startup-project ../WebApi
```

In order to create or update the database.
```cmd
dotnet ef database update --startup-project ../WebApi
```

## Useful Commands

The following are some useful commands for working with the project:

### 1. Build the Project
```bash
dotnet build --configuration Release
```
This command builds the project in the Release configuration. The Release configuration is used to compile the code optimized for production execution.

### 2. Check code style
```bash
dotnet format --verify-no-changes
```
This command checks that the code is formatted according to the defined style rules.

Note: Before running this command, make sure to install the formatting tool by executing:
```bash
dotnet tool install -g dotnet-format
```
### 2. Run tests and collect coverage
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:ExcludeByFile="**/Program.cs" /p:ExcludeByAttribute="ExcludeFromCodeCoverage"
```
This command runs the project tests and collects code coverage information. The specified options do the following:

- /p:CollectCoverage=true: Enables the collection of code coverage data.
- /p:CoverletOutputFormat=opencover: Sets the output format of the coverage to OpenCover, which is a commonly used format.
- /p:ExcludeByFile="**/Program.cs": Excludes the Program.cs file from the coverage collection.
- /p:ExcludeByAttribute="ExcludeFromCodeCoverage": Excludes any code marked with the ExcludeFromCodeCoverage attribute from the coverage collection.