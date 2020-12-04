using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealApiEleme.Entity
{
    public class RatingOrderRes : BaseEntity
    {
        public RatingOrderRes_data data { get; set; }
    }

    public class RatingOrderRes_data
    {
        public int rating_id { get; set; }
    } 
}
