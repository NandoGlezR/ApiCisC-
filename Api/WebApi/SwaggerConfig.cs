using System.Diagnostics.CodeAnalysis;
using Domain.Entities;
using Domain.Repository;
using Microsoft.OpenApi.Models;
using Persistence.Repository;
using Swashbuckle.AspNetCore.Annotations; 

namespace WebApi;

[ExcludeFromCodeCoverage]
public static class SwaggerConfig
{
    public static void AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });
            
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    new string[]{}
                }
            });
            
            options.EnableAnnotations();
        });

        services.AddScoped<IRepositoryAsync<Topic, string>, TopicRepository>();
        services.AddScoped<IRepositoryAsync<Idea, string>, IdeaRepository>();
    }
}