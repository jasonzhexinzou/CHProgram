using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealApiInterface.Entity
{
    public class Rating
    {
        public string id { get; set; }
        public string from { get; set; }
        public string rated_at { get; set; }
        public int rating_star { get; set; }
        public string rating_text { get; set; }
        public string user_name { get; set; }
        public List<RatingFood> foods { get; set; }
    }
}
