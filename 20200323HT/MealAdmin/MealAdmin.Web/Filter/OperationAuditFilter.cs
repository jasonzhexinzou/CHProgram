using IamPortal.AppLogin;
using MealAdmin.Entity;
using MealAdmin.Service;
using MealAdmin.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using XFramework.XInject.Attributes;

namespace MealAdmin.Web.Filter
{
    public class OperationAuditFilter : ActionFilterAttribute
    {
        public string Operation { get; set; }
        public string OperationAuditTypeName { get; set; }

        #region 离开Action之后进行拦截
        /// <summary>
        /// 离开Action之后进行拦截
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            P_OperationAudit operationAudit;

            HttpSessionStateBase Session = filterContext.HttpContext.Session;

            AdminUser adminUser = Session[ConstantHelper.CurrentAdminUser] as AdminUser;
            if (adminUser != null)
            {
                var param = filterContext.HttpContext.Request.Form.ToString();
                //param = param.Replace("%u", @"\u");
                string OperationText = Operation;
                operationAudit = new P_OperationAudit();
                operationAudit.ID = Guid.NewGuid();
                operationAudit.OperatorName = adminUser.Name;
                operationAudit.OperatorID = adminUser.Email;
                if (!string.IsNullOrEmpty(param) && param.Split('&').Length > 0)
                {
                    OperationText += ",参数：";
                    foreach (var item in param.Split('&'))
                    {
                        var value = item.Substring(item.LastIndexOf("=") + 1);
                        if (!string.IsNullOrEmpty(value))
                        {
                            var str = item.Substring(item.LastIndexOf("=") + 1);
                            string[] strlist = str.Replace("%u", ";").Split(';');
                            var outStr = "";
                            if (strlist.Length > 1)
                            {
                                for (int i = 0; i < strlist.Length; i++)
                                {
                                    if (!string.IsNullOrEmpty(strlist[i]))
                                    {
                                        if (strlist[i].Contains("+"))
                                        {
                                            strlist[i] = strlist[i].Replace("+", " ");
                                        }
                                        //将unicode字符转为10进制整数，然后转为char中文字符
                                        outStr += (char)int.Parse(strlist[i], System.Globalization.NumberStyles.HexNumber);
                                    }
                                }
                            }
                            else
                            {
                                outStr = strlist[0];
                            }
                            OperationText += "【@" + item.Substring(0, item.IndexOf('=') + 1) + outStr + "】";
                        }
                    }
                }
                else if (filterContext.HttpContext.Request.QueryString.Count > 0)
                {
                    OperationText += ",参数：";
                    foreach (string item in filterContext.HttpContext.Request.QueryString)
                    {
                        var value = filterContext.HttpContext.Request.QueryString[item].ToString();
                        if (!string.IsNullOrEmpty(value))
                        {
                            OperationText += "【@" + item+"="+value+ "】";
                        }
                    }
                }
                operationAudit.Operation = OperationText;
                if (filterContext.Exception == null)
                {
                    operationAudit.StateID = 1;
                }
                else
                {
                    operationAudit.StateID = -1;
                    operationAudit.Exception= filterContext.Exception.Message;
                }
                operationAudit.CreateDate = DateTime.Now;
                LogOperationAudit(operationAudit);
            }
        }
        #endregion

        #region 异步记录访问日志
        /// <summary>
        /// 异步记录访问日志
        /// </summary>
        /// <param name="operation"></param>
        public void LogOperationAudit(P_OperationAudit OperationAudit)
        {
            IOperationAuditService operationAuditService = MvcApplication.applicationContext.GetBean("operationAuditService") as OperationAuditService;
            Task.Factory.StartNew(() =>
            {
                operationAuditService.SaveOperationAudit(OperationAudit);
            });
        }
        #endregion
    }
}