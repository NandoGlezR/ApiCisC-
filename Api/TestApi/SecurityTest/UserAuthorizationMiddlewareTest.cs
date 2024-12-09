using System.Security.Claims;
using Domain.Core.Models;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repository;
using Microsoft.AspNetCore.Authorization;
using Moq;
using WebApi.Security;
using WebApi.Security.Exceptions;

namespace TestApi.SecurityTest;

[TestFixture]
public class UserAuthorizationMiddlewareTest
{
    private Mock<IAuthorizationService> _mockAuthorizationService;
    private Mock<IRepositoryAsync<UserOwnedEntity, string>> _mockRepository;
    private UserAuthorizationMiddleware<UserOwnedEntity, string> _middleware;

    [SetUp]
    public void SetUp()
    {
        _mockAuthorizationService = new Mock<IAuthorizationService>();
        _mockRepository = new Mock<IRepositoryAsync<UserOwnedEntity, string>>();
    
        _middleware = new UserAuthorizationMiddleware<UserOwnedEntity, string>(
            _mockAuthorizationService.Object,
            _mockRepository.Object
        );
    }
    
    [Test]
    public void CheckAuthorizationAsync_ShouldSucceed_WhenAuthorizationIsSuccessful()
    {
        var userId = Guid.NewGuid().ToString();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        }));
        var requirement = new Mock<IAuthorizationRequirement>().Object;
        var resourceId = Guid.NewGuid().ToString();

        var resource = new Topic { Id = resourceId, Title = "Test Topic", UserId = userId};

        _mockRepository.Setup(r => r.FindByIdAsync(resourceId))
            .ReturnsAsync(resource);

        _mockAuthorizationService.Setup(a => 
                a.AuthorizeAsync(user, resource, It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
            .ReturnsAsync(AuthorizationResult.Success());
        
        Assert.DoesNotThrowAsync(() => _middleware.CheckAuthorizationAsync(user, requirement, resourceId));
    }
    
    [Test]
    public void CheckAuthorizationAsync_ShouldThrowUserNotAuthorizedException_WhenAuthorizationFails()
    {
        var userId = Guid.NewGuid().ToString();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        }));
        var requirement = new Mock<IAuthorizationRequirement>().Object;
        var resourceId = Guid.NewGuid().ToString();

        var resource = new Topic { Id = resourceId, Title = "Test Topic", UserId = userId};

        _mockRepository.Setup(r => r.FindByIdAsync(resourceId))
            .ReturnsAsync(resource);

        _mockAuthorizationService.Setup(a => 
                a.AuthorizeAsync(user, resource, It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
            .ReturnsAsync(AuthorizationResult.Failed());
        
        var exception = Assert.ThrowsAsync<UserNotAuthorizedException>(
            () => _middleware.CheckAuthorizationAsync(user, requirement, resourceId)
        );
    
        Assert.That(exception.Message, Is.EqualTo($"You are not authorized to modify this {resource.GetType().Name.ToLower()}"));
    }
    
    [Test]
    public void CheckAuthorizationAsync_ShouldThrowEntityNullException_WhenResourceNotFound()
    {
        var userId = Guid.NewGuid().ToString();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        }));
        var requirement = new Mock<IAuthorizationRequirement>().Object;
        var resourceId = Guid.NewGuid().ToString();

        _mockRepository.Setup(r => r.FindByIdAsync(resourceId))
            .ReturnsAsync((Topic)null);
        
        var ex = Assert.ThrowsAsync<EntityNullException>(async () =>
            await _middleware.CheckAuthorizationAsync(user, requirement, resourceId));

        Assert.That(ex.Message, Is.EqualTo($"{typeof(UserOwnedEntity).Name} not found."));
    }
}