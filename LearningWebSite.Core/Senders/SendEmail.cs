using System.Net.Mail;

namespace LearningWebSite.Core.Senders
{
    public class SendEmail
    {
        public static void Send(string To, string Subject, string Body)
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress("info.fetrati@gmail.com", "ایزی لرن");
            mail.To.Add(To);
            mail.Subject = Subject;
            mail.Body = Body;
            mail.IsBodyHtml = true;

            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("info.fetrati@gmail.com", "wynksewczkmvrvut");
            SmtpServer.EnableSsl = true;

            SmtpServer.Send(mail);

        }
    }
}