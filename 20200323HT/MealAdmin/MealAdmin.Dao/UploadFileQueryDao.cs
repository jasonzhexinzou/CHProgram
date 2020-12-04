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
using System.Data.SqlTypes;
namespace MealAdmin.Dao
{
    public class UploadFileQueryDao : IUploadFileQueryDao
    {
        [Bean("sqlServerTemplFactory")]
        public DbTemplateFactory sqlServerTemplFactory { get; set; }

        #region Sql语句
        private string selectSql = "select a.ModifyDate as ModifyDate,a.ApplierName as ApplierName,a.ApplierMUDID as ApplierMUDID,u.Position as Position, a.ID as ID, " +
            "b.ApplierMobile as ApplierMobile,a.HTCode as HTCode,b.Market as Market,b.VeevaMeetingID as VeevaMeetingID,b.TA as TA," +
            "b.Province as Province,b.City as City,b.HospitalName as HospitalName,b.MeetingName as MeetingName," +
            "b.AttendCount as AttendCount,b.CostCenter as CostCenter,c.Channel as Channel,c.CN as CN," +
            "c.DeliverTime as DeliverTime,c.AttendCount as AttendCounts,c.FoodCount as FoodCount," +
            "c.TotalPrice as TotalPrice,c.XmsTotalPrice as XmsTotalPrice,c.ChangeTotalPriceReason as ChangeTotalPriceReason," +
            "c.ReceiveDate as ReceiveDate,c.RealPrice as RealPrice,c.RealPriceChangeReason as RealPriceChangeReason," +
            "c.RealPriceChangeRemark as RealPriceChangeRemark,c.RealCount as RealCount,c.RealCountChangeReason as RealCountChangeReason," +
            "c.RealCountChangeRemrak as RealCountChangeRemrak,c.State as State,c.IsOrderUpload as IsOrderUpload," +
            "b.IsReAssign as IsReAssign,a.BUHeadName as ReAssignBUHeadName,a.BUHeadMUDID as ReAssignBUHeadMUDID," +
            "a.State as Stated,a.IsAttentSame as IsAttentSame,a.AttentSameReason as AttentSameReason," +
            "a.IsReopen as IsReopen,a.ReopenOperatorName as ReopenOperatorName,a.ReopenOperatorMUDID as ReopenOperatorMUDID," +
            "a.ReopenOperateDate as ReopenOperateDate,a.ReopenReason as ReopenReason,a.UploadReOpenState as UploadReOpenState," +
            "CASE WHEN c.IsRetuen=0 and c.State<>5 THEN N'否' when c.State=5 then null ELSE N'是' END as IsCancel," +
            "case when c.IsRetuen=0 then null when c.State=5 then null when c.IsReturn=1 and c.IsRetuen=2 then N'是' else N'否' end as CancelState," +
            "CASE WHEN c.ReceiveState = 6 THEN N'是' WHEN c.ReceiveState = 7 THEN N'自动' WHEN c.ReceiveState = 8 THEN N'未送达' WHEN(c.State = 5 OR c.State = 11) THEN null END as IsReceive," +
            "case when c.IsMealSame=1 then N'是' when c.IsMealSame=2 then N'否' else null end as IsMealSame," +
            "c.SpecialOrderReason as SpecialReason,CONVERT(DATE, a.BUHeadApproveDate, 23) as ApproveDate," +
            "case when a.IsMeetingInfoSame=1 then N'是' when a.IsMeetingInfoSame=2 then N'否' else null end as IsMeetingSame," +
            "a.MeetingInfoSameReason as MeetingSameReason," +
            "a.SpecialReason as SpecialUploadReason," +
            "a.MMCoEImageOne as MMCoEImageOne,a.MMCoEImageTwo as MMCoEImageTwo,a.MMCoEImageThree as MMCoEImageThree from [P_PreUploadOrder] a  WITH(NOLOCK)" +
            "left join[P_PreApproval] b on a.HTCode = b.HTCode and a.ApplierMUDID = b.ApplierMUDID " +
            "left join[P_ORDER] c on a.HTCode = c.CN  and c.[State]<>5 and c.[State]<>11" +
            "left join [WP_QYUSER] AS u " +
            "on a.ApplierMUDID=u.UserId where 1=1 ";
        //"where (@HTCode='' or a.HTCode like '%' + @HTCode + '%') and (@ApplierMUDID='' or a.ApplierMUDID like '%' + @ApplierMUDID + '%') " +
        //"and ((@Begin='') or (c.DeliverTime >= @Begin)) and ((@End='') or (c.DeliverTime <= @End)) " +
        //"and ((@IsReopen='') or (a.IsReopen = @IsReopen)) AND c.State NOT IN (5,11) ";

        private string selectOrderBySql = " ORDER BY a.ModifyDate DESC ";
        #endregion

        #region 上传文件查询
        public List<P_UploadFileQuery_TXT> UploadFileLoad(string HTCode, string ApplierMUDID, string Begin, string End, string State)
        {
            List<P_UploadFileQuery_TXT> rtnData;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                string selectSqlBase = selectSql;
                if (!string.IsNullOrEmpty(HTCode))
                {
                    selectSqlBase += " and (@HTCode='' or @HTCode like '%' + Rtrim(a.HTCode) + '%')";
                }
                if (!string.IsNullOrEmpty(ApplierMUDID))
                {
                    selectSqlBase += " and (@ApplierMUDID='' or a.ApplierMUDID like '%' + @ApplierMUDID + '%')";
                }
                if (!string.IsNullOrEmpty(Begin))
                {
                    selectSqlBase += " and ((@Begin='') or (c.DeliverTime >= @Begin))";
                }
                if (!string.IsNullOrEmpty(End))
                {
                    selectSqlBase += " and ((@End='') or (c.DeliverTime <= @End))";
                }
                if (!string.IsNullOrEmpty(State))
                {
                    selectSqlBase += " and ((@IsReopen='') or (a.IsReopen = @IsReopen)) AND c.State NOT IN (5,11)";
                }
                rtnData = sqlServerTemplate.Load<P_UploadFileQuery_TXT>(selectSqlBase + selectOrderBySql,
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@HTCode", HTCode),
                        SqlParameterFactory.GetSqlParameter("@ApplierMUDID", ApplierMUDID),
                        SqlParameterFactory.GetSqlParameter("@Begin", Begin),
                        SqlParameterFactory.GetSqlParameter("@End", End),
                        SqlParameterFactory.GetSqlParameter("@IsReopen", State)
                    });
            }
            return rtnData;
        }
        #endregion

        #region 上传文件查询
        public List<P_UploadFileQuery_TXT> LoadPage(string HTCode, string ApplierMUDID, string Begin, string End, string State, int rows, int page, out int total)
        {
            List<P_UploadFileQuery_TXT> rtnData;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                string selectSqlBase = selectSql;
                if (!string.IsNullOrEmpty(HTCode))
                {
                    selectSqlBase += " and (@HTCode='' or @HTCode like '%' + Rtrim(a.HTCode) + '%')";
                }
                if (!string.IsNullOrEmpty(ApplierMUDID))
                {
                    selectSqlBase += " and (@ApplierMUDID='' or a.ApplierMUDID like '%' + @ApplierMUDID + '%')";
                }
                if (!string.IsNullOrEmpty(Begin))
                {
                    selectSqlBase += " and ((@Begin='') or (c.DeliverTime >= @Begin))";
                }
                if (!string.IsNullOrEmpty(End))
                {
                    selectSqlBase += " and ((@End='') or (c.DeliverTime <= @End))";
                }
                if (!string.IsNullOrEmpty(State))
                {
                    selectSqlBase += " and ((@IsReopen='') or (a.IsReopen = @IsReopen)) AND c.State NOT IN (5,11)";
                }
                conn.Open();
                rtnData = sqlServerTemplate.LoadPages<P_UploadFileQuery_TXT>(rows, page, out total,
                    selectSqlBase,
                    selectOrderBySql, new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@HTCode", HTCode),
                        SqlParameterFactory.GetSqlParameter("@ApplierMUDID", ApplierMUDID),
                        SqlParameterFactory.GetSqlParameter("@Begin",Begin==""?"":Begin+" 00:00:00.000"),
                        SqlParameterFactory.GetSqlParameter("@End",End==""?"":End+" 23:59:59.999"),
                        SqlParameterFactory.GetSqlParameter("@IsReopen", State)
                    });
            }
            return rtnData;
        }
        #endregion

        #region 导出上传文件查询
        public List<P_UploadFileQuery_TXT> ExportUploadFile(string HTCode, string ApplierMUDID, string Begin, string End, string State)
        {
            string selectExcelSql = "select a.ApplierName as ApplierName,a.ApplierMUDID as ApplierMUDID, a.ID as ID, " +
            "b.ApplierMobile as ApplierMobile,a.HTCode as HTCode,b.Market as Market,b.VeevaMeetingID as VeevaMeetingID,b.TA as TA," +
            "b.Province as Province,b.City as City,b.HospitalName as HospitalName,b.MeetingName as MeetingName," +
            "b.AttendCount as AttendCount,b.CostCenter as CostCenter,c.Channel as Channel,c.CN as CN," +
            "c.DeliverTime as DeliverTime,c.AttendCount as AttendCounts,c.FoodCount as FoodCount," +
            "c.TotalPrice as TotalPrice,c.XmsTotalPrice as XmsTotalPrice,c.ChangeTotalPriceReason as ChangeTotalPriceReason," +
            "c.ReceiveDate as ReceiveDate,c.RealPrice as RealPrice,c.RealPriceChangeReason as RealPriceChangeReason," +
            "c.RealPriceChangeRemark as RealPriceChangeRemark,c.RealCount as RealCount,c.RealCountChangeReason as RealCountChangeReason," +
            "c.RealCountChangeRemrak as RealCountChangeRemrak,c.State as State,c.IsOrderUpload as IsOrderUpload," +
            "b.IsReAssign as IsReAssign,a.BUHeadName as ReAssignBUHeadName,a.BUHeadMUDID as ReAssignBUHeadMUDID," +
            "a.State as Stated,a.IsAttentSame as IsAttentSame,a.AttentSameReason as AttentSameReason," +
            "a.IsReopen as IsReopen,a.ReopenOperatorName as ReopenOperatorName,a.ReopenOperatorMUDID as ReopenOperatorMUDID,a.ReopenOriginatorName as ReopenOriginatorName,a.ReopenOriginatorMUDID as ReopenOriginatorMUDID," +
            "a.ReopenOperateDate as ReopenOperateDate,a.ReopenReason as ReopenReason,a.ReopenRemark as ReopenRemark,a.UploadReOpenState as UploadReOpenState," +
            "CASE WHEN c.IsRetuen=0 and c.State<>5 THEN N'否' when c.State=5 then null ELSE N'是' END as IsCancel," +
            "case when c.IsRetuen=0 then null when c.State=5 then null when c.IsReturn=1 and c.IsRetuen=2 then N'是' else N'否' end as CancelState," +
            "CASE WHEN c.ReceiveState = 6 THEN N'是' WHEN c.ReceiveState = 7 THEN N'自动' WHEN c.ReceiveState = 8 THEN N'未送达' WHEN(c.State = 5 OR c.State = 11) THEN null END as IsReceive," +
            "case when c.IsMealSame=1 then N'是' when c.IsMealSame=2 then N'否' else null end as IsMealSame," +
            "c.SpecialOrderReason as SpecialReason,CONVERT(DATE, a.BUHeadApproveDate, 23) as ApproveDate," +
            "case when a.IsMeetingInfoSame=1 then N'是' when a.IsMeetingInfoSame=2 then N'否' else null end as IsMeetingSame," +
            "a.MeetingInfoSameReason as MeetingSameReason," +
            "a.SpecialReason as SpecialUploadReason," +
            "a.MMCoEImageOne as MMCoEImageOne,a.MMCoEImageTwo as MMCoEImageTwo,a.MMCoEImageThree as MMCoEImageThree,u.Position as Position " +
            "from [P_PreUploadOrder] a WITH(NOLOCK) " +
            "left join[P_PreApproval] b on a.HTCode = b.HTCode and a.ApplierMUDID = b.ApplierMUDID " +
            "left join[P_ORDER] c on a.HTCode = c.CN and a.ApplierMUDID = c.UserId " +
            "left join WP_QYUSER AS u " +
            "on a.ApplierMUDID=u.UserId " +
            "where (@HTCode='' or @HTCode like '%' + Rtrim(a.HTCode) + '%') and (@ApplierMUDID='' or a.ApplierMUDID like '%' + @ApplierMUDID + '%') " +
            "and ((@Begin='') or (c.DeliverTime >= @Begin)) and ((@End='') or (c.DeliverTime <= @End)) " +
            "and ((@IsReopen='') or (a.IsReopen = @IsReopen)) ORDER BY c.DeliverTime DESC ";

            List<P_UploadFileQuery_TXT> rtnData;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<P_UploadFileQuery_TXT>(selectExcelSql,
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@HTCode", HTCode),
                        SqlParameterFactory.GetSqlParameter("@ApplierMUDID", ApplierMUDID),
                        SqlParameterFactory.GetSqlParameter("@Begin",Begin==""?"":Begin+" 00:00:00.000"),
                        SqlParameterFactory.GetSqlParameter("@End",End==""?"":End+" 23:59:59.999"),
                        SqlParameterFactory.GetSqlParameter("@IsReopen", State)
                    });
            }
            return rtnData;
        }
        #endregion

        #region 上传文件查询记录
        public List<P_UploadFileQuery_TXT> RecordsLoad(string HTCode, string ApplierMUDID, string Begin, string End, string State, int rows, int page, out int total)
        {
            List<P_UploadFileQuery_TXT> rtnData;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {

                conn.Open();
                rtnData = sqlServerTemplate.LoadPages<P_UploadFileQuery_TXT>(rows, page, out total, "select a.ApplierName as ApplierName,a.ApplierMUDID as ApplierMUDID, a.ID as ID, " +
            "b.ApplierMobile as ApplierMobile,a.HTCode as HTCode,b.Market as Market,b.TA as TA," +
            "b.Province as Province,b.City as City,b.HospitalName as HospitalName,b.MeetingName as MeetingName," +
            "b.AttendCount as AttendCount,b.CostCenter as CostCenter,c.Channel as Channel,c.CN as CN," +
            "c.DeliverTime as DeliverTime,c.AttendCount as AttendCounts,c.FoodCount as FoodCount," +
            "c.TotalPrice as TotalPrice,c.XmsTotalPrice as XmsTotalPrice,c.ChangeTotalPriceReason as ChangeTotalPriceReason," +
            "c.ReceiveDate as ReceiveDate,c.RealPrice as RealPrice,c.RealPriceChangeReason as RealPriceChangeReason," +
            "c.RealPriceChangeRemark as RealPriceChangeRemark,c.RealCount as RealCount,c.RealCountChangeReason as RealCountChangeReason," +
            "c.RealCountChangeRemrak as RealCountChangeRemrak,c.State as State,c.IsOrderUpload as IsOrderUpload," +
            "b.IsReAssign as IsReAssign,b.ReAssignBUHeadName as ReAssignBUHeadName,b.ReAssignBUHeadMUDID as ReAssignBUHeadMUDID," +
            "a.State as Stated,a.IsAttentSame as IsAttentSame,c.RealCount as RealCounts,a.AttentSameReason as AttentSameReason," +
            "a.IsReopen as IsReopen,a.ReopenOperatorName as ReopenOperatorName,a.ReopenOperatorMUDID as ReopenOperatorMUDID," +
            "a.ReopenOperateDate as ReopenOperateDate,a.ReopenReason as ReopenReason,a.UploadReOpenState as UploadReOpenState," +
            "a.MMCoEImageOne as MMCoEImageOne,a.MMCoEImageTwo as MMCoEImageTwo from [P_PreUploadOrder] a WITH(NOLOCK) " +
            "left join[P_PreApproval] b on a.HTCode = b.HTCode and a.ApplierMUDID = b.ApplierMUDID " +
            "left join[P_ORDER] c on a.HTCode = c.CN and a.ApplierMUDID = c.UserId " +
            "where (@HTCode='' or @HTCode like '%' + Rtrim(a.HTCode) + '%') and (@ApplierMUDID='' or a.ApplierMUDID like '%' + @ApplierMUDID + '%') " +
            "and ((@Begin='') or (c.DeliverTime >= @Begin)) and ((@End='') or (c.DeliverTime < @End)) " +
            "and ((@IsReopen='') or (a.IsReopen = @IsReopen)) "
             , " ORDER BY c.DeliverTime DESC",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@HTCode", HTCode),
                        SqlParameterFactory.GetSqlParameter("@ApplierMUDID", ApplierMUDID),
                        SqlParameterFactory.GetSqlParameter("@Begin", Begin==""?"1900-1-1":Begin),
                        SqlParameterFactory.GetSqlParameter("@End", End==""?"1900-1-1":End),
                        SqlParameterFactory.GetSqlParameter("@IsReopen", State)
                    });
            }
            return rtnData;
        }
        #endregion

        public P_PREUPLOADORDER FindPreUploadFile(string id)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var preUploadFile = sqlServerTemplate.Find<P_PREUPLOADORDER>("select * from P_PreUploadOrder where id=@id"
                    , new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@id", id)
                    });
                return preUploadFile;
            }
        }

        public P_PREUPLOADORDER FindPreUploadFileByHTCode(string HTCode)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var preUploadFile = sqlServerTemplate.Find<P_PREUPLOADORDER>("select * from P_PreUploadOrder where HTCode=@HTCode"
                    , new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@HTCode", HTCode)
                    });
                return preUploadFile;
            }
        }

        public List<P_OrderApproveHistory> GetApproval(string id)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Load<P_OrderApproveHistory>(" Select *  "
                                                                    + " from P_UploadApproveHistory  "
                                                                    + " where PID = @ID ORDER BY ApproveDate",
                                                                                    new SqlParameter[]{
                                                                                        SqlParameterFactory.GetSqlParameter("@ID",id)
                                                                                    });
            }
        }

        #region 通过ID查询上传文件信息
        public List<P_UploadFileQuery_TXT> GetUpdateFileByID(string UpdateFilelID)
        {
            List<P_UploadFileQuery_TXT> rtnData;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                //rtnData = sqlServerTemplate.Load<P_UploadFileQuery_TXT>("SELECT * FROM P_PreUploadOrder WHERE (ID LIKE '%' + @ID + '%')",
                rtnData = sqlServerTemplate.Load<P_UploadFileQuery_TXT>("select a.ApplierName as ApplierName,a.ApplierMUDID as ApplierMUDID,u.Position AS Position,a.ID as ID, " +
                        "b.ApplierMobile as ApplierMobile,a.HTCode as HTCode,b.Market as Market,b.VeevaMeetingID as VeevaMeetingID,b.TA as TA," +
                        "b.Province as Province,b.City as City,b.HospitalName as HospitalName,b.MeetingName as MeetingName," +
                        "b.AttendCount as AttendCount,b.CostCenter as CostCenter,c.Channel as Channel,c.CN as CN," +
                        "c.DeliverTime as DeliverTime,c.AttendCount as AttendCounts,c.FoodCount as FoodCount," +
                        "c.TotalPrice as TotalPrice,c.XmsTotalPrice as XmsTotalPrice,c.ChangeTotalPriceReason as ChangeTotalPriceReason," +
                        "c.ReceiveDate as ReceiveDate,c.RealPrice as RealPrice,c.RealPriceChangeReason as RealPriceChangeReason," +
                        "c.RealPriceChangeRemark as RealPriceChangeRemark,c.RealCount as RealCount,c.RealCountChangeReason as RealCountChangeReason," +
                        "c.RealCountChangeRemrak as RealCountChangeRemrak,c.State as State,c.IsOrderUpload as IsOrderUpload," +
                        "b.IsReAssign as IsReAssign,a.BUHeadName as ReAssignBUHeadName,a.BUHeadMUDID as ReAssignBUHeadMUDID," +
                        "a.State as Stated,a.IsAttentSame as IsAttentSame,a.AttentSameReason as AttentSameReason," +
                        "a.IsReopen as IsReopen,a.ReopenOperatorName as ReopenOperatorName,a.ReopenOperatorMUDID as ReopenOperatorMUDID," +
                        "a.ReopenOperateDate as ReopenOperateDate,a.ReopenReason as ReopenReason,a.UploadReOpenState as UploadReOpenState," +
                        "CASE WHEN c.IsRetuen=0 and c.State<>5 THEN N'否' when c.State=5 then null ELSE N'是' END as IsCancel," +
                        "case when c.IsRetuen=0 then null when c.State=5 then null when c.IsReturn=1 and c.IsRetuen=2 then N'是' else N'否' end as CancelState," +
                        "CASE WHEN c.ReceiveState = 6 THEN N'是' WHEN c.ReceiveState = 7 THEN N'自动' WHEN c.ReceiveState = 8 THEN N'未送达' WHEN(c.State = 5 OR c.State = 11) THEN null END as IsReceive," +
                        "case when c.IsMealSame=1 then N'是' when c.IsMealSame=2 then N'否' else null end as IsMealSame," +
                        "c.SpecialOrderReason as SpecialReason,CONVERT(DATE, a.BUHeadApproveDate, 23) as ApproveDate," +
                        "case when a.IsMeetingInfoSame=1 then N'是' when a.IsMeetingInfoSame=2 then N'否' else null end as IsMeetingSame," +
                        "a.MeetingInfoSameReason as MeetingSameReason," +
                        "a.SpecialReason as SpecialUploadReason," +
                        "a.MMCoEImageOne as MMCoEImageOne,a.MMCoEImageTwo as MMCoEImageTwo,a.MMCoEImageThree as MMCoEImageThree from [P_PreUploadOrder] a  WITH(NOLOCK)" +
                        "left join[P_PreApproval] b on a.HTCode = b.HTCode and a.ApplierMUDID = b.ApplierMUDID " +
                        "left join[P_ORDER] c on a.HTCode = c.CN and a.ApplierMUDID = c.UserId " +
                        "left join [WP_QYUSER] AS u " +
                        "on a.ApplierMUDID=u.UserId " +
                        " WHERE (a.ID LIKE '%' + @ID + '%') AND c.State NOT IN (5,11)",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", UpdateFilelID)
                    });
            }
            return rtnData;
        }
        #endregion

        #region 定时导出上传文件（图片）
        public List<P_UploadFileQuery> TimingExpotr()
        {

            List<P_UploadFileQuery> rtnData;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var startTime = DateTime.Now.AddDays(-8).ToString("yyyy-MM-dd") + " 23:59:59";
                var endTime = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
                //var startTime = "2018-07-30 00:00:00";
                //var endTime = "2018-08-06 00:00:00";
                rtnData = sqlServerTemplate.Load<P_UploadFileQuery>("SELECT * FROM P_PreUploadOrder WITH(NOLOCK) WHERE BUHeadApproveDate>=@startTime AND BUHeadApproveDate<=@endTime AND State = 4",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@startTime", startTime),
                        SqlParameterFactory.GetSqlParameter("@endTime", endTime)
                    });
            }
            return rtnData;
        }

        #endregion

    }
}
