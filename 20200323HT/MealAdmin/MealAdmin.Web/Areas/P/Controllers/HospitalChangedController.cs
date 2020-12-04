using MealAdmin.Service;
using MeetingMealApiClient;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using XFramework.XInject.Attributes;
using MealAdmin.Entity;

namespace MealAdmin.Web.Areas.P.Controllers
{
    public class HospitalChangedController : Controller
    {
        [Bean("hospitalService")]
        public IHospitalService hospitalService { get; set; }

        #region 获取医院数据变量报告接口
        /// <summary>
        /// 获取医院数据变量报告接口
        /// </summary>
        /// <returns></returns>
        public JsonResult SyncHospitalChangedXMS()
        {
            var time = DateTime.Now;
            var channel = OpenApiChannelFactory.GetChannel();
            // XMS 获取医院数据变量报告接口
            var resXMS = channel.SyncHospitalChangedXMS();
            //// BDS 获取医院数据变量报告接口
            //var resBDS = channel.SyncHospitalChanged();
            return Json(new { state = 1 });
        }

        #endregion

        #region 获取医院数据变量报告接口
        /// <summary>
        /// 获取医院数据变量报告接口
        /// </summary>
        /// <returns></returns>
        public JsonResult SyncHospitalChangedBDS()
        {
            var time = DateTime.Now;
            var channel = OpenApiChannelFactory.GetChannel();
            //// XMS 获取医院数据变量报告接口
            //var resXMS = channel.SyncHospitalChangedXMS();
            // BDS 获取医院数据变量报告接口
            var resBDS = channel.SyncHospitalChanged();
            return Json(new { state = 1 });
        }

        #endregion

    }
}