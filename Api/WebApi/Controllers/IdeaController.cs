using Application.Dto;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using WebApi.Security;
using WebApi.Security.Requirements;

namespace WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/idea")]
    public class IdeaController : ControllerBase
    {
        private readonly IIdeaService _ideaService;
        private readonly IVoteService _voteService;
        private readonly UserAuthorizationMiddleware<Idea, string> _authorizationMiddleware;
        private readonly UserAuthorizationMiddleware<Vote, string> _voteAuthorizationMiddleware;

        public IdeaController(IIdeaService ideaService, IVoteService voteService, 
            UserAuthorizationMiddleware<Idea, string> authorizationMiddleware,
            UserAuthorizationMiddleware<Vote, string> voteAuthorizationMiddleware)
        {
            _ideaService = ideaService;
            _voteService = voteService;
            _authorizationMiddleware = authorizationMiddleware;
            _voteAuthorizationMiddleware = voteAuthorizationMiddleware;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        }

        [HttpPatch]
        [Route("{ideaId}")]
        public async Task<IActionResult> UpdateIdea([FromRoute] string ideaId, [FromBody] IdeaDto ideaDto)
        {
            await _authorizationMiddleware.CheckAuthorizationAsync(User, new EditionRequirement(), ideaId);
            var userId = GetUserId();
            ideaDto.Id = ideaId;
            var updatedIdea = await _ideaService.UpdateIdeaAsync(ideaDto, userId);
            return Ok(updatedIdea);
        }

        [HttpDelete]
        [Route("{ideaId}")]
        public async Task<IActionResult> DeleteIdea([FromRoute] string ideaId)
        {
            await _authorizationMiddleware.CheckAuthorizationAsync(User, new DeletionRequirement(), ideaId);
            var userId = GetUserId();
            await _ideaService.DeleteIdeaAsync(ideaId, userId);
            return Ok(true);
        }

        // Endpoints de votos
        [HttpPost]
        [Route("{ideaId}/vote")]
        public async Task<IActionResult> CreateVote([FromRoute] string ideaId)
        {
            await _authorizationMiddleware.CheckAuthorizationAsync(User, new AggregationRequirement(), ideaId);
            var userId = GetUserId();
            var voteCreated = await _voteService.CreateVoteAsync(ideaId, userId);
            return Ok(voteCreated);
        }

        [HttpDelete]
        [Route("{ideaId}/vote")]
        public async Task<IActionResult> DeleteVote([FromRoute] string ideaId)
        {
            await _voteAuthorizationMiddleware.CheckAuthorizationAsync(User, new DeletionRequirement(), ideaId);
            var userId = GetUserId();
            var voteDeleted = await _voteService.DeleteVoteAsync(ideaId, userId);
            return Ok(true);
        }
    }
}



