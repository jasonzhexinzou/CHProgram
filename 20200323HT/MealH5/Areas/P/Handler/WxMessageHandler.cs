using MealAdmin.Entity;
using MealAdmin.Entity.Enum;
using MealAdmin.Entity.Helper;
using MealAdminApiClient;
using MealH5.Handler;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XFramework.WeChatAPI.Entity;
using XFramework.WeChatAPI.SessionHandlers;

namespace MealH5.Areas.P.Handler
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

        #region 发出订单状态消息
        /// <summary>
        /// 发出订单状态消息
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="order"></param>
        public void SendMessageToUser(string userid, P_ORDER order)
        {
            var channel = WxMessageClientChannelFactory.GetChannel();
            channel.SendWxMessageByOrder(order.ID);
        }
        #endregion

        #region 发出订单金额修改消息
        /// <summary>
        /// 发出订单金额修改消息
        /// </summary>
        /// <param name="xmsOrderid"></param>
        public void SendMessageToUserForChangeFee(string xmsOrderid)
        {
            var channel = WxMessageClientChannelFactory.GetChannel();
            channel.SendMessageToUserForChangeFee(xmsOrderid);
        }
        #endregion

        #region 发出订单金额修改消息
        /// <summary>
        /// 发出订单金额修改消息
        /// </summary>
        /// <param name="xmsOrderid"></param>
        public void SendNonHTMessageToUserForChangeFee(string xmsOrderid)
        {
            var channel = WxMessageClientChannelFactory.GetChannel();
            channel.SendMessageToUserForChangeFee(xmsOrderid);
        }
        #endregion

        public void SendPreApprovalStateChangeMessageToUser(P_PreApproval preApproval)
        {
            var channel = WxMessageClientChannelFactory.GetChannel();
            channel.SendPreApprovalStateChangeMessageToUser(preApproval);
        }

        public void SendApproveStepErrorMessageToGroup(P_PreApproval preApproval)
        {
            var channel = WxMessageClientChannelFactory.GetChannel();
            channel.SendApproveStepErrorMessageToGroup(preApproval);
        }

        public void SendPreApprovalRejectMessageToUser(string Comments, P_PreApproval preApproval)
        {
            var channel = WxMessageClientChannelFactory.GetChannel();
            channel.SendPreApprovalRejectMessageToUser(Comments, preApproval);
        }

        //文件上传操作部分
        public void SendOrderStateChangeMessageToUser(P_PREUPLOADORDER order)
        {
            var channel = WxMessageClientChannelFactory.GetChannel();
            channel.SendOrderStateChangeMessageToUser(order);
        }

        public void SendOrderRejectMessageToUser(string Comments, P_PREUPLOADORDER order)
        {
            var channel = WxMessageClientChannelFactory.GetChannel();
            channel.SendOrderRejectMessageToUser(Comments, order);
        }

        public void SendAddressApprovalStateChangeMessageToUser(P_AddressApproval addressApproval)
        {
            var channel = WxMessageClientChannelFactory.GetChannel();
            channel.SendAddressApprovalStateChangeMessageToUser(addressApproval);
        }

        public void SendSpecialMessageToUser(P_AddressApproval addressApproval)
        {
            var channel = WxMessageClientChannelFactory.GetChannel();
            channel.SendSpecialMessageToUser(addressApproval);
        }

        public void SendOrderErrorToUser(string HTCode, string Touser)
        {
            var channel = WxMessageClientChannelFactory.GetChannel();
            channel.SendOrderErrorToUser(HTCode, Touser);
        }

        public void SendCoverChangeToUser(string HospitalCode, string HospitalName, string Address, string Touser, int Type, string ResId, string ResName)
        {
            var channel = WxMessageClientChannelFactory.GetChannel();
            channel.SendCoverChangeToUser(HospitalCode, HospitalName, Address, Touser, Type, ResId, ResName);
        }

        public void SendOrderErrorMsgToUser(Guid id, string HTCode, string Touser, string remark, int type)
        {
            var channel = WxMessageClientChannelFactory.GetChannel();
            channel.SendOrderErrorMsgToUser(id, HTCode, Touser, remark, type);
        }

    }
}