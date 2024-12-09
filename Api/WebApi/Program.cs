using Persistence;
using WebApi;

var builder = WebApplication.CreateBuilder(args);

var startUp = new StartUp(builder.Configuration);

startUp.ConfigureServices(builder.Services);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.AddMigrations();
}

startUp.Configure(app, app.Environment);

app.MapControllers();
app.Run();