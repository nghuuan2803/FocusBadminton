using AutoMapper;
using Domain.Repositories;
using Shared.Bookings;

namespace Application.Features.Bookings.Queries
{
    public class GetBookingHistoryQuery : IRequest<Result<IEnumerable<BookingDTO>>>
    {
        public int MemberId { get; set; }
    }

    public class GetBookingHistoryQueryHandler : IRequestHandler<GetBookingHistoryQuery, Result<IEnumerable<BookingDTO>>>
    {
        private readonly IRepository<Booking> _repository;
        private readonly IMapper _mapper;
        public GetBookingHistoryQueryHandler(IRepository<Booking> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<Result<IEnumerable<BookingDTO>>> Handle(GetBookingHistoryQuery request, CancellationToken cancellationToken)
        {
            var bookings = (await _repository.GetAllAsync(p => p.MemberId == request.MemberId)).OrderByDescending(p=>p.Id).Take(10);
            if (bookings == null)
            {
                return Result<IEnumerable<BookingDTO>>.Failure(Error.NotFound($"Booking[{request.MemberId}]", "Không tìm thấy"));
            }
            return Result<IEnumerable<BookingDTO>>.Success(_mapper.Map<List<BookingDTO>>(bookings));
        }
    }
}
