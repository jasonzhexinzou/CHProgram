using MealAdmin.Entity;
using MealAdmin.Entity.Enum;
using MealAdmin.Entity.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MealAdmin.Service.Helper
{
    public class SessionOrderInfoToP_ORDER
    {
        #region 把内存订单数据转换成数据库格式
        /// <summary>
        /// 把内存订单数据转换成数据库格式
        /// </summary>
        /// <param name="_orderInfo"></param>
        /// <returns></returns>
        public static P_ORDER ConvterIt(P_Order _orderInfo, P_ORDER oldOrder = null)
        {
            var order =  new P_ORDER()
            {
                Market = _orderInfo.hospital.market,
                HospitalId = _orderInfo.hospital.hospital,
                Province=_orderInfo.hospital.provinceName,
                City=_orderInfo.hospital.cityName,
                HospitalName=_orderInfo.hospital.hospitalName,
                Address = _orderInfo.hospital.address,
                CN = _orderInfo.CnCode,
                RestaurantId = _orderInfo.foods.resId,
                RestaurantName = _orderInfo.foods.resName,
                RestaurantLogo = _orderInfo.foods.resLogo,
                TotalPrice = _orderInfo.foods.allPrice,
                DeliveryGeo = $"{_orderInfo.hospital.latitude},{_orderInfo.hospital.longitude}",
                Detail = JsonConvert.SerializeObject(_orderInfo),
                FoodCount = _orderInfo.foods.foods.Sum(a => Convert.ToInt32(a.count)),
                AttendCount = _orderInfo.details.attendCount,
                DeliveryAddress = _orderInfo.details.deliveryAddress,
                Consignee = _orderInfo.details.consignee,
                Phone = _orderInfo.details.phone,
                DeliverTime = _orderInfo.details.deliverTime.Value,
                Remark = _orderInfo.details.remark,
                MMCoEImage = _orderInfo.mmCoE,
                State = string.IsNullOrEmpty(_orderInfo.mmCoE) ? OrderState.SUBMITTED : OrderState.WAITAPPROVE,
                ReceiveCode = string.Empty,
                XmsTotalPrice = -1,
                IsOuterMeeting = _orderInfo.hospital.isExternal,
                RestaurantTel = _orderInfo.foods.resTel,
                RestaurantAddress = _orderInfo.foods.resAddress,
                MMCoEApproveState = string.IsNullOrEmpty(_orderInfo.mmCoE) ? MMCoEApproveState.NO : MMCoEApproveState.WAITAPPROVE,
                PO= _orderInfo.PO,
                WBS= _orderInfo.WBS,
                MeetingCode=_orderInfo.MeetingCode,
                MeetingName=_orderInfo.MeetingName,
                TA=_orderInfo.hospital.ta,
                IsNonHT=_orderInfo.IsNonHT,
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