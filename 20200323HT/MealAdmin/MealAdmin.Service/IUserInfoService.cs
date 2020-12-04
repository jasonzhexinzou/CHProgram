using MealAdmin.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Service
{
    public interface IUserInfoService
    {
        P_USERINFO FindByUserId(string userId);
        P_USERINFO FindTAByUserId(string userId);
        int Add(P_USERINFO entity);
        int Edit(string userId, string phoneNumber, string market, string tacode);
        WP_QYUSER Find(string userId);
        #region 判断是否有审批权限 
        P_Count isHaveApproval(string userId);
        #endregion

        #region 判断是否是二线经理
        P_Count isSecondApproval(string userId);
        #endregion

        #region 用户接收协议
        /// <summary>
        /// 用户接收协议
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        int CheckedStatement(string userId);
        #endregion

        #region 后台人员管理-list
        List<P_USERINFO> LoadPage(string userId, string DMUserId, int rows, int page, out int total);
        #endregion

        #region 保存代理人
        int SaveAgent(Guid ID, string userId, string userName, string delegateUserMUDID, string delegateUserName, DateTime startTime, DateTime endTime, int isEnable, string OperatorMUDID);

        int SaveSecondAgent(Guid ID, string userId, string userName, string delegateUserMUDID, string delegateUserName, DateTime startTime, DateTime endTime, int isEnable, string OperatorMUDID);
        #endregion

        #region 判断是否是代理人
        P_UserDelegate isAgent(string userId);
        P_UserDelegate isAgentBack(string userId);
        #endregion

        #region 判断是否是二线代理人
        P_UserDelegatePre isSecondAgent(string userId);
        P_UserDelegatePre isSecondAgentBack(string userId);
        #endregion

        #region 代理人信息
        P_UserDelegate AgentInfo(string userId);
        #endregion

        #region 查询代理人信息是否存在
        P_UserDelegate AgentExist(Guid DelegateID);
        #endregion

        #region  修改代理人
        int UpdateAgent(Guid DelegateID, string userId, string userName, string delegateUserMUDID, string delegateUserName, DateTime startTime, DateTime endTime, int isEnable, string OperatorMUDID);

        int UpdateSecondAgent(Guid DelegateID, string userId, string userName, string delegateUserMUDID, string delegateUserName, DateTime startTime, DateTime endTime, int isEnable, string OperatorMUDID);
        #endregion

        #region 审批人代理查询
        List<P_UserDelegate> ApproverAgentLoad(string ApprovalNameOrMUDID, string AgentNameOrMUDID, int rows, int page, out int total);

        List<P_UserDelegatePre> ApproverSecondAgentLoad(string ApprovalNameOrMUDID, string AgentNameOrMUDID, int rows, int page, out int total);
        #endregion

        #region 同步WD人员数据
        void SyncWorkDayUserInfo(List<WP_QYUSER> UserList);
        #endregion

        #region 抓取已离职人员的未完成订单、上传文件
        List<P_AutoTransferView> LoadLeaveUserInfo();
        LineManager FindUserManagerInfo(Guid LineManagerId);
        LineManager FindUserManagerInfo(string UserId);

        List<LineManagerUser> FindUserManagerInfo();
        #endregion

        #region 代理人历史查询
        List<P_UserDelegatePreHis> ApproverSecondAgentHisLoad(string UserId);

        List<P_UserDelegateHis> ApproverAgentHisLoad(string UserId);
        #endregion

        List<WP_QYUSER> FindUserByLineManager(Guid LineManagerId);

        int DeleteAgent(Guid ID);

        int DeleteSecondAgent(Guid ID);

        P_UserDelegate FindById(Guid ID);

        P_UserDelegatePre FindSecondById(Guid ID);

        #region 判断用户角色是否存在
        int CheckUserRole(string Role, string UserID, string Market);
        #endregion
    }
}
