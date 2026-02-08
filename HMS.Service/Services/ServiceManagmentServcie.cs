using AutoMapper;
using HMS.Core.Contracts;
using HMS.Core.Entities.ServiceModule;
using HMS.ServiceAbstraction;
using HMS.Shared.DTOs.ServieDTOs;
using HMS.Shared.Responses;
using HMS.Shared.SharedEnums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Service.Services
{
    public class ServiceManagmentServcie : IServiceManagmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ServiceManagmentServcie(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GenericResponse<bool>> AssignStaffAsync(int requestId, string staffId)
        {
            var response = new GenericResponse<bool>();

            var repo = _unitOfWork.Repository<ServiceRequest, int>();
            var request = await repo.GetByIdAsync(requestId);

            if (request == null)
            {
                response.StatusCode = 404;
                response.Message = "Service request not found.";
                response.Data = false;
                return response;
            }

            request.StaffId = staffId;
            request.Status = Core.Entities.ServiceModule.ServiceRequestStatus.Assigned;
            request.UpdatedAt = DateTime.Now;

            var result = await _unitOfWork.SaveChangesAsync() > 0;
            if (result)
            {
                response.StatusCode = 200;
                response.Message = "Staff assigned successfully.";
                response.Data = true;
            }
            else
            {
                response.StatusCode = 500;
                response.Message = "Failed to assign staff.";
                response.Data = false;
            }
            return response;
        }
        public async Task<GenericResponse<bool>> CreateAsync(CreateServiceRequestDto dto, string guestId)
        {
             var response = new GenericResponse<bool>();

            var services = await _unitOfWork.Repository<Core.Entities.ServiceModule.Service, int>()
                                    .GetByIdAsync(dto.ServiceId);

            if (services == null || !services.IsActive)
            {
                response.StatusCode = 404;
                response.Message = "Service not found or inactive.";
                response.Data = false;
                return response;
            }

            var request = new ServiceRequest
            {
                ServiceId = dto.ServiceId,
                GuestId = guestId,
                Notes = dto.Notes,
                Status = Core.Entities.ServiceModule.ServiceRequestStatus.Pending
            };

            await _unitOfWork.Repository<ServiceRequest, int>().AddAsync(request);
            var result = await _unitOfWork.SaveChangesAsync() > 0;

            if (result)
            {
                response.StatusCode = 201;
                response.Message = "Service request created successfully.";
                response.Data = true;
            }
            else
            {
                response.StatusCode = 500;
                response.Message = "Failed to create service request.";
                response.Data = false;
            }
            return response;
        }
        public async Task<GenericResponse<bool>> UpdateStatusAsync(int requestId, Shared.SharedEnums.ServiceRequestStatus status, string staffId)
        {
            var response = new GenericResponse<bool>();

            var repo = _unitOfWork.Repository<ServiceRequest, int>();
            var request = await repo.GetByIdAsync(requestId);

            if (request == null || request.StaffId != staffId)
            {
                response.StatusCode = 404;
                response.Message = "Service request not found or unauthorized.";
                response.Data = false;
                return response;
            }

            request.Status = (Core.Entities.ServiceModule.ServiceRequestStatus)status;
            request.UpdatedAt = DateTime.Now;
            var result = await _unitOfWork.SaveChangesAsync() > 0;

            if (result)
            {
                response.StatusCode = 200;
                response.Message = "Service request status updated successfully.";
                response.Data = true;
            }
            else
            {
                response.StatusCode = 500;
                response.Message = "Failed to update service request status.";
                response.Data = false;
            }
            return response;
        }
        public async Task<GenericResponse<ServiceDto>> CreateAsync(CreateServiceDto dto)
        {
            var response = new GenericResponse<ServiceDto>();

            var service = _mapper.Map<Core.Entities.ServiceModule.Service>(dto);

            if (service == null)
            {
                response.StatusCode = 400;
                response.Message = "Invalid service data.";
                response.Data = null;
                return response;
            }

            await _unitOfWork.Repository<Core.Entities.ServiceModule .Service, int>().AddAsync(service);
            var result = await _unitOfWork.SaveChangesAsync() > 0;

            if (result)
            {
                response.StatusCode = 201;
                response.Message = "Service created successfully.";
                response.Data = _mapper.Map<ServiceDto>(service);

            }
            else
            {
                response.StatusCode = 500;
                response.Message = "Failed to create service.";
                response.Data = null;
            }
            return response;
        }

        public async Task<GenericResponse<IEnumerable<ServiceDto>>> GetAllAsync()
        {
           var response = new GenericResponse<IEnumerable<ServiceDto>>();

            var services = await _unitOfWork.Repository<Core.Entities.ServiceModule.Service, int>().GetAllAsync();

            var serviceDtos = _mapper.Map<IEnumerable<ServiceDto>>(services);

            response.StatusCode = 200;
            response.Message = "Services retrieved successfully.";
            response.Data = serviceDtos;
            return response;
        }

        public async Task<GenericResponse<IEnumerable<ServiceRequestDto>>> GetMyRequestsAsync(string guestId)
        {
            var response = new GenericResponse<IEnumerable<ServiceRequestDto>>();

            var requests = await _unitOfWork.Repository<ServiceRequest, int>()
                                    .GetAllAsync(r => r.GuestId == guestId);

            var requestDtos = _mapper.Map<IEnumerable<ServiceRequestDto>>(requests);

            response.StatusCode = 200;
            response.Message = "Service requests retrieved successfully.";
            response.Data = requestDtos;
            return response;

        }

        public async Task<GenericResponse<bool>> ToggleStatusAsync(int id)
        {
           var response = new GenericResponse<bool>();
            var repo = _unitOfWork.Repository<Core.Entities.ServiceModule.Service, int>();
            var service = await repo.GetByIdAsync(id);
            if (service == null)
            {
                response.StatusCode = 404;
                response.Message = "Service not found.";
                response.Data = false;
                return response;
            }
            service.IsActive = !service.IsActive;
            service.UpdatedAt = DateTime.Now;
            var result = await _unitOfWork.SaveChangesAsync() > 0;
            if (result)
            {
                response.StatusCode = 200;
                response.Message = "Service status toggled successfully.";
                response.Data = true;
            }
            else
            {
                response.StatusCode = 500;
                response.Message = "Failed to toggle service status.";
                response.Data = false;
            }
            return response;
        }


    }
}
