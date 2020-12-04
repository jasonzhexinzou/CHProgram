using MealAdmin.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace System.Web.Mvc
{
    public static class PermissionHelper
    {
        public static bool IsPermission(this WebViewPage viewPage, string Permission)
        {
            List<string> listPermission = viewPage.Session[ConstantHelper.CurrentPermission] as List<string>;
            if (listPermission == null || listPermission.Count < 1)
            {
                return false;
            }
            if (listPermission == null || !listPermission.Contains(Permission))
            {
                return false;
            }
            return true;
        }

        #region 判断是否有 成本中心查询 权限
        /// <summary>
        /// 判断是否有 成本中心查询 权限
        /// </summary>
        /// <param name="viewPage"></param>
        /// <param name="Permission"></param>
        /// <returns></returns>
        public static bool IsPermissionCc(this WebViewPage viewPage)
        {
            var sessionUser = viewPage.Session[ConstantHelper.CurrentAdminUser] as IamPortal.AppLogin.AdminUser;
            var userName = sessionUser.Email;
            return true;
        }
        #endregion
    }
}