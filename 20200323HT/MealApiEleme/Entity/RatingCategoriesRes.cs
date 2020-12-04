using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealApiEleme.Entity
{
    public class RatingCategoriesRes : BaseEntity
    {
        public RatingCategoriesRes_data data { get; set; }
    }

    public class RatingCategoriesRes_data
    {
        public List<RatingCategoriesRes_data_categories> categories { get; set; }
    }

    public class RatingCategoriesRes_data_categories
    {
        public int amount { get; set; }
        public string name { get; set; }
        public int record_type { get; set; }
    }
}
