using Attendance.ClientModel;
using Attendance.Model;
using Attendance.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Results;

namespace Attendance.Web.Helpers
{
    public class AccountHelper
    {
        public static string ResetPassword(ISecurityService securityService, IUserService userService, User user, out bool isSuccess, out string message)
        {
            bool includeLowercase = true;
            bool includeUppercase = true;
            bool includeNumeric = true;
            bool includeSpecial = false;
            bool includeSpaces = false;
            int lengthOfPassword = 8;

            string tempPwd = PasswordGenerator.GeneratePassword(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, lengthOfPassword);

            user.Password = securityService.GenerateHashWithSalt(tempPwd, user.Email);
            userService.UpdateUser(user);

            EmailUtility emailObj = new EmailUtility();
            var emailSubject = @System.Configuration.ConfigurationManager.AppSettings["mail:PasswordRecovery:Subject"];
            var emailBody = @System.Configuration.ConfigurationManager.AppSettings["mail:PasswordRecovery:Body"];
            emailBody = string.Format(emailBody, tempPwd);
            emailBody = emailBody.Replace("/r/n", "<br>");
            string str = @System.Configuration.ConfigurationManager.AppSettings["mail:SourceUrl"];
            string str1 = "<a href=" + str + ">Click here</a>";
            var siteUrl = @System.Configuration.ConfigurationManager.AppSettings["mail:SiteUrl"];
            siteUrl = string.Format(siteUrl, str1);
            siteUrl = siteUrl.Replace("/r/n", "<br>");

            var sender = @System.Configuration.ConfigurationManager.AppSettings["mail:Sender"];
            sender = sender.Replace("/r/n", "<br>");

            isSuccess = emailObj.SendMailForgetPassWord(user.Email, emailSubject, emailBody, siteUrl, sender);
            if (isSuccess == true)
                message = "Email sent to the email address " + user.Email + " successfully!";
            else
                message = "Email could not send!";

            return message;
        }
    }
}