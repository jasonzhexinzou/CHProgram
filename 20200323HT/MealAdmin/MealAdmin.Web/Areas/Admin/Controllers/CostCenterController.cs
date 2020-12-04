using MealAdmin.Entity;
using MealAdmin.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XFramework.XInject.Attributes;

namespace MealAdmin.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 成本中心
    /// </summary>
    public class CostCenterController : AdminBaseController
    {

        [Bean("costCenterService")]
        public ICostCenterService costCenterService { get; set; }

        [Bean("costCenterManagerService")]
        public ICostCenterManagerService costCenterManagerService { get; set; }

        #region 成本中心
        /// <summary>
        /// 成本中心
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region 分页载入成本中心
        /// <summary>
        /// 分页载入成本中心
        /// </summary>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public JsonResult Load(string code, string name, int rows, int page)
        {
            int total = 0;
            var list = costCenterService.LoadPage(code, name, rows, page, out total);
            return Json(new { state = 1, rows = list , total = total});
        }
        #endregion

        #region 编辑成本中心
        /// <summary>
        /// 编辑成本中心
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(string id)
        {
            M_COSTCENTER costcenter = null;
            if (string.IsNullOrEmpty(id))
            {
                costcenter = new M_COSTCENTER();
            }
            else
            {
                costcenter = costCenterService.Find(Guid.Parse(id));
            }
            
            ViewBag.costcenter = costcenter;
            return View();
        }
        #endregion

        #region 保存修改
        /// <summary>
        /// 保存修改
        /// </summary>
        /// <param name="costcenter"></param>
        /// <returns></returns>
        public JsonResult Save(M_COSTCENTER costcenter)
        {
            costcenter.Modifier = CurrentAdminUser.ID;
            costcenter.ModifyDate = DateTime.Now;

            costCenterService.Save(costcenter);
            return Json(new { state = 1});
        }
        #endregion

        #region 删除成本中心
        /// <summary>
        /// 删除成本中心
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Remove(Guid id)
        {
            costCenterService.Del(id);
            return Json(new { state = 1 });
        }
        #endregion

        #region 成本中心审批人
        /// <summary>
        /// 成本中心审批人
        /// </summary>
        /// <param name="id">Cost Center ID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CostManager(Guid id)
        {
            ViewBag.costcenter = costCenterService.Find(id);
            return View();
        }
        #endregion

        #region 载入成本中心审批人列表
        /// <summary>
        /// 载入成本中心审批人列表
        /// </summary>
        /// <param name="CostID"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LoadCostManager(Guid CostID)
        {
            var list = costCenterManagerService.Load(CostID);
            return Json(new { state = 1, rows = list, total = list.Count});
        }
        #endregion

        #region 编辑成本中心审批人
        /// <summary>
        /// 编辑成本中心审批人
        /// </summary>
        /// <param name="id"></param>
        /// <param name="CostID"></param>
        /// <returns></returns>
        public ActionResult EditManager(Guid id, Guid CostID)
        {


            return View();
        }
        #endregion

    }
}