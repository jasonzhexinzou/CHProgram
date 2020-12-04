using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity.View
{
    public class P_HOSPITAL_INFO_VIEW
    {
        public string GskHospital { get; set; }
        public string Provice { get; set; }
        public string City { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Type { get; set; }
        public string Remark { get; set; }
        public int External { get; set; }

        public string MainAddress { get; set; }
        public string HospitalCode { get; set; }
    }
}
