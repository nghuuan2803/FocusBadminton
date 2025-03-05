using AutoMapper;
using Domain.Repositories;
using Shared.Bookings;
using Shared.Enums;

namespace Application.Features.Bookings.Commands
{
    public class ResumeBookingCommand : IRequest<Result<BookingDTO>>
    {
        public int BookingId { get; set; }
        public string? UpdateBy { get; set; }
    }
    public class ResumeBookingCommandHandler : IRequestHandler<ResumeBookingCommand, Result<BookingDTO>>
    {
        private readonly IRepository<Booking> _repository;
        private readonly IMapper _mapper;

        public ResumeBookingCommandHandler(IRepository<Booking> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<BookingDTO>> Handle(ResumeBookingCommand request, CancellationToken cancellationToken)
        {
            var booking = await _repository.FindAsync(p => p.Id == request.BookingId);
            if (booking == null)
            {
                return Result<BookingDTO>.Failure(Error.NotFound("Booking", request.BookingId.ToString()));
            }
            if (booking.Status != BookingStatus.Paused)
            {
                return Result<BookingDTO>.Failure(Error.Validation("Status must be Paused"));
            }
            booking.Status = BookingStatus.Approved;
            booking.UpdatedAt = DateTimeOffset.UtcNow;
            booking.UpdatedBy = request.UpdateBy;
            await _repository.SaveAsync();

            return Result<BookingDTO>.Success(_mapper.Map<BookingDTO>(booking));
        }
    }
}
