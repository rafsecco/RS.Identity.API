using Microsoft.AspNetCore.Mvc;
using RS.Identity.API.Models;

namespace RS.Identity.API.Controllers;

[ApiController]
//[Route("api/identity")]
[Route("api/[controller]")]
public class IdentityController : MainController
{

	private readonly ILogger<IdentityController> _logger;

	public IdentityController(ILogger<IdentityController> logger)
	{
		_logger = logger;
	}

	[HttpPost("new-user")]
	public Task<ActionResult> NewUser(User newUser)
	{
		return Task.FromResult<ActionResult>(Ok());
	}
}
