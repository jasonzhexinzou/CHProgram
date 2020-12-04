using MealAdmin.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Dao
{
    public interface IGroupMemberDao
    {
        List<P_GROUP_MEMBER> LoadPage(int groupType, string srh_userId, string srh_userName, int rows, int page, out int total);
        List<P_GROUP_MEMBER> LoadNonHTPage(int groupType, string srh_userId, string srh_userName, int rows, int page, out int total);
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
        int AddNonHT(List<string> userIds, int groupType, DateTime createDate, string createUserId, int state, out List<string> unSuccesUserId);
        int DelByMemberID(Guid MemberID);
        int DelNonHTByGroupType(int GroupType);
        int DelNonHTByMemberID(Guid MemberID);
        int DelByGroupType(int GroupType);
        //导出组别管理
        List<P_GROUP_MEMBER> ExportGroupList(string MUDID, string Name, int GroupType);
        List<P_GROUP_MEMBER> ExportNonHTGroupList(string MUDID, string Name, int GroupType);
        List<P_ServPause_Detail> ExportServPauseGroupList(string MUDID, string Name, int GroupType);

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

        #region ****************消息推送相关********************

        #region 解除暂停服务状态（未收餐）
        /// <summary>
        /// 解除暂停服务状态
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        int DelByState1(string userId);
        #endregion

        #region 解除暂停服务状态（未上传文件）
        /// <summary>
        /// 解除暂停服务状态
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        int DelByState2(string userId);
        #endregion

        #region 解除暂停服务状态（未完成审批）
        /// <summary>
        /// 解除暂停服务状态
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        int DelByState3(string userId);
        #endregion

        P_GROUP_MEMBER FindByUser(string userId);

        #region 更新暂停服务状态
        int UpdateState1(string userId);
        int UpdateState2(string userId);
        int UpdateState3(string userId);
        #endregion

        int AddUser(List<string> userIds, int state);
        int AddServPauseDetail(string UserId, string UserName,string HTCode, int ServPauseType,string Memo);
        int UpdateServPauseDetail(string HTCode,int ServPauseType);
        #endregion

    }
}
