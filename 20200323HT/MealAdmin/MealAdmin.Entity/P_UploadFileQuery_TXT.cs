using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity
{
    public class P_UploadFileQuery_TXT
    {
        public Guid ID { get; set; }
        public string ApplierName { get; set; }
        public string ApplierMUDID { get; set; }
        public string ApplierMobile { get; set; }
        public string HTCode { get; set; }
        public string Market { get; set; }
        public string VeevaMeetingID { get; set; }
        public string TA { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string HospitalName { get; set; }
        public string MeetingName { get; set; }
        public int AttendCount { get; set; }
        public string CostCenter { get; set; }
        public string Channel { get; set; }
        public string CN { get; set; }
        public DateTime DeliverTime { get; set; }
        public int AttendCounts { get; set; }
        public int FoodCount { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal XmsTotalPrice { get; set; }
        public string ChangeTotalPriceReason { get; set; }
        public DateTime ReceiveDate { get; set; }
        public string RealPrice { get; set; }
        public string RealPriceChangeReason { get; set; }
        public string RealPriceChangeRemark { get; set; }
        public string RealCount { get; set; }
        public string RealCountChangeReason { get; set; }
        public string RealCountChangeRemrak { get; set; }
        public int State { get; set; }
        public int IsOrderUpload { get; set; }
        public bool IsReAssign { get; set; }
        public string ReAssignBUHeadName { get; set; }
        public string ReAssignBUHeadMUDID { get; set; }
        public string Stated { get; set; }
        public int IsAttentSame { get; set; }
        public string AttentSameReason { get; set; }
        public int IsReopen { get; set; }
        public string ReopenOperatorName { get; set; }
        public string ReopenOperatorMUDID { get; set; }
        public string ReopenOriginatorMUDID { get; set; }
        public string ReopenOriginatorName { get; set; }
        public DateTime? ReopenOperateDate { get; set; }
        public string ReopenReason { get; set; }
        public string ReopenRemark { get; set; }
        public int UploadReOpenState { get; set; }
        public string MMCoEImageOne { get; set; }
        public string MMCoEImageTwo { get; set; }
        public string MMCoEImageThree { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string Position { get; set; }
        //是否申请退单
        public string IsCancel { get; set; }
        //是否退单成功
        public string CancelState { get; set; }
        //是否收餐/未送达
        public string IsReceive { get; set; }
        //是否与预订餐品一致
        public string IsMealSame { get; set; }
        //项目组特殊备注
        public string SpecialReason { get; set; }
        //上传文件审批日期
        public DateTime? ApproveDate { get; set; }
        //是否与会议信息一致
        public string IsMeetingSame { get; set; }
        //会议信息不一致原因
        public string MeetingSameReason { get; set; }
        //会议未正常召开原因/会议文件丢失原因
        public string SpecialUploadReason { get; set; }

    }
    public class P_UploadFileQuery
    {
        public Guid ID { get; set; }
        public string ApplierName { get; set; }
        public string ApplierMUDID { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public string HTCode { get; set; }
        public string BUHeadName { get; set; }
        public string BUHeadMUDID { get; set; }
        public DateTime BUHeadApproveDate { get; set; }
        public bool IsReAssign { get; set; }
        public string ReAssignBUHeadName { get; set; }
        public string ReAssignBUHeadMUDID { get; set; }
        public DateTime ReAssignBUHeadApproveDate { get; set; }
        public string State { get; set; }
        public string MMCoEImageOne { get; set; }
        public string MMCoEImageTwo { get; set; }
        public string MMCoEImageThree { get; set; }
        public int FileType { get; set; }
    }
}
