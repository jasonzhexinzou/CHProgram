using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity.Enum
{
    public class GroupType
    {
        /// <summary>
        /// 投诉组
        /// Complaints = 1
        /// </summary>
        public const int Complaints = 1;
        /// <summary>
        /// MMCoe审批组
        /// MMCoE = 2
        /// </summary>
        public const int MMCoE = 2;
        /// <summary>
        /// 简报接收组
        /// BriefReport = 3
        /// </summary>
        public const int BriefReport = 3;
        /// <summary>
        /// HT暂停服务名单
        /// ServPause = 4
        /// </summary>
        public const int ServPause = 4;
        /// <summary>
        /// 院外HT名单
        /// OutSideHT = 5
        /// </summary>
        public const int OutSideHT = 5;
        /// <summary>
        /// non-HT暂停服务名单
        /// NServPause = 6
        /// </summary>
        public const int NServPause = 6;
        /// <summary>
        /// non-HT暂停服务名单
        /// NServPause = 6
        /// </summary>
        public const int DevBus = 7;

    }
}
