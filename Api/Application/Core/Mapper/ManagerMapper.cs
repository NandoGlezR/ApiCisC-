using Application.Dto;
using AutoMapper;
using Domain.Entities;

namespace Application.Core.Mapper;

public class ManagerMapper : Profile
{
    public ManagerMapper()
    {
        ConvertToEntity();
        ConvertToDto();
    }

    private void ConvertToEntity()
    {
        CreateMap<TopicDto, Topic>();
        CreateMap<IdeaDto, Idea>();
    }

    private void ConvertToDto()
    {
        CreateMap<Topic, TopicDto>();
        CreateMap<Idea,IdeaDto>();
    }
}