using MealAdmin.Entity;
using MealAdmin.Entity.Helper;
using MeetingMealEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MeetingMealApiService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IOpenApi”。
    [ServiceContract]
    public interface IOpenApi
    {
        #region 同步城市
        /// <summary>
        /// 同步城市
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        MeetingMealEntity.SyncCityResponse syncCity();
        #endregion

        #region 同步医院
        /// <summary>
        /// 同步医院
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        [OperationContract]
        MeetingMealEntity.SyncHospitalResponse syncHospital(string cityId);
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
        [OperationContract]
        MeetingMealEntity.QueryResResponse queryRes(string hospitalId, string address, string latitude, string longitude);
        #endregion

        #region 查询餐厅菜品
        /// <summary>
        /// 查询餐厅菜品
        /// </summary>
        /// <param name="resId"></param>
        /// <returns></returns>
        [OperationContract]
        MeetingMealEntity.QueryResFoodResponse queryResFood(string resId);
        #endregion

        #region 预算费用
        /// <summary>
        /// 预算费用
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <param name="resId"></param>
        /// <param name="foods"></param>
        /// <returns></returns>
        [OperationContract]
        MeetingMealEntity.CalculateFeeResponse calculateFee(string hospitalId, string resId, MeetingMealEntity.FoodRequest[] foods);
        #endregion

        #region 订单下单
        /// <summary>
        /// 订单下单
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
        [OperationContract]
        MeetingMealEntity.CreateOrderResponse createOrder(string enterpriseOrderId, string oldXMSOrderId, string typeId, string sendTime, string foodFee,
            string packageFee, string sendFee, string discountFee, string totalFee, string invoiceTitle, string orderTime, string remark,
            string dinnerName, string dinnernum, string cityId, string sex, string phone, string address, string resId, string resName,
            string longitude, string latitude, string hospitalId, string cn, string cnAmount, string department, string mudId, MeetingMealEntity.FoodRequest[] foods);
        #endregion

        #region 下单
        /// <summary>
        /// 下单
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        MeetingMealEntity.CreateOrderResponse createOrder2(string enterpriseOrderId, string oldXMSOrderId, string typeId, P_Order _orderInfo);
        #endregion

        #region 取消订单
        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="xmsOrderId"></param>
        /// <returns></returns>
        [OperationContract]
        MeetingMealEntity.CancleOrderResponse cancleOrder(string xmsOrderId, string remark);
        #endregion

        #region 完成订单
        /// <summary>
        /// 完成订单
        /// </summary>
        /// <param name="xmsOrderId"></param>
        /// <param name="type"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        [OperationContract]
        ResponseBase finishOrder(string xmsOrderId, string type, string remark);
        #endregion

        #region 订单未送达
        /// <summary>
        /// 订单未送达
        /// </summary>
        /// <param name="xmsOrderId"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        [OperationContract]
        ResponseBase orderDeliveryFailure(string xmsOrderId, string remark);
        #endregion

        #region 取消订单失败反馈（原单配送）
        /// <summary>
        /// 取消订单失败反馈（原单配送）
        /// </summary>
        /// <param name="xmsOrderId"></param>
        /// <param name="type"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        [OperationContract]
        ResponseBase cancleFailOrderFeedBack(string xmsOrderId, string type, string remark);
        #endregion

        #region 查询订单
        /// <summary>
        /// 查询订单
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="timeType"></param>
        /// <returns></returns>
        [OperationContract]
        OrderQueryResponse OrderQuery(string startTime, string endTime, string timeType);
        #endregion

        #region 日报 周报接口
        /// <summary>
        /// 日报 周报接口
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="timeType"></param>
        /// <returns></returns>
        [OperationContract]
        GetReportResponse GetReport(string startTime, string endTime, string timeType);
        #endregion

        #region 获取可送餐列表接口XMS
        /// <summary>
        /// 获取可送餐列表接口XMS
        /// </summary>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        [OperationContract]
        GetSyncHospitalResRponse SyncHospitalRes(string province, string city);
        #endregion

        #region 获取餐厅表接口XMS
        /// <summary>
        /// 获取餐厅表接口XMS
        /// </summary>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        [OperationContract]
        GetSyncResResponse SyncRes(string province, string city);
        #endregion

        #region 获取可送餐列表接口BDS
        /// <summary>
        /// 获取可送餐列表接口BDS
        /// </summary>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        [OperationContract]
        GetSyncHospitalResRponse SyncHospitalResBDS(string province, string city);
        #endregion

        #region 获取餐厅表接口BDS
        /// <summary>
        /// 获取餐厅表接口BDS
        /// </summary>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        [OperationContract]
        GetSyncResResponse SyncResBDS(string province, string city);
        #endregion

        #region BDS获取医院数据变量报告接口
        /// <summary>
        /// 获取医院数据变量报告接口
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        GetSyncHospitalChangedRponse SyncHospitalChanged();
        #endregion

        #region XMS获取医院数据变量报告接口
        /// <summary>
        /// 获取医院数据变量报告接口
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        GetSyncHospitalChangedRponse SyncHospitalChangedXMS();
        #endregion
    }
}
