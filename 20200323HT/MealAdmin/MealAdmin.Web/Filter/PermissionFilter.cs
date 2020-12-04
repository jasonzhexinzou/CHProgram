using MealAdmin.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XFramework.XException;

namespace MealAdmin.Web.Filter
{
    public class PermissionFilter : ActionFilterAttribute
    {
        public string Permission { get; set; }
        public string[] Permissions { get; set; }

        #region 进入Action之前进行拦截
        /// <summary>
        /// 进入Action之前进行拦截
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpSessionStateBase Session = filterContext.HttpContext.Session;
            List<string> listPermission = Session[ConstantHelper.CurrentPermission] as List<string>;

            havePermission(listPermission);
        }

        /// <summary>
        /// 判断是否拥有访问这个功能的授权
        /// </summary>
        /// <param name="listPermission"></param>
        private void havePermission(List<string> listPermission)
        {
            if (listPermission == null || listPermission.Count < 1)
            {
                // 没权限，不可访问
                throw new BusinessBaseException(ExceptionCode.NoPermission);
            }
            if (!string.IsNullOrEmpty(Permission))
            {
                if (!listPermission.Contains(Permission))
                {
                    // 没权限，不可访问
                    throw new BusinessBaseException(ExceptionCode.NoPermission);
                }
            }
            else
            {
                if (listPermission.Where(a => Permissions.Contains(a)).Count() < 1)
                {
                    // 没权限，不可访问
                    throw new BusinessBaseException(ExceptionCode.NoPermission);
                }
            }
        }
        #endregion
    }
}