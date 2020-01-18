using System;
using System.Collections.Generic;
using System.Web;
using Attendance.Model;
using Attendance.ClientModel;

namespace Attendance.Web.Helpers
{
    public static class UserSession
    {
        public static void SetUserFromSession(UserModel userModel)
        {
            HttpContext.Current.Session["LoggedInUser"] = userModel;
        }

        public static UserModel GetUserFromSession()
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
                return (UserModel)HttpContext.Current.Session["LoggedInUser"];
            else if (HttpContext.Current != null && HttpContext.Current.Session == null)
            {
                return null;
            }
            else
            {
                return null;
            }
        }

        public static bool IsLoggedIn()
        {
            UserModel userModel = GetUserFromSession();
            return userModel != null ? true : false;
        }



        public static string GetUserFullNameFromSession()
        {
            UserModel userModel = GetUserFromSession();
            return userModel != null ? (string)userModel.Name : "";
        }

        public static void DestroySessionAfterUserLogout()
        {
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            HttpContext.Current.Response.Cache.SetNoStore();

            HttpContext.Current.Session.Abandon();
        }

        public static bool IsAdmin()
        {
            var isAdminRole = false;
            UserModel userModel = GetUserFromSession();
            if (userModel != null)
            {
                var defaultRoleId = userModel.RoleId;
                if (defaultRoleId != null && defaultRoleId == 1)
                    isAdminRole = true;
            }
            return isAdminRole;
        }

        public static bool IsSchoolUser()
        {
            var isSchoolRole = false;
            UserModel userModel = GetUserFromSession();
            if (userModel != null)
            {
                if (userModel.Level == 0)
                    isSchoolRole = true;
            }
            return isSchoolRole;
        }

        public static void SetTimeZoneOffset(long offset)
        {
            HttpContext.Current.Session["TimezoneOffset"] = offset;
        }

        public static long GetTimeZoneOffset()
        {
            return 360;// (long)HttpContext.Current.Session["TimezoneOffset"];
        }

        public static void SetUserSchoolAccess(List<int> schoolIds)
        {
            HttpContext.Current.Session["UserSchoolAccess"] = schoolIds;
        }

        public static List<int> GetUserSchoolAccess()
        {
            return (List<int>)HttpContext.Current.Session["UserSchoolAccess"];
        }

        public static int GetCurrentUserSchool()
        {
            if (GetUserSchoolAccess().Count > 0)
                return GetUserSchoolAccess()[0];
            else
                return -1;//TODO: think on this 
        }

        public static void SetModuleClicked(string id)
        {
            HttpContext.Current.Session["ModuleId"] = id;
        }

        public static string GetModuleId()
        {
            return (string)HttpContext.Current.Session["ModuleId"];
        }

        public static void SetCurrentUICulture(string value)
        {
            if (value == "bn")
            {
                HttpContext.Current.Session["CurrentUICulture"] = "bn-BD";
            }
            else
            {
                HttpContext.Current.Session["CurrentUICulture"] = "en-US";
            }
        }

        public static string GetCurrentUICulture()
        {
            return (string)HttpContext.Current.Session["CurrentUICulture"];

        }

        public static int GetUserIdFromSession()
        {
            UserModel userModel = GetUserFromSession();
            return userModel != null ? userModel.Id:0;
        }
    }
}