using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealApiEleme.Entity
{
    public class RestaurantsRes : BaseEntity
    {
        public RestaurantsRes_data data { get; set; }
    }

    public class RestaurantsRes_data
    {
        public List<RestaurantsItem> restaurants { get; set; }
    }

}
