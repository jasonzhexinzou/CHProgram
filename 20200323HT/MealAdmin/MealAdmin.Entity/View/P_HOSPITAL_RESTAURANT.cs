using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity.View
{
    public class P_HOSPITAL_RESTAURANT
    {
        public Guid ID { get; set; }
        public string DataSources { get; set; }
        public string GskHospital { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Type { get; set; }
        public string Memo { get; set; }
        public int ResCount { get; set; }
        public int XMSCount { get; set; }
        public int BDSCount { get; set; }
        public int MTCount { get; set; }
        public DateTime CreateDate { get; set; }
        public List<RangRestaurant> resList { get; set; }
    }

    public class RangRestaurant
    {
        public Guid ID { get; set; }
        public Guid HospitalRangeID { get; set; }
        public string ResId { get; set; }
        public string ResName { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
