using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealApiInterface.Entity
{
    public class Order
    {
        public int error_code { get; set; }
        public string error_msg { get; set; }
        public string order_id { get; set; }

        public decimal original_price { get; set; }
        public decimal total_price { get; set; }
        public decimal deliver_fee { get; set; }
        public string deliver_time { get; set; }
        public int status_code { get; set; }
        public int restaurant_id { get; set; }
        public string consignee { get; set; }
        public string invoice { get; set; }
        public string restaurant_name { get; set; }
        public string address { get; set; }
        public int is_online_paid { get; set; }
        public string description { get; set; }
        public string delivery_poi_address { get; set; }
        public string created_at { get; set; }
        public string delivery_geo { get; set; }
        public string phones { get; set; }
        public int is_book { get; set; }

        public List<FoodCart> foods { get; set; }
        public List<FoodCartExtra> extras { get; set; }

    }
}
