using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealApiEleme.Entity
{
    public class SubmitRatingFoodRes : BaseEntity
    {
        public Dictionary<string, int> data { get; set; }
    }
}
