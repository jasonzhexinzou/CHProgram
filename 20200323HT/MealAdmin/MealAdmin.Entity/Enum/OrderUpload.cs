using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity.Enum
{
    public class OrderUpload
    {
        /// <summary>
        /// 尚未上传文件
        /// </summary>
        public const int NOT = 0;
        /// <summary>
        /// 文件上传成功
        /// </summary>
        public const int YES = 1;
        /// <summary>
        /// 审批驳回
        /// </summary>
        public const int FAIL = 2;
        /// <summary>
        /// 财务审批驳回
        /// </summary>
        public const int CWFAIL = 3;
        /// <summary>
        /// 审批通过
        /// </summary>
        public const int SUCCESS = 4;
    }
}
