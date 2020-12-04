using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity
{
    public partial class V_COST_SUMMARY
    {
        public string BudgetTotal { get; set; }

        public string PreApprovalCount { get; set; }

        public string RealPrice { get; set; }

        public string OrderCount { get; set; }

        public string SpecialOrderApplierCount { get; set; }

        public string SpecialOrderCount { get; set; }

        public string UnfinishedOrderApplierCount { get; set; }

        public string UnfinishedOrderCount { get; set; }
    }
}
