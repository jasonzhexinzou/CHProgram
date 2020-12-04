using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity.View
{
    public class P_ORDER_UPLOADORDER
    {
        public Guid ID { get; set; }
        public string ApplierName { get; set; }
        public string ApplierMUDID { get; set; }
        public DateTime ACreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string HTCode { get; set; }
        public string BUHeadName { get; set; }
        public string BUHeadMUDID { get; set; }
        public DateTime? BUHeadApproveDate { get; set; }
        public string ReAssignOperatorMUDID { get; set; }           //上传文件重新分配审批人-操作人MUDID
        public string ReAssignOperatorName { get; set; }            //上传文件重新分配审批人-操作人
        public bool IsReAssign { get; set; }                        //上传文件是否重新分配审批人
        public string ReAssignBUHeadName { get; set; }              //上传文件被重新分配审批人姓名
        public string ReAssignBUHeadMUDID { get; set; }             //上传文件被重新分配审批人MUDID
        public DateTime? ReAssignBUHeadApproveDate { get; set; }    //上传文件重新分配审批人时间
        public string RestaurantId { get; set; }   //
        public string RestaurantName { get; set; }   //
        public decimal TotalPrice { get; set; }   //预定金额
        public int FoodCount { get; set; }   //
        public int AttendCount { get; set; }   //用餐人数
        public string DeliveryAddress { get; set; }   //送餐地址
        public string Consignee { get; set; }   //收餐人
        public string Phone { get; set; }   //联系电话
        public DateTime DeliverTime { get; set; }   //送餐时间
        public string Remark { get; set; }   //备注
        public string RealCount { get; set; }   //实际用餐人数
        public string RealCountChangeReason { get; set; }   //用餐人数调整原因
        public string RealCountChangeRemrak { get; set; }   //用餐人数调整备注
        public string RealPrice { get; set; }   //用户确认金额
        public string RealPriceChangeReason { get; set; }   //确认金额调整原因
        public string RealPriceChangeRemark { get; set; }   //确认金额调整备注
        public string SpecialRemarksProjectTeam { get; set; }   //项目组特殊备注
        public int IsTransfer { get; set; }                     //上传文件是否重新分配
        public string TransferOperatorMUDID { get; set; }       //上传文件重新分配操作人MUDID
        public string TransferOperatorName { get; set; }        //上传文件重新分配操作人
        public string TransferUserMUDID { get; set; }           //上传文件被重新分配人MUDID
        public string TransferUserName { get; set; }            //上传文件被重新分配人姓名
        public DateTime? TransferOperateDate { get; set; }         //上传文件被重新分配时间



        public string ReopenOriginatorMUDID { get; set; }       //ReOpen发起人MUDID
        public string ReopenOriginatorName { get; set; }        //ReOpen发起人姓名

    }
}
