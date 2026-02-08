using HMS.Core.Entities.ServiceModule;
using HMS.ServiceAbstraction;
using HMS.Shared.DTOs.ServieDTOs;
using HMS.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HMS.Api.Controllers
{
    public class ServicesController(IServiceManagmentService serviceManagmentService) : BaseApiController
    {
        [Authorize(Roles = "Guest")]
        [HttpPost("Request")]
        public async Task<ActionResult<GenericResponse<int>>> Create(CreateServiceRequestDto createServiceRequestDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await serviceManagmentService.CreateAsync(createServiceRequestDto, userId!);

            return HandleResponse(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/AssignStaff")]
        public async Task<ActionResult<GenericResponse<bool>>> AssignStaff([FromRoute] int id, [FromQuery] string staffId)
        {
            var result = await serviceManagmentService.AssignStaffAsync(id, staffId);
            return HandleResponse(result);
        }

        [Authorize(Roles = "Staff")]
        [HttpPatch("{id}/Status")]
        public async Task<ActionResult<GenericResponse<bool>>> UpdateStatus([FromRoute] int id, HMS.Shared.SharedEnums.ServiceRequestStatus status)
        {
            var result = await serviceManagmentService.UpdateStatusAsync(id, status, User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return HandleResponse(result);

        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<GenericResponse<int>>> CreateService([FromBody] CreateServiceDto createServiceDto)
        {
            var result = await serviceManagmentService.CreateAsync(createServiceDto);
            return HandleResponse(result);
        }

        [HttpGet("my")]
        public async Task<ActionResult<GenericResponse<IEnumerable<ServiceRequestDto>>>> GetMyServiceRequests()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await serviceManagmentService.GetMyRequestsAsync(userId!);
            return HandleResponse(result);
        }

        [HttpGet]
        public async Task<ActionResult<GenericResponse<IEnumerable<ServiceDto>>>> GetAllService()
        {
            var result = await serviceManagmentService.GetAllAsync();
            return HandleResponse(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/toggle")]
        public async Task<ActionResult<GenericResponse<bool>>> ToggleServiceAvailability([FromRoute] int id)
        {
            var result = await serviceManagmentService.ToggleStatusAsync(id);
            return HandleResponse(result);
        }
    }
}