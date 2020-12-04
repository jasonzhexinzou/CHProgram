using MealAdmin.Entity;
using MealAdmin.Entity.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Dao
{
    public interface IMeetingDao
    {
        #region 找到可用(未订餐)的会议
        /// <summary>
        /// 找到可用(未订餐)的会议
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<P_MEETING> LoadByUserId(string userId,string approvedDate);
        #endregion

        #region 根据CN号查找会议信息
        /// <summary>
        /// 根据CN号查找会议信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        P_MEETING_VIEW FindByCode(string code);
        #endregion

        #region 使用CN号
        /// <summary>
        /// 使用CN号
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        int UsedCN(string code);
        #endregion

        #region 释放CN号
        /// <summary>
        /// 释放CN号
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        int ReleaseCN(string code);
        #endregion

        #region 后台管理
        /// <summary>
        /// 后台管理
        /// </summary>
        /// <param name="groupType"></param>
        /// <param name="srh_userId"></param>
        /// <param name="srh_userName"></param>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        List<P_MEETING> LoadPage(int groupType, string srh_userId, string srh_userName, int rows, int page, out int total);
        #endregion

        #region 后台会议管理
        /// <summary>
        /// 后台会议管理
        /// </summary>
        /// <returns></returns>
        List<P_MEETING> LoadMeeting(string CN, string MUDID, DateTime? SubmitTimeBegin, DateTime? SubmitTimeEnd, DateTime? ApprovedTimeBegin, DateTime? ApprovedTimeEnd, int rows, int page, out int total);
        #endregion
    }
}
