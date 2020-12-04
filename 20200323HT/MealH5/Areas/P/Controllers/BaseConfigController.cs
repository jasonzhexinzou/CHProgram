using MealAdminApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MealH5.Areas.P.Controllers
{
    public class BaseConfigController : Controller
    {
        #region 加载时间配置
        /// <summary>
        /// 加载时间配置
        /// </summary>
        /// <returns></returns>
        public ActionResult Time()
        {
            var channel = BaseDataClientChannelFactory.GetChannel();
            var config = channel.GetTimeConfig();

            config._3_1_SysWorkTimeBegin = TimeFormatter(config._3_1_SysWorkTimeBegin);
            config._3_2_SysWorkTimeEnd = TimeFormatter(config._3_2_SysWorkTimeEnd);
            config._4_1_ThatDayCanOrderMealTimeBegin = TimeFormatter(config._4_1_ThatDayCanOrderMealTimeBegin);
            config._4_2_ThatDayCanOrderMealTimeEnd = TimeFormatter(config._4_2_ThatDayCanOrderMealTimeEnd);
            config._5_1_ThatDayCanSendMealTimeBegin = TimeFormatter(config._5_1_ThatDayCanSendMealTimeBegin);
            config._5_2_ThatDayCanSendMealTimeEnd = TimeFormatter(config._5_2_ThatDayCanSendMealTimeEnd);
            config._6_1_NotThatDayCanSendMealTimeBegin = TimeFormatter(config._6_1_NotThatDayCanSendMealTimeBegin);
            config._6_2_NotThatDayCanSendMealTimeEnd = TimeFormatter(config._6_2_NotThatDayCanSendMealTimeEnd);
            config._11_1_SysOperableTimeBegin = TimeFormatter(config._11_1_SysOperableTimeBegin);
            config._11_2_SysOperableTimeEnd = TimeFormatter(config._11_2_SysOperableTimeEnd);
            config._12_1_PreApprovalCanApplyMeetingTimeSpanBegin= TimeFormatter(config._12_1_PreApprovalCanApplyMeetingTimeSpanBegin);
            config._12_2_PreApprovalCanApplyMeetingTimeSpanEnd = TimeFormatter(config._12_2_PreApprovalCanApplyMeetingTimeSpanEnd);
            config._13_1_PreApprovalNextWorkDaySendTimeSpanBegin = TimeFormatter(config._13_1_PreApprovalNextWorkDaySendTimeSpanBegin);
            config._13_2_PreApprovalNextWorkDaySendTimeSpanEnd = TimeFormatter(config._13_2_PreApprovalNextWorkDaySendTimeSpanEnd);
            config._14_1_PreWorkTimeSpanBegin = TimeFormatter(config._14_1_PreWorkTimeSpanBegin);
            config._14_2_PreWorkTimeSpanEnd = TimeFormatter(config._14_2_PreWorkTimeSpanEnd);
            config._15_1_PreOperableTimeSpanBegin = TimeFormatter(config._15_1_PreOperableTimeSpanBegin);
            config._15_2_PreOperableTimeSpanEnd = TimeFormatter(config._15_2_PreOperableTimeSpanEnd);
            config._16_1_UploadWorkTimeSpanBegin = TimeFormatter(config._16_1_UploadWorkTimeSpanBegin);
            config._16_2_UploadWorkTimeSpanEnd = TimeFormatter(config._16_2_UploadWorkTimeSpanEnd);
            config._17_1_UploadOperableTimeSpanBegin = TimeFormatter(config._17_1_UploadOperableTimeSpanBegin);
            config._17_2_UploadOperableTimeSpanEnd = TimeFormatter(config._17_2_UploadOperableTimeSpanEnd);

            config._18_1_AddAddressTimeSpanBegin = TimeFormatter(config._18_1_AddAddressTimeSpanBegin);
            config._18_2_AddAddressTimeSpanEnd = TimeFormatter(config._18_2_AddAddressTimeSpanEnd);
            config._19_1_ResubmitAddressTimeSpanBegin = TimeFormatter(config._19_1_ResubmitAddressTimeSpanBegin);
            config._19_2_ResubmitAddressTimeSpanEnd = TimeFormatter(config._19_2_ResubmitAddressTimeSpanEnd);
            config._20_1_UpdateAddressTimeSpanBegin = TimeFormatter(config._20_1_UpdateAddressTimeSpanBegin);
            config._20_2_UpdateAddressTimeSpanEnd = TimeFormatter(config._20_2_UpdateAddressTimeSpanEnd);

            var _7_OrderingBufferMin = TimeFormatter($"0:{config._7_OrderingBufferMin}");

            ViewBag._7_OrderingBufferMin = _7_OrderingBufferMin;
            return View(config);
        }
        #endregion

        private string TimeFormatter(string time)
        {
            var tis = time.Split(':').Select(a => Convert.ToInt32(a).ToString("00")).ToList();
            tis.Add("00");
            return string.Join(":", tis);
        }
    }
}