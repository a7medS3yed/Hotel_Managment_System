using AutoMapper;
using HMS.Core.Entities.RoomModule;
using HMS.Shared.DTOs.RoomDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Service.MappingProfile
{
    internal class RoomProfile : Profile
    {
        public RoomProfile()
        {
            CreateMap<Room, RoomDTO>();

            CreateMap<Room, RoomDetailsDTO>()
           .ForMember(
               dest => dest.Iamges,
               opt => opt.MapFrom(src => src.RoomImages
                                             .Select(img => img.ImageUrl)
                                             .ToList())
           );
        }
    }
}
