using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingMealEntity
{
    public class SyncHospitalResponse : ResponseBase
    {
        public List<HospitalResponseResult> result { get; set; }

    }

    public class HospitalResponseResult
    {
        public string address { get; set; }
        public string cityId { get; set; }
        public int deleted { get; set; }
        public string firstLetters { get; set; }
        public string hospitalId { get; set; }
        public string hospitalName { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string type { get; set; }
        public string gskHospital { get; set; }
        public int External { get; set; }
    }

}
