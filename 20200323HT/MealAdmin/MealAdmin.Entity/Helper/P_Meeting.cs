using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MealAdmin.Entity.Helper
{
    public class P_Meeting 
    {
        public string code { get; set; }
        public string title { get; set; }
        public int budgetTotal { get; set; }
        public DateTime submittedDate { get; set; }
        public DateTime approvedDate { get; set; }
        public string status { get; set; }
        public string pendingWith { get; set; }
        public string userId { get; set; }
        public string userName { get; set; }
    }
}