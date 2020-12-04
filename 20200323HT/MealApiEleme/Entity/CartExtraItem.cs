using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealApiEleme.Entity
{
    public class CartExtraItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double price { get; set; }

        /// <summary>
        /// 配送费
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int category_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int quantity { get; set; }
    }
}
