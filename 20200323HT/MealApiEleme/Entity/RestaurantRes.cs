using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealApiEleme.Entity
{
    public class RestaurantRes : BaseEntity
    {
        public RestaurantRes_data data { get; set; }
    }

    public class RestaurantRes_data
    {
        public RestaurantsItem restaurant { get; set; }
    }
}
