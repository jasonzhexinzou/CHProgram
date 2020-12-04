using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity
{
    public class P_AddressApproveHistory
    {
        public Guid ID { get; set; }
        public Guid DA_ID { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public int ActionType { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string Comments { get; set; }
        public int type { get; set; }
        public int IsDelete { get; set; }
    }
}
