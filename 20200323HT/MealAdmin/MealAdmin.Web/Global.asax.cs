using MealAdmin.Web.App_Start;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using XFramework.XException;
using XFramework.XInject;
using XFramework.XInject.MVC;
using XFramework.XUtil;

namespace MealAdmin.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static ClassPathXmlApplicationContext applicationContext;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            #region 依赖注入
            // 应用程序下上文
            applicationContext = new ClassPathXmlApplicationContext(Path.Combine(HttpRuntime.AppDomainAppPath, "Bean_Config.xml"));
            // 重写ControllerFactory以便于注入
            ControllerBuilder.Current.SetControllerFactory(
                new XInjectControllerFactory()
                {
                    applicationContext = applicationContext
                });
            #endregion
            
            #region 使用框架
            BaseException.Init(HttpRuntime.AppDomainAppPath);
            LogHelper.Init();
            #endregion

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;

        }
    }
}
