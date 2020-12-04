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
using MealAdmin.Entity.View;

namespace MealAdmin.Dao
{
    public class BaseDataDao : IBaseDataDao
    {
        [Bean("sqlServerTemplFactory")]
        public DbTemplateFactory sqlServerTemplFactory { get; set; }
        [Bean("sqlServerTemplFactoryNonHT")]
        public DbTemplateFactory sqlServerTemplFactoryNonHT { get; set; }

        #region 获取省份
        /// <summary>
        /// 获取省份
        /// </summary>
        /// <returns></returns>
        public List<P_PROVINCE> LoadProvince(string Type, string TA)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                if (string.IsNullOrEmpty(TA))
                {
                    var list = sqlServerTemplate.Load<P_PROVINCE>($"SELECT DISTINCT A.* FROM [P_PROVINCE] A JOIN P_HOSPITAL B ON A.ID = B.ProvinceId where A.Type=@Type", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@Type", Type),
                        SqlParameterFactory.GetSqlParameter("@TACode", TA)
                    });
                    return list;
                }else
                {
                    var list = sqlServerTemplate.Load<P_PROVINCE>($"SELECT DISTINCT A.* FROM [P_PROVINCE] A JOIN P_HOSPITAL B ON A.ID = B.ProvinceId JOIN V_Territory_Hospital C ON C.[HospitalCode] = B.GskHospital where A.Type=@Type AND (C.TA_CODE =@TACode or '' = @TACode)", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@Type", Type),
                        SqlParameterFactory.GetSqlParameter("@TACode", TA)
                    });
                    return list;
                }
                
            }
        }

        /// <summary>
        /// 获取省份
        /// </summary>
        /// <returns></returns>
        public List<P_PROVINCE> LoadProvinceByHos(string Type, string Mudid, string TerritoryCode)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<P_PROVINCE>($"SELECT DISTINCT A.* FROM [P_PROVINCE] A JOIN P_HOSPITAL B ON A.ID = B.ProvinceId JOIN V_Territory_Hospital C ON C.[HospitalCode] = B.GskHospital where A.Type=@Type AND C.MUDID =@Mudid AND C.TA_CODE = @TerritoryCode", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@Type", Type),
                        SqlParameterFactory.GetSqlParameter("@Mudid", Mudid),
                        SqlParameterFactory.GetSqlParameter("@TerritoryCode", TerritoryCode)
                    });
                return list;
            }
        }
        #endregion

        #region 获取省份
        /// <summary>
        /// 获取省份
        /// </summary>
        /// <returns></returns>
        public List<P_PROVINCE> LoadProvinceByMarketList(string Type)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<P_PROVINCE>($"SELECT * FROM [P_PROVINCE] where Type in " + Type + "", new SqlParameter[]
                    {

                    });
                return list;
            }
        }
        #endregion

        #region 获取全部城市
        /// <summary>
        /// 获取全部城市
        /// </summary>
        /// <returns></returns>
        public List<P_CITY> LoadCity(string Type)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<P_CITY>($"SELECT * FROM [P_CITY] where Type=@Type ", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@Type", Type)
                    });
                return list;
            }
        }

        /// <summary>
        /// 获取全部城市
        /// </summary>
        /// <returns></returns>
        public List<P_CITY> LoadCityByHos(string Type)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<P_CITY>($"SELECT DISTINCT A.* FROM [P_CITY] A JOIN P_HOSPITAL B ON A.ID = B.CityId WHERE A.Type=@Type ", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@Type", Type)
                    });
                return list;
            }
        }
        #endregion

        #region 获取全部城市
        /// <summary>
        /// 获取全部城市
        /// </summary>
        /// <returns></returns>
        public List<P_CITY> LoadCityByMarketList(string Type)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<P_CITY>($"SELECT * FROM [P_CITY] where Type in " + Type + " ", new SqlParameter[]
                    {
                    });
                return list;
            }
        }
        #endregion

        #region 根据省份获取城市
        /// <summary>
        /// 根据省份获取城市
        /// </summary>
        /// <returns></returns>
        public List<P_CITY> LoadCity(int provinceId, string Type, string TA)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                if (string.IsNullOrEmpty(TA))
                {
                    var list = sqlServerTemplate.Load<P_CITY>($"SELECT DISTINCT C.* FROM [P_CITY] C JOIN [P_HOSPITAL] H ON H.CityId = C.ID WHERE C.[ProvinceId]=@ProvinceId AND H.Type=@Type",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ProvinceId", provinceId),
                        SqlParameterFactory.GetSqlParameter("@Type", Type),
                        SqlParameterFactory.GetSqlParameter("@TACode", TA)
                    });
                    return list;
                }else
                {
                    var list = sqlServerTemplate.Load<P_CITY>($"SELECT DISTINCT C.* FROM [P_CITY] C JOIN [P_HOSPITAL] H ON H.CityId = C.ID JOIN V_Territory_Hospital TH ON H.GskHospital = TH.HospitalCode WHERE C.[ProvinceId]=@ProvinceId AND H.Type=@Type AND (TH.TA_CODE =@TACode or '' = @TACode)",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ProvinceId", provinceId),
                        SqlParameterFactory.GetSqlParameter("@Type", Type),
                        SqlParameterFactory.GetSqlParameter("@TACode", TA)
                    });
                    return list;
                }
                
            }
        }

        /// <summary>
        /// 根据省份获取城市
        /// </summary>
        /// <returns></returns>
        public List<P_CITY> LoadCityByHos(int provinceId, string Type, string UserId, string TerritoryCode)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<P_CITY>($"SELECT DISTINCT C.* FROM [P_CITY] C JOIN [P_HOSPITAL] H ON H.CityId = C.ID JOIN V_Territory_Hospital TH ON H.GskHospital = TH.HospitalCode WHERE C.[ProvinceId]=@ProvinceId AND H.[Type]=@market AND TH.[MUDID] =@MUDID AND TH.[TA_CODE] = @TerritoryCode ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ProvinceId", provinceId),
                        SqlParameterFactory.GetSqlParameter("@market", Type),
                        SqlParameterFactory.GetSqlParameter("@MUDID", UserId),
                        SqlParameterFactory.GetSqlParameter("@TerritoryCode", TerritoryCode)
                    });
                return list;
            }
        }
        #endregion

        #region 根据主键查城市
        /// <summary>
        /// 根据主键查城市
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public P_CITY FindCity(int id)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Find<P_CITY>($"SELECT TOP 1 * FROM [P_CITY] WHERE [ID]=@ID",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", id)
                    });
                return list;
            }
        }
        #endregion

        #region 根据城市获取医院
        /// <summary>
        /// 根据城市获取医院
        /// </summary>
        /// <param name="ciytId"></param>
        /// <param name="market"></param>
        /// <returns></returns>
        public List<P_HOSPITAL> LoadHospital(int ciytId, string market, string TA)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                if (string.IsNullOrEmpty(TA))
                {
                    var list = sqlServerTemplate.Load<P_HOSPITAL>($"SELECT H.* FROM [P_HOSPITAL] H  WHERE IsDelete <>1 and IsDelete <>2 and IsDelete <>3 AND [CityId]=@CityId AND [Type]=@market "
                    + " ORDER BY HospitalCode,MainAddress,Name ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@CityId", ciytId),
                        SqlParameterFactory.GetSqlParameter("@market", market),
                        SqlParameterFactory.GetSqlParameter("@TACode", TA)
                    });
                    return list;
                }else
                {
                    var list = sqlServerTemplate.Load<P_HOSPITAL>($"SELECT DISTINCT H.* FROM [P_HOSPITAL] H JOIN V_Territory_Hospital TH ON H.GskHospital = TH.HOSPITALCODE OR H.GskHospital = TH.HOSPITALCODE + '-OH' WHERE IsDelete <>1 and IsDelete <>2 and IsDelete <>3 AND [CityId]=@CityId AND [Type]=@market  AND (TH.TA_CODE =@TACode or '' = @TACode) "
                    + " ORDER BY HospitalCode,MainAddress,Name ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@CityId", ciytId),
                        SqlParameterFactory.GetSqlParameter("@market", market),
                        SqlParameterFactory.GetSqlParameter("@TACode", TA)
                    });
                    return list;
                }
            }
        }

        public List<P_HOSPITAL> LoadHospitalByTaUser(int ciytId, string market, string userid, string territoryCode)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<P_HOSPITAL>($"SELECT H.* FROM [P_HOSPITAL] H JOIN V_Territory_Hospital TH ON H.GskHospital = TH.HOSPITALCODE OR H.GskHospital = TH.HOSPITALCODE + '-OH' WHERE IsDelete <>1 and IsDelete <>2 and IsDelete <>3 AND [CityId]=@CityId AND [Type]=@market AND TH.[MUDID] =@MUDID AND TH.[TA_CODE] = @TerritoryCode "
                    + " ORDER BY HospitalCode,MainAddress,Name ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@CityId", ciytId),
                        SqlParameterFactory.GetSqlParameter("@market", market),
                        SqlParameterFactory.GetSqlParameter("@MUDID", userid),
                        SqlParameterFactory.GetSqlParameter("@TerritoryCode", territoryCode)
                    });
                return list;
            }
        }
        #endregion

        #region 根据关键字搜索医院
        /// <summary>
        /// 根据关键字搜索医院
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="market"></param>
        /// <returns></returns>
        public List<P_HOSPITAL> SearchHospital(string keyword, int province, int city, string market, string TA)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                if (string.IsNullOrEmpty(TA))
                {
                    var list = sqlServerTemplate.Load<P_HOSPITAL>(
                    "SELECT DISTINCT TOP 20 a.*, b.ProvinceId FROM [P_HOSPITAL] a LEFT JOIN [P_CITY] b ON a.CityId=b.ID and a.Type=b.Type"
                    + " WHERE a.IsDelete <>1 and a.IsDelete <>2 and a.IsDelete <>3 AND (a.Name LIKE '" + keyword + "' OR a.FirstLetters LIKE '" + keyword + "' ) AND (a.CityId = @city OR -1 = @city) AND (b.ProvinceId = @province OR -1 = @province) AND a.Type=@market AND a.Type=@market "
                    + " ORDER BY a.HospitalCode,a.MainAddress, a.Name ",
                    new SqlParameter[]
                    {
                        //SqlParameterFactory.GetSqlParameter("@keyword", keyword),
                        SqlParameterFactory.GetSqlParameter("@city", city),
                        SqlParameterFactory.GetSqlParameter("@province", province),
                        SqlParameterFactory.GetSqlParameter("@market", market),
                        SqlParameterFactory.GetSqlParameter("@TACode", TA)
                    });
                    return list;
                }
                else
                {
                    var list = sqlServerTemplate.Load<P_HOSPITAL>(
                    "SELECT DISTINCT TOP 20 a.*, b.ProvinceId FROM [P_HOSPITAL] a LEFT JOIN [P_CITY] b ON a.CityId=b.ID and a.Type=b.Type JOIN V_Territory_Hospital TH ON a.GskHospital = TH.HOSPITALCODE OR a.GskHospital = TH.HOSPITALCODE + '-OH' "
                    + " WHERE a.IsDelete <>1 and a.IsDelete <>2 and a.IsDelete <>3 AND (a.Name LIKE '" + keyword + "' OR a.FirstLetters LIKE '" + keyword + "' ) AND (a.CityId = @city OR -1 = @city) AND (b.ProvinceId = @province OR -1 = @province) AND a.Type=@market AND (TH.TA_CODE =@TACode or '' = @TACode) "
                    + " ORDER BY a.HospitalCode,a.MainAddress, a.Name ",
                    new SqlParameter[]
                    {
                        //SqlParameterFactory.GetSqlParameter("@keyword", keyword),
                        SqlParameterFactory.GetSqlParameter("@city", city),
                        SqlParameterFactory.GetSqlParameter("@province", province),
                        SqlParameterFactory.GetSqlParameter("@market", market),
                        SqlParameterFactory.GetSqlParameter("@TACode", TA)
                    });
                    return list;
                }
            }
        }

        public List<P_HOSPITAL> SearchHospitalByTA(string keyword, int province, int city, string market, string userid, string TerritoryCode)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<P_HOSPITAL>(
                    "SELECT DISTINCT TOP 20 a.*, b.ProvinceId FROM [P_HOSPITAL] a LEFT JOIN [P_CITY] b ON a.CityId=b.ID and a.Type=b.Type join V_Territory_Hospital c on (a.GskHospital = c.HOSPITALCODE OR a.GskHospital = c.HOSPITALCODE + '-OH') and a.Type = c.Market "
                    + " WHERE a.IsDelete <>1 and a.IsDelete <>2 and a.IsDelete <>3 AND (a.Name LIKE '" + keyword + "' OR a.FirstLetters LIKE '" + keyword + "' ) AND (a.CityId = @city OR -1 = @city) AND (b.ProvinceId = @province OR -1 = @province) AND a.Type=@market  AND c.MUDID = @userid AND c.TA_CODE = @TerritoryCode "
                    + " ORDER BY a.HospitalCode,a.MainAddress, a.Name ",
                    new SqlParameter[]
                    {
                        //SqlParameterFactory.GetSqlParameter("@keyword", keyword),
                        SqlParameterFactory.GetSqlParameter("@city", city),
                        SqlParameterFactory.GetSqlParameter("@province", province),
                        SqlParameterFactory.GetSqlParameter("@market", market),
                        SqlParameterFactory.GetSqlParameter("@userid", userid),
                        SqlParameterFactory.GetSqlParameter("@TerritoryCode", TerritoryCode)
                    });
                return list;
            }
        }
        #endregion

        #region 根据医院编码搜索医院
        /// <summary>
        /// 根据医院编码搜索医院
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="market"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<P_HOSPITAL> SearchHospitalByCode(string keyword, int province, int city, string market, string TA)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                if(string.IsNullOrEmpty(TA))
                {
                    var list = sqlServerTemplate.Load<P_HOSPITAL>(
                    "SELECT DISTINCT TOP 20 a.*, b.ProvinceId FROM [P_HOSPITAL] a LEFT JOIN [P_CITY] b ON a.CityId=b.ID and a.Type=b.Type "
                    + " WHERE a.IsDelete <>1 and a.IsDelete <>2 and a.IsDelete <>3 AND (a.GskHospital LIKE '" + keyword + "') AND (a.CityId = @city OR -1 = @city) AND (b.ProvinceId = @province OR -1 = @province) AND a.Type=@market "
                    + " ORDER BY a.HospitalCode,a.MainAddress,a.GskHospital ",
                    new SqlParameter[]
                    {
                        //SqlParameterFactory.GetSqlParameter("@keyword", keyword),
                        SqlParameterFactory.GetSqlParameter("@city", city),
                        SqlParameterFactory.GetSqlParameter("@province", province),
                        SqlParameterFactory.GetSqlParameter("@market", market),
                        SqlParameterFactory.GetSqlParameter("@TACode", TA)
                    });
                    return list;
                }else
                {
                    var list = sqlServerTemplate.Load<P_HOSPITAL>(
                    "SELECT DISTINCT TOP 20 a.*, b.ProvinceId FROM [P_HOSPITAL] a LEFT JOIN [P_CITY] b ON a.CityId=b.ID and a.Type=b.Type JOIN V_Territory_Hospital TH ON a.GskHospital = TH.HOSPITALCODE OR a.GskHospital = TH.HOSPITALCODE + '-OH' "
                    + " WHERE a.IsDelete <>1 and a.IsDelete <>2 and a.IsDelete <>3 AND (a.GskHospital LIKE '" + keyword + "') AND (a.CityId = @city OR -1 = @city) AND (b.ProvinceId = @province OR -1 = @province) AND a.Type=@market AND TH.TA_CODE =@TACode "
                    + " ORDER BY a.HospitalCode,a.MainAddress,a.GskHospital ",
                    new SqlParameter[]
                    {
                        //SqlParameterFactory.GetSqlParameter("@keyword", keyword),
                        SqlParameterFactory.GetSqlParameter("@city", city),
                        SqlParameterFactory.GetSqlParameter("@province", province),
                        SqlParameterFactory.GetSqlParameter("@market", market),
                        SqlParameterFactory.GetSqlParameter("@TACode", TA)
                    });
                    return list;
                }
            }
        }

        public List<P_HOSPITAL> SearchHospitalByCodeTA(string keyword, int province, int city, string market, string userid, string TerritoryCode)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<P_HOSPITAL>(
                    "SELECT DISTINCT TOP 20 a.*, b.ProvinceId FROM [P_HOSPITAL] a LEFT JOIN [P_CITY] b ON a.CityId=b.ID and a.Type=b.Type join V_Territory_Hospital c on (a.GskHospital = c.HospitalCode or a.GskHospital = c.HospitalCode + '-OH') and a.Type = c.Market "
                    + " WHERE a.IsDelete <>1 and a.IsDelete <>2 and a.IsDelete <>3 AND (a.HospitalCode LIKE '"+ keyword +"') AND (a.CityId = @city OR -1 = @city) AND (b.ProvinceId = @province OR -1 = @province) AND a.Type=@market AND c.MUDID = @userid AND c.TA_CODE = @TerritoryCode "
                    + " ORDER BY a.HospitalCode,a.MainAddress,a.GskHospital ",
                    new SqlParameter[]
                    {
                        //SqlParameterFactory.GetSqlParameter("@keyword", keyword),
                        SqlParameterFactory.GetSqlParameter("@city", city),
                        SqlParameterFactory.GetSqlParameter("@province", province),
                        SqlParameterFactory.GetSqlParameter("@market", market),
                        SqlParameterFactory.GetSqlParameter("@userid", userid),
                        SqlParameterFactory.GetSqlParameter("@TerritoryCode", TerritoryCode)
                    });
                conn.Close();
                return list;
            }
        }
        #endregion

        #region 根据id查询医院
        /// <summary>
        /// 根据id查询医院
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <returns></returns>
        public P_HOSPITAL FindHospital(string hospitalId)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Find<P_HOSPITAL>($"SELECT TOP 1 * FROM [P_HOSPITAL] WHERE [GskHospital]=@GskHospital ORDER BY Name ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@GskHospital", hospitalId)
                    });
            }
        }
        #endregion

        #region 同步基础数据
        /// <summary>
        /// 同步基础数据
        /// </summary>
        /// <returns></returns>
        public int SyncBaseData(List<P_PROVINCE> listProvince, List<P_CITY> listCity, List<P_HOSPITAL> listHospital)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                var tran = conn.BeginTransaction();

                string sqlDelProvince = "delete P_PROVINCE";
                SqlCommand comDelProvince = new SqlCommand(sqlDelProvince, conn);
                comDelProvince.Transaction = tran;
                comDelProvince.ExecuteNonQuery();
                string sqlDelCity = "delete P_CITY";
                SqlCommand comDelCity = new SqlCommand(sqlDelCity, conn);
                comDelCity.Transaction = tran;
                comDelCity.ExecuteNonQuery();
                string sqlDelHospital = "delete P_HOSPITAL";
                SqlCommand comDelHospital = new SqlCommand(sqlDelHospital, conn);
                comDelHospital.Transaction = tran;
                comDelHospital.ExecuteNonQuery();


                foreach (var province in listProvince)
                {
                    string sqlProvince = "INSERT INTO P_PROVINCE VALUES(@ID,@Name,@PinYin,@CreateDate)";
                    SqlCommand commandProvince = new SqlCommand(sqlProvince, conn);
                    commandProvince.Transaction = tran;
                    commandProvince.Parameters.AddRange(new SqlParameter[]
                        {
                            SqlParameterFactory.GetSqlParameter("@ID", province.ID),
                            SqlParameterFactory.GetSqlParameter("@Name", province.Name),
                            SqlParameterFactory.GetSqlParameter("@PinYin", province.PinYin),
                            SqlParameterFactory.GetSqlParameter("@CreateDate", province.CreateDate)
                        });
                    commandProvince.ExecuteNonQuery();
                }

                foreach (var city in listCity)
                {
                    string sqlCity = "INSERT INTO P_CITY VALUES(@ID,@ProvinceId,@Name,@PinYin,@CreateDate)";
                    SqlCommand commandCity = new SqlCommand(sqlCity, conn);
                    commandCity.Transaction = tran;
                    commandCity.Parameters.AddRange(new SqlParameter[]
                        {
                            SqlParameterFactory.GetSqlParameter("@ID", city.ID),
                            SqlParameterFactory.GetSqlParameter("@ProvinceId", city.ProvinceId),
                            SqlParameterFactory.GetSqlParameter("@Name", city.Name),
                            SqlParameterFactory.GetSqlParameter("@PinYin", city.PinYin),
                            SqlParameterFactory.GetSqlParameter("@CreateDate", city.CreateDate)
                        });
                    commandCity.ExecuteNonQuery();
                }

                foreach (var hospital in listHospital)
                {
                    string sqlHospital = "INSERT INTO P_HOSPITAL(ID,CityId,GskHospital,Name,FirstLetters,Address,Latitude,Longitude,Type,[External],CreateDate) VALUES(@ID,@CityId,@GskHospital,@Name,@FirstLetters,@Address,@Latitude,@Longitude,@Type,@External,@CreateDate)";
                    SqlCommand commandHospital = new SqlCommand(sqlHospital, conn);
                    commandHospital.Transaction = tran;
                    commandHospital.Parameters.AddRange(new SqlParameter[]
                       {
                            SqlParameterFactory.GetSqlParameter("@ID", hospital.ID),
                            SqlParameterFactory.GetSqlParameter("@CityId", hospital.CityId),
                            SqlParameterFactory.GetSqlParameter("@GskHospital", hospital.GskHospital),
                            SqlParameterFactory.GetSqlParameter("@Name", hospital.Name),
                            SqlParameterFactory.GetSqlParameter("@FirstLetters", hospital.FirstLetters),
                            SqlParameterFactory.GetSqlParameter("@Address", hospital.Address),
                            SqlParameterFactory.GetSqlParameter("@Latitude", hospital.Latitude),
                            SqlParameterFactory.GetSqlParameter("@Longitude", hospital.Longitude),
                            SqlParameterFactory.GetSqlParameter("@Type", hospital.Type),
                            SqlParameterFactory.GetSqlParameter("@External", hospital.External),
                            SqlParameterFactory.GetSqlParameter("@CreateDate", hospital.CreateDate)
                       });
                    commandHospital.ExecuteNonQuery();
                }
                tran.Commit();
            }
            return 1;
        }
        #endregion

        #region 增加省
        /// <summary>
        /// 增加省
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pinyin"></param>
        /// <param name="market"></param>
        /// <returns></returns>
        public int AddProvince(string name, string pinyin,string market)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery(
                    "INSERT INTO [P_PROVINCE] ([Name], [PinYin],[Type], [CreateDate]) VALUES (@Name, @PinYin,@Type,@CreateDate)",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@Name", name),
                        SqlParameterFactory.GetSqlParameter("@PinYin", pinyin),
                        SqlParameterFactory.GetSqlParameter("@Type", market),
                        SqlParameterFactory.GetSqlParameter("@CreateDate", DateTime.Now),
                    });
            }
        }
        #endregion

        #region 增加省
        /// <summary>
        /// 增加省
        /// </summary>
        /// <param name="listProvince"></param>
        /// <returns></returns>
        public int AddProvince(List<P_PROVINCE> listProvince)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                foreach (var item in listProvince)
                {
                    sqlServerTemplate.ExecuteNonQuery(
                        "INSERT INTO [P_PROVINCE] ([Name], [PinYin],[Type], [CreateDate]) VALUES (@Name, @PinYin,@Type, @CreateDate)",
                        new SqlParameter[]
                        {
                            SqlParameterFactory.GetSqlParameter("@Name", item.Name),
                            SqlParameterFactory.GetSqlParameter("@PinYin", item.PinYin),
                            SqlParameterFactory.GetSqlParameter("@Type", item.Type),
                            SqlParameterFactory.GetSqlParameter("@CreateDate", DateTime.Now),
                        });
                }


                return 1;
            }
        }
        #endregion

        #region 增加市
        /// <summary>
        /// 增加市
        /// </summary>
        /// <param name="provinceId"></param>
        /// <param name="name"></param>
        /// <param name="pinyin"></param>
        /// <returns></returns>
        public int AddCity(int provinceId, string name, string pinyin,string market)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery(
                    "INSERT INTO [P_CITY] ([ProvinceId], [Name], [PinYin],[Type], [CreateDate]) VALUES (@ProvinceId, @Name, @PinYin,@Type, @CreateDate)",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ProvinceId", provinceId),
                        SqlParameterFactory.GetSqlParameter("@Name", name),
                        SqlParameterFactory.GetSqlParameter("@PinYin", pinyin),
                        SqlParameterFactory.GetSqlParameter("@Type", market),
                        SqlParameterFactory.GetSqlParameter("@CreateDate", DateTime.Now),
                    });
            }
        }
        #endregion

        #region 增加市
        /// <summary>
        /// 增加市
        /// </summary>
        /// <param name="listCity"></param>
        /// <returns></returns>
        public int AddCity(List<P_CITY> listCity)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                foreach (var item in listCity)
                {
                    sqlServerTemplate.ExecuteNonQuery(
                        "INSERT INTO [P_CITY] ([ProvinceId], [Name], [PinYin],[Type], [CreateDate]) VALUES (@ProvinceId, @Name, @PinYin,@Type, @CreateDate)",
                        new SqlParameter[]
                        {
                            SqlParameterFactory.GetSqlParameter("@ProvinceId", item.ProvinceId),
                            SqlParameterFactory.GetSqlParameter("@Name", item.Name),
                            SqlParameterFactory.GetSqlParameter("@PinYin", item.PinYin),
                            SqlParameterFactory.GetSqlParameter("@Type", item.Type),
                            SqlParameterFactory.GetSqlParameter("@CreateDate", DateTime.Now)
                        });
                }

                return 1;
            }
        }
        #endregion

        #region 插入JsAPI Tickets
        /// <summary>
        /// 插入JsAPI Tickets
        /// </summary>
        /// <param name="listCity"></param>
        /// <returns></returns>
        public int UpdateJsApiTicket(string Signature, string Timestamp)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                sqlServerTemplate.ExecuteNonQuery(
                        "update [P_JsApiTicket] set [Signature]=@Signature,[Timestamp]=@Timestamp",
                        new SqlParameter[]
                        {
                            SqlParameterFactory.GetSqlParameter("@Signature", Signature),
                            SqlParameterFactory.GetSqlParameter("@Timestamp", Timestamp)
                        });

                return 1;
            }
        }
        #endregion

        #region 获取JsAPI Tickets
        /// <summary>
        /// 插入JsAPI Tickets
        /// </summary>
        /// <param name="listCity"></param>
        /// <returns></returns>
        public P_JsApiTicket GetJsApiTicket()
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Find<P_JsApiTicket>($"SELECT * FROM [P_JsApiTicket]",
                    new SqlParameter[]
                    {
                        
                    });
            }
        }
        #endregion

        #region 获取黑名单餐厅列表
        /// <summary>
        /// 获取黑名单餐厅列表
        /// </summary>
        /// <param name="listCity"></param>
        /// <returns></returns>
        public List<V_RestaurantState> LoadRestaurantState()
        {
            //特别注意
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Load<V_RestaurantState>($"SELECT * FROM [V_RestaurantState]",
                    new SqlParameter[]
                    {

                    });
            }
        }

        #endregion

        #region 判断一线城市
        /// <summary>
        /// 判断一线城市
        /// </summary>
        /// <param name="cityID"></param>
        /// <returns></returns>
        public P_CITY_OUT FindCityBudget(string hospitalId)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                var res = sqlServerTemplate.Find<P_CITY_OUT>($"SELECT h.[External] AS IsOut,l.CityName AS CityName FROM P_HOSPITAL h LEFT JOIN P_CITY c on h.CityId = c.ID" +
                    " LEFT JOIN P_CITY_LIST l on l.CityName like('%' + CONVERT(NVARCHAR(50), c.Name) + '%')" +
                    " WHERE h.[GskHospital]=@GskHospital ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@GskHospital", hospitalId)
                    });
                return res;
            }
        }
        #endregion

        #region 分页查询一线城市
        /// <summary>
        /// 分页查询一线城市
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<P_CITY_LIST> LoadBiggestCity(string key,int rows, int page, out int total)
        {
            var sqlServerTemplNonHT = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplNonHT.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplNonHT.LoadPages<P_CITY_LIST>(rows, page, out total,
                    $"SELECT * FROM [P_CITY_LIST] Where (@name='' or ProvinceName like '%" + key + "%') or (@name='' or CityName like '%"+ key + "%') ",
                    " ORDER BY CreateDate DESC  ", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@name", key)
                    });
                return list;
            }
        }
        #endregion

        #region 城市列表导入
        /// <summary>
        /// 城市列表导入
        /// </summary>
        /// <param name="listCity"></param>
        /// <param name="fails"></param>
        /// <returns></returns>
        public int Import(List<P_CITY_LIST> list)
        {
            var sqlServerTemplNonHT = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplNonHT.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                sqlServerTemplNonHT.ExecuteNonQuery("TRUNCATE TABLE P_CITY_LIST ");

                var tran = conn.BeginTransaction();

                foreach (var item in list)
                {
                    SqlCommand commandAdd = new SqlCommand(
                        "INSERT INTO [P_CITY_LIST] ([ID] ,[ProvinceName],[CityName],[Rank],[CreateDate]) "
                        + " VALUES (@ID ,@ProvinceName,@CityName,@Rank,@CreateDate) ", conn);
                    commandAdd.Transaction = tran;
                    commandAdd.Parameters.AddRange(
                        new SqlParameter[]
                        {
                            SqlParameterFactory.GetSqlParameter("@ID", item.ID),
                            SqlParameterFactory.GetSqlParameter("@ProvinceName", item.ProvinceName),
                            SqlParameterFactory.GetSqlParameter("@CityName", item.CityName),
                            SqlParameterFactory.GetSqlParameter("@Rank", item.Rank),
                            SqlParameterFactory.GetSqlParameter("@CreateDate", item.CreateDate)
                        }
                    );
                    commandAdd.ExecuteNonQuery();
                }
                tran.Commit();
            }
            return 1;
        }
        #endregion

        //获取RM的TERRITORYCODE
        public V_TerritoryRM FindTerritoryDM(string TerritoryCode)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Find<V_TerritoryRM>($"SELECT TOP 1 * FROM [V_TerritoryDM] WHERE USER_TA = @TerritoryCode ORDER BY TA_CODE ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@TerritoryCode", TerritoryCode)
                    });
            }
        }

        #region 新增地址
        public List<P_HOSPITAL_NEW> SearchHospitalByGskHospital(string gskHospital)
        {
            List<P_HOSPITAL_NEW> list_p_hospital_new = new List<P_HOSPITAL_NEW>();
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<P_HOSPITAL_NEW>(
                    "SELECT TOP 20 a.*, b.Name AS CityName,c.Name AS ProvinceName,d.ApplierName,d.ApplierMUDID,ISNULL(d.ApprovalStatus,99) AS ApprovalStatus,d.ID AS DA_ID FROM [P_HOSPITAL] a LEFT JOIN [P_CITY] b ON a.CityId=b.ID and a.Type=b.Type "
                    + " left join P_PROVINCE c on a.ProvinceId = c.ID and a.Type = b.Type left join [P_AddressApproval] d on a.HospitalCode = d.HospitalCode "
                    + " WHERE a.IsDelete not in(1,2,3) and a.GskHospital = @GskHospital "
                    + " ORDER BY a.GskHospital ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@GskHospital", gskHospital)
                    });

                var inProgressList = sqlServerTemplate.Load<P_AddressApproval>(
                    " SELECT * FROM [P_AddressApproval] WHERE GskHospital = @GskHospital ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@GskHospital", gskHospital)
                    });
                foreach (P_HOSPITAL_NEW item in list)
                {
                    item.inProgressList = inProgressList;
                    list_p_hospital_new.Add(item);
                }
                return list_p_hospital_new;
            }
        }

        public int AddNewAddress(P_AddressApproval addressApproval)
        {
            //todo
            //增加审批历史
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                int res = 0;
                res = sqlServerTemplate.ExecuteNonQuery("insert into [P_AddressApproveHistory]([ID],[DA_ID],[UserName],[UserId],[ActionType],[ApproveDate],[Comments],[Type],[IsDelete])" +
                                                        " values (@ID, @DA_ID, @UserName, @UserId, @ActionType, @ApproveDate, @Comments, @Type, @IsDelete) ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", Guid.NewGuid()),
                        SqlParameterFactory.GetSqlParameter("@DA_ID", addressApproval.ID),
                        SqlParameterFactory.GetSqlParameter("@UserName", addressApproval.ApplierName),
                        SqlParameterFactory.GetSqlParameter("@UserId", addressApproval.ApplierMUDID),
                        SqlParameterFactory.GetSqlParameter("@ActionType", 0),
                        SqlParameterFactory.GetSqlParameter("@ApproveDate", DateTime.Now),
                        SqlParameterFactory.GetSqlParameter("@Comments", ""),
                        SqlParameterFactory.GetSqlParameter("@Type", 0),
                        SqlParameterFactory.GetSqlParameter("@IsDelete", 0),
                    });
                if (res > 0)
                {

                    return sqlServerTemplate.ExecuteNonQuery("insert into [P_AddressApproval]([ID],[ApplierName],[ApplierMUDID],[ApplierMobile],[CreateDate],[DACode],[Market],[TA],[Province],[City],[District],[ApprovalStatus],[LineManagerName],[LineManagerMUDID], [GskHospital],[HospitalCode],[HospitalName],[AddAddress],[Distance],[Latitude],[Longitude],AddressName,OtherAddressDistance,MAddress,AddressNameDisplay)" +
                                                            " values (@ID, @ApplierName, @ApplierMUDID, @ApplierMobile, @CreateDate, @DACode, @Market, @TA, @Province, @City, @District, @ApprovalStatus, @LineManagerName, @LineManagerMUDID, @GskHospital, @HospitalCode, @HospitalName, @AddAddress, @Distance, @Latitude, @Longitude,@AddressName,@OtherAddressDistance,@MAddress,@AddressNameDisplay) ",
                        new SqlParameter[]
                        {
                        SqlParameterFactory.GetSqlParameter("@ID", addressApproval.ID),
                        SqlParameterFactory.GetSqlParameter("@ApplierName", addressApproval.ApplierName),
                        SqlParameterFactory.GetSqlParameter("@ApplierMUDID", addressApproval.ApplierMUDID),
                        SqlParameterFactory.GetSqlParameter("@ApplierMobile", addressApproval.ApplierMobile),
                        SqlParameterFactory.GetSqlParameter("@CreateDate", addressApproval.CreateDate),
                        SqlParameterFactory.GetSqlParameter("@DACode", addressApproval.DACode),
                        SqlParameterFactory.GetSqlParameter("@Market", addressApproval.Market),
                        SqlParameterFactory.GetSqlParameter("@TA", addressApproval.TA),
                        SqlParameterFactory.GetSqlParameter("@Province", addressApproval.Province),
                        SqlParameterFactory.GetSqlParameter("@City", addressApproval.City),
                        SqlParameterFactory.GetSqlParameter("@District", addressApproval.District),
                        SqlParameterFactory.GetSqlParameter("@ApprovalStatus", addressApproval.ApprovalStatus),
                        SqlParameterFactory.GetSqlParameter("@LineManagerName", addressApproval.LineManagerName),
                        SqlParameterFactory.GetSqlParameter("@LineManagerMUDID", addressApproval.LineManagerMUDID),
                        //SqlParameterFactory.GetSqlParameter("@LineManagerApproveDate", P_AddressApproval.LineManagerApproveDate),
                        SqlParameterFactory.GetSqlParameter("@GskHospital", addressApproval.GskHospital),
                        SqlParameterFactory.GetSqlParameter("@HospitalCode", addressApproval.HospitalCode),
                        SqlParameterFactory.GetSqlParameter("@HospitalName", addressApproval.HospitalName),
                        SqlParameterFactory.GetSqlParameter("@AddAddress", addressApproval.AddAddress),
                        SqlParameterFactory.GetSqlParameter("@Distance", addressApproval.Distance),
                        SqlParameterFactory.GetSqlParameter("@Latitude", addressApproval.Latitude),
                        SqlParameterFactory.GetSqlParameter("@Longitude", addressApproval.Longitude),
                        SqlParameterFactory.GetSqlParameter("@AddressName", addressApproval.AddressName),
                        SqlParameterFactory.GetSqlParameter("@OtherAddressDistance", addressApproval.OtherAddressDistance),

                        SqlParameterFactory.GetSqlParameter("@MAddress", addressApproval.MAddress),
                        SqlParameterFactory.GetSqlParameter("@AddressNameDisplay", addressApproval.AddressNameDisplay)

                        });
                }
                else
                    return 0;
            }
        }

        public int AddressCancel(P_AddressApproval addressApproval)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                int res = 0;
                res = sqlServerTemplate.ExecuteNonQuery("insert into [P_AddressApproveHistory]([ID],[DA_ID],[UserName],[UserId],[ActionType],[ApproveDate],[Comments],[Type],[IsDelete])" +
                                                        " values (@ID, @DA_ID, @UserName, @UserId, @ActionType, @ApproveDate, @Comments, @Type, @IsDelete) ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", Guid.NewGuid()),
                        SqlParameterFactory.GetSqlParameter("@DA_ID", addressApproval.ID),
                        SqlParameterFactory.GetSqlParameter("@UserName", addressApproval.ApplierName),
                        SqlParameterFactory.GetSqlParameter("@UserId", addressApproval.ApplierMUDID),
                        SqlParameterFactory.GetSqlParameter("@ActionType", 4),
                        SqlParameterFactory.GetSqlParameter("@ApproveDate", DateTime.Now),
                        SqlParameterFactory.GetSqlParameter("@Comments", ""),
                        SqlParameterFactory.GetSqlParameter("@Type", 0),
                        SqlParameterFactory.GetSqlParameter("@IsDelete", 0),
                    });
                if (res > 0)
                {
                    return sqlServerTemplate.ExecuteNonQuery("Update [P_AddressApproval] set [ApprovalStatus] = 4 WHERE ID = @ID ",
                        new SqlParameter[]
                        {
                            SqlParameterFactory.GetSqlParameter("@ID", addressApproval.ID)
                        });
                }
                else
                    return 0;
            }
        }

        public int AddressUpdate(P_AddressApproval addressApproval)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                int res = 0;
                res = sqlServerTemplate.ExecuteNonQuery("insert into [P_AddressApproveHistory]([ID],[DA_ID],[UserName],[UserId],[ActionType],[ApproveDate],[Comments],[Type],[IsDelete])" +
                                                        " values (@ID, @DA_ID, @UserName, @UserId, @ActionType, @ApproveDate, @Comments, @Type, @IsDelete) ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", Guid.NewGuid()),
                        SqlParameterFactory.GetSqlParameter("@DA_ID", addressApproval.ID),
                        SqlParameterFactory.GetSqlParameter("@UserName", addressApproval.ApplierName),
                        SqlParameterFactory.GetSqlParameter("@UserId", addressApproval.ApplierMUDID),
                        SqlParameterFactory.GetSqlParameter("@ActionType", addressApproval.ApprovalStatus),
                        SqlParameterFactory.GetSqlParameter("@ApproveDate", DateTime.Now),
                        SqlParameterFactory.GetSqlParameter("@Comments", ""),
                        SqlParameterFactory.GetSqlParameter("@Type", 0),
                        SqlParameterFactory.GetSqlParameter("@IsDelete", 0),
                    });
                if (res > 0)
                {
                    return sqlServerTemplate.ExecuteNonQuery("Update [P_AddressApproval] set [ApprovalStatus] = @ApprovalStatus, AddAddress =@AddAddress, Distance = @Distance, Latitude =@Latitude, Longitude=@Longitude,OtherAddressDistance=@OtherAddressDistance,ModifyDate = getdate(), AddressName = @AddressName, HospitalCode = @HospitalCode, MAddress = @MAddress, AddressNameDisplay = @AddressNameDisplay WHERE ID = @ID ",
                        new SqlParameter[]
                        {
                            SqlParameterFactory.GetSqlParameter("@ApprovalStatus", addressApproval.ApprovalStatus),
                            SqlParameterFactory.GetSqlParameter("@AddAddress", addressApproval.AddAddress),
                            SqlParameterFactory.GetSqlParameter("@Distance", addressApproval.Distance),
                            SqlParameterFactory.GetSqlParameter("@Latitude", addressApproval.Latitude),
                            SqlParameterFactory.GetSqlParameter("@Longitude", addressApproval.Longitude),

                            SqlParameterFactory.GetSqlParameter("@AddressName", addressApproval.AddressName),
                            SqlParameterFactory.GetSqlParameter("@HospitalCode", addressApproval.HospitalCode),

                            SqlParameterFactory.GetSqlParameter("@OtherAddressDistance", addressApproval.OtherAddressDistance),
                            SqlParameterFactory.GetSqlParameter("@MAddress", addressApproval.MAddress),
                            SqlParameterFactory.GetSqlParameter("@AddressNameDisplay", addressApproval.AddressNameDisplay),
                            SqlParameterFactory.GetSqlParameter("@ID", addressApproval.ID)
                        });
                }
                else
                    return 0;
            }
        }

        public List<P_HOSPITAL_NEW> SearchMainHospitalByGskHospital(string gskHospital)
        {
            List<P_HOSPITAL_NEW> list_p_hospital_new = new List<P_HOSPITAL_NEW>();
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<P_HOSPITAL_NEW>(
                    "SELECT a.*, b.Name AS CityName,c.Name AS ProvinceName,d.ApplierName,d.ApplierMUDID,ISNULL(d.ApprovalStatus,99) AS ApprovalStatus,d.ID AS DA_ID FROM [P_HOSPITAL] a LEFT JOIN [P_CITY] b ON a.CityId=b.ID and a.Type=b.Type "
                    + " left join P_PROVINCE c on a.ProvinceId = c.ID and a.Type = b.Type left join [P_AddressApproval] d on a.HospitalCode = d.HospitalCode "
                    + " WHERE MainAddress = N'主地址' AND a.GskHospital = @GskHospital "
                    + " ORDER BY a.GskHospital ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@GskHospital", gskHospital)
                    });

                var inProgressList = sqlServerTemplate.Load<P_AddressApproval>(
                    " SELECT * FROM [P_AddressApproval] WHERE GskHospital = @GskHospital ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@GskHospital", gskHospital)
                    });
                foreach (P_HOSPITAL_NEW item in list)
                {
                    item.inProgressList = inProgressList;
                    list_p_hospital_new.Add(item);
                }
                return list_p_hospital_new;
            }
        }
        #endregion

    }
}
