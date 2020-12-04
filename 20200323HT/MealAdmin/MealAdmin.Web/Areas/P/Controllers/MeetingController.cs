using MealAdmin.Service;
using MealAdmin.Web.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XFramework.XInject.Attributes;
using XFramework.XUtil;

namespace MealAdmin.Web.Areas.P.Controllers
{
    public class MeetingController : Controller
    {
        [Bean("meetingService")]
        public IMeetingService meetingService { get; set; }

        // GET: P/Meeting
        //[AdminSessionFilter(Order = 1)]
        //[PermissionFilter(Order = 2, Permission = "00000000-0000-0000-6000-000000000001")]
        public ActionResult Index()
        {
            return View();
        }

        //[AdminSessionFilter(Order = 1)]
        //[PermissionFilter(Order = 2, Permission = "00000000-0000-0000-6000-000000000001")]
        public JsonResult Load(string CN, string MUDID, string SubmitTimeBegin, string SubmitTimeEnd, string ApprovedTimeBegin, string ApprovedTimeEnd, int rows, int page)
        {
            LogHelper.Info(Request.RawUrl);

            DateTime _tmpTime;
            DateTime? _DTBegin, _DTEnd;
            if (DateTime.TryParse(SubmitTimeBegin, out _tmpTime) == true)
            {
                _DTBegin = _tmpTime;
            }
            else
            {
                _DTBegin = null;
            }
            if (DateTime.TryParse(SubmitTimeEnd, out _tmpTime) == true)
            {
                _DTEnd = _tmpTime.AddDays(1d);
            }
            else
            {
                _DTEnd = null;
            }
            DateTime tmpTime;
            DateTime? DTBegin, DTEnd;
            if (DateTime.TryParse(ApprovedTimeBegin, out tmpTime) == true)
            {
                DTBegin = tmpTime;
            }
            else
            {
                DTBegin = null;
            }
            if (DateTime.TryParse(ApprovedTimeEnd, out tmpTime) == true)
            {
                DTEnd = tmpTime.AddDays(1d);
            }
            else
            {
                DTEnd = null;
            }
            int total;
            var list = meetingService.LoadMeeting(CN, MUDID, _DTBegin, _DTEnd, DTBegin, DTEnd, rows, page, out total);

            return Json(new { state = 1, rows = list, total = total });

        }

    }
}