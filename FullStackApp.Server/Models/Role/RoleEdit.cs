using Microsoft.AspNetCore.Identity;
using FullStackApp.Server.Models.User;

namespace FullStackApp.Server.Models.Role
{
    public class RoleEdit
    {
        public IdentityRole Role { get; set; }
        public IEnumerable<Models.User.User> Members { get; set; }
        public IEnumerable<Models.User.User> NonMembers { get; set; }
    }
}
