using WebApi;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

var startUp = new StartUp(builder.Configuration);

startUp.ConfigureServices(builder.Services);

var app = builder.Build();

startUp.Configure(app, app.Environment);

app.MapControllers();
app.Run();