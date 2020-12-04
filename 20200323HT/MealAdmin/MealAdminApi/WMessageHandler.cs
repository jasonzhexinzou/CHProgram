using MealAdmin.Entity;
using MealAdmin.Entity.Enum;
using MealAdmin.Entity.Helper;
using MealAdmin.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XFramework.WeChatAPI.Entity;
using XFramework.WeChatAPI.SessionHandlers;
using XFramework.XUtil;

namespace MealAdminApi
{
    public class WMessageHandler
    {
        private static WMessageHandler _handler = new WMessageHandler();
        public static WMessageHandler GetInstance()
        {
            return _handler;
        }

        public QyApiHandler wApiHandler = Global.applicationContext.GetBean("wApiHandler") as QyApiHandler;
        public IOrderService orderService = Global.applicationContext.GetBean("orderService") as IOrderService;
        public IGroupMemberService groupMemberService = Global.applicationContext.GetBean("groupMemberService") as IGroupMemberService;
        public IEvaluateService evaluateService = Global.applicationContext.GetBean("evaluateService") as IEvaluateService;

        #region 发送企业文字消息
        /// <summary>
        /// 发送企业文字消息
        /// </summary>
        /// <param name="listMessage"></param>
        /// <param name="userid"></param>
        private void SendQyMsg(List<string> listMessage, string userid)
        {
            foreach (var msg in listMessage)
            {
                var messageId = DateTime.Now.Ticks;
                LogHelper.Info($"[{messageId}]发送:{userid}|{msg}");
                var res = wApiHandler.Message_Send(new MessageSend()
                {
                    touser = userid,
                    content = msg,
                    msgtype = "text"
                });
                LogHelper.Info($"[{messageId}]结果:{res.errcode} -> {res.errmsg}");
            }
        }
        #endregion

        #region 发送订单状态变动通知给订餐人
        /// <summary>
        /// 发送订单状态变动通知给订餐人
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="order"></param>
        public void SendMessageToUser(string userid, P_ORDER order)
        {
            var listMessage = new List<string>();
            //var msg = string.Empty;
            if (order.State == OrderState.SUBMITTED)
            {
                // 订单提交成功，等待小秘书确认
                if (order.IsChange == OrderIsChange.YES)
                {
                    var msg = $"{order.PO}，供应商: {order.Channel.ToUpper()}，您的修改请求已经提交成功，等待餐厅确认。";
                    listMessage.Add(msg);
                }
                else if (order.MMCoEApproveState == MMCoEApproveState.APPROVESUCCESS)
                {
                    var msg = $"{order.PO}，供应商: {order.Channel.ToUpper()}，您的订单中央订餐项目组已审批，正在等待餐厅确认。";
                    listMessage.Add(msg);
                }
                else
                {
                    var msg = $"{order.PO}，供应商: {order.Channel.ToUpper()}，您的订单已提交成功，正在等待餐厅确认。";
                    listMessage.Add(msg);
                }


            }
            else if (order.State == OrderState.WAITAPPROVE)
            {
                // 订单提交成功，等待MMCoE审批
                var msg = $"{order.PO}，供应商: {order.Channel.ToUpper()}，您的订单已提交成功，正在等待中央订餐项目组确认。";
                listMessage.Add(msg);
            }
            else if (order.State == OrderState.SCHEDULEDSUCCESS)
            {
                // 小秘书反馈预定成功
                if (order.IsChange == OrderIsChange.SUCCESS)
                {
                    var msg = $"{order.PO}，供应商: {order.Channel.ToUpper()}，您的订单已修改成功，餐厅将依据修改后的订单进行配送。";
                    listMessage.Add(msg);
                }
                else if (order.IsChange == OrderIsChange.FAIL)
                {
                    var msg = "";
                    switch (order.Channel.ToLower())
                    {
                        case "xms":
                            msg = $"{order.PO}，供应商: {order.Channel.ToUpper()}，您的订单修改失败，原因: {order.XmsOrderReason}。稍后呼叫中心人员会与您联系。如有疑问，请联系: 400-820-5577";
                            break;
                        case "bds":
                            msg = $"{order.PO}，供应商: {order.Channel.ToUpper()}，您的订单修改失败，原因: {order.XmsOrderReason}。稍后呼叫中心人员会与您联系。如有疑问，请联系: 400-6868-912";
                            break;
                    }
                    listMessage.Add(msg);
                }
                else
                {
                    if (order.IsOuterMeeting == 0)
                    {
                        // 院内会
                        var msg = "";
                        switch (order.Channel.ToLower())
                        {
                            case "xms":
                                msg = $"{order.PO}，供应商: {order.Channel.ToUpper()}，您的订单已预订成功，收货码为{order.ReceiveCode}。请在收餐时主动出示收货码，祝您用餐愉快。如有疑问，请联系: 400-820-5577";
                                break;
                            case "bds":
                                msg = $"{order.PO}，供应商: {order.Channel.ToUpper()}，您的订单已预订成功，收货码为{order.ReceiveCode}。请在收餐时主动出示收货码，祝您用餐愉快。如有疑问，请联系: 400-6868-912";
                                break;
                        }
                        listMessage.Add(msg);
                    }
                    else
                    {
                        // 院外会
                        var msg = "";
                        switch (order.Channel.ToLower())
                        {
                            case "xms":
                                msg = $"{order.PO}，供应商: {order.Channel.ToUpper()}，您的订单已预订成功，收货码为{order.ReceiveCode}；餐厅: {order.RestaurantName}；餐厅地址: {order.RestaurantAddress}；餐厅电话: {order.RestaurantTel}，请主动出示收货码并在会议结束后收取订餐小票及发票，祝您用餐愉快。如有疑问，请联系: 400-820-5577";
                                break;
                            case "bds":
                                msg = $"{order.PO}，供应商: {order.Channel.ToUpper()}，您的订单已预订成功，收货码为{order.ReceiveCode}；餐厅: {order.RestaurantName}；餐厅地址: {order.RestaurantAddress}；餐厅电话: {order.RestaurantTel}，请主动出示收货码并在会议结束后收取订餐小票及发票，祝您用餐愉快。如有疑问，请联系: 400-6868-912";
                                break;
                        }
                        listMessage.Add(msg);
                    }
                }


            }
            else if (order.State == OrderState.SCHEDULEDFAIL)
            {
                // 小秘书反馈预定失败
                var msg = "";
                switch (order.Channel.ToLower())
                {
                    case "xms":
                        msg = $"{order.PO}，供应商: {order.Channel.ToUpper()}，您的订单预订失败，原因: {order.XmsOrderReason}。稍候呼叫中心人员会与您联系。如有疑问，请联系: 400-820-5577";
                        break;
                    case "bds":
                        msg = $"{order.PO}，供应商: {order.Channel.ToUpper()}，您的订单预订失败，原因: {order.XmsOrderReason}。稍候呼叫中心人员会与您联系。如有疑问，请联系: 400-6868-912";
                        break;
                }
                listMessage.Add(msg);
            }
            else if (order.State == OrderState.REJECT)
            {
                // MMCoE审批拒绝
                var msg = $"{order.PO}，供应商: {order.Channel.ToUpper()}，您的订单被中央订餐项目组驳回，原因：{order.MMCoEReason}。如需重新上传截图，请<a href='{WebConfigHandler.H5Domain}/P/Food/MMCoEShell/{order.ID.ToString()}'>点击这里</a>，如需修改订单，请<a href='{WebConfigHandler.H5Domain}/P/Order/Details/{order.ID.ToString()}?fromuri=2&supplier={order.Channel}'>点击这里</a>。如有疑问，请联系cn.chinarx-pmo@gsk.com";
                listMessage.Add(msg);
            }
            else if (order.State == OrderState.RETURNING)
            {
                // 退订中
                var msg = $"{order.PO}，供应商: {order.Channel.ToUpper()}，您的退订请求已经提交成功，等待餐厅确认。";
                listMessage.Add(msg);
            }
            else if (order.State == OrderState.RETURNSUCCES)
            {
                // 退订成功
                var msg = $"{order.PO}，供应商: {order.Channel.ToUpper()}，您的退订已成功。";
                listMessage.Add(msg);
            }
            else if (order.State == OrderState.RETURNFAIL)
            {
                if (order.IsRetuen == OrderIsRetuen.FAIL)
                {
                    // 退订失败
                    if ((order.DeliverTime - order.ReturnOrderDate).TotalHours > 1)
                    {
                        // 距离配送时间大于一小时
                        var msg = "";
                        switch (order.Channel.ToLower())
                        {
                            case "xms":
                                msg = $"{order.PO}，供应商: {order.Channel.ToUpper()}，您的订单退订失败，原因: {order.XmsOrderReason}。如临时决定会议照常进行，请点击<a href='{WebConfigHandler.H5Domain}/P/Order/OriginalOrder/{order.ID.ToString()}?supplier=xms'>按原订单配送</a>，稍后呼叫中心人员会与您联系。如有疑问，请联系: 400-820-5577";
                                break;
                            case "bds":
                                msg = $"{order.PO}，供应商: {order.Channel.ToUpper()}，您的订单退订失败，原因: {order.XmsOrderReason}。如临时决定会议照常进行，请点击<a href='{WebConfigHandler.H5Domain}/P/Order/OriginalOrder/{order.ID.ToString()}?supplier=bds'>按原订单配送</a>，稍后呼叫中心人员会与您联系。如有疑问，请联系: 400-6868-912";
                                break;
                        }
                        listMessage.Add(msg);
                    }
                    else
                    {
                        // 距离配送时间小于一小时
                        var msg = "";
                        switch (order.Channel.ToLower())
                        {
                            case "xms":
                                msg = $"{order.PO}，供应商：{order.Channel.ToUpper()}，您的订单退订失败，原因：{order.XmsOrderReason}。如有疑问，请联系: 400-820-5577";
                                break;
                            case "bds":
                                msg = $"{order.PO}，供应商：{order.Channel.ToUpper()}，您的订单退订失败，原因：{order.XmsOrderReason}。如有疑问，请联系: 400-6868-912";
                                break;
                        }
                        listMessage.Add(msg);
                    }
                }
                else if (order.IsRetuen == OrderIsRetuen.POSTSUCCESS)
                {
                    // 按原订单配送请求提交成功
                    var msg = $"{order.PO}，供应商：{order.Channel.ToUpper()}，您的餐品将按原订单配送，请耐心等候。";
                    listMessage.Add(msg);
                }
                else if (order.IsRetuen == OrderIsRetuen.POSTFAIL)
                {
                    // 按原订单配送失败
                    var msg = "";
                    switch (order.Channel.ToLower())
                    {
                        case "xms":
                            msg = $"{order.PO}，供应商: {order.Channel.ToUpper()}，对不起，订单无法按原订单配送，原因: {order.XmsOrderReason}。稍后呼叫中心人员将会与您联系。如有疑问，请联系: 400-820-5577";
                            break;
                        case "bds":
                            msg = $"{order.PO}，供应商: {order.Channel.ToUpper()}，对不起，订单无法按原订单配送，原因: {order.XmsOrderReason}。稍后呼叫中心人员将会与您联系。如有疑问，请联系: 400-6868-912";
                            break;
                    }
                    listMessage.Add(msg);
                }

            }
            else if (order.State == OrderState.PERSIONRECEIVE)
            {
                // 人类收餐
                var msg = $"{order.PO}，供应商: {order.Channel.ToUpper()}，您已收餐完成, 请至“评价投诉”页面对本订单进行评价。";
                listMessage.Add(msg);
            }
            else if (order.State == OrderState.FOODLOSE)
            {
                // 未送达
                var msg = "";
                switch (order.Channel.ToLower())
                {
                    case "xms":
                        msg = $"{order.PO}，供应商: {order.Channel.ToUpper()}，我们已收到您的反馈，稍后呼叫中心人员会与您联系。如有疑问，请联系: 400-820-5577";
                        break;
                    case "bds":
                        msg = $"{order.PO}，供应商: {order.Channel.ToUpper()}，我们已收到您的反馈，稍后呼叫中心人员会与您联系。如有疑问，请联系: 400-6868-912";
                        break;
                }
                listMessage.Add(msg);
            }
            else if (order.State == OrderState.EVALUATED)
            {
                // 订单已经评价
                var msg = $"{order.PO}，供应商: {order.Channel.ToUpper()}，您的评价已提交，感谢您的反馈。";
                listMessage.Add(msg);
            }


            if (order.State == OrderState.SUBMITTED
                || order.State == OrderState.WAITAPPROVE
                || order.State == OrderState.EVALUATED)
            {
                var count = orderService.NotEvaluateCount(userid, order.IsNonHT);
                if (count > 0)
                {
                    var msg = $"您有未评价订单，请<a href='{WebConfigHandler.H5Domain}/P/Order/Index4'>点击这里</a>，选择相应订单进行评价。";
                    listMessage.Add(msg);
                }
            }


            SendQyMsg(listMessage, userid);
            SendMessageToWorkGroup(order);
        }
        #endregion

        #region 发送消息给订餐项目组
        /// <summary>
        /// 发送消息给订餐项目组
        /// </summary>
        public void SendMessageToWorkGroup(P_ORDER order)
        {
            P_WeChatOrder orderInfo = JsonConvert.DeserializeObject<P_WeChatOrder>(order.Detail);

            List<string> listMessage = new List<string>();
            List<string> listTouser = new List<string>();

            if (order.State == OrderState.WAITAPPROVE)
            {
                // 有MMCoE需要审批
                var msg = $"您有需要审批的订单，{order.PO}，供应商: {order.Channel.ToUpper()}，请<a href='{WebConfigHandler.H5Domain}/P/Order/OrderApproval/{order.ID}'>点击这里</a>。";
                listMessage.Add(msg);
                listTouser = groupMemberService.GetGroupMembersByType(GroupTypeEnum.MMCoE).Select(a => a.UserId).ToList();
            }
            else if (order.State == OrderState.FOODLOSE)
            {
                // 未送达
                var evaluate = evaluateService.LoadByOrderID(order.ID);
                if (evaluate != null)
                {
                    var msg = $"订单未送达：{evaluate.OnTimeDiscrpion}。{order.PO}，供应商: {order.Channel.ToUpper()}，{order.DeliverTime.ToString("yyyy-MM-dd HH:mm")}，医院:{orderInfo.preApproval.HospitalName}，收餐人:{order.Consignee}，收餐人电话:{order.Phone}";
                    listMessage.Add(msg);
                    listTouser = groupMemberService.GetGroupMembersByType(GroupTypeEnum.Complaints).Select(a => a.UserId).ToList();
                }

            }
            else if (order.State == OrderState.EVALUATED)
            {
                // 已评价
                var evaluate = evaluateService.LoadByOrderID(order.ID);
                if (evaluate != null)
                {
                    if (evaluate.OnTime == 5)
                    {
                        var msg = $"迟到超过60分钟：{evaluate.OnTimeDiscrpion}。{order.PO}，供应商: {order.Channel.ToUpper()}，{order.DeliverTime.ToString("yyyy-MM-dd HH:mm")}，医院:{orderInfo.preApproval.HospitalName}，餐厅:{order.RestaurantName}，收餐人:{order.Consignee}，收餐人电话:{order.Phone}";
                        listMessage.Add(msg);
                    }
                    if (evaluate.IsSafe == 1)
                    {
                        var msg = $"食品安全存在问题：{evaluate.SafeDiscrpion}。{order.PO}，供应商: {order.Channel.ToUpper()}，{order.DeliverTime.ToString("yyyy-MM-dd HH:mm")}，医院:{orderInfo.preApproval.HospitalName}，餐厅:{order.RestaurantName}，收餐人:{order.Consignee}，收餐人电话:{order.Phone}";
                        listMessage.Add(msg);
                    }

                    listTouser = groupMemberService.GetGroupMembersByType(GroupTypeEnum.Complaints).Select(a => a.UserId).ToList();
                }
            }
            else if (order.State == OrderState.SCHEDULEDFAIL)
            {
                var msg = $"预定失败:{order.CN}，原因:{order.XmsOrderReason}。供应商:{order.Channel.ToUpper()}，{order.DeliverTime.ToString("yyyy-MM-dd HH:mm")}，医院:{order.HospitalName}，餐厅:{order.RestaurantName}，收餐人:{order.Consignee}， 收餐人电话:{order.Phone}";
                //预定失败
                listTouser = groupMemberService.GetGroupMembersByType(GroupTypeEnum.Complaints).Select(a => a.UserId).ToList();
            }

            var touser = string.Join("|", listTouser);
            SendQyMsg(listMessage, touser);
        }
        #endregion

        #region 发出订单金额变动消息
        /// <summary>
        /// 发出订单金额变动消息
        /// </summary>
        /// <param name="order"></param>
        public void SendMessageForChangeFee(P_ORDER order)
        {
            var msg = $"{order.PO}，供应商: {order.Channel.ToUpper()}，调整内容及原因：{order.ChangeTotalPriceReason}";
            var listMessage = new List<string>()
            {
                msg
            };
            SendQyMsg(listMessage, order.UserId);
        }
        #endregion
        
    }
}