using Microsoft.AspNetCore.Identity;

namespace FullStackApp.Server.Models.User
{
    public class User : IdentityUser
    //IdentityUser.cs class provides it with some of the properties
    //like the user name, e-mail, phone number, password hash, role memberships
    {
        public string? CustomTag { get; set; }
        /*
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        */
    }
}
