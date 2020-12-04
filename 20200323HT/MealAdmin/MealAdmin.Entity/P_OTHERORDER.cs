using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity
{
    public class P_OTHERORDER
    {
        public Guid ID { get; set; }
        public Guid ChangeID { get; set; }
        public string UserId { get; set; }
        public string Market { get; set; }
        public string HospitalId { get; set; }
        public string Address { get; set; }
        public string CN { get; set; }
        public string RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public string RestaurantLogo { get; set; }
        public decimal TotalPrice { get; set; }
        public string DeliveryGeo { get; set; }
        public string Detail { get; set; }
        public string ChangeDetail { get; set; }
        public int FoodCount { get; set; }
        public int AttendCount { get; set; }
        public string DeliveryAddress { get; set; }
        public string Consignee { get; set; }
        public string Phone { get; set; }
        public DateTime DeliverTime { get; set; }
        public string Remark { get; set; }
        public string MMCoEImage { get; set; }
        public int State { get; set; }
        public int ReceiveState { get; set; }
        public string ReceiveCode { get; set; }
        public DateTime CreateDate { get; set; }
        public string XmsOrderId { get; set; }
        public string OldXmlOrderId { get; set; }
        public DateTime SendOrderDate { get; set; }
        public DateTime ChangeOrderDate { get; set; }
        public DateTime ReturnOrderDate { get; set; }
        public decimal XmsTotalPrice { get; set; }
        public string ChangeTotalPriceReason { get; set; }
        public DateTime ReceiveDate { get; set; }
        public int IsRetuen { get; set; }
        public int IsChange { get; set; }
        public string XmsOrderReason { get; set; }
        public int IsOuterMeeting { get; set; }
        public string RestaurantTel { get; set; }
        public string RestaurantAddress { get; set; }
        public string ApproveReason { get; set; }
        public int MMCoEApproveState { get; set; }
        public string MMCoEReason { get; set; }
        public int IsReturn { get; set; }
        public int IsDelivery { get; set; }
        public string PO { get; set; }
        public string WBS { get; set; }
        public int IsNonHT { get; set; }
        public string Channel { get; set; }
        public string MeetingCode { get; set; }
        public string MeetingName { get; set; }
        public string TA { get; set; }
        public DateTime ReceiveTime { get; set; }
        public string EnterpriseOrderId { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string HospitalName { get; set; }
    }
}
