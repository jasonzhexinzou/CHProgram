using iPathAuthService.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using XFramework.XException;
using XFramework.XUtil;

namespace iPathAuthService
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            #region 使用框架
            BaseException.Init(HttpRuntime.AppDomainAppPath);
            LogHelper.Init();
            #endregion

            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                    (sender, certificate, chain, errors) => true;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
        }
    }
}
