using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity.View
{
    public class P_ORDER_DAILY_VIEW
    {
        public Guid ID { get; set; }

        public string DCUserName { get; set; }
        public string DCPhoneNum { get; set; }
        public string DCUserName1 { get; set; }
        public string DCPhoneNum1 { get; set; }
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
        public DateTime? OrderingTime { get; set; }
        public DateTime? SendTime { get; set; }
        public string RestaurantId { get; set; }
        public string RestName { get; set; }
        public int UserQuantity { get; set; }
        public int MealQuantity { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? XMSTotalPrice { get; set; }
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
        public int OrderState { get; set; }
        public int ReceiveState { get; set; }
        public int EOnTime { get; set; }
        public string EOnTimeDesc { get; set; }
        public int EIsSafe { get; set; }
        public string EIsSafeDesc { get; set; }
        public int EHealth { get; set; }
        public string EHealthDesc { get; set; }
        public int EPack { get; set; }
        public string EPackDesc { get; set; }
        public int ECost { get; set; }
        public string ECostDesc { get; set; }
        public string EOtherDesc { get; set; }
        public int EStar { get; set; }
        public DateTime? ECreateDate { get; set; }
        public int IsReturn { get; set; }
        public string XMSOrderCancelReason { get; set; }
        public string XMSCancelFeedback { get; set; }
        public string XMSCancelState { get; set; }
        public string XMSBookState { get; set; }
        public int TJIsReturn { get; set; }
        public int TJIsDelivery { get; set; }
        public DateTime? ReceiveTime { get; set; }
        public string Channel { get; set; }
    }
}
/*



SELECT     P_ORDER.ID, DCUser.Name AS DCUserName, DCUser.PhoneNumber AS DCPhoneNum, P_ORDER.UserId AS MUDID, P_ORDER.Market, P_ORDER.CN, P_ORDER.XmsOrderId AS XMSOrderID, 
                      P_ORDER.CreateDate AS OrderingTime, P_ORDER.DeliverTime AS SendTime, P_ORDER.RestaurantName AS RestName, P_ORDER.AttendCount AS UserQuantity, 1 AS MealQuantity, 
                      P_ORDER.TotalPrice, P_ORDER.XmsTotalPrice AS XMSTotalPrice, P_ORDER.ChangeTotalPriceReason AS ChangePriceReason, P_ORDER_XMS_REPORT.TYYYDRDC, 
                      P_ORDER_XMS_REPORT.TYDBDRDC, P_ORDER_XMS_REPORT.TYDBTYYYDRDC, P_ORDER_XMS_REPORT.CHRSDYLS, P_ORDER_XMS_REPORT.CHRSXYLSDDFSDYLS, 
                      P_ORDER_XMS_REPORT.customerPickup, P_PROVINCE.Name AS ProvinceName, P_CITY.Name AS CityName, P_HOSPITAL.GskHospital, P_HOSPITAL.Name AS HospitalName, 
                      P_HOSPITAL.Address AS HospitalAddr, P_ORDER.Address AS HospitalRoom, P_ORDER.Consignee AS SCUserName, P_ORDER.Phone AS SCPhoneNum, P_ORDER.State AS OrderState, 
                      P_EVALUATE.OnTime AS EOnTime, P_EVALUATE.OnTimeDiscrpion AS EOnTimeDesc, P_EVALUATE.IsSafe AS EIsSafe, P_EVALUATE.SafeDiscrpion AS EIsSafeDesc, P_EVALUATE.Health AS EHealth, 
                      P_EVALUATE.HealthDiscrpion AS EHealthDesc, P_EVALUATE.Pack AS EPack, P_EVALUATE.PackDiscrpion AS EPackDesc, P_EVALUATE.CostEffective AS ECost, 
                      P_EVALUATE.CostEffectiveDiscrpion AS ECostDesc, P_EVALUATE.OtherDiscrpion AS EOtherDesc, P_EVALUATE.Star AS EStar, P_EVALUATE.CreateDate AS ECreateDate, 
                      P_ORDER.IsRetuen AS IsReturn, P_ORDER.XmsOrderReason AS XMSOrderCancelReason
FROM         P_ORDER LEFT OUTER JOIN
                      WP_QYUSER AS DCUser ON P_ORDER.UserId = DCUser.UserId LEFT OUTER JOIN
                      P_HOSPITAL ON P_ORDER.HospitalId = P_HOSPITAL.ID LEFT OUTER JOIN
                      P_CITY ON P_HOSPITAL.CityId = P_CITY.ID LEFT OUTER JOIN
                      P_PROVINCE ON P_CITY.ProvinceId = P_PROVINCE.ID LEFT OUTER JOIN
                      P_ORDER_XMS_REPORT ON P_ORDER.XmsOrderId = P_ORDER_XMS_REPORT.XmsOrderId LEFT OUTER JOIN
                      P_EVALUATE ON P_ORDER.ID = P_EVALUATE.OrderID
WHERE     (P_ORDER.CN LIKE '%' + @CN + '%') AND (P_ORDER.UserId LIKE '%' + @MUDID + '%') AND ((@DeliverTimeBegin IS NULL) OR (P_ORDER.DeliverTime >= @DeliverTimeBegin)) 
                       AND ((@DeliverTimeEnd IS NULL) OR (P_ORDER.DeliverTime < @DeliverTimeEnd))



 */
