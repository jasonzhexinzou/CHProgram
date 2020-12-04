using IamPortal.AppLogin;
using MealAdmin.Util;
using MealAdmin.Web.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XFramework.XException;
using XFramework.XInject.Attributes;
using XFramework.XUtil;

namespace MealAdmin.Web.Areas.Admin.Controllers
{
    public class HomeController : AdminBaseController
    {
        #region 后台首页
        /// <summary>
        /// 后台首页
        /// </summary>
        /// <returns></returns>
        public ContentResult Index()
        {
            return Content("<h1>It's working!</h1>");
        }
        #endregion

        #region 预登录
        /// <summary>
        /// 预登录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public RedirectResult PerLogin()
        {
            Session.Clear();
            var random = Guid.NewGuid().ToString();
            Session["LoginRandomStr"] = random;

            return Redirect(WebConfig.AppLoginServer + "?random=" + random);
        }
        #endregion

        #region 登录
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="security"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(string security)
        {
            var random = Session["LoginRandomStr"] as string;

            var loginUserInfo = LoginHelper.DecodingUserInfo(security, WebConfig.AppLoginSecret);
            if (loginUserInfo.random != random)
            {
                throw new BusinessBaseException(ExceptionCode.NoPermission);
            }

            var admin = LoginHelper.FindAdminUser(loginUserInfo.userid, WebConfig.AppLoginSecret, WebConfig.IamServer);
            if (admin != null && admin.ListPermissions.Contains(WebConfig.IamAppID))
            {
                CurrentAdminUser = admin;
                Session[ConstantHelper.CurrentPermission] = admin.ListPermissions;
                return RedirectToAction("Main");
            }
            else
            {
                throw new BusinessBaseException(ExceptionCode.NoPermission);
            }

        }
        #endregion

        #region 后台主页
        /// <summary>
        /// 后台主页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AdminSessionFilter]
        public ActionResult Main()
        {
            return View("PMain");
        }
        #endregion

    }
}