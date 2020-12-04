using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using MeetingMealEntity;
using XFramework.XUtil;
using MealAdmin.Entity;
using MealAdmin.Entity.Helper;

namespace MeetingMealApiService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“OpenApi”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 OpenApi.svc 或 OpenApi.svc.cs，然后开始调试。

    public class OpenApi : IOpenApi
    {
        static LittleSecretary.LittleSecretarySessionHandler handler
            = new LittleSecretary.LittleSecretarySessionHandler();

        static LinkBDS.LinkBDSSessionHandler handlerBDS
            = new LinkBDS.LinkBDSSessionHandler();

        #region 同步城市
        /// <summary>
        /// 同步城市
        /// </summary>
        /// <returns></returns>
        public SyncCityResponse syncCity()
        {
            SyncCityResponse res = null;
            try
            {
                res = handler.syncCity();
            }
            catch (Exception ex)
            {
                LogHelper.Error("OpenApi.syncCity", ex);
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
            SyncHospitalResponse res = null;
            try
            {
                res = handler.syncHospital(cityId);
            }
            catch (Exception ex)
            {
                LogHelper.Error("OpenApi.syncHospital", ex);
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
            return handler.queryRes(hospitalId, address, latitude, longitude);
        }
        #endregion

        #region 查询餐厅菜品
        /// <summary>
        /// 查询餐厅菜品
        /// </summary>
        /// <param name="resId"></param>
        /// <returns></returns>
        public QueryResFoodResponse queryResFood(string resId)
        {
            QueryResFoodResponse res = null;
            try
            {
                res = handler.queryResFood(resId);
            }
            catch (Exception ex)
            {
                LogHelper.Error("OpenApi.queryResFood", ex);
            }
            return res;
        }
        #endregion

        #region 预算费用
        /// <summary>
        /// 预算费用
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <param name="resId"></param>
        /// <param name="foods"></param>
        /// <returns></returns>
        public CalculateFeeResponse calculateFee(string hospitalId, string resId, FoodRequest[] foods)
        {
            CalculateFeeResponse res = null;
            try
            {
                res = handler.calculateFee(hospitalId, resId, foods);
            }
            catch (Exception ex)
            {
                LogHelper.Error("OpenApi.calculateFee", ex);
            }
            return res;
        }
        #endregion

        #region 下单
        /// <summary>
        /// 下单
        /// </summary>
        /// <param name="enterpriseOrderId"></param>
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
            string longitude, string latitude, string hospitalId, string cn, string cnAmount, string department, string mudId, MeetingMealEntity.FoodRequest[] foods)
        {
            CreateOrderResponse res = null;
            try
            {
                res = handler.createOrder(enterpriseOrderId, oldXMSOrderId, typeId, sendTime, foodFee,
                packageFee, sendFee, discountFee, totalFee, invoiceTitle, orderTime, remark,
                dinnerName, dinnernum, cityId, sex, phone, address, resId, resName,
                longitude, latitude, hospitalId, cn, cnAmount, department, mudId, foods);
            }
            catch (Exception ex)
            {
                LogHelper.Error("OpenApi.createOrder", ex);
            }
            return res;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enterpriseOrderId"></param>
        /// <param name="oldXMSOrderId"></param>
        /// <param name="_orderInfo"></param>
        /// <returns></returns>
        public CreateOrderResponse createOrder2(string enterpriseOrderId, string oldXMSOrderId, string typeId, P_Order _orderInfo)
        {
            CreateOrderResponse _res = null;

            try
            {
                if (_orderInfo.IsNonHT == 1)
                {
                    _res = handler.createOrder(
                                       enterpriseOrderId, oldXMSOrderId, typeId, _orderInfo.details.deliverTime.Value.ToString("yyyy-MM-dd HH:mm:ss"), _orderInfo.foods.foodFee.ToString(), _orderInfo.foods.packageFee.ToString(),
                                       _orderInfo.foods.sendFee.ToString(), "0", _orderInfo.foods.allPrice.ToString(), _orderInfo.hospital.invoiceTitle + " - " + _orderInfo.hospital.dutyParagraph,
                                       DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), _orderInfo.details.remark,
                                       _orderInfo.details.consignee, _orderInfo.details.attendCount.ToString(), _orderInfo.hospital.city, "1", _orderInfo.details.phone, _orderInfo.hospital.address + " - " + _orderInfo.details.deliveryAddress, _orderInfo.foods.resId,
                                       string.Empty, _orderInfo.hospital.longitude, _orderInfo.hospital.latitude, _orderInfo.hospital.hospital, _orderInfo.PO, "99999", string.Empty, _orderInfo.userid, _orderInfo.foods.foods.Select(a => a.ToFoodRequest()).ToArray());
                }
                else
                {
                    _res = handler.createOrder(
                   enterpriseOrderId, oldXMSOrderId, typeId, _orderInfo.details.deliverTime.Value.ToString("yyyy-MM-dd HH:mm:ss"), _orderInfo.foods.foodFee.ToString(), _orderInfo.foods.packageFee.ToString(),
                   _orderInfo.foods.sendFee.ToString(), "0", _orderInfo.foods.allPrice.ToString(), _orderInfo.hospital.invoiceTitle,
                   DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), _orderInfo.details.remark,
                   _orderInfo.details.consignee, _orderInfo.details.attendCount.ToString(), _orderInfo.hospital.city, "1", _orderInfo.details.phone, _orderInfo.hospital.address + " - " + _orderInfo.details.deliveryAddress, _orderInfo.foods.resId,
                   string.Empty, _orderInfo.hospital.longitude, _orderInfo.hospital.latitude, _orderInfo.hospital.hospital, _orderInfo.CnCode, _orderInfo.meeting.budgetTotal.ToString(),
                   string.Empty, _orderInfo.meeting.userId, _orderInfo.foods.foods.Select(a => a.ToFoodRequest()).ToArray());

                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("OpenApi.createOrder2", ex);
            }
            return _res;
        }

        #region 取消订单
        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="xmsOrderId"></param>
        /// <param name="type"></param>
        /// <param name="sendTime"></param>
        /// <param name="address"></param>
        /// <param name="foods"></param>
        /// <param name="phone"></param>
        /// <param name="dinnerName"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public CancleOrderResponse cancleOrder(string xmsOrderId, string remark)
        {
            CancleOrderResponse res = null;
            try
            {
                res = handler.cancleOrder(xmsOrderId, remark);
            }
            catch (Exception ex)
            {
                LogHelper.Error("OpenApi.cancleOrder", ex);
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
            ResponseBase res = null;
            try
            {
                res = handler.finishOrder(xmsOrderId, type, remark);
            }
            catch (Exception ex)
            {
                LogHelper.Error("OpenApi.finishOrder", ex);
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
            ResponseBase res = null;
            try
            {
                res = handler.orderDeliveryFailure(xmsOrderId, remark);
            }
            catch (Exception ex)
            {
                LogHelper.Error("OpenApi.orderDeliveryFailure", ex);
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
            ResponseBase res = null;
            try
            {
                res = handler.cancleFailOrderFeedBack(xmsOrderId, type, remark);
            }
            catch (Exception ex)
            {
                LogHelper.Error("OpenApi.cancleFailOrderFeedBack", ex);
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
            OrderQueryResponse res = null;
            try
            {
                res = handler.OrderQuery(startTime, endTime, timeType);
            }
            catch (Exception ex)
            {
                LogHelper.Error("OpenApi.OrderQuery", ex);
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
            GetReportResponse res = null;
            try
            {
                res = handler.GetReport(startTime, endTime, timeType);
            }
            catch (Exception ex)
            {
                LogHelper.Error("OpenApi.GetReport", ex);
            }
            return res;
        }
        #endregion

        #region 获取可送餐列表接口XMS
        /// <summary>
        /// 获取可送餐列表接口XMS
        /// </summary>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <returns></returns>
        public GetSyncHospitalResRponse SyncHospitalRes(string province, string city)
        {
            GetSyncHospitalResRponse res = null;
            try
            {
                res = handler.SyncHospitalRes(province, city);
            }
            catch (Exception ex)
            {
                LogHelper.Error("OpenApi.SyncHospitalRes", ex);
            }
            return res;
        }
        #endregion

        #region 获取餐厅表接口XMS
        /// <summary>
        /// 获取餐厅表接口XMS
        /// </summary>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <returns></returns>
        public GetSyncResResponse SyncRes(string province, string city)
        {
            GetSyncResResponse res = null;
            try
            {
                res = handler.SyncRes(province, city);
            }
            catch (Exception ex)
            {
                LogHelper.Error("OpenApi.SyncRes", ex);
            }
            return res;
        }
        #endregion

        #region 获取可送餐列表接口BDS
        /// <summary>
        /// 获取可送餐列表接口BDS
        /// </summary>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <returns></returns>
        public GetSyncHospitalResRponse SyncHospitalResBDS(string province, string city)
        {
            GetSyncHospitalResRponse res = null;
            try
            {
                res = handlerBDS.SyncHospitalResBDS(province, city);
            }
            catch (Exception ex)
            {
                LogHelper.Error("OpenApi.SyncHospitalRes", ex);
            }
            return res;
        }
        #endregion

        #region 获取餐厅表接口BDS
        /// <summary>
        /// 获取餐厅表接口BDS
        /// </summary>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <returns></returns>
        public GetSyncResResponse SyncResBDS(string province, string city)
        {
            GetSyncResResponse res = null;
            try
            {
                res = handlerBDS.SyncResBDS(province, city);
            }
            catch (Exception ex)
            {
                LogHelper.Error("OpenApi.SyncRes", ex);
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
            GetSyncHospitalChangedRponse hos = null;
            try
            {
                hos = handlerBDS.SyncHospitalChanged();
            }
            catch (Exception ex)
            {
                LogHelper.Error("OpenApi.SyncHospitalChanged", ex);
            }
            return hos;
        }
        #endregion

        #region XMS获取医院数据变量报告接口
        /// <summary>
        /// 获取医院数据变量报告接口
        /// </summary>
        /// <returns></returns>
        public GetSyncHospitalChangedRponse SyncHospitalChangedXMS()
        {
            GetSyncHospitalChangedRponse hos = null;
            try
            {
                hos = handler.SyncHospitalChangedXMS();
            }
            catch (Exception ex)
            {
                LogHelper.Error("OpenApi.SyncHospitalChangedXMS", ex);
            }
            return hos;
        }
        #endregion
    }
}
