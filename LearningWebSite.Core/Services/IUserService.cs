using LearningWebSite.Core.InfraStructure;
using LearningWebSite.Core.ViewModel.CourseVM;
using LearningWebSite.Core.ViewModel.Users;
using LearningWebSite.DataLayer.Context;
using LearningWebSite.DataLayer.Entities.Users;
using LearningWebSite.DataLayer.Entities.UserWallet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TopLearn.Core.Convertors;

namespace LearningWebSite.Core.Services;

public interface IUserService
{
    void UpdateUser(UserEditViewModel userEditView);
    List<CustomUser> GetAllUsers();
    CustomUser GetUserByActiveCode(string code);
    CustomUser GetUserById(string id);
    void changeActiveCode(string Id);
    CustomUser SignUpUser(RegisterViewModel registerViewModel);
    SideBarViewModel GetSideBarView(string username);
    Task updateUserAsync(string username, UserViewModel userViewModel, IFormFile image);
    Task<bool> ResetPasswordUserAsync(UserViewModel userViewModel, string username);
    Task AddUser(UserAddingViewModel userAddingViewModel);
    string GetUserId(string username);
    int GetUserCourses(string username);
    int WalletBalance(string username);
    List<GetUserCourseViewModel> getUserCourses(string userName);
    bool IsUserInCourse(int courseId, string username);
}

public class UserService : IUserService
{
    private readonly ApplicationDbContext context;
    private readonly UserManager<CustomUser> userManager;

    public UserService(ApplicationDbContext context, UserManager<CustomUser> userManager)
    {
        this.context = context;
        this.userManager = userManager;
    }

    public async Task AddUser(UserAddingViewModel userAddingViewModel)
    {
        var user = new CustomUser()
        {
            ActiveCode = GenerateActiveCode.GenerateCode(),
            Avatar = "Default.png",
            Email = userAddingViewModel.Email,
            EmailConfirmed = userAddingViewModel.IsActive,
            Gender = DataLayer.Entities.User.Gender.Unknown,
            UserName = userAddingViewModel.Email,
            FirstName = userAddingViewModel.FirstName,
            LastName = userAddingViewModel.LastName
        };

        await userManager.CreateAsync(user, userAddingViewModel.Password);

        if (userAddingViewModel.Claim == DataLayer.Entities.User.Claimtype.Admin)
        {
            await userManager.AddClaimAsync(user, new Claim("AdminType", "Admin"));
            await userManager.AddToRoleAsync(user, "Admin");
        }
        if (userAddingViewModel.Claim == DataLayer.Entities.User.Claimtype.Teacher)
        {
            await userManager.AddClaimAsync(user, new Claim("TeacherType", "Teacher"));
            await userManager.AddToRoleAsync(user, "Teacher");
        }
        if (userAddingViewModel.Claim == DataLayer.Entities.User.Claimtype.Student)
        {
            await userManager.AddClaimAsync(user, new Claim("StudentType", "Student"));
            await userManager.AddToRoleAsync(user, "Student");
        }
        await Task.CompletedTask;
    }

    public void changeActiveCode(string Id)
    {
        var data = context.Users.Find(Id);
        data.ActiveCode = GenerateActiveCode.GenerateCode();
        context.Users.Update(data);
        context.SaveChanges();
    }

    public List<CustomUser> GetAllUsers()
    {
        return context.Users
            .Select(
                w =>
                    new CustomUser()
                    {
                        Id = w.Id,
                        FirstName = w.FirstName,
                        LastName = w.LastName,
                        EmailConfirmed = w.EmailConfirmed,
                        Email = w.Email
                    }
            )
            .ToList();
    }

    public SideBarViewModel GetSideBarView(string username)
    {
        var user = userManager.FindByNameAsync(username).Result;
        return new SideBarViewModel()
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            ImageName = user.Avatar
        };
    }

    public CustomUser GetUserByActiveCode(string code)
    {
        return context.Users.SingleOrDefault(c => c.ActiveCode == code);
    }

    public CustomUser GetUserById(string id)
    {
        return context.Users.Find(id);
    }

    public int GetUserCourses(string username)
    {
        return context.UserInCourses.Where(u => u.UserName == username).Count();
    }

    public List<GetUserCourseViewModel> getUserCourses(string userName)
    {
        var userCourses = context.UserInCourses
            .Include(u => u.User)
            .Include(c => c.Course)
            .Where(u => u.UserName == userName)
            .Select(
                u =>
                    new GetUserCourseViewModel()
                    {
                        CourseId = u.CourseId,
                        CourseTitle = u.Course.CourseTitle,
                        CourseImageName = u.Course.CourseImageName
                    }
            )
            .ToList();

        return userCourses;
    }

    public string GetUserId(string username)
    {
        var user = userManager.FindByNameAsync(username).Result;
        return user.Id;
    }

    public bool IsUserInCourse(int courseId, string username)
    {
        return context.UserInCourses.Any(c => c.CourseId == courseId && c.UserName == username);
    }

    public async Task<bool> ResetPasswordUserAsync(UserViewModel userViewModel, string username)
    {
        var user = await userManager.FindByNameAsync(username);
        string token = await userManager.GeneratePasswordResetTokenAsync(user);
        await userManager.ResetPasswordAsync(user, token, userViewModel.Password);
        return true;
    }

    public CustomUser SignUpUser(RegisterViewModel registerViewModel)
    {
        var user = new CustomUser()
        {
            Email = registerViewModel.Email.FixEmail(),
            EmailConfirmed = false,
            Avatar = "Default.png",
            FirstName = registerViewModel.FirstName,
            LastName = registerViewModel.LastName,
            Gender = DataLayer.Entities.User.Gender.Unknown,
            UserName = registerViewModel.Email.FixEmail(),
            ActiveCode = GenerateActiveCode.GenerateCode(),
        };
        return user;
    }

    public void UpdateUser(UserEditViewModel userEditView)
    {
        var user = userManager.FindByIdAsync(userEditView.Id).Result;
        user.Id = userEditView.Id;
        user.Email = userEditView.Email;
        user.EmailConfirmed = userEditView.IsActive;
        user.FirstName = userEditView.FirstName;
        user.LastName = userEditView.LastName;
        context.Users.Update(user);
        context.SaveChanges();
    }

    public async Task updateUserAsync(
        string username,
        UserViewModel userViewModel,
        IFormFile image
    )
    {
        var user = await userManager.FindByNameAsync(username);
        user.Id = userViewModel.Id;
        user.FirstName = userViewModel.FirstName;
        user.LastName = userViewModel.LastName;
        if (image != null && image.IsImage())
        {
            try
            {
                if (user.Avatar != "Default.png")
                {
                    string deleteimagePath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/userAvatar",
                        user.Avatar
                    );
                    if (File.Exists(deleteimagePath))
                    {
                        File.Delete(deleteimagePath);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            user.Avatar = userViewModel.Avatar =
                NameGenerator.GenerateUniqCode() + Path.GetExtension(image.FileName);
            string imagePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/userAvatar",
                userViewModel.Avatar
            );

            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }
        }
        context.Users.Update(user);
        await context.SaveChangesAsync();
    }

    public int WalletBalance(string username)
    {
        var enter = context.Factors
            .Where(
                u =>
                    u.Username == username
                    && u.IsPay
                    && u.UserOperationType == UserOperationType.Charge
            )
            .Sum(c => c.Amount);
        var spend = context.Factors
            .Where(
                u =>
                    u.Username == username
                    && u.IsPay
                    && u.UserOperationType == UserOperationType.Collect
            )
            .Sum(c => c.Amount);
        if (spend>enter)
        {
            return 0;
        }
        return (enter - spend);
    }
}
