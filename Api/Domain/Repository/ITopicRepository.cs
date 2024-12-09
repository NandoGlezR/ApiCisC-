using Domain.Entities;
using Domain.Views;

namespace Domain.Repository;

public interface ITopicRepository : IRepositoryAsync<Topic, string>, IPaginationRepository<TopicPagination>
{
}