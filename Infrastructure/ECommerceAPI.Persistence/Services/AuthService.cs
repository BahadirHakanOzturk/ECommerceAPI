using ECommerceAPI.Application.Abstractions.Services;
using ECommerceAPI.Application.Abstractions.Token;
using ECommerceAPI.Application.DTOs;
using ECommerceAPI.Application.Exceptions;
using ECommerceAPI.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Persistence.Services;

public class AuthService : IAuthService
{
	readonly UserManager<AppUser> userManager;
	readonly SignInManager<AppUser> signInManager;
	readonly ITokenHandler tokenHandler;
	readonly IUserService userService;

	public AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenHandler tokenHandler, IUserService userService)
	{
		this.userManager = userManager;
		this.signInManager = signInManager;
		this.tokenHandler = tokenHandler;
		this.userService = userService;
	}

	public async Task<Token> LoginAsync(string usernameOrEmail, string password, int accessTokenLifeTime)
	{
		AppUser user = await userManager.FindByNameAsync(usernameOrEmail);
		if (user == null)
			user = await userManager.FindByEmailAsync(usernameOrEmail);

		if (user == null)
			throw new NotFoundUserException();

		SignInResult result = await signInManager.CheckPasswordSignInAsync(user, password, false);
		if (result.Succeeded)
		{
			Token token = tokenHandler.CreateAccessToken(accessTokenLifeTime);
			await userService.UpdateRefreshTokenAsync(token.RefreshToken, user, token.Expiration, 5);
			return token;
		}

		throw new AuthenticationErrorException();
	}

	public async Task<Token> RefreshTokenLoginAsync(string refreshToken)
	{
		AppUser? user = await userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
		if (user != null && user?.RefreshTokenExpireDate > DateTime.UtcNow)
		{
			Token token = tokenHandler.CreateAccessToken(15);
			await userService.UpdateRefreshTokenAsync(token.RefreshToken, user, token.Expiration, 5);
			return token;
		}
		else
			throw new NotFoundUserException();
	}
}
