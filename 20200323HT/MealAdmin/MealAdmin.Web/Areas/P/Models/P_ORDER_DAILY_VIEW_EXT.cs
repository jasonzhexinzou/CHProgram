using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MealAdmin.Web.Areas.P.Models
{
    public class P_ORDER_DAILY_VIEW_EXT
    {
        public Guid ID { get; set; }
        public string DCUserName { get; set; }
        public string DCPhoneNum { get; set; }
        public string MUDID { get; set; }
        public string Market { get; set; }
        public string CN { get; set; }
        public string BudgetTotal { get; set; }
        public string PO { get; set; }
        public string WBS { get; set; }
        public string TA { get; set; }
        public string MeetCode { get; set; }
        public string MeetName { get; set; }
        public string XMSOrderID { get; set; }
        public string OrderingDate { get; set; }
        public string OrderingTime { get; set; }
        public string SendDate { get; set; }
        public string SendTime { get; set; }
        public string RestaurantId { get; set; }
        public string RestName { get; set; }
        public string UserQuantity { get; set; }
        public string MealQuantity { get; set; }
        public string TotalPrice { get; set; }
        public string XMSTotalPrice { get; set; }
        public string ChangePriceReason { get; set; }
        public string TYYYDRDC { get; set; }
        public string TYDBDRDC { get; set; }
        public string TYDBTYYYDRDC { get; set; }
        public string CHRSDYLS { get; set; }
        public string CHRSXYLSDDFSDYLS { get; set; }
        public string TYCTDRDC { get; set; }
        public string TYDBTYCTDRDC { get; set; }
        public string TYDBTYYYTYCTDRDC { get; set; }
        public string CustomerPickup { get; set; }
        public string ProvinceName { get; set; }
        public string CityName { get; set; }
        public string GskHospital { get; set; }
        public string HospitalName { get; set; }
        public string HospitalAddr { get; set; }
        public string HospitalRoom { get; set; }
        public string SCUserName { get; set; }
        public string SCPhoneNum { get; set; }
        public string Remark { get; set; }
        public string XMSBookState { get; set; }
        public string ReceiveState { get; set; }
        public string EOnTime { get; set; }
        public string EOnTimeDesc { get; set; }
        public string EIsSafe { get; set; }
        public string EIsSafeDesc { get; set; }
        public string EHealth { get; set; }
        public string EHealthDesc { get; set; }
        public string EPack { get; set; }
        public string EPackDesc { get; set; }
        public string ECost { get; set; }
        public string ECostDesc { get; set; }
        public string EOtherDesc { get; set; }
        public string EStar { get; set; }
        public string ECreateDate { get; set; }
        public string ECreateTime { get; set; }
        public string IsReturn { get; set; }
        public string XMSCancelState { get; set; }
        public string IsPlatformLaunch { get; set; }
        public string XMSCancelFeedback { get; set; }
        public string XMSOrderCancelReason { get; set; }
        public string OrderState { get; set; }
        public string ReceiveDate { get; set; }
        public string ReceiveTime { get; set; }
        public string Channel { get; set; }
        public string EUnTimeDesc { get; set; }
    }
}