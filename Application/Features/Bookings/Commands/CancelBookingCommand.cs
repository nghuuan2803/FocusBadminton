using AutoMapper;
using Domain.Repositories;
using Shared.Bookings;
using Shared.Enums;

namespace Application.Features.Bookings.Commands
{
    public class CancelBookingCommand : IRequest<Result<BookingDTO>>
    {
        public int BookingId { get; init; }
        //public string? CancelBy { get; init; }
    }
    public class CancelBookingCommandHandler: IRequestHandler<CancelBookingCommand, Result<BookingDTO>>
    {
        private readonly IRepository<Booking> _repository;
        private readonly IMapper _mapper;
        public CancelBookingCommandHandler(IRepository<Booking> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<Result<BookingDTO>> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
        {
            //if (request.CancelBy == null)
            //{
            //    return Result<BookingDTO>.Failure(Error.Validation("CancelBy is null"));
            //}
            var booking = await _repository.FindAsync(p => p.Id == request.BookingId);
            if (booking == null)
            {
                return Result<BookingDTO>.Failure(Error.NotFound("Booking", request.BookingId.ToString()));
            }
            if (booking.Status != BookingStatus.Approved && booking.Status != BookingStatus.Pending)
            {
                return Result<BookingDTO>.Failure(Error.Validation($"Invalid status: Status must be [Approved] or [Pending], CurrentStatus is [{booking.Status.ToString()}]"));
            }


            booking.Status = BookingStatus.Canceled;
            var now = DateTimeOffset.UtcNow;
            booking.UpdatedAt = now;
            //booking.UpdatedBy = request.CancelBy;
            booking.UpdatedBy = "system";
            _repository.Update(booking);
            await _repository.SaveAsync();
            var dto = _mapper.Map<BookingDTO>(booking);
            return Result<BookingDTO>.Success(dto);
        }
    }
}
