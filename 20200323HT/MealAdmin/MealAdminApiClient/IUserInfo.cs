using MealAdmin.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MealAdminApi
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IUserInfo”。
    [ServiceContract]
    public interface IUserInfo
    {
        [OperationContract]
        P_USERINFO FindByUserId(string userId);

        [OperationContract]
        P_USERINFO FindTAByUserId(string userId);

        [OperationContract]
        int Add(P_USERINFO entity);

        [OperationContract]
        int Edit(string userId, string phoneNumber, string market, string tacode);

        [OperationContract]
        WP_QYUSER Find(string userId);

        #region 判断是否有审批权限 
        [OperationContract]
        P_Count isHaveApproval(string userId);

        [OperationContract]
        P_Count isSecondApproval(string userId);
        #endregion

        [OperationContract]
        int CheckedStatement(string userId);

        #region 保存代理人
        [OperationContract]
        int SaveAgent(Guid ID, string userId, string userName, string delegateUserMUDID, string delegateUserName, DateTime startTime, DateTime endTime, int isEnable, string OperatorMUDID);

        [OperationContract]
        int SaveSecondAgent(Guid ID, string userId, string userName, string delegateUserMUDID, string delegateUserName, DateTime startTime, DateTime endTime, int isEnable, string OperatorMUDID);
        #endregion

        #region 判断是否是代理人
        [OperationContract]
        P_UserDelegate isAgent(string userId);

        [OperationContract]
        P_UserDelegate isAgentBack(string userId);
        #endregion

        #region 判断是否是代理人
        [OperationContract]
        P_UserDelegatePre isSecondAgent(string userId);

        [OperationContract]
        P_UserDelegatePre isSecondAgentBack(string userId);
        #endregion

        #region 代理人信息
        [OperationContract]
        P_UserDelegate AgentInfo(string userId);
        #endregion

        #region 查询代理人信息是否存在
        [OperationContract]
        P_UserDelegate AgentExist(Guid DelegateID);
        #endregion

        #region  修改代理人
        [OperationContract]
        int UpdateAgent(Guid DelegateID, string userId, string userName, string delegateUserMUDID, string delegateUserName, DateTime startTime, DateTime endTime, int isEnable, string OperatorMUDID);

        [OperationContract]
        int UpdateSecondAgent(Guid DelegateID, string userId, string userName, string delegateUserMUDID, string delegateUserName, DateTime startTime, DateTime endTime, int isEnable, string OperatorMUDID);
        #endregion

        #region 判断用户角色是否存在
        [OperationContract]
        int CheckUserRole(string Role, string UserID, string Market);
        #endregion
    }
}
