using System.ComponentModel.DataAnnotations;

namespace CryptoStashIdentity.Pages.Register;

public class InputModel
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
    [Required]
    public string ConfirmPassword { get; set; }
    public string GivenName { get; set; }
    public string FamilyName { get; set; }

    public string Button { get; set; }
}
