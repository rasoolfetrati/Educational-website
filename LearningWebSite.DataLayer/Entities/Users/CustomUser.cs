using System;
using System.Collections.Generic;
using System.Text;
using LearningWebSite.DataLayer.Entities.Basket;
using LearningWebSite.DataLayer.Entities.Comments;
using LearningWebSite.DataLayer.Entities.Courses;
using LearningWebSite.DataLayer.Entities.User;
using LearningWebSite.DataLayer.Entities.UserWallet;
using Microsoft.AspNetCore.Identity;

namespace LearningWebSite.DataLayer.Entities.Users
{
    public class CustomUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
        public string ActiveCode { get; set; }
        public bool IsDelete { get; set; } = false;


        public virtual List<Factor> Factors { get; set; }
        public virtual List<Course> Courses { get; set; }
        public virtual List<Order> Orders { get; set; }
        public List<UserInCourse> UserCourses { get; set; }
        public List<Comment> Comments { get; set; }
        public List<UserDiscountCode> DiscountCodes { get; set; }
    }
}
