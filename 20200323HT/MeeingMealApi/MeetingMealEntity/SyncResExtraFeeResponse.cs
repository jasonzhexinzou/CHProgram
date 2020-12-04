using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingMealEntity
{
    public class SyncResExtraFeeResponse : ResponseBase
    {
        public List<SyncResExtraFeeResult> result { get; set; }
        public int totleCount { get; set; }
    }

    public class SyncResExtraFeeResult
    {
        public int id { get; set; }
        public string resId { get; set; }
        public int extraType { get; set; }
        public int condition { get; set; }
        public int detailType { get; set; }
        public int dataDeal { get; set; }
        public string description { get; set; }
        public int deleted { get; set; }
    }

}
