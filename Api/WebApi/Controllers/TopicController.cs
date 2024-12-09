using Application.Dto;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using WebApi.Security;
using WebApi.Security.Requirements;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/topic")]
    public class TopicController : ControllerBase
    {
        private readonly IIdeaService _ideaService;
        private readonly ITopicService _topicService;
        private readonly UserAuthorizationMiddleware<Topic, string> _authorizationMiddleware;
        
        public TopicController(ITopicService topicService, 
                UserAuthorizationMiddleware<Topic, string> authorizationMiddleware,
                IIdeaService ideaService)
        {
            _ideaService = ideaService;
            _topicService = topicService;
            _authorizationMiddleware = authorizationMiddleware;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        }

        [HttpPost]
        [SwaggerResponse(200, "Topic successfully created", typeof(TopicDto))]
        [SwaggerResponse(401, "Not Unauthorized or token is expired.")]
        [SwaggerResponse(400, "Invalid request data", typeof(ProblemDetails))]
        public async Task<IActionResult> CreateTopic([FromBody] TopicDto topicDto)
        {
            var createdTopic = await _topicService.CreateTopicAsync(topicDto, GetUserId());
            return Ok(createdTopic);
        }

        [HttpPatch]
        [Route("{topicId}")]
        [SwaggerResponse(200, "Topic successfully updated", typeof(TopicDto))]
        [SwaggerResponse(401, "Not Unauthorized or token is expired.")]
        [SwaggerResponse(403, "Forbidden: User not authorized", typeof(ProblemDetails))]
        [SwaggerResponse(400, "Invalid request data", typeof(ProblemDetails))]
        public async Task<IActionResult> UpdateTopic([FromRoute] string topicId, [FromBody] TopicDto topicDto)
        {
            await _authorizationMiddleware.CheckAuthorizationAsync(User, new EditionRequirement(), topicId);
            
            topicDto.Id = topicId;

            var updatedTopic = await _topicService.UpdateTopicAsync(topicDto, GetUserId());

            return Ok(updatedTopic);
        }

        [HttpDelete]
        [Route("{topicId}")]
        [SwaggerResponse(200, "Topic successfully deleted", typeof(object))]
        [SwaggerResponse(401, "Not authenticated or token is expired.")]
        [SwaggerResponse(403, "Forbidden: User not authorized", typeof(ProblemDetails))]
        public async Task<IActionResult> DeleteTopic([FromRoute] string topicId)
        {
            var userId = GetUserId();

            await _authorizationMiddleware.CheckAuthorizationAsync(User, new DeletionRequirement(), topicId);
            await _topicService.DeleteTopicAsync(topicId, userId);
            
            return Ok(new { message = "Topic successfully deleted." });
        }

        [HttpGet]
        [Route("{topicId}")]
        [SwaggerResponse(200, "Returns the topic", typeof(TopicDto))]
        [SwaggerResponse(401, "Not Unauthorized or token is expired.")]
        [SwaggerResponse(404, "Topic not found", typeof(ProblemDetails))]
        public async Task<IActionResult> GetTopicById([FromRoute] string topicId)
        {
            var topic = await _topicService.GetTopicByIdAsync(topicId);
            return Ok(topic);
        }

        [HttpGet]
        [Route("me")]
        [SwaggerResponse(200, "Returns a paginated list of topics", typeof(IEnumerable<TopicDto>))]
        [SwaggerResponse(401, "Not Unauthorized or token is expired.")]
        public async Task<IActionResult> GetTopicsByUser([FromQuery] int page, [FromQuery] int pageSize)
        {
            var topics = await _topicService.GetPageAsync(page, pageSize, GetUserId());
            return Ok(topics);
        }

        [HttpGet]
        [SwaggerResponse(200, "Returns a paginated list of topics", typeof(IEnumerable<TopicDto>))]
        [SwaggerResponse(401, "Not Unauthorized or token is expired.")]
        public async Task<IActionResult> GetListTopics([FromQuery] int page, [FromQuery] int pageSize)
        {
            var topics = await _topicService.GetPageAsync(page, pageSize);
            return Ok(topics);
        }

        [HttpPost]
        [Route("{topicId}/idea")]
        [SwaggerResponse(200, "Idea successfully created", typeof(IdeaDto))]
        [SwaggerResponse(401, "Not Unauthorized or token is expired.")]
        public async Task<IActionResult> CreateIdea([FromBody] IdeaDto ideaDto, [FromRoute] string topicId)
        {
            await _authorizationMiddleware.CheckAuthorizationAsync(User, new AggregationRequirement(), topicId);
            var userId = GetUserId();
            var createdIdea = await _ideaService.CreateIdeaAsync(ideaDto, userId, topicId);
            return Ok(createdIdea);
        }

        [HttpGet]
        [Route("{topicId}/idea")]
        [SwaggerResponse(200, "Returns a paginated list of ideas", typeof(IEnumerable<IdeaDto>))]
        [SwaggerResponse(401, "Not Unauthorized or token is expired.")]
        [SwaggerResponse(404, "Ideas not found", typeof(ProblemDetails))]
        public async Task<IActionResult> GetIdeasByTopic([FromQuery] int page, [FromQuery] int pageSize, [FromRoute] string topicId)
        {
            var ideas = await _ideaService.GetPageAsync(page, pageSize, topicId);
            return Ok(ideas);
        }
    }
}
