using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealApiEleme.Entity
{
    /// <summary>
    /// 下单
    /// </summary>
    public class OrderRes : BaseEntity
    {
        public OrderRes_data data { get; set; }
    }

    public class OrderRes_data
    {
        /// <summary>
        /// 
        /// </summary>
        public int error_code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double original_price { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string deliver_time { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string order_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int status_code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int restaurant_id { get; set; }

        /// <summary>
        /// 刘达先生
        /// </summary>
        public string consignee { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string invoice { get; set; }

        /// <summary>
        /// 八大金刚
        /// </summary>
        public string restaurant_name { get; set; }

        /// <summary>
        /// 上海市东方医院东方医院东方医院
        /// </summary>
        public string address { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int is_online_paid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 上海市东方医院东方医院东方医院(辽宁省大连市甘井子区敬贤街26号)
        /// </summary>
        public string delivery_poi_address { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double total_price { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int user_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string created_at { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OrderDetail detail { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double deliver_fee { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string delivery_geo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string user_name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> phone_list { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int is_book { get; set; }

    }


}
