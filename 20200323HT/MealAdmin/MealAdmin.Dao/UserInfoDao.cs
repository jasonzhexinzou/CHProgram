using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MealAdmin.Entity;
using XFramework.XInject.Attributes;
using System.Data.SqlClient;
using XFramework.XDataBase.SqlServer;
using XFramework.XDataBase;
using System.Data;
using MealAdmin.Entity.View;

namespace MealAdmin.Dao
{
    public class UserInfoDao : IUserInfoDao
    {
        [Bean("sqlServerTemplFactory")]
        public DbTemplateFactory sqlServerTemplFactory { get; set; }
        [Bean("sqlServerTemplFactoryNonHT")]
        public DbTemplateFactory sqlServerTemplFactoryNonHT { get; set; }


        #region 判断是否是直线经理
        public P_Count isHaveApproval(string userId)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Find<P_Count>("SELECT COUNT(*) AS ApprovalCount  "
                                                        + "FROM WP_QYUSER U INNER JOIN WP_QYUSER UP  "
                                                        + "ON U.ID = UP.LineManagerID  "
                                                        + "WHERE U.UserId = @userId ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@userId",userId),

                    });
            }

        }
        #endregion

        #region 判断是否是二线经理
        public P_Count isSecondApproval(string userId)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Find<P_Count>("SELECT COUNT(*) AS ApprovalCount  "
                                                        + "FROM WP_QYUSER U INNER JOIN WP_QYUSER UP  "
                                                        + "ON U.ID = UP.LineManagerID INNER JOIN WP_QYUSER UP2    "
                                                        + "ON UP.ID = UP2.LineManagerID    "
                                                        + "WHERE U.UserId = @userId ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@userId",userId),

                    });
            }

        }
        #endregion

        #region 保存代理审批
        public int SaveAgent(Guid ID, string userId, string userName, string delegateUserMUDID, string delegateUserName, DateTime startTime, DateTime endTime, int isEnable, string OperatorMUDID)
        {
            var end = Convert.ToDateTime(endTime.ToString("yyyy-MM-dd") + " 23:59:59");
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();

                var hisRes = sqlServerTemplate.ExecuteNonQuery("INSERT INTO P_UserDelegateHis " +
                    " (ID,UserMUDID,UserName,DelegateUserMUDID,DelegateUserName,StartTime,EndTime,IsEnable,OperationTime,OperatorMUDID)" + "VALUES (@ID,@UserMUDID,@UserName,@DelegateUserMUDID,@DelegateUserName,@StartTime,@EndTime,@IsEnable,@OperationTime,@OperatorMUDID)",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID",Guid.NewGuid()),
                        SqlParameterFactory.GetSqlParameter("@UserMUDID",userId),
                        SqlParameterFactory.GetSqlParameter("@UserName",userName),
                        SqlParameterFactory.GetSqlParameter("@DelegateUserMUDID",delegateUserMUDID),
                        SqlParameterFactory.GetSqlParameter("@DelegateUserName",delegateUserName),
                        SqlParameterFactory.GetSqlParameter("@StartTime",startTime.ToString("yyyy-MM-dd") ),
                        SqlParameterFactory.GetSqlParameter("@EndTime",end ),
                        SqlParameterFactory.GetSqlParameter("@IsEnable",isEnable),
                        SqlParameterFactory.GetSqlParameter("@OperationTime",DateTime.Now),
                        SqlParameterFactory.GetSqlParameter("@OperatorMUDID",OperatorMUDID)
                    });

                return sqlServerTemplate.ExecuteNonQuery("INSERT INTO P_UserDelegate " +
                    " (ID,UserMUDID,UserName,DelegateUserMUDID,DelegateUserName,StartTime,EndTime,IsEnable)" + "VALUES (@ID,@UserMUDID,@UserName,@DelegateUserMUDID,@DelegateUserName,@StartTime,@EndTime,@IsEnable)",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID",ID),
                        SqlParameterFactory.GetSqlParameter("@UserMUDID",userId),
                        SqlParameterFactory.GetSqlParameter("@UserName",userName),
                        SqlParameterFactory.GetSqlParameter("@DelegateUserMUDID",delegateUserMUDID),
                        SqlParameterFactory.GetSqlParameter("@DelegateUserName",delegateUserName),
                        SqlParameterFactory.GetSqlParameter("@StartTime",startTime.ToString("yyyy-MM-dd") ),
                        SqlParameterFactory.GetSqlParameter("@EndTime",end ),
                        SqlParameterFactory.GetSqlParameter("@IsEnable",isEnable)
                    });
            }
        }

        public int SaveSecondAgent(Guid ID, string userId, string userName, string delegateUserMUDID, string delegateUserName, DateTime startTime, DateTime endTime, int isEnable, string OperatorMUDID)
        {
            var end = Convert.ToDateTime(endTime.ToString("yyyy-MM-dd") + " 23:59:59");
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();

                var hisRes = sqlServerTemplate.ExecuteNonQuery("INSERT INTO P_UserDelegatePreHis " +
                    " (ID,UserMUDID,UserName,DelegateUserMUDID,DelegateUserName,StartTime,EndTime,IsEnable,OperationTime,OperatorMUDID)" + "VALUES (@ID,@UserMUDID,@UserName,@DelegateUserMUDID,@DelegateUserName,@StartTime,@EndTime,@IsEnable,@OperationTime,@OperatorMUDID)",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID",Guid.NewGuid()),
                        SqlParameterFactory.GetSqlParameter("@UserMUDID",userId),
                        SqlParameterFactory.GetSqlParameter("@UserName",userName),
                        SqlParameterFactory.GetSqlParameter("@DelegateUserMUDID",delegateUserMUDID),
                        SqlParameterFactory.GetSqlParameter("@DelegateUserName",delegateUserName),
                        SqlParameterFactory.GetSqlParameter("@StartTime",startTime.ToString("yyyy-MM-dd") ),
                        SqlParameterFactory.GetSqlParameter("@EndTime",end ),
                        SqlParameterFactory.GetSqlParameter("@IsEnable",isEnable),
                        SqlParameterFactory.GetSqlParameter("@OperationTime",DateTime.Now),
                        SqlParameterFactory.GetSqlParameter("@OperatorMUDID",OperatorMUDID)
                    });

                return sqlServerTemplate.ExecuteNonQuery("INSERT INTO P_UserDelegatePre " +
                    " (ID,UserMUDID,UserName,DelegateUserMUDID,DelegateUserName,StartTime,EndTime,IsEnable)" + "VALUES (@ID,@UserMUDID,@UserName,@DelegateUserMUDID,@DelegateUserName,@StartTime,@EndTime,@IsEnable)",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID",ID),
                        SqlParameterFactory.GetSqlParameter("@UserMUDID",userId),
                        SqlParameterFactory.GetSqlParameter("@UserName",userName),
                        SqlParameterFactory.GetSqlParameter("@DelegateUserMUDID",delegateUserMUDID),
                        SqlParameterFactory.GetSqlParameter("@DelegateUserName",delegateUserName),
                        SqlParameterFactory.GetSqlParameter("@StartTime",startTime.ToString("yyyy-MM-dd") ),
                        SqlParameterFactory.GetSqlParameter("@EndTime",end ),
                        SqlParameterFactory.GetSqlParameter("@IsEnable",isEnable)
                    });
            }
        }
        #endregion

        #region 判断是否是代理人
        public P_UserDelegate isAgent(string userId)
        {
            var nowDate = DateTime.Now;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Find<P_UserDelegate>("  SELECT*   "
                                                            + "FROM P_UserDelegate  "
                                                            + "WHERE (UserMUDID=@UserId) and (@nowDate>=StartTime) and  (@nowDate<=EndTime) and (IsEnable = 1) ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId",userId),
                        SqlParameterFactory.GetSqlParameter("@nowDate",nowDate)

                    });
            }
        }

        public P_UserDelegate isAgentBack(string userId)
        {
            var nowDate = DateTime.Now;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Find<P_UserDelegate>("  SELECT*   "
                                                            + "FROM P_UserDelegate  "
                                                            + "WHERE (UserMUDID=@UserId) ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId",userId),
                        SqlParameterFactory.GetSqlParameter("@nowDate",nowDate)

                    });
            }
        }
        #endregion

        #region 判断是否是二线代理人
        public P_UserDelegatePre isSecondAgent(string userId)
        {
            var nowDate = DateTime.Now;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Find<P_UserDelegatePre>("  SELECT*   "
                                                            + "FROM P_UserDelegatePre  "
                                                            + "WHERE (UserMUDID=@UserId) and (@nowDate>=StartTime) and  (@nowDate<=EndTime) and (IsEnable = 1) ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId",userId),
                        SqlParameterFactory.GetSqlParameter("@nowDate",nowDate)

                    });
            }
        }

        public P_UserDelegatePre isSecondAgentBack(string userId)
        {
            var nowDate = DateTime.Now;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Find<P_UserDelegatePre>("  SELECT*   "
                                                            + "FROM P_UserDelegatePre  "
                                                            + "WHERE (UserMUDID=@UserId)",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId",userId),
                        SqlParameterFactory.GetSqlParameter("@nowDate",nowDate)

                    });
            }
        }
        #endregion

        #region 添加使用人信息
        /// <summary>
        /// 添加使用人信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Add(P_USERINFO entity)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("INSERT INTO P_USERINFO " +
                    " (ID,UserId,Name,PhoneNumber,CreateDate)" + "VALUES (@ID,@UserId,@Name,@PhoneNumber,@CreateDate)",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", entity.ID),
                        SqlParameterFactory.GetSqlParameter("@UserId", entity.UserId),
                        SqlParameterFactory.GetSqlParameter("@Name", entity.Name),
                        SqlParameterFactory.GetSqlParameter("@PhoneNumber", entity.PhoneNumber),
                        SqlParameterFactory.GetSqlParameter("@CreateDate", entity.CreateDate)
                    });
            }
        }
        #endregion

        #region 修改信息
        /// <summary>
        /// 修改信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="market"></param>
        /// <returns></returns>
        public int Edit(string userId, string phoneNumber, string market, string tacode)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("UPDATE P_USERINFO SET PhoneNumber=@PhoneNumber,Market=@Market,TerritoryCode=@tacode WHERE UserId=@UserId",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@PhoneNumber", phoneNumber),
                        SqlParameterFactory.GetSqlParameter("@Market", market),
                        SqlParameterFactory.GetSqlParameter("@tacode", tacode),
                        SqlParameterFactory.GetSqlParameter("@UserId", userId)
                    });
            }
        }
        #endregion

        #region 根据userId查询人员信息（平台）
        /// <summary>
        /// 根据userId查询人员信息（平台）
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public WP_QYUSER Find(string userId)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var entity = sqlServerTemplate.Find<WP_QYUSER>(
                    "SELECT * FROM WP_QYUSER WHERE UserId=@UserId",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", userId)
                    });
                return entity;
            }
        }
        #endregion

        #region 根据userId查询人员信息（应用）
        /// <summary>
        /// 根据userId查询人员信息（应用）
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public P_USERINFO FindByUserId(string userId)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var entity = sqlServerTemplate.Find<P_USERINFO>(
                    "SELECT * FROM P_USERINFO WHERE UserId=@UserId",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", userId)
                    });
                return entity;
            }
        }

        public P_USERINFO FindTAByUserId(string userId)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var entity = sqlServerTemplate.Find<P_USERINFO>(
                    "SELECT [ID],[UserId],[Name],[PhoneNumber] ,[CreateDate],[IsCheckedStatement],[Market],[TerritoryCode] FROM P_USERINFO UI WHERE UI.UserId =@UserId",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", userId)
                    });
                return entity;
            }
        }
        #endregion

        #region 用户接收协议
        /// <summary>
        /// 用户接收协议
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int CheckedStatement(string userId)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("UPDATE P_USERINFO SET IsCheckedStatement=1 WHERE UserId=@UserId",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", userId)
                    });
            }
        }
        #endregion

        #region 后台人员管理-List 
        private string selectSql = "select * from(SELECT u.[ID], u.[UserId],u.[Name],u.[PhoneNumber],u.[CreateDate],u.[IsCheckedStatement],qy.[State],u.[Market],u.[TerritoryCode],isnull(db_owner.GetDMName(u.UserId),'') as DMName,isnull(db_owner.GetDMUserID(u.UserId),'') as DMUserId,qy.Position as Title FROM P_USERINFO u left join WP_QYUSER qy on u.userid=qy.userid)a where(a.UserId LIKE '%' + @UserId + '%' or @UserId='') AND(a.DMUserId LIKE '%'+@DMUserId+'%' or '@DMUserId'='') ";
        private string selectOrderBySql = " ORDER BY CreateDate DESC";
        public List<P_USERINFO> LoadPage(string userId, string DMUserId, int rows, int page, out int total)
        {
            List<P_USERINFO> rtnData;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.LoadPages<P_USERINFO>(rows, page, out total,
                    selectSql,
                    selectOrderBySql, new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", userId),
                        SqlParameterFactory.GetSqlParameter("@DMUserId", DMUserId)
                    });
            }
            return rtnData;
        }
        #endregion

        #region 代理人信息
        public P_UserDelegate AgentInfo(string userId)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var entity = sqlServerTemplate.Find<P_UserDelegate>(
                    "SELECT * FROM P_UserDelegate WHERE UserId=@UserId",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", userId)
                    });
                return entity;
            }
        }
        #endregion

        #region 查询代理人信息是否存在
        public P_UserDelegate AgentExist(Guid DelegateID)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var entity = sqlServerTemplate.Find<P_UserDelegate>(
                    "SELECT * FROM P_UserDelegate WHERE ID=@DelegateID",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@DelegateID", DelegateID)
                    });
                return entity;
            }
        }

        #endregion

        #region 修改代理人信息
        public int UpdateAgent(Guid DelegateID, string userId, string userName, string delegateUserMUDID, string delegateUserName, DateTime startTime, DateTime endTime, int isEnable, string OperatorMUDID)
        {
            var end = endTime.ToString("yyyy-MM-dd") + " 23:59:59";
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();

                var hisRes = sqlServerTemplate.ExecuteNonQuery("INSERT INTO P_UserDelegateHis " +
                    " (ID,UserMUDID,UserName,DelegateUserMUDID,DelegateUserName,StartTime,EndTime,IsEnable,OperationTime,OperatorMUDID)" + "VALUES (@ID,@UserMUDID,@UserName,@DelegateUserMUDID,@DelegateUserName,@StartTime,@EndTime,@IsEnable,@OperationTime,@OperatorMUDID)",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID",Guid.NewGuid()),
                        SqlParameterFactory.GetSqlParameter("@UserMUDID",userId),
                        SqlParameterFactory.GetSqlParameter("@UserName",userName),
                        SqlParameterFactory.GetSqlParameter("@DelegateUserMUDID",delegateUserMUDID),
                        SqlParameterFactory.GetSqlParameter("@DelegateUserName",delegateUserName),
                        SqlParameterFactory.GetSqlParameter("@StartTime",startTime.ToString("yyyy-MM-dd") ),
                        SqlParameterFactory.GetSqlParameter("@EndTime",end ),
                        SqlParameterFactory.GetSqlParameter("@IsEnable",isEnable),
                        SqlParameterFactory.GetSqlParameter("@OperationTime",DateTime.Now),
                        SqlParameterFactory.GetSqlParameter("@OperatorMUDID",OperatorMUDID)
                    });

                return sqlServerTemplate.ExecuteNonQuery("UPDATE P_UserDelegate SET UserMUDID=@userId,UserName=@userName,DelegateUserMUDID=@delegateUserMUDID,DelegateUserName=@delegateUserName,StartTime=@startTime,EndTime=@endTime,IsEnable=@isEnable  WHERE ID=@DelegateID",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@DelegateID", DelegateID),
                        SqlParameterFactory.GetSqlParameter("@userId", userId),
                        SqlParameterFactory.GetSqlParameter("@userName", userName),
                        SqlParameterFactory.GetSqlParameter("@delegateUserMUDID", delegateUserMUDID),
                        SqlParameterFactory.GetSqlParameter("@delegateUserName", delegateUserName),
                        SqlParameterFactory.GetSqlParameter("@startTime", startTime.ToString("yyyy-MM-dd")),
                        SqlParameterFactory.GetSqlParameter("@endTime", end),
                        SqlParameterFactory.GetSqlParameter("@isEnable", isEnable),
                    });
            }
        }

        public int UpdateSecondAgent(Guid DelegateID, string userId, string userName, string delegateUserMUDID, string delegateUserName, DateTime startTime, DateTime endTime, int isEnable, string OperatorMUDID)
        {
            var end = endTime.ToString("yyyy-MM-dd") + " 23:59:59";
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();

                var hisRes = sqlServerTemplate.ExecuteNonQuery("INSERT INTO P_UserDelegatePreHis " +
                    " (ID,UserMUDID,UserName,DelegateUserMUDID,DelegateUserName,StartTime,EndTime,IsEnable,OperationTime,OperatorMUDID)" + "VALUES (@ID,@UserMUDID,@UserName,@DelegateUserMUDID,@DelegateUserName,@StartTime,@EndTime,@IsEnable,@OperationTime,@OperatorMUDID)",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID",Guid.NewGuid()),
                        SqlParameterFactory.GetSqlParameter("@UserMUDID",userId),
                        SqlParameterFactory.GetSqlParameter("@UserName",userName),
                        SqlParameterFactory.GetSqlParameter("@DelegateUserMUDID",delegateUserMUDID),
                        SqlParameterFactory.GetSqlParameter("@DelegateUserName",delegateUserName),
                        SqlParameterFactory.GetSqlParameter("@StartTime",startTime.ToString("yyyy-MM-dd") ),
                        SqlParameterFactory.GetSqlParameter("@EndTime",end ),
                        SqlParameterFactory.GetSqlParameter("@IsEnable",isEnable),
                        SqlParameterFactory.GetSqlParameter("@OperationTime",DateTime.Now),
                        SqlParameterFactory.GetSqlParameter("@OperatorMUDID",OperatorMUDID)
                    });

                return sqlServerTemplate.ExecuteNonQuery("UPDATE P_UserDelegatePre SET UserMUDID=@userId,UserName=@userName,DelegateUserMUDID=@delegateUserMUDID,DelegateUserName=@delegateUserName,StartTime=@startTime,EndTime=@endTime,IsEnable=@isEnable  WHERE ID=@DelegateID",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@DelegateID", DelegateID),
                        SqlParameterFactory.GetSqlParameter("@userId", userId),
                        SqlParameterFactory.GetSqlParameter("@userName", userName),
                        SqlParameterFactory.GetSqlParameter("@delegateUserMUDID", delegateUserMUDID),
                        SqlParameterFactory.GetSqlParameter("@delegateUserName", delegateUserName),
                        SqlParameterFactory.GetSqlParameter("@startTime", startTime.ToString("yyyy-MM-dd")),
                        SqlParameterFactory.GetSqlParameter("@endTime", end),
                        SqlParameterFactory.GetSqlParameter("@isEnable", isEnable),
                    });
            }
        }

        #endregion

        #region 审批人代理查询
        public List<P_UserDelegate> ApproverAgentLoad(string ApprovalNameOrMUDID, string AgentNameOrMUDID, int rows, int page, out int total)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnData = sqlServerTemplate.LoadPages<P_UserDelegate>(rows, page, out total, "SELECT * FROM P_UserDelegate WHERE ((UserName LIKE '%' + @ApprovalNameOrMUDID + '%') or (UserMUDID LIKE '%' + @ApprovalNameOrMUDID + '%')) AND ((DelegateUserName LIKE '%' + @AgentNameOrMUDID + '%') or (DelegateUserMUDID LIKE '%' + @AgentNameOrMUDID + '%'))", " ORDER BY EndTime DESC",
                    new SqlParameter[]
                    {
                         SqlParameterFactory.GetSqlParameter("@ApprovalNameOrMUDID", ApprovalNameOrMUDID),
                        SqlParameterFactory.GetSqlParameter("@AgentNameOrMUDID", AgentNameOrMUDID)

                    });
                return rtnData;
            }
        }

        public List<P_UserDelegatePre> ApproverSecondAgentLoad(string ApprovalNameOrMUDID, string AgentNameOrMUDID, int rows, int page, out int total)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnData = sqlServerTemplate.LoadPages<P_UserDelegatePre>(rows, page, out total, "SELECT * FROM P_UserDelegatePre WHERE ((UserName LIKE '%' + @ApprovalNameOrMUDID + '%') or (UserMUDID LIKE '%' + @ApprovalNameOrMUDID + '%')) AND ((DelegateUserName LIKE '%' + @AgentNameOrMUDID + '%') or (DelegateUserMUDID LIKE '%' + @AgentNameOrMUDID + '%'))", " ORDER BY EndTime DESC",
                    new SqlParameter[]
                    {
                         SqlParameterFactory.GetSqlParameter("@ApprovalNameOrMUDID", ApprovalNameOrMUDID),
                        SqlParameterFactory.GetSqlParameter("@AgentNameOrMUDID", AgentNameOrMUDID)

                    });
                return rtnData;
            }
        }
        #endregion

        #region 同步WD人员数据
        /// <summary>
        /// 同步WD人员数据
        /// </summary>
        /// <param name="UserList"></param>
        /// <returns></returns>
        public void SyncWorkDayUserInfo(List<WP_QYUSER> UserList)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            //using (var conn = sqlServerTemplate.GetSqlConnection() as SqlConnection)
            //{
            //    conn.Open();
            //    var tran = conn.BeginTransaction();
            //    //删除旧的人员数据
            //    SqlCommand commandDelete = new SqlCommand("DELETE FROM WP_QYUSER where gender<>2", conn);
            //    commandDelete.Transaction = tran;
            //    commandDelete.ExecuteNonQuery();
            //    //导入新的人员数据
            //    foreach (var item in UserList)
            //    {
            //        SqlCommand commandAdd = new SqlCommand(
            //            "INSERT INTO [WP_QYUSER] ([ID] ,[WechatID] ,[Name] ,[UserId] ,[Email] ,[Gender] ,[Position] ,[State] ,[DeptNames] ,[CreateDate] ,[Creator] ,[ModifyDate] ,[Modifier] ,[LineManagerID]) "
            //            + " VALUES (@ID ,@WechatID ,@Name ,@UserId ,@Email ,@Gender ,@Position ,@State ,@DeptNames ,@CreateDate ,@Creator ,@ModifyDate ,@Modifier ,@LineManagerID) ",
            //            conn);
            //        commandAdd.Transaction = tran;
            //        commandAdd.CommandTimeout = 60;
            //        commandAdd.Parameters.AddRange(
            //            new SqlParameter[]
            //            {
            //                SqlParameterFactory.GetSqlParameter("@ID", item.ID),
            //                SqlParameterFactory.GetSqlParameter("@WechatID", item.WechatID),
            //                SqlParameterFactory.GetSqlParameter("@Name", item.Name),
            //                SqlParameterFactory.GetSqlParameter("@UserId", item.UserId),
            //                SqlParameterFactory.GetSqlParameter("@Email", item.Email),
            //                SqlParameterFactory.GetSqlParameter("@Gender", item.Gender),
            //                SqlParameterFactory.GetSqlParameter("@Position", item.Position),
            //                SqlParameterFactory.GetSqlParameter("@State", item.State),
            //                SqlParameterFactory.GetSqlParameter("@DeptNames", item.DeptNames),
            //                SqlParameterFactory.GetSqlParameter("@CreateDate", item.CreateDate),
            //                SqlParameterFactory.GetSqlParameter("@Creator", item.Creator),
            //                SqlParameterFactory.GetSqlParameter("@ModifyDate", item.ModifyDate),
            //                SqlParameterFactory.GetSqlParameter("@Modifier", item.Modifier),
            //                SqlParameterFactory.GetSqlParameter("@LineManagerID", item.LineManagerID)
            //            }
            //        );
            //        commandAdd.ExecuteNonQuery();
            //    }
            //    tran.Commit();
            //}

            int insCnt = 0;

            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            //int insCntTemp = 0;
            using (var conn = sqlServerTemplate.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();

                var delCmd = conn.CreateCommand();
                delCmd.CommandText = "DELETE FROM WP_QYUSER where gender<>2 ";
                delCmd.Transaction = tran;
                delCmd.ExecuteNonQuery();

                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT TOP 0 * FROM WP_QYUSER ";
                cmd.Transaction = tran;
                SqlDataAdapter adapter = new SqlDataAdapter();
                DataSet ds = new DataSet();
                adapter.SelectCommand = cmd;
                adapter.Fill(ds);

                var dt = ds.Tables[0];
                dt.Clear();
                foreach (var data in UserList)
                {
                    var dr = dt.NewRow();
                    dr[0] = data.ID;
                    dr[1] = data.WechatID;
                    dr[2] = data.Name;
                    dr[3] = data.UserId;
                    dr[4] = DBNull.Value;
                    dr[5] = DBNull.Value;
                    dr[6] = data.Email;
                    dr[7] = data.Gender;
                    dr[8] = data.Position;
                    dr[9] = DBNull.Value;
                    dr[10] = DBNull.Value;
                    dr[11] = data.State;
                    dr[12] = data.DeptNames;
                    dr[13] = DBNull.Value;
                    dr[14] = data.CreateDate;
                    dr[15] = data.Creator;
                    dr[16] = data.ModifyDate;
                    dr[17] = data.Modifier;
                    dr[18] = 0;
                    dr[19] = DBNull.Value;
                    dr[20] = DBNull.Value;
                    dr[21] = data.LineManagerID;
                    dr[22] = 0;
                    dr[23] = DBNull.Value;
                    dr[24] = DBNull.Value;
                    dt.Rows.Add(dr);
                }

                SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, tran);
                bulkCopy.DestinationTableName = "WP_QYUSER";
                bulkCopy.BatchSize = UserList.Count;

                bulkCopy.WriteToServer(dt);
                insCnt = bulkCopy.BatchSize;

                watch.Stop();
                watch.Restart();
                watch.Start();
                tran.Commit();
                watch.Stop();
                watch.Restart();
                watch.Start();
                conn.Close();
                watch.Stop();
            }

            //return insCnt;
        }
        #endregion

        #region 抓取已离职人员
        public List<P_AutoTransferView> LoadLeaveUserInfo()
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var entity = sqlServerTemplate.Load<P_AutoTransferView>(
                    "select qu.userid as UserId,qu.Name as UserName,qu.LineManagerID as LineManagerId,ord.CN as HTCode,ord.UserId as OrdUserId,ord.IsTransfer as OrdIsTransfer,"
                + " ord.TransferUserMUDID as OrdTransferMUDID,ord.IsOrderUpload as IsOrderUpload,"
                    + " pu.ApplierMUDID as UploadUserId,pu.IsTransfer as UploadIsTransfer,"
                    // Start UpdateBy zhexin.zou at 20190104
                    + " pu.TransferUserMUDID as UploadTransferMUDID,pu.State as UploadState from P_ORDER ord left join[dbo].[P_PreUploadOrder] pu on ord.CN = pu.HTCode"
                // End UpdateBy zhexin.zou at 20190104
                + " inner join[dbo].[WP_QYUSER] qu"
                   + " on (((ord.UserId = qu.UserId and ord.IsTransfer = 0) or(ord.TransferUserMUDID = qu.UserId and ord.IsTransfer = 1) and ord.IsOrderUpload = 0)"
                    + " or((pu.ApplierMUDID = qu.UserId and pu.IsTransfer = 0) or(ord.TransferUserMUDID = qu.UserId and pu.IsTransfer = 1)) and ord.IsOrderUpload = 1)"
                        + " and qu.[State]=4 where (pu.[State]<>4 or pu.[State] is null) and ord.[State]<>11 and ord.[State]<>5 and substring(ord.[CN],1,1)='H'",
                    new SqlParameter[]
                    {
                    });
                return entity;
            }
        }

        public LineManager FindUserManagerInfo(Guid LineManagerId)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var entity = sqlServerTemplate.Find<LineManager>(
                    "select userid as LineManagerId,name as LineManagerName from [dbo].[WP_QYUSER] where ID=@LineManagerId",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@LineManagerId", LineManagerId),
                    });
                return entity;
            }
        }

        public LineManager FindUserManagerInfo(string UserId)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var entity = sqlServerTemplate.Find<LineManager>(
                    "select pu.UserId as LineManagerId,pu.Name as LineManagerName from [dbo].[WP_QYUSER] qu inner join [dbo].[WP_QYUSER] pu on qu.LineManagerID=pu.ID where qu.UserId=@LineManagerId",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@LineManagerId", UserId),
                    });
                conn.Close();
                return entity;
            }
        }


        public List<LineManagerUser> FindUserManagerInfo()
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var entity = sqlServerTemplate.Load<LineManagerUser>(
                    "select qu.UserId,pu.UserId as LineManagerId from [dbo].[WP_QYUSER] as qu join [dbo].[WP_QYUSER] as pu on qu.LineManagerId = pu.ID",
                    new SqlParameter[]
                    {
                    });
                conn.Close();
                return entity;
            }
        }
        #endregion

        #region 代理人历史查询
        public List<P_UserDelegateHis> ApproverAgentHisLoad(string UserId)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnData = sqlServerTemplate.Load<P_UserDelegateHis>("SELECT * FROM P_UserDelegateHis WHERE UserMUDID = @UserId order by OperationTime desc",
                    new SqlParameter[]
                    {
                         SqlParameterFactory.GetSqlParameter("@UserId", UserId)

                    });
                return rtnData;
            }
        }


        public List<P_UserDelegatePreHis> ApproverSecondAgentHisLoad(string UserId)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnData = sqlServerTemplate.Load<P_UserDelegatePreHis>("SELECT * FROM P_UserDelegatePreHis WHERE UserMUDID = @UserId order by OperationTime desc",
                    new SqlParameter[]
                    {
                         SqlParameterFactory.GetSqlParameter("@UserId", UserId)

                    });
                return rtnData;
            }
        }
        #endregion


        public List<WP_QYUSER> FindUserByLineManager(Guid LineManagerID)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var entity = sqlServerTemplate.Load<WP_QYUSER>(
                    "select * from [dbo].[WP_QYUSER] where LineManagerID = @LineManagerId",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@LineManagerId", LineManagerID),
                    });
                return entity;
            }
        }

        public int DeleteAgent(Guid ID)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var res = sqlServerTemplate.ExecuteNonQuery("DELETE FROM P_UserDelegate WHERE (ID = @ID)  ",
                    new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@ID", ID)
                    });
                return res;
            }
        }

        public int DeleteSecondAgent(Guid ID)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var res = sqlServerTemplate.ExecuteNonQuery("DELETE FROM P_UserDelegatePre WHERE (ID = @ID)  ",
                    new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@ID", ID)
                    });
                return res;
            }
        }

        public P_UserDelegate FindById(Guid ID)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var entity = sqlServerTemplate.Find<P_UserDelegate>(
                    "SELECT * FROM P_UserDelegate WHERE ID=@ID",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", ID)
                    });
                return entity;
            }
        }

        public P_UserDelegatePre FindSecondById(Guid ID)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var entity = sqlServerTemplate.Find<P_UserDelegatePre>(
                    "SELECT * FROM P_UserDelegatePre WHERE ID=@ID",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", ID)
                    });
                return entity;
            }
        }

        #region 判断用户角色是否存在
        public int CheckUserRole(string Role, string UserID, string Market)
        {
            string sqlWhere = "";
            if (Role == "RD")
            {
                sqlWhere = "AND MUD_ID_RD=@UserID";
            }
            else if (Role == "RM")
            {
                sqlWhere = "AND MUD_ID_RM=@UserID";
            }
            else
            {
                sqlWhere = "AND MUD_ID_MR=@UserID";
            }
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var entity = sqlServerTemplate.Find<P_COUNT>(
                    "SELECT COUNT(ID) Count FROM Territory_Hospital WHERE Market=@Market " + sqlWhere,
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@Market", Market),
                        SqlParameterFactory.GetSqlParameter("@UserID", UserID)
                    });
                return entity.Count;
            }
        }
        #endregion
    }

}

