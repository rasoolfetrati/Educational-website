using LearningWebSite.Core.InfraStructure;
using LearningWebSite.Core.ViewModel.CourseVM;
using LearningWebSite.DataLayer.Context;
using LearningWebSite.DataLayer.Entities.Courses;
using LearningWebSite.DataLayer.Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LearningWebSite.Core.Services.CourseService
{
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
        ShowCourseViewModelWithIndex GetAllCourseForAdmin(int PageIndex = 1);
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
        ShowCourseViewModel showCourseViewModel(int id, string Title);
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
    }

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
                )
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
                )
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
            if (
                courseViewModel.imgCourseUp != null
                && ImageValidator.IsImage(courseViewModel.imgCourseUp)
            )
            {
                courseViewModel.CourseImageName =
                    NameGenerator.GenerateUniqCode()
                    + Path.GetExtension(courseViewModel.imgCourseUp.FileName);
                string imagePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/CourseImage",
                    courseViewModel.CourseImageName
                );

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await courseViewModel.imgCourseUp.CopyToAsync(stream);
                }

                ImageConvertor imageConverter = new ImageConvertor();

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
            };
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();
            await Task.CompletedTask;
        }

        public ShowCourseViewModelWithIndex GetAllCourseForAdmin(int PageIndex = 1)
        {
            ShowCourseViewModelWithIndex showCourseViewModelWithIndex = new ShowCourseViewModelWithIndex();
            int maxRows = 5;
            showCourseViewModelWithIndex.showCourseViewModels = _context.Courses
                .Select(
                    c =>
                        new ShowCourseViewModel()
                        {
                            CourseId = c.CourseId,
                            CourseImageName = c.CourseImageName,
                            CreateDate = c.CreateDate,
                            CourseTitle = c.CourseTitle,
                            IsDelete = c.IsDelete,
                            UpdateDate = c.UpdateDate,
                        }
                )
                .OrderBy(c => c.CourseId)
                .Skip((PageIndex - 1) * maxRows)
                .Take(maxRows)
                .ToList();
            double pageCount = (double)((decimal)this._context.Courses.Count() / Convert.ToDecimal(maxRows));
            showCourseViewModelWithIndex.PageCount = (int)Math.Ceiling(pageCount);

            showCourseViewModelWithIndex.CurrentPageIndex = PageIndex;

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
            return _context.CourseEpisodes.Where(c => c.CourseId == courseId).ToList();
        }

        public void UpdateCourse(Course courseView, IFormFile imgCourseUp, IFormFile demoUp)
        {
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
                }
                originalCourse.CourseImageName =
                    NameGenerator.GenerateUniqCode() + Path.GetExtension(imgCourseUp.FileName);
                string imagePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/CourseImage",
                    originalCourse.CourseImageName
                );

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    imgCourseUp.CopyTo(stream);
                }

                ImageConvertor imageConverter = new ImageConvertor();

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
                    "wwwroot/course/demos",
                    courseEpisode.EpisodeFileName
                );
                if (File.Exists(deletedemoPath))
                {
                    File.Delete(deletedemoPath);
                }
                courseEpisode.EpisodeFileName =
                    NameGenerator.GenerateUniqCode() + Path.GetExtension(demoUp.FileName);
                string demoPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/course/demos",
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
            return _context.CourseEpisodes.Where(c => c.CourseId == courseId).Count();
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
                            CourseImageName = c.CourseImageName
                        }
                )
                .Take(9)
                .ToList();
        }

        public ShowCourseViewModel showCourseViewModel(int id, string Title)
        {
            return _context.Courses
                .Include(c => c.CourseEpisodes)
                .Include(c => c.Comments)
                .Where(c => c.CourseId == id && c.CourseTitle == Title.Replace("-", " "))
                .Select(
                    c =>
                        new ShowCourseViewModel()
                        {
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
                            courseStatus = c.courseStatus
                        }
                )
                .SingleOrDefault();
        }

        public List<CourseGroups> GetAllGroupsForLayout()
        {
            return _context.CourseGroups.Include(g => g.CourseGroup).ToList();
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
            List<int> selectedGroups = null,
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

            if (selectedGroups != null && selectedGroups.Any())
            {
                foreach (int groupId in selectedGroups)
                {
                    result = result.Where(c => c.GroupId == groupId || c.SubGroup == groupId);
                }
            }
            int skip = (pageId - 1) * take;
            int pageCount =
                result
                    .Include(c => c.CourseEpisodes)
                    .Select(
                        c =>
                            new ShowCourseListItemViewModel()
                            {
                                ImageName = c.CourseImageName,
                                CourseId = c.CourseId,
                                Price = c.CoursePrice,
                                Title = c.CourseTitle,
                                CourseEpisodes = c.CourseEpisodes
                            }
                    )
                    .Count() / take;
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
                            CourseEpisodes = c.CourseEpisodes
                        }
                )
                .Skip(skip)
                .Take(take)
                .ToList();
            return Tuple.Create(query, pageCount);
        }

        public List<CourseGroups> GetAllGroups()
        {
            return _context.CourseGroups.Include(g => g.CourseGroup).ToList();
        }
    }
}
