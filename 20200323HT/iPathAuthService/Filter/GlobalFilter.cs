using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace iPathAuthService.Filter
{
    public class GlobalFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            #region 应对AppScan而增加的判断
            // 1. AppScan认为路径中凡是包含“.1”的都是临时文件，应该反馈404
            if (filterContext.HttpContext.Request.RawUrl.EndsWith(".1")
                || filterContext.HttpContext.Request.RawUrl.EndsWith(".bak")
                || filterContext.HttpContext.Request.RawUrl.EndsWith("/bin")
                || filterContext.HttpContext.Request.RawUrl.EndsWith("/bin/")
            )
            {
                filterContext.HttpContext.Response.StatusCode = 404;
                OverAction(filterContext);
            }
            #endregion

            filterContext.HttpContext.Response.Buffer = true;
            filterContext.HttpContext.Response.ExpiresAbsolute = DateTime.Now.AddDays(-1);
            filterContext.HttpContext.Response.Cache.SetExpires(DateTime.Now.AddDays(-1));
            filterContext.HttpContext.Response.Expires = 0;
            filterContext.HttpContext.Response.CacheControl = "no-cache";
            filterContext.HttpContext.Response.Cache.SetNoStore();
        }

        #region 终止请求
        /// <summary>
        /// 终止请求
        /// </summary>
        /// <param name="filterContext"></param>
        public void OverAction(ActionExecutingContext filterContext)
        {
            if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
                filterContext.Result = new ContentResult() { Content = "" };
            }
        }
        #endregion

    }
}