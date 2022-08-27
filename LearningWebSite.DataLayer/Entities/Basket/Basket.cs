namespace LearningWebSite.DataLayer.Entities.Basket
{
    public class Basket
    {
        public int BasketId { get; set; }
        public int CourseId { get; set; }
        public int CoursePrice { get; set; }
        public bool IsFinally { get; set; } = false;
        public string CourseTitle { get; set; }
        public string CourseImage { get; set; }
        public string UserName { get; set; }
    }
}
