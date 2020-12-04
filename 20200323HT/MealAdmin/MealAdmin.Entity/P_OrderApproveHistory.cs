using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity
{
    public class P_OrderApproveHistory
    {
        public Guid ID { get; set; }
        public Guid PID { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public int ActionType { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string Comments { get; set; }
        public int type { get; set; }
    }

    public class P_OrderApproveHistory_Time
    {
        public Guid ID { get; set; }
        public Guid PID { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string ActionType { get; set; }
        public string ApproveDate { get; set; }
        public string Comments { get; set; }
        public int type { get; set; }
    }

    public class P_OrderApproveHistory_View
    {
        public Guid ID { get; set; }
        public Guid PID { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public int ActionType { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string Comments { get; set; }
        public string ApplierName { get; set; }
        public string ApplierMUDID { get; set; }
        public string ApplierMobile { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string HTCode { get; set; }
        public string Market { get; set; }
        public string TA { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string HospitalCode { get; set; }
        public string HospitalName { get; set; }
        public string HospitalAddress { get; set; }
        public string MeetingName { get; set; }
        public DateTime? MeetingDate { get; set; }
        public int AttendCount { get; set; }
        public string CostCenter { get; set; }
        public decimal BudgetTotal { get; set; }
        public bool IsDMFollow { get; set; }
        public string BUHeadName { get; set; }
        public string BUHeadMUDID { get; set; }
        public DateTime? BUHeadApproveDate { get; set; }
        public bool IsReAssign { get; set; }
        public string ReAssignBUHeadName { get; set; }
        public string ReAssignBUHeadMUDID { get; set; }
        public DateTime? ReAssignBUHeadApproveDate { get; set; }
        public string State { get; set; }

    }

    public class P_OrderApproveHistory_Load
    {
        public Guid ID { get; set; }
        public Guid PID { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public int ActionType { get; set; }
        public string ApproveDate { get; set; }
        public string ApproveTime { get; set; }
        public string Comments { get; set; }
        public string ApplierName { get; set; }
        public string ApplierMUDID { get; set; }
        public string ApplierMobile { get; set; }
        public string CreateDate { get; set; }
        public string CreateTime { get; set; }
        public string ModifyDate { get; set; }
        public string ModifyTime { get; set; }
        public string HTCode { get; set; }
        public string Market { get; set; }
        public string TA { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string HospitalCode { get; set; }
        public string HospitalName { get; set; }
        public string HospitalAddress { get; set; }
        public string MeetingName { get; set; }
        public string MeetingDate { get; set; }
        public string MeetingTime { get; set; }
        public int AttendCount { get; set; }
        public string CostCenter { get; set; }
        public decimal BudgetTotal { get; set; }
        public bool IsDMFollow { get; set; }
        public string BUHeadName { get; set; }
        public string BUHeadMUDID { get; set; }
        public string BUHeadApproveDate { get; set; }
        public string BUHeadApproveTime { get; set; }
        public bool IsReAssign { get; set; }
        public string ReAssignBUHeadName { get; set; }
        public string ReAssignBUHeadMUDID { get; set; }
        public string ReAssignBUHeadApproveDate { get; set; }
        public string ReAssignBUHeadApproveTime { get; set; }
        public string State { get; set; }

    }
}
