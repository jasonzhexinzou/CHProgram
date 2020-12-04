using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealApiEleme.Entity
{
    public class CartRes : BaseEntity
    {
        public CartRes_data data { get; set; }
    }

    public class CartRes_data
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string> abandoned_extra { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int category_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<List<GroupItem>> group { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string phone { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int error_code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<CartExtraItem> extra { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int is_online_paid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int source { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string create_time { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string promotion_status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double total { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double deliver_amount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int restaurant { get; set; }
    }
}
