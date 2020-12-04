using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity
{
    public partial class P_PREUPLOADORDER
    {
        public Guid ID { get; set; }
        public string ApplierName { get; set; }
        public string ApplierMUDID { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string HTCode { get; set; }
        public string BUHeadName { get; set; }
        public string BUHeadMUDID { get; set; }
        public DateTime? BUHeadApproveDate { get; set; }
        public bool IsReAssign { get; set; }                        //上传文件是否重新分配审批人
        public string ReAssignBUHeadName { get; set; }              //上传文件被重新分配审批人姓名
        public string ReAssignBUHeadMUDID { get; set; }             //上传文件被重新分配审批人MUDID
        public DateTime? ReAssignBUHeadApproveDate { get; set; }    //上传文件重新分配审批人时间
        public string State { get; set; }
        public string MMCoEImageOne { get; set; }
        public string MMCoEImageTwo { get; set; }
        public string MMCoEImageThree { get; set; }
        public int FileType { get; set; }
        public int IsAttentSame { get; set; }
        public string AttentSameReason { get; set; }
        public string SpecialReason { get; set; }
        public int IsAddFile { get; set; }
        public int IsReopen { get; set; }                       //上传文件是否Re-Open
        public string ReopenOriginatorMUDID { get; set; }         //上传文件Re-Open操作人MUDID
        public string ReopenOriginatorName { get; set; }         //上传文件Re-Open操作人MUDID
        public string ReopenOperatorMUDID { get; set; }         //上传文件Re-Open操作人MUDID
        public string ReopenOperatorName { get; set; }          //上传文件Re-Open操作人
        public DateTime? ReopenOperateDate { get; set; }        //上传文件Re-Open时间
        public string ReopenReason { get; set; }                //上传文件Re-Open原因
        public string ReopenRemark { get; set; }                //上传文件Re-Open备注
        public int IsTransfer { get; set; }                //上传文件Re-Open备注
        public string TransferOperatorMUDID { get; set; }                //上传文件Re-Open备注
        public string TransferOperatorName { get; set; }                //上传文件Re-Open备注
        public string TransferUserMUDID { get; set; }                //上传文件Re-Open备注
        public string TransferUserName { get; set; }                //上传文件Re-Open备注
        public DateTime? TransferOperateDate { get; set; }       //上传文件重新分配审批人-操作人MUDID
        public string ReAssignOperatorMUDID { get; set; }        //上传文件重新分配审批人-操作人
        public string ReAssignOperatorName { get; set; }
        public int UploadReOpenState { get; set; }
        public int IsMeetingInfoSame { get; set; }
        public string MeetingInfoSameReason { get; set; }
        public string Memo { get; set; }

    }

    public partial class P_PreUploadOrderState
    {
        public Guid ID { get; set; }
        public string ApplierName { get; set; }
        public string ApplierMUDID { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string HTCode { get; set; }
        public string BUHeadName { get; set; }
        public string BUHeadMUDID { get; set; }
        public DateTime? BUHeadApproveDate { get; set; }
        public bool IsReAssign { get; set; }
        public string ReAssignBUHeadName { get; set; }
        public string ReAssignBUHeadMUDID { get; set; }
        public DateTime? ReAssignBUHeadApproveDate { get; set; }
        public string State { get; set; }
        public string MMCoEImageOne { get; set; }
        public string MMCoEImageTwo { get; set; }
        public string MMCoEImageThree { get; set; }
        public int FileType { get; set; }
        public int IsAttentSame { get; set; }
        public string AttentSameReason { get; set; }
        public string SpecialReason { get; set; }
        public int IsAddFile { get; set; }
        public DateTime? MeetingDate { get; set; }
        public string HospitalName { get; set; }
        public int IsTransfer { get; set; }
    }

    public partial class P_AutoTransferState
    {
        public Guid ID { get; set; }
        public string HTCode { get; set; }
        public DateTime? DeliverTime { get; set; }
        public string HospitalName { get; set; }
        public DateTime? CreateDate { get; set; }
        public string ApplierName { get; set; }
    }
}
