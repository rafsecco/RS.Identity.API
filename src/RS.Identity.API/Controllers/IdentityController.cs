using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RS.Identity.API.Extensions;
using RS.Identity.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RS.Identity.API.Controllers;

[ApiController]
[Route("api/identity")]
public class IdentityController : MainController
{
	private readonly SignInManager<IdentityUser> _signInManager;
	private readonly UserManager<IdentityUser> _userManager;
	private readonly AppSettings _appSettings;

	public IdentityController(
		SignInManager<IdentityUser> signInManager,
		UserManager<IdentityUser> userManager,
		IOptions<AppSettings> appSettings
		)
	{
		_signInManager = signInManager;
		_userManager = userManager;
		_appSettings = appSettings.Value;
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

		var result = await _userManager.CreateAsync(user, newUser.Password);

		if (result.Succeeded)
		{
			return CustomResponse(await GenerateJwt(newUser.Email));
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

		var result = await _signInManager.PasswordSignInAsync(userLogin.Email, userLogin.Password, false, true);

		if (result.Succeeded)
		{
			return CustomResponse(await GenerateJwt(userLogin.Email));
		}

		if (result.IsLockedOut)
		{
			AddProcessingError("User temporarily blocked due to invalid attempts");
			return CustomResponse();
		}

		AddProcessingError("Incorrect username or password");
		return CustomResponse();
	}

	#region Private Methods
	private async Task<UserLoginResponse> GenerateJwt(string email)
	{
		var user = await _userManager.FindByEmailAsync(email);
		var claims = await _userManager.GetClaimsAsync(user);

		var identityClaims = await GetUserClaims(claims, user);

		var encodedToken = CodedToken(identityClaims);

		return GetUserLoginResponse(encodedToken, user, claims);
	}

	private static long ToUnixEpochDate(DateTime date)
			=> (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
				.TotalSeconds);

	private async Task<ClaimsIdentity> GetUserClaims(ICollection<Claim> claims, IdentityUser user)
	{
		var userRoles = await _userManager.GetRolesAsync(user);

		claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
		claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
		claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
		claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
		claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));

		foreach (var userRole in userRoles)
		{
			claims.Add(new Claim("role", userRole));
		}

		var identityClaims = new ClaimsIdentity(claims);
		identityClaims.AddClaims(claims);

		return identityClaims;
	}

	private string CodedToken(ClaimsIdentity identityClaims)
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

		var token = tokenHandler.CreateToken(new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
		{
			Issuer = _appSettings.Issuer,
			Audience = _appSettings.Audience,
			Subject = identityClaims,
			Expires = DateTime.UtcNow.AddHours(_appSettings.ExpirationHours),
			SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
		});

		var encodedToken = tokenHandler.WriteToken(token);

		return encodedToken;
	}

	private UserLoginResponse GetUserLoginResponse(string encodedToken, IdentityUser user, IEnumerable<Claim> claims)
	{
		var response = new UserLoginResponse
		{
			AccessToken = encodedToken,
			ExpiresIn = TimeSpan.FromHours(_appSettings.ExpirationHours).TotalSeconds,
			UserToken = new UserToken
			{
				Id = user.Id,
				Email = user.Email,
				Claims = claims.Select(c => new UserClaim { Type = c.Type, Value = c.Value })
			}
		};

		return response;
	}
	#endregion
}
