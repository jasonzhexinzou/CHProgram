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
using System.Data;
using XFramework.XUtil;
using IamPortal.AppLogin;
using System.Web;
using MealAdmin.Util;
using MealAdmin.Entity.View;

namespace MealAdmin.Dao
{
    public class HospitalDao : IHospitalDao
    {
        [Bean("sqlServerTemplFactory")]
        public DbTemplateFactory sqlServerTemplFactory { get; set; }
        [Bean("sqlServerTemplFactoryNonHT")]
        public DbTemplateFactory sqlServerTemplFactoryNonHT { get; set; }

        private string selectSql = "SELECT P_HOSPITAL.ID, P_HOSPITAL.GskHospital,P_HOSPITAL.OldGskHospital, P_HOSPITAL.Name,P_HOSPITAL.OldName, P_HOSPITAL.FirstLetters, P_HOSPITAL.Address, P_HOSPITAL.Type, P_HOSPITAL.CreateDate, P_CITY.Name AS CityName, P_PROVINCE.Name AS ProvinceName,P_HOSPITAL.RelateUserList, P_HOSPITAL.MainAddress, P_HOSPITAL.HospitalCode "
                                + "FROM P_HOSPITAL LEFT OUTER JOIN P_CITY ON P_HOSPITAL.CityId = P_CITY.ID and P_HOSPITAL.Type = P_CITY.Type  LEFT OUTER JOIN P_PROVINCE ON P_CITY.ProvinceId = P_PROVINCE.ID and P_CITY.Type=P_PROVINCE.Type "
                                + "WHERE (P_HOSPITAL.GskHospital LIKE '%' + @GskHospital + '%') AND (P_HOSPITAL.Name LIKE '%' + @Name + '%') AND ((P_HOSPITAL.Type = @Type) OR (@Type = '')) AND (P_HOSPITAL.[IsDelete] = 0) ";
        private string selectOrderBySql = " ORDER BY P_HOSPITAL.GskHospital , P_HOSPITAL.HospitalCode , P_HOSPITAL.Name  ";


        private string selectTASql = "SELECT NEWID() AS ID,HOS.HospitalCode,P.Name Province,C.Name City,HOS.Name HospitalName,HOS.Address,HOS.MainAddress,HOS.Type Market,HOS.Latitude,HOS.Longitude,HD.District,HD.DistrictCode,HD.CustomerType,TH.MUD_ID_MR,TH.TERRITORY_MR,TH.MUD_ID_DM,TH.TERRITORY_DM,TH.MUD_ID_RM,TH.TERRITORY_RM,TH.MUD_ID_RD,TH.TERRITORY_RD,TH.MUD_ID_TA,TH.TERRITORY_TA FROM P_HOSPITAL HOS JOIN P_HOSPITAL_DETAIL HD ON HOS.HospitalCode = HD.GskHospital JOIN Territory_Hospital TH ON HOS.HospitalCode = TH.HospitalCode JOIN P_PROVINCE P ON P.ID = HOS.ProvinceId JOIN P_CITY C ON C.ID = HOS.CityId "
            + "WHERE (HOS.HospitalCode LIKE '%' + @GskHospital + '%') AND (HOS.Name LIKE '%' + @Name + '%') AND ((TH.MUD_ID_MR LIKE '%' + @MUDID + '%') OR (TH.MUD_ID_DM LIKE '%' + @MUDID + '%') OR (TH.MUD_ID_RM LIKE '%' + @MUDID + '%') OR (TH.MUD_ID_RD LIKE '%' + @MUDID + '%')) AND ((TH.TERRITORY_MR LIKE '%' + @TACODE + '%') OR (TH.TERRITORY_DM LIKE '%' + @TACODE + '%') OR (TH.TERRITORY_RM LIKE '%' + @TACODE + '%') OR (TH.TERRITORY_RD LIKE '%' + @TACODE + '%')) AND ((HOS.Type = @Type) OR (@Type = '')) AND ((TH.TERRITORY_TA = @TA) OR (@TA = '')) AND (HOS.[IsDelete] = 0) ";

        private string selectTAOrderBySql = " ORDER BY HOS.HospitalCode ,HOS.Name  ";


        public List<P_TERRITORY> LoadTA(string srh_GskHospital, string srh_HospitalName, string srh_MUDID, string srh_TerritoryCode, string srh_HospitalMarket, string srh_HospitalTA)
        {
            List<P_TERRITORY> rtnData;
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                //选择全部Type
                rtnData = sqlServerTemplate.Load<P_TERRITORY>(selectTASql + selectTAOrderBySql,
                new SqlParameter[]
                {
                                        SqlParameterFactory.GetSqlParameter("@GskHospital", srh_GskHospital),
                                        SqlParameterFactory.GetSqlParameter("@Name", srh_HospitalName),
                                        SqlParameterFactory.GetSqlParameter("@MUDID", srh_MUDID),
                                        SqlParameterFactory.GetSqlParameter("@TACODE", srh_TerritoryCode),
                                        SqlParameterFactory.GetSqlParameter("@Type", srh_HospitalMarket),
                                        SqlParameterFactory.GetSqlParameter("@TA", srh_HospitalTA)
                });
            }
            return rtnData;
        }


        public List<P_HOSPITAL_DATA_VIEW> Load(string srh_GskHospital, string srh_HospitalName, string srh_HospitalMarket, string srh_OHHospitalType)
        {
            List<P_HOSPITAL_DATA_VIEW> rtnData;
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                //选择全部Type
                if (srh_OHHospitalType == "")
                {
                    rtnData = sqlServerTemplate.Load<P_HOSPITAL_DATA_VIEW>(selectallDataSql + selectOrderByHData,
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@GskHospital", srh_GskHospital),
                        SqlParameterFactory.GetSqlParameter("@Name", srh_HospitalName),
                        SqlParameterFactory.GetSqlParameter("@Type", srh_HospitalMarket)
                    });
                }
                //选择院内
                else if (srh_OHHospitalType == "院内")
                {
                    rtnData = sqlServerTemplate.Load<P_HOSPITAL_DATA_VIEW>(selectHDataSql + selectOrderByHData,
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@GskHospital", srh_GskHospital),
                        SqlParameterFactory.GetSqlParameter("@Name", srh_HospitalName),
                        SqlParameterFactory.GetSqlParameter("@Type", srh_HospitalMarket)
                    });
                }
                //选择院外
                else
                {
                    rtnData = sqlServerTemplate.Load<P_HOSPITAL_DATA_VIEW>(selectOHDataSql + selectOrderByHData,
                    new SqlParameter[]
                    {
                         SqlParameterFactory.GetSqlParameter("@GskHospital", srh_GskHospital),
                         SqlParameterFactory.GetSqlParameter("@Name", srh_HospitalName),
                         SqlParameterFactory.GetSqlParameter("@Type", srh_HospitalMarket)
                    });
                }
            }
            return rtnData;
        }
        //查询院内数据
        private string selectHSql = "SELECT P_HOSPITAL.ID, P_HOSPITAL.GskHospital,P_HOSPITAL.OldGskHospital, P_HOSPITAL.Name,P_HOSPITAL.OldName, P_HOSPITAL.FirstLetters, P_HOSPITAL.Address, P_HOSPITAL.Type, P_HOSPITAL.CreateDate, P_CITY.Name AS CityName, P_PROVINCE.Name AS ProvinceName,P_HOSPITAL.RelateUserList, P_HOSPITAL.MainAddress, P_HOSPITAL.HospitalCode "
                                + "FROM P_HOSPITAL LEFT OUTER JOIN P_CITY ON P_HOSPITAL.CityId = P_CITY.ID and P_HOSPITAL.Type = P_CITY.Type  LEFT OUTER JOIN P_PROVINCE ON P_CITY.ProvinceId = P_PROVINCE.ID and P_CITY.Type=P_PROVINCE.Type "
                                + "WHERE (P_HOSPITAL.GskHospital LIKE '%' + @GskHospital + '%') AND (P_HOSPITAL.Name LIKE '%' + @Name + '%') AND ((P_HOSPITAL.Type = @Type) OR (@Type = '')) AND (P_HOSPITAL.[External] = 0) AND (P_HOSPITAL.[IsDelete] = 0) ";
        private string selectOrderByHSql = " ORDER BY P_HOSPITAL.GskHospital , P_HOSPITAL.HospitalCode , P_HOSPITAL.Name ";
        //查询院外数据
        private string selectOHSql = "SELECT P_HOSPITAL.ID, P_HOSPITAL.GskHospital,P_HOSPITAL.OldGskHospital, P_HOSPITAL.Name,P_HOSPITAL.OldName, P_HOSPITAL.FirstLetters, P_HOSPITAL.Address, P_HOSPITAL.Type, P_HOSPITAL.CreateDate, P_CITY.Name AS CityName, P_PROVINCE.Name AS ProvinceName,P_HOSPITAL.RelateUserList, P_HOSPITAL.MainAddress, P_HOSPITAL.HospitalCode "
                               + "FROM P_HOSPITAL LEFT OUTER JOIN P_CITY ON P_HOSPITAL.CityId = P_CITY.ID and P_HOSPITAL.Type = P_CITY.Type  LEFT OUTER JOIN P_PROVINCE ON P_CITY.ProvinceId = P_PROVINCE.ID and P_CITY.Type=P_PROVINCE.Type "
                               + "WHERE (P_HOSPITAL.GskHospital LIKE '%' + @GskHospital + '%') AND (P_HOSPITAL.Name LIKE '%' + @Name + '%') AND ((P_HOSPITAL.Type = @Type) OR (@Type = '')) AND (P_HOSPITAL.[External] = 1) AND (P_HOSPITAL.[IsDelete] = 0) ";
        private string selectOrderByOHSql = " ORDER BY P_HOSPITAL.GskHospital , P_HOSPITAL.Name ";

        public List<P_HOSPITAL_DATA_VIEW> LoadPage(string srh_GskHospital, string srh_HospitalName, string srh_HospitalMarket, string srh_OHHospitalType, int rows, int page, out int total)
        {
            List<P_HOSPITAL_DATA_VIEW> rtnData;
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                //选择全部Type
                if (srh_OHHospitalType == "")
                {
                    rtnData = sqlServerTemplate.LoadPages<P_HOSPITAL_DATA_VIEW>(rows, page, out total,
                    selectallDataSql,
                    selectOrderByHData, new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@GskHospital", srh_GskHospital),
                        SqlParameterFactory.GetSqlParameter("@Name", srh_HospitalName),
                        SqlParameterFactory.GetSqlParameter("@Type", srh_HospitalMarket)
                    });
                }
                //选择院内
                else if (srh_OHHospitalType == "院内")
                {
                    rtnData = sqlServerTemplate.LoadPages<P_HOSPITAL_DATA_VIEW>(rows, page, out total,
                    selectHDataSql,
                    selectOrderByHData, new SqlParameter[]
                    {
                         SqlParameterFactory.GetSqlParameter("@GskHospital", srh_GskHospital),
                         SqlParameterFactory.GetSqlParameter("@Name", srh_HospitalName),
                         SqlParameterFactory.GetSqlParameter("@Type", srh_HospitalMarket)
                    });
                }
                //选择院外
                else
                {
                    rtnData = sqlServerTemplate.LoadPages<P_HOSPITAL_DATA_VIEW>(rows, page, out total,
                    selectOHDataSql,
                    selectOrderByHData, new SqlParameter[]
                    {
                         SqlParameterFactory.GetSqlParameter("@GskHospital", srh_GskHospital),
                         SqlParameterFactory.GetSqlParameter("@Name", srh_HospitalName),
                         SqlParameterFactory.GetSqlParameter("@Type", srh_HospitalMarket)
                    });
                }
            }
            return rtnData;
        }


        public List<P_TERRITORY> LoadTAPage(string srh_GskHospital, string srh_HospitalName, string srh_MUDID, string srh_TerritoryCode, string srh_HospitalMarket, string srh_HospitalTA, int rows, int page, out int total)
        {
            List<P_TERRITORY> rtnData;
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                //选择全部Type
                rtnData = sqlServerTemplate.LoadPages<P_TERRITORY>(rows, page, out total,
                    selectTASql,
                    selectTAOrderBySql, new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@GskHospital", srh_GskHospital),
                        SqlParameterFactory.GetSqlParameter("@Name", srh_HospitalName),
                        SqlParameterFactory.GetSqlParameter("@MUDID", srh_MUDID),
                        SqlParameterFactory.GetSqlParameter("@TACODE", srh_TerritoryCode),
                        SqlParameterFactory.GetSqlParameter("@Type", srh_HospitalMarket),
                        SqlParameterFactory.GetSqlParameter("@TA", srh_HospitalTA)
                    });
            }
            return rtnData;
        }

        #region 导出医院数据

        //导出数据
        private string selectHDataSql = "SELECT * "
                              + "FROM V_HospitalDataReport "
                              + "WHERE (GskHospital LIKE '%' + @GskHospital + '%') AND ([Name] LIKE '%' + @Name + '%') AND (([Type] = @Type) OR (@Type = '')) AND ([External] = 0) ";
        private string selectOrderByHData = " ORDER BY [External],GskHospital,HospitalCode,[Name]  ";
        private string selectOHDataSql = "SELECT * "
                              + "FROM V_HospitalDataReport "
                              + "WHERE (GskHospital LIKE '%' + @GskHospital + '%') AND ([Name] LIKE '%' + @Name + '%') AND (([Type] = @Type) OR (@Type = '')) AND ([External] = 1) ";
        private string selectallDataSql = "SELECT * "
                              + "FROM V_HospitalDataReport "
                              + "WHERE (GskHospital LIKE '%' + @GskHospital + '%') AND ([Name] LIKE '%' + @Name + '%') AND (([Type] = @Type) OR (@Type = '')) ";

        public List<P_HOSPITAL_DATA_VIEW> LoadHData(string srh_GskHospital, string srh_HospitalName, string srh_HospitalMarket, string srh_OHHospitalType)
        {
            List<P_HOSPITAL_DATA_VIEW> rtnData;
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();

                //选择全部Type
                if (srh_OHHospitalType == "")
                {
                    rtnData = sqlServerTemplate.Load<P_HOSPITAL_DATA_VIEW>(selectallDataSql + selectOrderByHData,
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@GskHospital", srh_GskHospital),
                        SqlParameterFactory.GetSqlParameter("@Name", srh_HospitalName),
                        SqlParameterFactory.GetSqlParameter("@Type", srh_HospitalMarket)
                    });
                }
                //选择院内
                else if (srh_OHHospitalType == "院内")
                {
                    rtnData = sqlServerTemplate.Load<P_HOSPITAL_DATA_VIEW>(selectHDataSql + selectOrderByHData,
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@GskHospital", srh_GskHospital),
                        SqlParameterFactory.GetSqlParameter("@Name", srh_HospitalName),
                        SqlParameterFactory.GetSqlParameter("@Type", srh_HospitalMarket)
                    });
                }
                //选择院外
                else
                {
                    rtnData = sqlServerTemplate.Load<P_HOSPITAL_DATA_VIEW>(selectOHDataSql + selectOrderByHData,
                    new SqlParameter[]
                    {
                         SqlParameterFactory.GetSqlParameter("@GskHospital", srh_GskHospital),
                         SqlParameterFactory.GetSqlParameter("@Name", srh_HospitalName),
                         SqlParameterFactory.GetSqlParameter("@Type", srh_HospitalMarket)
                    });
                }

            }
            return rtnData;
        }
        #endregion

        #region 从Territory_Hospital根据Market查询[TERRITORY_TA]
        public List<Territory_Hospital> LoadTERRITORY_TAByMarket(string Market)
        {
            List<Territory_Hospital> rtnData;

            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<Territory_Hospital>("select top 100 b.* from(SELECT TERRITORY_TA,Market FROM [Territory_Hospital] "
                    + " group by Market, TERRITORY_TA) as b where b.Market = @Market ORDER BY b.TERRITORY_TA  ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@Market", Market)
                    });
            }
            return rtnData;
        }
        #endregion
        public P_HOSPITAL GetHospitalByID(int HospitalID)
        {
            P_HOSPITAL rtnData;

            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Find<P_HOSPITAL>(
                    "SELECT * FROM P_HOSPITAL WHERE (ID = @ID) ",
                    new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@ID", HospitalID)
                    });
            }
            return rtnData;
        }

        public P_HOSPITAL GetHospitalByGSKHospital(string GSKHospital)
        {
            P_HOSPITAL rtnData;

            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Find<P_HOSPITAL>(
                    "SELECT * FROM P_HOSPITAL WHERE (GskHospital = @GskHospital) ",
                    new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@GskHospital", GSKHospital)
                    });
            }
            return rtnData;
        }

        public List<P_HOSPITAL_ADDR> LoadHospitalAddr(int HospitalID)
        {
            List<P_HOSPITAL_ADDR> rtnData;

            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<P_HOSPITAL_ADDR>(
                    "SELECT ID, HospitalId, Address, CreateDate FROM P_HOSPITAL_ADDR WHERE (HospitalId = @HospitalId) ORDER BY CreateDate ",
                    new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@HospitalId", HospitalID)
                    });
            }
            return rtnData;
        }

        public bool DeleteHospitalAddr(Guid AddrID)
        {
            bool rtnVal;

            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            int updCnt;
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                updCnt = sqlServerTemplate.ExecuteNonQuery(
                    "DELETE FROM P_HOSPITAL_ADDR WHERE (ID = @ID)  ",
                    new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@ID", AddrID)
                    });
            }
            if (updCnt == 1)
            {
                rtnVal = true;
            }
            else
            {
                rtnVal = false;
            }
            return rtnVal;
        }

        public bool AddHospitalAddr(P_HOSPITAL_ADDR Data)
        {
            bool rtnVal;

            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            int updCnt;
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                updCnt = sqlServerTemplate.ExecuteNonQuery(
                    "INSERT INTO P_HOSPITAL_ADDR(ID, HospitalId, Address, CreateDate) VALUES (@ID, @HospitalId, @Address, @CreateDate) ",
                    new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@ID", Data.ID),
                        SqlParameterFactory.GetSqlParameter("@HospitalId", Data.HospitalId),
                        SqlParameterFactory.GetSqlParameter("@Address", Data.Address),
                        SqlParameterFactory.GetSqlParameter("@CreateDate", Data.CreateDate)
                    });
            }
            if (updCnt == 1)
            {
                rtnVal = true;
            }
            else
            {
                rtnVal = false;
            }

            return rtnVal;
        }

        private string _selectSql = "SELECT P_HOSPITAL.HospitalCode AS GskHospital, P_HOSPITAL.Name, P_HOSPITAL.Address, P_HOSPITAL.Latitude, P_HOSPITAL.Longitude, P_HOSPITAL.Type, P_HOSPITAL.[External], P_CITY.Name AS CityName, P_PROVINCE.Name AS ProvinceName "
                                + "FROM P_HOSPITAL LEFT OUTER JOIN P_CITY ON P_HOSPITAL.CityId = P_CITY.ID LEFT OUTER JOIN P_PROVINCE ON P_CITY.ProvinceId = P_PROVINCE.ID "
                                + "WHERE P_HOSPITAL.IsDelete = 0 AND ";
        private string _selectOrderBySql = " ORDER BY P_HOSPITAL.GskHospital , P_HOSPITAL.Name ";
        public List<P_HOSPITALINFO> LoadHospital(string channel)
        {
            List<P_HOSPITALINFO> dataList;
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                dataList = sqlServerTemplate.Load<P_HOSPITALINFO>(
                    _selectSql + channel + _selectOrderBySql,
                    new SqlParameter[] {

                    });
            }

            return dataList;

        }

        #region 根据Market删除医院
        /// <summary>
        /// 根据Market删除医院
        /// </summary>
        /// <param name="market"></param>
        /// <returns></returns>
        public int ClearByMarket(string market)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery(
                    "DELETE FROM P_HOSPITAL WHERE Type=@Type",
                    new SqlParameter[] { SqlParameterFactory.GetSqlParameter("@Type", market) });
            }
        }
        #endregion

        #region 根据Market删除省份
        /// <summary>
        /// 根据Market删除医院
        /// </summary>
        /// <param name="market"></param>
        /// <returns></returns>
        public int ClearProvinceByMarket(string market)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery(
                    "DELETE FROM P_PROVINCE WHERE Type=@Type",
                    new SqlParameter[] { SqlParameterFactory.GetSqlParameter("@Type", market) });
            }
        }
        #endregion

        #region 根据Market删除城市
        /// <summary>
        /// 根据Market删除医院
        /// </summary>
        /// <param name="market"></param>
        /// <returns></returns>
        public int ClearCityByMarket(string market)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery(
                    "DELETE FROM P_City WHERE Type=@Type",
                    new SqlParameter[] { SqlParameterFactory.GetSqlParameter("@Type", market) });
            }
        }
        #endregion

        #region 导入医院
        /// <summary>
        /// 导入医院
        /// </summary>
        /// <param name="list"></param>
        /// <param name="fails"></param>
        /// <returns></returns>
        public int Import(List<P_HOSPITAL> list, string MarketString, ref List<P_HOSPITAL> fails)
        {
            #region 导入医院
            var sqlServerTemplNonHT = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            var sqlServerTemplHT = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplNonHT.GetSqlConnection() as SqlConnection)
            {
                conn.Open();

                // 判断是否重复的医院代码
                List<string> listOtherHospitalCode = null;

                if (list[0].External == 0)
                {
                    // 内部会议
                    listOtherHospitalCode = sqlServerTemplNonHT.Load<P_HOSPITAL>(
                        "SELECT HospitalCode FROM P_HOSPITAL WHERE Type<>@Type OR [External]<>@External ",
                        new SqlParameter[]
                        {
                            SqlParameterFactory.GetSqlParameter("@Type", list[0].Type),
                            SqlParameterFactory.GetSqlParameter("@External", list[0].External),
                        }).Select(a => a.HospitalCode).ToList();
                }
                else
                {
                    // 外部会议
                    listOtherHospitalCode = sqlServerTemplNonHT.Load<P_HOSPITAL>(
                        "SELECT HospitalCode FROM P_HOSPITAL WHERE [External]<>@External ",
                        new SqlParameter[]
                        {
                            SqlParameterFactory.GetSqlParameter("@External", list[0].External),
                        }).Select(a => a.HospitalCode).ToList(); ;
                }

                if (list.Count(a => listOtherHospitalCode.Contains(a.HospitalCode)) > 0)
                {
                    // 发现医院代码重复数据
                    var _list = list.Where(a => listOtherHospitalCode.Contains(a.HospitalCode)).ToList();
                    fails.AddRange(_list);
                    return 0;
                }

                // 通过校验，删除旧数据，放入新数据
                var tran = conn.BeginTransaction();
                if (list[0].External == 0)
                {
                    // 内部会议
                    SqlCommand commandDelete = new SqlCommand("DELETE FROM P_HOSPITAL WHERE Type=@Type AND [External]=0 ", conn);
                    commandDelete.Transaction = tran;
                    commandDelete.Parameters.AddRange(new SqlParameter[]
                        {
                            SqlParameterFactory.GetSqlParameter("@Type", list[0].Type)
                        });
                    commandDelete.ExecuteNonQuery();
                }
                else
                {
                    // 外部会议
                    SqlCommand commandDelete = new SqlCommand("DELETE FROM P_HOSPITAL WHERE [External]=1 and Type in " + MarketString + " ", conn);
                    commandDelete.Transaction = tran;
                    commandDelete.Parameters.AddRange(new SqlParameter[]
                        {
                            SqlParameterFactory.GetSqlParameter("@Type", list[0].Type)
                        });
                    commandDelete.ExecuteNonQuery();
                }

                foreach (var item in list)
                {
                    SqlCommand commandAdd = new SqlCommand(
                        "INSERT INTO [P_HOSPITAL] ([CityId] ,[GskHospital] ,[Name] ,[FirstLetters] ,[Address] ,[Latitude] ,[Longitude] ,[Type] ,[External] ,[CreateDate] ,[ProvinceId] ,[IsXMS] ,[IsBDS] ,[IsMT] ,[IsDelete] ,[MainAddress] ,[HospitalCode] ) "
                        + " VALUES (@CityId ,@GskHospital,@Name,@FirstLetters ,@Address ,@Latitude ,@Longitude ,@Type ,@External ,@CreateDate ,@ProvinceId ,@IsXMS ,@IsBDS ,@IsMT ,@IsDelete, @MainAddress, @HospitalCode ) ",
                        conn);
                    commandAdd.Transaction = tran;
                    commandAdd.Parameters.AddRange(
                        new SqlParameter[]
                        {
                            SqlParameterFactory.GetSqlParameter("@CityId", item.CityId),
                            SqlParameterFactory.GetSqlParameter("@GskHospital", item.GskHospital),
                            SqlParameterFactory.GetSqlParameter("@Name", item.Name),
                            SqlParameterFactory.GetSqlParameter("@FirstLetters", item.FirstLetters),
                            SqlParameterFactory.GetSqlParameter("@Address", item.Address),
                            SqlParameterFactory.GetSqlParameter("@Latitude", item.Latitude),
                            SqlParameterFactory.GetSqlParameter("@Longitude", item.Longitude),
                            SqlParameterFactory.GetSqlParameter("@Type", item.Type),
                            SqlParameterFactory.GetSqlParameter("@External", item.External),
                            SqlParameterFactory.GetSqlParameter("@CreateDate", item.CreateDate),
                            SqlParameterFactory.GetSqlParameter("@ProvinceId", item.ProvinceId),
                            SqlParameterFactory.GetSqlParameter("@IsXMS", item.IsXMS),
                            SqlParameterFactory.GetSqlParameter("@IsBDS", item.IsBDS),
                            SqlParameterFactory.GetSqlParameter("@IsMT", item.IsMT),
                            SqlParameterFactory.GetSqlParameter("@IsDelete", item.IsDelete),
                            SqlParameterFactory.GetSqlParameter("@MainAddress", item.MainAddress),
                            SqlParameterFactory.GetSqlParameter("@HospitalCode", item.HospitalCode)
                        }
                    );
                    commandAdd.ExecuteNonQuery();
                }
                tran.Commit();
            }
            //using (var connHT = sqlServerTemplHT.GetSqlConnection() as SqlConnection)
            //{
            //    connHT.Open();
            //    var tran = connHT.BeginTransaction();
            //    foreach (var item in list)
            //    {
            //        SqlCommand commandUpd = new SqlCommand(
            //    "Update [P_PreApproval] set HospitalCode=@HospitalCode,HospitalAddress=@HospitalAddress,HospitalName=@HospitalName"
            //    + " where HospitalCode=@OldGskHospital and IsFinished=0 and  MeetingDate>@TimeNow",
            //    connHT);
            //        commandUpd.Transaction = tran;
            //        commandUpd.Parameters.AddRange(
            //            new SqlParameter[]
            //        {
            //                SqlParameterFactory.GetSqlParameter("@OldGskHospital", item.OldGskHospital),
            //                SqlParameterFactory.GetSqlParameter("@HospitalCode", item.GskHospital),
            //                SqlParameterFactory.GetSqlParameter("@HospitalName", item.Name),
            //                SqlParameterFactory.GetSqlParameter("@HospitalAddress", item.Address),
            //                SqlParameterFactory.GetSqlParameter("@TimeNow", DateTime.Now),
            //        });
            //        commandUpd.ExecuteNonQuery();
            //    }
            //    tran.Commit();
            //}
            return 1;
            #endregion
        }
        #endregion

        #region 删除医院
        /// <summary>
        /// 删除医院
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public int Del(string[] ids)
        {
            #region 删除NonHT
            var sqlServerTemplNonHT = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplNonHT.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                return sqlServerTemplNonHT.ExecuteNonQuery(
                    $"DELETE FROM P_HOSPITAL WHERE HospitalCode IN ('{string.Join("','", ids)}')"
                    );
            }
            #endregion
        }
        #endregion

        #region 批量删除医院
        /// <summary>
        /// 批量删除医院
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public int DelHospitals(List<string> ids, out List<string> unSuccesUserId)
        {
            unSuccesUserId = new List<string>();
            int upCnt = 0;
            if (ids.Count > 0)
            {
                var sqlInsert = "DELETE FROM P_HOSPITAL WHERE GskHospital = @GskHospital ";
                //var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                var sqlServerTemplNonHT = sqlServerTemplFactoryNonHT.CreateDbTemplate();
                using (var conn = sqlServerTemplNonHT.GetSqlConnection())
                {
                    conn.Open();
                    foreach (var _id in ids)
                    {
                        // 成功删除 
                        if (sqlServerTemplNonHT.ExecuteNonQuery(sqlInsert, new SqlParameter[] {
                                        SqlParameterFactory.GetSqlParameter("@GskHospital", _id)
                                        }) == 1)
                        {
                            upCnt++;
                        }
                        // 删除失败
                        else
                        {
                            unSuccesUserId.Add(_id);
                        }
                    }
                }
                //using (var conn = sqlServerTemplate.GetSqlConnection())
                //{
                //    conn.Open();
                //    foreach (var _id in ids)
                //    {
                //        // 成功删除 
                //        if (sqlServerTemplate.ExecuteNonQuery(sqlInsert, new SqlParameter[] {
                //                        SqlParameterFactory.GetSqlParameter("@GskHospital", _id)
                //                        }) == 1)
                //        {
                //            upCnt++;
                //        }
                //        // 删除失败
                //        else
                //        {
                //            unSuccesUserId.Add(_id);
                //        }
                //    }
                //}
            }
            return upCnt;
        }
        #endregion

        #region 修改医院
        /// <summary>
        /// 修改医院
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Change(P_HOSPITAL entity)
        {
            var sqlServerTempl = sqlServerTemplFactory.CreateDbTemplate();
            var sqlServerTemplNonHT = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplNonHT.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                var list = sqlServerTemplNonHT.Load<P_HOSPITAL>("SELECT * FROM P_HOSPITAL WHERE ID<>@ID AND HospitalCode=@HospitalCode ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", entity.ID),
                        SqlParameterFactory.GetSqlParameter("@HospitalCode", entity.HospitalCode)
                    });
                if (list.Count > 0)
                {
                    // 医院代码重复
                    return 2;
                }

                sqlServerTemplNonHT.ExecuteNonQuery(
                $"UPDATE P_HOSPITAL SET CityId=@CityId, GskHospital=@GskHospital,Name=@Name,FirstLetters=@FirstLetters,Address=@Address, "
                + " Type=@Type, [External]=@External,ProvinceId=@ProvinceId, IsXMS=@IsXMS, IsBDS=@IsBDS, IsMT=@IsMT, IsDelete=@IsDelete, "
                + " MainAddress=@MainAddress, HospitalCode=@HospitalCode  WHERE ID=@ID ",
                new SqlParameter[]
                {
                        SqlParameterFactory.GetSqlParameter("@CityId", entity.CityId),
                        SqlParameterFactory.GetSqlParameter("@GskHospital", entity.GskHospital),
                        SqlParameterFactory.GetSqlParameter("@Name", entity.Name),
                        SqlParameterFactory.GetSqlParameter("@FirstLetters", entity.FirstLetters),
                        SqlParameterFactory.GetSqlParameter("@Address", entity.Address),
                        SqlParameterFactory.GetSqlParameter("@Type", entity.Type),
                        SqlParameterFactory.GetSqlParameter("@External", entity.External),
                        SqlParameterFactory.GetSqlParameter("@ProvinceId", entity.ProvinceId),
                        SqlParameterFactory.GetSqlParameter("@IsXMS", entity.IsXMS),
                        SqlParameterFactory.GetSqlParameter("@IsBDS", entity.IsBDS),
                        SqlParameterFactory.GetSqlParameter("@IsMT", entity.IsMT),
                        SqlParameterFactory.GetSqlParameter("@IsDelete", entity.IsDelete),
                        SqlParameterFactory.GetSqlParameter("@MainAddress", entity.MainAddress),
                        SqlParameterFactory.GetSqlParameter("@HospitalCode", entity.HospitalCode),
                        SqlParameterFactory.GetSqlParameter("@ID", entity.ID)
                });

                return 1;
            }
        }
        #endregion

        #region 更改医院地址
        /// <summary>
        /// 更改医院地址
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int ChanngeAddress(P_HOSPITAL entity)
        {
            var sqlServerTemplNonHT = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplNonHT.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                var res = 0;
                if (string.IsNullOrEmpty(entity.OldGskHospital))
                {
                    res = sqlServerTemplNonHT.ExecuteNonQuery(
                   $"UPDATE P_HOSPITAL SET Address=@Address WHERE ID=@ID ",
                   new SqlParameter[]
                   {
                        SqlParameterFactory.GetSqlParameter("@Address", entity.Address),
                        SqlParameterFactory.GetSqlParameter("@ID", entity.ID)
                   });
                }
                else
                {

                }

                return res;
            }
        }
        #endregion

        #region 新增医院
        /// <summary>
        /// 新增医院
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Add(P_HOSPITAL entity)
        {
            var sqlServerTemplNonHT = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplNonHT.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                return sqlServerTemplNonHT.ExecuteNonQuery(
                    "INSERT INTO [P_HOSPITAL] ([CityId] ,[GskHospital] ,[Name],[FirstLetters] ,[Address] ,[Latitude] ,[Longitude] ,[Type] ,[External] ,[CreateDate] ,[ProvinceId] ,[IsXMS] ,[IsBDS] ,[IsMT] ,[IsDelete],[MainAddress],[HospitalCode] ) "
                        + " VALUES (@CityId ,@GskHospital,@Name,@FirstLetters ,@Address ,@Latitude ,@Longitude ,@Type ,@External ,@CreateDate ,@ProvinceId ,@IsXMS ,@IsBDS ,@IsMT,@IsDelete,@MainAddress,@HospitalCode) ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@CityId", entity.CityId),
                        SqlParameterFactory.GetSqlParameter("@GskHospital", entity.GskHospital),
                        SqlParameterFactory.GetSqlParameter("@Name", entity.Name),
                        SqlParameterFactory.GetSqlParameter("@FirstLetters", entity.FirstLetters),
                        SqlParameterFactory.GetSqlParameter("@Address", entity.Address),
                        SqlParameterFactory.GetSqlParameter("@Latitude", entity.Latitude),
                        SqlParameterFactory.GetSqlParameter("@Longitude", entity.Longitude),
                        SqlParameterFactory.GetSqlParameter("@Type", entity.Type),
                        SqlParameterFactory.GetSqlParameter("@External", entity.External),
                        SqlParameterFactory.GetSqlParameter("@CreateDate", entity.CreateDate),
                        SqlParameterFactory.GetSqlParameter("@ProvinceId", entity.ProvinceId),
                        SqlParameterFactory.GetSqlParameter("@IsXMS", entity.IsXMS),
                        SqlParameterFactory.GetSqlParameter("@IsBDS", entity.IsBDS),
                        SqlParameterFactory.GetSqlParameter("@IsMT", entity.IsMT),
                        SqlParameterFactory.GetSqlParameter("@IsDelete", entity.IsDelete),
                        SqlParameterFactory.GetSqlParameter("@MainAddress", entity.MainAddress),
                        SqlParameterFactory.GetSqlParameter("@HospitalCode", entity.HospitalCode)
                    });
            }
        }
        #endregion
        public void deleteProvince()
        {
            var sqlServerTemplNonHT = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplNonHT.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                sqlServerTemplNonHT.ExecuteNonQuery("DELETE FROM P_PROVINCE "
                                                    + "WHERE ID in(SELECT pro.ID "
                                                    + "FROM P_PROVINCE pro left join P_HOSPITAL hos "
                                                    + "ON hos.ProvinceId = pro.ID AND hos.Type = pro.Type "
                                                    + "WHERE hos.ID is NULL)");
            }
        }
        public void deleteCity()
        {
            var sqlServerTemplNonHT = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplNonHT.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                sqlServerTemplNonHT.ExecuteNonQuery("DELETE FROM P_CITY "
                                                    + "WHERE ID in(SELECT city.ID  "
                                                    + "FROM P_CITY city left join P_HOSPITAL hos  "
                                                    + "ON hos.CityId = city.ID AND hos.Type = city.Type  "
                                                    + "WHERE hos.ID is NULL)");

            }
        }

        #region 根据医院Code查询医院数据
        /// <summary>
        /// 根据医院Code查询医院数据
        /// </summary>
        /// <param name="hospitalCode"></param>
        /// <returns></returns>
        public P_HOSPITAL FindByCode(string hospitalCode)
        {
            P_HOSPITAL rtnData;

            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Find<P_HOSPITAL>(
                    "SELECT * FROM P_HOSPITAL WHERE (HospitalCode = @HospitalCode and IsDelete = 0) ",
                    new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@HospitalCode", hospitalCode)
                    });
            }
            return rtnData;
        }
        #endregion

        #region 查询医院覆盖变化情况
        /// <summary>
        /// 查询医院覆盖变化情况
        /// </summary>
        /// <param name="Type">类型</param>
        /// <returns></returns>
        public List<P_HospitalCoverChange> FindHospitalCoverChange(string Type)
        {
            List<P_HospitalCoverChange> rtnData;

            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<P_HospitalCoverChange>(
                    //"SELECT * FROM P_HospitalCoverChange WHERE (Type = @Type and State = 0) ",
                    "SELECT * FROM P_HospitalCoverChange WHERE State = 0 ",
                    new SqlParameter[] {
                        //SqlParameterFactory.GetSqlParameter("@Type", Type)
                    });
            }
            return rtnData;
        }
        #endregion

        #region 查询医院对应代表及直线经理
        /// <summary>
        /// 查询医院对应代表及直线经理
        /// </summary>
        /// <param name="HospitalCode">类型</param>
        /// <returns></returns>
        public List<V_TerritoryHospitalMRDM> FindHospitalUser(string HospitalCode)
        {
            List<V_TerritoryHospitalMRDM> rtnData;

            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<V_TerritoryHospitalMRDM>(
                    "SELECT * FROM V_TerritoryHospitalMRDM WHERE HospitalCode = @HospitalCode ",
                    new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@HospitalCode", HospitalCode)
                    });
            }
            return rtnData;
        }
        #endregion

        #region 修改发送状态
        /// <summary>
        /// 修改发送状态
        /// </summary>
        /// <returns></returns>
        public void UpdateMessageState()
        {
            var sqlServerTemplNonHT = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplNonHT.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                var datetime = DateTime.Now;
                sqlServerTemplNonHT.ExecuteNonQuery("UPDATE P_HospitalCoverChange SET STATE = 1,ModifyDate='"+ datetime + "' WHERE STATE = 0 ");
            }
        }
        #endregion

        #region 根据医院代码查询目标医院
        /// <summary>
        /// 根据医院代码查询目标医院 20190416
        /// </summary>
        /// <param name="GskHospitalOHCode"></param>
        /// <returns></returns>
        public List<P_HOSPITAL> GetDataByGskHospitalOH(string GskHospitalOHCode)
        {
            List<P_HOSPITAL> rtnData;

            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<P_HOSPITAL>(
                    "SELECT * FROM P_HOSPITAL WHERE (HospitalCode = @HospitalCode) AND ([MainAddress]=N'主地址') AND ([IsDelete] = 0) AND ([External] = 0) ",
                    new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@HospitalCode", GskHospitalOHCode)
                    });
            }
            return rtnData;
        }
        #endregion

        #region 根据医院代码名称查询目标医院
        /// <summary>
        /// 根据医院代码名称查询目标医院 20190416
        /// </summary>
        /// <param name="GskOHName"></param>
        /// <param name="GskHospitalOHCode"></param>
        /// <returns></returns>
        public P_HOSPITAL GetDataByGskOHName(string GskOHName, string GskHospitalOHCode)
        {
            P_HOSPITAL rtnData;

            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Find<P_HOSPITAL>(
                    "SELECT * FROM P_HOSPITAL WHERE (HospitalCode = @HospitalCode) AND ([Name] = @Name) AND ([MainAddress]=N'主地址') AND ([IsDelete] = 0) ",
                    new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@HospitalCode", GskHospitalOHCode),
                        SqlParameterFactory.GetSqlParameter("@Name", GskOHName)
                    });
            }
            return rtnData;
        }
        #endregion

        #region 根据医院代码名称查询目标医院省市Market
        private string selectProvinceCitySql = "SELECT P_CITY.Name AS CityName, P_PROVINCE.Name AS ProvinceName, P_HOSPITAL.Type "
                               + "FROM P_HOSPITAL LEFT OUTER JOIN P_CITY ON P_HOSPITAL.CityId = P_CITY.ID and P_HOSPITAL.Type = P_CITY.Type  LEFT OUTER JOIN P_PROVINCE ON P_CITY.ProvinceId = P_PROVINCE.ID and P_CITY.Type=P_PROVINCE.Type "
                               + "WHERE (P_HOSPITAL.HospitalCode = @HospitalCode) AND (P_HOSPITAL.[Name] = @Name) AND (P_HOSPITAL.[MainAddress]=N'主地址') AND (P_HOSPITAL.[IsDelete] = 0) ";

        public List<P_HOSPITAL_MNT_VIEW> GetDataByProvinceCityMarket(string GskOHName, string GskHospitalOHCode)
        {
            List<P_HOSPITAL_MNT_VIEW> rtnData;
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<P_HOSPITAL_MNT_VIEW>(selectProvinceCitySql,
                new SqlParameter[]
                {
                        SqlParameterFactory.GetSqlParameter("@HospitalCode", GskHospitalOHCode),
                        SqlParameterFactory.GetSqlParameter("@Name", GskOHName)
                });
            }
            return rtnData;
        }
        #endregion

        #region 获取Rx临时表数据
        /// <summary>
        /// 获取Rx临时表数据
        /// </summary>
        /// <param name="nowDate"></param>
        /// <returns></returns>
        public List<P_HOSPITAL_RxTemp> LoadRxTemp()
        {
            List<P_HOSPITAL_RxTemp> rtnData;
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<P_HOSPITAL_RxTemp>("SELECT * FROM Temp_Rx_Hospital  ",
                new SqlParameter[]
                {
                });
            }
            return rtnData;
        }
        #endregion

        #region 根据Market删除Detail表中关联主数据表的数据
        public void DeleteDetailData(string Market)
        {
            var sqlServerTemplNonHT = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplNonHT.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                sqlServerTemplNonHT.ExecuteNonQuery("DELETE FROM P_HOSPITAL_DETAIL "
                                                    + "WHERE P_HOSPITAL_DETAIL.GskHospital in(SELECT P_HOSPITAL.GskHospital "
                                                    + "FROM P_HOSPITAL  "
                                                    + "WHERE P_HOSPITAL.MainAddress =N'主地址' AND P_HOSPITAL.[Type] =@Type) ",
                   new SqlParameter[]
                   {
                        SqlParameterFactory.GetSqlParameter("@Type", Market)
                   });
            }
        }
        #endregion

        #region 根据Market删除主数据表中主地址数据
        public void DeleteMainAddressData(string Market)
        {
            var sqlServerTemplNonHT = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplNonHT.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                sqlServerTemplNonHT.ExecuteNonQuery("DELETE FROM P_HOSPITAL "
                                                   + "WHERE MainAddress =N'主地址' AND [Type] =@Type AND [External]=0 ",
                   new SqlParameter[]
                   {
                        SqlParameterFactory.GetSqlParameter("@Type", Market)
                   });
            }
        }
        #endregion

        #region 更新门地址IsDelete=1
        /// <summary>
        /// 更新门地址IsDelete=1
        /// </summary>
        /// <param name="Market"></param>
        /// <returns></returns>
        public int UpdateIsDelete(string Market)
        {
            var sqlServerTemplNonHT = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplNonHT.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                var res = sqlServerTemplNonHT.Update($"UPDATE [P_HOSPITAL] SET [IsDelete] = 1 WHERE ([Type] = '" + Market + "') AND [External] =0 AND (MainAddress <>N'主地址') AND [IsDelete]=0  ",
                    new SqlParameter[] { });
                return res;
            }
        }
        #endregion

        #region 更新院外IsDelete=1
        /// <summary>
        /// 更新院外IsDelete=1
        /// </summary>
        /// <param name="Market"></param>
        /// <returns></returns>
        public int UpdateOHIsDelete(string Market)
        {
            var sqlServerTemplNonHT = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplNonHT.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                var res = sqlServerTemplNonHT.Update($"UPDATE [P_HOSPITAL] SET [IsDelete] = 1 WHERE ([Type] = '" + Market + "') AND [External]=1 ",
                    new SqlParameter[] { });
                return res;
            }
        }
        #endregion

        //#region 插入医院
        ///// <summary>
        ///// 插入医院
        ///// </summary>
        ///// <param name="list"></param>
        ///// <returns></returns>
        //public int InsertHospitalData(List<P_HOSPITAL> list)
        //{
        //    #region 插入医院
        //    var sqlServerTemplNonHT = sqlServerTemplFactoryNonHT.CreateDbTemplate();
        //    //var sqlServerTemplHT = sqlServerTemplFactory.CreateDbTemplate();
        //    using (var conn = sqlServerTemplNonHT.GetSqlConnection() as SqlConnection)
        //    {
        //        conn.Open();
        //        var tran = conn.BeginTransaction();
        //        foreach (var item in list)
        //        {
        //            SqlCommand commandAdd = new SqlCommand(
        //                "INSERT INTO [P_HOSPITAL] ([CityId] ,[GskHospital],[OldGskHospital] ,[Name],[OldName] ,[FirstLetters] ,[Address] ,[Latitude] ,[Longitude] ,[Type] ,[External] ,[CreateDate] ,[ProvinceId] ,[IsXMS] ,[IsBDS] ,[IsMT] ,[Remark] ,[IsDelete] ,[MainAddress] ,[HospitalCode] ) "
        //                + " VALUES (@CityId ,@GskHospital,@OldGskHospital ,@Name,@OldName ,@FirstLetters ,@Address ,@Latitude ,@Longitude ,@Type ,@External ,@CreateDate ,@ProvinceId ,@IsXMS ,@IsBDS ,@IsMT ,@Remark ,@IsDelete, @MainAddress, @HospitalCode ) ",
        //                conn);
        //            commandAdd.Transaction = tran;
        //            commandAdd.Parameters.AddRange(
        //                new SqlParameter[]
        //                {
        //                    SqlParameterFactory.GetSqlParameter("@CityId", item.CityId),
        //                    SqlParameterFactory.GetSqlParameter("@GskHospital", item.GskHospital),
        //                    SqlParameterFactory.GetSqlParameter("@OldGskHospital", item.OldGskHospital),
        //                    SqlParameterFactory.GetSqlParameter("@Name", item.Name),
        //                    SqlParameterFactory.GetSqlParameter("@OldName", item.OldName),
        //                    SqlParameterFactory.GetSqlParameter("@FirstLetters", item.FirstLetters),
        //                    SqlParameterFactory.GetSqlParameter("@Address", item.Address),
        //                    SqlParameterFactory.GetSqlParameter("@Latitude", item.Latitude),
        //                    SqlParameterFactory.GetSqlParameter("@Longitude", item.Longitude),
        //                    SqlParameterFactory.GetSqlParameter("@Type", item.Type),
        //                    SqlParameterFactory.GetSqlParameter("@External", item.External),
        //                    SqlParameterFactory.GetSqlParameter("@CreateDate", item.CreateDate),
        //                    SqlParameterFactory.GetSqlParameter("@ProvinceId", item.ProvinceId),
        //                    SqlParameterFactory.GetSqlParameter("@IsXMS", item.IsXMS),
        //                    SqlParameterFactory.GetSqlParameter("@IsBDS", item.IsBDS),
        //                    SqlParameterFactory.GetSqlParameter("@IsMT", item.IsMT),
        //                    SqlParameterFactory.GetSqlParameter("@Remark", item.Remark),
        //                    SqlParameterFactory.GetSqlParameter("@IsDelete", item.IsDelete),
        //                    SqlParameterFactory.GetSqlParameter("@MainAddress", item.MainAddress),
        //                    SqlParameterFactory.GetSqlParameter("@HospitalCode", item.HospitalCode)
        //                }
        //            );
        //            commandAdd.ExecuteNonQuery();
        //        }
        //        tran.Commit();
        //    }

        //    return 1;
        //    #endregion
        //}
        //#endregion

        //#region 插入Detail
        ///// <summary>
        ///// 插入Detail
        ///// </summary>
        ///// <param name="list"></param>
        ///// <returns></returns>
        //public int InsertHospitalDetail(List<P_HOSPITAL_DETAIL> list)
        //{
        //    #region 插入Detail
        //    var sqlServerTemplNonHT = sqlServerTemplFactoryNonHT.CreateDbTemplate();
        //    //var sqlServerTemplHT = sqlServerTemplFactory.CreateDbTemplate();
        //    using (var conn = sqlServerTemplNonHT.GetSqlConnection() as SqlConnection)
        //    {
        //        conn.Open();
        //        var tran = conn.BeginTransaction();
        //        foreach (var item in list)
        //        {
        //            SqlCommand commandAdd = new SqlCommand(
        //                "INSERT INTO [P_HOSPITAL_DETAIL] ([GskHospital] ,[District],[DistrictCode] ,[CustomerType],[RESP] ,[HEP] ,[CNS] ,[HIV] ,[VOL] ,[MA] ,[Region] ,[IsDelete] ,[CreateDate] ,[UpdateDate] ,[IP] ) "
        //                + " VALUES (@GskHospital ,@District,@DistrictCode ,@CustomerType,@RESP ,@HEP ,@CNS ,@HIV ,@VOL ,@MA ,@Region ,@IsDelete ,@CreateDate ,@UpdateDate ,@IP ) ",
        //                conn);
        //            commandAdd.Transaction = tran;
        //            commandAdd.Parameters.AddRange(
        //                new SqlParameter[]
        //                {
        //                    SqlParameterFactory.GetSqlParameter("@GskHospital", item.GskHospital),
        //                    SqlParameterFactory.GetSqlParameter("@District", item.District),
        //                    SqlParameterFactory.GetSqlParameter("@DistrictCode", item.DistrictCode),
        //                    SqlParameterFactory.GetSqlParameter("@CustomerType", item.CustomerType),
        //                    SqlParameterFactory.GetSqlParameter("@RESP", item.RESP),
        //                    SqlParameterFactory.GetSqlParameter("@HEP", item.HEP),
        //                    SqlParameterFactory.GetSqlParameter("@CNS", item.CNS),
        //                    SqlParameterFactory.GetSqlParameter("@HIV", item.HIV),
        //                    SqlParameterFactory.GetSqlParameter("@VOL", item.VOL),
        //                    SqlParameterFactory.GetSqlParameter("@MA", item.MA),
        //                    SqlParameterFactory.GetSqlParameter("@Region", item.Region),
        //                    SqlParameterFactory.GetSqlParameter("@IsDelete", item.IsDelete),
        //                    SqlParameterFactory.GetSqlParameter("@CreateDate", item.CreateDate),
        //                    SqlParameterFactory.GetSqlParameter("@UpdateDate", item.UpdateDate),
        //                    SqlParameterFactory.GetSqlParameter("@IP", item.IP)
        //                }
        //            );
        //            commandAdd.ExecuteNonQuery();
        //        }
        //        tran.Commit();
        //    }

        //    return 1;
        //    #endregion
        //}
        //#endregion

        #region list转datatable插入医院
        /// <summary>
        /// list转datatable插入医院
        /// </summary>
        /// <returns></returns>
        public int InsertHospitalData(List<P_HOSPITAL> list)
        {
            //var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            var sqlServerTemplNonHT = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            int insCnt = 0;

            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            //int insCntTemp = 0;
            using (var conn = sqlServerTemplNonHT.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();

                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT TOP 0 * FROM P_HOSPITAL ";
                cmd.Transaction = tran;
                SqlDataAdapter adapter = new SqlDataAdapter();
                DataSet ds = new DataSet();
                adapter.SelectCommand = cmd;
                adapter.Fill(ds);

                var dt = ds.Tables[0];
                dt.Clear();
                foreach (var data in list)
                {
                    var dr = dt.NewRow();
                    //dr[0] = data.ID;
                    dr[1] = data.CityId;
                    dr[2] = data.GskHospital;
                    dr[3] = data.Name;
                    dr[4] = data.FirstLetters;
                    dr[5] = data.Address;
                    dr[6] = data.Latitude;
                    dr[7] = data.Longitude;
                    dr[8] = data.Type;
                    dr[9] = data.External;
                    dr[10] = data.CreateDate;
                    dr[11] = data.ProvinceId;
                    dr[12] = data.IsXMS;
                    dr[13] = data.IsBDS;
                    dr[14] = data.IsMT;
                    dr[15] = data.Remark;
                    dr[16] = data.IsDelete;
                    dr[17] = data.OldGskHospital;
                    dr[18] = data.OldName;
                    dr[19] = DBNull.Value;
                    dr[20] = data.MainAddress;
                    dr[21] = data.HospitalCode;
                    dt.Rows.Add(dr);
                }

                SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, tran);
                bulkCopy.DestinationTableName = "P_HOSPITAL";
                bulkCopy.BatchSize = list.Count;

                bulkCopy.WriteToServer(dt);
                insCnt = bulkCopy.BatchSize;

                watch.Stop();

                LogHelper.Info("Tidy up the data time :" + watch.ElapsedMilliseconds);
                watch.Restart();
                watch.Start();

                tran.Commit();

                watch.Stop();

                LogHelper.Info("Commit time :" + watch.ElapsedMilliseconds);
                watch.Restart();
                watch.Start();

                conn.Close();

                watch.Stop();

                LogHelper.Info("Close time :" + watch.ElapsedMilliseconds);
            }

            return insCnt;
        }
        #endregion

        #region list转datatable插入detail
        /// <summary>
        /// list转datatable插入detail
        /// </summary>
        /// <returns></returns>
        public int InsertHospitalDetail(List<P_HOSPITAL_DETAIL> list)
        {
            var sqlServerTemplNonHT = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            int insCnt = 0;

            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            using (var conn = sqlServerTemplNonHT.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();

                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT TOP 0 * FROM P_HOSPITAL_DETAIL ";
                cmd.Transaction = tran;
                SqlDataAdapter adapter = new SqlDataAdapter();
                DataSet ds = new DataSet();
                adapter.SelectCommand = cmd;
                adapter.Fill(ds);

                var dt = ds.Tables[0];
                dt.Clear();
                foreach (var data in list)
                {
                    var dr = dt.NewRow();
                    //dr[0] = data.ID;
                    dr[1] = data.GskHospital;
                    dr[2] = data.District;
                    dr[3] = data.DistrictCode;
                    dr[4] = data.CustomerType;
                    dr[5] = data.RESP;
                    dr[6] = data.HEP;
                    dr[7] = data.CNS;
                    dr[8] = data.HIV;
                    dr[9] = data.VOL;
                    dr[10] = data.MA;
                    dr[11] = data.Region;
                    dr[12] = data.IsDelete;
                    dr[13] = data.CreateDate;
                    dr[14] = data.UpdateDate;
                    dr[15] = data.IP;

                    dt.Rows.Add(dr);
                }

                SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, tran);
                bulkCopy.DestinationTableName = "P_HOSPITAL_DETAIL";
                bulkCopy.BatchSize = list.Count;

                bulkCopy.WriteToServer(dt);
                insCnt = bulkCopy.BatchSize;

                watch.Stop();

                LogHelper.Info("Tidy up the data time :" + watch.ElapsedMilliseconds);
                watch.Restart();
                watch.Start();

                tran.Commit();

                watch.Stop();

                LogHelper.Info("Commit time :" + watch.ElapsedMilliseconds);
                watch.Restart();
                watch.Start();

                conn.Close();

                watch.Stop();

                LogHelper.Info("Close time :" + watch.ElapsedMilliseconds);
            }

            return insCnt;
        }
        #endregion

        #region 更新门地址IsDelete=0
        /// <summary>
        /// 更新门地址IsDelete=0
        /// </summary>
        /// <param name="Market"></param>
        /// <returns></returns>
        public int UpdateRxStatus(string Market)
        {

            var sqlServerTemplNonHT = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplNonHT.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                var res = sqlServerTemplNonHT.Update($"UPDATE [P_HOSPITAL] SET [P_HOSPITAL].[IsDelete] = 0 "
                                                  + "WHERE [P_HOSPITAL].[Type] = '" + Market + "' AND [P_HOSPITAL].[External] =0 AND [P_HOSPITAL].MainAddress <> N'主地址' "
                                                  + " AND [P_HOSPITAL].[IsDelete]=1 AND [P_HOSPITAL].GskHospital in (select [Temp_Rx_Hospital].GskHospital from Temp_Rx_Hospital) ",
                    new SqlParameter[] { });
                return res;
            }
        }
        #endregion

        #region 根据Market查询主数据院外数据
        /// <summary>
        /// 根据Market查询主数据院外数据
        /// </summary>
        /// <param name="Market"></param>
        /// <returns></returns>
        public List<P_HOSPITAL> GetOHData(string Market)
        {
            List<P_HOSPITAL> rtnData;

            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<P_HOSPITAL>(
                    "SELECT * FROM P_HOSPITAL WHERE ([Type] = @Type) AND [External]=1 ",
                    new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@Type", Market)
                    });
            }
            return rtnData;
        }
        #endregion

        #region 根据Market院外Code查询主数据数据
        /// <summary>
        /// 根据Market院外Code查询主数据数据
        /// </summary>
        /// <param name="Market"></param>
        /// <param name="GskHospitalCode"></param>
        /// <returns></returns>
        public List<P_HOSPITAL> GetHData(string GskHospitalCode, string Market)
        {
            List<P_HOSPITAL> rtnData;

            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<P_HOSPITAL>(
                    "SELECT * FROM P_HOSPITAL WHERE GskHospital = @GskHospital AND [Type] = @Type AND [External]=0 AND [MainAddress]=N'主地址' AND [IsDelete] = 0 ",
                    new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@GskHospital", GskHospitalCode),
                        SqlParameterFactory.GetSqlParameter("@Type", Market)
                    });
            }
            return rtnData;
        }
        #endregion

        #region 更新院外数据信息与目标医院一致
        /// <summary>
        /// 更新院外数据信息与目标医院一致
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public int UpdateOHData(List<P_HOSPITAL> list)
        {
            #region 更新院外数据信息与目标医院一致
            var sqlServerTemplNonHT = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            //var sqlServerTemplHT = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplNonHT.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                var tran = conn.BeginTransaction();

                foreach (var item in list)
                {
                    SqlCommand commandAdd = new SqlCommand(
                        "UPDATE [P_HOSPITAL] SET [CityId] = @CityId , [Name] = @Name , [FirstLetters] = @FirstLetters , [ProvinceId] = @ProvinceId , [IsDelete] = @IsDelete  "
                        + " WHERE  [GskHospital] = @GskHospital ",
                        conn);
                    commandAdd.Transaction = tran;
                    commandAdd.Parameters.AddRange(
                        new SqlParameter[]
                        {
                            SqlParameterFactory.GetSqlParameter("@GskHospital", item.GskHospital),
                            SqlParameterFactory.GetSqlParameter("@CityId", item.CityId),
                            SqlParameterFactory.GetSqlParameter("@Name", item.Name),
                            SqlParameterFactory.GetSqlParameter("@FirstLetters", item.FirstLetters),
                            SqlParameterFactory.GetSqlParameter("@ProvinceId", item.ProvinceId),
                            SqlParameterFactory.GetSqlParameter("@IsDelete", item.IsDelete)
                        }
                    );
                    commandAdd.ExecuteNonQuery();

                }

                tran.Commit();
            }
            return 1;
            #endregion
        }
        #endregion

        #region 获取Vx临时表数据
        /// <summary>
        /// 获取Vx临时表数据
        /// </summary>
        /// <param name="nowDate"></param>
        /// <returns></returns>
        public List<P_HOSPITAL_VxTemp> LoadVxTemp()
        {
            List<P_HOSPITAL_VxTemp> rtnData;
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<P_HOSPITAL_VxTemp>("SELECT * FROM Temp_Vx_Hospital  ",
                new SqlParameter[]
                {
                });
            }
            return rtnData;
        }
        #endregion

        #region 更新门地址IsDelete=0
        /// <summary>
        /// 更新门地址IsDelete=0
        /// </summary>
        /// <param name="Market"></param>
        /// <returns></returns>
        public int UpdateVxStatus(string Market)
        {

            var sqlServerTemplNonHT = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplNonHT.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                var res = sqlServerTemplNonHT.Update($"UPDATE [P_HOSPITAL] SET [P_HOSPITAL].[IsDelete] = 0 "
                                                  + "WHERE [P_HOSPITAL].[Type] = '" + Market + "' AND [P_HOSPITAL].[External] =0 AND [P_HOSPITAL].MainAddress <> N'主地址' "
                                                  + " AND [P_HOSPITAL].[IsDelete]=1 AND [P_HOSPITAL].GskHospital in (select [Temp_Vx_Hospital].GskHospital from Temp_Vx_Hospital) ",
                    new SqlParameter[] { });
                return res;
            }
        }
        #endregion

        #region 获取DDT临时表数据
        /// <summary>
        /// 获取DDT临时表数据
        /// </summary>
        /// <param name="nowDate"></param>
        /// <returns></returns>
        public List<P_HOSPITAL_DDTTemp> LoadDDTTemp()
        {
            List<P_HOSPITAL_DDTTemp> rtnData;
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<P_HOSPITAL_DDTTemp>("SELECT * FROM Temp_DDT_Hospital  ",
                new SqlParameter[]
                {
                });
            }
            return rtnData;
        }
        #endregion

        #region 更新门地址IsDelete=0
        /// <summary>
        /// 更新门地址IsDelete=0
        /// </summary>
        /// <param name="Market"></param>
        /// <returns></returns>
        public int UpdateDDTStatus(string Market)
        {

            var sqlServerTemplNonHT = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplNonHT.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                var res = sqlServerTemplNonHT.Update($"UPDATE [P_HOSPITAL] SET [P_HOSPITAL].[IsDelete] = 0 "
                                                  + "WHERE [P_HOSPITAL].[Type] = '" + Market + "' AND [P_HOSPITAL].[External] =0 AND [P_HOSPITAL].MainAddress <> N'主地址' "
                                                  + " AND [P_HOSPITAL].[IsDelete]=1 AND [P_HOSPITAL].GskHospital in (select [Temp_DDT_Hospital].GskHospital from Temp_DDT_Hospital) ",
                    new SqlParameter[] { });
                return res;
            }
        }
        #endregion

        #region 获取TSKF临时表数据
        /// <summary>
        /// 获取TSKF临时表数据
        /// </summary>
        /// <param name="nowDate"></param>
        /// <returns></returns>
        public List<P_HOSPITAL_TSKFTemp> LoadTSKFTemp()
        {
            List<P_HOSPITAL_TSKFTemp> rtnData;
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<P_HOSPITAL_TSKFTemp>("SELECT * FROM Temp_TSKF_Hospital  ",
                new SqlParameter[]
                {
                });
            }
            return rtnData;
        }
        #endregion

        #region 更新门地址IsDelete=0
        /// <summary>
        /// 更新门地址IsDelete=0
        /// </summary>
        /// <param name="Market"></param>
        /// <returns></returns>
        public int UpdateTSKFStatus(string Market)
        {

            var sqlServerTemplNonHT = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplNonHT.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                var res = sqlServerTemplNonHT.Update($"UPDATE [P_HOSPITAL] SET [P_HOSPITAL].[IsDelete] = 0 "
                                                  + "WHERE [P_HOSPITAL].[Type] = '" + Market + "' AND [P_HOSPITAL].[External] =0 AND [P_HOSPITAL].MainAddress <> N'主地址' "
                                                  + " AND [P_HOSPITAL].[IsDelete]=1 AND [P_HOSPITAL].GskHospital in (select [Temp_TSKF_Hospital].GskHospital from Temp_TSKF_Hospital) ",
                    new SqlParameter[] { });
                return res;
            }
        }
        #endregion

        #region 根据Market及ACTION查询变量表数据
        /// <summary>
        /// 根据Market及ACTION查询变量表数据
        /// </summary>
        /// <param name="Market"></param>
        /// <param name="ACTION"></param>
        /// <returns></returns>
        public List<Temp_Hospital_Variables> GetHosVariables(string Market, string ACTION)
        {
            List<Temp_Hospital_Variables> rtnData;

            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<Temp_Hospital_Variables>(
                    "SELECT * FROM [dbo].[Temp_Hospital_Variables] WHERE Market = @Market AND [action]=@ACTION ",
                    new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@Market", Market),
                        SqlParameterFactory.GetSqlParameter("@ACTION", ACTION)
                    });
            }
            return rtnData;
        }
        #endregion

        #region 查询主数据表中与变量表匹配的门地址
        /// <summary>
        /// 查询主数据表中与变量表匹配的门地址
        /// </summary>
        /// <param name="Market"></param>
        /// <param name="ACTION"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public List<P_HOSPITAL> Getnotmain(string Market, string ACTION, string Type)
        {
            List<P_HOSPITAL> rtnData;

            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<P_HOSPITAL>(
                    "SELECT p.* FROM [dbo].[P_HOSPITAL] p WHERE p.[Type] = @Type AND p.MainAddress<>N'主地址' AND p.GskHospital in (SELECT t.GskHospital "
                   + " FROM [dbo].[Temp_Hospital_Variables] t where t.Market= @Market AND t.[action]=@ACTION) ",
                    new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@Type", Type),
                        SqlParameterFactory.GetSqlParameter("@Market", Market),
                        SqlParameterFactory.GetSqlParameter("@ACTION", ACTION)
                    });
            }
            return rtnData;
        }
        #endregion

        #region 更新失效门地址状态为IsDelete=2
        /// <summary>
        /// 更新失效门地址状态为IsDelete=2
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public int Updatenotmain(List<P_HOSPITAL> list, List<Temp_Hospital_Variables> varList)
        {
            #region 更新失效门地址状态为IsDelete=2
            var sqlServerTemplNonHT = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplNonHT.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                var tran = conn.BeginTransaction();
                string dt = DateTime.Now.ToString();
                foreach (var item in list)
                {
                    SqlCommand commandAdd = new SqlCommand(
                        "UPDATE [P_HOSPITAL] SET [IsDelete] = 2  "
                        + " WHERE  [GskHospital] = @GskHospital AND MainAddress<>N'主地址' AND [IsDelete]=1 ",
                        conn);
                    commandAdd.Transaction = tran;
                    commandAdd.Parameters.AddRange(
                        new SqlParameter[]
                        {
                            SqlParameterFactory.GetSqlParameter("@GskHospital", item.GskHospital)
                        }
                    );
                    commandAdd.ExecuteNonQuery();
                }
                foreach (var it in varList)
                {
                    //更新地址申请表中IsDeleteUpdate字段
                    SqlCommand commandUp = new SqlCommand(
                        "UPDATE [P_AddressApproval] SET [IsDeleteUpdate] = 2, DeleteDate = '" + dt + "' "
                        + " WHERE  [GskHospital] = @GskHospital ",
                        conn);
                    commandUp.Transaction = tran;
                    commandUp.Parameters.AddRange(
                        new SqlParameter[]
                        {
                            SqlParameterFactory.GetSqlParameter("@GskHospital", it.GskHospital)
                        }
                    );
                    commandUp.ExecuteNonQuery();
                }
                tran.Commit();
            }
            return 1;
            #endregion
        }
        #endregion

        #region 更新主地址地址变更的门地址状态为IsDelete=3
        /// <summary>
        /// 更新主地址地址变更的门地址状态为IsDelete=3
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public int UpdateAnotmain(List<P_HOSPITAL> list, List<Temp_Hospital_Variables> varList)
        {
            #region 更新主地址地址变更的门地址状态为IsDelete=3
            var sqlServerTemplNonHT = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplNonHT.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                var tran = conn.BeginTransaction();
                string dt = DateTime.Now.ToString();
                foreach (var item in list)
                {
                    SqlCommand commandAdd = new SqlCommand(
                        "UPDATE [P_HOSPITAL] SET [IsDelete] = 3  "
                        + " WHERE  [GskHospital] = @GskHospital AND MainAddress<>N'主地址' AND [IsDelete]=0  ",
                        conn);
                    commandAdd.Transaction = tran;
                    commandAdd.Parameters.AddRange(
                        new SqlParameter[]
                        {
                            SqlParameterFactory.GetSqlParameter("@GskHospital", item.GskHospital)
                        }
                    );
                    commandAdd.ExecuteNonQuery();
                }
                foreach (var it in varList)
                {
                    //更新地址申请表中IsDeleteUpdate字段
                    SqlCommand commandUp = new SqlCommand(
                        "UPDATE [P_AddressApproval] SET [IsDeleteUpdate] = 3, DeleteDate = '" + dt + "'  "
                        + " WHERE  [GskHospital] = @GskHospital ",
                        conn);
                    commandUp.Transaction = tran;
                    commandUp.Parameters.AddRange(
                        new SqlParameter[]
                        {
                            SqlParameterFactory.GetSqlParameter("@GskHospital", it.GskHospital)
                        }
                    );
                    commandUp.ExecuteNonQuery();
                }
                tran.Commit();
            }
            return 1;
            #endregion
        }
        #endregion

        #region 变量记录
        public List<P_HospitalVariables> LoadHospitalVariables(string srh_GskHospital, string srh_HospitalName, string srh_HospitalMarket, bool srh_Add, bool srh_City, bool srh_UpdateHospitalName, bool srh_LatLong, bool srh_Address, bool srh_Delete, int rows, int page, out int total)
        {
            List<P_HospitalVariables> rtnData;
            List<int> list = new List<int>();
            string selectSqlForLoad = " SELECT [GskHospital],[Province],[City],[HospitalName],case when [Address] is null then '' else [Address] end as Address,[IsMainAdd],[Market],case when [Longitude] is null then '' else [Longitude] end as Longitude ,case when [Latitude]  is null then '' else [Latitude] end as Latitude "
                                    + " ,case when [DistrictCode] is null then '' else [DistrictCode] end as DistrictCode,[District],[Remarks],[Action],[CreateDate],[CreateBy], "
                                    + " case when [Action] = 1 then N'新增' when [Action] = 2 then N'省份城市更新' when [Action] = 3 then N'医院更名' "
                                    + " when [Action] = 4 then N'更新经纬度' when [Action] = 5 then N'更新地址' when [Action] = 6 then N'删除' else '' end AS ActionDisplay "
                                    + " from [P_Hospital_Variables_Data] where 1=1 ";
            if (!string.IsNullOrEmpty(srh_GskHospital))
            {
                selectSqlForLoad += " and (GskHospital LIKE '%' + @GskHospital + '%')";
            }
            if (!string.IsNullOrEmpty(srh_HospitalName))
            {
                selectSqlForLoad += " and (HospitalName LIKE '%' + @HospitalName + '%')";
            }
            if (!string.IsNullOrEmpty(srh_HospitalMarket))
            {
                selectSqlForLoad += " and (Market LIKE '%' + @Market + '%')";
            }
            if (srh_Add == true)
            {
                list.Add(1);
            }
            if (srh_City == true)
            {
                list.Add(2);
            }
            if (srh_UpdateHospitalName == true)
            {
                list.Add(3);
            }
            if (srh_LatLong == true)
            {
                list.Add(4);
            }
            if (srh_Address == true)
            {
                list.Add(5);
            }
            if (srh_Delete == true)
            {
                list.Add(6);
            }
            string action = string.Join(",", list.ToArray());
            if (!string.IsNullOrEmpty(action))
            {
                selectSqlForLoad += " and (Action in (" + action + "))";
            }
            else
                selectSqlForLoad += " and 1<>1 ";
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {

                conn.Open();
                rtnData = sqlServerTemplate.LoadPages<P_HospitalVariables>(rows, page, out total, selectSqlForLoad, "order by CreateDate DESC,Market",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@GskHospital", srh_GskHospital),
                        SqlParameterFactory.GetSqlParameter("@HospitalName", srh_HospitalName),
                        SqlParameterFactory.GetSqlParameter("@Market", srh_HospitalMarket)
                    });
            }
            return rtnData;
        }

        public List<P_HospitalVariables> ExportHospitalVariablesList(string srh_GskHospital, string srh_HospitalName, string srh_HospitalMarket, bool srh_Add, bool srh_City, bool srh_UpdateHospitalName, bool srh_LatLong, bool srh_Address, bool srh_Delete)
        {
            List<P_HospitalVariables> rtnData;
            List<int> list = new List<int>();
            string selectSqlForLoad = " SELECT [GskHospital],[Province],[City],[HospitalName],case when [Address] is null then '' else [Address] end as Address,[IsMainAdd],[Market],case when [Longitude] is null then '' else [Longitude] end as Longitude ,case when [Latitude]  is null then '' else [Latitude] end as Latitude  "
                                    + " ,case when [DistrictCode] is null then '' else [DistrictCode] end as DistrictCode,[District],[Remarks],[Action],[CreateDate],[CreateBy], "
                                    + " case when [Action] = 1 then N'新增' when [Action] = 2 then N'省份城市更新' when [Action] = 3 then N'医院更名' "
                                    + " when [Action] = 4 then N'更新经纬度' when [Action] = 5 then N'更新地址' when [Action] = 6 then N'删除' else '' end AS ActionDisplay "
                                    + " from [P_Hospital_Variables_Data] where 1=1 ";
            if (!string.IsNullOrEmpty(srh_GskHospital))
            {
                selectSqlForLoad += " and (GskHospital LIKE '%' + @GskHospital + '%')";
            }
            if (!string.IsNullOrEmpty(srh_HospitalName))
            {
                selectSqlForLoad += " and (HospitalName LIKE '%' + @HospitalName + '%')";
            }
            if (!string.IsNullOrEmpty(srh_HospitalMarket))
            {
                selectSqlForLoad += " and (Market LIKE '%' + @Market + '%')";
            }
            if (srh_Add == true)
            {
                list.Add(1);
            }
            if (srh_City == true)
            {
                list.Add(2);
            }
            if (srh_UpdateHospitalName == true)
            {
                list.Add(3);
            }
            if (srh_LatLong == true)
            {
                list.Add(4);
            }
            if (srh_Address == true)
            {
                list.Add(5);
            }
            if (srh_Delete == true)
            {
                list.Add(6);
            }
            string action = string.Join(",", list.ToArray());
            if (!string.IsNullOrEmpty(action))
            {
                selectSqlForLoad += " and (Action in (" + action + "))";
            }
            else
                selectSqlForLoad += " and 1<>1 ";
            selectSqlForLoad += " order by CreateDate DESC,Market ";
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<P_HospitalVariables>(selectSqlForLoad,
                new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@GskHospital", srh_GskHospital),
                        SqlParameterFactory.GetSqlParameter("@HospitalName", srh_HospitalName),
                        SqlParameterFactory.GetSqlParameter("@Market", srh_HospitalMarket)
                    });
            }
            return rtnData;
        }

        public int SyncCoypHospitalVariablesData()
        {
            string dt = DateTime.Now.ToString(); ;
            var sqlServerTemplNonHT = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplNonHT.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                var res = sqlServerTemplNonHT.Update($" insert into P_Hospital_Variables_Data ( [GskHospital] ,[Province] ,[City] ,[HospitalName] ,[Address] ,[IsMainAdd] ,[Market] ,[Longitude] ,[Latitude] ,[DistrictCode] ,[District] ,[Remarks] ,[Action] ,[CreateDate] ,[CreateBy]) "
                                                       + "select [GskHospital] ,[Province] ,[City] ,[HospitalName] ,[Address] ,[IsMainAdd] ,[Market] ,[Longitude] ,[Latitude] ,[DistrictCode] ,[District] ,[Remarks] ,[Action] , '" + dt + "', [CreateBy] from [dbo].[Temp_Hospital_Variables]",
                    new SqlParameter[] { });
                return res;
            }
        }

        public int InsertHospitalVariablesCountData()
        {
            string dt = DateTime.Now.ToString(); ;
            var sqlServerTemplNonHT = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplNonHT.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                var res = sqlServerTemplNonHT.Update($" insert into P_Hospital_Variables_Count ( [Date], [RxCount], [VxCount], [DDTCount], [TSKFCount]) "
                                                      + " values( '" + dt + "', (select count(*) from p_hospital where type = 'Rx' and IsDelete=0), "
                                                      + "(select count(*) from p_hospital where type = 'Vx' and IsDelete=0),"
                                                      + "(select count(*) from p_hospital where type = 'DDT' and IsDelete=0),"
                                                      + "(select count(*) from p_hospital where type = 'TSKF' and IsDelete=0))",
                    new SqlParameter[] { });
                return res;
            }
        }
        public List<P_Hospital_Variables_Count> LoadHospitalVariablesCount(int rows, int page, out int total)
        {
            List<P_Hospital_Variables_Count> rtnData;
            string selectSqlForLoad = " SELECT ID,Date,RxCount,VxCount,DDTCount,TSKFCount,RxCount+VxCount+DDTCount+TSKFCount as AllCount from [P_Hospital_Variables_Count] ";

            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.LoadPages<P_Hospital_Variables_Count>(rows, page, out total, selectSqlForLoad, "order by ID DESC ",
                    new SqlParameter[]
                    {
                    });
            }
            return rtnData;
        }
        public List<P_CHECK_REPORT_LINE_RM> LoadHospitalVariablesCount(string srh_market, bool srh_Add, bool srh_Delete, int rows, int page, out int total)
        {
            List<P_CHECK_REPORT_LINE_RM> rtnData;
            List<string> list = new List<string>();
            string selectSqlForLoad = " SELECT [Market] ,[TERRITORY_TA] ,[TerritoryCode_RM] ,[Remarks] ,[createdate] "
                                    + " from [P_CHECK_REPORT_LINE_RM] where 1=1 ";
            if (!string.IsNullOrEmpty(srh_market))
            {
                selectSqlForLoad += " and (Market LIKE '%' + @Market + '%')";
            }
            if (srh_Add == true)
            {
                list.Add("N'新增'");
            }
            if (srh_Delete == true)
            {
                list.Add("N'删除'");
            }
            string action = string.Join(",", list.ToArray());
            if (!string.IsNullOrEmpty(action))
            {
                selectSqlForLoad += " and (Remarks in (" + action + "))";
            }
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {

                conn.Open();
                rtnData = sqlServerTemplate.LoadPages<P_CHECK_REPORT_LINE_RM>(rows, page, out total, selectSqlForLoad, "order by Market, TERRITORY_TA",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@Market", srh_market)
                    });
            }
            return rtnData;
        }
        public List<P_CHECK_REPORT_LINE_RM> ExportTerritoryRMVariables(string srh_market, bool srh_Add, bool srh_Delete)
        {
            List<P_CHECK_REPORT_LINE_RM> rtnData;
            List<string> list = new List<string>();
            string selectSqlForLoad = " SELECT [Market] ,[TERRITORY_TA] ,[TerritoryCode_RM] ,[Remarks] ,[createdate] "
                                    + " from [P_CHECK_REPORT_LINE_RM] where 1=1 ";
            if (!string.IsNullOrEmpty(srh_market))
            {
                selectSqlForLoad += " and (Market LIKE '%' + @Market + '%')";
            }
            if (srh_Add == true)
            {
                list.Add("N'新增'");
            }
            if (srh_Delete == true)
            {
                list.Add("N'删除'");
            }
            string action = string.Join(",", list.ToArray());
            if (!string.IsNullOrEmpty(action))
            {
                selectSqlForLoad += " and (Remarks in (" + action + "))";
            }
            selectSqlForLoad += " order by Market, TERRITORY_TA ";
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<P_CHECK_REPORT_LINE_RM>(selectSqlForLoad,
                new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@Market", srh_market)
                    });
            }
            return rtnData;
        }
        public List<CHECK_REPORT_LINE_RM> LoadTerritoryRMVariablesData()
        {
            List<CHECK_REPORT_LINE_RM> rtnData;
            string selectSqlForLoad = " SELECT * from [CHECK_REPORT_LINE_RM] ";
            
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<CHECK_REPORT_LINE_RM>(selectSqlForLoad,
                new SqlParameter[]
                    {
                    });
            }
            return rtnData;
        }
        public int SyncCoypTerritoryRMVariablesData(List<CHECK_REPORT_LINE_RM> list)
        {
            var addList = list.Where(c => c.ACTION == 1).ToList();
            var delList = list.Where(c => c.ACTION == 6).ToList();

            var sqlServerTemplNonHT = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplNonHT.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                var tran = conn.BeginTransaction();
                string dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                foreach (var item in addList)
                {
                    SqlCommand commandAdd = new SqlCommand(
                        "INSERT INTO [P_CHECK_REPORT_LINE_RM] values (@Market, @TERRITORY_TA, @Remarks, @createdate, @TerritoryCode_RM) ",
                        conn);
                    commandAdd.Transaction = tran;
                    commandAdd.Parameters.AddRange(
                        new SqlParameter[]
                        {
                            SqlParameterFactory.GetSqlParameter("@Market", item.Market.Trim()),
                            SqlParameterFactory.GetSqlParameter("@TERRITORY_TA", item.TERRITORY_TA.Trim()),
                            SqlParameterFactory.GetSqlParameter("@Remarks", string.IsNullOrEmpty(item.Remarks) ? "": item.Remarks.Trim()),
                            SqlParameterFactory.GetSqlParameter("@createdate", dt),
                            SqlParameterFactory.GetSqlParameter("@TerritoryCode_RM", item.TerritoryCode_RM.Trim())
                        }
                    );
                    commandAdd.ExecuteNonQuery();
                }
                foreach (var it in delList)
                {
                    SqlCommand commandUp = new SqlCommand(
                        "UPDATE [P_CHECK_REPORT_LINE_RM] SET [Remarks] = @Remarks, createdate = '" + dt + "'  "
                        + " WHERE  [TerritoryCode_RM] = @TerritoryCode_RM ",
                        conn);
                    commandUp.Transaction = tran;
                    commandUp.Parameters.AddRange(
                        new SqlParameter[]
                        {
                            SqlParameterFactory.GetSqlParameter("@Remarks", string.IsNullOrEmpty(it.Remarks) ? "": it.Remarks.Trim()),
                            SqlParameterFactory.GetSqlParameter("@TerritoryCode_RM", it.TerritoryCode_RM.Trim())
                        }
                    );
                    commandUp.ExecuteNonQuery();
                }
                tran.Commit();
            }
            return 1;
        }
        #endregion

        #region 获取变量临时表数据
        /// <summary>
        /// 获取变量临时表数据
        /// </summary>
        /// <param name="nowDate"></param>
        /// <returns></returns>
        public List<Temp_Hospital_Variables> LoadVariablesTemp()
        {
            List<Temp_Hospital_Variables> rtnData;
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<Temp_Hospital_Variables>("SELECT * FROM [dbo].[Temp_Hospital_Variables]  ",
                new SqlParameter[]
                {
                });
            }
            return rtnData;
        }
        #endregion

        #region 获取变量临时表非删除数据
        /// <summary>
        /// 获取变量临时表非删除数据
        /// </summary>
        /// <param name="nowDate"></param>
        /// <returns></returns>
        public List<Temp_Hospital_Variables> LoadVariablesTempNoDelete()
        {
            List<Temp_Hospital_Variables> rtnData;
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<Temp_Hospital_Variables>("SELECT * FROM [dbo].[Temp_Hospital_Variables] where [action]<>6  ",
                new SqlParameter[]
                {
                });
            }
            return rtnData;
        }
        #endregion

        #region 获取变量临时表不同数据
        /// <summary>
        /// 获取变量临时表不同数据
        /// </summary>
        /// <param name="ACTION"></param>
        /// <returns></returns>
        public List<Temp_Hospital_Variables> LoadVariablesTempAddData(string ACTION)
        {
            List<Temp_Hospital_Variables> rtnData;
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<Temp_Hospital_Variables>("SELECT * FROM [dbo].[Temp_Hospital_Variables] where [action]==@ACTION ",
                new SqlParameter[]
                {
                     SqlParameterFactory.GetSqlParameter("@ACTION", ACTION)
                });
            }
            return rtnData;
        }
        #endregion

        #region 删除地址
        public int DeleteAddress(P_HOSPITAL hospital)
        {
            var sqlServerTemplNonHT = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            AdminUser adminUser = HttpContext.Current.Session[ConstantHelper.CurrentAdminUser] as AdminUser;
            string dt = DateTime.Now.ToString();
            using (var conn = sqlServerTemplNonHT.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                //获取新增地址详细信息
                P_AddressApproval p_AddressApproval = sqlServerTemplNonHT.Find<P_AddressApproval>($"select * from [P_Addressapproval] where createdate = @createdate", new SqlParameter[]
                        {
                        SqlParameterFactory.GetSqlParameter("@createdate", hospital.CreateDate)
                        });

                var tran = conn.BeginTransaction();
                //更新主数据表中新增地址状态
                SqlCommand commandDel = new SqlCommand(
                    "UPDATE [p_hospital] SET [IsDelete] = 1 "
                    + " WHERE  [ID] = @ID ",
                    conn);
                commandDel.Transaction = tran;
                commandDel.Parameters.AddRange(
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", hospital.ID)
                    }
                );
                commandDel.ExecuteNonQuery();

                //添加审批记录
                SqlCommand commandAdd = new SqlCommand(
                        "INSERT INTO [P_AddressApproveHistory] values (newid(), @DA_ID, @UserName, @UserId, @ActionType, @ApproveDate, @Comments, @Type, @IsDelete) ",
                        conn);
                commandAdd.Transaction = tran;
                commandAdd.Parameters.AddRange(
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@DA_ID", p_AddressApproval.ID),
                        SqlParameterFactory.GetSqlParameter("@UserName", adminUser.Name),
                        SqlParameterFactory.GetSqlParameter("@UserId", adminUser.Email),
                        SqlParameterFactory.GetSqlParameter("@ActionType", 11),
                        SqlParameterFactory.GetSqlParameter("@ApproveDate", dt),
                        SqlParameterFactory.GetSqlParameter("@Comments", ""),
                        SqlParameterFactory.GetSqlParameter("@Type", 0),
                        SqlParameterFactory.GetSqlParameter("@IsDelete", 0)
                    }
                );
                commandAdd.ExecuteNonQuery();

                //更新外送地址申请表 IsDeleteUpdate&DeleteDate
                SqlCommand commandUp = new SqlCommand(
                    "UPDATE [P_AddressApproval] SET [IsDeleteUpdate] = 1, DeleteDate = '" + dt + "'  "
                    + " WHERE  [ID] = @ID ",
                    conn);
                commandUp.Transaction = tran;
                commandUp.Parameters.AddRange(
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", p_AddressApproval.ID)
                    }
                );
                commandUp.ExecuteNonQuery();

                //添加变量记录
                SqlCommand commandVar = new SqlCommand(
                    "INSERT INTO [P_Hospital_Variables_Data] values (@GskHospital, @Province, @City, @HospitalName, @Address, @IsMainAdd, @Market, @Longitude, @Latitude, @DistrictCode, @District, @Remarks, @Action, @CreateDate, @CreateBy) ",
                        conn);
                commandVar.Transaction = tran;
                commandVar.Parameters.AddRange(
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@GskHospital", p_AddressApproval.GskHospital),
                        SqlParameterFactory.GetSqlParameter("@Province", p_AddressApproval.Province),
                        SqlParameterFactory.GetSqlParameter("@City", p_AddressApproval.City),
                        SqlParameterFactory.GetSqlParameter("@HospitalName", p_AddressApproval.HospitalName),
                        SqlParameterFactory.GetSqlParameter("@Address", p_AddressApproval.AddAddress),
                        SqlParameterFactory.GetSqlParameter("@IsMainAdd", p_AddressApproval.AddressName),
                        SqlParameterFactory.GetSqlParameter("@Market", p_AddressApproval.Market),
                        SqlParameterFactory.GetSqlParameter("@Longitude", p_AddressApproval.Longitude),
                        SqlParameterFactory.GetSqlParameter("@Latitude", p_AddressApproval.Latitude),
                        SqlParameterFactory.GetSqlParameter("@DistrictCode", ""),
                        SqlParameterFactory.GetSqlParameter("@District", p_AddressApproval.District),
                        SqlParameterFactory.GetSqlParameter("@Remarks", "删除外送地址"),
                        SqlParameterFactory.GetSqlParameter("@Action", 6),
                        SqlParameterFactory.GetSqlParameter("@CreateDate", dt),
                        SqlParameterFactory.GetSqlParameter("@CreateBy", adminUser.Name)
                    }
                );
                commandVar.ExecuteNonQuery();

                tran.Commit();
            }
            return 1;
        }
        #endregion

        #region 医院数据summary
        public List<P_Brand_Coverage_Count> LoadBrandCoverageCount()
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var coon = sqlServerTemplate.GetSqlConnection())
            {
                coon.Open();
                var list = sqlServerTemplate.Load<P_Brand_Coverage_Count>($"select b.TotalCount,a.GskHospital,a.Type from P_HOSPITAL a "
                                                                    + " left join(select GskHospital, sum(totalcount) as TotalCount from[P_HOSPITAL_RANGE_RESTAURANTCOUNT] group by GskHospital) b on a.HospitalCode = b.GskHospital "
                                                                    + " where a.IsDelete = 0 and a.Address<>N'院外' order by TotalCount",
                    new SqlParameter[]
                    {
                    });
                return list;
            }
        }

        public List<P_Brand_Coverage_Count> LoadBrandCoverageCountOH()
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var coon = sqlServerTemplate.GetSqlConnection())
            {
                coon.Open();
                var list = sqlServerTemplate.Load<P_Brand_Coverage_Count>($"select b.TotalCount,a.GskHospital,a.Type from P_HOSPITAL a "
                                                                    + " left join(select GskHospital, sum(totalcount) as TotalCount from[P_HOSPITAL_RANGE_RESTAURANTCOUNT] group by GskHospital) b on a.HospitalCode = b.GskHospital "
                                                                    + " where a.IsDelete = 0 and a.Address = N'院外' order by TotalCount",
                    new SqlParameter[]
                    {
                    });
                return list;
            }
        }

        public List<P_HOSPITAL> LoadHospital()
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var coon = sqlServerTemplate.GetSqlConnection())
            {
                coon.Open();
                var list = sqlServerTemplate.Load<P_HOSPITAL>($"select * from P_HOSPITAL ",
                    new SqlParameter[]
                    {
                    });
                return list;
            }
        }

        public List<P_TERRITORY_TA> LoadTerritoryTA()
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var coon = sqlServerTemplate.GetSqlConnection())
            {
                coon.Open();
                var list = sqlServerTemplate.Load<P_TERRITORY_TA>($"select TERRITORY_TA from [Territory_Hospital] where TERRITORY_TA is not null and TERRITORY_TA <> '' group by TERRITORY_TA ORDER BY TERRITORY_TA ",
                    new SqlParameter[]
                    {
                    });
                return list;
            }
        }

        public List<P_TA_HOSPITAL> LoadTAHospital()
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var coon = sqlServerTemplate.GetSqlConnection())
            {
                coon.Open();
                var list = sqlServerTemplate.Load<P_TA_HOSPITAL>($"select a.TERRITORY_TA,b.Address,b.MainAddress from ( select distinct TERRITORY_TA,HospitalCode from [Territory_Hospital] ) a left join (select * from p_hospital where isdelete = 0) b on a.HospitalCode  = b.gskhospital where a.TERRITORY_TA is not null and a.TERRITORY_TA <> '' and b.address is not null and b.MainAddress is not null ",
                    new SqlParameter[]
                    {
                    });
                return list;
            }
        }

        public List<P_TA_HOSPITAL> LoadTAHospitalOH()
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var coon = sqlServerTemplate.GetSqlConnection())
            {
                coon.Open();
                var list = sqlServerTemplate.Load<P_TA_HOSPITAL>($"select a.TERRITORY_TA,b.Address,b.MainAddress from ( select distinct TERRITORY_TA,HospitalCode from [Territory_Hospital] ) a left join (select * from p_hospital where isdelete = 0 and Address = N'院外') b on a.HospitalCode = REPLACE(b.GskHospital,'-OH','') where a.TERRITORY_TA is not null and a.TERRITORY_TA <> '' and b.address is not null and b.MainAddress is not null ",
                    new SqlParameter[]
                    {
                    });
                return list;
            }
        }

        public List<P_Brand_Coverage_Count_TA> LoadBrandCoverageCountTA()
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var coon = sqlServerTemplate.GetSqlConnection())
            {
                coon.Open();
                var list = sqlServerTemplate.Load<P_Brand_Coverage_Count_TA>($" select m.TotalCount,m.GskHospital,n.TERRITORY_TA from ( select b.TotalCount,a.GskHospital from P_HOSPITAL a "
                                                                    + " left join(select GskHospital, sum(totalcount) as TotalCount from[P_HOSPITAL_RANGE_RESTAURANTCOUNT] group by GskHospital) b on a.HospitalCode = b.GskHospital "
                                                                    + " where a.IsDelete = 0 and a.Address<>N'院外'  ) m left join (SELECT DISTINCT TERRITORY_TA,HospitalCode FROM Territory_Hospital) n on m.GskHospital = n.HospitalCode where TERRITORY_TA is not null and TERRITORY_TA <> '' order by m.TotalCount ",
                    new SqlParameter[]
                    {
                    });
                return list;
            }
        }

        public int InsertHospitalVariablesCountDataTA(List<P_TERRITORY_TA> p_TAs)
        {
            string dt = DateTime.Now.ToString();
            int num = 0;
            var sqlServerTemplNonHT = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplNonHT.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                foreach (P_TERRITORY_TA ta in p_TAs)
                {
                    sqlServerTemplNonHT.ExecuteNonQuery(" insert into P_Hospital_Variables_Count_TA ( Date, TA, HospitalCount )"
                                                        + " values ( '" + dt + "',  '" + ta.TERRITORY_TA.Trim() + "', "
                                                        + " (select count( distinct hospitalcode ) from [Territory_Hospital] where TERRITORY_TA = '" + ta.TERRITORY_TA.Trim() + "') ) ",
                            new SqlParameter[]
                            {
                            });

                    sqlServerTemplNonHT.ExecuteNonQuery(" insert into Territory_TA ( TERRITORY_TA )"
                                                        + " values ( '" + ta.TERRITORY_TA.Trim() + "' ) ",
                            new SqlParameter[]
                            {
                            });

                    num++;
                }
            }
            return num;
        }

        public List<P_TERRITORY_TA> LoadAllTerritoryTA()
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var coon = sqlServerTemplate.GetSqlConnection())
            {
                coon.Open();
                var list = sqlServerTemplate.Load<P_TERRITORY_TA>($"select distinct TERRITORY_TA from [Territory_TA]  ",
                    new SqlParameter[]
                    {
                    });
                return list;
            }
        }
        #endregion

        #region 同步医院表
        public int SyncHospital()
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnCount = sqlServerTemplate.ExecuteNonQuery(@"DELETE FROM P_HOSPITAL_COST WHERE COPYDATE >= @CopyDate;",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@CopyDate", DateTime.Now.ToString("yyyy-MM") + "-01")
                    });


                var rtnData = sqlServerTemplate.ExecuteNonQuery(@"INSERT INTO P_HOSPITAL_COST
                SELECT [CityId],[GskHospital],[Name],[FirstLetters],[Address],[Latitude],[Longitude],[Type],[External],[CreateDate],[ProvinceId],[IsXMS],[IsBDS],[IsMT],[Remark],[IsDelete],[OldGskHospital]
                ,[OldName],[RelateUserList],[MainAddress],[HospitalCode],@CopyDate FROM [P_HOSPITAL];",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@CopyDate", DateTime.Now)
                    });
                return rtnData;
            }
        }

        public int SyncHospitalDetail()
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnCount = sqlServerTemplate.ExecuteNonQuery(@"DELETE FROM P_HOSPITAL_DETAIL_COST WHERE COPYDATE >= @CopyDate;",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@CopyDate", DateTime.Now.ToString("yyyy-MM") + "-01")
                    });


                var rtnData = sqlServerTemplate.ExecuteNonQuery(@"INSERT INTO P_HOSPITAL_DETAIL_COST
                SELECT [GskHospital],[District],[DistrictCode],[CustomerType],[RESP],[HEP],[CNS],[HIV],[VOL],[MA],[Region],[IsDelete],[CreateDate],[UpdateDate],[IP],@CopyDate
                  FROM [P_HOSPITAL_DETAIL] ;",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@CopyDate", DateTime.Now)
                    });
                return rtnData;
            }
        }

        public int SyncTerritoryHospital()
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnCount = sqlServerTemplate.ExecuteNonQuery(@"DELETE FROM Territory_Hospital_COST WHERE COPYDATE >= @CopyDate;",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@CopyDate", DateTime.Now.ToString("yyyy-MM") + "-01")
                    });


                var rtnData = sqlServerTemplate.ExecuteNonQuery(@"INSERT INTO Territory_Hospital_COST
                SELECT [Market],[TERRITORY_MR],[MUD_ID_MR],[TERRITORY_DM],[MUD_ID_DM],[TERRITORY_RM],[MUD_ID_RM],[TERRITORY_RD],[MUD_ID_RD],[TERRITORY_TA],[MUD_ID_TA],[HospitalCode],[CreateDate],@CopyDate
                  FROM [Territory_Hospital] ;",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@CopyDate", DateTime.Now)
                    });
                return rtnData;
            }
        }

        public int SyncHospitalRange()
        {
            var Date = DateTime.Now.ToString("yyyyMM");
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();

                var rtnData = sqlServerTemplate.Find<P_COUNT>("SELECT COUNT(id) AS Count from sysObjects where Id=OBJECT_ID(N'P_HOSPITAL_RANGE_RESTAURANT_COST_"+ Date + "') and xtype='U';",
                    new SqlParameter[]
                    {
                        
                    });
                int rtnCount = 0;
                if(rtnData != null && rtnData.Count > 0)
                {
                    sqlServerTemplate.ExecuteNonQuery("DROP TABLE P_HOSPITAL_RANGE_RESTAURANT_COST_" + Date + ";",
                    new SqlParameter[]
                    {

                    });

                    rtnCount = sqlServerTemplate.ExecuteNonQuery("SELECT A.* INTO dbo.P_HOSPITAL_RANGE_RESTAURANT_COST_" + Date + " FROM P_HOSPITAL_RANGE_RESTAURANT A WHERE ResId IN (select b.restaurantCode from vxm_dev.dbo.hr_RestaurantBasicInfo B where B.status = 0) or  A.ResName like N'%堂食%';",
                    new SqlParameter[]
                    {
                        
                    });
                }
                else
                {
                    rtnCount = sqlServerTemplate.ExecuteNonQuery("SELECT A.* INTO dbo.P_HOSPITAL_RANGE_RESTAURANT_COST_" + Date + " FROM P_HOSPITAL_RANGE_RESTAURANT A WHERE ResId IN (select b.restaurantCode from vxm_dev.dbo.hr_RestaurantBasicInfo B where B.status = 1) or  A.ResName like N'%堂食%';",
                    new SqlParameter[]
                    {

                    });
                }

                return rtnCount;
            }
        }
        #endregion

        #region 获取RM列表
        public List<V_TerritoryRM> LoadTerritoryRMList(string TerritoryStr)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnData = sqlServerTemplate.Load<V_TerritoryRM>(@"SELECT DISTINCT TA_CODE FROM V_TerritoryRM WHERE USER_TA IN ('"+ TerritoryStr + "') ;",
                    new SqlParameter[]
                    {
                        
                    });
                return rtnData;
            }
        }
        #endregion

    }
}
