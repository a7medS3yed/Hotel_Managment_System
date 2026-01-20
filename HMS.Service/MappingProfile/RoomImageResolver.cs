using AutoMapper;
using AutoMapper.Execution;
using HMS.Core.Entities.RoomModule;
using HMS.Shared.DTOs.RoomDTOs;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Service.MappingProfile
{
    public class RoomImageResolver(IConfiguration configuration) : IValueResolver<Room, RoomDetailsDTO, List<string>>
    {
        public List<string> Resolve(Room source, RoomDetailsDTO destination, List<string> destMember, ResolutionContext context)
        {
            var baseUrl = configuration.GetRequiredSection("URLs")["BaseUrl"];
            return source.RoomImages
                .Select(X => $"{baseUrl}/Images/Rooms/{X.ImageUrl}")
                .ToList();
        }
    }
}
