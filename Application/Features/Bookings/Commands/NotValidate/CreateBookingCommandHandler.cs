using AutoMapper;
using Domain.Repositories;
using Shared.Bookings;

namespace Application.Features.Bookings.Commands.NotValidate
{
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
            var holds = await _holdRepo.GetAllAsync(x => x.HeldBy == request.MemberId.ToString() && x.ExpiresAt > DateTimeOffset.Now);
            //if (holds.Count() != request.Details.Count)
            //{
            //    return Result<BookingDTO>.Failure(Error.Validation("Chưa giữ lịch"));
            //}
            //foreach (var item in request.Details)
            //{
            //    if (!holds.Any(x => x.CourtId == item.CourtId &&
            //        x.TimeSlotId == item.TimeSlotId &&
            //        x.BeginAt == item.BeginAt &&
            //        x.EndAt == item.EndAt &&
            //        x.DayOfWeek == item.DayOfWeek))
            //    {
            //        return Result<BookingDTO>.Failure(Error.Validation("Chưa giữ lịch"));
            //    }                
            //}
            await _unitOfWork.BeginAsync();
            var booking = _mapper.Map<Booking>(request);
            await _repository.AddAsync(booking);
            _holdRepo.RemoveRange(holds);
            await _unitOfWork.CommitAsync();
            var result = _mapper.Map<BookingDTO>(booking);
            return Result<BookingDTO>.Success(result);
        }
    }
}
