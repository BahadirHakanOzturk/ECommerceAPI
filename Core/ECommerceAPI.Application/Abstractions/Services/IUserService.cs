using ECommerceAPI.Application.DTOs.User;
using ECommerceAPI.Domain.Entities.Identity;

namespace ECommerceAPI.Application.Abstractions.Services;

public interface IUserService
{
	Task<CreateUserResponse> CreateAsync(CreateUser model);
	Task UpdateRefreshTokenAsync(string refreshToken, AppUser user, DateTime accessTokenExpireDate, int addOnAccessTokenDate);
}
