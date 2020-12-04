using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity
{
    public class P_UserDelegatePre
    {
        public Guid ID { get; set; }
        public string UserMUDID { get; set; }
        public string UserName { get; set; }
        public string DelegateUserMUDID { get; set; }
        public string DelegateUserName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int IsEnable { get; set; }
        public string Remarks { get; set; }

    }
}
