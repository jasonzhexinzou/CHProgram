using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealApiEleme.Entity
{
    public class OrderStateRes : BaseEntity
    {
        public OrderStateRes_data data { get; set; }
    }

    public class OrderStateRes_data
    {
        public int status_code { get; set; }
    }

}
