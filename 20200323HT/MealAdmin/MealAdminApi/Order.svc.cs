using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using MealAdmin.Entity;
using MealAdmin.Service;
using MealAdmin.Entity.Helper;
using XFramework.XUtil;
using iPathFeast.API.Client;
using iPathFeast.ApiEntity;

namespace MealAdminApi
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Order”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Order.svc 或 Order.svc.cs，然后开始调试。
    public class Order : IOrder
    {
        public IOrderService orderService = Global.applicationContext.GetBean("orderService") as IOrderService;

        public ApiV1Client apiClient = Global.applicationContext.GetBean("apiClient") as ApiV1Client;


        #region 查询用户订单
        /// <summary>
        /// 查询用户订单
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="state"></param>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<P_ORDER> LoadByUserId(string userId, DateTime begin, DateTime end, string state, int rows, int page, out int total)
        {
            try
            { 
                var list = orderService.LoadByUserId(userId, begin, end, state, rows, page, out total);
                return list;
            }
            catch (Exception ex)
            {
                LogHelper.Error("Order.LoadByUserId", ex);
            }
            total = -1;
            return null;
        }
        #endregion

        #region 查询用户1.0订单
        /// <summary>
        /// 查询用户1.0订单
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="state"></param>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<P_ORDER> LoadOldOrderByUserId(string userId, DateTime begin, DateTime end, string state, int rows, int page, out int total)
        {
            try
            {
                var list = orderService.LoadOldOrderByUserId(userId, begin, end, state, rows, page, out total);
                return list;
            }
            catch (Exception ex)
            {
                LogHelper.Error("Order.LoadOldOrderByUserId", ex);
            }
            total = -1;
            return null;
        }
        #endregion

        #region 查询用户订单
        /// <summary>
        /// 查询用户订单
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="state"></param>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<P_ORDER> LoadReceiveOrderByUserId(string userId, DateTime begin, DateTime end, string state, int rows, int page, out int total)
        {
            try
            {
                var list = orderService.LoadReceiveOrderByUserId(userId, begin, end, state, rows, page, out total);
                return list;
            }
            catch (Exception ex)
            {
                LogHelper.Error("Order.LoadByUserId", ex);
            }
            total = -1;
            return null;
        }
        #endregion

        #region 记录一条新订单
        /// <summary>
        /// 记录一条新订单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Add(Guid ID, string UserId, string XmsOrderId,  string ipathOrderId,string channel)
        {
            try
            {
                return orderService.Add(ID, UserId, XmsOrderId,ipathOrderId, channel);
            }
            catch(Exception ex)
            {
                LogHelper.Error("Order.Add", ex);
                throw ex;
            }
            return 0;
        }
        #endregion

        #region 记录一条新订单到缓存表
        /// <summary>
        /// 记录一条新订单到缓存表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int AddCache(Guid ID, string UserId, string XmsOrderId, P_WeChatOrder _orderInfo, string ipathOrderId, string channel)
        {
            try
            {
                return orderService.AddCache(ID, UserId, XmsOrderId, _orderInfo, ipathOrderId, channel);
            }
            catch (Exception ex)
            {
                LogHelper.Error("Order.AddCache", ex);
            }
            return 0;
        }
        #endregion

        #region 审批后下单保存小秘书单号
        /// <summary>
        /// 审批后下单保存小秘书单号
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="XmsOrderId"></param>
        /// <returns></returns>
        public int SaveXmsOrderId(Guid ID, string XmsOrderId)
        {
            try
            {
                return orderService.SaveXmsOrderId(ID, XmsOrderId);
            }
            catch (Exception ex)
            {
                LogHelper.Error("Order.SaveXmsOrderId", ex);
            }
            return 0;
        }
        #endregion

        #region 修改一条订单
        /// <summary>
        /// 记录一条新订单
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="ChangeID"></param>
        /// <param name="_orderInfo"></param>
        /// <returns></returns>
        public int Change(Guid ID, Guid ChangeID, P_WeChatOrder _orderInfo)
        {
            try
            {
                return orderService.Change(ID, ChangeID, _orderInfo);
            }
            catch(Exception ex)
            {
                LogHelper.Error("Order.Change", ex);
            }
            return 0;
        }
        #endregion

        #region 修改一条缓存订单
        /// <summary>
        /// 修改一条缓存订单
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="ChangeID"></param>
        /// <param name="_orderInfo"></param>
        /// <returns></returns>
        public int ChangeCache(Guid ID, Guid ChangeID, P_WeChatOrder _orderInfo)
        {
            try
            {
                return orderService.ChangeCache(ID, ChangeID, _orderInfo);
            }
            catch (Exception ex)
            {
                LogHelper.Error("Order.ChangeCache", ex);
            }
            return 0;
        }
        #endregion

        #region 根据订单号查询订单
        /// <summary>
        /// 根据订单号查询订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public P_ORDER FindByID(Guid id)
        {
            try
            {
                return orderService.FindByID(id);
            }
            catch (Exception ex)
            {
                LogHelper.Error("Order.FindByID", ex);
            }
            return null;
        }
        #endregion

        #region 根据订单号查询缓存订单
        /// <summary>
        /// 根据订单号查询缓存订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public P_ORDER FindCacheOrderByID(Guid id)
        {
            try
            {
                return orderService.FindCacheOrderByID(id);
            }
            catch (Exception ex)
            {
                LogHelper.Error("Order.FindCacheOrderByID", ex);
            }
            return null;
        }
        #endregion

        #region 根据订单号查询1.0订单
        /// <summary>
        /// 根据订单号查询1.0订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public P_ORDER FindOldOrderByID(Guid id)
        {
            try
            {
                return orderService.FindOldOrderByID(id);
            }
            catch (Exception ex)
            {
                LogHelper.Error("Order.FindOldOrderByID", ex);
            }
            return null;
        }
        #endregion

        #region 根据小秘单号查询订单
        /// <summary>
        /// 根据小秘单号查询订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public P_ORDER FindByXmlOrderId(string xmsOrderId)
        {
            try
            {
                return orderService.FindByXmlOrderId(xmsOrderId);
            }
            catch (Exception ex)
            {
                LogHelper.Error("Order.FindByXmlOrderId", ex);
            }
            return null;
        }
        #endregion

        #region 取消订单
        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int CancelOrder(Guid id)
        {
            try
            {
                return orderService.CancelOrder(id);
            }
            catch(Exception ex)
            {
                LogHelper.Error("Order.CancelOrder", ex);
                throw ex;
            }
            //return 0;
        }
        #endregion

        #region 退单成功
        /// <summary>
        /// 退单成功
        /// </summary>
        /// <param name="xmsOrderId"></param>
        /// <returns></returns>
        public int CancelOrderSuccess(string xmsOrderId)
        {
            try
            {
                return orderService.CancelOrderSuccess(xmsOrderId);
            }
            catch (Exception ex)
            {
                LogHelper.Error("Order.CancelOrderSuccess", ex);
                throw ex;
            }
            //return 0;
        }
        #endregion

        #region 退单失败
        /// <summary>
        /// 退单失败
        /// </summary>
        /// <param name="xmsOrderId"></param>
        /// <param name="xmsReason"></param>
        /// <returns></returns>
        public int CancelOrderFail(string xmsOrderId, string xmsReason)
        {
            try
            {
                return orderService.CancelOrderFail(xmsOrderId, xmsReason);
            }
            catch (Exception ex)
            {
                LogHelper.Error("Order.CancelOrderFail", ex);
                throw ex;
            }
            //return 0;
        }
        #endregion

        #region 原单配送
        /// <summary>
        /// 原单配送
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public int OriginalOrderSend(Guid ID)
        {
            try
            {
                return orderService.OriginalOrderSend(ID);
            }
            catch (Exception ex)
            {
                LogHelper.Error("Order.OriginalOrderSend", ex);
                throw ex;
            }
            //return 0;
        }
        #endregion

        #region 原单配送(小秘书反馈成功)
        /// <summary>
        /// 原单配送(小秘书反馈成功)
        /// </summary>
        /// <param name="xmsOrderId"></param>
        /// <returns></returns>
        public int OriginalOrderSendSuccess(string xmsOrderId,string xmsReason)
        {
            try
            {
                return orderService.OriginalOrderSendSuccess(xmsOrderId, xmsReason);
            }
            catch (Exception ex)
            {
                LogHelper.Error("Order.OriginalOrderSendSuccess", ex);
                throw ex;
            }
            //return 0;
        }
        #endregion

        #region 原单配送(小秘书反馈失败)
        /// <summary>
        /// 原单配送失败
        /// </summary>
        /// <param name="xmsOrderId"></param>
        /// <param name="xmsReason"></param>
        /// <returns></returns>
        public int OriginalOrderSendFail(string xmsOrderId, string xmsReason)
        {
            try
            {
                return orderService.OriginalOrderSendFail(xmsOrderId, xmsReason);
            }
            catch(Exception ex)
            {
                LogHelper.Error("Order.OriginalOrderSendFail", ex);
                throw ex;
            }
            //return 0;
        }
        #endregion

        #region 确认收餐
        /// <summary>
        /// 确认收餐
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Confirm(Guid id, string price, string reason, string remark, string count, string cReason, string cRemark,string isSame)
        {
            try
            {
                return orderService.Confirm(id,price,reason,remark,count,cReason,cRemark, isSame);
            }
            catch(Exception ex)
            {
                LogHelper.Error("Order.Confirm", ex);
                throw ex;
            }
            //return 0;
        }
        #endregion

        #region 未送达
        /// <summary>
        /// 未送达
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Lost(Guid id)
        {
            try
            {
                return orderService.Lost(id);
            }
            catch (Exception ex)
            {
                LogHelper.Error("Order.Lost", ex);
            }
            return 0;
        }
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
        public int ScheduledSuccess(string xmsOrderId, string code, string remark, string oldXmsOrderid)
        {
            try
            {
                return orderService.ScheduledSuccess(xmsOrderId, code, remark, oldXmsOrderid);
            }
            catch (Exception ex)
            {
                LogHelper.Error("Order.ScheduledSuccess", ex);
                throw ex;
            }
            //return 0;
        }
        #endregion

        public int ADDLOG(string message)
        {
            try
            {
                return orderService.ADDLOG(message);
            }
            catch (Exception ex)
            {
                LogHelper.Error("Order.ADDLOG", ex);
                throw ex;
            }

        }

        public int DeleteOrder(Guid ID, string cn)
        {
            try
            {
                return orderService.DeleteOrder(ID, cn);
            }
            catch (Exception ex)
            {
                LogHelper.Error("Order.DeleteOrder", ex);
            }
            return 0;
        }

        public int UpdateOrder(Guid ID, string XmsOrderId)
        {
            try
            {
                return orderService.UpdateOrder(ID, XmsOrderId);
            }
            catch (Exception ex)
            {
                LogHelper.Error("Order.UpdateOrder", ex);
            }
            return 0;
        }

        public int AddOrder(Guid ID, string UserId, string XmsOrderId, P_WeChatOrder _orderInfo, string ipathOrderId, string channel)
        {
            try
            {
                return orderService.AddOrder(ID, UserId, XmsOrderId, _orderInfo, ipathOrderId, channel);
            }
            catch (Exception ex)
            {
                LogHelper.Error("Order.AddOrder", ex);
            }
            return 0;
        }

        public int ChangeOrder(Guid ID, Guid ChangeID, P_WeChatOrder _orderInfo)
        {
            try
            {
                return orderService.ChangeOrder(ID, ChangeID, _orderInfo);
            }
            catch (Exception ex)
            {
                LogHelper.Error("Order.ChangeOrder", ex);
            }
            return 0;
        }

        public int RestoreOrder(Guid ID, Guid ChangeID, P_ORDER p_ORDER)
        {
            try
            {
                return orderService.RestoreOrder(ID, p_ORDER);
            }
            catch (Exception ex)
            {
                LogHelper.Error("Order.RestoreOrder", ex);
            }
            return 0;
        }

        #region 预定失败
        /// <summary>
        /// 预定失败
        /// </summary>
        /// <param name="xmsOrderId"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public int ScheduledFail(string xmsOrderId, string remark, string oldXmsOrderid)
        {
            try
            {
                return orderService.ScheduledFail(xmsOrderId, remark, oldXmsOrderid);
            }
            catch (Exception ex)
            {
                LogHelper.Error("Order.ScheduledFail", ex);
                throw ex;
            }
            //return 0;
        }
        #endregion

        #region 小秘书修改订单金额
        /// <summary>
        /// 小秘书修改订单金额
        /// </summary>
        /// <param name="xmsOrderId"></param>
        /// <param name="xmsTotalPrice"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public int XmsChangeTotalFee(string xmsOrderId, decimal xmsTotalPrice, string reason)
        {
            try
            {
                return orderService.XmsChangeTotalFee(xmsOrderId, xmsTotalPrice, reason);
            }
            catch (Exception ex)
            {
                LogHelper.Error("Order.XmsChangeTotalFee", ex);
            }
            return 0;
        }
        #endregion

        #region MMCoE审批
        /// <summary>
        /// MMCoE审批
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public int MMCoEResult(Guid orderID, int state, string reason)
        {
            try
            {
                return orderService.MMCoEResult(orderID, state, reason);
            }
            catch(Exception ex)
            {
                LogHelper.Error("Order.MMCoEResult", ex);
            }
            return 0;
        }
        #endregion

        #region MMCoE审批退回
        /// <summary>
        /// MMCoE审批退回
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public int _MMCoEResult(Guid orderID, int state, string reason)
        {
            try
            {
                return orderService._MMCoEResult(orderID, state, reason);
            }
            catch (Exception ex)
            {
                LogHelper.Error("Order._MMCoEResult", ex);
            }
            return 0;
        }
        #endregion

        #region 返回未评价订单数量
        /// <summary>
        /// 返回未评价订单数量
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public int NotEvaluateCount(string userid,int isNonHT)
        {
            try
            {
                return orderService.NotEvaluateCount(userid, isNonHT);
            }
            catch (Exception ex)
            {
                LogHelper.Error("Order.NotEvaluateCount", ex);
            }
            return 0;
        }
        #endregion

        #region PO相关
        /// <summary>
        /// 保存PO号
        /// </summary>
        /// <param name="po"></param>
        /// <returns></returns>
        public int SavePO(P_PO po)
        {
            return orderService.SavePO(po);
        }

        /// <summary>
        /// 查询PO
        /// </summary>
        /// <param name="po"></param>
        /// <returns></returns>
        public P_PO FindByPO(string po)
        {
            return orderService.FindByPO(po);
        }

        /// <summary>
        /// 修改PO号使用状态
        /// </summary>
        /// <param name="po"></param>
        /// <returns></returns>
        public int EditPO(string po, int isUsed)
        {
            return orderService.EditPO(po,isUsed);
        }
        #endregion

        #region 周报相关
        public int SyncReport(string channel)
        {
            
            var endTime = DateTime.Now.ToString("yyyy-MM-dd");
            //endTime = "2018-09-09";
            //var startTime = DateTime.Now.AddDays(-6).ToString("yyyy-MM-dd");
            var req = new GetReportReq()
            {
                _Channels = channel,
                startTime = endTime,
                endTime = endTime,
                timeType = "0"
            };
            var report = apiClient.GetReport(req);

            List<P_ORDER_XMS_REPORT> list = report.listReport.Select(a => new P_ORDER_XMS_REPORT()
            {
                XmsOrderId = a.ipathOrderId,
                totalFee = a.totalFee,
                customerPickup = string.IsNullOrEmpty(a.customerPickup) ? string.Empty : a.customerPickup,
                cancelFeedback = string.IsNullOrEmpty(a.cancelFeedback) ? string.Empty : a.cancelFeedback,
                cancelFailReason = string.IsNullOrEmpty(a.cancelFailReason) ? string.Empty : a.cancelFailReason,
                feeModifyReason = string.IsNullOrEmpty(a.feeModifyReason) ? string.Empty : a.feeModifyReason,
                bookState = string.IsNullOrEmpty(a.bookState) ? string.Empty : a.bookState,
                cancelState = string.IsNullOrEmpty(a.cancelState) ? string.Empty : a.cancelState,
                TYYYDRDC = string.IsNullOrEmpty(a.TYYYDRDC) ? string.Empty : a.TYYYDRDC,
                TYDBDRDC = string.IsNullOrEmpty(a.TYDBDRDC) ? string.Empty : a.TYDBDRDC,
                TYDBTYYYDRDC = string.IsNullOrEmpty(a.TYDBTYYYDRDC) ? string.Empty : a.TYDBTYYYDRDC,
                CHRSDYLS = string.IsNullOrEmpty(a.CHRSDYLS) ? string.Empty : a.CHRSDYLS,
                CHRSXYLSDDFSDYLS = string.IsNullOrEmpty(a.CHRSXYLSDDFSDYLS) ? string.Empty : a.CHRSXYLSDDFSDYLS,
                TYCTDRDC= string.IsNullOrEmpty(a.TYCTDRDC) ? string.Empty : a.TYCTDRDC,
                TYDBTYCTDRDC= string.IsNullOrEmpty(a.TYDBTYCTDRDC) ? string.Empty : a.TYDBTYCTDRDC,
                TYDBTYYYTYCTDRDC = string.IsNullOrEmpty(a.TYDBTYYYTYCTDRDC) ? string.Empty : a.TYDBTYYYTYCTDRDC,
            }).ToList();

            if (list.Count > 0)
            {
                return orderService.SyncReport(list);
            }

            return 1;
        }
        #endregion

        #region
        public P_ORDER GetOrderInfoByHTCode(string HTCode)
        {
            return orderService.GetOrderInfoByHTCode(HTCode);
        }
        #endregion

        # region 根据HTCode查询订单
        /// <summary>
        /// 根据HTCode查询订单
        /// </summary>
        /// <param name="htCode"></param>
        /// <returns></returns>
        public P_ORDER FindOrderByCN(string htCode)
        {
            return orderService.FindOrderByCN(htCode);
        }
        #endregion

        #region 费用分析-向上订单分析
        public List<P_Order_Count_Amount> LoadUpOrderAnalysisData(string userId, string position, string territoryCode, string begin, string end)
        {
            return orderService.LoadUpOrderAnalysisData(userId, position, territoryCode, begin, end);
        }
        #endregion

        #region 费用分析-向下订单分析
        public List<P_Order_By_State> LoadDownOrderAnalysisData(string userId, string position, string territoryCode, string begin, string end)
        {
            return orderService.LoadDownOrderAnalysisData(userId, position, territoryCode, begin, end);
        }
        #endregion

        #region 获取订单统计信息
        public V_COST_SUMMARY GetOrderList(List<string> TerritoryStr, string StartDate, string EndDate)
        {
            return orderService.GetOrderList(TerritoryStr, StartDate, EndDate);
        }
        #endregion

        #region 获取特殊订单统计信息
        public V_COST_SUMMARY GetSpecialOrderList(List<string> TerritoryStr, string StartDate, string EndDate)
        {
            return orderService.GetSpecialOrderList(TerritoryStr, StartDate, EndDate);
        }
        #endregion

        #region 获取未完成订单统计信息
        public V_COST_SUMMARY GetUnfinishedOrderList(List<string> TerritoryStr, string StartDate, string EndDate)
        {
            return orderService.GetUnfinishedOrderList(TerritoryStr, StartDate, EndDate);
        }
        #endregion

        #region 有效预申请/订单分析
        public List<P_PreOrder_Order> LoadPreOrderAnalysisData(string userId, string position, string territoryCode, string begin, string end)
        {
            return orderService.LoadPreOrderAnalysisData(userId, position, territoryCode, begin, end);
        }
        public List<P_PreOrder_PreApproval> LoadPreAnalysisData(string userId, string position, string territoryCode, string begin, string end)
        {
            return orderService.LoadPreAnalysisData(userId, position, territoryCode, begin, end);
        }
        public List<P_PreOrder_Hospital_View> LoadHospitalAnalysisData(string userId, string position, string territoryCode, string begin, string end)
        {
            return orderService.LoadHospitalAnalysisData(userId, position, territoryCode, begin, end);
        }
        #endregion
    }
}
