using MealAdmin.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XFramework.XException;

namespace MealAdmin.Web.Filter
{
    public class AdminSessionFilter : ActionFilterAttribute
    {
        #region 进入Action之前进行拦截
        /// <summary>
        /// 进入Action之前进行拦截
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpSessionStateBase Session = filterContext.HttpContext.Session;
            var adminUser = Session[ConstantHelper.CurrentAdminUser];
            if (adminUser == null)
            {
                throw new BusinessBaseException(BaseException.SESSIONTIMEOUT);
            }
        }
        #endregion
    }
}