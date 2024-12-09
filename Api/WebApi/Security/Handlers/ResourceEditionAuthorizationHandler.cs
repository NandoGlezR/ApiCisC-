using System.Security.Claims;
using Domain.Core.Models;
using Microsoft.AspNetCore.Authorization;
using WebApi.Security.Requirements;

namespace WebApi.Security.Handlers;

public class ResourceEditionAuthorizationHandler : AuthorizationHandler<EditionRequirement, UserOwnedEntity>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        EditionRequirement requirement,
        UserOwnedEntity resource)
    {
        string userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

        if (userId != resource.UserId)
        {
            context.Fail();
        }

        context.Succeed(requirement);
        
        return Task.CompletedTask;
    }
}