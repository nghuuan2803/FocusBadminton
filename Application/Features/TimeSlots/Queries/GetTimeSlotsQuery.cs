using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Features.TimeSlots.Queries
{
    public class GetTimeSlotsQuery : IRequest<IEnumerable<TimeSlot>>
    {
    }

    public class GetTimeSlotsHandler : IRequestHandler<GetTimeSlotsQuery, IEnumerable<TimeSlot>>
    {
        private readonly IRepository<TimeSlot> _repository;

        public GetTimeSlotsHandler(IRepository<TimeSlot> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<TimeSlot>> Handle(GetTimeSlotsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAllAsync(null!,cancellationToken);
        }
    }
}
