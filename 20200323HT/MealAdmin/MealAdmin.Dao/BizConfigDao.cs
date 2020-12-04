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
    public class BizConfigDao : IBizConfigDao
    {
        [Bean("sqlServerTemplFactory")]
        public DbTemplateFactory sqlServerTemplFactory { get; set; }
        [Bean("sqlServerTemplFactoryNonHT")]
        public DbTemplateFactory sqlServerTemplFactoryNonHT { get; set; }

        public List<P_BIZ_CONF> GetAllConfig()
        {
            List<P_BIZ_CONF> result;
            var sqlSelectAll = "SELECT ID, Name, Val1, Val2, Val3 FROM P_BIZ_CONF ";
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                result = sqlServerTemplate.Load<P_BIZ_CONF>(sqlSelectAll, new SqlParameter[] { });
            }
            return result;
        }

        public List<P_MARKET> GetAllMarkets()
        {
            List<P_MARKET> result;
            var sqlSelectAll = "SELECT ID, Name, InvoiceTitle,DutyParagraph FROM P_MARKET ";
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                result = sqlServerTemplate.Load<P_MARKET>(sqlSelectAll, new SqlParameter[] { });
            }
            return result;
        }

        public int UpdateConfig(List<P_BIZ_CONF> entity, out List<P_BIZ_CONF> unSuccessData)
        {
            unSuccessData = new List<P_BIZ_CONF>();
            int upCnt = 0;
            if (entity.Count > 0)
            {
                var sqlInsert = "INSERT INTO P_BIZ_CONF(ID, Name, Val1, Val2, Val3) VALUES (@ID,@Name,@Val1,@Val2,@Val3) ";
                var sqlSelectAll = "SELECT ID, Name, Val1, Val2, Val3 FROM P_BIZ_CONF ";
                var sqlUpdate = "UPDATE P_BIZ_CONF SET Name = @Name, Val1 = @Val1, Val2 = @Val2, Val3 = @Val3 WHERE (ID = @ID) ";
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    //using (var trans = conn.BeginTransaction())
                    //{
                    var allConfigs = sqlServerTemplate.Load<P_BIZ_CONF>(sqlSelectAll, new SqlParameter[] { });
                    foreach (var _itm in entity)
                    {
                        var matchItm = allConfigs.SingleOrDefault(w => w.ID == _itm.ID);

                        //已存在该ID配置项 
                        if (matchItm != null)
                        {
                            // 业务数据相同则不更新 
                            if (matchItm.Val1 == _itm.Val1 && matchItm.Val2 == _itm.Val2 && matchItm.Val3 == _itm.Val3)
                            {
                                upCnt++;
                            }
                            else
                            {
                                if (sqlServerTemplate.ExecuteNonQuery(sqlUpdate, new SqlParameter[] {
                                        SqlParameterFactory.GetSqlParameter("@Name", matchItm.Name),
                                        SqlParameterFactory.GetSqlParameter("@Val1", _itm.Val1),
                                        SqlParameterFactory.GetSqlParameter("@Val2", _itm.Val2),
                                        SqlParameterFactory.GetSqlParameter("@Val3", _itm.Val3),
                                        SqlParameterFactory.GetSqlParameter("@ID", _itm.ID)
                                        }) == 1)
                                {
                                    upCnt++;
                                }
                                else
                                {
                                    unSuccessData.Add(_itm);
                                }
                            }
                        }
                        else
                        {
                            if (sqlServerTemplate.ExecuteNonQuery(sqlInsert, new SqlParameter[] {
                                        SqlParameterFactory.GetSqlParameter("@Name", _itm.Name),
                                        SqlParameterFactory.GetSqlParameter("@Val1", _itm.Val1),
                                        SqlParameterFactory.GetSqlParameter("@Val2", _itm.Val2),
                                        SqlParameterFactory.GetSqlParameter("@Val3", _itm.Val3),
                                        SqlParameterFactory.GetSqlParameter("@ID", _itm.ID)
                                        }) == 1)
                            {
                                upCnt++;
                            }
                            else
                            {
                                unSuccessData.Add(_itm);
                            }
                        }
                    }
                    //trans.Commit();
                    //}
                }
            }
            return upCnt;
        }

        public int UpdateMarkets(List<P_MARKET> entity, out List<P_MARKET> unSuccessData)
        {
            unSuccessData = new List<P_MARKET>();
            int upCnt = 0;
            if (entity.Count > 0)
            {
                var sqlInsert = "INSERT INTO P_MARKET(ID, Name, InvoiceTitle,DutyParagraph) VALUES(@ID,@Name,@InvoiceTitle,@DutyParagraph) ";
                var sqlSelectAll = "SELECT ID, Name, InvoiceTitle,DutyParagraph FROM P_MARKET ";
                var sqlUpdate = "UPDATE P_MARKET SET InvoiceTitle = @InvoiceTitle,DutyParagraph=@DutyParagraph WHERE (Name = @Name) ";
                var sqlServerTemplNonHT = sqlServerTemplFactoryNonHT.CreateDbTemplate();
                using (var conn = sqlServerTemplNonHT.GetSqlConnection())
                {
                    conn.Open();
                    //using (var trans = conn.BeginTransaction())
                    //{
                    var allMarkets = sqlServerTemplNonHT.Load<P_MARKET>(sqlSelectAll, new SqlParameter[] { });
                    foreach (var _itm in entity)
                    {
                        if (_itm.Name == "RD")
                        {
                            _itm.Name = "R&D";
                        }
                        var matchItm = allMarkets.SingleOrDefault(w => w.Name == _itm.Name);

                        //已存在该ID配置项 
                        if (matchItm != null)
                        {
                            // 业务数据相同则不更新 
                            if (matchItm.InvoiceTitle == _itm.InvoiceTitle && matchItm.DutyParagraph==_itm.DutyParagraph)
                            {
                                upCnt++;
                            }
                            else
                            {
                                if (sqlServerTemplNonHT.ExecuteNonQuery(sqlUpdate, new SqlParameter[] {
                                        SqlParameterFactory.GetSqlParameter("@Name", matchItm.Name),
                                        SqlParameterFactory.GetSqlParameter("@InvoiceTitle", _itm.InvoiceTitle),
                                        SqlParameterFactory.GetSqlParameter("@DutyParagraph", _itm.DutyParagraph)
                                        }) == 1)
                                {
                                    upCnt++;
                                }
                                else
                                {
                                    unSuccessData.Add(_itm);
                                }
                            }
                        }
                        else
                        {
                            if (sqlServerTemplNonHT.ExecuteNonQuery(sqlInsert, new SqlParameter[] {
                                        SqlParameterFactory.GetSqlParameter("@Name", _itm.Name),
                                        SqlParameterFactory.GetSqlParameter("@InvoiceTitle", _itm.InvoiceTitle),
                                        SqlParameterFactory.GetSqlParameter("@DutyParagraph", _itm.DutyParagraph),
                                        SqlParameterFactory.GetSqlParameter("@ID", Guid.NewGuid()),
                                        }) == 1)
                            {
                                upCnt++;
                            }
                            else
                            {
                                unSuccessData.Add(_itm);
                            }
                        }
                    }
                    //trans.Commit();
                    //}
                }
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    //using (var trans = conn.BeginTransaction())
                    //{
                    var allMarkets = sqlServerTemplate.Load<P_MARKET>(sqlSelectAll, new SqlParameter[] { });
                    foreach (var _itm in entity)
                    {
                        if (_itm.Name == "R&D")
                        {
                            continue;
                        }
                        var matchItm = allMarkets.SingleOrDefault(w => w.Name == _itm.Name);

                        //已存在该ID配置项 
                        if (matchItm != null)
                        {
                            // 业务数据相同则不更新 
                            if (matchItm.InvoiceTitle == _itm.InvoiceTitle && matchItm.DutyParagraph == _itm.DutyParagraph)
                            {
                                upCnt++;
                            }
                            else
                            {
                                if (sqlServerTemplate.ExecuteNonQuery(sqlUpdate, new SqlParameter[] {
                                        SqlParameterFactory.GetSqlParameter("@Name", matchItm.Name),
                                        SqlParameterFactory.GetSqlParameter("@InvoiceTitle", _itm.InvoiceTitle),
                                        SqlParameterFactory.GetSqlParameter("@DutyParagraph", _itm.DutyParagraph)
                                        }) == 1)
                                {
                                    upCnt++;
                                }
                                else
                                {
                                    unSuccessData.Add(_itm);
                                }
                            }
                        }
                        else
                        {
                            if (sqlServerTemplate.ExecuteNonQuery(sqlInsert, new SqlParameter[] {
                                        SqlParameterFactory.GetSqlParameter("@Name", _itm.Name),
                                        SqlParameterFactory.GetSqlParameter("@InvoiceTitle", _itm.InvoiceTitle),
                                        SqlParameterFactory.GetSqlParameter("@DutyParagraph", _itm.DutyParagraph),
                                        SqlParameterFactory.GetSqlParameter("@ID", Guid.NewGuid()),
                                        }) == 1)
                            {
                                upCnt++;
                            }
                            else
                            {
                                unSuccessData.Add(_itm);
                            }
                        }
                    }
                    //trans.Commit();
                    //}
                }
            }
            return upCnt;
        }
    }
}
