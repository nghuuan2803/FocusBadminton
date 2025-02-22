using AutoMapper;
using Domain.Repositories;
using Shared.Courts;

namespace Application.Features.Courts.Queries
{
    public record GetCourtsQuery() : IRequest<IEnumerable<CourtDTO>> { }

    public class GetCourtsQueryHandler : IRequestHandler<GetCourtsQuery, IEnumerable<CourtDTO>>
    {
        private readonly IRepository<Court> _repository;
        private readonly IMapper _mapper;
        public GetCourtsQueryHandler(IRepository<Court> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<CourtDTO>> Handle(GetCourtsQuery request, CancellationToken cancellationToken)
        {
            var entities =  await _repository.GetAllAsync(null!,cancellationToken);
            return _mapper.Map<IEnumerable<CourtDTO>>(entities);
        }
    }
}
