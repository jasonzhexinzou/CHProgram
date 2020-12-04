using MeetingMealEntity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XFramework.XUtil;

namespace LittleSecretary
{
    public class LittleSecretaryBase
    {
        static LittleSecretaryBase()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                    (sender, certificate, chain, errors) => true;

            LogHelper.Init();
        }
        
        #region 为业务参数签名
        /// <summary>
        /// 为业务参数签名
        /// </summary>
        /// <param name="param"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static Dictionary<string, string> Sign(Dictionary<string, string> param, string secret)
        {
            var timestamp = XFramework.XUtil.DateTimeHelper.NowToJavaTimeMillis().ToString();
            if (param.ContainsKey("timestamp"))
            {
                param["timestamp"] = timestamp;
            }
            else
            {
                param.Add("timestamp", timestamp);
            }
            param.Add("secret", secret);

            var pu = param.OrderBy(a => a.Key).ToDictionary(a => a.Key, b => b.Value);
            var stringA = string.Join("&", pu.Select(a => a.Key + "=" + a.Value));
            var stringB = stringA.ToLower();

            var sign = XFramework.XUtil.SHA1Helper.SHA1_Hash_Encoding(stringB);

            pu.Remove("secret");
            pu.Add("sign", sign);

            return pu;
        }
        #endregion

        #region 制作请求报文体
        /// <summary>
        /// 制作请求报文体
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string GenerateRequestBody(object param)
        {
            var post_json = JsonConvert.SerializeObject(param);
            string post_json1 = post_json.Replace("\"[", "[").Replace("]\"", "]");
            return post_json = "{\"requestData\":" + post_json1.Replace("\\", "") + "}";
            //return post_json = "{\"requestData\":" + post_json + "}";
        }
        #endregion

        #region POST方式提交
        /// <summary>
        /// POST方式提交
        /// </summary>
        /// <param name="url"></param>
        /// <param name="post_json"></param>
        /// <returns></returns>
        private static string Post(string url, string post_json)
        {
            var wc = XFramework.XUtil.ThreadWebClientFactory.GetWebClient();
            wc.Encoding = Encoding.UTF8;
            wc.TimeOut = 600000;
            var i = DateTime.Now.Ticks;
            LogHelper.Info($"[{i}] - URL:[{url}] - POST:[{post_json}]");
            var res = wc.PostLoadString(url, post_json);
            LogHelper.Info($"[{i}] - RES:[{res}]");
            return res;
        }
        #endregion

        #region POST方式请求接口
        /// <summary>
        /// POST方式请求接口
        /// </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static T Post<T>(string url, Dictionary<string, string> param, string secret)
        {
            var pu = Sign(param, secret);
            var post_json = GenerateRequestBody(pu);
            var rel = Post(url, post_json);
            return JsonConvert.DeserializeObject<T>(rel);
        }
        #endregion

        #region 序列化对象为签名键值对
        /// <summary>
        /// 序列化对象为签名键值对
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="ht"></param>
        /// <param name="MyType"></param>
        /// <param name="sFieldName"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetKeyValue<T>(T obj, Dictionary<string, string> ht, Type MyType = null, string sFieldName = "")
        {
            if (MyType == null)
            {
                MyType = typeof(T);
            }
            foreach (PropertyInfo pi in MyType.GetProperties())
            {
                string sName = pi.Name;
                if ("sign".Equals(sName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                if ((pi.PropertyType.IsGenericType || pi.PropertyType.IsArray))
                {
                    Array list = (Array)pi.GetValue(obj, null);
                    int nNum = 0;
                    foreach (var o in list)
                    {
                        GetKeyValue(o, ht, o.GetType(), sName + "_" + nNum);
                        nNum++;
                    }
                }
                else
                {
                    object objTmp = pi.GetValue(obj, null);
                    string sValue = string.Empty;
                    if (objTmp != null)
                    {
                        sValue = objTmp.ToString();
                    }
                    if (string.IsNullOrEmpty(sFieldName))
                    {
                        ht.Add(sName, sValue);
                    }
                    else
                    {
                        ht.Add(sName + "_" + sFieldName, sValue);
                    }

                }
            }
            return ht;
        }
        #endregion

        #region POST方式请求接口
        /// <summary>
        /// POST方式请求接口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="request"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static T Post<T, K>(string url, K request, string secret) where K : RequestBase
        {
            var param = new Dictionary<string, string>();
            param = GetKeyValue(request, param);
            var pu = Sign(param, secret);
            request.sign = pu["sign"];
            request.timestamp = pu["timestamp"];
            var post_json = GenerateRequestBody(request);
            var rel = Post(url, post_json);
            return JsonConvert.DeserializeObject<T>(rel);
        }
        #endregion


    }
}
