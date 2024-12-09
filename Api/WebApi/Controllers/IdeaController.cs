using Application.Dto;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Domain.Entities;
using Domain.Repository;
using Microsoft.AspNetCore.Authorization;
using WebApi.Security;
using WebApi.Security.Requirements;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/idea")]
    public class IdeaController : ControllerBase
    {
        private readonly IIdeaService _ideaService;
        private readonly IVoteService _voteService;
        private readonly IVoteRepository _voteRepository;
        private readonly UserAuthorizationMiddleware<Idea, string> _authorizationMiddleware;
        private readonly UserAuthorizationMiddleware<Vote, string> _voteAuthorizationMiddleware;

        public IdeaController(IIdeaService ideaService, IVoteService voteService, IVoteRepository voteRepository, 
            UserAuthorizationMiddleware<Idea, string> authorizationMiddleware,
            UserAuthorizationMiddleware<Vote, string> voteAuthorizationMiddleware)
        {
            _ideaService = ideaService;
            _voteService = voteService;
            _voteRepository = voteRepository;
            _authorizationMiddleware = authorizationMiddleware;
            _voteAuthorizationMiddleware = voteAuthorizationMiddleware;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        }

        [HttpPatch]
        [Route("{ideaId}")]
        [SwaggerResponse(200, "Idea updated successfully.", typeof(IdeaDto))]
        [SwaggerResponse(400, "Bad request or invalid idea.")]
        [SwaggerResponse(401, "Not Unauthorized or token is expired.")]
        [SwaggerResponse(403, "Forbidden: User not authorized")]
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
        [SwaggerResponse(200, "Idea deleted successfully.")]
        [SwaggerResponse(401, "Not Unauthorized or token is expired.")]
        [SwaggerResponse(403, "Forbidden: User not authorized")]
        public async Task<IActionResult> DeleteIdea([FromRoute] string ideaId)
        {
            await _authorizationMiddleware.CheckAuthorizationAsync(User, new DeletionRequirement(), ideaId);
            var userId = GetUserId();
            await _ideaService.DeleteIdeaAsync(ideaId, userId);
            return Ok(true);
        }

        [HttpPost]
        [Route("{ideaId}/vote")]
        [SwaggerResponse(200, "Vote created successfully.", typeof(Vote))]
        [SwaggerResponse(400, "Bad request.")]
        [SwaggerResponse(401, "Not Unauthorized or token is expired.")]
        public async Task<IActionResult> CreateVote([FromRoute] string ideaId)
        {
            await _authorizationMiddleware.CheckAuthorizationAsync(User, new AggregationRequirement(), ideaId);
            var userId = GetUserId();
            var voteCreated = await _voteService.CreateVoteAsync(ideaId, userId);
            return Ok(voteCreated);
        }

        [HttpDelete]
        [Route("{ideaId}/vote")]
        [SwaggerResponse(200, "Vote deleted successfully.")]
        [SwaggerResponse(401, "Not Unauthorized or token is expired.")]
        [SwaggerResponse(403, "Forbidden: User not authorized")]
        public async Task<IActionResult> DeleteVote([FromRoute] string ideaId)
        {
            var userId = GetUserId();
            var vote = _voteRepository.FirstOrDefaultAsync(vote => vote.IdeaId == ideaId && vote.UserId == userId).Result;
            await _voteAuthorizationMiddleware.CheckAuthorizationAsync(User, new DeletionRequirement(), vote.Id);
            var voteDeleted = await _voteService.DeleteVoteAsync(ideaId, userId);
            return Ok(true);
        }
    }
}

