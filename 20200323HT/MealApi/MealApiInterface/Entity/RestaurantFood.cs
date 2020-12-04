using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealApiInterface.Entity
{
    public class RestaurantFood
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string image_url { get; set; }
        public float price { get; set; }
        public float rating { get; set; }
        public int sales { get; set; }
    }
}
