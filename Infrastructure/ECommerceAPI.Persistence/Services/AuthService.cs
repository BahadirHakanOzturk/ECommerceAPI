using ECommerceAPI.Application.Abstractions.Services;
using ECommerceAPI.Application.Abstractions.Token;
using ECommerceAPI.Application.DTOs;
using ECommerceAPI.Application.Exceptions;
using ECommerceAPI.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
namespace ECommerceAPI.Persistence.Services;

public class AuthService : IAuthService
{
	readonly UserManager<AppUser> userManager;
	readonly SignInManager<AppUser> signInManager;
	readonly ITokenHandler tokenHandler;

	public AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenHandler tokenHandler)
	{
		this.userManager = userManager;
		this.signInManager = signInManager;
		this.tokenHandler = tokenHandler;
	}

	public async Task<Token> LoginAsync(string usernameOrEmail, string password, int accessTokenLifeTime)
	{
		Domain.Entities.Identity.AppUser user = await userManager.FindByNameAsync(usernameOrEmail);
		if (user == null)
			user = await userManager.FindByEmailAsync(usernameOrEmail);

		if (user == null)
			throw new NotFoundUserException();

		SignInResult result = await signInManager.CheckPasswordSignInAsync(user, password, false);
		if (result.Succeeded)
		{
			Token token = tokenHandler.CreateAccessToken(accessTokenLifeTime);
			return token;
		}

		throw new AuthenticationErrorException();
	}
}
