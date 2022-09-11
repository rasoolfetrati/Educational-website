using LearningWebSite.DataLayer.Context;
using LearningWebSite.DataLayer.Entities.ContactUs;

namespace LearningWebSite.Core.Services.ContactUsService;

public interface IContactUsService
{
    Task SaveMessage(Contacts contacts);
    void Delete(int messageId);
    IList<Contacts> GetAllMessages();
}
public class ContactUsService : IContactUsService
{
    private readonly ApplicationDbContext context;
    public ContactUsService(ApplicationDbContext context)
    {
        this.context = context;
    }
    public void Delete(int messageId)
    {
        var message = context.Contacts.Find(messageId);
        context.Contacts.Remove(message);
        context.SaveChanges();
    }

    public IList<Contacts> GetAllMessages()
    {
        return context.Contacts.ToList();
    }

    public async Task SaveMessage(Contacts contacts)
    {
        await context.Contacts.AddAsync(contacts);
        await context.SaveChangesAsync();
    }
}
