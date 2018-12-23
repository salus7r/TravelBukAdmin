using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace TravelBuk.Helpers
{
    public class EmailSender : IEmailSender
    {
        readonly string from = "salus7r25@gmail.com";
        readonly string password = "25Sep1991-";

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Task.Run(() =>
            {
                try
                {
                    MailMessage mail = new MailMessage(from, email)
                    {
                        Subject = subject,
                        IsBodyHtml = true, //to make message body as html  
                        Body = message
                    };

                    SmtpClient smtp = new SmtpClient
                    {
                        Port = 587,
                        Host = "smtp.gmail.com", //for gmail host  
                        EnableSsl = true,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(from, password),
                        DeliveryMethod = SmtpDeliveryMethod.Network
                    };
                    smtp.SendMailAsync(mail);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
        }
    }
}
