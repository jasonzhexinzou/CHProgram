using MealAdmin.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XFramework.XDataBase;
using XFramework.XInject.Attributes;
using XFramework.XDataBase.SqlServer;
using System.Data.SqlClient;
using IamPortal.AppLogin;
using MealAdmin.Util;
using System.Web;


namespace MealAdmin.Dao
{
    public class OperationAuditDao : IOperationAuditDao
    {
        [Bean("sqlServerTemplFactory")]
        public DbTemplateFactory sqlServerTemplFactory { get; set; }
        private string selectSql = "SELECT [ID],[OperatorName] ,[OperatorID] ,[TypeID],[Operation] ,[StateID] ,[Exception],[CreateDate] "
                                + "FROM P_OPERATIONAUDIT  ";
        private string selectSql1 = "SELECT [ID],[UserID] ,[CreatDate] ,[Type],[ChangeContent] "
                               + "FROM db_owner.P_AUDIT  where 1=1";
        private string selectOrderBySql = " ORDER BY CREATEDATE DESC";
        private string selectOrderBySql1 = " ORDER BY CREATDATE DESC";
        public int AddAudit(string Type, string ChangeContent)
        {
            var CurrentAdminUser = HttpContext.Current.Session[MealAdmin.Util.ConstantHelper.CurrentAdminUser] as IamPortal.AppLogin.AdminUser;
            AdminUser adminUser = HttpContext.Current.Session[ConstantHelper.CurrentAdminUser] as AdminUser;
            var UserID = adminUser.Email;
            var CreatDate = DateTime.Now.ToString();
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery(
                    "INSERT db_owner.P_Audit(ID, UserId,CreatDate,Type,ChangeContent)"
                    + " VALUES (@ID,@UserId,@CreatDate,@Type,@ChangeContent ) ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", Guid.NewGuid()),
                        SqlParameterFactory.GetSqlParameter("@UserId",UserID ),
                        SqlParameterFactory.GetSqlParameter("@CreatDate", CreatDate),
                        SqlParameterFactory.GetSqlParameter("@Type", Type),
                        SqlParameterFactory.GetSqlParameter("@ChangeContent", ChangeContent)

                    });
            }
        }
        public List<P_OperationAudit> Load(int rows, int page, out int total)
        {
            List<P_OperationAudit> rtnData;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.LoadPages<P_OperationAudit>(rows, page, out total, selectSql1, selectOrderBySql, null);
            }
            return rtnData;
        }
        public List<P_Audit> Load1(string ApprovalMUDID, string Begin, string End, string CostCenter, string SpecialOrders1, string SpecialOrders2, string UploadFile, string SystemGroup, string AgentApprova,string Hospital, int rows, int page, out int total)
        {
            List<P_Audit> rtnData;
            var _selectSql1 = "";
            var _selectSql2 = "";
            var sql = " and Type IN (";
            List<string> list = new List<string>();
            if (!string.IsNullOrEmpty(ApprovalMUDID))
            {
                _selectSql1 += "and (UserId=@ApprovalMUDID)";
            }
            if (!string.IsNullOrEmpty(Begin))
            {
                _selectSql1 += " and (CreatDate>=@Begin)";
            }
            if (!string.IsNullOrEmpty(End))
            {
                DateTime End1 = Convert.ToDateTime(End);
                End = End1.AddDays(1).ToShortDateString();
                _selectSql1 += " and (CreatDate<=@End)";

            }

            if (!string.IsNullOrEmpty(CostCenter))
            {

                list.Add(CostCenter);
            }
            if (!string.IsNullOrEmpty(SpecialOrders1))
            {

                list.Add(SpecialOrders1);
            }
            if (!string.IsNullOrEmpty(SpecialOrders2))
            {

                list.Add(SpecialOrders2);
            }
            if (!string.IsNullOrEmpty(UploadFile))
            {

                list.Add(UploadFile);
            }
            if (!string.IsNullOrEmpty(SystemGroup))
            {

                list.Add(SystemGroup);
            }

            if (!string.IsNullOrEmpty(AgentApprova))
            {

                list.Add(AgentApprova);
            }
            if (!string.IsNullOrEmpty(Hospital))
            {

                list.Add(Hospital);
            }
           
            for (int i = 0; i < list.Count; i++)
            {
                if (i == 0)
                {
                    sql += list[i];
                }
                else
                {
                    sql += "," + list[i];
                }

                if (i + 1 == list.Count)
                {
                    sql += ")";
                    _selectSql2 = sql;
                    break;
                }
            }

            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.LoadPages<P_Audit>(rows, page, out total, selectSql1 + _selectSql1 + _selectSql2, selectOrderBySql1, new SqlParameter[]
                    {

                        SqlParameterFactory.GetSqlParameter("@ApprovalMUDID", ApprovalMUDID),
                        SqlParameterFactory.GetSqlParameter("@Begin",Begin ),
                        SqlParameterFactory.GetSqlParameter("@End", End),
                        SqlParameterFactory.GetSqlParameter("@CostCenter", CostCenter),
                        SqlParameterFactory.GetSqlParameter("@SpecialOrders1", SpecialOrders1),
                        SqlParameterFactory.GetSqlParameter("@SpecialOrders2", SpecialOrders2),
                        SqlParameterFactory.GetSqlParameter("@UploadFile", UploadFile),
                        SqlParameterFactory.GetSqlParameter("@SystemGroup", SystemGroup),
                         SqlParameterFactory.GetSqlParameter("@Hospital", Hospital),
                        //SqlParameterFactory.GetSqlParameter("@State",srh_State),
                      
                    });
            }
            return rtnData;
        }
        #region 保存操作日志
        /// <summary>
        /// 保存操作日志
        /// </summary>
        /// <param name="OperationAudit"></param>
        /// <returns></returns>
        public void SaveOperationAudit(P_OperationAudit OperationAudit)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            int updCnt;
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                updCnt = sqlServerTemplate.ExecuteNonQuery(
                    "INSERT INTO P_OPERATIONAUDIT(ID, OperatorName, OperatorID, TypeID,Operation,StateID,Exception,CreateDate) VALUES (@ID, @OperatorName, @OperatorID, @TypeID,@Operation,@StateID,@Exception,@CreateDate) ",
                    new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@ID", OperationAudit.ID),
                        SqlParameterFactory.GetSqlParameter("@OperatorName", OperationAudit.OperatorName),
                        SqlParameterFactory.GetSqlParameter("@OperatorID", OperationAudit.OperatorID),
                        SqlParameterFactory.GetSqlParameter("@TypeID", OperationAudit.TypeID),
                        SqlParameterFactory.GetSqlParameter("@Operation", OperationAudit.Operation),
                        SqlParameterFactory.GetSqlParameter("@StateID", OperationAudit.StateID),
                        SqlParameterFactory.GetSqlParameter("@Exception", OperationAudit.Exception),
                        SqlParameterFactory.GetSqlParameter("@CreateDate", OperationAudit.CreateDate)
                    });
            }
        }
        #endregion
    }
}
