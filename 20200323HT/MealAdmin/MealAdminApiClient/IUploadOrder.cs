using MealAdmin.Entity;
using MealAdmin.Entity.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MealAdminApi
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IUploadOrder”。
    [ServiceContract]
    public interface IUploadOrder
    {
        [OperationContract]
        List<P_ORDER> LoadHTCode(string userId);
        [OperationContract]
        P_ORDER FindOrderByHTCode(string HTCode);
        [OperationContract]
        int BUHeadApprove(Guid id, int state, string reason);
        [OperationContract]
        P_ORDER LoadOrderInfo(Guid id);
        [OperationContract]
        int AddOrderApproveHistory(P_OrderApproveHistory OrderlHistory);
        [OperationContract]
        int BUHeadReject(Guid id, int state, string reason);
        [OperationContract]
        int MMCoEReject(Guid id, int state, string reason);
        [OperationContract]
        int MMCoEApprove(Guid id, int state, string reason);
        [OperationContract]
        List<P_PreUploadOrderState> LoadMyOrderUserId(string UserId, DateTime Begin, DateTime End, string State, int rows, int page, out int total);
        [OperationContract]
        List<P_AutoTransferState> LoadMyAutoTransfer(string UserId,DateTime End, int rows, int page, out int total);
        [OperationContract]
        List<P_PreUploadOrderState> LoadMyApprove(string UserId, DateTime Begin, DateTime End, string State, string Applicant, int rows, int page, out int total);
        [OperationContract]
        List<P_PREUPLOADORDER> LoadMyApproveAll(string UserId, string Applicant);
        [OperationContract]
        P_PREUPLOADORDER FindActivityOrderByHTCode(string HTCode);
        [OperationContract]
        P_OrderApproveHistory LoadApproveHistoryInfo(Guid PID);
        [OperationContract]
        List<P_OrderApproveHistory> FindorderApproveHistory(Guid PID);
        [OperationContract]
        int Add(P_PREUPLOADORDER entity);
        [OperationContract]
        int Update(P_PREUPLOADORDER entity);
        [OperationContract]
        WP_QYUSER FindApproveInfo(string userId);
        [OperationContract]
        P_PREUPLOADORDER LoadPreUploadOrder(Guid id);


        
    }
}
