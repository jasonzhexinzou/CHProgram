using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using MealAdmin.Entity;
using MealAdmin.Service;

namespace MealAdminApi
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“UserInfo”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 UserInfo.svc 或 UserInfo.svc.cs，然后开始调试。
    public class UserInfo : IUserInfo
    {
        public IUserInfoService userInfoService = Global.applicationContext.GetBean("userInfoService") as IUserInfoService;

        public int Add(P_USERINFO entity)
        {
            return userInfoService.Add(entity);
        }

        public int Edit(string userId, string phoneNumber, string market, string tacode)
        {
            return userInfoService.Edit(userId, phoneNumber, market, tacode);
        }

        public WP_QYUSER Find(string userId)
        {
            return userInfoService.Find(userId);
        }
        public P_Count isHaveApproval(string userId)
        {
            return userInfoService.isHaveApproval(userId);
        }

        public P_Count isSecondApproval(string userId)
        {
            return userInfoService.isSecondApproval(userId);
        }

        public P_USERINFO FindByUserId(string userId)
        {
            return userInfoService.FindByUserId(userId);
        }

        public P_USERINFO FindTAByUserId(string userId)
        {
            return userInfoService.FindTAByUserId(userId);
        }

        #region 记忆用户选择了同意协议
        /// <summary>
        /// 记忆用户选择了同意协议
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int CheckedStatement(string userId)
        {
            return userInfoService.CheckedStatement(userId);
        }
        #endregion

        #region 保存代理人
        public int SaveAgent(Guid ID, string userId, string userName, string delegateUserMUDID, string delegateUserName, DateTime startTime, DateTime endTime, int isEnable, string OperatorMUDID)
        {
            return userInfoService.SaveAgent(ID, userId, userName, delegateUserMUDID, delegateUserName, startTime, endTime, isEnable, OperatorMUDID);
        }

        public int SaveSecondAgent(Guid ID, string userId, string userName, string delegateUserMUDID, string delegateUserName, DateTime startTime, DateTime endTime, int isEnable, string OperatorMUDID)
        {
            return userInfoService.SaveSecondAgent(ID, userId, userName, delegateUserMUDID, delegateUserName, startTime, endTime, isEnable, OperatorMUDID);
        }
        #endregion

        #region 判断是否是代理人
        public P_UserDelegate isAgent(string userId)
        {
            return userInfoService.isAgent(userId);
        }

        public P_UserDelegate isAgentBack(string userId)
        {
            return userInfoService.isAgentBack(userId);
        }
        #endregion

        #region 判断是否是二线代理人
        public P_UserDelegatePre isSecondAgent(string userId)
        {
            return userInfoService.isSecondAgent(userId);
        }

        public P_UserDelegatePre isSecondAgentBack(string userId)
        {
            return userInfoService.isSecondAgentBack(userId);
        }
        #endregion


        #region 代理人信息
        public P_UserDelegate AgentInfo(string userId)
        {
            return userInfoService.AgentInfo(userId);
        }
        #endregion

        #region 查询代理人信息是否存在
        public P_UserDelegate AgentExist(Guid DelegateID)
        {
            return userInfoService.AgentExist(DelegateID);
        }
        #endregion

        #region  修改代理人
        public int UpdateAgent(Guid DelegateID, string userId, string userName, string delegateUserMUDID, string delegateUserName, DateTime startTime, DateTime endTime, int isEnable, string OperatorMUDID)
        {
            return userInfoService.UpdateAgent(DelegateID, userId, userName, delegateUserMUDID, delegateUserName, startTime, endTime, isEnable, OperatorMUDID);
        }

        public int UpdateSecondAgent(Guid DelegateID, string userId, string userName, string delegateUserMUDID, string delegateUserName, DateTime startTime, DateTime endTime, int isEnable, string OperatorMUDID)
        {
            return userInfoService.UpdateSecondAgent(DelegateID, userId, userName, delegateUserMUDID, delegateUserName, startTime, endTime, isEnable, OperatorMUDID);
        }
        #endregion

        #region 判断用户角色是否存在
        public int CheckUserRole(string Role, string UserID, string Market)
        {
            return userInfoService.CheckUserRole(Role, UserID, Market);
        }
        #endregion

    }
}
