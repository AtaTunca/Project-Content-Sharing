using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace Project_Content_Sharing.Service
{

    public class MailTemplate
    {
        public string Subject { get; set; }
        public string Message { get; set; }
        public string From { get; set; }
        public string To { get; set; }

    }

    public class MailService
    {
        public bool SendMessage(MailTemplate mail)
        {
            MailMessage msg = new MailMessage("yazilim.sc301@gmail.com", mail.To);
            msg.Subject = mail.Subject;
            msg.Body = mail.Message;
            msg.IsBodyHtml = true;

            SmtpClient c = new SmtpClient();




            c.EnableSsl = true;
            c.Credentials = new NetworkCredential("yazilim.sc301@gmail.com", "Wissen2017");
            c.Host = "smtp.gmail.com"; //gmail mail sunucusu
            c.Port = 587; // reject yada spam olmaması için 25 değil 587 portundan göndeririz. 

            try
            {
                c.Send(msg);
                return true;
            }
            catch (Exception e)
            {
                var error = e;
                var error2 = e;
                return false;
            }

        }
    }
}