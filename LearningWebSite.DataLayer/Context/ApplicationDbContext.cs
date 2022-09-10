using LearningWebSite.DataLayer.Entities.Basket;
using LearningWebSite.DataLayer.Entities.Comments;
using LearningWebSite.DataLayer.Entities.ContactUs;
using LearningWebSite.DataLayer.Entities.Courses;
using LearningWebSite.DataLayer.Entities.Users;
using LearningWebSite.DataLayer.Entities.UserWallet;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LearningWebSite.DataLayer.Context
{
    public class ApplicationDbContext:IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {

        }

        public DbSet<CustomUser> Users { get; set; }
        public DbSet<Factor> Factors { get; set; }
        public DbSet<CourseGroups> CourseGroups { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseEpisode> CourseEpisodes { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> orderDetails { get; set; }
        public DbSet<UserInCourse> UserInCourses { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Contacts> Contacts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CourseGroups>().HasQueryFilter(g => !g.IsDelete);
            builder.Entity<Course>().HasQueryFilter(g => !g.IsDelete);
            base.OnModelCreating(builder);
        }
    }
}
