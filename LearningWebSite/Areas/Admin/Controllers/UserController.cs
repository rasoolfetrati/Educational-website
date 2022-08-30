using LearningWebSite.Areas.User.Controllers;
using LearningWebSite.Core.InfraStructure;
using LearningWebSite.Core.Services;
using LearningWebSite.Core.ViewModel.Users;
using LearningWebSite.DataLayer.Entities.User;
using LearningWebSite.DataLayer.Entities.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LearningWebSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AutoValidateAntiforgeryToken]
    public class UserController : AdminControllerBase
    {
        private readonly UserManager<CustomUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IUserService userService;
        public UserController(UserManager<CustomUser> userManager, RoleManager<IdentityRole> roleManager, IUserService userService)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.userService = userService;
        }

        public IActionResult Index()
        {
            ViewBag.Model = userService.GetAllUsers();
            return View();
        }
        public IActionResult AddUser()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddUser(UserAddingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            await userService.AddUser(model);
            return RedirectAndShowAlert(OperationResult.Success("افزودن کاربر با موفقیت انجام شد."), RedirectToAction(nameof(Index)));
        }
        [HttpGet]
        public IActionResult EditUser(string id)
        {
            var data = userService.GetUserById(id);
            UserEditViewModel model = new UserEditViewModel()
            {
                Email = data.Email,
                Id = data.Id,
                IsActive = data.EmailConfirmed,
                LastName = data.LastName,
                FirstName = data.FirstName,
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(UserEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            userService.UpdateUser(model);
            return RedirectAndShowAlert(OperationResult.Success("ویرایش کاربر با موفقیت انجام شد."), RedirectToAction(nameof(Index)));
        }
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            await userManager.DeleteAsync(user);
            return RedirectAndShowAlert(OperationResult.Success("حذف کاربر با موفقیت انجام شد."), RedirectToAction(nameof(Index)));
        }

        [Route("Role")]
        public IActionResult RoleIndex()
        {
            ViewBag.Model2 = userService.GetAllUsers();
            return View();
        }
        [Route("RoleList/{id}")]
        [HttpGet]
        public async Task<IActionResult> RoleList(string id)
        {

            var user = await userManager.FindByIdAsync(id);
            var list = await userManager.GetRolesAsync(user);
            ViewBag.Id = id;
            ViewBag.UserName = user.Email;
            return View(list);
        }
        [Route("CreateRole/{id}")]
        [HttpGet]
        public async Task<IActionResult> CreateRole(string id)
        {
            await CreateRoles();
            var claims = from Claimtype c in Enum.GetValues(typeof(Claimtype))
                         select new { ID = (int)c, Name = c.ToString() };
            var list = new SelectList(claims, "ID", "Name", "1");
            ViewBag.Claims = list;
            ViewBag.UserId = id;
            var user = await userManager.FindByIdAsync(id);
            ViewBag.UserEmail = user.Email;
            return View();
        }
        [Route("CreateRole/{id}")]
        [HttpPost]
        public async Task<IActionResult> CreateRole(string id, string RoleName)
        {
            var claims = from Claimtype c in Enum.GetValues(typeof(Claimtype))
                         select new { ID = (int)c, Name = c.ToString() };
            var list = new SelectList(claims, "ID", "Name", "1");
            SelectListItem[] items = list.ToArray();
            SelectListItem selectedItem = items.FirstOrDefault(i => i.Value == RoleName)
                ?? items[0];
            string selectedText = selectedItem.Text;
            var user = await userManager.FindByIdAsync(id);
            await userManager.AddToRoleAsync(user, selectedText);
            return RedirectToAction(nameof(RoleList), new { id = user.Id });
        }
        [Route("EditRole/{userId}/{RoleName}")]
        [HttpGet]
        public async Task<IActionResult> EditRole(string userId, string RoleName)
        {
            var claims = from Claimtype c in Enum.GetValues(typeof(Claimtype))
                         select new { ID = (int)c, Name = c.ToString() };
            ViewBag.UserRole = new SelectList(claims, "ID", "Name", claims.First(r => r.Name == RoleName).ID);
            var user = await userManager.FindByIdAsync(userId);
            ViewBag.UserEmail = user.Email;
            ViewBag.UserId = user.Id;
            return View();
        }
        [Route("EditRole/{userId}/{RoleName}")]
        [HttpPost]
        public async Task<IActionResult> EditRole(string userId, string RoleName, bool? isOk)
        {
            var claims = from Claimtype c in Enum.GetValues(typeof(Claimtype))
                         select new { ID = (int)c, Name = c.ToString() };
            var list = new SelectList(claims, "ID", "Name", "1");
            SelectListItem[] items = list.ToArray();
            SelectListItem selectedItem = items.FirstOrDefault(i => i.Value == RoleName)
                ?? items[0];
            var user = await userManager.FindByIdAsync(userId);
            var currentRole = await userManager.GetRolesAsync(user);
            await userManager.RemoveFromRoleAsync(user, currentRole.First().ToString());
            await userManager.AddToRoleAsync(user, selectedItem.Text.ToString());
            return RedirectToAction(nameof(RoleList), new { id = user.Id });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteRole(string Id)
        {
            var user = await userManager.FindByIdAsync(Id);
            var currentRole = await userManager.GetRolesAsync(user);
            await userManager.RemoveFromRoleAsync(user, currentRole.First().ToString());
            return RedirectToAction(nameof(RoleList), new { id = user.Id });
        }
        private async Task CreateRoles()
        {
            bool x = await roleManager.RoleExistsAsync("Admin");
            if (!x)
            {
                // first we create Admin rool    
                var role = new IdentityRole();
                role.Name = "Admin";
                await roleManager.CreateAsync(role);
            }

            // creating Creating Manager role     
            x = await roleManager.RoleExistsAsync("Teacher");
            if (!x)
            {
                var role = new IdentityRole();
                role.Name = "Teacher";
                await roleManager.CreateAsync(role);
            } 

            // creating Creating Employee role     
            x = await roleManager.RoleExistsAsync("Student");
            if (!x)
            {
                var role = new IdentityRole();
                role.Name = "Student";
                await roleManager.CreateAsync(role);
            }
        }
    }
}
