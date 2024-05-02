﻿using FoodService.Core.Dto;
using FoodService.Core.Interface.Repository;
using FoodService.Core.Interface.Service;
using FoodService.Data.Model.Auth.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FoodService.Core.Service
{
    /// <summary>
    /// Service implementation for authentication-related operations.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<UserBase> _userManager;

        /// <summary>
        /// Initializes a new instance of the AuthService class.
        /// </summary>
        public AuthService(
            IUserRepository userRepository,
            IConfiguration configuration,
            UserManager<UserBase> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        /// <summary>
        /// Retrieves a list of all users.
        /// </summary>
        public async Task<List<ApplicationUser>> ListUsers()
        {
            List<ApplicationUser> listUsers = await _userRepository.ListAll().ToListAsync();
            return listUsers;
        }

        /// <summary>
        /// Retrieves a user by their ID.
        /// </summary>
        public async Task<ApplicationUser> GetUserById(int userId)
        {
            ApplicationUser user = await _userRepository.GetByIdAsync(userId);
            return user ?? throw new ArgumentException("User does not exist.");
        }

        /// <summary>
        /// Retrieves a user DTO by their ID.
        /// </summary>
        public async Task<UserDto> GetUserDto(int userId)
        {
            var user = await this.GetUserById(userId);
            UserDto userDto = new(user);
            return userDto;
        }

        /// <summary>
        /// Updates a user.
        /// </summary>
        public async Task<int> UpdateUser(ApplicationUser user)
        {
            ApplicationUser findUser = await _userRepository.GetByIdAsync(user.Id) ?? throw new ArgumentException("User not found.");
            findUser.Email = user.Email;
            findUser.UserName = user.UserName;
            return await _userRepository.UpdateAsync(findUser);
        }

        /// <summary>
        /// Deletes a user by their ID.
        /// </summary>
        public async Task<bool> DeleteUser(int userId)
        {
            ApplicationUser findUser = await _userRepository.GetByIdAsync(userId) ?? throw new ArgumentException("User not found.");
            await _userRepository.DeleteAsync(findUser);
            return true;
        }

        /// <summary>
        /// Signs up a new user.
        /// </summary>
        public async Task<bool> SignUp(SignUpDto signUpDto)
        {
            ApplicationUser? userExists = await _userManager.FindByNameAsync(signUpDto.Username) as ApplicationUser;
            if (userExists != null)
                throw new ArgumentException("Username already exists");

            userExists = await _userManager.FindByEmailAsync(signUpDto.Email) as ApplicationUser;
            if (userExists != null)
                throw new ArgumentException("Email already exists");

            ApplicationUser user = new()
            {
                CpfCnpj = signUpDto.CpfCnpj,
                Email = signUpDto.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = signUpDto.Username,
                PhoneNumber = signUpDto.PhoneNumber,
            };

            var result = await _userManager.CreateAsync(user, signUpDto.Password);

            if (!result.Succeeded)
                if (result.Errors.ToList().Count > 0)
                    throw new ArgumentException(result.Errors.ToList()[0].Description);
                else
                    throw new ArgumentException("User registration failed.");

            // If this is the first user, add admin role
            var isFirstUser = await _userManager.Users.CountAsync() == 1;
            if (isFirstUser)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, "Admin");
                if (!roleResult.Succeeded)
                {
                    throw new ArgumentException("Failed to add user to admin role.");
                }
            }
            return true;
        }

        /// <summary>
        /// Adds a user to the administrator role.
        /// </summary>
        [Authorize(Roles = "Admin")]
        public async Task AddUserToAdminRole(int userId)
        {
            await AddUserToRoleAsync(userId, "Admin");
        }

        private async Task AddUserToRoleAsync(int userId, string roleName)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId!.ToString()) as ApplicationUser;
            if (user == null)
                throw new ArgumentException("User not found.");
            await _userManager.AddToRoleAsync(user, roleName);
        }

        /// <summary>
        /// Signs in a user.
        /// </summary>
        public async Task<SsoDto> SignIn(SignInDto signInDto)
        {
            var user = await _userManager.FindByNameAsync(signInDto.Username);
            if (user == null)
                throw new ArgumentException("User not found.");

            if (!await _userManager.CheckPasswordAsync(user, signInDto.Password))
                throw new ArgumentException("Invalid password.");

            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.UtcNow.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new SsoDto(new JwtSecurityTokenHandler().WriteToken(token), user);
        }

        /// <summary>
        /// Retrieves the currently authenticated user.
        /// </summary>
        public async Task<UserBase> GetCurrentUser()
        {
            UserBase user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            return user!;
        }
    }
}
