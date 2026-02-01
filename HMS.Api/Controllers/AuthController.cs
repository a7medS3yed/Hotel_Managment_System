using HMS.ServiceAbstraction;
using HMS.Shared.DTOs.AuthDTOs;
using HMS.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HMS.Api.Controllers
{
    public class AuthController : BaseApiController
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")] // api/auth/register
        public async Task<ActionResult<GenericResponse<UserDto>>> Register([FromBody] RegisterDto registerDto)
        {
            var result = await _authService.RegisterGuestAsync(registerDto);
            return HandleResponse(result);
        }

        [HttpPost("login")] // api/auth/login
        public async Task<ActionResult<GenericResponse<UserDto>>> Login([FromBody] LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            return HandleResponse(result);

        }

        [HttpPost("refresh-token")] // api/auth/refresh-token
        public async Task<ActionResult<GenericResponse<string>>> RefreshToken([FromBody] string refreshToken)
        {
            var result = await _authService.RefreshTokenAsync(refreshToken);
            return HandleResponse(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create-staff")] // api/auth/create-staff
        public async Task<ActionResult<GenericResponse<bool>>> CreateStaffUser([FromBody] StaffUserDto staffUser)
        {
            var result = await _authService.CreateStaffUserAsync(staffUser);
            return HandleResponse(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("users")] // api/auth/users
        public async Task<ActionResult<GenericResponse<IEnumerable<GetUserDto>>>> GetAllUsers()
        {
            var result = await _authService.GetAllUserAsync();
            return HandleResponse(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("users/{userId}/activate")] // api/auth/{userId}/activate
        public async Task<ActionResult<GenericResponse<bool>>> ActivateUser([FromRoute] string userId)
        {
            var result = await _authService.ActivateUserAsync(userId);
            return HandleResponse(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("users/{userId}/deactivate")] // api/auth/{userId}/deactivate
        public async Task<ActionResult<GenericResponse<bool>>> DeactivateUser([FromRoute] string userId)
        {
            var result = await _authService.DeactivateUserAsync(userId);
            return HandleResponse(result);
        }

        [HttpGet("emailExists")] // api/auth/check-email?email=
        public async Task<ActionResult<GenericResponse<bool>>> CheckEmailExists([FromQuery] string email)
        {
            var result = await _authService.CheckEmailExistsAsync(email);
            return HandleResponse(result);
        }

        [Authorize]
        [HttpGet("profile")] // api/auth/profile
        public async Task<ActionResult<GenericResponse<ProfileUserDto>>> UserProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _authService.UserProfileAsync(userId!);
            return HandleResponse(result);
        }

    }
    }
