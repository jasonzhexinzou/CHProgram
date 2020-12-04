using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity.Enum
{
    public class MMCoEApproveState
    {
        /// <summary>
        /// 不需要审批
        /// </summary>
        public const int NO = 0;
        /// <summary>
        /// 等待审批
        /// </summary>
        public const int WAITAPPROVE = 1;
        /// <summary>
        /// 审批拒绝
        /// </summary>
        public const int APPROVEREJECT = 2;
        /// <summary>
        /// 审批通过
        /// </summary>
        public const int APPROVESUCCESS = 3;
    }
}
