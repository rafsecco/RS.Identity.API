using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace RS.Identity.API.Security.WebAPI.User;

public interface IAspNetUser
{
	string Name { get; }
	Guid GetUserId();
	string GetUserEmail();
	string GetUserToken();
	string GetUserRefreshToken();
	bool IsAuthenticated();
	bool HasRole(string role);
	IEnumerable<Claim> GetClaims();
	HttpContext GetHttpContext();
}
