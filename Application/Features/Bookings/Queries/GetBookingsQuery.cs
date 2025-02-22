using AutoMapper;
using Domain.Repositories;
using Shared.Bookings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Bookings.Queries
{
    public record GetBookingsQuery : IRequest<IEnumerable<BookingDTO>>
    {
    }
    public class GetBookingsQueryHandler : IRequestHandler<GetBookingsQuery, IEnumerable<BookingDTO>>
    {
        private readonly IRepository<Booking> _repository;
        private readonly IMapper _mapper;
        public GetBookingsQueryHandler(IRepository<Booking> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<BookingDTO>> Handle(GetBookingsQuery request, CancellationToken cancellationToken)
        {
            var bookings = await _repository.GetAllAsync(null!,cancellationToken);
            return _mapper.Map<IEnumerable<BookingDTO>>(bookings);
        }
    }
}
