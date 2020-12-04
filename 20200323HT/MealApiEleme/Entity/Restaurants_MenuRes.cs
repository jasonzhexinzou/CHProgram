using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealApiEleme.Entity
{
    public class Restaurants_MenuRes : BaseEntity
    {
        public Restaurants_MenuRes_data data { get; set; }
    }

    public class Restaurants_MenuRes_data
    {
        public List<Restaurant_menuItem> restaurant_menu { get; set; }
    }
}
