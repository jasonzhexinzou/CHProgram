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
    public class MeetingDao : IMeetingDao
    {
        [Bean("sqlServerTemplFactory")]
        public DbTemplateFactory sqlServerTemplFactory { get; set; }

        #region 找到可用(未订餐)的会议
        /// <summary>
        /// 找到可用(未订餐)的会议
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<P_MEETING> LoadByUserId(string userId,string approvedDate)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<P_MEETING>("SELECT * FROM P_MEETING WHERE UserId=@UserId AND IsUsed=0 AND ApprovedDate>@ApprovedDate ORDER BY ApprovedDate DESC ",
                    new SqlParameter[] 
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", userId),
                        SqlParameterFactory.GetSqlParameter("@ApprovedDate",approvedDate)
                    });
                return list;
            }
        }
        #endregion

        #region 根据CN号查找会议信息
        /// <summary>
        /// 根据CN号查找会议信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public P_MEETING_VIEW FindByCode(string code)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var entity = sqlServerTemplate.Find<P_MEETING_VIEW>(
                    "SELECT a.*, b.Name AS 'UserName' FROM P_MEETING a LEFT JOIN WP_QYUSER b ON a.UserId=b.UserId WHERE a.Code=@Code ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@Code", code)
                    });
                return entity;
            }
        }
        #endregion

        #region 使用CN号
        /// <summary>
        /// 使用CN号
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public int UsedCN(string code)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var entity = sqlServerTemplate.Update(
                    "UPDATE [P_MEETING] SET IsUsed=1 WHERE [Code]=@Code ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@Code", code)
                    });
                return entity;
            }
        }
        #endregion

        #region 释放CN号
        /// <summary>
        /// 释放CN号
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public int ReleaseCN(string code)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var entity = sqlServerTemplate.Update(
                    "UPDATE [P_MEETING] SET IsUsed=0 WHERE [Code]=@Code ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@Code", code)
                    });
                return entity;
            }
        }
        #endregion

        #region 后台管理
        public List<P_MEETING> LoadPage(int groupType, string srh_userId, string srh_userName, int rows, int page, out int total)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region 后台会议管理
        /// <summary>
        /// 后台会议管理
        /// </summary>
        /// <param name="CN"></param>
        /// <param name="MUDID"></param>
        /// <param name="SubmitTimeBegin"></param>
        /// <param name="SubmitTimeEnd"></param>
        /// <param name="ApprovedTimeBegin"></param>
        /// <param name="ApprovedTimeEnd"></param>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        private const string _sqlOrderMntPageSelect = "SELECT * FROM P_MEETING "
                                                   + "WHERE (Code LIKE '%' + @CN + '%') AND (UserId LIKE '%' + @MUDID + '%')"
                                                   + "AND ((@SubmitTimeBegin IS NULL) OR (SubmittedDate >= @SubmitTimeBegin)) AND ((@SubmitTimeEnd IS NULL) OR (SubmittedDate < @SubmitTimeEnd)) "
                                                   + "AND ((@ApprovedTimeBegin IS NULL) OR (ApprovedDate >= @ApprovedTimeBegin)) AND ((@ApprovedTimeEnd IS NULL) OR (ApprovedDate < @ApprovedTimeEnd)) ";
        private const string _sqlOrderMntPageOrderBy = "ORDER BY ApprovedDate DESC ";
        public List<P_MEETING> LoadMeeting(string CN, string MUDID, DateTime? SubmitTimeBegin, DateTime? SubmitTimeEnd, DateTime? ApprovedTimeBegin, DateTime? ApprovedTimeEnd, int rows, int page, out int total)
        {
            List<P_MEETING> rtnData;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.LoadPages<P_MEETING>(rows, page, out total,
                   _sqlOrderMntPageSelect,
                    _sqlOrderMntPageOrderBy, new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@CN", CN),
                        SqlParameterFactory.GetSqlParameter("@MUDID", MUDID),
                        SqlParameterFactory.GetSqlParameter("@SubmitTimeBegin", SubmitTimeBegin),
                        SqlParameterFactory.GetSqlParameter("@SubmitTimeEnd", SubmitTimeEnd),
                        SqlParameterFactory.GetSqlParameter("@ApprovedTimeBegin", ApprovedTimeBegin),
                        SqlParameterFactory.GetSqlParameter("@ApprovedTimeEnd", ApprovedTimeEnd)
                    });
            }
            return rtnData;
        }
        #endregion
    }
}
