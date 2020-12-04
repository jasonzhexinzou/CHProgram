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
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“PreApproval”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 PreApproval.svc 或 PreApproval.svc.cs，然后开始调试。
    public class PreApproval : IPreApproval
    {
        public IPreApprovalService preApprovalService = Global.applicationContext.GetBean("preApprovalService") as IPreApprovalService;

        public List<P_PreApproval> LoadHTCode(string UserID)
        {
            return preApprovalService.LoadHTCode(UserID);
        }

        public P_PreApproval LoadPreApprovalInfo(Guid id)
        {
            return preApprovalService.LoadPreApprovalInfo(id);
        }

        public int BUHeadApprove(Guid id, int state, string reason)
        {
            return preApprovalService.BUHeadApprove(id,state,reason);
        }
        public int BUHeadReject(Guid id, int state, string reason)
        {
            return preApprovalService.BUHeadReject(id, state, reason);
        }

        public int MMCoEApprove(Guid id, int state, string reason)
        {
            return preApprovalService.MMCoEApprove(id, state, reason);
        }
        public int MMCoEReject(Guid id, int state, string reason)
        {
            return preApprovalService.MMCoEReject(id, state, reason);
        }

        public int AddPreApprovalApproveHistory(P_PreApprovalApproveHistory PreApprovalHistory)
        {
            return preApprovalService.AddPreApprovalApproveHistory(PreApprovalHistory);
        }

        public List<PreApprovalState> LoadMyPreApprovalUserId(string UserId, DateTime Begin, DateTime End, string State,string Budget, int rows, int page, out int total)
        {
            return preApprovalService.LoadMyPreApprovalUserId(UserId, Begin, End, State,Budget, rows, page, out total);
        }

        public List<P_PreApproval> LoadMyApprove(string UserId, DateTime Begin, DateTime End, string State,string Applicant, int rows, int page, out int total)
        {
            return preApprovalService.LoadMyApprove(UserId, Begin, End, State, Applicant, rows, page, out total);
        }
        public List<P_PreApproval> LoadCurrentApprove(string UserId, DateTime Begin, DateTime End, string State, string Applicant, int rows, int page, out int total)
        {
            return preApprovalService.LoadCurrentApprove(UserId, Begin, End, State, Applicant, rows, page, out total);
        }
        public List<P_PreApprovalApproveHistory> LoadApprovalRecords(Guid PID)
        {
            return preApprovalService.LoadApprovalRecords(PID);
        }
        public List<P_PreApproval> LoadMyApproveAll(string UserId, string Applicant)
        {
            return preApprovalService.LoadMyApproveAll(UserId, Applicant);
        }

        public List<P_PreApproval> FindPreApprovalByHTCode(string HTCode)
        {
            return preApprovalService.FindPreApprovalByHTCode(HTCode);
        }

        public bool HasApproveRights(string UserId)
        {
            return preApprovalService.HasApproveRights(UserId);
        }
        public bool HasApprove(string UserId)
        {
            return preApprovalService.HasApprove(UserId);
        }
        public bool HasApproveByTA(string UserId, string TA)
        {
            return preApprovalService.HasApproveByTA(UserId, TA);
        }
        public bool HasFileApproveRights(string UserId)
        {
            return preApprovalService.HasFileApproveRights(UserId);
        }

        public List<P_PreApprovalApproveHistory> FindPreApprovalApproveHistory(Guid PID)
        {
            return preApprovalService.FindPreApprovalApproveHistory(PID);
        }

        public P_PreApprovalApproveHistory LoadApproveHistoryInfo(Guid PID,int Type)
        {
            return preApprovalService.LoadApproveHistoryInfo(PID,Type);
        }
        public P_PreApprovalApproveHistory LoadApproveHistory(Guid PID, int Type)
        {
            return preApprovalService.LoadApproveHistory(PID, Type);
        }
        public P_PreApprovalApproveHistory LoadApproveHistoryRefused(Guid PID, int Type, string Refused)
        {
            return preApprovalService.LoadApproveHistoryRefused(PID, Type, Refused);
        }

        public P_ORDER FindActivityOrderByHTCode(string HTCode)
        {
            return preApprovalService.FindActivityOrderByHTCode(HTCode);
        }
        public P_PreApproval CheckPreApprovalState(string HTCode)
        {
            return preApprovalService.CheckPreApprovalState(HTCode);
        }

        public HTCode GetHTCodeByID(string htcodeId)
        {
            return preApprovalService.GetHTCodeByID(htcodeId);
        }

        #region 更新预申请医院地址
        /// <summary>
        /// 更新预申请医院地址
        /// </summary>
        /// <param name="preApprovalId"></param>
        /// <param name="hospitalAddress"></param>
        /// <returns></returns>
        public int UpdateAddress(string preApprovalId, string hospitalAddress)
        {
            return preApprovalService.UpdateAddress(preApprovalId, hospitalAddress);
        }
        #endregion

        #region 新增地址
        public List<P_AddressApproval> LoadMyAddressApprovalByUserId(string UserId, DateTime Begin, DateTime End, string State, string Budget, int rows, int page, out int total)
        {
            return preApprovalService.LoadMyAddressApprovalByUserId(UserId, Begin, End, State, Budget, rows, page, out total);
        }

        public List<P_AddressApproval> LoadMyAddressApprove(string UserId, DateTime Begin, DateTime End, string State, string Applicant, int rows, int page, out int total)
        {
            return preApprovalService.LoadMyAddressApprove(UserId, Begin, End, State, Applicant, rows, page, out total);
        }

        public P_AddressApproval_View LoadAddressApprovalInfo(Guid id)
        {
            return preApprovalService.LoadAddressApprovalInfo(id);
        }

        public P_AddressApproval_View LoadAddressApprovalInfoForUpdate(Guid id)
        {
            return preApprovalService.LoadAddressApprovalInfoForUpdate(id);
        }

        public int AddressApprove(P_AddressApproval_View p_addressApproval_View, string reason)
        {
            return preApprovalService.AddressApprove(p_addressApproval_View, reason);
        }

        public int AddAddressApproveHistory(P_AddressApproveHistory p_addressApproveHistory)
        {
            return preApprovalService.AddAddressApproveHistory(p_addressApproveHistory);
        }

        public List<P_AddressApproval_View> LoadMyAddressApproveAll(string UserId, string applicant)
        {
            return preApprovalService.LoadMyAddressApproveAll(UserId, applicant);
        }

        public List<P_AddressApproval> LoadAddressApprovalByDACode(string dA_CODE)
        {
            return preApprovalService.LoadAddressApprovalByDACode(dA_CODE);
        }

        public List<P_AddressApproval> LoadMyAddressApproveCount(string UserId, DateTime Begin, DateTime End, string State, string Applicant)
        {
            return preApprovalService.LoadMyAddressApproveCount(UserId, Begin, End, State, Applicant);
        }
        #endregion

        #region 取消预申请
        public int PreApprovalCancel(P_PreApproval p_preApproval)
        {
            return preApprovalService.PreApprovalCancel(p_preApproval);
        }
        #endregion


        #region 费用分析-向上预申请分析
        public List<P_PreApproval_OwnBelongCountAmount> LoadPreApprovalUpData(string userId, string position, string territoryCode, string begin, string end)
        {
            return preApprovalService.LoadPreApprovalUpData(userId, position, territoryCode, begin, end);
        }
        #endregion
        #region 费用分析-预申请分析
        public List<P_PreApproval_CountAmount_View> LoadPreApprovalData(string userId, string position, string territoryCode, string begin, string end)
        {
            return preApprovalService.LoadPreApprovalData(userId, position, territoryCode, begin, end);
        }
        #endregion

        public V_COST_SUMMARY GetPreApprovalList(List<string> TerritoryStr, string StartDate, string EndDate)
        {
            return preApprovalService.GetPreApprovalList(TerritoryStr, StartDate, EndDate);
        }

    }
}
