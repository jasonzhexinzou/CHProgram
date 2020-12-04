using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MealAdmin.Entity;
using XFramework.XInject.Attributes;
using MealAdmin.Dao;
using MealAdmin.Entity.View;

namespace MealAdmin.Service
{
    public class MeetingService : IMeetingService
    {
        [Bean("meetingDao")]
        public IMeetingDao meetingDao { get; set; }

        #region 找到可用(未订餐)的会议
        /// <summary>
        /// 找到可用(未订餐)的会议
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<P_MEETING> LoadByUserId(string userId,string approvedDate)
        {
            return meetingDao.LoadByUserId(userId, approvedDate);
        }
        #endregion

        #region 根据code查找会议信息
        /// <summary>
        /// 根据code查找会议信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public P_MEETING_VIEW FindByCode(string code)
        {
            return meetingDao.FindByCode(code);
        }
        #endregion

        #region 后台会议管理
        /// <summary>
        /// 后台会议列表
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
        public List<P_MEETING> LoadMeeting(string CN, string MUDID, DateTime? SubmitTimeBegin, DateTime? SubmitTimeEnd, DateTime? ApprovedTimeBegin, DateTime? ApprovedTimeEnd, int rows, int page, out int total)
        {
            return meetingDao.LoadMeeting(CN, MUDID, SubmitTimeBegin, SubmitTimeEnd, ApprovedTimeBegin, ApprovedTimeEnd, rows, page, out total);
        }
        #endregion
    }
}
