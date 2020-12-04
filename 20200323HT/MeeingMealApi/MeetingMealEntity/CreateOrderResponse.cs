using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingMealEntity
{
    public class CreateOrderResponse : ResponseBase
    {
        public CreateOrderResult result { get; set; }
        public int totleCount { get; set; }
    }

    public class CreateOrderResult
    {
        public string enterpriseOrderId { get; set; }
        public string xmsOrderId { get; set; }
    }
}
