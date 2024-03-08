using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace RS.Identity.API.Security.WebAPI.User;

public class AspNetUser : IAspNetUser
{
	private readonly IHttpContextAccessor _contextAccessor;

	public AspNetUser(IHttpContextAccessor contextAccessor)
	{
		_contextAccessor = contextAccessor;
	}

	public string Name => _contextAccessor.HttpContext.User.Identity.Name;

	public IEnumerable<Claim> GetClaims()
	{
		return _contextAccessor.HttpContext.User.Claims;
	}

	public HttpContext GetHttpContext()
	{
		return _contextAccessor.HttpContext;
	}

	public string GetUserEmail()
	{
		return IsAuthenticated() ? _contextAccessor.HttpContext.User.GetUserEmail() : "";
	}

	public Guid GetUserId()
	{
		return IsAuthenticated() ? Guid.Parse(_contextAccessor.HttpContext.User.GetUserId()) : Guid.Empty;
	}

	public string GetUserRefreshToken()
	{
		return IsAuthenticated() ? _contextAccessor.HttpContext.User.GetUserRefreshToken() : "";
	}

	public string GetUserToken()
	{
		return IsAuthenticated() ? _contextAccessor.HttpContext.User.GetUserToken() : "";
	}

	public bool HasRole(string role)
	{
		return _contextAccessor.HttpContext.User.IsInRole(role);
	}

	public bool IsAuthenticated()
	{
		return _contextAccessor.HttpContext.User.Identity.IsAuthenticated;
	}
}
