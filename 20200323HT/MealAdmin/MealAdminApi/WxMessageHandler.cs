using MealAdmin.Entity;
using MealAdmin.Entity.Enum;
using MealAdmin.Entity.Helper;
using MealAdmin.Service;
using MealAdmin.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using XFramework.WeChatAPI.Entity;
using XFramework.WeChatAPI.SessionHandlers;
using XFramework.XUtil;

namespace MealAdminApi
{
    /// <summary>
    /// P流程微信消息服务
    /// </summary>
    public class WxMessageHandler
    {
        private static WxMessageHandler _handler = new WxMessageHandler();
        public static WxMessageHandler GetInstance()
        {
            return _handler;
        }

        public QyApiHandler qyApiHandler = Global.applicationContext.GetBean("qyApiHandler") as QyApiHandler;
        public IOrderService orderService = Global.applicationContext.GetBean("orderService") as IOrderService;
        public IGroupMemberService groupMemberService = Global.applicationContext.GetBean("groupMemberService") as IGroupMemberService;
        public IEvaluateService evaluateService = Global.applicationContext.GetBean("evaluateService") as IEvaluateService;
        public IPreApprovalService preApprovalService = Global.applicationContext.GetBean("preApprovalService") as IPreApprovalService;
        public IUploadOrderService uploadOrderService = Global.applicationContext.GetBean("uploadOrderService") as IUploadOrderService;

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
                var res = qyApiHandler.Message_Send(new MessageSend()
                {
                    touser = userid,
                    content = msg,
                    msgtype = "text"
                });

                if(res.errcode == "-1")
                {
                    res = qyApiHandler.Message_Send(new MessageSend()
                    {
                        touser = userid,
                        content = msg,
                        msgtype = "text"
                    });
                }

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
            var isNotAssess = false;
            //var msg = string.Empty;
            if (order.State == OrderState.SUBMITTED)
            {
                // 订单提交成功，等待小秘书确认
                if (order.IsChange == OrderIsChange.YES)
                {
                    var msg = $"{order.CN}，供应商: {order.Channel.ToUpper()}，您的订单修改请求已经提交成功，正在等待餐厅确认。";
                    listMessage.Add(msg);
                }
                else if (order.MMCoEApproveState == MMCoEApproveState.APPROVESUCCESS)
                {
                    var msg = $"{order.CN}，供应商: {order.Channel.ToUpper()}，您的订单中央订餐项目组已审批，正在等待餐厅确认。";
                    listMessage.Add(msg);
                }
                else
                {
                    var msg = $"{order.CN}，供应商: {order.Channel.ToUpper()}，您的订单已提交成功，正在等待餐厅确认。";
                    isNotAssess = true;
                    listMessage.Add(msg);
                }
            }
            else if (order.State == OrderState.WAITAPPROVE)
            {
                // 订单提交成功，等待MMCoE审批
                var msg = $"{order.CN}，供应商: {order.Channel.ToUpper()}，您的订单已提交成功，正在等待中央订餐项目组确认。";
                listMessage.Add(msg);
            }
            else if (order.State == OrderState.SCHEDULEDSUCCESS)
            {
                // 小秘书反馈预定成功
                if (order.IsChange == OrderIsChange.SUCCESS)
                {
                    var msg = $"{order.CN}，供应商: {order.Channel.ToUpper()}，您的订单已修改成功，餐厅将依据修改后的订单进行配送。";
                    listMessage.Add(msg);
                }
                else if (order.IsChange == OrderIsChange.FAIL)
                {
                    var msg = "";
                    switch (order.Channel.ToLower())
                    {
                        case "xms":
                            msg = $"{order.CN}，供应商: {order.Channel.ToUpper()}，您的订单修改失败，原因: {order.XmsOrderReason}。您的餐品将按原订单配送。稍后呼叫中心人员会与您联系。如有疑问，请联系: 400-820-5577。";
                            break;
                        case "bds":
                            msg = $"{order.CN}，供应商: {order.Channel.ToUpper()}，您的订单修改失败，原因: {order.XmsOrderReason}。您的餐品将按原订单配送。稍后呼叫中心人员会与您联系。如有疑问，请联系: 400-6868-912。";
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
                                msg = $"{order.CN}，供应商: {order.Channel.ToUpper()}，您的订单已预订成功，收货码为{order.ReceiveCode}。请在收餐时主动出示收货码，祝您用餐愉快。如有疑问，请联系: 400-820-5577。";
                                break;
                            case "bds":
                                msg = $"{order.CN}，供应商: {order.Channel.ToUpper()}，您的订单已预订成功，收货码为{order.ReceiveCode}。请在收餐时主动出示收货码，祝您用餐愉快。如有疑问，请联系: 400-6868-912。";
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
                                msg = $"{order.CN}，供应商: {order.Channel.ToUpper()}，您的订单已预订成功，收货码为{order.ReceiveCode}；餐厅: {order.RestaurantName}；餐厅地址: {order.RestaurantAddress}；餐厅电话: {order.RestaurantTel}，请主动出示收货码并在会议结束后收取订餐小票及发票，祝您用餐愉快。如有疑问，请联系: 400-820-5577。";
                                break;
                            case "bds":
                                msg = $"{order.CN}，供应商: {order.Channel.ToUpper()}，您的订单已预订成功，收货码为{order.ReceiveCode}；餐厅: {order.RestaurantName}；餐厅地址: {order.RestaurantAddress}；餐厅电话: {order.RestaurantTel}，请主动出示收货码并在会议结束后收取订餐小票及发票，祝您用餐愉快。如有疑问，请联系: 400-6868-912。";
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
                        msg = $"{order.CN}，供应商: {order.Channel.ToUpper()}，您的订单预订失败，原因: {order.XmsOrderReason}。稍候呼叫中心人员会与您联系。如有疑问，请联系: 400-820-5577。";
                        break;
                    case "bds":
                        msg = $"{order.CN}，供应商: {order.Channel.ToUpper()}，您的订单预订失败，原因: {order.XmsOrderReason}。稍候呼叫中心人员会与您联系。如有疑问，请联系: 400-6868-912。";
                        break;
                }
                listMessage.Add(msg);
            }
            else if (order.State == OrderState.REJECT)
            {
                // MMCoE审批拒绝
                var msg = $"{order.CN}，供应商: {order.Channel.ToUpper()}，您的订单被中央订餐项目组驳回，原因: {order.MMCoEReason}。如需重新上传截图，请<a href='{WebConfigHandler.H5BaseDomain}/P/Food/MMCoEShell/{order.ID.ToString()}'>点击这里</a>，如需修改订单，请<a href='{WebConfigHandler.H5BaseDomain}/P/Order/Details/{order.ID.ToString()}?fromuri=2&supplier={order.Channel}'>点击这里</a>。如有疑问，请联系cn.chinarx-pmo@gsk.com";
                listMessage.Add(msg);
            }
            else if (order.State == OrderState.RETURNING)
            {
                // 退订中
                var msg = $"{order.CN}，供应商: {order.Channel.ToUpper()}，您的订单退单请求已经提交成功，正在等待餐厅确认。";
                listMessage.Add(msg);
            }
            else if (order.State == OrderState.RETURNSUCCES)
            {
                // 退订成功
                var msg = $"{order.CN}，供应商: {order.Channel.ToUpper()}，您的订单退单已成功。";
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
                                msg = $"{order.CN}，供应商: {order.Channel.ToUpper()}，您的订单退单失败，原因: {order.XmsOrderReason}。如临时决定会议照常进行，请点击<a href='{WebConfigHandler.H5BaseDomain}/P/Order/OriginalOrder/{order.ID.ToString()}?supplier=xms'>按原订单配送</a>，稍后呼叫中心人员会与您联系。如有疑问，请联系: 400-820-5577。";
                                break;
                            case "bds":
                                msg = $"{order.CN}，供应商: {order.Channel.ToUpper()}，您的订单退单失败，原因: {order.XmsOrderReason}。如临时决定会议照常进行，请点击<a href='{WebConfigHandler.H5BaseDomain}/P/Order/OriginalOrder/{order.ID.ToString()}?supplier=bds'>按原订单配送</a>，稍后呼叫中心人员会与您联系。如有疑问，请联系: 400-6868-912。";
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
                                msg = $"{order.CN}，供应商：{order.Channel.ToUpper()}，您的订单退单失败，原因：{order.XmsOrderReason}，稍后呼叫中心人员会与您联系。如有疑问，请联系: 400-820-5577。";
                                break;
                            case "bds":
                                msg = $"{order.CN}，供应商：{order.Channel.ToUpper()}，您的订单退单失败，原因：{order.XmsOrderReason}，稍后呼叫中心人员会与您联系。如有疑问，请联系: 400-6868-912。";
                                break;
                        }
                        listMessage.Add(msg);
                    }
                }
                else if (order.IsRetuen == OrderIsRetuen.POSTSUCCESS && order.ReturnOrderDate < order.DeliverTime)
                {
                    // 按原订单配送成功
                    var msg = $"{order.CN}，供应商: {order.Channel.ToUpper()}，您的餐品将按原订单配送，请耐心等待。";
                    listMessage.Add(msg);
                }
                else if (order.IsRetuen == OrderIsRetuen.POSTFAIL && order.ReturnOrderDate < order.DeliverTime)
                {
                    // 按原订单配送失败
                    var msg = "";
                    switch (order.Channel.ToLower())
                    {
                        case "xms":
                            msg = $"{order.CN}，供应商: {order.Channel.ToUpper()}，对不起，订单无法按原订单配送，原因: {order.XmsOrderReason}，稍后呼叫中心人员将会与您联系。如有疑问，请联系： 400-820-5577。";
                            break;
                        case "bds":
                            msg = $"{order.CN}，供应商: {order.Channel.ToUpper()}，对不起，订单无法按原订单配送，原因: {order.XmsOrderReason}，稍后呼叫中心人员将会与您联系。如有疑问，请联系： 400-6868-912。";
                            break;
                    }
                    listMessage.Add(msg);
                }
                else if (order.IsRetuen == OrderIsRetuen.POSTFOOD)
                {
                    // 发起按原订单配送
                    var msg = $"{order.CN}，供应商: {order.Channel.ToUpper()}，您的按原订单配送请求已提交成功，正在等待餐厅确认。";
                    listMessage.Add(msg);
                }
            }
            else if (order.State == OrderState.PERSIONRECEIVE)
            {
                // 人类收餐
                var msg = $"{order.CN}，您的订单已收餐完成，请<a href='{WebConfigHandler.H5BaseDomain}/P/Upload/UploadFiles?htCode={order.CN}&fileType=0'>点击这里</a>上传会议支持文件。";
                listMessage.Add(msg);
            }
            else if (order.State == OrderState.FOODLOSE)
            {
                // 未送达
                var msg = "";
                switch (order.Channel.ToLower())
                {
                    case "xms":
                        msg = $"{order.CN}，供应商: {order.Channel.ToUpper()}，我们已收到您的反馈，稍后呼叫中心人员会与您联系。如有疑问，请联系: 400-820-5577。";
                        break;
                    case "bds":
                        msg = $"{order.CN}，供应商: {order.Channel.ToUpper()}，我们已收到您的反馈，稍后呼叫中心人员会与您联系。如有疑问，请联系: 400-6868-912。";
                        break;
                }

                listMessage.Add(msg);
            }
            else if (order.State == OrderState.EVALUATED)
            {
                // 订单已经评价
                var msg = $"{order.CN}，供应商: {order.Channel.ToUpper()}，您的评价已提交成功，感谢您的反馈。";
                listMessage.Add(msg);
            }


            if (order.State == OrderState.SUBMITTED && isNotAssess)
            {
                var count = orderService.NotEvaluateCount(userid, order.IsNonHT);
                if (count > 0)
                {
                    var msg = $"您有未评价订单，请<a href='{WebConfigHandler.H5BaseDomain}/P/Order/Index4'>点击这里</a>，选择相应订单进行评价。";
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

            var preApproval = preApprovalService.FindPreApprovalByHTCode(order.CN).FirstOrDefault();

            List<string> listMessage = new List<string>();
            List<string> listTouser = new List<string>();

            if (order.State == OrderState.WAITAPPROVE)
            {
                // 有MMCoE需要审批
                var msg = $"您有需要审批的订单，{order.CN}，供应商：{order.Channel.ToUpper()}，请<a href='{WebConfigHandler.H5BaseDomain}/P/Order/OrderApproval/{order.ID}'>点击这里</a>。";
                listMessage.Add(msg);
                listTouser = groupMemberService.GetGroupMembersByType(GroupTypeEnum.MMCoE).Select(a => a.UserId).ToList();
            }
            else if (order.State == OrderState.FOODLOSE)
            {
                // 未送达
                var evaluate = evaluateService.LoadByOrderID(order.ID);
                if (evaluate != null)
                {
                    var msg = $"订单未送达：{evaluate.OnTimeDiscrpion}。{order.CN}，餐厅：{order.RestaurantName}，供应商：{order.Channel.ToUpper()}，{order.DeliverTime.ToString("yyyy-MM-dd HH:mm")}，医院：{orderInfo.preApproval.HospitalName}，收餐人：{order.Consignee}，收餐人电话：{order.Phone}。";
                    listMessage.Add(msg);
                    listTouser = groupMemberService.GetGroupMembersByType(GroupTypeEnum.Complaints).Select(a => a.UserId).ToList();

                    Task.Factory.StartNew(() =>
                    {
                        SendTSMailToVander(preApproval.HTCode, preApproval.MeetingDate.Value.ToString("yyyy-MM-dd HH:mm"), preApproval.HospitalName, "订单未送达", msg , order.Channel);
                    });
                    
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
                        var msg = $"迟到超过60分钟：{evaluate.OnTimeDiscrpion}。{order.CN}，餐厅：{order.RestaurantName}，供应商： {order.Channel.ToUpper()}，{order.DeliverTime.ToString("yyyy-MM-dd HH:mm")}，医院：{orderInfo.preApproval.HospitalName}，收餐人：{order.Consignee}，收餐人电话：{order.Phone}。";
                        listMessage.Add(msg);

                        Task.Factory.StartNew(() =>
                        {
                            SendTSMailToVander(preApproval.HTCode, preApproval.MeetingDate.Value.ToString("yyyy-MM-dd HH:mm"), preApproval.HospitalName, "迟到超过60分钟", msg, order.Channel);
                        });
                    }
                    if (evaluate.IsSafe == 1)
                    {
                        var msg = $"食品安全存在问题：{evaluate.SafeDiscrpion}。{order.CN}，餐厅：{order.RestaurantName}，供应商： {order.Channel.ToUpper()}，{order.DeliverTime.ToString("yyyy-MM-dd HH:mm")}，医院： {orderInfo.preApproval.HospitalName}，收餐人： {order.Consignee}，收餐人电话：{order.Phone}。";
                        listMessage.Add(msg);

                        Task.Factory.StartNew(() =>
                        {
                            SendTSMailToVander(preApproval.HTCode, preApproval.MeetingDate.Value.ToString("yyyy-MM-dd HH:mm"), preApproval.HospitalName, "食品安全存在问题", msg, order.Channel);
                        });
                        
                    }

                    listTouser = groupMemberService.GetGroupMembersByType(GroupTypeEnum.Complaints).Select(a => a.UserId).ToList();
                }
            }
            else if (order.State == OrderState.SCHEDULEDFAIL)
            {
                var msg = $"预订失败：{order.CN}，原因：{order.XmsOrderReason}，供应商：{order.Channel.ToUpper()}，{order.DeliverTime.ToString("yyyy-MM-dd HH:mm")}，医院：{order.HospitalName}，餐厅：{order.RestaurantName}，收餐人：{order.Consignee}， 收餐人电话：{order.Phone}。";
                listMessage.Add(msg);
                //预定失败
                listTouser = groupMemberService.GetGroupMembersByType(GroupTypeEnum.Complaints).Select(a => a.UserId).ToList();

                Task.Factory.StartNew(() =>
                {
                    SendTSMailToVander(preApproval.HTCode, preApproval.MeetingDate.Value.ToString("yyyy-MM-dd HH:mm"), preApproval.HospitalName, "预订失败", msg, order.Channel);
                });
                

            }

            

            var touser = string.Join("|", listTouser);
            SendQyMsg(listMessage, touser);
        }

        public void SendApproveStepErrorMessageToGroup(P_PreApproval preApproval)
        {
            List<string> listMessage = new List<string>();
            List<string> listTouser = new List<string>();

            var msg = $"预申请审批流程有误：{preApproval.HTCode}，TA：{preApproval.TA}，会议时间：{preApproval.MeetingDate.Value.ToString("yyyy-MM-dd HH:mm")}，参会人数：{preApproval.AttendCount}，预算金额：{preApproval.BudgetTotal}元，医院：{preApproval.HospitalName}，申请人：{preApproval.ApplierName}， 申请人电话：{preApproval.ApplierMobile}。";
            listMessage.Add(msg);
            //预定失败
            listTouser = groupMemberService.GetGroupMembersByType(GroupTypeEnum.Complaints).Select(a => a.UserId).ToList();

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
            var msg = $"{order.CN}，供应商：{order.Channel.ToUpper()}，调整内容及原因：{order.ChangeTotalPriceReason}";
            var listMessage = new List<string>()
            {
                msg
            };
            SendQyMsg(listMessage, order.UserId);
        }
        #endregion

        #region 发送预申请状态变动通知给申请人
        public void SendPreApprovalStateChangeMessageToUser(P_PreApproval preApproval)
        {
            List<string> listMessage = new List<string>();
            List<string> listTouser = new List<string>();
            var applicantMsg = "";
            var approverMsg = "";
            var touser = "";
            var hisList = preApprovalService.FindPreApprovalApproveHistory(preApproval.ID);
            P_PreApprovalApproveHistory lastMMCoEApprove = null;
            if (hisList.Count > 0)
            {
                lastMMCoEApprove = hisList.Where(p => p.PID == preApproval.ID && p.type == 1).OrderByDescending(p => p.ApproveDate).FirstOrDefault();
            }
            var isReject = true;
            if (lastMMCoEApprove != null)
            {
                if (lastMMCoEApprove.ActionType == 3)
                {
                    isReject = false;
                }
            }
            switch (preApproval.State)
            {
                //预申请提交成功
                case "0":
                    applicantMsg = $"{preApproval.HTCode}，您的预申请已提交成功。";
                    listMessage.Add(applicantMsg);
                    listTouser.Add(preApproval.ApplierMUDID);
                    touser = string.Join("|", listTouser);
                    SendQyMsg(listMessage, touser);
                    break;
                //预申请等待MMCoE审批
                case "1":
                    applicantMsg = preApproval.ModifyDate == preApproval.CreateDate ? $"{preApproval.HTCode}，您的预申请已提交成功，正在等待中央订餐项目组审批。" : $"{preApproval.HTCode}，您的预申请修改已提交成功，正在等待中央订餐项目组审批。";
                    listMessage.Add(applicantMsg);
                    listTouser.Add(preApproval.ApplierMUDID);
                    touser = string.Join("|", listTouser);
                    SendQyMsg(listMessage, touser);
                    listMessage.Clear();
                    listTouser.Clear();
                    approverMsg = preApproval.ModifyDate == preApproval.CreateDate ? $"{preApproval.HTCode}，您有需要审批的预申请。请<a href='{WebConfigHandler.H5BaseDomain}/P/PreApproval/MMCoEApprove/{preApproval.ID}'>点击这里</a>进行审批。" : $"{preApproval.HTCode}，您有需要审批的预申请修改。请<a href='{WebConfigHandler.H5BaseDomain}/P/PreApproval/MMCoEApprove/{preApproval.ID}'>点击这里</a>进行审批。";
                    listMessage.Add(approverMsg);
                    listTouser.AddRange(groupMemberService.GetGroupMembersByType(GroupTypeEnum.MMCoE).Select(a => a.UserId).ToList());
                    touser = string.Join("|", listTouser);
                    SendQyMsg(listMessage, touser);
                    break;
                //预申请等待BUHead审批
                case "3":
                    //if (preApproval.AttendCount >= 60 && (preApproval.IsMMCoEChange == true || preApproval.ModifyDate == null || isReject == true))
                    //    applicantMsg = preApproval.ModifyDate == preApproval.CreateDate ? $"{preApproval.HTCode}，您的预申请中央订餐项目组已审批通过，正在等待BU Head（Rx/Vx）或销售总监（DDT/TSKF）审批。" : $"{preApproval.HTCode}，您的预申请修改中央订餐项目组已审批通过，正在等待BU Head（Rx/Vx）或销售总监（DDT/TSKF）审批。";
                    //else
                    if (preApproval.IsOnc == 1)
                    {
                        applicantMsg = preApproval.ModifyDate == preApproval.CreateDate ? $"{preApproval.HTCode}，您的预申请已提交成功，正在等待BU Head（Rx/Vx）或销售总监（DDT/TSKF）审批。" : $"{preApproval.HTCode}，您的预申请修改已提交成功，正在等待BU Head（Rx/Vx）或销售总监（DDT/TSKF）审批。";
                        listMessage.Add(applicantMsg);
                        listTouser.Add(preApproval.ApplierMUDID);
                        touser = string.Join("|", listTouser);
                        SendQyMsg(listMessage, touser);
                        listMessage.Clear();
                        listTouser.Clear();
                    }

                    approverMsg = preApproval.ModifyDate == preApproval.CreateDate ? $"{preApproval.HTCode}，您有需要审批的预申请。请<a href='{WebConfigHandler.H5BaseDomain}/P/PreApproval/Approval?id={preApproval.ID}&from=0'>点击这里</a>进行审批。" : $"{preApproval.HTCode}，您有需要审批的预申请修改。请<a href='{WebConfigHandler.H5BaseDomain}/P/PreApproval/Approval?id={preApproval.ID}&from=0'>点击这里</a>进行审批。";
                    listMessage.Add(approverMsg);
                    listTouser.Add(preApproval.CurrentApproverMUDID);
                    touser = string.Join("|", listTouser);
                    SendQyMsg(listMessage, touser);


                    break;

                //预申请等待2级经理审批
                case "7":
                    //if (preApproval.AttendCount >= 60 && (preApproval.IsMMCoEChange == true || preApproval.ModifyDate == null || isReject == true))
                    //    applicantMsg = preApproval.ModifyDate == preApproval.CreateDate ? $"{preApproval.HTCode}，您的预申请中央订餐项目组已审批通过，正在等待BU Head（Rx/Vx）或销售总监（DDT/TSKF）审批。" : $"{preApproval.HTCode}，您的预申请修改中央订餐项目组已审批通过，正在等待BU Head（Rx/Vx）或销售总监（DDT/TSKF）审批。";
                    //else
                    applicantMsg = preApproval.ModifyDate == preApproval.CreateDate ? $"{preApproval.HTCode}，您的预申请已提交成功，正在等待二级经理审批。" : $"{preApproval.HTCode}，您的预申请修改已提交成功，正在等待二级经理审批。";
                    listMessage.Add(applicantMsg);
                    listTouser.Add(preApproval.ApplierMUDID);
                    touser = string.Join("|", listTouser);
                    SendQyMsg(listMessage, touser);
                    listMessage.Clear();
                    listTouser.Clear();

                    approverMsg = preApproval.ModifyDate == preApproval.CreateDate ? $"{preApproval.HTCode}，您有需要审批的预申请。请<a href='{WebConfigHandler.H5BaseDomain}/P/PreApproval/Approval?id={preApproval.ID}&from=0'>点击这里</a>进行审批。" : $"{preApproval.HTCode}，您有需要审批的预申请修改。请<a href='{WebConfigHandler.H5BaseDomain}/P/PreApproval/Approval?id={preApproval.ID}&from=0'>点击这里</a>进行审批。";
                    listMessage.Add(approverMsg);
                    listTouser.Add(preApproval.CurrentApproverMUDID);
                    touser = string.Join("|", listTouser);
                    SendQyMsg(listMessage, touser);
                    break;
                //预申请BU Head审批通过
                case "5":
                    if (preApproval.IsOnc != 2)
                    {
                        listMessage.Clear();
                        listTouser.Clear();
                        applicantMsg = preApproval.ModifyDate == preApproval.CreateDate ? $"{preApproval.HTCode}，您的预申请BU Head（Rx/Vx）或销售总监（DDT/TSKF）已审批通过。" : $"{preApproval.HTCode}，您的预申请修改BU Head（Rx/Vx）或销售总监（DDT/TSKF）已审批通过。";
                        listMessage.Add(applicantMsg);
                        listTouser.Add(preApproval.ApplierMUDID);
                        touser = string.Join("|", listTouser);
                        SendQyMsg(listMessage, touser);
                    }
                    //applicantMsg = preApproval.ModifyDate == preApproval.CreateDate ? $"{preApproval.HTCode}，您的预申请BU Head（Rx/Vx）或销售总监（DDT/TSKF）已审批通过。" : $"{preApproval.HTCode}，您的预申请修改BU Head（Rx/Vx）或销售总监（DDT/TSKF）已审批通过。";
                    //listMessage.Add(applicantMsg);
                    //listTouser.Add(preApproval.ApplierMUDID);
                    //touser = string.Join("|", listTouser);
                    //SendQyMsg(listMessage, touser);
                    if (preApproval.IsOnc == 2)
                    {
                        listMessage.Clear();
                        listTouser.Clear();
                        applicantMsg = preApproval.ModifyDate == preApproval.CreateDate ? $"{preApproval.HTCode}，您的预申请已提交成功。" : $"{preApproval.HTCode}，您的预申请修改已提交成功。";
                        listMessage.Add(applicantMsg);
                        listTouser.Add(preApproval.ApplierMUDID);
                        touser = string.Join("|", listTouser);
                        SendQyMsg(listMessage, touser);
                        listMessage.Clear();
                        listTouser.Clear();

                        applicantMsg = preApproval.ModifyDate == preApproval.CreateDate ? $"{preApproval.HTCode}，您的预申请已审批通过。" : $"{preApproval.HTCode}，您的预申请修改已审批通过。";
                        listMessage.Add(applicantMsg);
                        listTouser.Add(preApproval.ApplierMUDID);
                        touser = string.Join("|", listTouser);
                        SendQyMsg(listMessage, touser);
                    }
                    if (preApproval.AttendCount >= 60)
                    {
                        listMessage.Clear();
                        listTouser.Clear();

                        applicantMsg = $"预申请>=60人，" + preApproval.HTCode + "，TA：" + preApproval.TA + "，会议时间：" + preApproval.MeetingDate.Value.ToString("yyyy-MM-dd HH:mm") + "，参会人数：" + preApproval.AttendCount + "，预算金额：" + preApproval.BudgetTotal + "，医院：" + preApproval.HospitalName + "，申请人：" + preApproval.ApplierName + "，申请人电话：" + preApproval.ApplierMobile + "。";
                        listMessage.Add(applicantMsg);
                        listTouser = groupMemberService.GetGroupMembersByType(GroupTypeEnum.Complaints).Select(a => a.UserId).ToList();
                        touser = string.Join("|", listTouser);
                        SendQyMsg(listMessage, touser);

                        SendPreMailToVander(preApproval.HTCode, preApproval.MeetingDate.Value.ToString("yyyy-MM-dd HH:mm"), preApproval.HospitalName, preApproval.BudgetTotal.ToString(), applicantMsg);
                    }

                    if (preApproval.BudgetTotal >= 1500)
                    {
                        listMessage.Clear();
                        listTouser.Clear();

                        applicantMsg = $"预算金额>=1500元，" + preApproval.HTCode + "，TA：" + preApproval.TA + "，会议时间：" + preApproval.MeetingDate.Value.ToString("yyyy-MM-dd HH:mm") + "，参会人数：" + preApproval.AttendCount + "，预算金额：" + preApproval.BudgetTotal + "，医院：" + preApproval.HospitalName + "，申请人：" + preApproval.ApplierName + "，申请人电话：" + preApproval.ApplierMobile + "。";
                        listMessage.Add(applicantMsg);
                        listTouser = groupMemberService.GetGroupMembersByType(GroupTypeEnum.Complaints).Select(a => a.UserId).ToList();
                        touser = string.Join("|", listTouser);
                        SendQyMsg(listMessage, touser);

                        Task.Factory.StartNew(() =>
                        {
                            SendPreMailToVander(preApproval.HTCode, preApproval.MeetingDate.Value.ToString("yyyy-MM-dd HH:mm"), preApproval.HospitalName, preApproval.BudgetTotal.ToString(), applicantMsg);
                        });
                    }

                    break;
                //预申请2级经理审批通过
                case "9":
                    applicantMsg = preApproval.ModifyDate == preApproval.CreateDate ? $"{preApproval.HTCode}，您的预申请二级经理已审批通过。" : $"{preApproval.HTCode}，您的预申请修改二级经理已审批通过。";
                    listMessage.Add(applicantMsg);
                    listTouser.Add(preApproval.ApplierMUDID);
                    touser = string.Join("|", listTouser);
                    SendQyMsg(listMessage, touser);
                    if (preApproval.AttendCount >= 60)
                    {
                        listMessage.Clear();
                        listTouser.Clear();

                        applicantMsg = $"预申请>=60人，" + preApproval.HTCode + "，TA：" + preApproval.TA + "，会议时间：" + preApproval.MeetingDate.Value.ToString("yyyy-MM-dd HH:mm") + "，参会人数：" + preApproval.AttendCount + "，预算金额：" + preApproval.BudgetTotal + "，医院：" + preApproval.HospitalName + "，申请人：" + preApproval.ApplierName + "，申请人电话：" + preApproval.ApplierMobile + "。";
                        listMessage.Add(applicantMsg);
                        listTouser = groupMemberService.GetGroupMembersByType(GroupTypeEnum.Complaints).Select(a => a.UserId).ToList();
                        touser = string.Join("|", listTouser);
                        SendQyMsg(listMessage, touser);

                        Task.Factory.StartNew(() =>
                        {
                            SendPreMailToVander(preApproval.HTCode, preApproval.MeetingDate.Value.ToString("yyyy-MM-dd HH:mm"), preApproval.HospitalName, preApproval.BudgetTotal.ToString(), applicantMsg);
                        });
                        
                    }


                    break;
                case "6":
                    //判断参会人数是否超过60人
                    //if (preApproval.AttendCount >= 60)
                    //{
                    //    //新订单
                    //    if (preApproval.ModifyDate == preApproval.CreateDate)
                    //    {
                    //        applicantMsg = $"{preApproval.HTCode}，您的预申请中央订餐项目组已审批通过。";
                    //        listMessage.Add(applicantMsg);
                    //        listTouser.Add(preApproval.ApplierMUDID);
                    //        touser = string.Join("|", listTouser);
                    //        SendQyMsg(listMessage, touser);
                    //    }
                    //    else
                    //    {
                    //        //判断参会人数，图片是否发生变化
                    //        if (preApproval.IsMMCoEChange || isReject==true)
                    //        {
                    //            applicantMsg = $"{preApproval.HTCode}，您的预申请修改中央订餐项目组已审批通过。";
                    //            listMessage.Add(applicantMsg);
                    //            listTouser.Add(preApproval.ApplierMUDID);
                    //            touser = string.Join("|", listTouser);
                    //            SendQyMsg(listMessage, touser);
                    //        }
                    //        else
                    //        {
                    //            applicantMsg = $"{preApproval.HTCode}，您的预申请修改已提交成功。";
                    //            listMessage.Add(applicantMsg);
                    //            listTouser.Add(preApproval.ApplierMUDID);
                    //            touser = string.Join("|", listTouser);
                    //            SendQyMsg(listMessage, touser);
                    //            listMessage.Clear();
                    //            listTouser.Clear();

                    //            applicantMsg = $"{preApproval.HTCode}，您的预申请修改已审批通过。";
                    //            listMessage.Add(applicantMsg);
                    //            listTouser.Add(preApproval.ApplierMUDID);
                    //            touser = string.Join("|", listTouser);
                    //            SendQyMsg(listMessage, touser);
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    applicantMsg = preApproval.ModifyDate == preApproval.CreateDate ? $"{preApproval.HTCode}，您的预申请已提交成功。" : $"{preApproval.HTCode}，您的预申请修改已提交成功。";
                    listMessage.Add(applicantMsg);
                    listTouser.Add(preApproval.ApplierMUDID);
                    touser = string.Join("|", listTouser);
                    SendQyMsg(listMessage, touser);
                    listMessage.Clear();
                    listTouser.Clear();

                    applicantMsg = preApproval.ModifyDate == preApproval.CreateDate ? $"{preApproval.HTCode}，您的预申请已审批通过。" : $"{preApproval.HTCode}，您的预申请修改已审批通过。";
                    listMessage.Add(applicantMsg);
                    listTouser.Add(preApproval.ApplierMUDID);
                    touser = string.Join("|", listTouser);
                    SendQyMsg(listMessage, touser);
                    if (preApproval.AttendCount >= 60 && preApproval.BudgetTotal >= 0 && preApproval.BudgetTotal < 1500)
                    {
                        listMessage.Clear();
                        listTouser.Clear();

                        applicantMsg = $"预申请>=60人，" + preApproval.HTCode + "，TA：" + preApproval.TA + "，会议时间：" + preApproval.MeetingDate.Value.ToString("yyyy-MM-dd HH:mm") + "，参会人数：" + preApproval.AttendCount + "，预算金额：" + preApproval.BudgetTotal + "，医院：" + preApproval.HospitalName + "，申请人：" + preApproval.ApplierName + "，申请人电话：" + preApproval.ApplierMobile + "。";
                        listMessage.Add(applicantMsg);
                        listTouser = groupMemberService.GetGroupMembersByType(GroupTypeEnum.Complaints).Select(a => a.UserId).ToList();
                        touser = string.Join("|", listTouser);
                        SendQyMsg(listMessage, touser);

                        Task.Factory.StartNew(() =>
                        {
                            SendPreMailToVander(preApproval.HTCode, preApproval.MeetingDate.Value.ToString("yyyy-MM-dd HH:mm"), preApproval.HospitalName, preApproval.BudgetTotal.ToString(), applicantMsg);
                        });
                    }

                    if (preApproval.BudgetTotal >= 1500 && preApproval.IsHosOrMeetingTimeChange == true)
                    {
                        listMessage.Clear();
                        listTouser.Clear();

                        applicantMsg = $"预算金额>=1500元，" + preApproval.HTCode + "，TA：" + preApproval.TA + "，会议时间：" + preApproval.MeetingDate.Value.ToString("yyyy-MM-dd HH:mm") + "，参会人数：" + preApproval.AttendCount + "，预算金额：" + preApproval.BudgetTotal + "，医院：" + preApproval.HospitalName + "，申请人：" + preApproval.ApplierName + "，申请人电话：" + preApproval.ApplierMobile + "。";
                        listMessage.Add(applicantMsg);
                        listTouser = groupMemberService.GetGroupMembersByType(GroupTypeEnum.Complaints).Select(a => a.UserId).ToList();
                        touser = string.Join("|", listTouser);
                        SendQyMsg(listMessage, touser);

                        Task.Factory.StartNew(() =>
                        {
                            SendPreMailToVander(preApproval.HTCode, preApproval.MeetingDate.Value.ToString("yyyy-MM-dd HH:mm"), preApproval.HospitalName, preApproval.BudgetTotal.ToString(), applicantMsg);
                        });
                    }
                    break;
            }
        }
        #endregion

        #region 发送预申请审批驳回状态变动通知给申请人
        public void SendPreApprovalRejectMessageToUser(string Comments, P_PreApproval preApproval)
        {
            List<string> listMessage = new List<string>();
            List<string> listTouser = new List<string>();
            var applicantMsg = "";
            switch (preApproval.State)
            {
                case "2":
                    applicantMsg = preApproval.ModifyDate == preApproval.CreateDate ? $"{preApproval.HTCode}，您的预申请已被中央订餐项目组审批驳回，原因：{Comments}。如需重新上传MMCoE文件，请<a href='{WebConfigHandler.H5BaseDomain}/P/Food/MMCoEShell/{preApproval.ID}'>点击这里</a>。如需修改预申请，请<a href='{WebConfigHandler.H5BaseDomain}/P/PreApproval/Edit/{preApproval.ID}'>点击这里</a>。如有疑问，请联系：cn.chinarx-pmo@gsk.com。" : $"{preApproval.HTCode}，您的预申请修改已被中央订餐项目组审批驳回，原因：{Comments}。如需重新上传MMCoE文件，请<a href='{WebConfigHandler.H5BaseDomain}/P/Food/MMCoEShell/{preApproval.ID}'>点击这里</a>。如需修改预申请，请<a href='{WebConfigHandler.H5BaseDomain}/P/PreApproval/Edit/{preApproval.ID}'>点击这里</a>。如有疑问，请联系：cn.chinarx-pmo@gsk.com。";
                    listMessage.Add(applicantMsg);
                    listTouser.Add(preApproval.ApplierMUDID);
                    break;
                case "4":
                    applicantMsg = preApproval.ModifyDate == preApproval.CreateDate ? $"{preApproval.HTCode}，您的预申请已被BU Head（Rx/Vx）或销售总监（DDT/TSKF）审批驳回，原因：{Comments}。如需修改预申请，请<a href='{WebConfigHandler.H5BaseDomain}/P/PreApproval/Edit/{preApproval.ID}'>点击这里</a>。" : $"{preApproval.HTCode}，您的预申请修改已被BU Head（Rx/Vx）或销售总监（DDT/TSKF）审批驳回，原因：{Comments}。如需修改预申请，请<a href='{WebConfigHandler.H5BaseDomain}/P/PreApproval/Edit/{preApproval.ID}'>点击这里</a>。";
                    listMessage.Add(applicantMsg);
                    listTouser.Add(preApproval.ApplierMUDID);
                    break;
                case "8":
                    applicantMsg = preApproval.ModifyDate == preApproval.CreateDate ? $"{preApproval.HTCode}，您的预申请已被二级经理审批驳回，原因：{Comments}。如需修改预申请，请<a href='{WebConfigHandler.H5BaseDomain}/P/PreApproval/Edit/{preApproval.ID}'>点击这里</a>。" : $"{preApproval.HTCode}，您的预申请修改已被二级经理审批驳回，原因：{Comments}。如需修改预申请，请<a href='{WebConfigHandler.H5BaseDomain}/P/PreApproval/Edit/{preApproval.ID}'>点击这里</a>。";
                    listMessage.Add(applicantMsg);
                    listTouser.Add(preApproval.ApplierMUDID);
                    break;
                case "10":
                    applicantMsg = $"{preApproval.HTCode}，您的预申请已取消成功。";
                    listMessage.Add(applicantMsg);
                    listTouser.Add(preApproval.ApplierMUDID);
                    break;
            }
            var touser = string.Join("|", listTouser);
            SendQyMsg(listMessage, touser);
        }
        #endregion

        #region 发送上传文件状态变动通知给申请人
        public void SendOrderStateChangeMessageToUser(P_PREUPLOADORDER order)
        {
            List<string> listMessage = new List<string>();
            List<string> listTouser = new List<string>();
            var applicantMsg = "";
            var approverMsg = "";
            var touser = "";
            switch (order.State)
            {
                //上传文件等待直线经理审批
                case "1":
                    string messageBase = order.ModifyDate == order.CreateDate ? "您的会议支持文件已上传，正在等待直线经理审批。" : "您的会议支持文件已修改，正在等待直线经理审批。";
                    switch (order.FileType)
                    {
                        case 1:
                            messageBase = order.ModifyDate == order.CreateDate ? "您的退单原因已提交，正在等待直线经理审批。" : "您的退单原因已修改，正在等待直线经理审批。";
                            break;
                        case 2:
                            messageBase = order.ModifyDate == order.CreateDate ? "您的会议支持文件丢失原因已提交，正在等待RD/SD审批。" : "您的会议支持文件丢失原因已修改，正在等待RD/SD审批。";
                            break;
                        case 3:
                            messageBase = order.ModifyDate == order.CreateDate ? "您的未送达，会议未正常召开原因已提交，正在等待直线经理审批。" : "您的未送达，会议未正常召开原因已修改，正在等待直线经理审批。";
                            break;
                    }
                    applicantMsg = $"{order.HTCode}，{messageBase}";
                    listMessage.Add(applicantMsg);
                    listTouser.Add(order.IsTransfer == 1 ? order.TransferUserMUDID : order.ApplierMUDID);
                    touser = string.Join("|", listTouser);
                    SendQyMsg(listMessage, touser);
                    listMessage.Clear();
                    listTouser.Clear();

                    messageBase = "该订单已上传会议支持文件";
                    switch (order.FileType)
                    {
                        case 1:
                            messageBase = "该订单已提交退单原因";
                            break;
                        case 2:
                            messageBase = "该订单已提交会议支持文件丢失原因";
                            break;
                        case 3:
                            messageBase = "该订单已提交未送达，会议未正常召开原因";
                            break;
                    }
                    approverMsg = $"{order.HTCode}，{messageBase}，请<a href='{WebConfigHandler.H5BaseDomain}/P/Upload/Approval?id={order.ID}&from=0'>点击这里</a>进行审批。";
                    listMessage.Add(approverMsg);
                    listTouser.Add(order.IsReAssign ? order.ReAssignBUHeadMUDID : order.BUHeadMUDID);
                    touser = string.Join("|", listTouser);
                    SendQyMsg(listMessage, touser);
                    break;

                case "4":
                    string messageBaseComplete = order.CreateDate == order.ModifyDate ? "您的会议支持文件已审批通过。" : "您的会议支持文件修改已审批通过。";
                    switch (order.FileType)
                    {
                        case 1:
                            messageBaseComplete = order.CreateDate == order.ModifyDate ? "您的退单原因已审批通过。" : "您的退单原因修改已审批通过。";
                            break;
                        case 2:
                            messageBaseComplete = order.CreateDate == order.ModifyDate ? "您的会议支持文件丢失原因已审批通过。" : "您的会议支持文件丢失原因修改已审批通过。";
                            break;
                        case 3:
                            messageBaseComplete = order.CreateDate == order.ModifyDate ? "您的未送达，会议未正常召开原因已审批通过。" : "您的未送达，会议未正常召开原因修改已审批通过。";
                            break;
                    }
                    applicantMsg = $"{order.HTCode}，{messageBaseComplete}";
                    listMessage.Add(applicantMsg);
                    listTouser.Add(order.IsTransfer == 1 ? order.TransferUserMUDID : order.ApplierMUDID);
                    touser = string.Join("|", listTouser);
                    SendQyMsg(listMessage, touser);
                    break;
            }
        }
        #endregion

        #region 发送上传文件审批驳回状态变动通知给申请人
        public void SendOrderRejectMessageToUser(string Comments, P_PREUPLOADORDER order)
        {
            List<string> listMessage = new List<string>();
            List<string> listTouser = new List<string>();
            var applicantMsg = "";
            switch (order.State)
            {
                case "2":
                    string messageBase = order.CreateDate == order.ModifyDate ? "您的会议支持文件已被直线经理审批驳回" : "您的会议支持文件修改已被直线经理审批驳回";
                    switch (order.FileType)
                    {
                        case 1:
                            messageBase = order.CreateDate == order.ModifyDate ? "您的退单原因已被直线经理审批驳回" : "您的退单原因修改已被直线经理审批驳回";
                            break;
                        case 2:
                            messageBase = order.CreateDate == order.ModifyDate ? "您的会议支持文件丢失原因已被RD/SD审批驳回" : "您的会议支持文件丢失原因修改已被RD/SD审批驳回";
                            break;
                        case 3:
                            messageBase = order.CreateDate == order.ModifyDate ? "您的未送达，会议未正常召开原因已被直线经理审批驳回" : "您的未送达，会议未正常召开原因修改已被直线经理审批驳回";
                            break;
                    }
                    applicantMsg = $"{order.HTCode}，{messageBase}，原因：{Comments}。如需重新上传，请<a href='{WebConfigHandler.H5BaseDomain}/P/Upload/EditUploadFiles/{order.ID}'>点击这里</a>。";
                    listMessage.Add(applicantMsg);
                    listTouser.Add(order.IsTransfer == 1 ? order.TransferUserMUDID : order.ApplierMUDID);
                    break;
                case "3":
                    applicantMsg = order.ModifyDate == order.CreateDate ? $"{order.HTCode}，您的上传文件已被财务审批驳回，原因：{Comments}。如需修改上传文件，请<a href='{WebConfigHandler.H5BaseDomain}/P/PreApproval/Edit/{order.ID}'>点击这里</a>。" : $"{order.HTCode}，您的上传文件修改已被财务审批驳回，原因：{Comments}。如需修改上传文件，请<a href='{WebConfigHandler.H5BaseDomain}/P/PreApproval/Edit/{order.ID}'>点击这里</a>。";
                    listMessage.Add(applicantMsg);
                    listTouser.Add(order.IsTransfer == 1 ? order.TransferUserMUDID : order.ApplierMUDID);
                    break;
            }
            var touser = string.Join("|", listTouser);
            SendQyMsg(listMessage, touser);
        }
        #endregion


        #region *******************任务计划消息推送**********************

        #region 发送确认收餐消息
        /// <summary>
        /// 发送确认收餐消息
        /// </summary>
        /// <param name="order"></param>
        public void SendMessageForConfirm(List<string> listTouser, int state)
        {
            var msg = "";
            if (state == 1)
            {
                msg = $"您有未收餐的订单，请<a href='{WebConfigHandler.H5BaseUrl}/s/0x0016'>点击这里</a>，选择相应订单进行收餐。";
            }
            else if (state == 2)
            {
                msg = $"您有未完成收餐的订单。请<a href='{WebConfigHandler.H5BaseUrl}/s/0x0016'>点击这里</a>，选择相应订单并进行收餐操作。如48小时内未完成收餐，则将暂停您的订餐服务权限。如有疑问，请联系：cn.chinarx-pmo@gsk.com。";
            }
            else if (state == 3)
            {
                msg = $"您有未完成上传文件的订单。请<a href='{WebConfigHandler.H5BaseUrl}/s/0x0018'>点击这里</a>，选择相应订单并进行上传文件操作。如7个自然日内未完成上传文件，则将暂停您的订餐服务权限。如有疑问，请联系：cn.chinarx-pmo@gsk.com。";
            }
            else if (state == 4)
            {
                msg = $"您有未完成直线经理审批的订单。请<a href='{WebConfigHandler.H5BaseUrl}/s/0x0019'>点击这里</a>，查看相应订单并提醒您的直线经理进行审批。如7个自然日内未完成直线经理审批，则将暂停您的订餐服务权限。如有疑问，请联系：cn.chinarx-pmo@gsk.com。";
            }
            else if (state == 5)
            {
                msg = $"您未在48小时内完成收餐，已被暂停订餐服务权限。请<a href='{WebConfigHandler.H5BaseUrl}/s/0x0016'>点击这里</a>，选择相应订单并进行收餐操作。待您操作完成后，系统会自动开通您的服务。如有疑问，请联系：cn.chinarx-pmo@gsk.com。";
            }
            else if (state == 6)
            {
                msg = $"您未在7个自然日内完成上传文件，现已被暂停订餐服务权限。请<a href='{WebConfigHandler.H5BaseUrl}/s/0x0018'>点击这里</a>，选择相应订单并进行上传文件操作。待您操作完成后，系统会自动开通您的服务。如有疑问，请联系：cn.chinarx-pmo@gsk.com。";
            }
            else if (state == 7)
            {
                msg = $"您的直线经理在未在7个自然日未完成直线经理审批，您已被暂停订餐服务权限。请<a href='{WebConfigHandler.H5BaseUrl}/s/0x0019'>点击这里</a>，查看相应订单并提醒您的直线经理进行审批。待您的直线经理操作完成后，系统会自动开通您的服务。如有疑问，请联系：cn.chinarx-pmo@gsk.com。";
            }
            else if (state == 8)
            {
                msg = $"您有未上传会议支持文件的订单，请<a href='{WebConfigHandler.H5BaseUrl}/s/0x0018'>点击这里</a>，选择相应订单进行上传文件。";
            }
            var touser = string.Join("|", listTouser);
            var listMessage = new List<string>()
            {
                msg
            };
            SendQyMsg(listMessage, touser);
        }
        #endregion

        #region 发送自动转交消息
        // Start UpdateBy zhexin.zou at 20190104
        public void SendMessageForAutoTransfer(List<string> ListTouser, string HTCodeList, string UserName, string UserId, int HTCount, int Type)
        {
            var msg = $"您的下属{UserName}（MUDID：{UserId}）已离职，其尚未完成上传文件审批通过的HT订单{HTCount}单，现已分配给您。HT编号如下：{HTCodeList}。请<a href='{WebConfigHandler.H5BaseDomain}/P/Upload/AutoTransferState'>点击这里</a>，查看HT订单详情；请您尽快完成上传文件，如7个自然日内未完成上传文件，则将暂停您的订餐服务权限。如有疑问，请联系：cn.chinarx-pmo@gsk.com。";
            if (Type == 1)
            {
                msg = $"您的下属{UserName}（MUDID：{UserId}）已离职，其尚未提交上传文件的HT订单{HTCount}单，现已分配给您。HT编号如下：{HTCodeList}。请<a href='{WebConfigHandler.H5BaseDomain}/P/Upload/AutoTransferState'>点击这里</a>，查看HT订单详情；请您尽快提交上传文件并获得您的直线经理审批通过，自订单确认收餐日期起7个自然日内未完成上传文件，则将暂停您的订餐服务权限。如有疑问，请联系：cn.chinarx-pmo@gsk.com。";
            }
            else if (Type == 2)
            {
                msg = $"您的下属{UserName}（MUDID：{UserId}）已离职，其上传文件审批被驳回的HT订单{HTCount}单，现已分配给您。HT编号如下：{HTCodeList}。请<a href='{WebConfigHandler.H5BaseDomain}/P/Upload/AutoTransferState'>点击这里</a>，查看HT订单详情；请您尽快提交上传文件并获得您的直线经理审批通过，自上传文件被审批驳回日期起7个自然日内未完成重新上传文件，则将暂停您的订餐服务权限。如有疑问，请联系：cn.chinarx-pmo@gsk.com。";
            }
            else
            {
                msg = $"您的下属{UserName}（MUDID：{UserId}）已离职，其上传文件待审批的HT订单{HTCount}单，现已分配给您。HT编号如下：{HTCodeList}。请<a href='{WebConfigHandler.H5BaseDomain}/P/Upload/AutoTransferState'>点击这里</a>，查看HT订单详情；请提醒您的直线经理进行审批。自上传文件日期起7个自然日内未完成直线经理审批通过，则将暂停您的订餐服务权限。如有疑问，请联系：cn.chinarx-pmo@gsk.com。";
            }
            // End UpdateBy zhexin.zou at 20190104
            var touser = string.Join("|", ListTouser);
            var listMessage = new List<string>()
            {
                msg
            };
            SendQyMsg(listMessage, touser);
        }
        #endregion

        #region 发送预申请自动转交消息
        // Start UpdateBy zhexin.zou at 20190104
        public void SendMessageForAutoTransferPre(List<string> ListTouser, string HTCodeList, string UserName, string UserId, int HTCount)
        {
            var msg = $"您的下属{UserName}（MUDID：{UserId}）已离职，其尚未完成审批通过的HT预申请{HTCount}单，现已分配给您。HT编号如下：{HTCodeList}。请在预申请状态页面中，查看HT预申请详情；请您尽快完成审批。如有疑问，请联系：cn.chinarx-pmo@gsk.com。";
            // End UpdateBy zhexin.zou at 20190104
            var touser = string.Join("|", ListTouser);
            var listMessage = new List<string>()
            {
                msg
            };
            SendQyMsg(listMessage, touser);
        }
        #endregion


        #endregion

        public void SendAddressApprovalStateChangeMessageToUser(P_AddressApproval addressApproval)
        {
            List<string> listMessage = new List<string>();
            List<string> listTouser = new List<string>();
            var applicantMsg = "";
            var approverMsg = "";
            var touser = "";
            P_AddressApproval_View _AddressApproval = preApprovalService.LoadAddressApprovalInfo(addressApproval.ID);

            switch (_AddressApproval.ApprovalStatus)
            {
                //地址申请提交成功
                case 0:
                    applicantMsg = $"{_AddressApproval.DACode} {_AddressApproval.HospitalName}，您的地址申请已提交成功，正在等待直线经理审批。";
                    listMessage.Add(applicantMsg);
                    listTouser.Add(_AddressApproval.ApplierMUDID);
                    touser = string.Join("|", listTouser);
                    SendQyMsg(listMessage, touser);
                    listMessage.Clear();
                    listTouser.Clear();
                    approverMsg = $"{_AddressApproval.DACode}，您有待审批的地址申请。请<a href='{WebConfigHandler.H5BaseDomain}/P/PreApproval/AddressApprove?id={_AddressApproval.ID}&from=0'>点击这里</a>进行审批。";
                    listMessage.Add(approverMsg);
                    listTouser.Add(_AddressApproval.LineManagerMUDID);
                    touser = string.Join("|", listTouser);
                    SendQyMsg(listMessage, touser);
                    break;
                //地址申请修改成功
                case 9:
                    applicantMsg = $"{_AddressApproval.DACode} {_AddressApproval.HospitalName}，您的地址申请修改已提交成功，正在等待直线经理审批。";
                    listMessage.Add(applicantMsg);
                    listTouser.Add(_AddressApproval.ApplierMUDID);
                    touser = string.Join("|", listTouser);
                    SendQyMsg(listMessage, touser);
                    listMessage.Clear();
                    listTouser.Clear();
                    approverMsg = $"{_AddressApproval.DACode}，您有待审批的地址申请。请<a href='{WebConfigHandler.H5BaseDomain}/P/PreApproval/AddressApprove?id={_AddressApproval.ID}&from=0'>点击这里</a>进行审批。";
                    listMessage.Add(approverMsg);
                    listTouser.Add(_AddressApproval.LineManagerMUDID);
                    touser = string.Join("|", listTouser);
                    SendQyMsg(listMessage, touser);
                    break;
                //重新提交申请
                case 10:
                    applicantMsg = $"{_AddressApproval.DACode} {_AddressApproval.HospitalName}，您的地址申请已重新提交成功，正在等待直线经理审批。";
                    listMessage.Add(applicantMsg);
                    listTouser.Add(_AddressApproval.ApplierMUDID);
                    touser = string.Join("|", listTouser);
                    SendQyMsg(listMessage, touser);
                    listMessage.Clear();
                    listTouser.Clear();
                    approverMsg = $"{_AddressApproval.DACode}，您有待审批的地址申请。请<a href='{WebConfigHandler.H5BaseDomain}/P/PreApproval/AddressApprove?id={_AddressApproval.ID}&from=0'>点击这里</a>进行审批。";
                    listMessage.Add(approverMsg);
                    listTouser.Add(_AddressApproval.LineManagerMUDID);
                    touser = string.Join("|", listTouser);
                    SendQyMsg(listMessage, touser);
                    break;
                //审批通过
                case 1:
                    applicantMsg = $"{_AddressApproval.DACode} {_AddressApproval.HospitalName}，您的地址申请已被直线经理审批通过。";
                    listMessage.Add(applicantMsg);
                    listTouser.Add(_AddressApproval.ApplierMUDID);
                    touser = string.Join("|", listTouser);
                    SendQyMsg(listMessage, touser);
                    break;
                    //修改之后审批通过
                case 5:
                    applicantMsg = $"{_AddressApproval.DACode} {_AddressApproval.HospitalName}，您的地址申请修改已被直线经理审批通过。";
                    listMessage.Add(applicantMsg);
                    listTouser.Add(_AddressApproval.ApplierMUDID);
                    touser = string.Join("|", listTouser);
                    SendQyMsg(listMessage, touser);
                    break;
                    //重新提交之后审批通过
                case 7:
                    applicantMsg = $"{_AddressApproval.DACode} {_AddressApproval.HospitalName}，您的地址申请重新提交已被直线经理审批通过。";
                    listMessage.Add(applicantMsg);
                    listTouser.Add(_AddressApproval.ApplierMUDID);
                    touser = string.Join("|", listTouser);
                    SendQyMsg(listMessage, touser);
                    break;
                //驳回
                case 2:
                    applicantMsg = $"{_AddressApproval.DACode} {_AddressApproval.HospitalName}，您的地址申请已被直线经理审批驳回。原因：{_AddressApproval.RejectReason}。如需修改地址申请，请<a href='{WebConfigHandler.H5BaseDomain}/P/PreApproval/AddressDetail?id={_AddressApproval.ID}&from=0'>点击这里</a>。如有疑问，请联系：cn.chinarx-pmo@gsk.com。";
                    listMessage.Add(applicantMsg);
                    listTouser.Add(_AddressApproval.ApplierMUDID);
                    touser = string.Join("|", listTouser);
                    SendQyMsg(listMessage, touser);
                    break;
                    //修改之后驳回
                case 6:
                    applicantMsg = $"{_AddressApproval.DACode} {_AddressApproval.HospitalName}，您的地址申请修改已被直线经理审批驳回。原因：{_AddressApproval.RejectReason}。如需再次修改地址申请，请<a href='{WebConfigHandler.H5BaseDomain}/P/PreApproval/AddressDetail?id={_AddressApproval.ID}&from=0'>点击这里</a>。如有疑问，请联系：cn.chinarx-pmo@gsk.com。";
                    listMessage.Add(applicantMsg);
                    listTouser.Add(_AddressApproval.ApplierMUDID);
                    touser = string.Join("|", listTouser);
                    SendQyMsg(listMessage, touser);
                    break;
                    //重新提交之后驳回
                case 8:
                    applicantMsg = $"{_AddressApproval.DACode} {_AddressApproval.HospitalName}，您的地址申请重新提交已被直线经理审批驳回。原因：{_AddressApproval.RejectReason}。如需再次修改地址申请，请<a href='{WebConfigHandler.H5BaseDomain}/P/PreApproval/AddressDetail?id={_AddressApproval.ID}&from=0'>点击这里</a>。如有疑问，请联系：cn.chinarx-pmo@gsk.com。";
                    listMessage.Add(applicantMsg);
                    listTouser.Add(_AddressApproval.ApplierMUDID);
                    touser = string.Join("|", listTouser);
                    SendQyMsg(listMessage, touser);
                    break;
                //取消地址申请
                case 4:
                    applicantMsg = $"{_AddressApproval.DACode} {_AddressApproval.HospitalName}，您的地址申请已取消成功。";
                    listMessage.Add(applicantMsg);
                    listTouser.Add(_AddressApproval.ApplierMUDID);
                    touser = string.Join("|", listTouser);
                    SendQyMsg(listMessage, touser);
                    listMessage.Clear();
                    listTouser.Clear();
                    break;
                
            }
        }

        public void SendSpecialMessageToUser(P_AddressApproval addressApproval)
        {
            List<string> listMessage = new List<string>();
            List<string> listTouser = new List<string>();
            var applicantMsg = "";
            var approverMsg = "";
            var touser = "";
            P_AddressApproval_View _AddressApproval = preApprovalService.LoadAddressApprovalInfo(addressApproval.ID);

            applicantMsg = $"{_AddressApproval.DACode} {_AddressApproval.HospitalName}，您的地址申请已被直线经理审批驳回。原因：该医院已被删除";
            listMessage.Add(applicantMsg);
            listTouser.Add(_AddressApproval.ApplierMUDID);
            touser = string.Join("|", listTouser);
            SendQyMsg(listMessage, touser);
            
        }

        #region 插入订单失败消息发送
        /// <summary>
        /// 插入订单失败消息发送
        /// </summary>
        /// <param name="order"></param>
        public void SendOrderErrorToUser(string HTCode, string Touser)
        {
            var msg = $"HT编号为" + HTCode + "的预申请，插入订单失败，请及时查看原因。";
            //var touser = string.Join("|", ListTouser);
            var listMessage = new List<string>()
            {
                msg
            };
            SendQyMsg(listMessage, Touser);
        }
        #endregion

        #region 发送医院覆盖变更推送
        /// <summary>
        /// 插入订单失败消息发送
        /// </summary>
        /// <param name="order"></param>
        public void SendCoverChangeToUser(string HospitalCode, string HospitalName, string Address, string Touser, int Type, string ResId, string ResName)
        {
            List<string> listMessage = new List<string>();

            if(Type == 1)
            {
                var msg = HospitalCode + $"，" + HospitalName + "，" + Address + "，原餐厅已下线，目前暂不支持送餐服务。订餐供应商正在开发新的餐厅，待开发成功会发送微信通知。如有疑问，请联系：cn.chinarx-pmo@gsk.com.";
                listMessage.Add(msg);
            }
            else if(Type == 2)
            {
                var msg = HospitalCode + $"，" + HospitalName + "，" + Address + "，已新上线餐厅，支持送餐服务。您可以在“浏览餐厅”模块查询详细餐厅信息。";
                listMessage.Add(msg);
            }
            else if(Type == 3)
            {
                var msg = HospitalCode + $"，" + HospitalName + "，已上线餐厅" + ResName + "，支持送餐服务。您可以在“浏览餐厅”模块查询详细餐厅信息。";
                listMessage.Add(msg);
            }
            else if (Type == 4)
            {
                var msg = HospitalCode + $"，" + HospitalName + "，原餐厅" + ResName + "已下线，该医院还有其他餐厅支持送餐服务。您可以在“浏览餐厅”模块查询详细餐厅信息。如有疑问，请联系：cn.chinarx-pmo@gsk.com。";
                listMessage.Add(msg);
            }
            else if (Type == 5)
            {
                var msg = HospitalCode + $"，" + HospitalName + "，原餐厅" + ResName + "已下线，目前暂不支持送餐服务。订餐供应商正在开发新的餐厅，待开发成功会发送微信通知。如有疑问，请联系：cn.chinarx-pmo@gsk.com。";
                listMessage.Add(msg);
            }

            SendQyMsg(listMessage, Touser);
        }
        #endregion

        #region 邮件发送
        public bool SendMail(string Subject, string mailTo, string Body, string FilePath)
        {
            String FROM = "cn.igsk@gsk.com";
            String FROMNAME = "Catering Service";
            String SMTP_USERNAME = "AKIAIHZIQK74DCRBRYMQ";
            String SMTP_PASSWORD = "Av567RSLSBZUtaDNe/oCeDdDGP/AaYpkSeAt7NtHPFGe";
            String HOST = "email-smtp.us-east-1.amazonaws.com";
            int PORT = 25;


            //String FROM = "cn.igsk@gsk.com";
            //String FROMNAME = "Catering Service";
            //String SMTP_USERNAME = "AKIAIHZIQK74DCRBRYMQ";
            //String SMTP_PASSWORD = "Av567RSLSBZUtaDNe/oCeDdDGP/AaYpkSeAt7NtHPFGe";
            //String HOST = "email-smtp.us-east-1.amazonaws.com";
            //int PORT = 587;
            String SUBJECT = Subject;
            String BODY = Body;

            MailMessage message = new MailMessage();
            message.IsBodyHtml = true;
            message.From = new MailAddress(FROM, FROMNAME);
            //收件人
            string[] mailTos = mailTo.Split(';');
            foreach (string To in mailTos)
            {
                if (!string.IsNullOrEmpty(To))
                    message.To.Add(new MailAddress(To));
            }
            message.Subject = SUBJECT;
            message.Body = BODY;

            if (FilePath != "" && FilePath != null)
            {
                message.Attachments.Add(new Attachment(FilePath));
                //message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
            }

            using (var client = new System.Net.Mail.SmtpClient(HOST, PORT))
            {
                // Pass SMTP credentials
                client.Credentials =
                    new NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);

                // Enable SSL encryption
                client.EnableSsl = true;

                try
                {
                    client.Send(message);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public string ReplaceText(string path, string S3Paht)
        {

            //path = HttpContext.Current.Server.MapPath("EmailTemplate\\emailTemplate.html");  

            //if (path == string.Empty)  
            //{  
            //    return string.Empty;  
            //}  
            System.IO.StreamReader sr = new System.IO.StreamReader(path);
            string str = string.Empty;
            str = sr.ReadToEnd();
            str = str.Replace("$MailContent$", S3Paht);

            return str;
        }


        public void SendPreMailToVander(string HTCode, string MeetingDate, string HospitalName, string Budget, string Text)
        {
            var Month = DateTime.Now.Month;

            string mailTo = "";

            if (Month % 2 == 0)
            {
                mailTo = System.Configuration.ConfigurationManager.AppSettings["MailToUserBDS"].ToString();
            }
            else
            {
                mailTo = System.Configuration.ConfigurationManager.AppSettings["MailToUserXMS"].ToString();
            }

            string mailSubject = "超大预申请：" + HTCode + "，" + MeetingDate + "，" + HospitalName + "，" + Budget + "元";

            string mailBody = "大家好，<br/><br/>" +
                              "超大预申请信息请查收，请按流程跟进用户的订餐事宜。<br/><br/>" +
                              "预申请信息：" + Text +
                              "<br/><br/>" +
                              "这是由系统自动发送的，请不要回复。<br/><br/>";


            LogHelper.Info("MailTo:" + mailTo);
            LogHelper.Info("MailSubject:" + mailSubject);
            LogHelper.Info("MailBody:" + mailBody);

            SendMail(mailSubject, mailTo, mailBody, "");
        }


        public void SendTSMailToVander(string HTCode, string MeetingDate, string HospitalName, string Type, string Text, string Supplier)
        {
            //var Month = DateTime.Now.Month;

            string mailTo = "";

            if (Supplier == "xms")
            {
                mailTo = System.Configuration.ConfigurationManager.AppSettings["MailToUserXMS"].ToString();
            }
            else
            {
                mailTo = System.Configuration.ConfigurationManager.AppSettings["MailToUserBDS"].ToString();
            }

            string mailSubject = "严重投诉：" + HTCode + "，" + MeetingDate + "，" + HospitalName + "，" + Type + "";

            string mailBody = "大家好，<br/><br/>" +
                              "严重投诉订单请查收，请按流程处理用户的投诉。<br/><br/>" +
                              "订单信息：" + Text +
                              "<br/><br/>" +
                              "这是由系统自动发送的，请不要回复。<br/><br/>";

            LogHelper.Info("MailTo:" + mailTo);
            LogHelper.Info("MailSubject:" + mailSubject);
            LogHelper.Info("MailBody:" + mailBody);

            SendMail(mailSubject, mailTo, mailBody, "");
            
        }
        #endregion

        #region 插入订单失败消息发送
        /// <summary>
        /// 插入订单失败消息发送
        /// </summary>
        /// <param name="order"></param>
        public void SendOrderErrorMsgToUser(Guid id, string HTCode, string Touser, string remark, int type)
        {
            string msg = "";
            if (type == 0)
            {
                msg = $"订单ID：" + id.ToString() + "，HT编号：" + HTCode + "的预申请，更新XmsOrderID失败，XmsOrderID：" + remark + "，请及时将orderid更新到数据库";
            }
            //xms接口返回失败，删除订单失败
            if (type == 1)
            {
                msg = $"订单ID为" + id.ToString() + "HT编号为" + HTCode + "的预申请，删除订单失败";
            }
            if(type == 2)
            {
                msg = $"订单ID为" + id.ToString() + "HT编号为" + HTCode + "的预申请，代表改单时，将订单信息还原为原始订单信息时失败。";
            }
            if (type == 3)
            {
                msg = $"订单ID为" + id.ToString() + "HT编号为" + HTCode + "的预申请，代表改单失败。";
            }
            if(type == 4)
            {
                msg = $"订单ID为" + id.ToString() + "HT编号为" + HTCode + "的预申请改单时写入XmsOrderID失败，XmsOrderID为" + remark + "请及时将orderid更新到数据库";
            }
            if (type == 5)
            {
                msg = $"HT编号为" + HTCode + "的预申请,插入订单失败，请及时关注";
            }
            if (type == 6)
            {
                msg = $"HT编号为" + HTCode + "的预申请,由于网络中断，下单失败，订单已删除成功，HT占用已取消，请及时关注";
            }
            if (type == 7)
            {
                msg = $"HT编号为" + HTCode + "的预申请,由于网络中断，修改订单失败，订单已还原为原始订单数据，请及时关注";
            }
            var listMessage = new List<string>()
            {
                msg
            };
            SendQyMsg(listMessage, Touser);
        }
        #endregion
    }

    class WebConfigHandler
    {
        public static string H5BaseDomain
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["H5BaseDomain"];
            }
        }

        public static string H5Domain
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["H5Domain"];
            }
        }

        public static string H5BaseUrl
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["H5BaseUrl"];
            }
        }    
    }

}