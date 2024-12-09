using Domain.Entities;
using Domain.Views;

namespace Domain.Repository;

public interface IIdeaRepository : IRepositoryAsync<Idea, string>, IPaginationRepository<IdeaPagination>
{
}