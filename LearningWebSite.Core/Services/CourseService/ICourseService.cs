using LearningWebSite.Core.ViewModel.CourseVM;
using LearningWebSite.DataLayer.Entities.Courses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LearningWebSite.Core.Services.CourseService;

public interface ICourseService
{
    bool IsexistCourse(int courseId);
    List<CourseGroups> GetAllGroups();
    List<CourseGroups> GetAllGroupsForLayout();
    List<SelectListItem> GetCourseGroups();
    List<SelectListItem> GetTeachers();
    List<SelectListItem> GetCourseSubGroups(int groupId);
    Task CreateGroup(CourseGroups courseGroups);
    void UpdateGroup(CourseGroups courseGroups);
    void DeleteGroup(int groupId);
    CourseGroups GetGroup(int groupId);
    Task CreateCourse(CourseViewModel courseViewModel);
    ShowCourseViewModelWithIndex GetAllCourseForAdmin(string courseName, int PageIndex = 1);
    List<CourseEpisode> GetAllCourseEpisodes(int courseId);
    Task CreateEpisode(CourseEpisode courseEpisode, IFormFile file);
    bool CheckExistFile(string fileName);
    void UpdateCourse(Course courseView, IFormFile imgCourseUp, IFormFile demoUp);
    Task<Course> GetCourseById(int courseId);
    Task<CourseEpisode> GetCourseEpisodeById(int episodId);
    Task UpdateEpisode(CourseEpisode courseEpisode, IFormFile file);
    Task DeleteEpisode(int episodeId);
    bool IsEpisodeExist(int id);
    bool IsCourseExist(int id);
    int GetEpisodeCount(int courseId);
    void DeleteCourse(int courseId);
    List<CourseIndexViewModel> GetCoursesForIndex();
    List<Course> GetDeleteCourses();
    ShowCourseViewModel showCourseViewModel(int id, string slug);
    string GetTeacherName(int courseId);
    TimeSpan GetTimeSpan(int courseId);
    CourseEpisode GetEpisodeById(int episodeId);
    Tuple<List<ShowCourseListItemViewModel>, int> GetCourse(
        int pageId = 1,
        string filter = "",
        string getType = "all",
        string sort = "lates",
        List<int> selectedGroups = null,
        int take = 0
    );
    List<ShowCourseListItemViewModel> GetTeacherCourses(string TeacherId);
    Task AddSource(int courseId, IFormFile source);
    Task<IReadOnlyList<CourseIndexViewModel>> GetRecommendedCourses();
    Task<IReadOnlyList<CourseIndexViewModel>> GetBestSellerCourses();
}
