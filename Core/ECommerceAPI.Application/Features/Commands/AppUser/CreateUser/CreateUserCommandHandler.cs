using ECommerceAPI.Application.Abstractions.Services;
using ECommerceAPI.Application.DTOs.User;
using MediatR;

namespace ECommerceAPI.Application.Features.Commands.AppUser.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommandRequest, CreateUserCommandResponse>
{>
	readonly IUserService userService;

	public CreateUserCommandHandler(IUserService userService)
	{
		this.userService = userService;
	}

	public async Task<CreateUserCommandResponse> Handle(CreateUserCommandRequest request, CancellationToken cancellationToken)
	{
		CreateUserResponse response = await userService.CreateAsync(new()
		{
			Email = request.Email,
			NameSurname = request.NameSurname,
			Password = request.Password,
			PasswordConfirm = request.PasswordConfirm,
			Username = request.Username
		});

		return new()
		{
			Message = response.Message,
			Succeeded = response.Succeeded
		};
	}
}