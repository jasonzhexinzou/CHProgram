using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity
{
    public class P_HospitalCoverChange
    {
        public Guid ID { get; set; }

        public string HospitalCode { get; set; }

        public string HospitalName { get; set; }

        public string Address { get; set; }

        public string ResId { get; set; }

        public string ResName { get; set; }

        public int Type { get; set; }

        public int State { get; set; }

        public DateTime CreateDate  { get; set; }

        public DateTime ModifyDate { get; set; }

    }
}
