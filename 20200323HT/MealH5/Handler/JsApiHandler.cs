using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using XFramework.XUtil;

namespace MealH5.Handler
{
    public class JsApiHandler
    {
        #region 获取企业号JsApi调用凭据config
        /// <summary>
        /// 获取企业号JsApi调用凭据config
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static GetQyApiConfigRes GetQyJsApiConfig(string url)
        {
            url = HttpUtility.UrlEncode(url);
            var json = ThreadWebClientFactory.GetWebClient()
                            .DownloadString($"{ConfigurationManager.AppSettings["ShortUrlService"]}/JsApi/Index?url=" + url);

            var res = JsonConvert.DeserializeObject<GetQyApiConfigRes>(json);
            return res;
        }
        #endregion

        public class GetQyApiConfigRes
        {
            public int state { get; set; }
            public QyApiConfig config { get; set; }
        }

        public class QyApiConfig
        {
            public string appId { get; set; }
            public string timestamp { get; set; }
            public string nonceStr { get; set; }
            public string signature { get; set; }
        }

    }
}