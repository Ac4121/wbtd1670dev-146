using FullStackApp.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FullStackApp.Server.Controllers.API
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class KeysController : ControllerBase
    {

        // GET: api/Keys/GetGmailKeys
        [HttpGet]
        [Route("GetGmailKeys")]
        public async Task<IActionResult> GetGmailKeys()

        {
            DotNetEnv.Env.Load();
            var GMAIL_API_KEY = Environment.GetEnvironmentVariable("GMAIL_API_KEY");
            var GOOGLE_CLIENT_ID = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");


            var response = new CustomHttpResponse2();
            response.response["GMAIL_API_KEY"] = GMAIL_API_KEY;
            response.response["GOOGLE_CLIENT_ID"] = GOOGLE_CLIENT_ID;


            return Ok(response);
        }
    }
}

