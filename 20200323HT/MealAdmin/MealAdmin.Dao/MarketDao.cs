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
    public class MarketDao : IMarketDao
    {
        [Bean("sqlServerTemplFactory")]
        public DbTemplateFactory sqlServerTemplFactory { get; set; }
        [Bean("sqlServerTemplFactoryNonHT")]
        public DbTemplateFactory sqlServerTemplFactoryNonHT { get; set; }

        #region 载入全部发票抬头信息
        /// <summary>
        /// 载入全部发票抬头信息
        /// </summary>
        /// <returns></returns>
        public List<P_MARKET> Load()
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<P_MARKET>("SELECT * FROM P_MARKET ");
                return list;
            }
        }
        #endregion


        #region 载入全部发票抬头信息
        /// <summary>
        /// 载入全部发票抬头信息
        /// </summary>
        /// <returns></returns>
        public List<P_TACode> LoadTAByUserId(string UserId)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<P_TACode>("SELECT NEWID() AS ID, TA_CODE AS Name,TA_CODE AS NameShow FROM V_TerritoryCode WHERE MUDID=@UserId ", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", UserId)
                    });
                return list;
            }
        }

        public List<P_TACode> LoadTACodeByMarketAndUser(string Market, string UserId)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<P_TACode>("SELECT NEWID() AS ID, TA_CODE AS Name,TA_CODE AS NameShow FROM V_TerritoryCode WHERE MUDID=@UserId AND Market=@Market ", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", UserId),
                        SqlParameterFactory.GetSqlParameter("@Market", Market)
                    });
                return list;
            }
        }

        public P_TACode LoadRDCode(string Market, string UserId, string TCode)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Find<P_TACode>("SELECT NEWID() AS ID, TERRITORY_RD AS Name,TERRITORY_RD AS NameShow FROM V_TerritoryCodeRD WHERE MUDID=@UserId AND TA_CODE=@TCode AND Market=@Market ", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", UserId),
                        SqlParameterFactory.GetSqlParameter("@TCode", TCode),
                        SqlParameterFactory.GetSqlParameter("@Market", Market)
                    });
                return list;
            }
        }

        /// <summary>
        /// 载入全部发票抬头信息
        /// </summary>
        /// <returns></returns>
        public List<P_MARKET> LoadMarketByUserId(string UserId)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<P_MARKET>("SELECT NEWID() AS ID, * FROM (SELECT distinct Market AS Name,M.NameShow AS NameShow FROM V_TerritoryCode TC join P_MARKET M ON TC.Market = M.Name WHERE MUDID=@UserId) A ", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", UserId)
                    });
                return list;
            }
        }
        #endregion

        #region 根据marketName查询TA
        /// <summary>
        /// 根据marketName查询TA
        /// </summary>
        /// <param name="marketName"></param>
        /// <returns></returns>
        public List<P_TA> LoadTAByMarketName(string marketName)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<P_TA>("SELECT NEWID() AS ID,Market as MarketName,TERRITORY_TA as Name  FROM (SELECT DISTINCT Market,TERRITORY_TA FROM Territory_Hospital WHERE Market=@MarketName AND TERRITORY_TA != '') AS A", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@MarketName", marketName)
                    });
                return list;
            }
        }
        #endregion

        #region 根据marketName查询TA
        /// <summary>
        /// 根据marketName查询TA
        /// </summary>
        /// <param name="marketName"></param>
        /// <returns></returns>
        public List<P_TA> LoadTAByMarketUser(string marketName, string UserID)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<P_TA>("SELECT NEWID() AS ID,Market as MarketName, TERRITORY_TA as Name FROM V_TerritoryCodeTA WHERE Market=@MarketName AND MUDID=@UserID", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@MarketName", marketName),
                        SqlParameterFactory.GetSqlParameter("@UserID", UserID)
                    });
                return list;
            }
        }

        public List<P_TA> LoadTAByMarketUserId(string marketName, string UserID, string TerritoryCode)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<P_TA>("SELECT NEWID() AS ID,Market as MarketName, TERRITORY_TA as Name FROM V_TerritoryCodeTA WHERE Market=@MarketName AND MUDID=@UserID AND TA_CODE=@TerritoryCode", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@MarketName", marketName),
                        SqlParameterFactory.GetSqlParameter("@UserID", UserID),
                        SqlParameterFactory.GetSqlParameter("@TerritoryCode", TerritoryCode)
                    });
                return list;
            }
        }
        #endregion

        #region 根据TA查找成本中心
        /// <summary>
        /// 根据TA查找成本中心
        /// </summary>
        /// <param name="ta">TA</param>
        /// <returns></returns>
        public List<D_COSTCENTERSELECT> FindCostCenterByTA(string market, string ta)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<D_COSTCENTERSELECT>("SELECT NEWID() AS ID, * FROM (SELECT DISTINCT TERRITORY_RM AS Name FROM V_TerritoryCodeTA WHERE Market = @Market AND (TERRITORY_TA = @TA OR @TA = '')) AS A", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@Market", market),
                        SqlParameterFactory.GetSqlParameter("@TA", ta)
                    });
                return list;
            }
        }
        #endregion

        #region 根据TA查找成本中心
        /// <summary>
        /// 根据TA查找成本中心
        /// </summary>
        /// <param name="ta">TA</param>
        /// <returns></returns>
        public List<D_COSTCENTERSELECT> LoadCostCenterByMarketUserID(string market, string UserID, string TerritoryCode)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<D_COSTCENTERSELECT>("SELECT NEWID() as ID,TERRITORY_RM as Name FROM V_TerritoryCodeTA WHERE Market=@market and Mudid=@UserID AND TA_CODE = @TerritoryCode", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@market", market),
                        SqlParameterFactory.GetSqlParameter("@UserID", UserID),
                        SqlParameterFactory.GetSqlParameter("@TerritoryCode", TerritoryCode)
                    });
                return list;
            }
        }
        #endregion

        #region 根据名字查找market信息
        /// <summary>
        /// 根据名字查找market信息
        /// </summary>
        /// <param name="marketName"></param>
        /// <returns></returns>
        public P_MARKET FindByMarket(string marketName)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var res = sqlServerTemplate.Find<P_MARKET>("SELECT * FROM P_MARKET WHERE Name=@Name", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@Name", marketName)
                    });
                return res;
            }
        }
        #endregion 
    }
}
