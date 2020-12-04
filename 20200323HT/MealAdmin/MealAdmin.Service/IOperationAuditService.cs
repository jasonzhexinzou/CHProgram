using MealAdmin.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MealAdmin.Entity.View;

namespace MealAdmin.Service
{
    public interface IOperationAuditService
    {
        #region 查询操作记录
        /// <summary>
        /// 查询操作记录
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        List<P_OperationAudit> Load(int rows, int page, out int total);
        #endregion
        #region 查询操作记录
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Begin"></param>
        /// <param name="End"></param>
        /// <param name="CostCenter"></param>
        /// <param name="SpecialOrders1"></param>
        /// <param name="SpecialOrders2"></param>
        /// <param name="UploadFile"></param>
        /// <param name="SystemGroup"></param>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        List<P_Audit> Load1(string ApprovalMUDID, string Begin, string End, string CostCenter, string SpecialOrders1, string SpecialOrders2, string UploadFile, string SystemGroup, string AgentApprova, string Hospital, int rows, int page, out int total);
        #endregion
        int AddAudit(string Type, string ChangeContent);
        #region 保存操作日志
        /// <summary>
        /// 保存操作日志
        /// </summary>
        /// <param name="OperationAudit"></param>
        void SaveOperationAudit(P_OperationAudit OperationAudit);
        #endregion
    }
}
