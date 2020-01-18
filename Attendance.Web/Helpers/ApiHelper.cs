using System;
using System.Linq;
using System.Configuration;
using Attendance.Service;
using System.Net.Http.Headers;

namespace Attendance.Web.Helpers
{
    public class ApiHelper
    {
        public static bool CheckAuthentication(HttpRequestHeaders headers, IUserService userService, Guid userId)
        {
            string tokenNumber = string.Empty;

            if (headers.Contains("tokenNumber"))
            {
                tokenNumber = headers.GetValues("tokenNumber").First();
            }
            else
            {
                return false;
            }

            if (tokenNumber != null && tokenNumber != string.Empty)
            {
                var addTime = ConfigurationManager.AppSettings["sessionTime"];
                var user = userService.GetUser(userId);
                if (user != null && user.ExpiryDateTime != null && user.TokenNumber == tokenNumber && user.IsActive == true && user.ExpiryDateTime >= DateTime.Now)
                {
                    // Update ServerNowDateTime............
                    user.ExpiryDateTime = DateTime.Now.AddMinutes(Convert.ToInt32(addTime));
                    userService.UpdateUser(user);
                    return true;
                }
                else {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}