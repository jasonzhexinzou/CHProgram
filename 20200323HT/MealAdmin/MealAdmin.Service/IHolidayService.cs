using MealAdmin.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Service
{
    public interface IHolidayService
    {
        #region 找到距今最近的一个休假日区间
        /// <summary>
        /// 找到距今最近的一个休假日区间
        /// </summary>
        /// <param name="now"></param>
        /// <returns></returns>
        P_HOLIDAY FindNextRange(DateTime now);
        #endregion
    }
}
