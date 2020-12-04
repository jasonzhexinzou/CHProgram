using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MealAdmin.Web.Areas.P.Models
{
    public class P_ORDER_EVALUATE_VIEW_EXT
    {
        public Guid ID { get; set; }
        public string CN { get; set; }
        public string PO { get; set; }
        public string MUDID { get; set; }
        public string UserName { get; set; }
        public string DeliverDate { get; set; }
        public string DeliverTime { get; set; }
        public string RestName { get; set; }
        public string RestCode { get; set; }
        public string UNOnTimeRemark { get; set; }
        public string EStar { get; set; }
        public string EOnTime { get; set; }
        public string EOnTimeRemark { get; set; }
        public string EIsSafe { get; set; }
        public string EIsSafeRemark { get; set; }
        public string EHealth { get; set; }
        public string EHealthRemark { get; set; }
        public string EPack { get; set; }
        public string EPackRemark { get; set; }
        public string ECost { get; set; }
        public string ECostRemark { get; set; }
        public string EOtherRemark { get; set; }
        public string AppDate { get; set; }
        public string AppTime { get; set; }
        public string Channel { get; set; }
    }
}