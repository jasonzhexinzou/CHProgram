using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingMealEntity
{

    /// <summary>
    /// 回调金额变动
    /// </summary>
    public class OrderFeeChangeCallBack
    {

        public OrderFeeChangeResult requestData { get; set; }

    }

    public class OrderFeeChangeResult
    {
        public decimal totalFee { get; set; }
        public int msgId { get; set; }
        public string supplierOrderId { get; set; }
        public string remark { get; set; }
        public string timestamp { get; set; }
        public string Sign { get; set; }
    }

}
