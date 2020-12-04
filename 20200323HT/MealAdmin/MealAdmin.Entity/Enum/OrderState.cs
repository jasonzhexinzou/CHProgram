using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity.Enum
{
    public class OrderState
    {
        /// <summary>
        /// 订单等待审批
        /// </summary>
        public const int WAITAPPROVE = 1;
        /// <summary>
        /// 审批拒绝
        /// </summary>
        public const int REJECT = 2;
        /// <summary>
        /// 订单提交成功
        /// </summary>
        public const int SUBMITTED = 3;
        /// <summary>
        /// 预定成功
        /// </summary>
        public const int SCHEDULEDSUCCESS = 4;
        /// <summary>
        /// 预定失败
        /// </summary>
        public const int SCHEDULEDFAIL = 5;
        /// <summary>
        /// 人类收餐
        /// </summary>
        public const int PERSIONRECEIVE = 6;
        /// <summary>
        ///  系统收餐
        /// </summary>
        public const int SYSTEMRECEIVE = 7;
        /// <summary>
        /// 未送达
        /// </summary>
        public const int FOODLOSE = 8;
        /// <summary>
        /// 已评价
        /// </summary>
        public const int EVALUATED = 9;
        /// <summary>
        /// 申请退订
        /// </summary>
        public const int RETURNING = 10;
        /// <summary>
        /// 退订成功
        /// </summary>
        public const int RETURNSUCCES = 11;
        /// <summary>
        /// 退订失败
        /// </summary>
        public const int RETURNFAIL = 12;


    }
}
