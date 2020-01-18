using Attendance.CachingService;
using Attendance.Core.Common;
using Attendance.LoggerService;
using Attendance.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace Attendance.Service
{
    public class ServiceUtil<T> where T : class
    {
        private static LoggingService logger = new LoggingService("ServiceActionLog");
        private static LoggingService logger2 = new LoggingService("ServiceUtil");
        private static ICacheProvider cacheProvider = new DefaultCacheProvider();

        public static void WriteActionLog(object id, ENUMOperation opt, T entity=null)
        {
            try
            {
                string xml = null;
                if (entity != null)
                {
                    XmlSerializer xsSubmit = GetXmlSerilizer(entity);

                    using (StringWriter sww = new StringWriter())
                    using (XmlWriter writer = XmlWriter.Create(sww))
                    {
                        xsSubmit.Serialize(writer, entity);
                        xml = sww.ToString();
                    }
                }

                User aUser = (User)HttpContext.Current.Session["LoggedInUser"];

                //logger.WriteActionLog(aUser.Id, DateTime.UtcNow, id.ToString(), xml, opt.ToString(), typeof(T).Name);
            }
            catch (Exception ex) 
            {
                logger2.Error("Error in writing action log", ex);
            }
        }

        private static XmlSerializer GetXmlSerilizer(T entity)
        {
            XmlSerializer xsSubmit = cacheProvider.Get(typeof(T).Name) as XmlSerializer;
            
            if (xsSubmit == null)
            {
                XmlAttributeOverrides overrides = new XmlAttributeOverrides();
                XmlAttributes attribs = new XmlAttributes();
                attribs.XmlIgnore = true;
                PropertyInfo[] properties = entity.GetType().GetProperties().Where(p => p.GetMethod.IsVirtual).ToArray();
                if (properties.Length > 0)
                {
                    for (int i = 0; i < properties.Length; i++)
                    {
                        overrides.Add(typeof(T), properties[i].Name, attribs);
                    }
                }

                xsSubmit = new XmlSerializer(entity.GetType(), overrides);
                if (xsSubmit != null)
                {
                    cacheProvider.Set(typeof(T).Name, xsSubmit);
                }
            }
                      
            return xsSubmit;
         } 

    }
}
