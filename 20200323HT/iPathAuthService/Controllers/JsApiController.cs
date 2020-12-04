using iPathAuthService.Util;
using MealAdminApiClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XFramework.XUtil;
namespace iPathAuthService.Controllers
{
    public class JsApiController : Controller
    {
        #region JsApi测试页面
        /// <summary>
        /// JsApi测试页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var url = "http://meal.dev.imtpath.com/JsApi/Index";
            string noncestr = Guid.NewGuid().ToString();
            string timestamp = DateTimeHelper.NowToJavaTimeMillis().ToString();
            string signature = GlobalHandler.QyApiHandler.GetJsapiSignature(noncestr, timestamp, url);

            ViewBag.appId = GlobalHandler.QyApiHandler.wxConfigManager.CorpID;
            ViewBag.noncestr = noncestr;
            ViewBag.timestamp = timestamp;
            ViewBag.url = url;
            ViewBag.signature = signature;

            return View();
        }
        #endregion

        #region 页面Js权限注入
        /// <summary>
        /// 页面Js权限注入
        /// </summary>
        /// <param name="url"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public ActionResult QyConfigJs(string url, string debug)
        {
            LogHelper.Info("url:" + url);
            string noncestr = Guid.NewGuid().ToString();
            string timestamp = DateTimeHelper.NowToJavaTimeMillis().ToString();
            var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
            
            var ticket = baseDataChannel.GetJsApiTicket();

            LogHelper.Info("ticket=" + ticket.Signature);

            if (DateTime.Compare(ticket.Timestamp, DateTime.Now) > 0)
            {
                string[] brr = new string[]
                  {
                      "noncestr=" + noncestr,
                      "jsapi_ticket=" + ticket.Signature,
                      "timestamp=" + timestamp,
                      "url=" + url
                  };
                Array.Sort(brr);
                string p1 = string.Join("&", brr);
                string signature = SHA1Helper.SHA1_Hash(p1);

                ViewBag.debug = debug;
                ViewBag.appId = GlobalHandler.QyApiHandler.wxConfigManager.CorpID;
                ViewBag.noncestr = noncestr;
                ViewBag.timestamp = timestamp;
                ViewBag.url = url;
                ViewBag.signature = signature;
            }
            else
            {
                //string signature = GlobalHandler.QyApiHandler.GetJsapiSignature(noncestr, timestamp, url);
                var jsap_ticket = GlobalHandler.QyApiHandler.Get_jsapi_ticket();
                baseDataChannel.UpdateJsApiTicket(jsap_ticket.ticket, DateTime.Now.AddSeconds(7200).ToString());
                string[] arr = new string[]
                  {
                      "noncestr=" + noncestr,
                      "jsapi_ticket=" + jsap_ticket.ticket,
                      "timestamp=" + timestamp,
                      "url=" + url
                  };
                Array.Sort(arr);
                string p1 = string.Join("&", arr);
                string signature = SHA1Helper.SHA1_Hash(p1);
                
                ViewBag.debug = debug;
                ViewBag.appId = GlobalHandler.QyApiHandler.wxConfigManager.CorpID;
                ViewBag.noncestr = noncestr;
                ViewBag.timestamp = timestamp;
                ViewBag.url = url;
                ViewBag.signature = signature;
            }
            return View();
        }
        #endregion

        #region 返回JsApi配置参数
        /// <summary>
        /// 返回JsApi配置参数
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public JsonResult QyConfig(string url)
        {
            LogHelper.Info("url=" + url);
            string noncestr = Guid.NewGuid().ToString();
            string timestamp = DateTimeHelper.NowToJavaTimeMillis().ToString();
            //string signature = GlobalHandler.QyApiHandler.GetJsapiSignature(noncestr, timestamp, url);

            var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
            var ticket = baseDataChannel.GetJsApiTicket();
            if (DateTime.Compare(ticket.Timestamp, DateTime.Now) > 0)
            {
                string[] arr = new string[]
                 {
                     "noncestr=" + noncestr,
                     "jsapi_ticket=" + ticket,
                     "timestamp=" + timestamp,
                     "url=" + url
                 };
                Array.Sort(arr);
                string p1 = string.Join("&", arr);
                string signature = SHA1Helper.SHA1_Hash(p1);

                return Json(
                new
                {
                    state = 1,
                    config =
                    new
                    {
                        appId = GlobalHandler.QyApiHandler.wxConfigManager.CorpID,
                        timestamp = timestamp,
                        nonceStr = noncestr,
                        signature = signature
                    }
                });
            }
            else
            {
                var jsap_ticket = GlobalHandler.QyApiHandler.Get_jsapi_ticket();

                baseDataChannel.UpdateJsApiTicket(jsap_ticket.ticket, DateTime.Now.AddSeconds(7200).ToString());

                string[] arr = new string[]
                 {
                     "noncestr=" + noncestr,
                     "jsapi_ticket=" + jsap_ticket.ticket,
                     "timestamp=" + timestamp,
                     "url=" + url
                 };
                Array.Sort(arr);
                string p1 = string.Join("&", arr);
                string signature = SHA1Helper.SHA1_Hash(p1);
                
                return Json(
               new
               {
                   state = 1,
                   config =
                   new
                   {
                       appId = GlobalHandler.QyApiHandler.wxConfigManager.CorpID,
                       timestamp = timestamp,
                       nonceStr = noncestr,
                       signature = signature
                   }
               });
            }
           
        }
        #endregion
    }
}