using CryptoStashIdentity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Duende.IdentityServer.IdentityServerConstants;

namespace CryptoStashIdentity.Controllers;

[Route("Users/{userId}/Apn")]
[ApiController]
[Authorize(LocalApi.PolicyName)]
public class UsersApnController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersApnController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    // GET: /Users/5/Apn
    [HttpGet]
    public async Task<ActionResult<string>> GetUsersApn(string userId)
    {
        var owner = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (owner != null)
        {
            return Forbid();
        }

        var user = await _userManager.FindByIdAsync(userId);

        if(user == null)
        {
            return NotFound();
        }

        return user.Apn ?? "";
    }

    // PUT: /Users/5/Apn
    [HttpPut]
    public async Task<IActionResult> PutUsersApn(string userId, ApnBody body)
    {
        var owner = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        if(owner != null && userId != owner)
        {
            return Forbid();
        }

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        user.Apn = body.Apn;
        await _userManager.UpdateAsync(user);

        return NoContent();
    }
}

public class ApnBody
{
    public string Apn { get; set; }
}
