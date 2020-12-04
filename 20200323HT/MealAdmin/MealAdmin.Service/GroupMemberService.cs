using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MealAdmin.Entity;
using XFramework.XInject.Attributes;
using MealAdmin.Dao;

namespace MealAdmin.Service
{
    public class GroupMemberService : IGroupMemberService
    {

        [Bean("groupMemberDao")]
        public IGroupMemberDao groupMemberDao { get; set; }

        public int Add(List<string> userIds, int groupType, DateTime createDate, string createUserId, int state, out List<string> unSuccesUserId)
        {
            if (groupType == 6)
            {
                return groupMemberDao.AddNonHT(userIds, groupType, createDate, createUserId, state, out unSuccesUserId);
            }
            else
            {
                return groupMemberDao.Add(userIds, groupType, createDate, createUserId, state, out unSuccesUserId);
            }
        }

        public int DelByGroupType(int GroupType)
        {
            if (GroupType == 6)
            {
                return groupMemberDao.DelNonHTByGroupType(GroupType);

            }
            else
            {
                return groupMemberDao.DelByGroupType(GroupType);

            }
        }

        public int DelByMemberID(Guid MemberID,int GroupType)
        {
            if (GroupType == 6)
            {
                return groupMemberDao.DelNonHTByMemberID(MemberID);
            }
            else
            {
                return groupMemberDao.DelByMemberID(MemberID);
            }
        }

        public List<P_GROUP_MEMBER> Load(int groupType, string srh_userId, string srh_userName)
        {
            return groupMemberDao.Load(groupType, srh_userId, srh_userName);
        }

        public List<P_GROUP_MEMBER> LoadPage(int groupType, string srh_userId, string srh_userName, int rows, int page, out int total)
        {
            if (groupType == 6)
            {
                return groupMemberDao.LoadNonHTPage(groupType, srh_userId, srh_userName, rows, page, out total);
            }
            else
            {
                return groupMemberDao.LoadPage(groupType, srh_userId, srh_userName, rows, page, out total);
            }
        }


        public List<P_GROUP_MEMBER> GetGroupMembersByType(GroupTypeEnum GroupType)
        {
            return groupMemberDao.GetGroupMembersByType(GroupType);
        }

        #region 查找用户所在的组
        /// <summary>
        /// 查找用户所在的组
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<P_GROUP_MEMBER> LoadUserGroup(string userId)
        {
            return groupMemberDao.LoadUserGroup(userId);
        }
        #endregion

        #region 是否项目组成员
        /// <summary>
        /// 是否项目组成员
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsDevGroup(string userId)
        {
            return groupMemberDao.IsDevGroup(userId);
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
            return groupMemberDao.AddUser(userIds,state);
        }
        #endregion

        #region 导出组别管理
        public List<P_GROUP_MEMBER> ExportGroupList(string MUDID, string Name, int GroupType)
        {
            if (GroupType == 6)
            {
                return groupMemberDao.ExportNonHTGroupList(MUDID, Name, GroupType);
            }
            else
            {
                return groupMemberDao.ExportGroupList(MUDID, Name, GroupType);
            }
        }

        public List<P_ServPause_Detail> ExportServPauseGroupList(string MUDID, string Name, int GroupType)
        {
            return groupMemberDao.ExportServPauseGroupList(MUDID, Name, GroupType);
        }
        #endregion

        public int AddServPauseDetail(string UserId, string UserName,string HTCode, int ServPauseType,string Memo)
        {
            return groupMemberDao.AddServPauseDetail(UserId, UserName, HTCode, ServPauseType, Memo);
        }

        public int UpdateServPauseDetail(string HTCode, int ServPauseType)
        {
            return groupMemberDao.UpdateServPauseDetail(HTCode, ServPauseType);
        }
    }
}
