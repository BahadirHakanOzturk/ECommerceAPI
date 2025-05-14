using ECommerceAPI.Application.Features.Commands.AppUser.LoginUser;
using ECommerceAPI.Application.Features.Commands.AppUser.RefreshTokenLogin;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		readonly IMediator mediator;

		public AuthController(IMediator mediator)
		{
			this.mediator = mediator;
		}

		[HttpPost("[action]")]
		public async Task<IActionResult> Login(LoginUserCommandRequest loginUserCommandRequest)
		{
			LoginUserCommandResponse response = await mediator.Send(loginUserCommandRequest);
			return Ok(response);
		}

		[HttpPost("[action]")]
		public async Task<IActionResult> RefreshTokenLogin([FromBody]RefreshTokenLoginCommandRequest refreshTokenLoginCommandRequest)
		{
			RefreshTokenLoginCommandResponse response = await mediator.Send(refreshTokenLoginCommandRequest);
			return Ok(response);
		}
	}
}
