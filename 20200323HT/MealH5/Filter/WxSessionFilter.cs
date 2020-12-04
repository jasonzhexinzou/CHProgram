using MealH5.Models;
using MealH5.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XFramework.XUtil;

namespace MealH5.Filter
{
    /// <summary>
    /// 微信身份认证
    /// </summary>
    public class WxSessionFilter : ActionFilterAttribute
    {
        public string MappingKey { get; set; }

        #region 进入Action之前进行拦截
        /// <summary>
        /// 进入Action之前进行拦截
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpSessionStateBase Session = filterContext.HttpContext.Session;
            string userId = Session[ConstantHelper.CurrentOpenId] as string;
            if (string.IsNullOrEmpty(userId))
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
                            state = 0,
                            txt = "操作失败！页面超时失效，请重新打开当前页面"
                        }
                    };
                }
                else
                {
                    var user_ticket = filterContext.HttpContext.Request["user_ticket"];
                    if (string.IsNullOrEmpty(user_ticket))
                    {
                        // 还未通过oauth2取得openid
                        var oauth2url = $"{ConfigurationManager.AppSettings["ShortUrlService"]}/s/{MappingKey}";
                        filterContext.Result = new RedirectResult(oauth2url);
                    }
                    else
                    {
                        var json = ThreadWebClientFactory.GetWebClient()
                            .DownloadString($"{ConfigurationManager.AppSettings["ShortUrlService"]}/UrlMapping/GetUserInfoByTicket/{user_ticket}");
                        var userInfoRes = JsonConvert.DeserializeObject<WxUserInfoRes>(json);
                        if (userInfoRes.state == 1 && userInfoRes.userinfo.errcode == 1)
                        {
                            Session[ConstantHelper.CurrentOpenId] = userInfoRes.userinfo.userId;
                        }
                        else
                        {
                            // 没有权限
                            filterContext.HttpContext.Response.Write("您无使用此功能的权限");
                        }
                    }
                    
                }
            }
        }
        #endregion
    }
}