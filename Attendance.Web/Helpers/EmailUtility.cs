using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace Attendance.Web.Helpers
{
    public class EmailUtility
    {
        public static bool SendMail(List<string> receipients, List<string> carbonCopies, string subject, string body)
        {
            bool mailStatus = true;
            var mail = new MailMessage();
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            SmtpClient client = new SmtpClient();

            try
            {
                foreach (var aReceipient in receipients)
                    mail.To.Add(aReceipient); //to
                if (carbonCopies != null)
                    foreach (var aCarbonCopy in carbonCopies)
                        mail.CC.Add(aCarbonCopy);
                client.Send(mail);
            }
            catch (Exception)
            {
                mailStatus = false;
            }

            return mailStatus;
        }

        public string EmailAddress { get; set; }

        public bool SendMailForgetPassWord(string to, string subject, string body, string siteUrl, string sender)
        {
            bool mailStatus = true;

            MailMessage m = new MailMessage();
            SmtpClient sc = new SmtpClient();
            m.Subject = subject;
            m.IsBodyHtml = true;
            m.Body = body + "<br>" + "<br>" + siteUrl + "<br>" + sender;

            try
            {
                m.To.Add(to); //to
                sc.Send(m);
            }
            catch (Exception)
            {
                mailStatus = false;
            }
            return mailStatus;
        }

    }
}