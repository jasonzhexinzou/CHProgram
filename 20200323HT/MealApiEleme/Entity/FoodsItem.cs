using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealApiEleme.Entity
{
    public class FoodsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public float rating { get; set; }

        /// <summary>
        /// 两荤两素，赠送小菜一份
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string original_price { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int restaurant_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double packing_fee { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string pinyin_name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int sales { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<int> num_ratings { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 25元套餐
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int rating_count { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string image_url { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> attributes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float price { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int stock { get; set; }
    }
}
