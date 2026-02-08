using AutoMapper;
using HMS.Core.Entities.ServiceModule;
using HMS.Shared.DTOs.ServieDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Service.MappingProfile
{
    public class ServiceRequestProfile : Profile
    {
        public ServiceRequestProfile()
        {
            CreateMap<ServiceRequest, ServiceRequestDto>()
                .ForMember(d => d.ServiceName,
                    o => o.MapFrom(s => s.Service.Name))
                .ForMember(d => d.Status,
                    o => o.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.StaffName,
                    o => o.MapFrom(s => s.Staff != null ? s.Staff.FullName : null));
        }
    }

}
