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
        public async Task<IActionResult> CreateTopic([FromBody] TopicDto topicDto)
        {
            var createdTopic = await _topicService.CreateTopicAsync(topicDto, GetUserId());
            return Ok(createdTopic);
        }

        [HttpPatch]
        [Route("{topicId}")]
        public async Task<IActionResult> UpdateTopic([FromRoute] string topicId, [FromBody] TopicDto topicDto)
        {
            await _authorizationMiddleware.CheckAuthorizationAsync(User, new EditionRequirement(), topicId);
            
            topicDto.Id = topicId;

            var updatedTopic = await _topicService.UpdateTopicAsync(topicDto, GetUserId());

            return Ok(updatedTopic);
        }

        [HttpDelete]
        [Route("{topicId}")]
        public async Task<IActionResult> DeleteTopic([FromRoute] string topicId)
        {
            var userId = GetUserId();

            await _authorizationMiddleware.CheckAuthorizationAsync(User, new DeletionRequirement(), topicId);
            await _topicService.DeleteTopicAsync(topicId, userId);
            
            return Ok(new { message = "Topic successfully deleted." });
        }

        [HttpGet]
        [Route("{topicId}")]
        public async Task<IActionResult> GetTopicById([FromRoute] string topicId)
        {
            var topic = await _topicService.GetTopicByIdAsync(topicId);
            return Ok(topic);
        }
        
        [HttpGet]
        [Route("me")]
        public async Task<IActionResult> GetTopicsByUser([FromQuery] int page, [FromQuery] int pageSize)
        {
            var topics = await _topicService.GetPageAsync(page, pageSize, GetUserId());
            return Ok(topics);
        }

        [HttpGet]
        public async Task<IActionResult> GetListTopics([FromQuery] int page, [FromQuery] int pageSize)
        {
            var topics = await _topicService.GetPageAsync(page, pageSize);
            return Ok(topics);
        }

        [HttpPost]
        [Route("{topicId}/idea")]
        public async Task<IActionResult> CreateIdea([FromBody] IdeaDto ideaDto, [FromRoute] string topicId)
        {
            await _authorizationMiddleware.CheckAuthorizationAsync(User, new AggregationRequirement(), topicId);
            var userId = GetUserId();
            var createdIdea = await _ideaService.CreateIdeaAsync(ideaDto, userId, topicId);
            return Ok(createdIdea);
        }

        [HttpGet]
        [Route("{topicId}/idea")]
        public async Task<IActionResult> GetIdeasByTopic([FromQuery] int page, [FromQuery] int pageSize, [FromRoute] string topicId)
        {
            var ideas = await _ideaService.GetPageAsync(page, pageSize, topicId);
            return Ok(ideas);
        }
    }
}
