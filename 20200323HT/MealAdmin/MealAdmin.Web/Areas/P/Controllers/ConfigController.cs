using MealAdmin.Dao;
using MealAdmin.Entity;
using MealAdmin.Service;
using MealAdmin.Web.Filter;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XFramework.XInject.Attributes;
using XFramework.XUtil;
using a = System;

namespace MealAdmin.Web.Areas.P.Controllers
{
    public class ConfigController : Controller
    {

        [Bean("bizConfigService")]
        public IBizConfigService bizConfigService { get; set; }
        // GET: P/Config
        //public ActionResult Workday()
        //{
        //    return View();
        //}

        [Bean("operationAuditService")]
        public IOperationAuditService operationAuditService { get; set; }

        #region 系统审计
        /// <summary>
        /// 审计视图
        /// </summary>
        /// <returns></returns>
        public ActionResult OperationRecords()
        {
            return View();
        }
        #endregion
        #region 审计视图
        /// <summary>
        /// 审计视图
        /// </summary>
        /// <returns></returns>
        public ActionResult Audit()
        {
            return View();
        }
        #endregion

        #region 审计
        /// <summary>
        /// 审计
        /// </summary>
        /// <returns></returns>
        public JsonResult LoadAudit(int rows, int page)
        {
            int total;
            var data = operationAuditService.Load(rows, page, out total);
            return Json(new { state = 1, rows = data, total = total });
        }
        #endregion
        private P_Audit_View GetDisplayObj(P_Audit itm)
        {
            P_Audit_View rtnData = new Entity.P_Audit_View();
            rtnData.UserID = itm.UserID;
            rtnData.CreatDate = itm.CreatDate != null ? itm.CreatDate.Value.ToString("yyyy-MM-dd") : string.Empty;
            rtnData.CreatTime = itm.CreatDate != null ? itm.CreatDate.Value.ToString("HH:mm:ss") : string.Empty;
            rtnData.ChangeContent = itm.ChangeContent;
            if (itm.Type == 0)
            {
                rtnData.Type = "预申请最高审批人";
            }
            if (itm.Type == 1)
            {
                rtnData.Type = "特殊订单—呼叫中心操作退单";
            }
            if (itm.Type == 2)
            {
                rtnData.Type = "特殊订单—会议文件丢失";
            }
            if (itm.Type == 3)
            {
                rtnData.Type = "重新分配";
            }
            if (itm.Type == 4)
            {
                rtnData.Type = "系统组别管理";
            }
            if (itm.Type == 5)
            {
                rtnData.Type = "代理人审批";
            }
            if (itm.Type == 6)
            {
                rtnData.Type = "医院管理";
            }
            return rtnData;
        }
        public JsonResult LoadOperationAudit(string ApprovalMUDID, string Begin, string End, string CostCenter, string SpecialOrders1, string SpecialOrders2, string UploadFile, string SystemGroup, string AgentApprova, string Hospital, int rows, int page)
        {
            int total;
            var list = operationAuditService.Load1(ApprovalMUDID, Begin, End, CostCenter, SpecialOrders1, SpecialOrders2, UploadFile, SystemGroup, AgentApprova, Hospital, rows, page, out total);
            var audit = new List<P_Audit_View>();
            foreach (var i in list)
            {
                audit.Add(GetDisplayObj(i));
            }
            return Json(new { state = 1, rows = audit, total = total });
        }
        public ActionResult ExportAudit1()
        {
            return null;
        }
        public ActionResult ExportAudit(string ApprovalMUDID, string Begin, string End, string CostCenter, string SpecialOrders1, string SpecialOrders2, string UploadFile, string SystemGroup, string AgentApprova, string Hospital)
        {
            int total = 0;
            var list = operationAuditService.Load1(ApprovalMUDID, Begin, End, CostCenter, SpecialOrders1, SpecialOrders2, UploadFile, SystemGroup, AgentApprova, Hospital, int.MaxValue, 1, out total);
            if (list != null && list.Count > 0)
            {
                XSSFWorkbook book = new XSSFWorkbook();
                #region var headerStyle = book.CreateCellStyle();
                var headerStyle = book.CreateCellStyle();

                var headerFontStyle = book.CreateFont();
                headerFontStyle.Color = NPOI.HSSF.Util.HSSFColor.White.Index;
                headerFontStyle.Boldweight = short.MaxValue;
                headerFontStyle.FontHeightInPoints = 10;

                headerStyle.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;
                headerStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightOrange.Index;
                headerStyle.Alignment = HorizontalAlignment.Center;
                headerStyle.SetFont(headerFontStyle);
                #endregion
                var sheet = book.CreateSheet("report");
                var row = sheet.CreateRow(0);
                #region header
                sheet.SetColumnWidth(0, 30 * 256);
                sheet.SetColumnWidth(1, 30 * 256);
                sheet.SetColumnWidth(2, 30 * 256);
                sheet.SetColumnWidth(3, 30 * 256);
                sheet.SetColumnWidth(4, 30 * 256);



                var cell = row.CreateCell(0);
                cell.SetCellValue("操作人");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(1);
                cell.SetCellValue("操作日期");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(2);
                cell.SetCellValue("操作时间");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(3);
                cell.SetCellValue("操作类型");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(4);
                cell.SetCellValue("变更内容");
                cell.CellStyle = headerStyle;





                #endregion
                #region var dataCellStyle = book.CreateCellStyle();
                var dataCellStyle = book.CreateCellStyle();
                var dataFontStyle = book.CreateFont();
                dataFontStyle.Color = NPOI.HSSF.Util.HSSFColor.Black.Index;
                dataFontStyle.Boldweight = short.MaxValue;
                dataFontStyle.FontHeightInPoints = 10;

                dataCellStyle.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;
                dataCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.White.Index;
                //dataCellStyle.Alignment = HorizontalAlignment.Center;
                dataCellStyle.SetFont(dataFontStyle);
                #endregion
                P_Audit disItm;
                int dataCnt = list.Count;
                for (int i = 0; i < dataCnt; i++)
                {
                    disItm = list[i];
                    row = sheet.CreateRow(1 + i);
                    var audit = new List<P_Audit_View>();

                    audit.Add(GetDisplayObj(disItm));


                    #region data cell

                    cell = row.CreateCell(0);
                    cell.SetCellValue(audit[0].UserID);// 操作人
                    cell = row.CreateCell(1);
                    cell.SetCellValue(audit[0].CreatDate);//操作日期
                    cell = row.CreateCell(2);
                    cell.SetCellValue(audit[0].CreatTime);// 操作时间
                    cell = row.CreateCell(3);
                    cell.SetCellValue(audit[0].Type);// 操作类型
                    cell = row.CreateCell(4);
                    cell.SetCellValue(audit[0].ChangeContent);// 变更内容
                    cell = row.CreateCell(5);


                    #endregion
                }

                byte[] excelData;
                using (System.IO.MemoryStream _ms = new System.IO.MemoryStream())
                {
                    book.Write(_ms);
                    excelData = _ms.ToArray();
                    //_ms.Close();
                }
                //var cont = "导出直线经理代理审批";
                //var num = operationAuditService.AddAudit("5", cont);
                ViewBag.Msg = "导出成功！";
                return File(excelData, "application/vnd.ms-excel", HttpUtility.UrlEncode("Audit_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".xlsx", a.Text.Encoding.UTF8));
            }
            else
            {
                ViewBag.Msg = "无符合条件数据！";
                return View();
            }

        }
        [AdminSessionFilter(Order = 1)]
        [PermissionFilter(Order = 2, Permission = "00000000-0000-0000-8000-000000000001")]
        public ActionResult Invoice()
        {

            var _invoice = bizConfigService.GetAllMarkets();
            ViewBag.Invoice = _invoice;
            return View();
        }

        [HttpPost]
        //[AdminSessionFilter(Order = 1)]
        //[PermissionFilter(Order = 2, Permission = "00000000-0000-0000-4000-000000000001")]
        public JsonResult SaveInvoiceData(P_MARKET_INVOICE_OBJ Data)
        {
            List<P_MARKET> unSuccessData;
            var updCnt = bizConfigService.UpdateMarketsInvoice(Data, out unSuccessData);
            if (unSuccessData.Count == 0 && updCnt > 0)
            {
                return Json(new { state = 1 });
            }
            else
            {
                string _txt;
                if (updCnt == 0)
                {
                    _txt = "保存失败！";
                }
                else
                {
                    _txt = "部分保存失败！" + JsonConvert.SerializeObject(unSuccessData);
                }

                return Json(new { state = 0, txt = _txt });
            }
        }

        [AdminSessionFilter(Order = 1)]
        [PermissionFilter(Order = 2, Permission = "00000000-0000-0000-8000-000000000002")]
        public ActionResult System()
        {
            var _conf = bizConfigService.GetConfig();
            ViewBag.Conf = _conf;
            return View();
        }

        [HttpPost]
        //[AdminSessionFilter(Order = 1)]
        //[PermissionFilter(Order = 2, Permission = "00000000-0000-0000-4000-000000000002")]
        public JsonResult SaveSystemConfig(P_BIZ_CONF_OBJ Data)
        {
            List<P_BIZ_CONF> unSuccessData;
            var updCnt = bizConfigService.UpdateConfig(Data, out unSuccessData);
            if (unSuccessData.Count == 0 && updCnt > 0)
            {
                return Json(new { state = 1 });
            }
            else
            {
                var unSuccessDataStr = JsonConvert.SerializeObject(unSuccessData);
                LogHelper.Info("P/Config/SaveSystemConfig unsuccess data:" + unSuccessDataStr);
                string _txt;
                if (updCnt == 0)
                {
                    _txt = "保存失败！";
                }
                else
                {
                    _txt = "部分保存失败！" + unSuccessDataStr;
                }

                return Json(new { state = 0, txt = _txt });
            }
        }
    }
}