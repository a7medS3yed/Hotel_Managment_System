using HMS.Shared.DTOs.AuthDTOs;
using HMS.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.ServiceAbstraction
{
    public interface IAuthService
    {
        Task<GenericResponse<UserDto>> RegisterGuestAsync(RegisterDto registerDto);
        Task<GenericResponse<UserDto>> LoginAsync(LoginDto loginDto);
        Task<GenericResponse<string>> RefreshTokenAsync(string refreshToken);
        Task<GenericResponse<bool>> CreateStaffUserAsync(StaffUserDto staffUser);
        Task<GenericResponse<IEnumerable<GetUserDto>>> GetAllUserAsync();
        Task<GenericResponse<bool>> ActivateUserAsync(string userId);
        Task<GenericResponse<bool>> DeactivateUserAsync(string userId);
        Task<GenericResponse<bool>> CheckEmailExistsAsync(string email);
        Task<GenericResponse<ProfileUserDto>> UserProfileAsync(string userId);
    }
}
