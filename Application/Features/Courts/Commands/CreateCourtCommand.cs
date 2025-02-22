using AutoMapper;
using Domain.Repositories;
using Shared.Courts;
using Shared.Enums;

namespace Application.Features.Courts.Commands
{
    public record CreateCourtCommand: IRequest<Result<CourtDTO>>
    {
        public string Name { get; init; }
        public string? Description { get; init; }
        public int? FacilityId { get; init; }
        public double Coofficient { get; init; } = 1.0;
        public CourtType Type { get; init; } = CourtType.Normal;
        public string? Images { get; init; }
        public string? CreatedBy { get; init; }
    }

    public class CreateCourtCommandHandler : IRequestHandler<CreateCourtCommand, Result<CourtDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Court> _repository;

        public CreateCourtCommandHandler(IRepository<Court> repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<Result<CourtDTO>> Handle(CreateCourtCommand request, CancellationToken cancellationToken)
        {
            var court = _mapper.Map<Court>(request);
            await _repository.AddAsync(court, cancellationToken);
            await _repository.SaveAsync(cancellationToken);
            var dto = _mapper.Map<CourtDTO>(court);
            return Result<CourtDTO>.Success(dto);
        }
    }
}
