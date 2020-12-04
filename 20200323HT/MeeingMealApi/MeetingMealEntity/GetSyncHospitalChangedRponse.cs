using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingMealEntity
{
    public class GetSyncHospitalChangedRponse : ResponseBase
    { 
        public List<GetSyncHospitalChangedRponseResult> result { get; set; }
    }

    public class GetSyncHospitalChangedRponseResult
    {
        public int action { get; set; }
        public string address { get; set; }
        public string cityName { get; set; }
        public string gskHospital { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string name { get; set; }
        public string provinceName { get; set; }
        public string type { get; set; }

        public int ID { get; set; }
        public string GskHospital { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string HospitalName { get; set; }
        public string Address { get; set; }
        public string IsMainAdd { get; set; }
        public string Market { get; set; }
        public Double Longitude { get; set; }
        public Double Latitude { get; set; }
        public string DistrictCode { get; set; }
        public string District { get; set; }
        public string Remarks { get; set; }
        public int Action { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateBy { get; set; }

    }
}
