using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity
{
    /// <summary>
    /// 预提交申请 实体类信息
    /// </summary>
    /// <summary>
    /// 预提交申请 实体类信息
    /// </summary>
    public partial class P_PreApproval
    {
        public Guid ID { get; set; }
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
        public string MMCoEImage { get; set; }
        public int MMCoEApproveState { get; set; }
        public bool IsBudgetChange { get; set; }
        public bool IsMMCoEChange { get; set; }
        public int IsUsed { get; set; }
        public int IsFinished { get; set; }
        public bool IsFreeSpeaker { get; set; }
        public string SpeakerServiceImage { get; set; }
        public string SpeakerBenefitImage { get; set; }
        public string ReAssignOperatorName { get; set; }
        public string ReAssignOperatorMUDID { get; set; }
        public string Position { get; set; }
        public string HospitalAddressCode { get; set; }
        public string RDSDName { get; set; }
        public string RDSDMUDID { get; set; }
        public string VeevaMeetingID { get; set; }
        public string CurrentApproverName { get; set; }
        public string CurrentApproverMUDID { get; set; }
        public int IsOnc { get; set; }
        public int IsFirst { get; set; }
        public bool IsHosOrMeetingTimeChange { get; set; }
        public string RDTerritoryCode { get; set; }
        public string DMTerritoryCode { get; set; }
        public string MRTerritoryCode { get; set; }
        public string HTType { get; set; }
    }
    public class P_PreApproval_View
    {
        public Guid ID { get; set; }
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
        public string BudgetTotal { get; set; }
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
        public bool IsFreeSpeaker { get; set; }
        public string SpeakerServiceImage { get; set; }
        public string SpeakerBenefitImage { get; set; }
        public string ReAssignOperatorName { get; set; }
        public string ReAssignOperatorMUDID { get; set; }
        public string MMCoEImage { get; set; }
        public int MMCoEApproveState { get; set; }
        public bool IsBudgetChange { get; set; }
        public bool IsMMCoEChange { get; set; }
        public int IsUsed { get; set; }
        public int IsFinished { get; set; }
        public string Position { get; set; }
        public string HospitalAddressCode { get; set; }
        public string RDSDName { get; set; }
        public string RDSDMUDID { get; set; }
        public string VeevaMeetingID { get; set; }//VeevaMeetingID
        public string RDTerritoryCode { get; set; }
        public string MRTerritoryCode { get; set; }
        public string HTType { get; set; }

    }
    public class P_PreApproval_View_Load
    {
        public Guid ID { get; set; }
        public string HTCode { get; set; }
        public string Market { get; set; }
        public string TA { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string HospitalCode { get; set; }
        public string HospitalName { get; set; }
        public string HospitalAddress { get; set; }
        public string MeetingName { get; set; }
        public DateTime? MeetingTime { get; set; }
        public int PersonCount { get; set; }
        public string CostCenterCode { get; set; }
        public string BUHeadName { get; set; }
        public string BUHeadMUDID { get; set; }
        public decimal BudgetTotal { get; set; }
        public bool IsFollow { get; set; }
        public DateTime? CreateDate { get; set; }
        public string UserId { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public int IsCheckedStatement { get; set; }
    }

    public class HTCode
    {
        public string HTCodeString { get; set; }
    }

    public class PreApprovalState
    {
        public Guid ID { get; set; }
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
        public string MMCoEImage { get; set; }
        public int MMCoEApproveState { get; set; }
        public bool IsBudgetChange { get; set; }
        public bool IsMMCoEChange { get; set; }
        public int IsUsed { get; set; }
        public int IsFinished { get; set; }
        public int OrderState { get; set; }
    }
    public class P_CostCenter_Count
    {
        public int Count { get; set; }
    }
    public class P_PreApproval_TERRITORY
    {

        public string TA { get; set; }
        public string RDTerritoryCode { get; set; }
        public string CostCenter { get; set; }
        public string DMTerritoryCode { get; set; }
        public string MRTerritoryCode { get; set; }
        public string TERRITORY_MR { get; set; }
        public string TERRITORY_RD { get; set; }
        public string TERRITORY_RM { get; set; }
        public string TERRITORY_DM { get; set; }
        public string MUD_ID_MR { get; set; }
        public string MUD_ID_DM { get; set; }
        public string MUD_ID_RM { get; set; }
        public string MUD_ID_RD { get; set; }
        public string NAME { get; set; }
        public double PreCount { get; set; }
        public string ConCode { get; set; }
        public decimal PrePrice { get; set; }

    }
    public class P_PreApproval_Count_View
    {
        public string CodeandNAME { get; set; }
        public double PreCount { get; set; }
        public string PrePrice { get; set; }
        public decimal PreAmount { get; set; }

    }
    public class P_PreApproval_CountAmount
    {

        public string ConCode { get; set; }
        public string NAME { get; set; }
        public string MUDID { get; set; }
        public double newZeroCount { get; set; }
        public double newNonZeroCount { get; set; }
        public double TotalCount { get; set; }
        public decimal TotalPrice { get; set; }

    }
    public class P_PreApproval_HospitalRanking
    {

        public string HospitalCode { get; set; }
        public string HospitalName { get; set; }
        public double ZeroCount { get; set; }
        public double NonZeroCount { get; set; }
        public double newTotalCount { get; set; }
        public decimal newTotalPrice { get; set; }

    }
    public partial class P_PreApproval_LIST_VIEW
    {
        public Guid ID { get; set; }
        public string TA { get; set; }
        public string ApplierMUDID { get; set; }
        public string HospitalCode { get; set; }
        public int HTType { get; set; }
        public decimal BudgetTotal { get; set; }
    }
    public partial class P_PreApproval_Hospital_VIEW
    {
       
        public string HospitalCode { get; set; }
        public string MainAddress { get; set; }
        public string Address { get; set; }
        public int IsDelete { get; set; }
        public DateTime? CopyDate { get; set; }
    }
    public partial class P_PreApproval_ANALYSIS_LIST
    {
        public string TA { get; set; }
        public string ZeroCount { get; set; }
        public string NonZeroCount { get; set; }
        public string TotalCount { get; set; }
        public string HospitalCount { get; set; }
        public string PreHospitalCount { get; set; }
        public string MUDIDCount { get; set; }
        public string ApplierCount { get; set; }
    }

    public class P_PreApproval_CountAmount_View
    {

        public string DMTerritoryCode { get; set; }
        public string DMName { get; set; }
        public string MRTerritoryCode { get; set; }
        public string MRName { get; set; }        
        public double newZeroCount { get; set; }
        public double newNonZeroCount { get; set; }
        public double TotalCount { get; set; }
        public decimal TotalPrice { get; set; }
        public string PreState { get; set; }

    }

    public class P_PreApproval_OwnBelongCountAmount
    {
        public double TotalCount { get; set; }
        public decimal TotalPrice { get; set; }
        public double newZeroCount { get; set; }
        public double newNonZeroCount { get; set; }
        public string OwnTerritory { get; set; }
        public string BelongTerritory { get; set; }

    }
    public class P_PreApproval_Count_Amount_View
    {
        public string Name { get; set; }
        public decimal TotalBudget { get; set; }
        public double PreCount { get; set; }

    }

    public class P_PreApproval_TAB_VIEW
    {
        public string Name { get; set; }
        public string TerritoryCode { get; set; }
        public double NonZeroCount { get; set; }
        public double ZeroCount { get; set; }
        public double NonZeroProportion { get; set; }
        public double ZeroProportion { get; set; }
        public decimal TotalPrice { get; set; }
        public List<P_PreApproval_DOWN_VIEW> DownList { get; set; }
    }
    
    public class P_PreApproval_DOWN_VIEW
    {
        public string Name { get; set; }
        public string TerritoryCode { get; set; }
        public double NonZeroCount { get; set; }
        public double ZeroCount { get; set; }
        public double NonZeroProportion { get; set; }
        public double ZeroProportion { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class P_PreApproval_TotalCountAmount
    {       
        public double ZeroCount { get; set; }
        public double NonZeroCount { get; set; }
        public double TotalCount { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class P_PreApproval_PreStateCount
    {
        public double SuccessCount { get; set; }
        public double RefuseCount { get; set; }
        public double PendingCount { get; set; }
        public double CancelCount { get; set; }
        public double TotalCount { get; set; }
    }

    public class P_PreApproval_TAB_State
    {
        public string Name { get; set; }
        public string TerritoryCode { get; set; }
        public string PreState { get; set; }
        public double PreCount { get; set; }
        public decimal TotalBudget { get; set; }
        public List<P_PreApproval_DOWN_State> DownList { get; set; }
    }

    public class P_PreApproval_DOWN_State
    {
        public string Name { get; set; }
        public string TerritoryCode { get; set; }
        public string PreState { get; set; }
        public double PreCount { get; set; }
        public decimal TotalBudget { get; set; }
    }
}
