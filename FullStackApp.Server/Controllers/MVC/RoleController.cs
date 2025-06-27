using FullStackApp.Server.Models.Role;
using FullStackApp.Server.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FullStackApp.Server.Controllers.MVC
{
    [Authorize(Roles = "Admin")]
    [Route("staff/[controller]")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // GET: api/Role/Index
        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var roles = _roleManager.Roles.ToList();

            // getting users per role
            List<string> usersInRoleList = new List<string>();

            if (roles != null)
            {
                foreach (IdentityRole role in roles)
                {

                    {
                        var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
                        if (usersInRole == null || usersInRole.Count == 0)
                        {
                            usersInRoleList.Add("No users in role");
                        }
                        else
                            usersInRoleList.Add(string.Join(", ", usersInRole));
                    }
                }

                // combine the Identity list with RoleUser list
                var combined = roles.Zip(usersInRoleList, (a, b) => new { rolelist = a, userlist = b });
                return View(combined);
            }
            else
                return View();
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return View(new IdentityRole());
        }
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(IdentityRole role)
        {
            await _roleManager.CreateAsync(role);
            return RedirectToAction("Index");
        }


        // A page to add users to a role for update

        [HttpGet]
        [Route("UpdateRoleUsers")]
        public async Task<IActionResult> UpdateRoleUsers(string id)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            List<User> members = new List<User>();
            List<User> nonMembers = new List<User>();
            foreach (User user in _userManager.Users)
            {
                var list = await _userManager.IsInRoleAsync(user, role.Name) ? members : nonMembers;
                list.Add(user);
            }
            return View(new RoleEdit
            {
                Role = role,
                Members = members,
                NonMembers = nonMembers
            });
        }

        [HttpPost]
        [Route("UpdateRoleUsers")]
        public async Task<IActionResult> UpdateRoleUsers(RoleModification model)
        {
            IdentityResult result;

            // If the checkbox input has elem name 'AddIds' or 'DeleteIds'
            if (ModelState.IsValid)
            {
                foreach (string userId in model.AddIds ?? new string[] { })
                {
                    User user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        result = await _userManager.AddToRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                            ModelState.AddModelError("", "Cannot add to role");
                    }
                }
                foreach (string userId in model.DeleteIds ?? new string[] { })
                {
                    User user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {

                        if ((user.UserName == "Bg1234@gmail.com") && (model.RoleName == "Admin"))
                            ModelState.AddModelError("", "Cannot remove Bg1234 from admin role");
                        else
                        {
                            result = await _userManager.RemoveFromRoleAsync(user, model.RoleName);

                            if (!result.Succeeded)
                                ModelState.AddModelError("", "Cannot remove from role");
                        }
                    }
                }
            }
            ViewData["ModelState"] = ModelState;

            if (ModelState.IsValid)
                return RedirectToAction(nameof(Index));
            else
            {

                return await UpdateRoleUsers(model.RoleId);
            }
        }
    }
}



