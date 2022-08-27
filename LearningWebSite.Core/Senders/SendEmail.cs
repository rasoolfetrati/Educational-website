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

            //System.Net.Mail.Attachment attachment;
            // attachment = new System.Net.Mail.Attachment("c:/textfile.txt");
            // mail.Attachments.Add(attachment);

            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("info.fetrati@gmail.com", "ulmyrrxhcacsmljs");
            SmtpServer.EnableSsl = true;

            SmtpServer.Send(mail);

        }
    }
}