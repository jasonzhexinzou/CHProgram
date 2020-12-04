using MealAdmin.Entity;
using MeetingMealEntity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkBDS
{
    /// <summary>
    /// BDS有状态接口
    /// </summary>
    public class LinkBDSSessionHandler
    {
        readonly string consumer_key = ConfigurationManager.AppSettings["linkbds_consumer_key"];
        readonly string consumer_secret = ConfigurationManager.AppSettings["linkbds_consumer_secret"];

        string token = string.Empty;
        int tokenVersion = 0;

        #region 获取当前最新版本的token
        /// <summary>
        /// 获取当前最新版本的token
        /// </summary>
        /// <returns></returns>
        private string GetToken()
        {
            if (string.IsNullOrEmpty(token))
            {
                return RefreshToken(tokenVersion);
            }
            return token;
        }
        #endregion

        private object obj = new object();

        #region 刷新token
        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="oldVersion"></param>
        /// <returns></returns>
        private string RefreshToken(int oldVersion)
        {
            lock (obj)
            {
                if (oldVersion < tokenVersion)
                {
                    return token;
                }
                else
                {
                    getToken(consumer_key, consumer_secret);
                    return token;
                }
            }
        }
        #endregion

        #region 判断token是否超时
        /// <summary>
        /// 判断token是否超时
        /// </summary>
        /// <param name="errCode"></param>
        /// <returns></returns>
        private bool IsTokenTimeout(string errCode)
        {
            if (errCode == "304" || errCode == "306")
            {
                RefreshToken(tokenVersion);
                return true;
            }
            return false;
        }
        #endregion

        #region 获取token
        /// <summary>
        /// 获取token
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public GetTokenResponse getToken(string appId, string secret)
        {
            var res = LinkBDSAPI.getToken(consumer_key, consumer_secret);
            if (res.code == "200")
            {
                token = res.result.token;
                tokenVersion++;
            }
            return res;
        }
        #endregion

        #region 获取可送餐列表接口
        /// <summary>
        /// 获取可送餐列表接口
        /// </summary>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <returns></returns>
        public GetSyncHospitalResRponse SyncHospitalResBDS(string province, string city)
        {
            var res = LinkBDSAPI.SyncHospitalResBDS(token, consumer_secret, province, city);
            if (IsTokenTimeout(res.code))
            {
                res = LinkBDSAPI.SyncHospitalResBDS(token, consumer_secret, province, city);
            }
            return res;
        }
        #endregion

        #region 获取餐厅表接口
        /// <summary>
        /// 获取餐厅表接口
        /// </summary>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <returns></returns>
        public GetSyncResResponse SyncResBDS(string province, string city)
        {
            var res = LinkBDSAPI.SyncResBDS(token, consumer_secret, province, city);
            if (IsTokenTimeout(res.code))
            {
                res = LinkBDSAPI.SyncResBDS(token, consumer_secret, province, city);
            }
            return res;
        }
        #endregion

        #region BDS获取医院数据变量报告接口
        /// <summary>
        /// 获取医院数据变量报告接口
        /// </summary>

        /// <returns></returns>
        public GetSyncHospitalChangedRponse SyncHospitalChanged()
        {
            var hos = LinkBDSAPI.SyncHospitalChanged(token, consumer_secret);
            if (IsTokenTimeout(hos.code))
            {
                hos = LinkBDSAPI.SyncHospitalChanged(token, consumer_secret);
            }
            return hos;
        }
        #endregion

    }

}
