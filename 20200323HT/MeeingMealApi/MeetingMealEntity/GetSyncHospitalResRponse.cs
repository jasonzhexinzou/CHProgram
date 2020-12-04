using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingMealEntity
{
    public class GetSyncHospitalResRponse : ResponseBase
    {
        public string totleCount { get; set; }
        public List<GetSyncHospitalResResult> result { get; set; }
    }

    public class GetSyncHospitalResResult
    {
        public string address { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string gskHospital { get; set; }
        public string memo { get; set; }
        public string name { get; set; }
        public GetSyncHospitalResCount resCount { get; set; }
        public string type { get; set; }
        public List<GetSyncHospitalResList> resList { get; set; }
    }

    public class GetSyncHospitalResList
    {
        public string resId { get; set; }
        public string resName { get; set; }
    }

    public class GetSyncHospitalResCount
    {
        public int totalCount { get; set; }
        public int breakfastCount { get; set; }
        public int lunchCount { get; set; }
        public int teaCount { get; set; }
    }
}
