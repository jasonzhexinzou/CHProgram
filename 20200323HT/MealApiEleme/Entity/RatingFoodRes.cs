using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealApiEleme.Entity
{
    public class RatingFoodRes : BaseEntity
    {
        public List<RatingFoodRes_data> data { get; set; }
    }

    public class RatingFoodRes_data
    {
        public string food_id { get; set; }
        public int rating { get; set; }
        public string rating_text { get; set; }
        public string food_name { get; set; }
        public string image_url { get; set; }
        public string rated_at { get; set; }
    }

}
