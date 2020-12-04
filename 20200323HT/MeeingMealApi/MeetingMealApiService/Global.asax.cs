using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Routing;
using XFramework.XException;
using XFramework.XInject;
using XFramework.XUtil;

namespace MeetingMealApiService
{
    public class Application : System.Web.HttpApplication
    {
        public static ClassPathXmlApplicationContext applicationContext = null;
        protected void Application_Start(object sender, EventArgs e)
        {

            #region 使用框架
            LogHelper.Init();
            #endregion
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exp = Server.GetLastError();
            LogHelper.Error("", exp);
        }
    }
}
