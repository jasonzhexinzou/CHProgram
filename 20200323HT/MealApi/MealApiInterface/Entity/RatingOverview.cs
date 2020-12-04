using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealApiInterface.Entity
{
    /// <summary>
    /// 评价综述
    /// </summary>
    public class RatingOverview
    {
        public int amount { get; set; }
        public string name { get; set; }
        public int record_type { get; set; }
    }
}
