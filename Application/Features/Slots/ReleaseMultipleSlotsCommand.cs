using Application.Interfaces;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Slots
{
    public class ReleaseMultipleSlotsCommand : IRequest<bool>
    {
        public List<int> HoldIds { get; set; } = new List<int>();
        public string? HeldBy { get; set; }
    }

    public class ReleaseMultipleSlotsCommandHandler : IRequestHandler<ReleaseMultipleSlotsCommand, bool>
    {
        private readonly IRepository<BookingHold> _repository;
        private readonly ISlotNotification _slotNotification;

        public ReleaseMultipleSlotsCommandHandler(
            IRepository<BookingHold> repository,
            ISlotNotification slotNotification)
        {
            _repository = repository;
            _slotNotification = slotNotification;
        }

        public async Task<bool> Handle(ReleaseMultipleSlotsCommand request, CancellationToken cancellationToken)
        {
            bool allReleased = true;
            var bookingHolds = await _repository.GetAllAsync(b => request.HoldIds.Contains(b.Id));

            foreach (var bookingHold in bookingHolds)
            {
                // Kiểm tra thông tin xác thực
                if (bookingHold.HeldBy != request.HeldBy)
                {
                    allReleased = false;
                    continue;
                }

                // Lưu thông tin cần thiết trước khi xóa
                var payload = new
                {
                    HoldSlotId = bookingHold.Id,
                    bookingHold.CourtId,
                    bookingHold.TimeSlotId,
                    BookingType = (int)bookingHold.BookingType,
                    bookingHold.BeginAt,
                    bookingHold.EndAt,
                    bookingHold.DayOfWeek,
                    bookingHold.HeldBy
                };

                // Xóa record BookingHold
                _repository.Remove(bookingHold);

                // Gửi thông báo nhả slot
                await _slotNotification.NotifySlotReleasedAsync(payload, cancellationToken);
            }

            // Lưu thay đổi vào database
            await _repository.SaveAsync(cancellationToken);

            return allReleased;
        }
    }
}
