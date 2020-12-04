using MealAdmin.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XFramework.XDataBase;
using XFramework.XInject.Attributes;
using XFramework.XDataBase.SqlServer;
using System.Data.SqlClient;

namespace MealAdmin.Dao
{
    public class BUManagementDao : IBUManagementDao
    {
        [Bean("sqlServerTemplFactory")]
        public DbTemplateFactory sqlServerTemplFactory { get; set; }
        [Bean("sqlServerTemplFactoryNonHT")]
        public DbTemplateFactory sqlServerTemplFactoryNonHT { get; set; }

        public List<P_BUINFO> LoadBUInfo()
        {
            List<P_BUINFO> rtnData;

            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<P_BUINFO>("SELECT * FROM P_BUINFO ORDER BY BUName ",
                    new SqlParameter[] {
                        
                    });
            }
            return rtnData;
        }

        public List<P_TAINFOView> LoadTAInfo()
        {
            List<P_TAINFOView> rtnData;

            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<P_TAINFOView>(
                    "SELECT TA.* ,(CASE WHEN BU.BUName IS NULL THEN '' ELSE BU.BUName END) as BUName FROM P_TAINFO TA LEFT JOIN P_BUINFO BU ON TA.BUID = BU.ID ORDER BY TerritoryTA ",
                    new SqlParameter[] {

                    });
            }
            return rtnData;
        }

        public P_BUINFO GetBUInfoByID(Guid ID)
        {
            P_BUINFO rtnData;

            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Find<P_BUINFO>("SELECT * FROM P_BUINFO WHERE ID = @ID ORDER BY BUName ",
                    new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@ID", ID)
                    });
            }
            return rtnData;
        }

        public int DelBUInfoByID(Guid ID)
        {
            int rtnData;

            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnDataUpdate = sqlServerTemplate.ExecuteNonQuery("UPDATE P_TAINFO SET BUID=NULL WHERE BUID = @ID ",
                    new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@ID", ID)
                    });

                rtnData = sqlServerTemplate.ExecuteNonQuery("DELETE FROM P_BUINFO WHERE ID = @ID ",
                    new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@ID", ID)
                    });
            }
            return rtnData;
        }

        public int AddBUInfo(string BUName, string BUHead, string BUHeadMudid)
        {
            int rtnData;

            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.ExecuteNonQuery("INSERT INTO P_BUINFO VALUES(NEWID(),@BUName,@BUHead,@BUHeadMudid,@CreateDate,@ModifyDate) ",
                    new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@BUName", BUName),
                        SqlParameterFactory.GetSqlParameter("@BUHead", BUHead),
                        SqlParameterFactory.GetSqlParameter("@BUHeadMudid", BUHeadMudid),
                        SqlParameterFactory.GetSqlParameter("@CreateDate", DateTime.Now),
                        SqlParameterFactory.GetSqlParameter("@ModifyDate", DateTime.Now),
                    });
            }
            return rtnData;
        }

        public int AddTAInfo()
        {
            int rtnData;

            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.ExecuteNonQuery("INSERT INTO P_TAINFO SELECT NEWID(),*,'','','',NULL,@CreateDate,@ModifyDate FROM (SELECT DISTINCT TERRITORY_TA FROM V_TERRITORY_TA WHERE TERRITORY_TA NOT IN (SELECT TERRITORYTA FROM P_TAINFO)) TA ",
                    new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@CreateDate", DateTime.Now),
                        SqlParameterFactory.GetSqlParameter("@ModifyDate", DateTime.Now)
                    });
            }
            return rtnData;
        }

        public int UpdateBUInfo(Guid ID, string BUName, string BUHead, string BUHeadMudid)
        {
            int rtnData;

            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.ExecuteNonQuery("UPDATE P_BUINFO SET BUName=@BUName,BUHead=@BUHead,BUHeadMudid=@BUHeadMudid,ModifyDate=@ModifyDate WHERE ID = @ID ",
                    new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@BUName", BUName),
                        SqlParameterFactory.GetSqlParameter("@BUHead", BUHead),
                        SqlParameterFactory.GetSqlParameter("@BUHeadMudid", BUHeadMudid),
                        SqlParameterFactory.GetSqlParameter("@ModifyDate", DateTime.Now),
                        SqlParameterFactory.GetSqlParameter("@ID", ID)
                    });
            }
            return rtnData;
        }

        public P_TAINFO GetTAInfoByID(Guid ID)
        {
            P_TAINFO rtnData;

            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Find<P_TAINFO>("SELECT * FROM P_TAINFO WHERE ID = @ID ORDER BY TerritoryTA ",
                    new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@ID", ID)
                    });
            }
            return rtnData;
        }

        public int DelTAInfoByID(Guid ID)
        {
            int rtnData;

            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.ExecuteNonQuery("DELETE FROM P_TAINFO WHERE ID = @ID ",
                    new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@ID", ID)
                    });
            }
            return rtnData;
        }

        public int UpdateTAInfo(Guid ID, string TerritoryTA, string TerritoryHead, string TerritoryHeadName, string BUID)
        {
            int rtnData;

            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.ExecuteNonQuery("UPDATE P_TAINFO SET TerritoryTA=@TerritoryTA,TerritoryHead=@TerritoryHead,TerritoryHeadName=@TerritoryHeadName,BUID=@BUID WHERE ID = @ID ",
                    new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@TerritoryTA", TerritoryTA),
                        SqlParameterFactory.GetSqlParameter("@TerritoryHead", TerritoryHead),
                        SqlParameterFactory.GetSqlParameter("@TerritoryHeadName", TerritoryHeadName),
                        SqlParameterFactory.GetSqlParameter("@BUID", Guid.Parse(BUID)),
                        SqlParameterFactory.GetSqlParameter("@ModifyDate", DateTime.Now),
                        SqlParameterFactory.GetSqlParameter("@ID", ID)
                    });
            }
            return rtnData;
        }

        public P_BUINFO GetBUInfoByUserId(string UserId)
        {
            P_BUINFO rtnData;

            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Find<P_BUINFO>("SELECT * FROM P_BUINFO WHERE BUHeadMudid = @UserId ORDER BY BUName ",
                    new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@UserId", UserId)
                    });
            }
            return rtnData;
        }

        public P_TAINFO GetTAInfoByUserId(string UserId)
        {
            P_TAINFO rtnData;

            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Find<P_TAINFO>("SELECT * FROM P_TAINFO WHERE TerritoryHead = @UserId ORDER BY TerritoryTA ",
                    new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@UserId", UserId)
                    });
            }
            return rtnData;
        }

        public List<P_TAINFO> GetTAInfoByBUID(Guid BUID)
        {
            List<P_TAINFO> rtnData;

            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<P_TAINFO>("SELECT * FROM P_TAINFO WHERE BUID = @BUID ORDER BY TerritoryTA ",
                    new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@BUID", BUID)
                    });
            }
            return rtnData;
        }
    }
}
