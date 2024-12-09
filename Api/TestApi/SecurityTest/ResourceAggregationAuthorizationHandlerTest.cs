using System.Security.Claims;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using WebApi.Security.Handlers;
using WebApi.Security.Requirements;

namespace TestApi.SecurityTest;

[TestFixture]
public class ResourceAggregationAuthorizationHandlerTest
{
    private ResourceAggregationAuthorizationHandler _handler;
    private AggregationRequirement _requirement;
    private Topic _resource;
    private string _userId;

    [SetUp]
    public void SetUp()
    {
        _handler = new ResourceAggregationAuthorizationHandler();
        _requirement = new AggregationRequirement();
        _userId = Guid.NewGuid().ToString();
        _resource = new Topic {Id = Guid.NewGuid().ToString(), UserId =  _userId, Title = "Test Topic"};
    }
    
    [Test]
    public async Task HandleRequirementAsync_ShouldSucceed_WhenUserIdMatches()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, _userId)
        }));

        var authorizationContext = new AuthorizationHandlerContext(
            new[] { _requirement }, 
            user, 
            _resource
        );
        
        await _handler.HandleAsync(authorizationContext);

        Assert.That(authorizationContext.HasSucceeded, Is.True);
    }

    [Test]
    public async Task HandleRequirementAsync_ShouldFail_WhenUserIdDoesNotMatch()
    {
        
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        }));

        var authorizationContext = new AuthorizationHandlerContext(
            new[] { _requirement }, 
            user, 
            _resource
        );

        await _handler.HandleAsync(authorizationContext);

        Assert.That(authorizationContext.HasFailed, Is.False);
    }
}