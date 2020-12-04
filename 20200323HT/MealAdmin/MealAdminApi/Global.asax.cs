using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using XFramework.XException;
using XFramework.XInject;
using XFramework.XUtil;

namespace MealAdminApi
{
    public class Global : System.Web.HttpApplication
    {
        public static ClassPathXmlApplicationContext applicationContext = null;
        protected void Application_Start(object sender, EventArgs e)
        {
            #region 依赖注入
            // 应用程序下上文
            applicationContext = new ClassPathXmlApplicationContext(Path.Combine(HttpRuntime.AppDomainAppPath, "Bean_Config.xml"));
            //// 重写ControllerFactory以便于注入
            //ControllerBuilder.Current.SetControllerFactory(
            //    new XInjectControllerFactory()
            //    {
            //        applicationContext = applicationContext
            //    });
            #endregion

            #region 使用框架
            BaseException.Init(HttpRuntime.AppDomainAppPath);
            LogHelper.Init();
            #endregion

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exp = Server.GetLastError();
            LogHelper.Error("", exp);
        }

    }
}