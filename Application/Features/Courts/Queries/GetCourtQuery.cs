using AutoMapper;
using Domain.Repositories;
using Shared.Courts;

namespace Application.Features.Courts.Queries
{
    public record GetCourtQuery : IRequest<Result<CourtDTO>>
    {
        public int Id { get; init; }
    }

    public class GetCourtQueryHandler : IRequestHandler<GetCourtQuery, Result<CourtDTO>>
    {
        private readonly IRepository<Court> _repository;
        private readonly IMapper _mapper;
        public GetCourtQueryHandler(IRepository<Court> repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }
        public async Task<Result<CourtDTO>> Handle(GetCourtQuery request, CancellationToken cancellationToken)
        {
            var court = await _repository.FindAsync(p=>p.Id == request.Id , cancellationToken);
            if (court == null)
            {
                return Result<CourtDTO>.Failure(Error.NotFound("Court", request.Id.ToString()));
            }
            var courtDto = _mapper.Map<CourtDTO>(court);
            return Result<CourtDTO>.Success(courtDto);
        }
    }
}
