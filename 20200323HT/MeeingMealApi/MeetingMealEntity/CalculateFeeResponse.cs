using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingMealEntity
{
    public class CalculateFeeResponse : ResponseBase
    {
        public CalculateFeeResult result { get; set; }
        public int totleCount { get; set; }
    }

    public class CalculateFeeResult
    {
        public decimal allPrice { get; set; }
        public decimal foodFee { get; set; }
        public decimal packageFee { get; set; }
        public decimal sendFee { get; set; }
    }

}
