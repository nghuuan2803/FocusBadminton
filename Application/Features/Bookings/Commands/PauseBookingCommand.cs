using AutoMapper;
using Domain.Repositories;
using Shared.Bookings;
using Shared.Enums;

namespace Application.Features.Bookings.Commands
{
    public class PauseBookingCommand : IRequest<Result<BookingDTO>>
    {
        public int BookingId { get; set; }
        public DateTimeOffset PauseAt { get; set; }
        public DateTimeOffset ResumeAt { get; set; }
        public string? UpdateBy { get; set; }
    }

    public class PauseBookingCommandHandler : IRequestHandler<PauseBookingCommand, Result<BookingDTO>>
    {
        private readonly IRepository<Booking> _repository;
        private readonly IMapper _mapper;

        public PauseBookingCommandHandler(IRepository<Booking> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<BookingDTO>> Handle(PauseBookingCommand request, CancellationToken cancellationToken)
        {
            var booking = await _repository.FindAsync(p => p.Id == request.BookingId);
            if (booking == null)
            {
                return Result<BookingDTO>.Failure(Error.NotFound("Booking", request.BookingId.ToString()));
            }
            if (booking.Status != BookingStatus.Approved)
            {
                return Result<BookingDTO>.Failure(Error.Validation("Status must be Apporved"));
            }
            booking.Status = BookingStatus.Paused;
            booking.UpdatedAt = DateTimeOffset.UtcNow;
            booking.UpdatedBy = request.UpdateBy;
            booking.PausedDate = request.PauseAt;
            booking.ResumeDate = request.ResumeAt;
            await _repository.SaveAsync();

            return Result<BookingDTO>.Success(_mapper.Map<BookingDTO>(booking));
        }
    }
}
