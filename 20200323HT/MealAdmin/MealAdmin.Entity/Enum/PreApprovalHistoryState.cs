using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity.Enum
{
    public class PreApprovalHistoryState
    {
        /// <summary>
        /// 预申请提交成功
        /// </summary>
        public const int SUBMITSUCCEED = 1;
        /// <summary>
        /// 预申请审批驳回
        /// </summary>
        public const int REJECT = 2;
        /// <summary>
        /// 预申请审批通过
        /// </summary>
        public const int APPROVE = 3;
        /// <summary>
        /// 预申请修改成功
        /// </summary>
        public const int RESUBMITSUCCEED = 4;
        /// <summary>
        /// 预申请取消成功
        /// </summary>
        public const int CANCELSUCCEED = 5;
    }
}
