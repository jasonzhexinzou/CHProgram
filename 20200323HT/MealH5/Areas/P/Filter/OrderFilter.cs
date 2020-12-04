using MealAdmin.Entity.Helper;
using MealH5.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MealH5.Areas.P.Filter
{
    public class OrderFilter : ActionFilterAttribute
    {
        #region 进入Action之前进行拦截
        /// <summary>
        /// 进入Action之前进行拦截
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpSessionStateBase Session = filterContext.HttpContext.Session;
            var order = Session[ConstantHelper.CURRENTPWECHATORDER] as P_WeChatOrder;

            if (order == null)
            {
                // 如果是ajax请求，应直接反馈json提示手动刷新页面
                if (filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest"
                || filterContext.HttpContext.Request["IsJsonCall"] == "1")
                {
                    filterContext.Result = new JsonResult()
                    {
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        Data = new
                        {
                            state = "timout",
                            txt = "操作失败！页面超时失效"
                        }
                    };
                }
                else
                {
                    ViewResult result = new ViewResult();
                    result.ViewName = "Exception";
                    result.ViewBag.msglevel = MsgLevelcs.warn;
                    result.ViewBag.info_message = "操作已超时";
                    result.ViewBag.needBackButton = false;
                    result.ViewBag.needCloseButton = true;

                    filterContext.Result = result;

                }
            }
        }
        #endregion
    }
}