using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Core.Settings;
using Microsoft.Extensions.Options;
using Core.Interfaces;
using System.Data;

namespace Core.Services
{
    public class SendEmailServices
    {
        private readonly EmailConfgSettings email_confg;
        public SendEmailServices(IOptions<EmailConfgSettings> options)
        {
            email_confg = options.Value;
           
        }



        public async Task SendEmail(string Email, string AcountPassword, string role)
        {
            try
            {

                var email = email_confg.Email;
                var password = email_confg.Password;
                var host = email_confg.Host;
                var port = email_confg.Port;


                var smtpClient = new SmtpClient(host, port);
                smtpClient.EnableSsl = true;

                smtpClient.UseDefaultCredentials = false;

                smtpClient.Credentials = new NetworkCredential(email, password);



                var subject = "EslamOffers.com - Account Information";
                var body = $@"
                Hello,

                Here are your account details:

                Email: {email}
                Password: {AcountPassword}
                Role: {role}

                Please keep this information confidential.

                Best regards,  
                EslamOffers.com Team";
                var message = new MailMessage(email! , Email , subject , body);

                await smtpClient.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                // Log the exception (to file, console, or database)
                Console.WriteLine(ex.Message);
                throw; // or handle accordingly
            }


        }
    }
}
