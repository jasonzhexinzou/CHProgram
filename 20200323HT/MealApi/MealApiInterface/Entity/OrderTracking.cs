using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealApiInterface.Entity
{
    public class OrderTracking
    {
        public string state_code { get; set; }
        public string last_updated_at { get; set; }
        public DeliveryMan deliveryman_info { get; set; }
        public Tracking tracking_info { get; set; }
    }
}
