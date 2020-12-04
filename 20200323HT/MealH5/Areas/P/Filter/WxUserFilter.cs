using MealAdmin.Entity;
using MealH5.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MealH5.Areas.P.Filter
{
    /// <summary>
    /// 微信用户身份过滤器
    /// </summary>
    public class WxUserFilter : ActionFilterAttribute
    {
        private List<string> listNoFilterUrl = new List<string>();

        public WxUserFilter()
        {
            listNoFilterUrl.Add("/P/Food/ChooseHospital");
            listNoFilterUrl.Add("/P/Order/OriginalOrder");
            listNoFilterUrl.Add("/P/Order/Index");
            listNoFilterUrl.Add("/P/Order/Details");
            listNoFilterUrl.Add("/P/Order/OrderApproval");
            listNoFilterUrl.Add("/P/Food/MMCoEShell");
            listNoFilterUrl.Add("/P/PreApproval/Submit");
            listNoFilterUrl.Add("/P/Food/ChoosePreApproval");
            listNoFilterUrl.Add("/P/PreApprovalState/Index");
            listNoFilterUrl.Add("/P/PreApproval/Approval");
            listNoFilterUrl.Add("/P/PreApproval/Details");
            listNoFilterUrl.Add("/P/PreApproval/Edit");
            listNoFilterUrl.Add("/P/PreApproval/MMCoEApprove");
            listNoFilterUrl.Add("/P/Upload/UploadOrder");
            listNoFilterUrl.Add("/P/Upload/UploadFiles");
            listNoFilterUrl.Add("/P/Upload/InformationCue");
            listNoFilterUrl.Add("/P/Upload/UploadFileStatus");
            listNoFilterUrl.Add("/P/Upload/Details");
            listNoFilterUrl.Add("/P/Upload/Approval");
            listNoFilterUrl.Add("/P/Upload/AutoTransferState");
            listNoFilterUrl.Add("/P/Upload/EditUploadFiles");
            listNoFilterUrl.Add("/P/PreApproval/LoadApprovalRecords");
            listNoFilterUrl.Add("/P/PreApproval/ApprovalDetails");
            listNoFilterUrl.Add("/P/PreApprovalstate/Address");
            listNoFilterUrl.Add("/P/PreApproval/AddressApprove");
            listNoFilterUrl.Add("/P/PreApproval/AddressDetail");

            #region 费用分析
            listNoFilterUrl.Add("/P/CostAnalysis/OrderAnalysisUp");
            listNoFilterUrl.Add("/P/CostAnalysis/PreApprovalAnalysis");
            listNoFilterUrl.Add("/P/CostAnalysis/OrderAnalysis");
            listNoFilterUrl.Add("/P/CostAnalysis/CostSummary");
            listNoFilterUrl.Add("/P/CostAnalysis/CostError");
            listNoFilterUrl.Add("/P/CostAnalysis/PreOrderAnalysis");
            #endregion
        }

        #region 判断URL是否不用过滤
        /// <summary>
        /// 判断URL是否不用过滤
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private bool IsNoFilterUrl(string url)
        {
            var count = listNoFilterUrl.Count(a => url.Contains(a));
            return count > 0;
        }
        #endregion

        #region 进入Action之前进行拦截
        /// <summary>
        /// 进入Action之前进行拦截
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            var uri = filterContext.HttpContext.Request.AppRelativeCurrentExecutionFilePath;

            if (IsNoFilterUrl(uri))
            {
                return;
            }

            HttpSessionStateBase Session = filterContext.HttpContext.Session;
            var wxUser = Session[ConstantHelper.CURRENTWXUSER] as P_USERINFO;

            if (wxUser == null || string.IsNullOrEmpty(wxUser.UserId))
            {
                // 如果是ajax请求，应直接反馈json提示手动刷新页面
                if (filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest"
                || filterContext.HttpContext.Request["IsJsonCall"] == "1")
                {
                    filterContext.Result = new JsonResult()
                    {
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        Data = new
                        {
                            state = "nowxuser",
                            txt = "操作失败！微信身份验证失败，关闭当前页面返回菜单窗口重新进入"
                        }
                    };
                }
                else
                {
                    ViewResult result = new ViewResult();
                    result.ViewName = "Exception";
                    result.ViewBag.msglevel = MsgLevelcs.warn;
                    result.ViewBag.info_message = "操作已超时";
                    result.ViewBag.needBackButton = false;
                    result.ViewBag.needCloseButton = true;

                    filterContext.Result = result;
                }
            }
        }
        #endregion
    }
}