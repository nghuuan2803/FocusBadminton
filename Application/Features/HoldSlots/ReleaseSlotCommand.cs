using Application.Interfaces.DapperQueries;
using Domain.Repositories;

namespace Application.Features.HoldSlots
{
    public class ReleaseSlotCommand : IRequest<bool>
    {
        public int HoldId { get; set; }
        public DateTime HeldAt { get; set; }
        public string? HeldBy { get; set; }
    }
    public class ReleaseSlotCommandHandler : IRequestHandler<ReleaseSlotCommand, bool>
    {
        private readonly IRepository<BookingHold> _repository;
        public ReleaseSlotCommandHandler(IRepository<BookingHold> repository)
        {
            _repository = repository;
        }
        public async Task<bool> Handle(ReleaseSlotCommand request, CancellationToken cancellationToken)
        {
            var bookingHold = await _repository.FindAsync(request.HoldId);
            if (bookingHold == null)
            {
                return false;
            }
            if ( bookingHold.HeldAt != request.HeldAt || bookingHold.HeldBy != request.HeldBy)
            {
                return false;
            }
            _repository.Remove(bookingHold);
            await _repository.SaveAsync(cancellationToken);
            return true;
        }
    }
}
