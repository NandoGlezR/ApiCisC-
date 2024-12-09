using System.Security.Claims;
using Domain.Core.Models;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repository;
using Microsoft.AspNetCore.Authorization;
using WebApi.Security.Exceptions;

namespace WebApi.Security;

public class UserAuthorizationMiddleware<T, TKey> where T : UserOwnedEntity where TKey : IEquatable<TKey>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRepositoryAsync<T, TKey> _repository;

    public UserAuthorizationMiddleware(
        IAuthorizationService authorizationService,
        IRepositoryAsync<T, TKey> repository)
    {
        _authorizationService = authorizationService;
        _repository = repository;
    }

    public async Task CheckAuthorizationAsync(ClaimsPrincipal claims,
        IAuthorizationRequirement requirement,
        TKey resourceId)
    {
        T resource;
        if (typeof(T) == typeof(Vote))
        {
            resource = await _repository.FindByIdAsync(resourceId, claims.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        }
        else
        {
            resource = await _repository.FindByIdAsync(resourceId);
        }

        if (resource == null)
        {
            throw new EntityNullException($"{typeof(T).Name} not found.");
        }
        
        var authorizationResult = await _authorizationService.AuthorizeAsync(claims, resource, requirement);

        if (!authorizationResult.Succeeded)
        {
            throw new UserNotAuthorizedException($"You are not authorized to modify this " +
                                                 $"{resource.GetType().Name.ToLower()}");
        }
    }
}