using LearningWebSite.DataLayer.Context;
using LearningWebSite.DataLayer.Entities.Comments;
using Microsoft.EntityFrameworkCore;

namespace LearningWebSite.Core.Services.CommentService;

public interface ICommentServices
{
    Task AddCommentAsync(int courseId, string value, int commentId, string username);
    List<Comment> GetCourseComments(int courseId);
    List<Comment> GetComments();
    void DeleteComment(int commentId);
    void ConfirmComment(int commentId);

}

public class CommentServices : ICommentServices
{
    private readonly ApplicationDbContext context;

    public CommentServices(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task AddCommentAsync(int courseId, string value, int commentId, string username)
    {
        var userId = context.Users.First(u => u.UserName == username).Id;
        var comment = new Comment()
        {
            CommentText = value,
            CourseId = courseId,
            UserId = userId,
            SaveDate = DateTime.Now,
            IsApprove = false,
            ParentId = commentId,
        };
        await context.Comments.AddAsync(comment);
        await context.SaveChangesAsync();
        await Task.CompletedTask;
    }

    public void ConfirmComment(int commentId)
    {
        var comment = context.Comments.Find(commentId);
        comment.IsApprove = true;
        context.Comments.Update(comment);
        context.SaveChanges();
    }

    public void DeleteComment(int commentId)
    {
        var comment = context.Comments.Find(commentId);
        context.Comments
            .Where(u => u.ParentId == commentId)
            .ToList()
            .ForEach(r => context.Comments.Remove(r));
        context.Comments.Remove(comment);
        context.SaveChanges();
    }

    public List<Comment> GetComments()
    {
        return context.Comments.Include(u => u.CustomUser).Include(c => c.Course).Where(c => !c.IsApprove).AsNoTracking().ToList();
    }

    public List<Comment> GetCourseComments(int courseId)
    {
        return context.Comments
            .Include(u => u.CustomUser)
            .Where(c => c.CourseId == courseId)
            .AsNoTracking()
            .ToList();
    }
}
