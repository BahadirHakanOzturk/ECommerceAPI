using ECommerceAPI.Application.Abstractions.Services;
using ECommerceAPI.Application.DTOs.User;
using ECommerceAPI.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace ECommerceAPI.Persistence.Services;

public class UserService : IUserService
{
	readonly UserManager<AppUser> userManager;

	public UserService(UserManager<AppUser> userManager)
	{
		this.userManager = userManager;
	}

	public async Task<CreateUserResponse> CreateAsync(CreateUser model)
	{
		IdentityResult result = await userManager.CreateAsync(new()
		{
			Id = Guid.NewGuid().ToString(),
			UserName = model.Username,
			Email = model.Email,
			NameSurname = model.NameSurname
		}, model.Password);

		CreateUserResponse response = new() { Succeeded = result.Succeeded };

		if (result.Succeeded)
			response.Message = "Kullanıcı başarıyla oluşturulmuştur.";
		else
			foreach (var error in result.Errors)
				response.Message += $"{error.Code} - {error.Description}\n";

		return response;
	}
}
