using MealAdmin.Entity;
using MealAdmin.Entity.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MealAdminApi
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IOrder”。
    [ServiceContract]
    public interface IOrder
    {
        [OperationContract]
        List<P_ORDER> LoadByUserId(string userId, DateTime begin, DateTime end, string state, int rows, int page, out int total);
        [OperationContract]
        List<P_ORDER> LoadOldOrderByUserId(string userId, DateTime begin, DateTime end, string state, int rows, int page, out int total);
        [OperationContract]
        List<P_ORDER> LoadReceiveOrderByUserId(string userId, DateTime begin, DateTime end, string state, int rows, int page, out int total);

        [OperationContract]
        int Add(Guid ID, string UserId, string XmsOrderId, string ipathOrderId, string channel);
        [OperationContract]
        int AddCache(Guid ID, string UserId, string XmsOrderId, P_WeChatOrder _orderInfo, string ipathOrderId, string channel);

        [OperationContract]
        int SaveXmsOrderId(Guid ID, string XmsOrderId);

        [OperationContract]
        int Change(Guid ID, Guid ChangeID, P_WeChatOrder _orderInfo);

        [OperationContract]
        int ChangeCache(Guid ID, Guid ChangeID, P_WeChatOrder _orderInfo);

        [OperationContract]
        P_ORDER FindByID(Guid id);

        [OperationContract]
        P_ORDER FindCacheOrderByID(Guid id);

        [OperationContract]
        P_ORDER FindOldOrderByID(Guid id);
        
        [OperationContract]
        P_ORDER FindByXmlOrderId(string xmsOrderId);

        [OperationContract]
        int CancelOrder(Guid id);

        [OperationContract]
        int CancelOrderSuccess(string xmsOrderId);

        [OperationContract]
        int CancelOrderFail(string xmsOrderId, string xmsReason);

        [OperationContract]
        int OriginalOrderSend(Guid ID);

        [OperationContract]
        int OriginalOrderSendSuccess(string xmsOrderId,string xmsReason);

        [OperationContract]
        int OriginalOrderSendFail(string xmsOrderId, string xmsReason);

        [OperationContract]
        int Confirm(Guid id,string price,string reason,string remark,string count, string cReason, string cRemark,string isSame);

        [OperationContract]
        int Lost(Guid id);

        [OperationContract]
        int ScheduledSuccess(string xmsOrderId, string code, string remark, string oldXmsOrderid);

        [OperationContract]
        int ADDLOG(string message);

        [OperationContract]
        int DeleteOrder(Guid ID, string cn);

        [OperationContract]
        int UpdateOrder(Guid ID, string XmsOrderId);

        [OperationContract]
        int AddOrder(Guid ID, string UserId, string XmsOrderId, P_WeChatOrder _orderInfo, string ipathOrderId, string channel);

        [OperationContract]
        int ChangeOrder(Guid ID, Guid ChangeID, P_WeChatOrder _orderInfo);

        [OperationContract]
        int RestoreOrder(Guid ID, P_ORDER p_order);

        [OperationContract]
        int ScheduledFail(string xmsOrderId, string remark, string oldXmsOrderid);

        [OperationContract]
        int XmsChangeTotalFee(string xmsOrderId, decimal xmsTotalPrice, string reason);

        [OperationContract]
        int MMCoEResult(Guid orderID, int state, string reason);

        [OperationContract]
        int _MMCoEResult(Guid orderID, int state, string reason);

        [OperationContract]
        int NotEvaluateCount(string userid,int isNonHT);

        [OperationContract]
        int SavePO(P_PO po);

        [OperationContract]
        P_PO FindByPO(string po);

        [OperationContract]
        int EditPO(string po, int isUsed);

        [OperationContract]
        int SyncReport(string channel);
        [OperationContract]
        P_ORDER GetOrderInfoByHTCode(string HTCode);

        [OperationContract]
        P_ORDER FindOrderByCN(string htCode);

        [OperationContract]
        List<P_Order_Count_Amount> LoadUpOrderAnalysisData(string userId, string position, string territoryCode, string begin, string end);

        [OperationContract]
        List<P_Order_By_State> LoadDownOrderAnalysisData(string userId, string position, string territoryCode, string begin, string end);

        [OperationContract]
        List<P_PreOrder_Order> LoadPreOrderAnalysisData(string userId, string position, string territoryCode, string begin, string end);

        [OperationContract]
        List<P_PreOrder_PreApproval> LoadPreAnalysisData(string userId, string position, string territoryCode, string begin, string end);

        [OperationContract]
        List<P_PreOrder_Hospital_View> LoadHospitalAnalysisData(string userId, string position, string territoryCode, string begin, string end);

        #region 获取订单统计信息
        [OperationContract]
        V_COST_SUMMARY GetOrderList(List<string> TerritoryStr, string StartDate, string EndDate);
        #endregion

        #region 获取特殊订单统计信息
        [OperationContract]
        V_COST_SUMMARY GetSpecialOrderList(List<string> TerritoryStr, string StartDate, string EndDate);
        #endregion

        #region 获取未完成订单统计信息
        [OperationContract]
        V_COST_SUMMARY GetUnfinishedOrderList(List<string> TerritoryStr, string StartDate, string EndDate);
        #endregion

    }
}
