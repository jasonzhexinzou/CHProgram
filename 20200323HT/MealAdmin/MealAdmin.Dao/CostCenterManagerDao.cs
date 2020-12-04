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
    public class CostCenterManagerDao : ICostCenterManagerDao
    {
        [Bean("sqlServerTemplFactory")]
        public DbTemplateFactory sqlServerTemplFactory { get; set; }

        #region 批量增加
        /// <summary>
        /// 批量增加
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public int Add(List<M_COSTCENTER_MANAGER> entitys)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                foreach (var entity in entitys)
                {
                    sqlServerTemplate.ExecuteNonQuery(
                        "INSERT INTO M_COSTCENTER_MANAGER (ID, CostCenterID, QyUserID, CreateDate) VALUES (@ID, @CostCenterID, @QyUserID, @CreateDate) ",
                        new SqlParameter[]
                        {
                            SqlParameterFactory.GetSqlParameter("@ID", entity.ID),
                            SqlParameterFactory.GetSqlParameter("@CostCenterID", entity.CostCenterID),
                            SqlParameterFactory.GetSqlParameter("@QyUserID", entity.QyUserID),
                            SqlParameterFactory.GetSqlParameter("@CreateDate", entity.CreateDate)
                        });
                }
                return 1;
            }
        }
        #endregion

        #region 增加一条
        /// <summary>
        /// 增加一条
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Add(M_COSTCENTER_MANAGER entity)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("INSERT INTO M_COSTCENTER_MANAGER (ID, CostCenterID, QyUserID, CreateDate) VALUES (@ID, @CostCenterID, @QyUserID, @CreateDate) ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", entity.ID),
                        SqlParameterFactory.GetSqlParameter("@CostCenterID", entity.CostCenterID),
                        SqlParameterFactory.GetSqlParameter("@QyUserID", entity.QyUserID),
                        SqlParameterFactory.GetSqlParameter("@CreateDate", entity.CreateDate)
                    });
            }
        }
        #endregion

        #region 删除一条
        /// <summary>
        /// 删除一条
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public int Del(Guid ID)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("DELETE FROM M_COSTCENTER_MANAGER WHERE ID=@ID ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", ID)
                    });
            }
        }
        #endregion

        #region 批量删除
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="CostID"></param>
        /// <returns></returns>
        public int DelByCostID(Guid CostID)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("DELETE FROM M_COSTCENTER_MANAGER WHERE CostCenterID=@CostCenterID ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@CostCenterID", CostID)
                    });
            }
        }
        #endregion

        #region 载入成本中心审批人
        /// <summary>
        /// 载入成本中心审批人
        /// </summary>
        /// <param name="CostID"></param>
        /// <returns></returns>
        public List<M_COSTCENTER_MANAGER> Load(Guid CostID)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<M_COSTCENTER_MANAGER>(
                    "SELECT * FROM M_COSTCENTER_MANAGER WHERE CostCenterID=@CostCenterID ", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@CostCenterID", CostID)
                    });
                return list;
            }
        }
        #endregion
    }
}
