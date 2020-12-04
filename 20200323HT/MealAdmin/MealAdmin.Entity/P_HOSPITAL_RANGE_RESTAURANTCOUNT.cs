using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity
{
    public partial class P_HOSPITAL_RANGE_RESTAURANTCOUNT
    {
        public Guid ID { get; set; }
        public string GskHospital { get; set; }
        public string DataSources { get; set; }
        public int TotalCount { get; set; }
        public int BreakfastCount { get; set; }
        public int LunchCount { get; set; }
        public int TeaCount { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
