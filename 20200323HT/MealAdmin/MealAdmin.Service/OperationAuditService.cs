using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MealAdmin.Entity;
using XFramework.XInject.Attributes;
using MealAdmin.Dao;

namespace MealAdmin.Service
{
    public class OperationAuditService : IOperationAuditService
    {
        [Bean("operationAuditDao")]
        public IOperationAuditDao operationAuditDao { get; set; }
       
      
        public List<P_Audit> Load1(string ApprovalMUDID, string Begin, string End, string CostCenter, string SpecialOrders1, string SpecialOrders2, string UploadFile, string SystemGroup, string AgentApprova,string Hospital, int rows, int page, out int total)
        {
            return operationAuditDao.Load1(ApprovalMUDID, Begin, End, CostCenter, SpecialOrders1, SpecialOrders2, UploadFile, SystemGroup, AgentApprova, Hospital, rows, page, out total);
        }
        public int AddAudit(string Type, string ChangeContent)
        {
            return operationAuditDao.AddAudit(Type, ChangeContent);
        }
        #region 查询操作记录
        /// <summary>
        /// 查询操作记录
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<P_OperationAudit> Load(int rows, int page, out int total)
        {
            return operationAuditDao.Load(rows, page, out total);
        }
        #endregion

        #region 保存操作日志
        /// <summary>
        /// 保存操作日志
        /// </summary>
        /// <param name="OperationAudit"></param>
        /// <returns></returns>
        public void SaveOperationAudit(P_OperationAudit OperationAudit)
        {
            operationAuditDao.SaveOperationAudit(OperationAudit);
        }
        #endregion
    }
}
