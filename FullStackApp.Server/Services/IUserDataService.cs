using FullStackApp.Server.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FullStackApp.Server.Services
{
    public interface IUserDataService
    {
        Task<UserDTO> GetUserDetails(ClaimsPrincipal userPrincipal);
        Task<Boolean> ConfirmUser(ClaimsPrincipal userPrincipal, string id);
    }

}
