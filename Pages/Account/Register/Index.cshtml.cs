using CryptoStashIdentity.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace CryptoStashIdentity.Pages.Register;

[SecurityHeaders]
[Authorize]
public class IndexModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IIdentityServerInteractionService _interaction;

    public IndexModel(UserManager<ApplicationUser> userManager,
        IIdentityServerInteractionService interaction)
    {
        _userManager = userManager;
        _interaction = interaction;
    }

    public ViewModel View { get; set; }

    [BindProperty]
    public InputModel Input { get; set; }

    public async Task<IActionResult> OnGet()
    {
        if (!User.HasClaim(JwtClaimTypes.Role, "admin"))
        {
            return RedirectToPage("/Error/Index");
        }
        
        BuildModel();
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        if (ModelState.IsValid)
        {
            if(Input.Password == Input.ConfirmPassword)
            {
                var newEmail = _userManager.FindByEmailAsync(Input.Email).Result;
                var newUser = _userManager.FindByNameAsync(Input.Username).Result;
                if (newUser == null && newEmail == null)
                {
                    newUser = new ApplicationUser
                    {
                        UserName = Input.Username,
                        Email = Input.Email,
                        EmailConfirmed = true,
                    };
                    var result = await _userManager.CreateAsync(newUser, Input.Password);
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    var claims = new List<Claim>();
                    if(Input.GivenName != "") claims.Add(new Claim(JwtClaimTypes.GivenName, Input.GivenName));
                    if(Input.FamilyName != "") claims.Add(new Claim(JwtClaimTypes.FamilyName, Input.FamilyName));
                    if(Input.GivenName != "" && Input.FamilyName != "") claims.Add(new Claim(JwtClaimTypes.Name, String.Concat(Input.GivenName, " ", Input.FamilyName)));

                    var result2 = await _userManager.AddClaimsAsync(newUser, claims.ToArray());
                    if (!result2.Succeeded)
                    {
                        throw new Exception(result2.Errors.First().Description);
                    }

                    return Redirect("~/");
                }
            }
        }

        // something went wrong, show form with error
        //BuildModel();
        View.RegistrationFailed = true;
        return Page();
    }

    private void BuildModel()
    {
        Input = new InputModel();
        View = new ViewModel();
    }
}
