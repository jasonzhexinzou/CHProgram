using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity
{
    public class P_HOSPITAL_ADDR
    {
        public Guid ID { get; set; }
        public int HospitalId { get; set; }
        public string Address { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
