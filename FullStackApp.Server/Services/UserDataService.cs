using FullStackApp.Server.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Security.Claims;

namespace FullStackApp.Server.Services
{
    public class UserDataService : IUserDataService
    {
  

            private readonly RoleManager<IdentityRole> _roleManager;
            private readonly UserManager<User> _userManager;
            public UserDataService(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
            {
                _roleManager = roleManager;
                _userManager = userManager;
            }

            public async Task<UserDTO> GetUserDetails(ClaimsPrincipal userPrincipal)

            {

                var user = await _userManager.GetUserAsync(userPrincipal);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    var details = new UserDTO(user.UserName, roles, user.Id
                );
                    /* Returns an object with user details
                    {
                     Username: 
                     Roles:
                     UserId:
                    }
                    */
                    return details;
                }
                return null;
            }

        public async Task<Boolean> ConfirmUser(ClaimsPrincipal userPrincipal, string userIdCheck)
        {
            var user = await _userManager.GetUserAsync(userPrincipal);
            if (user != null)
            {
                var userId = user.Id;

                if (userId == userIdCheck)
                {
                    /* Returns an object with user details
                    {
                     Username: 
                     Roles:
                     UserId:
                    }
                    */
                    return true;
                }
                else
                    return false;
            }
            return false;
        }

    
    }
}
