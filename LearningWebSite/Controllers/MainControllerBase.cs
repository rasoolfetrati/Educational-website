using LearningWebSite.Core.InfraStructure;
using LearningWebSite.DataLayer.Entities.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LearningWebSite.Areas.Admin.Controllers;

//[Authorize]
public class MainControllerBase : Controller
{
    protected IActionResult RedirectAndShowAlert(OperationResult result, IActionResult redirectPath)
    {
        var model = JsonConvert.SerializeObject(result);
        HttpContext.Response.Cookies.Append("SystemAlert", model);

        return redirectPath;
    }
   
    protected void SuccessAlert()
    {
        var model = JsonConvert.SerializeObject(OperationResult.Success());
        HttpContext.Response.Cookies.Append("SystemAlert", model);
    }
    protected void SuccessAlert(string message)
    {
        var model = JsonConvert.SerializeObject(OperationResult.Success(message));
        HttpContext.Response.Cookies.Append("SystemAlert", model);
    }
    protected void ErrorAlert()
    {
        var model = JsonConvert.SerializeObject(OperationResult.Error());
        HttpContext.Response.Cookies.Append("SystemAlert", model);
    }
    protected void ErrorAlert(string message)
    {
        var model = JsonConvert.SerializeObject(OperationResult.Error(message));
        HttpContext.Response.Cookies.Append("SystemAlert", model);
    }
}