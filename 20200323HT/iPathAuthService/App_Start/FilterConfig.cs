using iPathAuthService.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XFramework.XException;

namespace iPathAuthService
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new GlobalFilter());
            //filters.Add(new ExceptionFilter());
            filters.Add(new HandleErrorAttribute());
        }
    }
}