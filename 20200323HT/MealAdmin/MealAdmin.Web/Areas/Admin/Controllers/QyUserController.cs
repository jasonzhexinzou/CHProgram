using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MealAdmin.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 企业员工
    /// </summary>
    public class QyUserController : AdminBaseController
    {
        #region 转到企业员工选择页面
        /// <summary>
        /// 转到企业员工选择页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Choose()
        {
            return View();
        }
        #endregion

        #region 载入全部
        /// <summary>
        /// 载入全部
        /// </summary>
        /// <returns></returns>
        public JsonResult LoadALL()
        {

            return Json(new { state = 1});
        }
        #endregion



    }
}