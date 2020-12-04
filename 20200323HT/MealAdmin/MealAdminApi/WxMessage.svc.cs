using MealAdmin.Entity;
using MealAdmin.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using XFramework.WeChatAPI.SessionHandlers;
using XFramework.XUtil;

namespace MealAdminApi
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“WxMessage”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 WxMessage.svc 或 WxMessage.svc.cs，然后开始调试。
    public class WxMessage : IWxMessage
    {
        WxMessageHandler wxMessageHandler = WxMessageHandler.GetInstance();
        public QyApiHandler qyApiHandler = Global.applicationContext.GetBean("qyApiHandler") as QyApiHandler;

        public IOrderService orderService = Global.applicationContext.GetBean("orderService") as IOrderService;

        #region 发送订单状态变更消息
        /// <summary>
        /// 发送订单状态变更消息
        /// </summary>
        /// <param name="ID"></param>
        public void SendWxMessageByOrder(Guid ID)
        {
            var order = orderService.FindByID(ID);
            wxMessageHandler.SendMessageToUser(order.UserId, order);
        }
        #endregion

        #region 发送微信企业号文字消息
        /// <summary>
        /// 发送微信企业号文字消息
        /// </summary>
        /// <param name="toUsers"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public string SendText(string toUsers, string text)
        {
            var messageId = DateTime.Now.Ticks;
            LogHelper.Info($"[{messageId}]发送:{toUsers}|{text}");
            var res = qyApiHandler.Message_Send(new XFramework.WeChatAPI.Entity.MessageSend()
            {
                msgtype = "text",
                touser = toUsers,
                content = text

            });
            LogHelper.Info($"[{messageId}]结果:{res.errcode} -> {res.errmsg}");


            return JsonConvert.SerializeObject(res);
        }
        #endregion

        #region 发出订单金额修改消息
        /// <summary>
        /// 发出订单金额修改消息
        /// </summary>
        /// <param name="xmsOrderId"></param>
        public void SendMessageToUserForChangeFee(string xmsOrderId)
        {
            var order = orderService.FindByXmlOrderId(xmsOrderId);
            wxMessageHandler.SendMessageForChangeFee(order);
        }
        #endregion

        public void SendPreApprovalStateChangeMessageToUser(P_PreApproval preApproval)
        {
            wxMessageHandler.SendPreApprovalStateChangeMessageToUser(preApproval);
        }

        public void SendApproveStepErrorMessageToGroup(P_PreApproval preApproval)
        {
            wxMessageHandler.SendApproveStepErrorMessageToGroup(preApproval);
        }


        public void SendPreApprovalRejectMessageToUser(string Comments, P_PreApproval preApproval)
        {
            wxMessageHandler.SendPreApprovalRejectMessageToUser(Comments, preApproval);
        }

        public void SendOrderStateChangeMessageToUser(P_PREUPLOADORDER order)
        {
            wxMessageHandler.SendOrderStateChangeMessageToUser(order);
        }

        public void SendOrderRejectMessageToUser(string Comments, P_PREUPLOADORDER order)
        {
            wxMessageHandler.SendOrderRejectMessageToUser(Comments, order);
        }

        public void SendOrderErrorToUser(string HTCode, string Touser)
        {
            wxMessageHandler.SendOrderErrorToUser(HTCode, Touser);
        }

        public void SendCoverChangeToUser(string HospitalCode, string HospitalName, string Address, string Touser, int Type, string ResId, string ResName)
        {
            wxMessageHandler.SendCoverChangeToUser(HospitalCode, HospitalName, Address, Touser, Type, ResId, ResName);
        }

        public void SendOrderErrorMsgToUser(Guid id, string HTCode, string Touser, string remark, int type)
        {
            wxMessageHandler.SendOrderErrorMsgToUser(id, HTCode, Touser, remark, type);
        }

        #region *******************任务计划消息推送**********************

        #region 发送确认收餐消息
        /// <summary>
        /// 发送确认收餐消息
        /// </summary>
        /// <param name="order"></param>
        public void SendMessageForConfirm(List<string> listTouser, int state)
        {
            wxMessageHandler.SendMessageForConfirm(listTouser, state);
        }
        #endregion

        // Start UpdateBy zhexin.zou at 20190104
        public void SendMessageForAutoTransfer(List<string> ListTouser, string HTCodeList, string UserName, string UserId, int HTCount, int Type)
        {
            wxMessageHandler.SendMessageForAutoTransfer(ListTouser, HTCodeList, UserName, UserId, HTCount, Type);
        }
        // End UpdateBy zhexin.zou at 20190104

        public void SendMessageForAutoTransferPre(List<string> ListTouser, string HTCodeList, string UserName, string UserId, int HTCount)
        {
            wxMessageHandler.SendMessageForAutoTransferPre(ListTouser, HTCodeList, UserName, UserId, HTCount);
        }

        public void SendMessageToUser(string userId, P_ORDER order)
        {
            wxMessageHandler.SendMessageToUser(userId, order);
        }

        #endregion

        #region 新增地址
        public void SendAddressApprovalStateChangeMessageToUser(P_AddressApproval addressApproval)
        {
            wxMessageHandler.SendAddressApprovalStateChangeMessageToUser(addressApproval);
        }

        public void SendSpecialMessageToUser(P_AddressApproval addressApproval)
        {
            wxMessageHandler.SendSpecialMessageToUser(addressApproval);
        }
        #endregion
    }
}
