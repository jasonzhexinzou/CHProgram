using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealApiInterface.Entity
{
    #region 餐厅实体类
    /// <summary>
    /// 餐厅实体类
    /// </summary>
    public class Restaurant
    {
        public string restaurant_id { get; set; }
        public string restaurant_name { get; set; }
        public string mobile { get; set; }
        public List<string> phone_list { get; set; }
        public string agent_fee { get; set; }
        public string deliver_amount { get; set; }
        public string image_url { get; set; }
        public int deliver_spent { get; set; }
        public float num_rating { get; set; }
        public int recent_order_num { get; set; }
        public int distances { get; set; }
        public List<string> serving_time { get; set; }
        public int is_bookable { get; set; }
        public List<string> deliver_times { get; set; }
        public string promotion_info { get; set; }
        public string address_text { get; set; }
    }
    #endregion
}
