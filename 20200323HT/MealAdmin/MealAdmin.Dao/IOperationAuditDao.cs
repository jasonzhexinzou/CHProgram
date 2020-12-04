using MealAdmin.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Dao
{
    public interface IOperationAuditDao
    {
        int AddAudit(string Type, string ChangeContent);
        List<P_Audit> Load1(string ApprovalMUDID, string Begin, string End, string CostCenter, string SpecialOrders1, string SpecialOrders2, string UploadFile, string SystemGroup, string AgentApprova, string Hospital, int rows, int page, out int total);
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

        #region 保存操作日志
        /// <summary>
        /// 保存操作日志
        /// </summary>
        /// <param name="OperationAudit"></param>
        /// <returns></returns>
        void SaveOperationAudit(P_OperationAudit OperationAudit);
        #endregion
    }
}
