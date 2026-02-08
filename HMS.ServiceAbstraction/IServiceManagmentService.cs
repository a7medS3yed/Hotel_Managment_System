using HMS.Shared.DTOs.ServieDTOs;
using HMS.Shared.Responses;
using HMS.Shared.SharedEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.ServiceAbstraction
{
    public interface IServiceManagmentService
    {
        Task<GenericResponse<bool>> CreateAsync(CreateServiceRequestDto dto, string guestId);
        Task<GenericResponse<IEnumerable<ServiceRequestDto>>> GetMyRequestsAsync(string guestId);
        Task<GenericResponse<bool>> AssignStaffAsync(int requestId, string staffId);
        Task<GenericResponse<bool>> UpdateStatusAsync(int requestId, ServiceRequestStatus status, string staffId);

        Task<GenericResponse<ServiceDto>> CreateAsync(CreateServiceDto dto);
        Task<GenericResponse<IEnumerable<ServiceDto>>> GetAllAsync();
        Task<GenericResponse<bool>> ToggleStatusAsync(int id);
    }
}
