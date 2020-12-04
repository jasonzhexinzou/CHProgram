using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity
{
    public partial class P_USERINFO
    {
        /// <summary>
        /// 已经被停用
        /// </summary>
        public int IsServPause { get; set; }
        /// <summary>
        /// 允许使用院外会议
        /// </summary>
        public int IsOutSideHT { get; set; }
    }
}
