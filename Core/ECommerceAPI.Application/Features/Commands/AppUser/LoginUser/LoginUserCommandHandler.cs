using ECommerceAPI.Application.Abstractions.Services;
using ECommerceAPI.Application.DTOs;
using MediatR;

namespace ECommerceAPI.Application.Features.Commands.AppUser.LoginUser;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommandRequest, LoginUserCommandResponse>
{

	readonly IAuthService authService;

	public LoginUserCommandHandler(IAuthService authService)
	{
		this.authService = authService;
	}

	public async Task<LoginUserCommandResponse> Handle(LoginUserCommandRequest request, CancellationToken cancellationToken)
	{
		Token token = await authService.LoginAsync(request.UsernameOrEmail, request.Password, 15);

		return new LoginUserSuccessCommandResponse() { Token = token };
	}
}
