#region Usings
using LearningWebSite.Core.Identity;
using LearningWebSite.Core.InfraStructure;
using LearningWebSite.Core.Services;
using LearningWebSite.Core.Services.BasketService;
using LearningWebSite.Core.Services.BotService;
using LearningWebSite.Core.Services.CommentService;
using LearningWebSite.Core.Services.ContactUsService;
using LearningWebSite.Core.Services.CourseService;
using LearningWebSite.Core.Services.DiscountService;
using LearningWebSite.Core.Services.WalletService;
using LearningWebSite.DataLayer.Context;
using LearningWebSite.DataLayer.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
#endregion

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

#region SQL
builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
#endregion
#region IoC
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFactorService, FactorService>();
builder.Services.AddScoped<IViewRenderService, RenderViewToString>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IBasketService, BasketService>();
builder.Services.AddScoped<ICommentServices, CommentServices>();
builder.Services.AddScoped<IContactUsService, ContactUsService>();
builder.Services.AddScoped<IDiscountService, DiscountService>();
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
#endregion
#region CookieConfig
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login";
    options.LogoutPath = "/Logout";
    options.AccessDeniedPath = new PathString("/AccessDenied");
    options.ExpireTimeSpan = TimeSpan.FromDays(3);
    options.Cookie.HttpOnly = true;
});
#endregion
#region TelBot
//var botConfig = builder.Configuration.GetSection("BotConfiguration").Get<BotConfiguration>();

//builder.Services.AddHostedService<ConfigureWebhook>();
//builder.Services.AddHttpClient("LearningWebSite")
//    .AddTypedClient<ITelegramBotClient>(httpClient => new TelegramBotClient(botConfig.BotToken, httpClient));

////Dummy business-logic service
//builder.Services.AddScoped<HandleUpdateService>();
#endregion

//===================APP=======================

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

#region ExeptionHandlers
app.Use(async (context, next) =>
{
    await next.Invoke();
    if (context.Response.StatusCode == 404)
    {
        context.Response.Redirect("/NotFound");
    }
});
#endregion
app.UseHttpsRedirection();
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
