using MealAdmin.Entity;
using MealAdmin.Entity.Helper;
using MealAdmin.Entity.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Service
{
    public interface IOrderService
    {
        #region 查找用户订单
        /// <summary>
        /// 查找用户订单
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="state"></param>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        List<P_ORDER> LoadByUserId(string userId, DateTime begin, DateTime end, string state, int rows, int page, out int total);
        #endregion

        #region 查找用户1.0订单
        /// <summary>
        /// 查找用户1.0订单
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="state"></param>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        List<P_ORDER> LoadOldOrderByUserId(string userId, DateTime begin, DateTime end, string state, int rows, int page, out int total);
        #endregion

        #region 查找用户待收餐订单
        /// <summary>
        /// 查找用户订单
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="state"></param>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        List<P_ORDER> LoadReceiveOrderByUserId(string userId, DateTime begin, DateTime end, string state, int rows, int page, out int total);
        #endregion

        #region 增加一条新订单
        /// <summary>
        /// 增加一条新订单
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="UserId"></param>
        /// <param name="XmsOrderId"></param>
        /// <param name="_orderInfo"></param>
        /// <returns></returns>
        int Add(Guid ID, string UserId, string XmsOrderId, string ipathOrderId, string channel);
        #endregion

        #region 增加一条新订单到缓存表
        /// <summary>
        /// 增加一条新订单到缓存表
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="UserId"></param>
        /// <param name="XmsOrderId"></param>
        /// <param name="_orderInfo"></param>
        /// <returns></returns>
        int AddCache(Guid ID, string UserId, string XmsOrderId, P_WeChatOrder _orderInfo, string ipathOrderId, string channel);
        #endregion

        #region 审批后下单
        /// <summary>
        /// 审批后下单
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="xmsOrderId"></param>
        /// <returns></returns>
        int SaveXmsOrderId(Guid ID, string xmsOrderId);
        #endregion

        #region 修改订单
        /// <summary>
        /// 修改订单
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="ChangeID"></param>
        /// <param name="_orderInfo"></param>
        /// <returns></returns>
        int Change(Guid ID, Guid ChangeID, P_WeChatOrder _orderInfo);
        #endregion

        #region 修改缓存订单
        /// <summary>
        /// 修改缓存订单
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="ChangeID"></param>
        /// <param name="_orderInfo"></param>
        /// <returns></returns>
        int ChangeCache(Guid ID, Guid ChangeID, P_WeChatOrder _orderInfo);
        #endregion

        #region 根据订单号查询订单
        /// <summary>
        /// 根据订单号查询订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        P_ORDER FindByID(Guid id);
        #endregion

        #region 根据订单号查询缓存订单
        /// <summary>
        /// 根据订单号查询缓存订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        P_ORDER FindCacheOrderByID(Guid id);
        #endregion

        #region 根据订单号查询1.0订单
        /// <summary>
        /// 根据订单号查询1.0订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        P_ORDER FindOldOrderByID(Guid id);
        #endregion

        #region 根据订单号查询订单
        /// <summary>
        /// 根据订单号查询订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        P_ORDER FindByXmlOrderId(string xmsOrderId);
        #endregion

        P_ORDER FindOrderByCN(string htCode);

        #region 取消订单
        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int CancelOrder(Guid id);
        #endregion

        #region 取消订单成功
        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="xmsOrderId"></param>
        /// <returns></returns>
        int CancelOrderSuccess(string xmsOrderId);
        #endregion

        #region 取消订单失败
        /// <summary>
        /// 取消订单失败
        /// </summary>
        /// <param name="xmsOrderId"></param>
        /// <param name="xmsReason"></param>
        /// <returns></returns>
        int CancelOrderFail(string xmsOrderId, string xmsReason);
        #endregion

        #region 原单配送
        /// <summary>
        /// 原单配送
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        int OriginalOrderSend(Guid ID);
        #endregion

        #region 原单配送(小秘书反馈成功)
        /// <summary>
        /// 原单配送成功
        /// </summary>
        /// <param name="xmsOrderId"></param>
        /// <returns></returns>
        int OriginalOrderSendSuccess(string xmsOrderId,string xmsReason);
        #endregion

        #region 原单配送(小秘书反馈失败)
        /// <summary>
        /// 原单配送失败
        /// </summary>
        /// <param name="xmsOrderId"></param>
        /// <param name="xmsReason"></param>
        /// <returns></returns>
        int OriginalOrderSendFail(string xmsOrderId, string xmsReason);
        #endregion

        #region 确认收餐
        /// <summary>
        /// 确认收餐
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int Confirm(Guid id, string price, string reason, string remark, string count, string cReason, string cRemark,string isSame);
        #endregion

        #region 系统收餐
        /// <summary>
        /// 系统收餐
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int SystemConfirm(Guid id);
        #endregion

        #region 查询需要系统收餐的订单
        /// <summary>
        /// 查询需要系统收餐的订单
        /// </summary>
        /// <returns></returns>
        List<P_ORDER> LoadOrders();
        #endregion

        #region 系统收餐
        /// <summary>
        /// 系统收餐
        /// </summary>
        /// <returns></returns>
        int SystemConfirm();
        #endregion

        #region 未送达
        /// <summary>
        /// 未送达
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int Lost(Guid id);
        #endregion

        #region 预定成功
        /// <summary>
        /// 预定成功
        /// </summary>
        /// <param name="xmsOrderId"></param>
        /// <param name="code"></param>
        /// <param name="remark"></param>
        /// <param name="oldXmsOrderid"></param>
        /// <returns></returns>
        int ScheduledSuccess(string xmsOrderId, string code, string remark, string oldXmsOrderid);
        #endregion

        int ADDLOG(string message);

        int DeleteOrder(Guid ID, string cn);

        int UpdateOrder(Guid ID, string XmsOrderId);

        int AddOrder(Guid ID, string UserId, string XmsOrderId, P_WeChatOrder _orderInfo, string ipathOrderId, string channel);

        int ChangeOrder(Guid ID, Guid ChangeID, P_WeChatOrder _orderInfo);

        int RestoreOrder(Guid ID, P_ORDER p_ORDER);

        #region 预定失败
        /// <summary>
        /// 预定失败
        /// </summary>
        /// <param name="xmsOrderId"></param>
        /// <param name="remark"></param>
        /// <param name="oldXmsOrderid"></param>
        /// <returns></returns>
        int ScheduledFail(string xmsOrderId, string remark, string oldXmsOrderid);
        #endregion

        #region 修改小秘书金额
        /// <summary>
        /// 修改小秘书金额
        /// </summary>
        /// <param name="xmsOrderId"></param>
        /// <param name="xmsTotalPrice"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        int XmsChangeTotalFee(string xmsOrderId, decimal xmsTotalPrice, string reason);
        #endregion

        #region MMCoE审批
        /// <summary>
        /// MMCoE审批
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        int MMCoEResult(Guid orderID, int state, string reason);
        #endregion

        #region MMCoE审批退回
        /// <summary>
        /// MMCoE审批退回
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        int _MMCoEResult(Guid orderID, int state, string reason);
        #endregion

        #region 返回未评价订单数量
        /// <summary>
        /// 返回未评价订单数量
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        int NotEvaluateCount(string userid,int isNonHT);
        #endregion

        #region 后台订单服务-list
        List<P_ORDER_DAILY_VIEW> LoadOrderMntPage(string srh_CN, string srh_MUDID, DateTime? srh_CreateTimeBegin, DateTime? srh_CreateTimeEnd, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, int? srh_State,string Supplier, int isNonHT,int rows, int page, out int total);
        #endregion

        #region 后台订单服务-report
        List<P_ORDER_DAILY_VIEW> LoadOrderMnt(string srh_CN, string srh_MUDID, DateTime? srh_CreateTimeBegin, DateTime? srh_CreateTimeEnd, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, int? srh_State,string Supplier,int isNonHT);
        #endregion

        #region 后台订单审核-List
        List<P_ORDER_APPROVE_VIEW> LoadOrderApprovePage(string srh_CN, string srh_MUDID, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, int srh_MMCoEApproveState, int isNonHt,int rows, int page, out int total);

        P_ORDER GetOrderInfo(Guid OrderID);
        #endregion

        #region 同步周报日报
        /// <summary>
        /// 同步周报日报
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        int SyncReport(List<P_ORDER_XMS_REPORT> list);
        #endregion

        #region 载入简报
        /// <summary>
        /// 载入简报
        /// </summary>
        /// <returns></returns>
        P_LOADORDER_BRIEF LoadBriefing(int isNonHT);
        #endregion

        #region 后台订单评价-List
        List<P_ORDER_EVALUATE_VIEW> LoadOrderEvaluatePage(string srh_CN, string srh_MUDID, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, bool srh_Large60, bool srh_UnSafe, bool srh_UnSend, int isNonHt, int star, string channel, int rows, int page, out int total);

        List<P_ORDER_EVALUATE_VIEW> LoadOrderEvaluate(string srh_CN, string srh_MUDID, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, bool srh_Large60, bool srh_UnSafe, bool srh_UnSend, int isNonHt, int star, string channel);
        #endregion

        #region PO相关
        int SavePO(P_PO po);

        P_PO FindByPO(string po);

        int EditPO(string po, int isUsed);
        #endregion

        #region 保存特殊订单
        int SaveChange(string HTCode, string SpecialOrderReason, string SpecialRemarksProjectTeam, string SpecialOrderOperatorName, string SpecialOrderOperatorMUDID, int IsSpecialOrder, int State);
        #endregion

        List<HT_ORDER_REPORT_VIEW> LoadOrderReportPage(string srh_CN, string srh_HospitalCode, string srh_RestaurantId, string srh_CostCenter, string srh_MUDID, string srh_TACode, DateTime? srh_CreateTimeBegin, DateTime? srh_CreateTimeEnd, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, int? srh_State, string Supplier, string IsSpecialOrder, string RD, int rows, int page, out int total);
        List<HT_ORDER_REPORT_VIEW> LoadOrderReport(string srh_CN, string srh_MUDID, string srh_TACode, string srh_HospitalCode, string srh_RestaurantId, string srh_CostCenter, DateTime? srh_CreateTimeBegin, DateTime? srh_CreateTimeEnd, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, int? srh_State, string Supplier, string IsSpecialOrder, string RD);

        int GetOrderCount(string srh_CN, string srh_MUDID, string srh_TACode, string srh_HospitalCode, string srh_RestaurantId, string srh_CostCenter, DateTime? srh_CreateTimeBegin, DateTime? srh_CreateTimeEnd, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, int? srh_State, string Supplier, string IsSpecialOrder, string RD);

        #region 后台订单服务-report
        List<P_ORDER_DAILY_VIEW> LoadNonHTOrderMntPage(string srh_CN, string srh_MUDID, string srh_HospitalCode, string srh_RestaurantId, DateTime? srh_CreateTimeBegin, DateTime? srh_CreateTimeEnd, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, int? srh_State, string Supplier, int isNonHT, int rows, int page, out int total);
        List<P_ORDER_DAILY_VIEW> LoadNonHTOrderMnt(string srh_CN, string srh_MUDID, string srh_HospitalCode, string srh_RestaurantId, DateTime? srh_CreateTimeBegin, DateTime? srh_CreateTimeEnd, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, int? srh_State, string Supplier, int isNonHT);
        List<P_ORDER_APPROVE_VIEW> LoadNonHTOrderApprovePage(string srh_CN, string srh_MUDID, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, int srh_MMCoEApproveState, int isNonHt, int rows, int page, out int total);
        #endregion

        #region 后台订单评价-List
        List<P_ORDER_EVALUATE_VIEW> LoadNonHTOrderEvaluatePage(string srh_CN, string srh_MUDID, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, bool srh_Large60, bool srh_UnSafe, bool srh_UnSend, int isNonHt, int rows, int page, out int total);
        List<P_ORDER_EVALUATE_VIEW> LoadNonHTOrderEvaluate(string srh_CN, string srh_MUDID, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, bool srh_Large60, bool srh_UnSafe, bool srh_UnSend, int isNonHt);
        
        #endregion

        #region ****************消息推送相关*************************

        #region 获取需要发送确认收餐消息的订单(收餐时间后一小时)
        /// <summary>
        /// 获取需要发送确认收餐消息的订单
        /// </summary>
        /// <param name="nowDate"></param>
        /// <returns></returns>
        List<P_ORDER> LoadConfirmOrders(DateTime nowDate);
        #endregion

        #region 更新推送状态
        /// <summary>
        /// 更新推送状态
        /// </summary>
        /// <param name="xmsOrderIds"></param>
        /// <returns></returns>
        int UpdatePushOne(string xmsOrderIds);
        #endregion 

        #region 更新上传文件状态
        /// <summary>
        /// 更新上传文件状态
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        int UpdateOrderUpload(int state, string htCode);
        #endregion

        #region 获取需要发送确认收餐消息的订单(晚十点)
        /// <summary>
        /// 获取需要发送确认收餐消息的订单
        /// </summary>
        /// <param name="nowDate"></param>
        /// <returns></returns>
        List<P_ORDER> LoadConfirmOrders();
        #endregion

        #region 获取需要上传文件的订单(晚十点)
        /// <summary>
        /// 获取需要上传文件的订单
        /// </summary>
        /// <returns></returns>
        List<P_ORDER> LoadUploadOrders();
        #endregion

        #region 获取需要上传文件审批的订单(晚十点)
        /// <summary>
        /// 获取需要上传文件审批的订单
        /// </summary>
        /// <returns></returns>
        List<P_ORDER> LoadUploadFailOrders();
        #endregion

        #region 获取需要确认收餐的订单(晚六点)
        /// <summary>
        /// 获取需要确认收餐的订单
        /// </summary>
        /// <param name="nowDate"></param>
        /// <returns></returns>
        List<P_ORDER> LoadOrderConfirms(DateTime nowDate);
        #endregion

        #region 获取需要上传文件的订单
        /// <summary>
        /// 获取需要上传文件的订单
        /// </summary>
        /// <param name="nowDate"></param>
        /// <returns></returns>
        List<P_ORDER> LoadOrderUpload(DateTime nowDate);
        #endregion

        #region 获取需要审批上传文件的订单
        /// <summary>
        /// 获取需要审批上传文件的订单
        /// </summary>
        /// <param name="nowDate"></param>
        /// <returns></returns>
        List<P_ORDER> LoadFailOrder(DateTime nowDate);
        #endregion

        #region 获取需要审批上传文件的订单
        /// <summary>
        /// 获取需要审批上传文件的订单
        /// </summary>
        /// <param name="nowDate"></param>
        /// <returns></returns>
        List<P_PREUPLOADORDER> LoadFailUploadOrder(DateTime nowDate);
        #endregion

        #region 收餐后一小时未上传文件
        /// <summary>
        /// 收餐后一小时未上传文件
        /// </summary>
        /// <param name="nowTime"></param>
        /// <returns></returns>
        List<P_ORDER> LoadUploadFiles(DateTime nowTime);
        #endregion

        #region 更新推送状态（收餐后一小时未上传文件）
        /// <summary>
        /// 更新推送状态
        /// </summary>
        /// <param name="xmsOrderIds"></param>
        /// <returns></returns>
        int UpdatePushTwo(string xmsOrderIds);
        #endregion

        #endregion


        #region 获取需要自动失败的订单
        /// <summary>
        /// 获取需要自动失败的订单
        /// </summary>
        /// <returns></returns>
        List<P_ORDER> LoadAutoChangeFail();
        #endregion 

        #region 获取需要自动成功的订单
        /// <summary>
        /// 获取需要自动成功的订单
        /// </summary>
        /// <returns></returns>
        List<P_ORDER> LoadAutoChangeSuccess();
        #endregion

        #region 根据HT编号获得订单详情
        P_ORDER GetOrderInfoByHTCode(string HTCode);
        #endregion

        #region 自动转交
        int AutoTransferOrder(string HTCode, string TransferUserMUDID, string TransferUserName);
        int AutoTransferUpload(string HTCode, string TransferUserMUDID, string TransferUserName);
        int AddAutoTransferHistory(string HTCode, string FromMUDID, string ToMUDID, int Type);
        #endregion

        List<P_ORDER> LoadDataByInHTCode(string subHTCode);

        int Import(string sql);

        List<HT_Order_Report> LoadReportByHTOrder(string HTCode,string EnterpriseOrderId);

        int ImportReport(string reportsql);
        
        #region 同步订单表
        int SyncOrder();
        #endregion

        List<P_Order_Count_Amount> LoadUpOrderAnalysisData(string userId, string position, string territoryCode, string begin, string end);

        List<P_Order_By_State> LoadDownOrderAnalysisData(string userId, string position, string territoryCode, string begin, string end);
        List<P_PreOrder_Order> LoadPreOrderAnalysisData(string userId, string position, string territoryCode, string begin, string end);
        List<P_PreOrder_PreApproval> LoadPreAnalysisData(string userId, string position, string territoryCode, string begin, string end);
        List<P_PreOrder_Hospital_View> LoadHospitalAnalysisData(string userId, string position, string territoryCode, string begin, string end);

        #region 获取订单统计信息
        V_COST_SUMMARY GetOrderList(List<string> TerritoryStr, string StartDate, string EndDate);
        #endregion

        #region 获取特殊订单统计信息
        V_COST_SUMMARY GetSpecialOrderList(List<string> TerritoryStr, string StartDate, string EndDate);
        #endregion

        #region 获取未完成订单统计信息
        V_COST_SUMMARY GetUnfinishedOrderList(List<string> TerritoryStr, string StartDate, string EndDate);
        #endregion
    }
}
