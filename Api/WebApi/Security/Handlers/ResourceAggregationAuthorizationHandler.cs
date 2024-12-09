using Domain.Core.Models;
using Microsoft.AspNetCore.Authorization;
using WebApi.Security.Requirements;

namespace WebApi.Security.Handlers;

public class ResourceAggregationAuthorizationHandler : AuthorizationHandler<AggregationRequirement, UserOwnedEntity>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        AggregationRequirement requirement,
        UserOwnedEntity? resource)
    {
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}