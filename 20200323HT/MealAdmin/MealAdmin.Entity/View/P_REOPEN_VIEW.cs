using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity.View
{
    public class P_REOPEN_VIEW
    {
        public string HTCode { get; set; }
        public string OriginatorMUDID { get; set; }
        public string OriginatorName { get; set; }
        public string ReopenReason { get; set; }
        public string ReopenRemark { get; set; }
        public string CurrentUserId { get; set; }
        public string CurrentUserName { get; set; }
        public string ApplierMUDID { get; set; }
    }
}
