using ECommerceAPI.Application.Abstractions.Token;
using ECommerceAPI.Application.DTOs;
using ECommerceAPI.Application.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ECommerceAPI.Application.Features.Commands.AppUser.LoginUser;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommandRequest, LoginUserCommandResponse>
{
	readonly UserManager<Domain.Entities.Identity.AppUser> userManager;
	readonly SignInManager<Domain.Entities.Identity.AppUser> signInManager;
	readonly ITokenHandler tokenHandler;

	public LoginUserCommandHandler(UserManager<Domain.Entities.Identity.AppUser> userManager, SignInManager<Domain.Entities.Identity.AppUser> signInManager, ITokenHandler tokenHandler)
	{
		this.userManager = userManager;
		this.signInManager = signInManager;
		this.tokenHandler = tokenHandler;
	}

	public async Task<LoginUserCommandResponse> Handle(LoginUserCommandRequest request, CancellationToken cancellationToken)
	{
		Domain.Entities.Identity.AppUser user = await userManager.FindByNameAsync(request.UsernameOrEmail);
		if (user == null)
			user = await userManager.FindByEmailAsync(request.UsernameOrEmail);

		if (user == null)
			throw new NotFoundUserException();

		SignInResult result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
		if (result.Succeeded)
		{
			Token token = tokenHandler.CreateAccessToken(5);
			return new LoginUserSuccessCommandResponse() { Token = token };
		}

		//return new LoginUserErrorCommandResponse()
		//{
		//	Message = "Kullanıcı adı veya şifre hatalı..."
		//};

		throw new AuthenticationErrorException();
	}
}
