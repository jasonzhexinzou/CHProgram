namespace MealAdmin.Entity
{
    using System;
    using System.Collections.Generic;

    public partial class P_ORDER
    {
        public Guid ID { get; set; }   //主键ID
        public Guid ChangeID { get; set; }   //
        public string UserId { get; set; }   //用户ID
        public string Market { get; set; }   //Market
        public string VeevaMeetingID { get; set; }//VeevaMeetingID
        public string HospitalId { get; set; }   //医院编号
        public string Address { get; set; }   //
        public string CN { get; set; }   //CN编号，关联HTCode关联HTCode
        public string RestaurantId { get; set; }   //
        public string RestaurantName { get; set; }   //
        public string RestaurantLogo { get; set; }   //
        public decimal TotalPrice { get; set; }   //预定金额
        public string DeliveryGeo { get; set; }   //
        public string Detail { get; set; }   //
        public string ChangeDetail { get; set; }   //
        public int FoodCount { get; set; }   //
        public int AttendCount { get; set; }   //用餐人数
        public string DeliveryAddress { get; set; }   //送餐地址
        public string Consignee { get; set; }   //收餐人
        public string Phone { get; set; }   //联系电话
        public DateTime DeliverTime { get; set; }   //送餐时间
        public string Remark { get; set; }   //备注
        public string MMCoEImage { get; set; }   //
        public int State { get; set; }   //
        public int ReceiveState { get; set; }   //
        public string ReceiveCode { get; set; }   //
        public DateTime CreateDate { get; set; }   //
        public string XmsOrderId { get; set; }   //
        public string OldXmlOrderId { get; set; }   //
        public DateTime SendOrderDate { get; set; }   //
        public DateTime ChangeOrderDate { get; set; }   //
        public DateTime ReturnOrderDate { get; set; }   //
        public decimal XmsTotalPrice { get; set; }   //实际金额
        public string ChangeTotalPriceReason { get; set; }   //实际金额调整原因
        public DateTime ReceiveDate { get; set; }   //
        public int IsRetuen { get; set; }   //
        public int IsChange { get; set; }   //
        public string XmsOrderReason { get; set; }   //
        public int IsOuterMeeting { get; set; }   //
        public string RestaurantTel { get; set; }   //
        public string RestaurantAddress { get; set; }   //
        public string ApproveReason { get; set; }   //
        public int MMCoEApproveState { get; set; }   //
        public string MMCoEReason { get; set; }   //
        public int IsReturn { get; set; }   //
        public int IsDelivery { get; set; }   //
        public string PO { get; set; }   //
        public string WBS { get; set; }   //
        public int IsNonHT { get; set; }   //
        public string Channel { get; set; }   //供应商
        public string MeetingCode { get; set; }   //
        public string MeetingName { get; set; }   //会议名称
        public string TA { get; set; }   //TA
        public DateTime ReceiveTime { get; set; }   //
        public string EnterpriseOrderId { get; set; }   //
        public string Province { get; set; }   //省份
        public string City { get; set; }   //城市
        public string HospitalName { get; set; }   //医院名称
        public string RealCount { get; set; }   //实际用餐人数
        public string RealCountChangeReason { get; set; }   //用餐人数调整原因
        public string RealCountChangeRemrak { get; set; }   //用餐人数调整备注
        public string RealPrice { get; set; }   //用户确认金额
        public string RealPriceChangeReason { get; set; }   //确认金额调整原因
        public string RealPriceChangeRemark { get; set; }   //确认金额调整备注
        public string SpecialRemarksProjectTeam { get; set; }   //项目组特殊备注
        public int IsOrderUpload { get; set; }   //订单是否上传（1：已上传；0：未上传）
        public int IsSpecialOrder { get; set; }
        public int IsPushOne { get; set; }     //送餐时间后一小时推送
        public int IsPushTwo { get; set; }     //收餐时间后一小时推送
        public int IsTransfer { get; set; }                     //上传文件是否重新分配
        public string TransferOperatorMUDID { get; set; }       //上传文件重新分配操作人MUDID
        public string TransferOperatorName { get; set; }        //上传文件重新分配操作人
        public string TransferUserMUDID { get; set; }           //上传文件被重新分配人MUDID
        public string TransferUserName { get; set; }            //上传文件被重新分配人姓名
        public DateTime? TransferOperateDate { get; set; }         //上传文件被重新分配时间

    }

    public class P_ORDER_View
    {
        public Guid ID { get; set; }   //主键ID
        public Guid ChangeID { get; set; }   //
        public string UserId { get; set; }   //用户ID
        public string Market { get; set; }   //Market
        public string HospitalId { get; set; }   //医院编号
        public string Address { get; set; }   //
        public string CN { get; set; }   //CN编号，关联HTCode
        public string RestaurantId { get; set; }   //
        public string RestaurantName { get; set; }   //
        public string RestaurantLogo { get; set; }   //
        public string TotalPrice { get; set; }   //预定金额
        public string DeliveryGeo { get; set; }   //
        public string Detail { get; set; }   //
        public string ChangeDetail { get; set; }   //
        public int FoodCount { get; set; }   //
        public int AttendCount { get; set; }   //用餐人数
        public string DeliveryAddress { get; set; }   //送餐地址
        public string Consignee { get; set; }   //收餐人
        public string Phone { get; set; }   //联系电话
        public string DeliverTime { get; set; }   //送餐时间
        public string Remark { get; set; }   //备注
        public string MMCoEImage { get; set; }   //
        public string State { get; set; }   //
        public int ReceiveState { get; set; }   //
        public string ReceiveCode { get; set; }   //
        public string CreateDate { get; set; }   //
        public string XmsOrderId { get; set; }   //
        public string OldXmlOrderId { get; set; }   //
        public string SendOrderDate { get; set; }   //
        public string ChangeOrderDate { get; set; }   //
        public string ReturnOrderDate { get; set; }   //
        public string XmsTotalPrice { get; set; }   //实际金额
        public string ChangeTotalPriceReason { get; set; }   //实际金额调整原因
        public string ReceiveDate { get; set; }   //
        public int IsRetuen { get; set; }   //
        public int IsChange { get; set; }   //
        public string XmsOrderReason { get; set; }   //
        public int IsOuterMeeting { get; set; }   //
        public string RestaurantTel { get; set; }   //
        public string RestaurantAddress { get; set; }   //
        public string ApproveReason { get; set; }   //
        public int MMCoEApproveState { get; set; }   //
        public string MMCoEReason { get; set; }   //
        public int IsReturn { get; set; }   //
        public int IsDelivery { get; set; }   //
        public string PO { get; set; }   //
        public string WBS { get; set; }   //
        public int IsNonHT { get; set; }   //
        public string Channel { get; set; }   //供应商
        public string MeetingCode { get; set; }   //
        public string MeetingName { get; set; }   //会议名称
        public string TA { get; set; }   //TA
        public string ReceiveTime { get; set; }   //
        public string EnterpriseOrderId { get; set; }   //
        public string Province { get; set; }   //省份
        public string City { get; set; }   //城市
        public string HospitalName { get; set; }   //医院名称
        public string RealCount { get; set; }   //实际用餐人数
        public string RealCountChangeReason { get; set; }   //用餐人数调整原因
        public string RealCountChangeRemrak { get; set; }   //用餐人数调整备注
        public string RealPrice { get; set; }   //用户确认金额
        public string RealPriceChangeReason { get; set; }   //确认金额调整原因
        public string RealPriceChangeRemark { get; set; }   //确认金额调整备注
        public string SpecialRemarksProjectTeam { get; set; }   //项目组特殊备注
        public int IsOrderUpload { get; set; }   //订单是否上传（1：已上传；0：未上传）
        public int IsSpecialOrder { get; set; }
    }

    public class P_ORDER_UnfinishedOrder_View
    {
        
        public string TA { get; set; }  
        public string ApplierName { get; set; }
        public string ApplierMUDID { get; set; }   
        public string DeliverTime { get; set; }   //送餐时间       
        public string IsWorkdayQuit { get; set; }   //
        public string TransferUserMUDID { get; set; }   //
        public double UnfinishedCount { get; set; }   //
        public string ReAssignBUHeadMUDID { get; set; }   //
        public int IsOrderUpload { get; set; }   //订单是否上传（1：已上传；0：未上传）
        public string Address { get; set; }   //
    }
    public class P_ORDER_UnfinishedData_View
    {
        public int DeliverTime { get; set; }   //送餐时间       
        public double newOrdersCount { get; set; }   //
        public double newUnfinishedCount { get; set; }   //
       
    }
    public class P_ORDER_UnfinishedDM_View
    {
        public string  TA { get; set; }   //     
        public double newTransferDM { get; set; }   //
        public double newUnfinishedCount { get; set; }   //

    }
    public class P_ORDER_UnfinishedUser_View
    {
        public string TA { get; set; }   //     
        public double UserCount { get; set; }   //
        public double UnfinishedCount { get; set; }   //

    }

    public class P_ORDER_Unfinished_VIEW
    {
        public string ApplierName { get; set; }
        public string ApplierMUDID { get; set; }
        public string Position { get; set; }
        public string HTCode { get; set; }
        public string Market { get; set; }
        public string TA { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string HospitalCode { get; set; }
        public string HospitalName { get; set; }
        public string CostCenter { get; set; }
        public string Channel { get; set; }
        public DateTime DeliverTime { get; set; }
        public string RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public int ORDAttendCount { get; set; }
        public decimal totalFee { get; set; }
        public int IsRetuen { get; set; }
        public string cancelState { get; set; }
        public string cancelFeedback { get; set; }
        public string cancelFailReason { get; set; }
        public int ReceiveState { get; set; }
        public int ORDState { get; set; }
        public string RealPrice { get; set; }
        public string RealPriceChangeReason { get; set; }
        public string RealPriceChangeRemark { get; set; }
        public string RealCount { get; set; }
        public string RealCountChangeReason { get; set; }
        public string RealCountChangeRemrak { get; set; }
        public int IsOrderUpload { get; set; }             
        public DateTime? PUOCreateDate { get; set; }
        public string PUOBUHeadName { get; set; }
        public string PUOBUHeadMUDID { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string PUOState { get; set; }
        public int IsAttentSame { get; set; }
        public string AttentSame { get; set; }
        public string AttentSameReason { get; set; }
        public int IsReopen { get; set; }
        public string Reopen { get; set; }
        public string ReopenOperatorName { get; set; }
        public string ReopenOperatorMUDID { get; set; }
        public DateTime? ReopenOperateDate { get; set; }
        public string ReopenOriginatorName { get; set; }
        public string ReopenOriginatorMUDID { get; set; }
        public string ReopenReason { get; set; }
        public string ReopenRemark { get; set; }
        public int IsTransfer { get; set; }
        public string TransferOperatorName { get; set; }
        public string TransferOperatorMUDID { get; set; }
        public string TransferUserName { get; set; }
        public string TransferUserMUDID { get; set; }
        public DateTime? TransferOperateDate { get; set; }
        public bool IsReAssign { get; set; }
        public string ReAssign { get; set; }
        public string ReAssignOperatorName { get; set; }
        public string ReAssignOperatorMUDID { get; set; }
        public string ReAssignBUHeadName { get; set; }
        public string ReAssignBUHeadMUDID { get; set; }
        public DateTime? ReAssignBUHeadApproveDate { get; set; }
        public string SpecialOrderReason { get; set; }    
        public int IsMealSame { get; set; }
        public string MealSame { get; set; }
        public int IsMeetingInfoSame { get; set; }
        public string MeetingInfoSame { get; set; }
        public string MeetingInfoSameReason { get; set; }
        public string IsWorkdayQuit { get; set; }
    }
    public class P_ORDER_Unfinished_VIEW_EXT
    {
        public string ApplierName { get; set; }
        public string ApplierMUDID { get; set; }
        public string Position { get; set; }
        public string HTCode { get; set; }
        public string Market { get; set; }
        public string TA { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string HospitalCode { get; set; }
        public string HospitalName { get; set; }
        public string CostCenter { get; set; }
        public string Channel { get; set; }
        public string ORDDeliverDate { get; set; }
        public string ORDDeliverTime { get; set; }
        public string RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public string ORDAttendCount { get; set; }
        public string totalFee { get; set; }
        public string IsRetuen { get; set; }
        public string cancelState { get; set; }
        public string cancelFeedback { get; set; }
        public string cancelFailReason { get; set; }
        public string IsRetuenSuccess { get; set; }
        public string ReceiveState { get; set; }
        public string ORDState { get; set; }
        public string RealPrice { get; set; }
        public string RealPriceChangeReason { get; set; }
        public string RealPriceChangeRemark { get; set; }
        public string RealCount { get; set; }
        public string RealCountChangeReason { get; set; }
        public string RealCountChangeRemrak { get; set; }
        public string IsOrderUpload { get; set; }
        public string PUOCreateDate { get; set; }
        public string PUOCreateTime { get; set; }
        public string PUOBUHeadName { get; set; }
        public string PUOBUHeadMUDID { get; set; }
        public string ApproveDate { get; set; }
        public string ApproveTime { get; set; }
        public string PUOState { get; set; }
        public string IsAttentSame { get; set; }
        public string AttentSame { get; set; }
        public string AttentSameReason { get; set; }
        public string IsReopen { get; set; }
        public string Reopen { get; set; }
        public string ReopenOperatorName { get; set; }
        public string ReopenOperatorMUDID { get; set; }
        public string ReopenOperateDate { get; set; }
        public string ReopenOperateTime { get; set; }
        public string ReopenOriginatorName { get; set; }
        public string ReopenOriginatorMUDID { get; set; }
        public string ReopenReason { get; set; }
        public string ReopenRemark { get; set; }
        public string ReopenState { get; set; }
        public string IsTransfer { get; set; }
        public string TransferOperatorName { get; set; }
        public string TransferOperatorMUDID { get; set; }
        public string TransferUserName { get; set; }
        public string TransferUserMUDID { get; set; }
        public string TransferOperateDate { get; set; }
        public string TransferOperateTime { get; set; }
        public string IsReAssign { get; set; }
        public string ReAssign { get; set; }
        public string ReAssignOperatorName { get; set; }
        public string ReAssignOperatorMUDID { get; set; }
        public string ReAssignBUHeadName { get; set; }
        public string ReAssignBUHeadMUDID { get; set; }
        public string ReAssignBUHeadApproveDate { get; set; }
        public string ReAssignBUHeadApproveTime { get; set; }
        public string SpecialOrderReason { get; set; }
        public string IsMealSame { get; set; }
        public string MealSame { get; set; }
        public string IsMeetingInfoSame { get; set; }
        public string MeetingInfoSame { get; set; }
        public string MeetingInfoSameReason { get; set; }
        public string IsWorkdayQuit { get; set; }
    }
}
