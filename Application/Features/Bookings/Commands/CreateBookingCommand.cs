using AutoMapper;
using Domain.Repositories;
using Shared.Bookings;
using Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Bookings.Commands
{
    public class CreateBookingCommand : IRequest<Result<BookingDTO>>
    {
        public int MemberId { get; set; }
        public int? TeamId { get; set; }
        public BookingType Type { get; set; } = BookingType.InDay;
        public DateTimeOffset? ApprovedAt { get; set; }
        public DateTimeOffset? CompletedAt { get; set; }
        public double Amount { get; set; }
        public double Deposit { get; set; }
        public int? VoucherId { get; set; }
        public double Discount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        [MaxLength(250)]
        public string? Note { get; set; }
        [MaxLength(250)]
        public string? AdminNote { get; set; }

        public ICollection<BookingItem>? Details { get; set; }
    }

    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, Result<BookingDTO>>
    {
        private readonly IRepository<Booking> _repository;
        private readonly IRepository<BookingHold> _holdRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateBookingCommandHandler(IRepository<Booking> repository,
            IRepository<BookingHold> holds, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _repository = repository;
            _holdRepo = holds;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<BookingDTO>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            // kiểm tra xem đã giữ lịch chưa
            var holds = await _holdRepo.GetAllAsync(x => x.HeldBy == request.MemberId.ToString() && x.ExpiresAt > DateTimeOffset.Now);
            if (holds.Count() != request.Details.Count)
            {
                return Result<BookingDTO>.Failure(Error.Validation("Chưa giữ lịch"));
            }
            foreach (var item in request.Details)
            {
                if (!holds.Any(x => x.CourtId == item.CourtId &&
                    x.TimeSlotId == item.TimeSlotId &&
                    x.BeginAt == item.BeginAt &&
                    x.EndAt == item.EndAt &&
                    x.DayOfWeek == item.DayOfWeek))
                {
                    return Result<BookingDTO>.Failure(Error.Validation("Chưa giữ lịch"));
                }
            }
            // bắt đầu transaction
            await _unitOfWork.BeginAsync();

            // tạo booking
            var booking = _mapper.Map<Booking>(request);

            // xóa giữ lịch
            _holdRepo.RemoveRange(holds);

            // lưu booking
            await _repository.AddAsync(booking);

            // commit transaction
            await _unitOfWork.CommitAsync();

            // trả về kết quả
            var result = _mapper.Map<BookingDTO>(booking);
            return Result<BookingDTO>.Success(result);
        }
    }
}
