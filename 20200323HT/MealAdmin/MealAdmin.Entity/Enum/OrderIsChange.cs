using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity.Enum
{
    public class OrderIsChange
    {
        /// <summary>
        /// 未发起改单
        /// </summary>
        public const int NOT = 0;
        /// <summary>
        /// 发起改单
        /// </summary>
        public const int YES = 1;
        /// <summary>
        /// 改单成功
        /// </summary>
        public const int SUCCESS = 2;
        /// <summary>
        /// 改单失败
        /// </summary>
        public const int FAIL = 3;



    }
}
