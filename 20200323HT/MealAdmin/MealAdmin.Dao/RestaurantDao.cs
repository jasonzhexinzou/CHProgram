using MealAdmin.Entity;
using MealAdmin.Entity.View;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using XFramework.XDataBase;
using XFramework.XDataBase.SqlServer;
using XFramework.XInject.Attributes;
using XFramework.XUtil;

namespace MealAdmin.Dao
{
    public class RestaurantDao : IRestaurantDao
    {
        [Bean("sqlServerTemplFactory")]
        public DbTemplateFactory sqlServerTemplFactory { get; set; }
        [Bean("sqlServerTemplFactoryNonHT")]
        public DbTemplateFactory sqlServerTemplFactoryNonHT { get; set; }

        #region 全量删除餐厅列表
        /// <summary>
        /// 全量删除餐厅列表
        /// </summary>
        /// <returns></returns>
        public int Del()
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("DELETE FROM P_RESTAURANT_LIST");
            }
        }
        #endregion

        #region 全量删除医院可送餐餐厅关系表
        /// <summary>
        /// 全量删除医院可送餐餐厅关系表
        /// </summary>
        /// <returns></returns>
        public int DelRangeRestaurant()
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("DELETE FROM P_HOSPITAL_RANGE_RESTAURANT");
            }
        }
        #endregion

        #region 导入餐厅列表
        /// <summary>
        /// 导入可送餐关系表
        /// </summary>
        /// <returns></returns>
        public int Import(List<P_RESTAURANT_LIST> list)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();

            int insCnt = 0;

            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            //int insCntTemp;
            using (var conn = sqlServerTemplate.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();

                var delCmd = conn.CreateCommand();
                delCmd.CommandText = "DELETE FROM P_RESTAURANT_LIST ";
                delCmd.Transaction = tran;
                delCmd.ExecuteNonQuery();

                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT TOP 0 * FROM P_RESTAURANT_LIST ";
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
                    dr[0] = data.ID;
                    dr[1] = data.DataSources;
                    dr[2] = data.ResId;
                    dr[3] = data.ResName;
                    dr[4] = data.ResType;
                    dr[5] = data.Province;
                    dr[6] = data.City;
                    dr[7] = data.CreateDate;
                    dt.Rows.Add(dr);
                }

                SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, tran);
                bulkCopy.DestinationTableName = "P_RESTAURANT_LIST";
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

        #region 复制前一天关系数据

        /// <summary>
        /// 复制前一天关系数据
        /// </summary>
        /// <returns></returns>
        public int CopyRangeRestaurant()
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();

            int res = 0;

            using (var conn = sqlServerTemplate.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                var res1 = sqlServerTemplate.ExecuteNonQuery("DELETE FROM P_HOSPITAL_RANGE_RESTAURANT_YES");

                if (res1 > -1)
                {
                    res = sqlServerTemplate.ExecuteNonQuery("INSERT INTO P_HOSPITAL_RANGE_RESTAURANT_YES SELECT * FROM P_HOSPITAL_RANGE_RESTAURANT");
                }
                
            }

            return res;
        }
        #endregion

        #region 导入医院可送餐餐厅关系表
        /// <summary>
        /// 导入医院可送餐餐厅关系表
        /// </summary>
        /// <returns></returns>
        public int ImportRangeRestaurant(List<P_HOSPITAL_RANGE_RESTAURANT> list)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();

            int insCnt = 0;

            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            //int insCntTemp = 0;
            using (var conn = sqlServerTemplate.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();

                var delCmd = conn.CreateCommand();
                delCmd.CommandText = "DELETE FROM P_HOSPITAL_RANGE_RESTAURANT ";
                delCmd.Transaction = tran;
                delCmd.ExecuteNonQuery();

                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT TOP 0 * FROM P_HOSPITAL_RANGE_RESTAURANT ";
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
                    dr[0] = data.ID;
                    dr[1] = data.GskHospital;
                    dr[2] = data.DataSources;
                    dr[3] = data.ResId;
                    dr[4] = data.ResName;
                    dr[5] = data.CreateDate;
                    dt.Rows.Add(dr);
                }

                SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, tran);
                bulkCopy.DestinationTableName = "P_HOSPITAL_RANGE_RESTAURANT";
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

        #region 导入医院可送餐餐厅关系表
        /// <summary>
        /// 导入医院可送餐餐厅关系表
        /// </summary>
        /// <returns></returns>
        public int ImportRangeRestaurantCount(List<P_HOSPITAL_RANGE_RESTAURANTCOUNT> list)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();

            int insCnt = 0;

            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            //int insCntTemp = 0;
            using (var conn = sqlServerTemplate.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();

                var delCmd = conn.CreateCommand();
                delCmd.CommandText = "DELETE FROM P_HOSPITAL_RANGE_RESTAURANTCOUNT ";
                delCmd.Transaction = tran;
                delCmd.ExecuteNonQuery();

                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT TOP 0 * FROM P_HOSPITAL_RANGE_RESTAURANTCOUNT ";
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
                    dr[0] = data.ID;
                    dr[1] = data.GskHospital;
                    dr[2] = data.DataSources;
                    dr[3] = data.TotalCount;
                    dr[4] = data.BreakfastCount;
                    dr[5] = data.LunchCount;
                    dr[6] = data.TeaCount;
                    dr[7] = data.CreateDate;
                    dt.Rows.Add(dr);
                }

                SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, tran);
                bulkCopy.DestinationTableName = "P_HOSPITAL_RANGE_RESTAURANTCOUNT";
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

        #region 查询所有可送餐列表
        /// <summary>
        /// 查询所有可送餐列表
        /// </summary>
        /// <returns></returns>
        public void QryAllRangeRestaurant(ref List<P_HOSPITAL_RANGE_RESTAURANT> listHospitalRestaurant, ref List<P_HOSPITAL_INFO_VIEW> listHospital, ref List<P_HOSPITAL_RANGE_RESTAURANTCOUNT> listHospitalRestaurantCount)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();

            using (var conn = sqlServerTemplate.GetSqlConnection() as SqlConnection)
            {
                //WX_Catering_NonHT.DBO.hr_RestaurantInfo
                conn.Open();
                //listHospitalRestaurant = sqlServerTemplate.Load<P_HOSPITAL_RANGE_RESTAURANT>("SELECT A.ID, A.GskHospital, A.DataSources, A.ResId, A.ResName, A.CreateDate  FROM P_HOSPITAL_RANGE_RESTAURANT A where ResId IN (select b.restaurantCode from WX_Catering_NonHT.dbo.hr_RestaurantInfo B where B.status = 1) or A.ResName like N'%堂食%';");
                listHospitalRestaurant = sqlServerTemplate.Load<P_HOSPITAL_RANGE_RESTAURANT>("SELECT A.ID, A.GskHospital, A.DataSources, A.ResId, A.ResName, A.CreateDate  FROM P_HOSPITAL_RANGE_RESTAURANT_COST_202005 A where 1=1;");
                listHospital = sqlServerTemplate.Load<P_HOSPITAL_INFO_VIEW>(
                    $"SELECT A.GskHospital AS GskHospital, B.Name AS Provice, C.Name AS City, A.Name AS Name, A.Address AS Address, A.Type AS Type, A.Remark AS Remark,A.[External] as [External],A.HospitalCode as HospitalCode, A.MainAddress as MainAddress FROM P_HOSPITAL A left join P_PROVINCE B on A.ProvinceId = B.ID left join P_CITY C on A.CityId = C.ID WHERE A.IsDelete = 0");
                listHospitalRestaurantCount = sqlServerTemplate.Load<P_HOSPITAL_RANGE_RESTAURANTCOUNT>("SELECT A.ID, A.GskHospital, A.DataSources, A.TotalCount, A.BreakfastCount, A.LunchCount, A.TeaCount, A.CreateDate  FROM P_HOSPITAL_RANGE_RESTAURANTCOUNT_COST A;");
            }
        }
        #endregion

        #region 根据gsk查询所有医院信息
        /// <summary>
        /// 根据gsk查询所有医院信息
        /// </summary>
        /// <returns></returns>
        public List<P_HOSPITAL_INFO_VIEW> QryAllHospitalInfoByGsk(List<string> gskHospitalList)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();

            var hospitalInfoList = new List<P_HOSPITAL_INFO_VIEW>();

            using (var conn = sqlServerTemplate.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                string[] gskHospitalL = gskHospitalList.ToArray();
                var sql = "SELECT A.GskHospital AS GskHospital, B.Name AS Provice, C.Name AS City, A.Name AS Name, A.Address AS Address, A.Type AS Type, A.Remark AS Remark, A.HospitalCode as HospitalCode, A.MainAddress as MainAddress FROM P_HOSPITAL A left join P_PROVINCE B on A.ProvinceId = B.ID left join P_CITY C on A.CityId = C.ID;";
                var hospitalInfoListTemp = sqlServerTemplate.Load<P_HOSPITAL_INFO_VIEW>(sql).Where(a => gskHospitalL.Contains(a.GskHospital));
                hospitalInfoList.AddRange(hospitalInfoListTemp);
            }
            return hospitalInfoList;
        }
        #endregion

        #region 查询所有餐厅列表
        /// <summary>
        /// 查询所有餐厅列表
        /// </summary>
        /// <returns></returns>
        public List<P_RESTAURANT_LIST> QryAllRestaurant()
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            
            var restaurantList = new List<P_RESTAURANT_LIST>();

            using (var conn = sqlServerTemplate.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                //WX_Catering_NonHT.DBO.hr_RestaurantInfo
                var restaurantRangList = sqlServerTemplate.Load<P_RESTAURANT_LIST>("SELECT A.ID, A.DataSources, A.ResId, A.ResName, A.ResType, A.Province, A.City, A.CreateDate FROM P_RESTAURANT_LIST A where ResId IN (select b.restaurantCode from WX_Catering_NonHT.dbo.hr_RestaurantInfo B where B.status = 1) or ResName like N'%堂食%' ORDER BY A.ResId;");
                //var restaurantRangList = sqlServerTemplate.Load<P_RESTAURANT_LIST>("SELECT A.ID, A.DataSources, A.ResId, A.ResName, A.ResType, A.Province, A.City, A.CreateDate FROM P_RESTAURANT_LIST A where ResId IN (select b.restaurantCode from vxm_dev.dbo.hr_RestaurantInfo B where B.status = 1) or ResName like N'%堂食%' ORDER BY A.ResId;");

                foreach (var restaurantOne in restaurantRangList)
                {
                    var restaurant = new P_RESTAURANT_LIST();
                    restaurant.ID = restaurantOne.ID;
                    restaurant.DataSources = restaurantOne.DataSources;
                    restaurant.ResId = restaurantOne.ResId;
                    restaurant.ResName = restaurantOne.ResName;
                    restaurant.ResType = restaurantOne.ResType;
                    restaurant.Province = restaurantOne.Province;
                    restaurant.City = restaurantOne.City;
                    restaurant.CreateDate = restaurantOne.CreateDate;
                    restaurantList.Add(restaurant);
                }
            }
            return restaurantList;
        }
        #endregion

        public List<P_TERRITORY> QryAllArea()
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();

            var restaurantList = new List<P_TERRITORY>();

            using (var conn = sqlServerTemplate.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                //WX_Catering_NonHT.DBO.hr_RestaurantInfo
                var restaurantRangList = sqlServerTemplate.Load<P_TERRITORY>("SELECT NEWID() AS ID,HOS.HospitalCode,P.Name Province,C.Name City,HOS.Name HospitalName,HOS.Address,HOS.MainAddress,HOS.Type Market,HOS.Latitude,HOS.Longitude,HD.District,HD.DistrictCode,HD.CustomerType,TH.MUD_ID_MR,TH.TERRITORY_MR,TH.MUD_ID_DM,TH.TERRITORY_DM,TH.MUD_ID_RM,TH.TERRITORY_RM,TH.MUD_ID_RD,TH.TERRITORY_RD,TH.MUD_ID_TA,TH.TERRITORY_TA FROM P_HOSPITAL HOS JOIN P_HOSPITAL_DETAIL HD ON HOS.HospitalCode = HD.GskHospital JOIN Territory_Hospital TH ON HOS.HospitalCode = TH.HospitalCode JOIN P_PROVINCE P ON P.ID = HOS.ProvinceId JOIN P_CITY C ON C.ID = HOS.CityId;");

                foreach (var restaurantOne in restaurantRangList)
                {
                    var restaurant = new P_TERRITORY();
                    restaurant.ID = restaurantOne.ID;
                    restaurant.HospitalCode = restaurantOne.HospitalCode;
                    restaurant.HospitalName = restaurantOne.HospitalName;
                    restaurant.Province = restaurantOne.Province;
                    restaurant.City = restaurantOne.City;
                    restaurant.Address = restaurantOne.Address;
                    restaurant.MainAddress = restaurantOne.MainAddress;
                    restaurant.Market = restaurantOne.Market;
                    restaurant.Latitude = restaurantOne.Latitude;
                    restaurant.Longitude = restaurantOne.Longitude;
                    restaurant.District = restaurantOne.District;
                    restaurant.DistrictCode = restaurantOne.DistrictCode;
                    restaurant.CustomerType = restaurantOne.CustomerType;
                    restaurant.MUD_ID_MR = restaurantOne.MUD_ID_MR;
                    restaurant.MUD_ID_DM = restaurantOne.MUD_ID_DM;
                    restaurant.MUD_ID_RM = restaurantOne.MUD_ID_RM;
                    restaurant.MUD_ID_RD = restaurantOne.MUD_ID_RD;
                    restaurant.MUD_ID_TA = restaurantOne.MUD_ID_TA;
                    restaurant.TERRITORY_DM = restaurantOne.TERRITORY_DM;
                    restaurant.TERRITORY_MR = restaurantOne.TERRITORY_MR;
                    restaurant.TERRITORY_RD = restaurantOne.TERRITORY_RD;
                    restaurant.TERRITORY_RM = restaurantOne.TERRITORY_RM;
                    restaurant.TERRITORY_TA = restaurantOne.TERRITORY_TA;
                    restaurantList.Add(restaurant);
                }
            }
            return restaurantList;
        }

        #region 比较插入餐厅变更数据
        /// <summary>
        /// 比较插入餐厅变更数据
        /// </summary>
        /// <returns></returns>
        public int ImportRangeRestaurantChange()
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();

            int res = 0;

            using (var conn = sqlServerTemplate.GetSqlConnection() as SqlConnection)
            {
                conn.Open();

                var datetime = DateTime.Now;
                var res1 = sqlServerTemplate.ExecuteNonQuery("INSERT INTO dbo.P_HospitalCoverChange SELECT NEWID(),HOS.GskHospital,HOS.Name,'','',2,0,'" + datetime + "','" + datetime + "',HOS.MainAddress + ':' + HOS.Address FROM (SELECT DISTINCT GskHospital FROM P_HOSPITAL_RANGE_RESTAURANT WHERE GskHospital NOT LIKE N'%-OH' AND GskHospital NOT IN (SELECT DISTINCT GskHospital FROM P_HOSPITAL_RANGE_RESTAURANT_YES WHERE GskHospital NOT LIKE N'%-OH')) HOSRE JOIN P_HOSPITAL HOS ON HOSRE.GskHospital = HOS.HospitalCode");

                var res2 = sqlServerTemplate.ExecuteNonQuery("INSERT INTO dbo.P_HospitalCoverChange SELECT NEWID(),HOS.GskHospital,HOS.Name,'','',1,0,'" + datetime + "','" + datetime + "',HOS.MainAddress + ':' + HOS.Address FROM (SELECT DISTINCT GskHospital FROM P_HOSPITAL_RANGE_RESTAURANT_YES WHERE GskHospital NOT LIKE N'%-OH' AND  GskHospital NOT IN (SELECT DISTINCT GskHospital FROM P_HOSPITAL_RANGE_RESTAURANT WHERE GskHospital NOT LIKE N'%-OH')) HOSRE JOIN P_HOSPITAL HOS ON HOSRE.GskHospital = HOS.HospitalCode");

                var res3 = sqlServerTemplate.ExecuteNonQuery("INSERT INTO dbo.P_HospitalCoverChange SELECT NEWID(),HOSRE.GskHospital,HOS.Name,HOSRE.ResId,PRL.ResName,3,0,'" + datetime + "','" + datetime + "','' FROM (SELECT A.GskHospital,A.ResId,a.ResName FROM dbo.P_HOSPITAL_RANGE_RESTAURANT A WHERE NOT EXISTS (SELECT  * FROM  dbo.P_HOSPITAL_RANGE_RESTAURANT_YES B WHERE A.GskHospital = B.GskHospital AND A.ResId = B.ResId) AND A.GskHospital LIKE N'%-OH') HOSRE JOIN P_HOSPITAL HOS ON HOSRE.GskHospital = HOS.HospitalCode LEFT JOIN P_RESTAURANT_LIST PRL ON HOSRE.ResId = PRL.ResId");

                var res4 = sqlServerTemplate.ExecuteNonQuery("INSERT INTO dbo.P_HospitalCoverChange SELECT NEWID(),HOSRE.GskHospital,HOS.Name,HOSRE.ResId,PRL.ResName,4,0,'" + datetime + "','" + datetime + "','' FROM (SELECT A.GskHospital,A.ResId,a.ResName FROM dbo.P_HOSPITAL_RANGE_RESTAURANT_YES A WHERE NOT EXISTS (SELECT  * FROM  dbo.P_HOSPITAL_RANGE_RESTAURANT B WHERE A.GskHospital = B.GskHospital AND A.ResId = B.ResId) AND A.GskHospital LIKE N'%-OH' AND A.GskHospital IN (SELECT DISTINCT GskHospital FROM P_HOSPITAL_RANGE_RESTAURANT WHERE GskHospital LIKE N'%-OH') ) HOSRE JOIN P_HOSPITAL HOS ON HOSRE.GskHospital = HOS.HospitalCode LEFT JOIN P_RESTAURANT_LIST PRL ON HOSRE.ResId = PRL.ResId");

                var res5 = sqlServerTemplate.ExecuteNonQuery("INSERT INTO dbo.P_HospitalCoverChange SELECT NEWID(),HOSRE.GskHospital,HOS.Name,HOSRE.ResId,PRL.ResName,5,0,'" + datetime + "','" + datetime + "','' FROM (SELECT A.GskHospital,A.ResId,a.ResName FROM dbo.P_HOSPITAL_RANGE_RESTAURANT_YES A WHERE NOT EXISTS (SELECT  * FROM  dbo.P_HOSPITAL_RANGE_RESTAURANT B WHERE A.GskHospital = B.GskHospital AND A.ResId = B.ResId) AND A.GskHospital LIKE N'%-OH' AND A.GskHospital NOT IN (SELECT DISTINCT GskHospital FROM P_HOSPITAL_RANGE_RESTAURANT WHERE GskHospital LIKE N'%-OH') ) HOSRE JOIN P_HOSPITAL HOS ON HOSRE.GskHospital = HOS.HospitalCode LEFT JOIN P_RESTAURANT_LIST PRL ON HOSRE.ResId = PRL.ResId");

                if(res1 > -1 && res2 > -1 && res3 > -1 && res4 > -1 && res5 > -1)
                {
                    res = 1;
                }

                var res6 = sqlServerTemplate.ExecuteNonQuery("update A SET A.TOTALCOUNT = B.TOTALCOUNT, A.BreakfastCount = B.BreakfastCount, A.LunchCount = B.LunchCount, A.TeaCount = B.TeaCount from P_HOSPITAL_RANGE_RESTAURANTCOUNT A JOIN V_HOSPITAL_RANGE_RESTAURANTCOUNT B ON A.GskHospital = B.GskHospital AND A.DATASOURCES = B.DATASOURCES");

            }

            return res;
        }
        #endregion

    }
}
