using Application.Core.Mapper;
using Application.Core.Validations;
using Application.Dto;
using Application.Interfaces;
using Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Application;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection service)
    {
        service.AddAutoMapper(typeof(ManagerMapper));
        service.AddScoped<ITopicService, TopicService>();
        service.AddScoped<IVoteService, VoteService>();
        service.AddScoped<IIdeaService, IdeaService>();
        service.AddTransient<IValidator<TopicDto>, TopicDtoValidator>();
        service.AddTransient<IValidator<IdeaDto>, IdeaDtoValidator>();
        service.AddTransient<IValidator<string>, StringValidator>();
        service.AddTransient<IEntityValidator, EntityValidator>();
    }
}