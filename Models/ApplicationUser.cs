using Microsoft.AspNetCore.Identity;

namespace CryptoStashIdentity.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string? Apn { get; set; } // Apple device's push notification identifier

    }
}
