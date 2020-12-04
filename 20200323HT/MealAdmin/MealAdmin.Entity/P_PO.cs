using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity
{
    public class P_PO
    {
        public Guid ID { get; set; }
        public string PO { get; set; }
        public int IsUsed { get; set; }
    }
}
