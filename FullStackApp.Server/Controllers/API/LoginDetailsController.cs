using Azure.Identity;
using FullStackApp.Server.Models;
using FullStackApp.Server.Models.User;
using FullStackApp.Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FullStackApp.Server.Controllers.API
{


    [Route("api/[controller]")]
    [ApiController]
    public class LoginDetailsController : ControllerBase
    {
       
        private readonly IUserDataService _userDataService;
        public LoginDetailsController(IUserDataService userDataService)
        {
            _userDataService = userDataService;
        }


        [HttpGet]
        [Route("GetUserDetails")]
        public async Task<IActionResult> GetUserDetails()

        {

            var userDetails = await _userDataService.GetUserDetails(HttpContext.User);
            if (userDetails != null)
            {
                
                /* Returns an object with user details
                {
                 Username: 
                 Roles:
                 UserId:
                }
                */
                return Ok(userDetails);
            }
            return BadRequest();
        }


    }
}
