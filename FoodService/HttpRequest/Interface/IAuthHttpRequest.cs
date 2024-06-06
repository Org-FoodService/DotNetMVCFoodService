﻿using FoodService.Models.Dto;
using FoodService.Models.Auth.User;
using FoodService.Models.Responses;

namespace FoodService.HttpRequest.Interface
{
    public interface IAuthHttpRequest
    {
        Task<ResponseCommon<bool>> AddUserToAdminRole(int userId);
        Task<ResponseCommon<UserBase>> GetCurrentUser();
        Task<ResponseCommon<UserDto>> GetUserDto(int id);
        Task<ResponseCommon<List<ClientUser>>> ListUsers();
        Task<ResponseCommon<SsoDto>> SignIn(SignInDto signInDto);
        Task<ResponseCommon<bool>> SignUp(SignUpDto signUpDto);
    }
}