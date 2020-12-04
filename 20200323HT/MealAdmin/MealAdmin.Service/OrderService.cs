using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MealAdmin.Entity;
using XFramework.XInject.Attributes;
using MealAdmin.Dao;
using MealAdmin.Entity.View;
using MealAdmin.Entity.Helper;
using MealAdmin.Service.Helper;
using Newtonsoft.Json;
using MealAdmin.Entity.Enum;
using MeetingMealApiClient;

namespace MealAdmin.Service
{
    public class OrderService : IOrderService
    {
        [Bean("orderDao")]
        public IOrderDao orderDao { get; set; }

        [Bean("meetingDao")]
        public IMeetingDao meetingDao { get; set; }

        [Bean("preApprovalDao")]
        public IPreApprovalDao preApprovalDao { get; set; }

        [Bean("groupMemberDao")]
        public IGroupMemberDao groupMemberDao { get; set; }

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
        public List<P_ORDER> LoadByUserId(string userId, DateTime begin, DateTime end, string state, int rows, int page, out int total)
        {
            var _state = state.Split(',').Where(a => !string.IsNullOrEmpty(a)).Select(a => Convert.ToInt32(a)).ToArray();

            return orderDao.LoadByUserId(userId, begin, end, _state, rows, page, out total);
        }
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
        public List<P_ORDER> LoadOldOrderByUserId(string userId, DateTime begin, DateTime end, string state, int rows, int page, out int total)
        {
            var _state = state.Split(',').Where(a => !string.IsNullOrEmpty(a)).Select(a => Convert.ToInt32(a)).ToArray();

            return orderDao.LoadOldOrderByUserId(userId, begin, end, _state, rows, page, out total);
        }
        #endregion

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
        public List<P_ORDER> LoadReceiveOrderByUserId(string userId, DateTime begin, DateTime end, string state, int rows, int page, out int total)
        {
            var _state = state.Split(',').Where(a => !string.IsNullOrEmpty(a)).Select(a => Convert.ToInt32(a)).ToArray();

            return orderDao.LoadReceiveOrderByUserId(userId, begin, end, _state, rows, page, out total);
        }
        #endregion

        #region 新增订单记录
        /// <summary>
        /// 新增订单记录
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="UserId"></param>
        /// <param name="XmsOrderId"></param>
        /// <param name="_orderInfo"></param>
        /// <returns></returns>
        public int Add(Guid ID, string UserId, string XmsOrderId, string ipathOrderId, string channel)
        {
            var CreateDate = DateTime.Now;
            var order = orderDao.FindCacheOrderByID(ID);
            order.CreateDate = CreateDate;
            order.XmsOrderId = XmsOrderId;
            order.SendOrderDate = CreateDate;
            var res = orderDao.Add(order);
            if (res > 0)
            {
                // 新增订单记录应当占用CN号
                preApprovalDao.UsedHTCode(order.CN);
            }
            return res;
        }
        #endregion

        #region 新增订单记录到缓存表
        /// <summary>
        /// 新增订单记录到缓存表
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="UserId"></param>
        /// <param name="XmsOrderId"></param>
        /// <param name="_orderInfo"></param>
        /// <returns></returns>
        public int AddCache(Guid ID, string UserId, string XmsOrderId, P_WeChatOrder _orderInfo, string ipathOrderId, string channel)
        {
            var CreateDate = DateTime.Now;
            var order = SessionOrderToP_ORDER.ConvterIt(_orderInfo);
            order.ID = ID;
            order.UserId = UserId;
            order.CreateDate = CreateDate;
            order.XmsOrderId = XmsOrderId;
            order.SendOrderDate = CreateDate;
            order.EnterpriseOrderId = ipathOrderId;
            order.Channel = channel;
            var res = orderDao.AddCache(order);
            return res;
        }
        #endregion

        #region 审批后下单保存小秘书单号
        /// <summary>
        /// 审批后下单保存小秘书单号
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="xmsOrderId"></param>
        /// <returns></returns>
        public int SaveXmsOrderId(Guid ID, string xmsOrderId)
        {
            return orderDao.SaveXmsOrderId(ID, xmsOrderId);
        }
        #endregion

        #region 修改订单
        /// <summary>
        /// 修改订单
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="ChangeID"></param>
        /// <param name="_orderInfo"></param>
        /// <returns></returns>
        public int Change(Guid ID, Guid ChangeID, P_WeChatOrder _orderInfo)
        {
            var oldOrder = orderDao.FindCacheOrderByID(ID);
            return orderDao.Change(oldOrder);
        }
        #endregion

        #region 修改缓存订单
        /// <summary>
        /// 修改缓存订单
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="ChangeID"></param>
        /// <param name="_orderInfo"></param>
        /// <returns></returns>
        public int ChangeCache(Guid ID, Guid ChangeID, P_WeChatOrder _orderInfo)
        {
            var oldOrder = orderDao.FindByID(ID);
            // 1.主动改单 或者 改单MMCoE申请被拒绝的
            oldOrder.ChangeID = ChangeID;
            oldOrder.ChangeDetail = JsonConvert.SerializeObject(_orderInfo);
            oldOrder.IsChange = OrderIsChange.YES;
            oldOrder = SessionOrderToP_ORDER.ConvterIt(_orderInfo, oldOrder);
            oldOrder.State = OrderState.SUBMITTED;
            return orderDao.ChangeCache(oldOrder);
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
            return orderDao.FindByID(id);
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
            return orderDao.FindCacheOrderByID(id);
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
            return orderDao.FindOldOrderByID(id);
        }
        #endregion

        #region 根据HTCode查询订单
        /// <summary>
        /// 根据HTCode查询订单
        /// </summary>
        /// <param name="htCode"></param>
        /// <returns></returns>
        public P_ORDER FindOrderByCN(string htCode)
        {
            return orderDao.FindOrderByCN(htCode);
        }
        #endregion

        #region 根据订单号查询订单
        /// <summary>
        /// 根据订单号查询订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public P_ORDER FindByXmlOrderId(string xmsOrderId)
        {
            return orderDao.FindByXmlOrderId(xmsOrderId);
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
            return orderDao.CancelOrder(id);
        }
        #endregion

        #region 取消订单成功
        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="xmsOrderId"></param>
        /// <returns></returns>
        public int CancelOrderSuccess(string xmsOrderId)
        {
            var res = orderDao.CancelOrderSuccess(xmsOrderId);
            if (res > 0)
            {
                var order = orderDao.FindByXmlOrderId(xmsOrderId);
                if (order.IsNonHT == 0)
                {
                    // 退单成功，释放CN
                    preApprovalDao.ReleaseHTCode(order.CN);
                }
                else
                {
                    // 退单成功，释放PO
                    orderDao.EditPO(order.PO, 0);
                }
            }
            return res;
        }
        #endregion

        #region 取消订单失败
        /// <summary>
        /// 取消订单失败
        /// </summary>
        /// <param name="xmsOrderId"></param>
        /// <param name="xmsReason"></param>
        /// <returns></returns>
        public int CancelOrderFail(string xmsOrderId, string xmsReason)
        {
            return orderDao.CancelOrderFail(xmsOrderId, xmsReason);
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
            return orderDao.OriginalOrderSend(ID);
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
            return orderDao.OriginalOrderSendSuccess(xmsOrderId, xmsReason);
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
            return orderDao.OriginalOrderSendFail(xmsOrderId, xmsReason);
        }
        #endregion

        #region 确认收餐
        /// <summary>
        /// 确认收餐
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Confirm(Guid id, string price, string reason, string remark, string count, string cReason, string cRemark, string isSame)
        {
            preApprovalDao.FinishPreApproval(id);
            var res = orderDao.Confirm(id, price, reason, remark, count, cReason, cRemark, isSame);
            var order = orderDao.FindByID(id);
            var nowDate = DateTime.Now.AddHours(-48);
            var userId = order.UserId;
            if (order.IsTransfer == 1)
            {
                userId = order.TransferUserMUDID;
            }
            var orderList = orderDao.LoadUserConfirmOrders(userId, nowDate);
            groupMemberDao.UpdateServPauseDetail(order.CN, 1);
            if (orderList.Count < 1)
            {
                //没有48小时内未收餐订单，解除暂定服务
                var group = groupMemberDao.FindByUser(userId);
                if (group != null)
                {
                    if (group.State == 0 && group.State2 == 0 && group.State3 == 0)
                    {
                        groupMemberDao.DelByState1(userId);
                    }
                    else
                    {
                        groupMemberDao.UpdateState1(userId);
                    }
                }
            }
            return res;
        }
        #endregion

        #region 系统收餐
        /// <summary>
        /// 系统收餐
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int SystemConfirm(Guid id)
        {
            preApprovalDao.FinishPreApproval(id);
            var order = orderDao.FindByID(id);
            var res = orderDao.SystemConfirm(id, order.TotalPrice.ToString(), order.AttendCount.ToString());
            var nowDate = DateTime.Now.AddHours(-48);
            var userId = order.UserId;
            if (order.IsTransfer == 1)
            {
                userId = order.TransferUserMUDID;
            }
            var orderList = orderDao.LoadUserConfirmOrders(userId, nowDate);
            groupMemberDao.UpdateServPauseDetail(order.CN, 1);
            if (orderList.Count < 1)
            {
                var group = groupMemberDao.FindByUser(userId);
                if (group != null)
                {
                    if (group.State == 0 && group.State2 == 0 && group.State3 == 0)
                    {
                        groupMemberDao.DelByState1(userId);
                    }
                    else
                    {
                        groupMemberDao.UpdateState1(userId);
                    }
                }
            }
            return res;
        }
        #endregion

        #region 查询需要系统收餐的订单
        /// <summary>
        /// 查询需要系统收餐的订单
        /// </summary>
        /// <returns></returns>
        public List<P_ORDER> LoadOrders()
        {
            return orderDao.LoadOrders();
        }
        #endregion

        #region 系统收餐
        /// <summary>
        /// 系统收餐
        /// </summary>
        /// <returns></returns>
        public int SystemConfirm()
        {
            return orderDao.SystemConfirm();
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
            preApprovalDao.FinishPreApproval(id);
            var res = orderDao.Lost(id);
            var order = orderDao.FindByID(id);
            var nowDate = DateTime.Now.AddHours(-48);
            var userId = order.UserId;
            if (order.IsTransfer == 1)
            {
                userId = order.TransferUserMUDID;
            }
            var orderList = orderDao.LoadUserConfirmOrders(userId, nowDate);
            if (orderList.Count < 1)
            {
                var group = groupMemberDao.FindByUser(userId);
                if (group != null)
                {
                    if (group.State == 0 && group.State2 == 0 && group.State3 == 0)
                    {
                        groupMemberDao.DelByState1(userId);
                    }
                    else
                    {
                        groupMemberDao.UpdateState1(userId);
                    }
                }
            }
            return res;
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
            var orderId = string.IsNullOrEmpty(oldXmsOrderid) ? xmsOrderId : oldXmsOrderid;
            var oldOrder = orderDao.FindByXmlOrderId(orderId);

            if (string.IsNullOrEmpty(oldXmsOrderid))
            {
                // 新单
                return orderDao.ScheduledSuccess(xmsOrderId, code, remark);
            }
            else
            {
                // 改单
                oldOrder.OldXmlOrderId = oldOrder.XmsOrderId;
                oldOrder.XmsOrderId = xmsOrderId;
                // 将新单Detail赋值给Detail正位,副位清空
                oldOrder.Detail = oldOrder.ChangeDetail;
                oldOrder.ChangeDetail = string.Empty;
                // 状态修改
                oldOrder.State = OrderState.SCHEDULEDSUCCESS;
                oldOrder.IsChange = OrderIsChange.SUCCESS;
                oldOrder.ReceiveCode = code;

                return orderDao.ScheduledSuccessForChange(oldOrder);
            }
        }
        #endregion

        public int ADDLOG(string message)
        {
            return orderDao.ADDLOG(message);
        }

        public int DeleteOrder(Guid ID, string cn)
        {
            var res = orderDao.DeleteOrder(ID, cn);
            return res;
        }

        public int UpdateOrder(Guid ID, string XmsOrderId)
        {
            var res = orderDao.UpdateOrder(ID, XmsOrderId);
            return res;
        }

        public int AddOrder(Guid ID, string UserId, string XmsOrderId, P_WeChatOrder _orderInfo, string ipathOrderId, string channel)
        {
            var CreateDate = DateTime.Now;
            var order = SessionOrderToP_ORDER.ConvterIt(_orderInfo);
            order.ID = ID;
            order.UserId = UserId;
            order.CreateDate = CreateDate;
            order.XmsOrderId = XmsOrderId;
            order.SendOrderDate = CreateDate;
            order.EnterpriseOrderId = ipathOrderId;
            order.Channel = channel;
            var res = orderDao.AddOrder(order);
            return res;
        }

        public int ChangeOrder(Guid ID, Guid ChangeID, P_WeChatOrder _orderInfo)
        {
            var oldOrder = orderDao.FindByID(ID);
            // 1.主动改单 或者 改单MMCoE申请被拒绝的
            oldOrder.ChangeID = ChangeID;
            oldOrder.ChangeDetail = JsonConvert.SerializeObject(_orderInfo);
            oldOrder.IsChange = OrderIsChange.YES;
            oldOrder = SessionOrderToP_ORDER.ConvterIt(_orderInfo, oldOrder);
            oldOrder.State = OrderState.SUBMITTED;
            return orderDao.ChangeOrder(oldOrder);
        }

        public int RestoreOrder(Guid ID, P_ORDER p_ORDER)
        {
            return orderDao.RestoreOrder(p_ORDER);
        }

        #region 预定失败
        /// <summary>
        /// 预定失败
        /// </summary>
        /// <param name="xmsOrderId"></param>
        /// <param name="remark"></param>
        /// <param name="oldXmsOrderid"></param>
        /// <returns></returns>
        public int ScheduledFail(string xmsOrderId, string remark, string oldXmsOrderid)
        {
            var orderId = string.IsNullOrEmpty(oldXmsOrderid) ? xmsOrderId : oldXmsOrderid;
            var oldOrder = orderDao.FindByXmlOrderId(orderId);

            if (string.IsNullOrEmpty(oldXmsOrderid))
            {
                // 新单
                var res = orderDao.ScheduledFail(xmsOrderId, remark);

                if (res > 0)
                {
                    // 预定失败，释放CN
                    //meetingDao.ReleaseCN(oldOrder.CN);
                    var order = orderDao.FindByXmlOrderId(xmsOrderId);
                    if (order.IsNonHT == 0)
                    {
                        // 预定失败，释放CN
                        preApprovalDao.ReleaseHTCode(order.CN);
                    }
                    else
                    {
                        // 预定失败，释放PO
                        orderDao.EditPO(order.PO, 0);
                    }
                }
                return res;
            }
            else
            {
                // 改单
                oldOrder.ChangeID = Guid.Empty;
                oldOrder.ChangeDetail = string.Empty;
                // 状态修改(改单失败维持原单)
                oldOrder.State = OrderState.SCHEDULEDSUCCESS;
                oldOrder.IsChange = OrderIsChange.FAIL;
                // 订单的详情要恢复到原单
                var _orderInfo = JsonConvert.DeserializeObject<P_Order>(oldOrder.Detail);
                oldOrder = SessionOrderInfoToP_ORDER.ConvterIt(_orderInfo, oldOrder);
                oldOrder.XmsOrderReason = remark;

                return orderDao.ScheduledFailForChange(oldOrder);
            }
        }
        #endregion

        #region 修改订单金额
        /// <summary>
        /// 修改订单金额
        /// </summary>
        /// <param name="xmsOrderId"></param>
        /// <param name="xmsTotalPrice"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public int XmsChangeTotalFee(string xmsOrderId, decimal xmsTotalPrice, string reason)
        {
            return orderDao.XmsChangeTotalFee(xmsOrderId, xmsTotalPrice, reason);
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
            return orderDao.MMCoEResult(orderID, state, reason);
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
            return orderDao._MMCoEResult(orderID, state, reason);
        }
        #endregion

        #region 返回未评价订单数量
        /// <summary>
        /// 返回未评价订单数量
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public int NotEvaluateCount(string userid, int isNonHT)
        {
            return orderDao.NotEvaluateCount(userid, isNonHT);
        }
        #endregion

        #region 后台订单服务
        #region 后台订单服务-list
        public List<P_ORDER_DAILY_VIEW> LoadOrderMntPage(string srh_CN, string srh_MUDID, DateTime? srh_CreateTimeBegin, DateTime? srh_CreateTimeEnd, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, int? srh_State, string Supplier, int isNonHT, int rows, int page, out int total)
        {
            return orderDao.LoadOrderMntPage(srh_CN, srh_MUDID, srh_CreateTimeBegin, srh_CreateTimeEnd, srh_DeliverTimeBegin, srh_DeliverTimeEnd, srh_State, Supplier, isNonHT, rows, page, out total);
        }
        #endregion

        #region 后台订单服务-report
        public List<P_ORDER_DAILY_VIEW> LoadOrderMnt(string srh_CN, string srh_MUDID, DateTime? srh_CreateTimeBegin, DateTime? srh_CreateTimeEnd, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, int? srh_State, string Supplier, int isNonHT)
        {
            return orderDao.LoadOrderMnt(srh_CN, srh_MUDID, srh_CreateTimeBegin, srh_CreateTimeEnd, srh_DeliverTimeBegin, srh_DeliverTimeEnd, srh_State, Supplier, isNonHT);
        }
        #endregion

        #region 后台订单审核-List
        public List<P_ORDER_APPROVE_VIEW> LoadOrderApprovePage(string srh_CN, string srh_MUDID, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, int srh_MMCoEApproveState, int isNonHt, int rows, int page, out int total)
        {
            return orderDao.LoadOrderApprovePage(srh_CN, srh_MUDID, srh_DeliverTimeBegin, srh_DeliverTimeEnd, srh_MMCoEApproveState, isNonHt, rows, page, out total);
        }

        #endregion

        #endregion

        #region 后台订单审核 根据订单ID 获取订单信息
        public P_ORDER GetOrderInfo(Guid OrderID)
        {
            return orderDao.GetOrderInfo(OrderID);
        }
        #endregion

        #region 同步周报日报
        /// <summary>
        /// 同步周报日报
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public int SyncReport(List<P_ORDER_XMS_REPORT> list)
        {
            var timeNow = DateTime.Now.ToString("yyyy-MM-dd");
            //timeNow = "2018-09-09";
            var begin = Convert.ToDateTime(timeNow + " 00:00:00");
            var end = Convert.ToDateTime(timeNow + " 23:59:59");
            //当日所有订单（送餐时间是当天）
            var orderList = orderDao.LoadOrdersNow(begin, end);
            //当日有效订单（除预定失败、退订成功）
            var res = orderDao.LoadOrdersNow(begin, end);
            //1.0当日有效订单
            //var otherList = orderDao.LoadOtherOrdersNow(begin,end);

            if (orderList.Count > 0)
            {
                foreach (var order in orderList)
                {
                    var hosId = order.HospitalId;
                    var mudId = order.UserId;
                    var resId = order.RestaurantId;
                    var _hosId = string.Empty;
                    if (hosId.Contains("-OH"))
                    {
                        _hosId = hosId.Substring(0, hosId.Length - 3);
                    }
                    else
                    {
                        _hosId = hosId + "-OH";
                    }

                    var TYYYDRDC = (res.Where(a => a.HospitalId == hosId || a.HospitalId == _hosId).ToList().Count) > 1 ? "是" : "否";//同一医院当日多场
                    var TYDBDRDC = (res.Where(a => a.UserId == mudId).ToList().Count) > 1 ? "是" : "否";//同一代表当日多场
                    var TYDBTYYYDRDC = (res.Where(a => (a.HospitalId == hosId || a.HospitalId == _hosId) && a.UserId == mudId).ToList().Count) > 1 ? "是" : "否";//同一代表同一医院当日多场
                    var CHRSDYLS = order.AttendCount >= 60 ? "是" : "否";//参会人数大于60
                    var CHRSXYLSDDFSDYLS = (order.AttendCount < 60 && order.FoodCount >= 60) ? "是" : "否";//参会人数小于60，订单分数大于60
                    var TYCTDRDC = (res.Where(a => a.RestaurantId == resId).ToList().Count > 1) ? "是" : "否";//同一餐厅当日多场
                    var TYDBTYCTDRDC = (res.Where(a => a.RestaurantId == resId && a.UserId == mudId).ToList().Count) > 1 ? "是" : "否";//同一代表同一餐厅当日多场
                    var TYDBTYYYTYCTDRDC = (res.Where(a => a.RestaurantId == resId && (a.HospitalId == hosId || a.HospitalId == _hosId) && a.UserId == mudId).ToList().Count) > 1 ? "是" : "否";//同一代表同一医院同一餐厅当日多场

                    var report = list.Find(a => a.XmsOrderId == order.XmsOrderId); //更改日报数据
                    if (report != null)
                    {
                        report.TYYYDRDC = TYYYDRDC;
                        report.TYDBDRDC = TYDBDRDC;
                        report.TYDBTYYYDRDC = TYDBTYYYDRDC;
                        report.CHRSDYLS = CHRSDYLS;
                        report.CHRSXYLSDDFSDYLS = CHRSXYLSDDFSDYLS;
                        report.TYCTDRDC = TYCTDRDC;
                        report.TYDBTYCTDRDC = TYDBTYCTDRDC;
                        report.TYDBTYYYTYCTDRDC = TYDBTYYYTYCTDRDC;
                    }
                }
            }
            var count = orderDao.SyncReport(list);
            return orderDao.SyncReport(list);
        }
        #endregion

        #region 载入简报
        /// <summary>
        /// 载入简报
        /// </summary>
        /// <returns></returns>
        public P_LOADORDER_BRIEF LoadBriefing(int isNonHT)
        {
            return orderDao.LoadBriefing(isNonHT);
        }
        #endregion

        #region 后台订单评价-List
        public List<P_ORDER_EVALUATE_VIEW> LoadOrderEvaluatePage(string srh_CN, string srh_MUDID, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, bool srh_Large60, bool srh_UnSafe, bool srh_UnSend, int isNonHt, int star, string channel, int rows, int page, out int total)
        {
            return orderDao.LoadOrderEvaluatePage(srh_CN, srh_MUDID, srh_DeliverTimeBegin, srh_DeliverTimeEnd, srh_Large60, srh_UnSafe, srh_UnSend, isNonHt, star, channel, rows, page, out total);
        }

        public List<P_ORDER_EVALUATE_VIEW> LoadOrderEvaluate(string srh_CN, string srh_MUDID, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, bool srh_Large60, bool srh_UnSafe, bool srh_UnSend, int isNonHt, int star, string channel)
        {
            return orderDao.LoadOrderEvaluate(srh_CN, srh_MUDID, srh_DeliverTimeBegin, srh_DeliverTimeEnd, srh_Large60, srh_UnSafe, srh_UnSend, isNonHt, star, channel);
        }
        #endregion

        #region PO相关
        public int SavePO(P_PO po)
        {
            return orderDao.SavePO(po);
        }

        public P_PO FindByPO(string po)
        {
            return orderDao.FindByPO(po);
        }

        public int EditPO(string po, int isUsed)
        {
            return orderDao.EditPO(po, isUsed);
        }
        #endregion

        #region 保存特殊订单
        public int SaveChange(string HTCode, string SpecialOrderReason, string SpecialRemarksProjectTeam, string SpecialOrderOperatorName, string SpecialOrderOperatorMUDID, int IsSpecialOrder, int State)
        {
            return orderDao.SaveChange(HTCode, SpecialOrderReason, SpecialRemarksProjectTeam, SpecialOrderOperatorName, SpecialOrderOperatorMUDID, IsSpecialOrder, State);
        }
        #endregion


        #region 订单查询
        /// <summary>
        /// 订单查询
        /// </summary>
        /// <param name="srh_CN"></param>
        /// <param name="srh_MUDID"></param>
        /// <param name="srh_TACode"></param>
        /// <param name="srh_CreateTimeBegin"></param>
        /// <param name="srh_CreateTimeEnd"></param>
        /// <param name="srh_DeliverTimeBegin"></param>
        /// <param name="srh_DeliverTimeEnd"></param>
        /// <param name="srh_State"></param>
        /// <param name="Supplier"></param>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<HT_ORDER_REPORT_VIEW> LoadOrderReportPage(string srh_CN, string srh_MUDID, string srh_TACode, string srh_HospitalCode, string srh_RestaurantId, string srh_CostCenter, DateTime? srh_CreateTimeBegin, DateTime? srh_CreateTimeEnd, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, int? srh_State, string Supplier, string IsSpecialOrder, string RD, int rows, int page, out int total)
        {
            return orderDao.LoadOrderReportPage(srh_CN, srh_MUDID, srh_TACode, srh_HospitalCode, srh_RestaurantId, srh_CostCenter, srh_CreateTimeBegin, srh_CreateTimeEnd, srh_DeliverTimeBegin, srh_DeliverTimeEnd, srh_State, Supplier, IsSpecialOrder, RD, rows, page, out total);
        }
        #endregion 

        public List<HT_ORDER_REPORT_VIEW> LoadOrderReport(string srh_CN, string srh_MUDID, string srh_TACode, string srh_HospitalCode, string srh_RestaurantId, string srh_CostCenter, DateTime? srh_CreateTimeBegin, DateTime? srh_CreateTimeEnd, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, int? srh_State, string Supplier, string IsSpecialOrder, string RD)
        {
            return orderDao.LoadOrderReport(srh_CN, srh_MUDID, srh_TACode, srh_HospitalCode, srh_RestaurantId, srh_CostCenter, srh_CreateTimeBegin, srh_CreateTimeEnd, srh_DeliverTimeBegin, srh_DeliverTimeEnd, srh_State, Supplier, IsSpecialOrder, RD);
        }

        #region 后台订单服务-list
        public List<P_ORDER_DAILY_VIEW> LoadNonHTOrderMntPage(string srh_CN, string srh_MUDID, string srh_HospitalCode, string srh_RestaurantId, DateTime? srh_CreateTimeBegin, DateTime? srh_CreateTimeEnd, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, int? srh_State, string Supplier, int isNonHT, int rows, int page, out int total)
        {
            return orderDao.LoadNonHTOrderMntPage(srh_CN, srh_MUDID, srh_HospitalCode, srh_RestaurantId, srh_CreateTimeBegin, srh_CreateTimeEnd, srh_DeliverTimeBegin, srh_DeliverTimeEnd, srh_State, Supplier, isNonHT, rows, page, out total);
        }

        public List<P_ORDER_DAILY_VIEW> LoadNonHTOrderMnt(string srh_CN, string srh_MUDID, string srh_HospitalCode, string srh_RestaurantId, DateTime? srh_CreateTimeBegin, DateTime? srh_CreateTimeEnd, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, int? srh_State, string Supplier, int isNonHT)
        {
            return orderDao.LoadNonHTOrderMnt(srh_CN, srh_MUDID, srh_HospitalCode, srh_RestaurantId, srh_CreateTimeBegin, srh_CreateTimeEnd, srh_DeliverTimeBegin, srh_DeliverTimeEnd, srh_State, Supplier, isNonHT);
        }
        #endregion

        #region 后台订单审核-List
        public List<P_ORDER_APPROVE_VIEW> LoadNonHTOrderApprovePage(string srh_CN, string srh_MUDID, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, int srh_MMCoEApproveState, int isNonHt, int rows, int page, out int total)
        {
            return orderDao.LoadNonHTOrderApprovePage(srh_CN, srh_MUDID, srh_DeliverTimeBegin, srh_DeliverTimeEnd, srh_MMCoEApproveState, isNonHt, rows, page, out total);
        }

        #endregion

        #region 后台订单评价-List
        public List<P_ORDER_EVALUATE_VIEW> LoadNonHTOrderEvaluatePage(string srh_CN, string srh_MUDID, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, bool srh_Large60, bool srh_UnSafe, bool srh_UnSend, int isNonHt, int rows, int page, out int total)
        {
            return orderDao.LoadNonHTOrderEvaluatePage(srh_CN, srh_MUDID, srh_DeliverTimeBegin, srh_DeliverTimeEnd, srh_Large60, srh_UnSafe, srh_UnSend, isNonHt, rows, page, out total);
        }

        public List<P_ORDER_EVALUATE_VIEW> LoadNonHTOrderEvaluate(string srh_CN, string srh_MUDID, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, bool srh_Large60, bool srh_UnSafe, bool srh_UnSend, int isNonHt)
        {
            return orderDao.LoadNonHTOrderEvaluate(srh_CN, srh_MUDID, srh_DeliverTimeBegin, srh_DeliverTimeEnd, srh_Large60, srh_UnSafe, srh_UnSend, isNonHt);
        }
        #endregion

        #region  ********************消息推送相关*********************

        #region 获取需要发送确认收餐消息的订单(收餐时间后一小时)
        /// <summary>
        /// 获取需要发送确认收餐消息的订单
        /// </summary>
        /// <param name="nowDate"></param>
        /// <returns></returns>
        public List<P_ORDER> LoadConfirmOrders(DateTime nowDate)
        {
            return orderDao.LoadConfirmOrders(nowDate);
        }
        #endregion

        #region 更新推送状态（送餐时间后一小时未确认收餐）
        /// <summary>
        /// 更新推送状态
        /// </summary>
        /// <param name="xmsOrderIds"></param>
        /// <returns></returns>
        public int UpdatePushOne(string xmsOrderIds)
        {
            return orderDao.UpdatePushOne(xmsOrderIds);
        }
        #endregion

        #region 更新上传文件状态
        /// <summary>
        /// 更新上传文件状态
        /// </summary>
        /// <param name="state"></param>
        /// <param name="htCode"></param>
        /// <returns></returns>
        public int UpdateOrderUpload(int state, string htCode)
        {
            var res = orderDao.UpdateOrderUpload(state, htCode);
            var order = orderDao.FindOrderByCN(htCode);
            var nowDate = DateTime.Now;
            nowDate = nowDate.AddHours(-168);
            var userId = order.UserId;
            if (order.IsTransfer == 1)
            {
                userId = order.TransferUserMUDID;
            }
            var orderList = orderDao.LoadUserOrderUpload(userId, nowDate);
            if (orderList.Count < 1)
            {
                var group = groupMemberDao.FindByUser(userId);
                if (group != null)
                {
                    if (group.State == 0 && group.State1 == 0 && group.State3 == 0)
                    {
                        groupMemberDao.DelByState2(userId);
                    }
                    else
                    {
                        groupMemberDao.UpdateState2(userId);
                    }
                }
            }

            return res;

        }
        #endregion

        #region 获取需要发送确认收餐消息的订单(晚十点)
        /// <summary>
        /// 获取需要发送确认收餐消息的订单
        /// </summary>
        /// <param name="nowDate"></param>
        /// <returns></returns>
        public List<P_ORDER> LoadConfirmOrders()
        {
            return orderDao.LoadConfirmOrders();
        }
        #endregion

        #region 获取需要上传文件的订单(晚十点)
        /// <summary>
        /// 获取需要上传文件的订单
        /// </summary>
        /// <returns></returns>
        public List<P_ORDER> LoadUploadOrders()
        {
            return orderDao.LoadUploadOrders();
        }
        #endregion

        #region 获取需要上传文件审批的订单(晚十点)
        /// <summary>
        /// 获取需要上传文件审批的订单
        /// </summary>
        /// <returns></returns>
        public List<P_ORDER> LoadUploadFailOrders()
        {
            return orderDao.LoadUploadFailOrders();
        }
        #endregion

        #region 获取需要确认收餐的订单(晚六点)
        /// <summary>
        /// 获取需要确认收餐的订单
        /// </summary>
        /// <param name="nowDate"></param>
        /// <returns></returns>
        public List<P_ORDER> LoadOrderConfirms(DateTime nowDate)
        {
            return orderDao.LoadOrderConfirms(nowDate);
        }
        #endregion

        #region 获取需要上传文件的订单(晚六点)
        /// <summary>
        /// 获取需要上传文件的订单
        /// </summary>
        /// <param name="nowDate"></param>
        /// <returns></returns>
        public List<P_ORDER> LoadOrderUpload(DateTime nowDate)
        {
            return orderDao.LoadOrderUpload(nowDate);
        }
        #endregion

        #region 获取需要审批上传文件的订单(晚六点)
        /// <summary>
        /// 获取需要审批上传文件的订单
        /// </summary>
        /// <param name="nowDate"></param>
        /// <returns></returns>
        public List<P_ORDER> LoadFailOrder(DateTime nowDate)
        {
            return orderDao.LoadFailOrder(nowDate);
        }
        #endregion

        #region 获取需要审批上传文件的订单(晚六点)
        /// <summary>
        /// 获取需要审批上传文件的订单
        /// </summary>
        /// <param name="nowDate"></param>
        /// <returns></returns>
        public List<P_PREUPLOADORDER> LoadFailUploadOrder(DateTime nowDate)
        {
            return orderDao.LoadFailUploadOrder(nowDate);
        }
        #endregion

        #region 确认收餐后一小时未上传文件
        /// <summary>
        /// 确认收餐后一小时未上传文件
        /// </summary>
        /// <param name="nowTime"></param>
        /// <returns></returns>
        public List<P_ORDER> LoadUploadFiles(DateTime nowTime)
        {
            return orderDao.LoadUploadFiles(nowTime);
        }
        #endregion

        #region 更新推送状态（确认收餐后一小时未上传文件）
        /// <summary>
        /// 更新推送状态
        /// </summary>
        /// <param name="xmsOrderIds"></param>
        /// <returns></returns>
        public int UpdatePushTwo(string xmsOrderIds)
        {
            return orderDao.UpdatePushTwo(xmsOrderIds);
        }
        #endregion

        #endregion


        #region 获取需要自动失败的订单
        /// <summary>
        /// 获取需要自动失败的订单
        /// </summary>
        /// <returns></returns>
        public List<P_ORDER> LoadAutoChangeFail()
        {
            return orderDao.LoadAutoChangeFail();
        }
        #endregion

        #region 获取需要自动成功的订单
        /// <summary>
        /// 获取需要自动成功的订单
        /// </summary>
        /// <returns></returns>
        public List<P_ORDER> LoadAutoChangeSuccess()
        {
            return orderDao.LoadAutoChangeSuccess();
        }
        #endregion

        #region
        public P_ORDER GetOrderInfoByHTCode(string HTCode)
        {
            return orderDao.FindOrderByCN(HTCode);
        }
        #endregion

        #region 转交订单
        public int AutoTransferOrder(string HTCode, string TransferUserMUDID, string TransferUserName)
        {
            return orderDao.AutoTransferOrder(HTCode, TransferUserMUDID, TransferUserName);
        }

        public int AutoTransferUpload(string HTCode, string TransferUserMUDID, string TransferUserName)
        {
            return orderDao.AutoTransferUpload(HTCode, TransferUserMUDID, TransferUserName);
        }
        public int AddAutoTransferHistory(string HTCode, string FromMUDID, string ToMUDID, int Type)
        {
            return orderDao.AddAutoTransferHistory(HTCode, FromMUDID, ToMUDID,Type);
        }
        #endregion

        public List<P_ORDER> LoadDataByInHTCode(string subHTCode)
        {
            return orderDao.LoadDataByInHTCode(subHTCode);
        }

        public int Import(string sql)
        {
            return orderDao.Import(sql);
        }

        public List<HT_Order_Report> LoadReportByHTOrder(string HTCode, string EnterpriseOrderId)
        {
            return orderDao.LoadReportByHTOrder(HTCode, EnterpriseOrderId);
        }

        public int ImportReport(string reportsql)
        {
            return orderDao.ImportReport(reportsql);
        }

        public int GetOrderCount(string srh_CN, string srh_MUDID, string srh_TACode, string srh_HospitalCode, string srh_RestaurantId, string srh_CostCenter, DateTime? srh_CreateTimeBegin, DateTime? srh_CreateTimeEnd, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, int? srh_State, string Supplier, string IsSpecialOrder, string RD)
        {
            return orderDao.GetOrderCount(srh_CN, srh_MUDID, srh_TACode, srh_HospitalCode, srh_RestaurantId, srh_CostCenter, srh_CreateTimeBegin, srh_CreateTimeEnd, srh_DeliverTimeBegin, srh_DeliverTimeEnd, srh_State, Supplier, IsSpecialOrder, RD);
        }

        #region 同步订单表
        public int SyncOrder()
        {
            return orderDao.SyncOrder();
        }
        #endregion

        public List<P_Order_Count_Amount> LoadUpOrderAnalysisData(string userId, string position, string territoryCode, string begin, string end)
        {
            return orderDao.LoadUpOrderAnalysisData(userId, position, territoryCode, begin, end);
        }

        public List<P_Order_By_State> LoadDownOrderAnalysisData(string userId, string position, string territoryCode, string begin, string end)
        {
            return orderDao.LoadDownOrderAnalysisData(userId, position, territoryCode, begin, end);
        }
        public List<P_PreOrder_Order> LoadPreOrderAnalysisData(string userId, string position, string territoryCode, string begin, string end)
        {
            return orderDao.LoadPreOrderAnalysisData(userId, position, territoryCode, begin, end);
        }
        public List<P_PreOrder_PreApproval> LoadPreAnalysisData(string userId, string position, string territoryCode, string begin, string end)
        {
            return orderDao.LoadPreAnalysisData(userId, position, territoryCode, begin, end);
        }
        public List<P_PreOrder_Hospital_View> LoadHospitalAnalysisData(string userId, string position, string territoryCode, string begin, string end)
        {
            return orderDao.LoadHospitalAnalysisData(userId, position, territoryCode, begin, end);
        }

        #region 获取订单统计信息
        public V_COST_SUMMARY GetOrderList(List<string> TerritoryStr, string StartDate, string EndDate)
        {
            return orderDao.GetOrderList(TerritoryStr, StartDate, EndDate);
        }
        #endregion

        #region 获取特殊订单统计信息
        public V_COST_SUMMARY GetSpecialOrderList(List<string> TerritoryStr, string StartDate, string EndDate)
        {
            return orderDao.GetSpecialOrderList(TerritoryStr, StartDate, EndDate);
        }
        #endregion

        #region 获取未完成订单统计信息
        public V_COST_SUMMARY GetUnfinishedOrderList(List<string> TerritoryStr, string StartDate, string EndDate)
        {
            return orderDao.GetUnfinishedOrderList(TerritoryStr, StartDate, EndDate);
        }
        #endregion
    }
}
