using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity
{
    public partial class P_OperationAudit
    {
        public Guid ID { get; set; }
        public string OperatorName { get; set; }
        public string OperatorID { get; set; }
        public string TypeID { get; set; }
        public string Operation { get; set; }
        public int StateID { get; set; }
        public string Exception { get; set; }
        public DateTime? CreateDate { get; set; }

    }
}
