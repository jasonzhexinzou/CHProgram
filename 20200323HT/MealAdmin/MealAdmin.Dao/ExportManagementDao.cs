using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MealAdmin.Entity;
using XFramework.XInject.Attributes;
using XFramework.XDataBase;
using XFramework.XDataBase.SqlServer;
using System.Data.SqlClient;
using MealAdmin.Entity.Enum;
using MealAdmin.Entity.View;
using XFramework.XUtil;
using System.Configuration;

namespace MealAdmin.Dao
{
    /// <summary>
    /// 订单
    /// </summary>
    public class ExportManagementDao : IExportManagementDao
    {
        [Bean("sqlServerTemplFactory")]
        public DbTemplateFactory sqlServerTemplFactory { get; set; }

        [Bean("sqlServerTemplFactoryNonHT")]
        public DbTemplateFactory sqlServerTemplFactoryNonHT { get; set; }

        private static string _dbName = ConfigurationManager.AppSettings["NonPositionDBName"];

        #region 特殊订单
        public List<P_SPECIAL_ORDER> ExportSpecialOrder(DateTime? _DTBegin, DateTime? _DTEnd, int SpecialOrderType)
        {
            try
            {
                List<P_SPECIAL_ORDER> rtnData;
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                string sqlString = "SELECT TA, COUNT(DISTINCT UserId) AS CountUser, COUNT(id) AS CountOrder, SUM( CASE WHEN P_ORDER_COST.GSKConfirmAmount is not null THEN P_ORDER_COST.GSKConfirmAmount " 
                            + " WHEN P_ORDER_COST.GSKConfirmAmount is null and P_ORDER_COST.XmsTotalPrice < 0 THEN P_ORDER_COST.TotalPrice"
                            + " WHEN P_ORDER_COST.GSKConfirmAmount is null and P_ORDER_COST.XmsTotalPrice >= 0 THEN P_ORDER_COST.XmsTotalPrice END ) AS CountPrice FROM [P_ORDER_COST] P_ORDER_COST LEFT OUTER JOIN P_ORDER_XMS_REPORT ON P_ORDER_COST.XmsOrderId = P_ORDER_XMS_REPORT.XmsOrderId WHERE 1 = 1 AND( P_ORDER_COST.State not in( 5, 11) )";

                var listParams = new List<SqlParameter>();

                if (_DTBegin != null)
                {
                    sqlString += " AND ((@DeliverTimeBegin IS NULL) OR (P_ORDER_COST.DeliverTime >= @DeliverTimeBegin)) ";
                    listParams.Add(new SqlParameter("@DeliverTimeBegin", _DTBegin));
                }
                if (_DTEnd != null)
                {
                    sqlString += " AND ((@DeliverTimeEnd IS NULL) OR (P_ORDER_COST.DeliverTime < @DeliverTimeEnd))";
                    listParams.Add(new SqlParameter("@DeliverTimeEnd", _DTEnd));
                }
                if (SpecialOrderType == 1)
                {
                    sqlString += " AND( P_ORDER_XMS_REPORT.TYDBTYYYTYCTDRDC = N'是' ) ";
                }
                else if (SpecialOrderType == 2)
                {
                    sqlString += " AND( P_ORDER_COST.[State] in ('12') OR P_ORDER_COST.IsRetuen=3 ) ";
                }
                else if (SpecialOrderType == 3)
                {
                    sqlString += " AND( SpecialOrderReason = N'会议支持文件丢失' ) ";
                }

                sqlString += " GROUP BY TA ORDER BY TA";

                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_SPECIAL_ORDER>(sqlString, listParams.ToArray());
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
            }
            return null;
        }

        public List<P_SPECIAL_ORDER_DETAIL> LoadSpecialOrderDetail(DateTime? _DTBegin, DateTime? _DTEnd, int SpecialOrderType)
        {
            try
            {
                List<P_SPECIAL_ORDER_DETAIL> rtnData;
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                string sqlString = @"SELECT P_PreApproval_COST.ApplierName, P_PreApproval_COST.ApplierMUDID, P_PreApproval_COST.HTCode, P_PreApproval_COST.Market,P_PreApproval_COST.VeevaMeetingID, P_PreApproval_COST.TA, P_PreApproval_COST.Province,
                                    P_PreApproval_COST.City, P_PreApproval_COST.HospitalCode, P_PreApproval_COST.HospitalName, P_PreApproval_COST.HospitalAddress, P_PreApproval_COST.CostCenter, 
                                    P_ORDER_COST.Channel, P_ORDER_COST.DeliverTime,
                                    P_ORDER_COST.RestaurantId, P_ORDER_COST.RestaurantName, P_ORDER_COST.ReceiveState,P_ORDER_COST.RealCount,
                                    CASE WHEN P_ORDER_COST.GSKConfirmAmount is not null THEN P_ORDER_COST.GSKConfirmAmount
                                    WHEN P_ORDER_COST.GSKConfirmAmount is null and P_ORDER_COST.XmsTotalPrice < 0 THEN P_ORDER_COST.TotalPrice
                                    WHEN P_ORDER_COST.GSKConfirmAmount is null and P_ORDER_COST.XmsTotalPrice >= 0 THEN P_ORDER_COST.XmsTotalPrice END AS GSKConfirmAmount,
                                    P_ORDER_COST.GSKConAAReason,P_ORDER_COST.MealPaymentAmount,P_ORDER_COST.MealPaymentPO,P_ORDER_COST.AccountingTime
                                    FROM P_ORDER_COST WITH(NOLOCK) LEFT OUTER JOIN P_PreApproval_COST ON P_ORDER_COST.CN = P_PreApproval_COST.HTCode
                                    LEFT OUTER JOIN P_ORDER_XMS_REPORT ON P_ORDER_COST.XmsOrderId = P_ORDER_XMS_REPORT.XmsOrderId
                                    WHERE 1 = 1 AND( P_ORDER_COST.State not in( 5, 11) ) ";
                var listParams = new List<SqlParameter>();

                if (_DTBegin != null)
                {
                    sqlString += " AND ((@DeliverTimeBegin IS NULL) OR (P_ORDER_COST.DeliverTime >= @DeliverTimeBegin)) ";
                    listParams.Add(new SqlParameter("@DeliverTimeBegin", _DTBegin));
                }
                if (_DTEnd != null)
                {
                    sqlString += " AND ((@DeliverTimeEnd IS NULL) OR (P_ORDER_COST.DeliverTime < @DeliverTimeEnd))";
                    listParams.Add(new SqlParameter("@DeliverTimeEnd", _DTEnd));
                }
                if (SpecialOrderType == 1)
                {
                    sqlString += " AND( P_ORDER_XMS_REPORT.TYDBTYYYTYCTDRDC = N'是' ) ";
                }
                else if (SpecialOrderType == 2)
                {
                    sqlString += " AND( P_ORDER_COST.[State] in ('12') OR P_ORDER_COST.IsRetuen=3 ) ";
                }
                else if (SpecialOrderType == 3)
                {
                    sqlString += " AND( SpecialOrderReason = N'会议支持文件丢失' ) ";
                }

                sqlString += " Order by P_ORDER_COST.DeliverTime DESC";

                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_SPECIAL_ORDER_DETAIL>(sqlString, listParams.ToArray());
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
            }
            return null;
        }
        #endregion
        public List<P_EVALUATE> ExportOrderEvaluate(DateTime? _DTBegin, DateTime? _DTEnd, string sltTA, string supplier)
        {
            try
            {
                List<P_EVALUATE> rtnData;
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                string sqlString = @"SELECT * FROM [P_EVALUATE]  where OnTime is not null and Star>0 and OrderID in (SELECT distinct b.ID FROM [P_ORDER_COST] b where 1=1 ";
                var listParams = new List<SqlParameter>();

                if (_DTBegin != null)
                {
                    sqlString += " AND (b.DeliverTime >= @DeliverTimeBegin) ";
                    listParams.Add(new SqlParameter("@DeliverTimeBegin", _DTBegin));
                }
                if (_DTEnd != null)
                {
                    sqlString += " AND (b.DeliverTime < @DeliverTimeEnd)";
                    listParams.Add(new SqlParameter("@DeliverTimeEnd", _DTEnd));
                }
                if (supplier != "all")
                {
                    sqlString += " AND (b.Channel = @Channel) ";
                    listParams.Add(new SqlParameter("@Channel", supplier));
                }
                if (sltTA != null)
                {
                    sqlString += " AND (b.TA in (" + sltTA + ")) ";
                }
                sqlString += " AND (b.[State] not in (5,11)) )";

                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_EVALUATE>(sqlString, listParams.ToArray());
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
            }
            return null;
        }
        public List<P_ORDER> ExportOrderCount(DateTime? _DTBegin, DateTime? _DTEnd, string sltTA, string supplier)
        {
            try
            {
                List<P_ORDER> rtnData;
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                string sqlString = @"SELECT distinct b.[EnterpriseOrderId] FROM [P_ORDER_COST] b where 1=1 ";
                var listParams = new List<SqlParameter>();

                if (_DTBegin != null)
                {
                    sqlString += " AND ((@DeliverTimeBegin IS NULL) OR (b.DeliverTime >= @DeliverTimeBegin)) ";
                    listParams.Add(new SqlParameter("@DeliverTimeBegin", _DTBegin));
                }
                if (_DTEnd != null)
                {
                    sqlString += " AND ((@DeliverTimeEnd IS NULL) OR (b.DeliverTime < @DeliverTimeEnd))";
                    listParams.Add(new SqlParameter("@DeliverTimeEnd", _DTEnd));
                }
                if (supplier != "all")
                {
                    sqlString += " AND (b.Channel = @Channel) ";
                    listParams.Add(new SqlParameter("@Channel", supplier));
                }
                if (sltTA != null)
                {
                    sqlString += " AND (b.TA in (" + sltTA + ")) ";
                }
                sqlString += " AND (b.[State] not in (5,11)) ";

                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_ORDER>(sqlString, listParams.ToArray());
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
            }
            return null;
        }

        public List<P_EVALUATE> ExportNonHTOrderEvaluate(DateTime? _DTBegin, DateTime? _DTEnd, string sltTA, string supplier)
        {
            try
            {
                List<P_EVALUATE> rtnData;
                var sqlServerTemplNonHT = sqlServerTemplFactoryNonHT.CreateDbTemplate();
                string sqlString = @"SELECT * FROM [P_EVALUATE]  where OnTime is not null and Star>0 and OrderID in (SELECT distinct b.ID FROM [P_ORDER] b where 1=1 ";
                var listParams = new List<SqlParameter>();

                if (_DTBegin != null)
                {
                    sqlString += " AND (b.DeliverTime >= @DeliverTimeBegin) ";
                    listParams.Add(new SqlParameter("@DeliverTimeBegin", _DTBegin));
                }
                if (_DTEnd != null)
                {
                    sqlString += " AND (b.DeliverTime < @DeliverTimeEnd)";
                    listParams.Add(new SqlParameter("@DeliverTimeEnd", _DTEnd));
                }
                if (supplier != "all")
                {
                    sqlString += " AND (b.Channel = @Channel) ";
                    listParams.Add(new SqlParameter("@Channel", supplier));
                }
                if (sltTA != null)
                {
                    sqlString += " AND (b.TA in (" + sltTA + ")) ";
                }
                sqlString += " AND (b.[State] not in (5,11)) )";

                using (var conn = sqlServerTemplNonHT.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplNonHT.Load<P_EVALUATE>(sqlString, listParams.ToArray());
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
            }
            return null;
        }
        public List<P_ORDER> ExportNonHTOrderCount(DateTime? _DTBegin, DateTime? _DTEnd, string sltTA, string supplier)
        {
            try
            {
                List<P_ORDER> rtnData;
                var sqlServerTemplNonHT = sqlServerTemplFactoryNonHT.CreateDbTemplate();
                string sqlString = @"SELECT distinct b.[EnterpriseOrderId] FROM [P_ORDER] b where 1=1 ";
                var listParams = new List<SqlParameter>();

                if (_DTBegin != null)
                {
                    sqlString += " AND ((@DeliverTimeBegin IS NULL) OR (b.DeliverTime >= @DeliverTimeBegin)) ";
                    listParams.Add(new SqlParameter("@DeliverTimeBegin", _DTBegin));
                }
                if (_DTEnd != null)
                {
                    sqlString += " AND ((@DeliverTimeEnd IS NULL) OR (b.DeliverTime < @DeliverTimeEnd))";
                    listParams.Add(new SqlParameter("@DeliverTimeEnd", _DTEnd));
                }
                if (supplier != "all")
                {
                    sqlString += " AND (b.Channel = @Channel) ";
                    listParams.Add(new SqlParameter("@Channel", supplier));
                }
                if (sltTA != null)
                {
                    sqlString += " AND (b.TA in (" + sltTA + ")) ";
                }
                sqlString += " AND (b.[State] not in (5,11)) ";

                using (var conn = sqlServerTemplNonHT.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplNonHT.Load<P_ORDER>(sqlString, listParams.ToArray());
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
            }
            return null;
        }

        #region Special Order
        public List<P_TA> LoadTA()
        {
            try
            {
                List<P_TA> rtnData;
                string sqlString = "SELECT DISTINCT TA as Name from dbo.P_PreApproval_COST WHERE MRTerritoryCode IS NOT NULL AND MRTerritoryCode <>'' UNION SELECT DISTINCT TERRITORY_TA AS TA FROM " + _dbName + ".dbo.Territory_Hospital_COST";
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_TA>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
                return null;
            }
        }

        public List<P_SPECIAL_ORDER_PROPORTION> LoadSpecialOrderProportionSummary(string date, string ta, string htType, int specialOrderCnt, string resCnt, int proportion)
        {
            try
            {
                List<P_SPECIAL_ORDER_PROPORTION> rtnData;
                string sqlString = string.Empty;
                string conditionDate = string.Empty;
                string conditionTA = string.Empty;
                string conditionOH = string.Empty;
                string conditionResCnt = string.Empty;
                string cDate = date + "-01";
                string rangTable = _dbName + ".dbo.P_HOSPITAL_RANGE_RESTAURANT_COST_" + date.Replace("-", "");
                string rangTableNull = _dbName + ".dbo.P_HOSPITAL_RANGE_RESTAURANT_COST_EMPTY";
                conditionDate = " DeliverTime >= DATEADD(month, datediff(month, 0, '" + cDate + "'), 0) AND DeliverTime <= dateadd(second, -1, DATEADD(month, datediff(month, -1, '" + cDate + "'), 0))";
                //if (ta != "全部")
                //{
                //    conditionTA = " AND P_ORDER_COST.TA = '" + ta + "'";
                //}
                if (ta != null)
                {
                    conditionTA = " AND (P_ORDER_COST.TA in (" + ta + ")) ";
                }
                if (htType == "院内")
                {
                    conditionOH = " AND Address <> N'院外' ";
                }
                else if (htType == "院外")
                {
                    conditionOH = " AND Address = N'院外' ";
                }
                if (!string.IsNullOrEmpty(resCnt))
                {
                    conditionResCnt = " AND R.ResIdCnt >=" + resCnt;
                }
                sqlString = " IF OBJECT_ID('" + rangTable + "','u') IS NOT NULL"
                            + " SELECT * FROM (SELECT A.*,B.OrdCnt,ISNULL(C.ResIdCnt,-1) AS ResIdCnt, CAST((CAST(A.SpecialCnt as decimal(10,4))/CAST(B.OrdCnt as decimal(10,4))) AS DECIMAL(10,2))*100 AS Proportion FROM "
                            + " ( SELECT P_PreApproval_COST.ApplierName,P_ORDER_COST.UserId,P_ORDER_COST.TA ,HospitalId,RestaurantId,P_ORDER_COST.RestaurantName, "
                            + " COUNT(P_ORDER_COST.ID) AS SpecialCnt,SUM( CASE WHEN P_ORDER_COST.GSKConfirmAmount is not null THEN P_ORDER_COST.GSKConfirmAmount "
                            + " WHEN P_ORDER_COST.GSKConfirmAmount is null and P_ORDER_COST.XmsTotalPrice < 0 THEN P_ORDER_COST.TotalPrice"
                            + " WHEN P_ORDER_COST.GSKConfirmAmount is null and P_ORDER_COST.XmsTotalPrice >= 0 THEN P_ORDER_COST.XmsTotalPrice END ) AS PriceCnt "
                            + " FROM dbo.P_ORDER_COST P_ORDER_COST LEFT JOIN dbo.P_PreApproval_COST P_PreApproval_COST ON P_ORDER_COST.CN = P_PreApproval_COST.HTCode "
                            + " WHERE ( P_ORDER_COST.State not in( 5, 11) ) AND "
                            + conditionDate
                            + conditionTA
                            + conditionOH
                            + " GROUP BY P_ORDER_COST.UserId,P_ORDER_COST.HospitalId,P_ORDER_COST.RestaurantId,P_PreApproval_COST.ApplierName,P_ORDER_COST.TA,P_ORDER_COST.RestaurantName "
                            + " ) A "
                            + " LEFT JOIN "
                            + " ( SELECT P_ORDER_COST.UserId, COUNT(*) AS OrdCnt FROM dbo.P_ORDER_COST P_ORDER_COST WHERE ( P_ORDER_COST.State not in( 5, 11) ) AND "
                            + conditionDate
                            + conditionTA
                            + conditionOH
                            + " GROUP BY P_ORDER_COST.UserId "
                            + " ) B ON A.UserId = B.UserId "
                            + " LEFT JOIN "
                            + " ( SELECT GskHospital, COUNT(DISTINCT ResId) AS ResIdCnt FROM " + rangTable
                            + " GROUP BY GskHospital ) C ON A.HospitalId = C.GskHospital COLLATE Chinese_PRC_CI_AS "
                            + " ) R WHERE R.SpecialCnt >="
                            + specialOrderCnt
                            + conditionResCnt
                            + " AND R.Proportion >="
                            + proportion
                            + " ORDER BY R.ApplierName"

                            + " ELSE "

                            + " SELECT * FROM (SELECT A.*,B.OrdCnt,ISNULL(C.ResIdCnt,-1) AS ResIdCnt, CAST((CAST(A.SpecialCnt as decimal(10,4))/CAST(B.OrdCnt as decimal(10,4))) AS DECIMAL(10,2))*100 AS Proportion FROM "
                            + " ( SELECT P_PreApproval_COST.ApplierName,P_ORDER_COST.UserId,P_ORDER_COST.TA ,HospitalId,RestaurantId,P_ORDER_COST.RestaurantName, "
                            + " COUNT(P_ORDER_COST.ID) AS SpecialCnt,SUM( CASE WHEN P_ORDER_COST.GSKConfirmAmount is not null THEN P_ORDER_COST.GSKConfirmAmount "
                            + " WHEN P_ORDER_COST.GSKConfirmAmount is null and P_ORDER_COST.XmsTotalPrice < 0 THEN P_ORDER_COST.TotalPrice"
                            + " WHEN P_ORDER_COST.GSKConfirmAmount is null and P_ORDER_COST.XmsTotalPrice >= 0 THEN P_ORDER_COST.XmsTotalPrice END ) AS PriceCnt "
                            + " FROM dbo.P_ORDER_COST P_ORDER_COST LEFT JOIN dbo.P_PreApproval_COST P_PreApproval_COST ON P_ORDER_COST.CN = P_PreApproval_COST.HTCode "
                            + " WHERE ( P_ORDER_COST.State not in( 5, 11) ) AND "
                            + conditionDate
                            + conditionTA
                            + conditionOH
                            + " GROUP BY P_ORDER_COST.UserId,P_ORDER_COST.HospitalId,P_ORDER_COST.RestaurantId,P_PreApproval_COST.ApplierName,P_ORDER_COST.TA,P_ORDER_COST.RestaurantName "
                            + " ) A "
                            + " LEFT JOIN "
                            + " ( SELECT P_ORDER_COST.UserId, COUNT(*) AS OrdCnt FROM dbo.P_ORDER_COST P_ORDER_COST WHERE ( P_ORDER_COST.State not in( 5, 11) ) AND "
                            + conditionDate
                            + conditionTA
                            + conditionOH
                            + " GROUP BY P_ORDER_COST.UserId "
                            + " ) B ON A.UserId = B.UserId "
                            + " LEFT JOIN "
                            + " ( SELECT GskHospital, COUNT(DISTINCT ResId) AS ResIdCnt FROM " + rangTableNull
                            + " GROUP BY GskHospital ) C ON A.HospitalId = C.GskHospital COLLATE Chinese_PRC_CI_AS "
                            + " ) R WHERE R.SpecialCnt >="
                            + specialOrderCnt
                            + conditionResCnt
                            + " AND R.Proportion >="
                            + proportion
                            + " ORDER BY R.ApplierName";

                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_SPECIAL_ORDER_PROPORTION>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
            }
            return null;
        }

        public List<P_SPECIAL_ORDER_DETAIL> LoadSpecialOrderProportionDetail(string date, string ta, string htType, int specialOrderCnt, string resCnt, int proportion)
        {
            try
            {
                List<P_SPECIAL_ORDER_DETAIL> rtnData;
                string sqlString = string.Empty;
                string conditionDate = string.Empty;
                string conditionTA = string.Empty;
                string conditionOH = string.Empty;
                string conditionResCnt = string.Empty;
                string cDate = date + "-01";
                string rangTable = _dbName + ".dbo.P_HOSPITAL_RANGE_RESTAURANT_COST_" + date.Replace("-", "");
                string rangTableNull = _dbName + ".dbo.P_HOSPITAL_RANGE_RESTAURANT_COST_EMPTY";
                conditionDate = " DeliverTime >= DATEADD(month, datediff(month, 0, '" + cDate + "'), 0) AND DeliverTime <= dateadd(second, -1, DATEADD(month, datediff(month, -1, '" + cDate + "'), 0))";
                if (ta != null)
                {
                    conditionTA = " AND (P_ORDER_COST.TA in (" + ta + ")) ";
                }
                if (htType == "院内")
                {
                    conditionOH = " AND Address <> N'院外' ";
                }
                else if (htType == "院外")
                {
                    conditionOH = " AND Address = N'院外' ";
                }
                if (!string.IsNullOrEmpty(resCnt))
                {
                    conditionResCnt = " AND M.ResIdCnt >=" + resCnt;
                }
                sqlString = " IF OBJECT_ID('" + rangTable + "','u') IS NOT NULL"
                            + " SELECT * FROM (SELECT A.*,B.OrdCnt,C.ResIdCnt, CAST((CAST(A.SpecialCnt as decimal(10,4))/CAST(B.OrdCnt as decimal(10,4))) AS DECIMAL(10,2))*100 AS Proportion FROM "
                            + " ( SELECT P_ORDER_COST.UserId,P_ORDER_COST.TA ,HospitalId,RestaurantId, "
                            + " COUNT(P_ORDER_COST.ID) AS SpecialCnt,SUM( CASE WHEN P_ORDER_COST.GSKConfirmAmount is not null THEN P_ORDER_COST.GSKConfirmAmount "
                            + " WHEN P_ORDER_COST.GSKConfirmAmount is null and P_ORDER_COST.XmsTotalPrice < 0 THEN P_ORDER_COST.TotalPrice "
                            + " WHEN P_ORDER_COST.GSKConfirmAmount is null and P_ORDER_COST.XmsTotalPrice >= 0 THEN P_ORDER_COST.XmsTotalPrice END ) AS PriceCnt "
                            + " FROM dbo.P_ORDER_COST P_ORDER_COST LEFT JOIN dbo.P_PreApproval_COST P_PreApproval_COST ON P_ORDER_COST.CN = P_PreApproval_COST.HTCode "
                            + " WHERE ( P_ORDER_COST.State not in( 5, 11) ) AND "
                            + conditionDate
                            + conditionTA
                            + conditionOH
                            + " GROUP BY P_ORDER_COST.UserId,P_ORDER_COST.HospitalId,P_ORDER_COST.RestaurantId,P_ORDER_COST.TA "
                            + " ) A "
                            + " LEFT JOIN "
                            + " ( SELECT P_ORDER_COST.UserId, COUNT(*) AS OrdCnt FROM dbo.P_ORDER_COST P_ORDER_COST WHERE ( P_ORDER_COST.State not in( 5, 11) ) AND "
                            + conditionDate
                            + conditionTA
                            + conditionOH
                            + " GROUP BY P_ORDER_COST.UserId "
                            + " ) B ON A.UserId = B.UserId "
                            + " LEFT JOIN "
                            + " ( SELECT GskHospital, COUNT(DISTINCT ResId) AS ResIdCnt FROM " + rangTable
                            + " GROUP BY GskHospital ) C ON A.HospitalId = C.GskHospital COLLATE Chinese_PRC_CI_AS "
                            + " ) M "
                            + " RIGHT JOIN ( "
                            + " SELECT P_PreApproval_COST.ApplierName, P_PreApproval_COST.ApplierMUDID, P_PreApproval_COST.HTCode, P_PreApproval_COST.Market,P_PreApproval_COST.VeevaMeetingID, P_PreApproval_COST.TA, P_PreApproval_COST.Province, "
                            + " P_PreApproval_COST.City, P_PreApproval_COST.HospitalCode, P_PreApproval_COST.HospitalName, P_PreApproval_COST.HospitalAddress, P_PreApproval_COST.CostCenter,  "
                            + " P_ORDER_COST.Channel, P_ORDER_COST.DeliverTime, "
                            + " P_ORDER_COST.RestaurantId, P_ORDER_COST.RestaurantName, P_ORDER_COST.ReceiveState,P_ORDER_COST.RealCount, "
                            + " CASE WHEN P_ORDER_COST.GSKConfirmAmount is not null THEN P_ORDER_COST.GSKConfirmAmount "
                            + " WHEN P_ORDER_COST.GSKConfirmAmount is null and P_ORDER_COST.XmsTotalPrice < 0 THEN P_ORDER_COST.TotalPrice "
                            + " WHEN P_ORDER_COST.GSKConfirmAmount is null and P_ORDER_COST.XmsTotalPrice >= 0 THEN P_ORDER_COST.XmsTotalPrice END AS GSKConfirmAmount, "
                            + " P_ORDER_COST.GSKConAAReason,P_ORDER_COST.MealPaymentAmount,P_ORDER_COST.MealPaymentPO,P_ORDER_COST.AccountingTime "
                            + " FROM P_ORDER_COST WITH(NOLOCK) LEFT OUTER JOIN P_PreApproval_COST ON P_ORDER_COST.CN = P_PreApproval_COST.HTCode "
                            + " WHERE(P_ORDER_COST.State not in (5, 11)) AND "
                            + conditionDate
                            + conditionTA
                            + conditionOH
                            + " ) N "
                            + " ON M.UserId = N.ApplierMUDID AND M.TA = N.TA AND M.HospitalId = N.HospitalCode COLLATE Chinese_PRC_CI_AS  AND M.RestaurantId = N.RestaurantId "

                            + " WHERE M.SpecialCnt >="
                            + specialOrderCnt
                            + conditionResCnt
                            + " AND M.Proportion >="
                            + proportion
                            + " ORDER BY N.ApplierName"

                            + " ELSE "

                            + " SELECT * FROM (SELECT A.*,B.OrdCnt,C.ResIdCnt, CAST((CAST(A.SpecialCnt as decimal(10,4))/CAST(B.OrdCnt as decimal(10,4))) AS DECIMAL(10,2))*100 AS Proportion FROM "
                            + " ( SELECT P_ORDER_COST.UserId,P_ORDER_COST.TA ,HospitalId,RestaurantId, "
                            + " COUNT(P_ORDER_COST.ID) AS SpecialCnt,SUM( CASE WHEN P_ORDER_COST.GSKConfirmAmount is not null THEN P_ORDER_COST.GSKConfirmAmount "
                            + " WHEN P_ORDER_COST.GSKConfirmAmount is null and P_ORDER_COST.XmsTotalPrice < 0 THEN P_ORDER_COST.TotalPrice "
                            + " WHEN P_ORDER_COST.GSKConfirmAmount is null and P_ORDER_COST.XmsTotalPrice >= 0 THEN P_ORDER_COST.XmsTotalPrice END ) AS PriceCnt "
                            + " FROM dbo.P_ORDER_COST P_ORDER_COST LEFT JOIN dbo.P_PreApproval_COST P_PreApproval_COST ON P_ORDER_COST.CN = P_PreApproval_COST.HTCode "
                            + " WHERE ( P_ORDER_COST.State not in( 5, 11) ) AND "
                            + conditionDate
                            + conditionTA
                            + conditionOH
                            + " GROUP BY P_ORDER_COST.UserId,P_ORDER_COST.HospitalId,P_ORDER_COST.RestaurantId,P_ORDER_COST.TA "
                            + " ) A "
                            + " LEFT JOIN "
                            + " ( SELECT P_ORDER_COST.UserId, COUNT(*) AS OrdCnt FROM dbo.P_ORDER_COST P_ORDER_COST WHERE ( P_ORDER_COST.State not in( 5, 11) ) AND "
                            + conditionDate
                            + conditionTA
                            + conditionOH
                            + " GROUP BY P_ORDER_COST.UserId "
                            + " ) B ON A.UserId = B.UserId "
                            + " LEFT JOIN "
                            + " ( SELECT GskHospital, COUNT(DISTINCT ResId) AS ResIdCnt FROM " + rangTableNull
                            + " GROUP BY GskHospital ) C ON A.HospitalId = C.GskHospital COLLATE Chinese_PRC_CI_AS "
                            + " ) M "
                            + " RIGHT JOIN ( "
                            + " SELECT P_PreApproval_COST.ApplierName, P_PreApproval_COST.ApplierMUDID, P_PreApproval_COST.HTCode, P_PreApproval_COST.Market,P_PreApproval_COST.VeevaMeetingID, P_PreApproval_COST.TA, P_PreApproval_COST.Province, "
                            + " P_PreApproval_COST.City, P_PreApproval_COST.HospitalCode, P_PreApproval_COST.HospitalName, P_PreApproval_COST.HospitalAddress, P_PreApproval_COST.CostCenter,  "
                            + " P_ORDER_COST.Channel, P_ORDER_COST.DeliverTime, "
                            + " P_ORDER_COST.RestaurantId, P_ORDER_COST.RestaurantName, P_ORDER_COST.ReceiveState,P_ORDER_COST.RealCount, "
                            + " CASE WHEN P_ORDER_COST.GSKConfirmAmount is not null THEN P_ORDER_COST.GSKConfirmAmount "
                            + " WHEN P_ORDER_COST.GSKConfirmAmount is null and P_ORDER_COST.XmsTotalPrice < 0 THEN P_ORDER_COST.TotalPrice "
                            + " WHEN P_ORDER_COST.GSKConfirmAmount is null and P_ORDER_COST.XmsTotalPrice >= 0 THEN P_ORDER_COST.XmsTotalPrice END AS GSKConfirmAmount, "
                            + " P_ORDER_COST.GSKConAAReason,P_ORDER_COST.MealPaymentAmount,P_ORDER_COST.MealPaymentPO,P_ORDER_COST.AccountingTime "
                            + " FROM P_ORDER_COST WITH(NOLOCK) LEFT OUTER JOIN P_PreApproval_COST ON P_ORDER_COST.CN = P_PreApproval_COST.HTCode "
                            + " WHERE(P_ORDER_COST.State not in (5, 11)) AND "
                            + conditionDate
                            + conditionTA
                            + conditionOH
                            + " ) N "
                            + " ON M.UserId = N.ApplierMUDID AND M.TA = N.TA AND M.HospitalId = N.HospitalCode COLLATE Chinese_PRC_CI_AS  AND M.RestaurantId = N.RestaurantId "

                            + " WHERE M.SpecialCnt >="
                            + specialOrderCnt
                            + conditionResCnt
                            + " AND M.Proportion >="
                            + proportion
                            + " ORDER BY N.ApplierName";

                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_SPECIAL_ORDER_DETAIL>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
            }
            return null;
        }
        #endregion

        #region 特殊发票订单
        public List<P_SPECIAL_INVOICE_ORDER> LoadSpecialInvoiceList(string date, string channel)
        {
            try
            {
                List<P_SPECIAL_INVOICE_ORDER> rtnData;
                string sqlString = string.Empty;
                string conditionDate = string.Empty;
                conditionDate = " AND DeliverTime >= DATEADD(month, datediff(month, 0, '" + date + "'), 0) AND DeliverTime <= dateadd(second, -1, DATEADD(month, datediff(month, -1, '" + date + "'), 0))";

                sqlString = " SELECT Channel, "
                            + " COUNT ( CASE WHEN M.DeliverTime >= M.specialInvoiceBeginDate AND  M.DeliverTime<= M.specialInvoiceEndDate THEN 'A' end ) AS SpecialCnt, "
                            + " COUNT ( CASE WHEN M.DeliverTime < M.specialInvoiceBeginDate OR  M.DeliverTime > M.specialInvoiceEndDate "
                            + " OR M.specialInvoiceBeginDate IS NULL OR M.specialInvoiceEndDate IS NULL THEN 'B' end  ) AS NonSpecialCnt"
                            + " FROM ( SELECT A.Channel,A.DeliverTime,B.specialInvoiceBeginDate,B.specialInvoiceEndDate FROM dbo.P_ORDER_COST A "
                            + " LEFT JOIN ( SELECT y.* FROM " + _dbName + ".dbo.hr_RestaurantBasicInfo x INNER JOIN " + _dbName + ".dbo.hr_SpecialInvoiceInfo y ON x.restaurantCode= y.restaurantCode ) B ON A.RestaurantId =B.restaurantCode "
                            + " WHERE A.State NOT IN (5,11) AND SUBSTRING(A.RestaurantId,1,4)<>'R145' "
                            + conditionDate
                            + " ) M WHERE Channel = '" + channel + "' GROUP BY M.Channel ";

                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_SPECIAL_INVOICE_ORDER>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
            }
            return null;
        }

        public List<P_SPECIAL_INVOICE_ORDER_NEW> LoadStarbucksList(string date, string channel)
        {
            try
            {
                List<P_SPECIAL_INVOICE_ORDER_NEW> rtnData;
                string sqlString = string.Empty;
                string conditionDate = string.Empty;
                conditionDate = " AND DeliverTime >= DATEADD(month, datediff(month, 0, '" + date + "'), 0) AND DeliverTime <= dateadd(second, -1, DATEADD(month, datediff(month, -1, '" + date + "'), 0))";

                sqlString = " SELECT P_ORDER_COST.Channel, COUNT(P_ORDER_COST.ID) AS OrderCnt, "
                            + " SUM( CASE WHEN P_ORDER_COST.GSKConfirmAmount is not null THEN P_ORDER_COST.GSKConfirmAmount "
                            + " WHEN P_ORDER_COST.GSKConfirmAmount is null and P_ORDER_COST.XmsTotalPrice < 0 THEN P_ORDER_COST.TotalPrice "
                            + " WHEN P_ORDER_COST.GSKConfirmAmount is null and P_ORDER_COST.XmsTotalPrice >= 0 THEN P_ORDER_COST.XmsTotalPrice END ) AS OrderPrice"
                            + " FROM dbo.P_ORDER_COST        "
                            + " WHERE P_ORDER_COST.State NOT IN (5,11) AND SUBSTRING(P_ORDER_COST.RestaurantId,1,4) = 'R145' "
                            + conditionDate
                            + " AND P_ORDER_COST.Channel ='" + channel + "' GROUP BY P_ORDER_COST.Channel";

                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_SPECIAL_INVOICE_ORDER_NEW>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
            }
            return null;
        }

        public List<P_SPECIAL_INVOICE_ORDER_NEW> LoadNonHTList(string date, string channel)
        {
            try
            {
                List<P_SPECIAL_INVOICE_ORDER_NEW> rtnData;
                string sqlString = string.Empty;
                string conditionDate = string.Empty;
                conditionDate = " AND DeliverTime >= DATEADD(month, datediff(month, 0, '" + date + "'), 0) AND DeliverTime <= dateadd(second, -1, DATEADD(month, datediff(month, -1, '" + date + "'), 0))";

                sqlString = " SELECT P_ORDER.Channel, COUNT(P_ORDER.ID) AS OrderCnt, "
                            + " SUM( CASE WHEN P_ORDER.XmsTotalPrice >=0 THEN P_ORDER.XmsTotalPrice else P_ORDER.TotalPrice END ) AS OrderPrice"
                            + " FROM dbo.P_ORDER        "
                            + " WHERE P_ORDER.State NOT IN (5,11) AND P_ORDER.RestaurantId IN (SELECT restaurantCode FROM dbo.hr_SpecialInvoiceInfo WHERE brandCode = 'R145') "
                            + conditionDate
                            + " AND P_ORDER.Channel ='" + channel + "' GROUP BY P_ORDER.Channel";

                var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_SPECIAL_INVOICE_ORDER_NEW>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
            }
            return null;
        }
        #endregion

        public List<P_ORDER_UnfinishedOrder_View> ExportUnfinishedOrder(string startdate, string enddate, string sltTA, string HTType)
        {
            try
            {
                List<P_ORDER_UnfinishedOrder_View> rtnData;
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                string sqlString = @"SELECT a.TA,b.ApplierName, b.ApplierMUDID,CASE WHEN c.[State] = 1 THEN N'否' ELSE N'是' END AS IsWorkdayQuit,
a.TransferUserMUDID,d.ReAssignBUHeadMUDID,COUNT(a.EnterpriseOrderId) as UnfinishedCount from [dbo].[P_ORDER_COST] a WITH(NOLOCK) 
LEFT OUTER JOIN [dbo].[P_PreApproval_COST] b ON a.CN = b.HTCode
LEFT OUTER JOIN [dbo].[P_PreUploadOrder_COST] d ON a.CN = d.HTCode 
LEFT OUTER JOIN WP_QYUSER c ON a.UserId = c.UserId  
WHERE (d.State IS NULL or d.[State] <> 4 or a.IsOrderUpload=0) and a.[State] not in (5,11)   ";
                var listParams = new List<SqlParameter>();
                if (startdate != "")
                {
                    sqlString += " AND a.DeliverTime >= DATEADD(month, datediff(month, 0, '" + startdate + "'), 0)  ";
                }
                if (enddate != "")
                {
                    sqlString += " AND a.DeliverTime < DATEADD(month, datediff(month, 0, '" + enddate + "'), 0)  ";
                }
                if (sltTA != null)
                {
                    sqlString += " AND (a.TA in (" + sltTA + ")) ";
                }
                if (HTType == "0")
                {
                    sqlString += " AND a.Address <> N'院外' ";
                }
                if (HTType == "1")
                {
                    sqlString += " AND a.Address = N'院外' ";
                }
                sqlString += " group by a.TA,b.ApplierName, b.ApplierMUDID,CASE WHEN c.[State] = 1 THEN N'否' ELSE N'是' END ,a.TransferUserMUDID,d.ReAssignBUHeadMUDID ";
                sqlString += " order by a.TA ";

                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_ORDER_UnfinishedOrder_View>(sqlString, listParams.ToArray());
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
            }
            return null;
        }

        public List<P_ORDER_UnfinishedData_View> ExportUnfinishedData(string startdate, string enddate, string sltTA, string HTType)
        {
            try
            {
                List<P_ORDER_UnfinishedData_View> rtnData;
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                string conditionstartDate = string.Empty;
                string conditionendDate = string.Empty;
                string conditionHT = string.Empty;
                string conditionNonHT = string.Empty;
                string conditionTA = string.Empty;
                if (startdate != "")
                {
                    conditionstartDate = " AND a.DeliverTime >= DATEADD(month, datediff(month, 0, '" + startdate + "'), 0)  ";
                }
                if (enddate != "")
                {
                    conditionendDate = " AND a.DeliverTime < DATEADD(month, datediff(month, 0, '" + enddate + "'), 0)  ";
                }
                if (HTType == "0")
                {
                    conditionHT = " AND a.Address <> N'院外' ";
                }
                if (HTType == "1")
                {
                    conditionNonHT = " AND a.Address = N'院外' ";
                }
                if (sltTA != null)
                {
                    conditionTA = " AND (a.TA in (" + sltTA + ")) ";
                }

                string sqlString = "SELECT  m.DeliverTime,sum(m.OrdersCount) as newOrdersCount,sum(m.UnfinishedCount) as newUnfinishedCount from  "
+ "(SELECT datepart(year,a.DeliverTime)*100+datepart(month,a.DeliverTime) as DeliverTime, COUNT(a.EnterpriseOrderId) as OrdersCount,'' as UnfinishedCount "
+ " from [dbo].[P_ORDER_COST] a WITH(NOLOCK) "
+ " LEFT OUTER JOIN [dbo].[P_PreApproval_COST] b ON a.CN = b.HTCode "
+ " WHERE b.BudgetTotal<>0.00 and a.[State] not in (5,11) " + conditionstartDate + conditionendDate + conditionHT + conditionNonHT + conditionTA +
"  group by datepart(year,a.DeliverTime)*100+datepart(month,a.DeliverTime) "
+ " union "
+ " SELECT datepart(year,a.DeliverTime)*100+datepart(month,a.DeliverTime) as DeliverTime,'' as OrdersCount,COUNT(a.EnterpriseOrderId) as UnfinishedCount "
+ " from [dbo].[P_ORDER_COST] a WITH(NOLOCK) "
+ " LEFT OUTER JOIN [dbo].[P_PreUploadOrder_COST] d ON a.CN = d.HTCode  "
+ " WHERE  (d.State IS NULL or d.[State] <> 4 or a.IsOrderUpload=0) and a.[State] not in (5,11) " + conditionstartDate + conditionendDate + conditionHT + conditionNonHT + conditionTA +
"  group by datepart(year,a.DeliverTime)*100+datepart(month,a.DeliverTime)) m "
+ " group by m.DeliverTime "
+ " order by m.DeliverTime desc   ";

                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_ORDER_UnfinishedData_View>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
            }
            return null;
        }

        public List<P_ORDER_UnfinishedDM_View> ExportUnfinishedDM(string startdate, string enddate, string sltTA, string HTType)
        {
            try
            {
                List<P_ORDER_UnfinishedDM_View> rtnData;
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                string conditionstartDate = string.Empty;
                string conditionendDate = string.Empty;
                string conditionHT = string.Empty;
                string conditionNonHT = string.Empty;
                string conditionTA = string.Empty;
                if (startdate != "")
                {
                    conditionstartDate = " AND a.DeliverTime >= DATEADD(month, datediff(month, 0, '" + startdate + "'), 0)  ";
                }
                if (enddate != "")
                {
                    conditionendDate = " AND a.DeliverTime < DATEADD(month, datediff(month, 0, '" + enddate + "'), 0)  ";
                }
                if (HTType == "0")
                {
                    conditionHT = " AND a.Address <> N'院外' ";
                }
                if (HTType == "1")
                {
                    conditionNonHT = " AND a.Address = N'院外' ";
                }
                if (sltTA != null)
                {
                    conditionTA = " AND (a.TA in (" + sltTA + ")) ";
                }

                string sqlString = "SELECT  m.TA,sum(m.TransferDM) as newTransferDM,sum(m.UnfinishedCount) as newUnfinishedCount from   "
+ "(SELECT a.TA,COUNT(distinct a.TransferUserMUDID) as TransferDM,'' as UnfinishedCount "
+ " from [dbo].[P_ORDER_COST] a WITH(NOLOCK) "
+ " LEFT OUTER JOIN [dbo].[P_PreUploadOrder_COST] d ON a.CN = d.HTCode "
+ " WHERE (d.State IS NULL or d.[State] <> 4 or a.IsOrderUpload=0) and a.[State] not in (5,11) and a.IsTransfer=1  "
+ conditionstartDate + conditionendDate + conditionHT + conditionNonHT + conditionTA +
"  group by a.TA  "
+ " UNION "
+ " SELECT a.TA,'' as TransferDM,COUNT(a.EnterpriseOrderId) as UnfinishedCount "
+ " from [dbo].[P_ORDER_COST] a WITH(NOLOCK) "
+ " LEFT OUTER JOIN [dbo].[P_PreUploadOrder_COST] d ON a.CN = d.HTCode  "
+ " LEFT OUTER JOIN WP_QYUSER c ON a.UserId = c.UserId  "
+ " WHERE (d.State IS NULL or d.[State] <> 4 or a.IsOrderUpload=0) and a.[State] not in (5,11) and a.IsTransfer=1 and c.[State] <> 1 "
+ conditionstartDate + conditionendDate + conditionHT + conditionNonHT + conditionTA +
"  group by a.TA) m  "
+ " group by m.TA "
+ " order by m.TA  ";

                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_ORDER_UnfinishedDM_View>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
            }
            return null;
        }
        public List<P_ORDER_UnfinishedUser_View> ExportUnfinishedUser(string startdate, string enddate, string sltTA, string HTType)
        {
            try
            {
                List<P_ORDER_UnfinishedUser_View> rtnData;
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                string conditionstartDate = string.Empty;
                string conditionendDate = string.Empty;
                string conditionHT = string.Empty;
                string conditionNonHT = string.Empty;
                string conditionTA = string.Empty;
                string conditionGetDate = string.Empty;
                conditionGetDate = " AND a.DeliverTime < Dateadd(mm, Datediff(mm, 0, GETDATE())-1, 0)  ";
                if (startdate != "")
                {
                    conditionstartDate = " AND a.DeliverTime >= DATEADD(month, datediff(month, 0, '" + startdate + "'), 0)  ";
                }
                if (enddate != "")
                {
                    conditionendDate = " AND a.DeliverTime < DATEADD(month, datediff(month, 0, '" + enddate + "'), 0)  ";
                }
                if (HTType == "0")
                {
                    conditionHT = " AND a.Address <> N'院外' ";
                }
                if (HTType == "1")
                {
                    conditionNonHT = " AND a.Address = N'院外' ";
                }
                if (sltTA != null)
                {
                    conditionTA = " AND (a.TA in (" + sltTA + ")) ";
                }

                string sqlString = "SELECT a.TA,COUNT(distinct a.UserId) as UserCount,COUNT(a.EnterpriseOrderId) as UnfinishedCount   "
+ " from [dbo].[P_ORDER_COST] a WITH(NOLOCK) "
+ " LEFT OUTER JOIN [dbo].[P_PreUploadOrder_COST] d ON a.CN = d.HTCode "
+ " WHERE  (d.State IS NULL or d.[State] <> 4 or a.IsOrderUpload=0) and a.[State] not in (5,11)  "
+ conditionGetDate + conditionstartDate + conditionendDate + conditionHT + conditionNonHT + conditionTA +
"  group by a.TA  "
+ " order by a.TA  ";

                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_ORDER_UnfinishedUser_View>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
            }
            return null;
        }
        public List<P_ORDER_Unfinished_VIEW> ExportUnfinished(string startdate, string enddate, string sltTA, string HTType)
        {
            try
            {
                List<P_ORDER_Unfinished_VIEW> rtnData;
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                string conditionstartDate = string.Empty;
                string conditionendDate = string.Empty;
                string conditionHT = string.Empty;
                string conditionNonHT = string.Empty;
                string conditionTA = string.Empty;
                if (startdate != "")
                {
                    conditionstartDate = " AND a.DeliverTime >= DATEADD(month, datediff(month, 0, '" + startdate + "'), 0)  ";
                }
                if (enddate != "")
                {
                    conditionendDate = " AND a.DeliverTime < DATEADD(month, datediff(month, 0, '" + enddate + "'), 0)  ";
                }
                if (HTType == "0")
                {
                    conditionHT = " AND a.Address <> N'院外' ";
                }
                if (HTType == "1")
                {
                    conditionNonHT = " AND a.Address = N'院外' ";
                }
                if (sltTA != null)
                {
                    conditionTA = " AND (a.TA in (" + sltTA + ")) ";
                }

                string sqlString = "SELECT b.ApplierName, b.ApplierMUDID, e.Position,b.HTCode, b.Market, b.TA, b.Province, "
                    + " b.City, b.HospitalCode, b.HospitalName, b.CostCenter,a.Channel,a.DeliverTime,a.RestaurantId,  "
                    + " a.RestaurantName,a.AttendCount AS ORDAttendCount,  "
                    + " CASE WHEN a.XmsTotalPrice< 0 THEN a.TotalPrice ELSE a.XmsTotalPrice END AS totalFee, a.IsRetuen,  "
                    + " case when a.IsRetuen=0 then null when a.[State]= 5 then null  "
                    + " WHEN a.IsReturn= 1 and a.IsRetuen= 2 then N'是' else N'否' end as cancelState, "
                    + " c.cancelFeedback, c.cancelFailReason,  "
                    + " a.ReceiveState, a.[State] AS ORDState, a.RealPrice, a.IsMealSame,  "
                    + " CASE WHEN a.IsMealSame = 1 THEN N'是' WHEN a.IsMealSame = 2 THEN N'否' ELSE NULL END AS MealSame, "
                    + " a.RealPriceChangeReason,a.RealPriceChangeRemark, a.RealCount, a.RealCountChangeReason, a.RealCountChangeRemrak,  "
                    + " a.IsOrderUpload, d.CreateDate AS PUOCreateDate, d.BUHeadName AS PUOBUHeadName,  "
                    + " d.BUHeadMUDID AS PUOBUHeadMUDID,  d.BUHeadApproveDate AS ApproveDate,d.[State] AS PUOState, d.IsAttentSame,  "
                    + " CASE WHEN d.IsAttentSame = 1 THEN N'是' WHEN d.IsAttentSame IS NULL THEN NULL  ELSE N'否' END AS AttentSame, d.AttentSameReason,  "
                    + " d.IsMeetingInfoSame,CASE WHEN d.IsMeetingInfoSame = 1 THEN N'是' WHEN d.IsMeetingInfoSame = 2 THEN N'否' ELSE NULL END AS MeetingInfoSame, "
                    + " d.MeetingInfoSameReason,d.IsReopen,d.MeetingInfoSameReason,d.IsReopen, CASE WHEN d.IsReopen = 1 THEN N'是' WHEN d.IsReopen = 0 THEN N'否' ELSE NULL END AS Reopen,  "
                    + " d.ReopenOperatorName, d.ReopenOperatorMUDID, d.ReopenOperateDate,  "
                    + " d.ReopenOriginatorName,d.ReopenOriginatorMUDID,d.ReopenReason,  "
                    + " d.ReopenRemark,a.IsTransfer, a.TransferOperatorName, a.TransferOperatorMUDID,  "
                    + " a.TransferUserName, a.TransferUserMUDID, a.TransferOperateDate,d.IsReAssign,  "
                    + " CASE WHEN d.IsReAssign = 1 THEN N'是' WHEN d.IsReAssign = 0 THEN N'否' ELSE NULL END AS ReAssign,  "
                    + " d.ReAssignOperatorName, d.ReAssignOperatorMUDID, d.ReAssignBUHeadName, "
                    + " d.ReAssignBUHeadMUDID, d.ReAssignBUHeadApproveDate, a.SpecialOrderReason, "
                    + " CASE WHEN e.[State] = 1 THEN N'否' ELSE N'是' END AS IsWorkdayQuit  "
                    + " FROM [dbo].[P_ORDER_COST] a WITH(NOLOCK) "
                    + " LEFT OUTER JOIN [P_PreApproval_COST] b ON a.CN = b.HTCode  "
                    + " LEFT OUTER JOIN P_ORDER_XMS_REPORT c ON a.XmsOrderId = c.XmsOrderId  "
                    + " LEFT OUTER JOIN [P_PreUploadOrder_COST] d ON a.CN = d.HTCode  "
                    + " LEFT OUTER JOIN WP_QYUSER e ON a.UserId = e.UserId  "
                    + " WHERE (d.[State] IS NULL or d.[State] <> 4 or a.IsOrderUpload=0) and a.[State] not in (5,11)  "
                    + conditionstartDate + conditionendDate + conditionHT + conditionNonHT + conditionTA
                    + " order by a.DeliverTime desc  ";

                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_ORDER_Unfinished_VIEW>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
            }
            return null;
        }

        #region 医院覆盖率
        public List<P_HOSPITAL_COVERAGE> LoadHospitalCoverageData(string date, string ta, string channel)
        {
            try
            {
                string sqlString = string.Empty;
                string conditionDate = string.Empty;
                string conditionTA = string.Empty;
                var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
                conditionDate = " CopyDate >= DATEADD(month, datediff(month, 0, '" + date + "'), 0) AND CopyDate <= dateadd(second, -1, DATEADD(month, datediff(month, -1, '" + date + "'), 0))";
                if (!string.IsNullOrEmpty(ta))
                {
                    conditionTA = " WHERE M.TERRITORY_TA in (" + ta + ")";
                }
                List<P_HOSPITAL_COVERAGE> rtnList = new List<P_HOSPITAL_COVERAGE>();
                sqlString = " SELECT Territory_TA,M.Market,COUNT(M.HospitalCode) AS AddressCnt,COUNT(CASE WHEN M.TotalCount>=1  THEN M.TotalCount END) AS BrandCnt1, "
                            + "COUNT(CASE WHEN M.TotalCount>=2  THEN M.TotalCount END) AS BrandCnt2, "
                            + "COUNT(CASE WHEN M.TotalCount>=3  THEN M.TotalCount END) AS BrandCnt3, "
                            + "COUNT(CASE WHEN M.TotalCount>=4  THEN M.TotalCount END) AS BrandCnt4, "
                            + "COUNT(CASE WHEN M.TotalCount>=5  THEN M.TotalCount END) AS BrandCnt5, "
                            + "COUNT(CASE WHEN M.BreakfastCount>0  THEN M.BreakfastCount END) AS BreakfastCnt, "
                            + "COUNT(CASE WHEN M.TeaCount>0  THEN M.TeaCount END) AS TeaCnt,"
                            + "CASE WHEN COUNT(M.HospitalCode) = 0 THEN CAST(0 AS DECIMAL(10,4)) ELSE "
                            + "CAST((CAST(COUNT(CASE WHEN M.TotalCount>=1  THEN M.TotalCount END) AS DECIMAL(10,4))/CAST(COUNT(M.HospitalCode) AS DECIMAL(10,4))) AS DECIMAL(10,4)) END AS BrandCoverage1, "
                            + "CASE WHEN COUNT(M.HospitalCode) = 0 THEN CAST(0 AS DECIMAL(10,4)) ELSE "
                            + "CAST((CAST(COUNT(CASE WHEN M.TotalCount>=2  THEN M.TotalCount END) AS DECIMAL(10,4))/CAST(COUNT(M.HospitalCode) AS DECIMAL(10,4))) AS DECIMAL(10,4)) END AS BrandCoverage2, "
                            + "CASE WHEN COUNT(M.HospitalCode) = 0 THEN CAST(0 AS DECIMAL(10,4)) ELSE "
                            + "CAST((CAST(COUNT(CASE WHEN M.TotalCount>=3  THEN M.TotalCount END) AS DECIMAL(10,4))/CAST(COUNT(M.HospitalCode) AS DECIMAL(10,4))) AS DECIMAL(10,4)) END AS BrandCoverage3, "
                            + "CASE WHEN COUNT(M.HospitalCode) = 0 THEN CAST(0 AS DECIMAL(10,4)) ELSE "
                            + "CAST((CAST(COUNT(CASE WHEN M.TotalCount>=4  THEN M.TotalCount END) AS DECIMAL(10,4))/CAST(COUNT(M.HospitalCode) AS DECIMAL(10,4))) AS DECIMAL(10,4)) END AS BrandCoverage4, "
                            + "CASE WHEN COUNT(M.HospitalCode) = 0 THEN CAST(0 AS DECIMAL(10,4)) ELSE "
                            + "CAST((CAST(COUNT(CASE WHEN M.TotalCount>=5  THEN M.TotalCount END) AS DECIMAL(10,4))/CAST(COUNT(M.HospitalCode) AS DECIMAL(10,4))) AS DECIMAL(10,4)) END AS BrandCoverage5, "
                            + "CASE WHEN COUNT(M.HospitalCode) = 0 THEN CAST(0 AS DECIMAL(10,4)) ELSE "
                            + "CAST((CAST(COUNT(CASE WHEN M.BreakfastCount>0  THEN M.TotalCount END) AS DECIMAL(10,4))/CAST(COUNT(M.HospitalCode) AS DECIMAL(10,4))) AS DECIMAL(10,4)) END AS BreakfastCoverage, "
                            + "CASE WHEN COUNT(M.HospitalCode) = 0 THEN CAST(0 AS DECIMAL(10,4)) ELSE "
                            + "CAST((CAST(COUNT(CASE WHEN M.TeaCount>0  THEN M.TotalCount END) AS DECIMAL(10,4))/CAST(COUNT(M.HospitalCode) AS DECIMAL(10,4))) AS DECIMAL(10,4)) END AS TeaCoverage "
                            + " FROM ( "
                            + "SELECT A.HospitalCode, B.TERRITORY_TA, B.Market,C.TotalCount,C.BreakfastCount,C.LunchCount,C.TeaCount"
                            + " FROM "
                            + " (SELECT * from P_HOSPITAL_COST where isdelete = 0 AND Address<>N'院外' AND "
                            + conditionDate
                            + ") A "
                            + " LEFT JOIN "
                            + " (SELECT DISTINCT TERRITORY_TA,HospitalCode,Market from [Territory_Hospital_COST] WHERE"
                            + conditionDate
                            + ") B "
                            + " ON A.GskHospital = B.HospitalCode "
                            + " LEFT JOIN ( ";
                if (channel == "全部")
                {
                    sqlString += " SELECT GskHospital,SUM(TotalCount) AS TotalCount,SUM(BreakfastCount) AS BreakfastCount,SUM(LunchCount) AS LunchCount,SUM(TeaCount) AS TeaCount "
                                + " FROM dbo.P_HOSPITAL_RANGE_RESTAURANTCOUNT_COST WHERE "
                                + conditionDate
                                + " GROUP BY GskHospital";
                }
                else
                {
                    sqlString += " SELECT * FROM dbo.P_HOSPITAL_RANGE_RESTAURANTCOUNT_COST WHERE DataSources = '" + channel + "' AND "
                                + conditionDate;
                }
                sqlString += " ) C "
                            + " ON A.HospitalCode  = C.GskHospital "
                            + " WHERE B.TERRITORY_TA is not null and B.TERRITORY_TA <> '' "
                            + " )M "
                            + conditionTA
                            + " GROUP BY M.TERRITORY_TA ,M.Market";
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnList = sqlServerTemplate.Load<P_HOSPITAL_COVERAGE>(sqlString, new SqlParameter[] { });
                }
                return rtnList;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
            }
            return null;
        }

        public List<P_HOSPITAL_COVERAGE_TOTAL> LoadHospitalCoverageRxData(string date, string ta, string channel)
        {
            try
            {
                string sqlString = string.Empty;
                string conditionDate = string.Empty;
                string conditionTA = string.Empty;
                var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
                conditionDate = " CopyDate >= DATEADD(month, datediff(month, 0, '" + date + "'), 0) AND CopyDate <= dateadd(second, -1, DATEADD(month, datediff(month, -1, '" + date + "'), 0))";
                if (!string.IsNullOrEmpty(ta))
                {
                    conditionTA = " WHERE M.TERRITORY_TA in (" + ta + ")";
                }
                List<P_HOSPITAL_COVERAGE_TOTAL> rtnList = new List<P_HOSPITAL_COVERAGE_TOTAL>();
                sqlString = " SELECT M.HospitalCode,M.Market,M.TotalCount,M.BreakfastCount,M.LunchCount,M.TeaCount FROM ("
                            + " SELECT A.HospitalCode,B.Market, B.TERRITORY_TA, C.TotalCount,C.BreakfastCount,C.LunchCount,C.TeaCount"
                            + " FROM "
                            + " (SELECT * from P_HOSPITAL_COST where isdelete = 0 AND Address<>N'院外' AND "
                            + conditionDate
                            + ") A "
                            + " LEFT JOIN "
                            + " (SELECT DISTINCT TERRITORY_TA,HospitalCode,Market from [Territory_Hospital_COST] WHERE"
                            + conditionDate
                            + ") B "
                            + " ON A.GskHospital = B.HospitalCode "
                            + " LEFT JOIN ( ";
                if (channel == "全部")
                {
                    sqlString += " SELECT GskHospital,SUM(TotalCount) AS TotalCount,SUM(BreakfastCount) AS BreakfastCount,SUM(LunchCount) AS LunchCount,SUM(TeaCount) AS TeaCount "
                                + " FROM dbo.P_HOSPITAL_RANGE_RESTAURANTCOUNT_COST WHERE "
                                + conditionDate
                                + " GROUP BY GskHospital";
                }
                else
                {
                    sqlString += " SELECT * FROM dbo.P_HOSPITAL_RANGE_RESTAURANTCOUNT_COST WHERE DataSources = '" + channel + "' AND "
                                + conditionDate;
                }
                sqlString += " ) C "
                            + " ON A.HospitalCode  = C.GskHospital "
                            + " WHERE B.TERRITORY_TA is not null and B.TERRITORY_TA <> '' AND B.Market = 'Rx' AND C.TotalCount IS NOT NULL "
                            + " )M "
                            + conditionTA
                            + " GROUP BY M.HospitalCode,M.Market,M.TotalCount,M.BreakfastCount,M.LunchCount,M.TeaCount";
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnList = sqlServerTemplate.Load<P_HOSPITAL_COVERAGE_TOTAL>(sqlString, new SqlParameter[] { });
                }
                return rtnList;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
            }
            return null;
        }

        public List<P_TA> LoadTAForGroup()
        {
            try
            {
                List<P_TA> rtnData;
                string sqlString = "SELECT DISTINCT TA as Name from dbo.P_PreApproval_COST WHERE MRTerritoryCode IS NOT NULL AND MRTerritoryCode <>'' UNION SELECT DISTINCT TERRITORY_TA AS TA FROM " + _dbName + ".dbo.Territory_Hospital_COST "
                                 + "UNION SELECT DISTINCT TERRITORY_TA AS TA FROM " + _dbName + ".dbo.Territory_Hospital";
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_TA>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
                return null;
            }
        }
        #endregion

    }
}