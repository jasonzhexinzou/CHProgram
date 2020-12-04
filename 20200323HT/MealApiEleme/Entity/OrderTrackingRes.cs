using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealApiEleme.Entity
{
    public class OrderTrackingRes : BaseEntity
    {
        public OrderTrackingRes_data data { get; set; }
    }

    public class OrderTrackingRes_data
    {
        public string eleme_order_id { get; set; }
        public OrderTrackingRes_data_deliveryman deliveryman_info { get; set; }
        public string state_code { get; set; }
        public OrderTrackingRes_data_tracking tracking_info { get; set; }
        public int last_updated_at { get; set; }
    }

    public class OrderTrackingRes_data_deliveryman
    {
        public string name { get; set; }
        public string phone { get; set; }
    }

    public class OrderTrackingRes_data_tracking
    {
        public string latitude { get; set; }
        public string longitude { get; set; }
    }


}
