using LearningWebSite.Core.InfraStructure;
using LearningWebSite.Core.ViewModel.CourseVM;
using LearningWebSite.DataLayer.Context;
using LearningWebSite.DataLayer.Entities.Courses;
using LearningWebSite.DataLayer.Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using File = System.IO.File;

namespace LearningWebSite.Core.Services.CourseService;

public class CourseService : ICourseService
{
    private readonly ApplicationDbContext _context;
    private UserManager<CustomUser> _userManager;

    public CourseService(ApplicationDbContext context, UserManager<CustomUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public List<SelectListItem> GetTeachers()
    {
        var users = _userManager
            .GetUsersInRoleAsync("Teacher")
            .Result.Select(
                g =>
                    new SelectListItem() { Value = g.Id, Text = g.FirstName + " " + g.LastName }
            )
            .ToList();
        return users;
    }

    public List<SelectListItem> GetCourseSubGroups(int groupId)
    {
        return _context.CourseGroups
            .Where(p => p.ParentId == groupId)
            .Select(
                g => new SelectListItem() { Value = g.GroupId.ToString(), Text = g.GroupTitle }
            ).AsNoTracking()
            .ToList();
    }

    public async Task CreateGroup(CourseGroups courseGroups)
    {
        await _context.CourseGroups.AddAsync(courseGroups);
        await _context.SaveChangesAsync();
        await Task.CompletedTask;
    }

    public List<SelectListItem> GetCourseGroups()
    {
        return _context.CourseGroups
            .Where(p => p.ParentId == null)
            .Select(
                g => new SelectListItem() { Value = g.GroupId.ToString(), Text = g.GroupTitle }
            ).AsNoTracking()
            .ToList();
    }

    public void UpdateGroup(CourseGroups courseGroups)
    {
        _context.CourseGroups.Update(courseGroups);
        _context.SaveChanges();
    }

    public void DeleteGroup(int groupId)
    {
        var data = _context.CourseGroups.Find(groupId);
        data.IsDelete = true;
        _context.CourseGroups.Update(data);
        _context.SaveChanges();
    }

    public CourseGroups GetGroup(int groupId)
    {
        return _context.CourseGroups.Find(groupId);
    }

    public async Task CreateCourse(CourseViewModel courseViewModel)
    {
        courseViewModel.CourseImageName = "no-photo.jpg";
        courseViewModel.DemoFileName = "no-photo.jpg";
        ImageConvertor imageConverter = new ImageConvertor();

        if (courseViewModel.imgCourseUp != null && ImageValidator.IsImage(courseViewModel.imgCourseUp))
        {
            courseViewModel.CourseImageName =
                NameGenerator.GenerateUniqCode()
                + Path.GetExtension(courseViewModel.imgCourseUp.FileName);
            string imagePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/CourseImage",
                courseViewModel.CourseImageName
            );
            string CompressimagePath = Path.Combine(
             Directory.GetCurrentDirectory(),
             "wwwroot/CourseImage/CompressedImages",
             courseViewModel.CourseImageName
            );
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await courseViewModel.imgCourseUp.CopyToAsync(stream);
            }
            imageConverter.ImageCompress(imagePath, CompressimagePath, 230, 348);
            //thumbpath
            string thumbpath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/CourseImage/thumb",
                courseViewModel.CourseImageName
            );
            imageConverter.Image_resize(imagePath, thumbpath, 150);
        }

        //TODO Upload Demo
        if (courseViewModel.demoUp != null)
        {
            courseViewModel.DemoFileName =
                NameGenerator.GenerateUniqCode()
                + Path.GetExtension(courseViewModel.demoUp.FileName);
            string demoPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/course/demos",
                courseViewModel.DemoFileName
            );

            using (var stream = new FileStream(demoPath, FileMode.Create))
            {
                await courseViewModel.demoUp.CopyToAsync(stream);
            }
        }

        Course course = new Course()
        {
            CourseImageName = courseViewModel.CourseImageName,
            DemoFileName = courseViewModel.DemoFileName,
            CreateDate = DateTime.Now,
            IsDelete = false,
            CourseDescription = courseViewModel.CourseDescription,
            GroupId = courseViewModel.GroupId,
            CourseTitle = courseViewModel.CourseTitle,
            CoursePrice = courseViewModel.CoursePrice,
            TeacherId = courseViewModel.TeacherId,
            SubGroup = courseViewModel.SubGroup,
            courseLevel = courseViewModel.courseLevel,
            courseStatus = courseViewModel.courseStatus,
            Slug = courseViewModel.Slug.Replace(" ", "-"),
            CoursePresentation = courseViewModel.CoursePresentation,
            Tags = courseViewModel.Tags,
            IsRecommended = courseViewModel.IsRecommended,
        };
        await _context.Courses.AddAsync(course);
        await _context.SaveChangesAsync();
        await Task.CompletedTask;
    }

    public ShowCourseViewModelWithIndex GetAllCourseForAdmin(string courseName, int pageIndex = 1)
    {
        ShowCourseViewModelWithIndex showCourseViewModelWithIndex = new ShowCourseViewModelWithIndex();
        int maxRows = 5;

        IQueryable<Course> query = _context.Courses;

        if (!string.IsNullOrWhiteSpace(courseName))
        {
            query = query.Where(c => c.CourseTitle.Contains(courseName));
        }

        List<ShowCourseViewModel> courseViewModels = query
            .Select(c => new ShowCourseViewModel()
            {
                CourseId = c.CourseId,
                CourseImageName = c.CourseImageName,
                CreateDate = c.CreateDate,
                CourseTitle = c.CourseTitle,
                IsDelete = c.IsDelete,
                UpdateDate = c.UpdateDate,
            })
            .OrderBy(c => c.CourseId)
            .Skip((pageIndex - 1) * maxRows)
            .Take(maxRows)
            .AsNoTracking()
            .ToList();

        double pageCount = (double)((decimal)query.Count() / Convert.ToDecimal(maxRows));

        showCourseViewModelWithIndex.showCourseViewModels = courseViewModels;
        showCourseViewModelWithIndex.PageCount = (int)Math.Ceiling(pageCount);
        showCourseViewModelWithIndex.CurrentPageIndex = pageIndex;

        return showCourseViewModelWithIndex;
    }


    public async Task CreateEpisode(CourseEpisode courseEpisode, IFormFile file)
    {
        if (file != null)
        {
            courseEpisode.EpisodeFileName = file.FileName;
            string demoPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/course/Episode",
                courseEpisode.EpisodeFileName
            );

            using (var stream = new FileStream(demoPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
        }
        await _context.AddAsync(courseEpisode);
        await _context.SaveChangesAsync();
        await Task.CompletedTask;
    }

    public bool CheckExistFile(string fileName)
    {
        var path = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot/course/Episode",
            fileName
        );
        if (File.Exists(path))
        {
            return true;
        }
        return false;
    }

    public List<CourseEpisode> GetAllCourseEpisodes(int courseId)
    {
        return _context.CourseEpisodes.Where(c => c.CourseId == courseId).AsNoTracking().ToList();
    }

    public void UpdateCourse(Course courseView, IFormFile imgCourseUp, IFormFile demoUp)
    {
        ImageConvertor imageConverter = new ImageConvertor();
        var originalCourse = _context.Courses.Find(courseView.CourseId);
        courseView.UpdateDate = DateTime.Now;
        if (imgCourseUp != null && ImageValidator.IsImage(imgCourseUp))
        {
            if (courseView.CourseImageName != "no-photo.jpg")
            {
                string deletePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/CourseImage",
                    courseView.CourseImageName
                );
                if (File.Exists(deletePath))
                {
                    File.Delete(deletePath);
                }
                string deletethumbPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/CourseImage/thumb",
                    courseView.CourseImageName
                );
                if (File.Exists(deletethumbPath))
                {
                    File.Delete(deletethumbPath);
                }
                string DeleteCompressimagePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/CourseImage/CompressedImages",
                courseView.CourseImageName
               );
                if (File.Exists(DeleteCompressimagePath))
                {
                    File.Delete(DeleteCompressimagePath);
                }
            }
            originalCourse.CourseImageName =
                NameGenerator.GenerateUniqCode() + Path.GetExtension(imgCourseUp.FileName);
            string imagePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/CourseImage",
                originalCourse.CourseImageName
            );
            string CompressimagePath = Path.Combine(
             Directory.GetCurrentDirectory(),
             "wwwroot/CourseImage/CompressedImages",
             courseView.CourseImageName
            );
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                imgCourseUp.CopyTo(stream);
            }
            imageConverter.ImageCompress(imagePath, CompressimagePath, 230, 348);
            string thumbpath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/CourseImage/thumb",
                originalCourse.CourseImageName
            );
            imageConverter.Image_resize(imagePath, thumbpath, 150);
        }

        if (demoUp != null)
        {
            string deletedemoPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/course/demos",
                courseView.DemoFileName
            );
            if (File.Exists(deletedemoPath))
            {
                File.Delete(deletedemoPath);
            }
            originalCourse.DemoFileName =
                NameGenerator.GenerateUniqCode() + Path.GetExtension(demoUp.FileName);
            string demoPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/course/demos",
                originalCourse.DemoFileName
            );

            using (var stream = new FileStream(demoPath, FileMode.Create))
            {
                demoUp.CopyTo(stream);
            }
        }

        originalCourse.CourseId = courseView.CourseId;
        originalCourse.CoursePrice = courseView.CoursePrice;
        originalCourse.CourseTitle = courseView.CourseTitle;
        originalCourse.CourseDescription = courseView.CourseDescription;
        originalCourse.subGroup = courseView.subGroup;
        originalCourse.CourseGroup = courseView.CourseGroup;
        originalCourse.TeacherId = courseView.TeacherId;
        originalCourse.courseStatus = courseView.courseStatus;
        originalCourse.courseLevel = courseView.courseLevel;
        originalCourse.Slug = courseView.Slug.Replace(" ", "-");
        originalCourse.CoursePresentation = courseView.CoursePresentation;
        originalCourse.Tags = courseView.Tags;
        originalCourse.IsRecommended = courseView.IsRecommended;

        _context.Courses.Update(originalCourse);
        _context.SaveChanges();
    }

    public async Task<Course> GetCourseById(int courseId)
    {
        return await _context.Courses.FindAsync(courseId);
    }

    public async Task<CourseEpisode> GetCourseEpisodeById(int episodId)
    {
        return await _context.CourseEpisodes.FindAsync(episodId);
    }

    public async Task UpdateEpisode(CourseEpisode courseEpisode, IFormFile demoUp)
    {
        if (demoUp != null)
        {
            string deletedemoPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/course/Episode",
                courseEpisode.EpisodeFileName
            );
            if (File.Exists(deletedemoPath))
            {
                File.Delete(deletedemoPath);
            }
            courseEpisode.EpisodeFileName =
                demoUp.FileName + Path.GetExtension(demoUp.FileName);
            string demoPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/course/Episode",
                courseEpisode.EpisodeFileName
            );

            using (var stream = new FileStream(demoPath, FileMode.Create))
            {
                await demoUp.CopyToAsync(stream);
            }
        }
        _context.CourseEpisodes.Update(courseEpisode);
        await _context.SaveChangesAsync();
        await Task.CompletedTask;
    }

    public async Task DeleteEpisode(int episodeId)
    {
        var episode = await _context.CourseEpisodes.FindAsync(episodeId);
        string episodePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot/course/Episode",
            episode.EpisodeFileName
        );
        if (File.Exists(episodePath))
        {
            File.Delete(episodePath);
        }
        _context.CourseEpisodes.Remove(episode);
        await _context.SaveChangesAsync();
        await Task.CompletedTask;
    }

    public bool IsEpisodeExist(int id)
    {
        if (_context.CourseEpisodes.Any(e => e.EpisodeId == id))
        {
            return true;
        }
        return false;
    }

    public int GetEpisodeCount(int courseId)
    {
        return _context.CourseEpisodes.Where(c => c.CourseId == courseId).AsNoTracking().Count();
    }

    public void DeleteCourse(int courseId)
    {
        var course = _context.Courses.Find(courseId);
        course.CourseId = courseId;
        course.IsDelete = true;
        _context.Courses.Update(course);
        _context.SaveChanges();
    }

    public bool IsCourseExist(int id)
    {
        if (_context.Courses.Any(e => e.CourseId == id))
        {
            return true;
        }
        return false;
    }

    public List<CourseIndexViewModel> GetCoursesForIndex()
    {
        return _context.Courses
            .Select(
                c =>
                    new CourseIndexViewModel()
                    {
                        CourseId = c.CourseId,
                        CourseTitle = c.CourseTitle,
                        CoursePrice = c.CoursePrice,
                        CourseImageName = c.CourseImageName,
                        Slug = c.Slug
                    }
            )
            .OrderBy(c => c.CourseId)
            .Take(9)
            .AsNoTracking()
            .ToList();
    }

    public ShowCourseViewModel showCourseViewModel(int id, string slug)
    {
        return _context.Courses
            .Include(c => c.CourseEpisodes)
            .Include(c => c.Comments)
            .Include(c => c.UserCourses)
            .Where(c => c.CourseId == id && c.Slug == slug)
            .Select(
                c =>
                    new ShowCourseViewModel()
                    {
                        Slug = c.Slug,
                        CourseTitle = c.CourseTitle,
                        CourseId = c.CourseId,
                        CourseDescription = c.CourseDescription,
                        CourseImageName = c.CourseImageName,
                        CoursePrice = c.CoursePrice,
                        CreateDate = c.CreateDate,
                        TeacherId = c.TeacherId,
                        GroupId = c.GroupId,
                        UpdateDate = c.UpdateDate,
                        DemoFileName = c.DemoFileName,
                        SubGroup = c.SubGroup,
                        comments = c.Comments,
                        courseLevel = c.courseLevel,
                        courseStatus = c.courseStatus,
                        CoursePresentation = c.CoursePresentation,
                        Tags = c.Tags,
                        StudentCounter = c.UserCourses.Count(u => u.CourseId == id),
                    }
            )
            .AsNoTracking()
            .Single();
    }

    public List<CourseGroups> GetAllGroupsForLayout()
    {
        return _context.CourseGroups.Include(g => g.CourseGroup).AsNoTracking().ToList();
    }

    public string GetTeacherName(int courseId)
    {
        var userId = _context.Courses.Find(courseId).TeacherId;
        var username = _userManager.FindByIdAsync(userId).Result;
        var fullname = username.FirstName + " " + username.LastName;
        return fullname;
    }

    public TimeSpan GetTimeSpan(int courseId)
    {
        var course = _context.Courses
            .Include(e => e.CourseEpisodes)
            .Single(c => c.CourseId == courseId);
        TimeSpan time = new TimeSpan(course.CourseEpisodes.Sum(e => e.EpisodeTime.Ticks));
        return time;
    }

    public List<Course> GetDeleteCourses()
    {
        return _context.Courses.Where(c => c.IsDelete).IgnoreQueryFilters().ToList();
    }

    public bool IsexistCourse(int courseId)
    {
        return _context.Courses.Any(c => c.CourseId == courseId);
    }

    public CourseEpisode GetEpisodeById(int episodeId)
    {
        return _context.CourseEpisodes
            .Include(c => c.Course)
            .FirstOrDefault(e => e.EpisodeId == episodeId);
    }

    public Tuple<List<ShowCourseListItemViewModel>, int> GetCourse(
        int pageId = 1,
        string filter = "",
        string getType = "all",
        string sort = "lates",
        List<int> categories = null,
        int take = 0
    )
    {
        if (take == 0)
        {
            take = 4;
        }
        IQueryable<Course> result = _context.Courses;
        if (!string.IsNullOrEmpty(filter))
        {
            result = result.Where(c => c.CourseTitle.Contains(filter));
        }

        switch (getType)
        {
            case "all":
                break;
            case "price":
                {
                    result = result.Where(c => c.CoursePrice != 0);
                    break;
                }
            case "free":
                {
                    result = result.Where(c => c.CoursePrice == 0);
                    break;
                }
        }

        switch (sort)
        {
            case "lates":
                {
                    result = result.OrderByDescending(c => c.CreateDate);
                    break;
                }
            case "oldest":
                {
                    result = result.OrderBy(c => c.CreateDate);
                    break;
                }
            case "1":
                {
                    result = result.Where(c => c.courseStatus == 1);
                    break;
                }
            case "2":
                {
                    result = result.Where(c => c.courseStatus == 2);
                    break;
                }
        }

        if (categories != null && categories.Any())
        {
            result = result.Where(c => categories.Contains(c.GroupId) || categories.Contains((int)c.SubGroup));
        }


        int skip = (pageId - 1) * take;
        double pageCounter = (double)((decimal)result.Count() / Convert.ToDecimal(take));
        int pageCount = (int)Math.Ceiling(pageCounter);
        var query = result
            .Include(c => c.CourseEpisodes)
            .Select(
                c =>
                    new ShowCourseListItemViewModel()
                    {
                        ImageName = c.CourseImageName,
                        CourseId = c.CourseId,
                        Price = c.CoursePrice,
                        Title = c.CourseTitle,
                        CourseEpisodes = c.CourseEpisodes,
                        Slug = c.Slug
                    }
            )
            .Skip(skip)
            .Take(take)
            .AsNoTracking()
            .ToList();
        return Tuple.Create(query, pageCount);
    }

    public List<CourseGroups> GetAllGroups()
    {
        return _context.CourseGroups.Include(g => g.CourseGroup).AsNoTracking().ToList();
    }

    public List<ShowCourseListItemViewModel> GetTeacherCourses(string TeacherId)
    {
        var teachId = _context.Users.SingleOrDefault(u => u.Email == TeacherId).Id;
        var courses = _context.Courses.Include(e => e.CourseEpisodes).Where(u => u.TeacherId == teachId).Select(c => new ShowCourseListItemViewModel()
        {
            CourseId = c.CourseId,
            CourseEpisodes = c.CourseEpisodes,
            ImageName = c.CourseImageName,
            Title = c.CourseTitle
        }).ToList();
        return courses;
    }

    public async Task AddSource(int courseId, IFormFile source)
    {
        var projectSource = await _context.CourseEpisodes.Where(p => p.CourseId == courseId).FirstOrDefaultAsync(p => p.EpisodeTitle == "سورس دوره");
        if (projectSource == null)
        {
            CourseEpisode courseEpisode = new CourseEpisode()
            {
                CourseId = courseId,
                EpisodeTime = TimeSpan.Zero,
                EpisodeTitle = "سورس دوره",
                IsFree = false,
                Login = true
            };
            if (source != null)
            {
                string deletedemoPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/course/Episode",
                source.FileName);

                if (File.Exists(deletedemoPath))
                {
                    File.Delete(deletedemoPath);
                }
                courseEpisode.EpisodeFileName =
                    source.FileName + Path.GetExtension(source.FileName);
                string demoPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/course/Episode",
                    courseEpisode.EpisodeFileName
                );

                using (var stream = new FileStream(demoPath, FileMode.Create))
                {
                    await source.CopyToAsync(stream);
                }
            }
            await _context.CourseEpisodes.AddAsync(courseEpisode);
            await _context.SaveChangesAsync();
        }
        await Task.CompletedTask;
    }

    public async Task<IReadOnlyList<CourseIndexViewModel>> GetRecommendedCourses()
    {
        return await _context.Courses
            .Where(c => c.IsRecommended)
            .Select(c =>
                new CourseIndexViewModel()
                {
                    CourseId = c.CourseId,
                    CourseTitle = c.CourseTitle,
                    CoursePrice = c.CoursePrice,
                    CourseImageName = c.CourseImageName,
                    Slug = c.Slug
                })
            .OrderBy(c => c.CourseId)
            .Take(6)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<CourseIndexViewModel>> GetBestSellerCourses()
    {
        return await _context.Courses
                .Where(c => c.IsRecommended)
                .Select(c =>
                    new CourseIndexViewModel()
                    {
                        CourseId = c.CourseId,
                        CourseTitle = c.CourseTitle,
                        CoursePrice = c.CoursePrice,
                        CourseImageName = c.CourseImageName,
                        Slug = c.Slug
                    })
                .OrderBy(c => c.CourseId)
                .Take(3)
                .AsNoTracking()
                .ToListAsync();
    }
}
