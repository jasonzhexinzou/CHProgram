using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity.View
{
    public class P_ORDER_VIEW
    {
        public string ID { get; set; }
        public string State { get; set; }
        public string Return { get; set; }
        public string IsNormal { get; set; }
        public string SpecialOrder { get; set; }
        public string HTCode { get; set; }
        public string UserName { get; set; }
        public string MUDID { get; set; }
        public string Market { get; set; }
        public string TA { get; set; }
        public string Channel { get; set; }
        public string CallCenter { get; set; }
        public string AttendCount { get; set; }
        public string RealCount { get; set; }
        public string RealCountChangeReason { get; set; }
        public string RealCountChangeRemrak { get; set; }
        public string TotalPrice { get; set; }
        public string RealityPrice { get; set; }
        public string ChangeTotalPriceReason { get; set; }
        public string RealPrice { get; set; }
        public string RealPriceChangeReason { get; set; }
        public string RealPriceChangeRemark { get; set; }
        public string HospitalName { get; set; }
        public string HospitalId { get; set; }
        public string Address { get; set; }
        public string Consignee { get; set; }
        public string Phone { get; set; }
        public string DeliverTime { get; set; }
        public string MeetingName { get; set; }
        public string Remark { get; set; }

        public string IsTransfer { get; set; }                     //上传文件是否重新分配
        public string TransferOperatorMUDID { get; set; }       //上传文件重新分配操作人MUDID
        public string TransferOperatorName { get; set; }        //上传文件重新分配操作人
        public string TransferUserMUDID { get; set; }           //上传文件被重新分配人MUDID
        public string TransferUserName { get; set; }            //上传文件被重新分配人姓名
        public string TransferOperateDate { get; set; }         //上传文件被重新分配时间


        public string IsShowTransfer { get; set; }            //是否显示重新分配


    }
}
