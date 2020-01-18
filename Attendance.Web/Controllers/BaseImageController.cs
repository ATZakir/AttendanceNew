using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Attendance.Web.Controllers
{
    public class BaseImageController : Controller
    {        
      private string GetMainFolder()
      {
            var mainFolder = Server.MapPath("~/files");
            if (!Directory.Exists(mainFolder))
            {
                Directory.CreateDirectory(mainFolder);
            }
            return mainFolder;
        }

        public virtual bool UploadFile(string itemName, string subfolderName, HttpPostedFileBase file)
        {
            if (file == null) return false;
            var mainFolder = GetMainFolder(); 

            try
            {
                var subFolder = Server.MapPath("~/files/" + subfolderName);
                if (!Directory.Exists(subFolder))
                {
                    Directory.CreateDirectory(subFolder);
                }

         
                if (System.IO.File.Exists(Server.MapPath("~/files/" + subfolderName + "/" + itemName)))
                {
                    System.IO.File.Delete(Server.MapPath("~/files/" + subfolderName + "/" + itemName));
                }

                var saveToFileLoc = Server.MapPath("~/files/" + subfolderName + "/" + itemName);
                try
                {
                    file.SaveAs(saveToFileLoc);
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("File Save Error: " + e);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public virtual bool DeleteFile(string itemName, string subfolderName)
        {
            var mainFolder = GetMainFolder();

            try
            {               
                if (System.IO.File.Exists(
                Server.MapPath("~/files/" + subfolderName + "/" + itemName)))
                {
                    System.IO.File.Delete(
                        Server.MapPath("~/files/" + subfolderName + "/" + itemName));
                    return true;
                }
                else
                {
                    Console.WriteLine("No File Exists");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}