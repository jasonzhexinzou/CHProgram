using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MealH5.Handler
{
    public class WebConfigHandler
    {
        public static string OpenApiAddress
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["OpenApiAddress"];
            }
        }

        public static string ProgramTitle
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["ProgramTitle"];
            }
        }

        public static string ShortUrlService
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["ShortUrlService"];
            }

        }

        public static string CurrentDomain
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["CurrentDomain"];
            }
        }

        public static string AWSService
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["AWSService"];
            }

        }

        public static string ShortUrl
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["ShortUrl"];
            }

        }

        public static string IsLoadRestaurantData
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["IsLoadRestaurantData"];
            }

        }

    }
}