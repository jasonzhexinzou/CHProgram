using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity
{
    /// <summary>
    /// 业务组成员
    /// </summary>
    public class P_GROUP_MEMBER
    {
        public Guid ID { get; set; }

        /// <summary>
        /// enum GroupTypeEnum
        /// </summary>
        public int GroupType { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public DateTime CreateDate { get; set; }

        public string CreateUserId { get; set; }

        public int State { get; set; }       //后台添加
        public int State1 { get; set; }      //48小时未收餐
        public int State2 { get; set; }      //7天未上传文件
        public int State3 { get; set; }       //7天未完成审批
        public string ServPauseType { get; set; }
    }

    public class P_ServPauser
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string HTCode { get; set; }
        public int ServPauseType { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate{get;set;}
        public string Memo { get; set; }
        public int State { get; set; }
    }


    public class P_ServPause_Detail
    {
        public string ApplierName { get; set; }
        public string ApplierMUDID { get; set; }
        public string Position { get; set; }
        public string ApplierMobile { get; set; }
        public string HTCode { get; set; }
        public string Market { get; set; }
        public string TA { get; set; }
        public DateTime DeliverDate { get; set; }
        public TimeSpan DeliverTime { get; set; }
        public string ReceiveState { get; set; }
        public DateTime ReceiveDate { get; set; }
        public TimeSpan ReceiveTime { get; set; }
        public string DMName { get; set; }
        public string DMMUDID { get; set; }
        public string RMName { get; set; }
        public string RMMUDID { get; set; }
        public string RDName { get; set; }
        public string RDMUDID { get; set; }
        public string IsOrderUpload { get; set; }
        public DateTime OrderUploadDate { get; set; }
        public TimeSpan OrderUploadTime { get; set; }
        public string BUHeadName { get; set; }
        public string BUHeadMUDID { get; set; }
        public DateTime BUHeadApproveDate { get; set; }
        public TimeSpan BUHeadApproveTime { get; set; }
        public string OrderUploadState { get; set; }
        public string IsReopen { get; set; }
        public DateTime ReopenOperateDate { get; set; }
        public TimeSpan ReopenOperateTime { get; set; }
        public string ReopenOrderUploadState { get; set; }
        public string ServPauseType { get; set; }
        public DateTime ServPauseCreateDate { get; set; }
        public TimeSpan ServPauseCreateTime { get; set; }
        public DateTime ServPauseModifyDate { get; set; }
        public TimeSpan ServPauseModifyTime { get; set; }
        public string ServPauseState { get; set; }

    }

    /// <summary>
    /// 组类别
    /// </summary>
    public enum GroupTypeEnum
    {
        /// <summary>
        /// 投诉组
        /// Complaints = 1
        /// </summary>
        Complaints = 1,
        /// <summary>
        /// MMCoe审批组
        /// MMCoE = 2
        /// </summary>
        MMCoE = 2,
        /// <summary>
        /// 简报接收组
        /// BriefReport = 3
        /// </summary>
        BriefReport = 3,
        /// <summary>
        /// HT暂停服务名单
        /// ServPause = 4
        /// </summary>
        ServPause = 4,
        /// <summary>
        /// 院外HT名单
        /// OutSideHT = 5
        /// </summary>
        OutSideHT = 5,
        /// <summary>
        /// non-HT暂停服务名单
        /// NServPause = 6
        /// </summary>
        NServPause = 6,
        /// <summary>
        /// 变更审批人操作组
        /// ChangeAAG = 7
        /// </summary>
        ChangeAAG = 7,
        /// <summary>
        /// 订单重新分配组
        /// RSGroup = 8
        /// </summary>
        RSGroup = 8,
        /// <summary>
        /// 成本中心管理组
        /// ChangeAAG = 8
        /// </summary>
        CCMGroup = 9
    }
}
