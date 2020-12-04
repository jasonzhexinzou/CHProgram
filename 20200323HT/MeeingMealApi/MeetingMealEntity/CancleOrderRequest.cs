using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingMealEntity
{
    public class CancleOrderRequest : RequestBase
    {
        public string xmsOrderId { get; set; }
        public string remark { get; set; }
    }
}
