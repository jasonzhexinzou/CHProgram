using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Util
{
    public class WebConfig
    {
        public static string AppLoginSecret
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["AppLoginSecret"]; }
        }
        public static string AppLoginServer
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["AppLoginServer"]; }
        }

        public static string AppLoginServerCost
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["AppLoginServerCost"]; }
        }

        public static string IamServer
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["IamServer"]; }
        }

        public static string IamAppID
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["IamAppID"]; }
        }

        public static string MealH5SiteUrl
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["MealH5SiteUrl"]; }
        }

        public static string AWSServiceUrl
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["AWSService"]; }
        }
    }
}
