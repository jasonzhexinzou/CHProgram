using IamPortal.AppLogin;
using MealAdmin.Service;
using MealAdmin.Util;
using MealAdmin.Web.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XFramework.XException;
using XFramework.XInject.Attributes;
using XFramework.XUtil;

namespace MealAdmin.Web.Areas.P.Controllers
{
    public class HomeController : AdminBaseController
    {
        //注入管理组 Service
        [Bean("groupMemberService")]
        public IGroupMemberService groupMemberService { get; set; }
        // GET: P/Home

        #region 后台首页
        /// <summary>
        /// 后台首页
        /// </summary>
        /// <returns></returns>
        public ContentResult Index()
        {
            return Content("<h1>It's working!</h1>");
        }
        #endregion

        #region 预登录
        /// <summary>
        /// 预登录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public RedirectResult PerLogin()
        {
            Session.Clear();

            var random = Guid.NewGuid().ToString();
            Session["LoginRandomStr"] = random;

            var WASTTest = System.Configuration.ConfigurationManager.AppSettings["WASTTest"];
            if ("1".Equals(WASTTest))
            {
                return Redirect(WebConfig.AppLoginServer + "/" + random);
            }
            else
            {
                return Redirect(WebConfig.AppLoginServer + "?random=" + random);
            }
        }
        #endregion

        #region 登录
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="security"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(string security)
        {

            var random = Session["LoginRandomStr"] as string;

            var loginUserInfo = LoginHelper.DecodingUserInfo(security, WebConfig.AppLoginSecret);
            if (loginUserInfo.random != random)
            {
                LogHelper.Info("随机数不相等");
                throw new BusinessBaseException(ExceptionCode.NoPermission);
            }

            var admin = LoginHelper.FindAdminUser(loginUserInfo.userid, WebConfig.AppLoginSecret, WebConfig.IamServer);
            LogHelper.Info("admin=" + admin);
            if (admin != null && admin.ListPermissions.Contains(WebConfig.IamAppID))
            {
                LogHelper.Info("当前登录人信息准确");
                //查询成本中心管理组
                var CCMGroup = groupMemberService.GetGroupMembersByType(Entity.GroupTypeEnum.CCMGroup);
                //判断管理组中是否存在此人
                if (CCMGroup.Find(a => a.UserId == admin.Email) != null)
                {
                    admin.ListPermissions.Add("CCMGroupUser");
                }
                //查询订单重新分配管理组
                var RSGroup = groupMemberService.GetGroupMembersByType(Entity.GroupTypeEnum.RSGroup);
                //判断管理组中是否存在此人
                if (RSGroup.Find(a => a.UserId == admin.Email) != null)
                {
                    admin.ListPermissions.Add("RSGroupUser");
                }
                //查询变更审批人管理组
                var ChangeAAG = groupMemberService.GetGroupMembersByType(Entity.GroupTypeEnum.ChangeAAG);
                //判断管理组中是否存在此人
                if (ChangeAAG.Find(a => a.UserId == admin.Email) != null)
                {
                    admin.ListPermissions.Add("ChangeAAGUser");
                }
                CurrentAdminUser = admin;
                Session[ConstantHelper.CurrentPermission] = admin.ListPermissions;
                return RedirectToAction("Main");
            }
            else
            {
                LogHelper.Info("没有当前登陆人信息");
                throw new BusinessBaseException(ExceptionCode.NoPermission);
            }

        }
        #endregion

        #region 预登录
        /// <summary>
        /// 预登录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public RedirectResult PerLoginCost()
        {
            Session.Clear();

            var random = Guid.NewGuid().ToString();
            Session["LoginRandomStr"] = random;

            var WASTTest = System.Configuration.ConfigurationManager.AppSettings["WASTTest"];
            if ("1".Equals(WASTTest))
            {
                return Redirect(WebConfig.AppLoginServerCost + "/" + random);
            }
            else
            {
                return Redirect(WebConfig.AppLoginServerCost + "?random=" + random);
            }
        }
        #endregion

        #region 登录
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="security"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LoginCost(string security)
        {

            var random = Session["LoginRandomStr"] as string;

            var loginUserInfo = LoginHelper.DecodingUserInfo(security, WebConfig.AppLoginSecret);
            if (loginUserInfo.random != random)
            {
                LogHelper.Info("随机数不相等");
                throw new BusinessBaseException(ExceptionCode.NoPermission);
            }

            var admin = LoginHelper.FindAdminUser(loginUserInfo.userid, WebConfig.AppLoginSecret, WebConfig.IamServer);
            LogHelper.Info("admin=" + admin);
            if (admin != null && admin.ListPermissions.Contains(WebConfig.IamAppID))
            {
                LogHelper.Info("当前登录人信息准确");
                //查询成本中心管理组
                var CCMGroup = groupMemberService.GetGroupMembersByType(Entity.GroupTypeEnum.CCMGroup);
                //判断管理组中是否存在此人
                if (CCMGroup.Find(a => a.UserId == admin.Email) != null)
                {
                    admin.ListPermissions.Add("CCMGroupUser");
                }
                //查询订单重新分配管理组
                var RSGroup = groupMemberService.GetGroupMembersByType(Entity.GroupTypeEnum.RSGroup);
                //判断管理组中是否存在此人
                if (RSGroup.Find(a => a.UserId == admin.Email) != null)
                {
                    admin.ListPermissions.Add("RSGroupUser");
                }
                //查询变更审批人管理组
                var ChangeAAG = groupMemberService.GetGroupMembersByType(Entity.GroupTypeEnum.ChangeAAG);
                //判断管理组中是否存在此人
                if (ChangeAAG.Find(a => a.UserId == admin.Email) != null)
                {
                    admin.ListPermissions.Add("ChangeAAGUser");
                }
                CurrentAdminUser = admin;
                Session[ConstantHelper.CurrentPermission] = admin.ListPermissions;
                return RedirectToAction("CostMain");
            }
            else
            {
                LogHelper.Info("没有当前登陆人信息");
                throw new BusinessBaseException(ExceptionCode.NoPermission);
            }

        }
        #endregion

        #region 后台主页
        /// <summary>
        /// 后台主页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AdminSessionFilter]
        public ActionResult Main()
        {
            return View("PMain");
        }
        #endregion

        #region 后台主页
        /// <summary>
        /// 后台主页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Main2()
        {
            CurrentAdminUser = new AdminUser()
            {
                Email = "user01.admin",
                Name = "Admin",
                ListPermissions = new List<string>()
                {
                    "00000000-0000-0000-1000-000000000000",
                    "00000000-0000-0000-1000-000000000001",
                    "00000000-0000-0000-1000-000000000002",
                    "00000000-0000-0000-1000-000000000003",
                    "00000000-0000-0000-1000-000000000004",

                    "00000000-0000-0000-2000-000000000000",
                    "00000000-0000-0000-2000-000000000001",
                    "00000000-0000-0000-2000-000000000002",
                    "00000000-0000-0000-2000-000000000003",

                    "00000000-0000-0000-3000-000000000000",
                    "00000000-0000-0000-3000-000000000001",
                    "00000000-0000-0000-3000-000000000002",

                    "00000000-0000-0000-4000-000000000000",
                    "00000000-0000-0000-4000-000000000001",
                    "00000000-0000-0000-4000-000000000002",
                    "00000000-0000-0000-4000-000000000003",
                    "00000000-0000-0000-4000-000000000004",

                    "00000000-0000-0000-5000-000000000000",
                    "00000000-0000-0000-5000-000000000001",
                    "00000000-0000-0000-5000-000000000004",
                    "00000000-0000-0000-5000-000000000005",
                    "00000000-0000-0000-5000-000000000006",
                    "00000000-0000-0000-5000-000000000007",

                    "00000000-0000-0000-6000-000000000000",
                    "00000000-0000-0000-6000-000000000001",

                    "00000000-0000-0000-7000-000000000000",
                    "00000000-0000-0000-7000-000000000001",
                    "00000000-0000-0000-7000-000000000002",
                    "00000000-0000-0000-7000-000000000003",
                    "00000000-0000-0000-7000-000000000004",
                    "00000000-0000-0000-7000-000000000005",
                    "00000000-0000-0000-7000-000000000006",
                    "00000000-0000-0000-7000-000000000007",
                    "00000000-0000-0000-7000-000000000008",
                    "00000000-0000-0000-7000-000000000009",

                    "00000000-0000-0000-8000-000000000000",
                    "00000000-0000-0000-8000-000000000001",
                    "00000000-0000-0000-8000-000000000002",
                    "00000000-0000-0000-8000-000000000003",

                    "00000000-0000-0000-9000-000000000000",
                    "00000000-0000-0000-9000-000000000001",
                    "00000000-0000-0000-9000-000000000002",
                    "00000000-0000-0000-9000-000000000003"
                }
            };
            Session[ConstantHelper.CurrentPermission] = CurrentAdminUser.ListPermissions;

            return View("PMain");
        }
        #endregion

        #region 费用分析后台主页
        /// <summary>
        /// 费用分析后台主页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AdminSessionFilter]
        public ActionResult CostMain()
        {
            return View("CMain");
        }
        #endregion

        #region 费用分析后台主页
        /// <summary>
        /// 费用分析后台主页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Main3()
        {
            CurrentAdminUser = new AdminUser()
            {
                Email = "user01.admin",
                Name = "Admin",
                ListPermissions = new List<string>()
                {
                    "00000000-0000-0000-1001-000000000000",
                    "00000000-0000-0000-1001-000000000001",
                    "00000000-0000-0000-1002-000000000000",
                    "00000000-0000-0000-1002-000000000001",
                    "00000000-0000-0000-1002-000000000002",
                    "00000000-0000-0000-1002-000000000003",
                    "00000000-0000-0000-1003-000000000000",
                    "00000000-0000-0000-1003-000000000001",
                    "00000000-0000-0000-1004-000000000000",
                    "00000000-0000-0000-1004-000000000001",
                    "00000000-0000-0000-1005-000000000000",
                    "00000000-0000-0000-1005-000000000001"
                }
            };
            Session[ConstantHelper.CurrentPermission] = CurrentAdminUser.ListPermissions;

            return View("CMain");
        }
        #endregion

        /// <summary>
        /// 退出 
        /// </summary>
        /// <returns></returns>
        public ActionResult Exit()
        {
            Session.Clear();
            return View();
        }
    }
}