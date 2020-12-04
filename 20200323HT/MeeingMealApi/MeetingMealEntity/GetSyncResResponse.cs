using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingMealEntity
{
    public class GetSyncResResponse : ResponseBase
    {
        public string totleCount { get; set; }
        public List<GetSyncResResult> result { get; set; }
    }

    public class GetSyncResResult
    {
        public string city { get; set; }
        public string province { get; set; }
        public string resId { get; set; }
        public string resName { get; set; }
        public string resType { get; set; }
    }
}
