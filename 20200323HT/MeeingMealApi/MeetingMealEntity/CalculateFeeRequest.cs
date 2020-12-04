using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingMealEntity
{
    public class CalculateFeeRequest : RequestBase
    {
        public string hospitalId { get; set; }
        public string resId { get; set; }
        public FoodRequest[] foods { get; set; }
    }
}
