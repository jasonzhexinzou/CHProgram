using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealApiEleme.Entity
{
    public class FoodRating
    {
        public int food_id { get; set; }
        public int rating { get; set; }
        public string rating_text { get; set; }
        public string image_url { get; set; }
        public string image_ext { get; set; }
    }
}
