using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity.Enum
{
    public class UploadFileApprovalHistory
    {
        /// <summary>
        /// 上传文件提交成功
        /// </summary>
        public const int SUBMITSUCCEED = 1;
        /// <summary>
        /// 上传文件审批驳回
        /// </summary>
        public const int REJECT = 2;
        /// <summary>
        /// 上传文件审批通过
        /// </summary>
        public const int APPROVE = 3;
        /// <summary>
        /// 上传文件修改成功
        /// </summary>
        public const int RESUBMITSUCCEED = 4;
        /// <summary>
        /// 上传文件修改成功
        /// </summary>
        public const int FINANCEREOPEN = 5;
    }
}