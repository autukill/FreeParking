using MailKit.Net.Smtp;
using MimeKit;

namespace FreeParkingMaui.Services;

public class EmailService
{
    public async Task<(bool Success, string Message)> SendEmailAsync(string subject, string body)
    {
        var smtpUser = await SecureStorage.GetAsync(Constants.SmtpUserKey);
        var smtpPassword = await SecureStorage.GetAsync(Constants.SmtpPasswordKey);

        if (string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPassword))
        {
            return (false, "邮箱未配置，请先设置邮箱");
        }

        try
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(smtpUser));
            message.Sender = MailboxAddress.Parse(smtpUser);
            message.To.Add(MailboxAddress.Parse(smtpUser));
            message.Subject = subject;

            message.Body = new TextPart("plain")
            {
                Text = body
            };

            using (var client = new SmtpClient())
            {
                client.CheckCertificateRevocation = false;
                await client.ConnectAsync(Constants.SmtpHost, Constants.SmtpPort, true);
                await client.AuthenticateAsync(smtpUser, smtpPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }

            return (true, "邮件发送成功");
        }
        catch (Exception ex)
        {
            return (false, $"邮件发送失败: {ex.Message}");
        }
    }
}
