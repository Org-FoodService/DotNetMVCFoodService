﻿using FoodService.Models.Dto;
using FoodService.Models.Auth.User;
using FoodService.Models;
using FoodService.Util;
using FoodService.HttpRequest.Interface;

namespace FoodService.HttpRequest
{
    /// <summary>
    /// Provides methods for making HTTP requests related to authentication.
    /// </summary>
    public class AuthHttpRequest : BaseHttpRequest<AuthHttpRequest>, IAuthHttpRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthHttpRequest"/> class.
        /// </summary>
        /// <param name="baseUrl">The base URL of the authentication service.</param>
        /// <param name="logger">The logger instance.</param>
        public AuthHttpRequest(string baseUrl, ILogger<AuthHttpRequest> logger) : base(baseUrl, logger)
        {
        }

        /// <summary>
        /// Signs up a new user.
        /// </summary>
        /// <param name="signUpDto">The DTO containing sign-up information.</param>
        /// <returns>A response indicating the success or failure of the operation.</returns>
        public async Task<ResponseCommon<bool>> SignUp(SignUpDto signUpDto)
        {
            try
            {
                _logger.LogInformation("Sign up user...");

                return await PostAsync<bool>("/sign-up", signUpDto);
            }
            catch (Exception ex)
            {
                var errorMessage = "Error occurred while sign up user.";
                _logger.LogError(ex, message: errorMessage);
                return HttpUtils.FailedRequest<bool>(errorMessage, 500);
            }
        }

        /// <summary>
        /// Signs in a user.
        /// </summary>
        /// <param name="signInDto">The DTO containing sign-in information.</param>
        /// <returns>A response containing the Single Sign-On (SSO) token.</returns>
        public async Task<ResponseCommon<bool>> SignIn(SignInDto signInDto)
        {
            try
            {
                _logger.LogInformation("Sign in user...");

                var result = await PostAsync<SsoDto>("/sign-in", signInDto);

                var ssoDto = result.Data;
                AccessTokenManager.Instance.SetAccessToken(ssoDto.AccessToken, ssoDto.Expiration);

                return ResponseCommon<bool>.Success(true);
            }
            catch (Exception ex)
            {
                var errorMessage = "Error occurred while signing user.";
                _logger.LogError(ex, errorMessage);
                return HttpUtils.FailedRequest<bool>(errorMessage, 500);
            }
        }

        /// <summary>
        /// Adds a user to the admin role.
        /// </summary>
        /// <param name="userId">The ID of the user to add to the admin role.</param>
        /// <returns>A response indicating the success or failure of the operation.</returns>
        public async Task<ResponseCommon<bool>> AddUserToAdminRole(int userId)
        {
            try
            {
                return await PostAsync<bool>($"/add-user-to-admin-role?userId={userId}", null);
            }
            catch (Exception ex)
            {
                var errorMessage = "Error occurred while adding user to admin role.";
                _logger.LogError(ex, errorMessage);
                return HttpUtils.FailedRequest<bool>(errorMessage, 500);
            }
        }

        /// <summary>
        /// Retrieves the current user.
        /// </summary>
        /// <returns>A response containing the current user's information.</returns>
        public async Task<ResponseCommon<UserBase>> GetCurrentUser()
        {
            try
            {
                return await GetAsync<UserBase>("/get-current-user");
            }
            catch (Exception ex)
            {
                var errorMessage = "Error occurred while retrieving current user.";
                _logger.LogError(ex, errorMessage);
                return HttpUtils.FailedRequest<UserBase>(errorMessage, 500);
            }
        }

        /// <summary>
        /// Retrieves a user DTO by ID.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <returns>A response containing the user DTO.</returns>
        public async Task<ResponseCommon<UserDto>> GetUserDto(int id)
        {
            try
            {
                return await GetAsync<UserDto>($"/get-userdto?id={id}");
            }
            catch (Exception ex)
            {
                var errorMessage = "Error occurred while retrieving user DTO.";
                _logger.LogError(ex, errorMessage);
                return HttpUtils.FailedRequest<UserDto>(errorMessage, 500);
            }
        }

        /// <summary>
        /// Lists all users.
        /// </summary>
        /// <returns>A response containing a list of users.</returns>
        public async Task<ResponseCommon<List<ClientUser>>> ListUsers()
        {
            try
            {
                return await GetAsync<List<ClientUser>>("/list-users");
            }
            catch (Exception ex)
            {
                var errorMessage = "Error occurred while listing users.";
                _logger.LogError(ex, errorMessage);
                return HttpUtils.FailedRequest<List<ClientUser>>(errorMessage, 500);
            }
        }
    }
}
