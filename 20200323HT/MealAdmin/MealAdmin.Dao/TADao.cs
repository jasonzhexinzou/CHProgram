using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MealAdmin.Entity;
using XFramework.XInject.Attributes;
using XFramework.XDataBase;
using System.Data.SqlClient;
using XFramework.XDataBase.SqlServer;

namespace MealAdmin.Dao
{
    public class TADao : ITADao
    {
        [Bean("sqlServerTemplFactory")]
        public DbTemplateFactory sqlServerTemplFactory { get; set; }

        #region 载入全部TA信息
        /// <summary>
        /// 载入全部TA信息
        /// </summary>
        /// <returns></returns>
        public List<P_TA> Load()
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<P_TA>("SELECT * FROM P_TA ");
                return list;
            }
        }
        #endregion

        #region 根据TA查询成本中心
        /// <summary>
        /// 根据TA查询成本中心
        /// </summary>
        /// <param name="taName"></param>
        /// <returns></returns>
        public List<D_COSTCENTER> LoadCostCenterByTA(string taName)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<D_COSTCENTER>("SELECT RegionManagerName "+ "'('" + "CostCenter" +"')' as CostCenter FROM D_CostCenter WHERE TA=@TA", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@TA", taName)
                    });
                return list;
            }
        }
        #endregion
    }
}
