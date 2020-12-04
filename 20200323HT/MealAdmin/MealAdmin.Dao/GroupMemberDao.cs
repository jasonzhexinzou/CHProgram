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
    public class GroupMemberDao : IGroupMemberDao
    {
        [Bean("sqlServerTemplFactory")]
        public DbTemplateFactory sqlServerTemplFactory { get; set; }
        [Bean("sqlServerTemplFactoryNonHT")]
        public DbTemplateFactory sqlServerTemplFactoryNonHT { get; set; }
        /*
                private int Add(P_GROUP_MEMBER entity)
                {
                    var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                    using (var conn = sqlServerTemplate.GetSqlConnection())
                    {
                        conn.Open();
                        using (var trans = conn.BeginTransaction())
                        {

                        }
                        return sqlServerTemplate.ExecuteNonQuery("INSERT INTO P_GROUP_MEMBER(ID, GroupType, UserId, UserName, CreateDate, CreateUserId) VALUES(@ID,@GroupType,@UserId,@UserName,@CreateDate,@CreateUserId) ",
                            new SqlParameter[]
                            {
                                SqlParameterFactory.GetSqlParameter("@ID", entity.ID),
                                SqlParameterFactory.GetSqlParameter("@GroupType", entity.GroupType),
                                SqlParameterFactory.GetSqlParameter("@UserId", entity.UserId),
                                SqlParameterFactory.GetSqlParameter("@UserName", entity.UserName),
                                SqlParameterFactory.GetSqlParameter("@CreateDate", entity.CreateDate),
                                SqlParameterFactory.GetSqlParameter("@CreateUserId", entity.CreateUserId)
                            });
                    }
                }
                */
        public int Add(List<string> userIds, int groupType, DateTime createDate, string createUserId, int state, out List<string> unSuccesUserId)
        {
            unSuccesUserId = new List<string>();
            int upCnt = 0;
            if (userIds.Count > 0)
            {
                var sqlInsert = "INSERT INTO P_GROUP_MEMBER(ID, GroupType, UserId, UserName, CreateDate, CreateUserId, State) SELECT @ID, @GroupType, UserId, [Name], @CreateDate, @CreateUserId, @State FROM WP_QYUSER WHERE UserId = @UserId ";
                var sqlSelectAll = "SELECT UserId FROM P_GROUP_MEMBER WHERE (GroupType = @GroupType)";
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    //using (var trans = conn.BeginTransaction())
                    //{
                    var allMembers = sqlServerTemplate.Load<P_GROUP_MEMBER>(sqlSelectAll, new SqlParameter[] { SqlParameterFactory.GetSqlParameter("@GroupType", groupType) }).Select(i => i.UserId.ToLower());
                    foreach (var _id in userIds)
                    {
                        //已存储该用户
                        if (allMembers.Contains(_id.ToLower()) == true)
                        {
                            upCnt++;
                        }
                        else
                        {
                            // 成功从人员表匹配插入到组内 
                            if (sqlServerTemplate.ExecuteNonQuery(sqlInsert, new SqlParameter[] {
                                        SqlParameterFactory.GetSqlParameter("@ID", Guid.NewGuid()),
                                        SqlParameterFactory.GetSqlParameter("@GroupType", groupType),
                                        SqlParameterFactory.GetSqlParameter("@CreateDate", createDate),
                                        SqlParameterFactory.GetSqlParameter("@CreateUserId", createUserId),
                                        SqlParameterFactory.GetSqlParameter("@UserId", _id),
                                        SqlParameterFactory.GetSqlParameter("@State",1)
                                        }) == 1)
                            {
                                upCnt++;
                            }
                            // 未匹配到人员表
                            else
                            {
                                unSuccesUserId.Add(_id);
                            }
                        }
                    }
                    //trans.Commit();
                    //}
                }
            }
            return upCnt;
        }

        public int AddNonHT(List<string> userIds, int groupType, DateTime createDate, string createUserId, int state, out List<string> unSuccesUserId)
        {
            unSuccesUserId = new List<string>();
            int upCnt = 0;
            if (userIds.Count > 0)
            {
                var sqlInsert = "INSERT INTO P_GROUP_MEMBER(ID, GroupType, UserId, UserName, CreateDate, CreateUserId) SELECT @ID, @GroupType, UserId, [Name], @CreateDate, @CreateUserId FROM WP_QYUSER WHERE UserId = @UserId ";
                var sqlSelectAll = "SELECT UserId FROM P_GROUP_MEMBER WHERE (GroupType = @GroupType)";
                var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    //using (var trans = conn.BeginTransaction())
                    //{
                    var allMembers = sqlServerTemplate.Load<P_GROUP_MEMBER>(sqlSelectAll, new SqlParameter[] { SqlParameterFactory.GetSqlParameter("@GroupType", groupType) }).Select(i => i.UserId.ToLower());
                    foreach (var _id in userIds)
                    {
                        //已存储该用户
                        if (allMembers.Contains(_id.ToLower()) == true)
                        {
                            upCnt++;
                        }
                        else
                        {
                            // 成功从人员表匹配插入到组内 
                            if (sqlServerTemplate.ExecuteNonQuery(sqlInsert, new SqlParameter[] {
                                        SqlParameterFactory.GetSqlParameter("@ID", Guid.NewGuid()),
                                        SqlParameterFactory.GetSqlParameter("@GroupType", groupType),
                                        SqlParameterFactory.GetSqlParameter("@CreateDate", createDate),
                                        SqlParameterFactory.GetSqlParameter("@CreateUserId", createUserId),
                                        SqlParameterFactory.GetSqlParameter("@UserId", _id)
                                        }) == 1)
                            {
                                upCnt++;
                            }
                            // 未匹配到人员表
                            else
                            {
                                unSuccesUserId.Add(_id);
                            }
                        }
                    }
                    //trans.Commit();
                    //}
                }
            }
            return upCnt;
        }
        public int DelNonHTByGroupType(int GroupType)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("DELETE FROM P_GROUP_MEMBER WHERE (GroupType = @GroupType) ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@GroupType", GroupType)
                    });
            }
        }
        public int DelByGroupType(int GroupType)
        {

            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("DELETE FROM P_GROUP_MEMBER WHERE (GroupType = @GroupType) ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@GroupType", GroupType)
                    });
            }
        }

        public int DelByMemberID(Guid MemberID)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("DELETE FROM P_GROUP_MEMBER WHERE (ID = @ID) ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", MemberID)
                    });
            }
        }

        public int DelNonHTByMemberID(Guid MemberID)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("DELETE FROM P_GROUP_MEMBER WHERE (ID = @ID) ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", MemberID)
                    });
            }
        }

        public List<P_GROUP_MEMBER> Load(int groupType, string srh_userId, string srh_userName)
        {

            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<P_GROUP_MEMBER>(
                    "SELECT ID, GroupType, UserId, UserName, CreateDate, CreateUserId FROM P_GROUP_MEMBER WHERE (GroupType = @GroupType) AND (UserId LIKE '%@UserId%') AND (UserName LIKE '%@UserName%') ORDER BY CreateDate DESC ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@GroupType", groupType),
                        SqlParameterFactory.GetSqlParameter("@UserId", srh_userId),
                        SqlParameterFactory.GetSqlParameter("@UserName", srh_userName)
                    });
                return list;
            }
        }

        public List<P_GROUP_MEMBER> LoadPage(int groupType, string srh_userId, string srh_userName, int rows, int page, out int total)
        {

            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                //var list = sqlServerTemplate.LoadPages<P_GROUP_MEMBER>(rows, page, out total,
                //    "SELECT ID, GroupType, UserId, UserName, CreateDate, CreateUserId,State1,State2,State3 FROM P_GROUP_MEMBER WHERE (GroupType = @GroupType) AND (UserId LIKE '%' + @UserId + '%') AND (UserName LIKE '%' + @UserName + '%') AND (UserName LIKE '%' + @UserName + '%') ",
                //    " ORDER BY CreateDate DESC ", new SqlParameter[]
                var list = sqlServerTemplate.LoadPages<P_GROUP_MEMBER>(rows, page, out total,
                    "SELECT G.*,Q.State AS Qstate FROM [dbo].[P_GROUP_MEMBER] G LEFT JOIN dbo.WP_QYUSER Q ON G.UserId = Q.UserId  WHERE (G.GroupType = @GroupType) AND (G.UserId LIKE '%' + @UserId + '%') AND (G.UserName LIKE '%' + @UserName + '%') AND (Q.State=1)",
                    " ORDER BY G.CreateDate DESC ", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@GroupType", groupType),
                        SqlParameterFactory.GetSqlParameter("@UserId", srh_userId),
                        SqlParameterFactory.GetSqlParameter("@UserName", srh_userName)
                    });
                return list;
            }
        }

        public List<P_GROUP_MEMBER> LoadNonHTPage(int groupType, string srh_userId, string srh_userName, int rows, int page, out int total)
        {

            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.LoadPages<P_GROUP_MEMBER>(rows, page, out total,
                    "SELECT ID, GroupType, UserId, UserName, CreateDate, CreateUserId FROM P_GROUP_MEMBER WHERE (GroupType = @GroupType) AND (UserId LIKE '%' + @UserId + '%') AND (UserName LIKE '%' + @UserName + '%') ",
                    " ORDER BY CreateDate DESC ", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@GroupType", groupType),
                        SqlParameterFactory.GetSqlParameter("@UserId", srh_userId),
                        SqlParameterFactory.GetSqlParameter("@UserName", srh_userName)
                    });
                return list;
            }
        }


        public List<P_GROUP_MEMBER> GetGroupMembersByType(GroupTypeEnum GroupType)
        {
            List<P_GROUP_MEMBER> rtnData;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<P_GROUP_MEMBER>(
                    "SELECT ID, GroupType, UserId, UserName, CreateDate, CreateUserId FROM P_GROUP_MEMBER WHERE (GroupType = @GroupType) ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@GroupType", GroupType.ToString("D"))
                    });
            }
            return rtnData;
        }

        #region 导出组别管理
        public List<P_GROUP_MEMBER> ExportGroupList(string MUDID, string Name, int GroupType)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<P_GROUP_MEMBER>(
                    "SELECT G.*,Q.State AS Qstate FROM [dbo].[P_GROUP_MEMBER] G LEFT JOIN dbo.WP_QYUSER Q ON G.UserId = Q.UserId  WHERE (G.GroupType = @GroupType) AND (G.UserId LIKE '%' + @MUDID + '%') AND (G.UserName LIKE '%' + @Name + '%') AND (Q.State=1) ORDER BY G.CreateDate DESC  ", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@MUDID", MUDID),
                        SqlParameterFactory.GetSqlParameter("@Name", Name),
                        SqlParameterFactory.GetSqlParameter("@GroupType", GroupType)
                    });
                return list;
            }
        }
        public List<P_GROUP_MEMBER> ExportNonHTGroupList(string MUDID, string Name, int GroupType)
        {


            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<P_GROUP_MEMBER>(
                    "SELECT G.*,Q.State AS Qstate FROM [dbo].[P_GROUP_MEMBER] G LEFT JOIN dbo.WP_QYUSER Q ON G.UserId = Q.UserId  WHERE (G.GroupType = @GroupType) AND (G.UserId LIKE '%' + @MUDID + '%') AND (G.UserName LIKE '%' + @Name + '%') AND (Q.State=1) ORDER BY G.CreateDate DESC  ", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@MUDID", MUDID),
                        SqlParameterFactory.GetSqlParameter("@Name", Name),
                        SqlParameterFactory.GetSqlParameter("@GroupType", GroupType)
                    });
                return list;
            }

        }
        public List<P_ServPause_Detail> ExportServPauseGroupList(string MUDID, string Name, int GroupType)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<P_ServPause_Detail>(
                    "SELECT   pre.ApplierName AS ApplierName "
                    + ", pre.ApplierMUDID AS ApplierMUDID "
                    + ", qyu.Position as Position "
                    + ", pre.ApplierMobile AS ApplierMobile "
                    + ", pre.HTCode AS HTCode "
                    + ", pre.Market "
                    + ", pre.TA "
                    + ", CONVERT(DATE, ord.DeliverTime, 23) AS DeliverDate "
                    + ", CONVERT(TIME(0), ord.DeliverTime, 8) AS DeliverTime "
                    + ", CASE WHEN ord.ReceiveState = 6 THEN N'是' WHEN ord.ReceiveState = 7 THEN N'自动' WHEN ord.ReceiveState = 8 THEN "
                    + "N'未送达' else N'否' END AS ReceiveState "
                    + ", CASE WHEN ord.State = 8 THEN N'' ELSE CONVERT(DATE, ord.ReceiveDate, 23) END AS ReceiveDate "
                    + ", CASE WHEN ord.State = 8 THEN null ELSE CONVERT(TIME(0), ord.ReceiveDate, 8) END AS ReceiveTime "
                    + ", dbo.GetDMName(pre.ApplierMUDID) AS DMName "
                    + ", dbo.GetDMUserID(pre.ApplierMUDID) AS DMMUDID "
                    + ", dbo.GetDMName(dbo.GetDMUserID(pre.ApplierMUDID)) AS RMName "
                    + ", dbo.GetDMUserID(dbo.GetDMUserID(pre.ApplierMUDID)) AS RMMUDID "
                    + ", dbo.GetDMName(dbo.GetDMUserID(dbo.GetDMUserID(pre.ApplierMUDID))) AS RDName "
                    + ", dbo.GetDMUserID(dbo.GetDMUserID(dbo.GetDMUserID(pre.ApplierMUDID))) AS RDMUDID "
                    + ", CASE WHEN ord.IsOrderUpload = 1 then N'是' ELSE N'否' END AS IsOrderUpload "
                    + ", CASE WHEN ord.IsOrderUpload = 1 THEN CONVERT(DATE, puo.CreateDate, 23) ELSE null END AS OrderUploadDate "
                    + ", CASE WHEN ord.IsOrderUpload = 1 THEN CONVERT(TIME(0), puo.CreateDate, 8) ELSE null END AS OrderUploadTime "
                    + ", puo.BUHeadName AS BUHeadName "
                    + ", puo.BUHeadMUDID AS BUHeadMUDID "
                    + ", CONVERT(DATE, puo.BUHeadApproveDate, 23) AS BUHeadApproveDate "
                    + ", CONVERT(TIME(0), puo.BUHeadApproveDate, 8) AS BUHeadApproveTime "
                    + ", CASE puo.State WHEN 0 THEN null WHEN 1 THEN N'上传文件提交成功' WHEN 2 THEN N'上传文件审批驳回' WHEN 3 THEN "
                    + "N'上传文件审批驳回' WHEN 4 THEN N'上传文件审批通过' END AS OrderUploadState "
                    + ", CASE WHEN puo.IsReopen = 1 THEN N'是' when ord.IsOrderUpload = 0 then null ELSE N'否' END AS IsReopen "
                    + ", CONVERT(DATE, puo.ReopenOperateDate, 23) AS ReopenOperateDate  "
                    + ", CONVERT(TIME(0), puo.ReopenOperateDate, 8) AS ReopenOperateTime "
                    + ", CASE WHEN puo.IsReopen = 0 THEN null WHEN puo.State = 1 THEN N'上传文件Re-Open提交成功' WHEN puo.State = 2 THEN "
                    + "N'上传文件Re-Open审批驳回' WHEN puo.State = 3 THEN N'上传文件Re-Open审批驳回' WHEN puo.State = 4 THEN N'上传文件Re-Open审批通过'"
                    + " END AS ReopenOrderUploadState "
                    + ",case when sp.ServPauseType = 1 then N'未确认收餐' when sp.ServPauseType = 2 then N'确认收餐后未上传文件' when sp.ServPauseType = 3 then N'上传文件未审批' when sp.ServPauseType = 4 then N'Reopen后未重新上传' end as ServPauseType "
                    + ", CONVERT(DATE, sp.CreateDate, 23) AS ServPauseCreateDate"
                    + ", CONVERT(TIME(0), sp.CreateDate, 8) AS ServPauseCreateTime "
                    + ", CONVERT(DATE, sp.ModifyDate, 23) AS ServPauseModifyDate"
                    + ", CONVERT(TIME(0), sp.ModifyDate, 8) AS ServPauseModifyTime "
                    + ", case when sp.state=1 then N'已开通服务' else N'未开通服务' end as  ServPauseState "
                + "FROM dbo.P_ORDER AS ord WITH(NOLOCK) LEFT OUTER JOIN "
                    + "dbo.WP_QYUSER AS qyu ON ord.UserId = qyu.UserId LEFT OUTER JOIN "
                    + "dbo.P_USERINFO AS inf ON ord.UserId = inf.UserId LEFT OUTER JOIN "
                    + "dbo.P_ORDER_XMS_REPORT AS rep ON ord.XmsOrderId = rep.XmsOrderId LEFT OUTER JOIN "
                    + "dbo.P_EVALUATE AS eva ON ord.ID = eva.OrderID LEFT OUTER JOIN "
                    + "dbo.P_PreApproval AS pre ON ord.CN = pre.HTCode and ord.UserId = pre.ApplierMUDID LEFT OUTER JOIN "
                    + "dbo.P_PreUploadOrder AS puo ON ord.CN = puo.HTCode and ord.UserId = puo.ApplierMUDID AND ord.IsOrderUpload = 1 inner join "
                    + "dbo.P_ServPause_Detail as sp on ord.CN = sp.htcode and sp.State=0 and ord.[State]<>5 and ord.[State]<>11 and sp.State = 0"
                + "WHERE(ord.XmsOrderId IS NOT NULL) AND(ord.XmsOrderId <> '') AND (ord.IsNonHT = 0) and substring(ord.CN, 1, 1) = 'H' and (@UserId='' or pre.ApplierMUDID=@UserId) and (@UserName='' or pre.ApplierName= @UserName)",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", MUDID),
                        SqlParameterFactory.GetSqlParameter("@UserName", Name),
                    });
                return list;
            }
        }

        #endregion


        #region 查找用户所在的组
        /// <summary>
        /// 查找用户所在的组
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<P_GROUP_MEMBER> LoadUserGroup(string userId)
        {
            List<P_GROUP_MEMBER> rtnData;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<P_GROUP_MEMBER>(
                    "SELECT * FROM P_GROUP_MEMBER WHERE (UserId = @UserId) ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", userId)
                    });
            }
            return rtnData;
        }
        #endregion


        #region 查找用户所在的组
        /// <summary>
        /// 查找用户所在的组
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsDevGroup(string userId)
        {
            List<B_ADMINUSER> rtnData;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<B_ADMINUSER>(
                    "SELECT * FROM V_ADMINUSER WHERE EMAIL = @USERID",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@USERID", userId)
                    });
            }
            if(rtnData.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
        #endregion

        public int AddServPauseDetail(string UserId, string UserName, string HTCode, int ServPauseType, string Memo)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();

                var res = sqlServerTemplate.Find<P_ServPauser>("SELECT * FROM P_ServPause_Detail WHERE HTCode = @HTCode AND UserId=@UserId AND ServPauseType=@ServPauseType ", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", UserId),
                        SqlParameterFactory.GetSqlParameter("@HTCode", HTCode),
                        SqlParameterFactory.GetSqlParameter("@ServPauseType", ServPauseType)
                    });

                if (res != null)
                {
                    return 1;
                }

                return sqlServerTemplate.ExecuteNonQuery("INSERT INTO P_ServPause_Detail(Id,UserId,UserName,HTCode,ServPauseType,CreateDate,Memo,State) Values(@Id,@UserId,@UserName,@HTCode,@ServPauseType,@CreateDate,@Memo,@State)",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", Guid.NewGuid()),
                        SqlParameterFactory.GetSqlParameter("@UserId", UserId),
                        SqlParameterFactory.GetSqlParameter("@UserName", UserName),
                        SqlParameterFactory.GetSqlParameter("@HTCode", HTCode),
                        SqlParameterFactory.GetSqlParameter("@ServPauseType", ServPauseType),
                        SqlParameterFactory.GetSqlParameter("@CreateDate", DateTime.Now),
                        SqlParameterFactory.GetSqlParameter("@Memo", Memo),
                        SqlParameterFactory.GetSqlParameter("@State", 0)
                    });
            }
        }


        public int UpdateServPauseDetail(string HTCode, int ServPauseType)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("UPDATE P_ServPause_Detail set state=1,ModifyDate=@ModifyDate where ServPauseType = @ServPauseType AND HTCode=@HTCode",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@HTCode", HTCode),
                        SqlParameterFactory.GetSqlParameter("@ServPauseType", ServPauseType),
                        SqlParameterFactory.GetSqlParameter("@ModifyDate", DateTime.Now),
                    });
            }
        }


        #region ******************消息推送相关*****************

        #region 解除暂停服务状态(未收餐)
        /// <summary>
        /// 解除暂停服务状态
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public int DelByState1(string userId)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("DELETE FROM P_GROUP_MEMBER WHERE State1=1 AND UserId=@UserId AND GroupType = 4 ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", userId)
                    });
            }
        }
        #endregion

        #region 解除暂停服务状态(未上传文件)
        /// <summary>
        /// 解除暂停服务状态
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public int DelByState2(string userId)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("DELETE FROM P_GROUP_MEMBER WHERE State2=1 AND UserId=@UserId AND GroupType = 4 ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", userId)
                    });
            }
        }
        #endregion

        #region 解除暂停服务状态(未完成审批)
        /// <summary>
        /// 解除暂停服务状态
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public int DelByState3(string userId)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("DELETE FROM P_GROUP_MEMBER WHERE State3=1 AND UserId=@UserId AND GroupType = 4 ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", userId)
                    });
            }
        }
        #endregion

        #region 查询当前用户是否被暂停服务
        /// <summary>
        /// 查询当前用户是否被暂停服务
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public P_GROUP_MEMBER FindByUser(string userId)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Find<P_GROUP_MEMBER>(
                    "SELECT * FROM P_GROUP_MEMBER WHERE (UserId = @UserId) AND GroupType = 4 ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", userId)
                    });
            }
        }
        #endregion

        #region 更新暂停服务状态(未收餐)
        /// <summary>
        /// 更新暂停服务状态
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public int UpdateState1(string userId)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("UPDATE P_GROUP_MEMBER SET State1=0 WHERE UserId=@UserId AND GroupType = 4 ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", userId)
                    });
            }
        }
        #endregion

        #region 更新暂停服务状态(未上传文件)
        /// <summary>
        /// 更新暂停服务状态
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public int UpdateState2(string userId)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("UPDATE P_GROUP_MEMBER SET State2=0 WHERE UserId=@UserId AND GroupType = 4 ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", userId)
                    });
            }
        }
        #endregion

        #region 更新暂停服务状态(未完成审批)
        /// <summary>
        /// 更新暂停服务状态
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public int UpdateState3(string userId)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("UPDATE P_GROUP_MEMBER SET State3=0 WHERE UserId=@UserId AND GroupType = 4 ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", userId)
                    });
            }
        }
        #endregion

        #region 添加暂停服务人员
        /// <summary>
        /// 添加暂停服务人员
        /// </summary>
        /// <param name="userIds"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public int AddUser(List<string> userIds, int state)
        {
            var sqlInsert1 = "INSERT INTO P_GROUP_MEMBER(ID, GroupType, UserId, UserName, CreateDate, CreateUserId, State1) SELECT @ID, @GroupType, UserId, [Name], @CreateDate, @CreateUserId, @State FROM P_USERINFO WHERE UserId = @UserId ";
            var sqlInsert2 = "INSERT INTO P_GROUP_MEMBER(ID, GroupType, UserId, UserName, CreateDate, CreateUserId, State2) SELECT @ID, @GroupType, UserId, [Name], @CreateDate, @CreateUserId, @State FROM P_USERINFO WHERE UserId = @UserId ";
            var sqlInsert3 = "INSERT INTO P_GROUP_MEMBER(ID, GroupType, UserId, UserName, CreateDate, CreateUserId, State3) SELECT @ID, @GroupType, UserId, [Name], @CreateDate, @CreateUserId, @State FROM P_USERINFO WHERE UserId = @UserId ";
            var sqlInsert = "";
            var sqlUpdate1 = "UPDATE P_GROUP_MEMBER SET State1 = 1 WHERE GroupType = 4 AND UserId = @UserId ";
            var sqlUpdate2 = "UPDATE P_GROUP_MEMBER SET State2 = 1 WHERE GroupType = 4 AND UserId = @UserId ";
            var sqlUpdate3 = "UPDATE P_GROUP_MEMBER SET State3 = 1 WHERE GroupType = 4 AND UserId = @UserId ";
            var sqlUpdate = "";

            var sqlSelectAll = "SELECT UserId FROM P_GROUP_MEMBER WHERE GroupType = 4 ";

            switch (state)
            {
                case 1:
                    sqlInsert = sqlInsert1;
                    sqlUpdate = sqlUpdate1;
                    break;
                case 2:
                    sqlInsert = sqlInsert2;
                    sqlUpdate = sqlUpdate2;
                    break;
                case 3:
                    sqlInsert = sqlInsert3;
                    sqlUpdate = sqlUpdate3;
                    break;
            }

            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            var num = 0;
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var allMembers = sqlServerTemplate.Load<P_GROUP_MEMBER>(sqlSelectAll, new SqlParameter[] { }).Select(i => i.UserId.ToLower());
                foreach (var _id in userIds)
                {
                    //已存储该用户
                    if (allMembers.Contains(_id.ToLower()) == true)
                    {
                        sqlServerTemplate.ExecuteNonQuery(sqlUpdate, new SqlParameter[] {
                                        SqlParameterFactory.GetSqlParameter("@UserId", _id)
                                        });
                        num++;
                    }
                    else
                    {
                        // 新增用户
                        var res = sqlServerTemplate.ExecuteNonQuery(sqlInsert, new SqlParameter[] {
                                        SqlParameterFactory.GetSqlParameter("@ID", Guid.NewGuid()),
                                        SqlParameterFactory.GetSqlParameter("@GroupType", 4),
                                        SqlParameterFactory.GetSqlParameter("@CreateDate", DateTime.Now),
                                        SqlParameterFactory.GetSqlParameter("@CreateUserId", "user01.admin"),
                                        SqlParameterFactory.GetSqlParameter("@UserId", _id),
                                        SqlParameterFactory.GetSqlParameter("@State",1)
                                        });
                        num++;
                    }
                }
            }
            return num;
        }
        #endregion

        #endregion
    }
}
