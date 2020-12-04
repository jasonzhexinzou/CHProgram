using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using MealAdmin.Entity;
using MealAdmin.Service;
using MealAdmin.Entity.View;

namespace MealAdminApi
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“UploadOrder”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 UploadOrder.svc 或 UploadOrder.svc.cs，然后开始调试。
    public class UploadOrder : IUploadOrder
    {
        public IUploadOrderService uploadOrderService = Global.applicationContext.GetBean("uploadOrderService") as IUploadOrderService;

        public IOrderService orderService = Global.applicationContext.GetBean("orderService") as IOrderService;

        #region 加载HT编号
        /// <summary>
        /// 加载HT编号
        /// </summary>
        /// <returns></returns>
        public List<P_ORDER> LoadHTCode(string userId)
        {
            return uploadOrderService.LoadHTCode(userId);
        }
        #endregion

        #region 根据订单号查询订单详情
        /// <summary>
        /// 根据订单号查询订单详情
        /// </summary>
        /// <param name="HTCode"></param>
        /// <returns></returns>
        public P_ORDER FindOrderByHTCode(string HTCode)
        {
            return uploadOrderService.FindOrderByHTCode(HTCode);
        }
        #endregion

        public int BUHeadApprove(Guid id, int state, string reason)
        {
            return uploadOrderService.BUHeadApprove(id, state, reason);
        }
        public P_ORDER LoadOrderInfo(Guid id)
        {
            return uploadOrderService.LoadOrderInfo(id);
        }

        public int AddOrderApproveHistory(P_OrderApproveHistory OrderlHistory)
        {
            return uploadOrderService.AddOrderApproveHistory(OrderlHistory);
        }

        public int BUHeadReject(Guid id, int state, string reason)
        {
            return uploadOrderService.BUHeadReject(id, state, reason);
        }

        public int MMCoEReject(Guid id, int state, string reason)
        {
            return uploadOrderService.MMCoEReject(id, state, reason);
        }

        public int MMCoEApprove(Guid id, int state, string reason)
        {
            return uploadOrderService.MMCoEApprove(id, state, reason);
        }

        public List<P_PreUploadOrderState> LoadMyOrderUserId(string UserId, DateTime Begin, DateTime End, string State, int rows, int page, out int total)
        {
            return uploadOrderService.LoadMyOrderUserId(UserId, Begin, End, State, rows, page, out total);
        }

        public List<P_AutoTransferState> LoadMyAutoTransfer(string UserId,DateTime End, int rows, int page, out int total)
        {
            return uploadOrderService.LoadMyAutoTransfer(UserId, End, rows, page, out total);
        }

        public List<P_PreUploadOrderState> LoadMyApprove(string UserId, DateTime Begin, DateTime End, string State, string Applicant, int rows, int page, out int total)
        {
            return uploadOrderService.LoadMyApprove(UserId, Begin, End, State, Applicant, rows, page, out total);
        }

        public List<P_PREUPLOADORDER> LoadMyApproveAll(string UserId, string Applicant)
        {
            return uploadOrderService.LoadMyApproveAll(UserId, Applicant);
        }

        public P_PREUPLOADORDER FindActivityOrderByHTCode(string HTCode)
        {
            return uploadOrderService.FindActivityOrderByHTCode(HTCode);
        }

        public P_OrderApproveHistory LoadApproveHistoryInfo(Guid PID)
        {
            return uploadOrderService.LoadApproveHistoryInfo(PID);
        }

        public List<P_OrderApproveHistory> FindorderApproveHistory(Guid PID)
        {
            return uploadOrderService.FindorderApproveHistory(PID);
        }

        public int Add(P_PREUPLOADORDER entity)
        {
            var res1 = orderService.UpdateOrderUpload(1, entity.HTCode);      //更改订单表中的上传文件状态
            if (res1 > 0)
            {
                var res = uploadOrderService.Add(entity);                         //添加上传文件
                return res;
            }
            else {
                return 0;
            }
        }
        public int Update(P_PREUPLOADORDER entity)
        {
            return uploadOrderService.Update(entity);
        }

        public WP_QYUSER FindApproveInfo(string userId)
        {
            return uploadOrderService.FindApproveInfo(userId);
        }

        public P_PREUPLOADORDER LoadPreUploadOrder(Guid id)
        {
            return uploadOrderService.LoadPreUploadOrder(id);
        }
    }
}
