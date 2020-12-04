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
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“WMessage”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 WMessage.svc 或 WMessage.svc.cs，然后开始调试。
    public class WMessage : IWMessage
    {
       
        WMessageHandler wMessageHandler = WMessageHandler.GetInstance();

        public QyApiHandler wApiHandler = Global.applicationContext.GetBean("wApiHandler") as QyApiHandler;

        public IOrderService orderService = Global.applicationContext.GetBean("orderService") as IOrderService;

        #region 发送订单状态变更消息
        /// <summary>
        /// 发送订单状态变更消息
        /// </summary>
        /// <param name="ID"></param>
        public void SendWxMessageByOrder(Guid ID)
        {
            var order = orderService.FindByID(ID);
            wMessageHandler.SendMessageToUser(order.UserId, order);
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
            var res = wApiHandler.Message_Send(new XFramework.WeChatAPI.Entity.MessageSend()
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
            wMessageHandler.SendMessageForChangeFee(order);
        }
        #endregion
    }
}

