using HMS.Core.Entities.SecurityModul;
using HMS.Core.Entities.SecurityModule;
using HMS.ServiceAbstraction;
using HMS.Shared.DTOs.AuthDTOs;
using HMS.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<HotelUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<HotelUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<GenericResponse<UserDto>> RegisterGuestAsync(RegisterDto registerDto)
        {
            var response = new GenericResponse<UserDto>();

            if (registerDto is null)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = "Invalid registration data.";
                return response;
            }

            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);

            if (existingUser is not null)
            {
                response.StatusCode = StatusCodes.Status409Conflict;
                response.Message = "User with this email already exists.";
                return response;
            }

            var newUser = new HotelUser
            {
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                UserName = registerDto.Email.Split('@')[0],
                PhoneNumber = registerDto.PhoneNumber,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createUserResult = await _userManager.CreateAsync(newUser, registerDto.Password);
            await _userManager.AddToRoleAsync(newUser, "Guest");

            if (!createUserResult.Succeeded)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = string.Join("|",
                    createUserResult.Errors.Select(e => e.Description));
            }

            else
            {
                var accessToken = await GenerateJwtTokenAsync(newUser);
                var refreshToken = GenerateRefreshToken();

                newUser.RefreshToken = refreshToken;
                newUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

                await _userManager.UpdateAsync(newUser);

                response.StatusCode = StatusCodes.Status201Created;
                response.Message = "User registered successfully.";
                response.Data = new UserDto
                {
                    FullName = newUser.FullName,
                    Email = newUser.Email,
                    Token = accessToken,
                    RefreshToken = refreshToken
                };
            }
            return response;
        }
        public async Task<GenericResponse<UserDto>> LoginAsync(LoginDto loginDto)
        {
            var response = new GenericResponse<UserDto>();

            if (loginDto is null)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = "Invalid login data.";
                return response;
            }

            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                response.StatusCode = StatusCodes.Status401Unauthorized;
                response.Message = "Invalid email or password.";
                return response;
            }

            if (!user.IsActive)
            {
                response.StatusCode = StatusCodes.Status403Forbidden;
                response.Message = "Your account has been deactivated, please contact the adminstration";
                return response;
            }

            var accessToken = await GenerateJwtTokenAsync(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _userManager.UpdateAsync(user);


            response.StatusCode = StatusCodes.Status200OK;
            response.Message = "Login successful.";
            response.Data = new UserDto
            {
                FullName = user.FullName,
                Email = user.Email!,
                Token = accessToken,
                RefreshToken = refreshToken
            };

            return response;
        }

        private async Task<string> GenerateJwtTokenAsync(HotelUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim("Activity", user.IsActive.ToString()),
                
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60), // Access Token lifetime
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }

        public async Task<GenericResponse<string>> RefreshTokenAsync(string refreshToken)
        {
            var response = new GenericResponse<string>();

            var user = _userManager.Users
                .FirstOrDefault(u => u.RefreshToken == refreshToken);

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                response.StatusCode = StatusCodes.Status401Unauthorized;
                response.Message = "Invalid or expired refresh token.";
                return response;
            }

            var newAccessToken = await GenerateJwtTokenAsync(user);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _userManager.UpdateAsync(user);

            response.StatusCode = StatusCodes.Status200OK;
            response.Data = newAccessToken;
            response.Message = "Token refreshed successfully.";

            return response;
        }

        public async Task<GenericResponse<bool>> CreateStaffUserAsync(StaffUserDto staffUser)
        {
            var response = new GenericResponse<bool>();

            if (staffUser is null)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = "Invalid staff user data.";
                return response;
            }

            var existingUser = await _userManager.FindByEmailAsync(staffUser.Email);

            if (existingUser is not null)
            {
                response.StatusCode = StatusCodes.Status409Conflict;
                response.Message = "User with this email already exists.";
                return response;
            }

            var resultOfParsing = Enum.TryParse(staffUser.specialities,true, out StaffSpecialities speciality);

            if (!resultOfParsing)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = $"Invalid speciality: {staffUser.specialities}";
                return response;
            }

            var newStaffUser = new StaffUser
            {
                CreatedAt = DateTime.UtcNow,
                FullName = staffUser.FullName,
                Email = staffUser.Email,
                UserName = staffUser.Email.Split('@')[0],
                PhoneNumber = staffUser.PhoneNumber,
                IsActive = true,
                specialities = speciality,
            };

            var createUserResult = await _userManager.CreateAsync(newStaffUser, staffUser.Password);

            await _userManager.AddToRoleAsync(newStaffUser, "Sttaf");

            if (!createUserResult.Succeeded)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = string.Join("|",
                    createUserResult.Errors.Select(e => e.Description));
                response.Data = false;
            }
            else
            {
                response.StatusCode = StatusCodes.Status201Created;
                response.Message = "Staff user created successfully.";
                response.Data = true;
            }
            return response;
        }

        public async Task<GenericResponse<IEnumerable<GetUserDto>>> GetAllUserAsync()
        {
            var response = new GenericResponse<IEnumerable<GetUserDto>>();

            var users = await _userManager.Users.ToListAsync();

            if (users is null || users.Count == 0)
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "No users found.";
                response.Data = Enumerable.Empty<GetUserDto>();
                return response;
            }

            var userDtos = new List<GetUserDto>();
            foreach (var user in users )
            {
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    continue;
                }
                var role = await _userManager.GetRolesAsync(user);
                var userDto = new GetUserDto
                {
                    Email = user.Email!,
                    Id = user.Id,
                    IsActive = user.IsActive,
                    UserName = user.UserName!,
                    Role = role.FirstOrDefault()!,
                };
                userDtos.Add(userDto);
            }
            response.StatusCode = StatusCodes.Status200OK;
            response.Message = "Users retrieved successfully.";
            response.Data = userDtos;
            return response;

        }

        public async Task<GenericResponse<bool>> ActivateUserAsync(string userId)
        {
            var response = new GenericResponse<bool>();

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "User not found.";
                response.Data = false;
                return response;
            }

            user.IsActive = true;

           var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = "Failed to activate user.";
                response.Data = false;
                return response;
            }

            response.StatusCode = StatusCodes.Status200OK;
            response.Message = "User activated successfully.";
            response.Data = true;
            return response;
        }

        public async Task<GenericResponse<bool>> DeactivateUserAsync(string userId)
        {
            var response = new GenericResponse<bool>();

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "User not found.";
                response.Data = false;
                return response;
            }

            user.IsActive = false;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = "Failed to activate user.";
                response.Data = false;
                return response;
            }

            response.StatusCode = StatusCodes.Status200OK;
            response.Message = "User activated successfully.";
            response.Data = true;
            return response;
        }

        public async Task<GenericResponse<bool>> CheckEmailExistsAsync(string email)
        {
            var response = new GenericResponse<bool>();

            var user = await _userManager.FindByEmailAsync(email);

            response.StatusCode = StatusCodes.Status200OK;
            response.Message = "Email existence check completed.";
            response.Data = user is not null;

            return response;

        }

        public async Task<GenericResponse<ProfileUserDto>> UserProfileAsync(string userId)
        {
           var response = new GenericResponse<ProfileUserDto>();
           
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "User not found.";
                response.Data = null;
                return response;
            }

            var profileDto = new ProfileUserDto
            {
                Email = user.Email!,
                UserName = user.UserName!,
                PhoneNumber = user.PhoneNumber!,
                FullName = user.FullName!,
            };

            response.StatusCode = StatusCodes.Status200OK;
            response.Message = "User profile retrieved successfully.";
            response.Data = profileDto;

            return response;
        }
    }
}