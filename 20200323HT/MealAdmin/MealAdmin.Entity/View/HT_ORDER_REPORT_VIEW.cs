using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity.View
{
    public class HT_ORDER_REPORT_VIEW
    {
        public string ApplierName { get; set; }
        public string ApplierMUDID { get; set; }
        public string Position { get; set; }
        public string ApplierMobile { get; set; }
        public DateTime PRECreateDate { get; set; }
        public DateTime PREModifyDate { get; set; }
        public string HTCode { get; set; }
        public string Market { get; set; }
        public string VeevaMeetingID { get; set; }//VeevaMeetingID
        public string TA { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string HospitalCode { get; set; }
        public string HospitalName { get; set; }
        public string HospitalAddress { get; set; }
        public string MeetingName { get; set; }
        public DateTime MeetingDate { get; set; }
        public int PREAttendCount { get; set; }
        public string CostCenter { get; set; }
        public decimal BudgetTotal { get; set; }
        public bool IsDMFollow { get; set; }
        public bool IsFreeSpeaker { get; set; }
        public string RDSDName { get; set; }
        public string RDSDMUDID { get; set; }
        public string PREBUHeadName { get; set; }
        public string PREBUHeadMUDID { get; set; }
        public DateTime? PREBUHeadApproveDate { get; set; }
        public string PREState { get; set; }
        public bool PREIsReAssign { get; set; }
        public string PREReAssignOperatorName { get; set; }
        public string PREReAssignOperatorMUDID { get; set; }
        public string PREReAssignBUHeadName { get; set; }
        public string PREReAssignBUHeadMUDID { get; set; }
        public DateTime? PREReAssignBUHeadApproveDate { get; set; }             //30
        public string Channel { get; set; }
        public string EnterpriseOrderId { get; set; }
        public DateTime ORDCreateDate { get; set; }
        public DateTime DeliverTime { get; set; }
        public string RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public int ORDAttendCount { get; set; }
        public int FoodCount { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal totalFee { get; set; }
        public string feeModifyReason { get; set; }
        public string DeliveryAddress { get; set; }
        public string Consignee { get; set; }
        public string Phone { get; set; }
        public string Remark { get; set; }
        public string bookState { get; set; }
        public DateTime? ReceiveTime { get; set; }
        public int IsRetuen { get; set; }
        public string cancelState { get; set; }
        public string cancelFeedback { get; set; }
        public string cancelFailReason { get; set; }
        public int ReceiveState { get; set; }
        public DateTime? ReceiveDate { get; set; }
        public int ORDState { get; set; }
        public string RealPrice { get; set; }
        public string RealPriceChangeReason { get; set; }
        public string RealPriceChangeRemark { get; set; }
        public string RealCount { get; set; }
        public string RealCountChangeReason { get; set; }
        public string RealCountChangeRemrak { get; set; }
        public int IsOrderUpload { get; set; }             // ***************60
        public int EOnTime { get; set; }
        public string EOnTimeDesc { get; set; }
        public int EIsSafe { get; set; }
        public string EIsSafeDesc { get; set; }
        public int EHealth { get; set; }
        public string EHealthDesc { get; set; }
        public int EPack { get; set; }
        public string EPackDesc { get; set; }
        public int ECost { get; set; }
        public string ECostDesc { get; set; }
        public string EOtherDesc { get; set; }
        public int EStar { get; set; }
        public DateTime? ECreateDate { get; set; }
        public string TYYYDRDC { get; set; }
        public string TYDBDRDC { get; set; }
        public string TYCTDRDC { get; set; }
        public string TYDBTYYYDRDC { get; set; }
        public string TYDBTYCTDRDC { get; set; }
        public string TYDBTYYYTYCTDRDC { get; set; }
        public string CHRSDYLS { get; set; }
        public string CHRSXYLSDDFSDYLS { get; set; }
        public string customerPickup { get; set; }             //*****************82
        public DateTime? PUOCreateDate { get; set; }
        public string PUOBUHeadName { get; set; }
        public string PUOBUHeadMUDID { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string PUOState { get; set; }
        public int IsAttentSame { get; set; }
        public string AttentSameReason { get; set; }
        public int IsReopen { get; set; }
        public string ReopenOperatorName { get; set; }
        public string ReopenOperatorMUDID { get; set; }
        public DateTime? ReopenOperateDate { get; set; }
        public string ReopenReason { get; set; }
        public int IsTransfer { get; set; }
        public string TransferOperatorName { get; set; }
        public string TransferOperatorMUDID { get; set; }
        public string TransferUserName { get; set; }
        public string TransferUserMUDID { get; set; }
        public DateTime? TransferOperateDate { get; set; }
        public bool IsReAssign { get; set; }
        public string ReAssignOperatorName { get; set; }
        public string ReAssignOperatorMUDID { get; set; }
        public string ReAssignBUHeadName { get; set; }
        public string ReAssignBUHeadMUDID { get; set; }
        public DateTime? ReAssignBUHeadApproveDate { get; set; }
        public string SpecialReason { get; set; }    //*************106

        public string Level2Name { get; set; }
        public string Level2UserId { get; set; }
        public string Level3Name { get; set; }
        public string Level3UserId { get; set; }
        public int IsMealSame { get; set; }
        public int IsMeetingInfoSame { get; set; }
        public string MeetingInfoSameReason { get; set; }

        public string CurrentApproverName { get; set; }
        public string CurrentApproverMUDID { get; set; }
        public string MRTerritoryCode { get; set; }
        public string RDTerritoryCode { get; set; }

        public string SupplierSpecialRemark { get; set; }
        public string IsCompleteDelivery { get; set; }      
        public decimal? SupplierConfirmAmount { get; set; }
        public decimal? GSKConfirmAmount { get; set; }
        public string GSKConAAReason { get; set; }
        public decimal? MealPaymentAmount { get; set; }
        public string MealPaymentPO { get; set; }
        public string AccountingTime { get; set; }

    }
}
