using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
namespace TMDT.Models
{
    public class EmailService
    {
        public bool Send(string smtpUserName, string smtpPassword, string smtpHost, int smtpPort,
            string toEmail, string subject, string body,string pic)
        {
            try
            {
                using (var smtpClient = new SmtpClient())
                {
                    string path = pic;// my logo is placed in images folder

                    LinkedResource logo = new LinkedResource(path,MediaTypeNames.Image.Jpeg);
                    logo.ContentId = "companylogo";

                    smtpClient.EnableSsl = true;
                    smtpClient.Host = smtpHost;
                    smtpClient.Port = smtpPort;
                    smtpClient.UseDefaultCredentials = true;
                    smtpClient.Credentials = new NetworkCredential(smtpUserName, smtpPassword);
                    var msg = new MailMessage
                    {
                        BodyEncoding = Encoding.UTF8,
                        From = new MailAddress(smtpUserName),
                        Subject = subject,
                        Priority = MailPriority.Normal,
                    };
                    AlternateView av1 = AlternateView.CreateAlternateViewFromString(
                            "<html><body><img src=cid:companylogo/>" +
                            "<br></body></html>" + body,
                            null, MediaTypeNames.Text.Html);

                    //now add the AlternateView
                    av1.LinkedResources.Add(logo);

                    //now append it to the body of the mail
                    msg.AlternateViews.Add(av1);
                    msg.IsBodyHtml = true;
                    msg.To.Add(toEmail);

                    smtpClient.Send(msg);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}