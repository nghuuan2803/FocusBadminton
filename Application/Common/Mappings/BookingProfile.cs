using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Features.Bookings.Commands;
using AutoMapper;
using Shared.Bookings;

namespace Application.Common.Mappings
{
    public class BookingProfile : Profile
    {
        public BookingProfile()
        {
            // Ánh xạ từ Booking -> BookingDTO
            CreateMap<Booking, BookingDTO>()
                .ForMember(dest => dest.MemberName, opt => opt.MapFrom(src => src.Member != null ? src.Member.FullName : null))
                .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Team != null ? src.Team.Name : null))
                .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.Details))
                .ReverseMap(); // Nếu cần ánh xạ ngược
            CreateMap<CreateBookingCommand, Booking>()
                .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.Details))
                .ReverseMap(); // Nếu cần ánh xạ ngược
            // Ánh xạ từ BookingDetail -> BookingItem
            CreateMap<BookingDetail, BookingItem>()
                .ForMember(dest => dest.CourtName, opt => opt.MapFrom(src => src.Court != null ? src.Court.Name : null))
                .ReverseMap()
                .ForMember(dest => dest.Court, opt => opt.Ignore());
        }
    }

}
