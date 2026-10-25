using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DomainLayer.Exceptions;
using DomainLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ServiceAbstractionLayer;
using Shared.DataTransferObjects;

namespace ServiceLayer
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        public AuthService(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }
        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                throw new UnauthorizedException();
            }
            return new AuthResponseDto
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = CreateToken(user)
            };
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            // Check if the email is already registered
            if(await _userManager.FindByEmailAsync(registerDto.Email) != null)
            {
                throw new FieldValidationException("email", "An account with this email already exists.");
            }
            
            // Create a new user
            var user = new AppUser
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                UserName = registerDto.Email
            }; 
            
            // Check the password requirements and create the user with the provided password
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            
            // If the password does not meet the requirements, throw a FieldValidationException with the error messages that shows the password requirements
            if(!result.Succeeded)
            {
                var errors = string.Join(" ", result.Errors.Select(e => e.Description));
                throw new FieldValidationException("password", errors);
            }
            
            // Generate a JWT token for the newly registered user
            var token = CreateToken(user);
            return new AuthResponseDto {DisplayName = user.DisplayName , Email = user.Email, Token = token };

        }
        private string CreateToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.DisplayName)
            };
        
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(double.Parse(_configuration["Jwt:DurationInDays"]!)),
                signingCredentials: credentials);
        
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
