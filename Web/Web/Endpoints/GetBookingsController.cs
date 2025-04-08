using AutoMapper;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Bookings;
using Shared.Enums;

namespace Web.Endpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetBookingsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetBookingsController(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetBookings()
        {
            var bookings = await _dbContext.Bookings.ToListAsync();
            return Ok(bookings);
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingBookings()
        {
            var bookings = await _dbContext.Bookings
                .Where(p=>p.Status==BookingStatus.Pending)
                .Include(p => p.Member)
                .Include(p=>p.Team)
                .Include(p=>p.Voucher)
                .Include(p=>p.Promotion)
                .Include(p=>p.Details)
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            var result = _mapper.Map<List<BookingDTO>>(bookings);
            return Ok(result);
        }
    }
}
