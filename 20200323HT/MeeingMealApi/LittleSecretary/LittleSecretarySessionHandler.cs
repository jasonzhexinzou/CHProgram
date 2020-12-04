using MeetingMealEntity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleSecretary
{
    /// <summary>
    /// 小秘书有状态接口
    /// </summary>
    public class LittleSecretarySessionHandler
    {
        readonly string consumer_key = ConfigurationManager.AppSettings["littlesecretary_consumer_key"];
        readonly string consumer_secret = ConfigurationManager.AppSettings["littlesecretary_consumer_secret"];

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
            var res = LittleSecretaryAPI.getToken(consumer_key, consumer_secret);
            if (res.code == "200")
            {
                token = res.result.token;
                tokenVersion++;
            }
            return res;
        }
        #endregion

        #region 同步城市
        /// <summary>
        /// 同步城市
        /// </summary>
        /// <returns></returns>
        public SyncCityResponse syncCity()
        {
            var res = LittleSecretaryAPI.syncCity(token, consumer_secret);
            if (IsTokenTimeout(res.code))
            {
                res = LittleSecretaryAPI.syncCity(token, consumer_secret);
            }
            return res;
        }
        #endregion

        #region 同步医院
        /// <summary>
        /// 同步医院
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public SyncHospitalResponse syncHospital(string cityId)
        {
            var res = LittleSecretaryAPI.syncHospital(token, consumer_secret, cityId);
            if (IsTokenTimeout(res.code))
            {
                res = LittleSecretaryAPI.syncHospital(token, consumer_secret, cityId);
            }
            return res;
        }
        #endregion

        #region 查询餐厅
        /// <summary>
        /// 查询餐厅
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <param name="address"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public QueryResResponse queryRes(string hospitalId, string address, string latitude, string longitude)
        {
            var res = LittleSecretaryAPI.queryRes(token, consumer_secret, hospitalId, address, latitude, longitude);
            if (IsTokenTimeout(res.code))
            {
                res = LittleSecretaryAPI.queryRes(token, consumer_secret, hospitalId, address, latitude, longitude);
            }
            return res;
        }
        #endregion

        #region 查询餐厅菜单
        /// <summary>
        /// 查询餐厅菜单
        /// </summary>
        /// <param name="resId"></param>
        /// <returns></returns>
        public QueryResFoodResponse queryResFood(string resId)
        {
            var res = LittleSecretaryAPI.queryResFood(token, consumer_secret, resId);
            if (IsTokenTimeout(res.code))
            {
                res = LittleSecretaryAPI.queryResFood(token, consumer_secret, resId);
            }
            return res;
        }
        #endregion

        #region 计算费用
        /// <summary>
        /// 计算费用
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <param name="resId"></param>
        /// <param name="foods"></param>
        /// <returns></returns>
        public CalculateFeeResponse calculateFee(string hospitalId, string resId, FoodRequest[] foods)
        {
            var res = LittleSecretaryAPI.calculateFee(token, consumer_secret, hospitalId, resId, foods);
            if (IsTokenTimeout(res.code))
            {
                res = LittleSecretaryAPI.calculateFee(token, consumer_secret, hospitalId, resId, foods);
            }
            return res;
        }
        #endregion

        #region 创建外卖订单
        /// <summary>
        /// 创建外卖订单
        /// </summary>
        /// <param name="enterpriseOrderId"></param>
        /// <param name="oldXMSOrderId"></param>
        /// <param name="typeId"></param>
        /// <param name="sendTime"></param>
        /// <param name="foodFee"></param>
        /// <param name="packageFee"></param>
        /// <param name="sendFee"></param>
        /// <param name="discountFee"></param>
        /// <param name="totalFee"></param>
        /// <param name="invoiceTitle"></param>
        /// <param name="orderTime"></param>
        /// <param name="remark"></param>
        /// <param name="dinnerName"></param>
        /// <param name="dinnernum"></param>
        /// <param name="cityId"></param>
        /// <param name="sex"></param>
        /// <param name="phone"></param>
        /// <param name="address"></param>
        /// <param name="resId"></param>
        /// <param name="resName"></param>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <param name="hospitalId"></param>
        /// <param name="cn"></param>
        /// <param name="cnAmount"></param>
        /// <param name="department"></param>
        /// <param name="mudId"></param>
        /// <param name="foods"></param>
        /// <returns></returns>
        public CreateOrderResponse createOrder(string enterpriseOrderId, string oldXMSOrderId, string typeId, string sendTime, string foodFee,
            string packageFee, string sendFee, string discountFee, string totalFee, string invoiceTitle, string orderTime, string remark,
            string dinnerName, string dinnernum, string cityId, string sex, string phone, string address, string resId, string resName,
            string longitude, string latitude, string hospitalId, string cn, string cnAmount, string department, string mudId, FoodRequest[] foods)
        {
            var res = LittleSecretaryAPI.createOrder(token, consumer_secret, enterpriseOrderId, oldXMSOrderId, typeId, sendTime, foodFee,
                packageFee, sendFee, discountFee, totalFee, invoiceTitle, orderTime, remark,
                dinnerName, dinnernum, cityId, sex, phone, address, resId, resName,
                longitude, latitude, hospitalId, cn, cnAmount, department, mudId, foods);
            if (IsTokenTimeout(res.code))
            {
                res = LittleSecretaryAPI.createOrder(token, consumer_secret, enterpriseOrderId, oldXMSOrderId, typeId, sendTime, foodFee,
                packageFee, sendFee, discountFee, totalFee, invoiceTitle, orderTime, remark,
                dinnerName, dinnernum, cityId, sex, phone, address, resId, resName,
                longitude, latitude, hospitalId, cn, cnAmount, department, mudId, foods);
            }
            return res;
        }
        #endregion

        #region 取消订单
        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="xmsOrderId"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public CancleOrderResponse cancleOrder(string xmsOrderId, string remark)
        {
            var res = LittleSecretaryAPI.cancleOrder(token, consumer_secret, xmsOrderId, remark);
            if (IsTokenTimeout(res.code))
            {
                res = LittleSecretaryAPI.cancleOrder(token, consumer_secret, xmsOrderId, remark);
            }
            return res;
        }
        #endregion

        #region 完成订单
        /// <summary>
        /// 完成订单
        /// </summary>
        /// <param name="xmsOrderId"></param>
        /// <param name="type"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public ResponseBase finishOrder(string xmsOrderId, string type, string remark)
        {
            var res = LittleSecretaryAPI.finishOrder(token, consumer_secret, xmsOrderId, type, remark);
            if (IsTokenTimeout(res.code))
            {
                res = LittleSecretaryAPI.finishOrder(token, consumer_secret, xmsOrderId, type, remark);
            }
            return res;
        }
        #endregion

        #region 订单未送达
        /// <summary>
        /// 订单未送达
        /// </summary>
        /// <param name="xmsOrderId"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public ResponseBase orderDeliveryFailure(string xmsOrderId, string remark)
        {
            var res = LittleSecretaryAPI.orderDeliveryFailure(token, consumer_secret, xmsOrderId, remark);
            if (IsTokenTimeout(res.code))
            {
                res = LittleSecretaryAPI.orderDeliveryFailure(token, consumer_secret, xmsOrderId, remark);
            }
            return res;
        }
        #endregion

        #region 取消订单失败反馈（原单配送）
        /// <summary>
        /// 取消订单失败反馈（原单配送）
        /// </summary>
        /// <param name="xmsOrderId"></param>
        /// <param name="type"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public ResponseBase cancleFailOrderFeedBack(string xmsOrderId, string type, string remark)
        {
            var res = LittleSecretaryAPI.cancleFailOrderFeedBack(token, consumer_secret, xmsOrderId, type, remark);
            if (IsTokenTimeout(res.code))
            {
                res = LittleSecretaryAPI.cancleFailOrderFeedBack(token, consumer_secret, xmsOrderId, type, remark);
            }
            return res;
        }
        #endregion

        #region 查询订单
        /// <summary>
        /// 查询订单
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="timeType"></param>
        /// <returns></returns>
        public OrderQueryResponse OrderQuery(string startTime, string endTime, string timeType)
        {
            var res = LittleSecretaryAPI.OrderQuery(token, consumer_secret, startTime, endTime, timeType);
            if (IsTokenTimeout(res.code))
            {
                res = LittleSecretaryAPI.OrderQuery(token, consumer_secret, startTime, endTime, timeType);
            }
            return res;
        }
        #endregion

        #region 日报 周报接口
        /// <summary>
        /// 日报 周报接口
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="timeType"></param>
        /// <returns></returns>
        public GetReportResponse GetReport(string startTime, string endTime, string timeType)
        {
            var res = LittleSecretaryAPI.GetReport(token, consumer_secret, startTime, endTime, timeType);
            if (IsTokenTimeout(res.code))
            {
                res = LittleSecretaryAPI.GetReport(token, consumer_secret, startTime, endTime, timeType);
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
        public GetSyncHospitalResRponse SyncHospitalRes(string province, string city)
        {
            var res = LittleSecretaryAPI.SyncHospitalRes(token, consumer_secret, province, city);
            if (IsTokenTimeout(res.code))
            {
                res = LittleSecretaryAPI.SyncHospitalRes(token, consumer_secret, province, city);
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
        public GetSyncResResponse SyncRes(string province, string city)
        {
            var res = LittleSecretaryAPI.SyncRes(token, consumer_secret, province, city);
            if (IsTokenTimeout(res.code))
            {
                res = LittleSecretaryAPI.SyncRes(token, consumer_secret, province, city);
            }
            return res;
        }
        #endregion

        #region XMS获取医院数据变量报告接口
        /// <summary>
        /// 获取医院数据变量报告接口
        /// </summary>

        /// <returns></returns>
        public GetSyncHospitalChangedRponse SyncHospitalChangedXMS()
        {
            var hos = LittleSecretaryAPI.SyncHospitalChangedXMS(token, consumer_secret);
            if (IsTokenTimeout(hos.code))
            {
                hos = LittleSecretaryAPI.SyncHospitalChangedXMS(token, consumer_secret);
            }
            return hos;
        }
        #endregion

    }

}
