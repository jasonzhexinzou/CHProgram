using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealApiEleme.Entity
{
    public class Restaurant_menuItem
    {
        /// <summary>
        /// 亚惠厨房特惠套餐
        /// </summary>
        public string category { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int must_pay_online { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<FoodsItem> foods { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int must_new_user { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int category_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string is_activity { get; set; }
    }
}
