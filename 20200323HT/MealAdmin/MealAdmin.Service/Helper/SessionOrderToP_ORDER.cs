using MealAdmin.Entity;
using MealAdmin.Entity.Enum;
using MealAdmin.Entity.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Service.Helper
{
    public class SessionOrderToP_ORDER
    {
        #region 把内存订单数据转换成数据库格式
        /// <summary>
        /// 把内存订单数据转换成数据库格式
        /// </summary>
        /// <param name="_orderInfo"></param>
        /// <returns></returns>
        public static P_ORDER ConvterIt(P_WeChatOrder _orderInfo, P_ORDER oldOrder = null)
        {
            var order = new P_ORDER()
            {
                Market = _orderInfo.preApproval.Market,
                HospitalId = _orderInfo.preApproval.HospitalCode,
                Province = _orderInfo.preApproval.Province,
                City = _orderInfo.preApproval.City,
                HospitalName = _orderInfo.preApproval.HospitalName,
                Address = _orderInfo.preApproval.HospitalAddress,
                CN = _orderInfo.preApproval.HTCode,
                RestaurantId = _orderInfo.foods.resId,
                RestaurantName = _orderInfo.foods.resName,
                RestaurantLogo = _orderInfo.foods.resLogo,
                TotalPrice = _orderInfo.foods.allPrice,
                DeliveryGeo = $"{_orderInfo.hospital.Latitude},{_orderInfo.hospital.Longitude}",
                Detail = JsonConvert.SerializeObject(_orderInfo),
                FoodCount = _orderInfo.foods.foods.Sum(a => Convert.ToInt32(a.count)),
                AttendCount = _orderInfo.details.attendCount,
                DeliveryAddress = _orderInfo.details.deliveryAddress,
                Consignee = _orderInfo.details.consignee,
                Phone = _orderInfo.details.phone,
                DeliverTime = _orderInfo.details.deliverTime.Value,
                Remark = _orderInfo.details.remark,
                MMCoEImage = _orderInfo.preApproval.MMCoEImage,
                State = OrderState.SUBMITTED,
                ReceiveCode = string.Empty,
                XmsTotalPrice = -1,
                IsOuterMeeting = _orderInfo.hospital.External,
                RestaurantTel = _orderInfo.foods.resTel,
                RestaurantAddress = _orderInfo.foods.resAddress,
                MMCoEApproveState = string.IsNullOrEmpty(_orderInfo.preApproval.MMCoEImage) ? MMCoEApproveState.NO : MMCoEApproveState.WAITAPPROVE,
                MeetingCode = _orderInfo.preApproval.HTCode,
                MeetingName = _orderInfo.preApproval.MeetingName,
                TA = _orderInfo.preApproval.TA,
                IsNonHT = 0,
            };

            if (oldOrder == null)
            {
                return order;
            }
            else
            {
                oldOrder.RestaurantId = order.RestaurantId;
                oldOrder.RestaurantName = order.RestaurantName;
                oldOrder.RestaurantLogo = order.RestaurantLogo;
                oldOrder.TotalPrice = order.TotalPrice;
                oldOrder.FoodCount = order.FoodCount;
                oldOrder.AttendCount = order.AttendCount;
                oldOrder.DeliveryAddress = order.DeliveryAddress;
                oldOrder.Consignee = order.Consignee;
                oldOrder.Phone = order.Phone;
                oldOrder.DeliverTime = order.DeliverTime;
                oldOrder.Remark = order.Remark;
                oldOrder.MMCoEImage = order.MMCoEImage;
                oldOrder.RestaurantTel = order.RestaurantTel;
                oldOrder.RestaurantAddress = order.RestaurantAddress;

                return oldOrder;
            }
        }
        #endregion
    }
}
