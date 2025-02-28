using Application.Interfaces;
using Domain.Repositories;

namespace Application.Features.HoldSlots
{
    public class ReleaseSlotCommand : IRequest<bool>
    {
        public int HoldId { get; set; }
        public string? HeldBy { get; set; }
    }

    public class ReleaseSlotCommandHandler : IRequestHandler<ReleaseSlotCommand, bool>
    {
        private readonly IRepository<BookingHold> _repository;
        private readonly ISlotNotification _slotNotification;

        public ReleaseSlotCommandHandler(IRepository<BookingHold> repository, 
            ISlotNotification slotNotification)
        {
            _repository = repository;
            _slotNotification = slotNotification;
        }

        public async Task<bool> Handle(ReleaseSlotCommand request, CancellationToken cancellationToken)
        {
            var bookingHold = await _repository.FindAsync(request.HoldId);
            if (bookingHold == null)
            {
                return false;
            }
            // Kiểm tra thông tin xác thực
            if (bookingHold.HeldBy != request.HeldBy)
            {
                return false;
            }

            // Lưu thông tin cần thiết trước khi xóa (để thông báo cho client)
            var payload = new
            {
                HoldSlotId = bookingHold.Id,
                CourtId = bookingHold.CourtId,
                TimeSlotId = bookingHold.TimeSlotId,
                BookingType = (int)bookingHold.BookingType,   // Enum có thể được chuyển sang string nếu cần
                BeginAt = bookingHold.BeginAt,
                EndAt = bookingHold.EndAt,
                DayOfWeek = bookingHold.DayOfWeek,
                HeldBy = bookingHold.HeldBy
            };

            // Xóa record BookingHold
            _repository.Remove(bookingHold);
            await _repository.SaveAsync(cancellationToken);

            // Gửi thông báo nhả slot cho tất cả các client
            await _slotNotification.NotifySlotReleasedAsync(payload, cancellationToken);

            return true;
        }
    }
}
