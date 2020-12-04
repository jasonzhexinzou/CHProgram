using iPathAuthService.Models;
using iPathAuthService.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XFramework.WeChatAPI.Entity;
using XFramework.WeChatAPI.SessionHandlers;
using XFramework.XException;
using XFramework.XUtil;

namespace iPathAuthService.Controllers
{
    /// <summary>
    /// 短网址
    /// </summary>
    public class UrlMappingController : Controller
    {

        static private Dictionary<string, WxUserInfo> cacheUserInfo = new Dictionary<string, WxUserInfo>();

        void SetResponseFlag()
        {
            Response.Headers.Add("mic", "1");
        }

        #region 短网址映射入口
        /// <summary>
        /// 短网址映射入口
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ext"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public RedirectResult Index(string id, string ext, string callback)
        {
            SetResponseFlag();

            Session["callback"] = callback;
            string appid = GlobalHandler.QyApiHandler.wxConfigManager.CorpID;
            string redirect_uri = HttpUtility.UrlEncode(ConfigurationManager.AppSettings["WebSiteUrl"] + "/UrlMapping/OAuth2Wx");
            string state = id;
            if (!string.IsNullOrEmpty(ext))
            {
                state += "|" + ext;
            }
            var oauth2url = $"{"https://"}open.weixin.qq.com/connect/oauth2/authorize?appid={appid}&redirect_uri={redirect_uri}&response_type=code&scope=snsapi_base&state={state}#wechat_redirect";
            return Redirect(oauth2url);
        }
        #endregion

        #region 微信OAuth2授权获取当前登录人信息
        /// <summary>
        /// 微信OAuth2授权获取当前登录人信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public RedirectResult OAuth2Wx(string code, string state)
        {
            SetResponseFlag();
            var gotoCount = 0;
            LAB_FUNCTION_START:
            var rel = GlobalHandler.QyApiHandler.User_GetUserInfo(code);
            if (rel == null)
            {
                LogHelper.Info("rel为空");
            }

            var key = Guid.NewGuid().ToString();
            var redirect_url = "";
            if (rel.errcode == "0")
            {
                string id = string.Empty;
                string ext = string.Empty;
                if (state.Contains('|'))
                {
                    var state_arr = state.Split('|');
                    id = state_arr[0];
                    ext = state_arr[1];
                }
                else
                {
                    id = state;
                }

                cacheUserInfo.Add(key, new WxUserInfo()
                {
                    errcode = 1,
                    userId = rel.UserId,
                    openId = rel.OpenId,
                    deviceId = rel.DeviceId,
                    ext = ext
                });

                var callback = Session["callback"] as string;
                if (string.IsNullOrEmpty(callback))
                {
                    redirect_url = $"{UrlMappingConfig.Setting[id].Value}user_ticket={key}";
                }
                else
                {
                    callback = GenerateRedirectUrl(callback);
                    redirect_url = $"{callback}user_ticket={key}";
                }
                Session.Clear();
            }
            else
            {
                GlobalHandler.QyApiHandler.wxSessionManager = new WxSessionManager();
                LogHelper.Info($"WXAPIAccessToken超时:{rel.errcode}：{rel.errmsg}");
                gotoCount++;
                if (gotoCount < 10)
                {
                    goto LAB_FUNCTION_START;
                }
                throw new BusinessBaseException("9000", new Exception($"WXAPIAccessToken超时:{rel.errcode}：{rel.errmsg}"));
            }
                
            return Redirect(redirect_url);
        }

        private static string GenerateRedirectUrl(string url)
        {
            if (url.Contains("?"))
            {
                if (url.EndsWith("?") || url.EndsWith("&"))
                {
                    return url;
                }
                else
                {
                    return url + "&";
                }
            }
            else
            {
                return url + "?";
            }


        }
        #endregion

        #region 根据用户凭据获取用户信息
        /// <summary>
        /// 根据用户凭据获取用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetUserInfoByTicket(string id)
        {
            if (cacheUserInfo.ContainsKey(id))
            {
                var userInfo = cacheUserInfo[id];
                try
                {
                    cacheUserInfo.Remove(id);
                }
                catch { }
                return Json(new { state = 1, userinfo = userInfo }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { state = 0, txt = "凭据无效" }, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}