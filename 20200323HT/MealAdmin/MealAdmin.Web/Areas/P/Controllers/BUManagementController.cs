using MealAdmin.Service;
using MealAdmin.Web.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XFramework.XInject.Attributes;

namespace MealAdmin.Web.Areas.P.Controllers
{
    public class BUManagementController : AdminBaseController
    {
        //注入管理组 Service
        [Bean("BUManagementService")]
        public IBUManagementService buManagementService { get; set; }

        [Bean("userInfoService")]
        public IUserInfoService IUserInfoService { get; set; }

        // GET: P/BUManagement
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult BUIndex()
        {
            return View();
        }

        public ActionResult EditBU(string ID)
        {
            ViewBag.ID = ID;
            return View();
        }

        public ActionResult EditTA(string ID)
        {
            ViewBag.ID = ID;
            return View();
        }

        [HttpPost]
        //[AdminSessionFilter(Order = 1)]
        //[PermissionFilter(Order = 2, Permission = "00000000-0000-0000-2000-000000000001")]
        [OperationAuditFilter(Operation = "查询BU数据", OperationAuditTypeName = "查询BU数据")]
        public JsonResult LoadBUInfo()
        {
            int total;
            var list = buManagementService.LoadBUInfo();
            return Json(new { state = 1, rows = list });
        }

        [HttpPost]
        //[AdminSessionFilter(Order = 1)]
        //[PermissionFilter(Order = 2, Permission = "00000000-0000-0000-2000-000000000001")]
        [OperationAuditFilter(Operation = "查询TA数据", OperationAuditTypeName = "查询TA数据")]
        public JsonResult LoadTAInfo()
        {
            int total;
            var list = buManagementService.LoadTAInfo();
            return Json(new { state = 1, rows = list });
        }

        [HttpPost]
        public JsonResult DelBUInfoByID(string ID)
        {
            var res = buManagementService.DelBUInfoByID(Guid.Parse(ID));
            return Json(new { state = 1, rows = res });
        }


        [HttpPost]
        public JsonResult GetBUInfoByID(string ID)
        {
            var res = buManagementService.GetBUInfoByID(Guid.Parse(ID));
            return Json(new { state = 1, data = res });
        }

        [HttpPost]
        public JsonResult SaveBU(string ID, string BUName, string BUHead, string BUHeadMudid)
        {
            int res = 0;
            if (string.IsNullOrEmpty(ID))
            {
                res = buManagementService.AddBUInfo(BUName, BUHead, BUHeadMudid);
            }
            else
            {
                res = buManagementService.UpdateBUInfo(Guid.Parse(ID), BUName, BUHead, BUHeadMudid);
            }
            return Json(new { state = 1 });
        }

        [HttpPost]
        public JsonResult DelTAInfoByID(string ID)
        {
            var res = buManagementService.DelTAInfoByID(Guid.Parse(ID));
            return Json(new { state = 1, rows = res });
        }


        [HttpPost]
        public JsonResult GetTAInfoByID(string ID)
        {
            var res = buManagementService.GetTAInfoByID(Guid.Parse(ID));
            return Json(new { state = 1, data = res });
        }

        [HttpPost]
        public JsonResult SaveTA(string ID, string TerritoryTA, string TerritoryHead, string TerritoryHeadName, string BU)
        {
            var res = buManagementService.UpdateTAInfo(Guid.Parse(ID), TerritoryTA, TerritoryHead, TerritoryHeadName, BU);
            return Json(new { state = 1 });
        }

        #region 自动添加姓名
        [HttpPost]
        public JsonResult ShowName(string Mudid)
        {

            var res = IUserInfoService.Find(Mudid);
            if (res != null)
            {
                return Json(new { state = 1, ishave = 1, res = res });
            }
            return Json(new { state = 1, ishave = 0, txt = "不存在此MUDID" });
        }
        #endregion
    }
}