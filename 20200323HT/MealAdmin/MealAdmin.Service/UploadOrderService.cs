using MealAdmin.Dao;
using MealAdmin.Entity;
using MealAdmin.Entity.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XFramework.XInject.Attributes;

namespace MealAdmin.Service
{
    public class UploadOrderService : IUploadOrderService
    {
        [Bean("uploadOrderDao")]
        public IUploadOrderDao UploadOrderDao { get; set; }

        [Bean("orderDao")]
        public IOrderDao orderDao { get; set; }

        [Bean("groupMemberDao")]
        public IGroupMemberDao groupMemberDao { get; set; }

        #region 加载HT编号
        /// <summary>
        /// 加载HT编号
        /// </summary>
        /// <returns></returns>
        public List<P_ORDER> LoadHTCode(string userId)
        {
            return UploadOrderDao.LoadHTCode(userId);
        }
        #endregion

        #region 根据订单号查询订单详情
        /// <summary>
        /// 根据订单号查询订单详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public P_ORDER FindOrderByHTCode(string HTCode)
        {
            return UploadOrderDao.FindOrderByHTCode(HTCode);
        }
        #endregion

        #region 审批通过
        /// <summary>
        /// 审批通过
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public int BUHeadApprove(Guid id, int state, string reason)
        {
            var res = UploadOrderDao.BUHeadApprove(id, state, reason);
            var upload = UploadOrderDao.LoadPreUploadOrder(id);
            var nowDate = DateTime.Now;
            nowDate = nowDate.AddHours(-168);
            groupMemberDao.UpdateServPauseDetail(upload.HTCode, upload.IsReopen == 1 ? 4 : 3);
            var userId = upload.ApplierMUDID;
            if (upload.IsTransfer == 1)
            {
                userId = upload.TransferUserMUDID;
            }
            var orderList = orderDao.LoadFailOrder(nowDate, userId);
            if (orderList.Count < 1)
            {
                var group = groupMemberDao.FindByUser(userId);
                if (group != null)
                {
                    if (group.State == 0 && group.State1 == 0 && group.State2 == 0)
                    {
                        groupMemberDao.DelByState3(userId);
                    }
                    else
                    {
                        groupMemberDao.UpdateState3(userId);
                    }
                }
            }

            return res;
        }
        #endregion

        public P_ORDER LoadOrderInfo(Guid id)
        {
            return UploadOrderDao.LoadOrderInfo(id);
        }

        public int AddOrderApproveHistory(P_OrderApproveHistory OrderlHistory)
        {
            return UploadOrderDao.AddOrderApproveHistory(OrderlHistory);
        }

        public int BUHeadReject(Guid id, int state, string reason)
        {
            return UploadOrderDao.BUHeadReject(id, state, reason);
        }

        public int MMCoEReject(Guid id, int state, string reason)
        {
            return UploadOrderDao.MMCoEReject(id, state, reason);
        }

        public int MMCoEApprove(Guid id, int state, string reason)
        {
            var res = UploadOrderDao.MMCoEApprove(id, state, reason);
            return res;

        }

        public List<P_PreUploadOrderState> LoadMyOrderUserId(string UserId, DateTime Begin, DateTime End, string State, int rows, int page, out int total)
        {
            return UploadOrderDao.LoadMyOrderUserId(UserId, Begin, End, State, rows, page, out total);
        }

        public List<P_AutoTransferState> LoadMyAutoTransfer(string UserId, DateTime End, int rows, int page, out int total)
        {
            return UploadOrderDao.LoadMyAutoTransfer(UserId, End, rows, page, out total);
        }

        public List<P_PreUploadOrderState> LoadMyApprove(string UserId, DateTime Begin, DateTime End, string State, string Applicant, int rows, int page, out int total)
        {
            return UploadOrderDao.LoadMyApprove(UserId, Begin, End, State, Applicant, rows, page, out total);
        }

        public List<P_PREUPLOADORDER> LoadMyApproveAll(string UserId, string Applicant)
        {
            return UploadOrderDao.LoadMyApproveAll(UserId, Applicant);
        }

        public P_PREUPLOADORDER FindActivityOrderByHTCode(string HTCode)
        {
            return UploadOrderDao.FindActivityOrderByHTCode(HTCode);
        }

        public P_OrderApproveHistory LoadApproveHistoryInfo(Guid PID)
        {
            return UploadOrderDao.LoadApproveHistoryInfo(PID);
        }

        public List<P_OrderApproveHistory> FindorderApproveHistory(Guid PID)
        {
            return UploadOrderDao.FindorderApproveHistory(PID);
        }

        public int Add(P_PREUPLOADORDER entity)
        {
            var res = UploadOrderDao.Add(entity);
            if (res>0)
            {
                groupMemberDao.UpdateServPauseDetail(entity.HTCode, 2);

            }
            return res;
        }
        public int Update(P_PREUPLOADORDER entity)
        {
            var res = UploadOrderDao.Update(entity);
            if (entity.IsReopen == 1)
            {
                groupMemberDao.UpdateServPauseDetail(entity.HTCode, 4);
                var nowDate = DateTime.Now;
                nowDate = nowDate.AddHours(-168);
                var userId = entity.ApplierMUDID;
                if (entity.IsTransfer == 1)
                {
                    userId = entity.TransferUserMUDID;
                }
                var orderList = orderDao.LoadFailOrder(nowDate, userId);
                if (orderList.Count < 1)
                {
                    var group = groupMemberDao.FindByUser(userId);
                    if (group != null)
                    {
                        if (group.State == 0 && group.State1 == 0 && group.State2 == 0)
                        {
                            groupMemberDao.DelByState3(userId);
                        }
                        else
                        {
                            groupMemberDao.UpdateState3(userId);
                        }
                    }
                }
            }
            return res;
        }

        public WP_QYUSER FindApproveInfo(string userId)
        {
            return UploadOrderDao.FindApproveInfo(userId);
        }

        public P_PREUPLOADORDER LoadPreUploadOrder(Guid id)
        {
            return UploadOrderDao.LoadPreUploadOrder(id);
        }


        #region RE-OPEN
        /// <summary>
        /// RE-OPEN
        /// </summary>
        /// <param name="id"></param>
        /// <param name="OperatorMUDID"></param>
        /// <param name="OperatorName"></param>
        /// <param name="Reason"></param>
        /// <returns></returns>
        public int UpdateReopen(string id, string operatorMUDID, string operatorName, string reason, string remark, string originatorMUDID, string originatorName)
        {
            return UploadOrderDao.UpdateReopen(id, operatorMUDID, operatorName, reason, remark, originatorMUDID, originatorName);
        }
        #endregion

        #region 上传文件重新分配
        /// <summary>
        /// 上传文件重新分配
        /// </summary>
        /// <param name="id"></param>
        /// <param name="OperatorMUDID"></param>
        /// <param name="OperatorName"></param>
        /// <param name="UserMUDID"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public int UpdateTransfer(Guid id, string OperatorMUDID, string OperatorName, string UserMUDID, string UserName)
        {
            return UploadOrderDao.UpdateTransfer(id, OperatorMUDID, OperatorName, UserMUDID, UserName);
        }

        public int UpdateTransfers(string id, string OperatorMUDID, string OperatorName, string UserMUDID, string UserName)
        {
            return UploadOrderDao.UpdateTransfers(id, OperatorMUDID, OperatorName, UserMUDID, UserName);
        }
        #endregion



        public List<P_ORDER> LoadUploadOrder(string userId, string htCode, int rows, int page, out int total)
        {
            return UploadOrderDao.LoadUploadOrder(userId, htCode, rows, page, out total);
        }



        public bool FindApproveState(string htCode)
        {
            return UploadOrderDao.FindApproveState(htCode);
        }


        public int Import(List<P_REOPEN_VIEW> list, ref List<P_REOPEN_VIEW> fails)
        {
            return UploadOrderDao.Import(list, ref fails);
        }

        public List<P_PREUPLOADORDER> GetUploadOrderByUserId(string UserId)
        {
            return UploadOrderDao.GetUploadOrderByUserId(UserId);
        }

        public List<V_UnFinishOrder> LoadUnFinishOrder(string CN, string MUDID, string StartDate, string EndDate,int page, int rows, out int total)
        {
            return UploadOrderDao.LoadUnFinishOrder(CN, MUDID, StartDate, EndDate,page, rows, out total);
        }

        public List<V_UnFinishOrder> LoadUnFinishOrderForMessage(string CN, string MUDID, string StartDate, string EndDate)
        {
            return UploadOrderDao.LoadUnFinishOrderForMessage(CN, MUDID, StartDate, EndDate);
        }
       
        public List<P_PREUPLOADORDER> LoadHTCode()
        {
            return UploadOrderDao.LoadHTCode();
        }
        public List<P_PREUPLOADORDER> LoadDataByInHTCode(string subHTCode)
        {
            return UploadOrderDao.LoadDataByInHTCode(subHTCode);
        }

        #region 同步上传文件表
        public int SyncPreUploadOrder()
        {
            return UploadOrderDao.SyncPreUploadOrder();
        }
        #endregion
    }
}
