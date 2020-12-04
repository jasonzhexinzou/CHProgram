using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity
{
    public class P_Audit
    {
        public Guid ID { get; set; }
        public string UserID { get; set; }

        public int Type { get; set; }
        public string ChangeContent { get; set; }
        public DateTime? CreatDate { get; set; }
    }
    public class P_Audit_View
    {
        public Guid ID { get; set; }
        public string UserID { get; set; }

        public string Type { get; set; }
        public string ChangeContent { get; set; }
        public string CreatDate { get; set; }
        public string CreatTime { get; set; }
    }
}
