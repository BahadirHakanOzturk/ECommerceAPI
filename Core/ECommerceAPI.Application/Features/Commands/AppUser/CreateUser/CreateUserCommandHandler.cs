using ECommerceAPI.Application.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ECommerceAPI.Application.Features.Commands.AppUser.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommandRequest, CreateUserCommandResponse>
{
	readonly UserManager<Domain.Entities.Identity.AppUser> userManager;

	public CreateUserCommandHandler(UserManager<Domain.Entities.Identity.AppUser> userManager)
	{
		this.userManager = userManager;
	}

	public async Task<CreateUserCommandResponse> Handle(CreateUserCommandRequest request, CancellationToken cancellationToken)
	{
		IdentityResult result = await userManager.CreateAsync(new()
		{
			Id = Guid.NewGuid().ToString(),
			UserName = request.Username,
			Email = request.Email,
			NameSurname = request.NameSurname
		}, request.Password);

		CreateUserCommandResponse response = new() { Succeeded = result.Succeeded };

		if (result.Succeeded)
			response.Message = "Kullanıcı başarıyla oluşturulmuştur.";
		else
			foreach (var error in result.Errors)
				response.Message += $"{error.Code} - {error.Description}\n";

		return response;
		//throw new UserCreateFailedException;
	}
}