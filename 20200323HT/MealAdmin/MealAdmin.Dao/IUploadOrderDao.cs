using MealAdmin.Entity;
using MealAdmin.Entity.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Dao
{
    public interface IUploadOrderDao
    {
        #region 加载HT编号
        // <summary>
        /// 加载HT编号
        /// </summary>
        /// <returns></returns>
        List<P_ORDER> LoadHTCode(string userId);
        #endregion

        #region 根据订单号查询订单详情
        /// <summary>
        /// 根据订单号查询订单详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        P_ORDER FindOrderByHTCode(string HTCode);
        #endregion

        int BUHeadApprove(Guid id, int state, string reason);
        P_ORDER LoadOrderInfo(Guid id);
        int AddOrderApproveHistory(P_OrderApproveHistory OrderlHistory);
        int BUHeadReject(Guid id, int state, string reason);
        int MMCoEReject(Guid id, int state, string reason);
        int MMCoEApprove(Guid id, int state, string reason);
        List<P_PreUploadOrderState> LoadMyOrderUserId(string UserId, DateTime Begin, DateTime End, string State, int rows, int page, out int total);
        List<P_AutoTransferState> LoadMyAutoTransfer(string UserId,DateTime End, int rows, int page, out int total);
        List<P_PreUploadOrderState> LoadMyApprove(string UserId, DateTime Begin, DateTime End, string State, string Applicant, int rows, int page, out int total);
        List<P_PREUPLOADORDER> LoadMyApproveAll(string UserId, string Applicant);
        P_PREUPLOADORDER FindActivityOrderByHTCode(string HTCode);
        P_OrderApproveHistory LoadApproveHistoryInfo(Guid PID);
        List<P_OrderApproveHistory> FindorderApproveHistory(Guid PID);
        int Add(P_PREUPLOADORDER entity);
        int Update(P_PREUPLOADORDER entity);
        WP_QYUSER FindApproveInfo(string userId);
        P_PREUPLOADORDER LoadPreUploadOrder(Guid id);
        int UpdateReopen(string id, string operatorMUDID, string operatorName, string reason, string remark, string originatorMUDID, string originatorName);
        int UpdateTransfer(Guid id, string OperatorMUDID, string OperatorName, string UserMUDID, string UserName);
        int UpdateTransfers(string id, string OperatorMUDID, string OperatorName, string UserMUDID, string UserName);
        List<P_ORDER> LoadUploadOrder(string userId, string htCode, int rows, int page, out int total);
        bool FindApproveState(string htCode);
        int Import(List<P_REOPEN_VIEW> list, ref List<P_REOPEN_VIEW> fails);
        List<P_PREUPLOADORDER> GetUploadOrderByUserId(string UserId);
        List<V_UnFinishOrder> LoadUnFinishOrder(string CN, string MUDID, string StartDate, string EndDate,int page, int rows, out int total);
        List<V_UnFinishOrder> LoadUnFinishOrderForMessage(string CN, string MUDID, string StartDate, string EndDate);
        List<P_PREUPLOADORDER> LoadHTCode();
        List<P_PREUPLOADORDER> LoadDataByInHTCode(string subHTCode);

        #region 同步上传文件表
        int SyncPreUploadOrder();
        #endregion
    }
}
