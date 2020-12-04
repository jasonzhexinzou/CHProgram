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
using MealAdmin.Entity.View;

namespace MealAdmin.Dao
{
    public class UploadOrderDao : IUploadOrderDao
    {
        [Bean("sqlServerTemplFactory")]
        public DbTemplateFactory sqlServerTemplFactory { get; set; }

        #region 加载HT编号
        /// <summary>
        /// 加载HT编号
        /// </summary>
        /// <returns></returns>
        public List<P_ORDER> LoadHTCode(string userId)
        {
            List<P_ORDER> rtnData;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<P_ORDER>("SELECT a.CN AS CN,a.MeetingName FROM P_ORDER a where (a.State=6 OR a.State=7 OR a.State=8 OR a.State=9) AND a.IsOrderUpload = 0 AND ((a.UserId = @UserId and isnull(IsTransfer,0)=0) or (TransferUserMUDID=@UserId and IsTransfer=1)) ORDER BY a.CreateDate DESC",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", userId)
                    });
            }

            return rtnData;
        }
        #endregion

        #region 根据订单号查询订单详情
        /// <summary>
        /// 根据订单号查询订单详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public P_ORDER FindOrderByHTCode(string HTCode)
        {
            P_ORDER rtnData;

            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Find<P_ORDER>("SELECT * FROM P_ORDER WHERE CN = @CN and State not in(5,11)",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@CN", HTCode)
                    });
            }
            return rtnData;
        }
        #endregion

        #region 上传文件BUHead审批通过
        /// <summary>
        /// 上传文件BUHead审批通过
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public int BUHeadApprove(Guid id, int state, string reason)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("UPDATE P_PreUploadOrder SET State=4,BUHeadApproveDate=@BUHeadApproveDate,ActionState='0' WHERE ID=@ID ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", id),
                        SqlParameterFactory.GetSqlParameter("@BUHeadApproveDate", DateTime.Now),
                    });
            }
        }
        #endregion

        #region 获取上传文件详情
        public P_ORDER LoadOrderInfo(Guid id)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var result = sqlServerTemplate.Find<P_ORDER>($"SELECT * FROM P_ORDER where ID=@ID", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", id)});
                return result;
            }
        }
        #endregion

        public int AddOrderApproveHistory(P_OrderApproveHistory OrderlHistory)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("insert into P_UploadApproveHistory values(@ID,@PID,@UserName,@UserID,@ActionType,@ApproveDate,@Comments,@Type) ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", OrderlHistory.ID),
                        SqlParameterFactory.GetSqlParameter("@PID", OrderlHistory.PID),
                        SqlParameterFactory.GetSqlParameter("@UserName", OrderlHistory.UserName),
                        SqlParameterFactory.GetSqlParameter("@UserID", OrderlHistory.UserId),
                        SqlParameterFactory.GetSqlParameter("@ActionType", OrderlHistory.ActionType),
                        SqlParameterFactory.GetSqlParameter("@ApproveDate", OrderlHistory.ApproveDate),
                        SqlParameterFactory.GetSqlParameter("@Comments", OrderlHistory.Comments),
                        SqlParameterFactory.GetSqlParameter("@Type", OrderlHistory.type),
                    });
            }
        }

        #region 上传文件BUHead审批驳回
        /// <summary>
        /// 上传文件BUHead审批驳回
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public int BUHeadReject(Guid id, int state, string reason)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("UPDATE P_PreUploadOrder SET State=2,BUHeadApproveDate=@BUHeadApproveDate,ActionState='0' WHERE ID=@ID ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", id),
                        SqlParameterFactory.GetSqlParameter("@BUHeadApproveDate", DateTime.Now),
                    });
            }
        }
        #endregion

        #region 上传文件MMCoE审批驳回
        /// <summary>
        /// 预申请BUHead审批驳回
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public int MMCoEReject(Guid id, int state, string reason)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("UPDATE P_PreUploadOrder SET State=@State WHERE ID=@ID ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", id),
                        SqlParameterFactory.GetSqlParameter("@State", state),
                    });
            }
        }
        #endregion

        #region 上传文件MMCoE审批通过
        /// <summary>
        /// 上传文件BUHead审批通过
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public int MMCoEApprove(Guid id, int state, string reason)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("UPDATE P_PreUploadOrder SET State=@State WHERE ID=@ID ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", id),
                        SqlParameterFactory.GetSqlParameter("@State", state)
                    });
            }
        }
        #endregion

        public List<P_PreUploadOrderState> LoadMyOrderUserId(string UserId, DateTime Begin, DateTime End, string State, int rows, int page, out int total)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.LoadPages<P_PreUploadOrderState>(rows, page, out total,
                    "SELECT po.ID,po.HTCode,po.State,po.CreateDate,pa.MeetingDate as MeetingDate,pa.HospitalName as HospitalName,po.IsTransfer,po.ApplierName FROM [P_PreUploadOrder] po inner join [P_PreApproval] pa on po.[HTCode]=pa.[HTCode] WHERE ((po.[ApplierMUDID]=@UserId and po.[IsTransfer]=0) or (po.[TransferUserMUDID]=@UserId and po.[IsTransfer]=1)) AND po.[CreateDate] > @begin AND po.[CreateDate] <= @end AND po.[State] in (" + State + ")"
                    , "  ORDER BY po.CreateDate DESC", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", UserId),
                        SqlParameterFactory.GetSqlParameter("@begin", Begin),
                        SqlParameterFactory.GetSqlParameter("@end", End)
                    });
                return list;
            }
        }

        public List<P_AutoTransferState> LoadMyAutoTransfer(string UserId, DateTime End, int rows, int page, out int total)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.LoadPages<P_AutoTransferState>(rows, page, out total,
                    "select po.ID,ad.HTCode,po.HospitalName,po.DeliverTime,ad.CreateDate,pa.ApplierName from [P_AutoTransferDetail] ad inner join [dbo].[P_ORDER] po on ad.htcode=po.CN inner join [dbo].[P_PreApproval] pa on ad.HTCode=pa.HTCode where ad.ToMUDID = @UserId and ad.CreateDate<@End"
                    , "  ORDER BY ad.CreateDate DESC", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", UserId),
                        SqlParameterFactory.GetSqlParameter("@End", End)
                    });
                return list;
            }
        }

        public List<P_PreUploadOrderState> LoadMyApprove(string UserId, DateTime Begin, DateTime End, string State, string Applicant, int rows, int page, out int total)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.LoadPages<P_PreUploadOrderState>(rows, page, out total,
                    $"SELECT po.ID,po.HTCode,po.State,po.CreateDate,pa.MeetingDate as MeetingDate,pa.HospitalName as HospitalName FROM [P_PreUploadOrder] po inner join [P_PreApproval] pa on po.[HTCode]=pa.[HTCode] WHERE ((po.[BUHeadMUDID]=@UserId and po.[IsReAssign]=0) or (po.[ReAssignBUHeadMUDID]=@UserId and po.[IsReAssign]=1)) AND po.[CreateDate] > @begin AND po.[CreateDate] <= @end AND po.[State] in (" + State + ") and ((po.[ApplierName] like '%" + Applicant + "%' or '" + Applicant + "'='') or (po.[ApplierMUDID] like '%" + Applicant + "%' or '" + Applicant + "'=''))",
                    " ORDER BY po.CreateDate DESC", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", UserId),
                        SqlParameterFactory.GetSqlParameter("@begin", Begin),
                        SqlParameterFactory.GetSqlParameter("@end", End),
                        SqlParameterFactory.GetSqlParameter("@Applicant", Applicant)
                    });
                return list;
            }
        }

        public List<P_PREUPLOADORDER> LoadMyApproveAll(string UserId, string Applicant)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<P_PREUPLOADORDER>(
                    $"SELECT * FROM [P_PreUploadOrder] WHERE [BUHeadMUDID]=@UserId AND  State='3' and ((ApplierName like '%" + Applicant + "%' or '" + Applicant + "'='') or (ApplierMUDID like '%" + Applicant + "%' or '" + Applicant + "'=''))",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", UserId),
                        SqlParameterFactory.GetSqlParameter("@Applicant", Applicant)
                    });
                return list;
            }
        }

        public P_PREUPLOADORDER FindActivityOrderByHTCode(string HTCode)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnData = sqlServerTemplate.Find<P_PREUPLOADORDER>("SELECT * FROM P_PreUploadOrder WHERE HTCode=@HTCode",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@HTCode", HTCode)
                    });
                return rtnData;
            }
        }

        public P_OrderApproveHistory LoadApproveHistoryInfo(Guid PID)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnData = sqlServerTemplate.Find<P_OrderApproveHistory>("SELECT top 1 * FROM P_UploadApproveHistory WHERE PID=@PID and ActionType in(2,3) order by [ApproveDate] desc",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@PID", PID),
                    });
                return rtnData;
            }
        }

        public List<P_OrderApproveHistory> FindorderApproveHistory(Guid PID)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnData = sqlServerTemplate.Load<P_OrderApproveHistory>("SELECT * from [P_OrderApproveHistory] WHERE PID=@PID",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@PID", PID)
                    });
                return rtnData;
            }
        }

        #region 新增文件上传信息
        /// <summary>
        /// 新增文件上传信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Add(P_PREUPLOADORDER entity)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var data = sqlServerTemplate.Find<P_PREUPLOADORDER>("SELECT * FROM dbo.P_PreUploadOrder WHERE HTCode=@HTCode", new SqlParameter[]
                {
                    SqlParameterFactory.GetSqlParameter("HTCode",entity.HTCode)
                });
                if (data != null)
                {
                    return 0;

                }
                else
                {
                    var res = sqlServerTemplate.ExecuteNonQuery(
                        "INSERT INTO P_PreUploadOrder (ID,ApplierName,ApplierMUDID,CreateDate,ModifyDate,HTCode,BUHeadName,BUHeadMUDID,IsReAssign,State,MMCoEImageOne,MMCoEImageTwo,MMCoEImageThree,FileType,IsAttentSame,AttentSameReason,SpecialReason,IsAddFile,IsMeetingInfoSame, MeetingInfoSameReason,Memo,IsTransfer,TransferOperatorMUDID,TransferOperatorName,TransferUserMUDID,TransferUserName,TransferOperateDate,ActionState) VALUES(@ID,@ApplierName,@ApplierMUDID,@CreateDate,@ModifyDate,@HTCode,@BUHeadName,@BUHeadMUDID,@IsReAssign,@State,@MMCoEImageOne,@MMCoEImageTwo,@MMCoEImageThree,@FileType,@IsAttentSame,@AttentSameReason,@SpecialReason,@IsAddFile,@IsMeetingInfoSame,@MeetingInfoSameReason,@Memo,@IsTransfer,@TransferOperatorMUDID,@TransferOperatorName,@TransferUserMUDID,@TransferUserName,@TransferOperateDate,'0') ",
                        new SqlParameter[]
                        {
                        SqlParameterFactory.GetSqlParameter("@ID", entity.ID),
                        SqlParameterFactory.GetSqlParameter("@ApplierName", entity.ApplierName),
                        SqlParameterFactory.GetSqlParameter("@ApplierMUDID", entity.ApplierMUDID),
                        SqlParameterFactory.GetSqlParameter("@CreateDate", entity.CreateDate),
                        SqlParameterFactory.GetSqlParameter("@ModifyDate", entity.ModifyDate),
                        SqlParameterFactory.GetSqlParameter("@HTCode", entity.HTCode),
                        SqlParameterFactory.GetSqlParameter("@BUHeadName", entity.BUHeadName),
                        SqlParameterFactory.GetSqlParameter("@BUHeadMUDID", entity.BUHeadMUDID),
                        SqlParameterFactory.GetSqlParameter("@IsReAssign", entity.IsReAssign),
                        SqlParameterFactory.GetSqlParameter("@State", entity.State),
                        SqlParameterFactory.GetSqlParameter("@MMCoEImageOne", entity.MMCoEImageOne),
                        SqlParameterFactory.GetSqlParameter("@MMCoEImageTwo", entity.MMCoEImageTwo),
                        SqlParameterFactory.GetSqlParameter("@MMCoEImageThree", entity.MMCoEImageThree),
                        SqlParameterFactory.GetSqlParameter("@FileType", entity.FileType),
                        SqlParameterFactory.GetSqlParameter("@IsAttentSame", entity.IsAttentSame),
                        SqlParameterFactory.GetSqlParameter("@AttentSameReason", entity.AttentSameReason),
                        SqlParameterFactory.GetSqlParameter("@SpecialReason", entity.SpecialReason),
                        SqlParameterFactory.GetSqlParameter("@IsAddFile", entity.IsAddFile),
                        SqlParameterFactory.GetSqlParameter("@IsMeetingInfoSame", entity.IsMeetingInfoSame),
                        SqlParameterFactory.GetSqlParameter("@MeetingInfoSameReason", entity.MeetingInfoSameReason),
                        SqlParameterFactory.GetSqlParameter("@Memo", entity.Memo),
                        SqlParameterFactory.GetSqlParameter("@IsTransfer", entity.IsTransfer),
                        SqlParameterFactory.GetSqlParameter("@TransferOperatorMUDID", entity.TransferOperatorMUDID),
                        SqlParameterFactory.GetSqlParameter("@TransferOperatorName", entity.TransferOperatorName),
                        SqlParameterFactory.GetSqlParameter("@TransferUserMUDID", entity.TransferUserMUDID),
                        SqlParameterFactory.GetSqlParameter("@TransferUserName", entity.TransferUserName),
                        SqlParameterFactory.GetSqlParameter("@TransferOperateDate", entity.TransferOperateDate),
                        });
                    return res;
                }
            }
        }
        #endregion

        #region 编辑文件上传信息
        /// <summary>
        /// 编辑文件上传信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Update(P_PREUPLOADORDER entity)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery(
                    "UPDATE P_PreUploadOrder SET ModifyDate=@ModifyDate,BUHeadName=@BUHeadName," +
                    "BUHeadMUDID=@BUHeadMUDID,BUHeadApproveDate=@BUHeadApproveDate," +
                    "State=@State,MMCoEImageOne=@MMCoEImageOne," +
                    "MMCoEImageTwo=@MMCoEImageTwo,MMCoEImageThree=@MMCoEImageThree ," +
                    "FileType=@FileType,IsAttentSame=@IsAttentSame,AttentSameReason=@AttentSameReason,SpecialReason=@SpecialReason,IsAddFile=@IsAddFile,IsMeetingInfoSame=@IsMeetingInfoSame,MeetingInfoSameReason=@MeetingInfoSameReason,Memo=@Memo," +
                    "IsTransfer=@IsTransfer,TransferOperatorMUDID=@TransferOperatorMUDID,TransferOperatorName=@TransferOperatorName,TransferUserMUDID=@TransferUserMUDID,TransferUserName=@TransferUserName,TransferOperateDate=@TransferOperateDate,ActionState='0' WHERE ID=@ID ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", entity.ID),
                        SqlParameterFactory.GetSqlParameter("@ModifyDate", entity.ModifyDate),
                        SqlParameterFactory.GetSqlParameter("@BUHeadName", entity.BUHeadName),
                        SqlParameterFactory.GetSqlParameter("@BUHeadMUDID", entity.BUHeadMUDID),
                        SqlParameterFactory.GetSqlParameter("@BUHeadApproveDate", entity.BUHeadApproveDate),
                        SqlParameterFactory.GetSqlParameter("@State", entity.State),
                        SqlParameterFactory.GetSqlParameter("@MMCoEImageOne", entity.MMCoEImageOne),
                        SqlParameterFactory.GetSqlParameter("@MMCoEImageTwo", entity.MMCoEImageTwo),
                        SqlParameterFactory.GetSqlParameter("@MMCoEImageThree", entity.MMCoEImageThree),
                        SqlParameterFactory.GetSqlParameter("@FileType", entity.FileType),
                        SqlParameterFactory.GetSqlParameter("@IsAttentSame", entity.IsAttentSame),
                        SqlParameterFactory.GetSqlParameter("@AttentSameReason", entity.AttentSameReason),
                        SqlParameterFactory.GetSqlParameter("@SpecialReason", entity.SpecialReason),
                        SqlParameterFactory.GetSqlParameter("@IsAddFile", entity.IsAddFile),
                        SqlParameterFactory.GetSqlParameter("@IsMeetingInfoSame", entity.IsMeetingInfoSame),
                        SqlParameterFactory.GetSqlParameter("@MeetingInfoSameReason", entity.MeetingInfoSameReason),
                        SqlParameterFactory.GetSqlParameter("@Memo", entity.Memo),
                        SqlParameterFactory.GetSqlParameter("@IsTransfer", entity.IsTransfer),
                        SqlParameterFactory.GetSqlParameter("@TransferOperatorMUDID", entity.TransferOperatorMUDID),
                        SqlParameterFactory.GetSqlParameter("@TransferOperatorName", entity.TransferOperatorName),
                        SqlParameterFactory.GetSqlParameter("@TransferUserMUDID", entity.TransferUserMUDID),
                        SqlParameterFactory.GetSqlParameter("@TransferUserName", entity.TransferUserName),
                        SqlParameterFactory.GetSqlParameter("@TransferOperateDate", entity.TransferOperateDate),
                    });
            }
        }
        #endregion
        public WP_QYUSER FindApproveInfo(string userId)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnData = sqlServerTemplate.Find<WP_QYUSER>("select pu.* from WP_QYUSER u left join WP_QYUSER pu on u.LineManagerID=pu.ID where u.UserId=@UserId",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", userId)
                    });
                return rtnData;
            }
        }

        #region 获取上传文件详情
        public P_PREUPLOADORDER LoadPreUploadOrder(Guid id)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var result = sqlServerTemplate.Find<P_PREUPLOADORDER>($"SELECT * FROM P_PreUploadOrder where ID=@ID", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", id)});
                return result;
            }
        }
        #endregion

        #region 根据HT编号获取上传文件审批状态
        /// <summary>
        /// 根据HT编号获取上传文件审批状态
        /// </summary>
        /// <param name="htCode"></param>
        /// <returns></returns>
        public bool FindApproveState(string htCode)
        {
            bool result = false;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnData = sqlServerTemplate.Find<P_PREUPLOADORDER>("SELECT * FROM P_PreUploadOrder WHERE HTCode=@HTCode",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@HTCode", htCode)
                    });
                var state = rtnData.State;
                if (state == "2" || state == "3")
                {
                    result = true;
                }
            }
            return result;


        }
        #endregion

        #region ************后台功能*********** 

        #region reopen
        /// <summary>
        /// reopen
        /// </summary>
        /// <param name="id"></param>
        /// <param name="OperatorMUDID"></param>
        /// <param name="OperatorName"></param>
        /// <param name="Reason"></param>
        /// <returns></returns>
        public int UpdateReopen(string id, string operatorMUDID, string operatorName, string reason, string remark, string originatorMUDID, string originatorName)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("UPDATE P_PreUploadOrder SET IsReopen=1, State=3, ModifyDate=@ModifyDate, ReopenOriginatorMUDID=@ReopenOriginatorMUDID, ReopenOriginatorName=@ReopenOriginatorName, ReopenOperatorMUDID=@ReopenOperatorMUDID, ReopenOperatorName=@ReopenOperatorName, "
                    + "ReopenOperateDate=@ReopenOperateDate, ReopenReason=@ReopenReason, ReopenRemark=@ReopenRemark,ActionState='0' WHERE ID IN ('" + id + "') ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", id),
                        SqlParameterFactory.GetSqlParameter("@ModifyDate", DateTime.Now),
                        SqlParameterFactory.GetSqlParameter("@ReopenOriginatorMUDID", originatorMUDID),
                        SqlParameterFactory.GetSqlParameter("@ReopenOriginatorName", originatorName),
                        SqlParameterFactory.GetSqlParameter("@ReopenOperatorMUDID", operatorMUDID),
                        SqlParameterFactory.GetSqlParameter("@ReopenOperatorName", operatorName),
                        SqlParameterFactory.GetSqlParameter("@ReopenReason", reason),
                        SqlParameterFactory.GetSqlParameter("@ReopenRemark", remark),
                        SqlParameterFactory.GetSqlParameter("@ReopenOperateDate", DateTime.Now)
                    });
            }
        }
        #endregion


        #region 上传文件重新分配
        /// <summary>
        /// 上传文件重新分配
        /// </summary>
        /// <param name="id"></param>
        /// <param name="OperatorMUDID"></param>
        /// <param name="OperatorName"></param>
        /// <param name="UserMUDID"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public int UpdateTransfer(Guid id, string OperatorMUDID, string OperatorName, string UserMUDID, string UserName)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("UPDATE P_ORDER SET IsTransfer=1, TransferOperatorMUDID=@TransferOperatorMUDID, TransferOperatorName=@TransferOperatorName, TransferUserMUDID=@TransferUserMUDID, TransferUserName=@TransferUserName, TransferOperateDate=@TransferOperateDate,ActionState='0' WHERE ID=@ID ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", id),
                        SqlParameterFactory.GetSqlParameter("@TransferOperatorMUDID", OperatorMUDID),
                        SqlParameterFactory.GetSqlParameter("@TransferOperatorName", OperatorName),
                        SqlParameterFactory.GetSqlParameter("@TransferUserMUDID", UserMUDID),
                        SqlParameterFactory.GetSqlParameter("@TransferUserName", UserName),
                        SqlParameterFactory.GetSqlParameter("@TransferOperateDate", DateTime.Now)
                    });
            }
        }


        public int UpdateTransfers(string id, string OperatorMUDID, string OperatorName, string UserMUDID, string UserName)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();

                var res = sqlServerTemplate.ExecuteNonQuery("UPDATE P_ORDER SET IsTransfer=1, TransferOperatorMUDID=@TransferOperatorMUDID, TransferOperatorName=@TransferOperatorName, TransferUserMUDID=@TransferUserMUDID, TransferUserName=@TransferUserName, TransferOperateDate=@TransferOperateDate,ActionState='0' WHERE ID in ('" + id + "') ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@TransferOperatorMUDID", OperatorMUDID),
                        SqlParameterFactory.GetSqlParameter("@TransferOperatorName", OperatorName),
                        SqlParameterFactory.GetSqlParameter("@TransferUserMUDID", UserMUDID),
                        SqlParameterFactory.GetSqlParameter("@TransferUserName", UserName),
                        SqlParameterFactory.GetSqlParameter("@TransferOperateDate", DateTime.Now)
                    });
                if (res > 0)
                {
                    return sqlServerTemplate.ExecuteNonQuery("update a SET a.IsTransfer=1, a.TransferOperatorMUDID=@TransferOperatorMUDID, a.TransferOperatorName=@TransferOperatorName, a.TransferUserMUDID=@TransferUserMUDID, a.TransferUserName=@TransferUserName, a.TransferOperateDate=@TransferOperateDate,a.ActionState='0' from P_PreUploadOrder a join P_ORDER b on a.HTCode = b.CN WHERE b.ID in ('" + id + "')",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@TransferOperatorMUDID", OperatorMUDID),
                        SqlParameterFactory.GetSqlParameter("@TransferOperatorName", OperatorName),
                        SqlParameterFactory.GetSqlParameter("@TransferUserMUDID", UserMUDID),
                        SqlParameterFactory.GetSqlParameter("@TransferUserName", UserName),
                        SqlParameterFactory.GetSqlParameter("@TransferOperateDate", DateTime.Now)
                    });
                }
                else
                {
                    return 0;
                }

            }
        }
        #endregion


        public List<P_ORDER> LoadUploadOrder(string userId, string htCode, int rows, int page, out int total)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.LoadPages<P_ORDER>(rows, page, out total,
                    $"SELECT * FROM P_ORDER WHERE (UserId LIKE '%' + @UserId + '%') AND (CN LIKE '%' + @HtCode + '%') AND State NOT IN (5, 11) ",
                    " ORDER BY CreateDate DESC", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", userId),
                        SqlParameterFactory.GetSqlParameter("@HtCode", htCode)
                    });
                return list;
            }
        }

        #endregion

        #region 导入REOPEN数据
        /// <summary>
        /// 导入REOPEN数据
        /// </summary>
        /// <param name="list"></param>
        /// <param name="fails"></param>
        /// <returns></returns>
        public int Import(List<P_REOPEN_VIEW> list, ref List<P_REOPEN_VIEW> fails)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection() as SqlConnection)
            {
                conn.Open();

                var tran = conn.BeginTransaction();
                var reopenTime = DateTime.Now;
                foreach (var item in list)
                {
                    SqlCommand commandAdd = new SqlCommand(
                        "UPDATE P_PreUploadOrder SET IsReopen=1, State=3, ReopenOriginatorMUDID=@ReopenOriginatorMUDID, ReopenOriginatorName=@ReopenOriginatorName, ReopenOperatorMUDID=@ReopenOperatorMUDID, ReopenOperatorName=@ReopenOperatorName, ReopenOperateDate=@ReopenOperateDate, ReopenReason=@ReopenReason, ReopenRemark=@ReopenRemark,ModifyDate=@ReopenTime,ActionState='0' WHERE HTCode=@HTCode",
                        conn);
                    commandAdd.Transaction = tran;
                    commandAdd.Parameters.AddRange(
                        new SqlParameter[]
                        {
                            SqlParameterFactory.GetSqlParameter("@ReopenOriginatorMUDID", item.OriginatorMUDID),
                            SqlParameterFactory.GetSqlParameter("@ReopenOriginatorName", item.OriginatorName),
                            SqlParameterFactory.GetSqlParameter("@ReopenOperatorMUDID", item.CurrentUserId),
                            SqlParameterFactory.GetSqlParameter("@ReopenOperatorName", item.CurrentUserName),
                            SqlParameterFactory.GetSqlParameter("@ReopenOperateDate", DateTime.Now),
                            SqlParameterFactory.GetSqlParameter("@ReopenReason", item.ReopenReason),
                            SqlParameterFactory.GetSqlParameter("@ReopenRemark", item.ReopenRemark),
                            SqlParameterFactory.GetSqlParameter("@HTCode", item.HTCode),
                            SqlParameterFactory.GetSqlParameter("@ReopenTime", reopenTime)
                        }
                    );
                    commandAdd.ExecuteNonQuery();
                }

                tran.Commit();
                return 1;
            }
        }
        #endregion

        public List<P_PREUPLOADORDER> GetUploadOrderByUserId(string UserId)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnData = sqlServerTemplate.Load<P_PREUPLOADORDER>("SELECT * from [P_PreUploadOrder] Where ApplierMudid=@UserId and state='1' and ([IsReAssign]=0 or  ([IsReAssign]=1 and ReAssignOperatorMUDID=N'系统自动'))",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", UserId)
                    });
                return rtnData;
            }
        }
        public List<V_UnFinishOrder> LoadUnFinishOrder(string CN, string MUDID, string StartDate, string EndDate, int page, int rows, out int total)
        {
            #region sql
            var sql = @"SELECT * FROM (SELECT [申请人姓名] as c1
      ,[申请人MUDID] as c2
      ,[申请人职位]as c3
      ,[申请人手机号码] as c4
      ,[HT编号] as c5
      ,[Market] as c6
      ,[TA] as c7
      ,[省份] as c8
      ,[城市] as c9
      ,[医院编码] as c10
      ,[医院名称] as c11
      ,[成本中心] as c12
      ,[供应商] as c13
      ,[送餐日期] as c14
      ,[送餐时间] as c15
      ,[餐厅编码] as c16
      ,[餐厅名称] as c17
      ,[用餐人数] as c18
      ,[实际金额] as c19
      ,[是否申请退单] as c20
      ,[是否退单成功] as c21
      ,[退单失败平台发起配送需求] as c22
      ,[退单失败线下反馈配送需求] as c23
      ,[预订/退单失败原因] as c24
      ,[是否收餐/未送达] as c25
      ,[用户确认金额] as c26
      ,[是否与预定餐品一致] as c27
      ,[用户确认金额调整原因] as c28
      ,[用户确认金额调整备注] as c29
      ,[实际用餐人数] as c30
      ,[实际用餐人数调整原因] as c31
      ,[实际用餐人数调整备注] as c32
      ,[直线经理姓名] as c33
      ,[直线经理MUDID] as c34
      ,[Level2 Manager 姓名] as c35
      ,[Level2 Manager MUDID] as c36
      ,[Level3 Manager 姓名] as c37
      ,[Level3 Manager MUDID] as c38
      ,[是否上传文件] as c39
      ,[上传文件提交日期] as c40
      ,[上传文件提交时间] as c41
      ,[上传文件审批直线经理姓名] as c42
      ,[上传文件审批直线经理MUDID] as c43
      ,[上传文件审批日期] as c44
      ,[上传文件审批时间] as c45
      ,[上传文件审批状态] as c46
      ,[签到人数是否等于实际用餐人数] as c47
      ,[签到人数调整原因] as c48
      ,[是否与会议信息一致] as c49
      ,[会议信息不一致原因] as c50
      ,[上传文件是否Re-Open] as c51
      ,[上传文件Re-Open操作人] as c52
      ,[上传文件Re-Open操作人MUDID] as c53
      ,[上传文件Re-Open日期] as c54
      ,[上传文件Re-Open时间] as c55
      ,[上传文件Re-Open发起人姓名] as c56
      ,[上传文件Re-Open发起人MUDID] as c57
      ,[上传文件Re-Open原因] as c58
      ,[上传文件Re-Open备注] as c59
      ,[上传文件Re-Open审批状态] as c60
      ,[上传文件是否重新分配] as c61
      ,[上传文件重新分配操作人] as c62
      ,[上传文件重新分配操作人MUDID] as c63
      ,[上传文件被重新分配人姓名] as c64
      ,[上传文件被重新分配人MUDID] as c65
      ,[上传文件被重新分配日期] as c66
      ,[上传文件被重新分配时间] as c67
      ,[上传文件是否重新分配审批人] as c68
      ,[上传文件重新分配审批人-操作人] as c69
      ,[上传文件重新分配审批人-操作人MUDID] as c70
      ,[上传文件被重新分配审批人姓名] as c71
      ,[上传文件被重新分配审批人MUDID] as c72
      ,[上传文件重新分配审批人日期] as c73
      ,[上传文件重新分配审批人时间] as c74
      ,[项目组特殊备注] as c75
      ,[Workday是否离职] as c76
  FROM [V_UnFinishOrderView]) b where 1=1 ";
            #endregion
            var _sql = " ";
            List<V_UnFinishOrder> rtnData = null;
            var listParams = new List<SqlParameter>();
            //var endDate = DateTime.Now.AddMonths(-1).ToString("yyyy-MM")+"-01";
            if (!string.IsNullOrEmpty(CN))
            {
                _sql += " and b.C5 like '%" + CN + "%'";
            }
            if (!string.IsNullOrEmpty(MUDID))
            {
                _sql += " and b.C2 like '%" + MUDID + "%'";
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                _sql += " and b.C14 >= @StartDate";
                listParams.Add(new SqlParameter("@StartDate", StartDate));
            }

            if (!string.IsNullOrEmpty(EndDate))
            {
                _sql += " and b.C14 <= @EndDate";
                listParams.Add(new SqlParameter("@EndDate", EndDate));
            }

            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.LoadPages<V_UnFinishOrder>(rows, page, out total,
                    sql + _sql, " ORDER BY b.C14 DESC, b.C15 DESC ", listParams.ToArray());
            }
            return rtnData;
        }
        public List<V_UnFinishOrder> LoadUnFinishOrderForMessage(string CN, string MUDID, string StartDate, string EndDate)
        {
            #region sql
            var sql = @"SELECT * FROM (SELECT [申请人姓名] as c1
      ,[申请人MUDID] as c2
      ,[申请人职位]as c3
      ,[申请人手机号码] as c4
      ,[HT编号] as c5
      ,[Market] as c6
      ,[TA] as c7
      ,[省份] as c8
      ,[城市] as c9
      ,[医院编码] as c10
      ,[医院名称] as c11
      ,[成本中心] as c12
      ,[供应商] as c13
      ,[送餐日期] as c14
      ,[送餐时间] as c15
      ,[餐厅编码] as c16
      ,[餐厅名称] as c17
      ,[用餐人数] as c18
      ,[实际金额] as c19
      ,[是否申请退单] as c20
      ,[是否退单成功] as c21
      ,[退单失败平台发起配送需求] as c22
      ,[退单失败线下反馈配送需求] as c23
      ,[预订/退单失败原因] as c24
      ,[是否收餐/未送达] as c25
      ,[用户确认金额] as c26
      ,[是否与预定餐品一致] as c27
      ,[用户确认金额调整原因] as c28
      ,[用户确认金额调整备注] as c29
      ,[实际用餐人数] as c30
      ,[实际用餐人数调整原因] as c31
      ,[实际用餐人数调整备注] as c32
      ,[直线经理姓名] as c33
      ,[直线经理MUDID] as c34
      ,[Level2 Manager 姓名] as c35
      ,[Level2 Manager MUDID] as c36
      ,[Level3 Manager 姓名] as c37
      ,[Level3 Manager MUDID] as c38
      ,[是否上传文件] as c39
      ,[上传文件提交日期] as c40
      ,[上传文件提交时间] as c41
      ,[上传文件审批直线经理姓名] as c42
      ,[上传文件审批直线经理MUDID] as c43
      ,[上传文件审批日期] as c44
      ,[上传文件审批时间] as c45
      ,[上传文件审批状态] as c46
      ,[签到人数是否等于实际用餐人数] as c47
      ,[签到人数调整原因] as c48
      ,[是否与会议信息一致] as c49
      ,[会议信息不一致原因] as c50
      ,[上传文件是否Re-Open] as c51
      ,[上传文件Re-Open操作人] as c52
      ,[上传文件Re-Open操作人MUDID] as c53
      ,[上传文件Re-Open日期] as c54
      ,[上传文件Re-Open时间] as c55
      ,[上传文件Re-Open发起人姓名] as c56
      ,[上传文件Re-Open发起人MUDID] as c57
      ,[上传文件Re-Open原因] as c58
      ,[上传文件Re-Open备注] as c59
      ,[上传文件Re-Open审批状态] as c60
      ,[上传文件是否重新分配] as c61
      ,[上传文件重新分配操作人] as c62
      ,[上传文件重新分配操作人MUDID] as c63
      ,[上传文件被重新分配人姓名] as c64
      ,[上传文件被重新分配人MUDID] as c65
      ,[上传文件被重新分配日期] as c66
      ,[上传文件被重新分配时间] as c67
      ,[上传文件是否重新分配审批人] as c68
      ,[上传文件重新分配审批人-操作人] as c69
      ,[上传文件重新分配审批人-操作人MUDID] as c70
      ,[上传文件被重新分配审批人姓名] as c71
      ,[上传文件被重新分配审批人MUDID] as c72
      ,[上传文件重新分配审批人日期] as c73
      ,[上传文件重新分配审批人时间] as c74
      ,[项目组特殊备注] as c75
      ,[Workday是否离职] as c76
  FROM [V_UnFinishOrderView]) b where 1=1 ";
            #endregion
            var _sql = " ";
            List<V_UnFinishOrder> rtnData = null;
            var listParams = new List<SqlParameter>();
            //var endDate = DateTime.Now.AddMonths(-1).ToString("yyyy-MM") + "-01";
            if (!string.IsNullOrEmpty(CN))
            {
                _sql += " and b.C5 like '%" + CN + "%'";
            }
            if (!string.IsNullOrEmpty(MUDID))
            {
                _sql += " and b.C2 like '%" + MUDID + "%'";
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                _sql += " and b.C14 > @StartDate";
                listParams.Add(new SqlParameter("@StartDate", StartDate));
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                _sql += " and b.C14 <= @EndDate";
                listParams.Add(new SqlParameter("@EndDate", EndDate));
            }
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<V_UnFinishOrder>(sql + _sql, listParams.ToArray());
            }
            return rtnData;
        }


        public List<P_PREUPLOADORDER> LoadHTCode()
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnData = sqlServerTemplate.Load<P_PREUPLOADORDER>("SELECT * from [P_PreUploadOrder]  ",
                    new SqlParameter[]
                    {                        
                    });
                return rtnData;
            }
        }
        public List<P_PREUPLOADORDER> LoadDataByInHTCode(string subHTCode)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnData = sqlServerTemplate.Load<P_PREUPLOADORDER>("SELECT * from [P_PreUploadOrder] where HTCode  in("+ subHTCode + ") ",
                    new SqlParameter[]
                    {
                    });
                return rtnData;
            }
        }


        #region 同步上传文件表
        public int SyncPreUploadOrder()
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnCount = sqlServerTemplate.ExecuteNonQuery(@"DELETE FROM P_PreUploadOrder_COST WHERE ID IN (SELECT ID FROM [P_PreUploadOrder] WHERE ActionState = 0);",
                    new SqlParameter[]
                    {

                    });

                var rtnData = sqlServerTemplate.ExecuteNonQuery(@"INSERT INTO P_PreUploadOrder_COST SELECT [ID],[ApplierName],[ApplierMUDID],[CreateDate],[ModifyDate],[HTCode],[BUHeadName],[BUHeadMUDID],[BUHeadApproveDate],[IsReAssign],[ReAssignBUHeadName],[ReAssignBUHeadMUDID],[ReAssignBUHeadApproveDate]
                                                                  ,[State],[MMCoEImageOne],[MMCoEImageTwo],[MMCoEImageThree],[FileType],[IsAttentSame],[AttentSameReason],[SpecialReason],[IsAddFile],[IsReopen],[ReopenOriginatorMUDID],[ReopenOriginatorName],[ReopenOperatorMUDID]
                                                                  ,[ReopenOperatorName],[ReopenOperateDate],[ReopenReason],[ReopenRemark],[IsTransfer],[TransferOperatorMUDID],[TransferOperatorName],[TransferUserMUDID],[TransferUserName],[TransferOperateDate],[ReAssignOperatorMUDID]
                                                                  ,[ReAssignOperatorName],[UploadReOpenState],[IsMeetingInfoSame],[MeetingInfoSameReason],[Memo],'1' 
                                                                  FROM [P_PreUploadOrder] WHERE ActionState = '0';",
                    new SqlParameter[]
                    {

                    });

                if(rtnData > 0)
                {
                    sqlServerTemplate.ExecuteNonQuery(@"UPDATE P_PreUploadOrder SET ActionState = 1 WHERE ActionState = 0);",
                    new SqlParameter[]
                    {

                    });
                }
                return rtnData;
            }
        }
        #endregion

    }
}
