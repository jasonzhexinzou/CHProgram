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
    public class UserInfoService : IUserInfoService
    {
        [Bean("userInfoDao")]
        public IUserInfoDao userInfoDao { get; set; }

        public int Add(P_USERINFO entity)
        {
            return userInfoDao.Add(entity);
        }

        public int Edit(string userId, string phoneNumber, string market, string tacode)
        {
            return userInfoDao.Edit(userId, phoneNumber, market, tacode);
        }

        public WP_QYUSER Find(string userId)
        {
            return userInfoDao.Find(userId);
        }

        //判断是否有审批权限
        public P_Count isHaveApproval(string userId)
        {
            return userInfoDao.isHaveApproval(userId);
        }

        public P_Count isSecondApproval(string userId)
        {
            return userInfoDao.isSecondApproval(userId);
        }

        public P_USERINFO FindByUserId(string userId)
        {
            return userInfoDao.FindByUserId(userId);
        }

        public P_USERINFO FindTAByUserId(string userId)
        {
            return userInfoDao.FindTAByUserId(userId);
        }

        #region 用户接收协议
        /// <summary>
        /// 用户接收协议
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int CheckedStatement(string userId)
        {
            return userInfoDao.CheckedStatement(userId);
        }
        #endregion

        #region 后台人员管理-list
        public List<P_USERINFO> LoadPage(string userId, string DMUserId, int rows, int page, out int total)
        {
            return userInfoDao.LoadPage(userId, DMUserId, rows, page, out total);
        }
        #endregion

        #region 保存代理人
        public int SaveAgent(Guid ID, string userId, string userName, string delegateUserMUDID, string delegateUserName, DateTime startTime, DateTime endTime, int isEnable, string OperatorMUDID)
        {
            return userInfoDao.SaveAgent(ID, userId, userName, delegateUserMUDID, delegateUserName, startTime, endTime, isEnable, OperatorMUDID);
        }

        public int SaveSecondAgent(Guid ID, string userId, string userName, string delegateUserMUDID, string delegateUserName, DateTime startTime, DateTime endTime, int isEnable, string OperatorMUDID)
        {
            return userInfoDao.SaveSecondAgent(ID, userId, userName, delegateUserMUDID, delegateUserName, startTime, endTime, isEnable, OperatorMUDID);
        }
        #endregion

        #region 判断是否是代理人
        public P_UserDelegate isAgent(string userId)
        {
            return userInfoDao.isAgent(userId);
        }
        public P_UserDelegate isAgentBack(string userId)
        {
            return userInfoDao.isAgentBack(userId);
        }
        #endregion

        #region 判断是否是代理人
        public P_UserDelegatePre isSecondAgent(string userId)
        {
            return userInfoDao.isSecondAgent(userId);
        }

        public P_UserDelegatePre isSecondAgentBack(string userId)
        {
            return userInfoDao.isSecondAgentBack(userId);
        }
        #endregion

        #region 代理人信息
        public P_UserDelegate AgentInfo(string userId)
        {
            return userInfoDao.isAgent(userId);
        }
        #endregion

        #region 查询代理人信息是否存在
        public P_UserDelegate AgentExist(Guid DelegateID)
        {
            return userInfoDao.AgentExist(DelegateID);
        }
        #endregion

        #region  修改代理人
        public int UpdateAgent(Guid DelegateID, string userId, string userName, string delegateUserMUDID, string delegateUserName, DateTime startTime, DateTime endTime, int isEnable, string OperatorMUDID)
        {
            return userInfoDao.UpdateAgent(DelegateID, userId, userName, delegateUserMUDID, delegateUserName, startTime, endTime, isEnable, OperatorMUDID);
        }

        public int UpdateSecondAgent(Guid DelegateID, string userId, string userName, string delegateUserMUDID, string delegateUserName, DateTime startTime, DateTime endTime, int isEnable, string OperatorMUDID)
        {
            return userInfoDao.UpdateSecondAgent(DelegateID, userId, userName, delegateUserMUDID, delegateUserName, startTime, endTime, isEnable, OperatorMUDID);
        }
        #endregion

        #region 审批人代理查询
        public List<P_UserDelegate> ApproverAgentLoad(string ApprovalNameOrMUDID, string AgentNameOrMUDID, int rows, int page, out int total)
        {
            return userInfoDao.ApproverAgentLoad(ApprovalNameOrMUDID, AgentNameOrMUDID, rows, page, out total);

        }

        public List<P_UserDelegatePre> ApproverSecondAgentLoad(string ApprovalNameOrMUDID, string AgentNameOrMUDID, int rows, int page, out int total)
        {
            return userInfoDao.ApproverSecondAgentLoad(ApprovalNameOrMUDID, AgentNameOrMUDID, rows, page, out total);

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
            userInfoDao.SyncWorkDayUserInfo(UserList);
        }
        #endregion

        #region 抓取已离职人员的未完成订单、上传文件
        public List<P_AutoTransferView> LoadLeaveUserInfo()
        {
            return userInfoDao.LoadLeaveUserInfo();
        }
        public LineManager FindUserManagerInfo(Guid LineManagerId)
        {
            return userInfoDao.FindUserManagerInfo(LineManagerId);
        }
        public LineManager FindUserManagerInfo(string UserId)
        {
            return userInfoDao.FindUserManagerInfo(UserId);
        }

        public List<LineManagerUser> FindUserManagerInfo()
        {
            return userInfoDao.FindUserManagerInfo();
        }
        #endregion

        #region 代理人历史查询
        public List<P_UserDelegatePreHis> ApproverSecondAgentHisLoad(string UserId)
        {
            return userInfoDao.ApproverSecondAgentHisLoad(UserId);
        }

        public List<P_UserDelegateHis> ApproverAgentHisLoad(string UserId)
        {
            return userInfoDao.ApproverAgentHisLoad(UserId);
        }
        #endregion

        public List<WP_QYUSER> FindUserByLineManager(Guid LineManagerId)
        {
            return userInfoDao.FindUserByLineManager(LineManagerId);
        }

        public int DeleteAgent(Guid ID)
        {
            return userInfoDao.DeleteAgent(ID);
        }

        public int DeleteSecondAgent(Guid ID)
        {
            return userInfoDao.DeleteSecondAgent(ID);
        }

        public P_UserDelegate FindById(Guid ID)
        {
            return userInfoDao.FindById(ID);
        }

        public P_UserDelegatePre FindSecondById(Guid ID)
        {
            return userInfoDao.FindSecondById(ID);
        }

        #region 判断用户角色是否存在
        public int CheckUserRole(string Role, string UserID, string Market)
        {
            return userInfoDao.CheckUserRole(Role, UserID, Market);
        }
        #endregion
    }
}
