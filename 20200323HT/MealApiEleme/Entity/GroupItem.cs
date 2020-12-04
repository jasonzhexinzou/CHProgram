using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealApiEleme.Entity
{
    public class GroupItem
    {
        /// <summary>
        /// 
        /// </summary>
        public int category_id { get; set; }

        /// <summary>
        /// 五味小吃桶
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double price { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int sale_mode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> garnish { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> specs { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int quantity { get; set; }
    }
}
