using MealAdmin.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MealAdminApi
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IPreApproval”。
    [ServiceContract]
    public interface IPreApproval
    {
        [OperationContract]
        P_PreApproval LoadPreApprovalInfo(Guid id);
        [OperationContract]
        int BUHeadApprove(Guid id, int state, string reason);
        [OperationContract]
        int BUHeadReject(Guid id, int state, string reason);
        [OperationContract]
        int MMCoEApprove(Guid id, int state, string reason);
        [OperationContract]
        int MMCoEReject(Guid id, int state, string reason);
        [OperationContract]
        int AddPreApprovalApproveHistory(P_PreApprovalApproveHistory PreApprovalHistory);
        [OperationContract]
        List<PreApprovalState> LoadMyPreApprovalUserId(string UserId, DateTime Begin, DateTime End, string State,string Budget, int rows, int page, out int total);
        [OperationContract]
        List<P_PreApproval> LoadMyApprove(string UserId, DateTime Begin, DateTime End, string State,string Applicant, int rows, int page, out int total);
        [OperationContract]
        List<P_PreApproval> LoadCurrentApprove(string UserId, DateTime Begin, DateTime End, string State, string Applicant, int rows, int page, out int total);
        [OperationContract]
        List<P_PreApproval> LoadMyApproveAll(string UserId, string Applicant);
        [OperationContract]
        List<P_PreApprovalApproveHistory> LoadApprovalRecords(Guid PID);
        [OperationContract]
        List<P_PreApproval> LoadHTCode(string UserID);
        [OperationContract]
        List<P_PreApproval> FindPreApprovalByHTCode(string HTCode);
        [OperationContract]
        bool HasApproveRights(string UserId);
        [OperationContract]
        bool HasApprove(string UserId);
        [OperationContract]
        bool HasApproveByTA(string UserId, string TA);
        [OperationContract]
        bool HasFileApproveRights(string UserId);
        [OperationContract]
        List<P_PreApprovalApproveHistory> FindPreApprovalApproveHistory(Guid PID);
        [OperationContract]
        P_PreApprovalApproveHistory LoadApproveHistoryInfo(Guid PID,int Type);
        [OperationContract]
        P_PreApprovalApproveHistory LoadApproveHistory(Guid PID, int Type);
       [OperationContract]
        P_PreApprovalApproveHistory LoadApproveHistoryRefused(Guid PID, int Type, string UserId);
        [OperationContract]
        P_ORDER FindActivityOrderByHTCode(string HTCode);
        [OperationContract]
        P_PreApproval CheckPreApprovalState(string HTCode);
        [OperationContract]
        HTCode GetHTCodeByID(string htcodeId);

        [OperationContract]
        int UpdateAddress(string preApprovalId, string hospitalAddress);

        #region 新增地址
        [OperationContract]
        List<P_AddressApproval> LoadMyAddressApprovalByUserId(string UserId, DateTime Begin, DateTime End, string State, string Budget, int rows, int page, out int total);

        [OperationContract]
        List<P_AddressApproval> LoadMyAddressApprove(string UserId, DateTime Begin, DateTime End, string State, string Applicant, int rows, int page, out int total);

        [OperationContract]
        P_AddressApproval_View LoadAddressApprovalInfo(Guid id);

        [OperationContract]
        P_AddressApproval_View LoadAddressApprovalInfoForUpdate(Guid id);

        [OperationContract]
        int AddressApprove(P_AddressApproval_View p_addressApproval_View, string reason);

        [OperationContract]
        int AddAddressApproveHistory(P_AddressApproveHistory p_addressApproveHistory);

        [OperationContract]
        List<P_AddressApproval_View> LoadMyAddressApproveAll(string UserId, string applicant);

        [OperationContract]
        List<P_AddressApproval> LoadAddressApprovalByDACode(string dA_CODE);

        [OperationContract]
        List<P_AddressApproval> LoadMyAddressApproveCount(string UserId, DateTime Begin, DateTime End, string State, string Applicant);

        #endregion

        #region 取消预申请
        [OperationContract]
        int PreApprovalCancel(P_PreApproval p_preApproval);
        #endregion


        [OperationContract]
        List<P_PreApproval_OwnBelongCountAmount> LoadPreApprovalUpData(string userId, string position, string territoryCode, string begin, string end);
        [OperationContract]
        List<P_PreApproval_CountAmount_View> LoadPreApprovalData(string userId, string position, string territoryCode, string begin, string end);

        [OperationContract]
        V_COST_SUMMARY GetPreApprovalList(List<string> TerritoryStr, string StartDate, string EndDate);

    }
}
