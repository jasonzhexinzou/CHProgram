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
    public class CostCenterDao : ICostCenterDao
    {
        [Bean("sqlServerTemplFactory")]
        public DbTemplateFactory sqlServerTemplFactory { get; set; }

        #region 新建成本中心
        /// <summary>
        /// 新建成本中心
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Add(M_COSTCENTER entity)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery(
                    "INSERT M_COSTCENTER (ID, Code, Name, MaxMoney, LimitMoney, LineManagerID, OwnerID, CostType, State, Creator, CreateDate, Modifier, ModifyDate) "
                    + " VALUES (@ID, @Code, @Name, @MaxMoney, @LimitMoney, @LineManagerID, @OwnerID, @CostType, @State, @Creator, @CreateDate, @Modifier, @ModifyDate) ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", entity.ID),
                        SqlParameterFactory.GetSqlParameter("@Code", entity.Code),
                        SqlParameterFactory.GetSqlParameter("@Name", entity.Name),
                        SqlParameterFactory.GetSqlParameter("@MaxMoney", entity.MaxMoney),
                        SqlParameterFactory.GetSqlParameter("@LimitMoney", entity.LimitMoney),
                        SqlParameterFactory.GetSqlParameter("@LineManagerID", entity.LineManagerID),
                        SqlParameterFactory.GetSqlParameter("@OwnerID", entity.OwnerID),
                        SqlParameterFactory.GetSqlParameter("@CostType", entity.CostType),
                        SqlParameterFactory.GetSqlParameter("@State", entity.State),
                        SqlParameterFactory.GetSqlParameter("@Creator", entity.Creator),
                        SqlParameterFactory.GetSqlParameter("@CreateDate", entity.CreateDate),
                        SqlParameterFactory.GetSqlParameter("@Modifier", entity.Modifier),
                        SqlParameterFactory.GetSqlParameter("@ModifyDate", entity.ModifyDate)
                    });
            }
        }
        #endregion

        #region 删除成本中心
        /// <summary>
        /// 删除成本中心
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Del(Guid id)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("UPDATE M_COSTCENTER SET State=4 WHERE ID=@ID ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", id)
                    });
            }
        }
        #endregion

        #region 查找成本中心
        /// <summary>
        /// 查找成本中心
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public M_COSTCENTER Find(Guid id)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Find<M_COSTCENTER>("SELECT * FROM M_COSTCENTER WHERE ID=@ID ",
                    new SqlParameter[] { SqlParameterFactory.GetSqlParameter("@ID", id) });
            }
        }
        #endregion

        #region 分页查询成本中心
        /// <summary>
        /// 分页查询成本中心
        /// </summary>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<M_COSTCENTER> LoadPage(string code, string name, int rows, int page, out int total)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.LoadPages<M_COSTCENTER>(rows, page, out total,
                    "SELECT * FROM M_COSTCENTER WHERE State<>4 AND (Code LIKE @Code OR Name LIKE @Name) ",
                    " ORDER BY ModifyDate DESC ", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@Code", code),
                        SqlParameterFactory.GetSqlParameter("@Name", name),
                    });
                return list;
            }
        }
        #endregion

        #region 更新成本中西
        /// <summary>
        /// 更新成本中西
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Upd(M_COSTCENTER entity)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery(
                    "UPDATE M_COSTCENTER SET Code=@Code, Name=@Name, MaxMoney=@MaxMoney, LimitMoney=@LimitMoney, State=@State, Modifier=@Modifier, ModifyDate=@ModifyDate WHERE ID=@ID ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", entity.ID),
                        SqlParameterFactory.GetSqlParameter("@Code", entity.Code),
                        SqlParameterFactory.GetSqlParameter("@Name", entity.Name),
                        SqlParameterFactory.GetSqlParameter("@MaxMoney", entity.MaxMoney),
                        SqlParameterFactory.GetSqlParameter("@LimitMoney", entity.LimitMoney),
                        SqlParameterFactory.GetSqlParameter("@State", entity.State),
                        SqlParameterFactory.GetSqlParameter("@Modifier", entity.Modifier),
                        SqlParameterFactory.GetSqlParameter("@ModifyDate", entity.ModifyDate)
                    });
            }
        }
        #endregion
        
    }
}
