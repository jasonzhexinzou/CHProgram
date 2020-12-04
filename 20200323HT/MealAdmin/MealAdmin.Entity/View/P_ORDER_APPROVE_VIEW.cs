using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity.View
{
    public class P_ORDER_APPROVE_VIEW
    {
        public Guid ID { get; set; }
        public string DCUserName { get; set; }
        public string DCPhoneNum { get; set; }
        public string MUDID { get; set; }
        public string Market { get; set; }
        public string CN { get; set; }
        public string PO { get; set; }
        public string HospitalName { get; set; }
        public string SCUserName { get; set; }
        public string SCPhoneNum { get; set; }
        public decimal? TotalPrice { get; set; }
        public DateTime SendTime { get; set; }
        public int OrderState { get; set; }
        public int MMCoEApproveState { get; set; }
    }
}
/*

SELECT     P_ORDER.ID, P_ORDER.Market, P_ORDER.CN, WP_QYUSER.Name AS DCUserName, WP_QYUSER.PhoneNumber AS DCPhoneNum, P_ORDER.UserId AS MUDID, 
                      P_HOSPITAL.Name AS HospitalName, P_ORDER.Consignee AS SCUserName, P_ORDER.Phone AS SCPhoneNum, P_ORDER.TotalPrice, P_ORDER.DeliverTime AS SendTime, 
                      P_ORDER.State AS OrderState
FROM         P_ORDER LEFT OUTER JOIN
                      WP_QYUSER ON P_ORDER.UserId = WP_QYUSER.UserId LEFT OUTER JOIN
                      P_HOSPITAL ON P_ORDER.HospitalId = P_HOSPITAL.ID

WHERE     (P_ORDER.CN LIKE '%' + @CN + '%') AND (P_ORDER.UserId LIKE '%' + @MUDID + '%') AND ((@DeliverTimeBegin IS NULL) OR (P_ORDER.DeliverTime >= @DeliverTimeBegin)) 
                       AND ((@DeliverTimeEnd IS NULL) OR (P_ORDER.DeliverTime < @DeliverTimeEnd))
*/
