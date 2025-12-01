namespace BeanScene.Web.Services
{
   using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

public class SmtpEmailSender : IEmailSender
{
    private readonly IConfiguration _config;

    public SmtpEmailSender(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        using var client = new SmtpClient(_config["Smtp:Host"])
        {
            Port = int.Parse(_config["Smtp:Port"] ?? "587"),
            EnableSsl = bool.Parse(_config["Smtp:EnableSSL"] ?? "true"),
            Credentials = new NetworkCredential(
                _config["Smtp:User"],
                _config["Smtp:Password"])
        };

        using var mail = new MailMessage
        {
            From = new MailAddress(_config["Smtp:User"], "Bean Scene"),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };

        mail.To.Add(email);

        await client.SendMailAsync(mail);
    }
}

}
