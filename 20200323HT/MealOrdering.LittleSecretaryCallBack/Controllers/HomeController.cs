using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XFramework.XUtil;

namespace MealOrdering.LittleSecretaryCallBack.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        #region 修改订单
        /// <summary>
        /// 修改订单
        /// </summary>
        /// <returns></returns>
        public JsonResult OrderStateChange()
        {
            StreamReader reader = new StreamReader(Request.InputStream);
            var req = reader.ReadToEnd();
            LogHelper.Info(req);
            return Json(new { code  = 200, message  = "ok"});
        }
        #endregion

        #region 订单金额修改
        /// <summary>
        /// 订单金额修改
        /// </summary>
        /// <returns></returns>
        public JsonResult OrderFeeChange()
        {
            StreamReader reader = new StreamReader(Request.InputStream);
            var req = reader.ReadToEnd();
            LogHelper.Info(req);
            return Json(new { code = 200, message = "ok" });
        }
        #endregion
    }
}