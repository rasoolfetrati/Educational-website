using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningWebSite.Core.ViewModel.BasketVM
{
    public class ShowBasketVM
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public string CourseImage { get; set; }
        public int CoursePrice { get; set; }
        public int SumOrder { get; set; }
    }
}
