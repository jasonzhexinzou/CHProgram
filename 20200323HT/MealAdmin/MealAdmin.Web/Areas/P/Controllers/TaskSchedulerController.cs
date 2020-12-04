using iPathFeast.API.Client;
using iPathFeast.ApiEntity;
using MealAdmin.Service;
using MealAdminApiClient;
using MeetingMealApiClient;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using XFramework.XInject.Attributes;
using XFramework.XUtil;

namespace MealAdmin.Web.Areas.P.Controllers
{
    public class TaskSchedulerController : Controller
    {
        // GET: P/TaskScheduler
        #region =============Service Instance
        [Bean("bizConfigService")]
        public IBizConfigService bizConfigService { get; set; }

        [Bean("baseDataService")]
        public IBaseDataService baseDataService { get; set; }

        [Bean("orderService")]
        public IOrderService orderService { get; set; }

        [Bean("groupMemberService")]
        public IGroupMemberService groupMemberService { get; set; }
        #endregion

        [Autowired]
        ApiV1Client apiClient { get; set; }

        #region --------------Hospital
        public ActionResult Hospital()
        {
            var _thisTime = DateTime.Now;
            var _time = GetTSTime(TaskScheduler.Hospital);
            TimeSpan timeSpan = _thisTime - _time;

            var conf = bizConfigService.GetConfig();
            double h = double.Parse(conf._10_SyncHospoitalIntervalHour.ToString());
            if (timeSpan.TotalHours > h)
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var rtnVal = baseDataService.SyncBaseData();
                        LogHelper.Info("auto sync Hospital result:" + rtnVal);
                        SetTSTime(TaskScheduler.Hospital, _thisTime);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("auto sync Hospital  ERR", ex);
                        throw ex;
                    }
                });
                return Content("sync Hospital success！the next time at" + _thisTime.AddHours(h).ToString("yyyy-MM-dd HH:mm:ss"));
            }
            else
            {
                return Content("sync Hospital not run！last run time at" + _time.ToString("yyyy-MM-dd HH:mm:ss") + " |conf._10_SyncHospoitalIntervalHour:" + conf._10_SyncHospoitalIntervalHour);
            }
        }
        #endregion

        #region --------------XMSReport
        public ActionResult XMSReport()
        {
            var conf = bizConfigService.GetConfig();

            var _thisTime = DateTime.Now;
            var _strToday = _thisTime.ToString("yyyy-MM-dd");
            var _strConfTime = _strToday + " " + conf._9_SyncXMSReportTime + ":00";
            DateTime _confTime;
            if (DateTime.TryParse(_strConfTime, out _confTime) == true)
            {
                var _time = GetTSTime(TaskScheduler.XMSReport);
                TimeSpan timeSpan = _thisTime - _time;
                if ((_time.DayOfYear < _thisTime.DayOfYear || _time.Year < _thisTime.Year) && _thisTime >= _confTime)
                {
                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            var channel = OrderApiClientChannelFactory.GetChannel();
                            var rtnVal = channel.SyncReport("xms");
                            LogHelper.Info("auto sync XMSReport result:" + rtnVal);
                            SetTSTime(TaskScheduler.XMSReport, _thisTime);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error("auto sync XMSReport ERR", ex);
                            throw ex;
                        }
                    });
                    return Content("sync XMSReport success！the next time at:" + _thisTime.AddDays(1d).ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else
                {
                    return Content("sync XMSReport not run, ！last run time at:" + _time.ToString("yyyy-MM-dd HH:mm:ss"));
                }
            }
            else
            {
                return Content("sync XMSReport not run, ！conf._9_SyncXMSReportTime Err:" + conf._9_SyncXMSReportTime);
            }

        }
        #endregion

        #region --------------SendBriefReport
        public ActionResult SendBriefReport()
        {
            var _thisTime = DateTime.Now;
            var _time = GetTSTime(TaskScheduler.SendBriefReport);
            var conf = bizConfigService.GetConfig();
            TimeSpan timeSpan = _thisTime - _time;
            LogHelper.Info("debug SendBriefReport | GetLastSendBriefTime():" + _time.ToString("yyyy-MM-dd HH:mm:ss")
                            + " | DateTime.Now : " + _thisTime.ToString("yyyy-MM-dd HH:mm:ss")
                            + " | timeSpan.TotalHours:" + timeSpan.TotalHours
                            + " | DateTime.Now.Hour:" + _thisTime.Hour
                            + " | conf._8_SendBriefReportHour:" + conf._8_SendBriefReportHour);
            if (_thisTime.Hour != conf._8_SendBriefReportHour)
            {
                return Content("【briefReport】 is not send, not send hour: " + conf._8_SendBriefReportHour + " | currentTime:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            else if (timeSpan.TotalHours < 2d)
            {
                return Content("【briefReport】 is not send, it has been sent at:" + _time.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            else
            {
                var arr = groupMemberService.GetGroupMembersByType(Entity.GroupTypeEnum.BriefReport).Select(s => s.UserId).ToArray();
                var touser = string.Join("|", arr);
                var brief = orderService.LoadBriefing(0);
                var cnt = "当日预申请审批通过数量:"+brief.TodayApprove+"(0元-"+brief.TodayApproveZero+"; 非0元-"+brief.TodayApproveNotZero+")"
                    + "\r\n明日配送订单量: " + brief.TomorrowDeliver + "(XMS-" + brief.TomorrowDeliverXms + ";\t BDS-" + brief.TomorrowDeliverBds + ")"
                    + "\r\n明日配送订单金额: RMB" + brief.TomorrowDeliverTotal.ToString("n") + "(XMS- RMB" + brief.TomorrowDeliverTotalXms.ToString("n") + ";\t BDS- RMB" + brief.TomorrowDeliverTotalBds.ToString("n") + ")"
                    + "\r\n明日配送订单,参会人数>=60人: " + brief.TomorrowAttendCount60
                    + "\r\n明日配送订单,预定金额>=1500元: " + brief.TomorrowExceed2000
                    + "\r\n当日上传文件审批通过数量:"+brief.TodayUpLoadThroughCount 
                    + "\r\n当日确认收餐数量:" + brief.TodayUpLoadThroughCount;
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var rtnVal = WxMessageClientChannelFactory.GetChannel().SendText(touser, cnt);
                        LogHelper.Info("auto send BriefReport Content：【" + JsonConvert.SerializeObject(brief) + "】, | toUser: 【" + touser + "】，| result:" + rtnVal);
                        SetTSTime(TaskScheduler.SendBriefReport, _thisTime);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("auto send BriefReport  ERR", ex);
                        throw ex;
                    }
                });


                var _brief = orderService.LoadBriefing(1);
                var _cnt = "当日预申请审批通过数量:" + brief.TodayApprove + "(0元-" + brief.TodayApproveZero + "; 非0元-" + brief.TodayApproveNotZero + ")"
                    + "\r\n明日配送订单量: " + brief.TomorrowDeliver + "(MXS-" + brief.TomorrowDeliverXms + ";\t BDS-" + brief.TomorrowDeliverBds + ")"
                    + "\r\n明日配送订单金额: RMB" + brief.TomorrowDeliverTotal.ToString("n") + "(XMS- RMB" + brief.TomorrowDeliverTotalXms.ToString("n") + ";\t BDS- RMB" + brief.TomorrowDeliverTotalBds.ToString("n") + ")"
                    + "\r\n明日配送订单,参会人数>=60人: " + brief.TomorrowAttendCount60
                    + "\r\n明日配送订单,预定金额>=1500元: " + brief.TomorrowExceed2000
                    + "\r\n当日上传文件审批通过数量:" + brief.TodayUpLoadThroughCount
                    + "\r\n当日确认收餐数量:" + brief.TodayUpLoadThroughCount;
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var rtnVal = WMessageClientChannelFactory.GetChannel().SendText(touser, _cnt);
                        LogHelper.Info("auto send BriefReport Content：【" + JsonConvert.SerializeObject(brief) + "】, | toUser: 【" + touser + "】，| result:" + rtnVal);
                        SetTSTime(TaskScheduler.SendBriefReport, _thisTime);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("auto send BriefReport  ERR", ex);
                        throw ex;
                    }
                });

                return Content("【briefReport】 success！the next exec time: " + _thisTime.AddDays(1d).ToString("yyyy-MM-dd HH:mm:ss"));
            }

        }
        #endregion

        #region --------------SysConfirmOrder
        public ActionResult SysConfirmOrder()
        {
            var _time = GetTSTime(TaskScheduler.SysConfirmOrder);
            var _thisTime = DateTime.Now;
            TimeSpan timeSpan = _thisTime - _time;
            if (timeSpan.TotalMinutes > 30d)
            {
                Task.Factory.StartNew(() =>
            {
                try
                {
                    var rtnVal = orderService.LoadOrders();
                    //var rtnVals = orderService.SystemConfirm();
                    LogHelper.Info("auto sync SysConfirmOrder result:" + rtnVal);
                    SetTSTime(TaskScheduler.SysConfirmOrder, _thisTime);
                    // 告诉小秘书已经收餐
                    //var oponapiChannel = OpenApiChannelFactory.GetChannel();
                    foreach (var item in rtnVal)
                    {
                        //oponapiChannel.finishOrder(item.XmsOrderId, "1", string.Empty);
                        var req = new FinishOrderReq()
                        {
                            _Channels = item.Channel,
                            iPathOrderId = item.XmsOrderId,
                            type = "1",
                            remark = string.Empty
                        };
                        apiClient.FinishOrder(req);
                        orderService.SystemConfirm(item.ID);
                    }
                }

                catch (Exception ex)
                {

                    LogHelper.Error("auto sync SysConfirmOrder  ERR", ex);
                    throw ex;
                }
            });

                return Content("sync SysConfirmOrder success！the next time at" + _thisTime.AddMinutes(30d).ToString("yyyy-MM-dd HH:mm:ss"));
            }
            else
            {
                return Content("sync SysConfirmOrder not run！last run time at" + _time.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }
        #endregion

        #region --------------BDSReport
        public ActionResult BDSReport()
        {
            var conf = bizConfigService.GetConfig();

            var _thisTime = DateTime.Now;
            var _strToday = _thisTime.ToString("yyyy-MM-dd");
            var _strConfTime = _strToday + " " + conf._9_SyncXMSReportTime + ":00";
            DateTime _confTime;
            if (DateTime.TryParse(_strConfTime, out _confTime) == true)
            {
                var _time = GetTSTime(TaskScheduler.BDSReport);
                TimeSpan timeSpan = _thisTime - _time;
                if ((_time.DayOfYear < _thisTime.DayOfYear || _time.Year < _thisTime.Year) && _thisTime >= _confTime)
                {
                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            var channel = OrderApiClientChannelFactory.GetChannel();
                            var rtnVal = channel.SyncReport("bds");
                            LogHelper.Info("auto sync BDSReport result:" + rtnVal);
                            SetTSTime(TaskScheduler.BDSReport, _thisTime);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error("auto sync BDSReport ERR", ex);
                            throw ex;
                        }
                    });
                    return Content("sync BDSReport success！the next time at:" + _thisTime.AddDays(1d).ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else
                {
                    return Content("sync BDSReport not run, ！last run time at:" + _time.ToString("yyyy-MM-dd HH:mm:ss"));
                }
            }
            else
            {
                return Content("sync BDSReport not run, ！conf._9_SyncBDSReportTime Err:" + conf._9_SyncXMSReportTime);
            }

        }
        #endregion

        #region TaskScheduler Time Method
        private static DateTime lastUpdateHospitalTime = DateTime.MinValue;
        private static DateTime lastUpdateXMSReportTime = DateTime.MinValue;
        private static DateTime lastSendBriefReportTime = DateTime.MinValue;
        private static DateTime lastUpdateSysConfirmOrderTime = DateTime.MinValue;
        private static DateTime lastUpdateBDSReportTime = DateTime.MinValue;

        private DateTime GetTSTime(TaskScheduler Type)
        {
            if (Type == TaskScheduler.Hospital)
            {
                return _getTSTime("~/TaskScheduler_hsp.fzd", ref lastUpdateHospitalTime);
            }
            else if (Type == TaskScheduler.SendBriefReport)
            {
                return _getTSTime("~/TaskScheduler_sbt.fzd", ref lastSendBriefReportTime);
            }
            else if (Type == TaskScheduler.SysConfirmOrder)
            {
                return _getTSTime("~/TaskScheduler_sco.fzd", ref lastUpdateSysConfirmOrderTime);
            }
            else if (Type == TaskScheduler.XMSReport)
            {
                return _getTSTime("~/TaskScheduler_xms_rpt.fzd", ref lastUpdateXMSReportTime);
            }
            else if (Type == TaskScheduler.BDSReport)
            {
                return _getTSTime("~/TaskScheduler_bds_rpt.fzd", ref lastUpdateBDSReportTime);
            }
            return DateTime.MinValue;
        }

        private void SetTSTime(TaskScheduler Type, DateTime Time)
        {
            string fileUrl;
            if (Type == TaskScheduler.Hospital)
            {
                lastUpdateHospitalTime = Time;
                fileUrl = "~/TaskScheduler_hsp.fzd";
            }
            else if (Type == TaskScheduler.SendBriefReport)
            {
                lastSendBriefReportTime = Time;
                fileUrl = "~/TaskScheduler_sbt.fzd";
            }
            else if (Type == TaskScheduler.SysConfirmOrder)
            {
                lastUpdateSysConfirmOrderTime = Time;
                fileUrl = "~/TaskScheduler_sco.fzd";
            }
            else if (Type == TaskScheduler.XMSReport)
            {
                lastUpdateXMSReportTime = Time;
                fileUrl = "~/TaskScheduler_xms_rpt.fzd";
            }
            else if (Type == TaskScheduler.BDSReport)
            {
                lastUpdateBDSReportTime = Time;
                fileUrl = "~/TaskScheduler_bds_rpt.fzd";
            }
            else
            {
                fileUrl = string.Empty;
            }
            if (fileUrl != string.Empty)
            {
                _setTSTime(fileUrl, Time);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FileUrl">ps: "~/TaskScheduler_sbt.fzd"</param>
        /// <param name="CacheTime">ps: lastSendBriefReportTime</param>
        /// <returns></returns>
        private DateTime _getTSTime(string FileUrl, ref DateTime CacheTime)
        {
            if (CacheTime == DateTime.MinValue)
            {
                var _url = Server.MapPath(FileUrl);
                DateTime _fileTime;
                using (System.IO.FileStream fs = new System.IO.FileStream(_url, System.IO.FileMode.OpenOrCreate))
                {
                    string _sbtStr;
                    System.IO.StreamReader sr = new System.IO.StreamReader(fs, Encoding.UTF8);
                    fs.Seek(0, System.IO.SeekOrigin.Begin);
                    _sbtStr = sr.ReadToEnd();

                    if (DateTime.TryParse(_sbtStr, out _fileTime) == false)
                    {
                        _fileTime = DateTime.MinValue;

                        fs.Seek(0, System.IO.SeekOrigin.Begin);
                        System.IO.StreamWriter sw = new System.IO.StreamWriter(fs, Encoding.UTF8);
                        sw.Write(_fileTime.ToString("yyyy-MM-dd HH:mm:ss"));
                        sw.Flush();
                    }
                }
                CacheTime = _fileTime;
            }
            return CacheTime;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FileUrl">ps: "~/TaskScheduler_sbt.fzd"</param>
        /// <param name="CacheTime">ps: lastSendBriefReportTime</param>
        private void _setTSTime(string FileUrl, DateTime CacheTime)
        {
            var _url = Server.MapPath(FileUrl);
            using (System.IO.FileStream fs = new System.IO.FileStream(_url, System.IO.FileMode.OpenOrCreate))
            {
                fs.Seek(0, System.IO.SeekOrigin.Begin);
                System.IO.StreamWriter sw = new System.IO.StreamWriter(fs, Encoding.UTF8);
                sw.Write(CacheTime.ToString("yyyy-MM-dd HH:mm:ss"));
                sw.Flush();
            }
        }
        #endregion

    }

    public enum TaskScheduler
    {
        /// <summary>
        /// 同步医院 
        /// </summary>
        Hospital = 1,
        /// <summary>
        /// 小秘书日报接口 
        /// </summary>
        XMSReport = 2,
        /// <summary>
        /// 发送简报 
        /// </summary>
        SendBriefReport = 3,
        /// <summary>
        /// 系统收餐 
        /// </summary>
        SysConfirmOrder = 4,
        /// <summary>
        /// 商宴通日报接口 
        /// </summary>
        BDSReport = 5
    }


}