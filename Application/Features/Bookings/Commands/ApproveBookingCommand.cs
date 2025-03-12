using AutoMapper;
using Domain.Repositories;
using Shared.Bookings;
using Shared.Enums;

namespace Application.Features.Bookings.Commands
{
    public record ApproveBookingCommand : IRequest<Result<BookingDTO>>
    {
        public int BookingId { get; init; }
        public string? ApproveBy { get; init; }
    }

    public class ApproveBookingCommandHandler : IRequestHandler<ApproveBookingCommand, Result<BookingDTO>>
    {
        private readonly IRepository<Booking> _repository;
        private readonly IMapper _mapper;

        public ApproveBookingCommandHandler(IRepository<Booking> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<BookingDTO>> Handle(ApproveBookingCommand request, CancellationToken cancellationToken)
        {
            if(request.ApproveBy == null)
            {
                return Result<BookingDTO>.Failure(Error.Validation("ApprovedBy is null"));
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
            if (!CheckSlotTimeout(booking.Details!))
            {
                return Result<BookingDTO>.Failure(Error.Validation($"Invalid time: Có lịch đặt đã quá ngày hiện tại"));
            }
            
            booking.Status = BookingStatus.Approved;
            var now = DateTimeOffset.UtcNow;
            booking.ApprovedAt = now;
            booking.UpdatedAt = now;
            booking.UpdatedBy = request.ApproveBy; 
            _repository.Update(booking);
            await _repository.SaveAsync();
            var dto = _mapper.Map<BookingDTO>(booking);
            return Result<BookingDTO>.Success(dto);
        }
        private bool CheckSlotTimeout(IEnumerable<BookingDetail> slots)
        {
            var now = DateTimeOffset.UtcNow;
            foreach (var slot in slots)
            {
                if(slot.BeginAt < now)
                    return false;
            }
            return true;
        }
    }
}