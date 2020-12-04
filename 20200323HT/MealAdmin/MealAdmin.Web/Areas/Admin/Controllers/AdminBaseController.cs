using MealAdmin.Entity;
using MealAdmin.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MealAdmin.Web.Areas.Admin.Controllers
{
    public class AdminBaseController : Controller
    {
        public IamPortal.AppLogin.AdminUser CurrentAdminUser
        {
            get { return Session[ConstantHelper.CurrentAdminUser] as IamPortal.AppLogin.AdminUser; }
            set { Session[ConstantHelper.CurrentAdminUser] = value; }
        }
    }
}