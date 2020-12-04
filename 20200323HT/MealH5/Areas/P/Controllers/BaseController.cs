using MealAdmin.Entity;
using MealAdmin.Entity.Helper;
using MealH5.Models;
using MealH5.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MealH5.Areas.P.Controllers
{
    public class BaseController : Controller
    {
        #region 标记当前是否是修改订单流程
        /// <summary>
        /// 标记当前是否是修改订单流程
        /// </summary>
        public bool IsNewOrder
        {
            get
            {
                return Session[ConstantHelper.CHANGEORDERID] == null;
            }
        }
        #endregion

        #region 当前执行改单操作的订单ID
        /// <summary>
        /// 当前执行改单操作的订单ID
        /// </summary>
        public Guid? ChangeOrderID
        {
            get
            {
                return (Guid?)Session[ConstantHelper.CHANGEORDERID];
            }
            set
            {
                Session[ConstantHelper.CHANGEORDERID] = value;
            }
        }
        #endregion

        #region 正在操作的订单
        /// <summary>
        /// 正在操作的订单
        /// </summary>
        public P_Order OrderInfo
        {
            get
            {
                return Session[ConstantHelper.CURRENTPORDER] as P_Order;
            }
            set
            {
                Session[ConstantHelper.CURRENTPORDER] = value;
            }
        }
        #endregion

        #region 正在操作订单的医院部分数据
        /// <summary>
        /// 正在操作订单的医院部分数据
        /// </summary>
        public P_ChooseHospital HospitalInfo
        {
            get
            {
                var order = OrderInfo;
                if (order == null)
                {
                    return null;
                }
                else
                {
                    return order.hospital;
                }
            }
            set
            {
                var order = OrderInfo;
                if (order == null)
                {
                    order = new P_Order()
                    {
                        inTime = DateTime.Now.ToString("HH:mm:ss")
                    };
                    OrderInfo = order;
                }
                OrderInfo.hospital = value;
            }
        }
        #endregion

        #region 当前登陆的微信用户
        /// <summary>
        /// 当前登陆的微信用户
        /// </summary>
        public P_USERINFO CurrentWxUser
        {
            get
            {
                return Session[ConstantHelper.CURRENTWXUSER] as P_USERINFO;
            }
            set
            {
                Session[ConstantHelper.CURRENTWXUSER] = value;
            }
        }
        #endregion

        #region 正在操作的预申请
        /// <summary>
        /// 正在操作的订单
        /// </summary>
        public P_PreApproval PreApproval
        {
            //get
            //{
            //    return Session[ConstantHelper.CURRENTPPREAPPROVAL] as P_PreApproval;
            //}
            //set
            //{
            //    Session[ConstantHelper.CURRENTPPREAPPROVAL] = value;
            //}
            get
            {
                var order = WeChatOrderInfo;
                if (order == null)
                {
                    return null;
                }
                else
                {
                    return order.preApproval;
                }
            }
            set
            {
                var order = WeChatOrderInfo;
                if (order == null)
                {
                    order = new P_WeChatOrder()
                    {
                        inTime = DateTime.Now.ToString("HH:mm:ss")
                    };
                    WeChatOrderInfo = order;
                }
                WeChatOrderInfo.preApproval = value;
            }
        }
        #endregion

        #region 正在操作的预申请
        /// <summary>
        /// 正在操作的订单
        /// </summary>
        public P_WeChatOrder WeChatOrderInfo
        {
            get
            {
                return Session[ConstantHelper.CURRENTPWECHATORDER] as P_WeChatOrder;
            }
            set
            {
                Session[ConstantHelper.CURRENTPWECHATORDER] = value;
            }
        }
        #endregion

        #region 正在操作的上传文件
        /// <summary>
        /// 正在操作的订单
        /// </summary>
        public P_PREUPLOADORDER orderUpload
        {
            get
            {
                var order = WeChatOrderInfos;
                if (order == null)
                {
                    return null;
                }
                else
                {
                    return order.orderInfo;
                }
            }
            set
            {
                var order = WeChatOrderInfos;
                if (order == null)
                {
                    order = new P_WeChatOrder()
                    {
                        inTime = DateTime.Now.ToString("HH:mm:ss")
                    };
                    WeChatOrderInfos = order;
                }
                WeChatOrderInfos.orderInfo = value;
            }
        }
        #endregion

        #region 正在操作的上传文件
        /// <summary>
        /// 正在操作的订单
        /// </summary>
        public P_WeChatOrder WeChatOrderInfos
        {
            get
            {
                return Session[ConstantHelper.CURRENTPWECHATORDER] as P_WeChatOrder;
            }
            set
            {
                Session[ConstantHelper.CURRENTPWECHATORDER] = value;
            }
        }
        #endregion
    }
}