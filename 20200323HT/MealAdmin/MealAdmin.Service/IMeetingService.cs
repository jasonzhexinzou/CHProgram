using MealAdmin.Entity;
using MealAdmin.Entity.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Service
{
    public interface IMeetingService
    {
        List<P_MEETING> LoadByUserId(string userId,string approvedDate);
        P_MEETING_VIEW FindByCode(string code);

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
        List<P_MEETING> LoadMeeting(string CN, string MUDID, DateTime? SubmitTimeBegin, DateTime? SubmitTimeEnd, DateTime? ApprovedTimeBegin, DateTime? ApprovedTimeEnd, int rows, int page, out int total);
        #endregion
    }
}
