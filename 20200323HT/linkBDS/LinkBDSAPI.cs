using LinkBDS;
using MealAdmin.Entity;
using MeetingMealEntity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LinkBDS
{
    public class LinkBDSAPI : LinkBDSBase
    {

        // "https://gsk-api.dev.shangyantong.com/restaurant/";
        readonly static string domain = ConfigurationManager.AppSettings["linkbds_api"];
        // public string secret = "Z3NrdGVzdA==";

        // "https://gsk-api.dev.shangyantong.com/hospital/";
        readonly static string Hdomain = ConfigurationManager.AppSettings["bdsreport_api"];
        // public string secret = "Z3NrdGVzdA==";

        readonly static string sql = ConfigurationManager.AppSettings["sqlconnection"];

        #region 获取token接口
        /// <summary>
        /// 获取token接口
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static GetTokenResponse getToken(string appId, string secret)
        {
            var url = domain + "token/get";

            var param = new Dictionary<string, string>();
            param.Add("appId", appId);

            return Post<GetTokenResponse>(url, param, secret);
        }
        #endregion

        #region 刷新token接口
        /// <summary>
        /// 刷新token接口
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static GetTokenResponse refreshToken(string appId, string secret, string token)
        {
            var url = domain + "refreshToken?token=" + token;

            var param = new Dictionary<string, string>();
            param.Add("appId", appId);

            return Post<GetTokenResponse>(url, param, secret);
        }
        #endregion

        #region 获取可送餐列表接口
        /// <summary>
        /// 获取可送餐列表接口
        /// </summary>
        /// <param name="token"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static GetSyncHospitalResRponse SyncHospitalResBDS(string token, string secret, string province, string city)
        {
            var url = domain + "restaurant/syncHospitalRes?token=" + token;

            var param = new Dictionary<string, string>();
            param.Add("province", province);
            param.Add("city", city);

            return Post<GetSyncHospitalResRponse>(url, param, secret);
        }
        #endregion

        #region 获取餐厅接口
        /// <summary>
        /// 获取餐厅接口
        /// </summary>
        /// <param name="token"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static GetSyncResResponse SyncResBDS(string token, string secret, string province, string city)
        {
            var url = domain + "restaurant/syncRes?token=" + token;

            var param = new Dictionary<string, string>();
            param.Add("province", province);
            param.Add("city", city);

            return Post<GetSyncResResponse>(url, param, secret);
        }
        #endregion

        #region 获取医院数据变量报告接口
        /// <summary>
        /// 获取医院数据变量报告接口
        /// </summary>
        /// <param name="token"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static GetSyncHospitalChangedRponse SyncHospitalChanged(string token, string secret)
        {
            var url = Hdomain + "hospital/changedList?token=" + token;

            var param = new Dictionary<string, string>();
            //string DateNow = DateTime.Now.ToString("yyyy-MM-dd");
            //string DateNow = "2019-05-23";
            DataTable dt = new DataTable();
            dt = LoadChangedData();
            if (dt.Rows.Count > 0)
            {
                string hospitalDifReport = DataTableToJson(dt);
                param.Add("hospitalDifReport", hospitalDifReport);

                return Post<GetSyncHospitalChangedRponse>(url, param, secret);
            }
            else
            {
                return new GetSyncHospitalChangedRponse();
            }
        }
        #endregion

        #region 
        public static DataTable LoadChangedData()
        {
            //數據庫連接字符串
            //string sql = "server =sqldev.cipwbbqagptx.rds.cn-north-1.amazonaws.com.cn;database=vxm_dev; user =ipath_wxm_dev; pwd=imtpath@123;";
            //查詢
            //测试环境语句
            //string commandString = " SELECT [ACTION] AS [action],[Address] AS [address],[City] AS cityName,GskHospital AS gskHospital, "
            //       + " Latitude AS latitude, Longitude AS longitude, HospitalName AS[name], Province AS provinceName, Market AS[type] "
            //        + "FROM [vxm_dev].[db_owner].[Temp_Hospital_Variables] ";
            //+ "FROM[vxm_dev].[dbo].[Hospital_Variables] "
            //+ "where CreateDate> " + "'" + DateNow + "' ";
            //正式环境语句
            string commandString = " SELECT [action],[Address] AS [address],[City] AS cityName,GskHospital AS gskHospital, "
                               + " Latitude AS latitude, Longitude AS longitude, HospitalName AS[name], Province AS provinceName, Market AS[type] "
                                + "FROM [dbo].[Temp_Hospital_Variables] ";
            //創建SqlDataAdapter對象  并執行sql
            SqlDataAdapter dataAdapter = new SqlDataAdapter(commandString, sql);
            //創建數據集dataSet
            DataSet dataSet = new DataSet();
            //將數據添加到數據集中
            dataAdapter.Fill(dataSet);
            //將數據表添加到數據集中
            DataTable dataTable = dataSet.Tables[0];

            return dataTable;
        }
        #endregion

        public static string DataTableToJson(DataTable dt)
        {
            StringBuilder Json = new StringBuilder();
            // Json.Append("{\"" + jsonName + "\":[");
            Json.Append("[");
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Json.Append("{");//(用jquery datatable的时候把这个换成[下边的}换成])
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        Json.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":\"" + dt.Rows[i][j].ToString().Trim().Replace("\r\n", "") + "\"");
                        if (j < dt.Columns.Count - 1)
                        {
                            Json.Append(",");
                        }
                    }
                    Json.Append("}");
                    if (i < dt.Rows.Count - 1)
                    {
                        Json.Append(",");
                    }
                }
            }
            // Json.Append("]}");
            Json.Append("]");
            return Json.ToString();
        }
    }

}
