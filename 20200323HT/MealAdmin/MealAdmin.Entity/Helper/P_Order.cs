using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MealAdmin.Entity.Helper
{
    public class P_Order
    {
        public string userid { get; set; }
        public string userName { get; set; }
        public string inTime { get; set; }
        public P_ChooseHospital hospital { get; set; }
        public string CnCode { get; set; }
        public P_Meeting meeting { get; set; }
        public P_Food foods { get; set; }
        public P_OrderDetails details { get; set; }
        public string mmCoE { get; set; }

        public string MeetingCode { get; set; }
        public string MeetingName { get; set; }
        public string PO { get; set; }
        public string WBS { get; set; }
        public int IsNonHT { get; set; }
        public int Channel { get; set; }
        public string TA { get; set; }
        public string supplier { get; set; }
    }
}