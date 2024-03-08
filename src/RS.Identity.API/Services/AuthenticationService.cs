using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RS.Identity.API.Security.Interfaces;
using RS.Identity.API.Security.WebAPI.User;
using RS.Identity.API.Data;
using RS.Identity.API.Extensions;
using RS.Identity.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RS.Identity.API.Services;

public class AuthenticationService
{
	public readonly SignInManager<IdentityUser> SignInManager;
	public readonly UserManager<IdentityUser> UserManager;
	private readonly RSIdentityDbContext _dbContext;
	private readonly AppTokenSettings _appTokenSettingsSettings;
	private readonly IAspNetUser _aspNetUser;
	private readonly IJwtService _jwksService;

	public AuthenticationService(
		SignInManager<IdentityUser> signInManager,
		UserManager<IdentityUser> userManager,
		RSIdentityDbContext dbContext,
		IOptions<AppTokenSettings> appTokenSettingsSettings,
		IAspNetUser aspNetUser,
		IJwtService jwksService
		)
	{
		SignInManager = signInManager;
		UserManager = userManager;
		_dbContext = dbContext;
		_appTokenSettingsSettings = appTokenSettingsSettings.Value;
		_aspNetUser = aspNetUser;
		_jwksService = jwksService;
	}

	public async Task<UserLoginResponse> GenerateJwt(string email)
	{
		var user = await UserManager.FindByEmailAsync(email);
		var claims = await UserManager.GetClaimsAsync(user);
		var identityClaims = await GetUserClaims(claims, user);
		var encodedToken = await CodedToken(identityClaims);
		var refreshToken = await GenerateRefreshToken(email);
		var response = GetUserLoginResponseToken(encodedToken, user, claims, refreshToken);
		return response;
	}

	public async Task<RefreshToken> GetRefreshToken(Guid refreshToken)
	{
		var token = await _dbContext.RefreshTokens.AsNoTracking()
			.FirstOrDefaultAsync(u => u.Token == refreshToken);

		return token != null && token.ExpirationDate.ToLocalTime() > DateTime.Now ? token : null;
	}

	#region Private Methods
	private async Task<ClaimsIdentity> GetUserClaims(ICollection<Claim> claims, IdentityUser user)
	{
		var userRoles = await UserManager.GetRolesAsync(user);

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

	private async Task<string> CodedToken(ClaimsIdentity identityClaims)
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var key = await _jwksService.GetCurrentSigningCredentials();

		var currentIssuer = $"{_aspNetUser.GetHttpContext().Request.Scheme}://{_aspNetUser.GetHttpContext().Request.Host}";

		var token = tokenHandler.CreateToken(new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
		{
			Issuer = currentIssuer,
			Subject = identityClaims,
			Expires = DateTime.UtcNow.AddHours(1),
			SigningCredentials = key
		});

		var encodedToken = tokenHandler.WriteToken(token);

		return encodedToken;
	}

	private UserLoginResponse GetUserLoginResponseToken(string encodedToken, IdentityUser user, IEnumerable<Claim> claims, RefreshToken refreshToken)
	{
		var response = new UserLoginResponse
		{
			AccessToken = encodedToken,
			RefreshToken = refreshToken.Token,
			ExpiresIn = TimeSpan.FromHours(1).TotalSeconds,
			UserToken = new UserToken
			{
				Id = user.Id,
				Email = user.Email,
				Claims = claims.Select(c => new UserClaim { Type = c.Type, Value = c.Value })
			}
		};

		return response;
	}

	private async Task<RefreshToken> GenerateRefreshToken(string email)
	{
		var refreshToken = new RefreshToken
		{
			Username = email,
			ExpirationDate = DateTime.UtcNow.AddHours(_appTokenSettingsSettings.RefreshTokenExpiration)
		};

		_dbContext.RefreshTokens.RemoveRange(_dbContext.RefreshTokens.Where(u => u.Username == email));
		await _dbContext.RefreshTokens.AddAsync(refreshToken);

		await _dbContext.SaveChangesAsync();

		return refreshToken;
	}

	private static long ToUnixEpochDate(DateTime date) =>
		(long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
		.TotalSeconds);
	#endregion

}
