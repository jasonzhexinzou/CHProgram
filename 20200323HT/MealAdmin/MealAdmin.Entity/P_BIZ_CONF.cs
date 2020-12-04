using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity
{
    public class P_BIZ_CONF
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Val1 { get; set; }

        public string Val2 { get; set; }

        public string Val3 { get; set; }
    }

    public enum BizConfID
    {
        /// <summary>
        /// 人均标准 60 元
        /// </summary>
        ApprovePeopleNum = 1,
        /// <summary>
        /// MMCoE审批标准 60 人
        /// </summary>
        ApprovePercapitaPrice = 2,
        /// <summary>
        /// 系统工作时间 9:00 - 18:00
        /// </summary>
        SysWorkTimeSpan = 3,
        /// <summary>
        /// 可预订当日下午送餐的操作时间段 9:00  - 10:30
        /// </summary>
        ThatDayCanOrderMealTimeSpan = 4,
        /// <summary>
        /// 当日预定下午可送餐时间段 12:30 - 20:30
        /// </summary>
        ThatDayCanSendMealTimeSpan = 5,
        /// <summary>
        /// 非当日预定可送餐的时间段 8:00 - 20:30
        /// </summary>
        NotThatDayCanSendMealTimeSpan = 6,
        /// <summary>
        /// 订餐缓冲时间 15 分钟
        /// </summary>
        OrderingBufferMin = 7,

        ///// <summary>
        ///// 医院同步间隔 2 小时
        ///// </summary>
        //SyncHospitalTimeInterval = 8,
        ///// <summary>
        ///// XXXXX 同步日报间隔 24小时
        ///// </summary>
        //SyncDateReportTimeInterval = 9,

        /// <summary>
        /// 简报发送时间 22
        /// </summary>
        SendBriefReportHour = 8,

        /// <summary>
        /// 执行同步小秘书日报时间 21:30 
        /// </summary>
        SyncXMSReportTime = 9,

        /// <summary>
        /// 执行同步医院间隔小时 
        /// </summary>
        SyncHospoitalIntervalHour = 10,

        /// <summary>
        /// 修改/退订可操作时间
        /// </summary>
        SysOperableTime = 11,
        /// <summary>
        /// 预申请可申请会议时间
        /// </summary>
        PreApprovalCanApplyMeetingTimeSpan = 12,
        /// <summary>
        /// 预申请可申请隔日会议时间
        /// </summary>
        PreApprovalNextWorkDaySendTimeSpan = 13,
        /// <summary>
        /// 预申请工作时间
        /// </summary>
        PreWorkTimeSpan = 14,
        /// <summary>
        /// 预申请修改工作时间
        /// </summary>
        PreOperableTimeSpan = 15,
        /// <summary>
        /// 上传文件工作时间
        /// </summary>
        UploadWorkTimeSpan = 16,
        /// <summary>
        /// 上传文件修改工作时间
        /// </summary>
        UploadOperableTimeSpan = 17,

        /// <summary>
        /// 新增地址工作时间
        /// </summary>
        AddAddressTimeSpan = 18,
        /// <summary>
        /// 重新提交地址工作时间
        /// </summary>
        ResubmitAddressTimeSpan = 19,
        /// <summary>
        /// 修改地址工作时间
        /// </summary>
        UpdateAddressTimeSpan = 20,

    }

    public class P_BIZ_CONF_OBJ
    {
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        /// <summary>
        /// 1审批人数限制 
        /// </summary>
        public int _1_ApprovePeopleNum { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        /// <summary>
        /// 2审批单价设置
        /// </summary>
        public decimal _2_ApprovePercapitaPrice { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        /// <summary>
        /// 工作时间开始时间
        /// </summary>
        public string _3_1_SysWorkTimeBegin { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        /// <summary>
        /// 工作时间结束时间
        /// </summary>
        public string _3_2_SysWorkTimeEnd { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        /// <summary>
        /// 可预订当日下午送餐的操作时间段 9:00  - 10:30
        /// </summary>
        public string _4_1_ThatDayCanOrderMealTimeBegin { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        /// <summary>
        /// 可预订当日下午送餐的操作时间段 9:00  - 10:30
        /// </summary>
        public string _4_2_ThatDayCanOrderMealTimeEnd { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        /// <summary>
        /// 当日预定下午可送餐时间段 12:30 - 20:30
        /// </summary>
        public string _5_1_ThatDayCanSendMealTimeBegin { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        /// <summary>
        /// 当日预定下午可送餐时间段 12:30 - 20:30
        /// </summary>
        public string _5_2_ThatDayCanSendMealTimeEnd { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        /// <summary>
        /// 非当日预定可送餐的时间段 8:00 - 20:30
        /// </summary>
        public string _6_1_NotThatDayCanSendMealTimeBegin { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        /// <summary>
        /// 非当日预定可送餐的时间段 8:00 - 20:30
        /// </summary>
        public string _6_2_NotThatDayCanSendMealTimeEnd { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        /// <summary>
        /// 订餐缓冲时间 15 分钟
        /// </summary>
        public int _7_OrderingBufferMin { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        /// <summary>
        /// 简报发送时间 22小时
        /// </summary>
        public int _8_SendBriefReportHour { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        /// <summary>
        /// 执行同步小秘书日报时间 21:30 
        /// </summary>
        public string _9_SyncXMSReportTime { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        /// <summary>
        /// 执行同步医院间隔小时 2小时
        /// </summary>
        public int _10_SyncHospoitalIntervalHour { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        /// <summary>
        /// 修改/退订可操作时间开始时间
        /// </summary>
        public string _11_1_SysOperableTimeBegin { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        /// <summary>
        /// 修改/退订可操作时间结束时间
        /// </summary>
        public string _11_2_SysOperableTimeEnd { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        /// <summary>
        /// 预申请可申请会议时间开始时间
        /// </summary>
        public string _12_1_PreApprovalCanApplyMeetingTimeSpanBegin { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        /// <summary>
        /// 预申请可申请会议时间结束时间
        /// </summary>
        public string _12_2_PreApprovalCanApplyMeetingTimeSpanEnd { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        /// <summary>
        /// 预申请可申请隔日会议开始时间
        /// </summary>
        public string _13_1_PreApprovalNextWorkDaySendTimeSpanBegin { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        /// <summary>
        /// 预申请可申请隔日会议结束时间
        /// </summary>
        public string _13_2_PreApprovalNextWorkDaySendTimeSpanEnd { get; set; }
        /// <summary>
        /// 预申请工作时间开始时间
        /// </summary>
        public string _14_1_PreWorkTimeSpanBegin { get; set; }
        /// <summary>
        /// 预申请工作时间结束时间
        /// </summary>
        public string _14_2_PreWorkTimeSpanEnd { get; set; }
        /// <summary>
        /// 预申请修改工作时间开始时间
        /// </summary>
        public string _15_1_PreOperableTimeSpanBegin { get; set; }
        /// <summary>
        /// 预申请修改工作时间结束时间
        /// </summary>
        public string _15_2_PreOperableTimeSpanEnd { get; set; }
        /// <summary>
        /// 上传文件工作时间开始时间
        /// </summary>
        public string _16_1_UploadWorkTimeSpanBegin { get; set; }
        /// <summary>
        /// 上传文件工作时间结束时间
        /// </summary>
        public string _16_2_UploadWorkTimeSpanEnd { get; set; }
        /// <summary>
        /// 上传文件修改工作时间开始时间
        /// </summary>
        public string _17_1_UploadOperableTimeSpanBegin { get; set; }
        /// <summary>
        /// 上传文件修改工作时间结束时间
        /// </summary>
        public string _17_2_UploadOperableTimeSpanEnd { get; set; }


        /// <summary>
        /// 新增外送地址开始时间
        /// </summary>
        public string _18_1_AddAddressTimeSpanBegin { get; set; }
        /// <summary>
        /// 新增外送地址结束时间
        /// </summary>
        public string _18_2_AddAddressTimeSpanEnd { get; set; }
        /// <summary>
        /// 新增外送地址开始时间
        /// </summary>
        public string _19_1_ResubmitAddressTimeSpanBegin { get; set; }
        /// <summary>
        /// 新增外送地址结束时间
        /// </summary>
        public string _19_2_ResubmitAddressTimeSpanEnd { get; set; }
        /// <summary>
        /// 新增外送地址开始时间
        /// </summary>
        public string _20_1_UpdateAddressTimeSpanBegin { get; set; }
        /// <summary>
        /// 新增外送地址结束时间
        /// </summary>
        public string _20_2_UpdateAddressTimeSpanEnd { get; set; }
    }
}
