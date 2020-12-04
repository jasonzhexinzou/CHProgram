using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealApiInterface.Entity
{
    public class FoodCart
    {
        public string id { get; set; }
        public int quantity { get; set; }
        public string name { get; set; }
        public decimal price { get; set; }
    }
}
