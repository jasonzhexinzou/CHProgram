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
    public class ProvinceController : Controller
    {
        [Bean("baseDataService")]
        public IBaseDataService baseDataService { get; set; }

        [Bean("provinceService")]
        public IProvinceService provinceService { get; set; }

        // GET: P/Province
        [AdminSessionFilter(Order = 1)]
        [PermissionFilter(Order = 2, Permission = "00000000-0000-0000-5000-000000000001")]
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        //[AdminSessionFilter(Order = 1)]
        //[PermissionFilter(Order = 2, Permission = "00000000-0000-0000-2000-000000000002")]
        public JsonResult Load(int rows, int page)
        {
            int total;
            var list = provinceService.Load(rows,page,out total);
            return Json(new { state = 1, rows = list, total = total });
        }

        [HttpPost]
        //[AdminSessionFilter(Order = 1)]
        //[PermissionFilter(Order = 2, Permission = "00000000-0000-0000-2000-000000000002")]
        public JsonResult Add()
        {
            return Json(new { state=1});
        }
    }
}