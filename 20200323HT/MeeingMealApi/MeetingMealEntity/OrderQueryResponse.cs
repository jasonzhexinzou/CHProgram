using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingMealEntity
{
    public class OrderQueryResponse : ResponseBase
    {
        public OrderQueryResult[] result { get; set; }
        public int totleCount { get; set; }
    }

    public class OrderQueryResult
    {
        public string address { get; set; }
        public string cityId { get; set; }
        public string cn { get; set; }
        public string department { get; set; }
        public string dinnerName { get; set; }
        public int dinnerNum { get; set; }
        public string enterpriseOrderId { get; set; }
        public string hospitalId { get; set; }
        public string orderTime { get; set; }
        public string phone { get; set; }
        public string remark { get; set; }
        public string resId { get; set; }
        public string resName { get; set; }
        public string sendTime { get; set; }
        public int statId { get; set; }
        public decimal totalFee { get; set; }
        public string xmsOrderId { get; set; }


    }


}
