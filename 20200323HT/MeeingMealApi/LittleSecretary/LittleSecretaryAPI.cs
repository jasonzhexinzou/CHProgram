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

namespace LittleSecretary
{
    public class LittleSecretaryAPI : LittleSecretaryBase
    {
        // "https://safe.fg114.com/cooperate/deliver/v1/";
        readonly static string domain = ConfigurationManager.AppSettings["littlesecretary_api"];
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
            var url = domain + "getToken";

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

        #region 同步城市
        /// <summary>
        /// 同步城市
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static SyncCityResponse syncCity(string token, string secret)
        {
            var url = domain + "syncCity?token=" + token;

            var param = new Dictionary<string, string>();

            return Post<SyncCityResponse>(url, param, secret);
        }
        #endregion

        #region 同步医院
        /// <summary>
        /// 同步医院
        /// </summary>
        /// <param name="token"></param>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public static SyncHospitalResponse syncHospital(string token, string secret, string cityId)
        {
            var url = domain + "syncHospital?token=" + token;

            var param = new Dictionary<string, string>();
            param.Add("cityId", cityId);

            return Post<SyncHospitalResponse>(url, param, secret);
        }
        #endregion

        #region 同步餐厅收费规则表
        /// <summary>
        /// 同步餐厅收费规则表
        /// </summary>
        /// <param name="token"></param>
        /// <param name="extraType"></param>
        /// <returns></returns>
        public static SyncResExtraFeeResponse syncResExtraFee(string token, string secret, string extraType)
        {
            var url = domain + "syncResExtraFee?token=" + token;

            var param = new Dictionary<string, string>();
            param.Add("extraType", extraType);

            return Post<SyncResExtraFeeResponse>(url, param, secret);
        }
        #endregion

        #region 查询餐厅
        /// <summary>
        /// 查询餐厅
        /// </summary>
        /// <param name="token"></param>
        /// <param name="hospitalId"></param>
        /// <param name="address"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public static QueryResResponse queryRes(string token, string secret, string hospitalId, string address, string latitude, string longitude)
        {
            var url = domain + "queryRes?token=" + token;

            var param = new Dictionary<string, string>();
            param.Add("hospitalId", hospitalId);
            param.Add("address", address);
            param.Add("latitude", latitude);
            param.Add("longitude", longitude);

            return Post<QueryResResponse>(url, param, secret);
        }
        #endregion

        #region 查询餐厅菜单
        /// <summary>
        /// 查询餐厅菜单
        /// </summary>
        /// <param name="token"></param>
        /// <param name="resId"></param>
        /// <returns></returns>
        public static QueryResFoodResponse queryResFood(string token, string secret, string resId)
        {
            var url = domain + "queryResFood?token=" + token;

            var param = new Dictionary<string, string>();
            param.Add("resId", resId);

            return Post<QueryResFoodResponse>(url, param, secret);
        }
        #endregion

        #region 计算费用
        /// <summary>
        /// 计算费用
        /// </summary>
        /// <param name="token"></param>
        /// <param name="hospitalId"></param>
        /// <param name="resId"></param>
        /// <param name="foods"></param>
        /// <returns></returns>
        public static CalculateFeeResponse calculateFee(string token, string secret, string hospitalId, string resId, FoodRequest[] foods)
        {
            var url = domain + "calculateFee?token=" + token;

            var request = new CalculateFeeRequest()
            {
                hospitalId = hospitalId,
                resId = resId,
                foods = foods
            };

            return Post<CalculateFeeResponse, CalculateFeeRequest>(url, request, secret);
        }
        #endregion

        #region 创建外卖订单
        /// <summary>
        /// 创建外卖订单
        /// </summary>
        /// <param name="token"></param>
        /// <param name="secret"></param>
        /// <param name="enterpriseOrderId"></param>
        /// <param name="oldXMSOrderId"></param>
        /// <param name="typeId"></param>
        /// <param name="sendTime"></param>
        /// <param name="foodFee"></param>
        /// <param name="packageFee"></param>
        /// <param name="sendFee"></param>
        /// <param name="discountFee"></param>
        /// <param name="totalFee"></param>
        /// <param name="invoiceTitle"></param>
        /// <param name="orderTime"></param>
        /// <param name="remark"></param>
        /// <param name="dinnerName"></param>
        /// <param name="dinnernum"></param>
        /// <param name="cityId"></param>
        /// <param name="sex"></param>
        /// <param name="phone"></param>
        /// <param name="address"></param>
        /// <param name="resId"></param>
        /// <param name="resName"></param>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <param name="hospitalId"></param>
        /// <param name="cn"></param>
        /// <param name="cnAmount"></param>
        /// <param name="department"></param>
        /// <param name="mudId"></param>
        /// <param name="foods"></param>
        /// <returns></returns>
        public static CreateOrderResponse createOrder(string token, string secret, string enterpriseOrderId, string oldXMSOrderId, string typeId, string sendTime, string foodFee, 
            string packageFee, string sendFee, string discountFee, string totalFee, string invoiceTitle, string orderTime, string remark, 
            string dinnerName, string dinnernum, string cityId, string sex, string phone, string address, string resId, string resName,
            string longitude, string latitude, string hospitalId, string cn, string cnAmount, string department, string mudId, FoodRequest[] foods)
        {
            var url = domain + "createOrder?token=" + token;

            var request = new CreateOrderRequest();
            request.enterpriseOrderId = enterpriseOrderId;
            request.oldXMSOrderId = oldXMSOrderId;
            request.typeId = typeId;
            request.sendTime = sendTime;
            request.foodFee = foodFee;
            request.packageFee = packageFee;
            request.sendFee = sendFee;
            request.discountFee = discountFee;
            request.totalFee = totalFee;
            request.invoiceTitle = invoiceTitle;
            request.orderTime = orderTime;
            request.remark = remark;
            request.dinnerName = dinnerName;
            request.dinnerNum = dinnernum;
            request.cityId = cityId;
            request.sex = sex;
            request.phone = phone;
            request.address = address;
            request.resId = resId;
            request.resName = resName;
            request.longitude = longitude;
            request.latitude = latitude;
            request.hospitalId = hospitalId;
            request.cn = cn;
            request.cnAmount = cnAmount;
            request.department = department;
            request.mudId = mudId;
            request.foods = foods;

            return Post<CreateOrderResponse, CreateOrderRequest>(url, request, secret);
        }
        #endregion

        #region 取消订单
        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="token"></param>
        /// <param name="secret"></param>
        /// <param name="xmsOrderId"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public static CancleOrderResponse cancleOrder(string token, string secret, string xmsOrderId, string remark)
        {
            var url = domain + "cancleOrder?token=" + token;

            var request = new CancleOrderRequest();
            request.xmsOrderId= xmsOrderId;
            request.remark= remark;

            return Post<CancleOrderResponse, CancleOrderRequest>(url, request, secret);
        }
        #endregion

        #region 完成订单
        /// <summary>
        /// 完成订单
        /// </summary>
        /// <param name="token"></param>
        /// <param name="xmsOrderId"></param>
        /// <param name="type"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public static ResponseBase finishOrder(string token, string secret, string xmsOrderId, string type, string remark)
        {
            var url = domain + "finishOrder?token=" + token;

            var param = new Dictionary<string, string>();
            param.Add("xmsOrderId", xmsOrderId);
            param.Add("type", type);
            param.Add("remark", remark);

            return Post<ResponseBase>(url, param, secret);
        }
        #endregion

        #region 订单未送达
        /// <summary>
        /// 订单未送达
        /// </summary>
        /// <param name="token"></param>
        /// <param name="secret"></param>
        /// <param name="xmsOrderId"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public static ResponseBase orderDeliveryFailure(string token, string secret, string xmsOrderId, string remark)
        {
            var url = domain + "OrderDeliveryFailure?token=" + token;

            var param = new Dictionary<string, string>();
            param.Add("xmsOrderId", xmsOrderId);
            param.Add("remark", remark);

            return Post<ResponseBase>(url, param, secret);
        }
        #endregion

        #region （代表发起） 取消订单失败反馈
        /// <summary>
        /// （代表发起） 取消订单失败反馈
        /// </summary>
        /// <param name="token"></param>
        /// <param name="secret"></param>
        /// <param name="xmsOrderId"></param>
        /// <param name="type"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public static ResponseBase cancleFailOrderFeedBack(string token, string secret, string xmsOrderId, string type, string remark)
        {
            var url = domain + "cancleFailOrderFeedBack?token=" + token;

            var param = new Dictionary<string, string>();
            param.Add("xmsOrderId", xmsOrderId);
            param.Add("type", type);
            param.Add("remark", remark);

            return Post<ResponseBase>(url, param, secret);
        }
        #endregion

        #region 查询订单（对账）
        /// <summary>
        /// 查询订单（对账）
        /// </summary>
        /// <param name="token"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="timeType"></param>
        /// <returns></returns>
        public static OrderQueryResponse OrderQuery(string token, string secret, string startTime, string endTime, string timeType)
        {
            var url = domain + "OrderQuery?token=" + token;

            var param = new Dictionary<string, string>();
            param.Add("startTime", startTime);
            param.Add("endTime", endTime);
            param.Add("timeType", timeType);

            return Post<OrderQueryResponse>(url, param, secret);
        }
        #endregion

        #region 日报 周报接口
        /// <summary>
        /// 日报 周报接口
        /// </summary>
        /// <param name="token"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="timeType"></param>
        /// <returns></returns>
        public static GetReportResponse GetReport(string token, string secret, string startTime, string endTime, string timeType)
        {
            var url = domain + "GetReport?token=" + token;

            var param = new Dictionary<string, string>();
            param.Add("startTime", startTime);
            param.Add("endTime", endTime);
            param.Add("timeType", timeType);

            return Post<GetReportResponse>(url, param, secret);
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
        public static GetSyncHospitalResRponse SyncHospitalRes(string token, string secret, string province, string city)
        {
            var url = domain + "syncHospitalRes?token=" + token;

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
        public static GetSyncResResponse SyncRes(string token, string secret, string province, string city)
        {
            var url = domain + "syncRes?token=" + token;

            var param = new Dictionary<string, string>();
            param.Add("province", province);
            param.Add("city", city);

            return Post<GetSyncResResponse>(url, param, secret);
        }
        #endregion

        #region XMS获取医院数据变量报告接口
        /// <summary>
        /// 获取医院数据变量报告接口
        /// </summary>
        /// <param name="token"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static GetSyncHospitalChangedRponse SyncHospitalChangedXMS(string token, string secret)
        {
            var url = domain + "syncHospitalChanged?token=" + token;

            var param = new Dictionary<string, string>();
            //string DateNow=DateTime.Now.ToString("yyyy-MM-dd");
            //string DateNow = "2019-05-23";
            DataTable dt = new DataTable();
            dt = LoadChangedData();
            if (dt.Rows.Count>0)
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
            //        + " Latitude AS latitude, Longitude AS longitude, HospitalName AS[name], Province AS provinceName, Market AS[type] "
            //         + "FROM [vxm_dev].[db_owner].[Temp_Hospital_Variables] ";
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
