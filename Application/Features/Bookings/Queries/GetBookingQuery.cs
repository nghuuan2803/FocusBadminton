using AutoMapper;
using Domain.Repositories;
using Shared.Bookings;

namespace Application.Features.Bookings.Queries
{
    public record GetBookingQuery(int id) : IRequest<Result<BookingDTO>>
    {
    }
    public class GetBookingQueryHandler : IRequestHandler<GetBookingQuery, Result<BookingDTO>>
    {
        private readonly IRepository<Booking> _repository;
        private readonly IMapper _mapper;
        public GetBookingQueryHandler(IRepository<Booking> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<Result<BookingDTO>> Handle(GetBookingQuery request, CancellationToken cancellationToken)
        {
            var booking = await _repository.FindAsync(p=>p.Id==request.id);
            if (booking == null)
            {
                return Result<BookingDTO>.Failure(Error.NotFound($"Booking[{request.id}]","Không tìm thấy"));
            }
            return Result<BookingDTO>.Success(_mapper.Map<BookingDTO>(booking));
        }
    }
}
