using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingMealEntity
{
    /// <summary>
    /// 回调订单状态修改
    /// </summary>
    public class OrderStateChangeCallBack
    {
        public OrderStateChangeResult requestData { get; set; }
    }

    public class OrderStateChangeResult
    {
        public int status { get; set; }
        public string CheckCode { get; set; }
        public string oldsupplierOrderId { get; set; }
        public string cn { get; set; }
        public int msgId { get; set; }
        public string supplierOrderId { get; set; }
        public string remark { get; set; }
        public string timestamp { get; set; }
        public string Sign { get; set; }
    }

}
