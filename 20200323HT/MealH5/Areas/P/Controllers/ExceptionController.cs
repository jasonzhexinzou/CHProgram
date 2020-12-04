using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MealH5.Areas.P.Controllers
{
    public class ExceptionController : Controller
    {
        #region 提示消息
        /// <summary>
        /// 提示消息
        /// </summary>
        /// <returns></returns>
        public ActionResult Info()
        {
            ViewBag.msglevel = "info";
            return View("Exception");
        }
        #endregion

        #region 成功消息
        /// <summary>
        /// 成功消息
        /// </summary>
        /// <returns></returns>
        public ActionResult Success()
        {
            ViewBag.msglevel = "success";
            return View("Exception");
        }
        #endregion

        #region 警告消息
        /// <summary>
        /// 警告消息
        /// </summary>
        /// <returns></returns>
        public ActionResult Warn()
        {
            ViewBag.msglevel = "warn";
            return View("Exception");
        }
        #endregion

        #region 危险消息
        /// <summary>
        /// 危险消息
        /// </summary>
        /// <returns></returns>
        public ActionResult Danger()
        {
            ViewBag.msglevel = "danger";
            return View("Exception");
        }
        #endregion

        #region 等待消息
        /// <summary>
        /// 等待消息
        /// </summary>
        /// <returns></returns>
        public ActionResult Waiting()
        {
            ViewBag.msglevel = "waiting";
            return View("Exception");
        }
        #endregion 

    }
}