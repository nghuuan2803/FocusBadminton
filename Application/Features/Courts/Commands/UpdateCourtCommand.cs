using AutoMapper;
using Domain.Repositories;
using Shared.Courts;
using Shared.Enums;

namespace Application.Features.Courts.Commands
{
    public record UpdateCourtCommand : IRequest<Result<CourtDTO>>
    {
        public int Id { get; set; }
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public int? FacilityId { get; init; }
        public double Coofficient { get; init; } = 1.0;
        public CourtType Type { get; init; } = CourtType.Normal;
        public CourtStatus Status { get; init; } = CourtStatus.Available;
        public string? Images { get; init; }
        public string? UpdatedBy { get; init; }
    }
    
    public class UpdateCourtCommandHandler : IRequestHandler<UpdateCourtCommand, Result<CourtDTO>>
    {
        private readonly IRepository<Court> _repository;
        private readonly IMapper _mapper;
        public UpdateCourtCommandHandler(IRepository<Court> repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }
        public async Task<Result<CourtDTO>> Handle(UpdateCourtCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.FindAsync(request.Id, cancellationToken);
            if (entity == null)
            {
                return Result<CourtDTO>.Failure(Error.NotFound("court", request.Id.ToString()));
            }
            entity.Name = request.Name;
            entity.Coofficient = request.Coofficient;
            entity.Status = request.Status;
            entity.Type = request.Type;
            entity.Images = request.Images;
            entity.FacilityId = request.FacilityId;
            entity.Description = request.Description;
            entity.UpdatedAt = DateTimeOffset.UtcNow;
            await _repository.SaveAsync(cancellationToken);
            return _mapper.Map<CourtDTO>(entity);
        }
    }
}
