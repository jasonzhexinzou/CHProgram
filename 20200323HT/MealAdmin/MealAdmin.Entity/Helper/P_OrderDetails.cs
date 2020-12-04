using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MealAdmin.Entity.Helper
{
    public class P_OrderDetails
    {
        public int attendCount { get; set; }
        public string deliveryAddress { get; set; }
        public string consignee { get; set; }
        public string phone { get; set; }
        public DateTime? createTime { get; set; }
        public DateTime? deliverTime { get; set; }
        public DateTime? oldDeliverTime { get; set; }
        public string remark { get; set; }

    }
}