using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity
{
    public class P_CITY_LIST
    {
        public Guid ID { get; set; }
        public string ProvinceName { get; set; }
        public string CityName { get; set; }
        public string Rank { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
