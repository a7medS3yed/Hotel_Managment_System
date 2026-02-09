using AutoMapper;
using HMS.Shared.DTOs.ServieDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Service.MappingProfile
{
    public class ServiceProfile : Profile
    {
        public ServiceProfile()
        {
            CreateMap<Core.Entities.ServiceModule.Service, ServiceDto>();
            CreateMap<CreateServiceDto, Core.Entities.ServiceModule.Service>();
        }
    }
}
