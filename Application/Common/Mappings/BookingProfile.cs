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
                .ReverseMap();

            // Ánh xạ từ CreateBookingCommand -> Booking
            CreateMap<CreateBookingCommand, Booking>()
                .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.Details))
                .ReverseMap();

            // Ánh xạ từ BookingItem -> BookingDetail, tự động chuyển BeginAt và EndAt sang UTC
            CreateMap<BookingItem, BookingDetail>()
                .ForMember(dest => dest.BeginAt, opt => opt.MapFrom(src => src.BeginAt.HasValue ? src.BeginAt.Value.ToUniversalTime() : (DateTimeOffset?)null))
                .ForMember(dest => dest.EndAt, opt => opt.MapFrom(src => src.EndAt.HasValue ? src.EndAt.Value.ToUniversalTime() : (DateTimeOffset?)null))
                .ForMember(dest => dest.Booking, opt => opt.Ignore()); // Ignore vì Booking sẽ được gán sau

            // Ánh xạ ngược từ BookingDetail -> BookingItem (nếu cần)
            CreateMap<BookingDetail, BookingItem>()
                .ForMember(dest => dest.CourtName, opt => opt.MapFrom(src => src.Court != null ? src.Court.Name : null))
                .ForMember(dest => dest.BeginAt, opt => opt.MapFrom(src => src.BeginAt.HasValue ? src.BeginAt.Value.ToOffset(TimeSpan.FromHours(7)) : (DateTimeOffset?)null))
                .ForMember(dest => dest.EndAt, opt => opt.MapFrom(src => src.EndAt.HasValue ? src.EndAt.Value.ToOffset(TimeSpan.FromHours(7)) : (DateTimeOffset?)null))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.TimeSlot.StartTime))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.TimeSlot.EndTime));
        }
    }

}
