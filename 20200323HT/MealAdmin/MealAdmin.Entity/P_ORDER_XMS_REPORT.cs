using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity
{
    public class P_ORDER_XMS_REPORT
    {
        public string XmsOrderId { get; set; }
        public decimal totalFee { get; set; }
        public string customerPickup { get; set; }
        public string cancelFeedback { get; set; }
        public string cancelFailReason { get; set; }
        public string feeModifyReason { get; set; }
        public string bookState { get; set; }
        public string cancelState { get; set; }
        public string TYYYDRDC { get; set; }
        public string TYDBDRDC { get; set; }
        public string TYDBTYYYDRDC { get; set; }
        public string CHRSDYLS { get; set; }
        public string CHRSXYLSDDFSDYLS { get; set; }
        public string TYCTDRDC { get; set; }
        public string TYDBTYCTDRDC { get; set; }
        public string TYDBTYYYTYCTDRDC { get; set; }
    }
}
