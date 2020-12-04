using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealApiEleme.Entity
{
    public class FindOrderRes : BaseEntity
    {
        public FindOrderRes_data data { get; set; }
    }

    public class FindOrderRes_data
    {
        public string address { get; set; }
        public string consignee { get; set; }
        public string created_at { get; set; }
        public double deliver_fee { get; set; }
        public string deliver_time { get; set; }
        public string description { get; set; }
        public FindOrderRes_data_detail detail { get; set; }
        public string invoice { get; set; }
        public int is_book { get; set; }
        public int is_online_paid { get; set; }
        public string order_id { get; set; }
        public List<string> phone_list { get; set; }
        public int restaurant_id { get; set; }
        public string restaurant_name { get; set; }
        public int restaurant_number { get; set; }
        public int status_code { get; set; }
        public double total_price { get; set; }
        public double original_price { get; set; }
        public int user_id { get; set; }
        public string user_name { get; set; }
        public string delivery_geo { get; set; }
        public string delivery_poi_address { get; set; }
    }

    public class FindOrderRes_data_detail
    {
        public List<FindOrderRes_data_detail_group> group { get; set; }
        public List<FindOrderRes_data_detail_extra> extra { get; set; }
    }

    public class FindOrderRes_data_detail_group
    {
        public int category_id { get; set; }
        public string name { get; set; }
        public double price { get; set; }
        public int id { get; set; }
        public int quantity { get; set; }
        public List<FindOrderRes_data_detail_group_garnish> garnish { get; set; }
    }

    public class FindOrderRes_data_detail_group_garnish
    {
        public int category_id { get; set; }
        public string name { get; set; }
        public double price { get; set; }
        public int id { get; set; }
        public int quantity { get; set; }
    }

    public class FindOrderRes_data_detail_extra
    {
        public string description { get; set; }
        public double price { get; set; }
        public string name { get; set; }
        public int category_id { get; set; }
        public int id { get; set; }
        public int quantity { get; set; }
    }

}
