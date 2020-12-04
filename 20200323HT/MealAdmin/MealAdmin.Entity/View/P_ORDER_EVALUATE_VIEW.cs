using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity.View
{
    public class P_ORDER_EVALUATE_VIEW
    {
        public Guid ID { get; set; }
        public string DCUserName { get; set; }
        public string MUDID { get; set; }
        public string CN { get; set; }
        public string PO { get; set; }
        public DateTime SendTime { get; set; }
        public string RestName { get; set; }
        public string ResCode { get; set; }
        public int State { get; set; }
        public int EStar { get; set; }
        public int EOnTime { get; set; }
        public string EOnTimeRemark { get; set; }
        public int EIsSafe { get; set; }
        public string EIsSafeRemark { get; set; }
        public int EHealth { get; set; }
        public string EHealthRemark { get; set; }
        public int EPack { get; set; }
        public string EPackRemark { get; set; }
        public int ECost { get; set; }
        public string ECostRemark { get; set; }
        public string EOtherRemark { get; set; }
        public DateTime AppDate { get; set; }
        public string Channel { get; set; }
    }
}
