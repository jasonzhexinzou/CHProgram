using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MealAdmin.Entity;
using MealAdmin.Dao;
using XFramework.XInject.Attributes;

namespace MealAdmin.Service
{
    public class HolidayService : IHolidayService
    {

        [Bean("holidayDao")]
        public IHolidayDao holidayDao { get; set; }

        #region 找到距今最近的一个休假日区间
        /// <summary>
        /// 找到距今最近的一个休假日区间
        /// </summary>
        /// <param name="now"></param>
        /// <returns></returns>
        public P_HOLIDAY FindNextRange(DateTime now)
        {
            return holidayDao.FindNextRange(now.Date);
        }
        #endregion
    }
}
