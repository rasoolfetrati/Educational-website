using LearningWebSite.Core.Services.CourseService;
using Microsoft.AspNetCore.Mvc;

namespace LearningWebSite.ViewComponents;

public class CourseGroupComponent : ViewComponent
{
    private ICourseService _courseService;

    public CourseGroupComponent(ICourseService courseService)
    {
        _courseService = courseService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        return await Task.FromResult((IViewComponentResult)View("CourseGroup", _courseService.GetAllGroupsForLayout()));
    }
}
