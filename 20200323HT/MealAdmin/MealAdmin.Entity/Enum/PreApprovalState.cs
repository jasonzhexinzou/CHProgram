using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity.Enum
{
    public class PreApprovalState
    {
        /// <summary>
        /// 预申请提交成功
        /// </summary>
        public const int SUBMITSUCCEED = 0;
        /// <summary>
        /// 预申请等待MMCoE审批
        /// </summary>
        public const int WAITMMCOEAPPROVAL = 1;
        /// <summary>
        /// 预申请MMCoE审批驳回
        /// </summary>
        public const int MMCOEREJECT = 2;
        /// <summary>
        /// 预申请等待BUHead审批
        /// </summary>
        public const int WAITBUHEADAPPROVAL = 3;
        /// <summary>
        /// 预申请BUHead审批驳回
        /// </summary>
        public const int BUHEADAPPROVALREJECT = 4;
        /// <summary>
        /// 预申请BUHead审批通过
        /// </summary>
        public const int PREAPPROVALAPPROVED = 5;
        /// <summary>
        /// 预申请等待二级经理审批
        /// </summary>
        public const int WAITSECONDAPPROVAL = 7;
        /// <summary>
        /// 预申请二级经理审批驳回
        /// </summary>
        public const int SECONDAPPROVALREJECT = 8;
        /// <summary>
        /// 预申请二级经理审批通过
        /// </summary>
        public const int SECONDAPPROVALAPPROVED = 9;
        /// <summary>
        /// 预申请自动审批通过
        /// </summary>
        public const int PREAPPROVALAUTOAOPPROVED = 6;
        /// <summary>
        /// 预申请取消
        /// </summary>
        public const int PREAPPROVALCANCEL = 10;


        //0,1,3 提交成功
        //5,6 审批通过
        //2，4 审批被驳回
    }
}
