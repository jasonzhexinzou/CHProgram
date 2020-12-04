using iPathAuthService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XFramework.WeChatAPI.SessionHandlers;

namespace iPathAuthService.Util
{
    public class GlobalHandler
    {
        public static QyApiHandler QyApiHandler = new QyApiHandler()
        {
            wxConfigManager = new Models.WxQyConfigManager(),
            wxSessionManager = new WxSessionManager()
        };
    }
}