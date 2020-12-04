using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingMealEntity
{
    public class QueryResResponse : ResponseBase
    {
        public List<QueryResResult> result { get; set; }
        public int totleCount { get; set; }
    }

    public class QueryResResult
    {
        public string address { get; set; }
        public long businessHourEnd { get; set; }
        public long businessHourStart { get; set; }
        public string cityId { get; set; }
        public string deliveryNotice { get; set; }
        public long eveningHourEnd { get; set; }
        public long eveningHourStart { get; set; }
        public string imagePath { get; set; }
        public string introduction { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public int minAmount { get; set; }
        public int minSendCount { get; set; }
        public string packageFeeNotice { get; set; }
        public string resId { get; set; }
        public string resName { get; set; }
        public string resTel { get; set; }
        public List<Menu> menuList { get; set; }
    }

}
