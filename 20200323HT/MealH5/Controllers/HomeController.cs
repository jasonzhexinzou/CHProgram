using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MealH5.Controllers
{
    public class HomeController : Controller
    {
        #region 默认请求返回ok字符串
        /// <summary>
        /// 默认请求返回ok字符串
        /// </summary>
        /// <returns></returns>
        public ContentResult Index(string id, string random)
        {
            var Referer = Request.Headers["Referer"];

            return Content("ok");
        }
        #endregion
    }
}