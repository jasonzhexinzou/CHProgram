using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealApiInterface.Enum
{
    /// <summary>
    /// 订单状态
    /// </summary>
    public enum OrderState
    {
        /// <summary>
        /// 等待支付
        /// </summary>
        STATUS_CODE_NOT_PAID = -5,
        /// <summary>
        /// 支付失败
        /// </summary>
        STATUS_CODE_PAYMENT_FAIL = -4,
        /// <summary>
        /// 订单已取消
        /// </summary>
        STATUS_CODE_INVALID = -1,
        /// <summary>
        /// 订单未处理
        /// </summary>
        STATUS_CODE_UNPROCESSED = 0,
        /// <summary>
        /// 订单已处理
        /// </summary>
        STATUS_CODE_PROCESSED_AND_VALID = 2,
        /// <summary>
        /// 用户确认订单
        /// </summary>
        STATUS_CODE_USER_CONFIRMED = 11
    }
}
