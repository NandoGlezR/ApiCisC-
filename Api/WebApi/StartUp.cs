using System.Text.Json;
using Application;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Persistence;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using WebApi.ExceptionHandler;
using WebApi.Security;
using WebApi.Security.Handlers;

namespace WebApi;

[ExcludeFromCodeCoverage]
public class StartUp
{
    protected IConfiguration Configuration { get; }
    public StartUp(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddPersistence(Configuration);
        services.AddSwaggerConfiguration();
        services.AddJwtConfiguration(Configuration);
        services.AddApplication();
        services.AddControllers();
        services.AddExceptionHandler<InvalidRequestExceptionHandler>();
        services.AddExceptionHandler<EntityNullExceptionHandler>();
        services.AddExceptionHandler<AuthorizationExceptionHandler>();
        services.AddSingleton<IAuthorizationHandler, ResourceEditionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, ResourceDeletionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, ResourceAggregationAuthorizationHandler>();
        services.AddScoped(typeof(UserAuthorizationMiddleware<,>), typeof(UserAuthorizationMiddleware<,>));
        services.AddEndpointsApiExplorer();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseExceptionHandler("/error");
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
    }
}