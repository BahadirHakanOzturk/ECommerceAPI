using ECommerceAPI.Application.Abstractions.Services;
using ECommerceAPI.Application.DTOs;
using MediatR;

namespace ECommerceAPI.Application.Features.Commands.AppUser.RefreshTokenLogin;

public class RefreshTokenLoginCommandHandler : IRequestHandler<RefreshTokenLoginCommandRequest, RefreshTokenLoginCommandResponse>
{
	readonly IAuthService authService;

	public RefreshTokenLoginCommandHandler(IAuthService authService)
	{
		this.authService = authService;
	}

	public async Task<RefreshTokenLoginCommandResponse> Handle(RefreshTokenLoginCommandRequest request, CancellationToken cancellationToken)
	{
		Token token = await authService.RefreshTokenLoginAsync(request.RefreshToken);
		return new() { Token = token };
	}
}
