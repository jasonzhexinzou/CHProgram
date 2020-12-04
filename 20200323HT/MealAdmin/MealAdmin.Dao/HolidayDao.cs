using MealAdmin.Entity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XFramework.XDataBase;
using XFramework.XDataBase.SqlServer;
using XFramework.XInject.Attributes;

namespace MealAdmin.Dao
{
    public class HolidayDao : IHolidayDao
    {
        [Bean("sqlServerTemplFactory")]
        public DbTemplateFactory sqlServerTemplFactory { get; set; }

        #region 找到距今最近的一个休假日区间
        /// <summary>
        /// 找到距今最近的一个休假日区间
        /// </summary>
        /// <param name="now"></param>
        /// <returns></returns>
        public P_HOLIDAY FindNextRange(DateTime now)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var holiday = sqlServerTemplate.Find<P_HOLIDAY>(
                    "SELECT TOP 1 * FROM P_HOLIDAY WHERE EndDay >= @Now ORDER BY StartDay ", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@Now", now)
                    });
                return holiday;
            }
        }
        #endregion
    }
}
