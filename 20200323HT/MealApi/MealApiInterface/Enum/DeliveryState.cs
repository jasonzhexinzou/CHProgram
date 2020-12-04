using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealApiInterface.Enum
{
    /// <summary>
    /// 配送状态
    /// </summary>
    public enum DeliveryState
    {
        /// <summary>
        /// 待分配（物流系统已生成运单，待分配配送商）
        /// </summary>
        STATUS_CODE_WAITDELIVERY = 1,
        /// <summary>
        /// 待分配（配送系统已接单，待分配配送员）
        /// </summary>
        STATUS_CODE_WAITDELIVERYMAN = 2,
        /// <summary>
        /// 待取餐（已分配给配送员，配送员未取餐）
        /// </summary>
        STATUS_CODE_WAITDELIVERYMANCOMING = 3,
        /// <summary>
        /// 到店（配送员已经到店）
        /// </summary>
        STATUS_CODE_DELIVERYMANCOMING = 4,
        /// <summary>
        /// 配送中（配送员已取餐，正在配送）
        /// </summary>
        STATUS_CODE_DELIVERYING = 5,
        /// <summary>
        /// 配送成功（配送员配送完成）
        /// </summary>
        STATUS_CODE_DELIVERY_SUCCESS = 6,
        /// <summary>
        /// 配送取消（商家可以重新发起配送）
        /// </summary>
        STATUS_CODE_DELIVERY_FAIL = 7,
        /// <summary>
        /// 配送异常（配送过程出现异常，如食物丢失、配送员意外...）
        /// </summary>
        STATUS_CODE_DELIVERY_EXCEPTION = 8

    }
}
