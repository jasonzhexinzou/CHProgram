using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity.Enum
{
    public class OrderIsRetuen
    {
        /// <summary>
        /// 未申请退单
        /// </summary>
        public const int NOT = 0;
        /// <summary>
        /// 申请退单
        /// </summary>
        public const int YES = 1;
        /// <summary>
        /// 退单成功
        /// </summary>
        public const int SUCCESS = 2;
        /// <summary>
        /// 退单失败
        /// </summary>
        public const int FAIL = 3;
        /// <summary>
        /// 退单失败发起原单配送
        /// </summary>
        public const int POSTFOOD = 4;
        /// <summary>
        /// 原单配送成功
        /// </summary>
        public const int POSTSUCCESS = 5;
        /// <summary>
        /// 原单配送失败
        /// </summary>
        public const int POSTFAIL = 6;

    }
}
