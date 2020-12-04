using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XFramework.XUtil;

namespace MealApiInterface
{
    public class OpenApiBase
    {
        public virtual string WEBCLIENTTHREADKEY
        {
            get
            {
                return "WEBCLIENTTHREADKEY";
            }
        }

        #region GET方式请求一个字符串
        /// <summary>
        /// GET方式请求一个字符串
        /// </summary>
        /// <param name="url"></param>
        /// <param name="pu"></param>
        /// <returns></returns>
        public string GetString(string url, Dictionary<string, string> pu)
        {
            var param = string.Join("&", pu.Select(a => a.Key + "=" + a.Value));
            var wc = ThreadWebClientFactory.GetWebClient(WEBCLIENTTHREADKEY);
            wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            var rel = wc.GetLoadData(url, param);
            var json = Encoding.UTF8.GetString(rel);
            return json;
        }
        #endregion

        #region GET方式请求对象
        /// <summary>
        /// GET方式请求对象
        /// </summary>
        /// <param name="url"></param>
        /// <param name="pu"></param>
        /// <returns></returns>
        public T GetObject<T>(string url, Dictionary<string, string> pu)
        {
            var json = GetString(url, pu);
            return JsonConvert.DeserializeObject<T>(json);
        }
        #endregion

        #region POST方式请求一个字符串
        /// <summary>
        /// POST方式请求一个字符串
        /// </summary>
        /// <param name="url"></param>
        /// <param name="pu"></param>
        /// <returns></returns>
        public string PostString(string url, Dictionary<string, string> pu)
        {
            Dictionary<string, string> gets = new Dictionary<string, string>();
            gets.Add("timestamp", pu["timestamp"]);
            gets.Add("consumer_key", pu["consumer_key"]);
            gets.Add("sig", pu["sig"]);

            pu.Remove("timestamp");
            pu.Remove("consumer_key");
            pu.Remove("sig");

            var urlparam = string.Join("&", gets.Select(a => a.Key + "=" + a.Value));

            var param = string.Join("&", pu.Select(a => a.Key + "=" + a.Value));
            var wc = ThreadWebClientFactory.GetWebClient(WEBCLIENTTHREADKEY);
            wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            var rel = wc.PostLoadData(url + "?" + urlparam, param);
            var json = Encoding.UTF8.GetString(rel);
            return json;
        }
        #endregion

        #region POST方式请求对象
        /// <summary>
        /// POST方式请求对象
        /// </summary>
        /// <param name="url"></param>
        /// <param name="pu"></param>
        /// <returns></returns>
        public T PostObject<T>(string url, Dictionary<string, string> pu)
        {
            var json = PostString(url, pu);
            return JsonConvert.DeserializeObject<T>(json); ;
        }
        #endregion

        #region PUT方式请求一个字符串
        /// <summary>
        /// PUT方式请求一个字符串
        /// </summary>
        /// <param name="url"></param>
        /// <param name="pu"></param>
        /// <returns></returns>
        public string PutString(string url, Dictionary<string, string> pu)
        {
            Dictionary<string, string> gets = new Dictionary<string, string>();
            gets.Add("timestamp", pu["timestamp"]);
            gets.Add("consumer_key", pu["consumer_key"]);
            gets.Add("sig", pu["sig"]);

            pu.Remove("timestamp");
            pu.Remove("consumer_key");
            pu.Remove("sig");

            var urlparam = string.Join("&", gets.Select(a => a.Key + "=" + a.Value));

            var param = string.Join("&", pu.Select(a => a.Key + "=" + a.Value));
            var wc = ThreadWebClientFactory.GetWebClient(WEBCLIENTTHREADKEY);
            wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            var rel = wc.PutLoadData(url + "?" + urlparam, param);
            var json = Encoding.UTF8.GetString(rel);
            return json;
        }
        #endregion

        #region PUT方式请求对象
        /// <summary>
        /// PUT方式请求对象
        /// </summary>
        /// <param name="url"></param>
        /// <param name="pu"></param>
        /// <returns></returns>
        public T PutObject<T>(string url, Dictionary<string, string> pu)
        {
            var json = PutString(url, pu);
            return JsonConvert.DeserializeObject<T>(json); ;
        }
        #endregion
    }
}
