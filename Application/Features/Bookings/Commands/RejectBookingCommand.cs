using AutoMapper;
using Domain.Repositories;
using Shared.Bookings;
using Shared.Enums;

namespace Application.Features.Bookings.Commands
{
    public record RejectBookingCommand : IRequest<Result<BookingDTO>>
    {
        public int BookingId { get; init; }
        public string? RejectBy { get; init; }
    }

    public class RejectBookingCommandHandler : IRequestHandler<RejectBookingCommand, Result<BookingDTO>>
    {
        private readonly IRepository<Booking> _repository;
        private readonly IMapper _mapper;
        public RejectBookingCommandHandler(IRepository<Booking> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<Result<BookingDTO>> Handle(RejectBookingCommand request, CancellationToken cancellationToken)
        {
            if (request.RejectBy == null)
            {
                return Result<BookingDTO>.Failure(Error.Validation("UpdatedBy is null"));
            }
            var booking = await _repository.FindAsync(p => p.Id == request.BookingId);
            if (booking == null)
            {
                return Result<BookingDTO>.Failure(Error.NotFound("Booking", request.BookingId.ToString()));
            }
            if (booking.Status != BookingStatus.Pending)
            {
                return Result<BookingDTO>.Failure(Error.Validation($"Invalid status: Status must be [Pending], CurrentStatus is [{booking.Status.ToString()}]"));
            }
            booking.Status = BookingStatus.Rejected;
            var now = DateTimeOffset.Now;
            booking.ApprovedAt = now;
            booking.UpdatedAt = now;
            booking.UpdatedBy = request.RejectBy;
            _repository.Update(booking);
            await _repository.SaveAsync();
            var dto = _mapper.Map<BookingDTO>(booking);
            return Result<BookingDTO>.Success(dto);
        }
    }
}
