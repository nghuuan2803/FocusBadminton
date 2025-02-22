using Domain.Repositories;

namespace Application.Features.Facilities.Queries
{
    public record GetFacilitiesQuery : IRequest<IEnumerable<Facility>>
    {
    }

    public class GetFacilitiesQueryHandler : IRequestHandler<GetFacilitiesQuery, IEnumerable<Facility>>
    {
        private readonly IRepository<Facility> _repository;
        public GetFacilitiesQueryHandler(IRepository<Facility> repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<Facility>> Handle(GetFacilitiesQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAllAsync(null!,cancellationToken);
        }
    }
}
