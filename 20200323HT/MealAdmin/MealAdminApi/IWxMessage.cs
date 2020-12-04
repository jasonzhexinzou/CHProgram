using MealAdmin.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MealAdminApi
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IWxMessage”。
    [ServiceContract]
    public interface IWxMessage
    {
        [OperationContract]
        void SendWxMessageByOrder(Guid ID);

        [OperationContract]
        string SendText(string toUsers, string text);

        [OperationContract]
        void SendMessageToUserForChangeFee(string xmsOrderId);

        [OperationContract]
        void SendPreApprovalStateChangeMessageToUser(P_PreApproval preApproval);

        [OperationContract]
        void SendApproveStepErrorMessageToGroup(P_PreApproval preApproval);

        [OperationContract]
        void SendPreApprovalRejectMessageToUser(string Comments, P_PreApproval preApproval);

        [OperationContract]
        void SendOrderStateChangeMessageToUser(P_PREUPLOADORDER order);
        [OperationContract]
        void SendOrderRejectMessageToUser(string Comments, P_PREUPLOADORDER order);

        [OperationContract]
        void SendMessageForConfirm(List<string> listTouser, int state);

        [OperationContract]
        void SendMessageForAutoTransfer(List<string> ListTouser, string HTCodeList, string UserName, string UserId, int HTCount, int Type);

        [OperationContract]
        void SendMessageForAutoTransferPre(List<string> ListTouser, string HTCodeList, string UserName, string UserId, int HTCount);

        [OperationContract]
        void SendMessageToUser(string userId, P_ORDER order);

        [OperationContract]
        void SendAddressApprovalStateChangeMessageToUser(P_AddressApproval addressApproval);

        [OperationContract]
        void SendSpecialMessageToUser(P_AddressApproval addressApproval);

        [OperationContract]
        void SendOrderErrorToUser(string HTCode, string Touser);

        [OperationContract]
        void SendCoverChangeToUser(string HospitalCode, string HospitalName, string Address, string Touser, int Type, string ResId, string ResName);

        [OperationContract]
        void SendOrderErrorMsgToUser(Guid id, string HTCode, string Touser, string remark, int type);
    }
}
