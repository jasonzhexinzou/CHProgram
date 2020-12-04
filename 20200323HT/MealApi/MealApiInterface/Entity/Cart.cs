using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealApiInterface.Entity
{
    public class Cart
    {
        public string id { get; set; }
        public decimal total { get; set; }
        public decimal deliver_amount { get; set; }
        public int restaurant_id { get; set; }
        public string create_time { get; set; }
        public int error_code { get; set; }
        public string phone { get; set; }
        public List<FoodCart> foods { get; set; }
        public List<FoodCartExtra> extras { get; set; }
    }
}
