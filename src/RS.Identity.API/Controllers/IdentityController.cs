using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RS.Identity.API.Models;
using RS.Identity.API.Services;

namespace RS.Identity.API.Controllers;

[ApiController]
[Route("api/identity")]
public class IdentityController : MainController
{
	private readonly AuthenticationService _authenticationService;

	public IdentityController(AuthenticationService authenticationService)
	{
		_authenticationService = authenticationService;
	}

	[HttpPost("new-user")]
	public async Task<ActionResult> NewUser(User newUser)
	{
		if (!ModelState.IsValid) return CustomResponse(ModelState);

		var user = new IdentityUser
		{
			UserName = newUser.Email,
			Email = newUser.Email,
			EmailConfirmed = true
		};

		var result = await _authenticationService.UserManager.CreateAsync(user, newUser.Password);

		if (result.Succeeded)
		{
			return CustomResponse(await _authenticationService.GenerateJwt(newUser.Email));
		}

		foreach (var error in result.Errors)
		{
			AddProcessingError(error.Description);
		}

		return CustomResponse();
	}

	[HttpPost("login")]
	public async Task<ActionResult> Login(UserLogin userLogin)
	{
		if (!ModelState.IsValid) return CustomResponse(ModelState);

		var result = await _authenticationService.SignInManager.PasswordSignInAsync(userLogin.Email, userLogin.Password, false, true);

		if (result.Succeeded)
		{
			return CustomResponse(await _authenticationService.GenerateJwt(userLogin.Email));
		}

		if (result.IsLockedOut)
		{
			AddProcessingError("User temporarily blocked due to invalid attempts");
			return CustomResponse();
		}

		AddProcessingError("Incorrect username or password");
		return CustomResponse();
	}

	[HttpPost("refresh-token")]
	public async Task<ActionResult> RefreshToken([FromBody] string refreshToken)
	{
		if (string.IsNullOrEmpty(refreshToken))
		{
			AddProcessingError("Refresh Token invalid");
			return CustomResponse();
		}

		var token = await _authenticationService.GetRefreshToken(Guid.Parse(refreshToken));

		if (token is null)
		{
			AddProcessingError("Refresh Token expired");
			return CustomResponse();
		}

		return CustomResponse(await _authenticationService.GenerateJwt(token.Username));
	}
}
