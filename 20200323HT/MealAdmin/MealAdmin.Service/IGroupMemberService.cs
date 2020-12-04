using MealAdmin.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Service
{
    public interface IGroupMemberService
    {
        List<P_GROUP_MEMBER> LoadPage(int groupType, string srh_userId, string srh_userName, int rows, int page, out int total);
        List<P_GROUP_MEMBER> Load(int groupType, string srh_userId, string srh_userName);

        List<P_GROUP_MEMBER> GetGroupMembersByType(GroupTypeEnum GroupType);

        /// <summary>
        /// 批量导入用户
        /// </summary>
        /// <param name="userIds"></param>
        /// <param name="groupType"></param>
        /// <param name="createDate"></param>
        /// <param name="createUserId"></param>
        /// <param name="unSuccesUserId"></param>
        /// <returns></returns>
        int Add(List<string> userIds, int groupType, DateTime createDate, string createUserId, int state, out List<string> unSuccesUserId);

        int DelByMemberID(Guid MemberID,int GroupType);
        int DelByGroupType(int GroupType);

        #region 查找用户所在的组
        /// <summary>
        /// 查找用户所在的组
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<P_GROUP_MEMBER> LoadUserGroup(string userId);
        #endregion


        #region 是否项目组成员
        /// <summary>
        /// 是否项目组成员
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool IsDevGroup(string userId);
        #endregion



        int AddUser(List<string> userIds, int state);
        //导出组别管理
        List<P_GROUP_MEMBER> ExportGroupList(string MUDID, string Name, int GroupType);
        List<P_ServPause_Detail> ExportServPauseGroupList(string MUDID, string Name, int GroupType);
        //添加暂停服务明细
        int AddServPauseDetail(string UserId, string UserName,string HTCode, int ServPauseType,string Memo);
        int UpdateServPauseDetail(string HTCode, int ServPauseType);
    }
}
