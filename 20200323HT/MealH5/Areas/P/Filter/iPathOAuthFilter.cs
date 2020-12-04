using MealAdmin.Entity;
using MealAdmin.Entity.Enum;
using MealAdminApiClient;
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

namespace MealH5.Areas.P.Filter
{
    public class iPathOAuthFilter : ActionFilterAttribute
    {
        public string MappingKey { get; set; }
        public bool CallBackUrl { get; set; }

        #region 进入Action之前进行拦截
        /// <summary>
        /// 进入Action之前进行拦截
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Headers.Add("mic", "1");

            var uri = filterContext.HttpContext.Request.AppRelativeCurrentExecutionFilePath;

            HttpSessionStateBase Session = filterContext.HttpContext.Session;
            var wxUser = Session[ConstantHelper.CURRENTWXUSER] as P_USERINFO;

            if (wxUser == null || string.IsNullOrEmpty(wxUser.UserId))
            {

                var user_ticket = filterContext.HttpContext.Request["user_ticket"];
                if (string.IsNullOrEmpty(user_ticket))
                {
                    // 还未通过oauth2取得openid

                    var oauth2url = $"{ConfigurationManager.AppSettings["ShortUrlService"]}/s/{MappingKey}";
                    if (CallBackUrl)
                    {
                        var nowUrl = System.Configuration.ConfigurationManager.AppSettings["CurrentRootDomain"] + filterContext.HttpContext.Request.Url.PathAndQuery;
                        oauth2url += $"?callback={HttpUtility.UrlEncode(nowUrl)}";
                    }

                    filterContext.Result = new RedirectResult(oauth2url);
                }
                else
                {
                    var getUserInfoUrl = $"{ConfigurationManager.AppSettings["iPathAuthService"]}/UrlMapping/GetUserInfoByTicket/{user_ticket}";
                    LogHelper.Info(getUserInfoUrl);

                    var json = ThreadWebClientFactory.GetWebClient().DownloadString(getUserInfoUrl);
                    LogHelper.Info(json);
                    var userInfoRes = JsonConvert.DeserializeObject<WxUserInfoRes>(json);
                    if (userInfoRes.state == 1 && userInfoRes.userinfo.errcode == 1)
                    {
                        var channelUser = UserInfoClientChannelFactory.GetChannel();
                        var channelBase = BaseDataClientChannelFactory.GetChannel();

                        var userId = userInfoRes.userinfo.userId;
                        var userInfo = channelUser.FindByUserId(userId);
                        if (userInfo == null)
                        {
                            var res1 = channelUser.Find(userId);

                            P_USERINFO entity = new P_USERINFO();
                            entity.ID = Guid.NewGuid();
                            entity.UserId = userId;
                            entity.Name = res1.Name;
                            entity.CreateDate = DateTime.Now;
                            var _res = channelUser.Add(entity);

                            userInfo = channelUser.FindByUserId(userId);
                        }

                        var listGroup = channelBase.LoadUserGroup(userId).Select(a => a.GroupType).ToArray();

                        if (listGroup.Contains(GroupType.ServPause))
                        {
                            userInfo.IsServPause = 1;
                        }
                        if (listGroup.Contains(GroupType.OutSideHT))
                        {
                            userInfo.IsOutSideHT = 1;
                        }


                        Session[ConstantHelper.CURRENTWXUSER] = userInfo;
                        var url = filterContext.HttpContext.Request.RawUrl;
                        if (url.Contains("user_ticket"))
                        {
                            url = url.Replace($"user_ticket={user_ticket}", string.Empty);
                        }

                        filterContext.Result = new RedirectResult(url);
                    }
                    else
                    {
                        // 没有权限
                        filterContext.Result = new ContentResult()
                        {
                            Content = "您无使用此功能的权限"
                        };
                    }
                }

            }
            else
            {
                var IsPublishProgram = ConfigurationManager.AppSettings["IsPublishProgram"];
                var SystemCoders = ConfigurationManager.AppSettings["SystemCoders"];
                List<string> Coders = new List<string>();
                if (!string.IsNullOrEmpty(SystemCoders))
                {
                    Coders = SystemCoders.Split(';').ToList();
                }
                if (IsPublishProgram == "1" && !Coders.Contains(wxUser.UserId))
                {
                    var LocalService = ConfigurationManager.AppSettings["LocalService"];
                    filterContext.Result = new RedirectResult(LocalService + "/P/Publish/Index");
                }
            }
        }
        #endregion

    }
}