using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingMealEntity
{
    public class GetReportResponse : ResponseBase
    {
        public GetReportResult[] result { get; set; }
        public int totleCount { get; set; }
    }

    public class GetReportResult
    {
        public string CHRSDYLS { get; set; }
        public string CHRSXYLSDDFSDYLS { get; set; }
        public string TYDBDRDC { get; set; }
        public string TYDBTYYYDRDC { get; set; }
        public string TYYYDRDC { get; set; }
        public string bookState { get; set; }
        public string cancelFailReason { get; set; }
        public string cancelFeedback { get; set; }
        public string cancelState { get; set; }
        public string customerPickup { get; set; }
        public string feeModifyReason { get; set; }
        public decimal totalFee { get; set; }
        public string xmsOrderId { get; set; }

    }
}
