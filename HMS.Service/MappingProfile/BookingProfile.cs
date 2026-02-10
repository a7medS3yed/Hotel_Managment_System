using AutoMapper;
using HMS.Core.Entities.BookingModule;
using HMS.Shared.DTOs.BookingDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Service.MappingProfile
{
    internal class BookingProfile : Profile
    {
        public BookingProfile()
        {
            CreateMap<Booking, BookingDto>()
                .ForMember(
                    dest => dest.GuestFullName,
                    opt => opt.MapFrom(src => src.Guest.FullName)
                )
                .ForMember(
                    dest => dest.GuestEmail,
                    opt => opt.MapFrom(src => src.Guest.Email)
                );

            CreateMap<Booking, MyBookingDto>();
                
                
        }
    }
}
