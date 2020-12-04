using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealApiEleme.Entity
{
    public class RatingRestaurantRes : BaseEntity
    {
        public RatingRestaurantRes_data data { get; set; }
    }


    public class RatingRestaurantRes_data
    {
        public List<RatingRestaurantRes_data_ratings> ratings { get; set; }
    }

    public class RatingRestaurantRes_data_ratings
    {
        public int rating_id { get; set; }
        public bool is_from_eleme { get; set; }
        public string rated_at { get; set; }
        public int rating_star { get; set; }
        public string rating_text { get; set; }
        public string user_name { get; set; }
        public List<RatingRestaurantRes_data_ratings_foods> foods { get; set; }

    }

    public class RatingRestaurantRes_data_ratings_foods
    {
        public int food_id { get; set; }
        public int rating { get; set; }
        public string rating_text { get; set; }
        public string food_name { get; set; }
        public string image_url { get; set; }
        public string rated_at { get; set; }

    }


}
