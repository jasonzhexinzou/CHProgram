using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MealAdmin.Entity;
using XFramework.XDataBase;
using XFramework.XInject.Attributes;
using XFramework.XDataBase.SqlServer;
using System.Data.SqlClient;

namespace MealAdmin.Dao
{

    public class DCostCenterDao : IDCostCenterDao
    {
        [Bean("sqlServerTemplFactory")]
        public DbTemplateFactory sqlServerTemplFactory { get; set; }

        #region 新建成本中心
        /// <summary>
        /// 新建成本中心
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int AddCostCenter(D_COSTCENTER entity)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery(
                    "INSERT D_COSTCENTER (ID,Market,TA,BUHeadName,Region,RegionManagerName,RegionManagerMUDID,CostCenter,CreateDate,CreatedBy,ModifyDate,ModifiedBy) "
                    + " VALUES (@ID,@Market,@TA,@BUHeadName,@Region,@RegionManagerName,@RegionManagerMUDID,@CostCenter,@CreateDate,@CreatedBy,@ModifyDate,@ModifiedBy) ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", entity.ID),
                        SqlParameterFactory.GetSqlParameter("@Market", entity.Market),
                        SqlParameterFactory.GetSqlParameter("@TA", entity.TA),
                        SqlParameterFactory.GetSqlParameter("@BUHeadName", entity.BUHeadName),
                        SqlParameterFactory.GetSqlParameter("@Region", entity.Region),
                        SqlParameterFactory.GetSqlParameter("@RegionManagerName", entity.RegionManagerName),
                        SqlParameterFactory.GetSqlParameter("@RegionManagerMUDID", entity.RegionManagerMUDID),
                        SqlParameterFactory.GetSqlParameter("@CostCenter", entity.CostCenter),
                        SqlParameterFactory.GetSqlParameter("@CreateDate", entity.CreateDate),
                        SqlParameterFactory.GetSqlParameter("@CreatedBy", entity.CreatedBy),
                        SqlParameterFactory.GetSqlParameter("@ModifyDate", entity.ModifyDate),
                        SqlParameterFactory.GetSqlParameter("@ModifiedBy", entity.ModifiedBy)
                    });
            }
        }
        #endregion

        #region 根据ID查找成本中心
        /// <summary>
        /// 根据ID查找成本中心
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public D_COSTCENTER FindCostCenter(Guid id)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Find<D_COSTCENTER>("SELECT * FROM D_COSTCENTER WHERE ID=@ID ",
                    new SqlParameter[] { SqlParameterFactory.GetSqlParameter("@ID", id) });
            }
        }
        #endregion

        #region 根据TA查找成本中心
        /// <summary>
        /// 根据TA查找成本中心
        /// </summary>
        /// <param name="ta">TA</param>
        /// <returns></returns>
        public List<D_COSTCENTER> FindCostCenterByTA(string ta)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<D_COSTCENTER>("SELECT TA " + "-" + "Region" + "'('" + "CostCenter" + "')' as CostCenter FROM D_CostCenter WHERE TA=@TA", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@TA", ta)
                    });
                return list;
            }
        }
        #endregion

        #region 获取全部成本中心
        /// <summary>
        /// 获取全部成本中心
        /// </summary>
        /// <returns></returns>
        public List<D_COSTCENTER> GetAllCostCenter()
        {
            List<D_COSTCENTER> result;
            var sqlSelectAll = "SELECT CostCenter FROM D_CostCenter ";
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                result = sqlServerTemplate.Load<D_COSTCENTER>(sqlSelectAll, new SqlParameter[] { });
            }
            return result;
        } 
        #endregion
    }
}
