using LearningWebSite.Core.Identity;
using LearningWebSite.Core.InfraStructure;
using LearningWebSite.Core.Services;
using LearningWebSite.Core.Services.BasketService;
using LearningWebSite.Core.Services.CommentService;
using LearningWebSite.Core.Services.CourseService;
using LearningWebSite.Core.Services.WalletService;
using LearningWebSite.Core.ViewModel;
using LearningWebSite.DataLayer.Context;
using LearningWebSite.DataLayer.Entities.Users;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    option.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFactorService, FactorService>();
builder.Services.AddScoped<IViewRenderService, RenderViewToString>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IBasketService, BasketService>();
builder.Services.AddScoped<ICommentServices, CommentServices>();
builder.Services.AddIdentity<CustomUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = true;
}).AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddErrorDescriber<PersianIdentityErrorDescriber>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = new PathString("/Account/Login");
    options.LogoutPath = new PathString("/Account/Logout");
    options.AccessDeniedPath = new PathString("/AccessDenied");
    options.Cookie.Expiration = TimeSpan.FromDays(3);
    options.Cookie.HttpOnly = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/NotFound");
}
app.Use(async (context, next) =>
{
    await next.Invoke();
    if (context.Response.StatusCode == 404)
    {
        context.Response.Redirect("/NotFound");
    }
});
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
    endpoints.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
});


app.Run();
