using MealAdmin.Entity;
using MealAdmin.Service;
using MealAdminApiClient;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XFramework.XInject.Attributes;
using MealAdmin.Web.Filter;

namespace MealAdmin.Web.Areas.P.Controllers
{
    public class OrderApprovalManagementController : AdminBaseController
    {
        [Bean("preApprovalService")]
        public IPreApprovalService PreApprovalService { get; set; }

        [Bean("baseDataService")]
        public IBaseDataService baseDataService { get; set; }
        [Bean("operationAuditService")]
        public IOperationAuditService operationAuditService { get; set; }

        [Bean("userInfoService")]
        public IUserInfoService IUserInfoService { get; set; }

        [Bean("uploadFileQueryService")]
        public IUploadFileQueryService UploadFileQueryService { get; set; }


        // GET: P/OrderApprovalManagement
        public ActionResult ApprovalStatusQuery()
        {
            return View();
        }
        public ActionResult ChangeApprover()
        {
            return View();
        }
        public ActionResult ApproverAgent()
        {
            return View();
        }
        public ActionResult SecoundApproverAgent()
        {
            return View();
        }
        public ActionResult AddApproverAgent(string id)
        {
            ViewBag.id = id;
            return View();
        }

        public ActionResult AddSecondApproverAgent(string id)
        {
            ViewBag.id = id;
            return View();
        }

        public ActionResult ApproverAgentHis(string UserId)
        {
            ViewBag.UserId = UserId;
            return View();
        }

        public ActionResult SecoundApproverAgentHis(string UserId)
        {
            ViewBag.UserId = UserId;
            return View();
        }

        #region 删除一条
        /// <summary>
        /// 删除一条
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [OperationAuditFilter(Operation = "删除代理数据", OperationAuditTypeName = "删除代理数据")]
        public JsonResult DelAgent(string id, string UserName)
        {
            var res = IUserInfoService.DeleteAgent(Guid.Parse(id));
            if (res > 0)
            {

                var cont = "删除直线经理审批人:" + UserName;
                var num = operationAuditService.AddAudit("5", cont);
                return Json(new { state = 1 });
            }
            return Json(new { state = 0, txt = "操作失败" });
        }

        [HttpPost]
        [OperationAuditFilter(Operation = "删除代理数据", OperationAuditTypeName = "删除代理数据")]
        public JsonResult DelSecondAgent(string id, string UserName)
        {
            var res = IUserInfoService.DeleteSecondAgent(Guid.Parse(id));
            if (res > 0)
            {
                var cont = "删除二线代理审批人:" + UserName;
                var num = operationAuditService.AddAudit("5", cont);
                return Json(new { state = 1 });
            }
            return Json(new { state = 0, txt = "操作失败" });
        }
        #endregion


        #region 审批状态导出
        public void ExportApproval(string HTCode, string ApplierMUDID, string BUHeadMUDID, string Category, string Type)
        {
            #region 抓取数据
            int total = 0;
            var list = PreApprovalService.QueryLoad(HTCode, ApplierMUDID, BUHeadMUDID, Category, Type, int.MaxValue, 1, out total
               ).Select(a => new
               {
                   c1 = FormatterNull(a.c1),
                   c2 = FormatterNull(a.c2),
                   c3 = FormatterNull(a.c3),
                   c4 = FormatterNull(a.c4),
                   c5 = FormatterNull(a.c5),
                   c6 = FormatterNull(a.c6),
                   c7 = FormatterNull(a.c7),
                   c8 = FormatterNull(a.c8),
                   c9 = FormatterNull(a.c9),
                   c10 = FormatterNull(a.c10),
                   c11 = FormatterNull(a.c11),
                   c12 = FormatterNull(a.c12),
                   c13 = FormatterNull(a.c13)
               }).ToArray(); ;
            #endregion

            #region 构建Excel
            HSSFWorkbook wk = new HSSFWorkbook();
            ISheet sheet = wk.CreateSheet("Cater");
            IRow row = sheet.CreateRow(0);

            //ICellStyle style = wk.CreateCellStyle();
            //style.WrapText = true;
            //style.Alignment = HorizontalAlignment.Center;
            //IFont font = wk.CreateFont();
            //font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
            //font.FontHeightInPoints = 10;
            //style.SetFont(font);
            ICellStyle style = wk.CreateCellStyle();
            style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightOrange.Index;
            style.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;
            style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightOrange.Index;
            style.Alignment = HorizontalAlignment.Center;
            IFont font = wk.CreateFont();
            font.Color = NPOI.HSSF.Util.HSSFColor.White.Index;
            font.Boldweight = short.MaxValue;
            font.FontHeightInPoints = 10;
            style.SetFont(font);
            #endregion

            #region 生成表头  
            var title = new string[] { "HT编号", "申请人姓名", "申请人MUDID", "流程类别", "流程状态", "提交日期", "提交时间", "审批人MUDID", "审批人姓名", "审批动作", "审批理由", "审批日期", "审批时间" };

            sheet.DefaultRowHeight = 200 * 2;

            for (var i = 0; i < title.Length; i++)
            {
                sheet.SetColumnWidth(i, 15 * 256);
            }

            for (var i = 0; i < title.Length; i++)
            {
                ICell cell = row.CreateCell(i);
                cell.SetCellValue(title[i]);
                cell.SetCellType(CellType.String);
                cell.CellStyle = style;
            }
            #endregion

            #region 制作表体
            for (var i = 1; i <= list.Length; i++)
            {
                var item = list[i - 1];
                row = sheet.CreateRow(i);
                ICell cell = null;

                //2018-1-12 史强 注释掉序号列
                //cell = row.CreateCell(0);
                //cell.SetCellValue(i);

                var j = 0;
                if (item != null)
                {

                    cell = row.CreateCell(j);
                    cell.SetCellValue(item.c1);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c2);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c3);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c4);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c5);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c6);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c7);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c8);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c9);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c10);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c11);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c12);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c13);
                    cell = row.CreateCell(++j);

                }

            }
            #endregion

            #region 写入到客户端
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                wk.Write(ms);
                Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xls", DateTime.Now.ToString("yyyyMMddHHmmssfff")));
                Response.ContentType = "application/vnd.ms-excel";
                Response.BinaryWrite(ms.ToArray());
            }
            #endregion
        }
        #endregion

        #region 审批状态查询
        public JsonResult QueryLoad(string srh_HTCode, string srh_ApplierMUDID, string srh_BUHeadMUDID, string srh_Category, string srh_Type, int rows, int page)
        {
            int total;
            var list = PreApprovalService.QueryLoad(srh_HTCode, srh_ApplierMUDID, srh_BUHeadMUDID, srh_Category, srh_Type, rows, page, out total
              ).Select(a => new
              {
                  c0 = a.c0,
                  c1 = FormatterNull(a.c1),
                  c2 = FormatterNull(a.c2),
                  c3 = FormatterNull(a.c3),
                  c4 = FormatterNull(a.c4),
                  c5 = FormatterNull(a.c5),
                  c6 = FormatterNull(a.c6),
                  c7 = FormatterNull(a.c7),
                  c8 = FormatterNull(a.c8),
                  c9 = FormatterNull(a.c9),
                  c10 = FormatterNull(a.c10),
                  c11 = FormatterNull(a.c11),
                  c12 = FormatterNull(a.c12),
                  c13 = FormatterNull(a.c13)
              }).ToArray(); ;


            return Json(new { state = 1, rows = list, total = total }); ;
        }

        #endregion

        #region 转换
        static string FormatterNull(string str)
        {
            return str == null ? string.Empty : str;
        }

        static string FormatterNull(DateTime str)
        {
            if (str.Ticks == 0)
            {
                return string.Empty;
            }
            return str.ToString("yyyy-MM-dd");
        }

        static string FormatterNull(TimeSpan str)
        {
            if (str.Ticks == 0)
            {
                return string.Empty;
            }
            return $"{string.Format("{0:D2}", str.Hours)}:{string.Format("{0:D2}", str.Minutes)}:{string.Format("{0:D2}", str.Seconds)}";
        }

        static string FormatterNull(int str)
        {
            return $"{str}";
        }

        static string FormatterNull(decimal str)
        {
            return $"{str}";
        }
        #endregion

        #region 审批人代理查询
        public JsonResult ApproverAgentLoad(string ApprovalNameOrMUDID, string AgentNameOrMUDID, int rows, int page)
        {
            int total;
            var list = IUserInfoService.ApproverAgentLoad(ApprovalNameOrMUDID, AgentNameOrMUDID, rows, page, out total);
            return Json(new { state = 1, rows = list, total = total });
        }

        public JsonResult ApproverSecondAgentLoad(string ApprovalNameOrMUDID, string AgentNameOrMUDID, int rows, int page)
        {
            int total;
            var list = IUserInfoService.ApproverSecondAgentLoad(ApprovalNameOrMUDID, AgentNameOrMUDID, rows, page, out total);
            return Json(new { state = 1, rows = list, total = total });
        }
        #endregion

        #region 代理人历史查询
        [HttpPost]
        public JsonResult ApproverAgentHisLoad(string UserId)
        {
            var list = IUserInfoService.ApproverAgentHisLoad(UserId);
            return Json(new { state = 1, data = list });
        }
        [HttpPost]
        public JsonResult ApproverSecondAgentHisLoad(string UserId)
        {
            var list = IUserInfoService.ApproverSecondAgentHisLoad(UserId);
            return Json(new { state = 1, data = list });
        }
        #endregion

        #region 导出代理审批人信息
        public ActionResult ExportApproverAgent(string ApprovalNameOrMUDID, string AgentNameOrMUDID)
        {
            int total = 0;
            var list = IUserInfoService.ApproverAgentLoad(ApprovalNameOrMUDID, AgentNameOrMUDID, int.MaxValue, 1, out total);
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
                sheet.SetColumnWidth(5, 30 * 256);
                sheet.SetColumnWidth(6, 30 * 256);


                var cell = row.CreateCell(0);
                cell.SetCellValue("申请人姓名");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(1);
                cell.SetCellValue("申请人MUDID");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(2);
                cell.SetCellValue("代理人姓名");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(3);
                cell.SetCellValue("代理人MUDID");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(4);
                cell.SetCellValue("开始日期");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(5);
                cell.SetCellValue("结束日期");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(6);
                cell.SetCellValue("是否启用");
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
                P_UserDelegate disItm;
                int dataCnt = list.Count;
                for (int i = 0; i < dataCnt; i++)
                {
                    disItm = list[i];
                    row = sheet.CreateRow(1 + i);
                    #region data cell
                    cell = row.CreateCell(0);
                    cell.SetCellValue(disItm.UserName);// 审批人姓名
                    cell = row.CreateCell(1);
                    cell.SetCellValue(disItm.UserMUDID);//申请人MUDID
                    cell = row.CreateCell(2);
                    cell.SetCellValue(disItm.DelegateUserName);// 代理人姓名
                    cell = row.CreateCell(3);
                    cell.SetCellValue(disItm.DelegateUserMUDID);// 代理人MUDID
                    cell = row.CreateCell(4);
                    cell.SetCellValue(disItm.StartTime == null ? "" : disItm.StartTime.ToString("yyyy-MM-dd HH:mm:ss"));// 开始日期
                    cell = row.CreateCell(5);
                    cell.SetCellValue(disItm.EndTime == null ? "" : disItm.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));// 结束日期
                    cell = row.CreateCell(6);
                    cell.SetCellValue(disItm.IsEnable == 0 ? "否" : "是");// 是否启用
                    cell = row.CreateCell(7);

                    #endregion
                }

                byte[] excelData;
                using (System.IO.MemoryStream _ms = new System.IO.MemoryStream())
                {
                    book.Write(_ms);
                    excelData = _ms.ToArray();
                    //_ms.Close();
                }
                var cont = "导出直线经理代理审批";
                var num = operationAuditService.AddAudit("5", cont);
                ViewBag.Msg = "导出成功！";
                return File(excelData, "application/vnd.ms-excel", HttpUtility.UrlEncode("ApproverAgent_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx", System.Text.Encoding.UTF8));
            }
            else
            {
                ViewBag.Msg = "无符合条件数据！";
                return View();
            }
        }

        public ActionResult ExportApproverSecondAgent(string ApprovalNameOrMUDID, string AgentNameOrMUDID)
        {
            int total = 0;
            var list = IUserInfoService.ApproverSecondAgentLoad(ApprovalNameOrMUDID, AgentNameOrMUDID, int.MaxValue, 1, out total);
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
                sheet.SetColumnWidth(5, 30 * 256);
                sheet.SetColumnWidth(6, 30 * 256);


                var cell = row.CreateCell(0);
                cell.SetCellValue("申请人姓名");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(1);
                cell.SetCellValue("申请人MUDID");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(2);
                cell.SetCellValue("代理人姓名");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(3);
                cell.SetCellValue("代理人MUDID");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(4);
                cell.SetCellValue("开始日期");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(5);
                cell.SetCellValue("结束日期");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(6);
                cell.SetCellValue("是否启用");
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
                P_UserDelegatePre disItm;
                int dataCnt = list.Count;
                for (int i = 0; i < dataCnt; i++)
                {
                    disItm = list[i];
                    row = sheet.CreateRow(1 + i);
                    #region data cell
                    cell = row.CreateCell(0);
                    cell.SetCellValue(disItm.UserName);// 审批人姓名
                    cell = row.CreateCell(1);
                    cell.SetCellValue(disItm.UserMUDID);//申请人MUDID
                    cell = row.CreateCell(2);
                    cell.SetCellValue(disItm.DelegateUserName);// 代理人姓名
                    cell = row.CreateCell(3);
                    cell.SetCellValue(disItm.DelegateUserMUDID);// 代理人MUDID
                    cell = row.CreateCell(4);
                    cell.SetCellValue(disItm.StartTime == null ? "" : disItm.StartTime.ToString("yyyy-MM-dd HH:mm:ss"));// 开始日期
                    cell = row.CreateCell(5);
                    cell.SetCellValue(disItm.EndTime == null ? "" : disItm.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));// 结束日期
                    cell = row.CreateCell(6);
                    cell.SetCellValue(disItm.IsEnable == 0 ? "否" : "是");// 是否启用
                    cell = row.CreateCell(7);

                    #endregion
                }

                byte[] excelData;
                using (System.IO.MemoryStream _ms = new System.IO.MemoryStream())
                {
                    book.Write(_ms);
                    excelData = _ms.ToArray();
                    //_ms.Close();
                }
                var cont = "导出二线经理代理审批";
                var num = operationAuditService.AddAudit("5", cont);
                ViewBag.Msg = "导出成功！";
                return File(excelData, "application/vnd.ms-excel", HttpUtility.UrlEncode("ApproverSecondAgent_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx", System.Text.Encoding.UTF8));
            }
            else
            {
                ViewBag.Msg = "无符合条件数据！";
                return View();
            }
        }
        #endregion

        #region 导出代理人历史
        public ActionResult ExportApproverAgentHis(string UserId)
        {
            var list = IUserInfoService.ApproverAgentHisLoad(UserId);
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
                sheet.SetColumnWidth(5, 30 * 256);
                sheet.SetColumnWidth(6, 30 * 256);


                var cell = row.CreateCell(0);
                cell.SetCellValue("申请人姓名");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(1);
                cell.SetCellValue("申请人MUDID");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(2);
                cell.SetCellValue("代理人姓名");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(3);
                cell.SetCellValue("代理人MUDID");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(4);
                cell.SetCellValue("开始日期");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(5);
                cell.SetCellValue("结束日期");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(6);
                cell.SetCellValue("是否启用");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(7);
                cell.SetCellValue("操作人");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(8);
                cell.SetCellValue("操作日期");
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
                P_UserDelegateHis disItm;
                int dataCnt = list.Count;
                for (int i = 0; i < dataCnt; i++)
                {
                    disItm = list[i];
                    row = sheet.CreateRow(1 + i);
                    #region data cell
                    cell = row.CreateCell(0);
                    cell.SetCellValue(disItm.UserName);// 审批人姓名
                    cell = row.CreateCell(1);
                    cell.SetCellValue(disItm.UserMUDID);//申请人MUDID
                    cell = row.CreateCell(2);
                    cell.SetCellValue(disItm.DelegateUserName);// 代理人姓名
                    cell = row.CreateCell(3);
                    cell.SetCellValue(disItm.DelegateUserMUDID);// 代理人MUDID
                    cell = row.CreateCell(4);
                    cell.SetCellValue(disItm.StartTime == null ? "" : disItm.StartTime.ToString("yyyy-MM-dd HH:mm:ss"));// 开始日期
                    cell = row.CreateCell(5);
                    cell.SetCellValue(disItm.EndTime == null ? "" : disItm.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));// 结束日期
                    cell = row.CreateCell(6);
                    cell.SetCellValue(disItm.IsEnable == 0 ? "否" : "是");// 是否启用
                    cell = row.CreateCell(7);
                    cell.SetCellValue(disItm.OperatorMUDID);// 操作人
                    cell = row.CreateCell(8);
                    cell.SetCellValue(disItm.OperationTime == null ? "" : disItm.OperationTime.ToString("yyyy-MM-dd HH:mm:ss"));// 操作日期
                    cell = row.CreateCell(9);

                    #endregion
                }

                byte[] excelData;
                using (System.IO.MemoryStream _ms = new System.IO.MemoryStream())
                {
                    book.Write(_ms);
                    excelData = _ms.ToArray();
                    //_ms.Close();
                }

                ViewBag.Msg = "导出成功！";
                return File(excelData, "application/vnd.ms-excel", HttpUtility.UrlEncode("ApproverAgentHis_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx", System.Text.Encoding.UTF8));
            }
            else
            {
                ViewBag.Msg = "无符合条件数据！";
                return View();
            }
        }


        public ActionResult ExportApproverSecondAgentHis(string UserId)
        {
            var list = IUserInfoService.ApproverSecondAgentHisLoad(UserId);
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
                sheet.SetColumnWidth(5, 30 * 256);
                sheet.SetColumnWidth(6, 30 * 256);


                var cell = row.CreateCell(0);
                cell.SetCellValue("申请人姓名");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(1);
                cell.SetCellValue("申请人MUDID");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(2);
                cell.SetCellValue("代理人姓名");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(3);
                cell.SetCellValue("代理人MUDID");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(4);
                cell.SetCellValue("开始日期");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(5);
                cell.SetCellValue("结束日期");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(6);
                cell.SetCellValue("是否启用");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(7);
                cell.SetCellValue("操作人");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(8);
                cell.SetCellValue("操作日期");
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
                P_UserDelegatePreHis disItm;
                int dataCnt = list.Count;
                for (int i = 0; i < dataCnt; i++)
                {
                    disItm = list[i];
                    row = sheet.CreateRow(1 + i);
                    #region data cell
                    cell = row.CreateCell(0);
                    cell.SetCellValue(disItm.UserName);// 审批人姓名
                    cell = row.CreateCell(1);
                    cell.SetCellValue(disItm.UserMUDID);//申请人MUDID
                    cell = row.CreateCell(2);
                    cell.SetCellValue(disItm.DelegateUserName);// 代理人姓名
                    cell = row.CreateCell(3);
                    cell.SetCellValue(disItm.DelegateUserMUDID);// 代理人MUDID
                    cell = row.CreateCell(4);
                    cell.SetCellValue(disItm.StartTime == null ? "" : disItm.StartTime.ToString("yyyy-MM-dd HH:mm:ss"));// 开始日期
                    cell = row.CreateCell(5);
                    cell.SetCellValue(disItm.EndTime == null ? "" : disItm.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));// 结束日期
                    cell = row.CreateCell(6);
                    cell.SetCellValue(disItm.IsEnable == 0 ? "否" : "是");// 是否启用
                    cell = row.CreateCell(7);
                    cell.SetCellValue(disItm.OperatorMUDID);// 操作人
                    cell = row.CreateCell(8);
                    cell.SetCellValue(disItm.OperationTime == null ? "" : disItm.OperationTime.ToString("yyyy-MM-dd HH:mm:ss"));// 操作日期
                    cell = row.CreateCell(9);

                    #endregion
                }

                byte[] excelData;
                using (System.IO.MemoryStream _ms = new System.IO.MemoryStream())
                {
                    book.Write(_ms);
                    excelData = _ms.ToArray();
                    //_ms.Close();
                }

                ViewBag.Msg = "导出成功！";
                return File(excelData, "application/vnd.ms-excel", HttpUtility.UrlEncode("ApproverSecondAgentHis_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx", System.Text.Encoding.UTF8));
            }
            else
            {
                ViewBag.Msg = "无符合条件数据！";
                return View();
            }
        }
        #endregion

        #region 添加代理审批人
        public JsonResult _AddApproverAgent(string ID, string txtUserName, string txtUserMUDID, string txtDelegateUserName, string txtDelegateUserMUDID, DateTime txtStartTime, DateTime txtEndTime, string txtIsEnable)
        {
            int state;
            //var have = IUserInfoService.AgentExist(Guid.Parse(ID));//判断代理人信息是否存在
            //查询代理人是否存在 如果存在就修改 不存在新增
            if (ID == string.Empty)
            {
                //创建代理人
                Guid _ID = Guid.NewGuid();
                state = IUserInfoService.SaveAgent(_ID, txtUserMUDID, txtUserName, txtDelegateUserMUDID, txtDelegateUserName, txtStartTime, txtEndTime, int.Parse(txtIsEnable), CurrentAdminUser.Email);
                var cont = "添加直线经理审批人:" + txtUserName;
                var num = operationAuditService.AddAudit("5", cont);
            }
            else
            {
                //修改代理人信息
                state = IUserInfoService.UpdateAgent(Guid.Parse(ID), txtUserMUDID, txtUserName, txtDelegateUserMUDID, txtDelegateUserName, txtStartTime, txtEndTime, int.Parse(txtIsEnable), CurrentAdminUser.Email);
                var cont = "修改直线经理审批人:" + txtUserName;
                var num = operationAuditService.AddAudit("5", cont);
            }
            return Json(new { state });
        }


        public JsonResult _AddApproverSecondAgent(string ID, string txtUserName, string txtUserMUDID, string txtDelegateUserName, string txtDelegateUserMUDID, DateTime txtStartTime, DateTime txtEndTime, string txtIsEnable)
        {
            int state;
            //var have = IUserInfoService.AgentExist(Guid.Parse(ID));//判断代理人信息是否存在
            //查询代理人是否存在 如果存在就修改 不存在新增
            if (ID == string.Empty)
            {
                //创建代理人
                Guid _ID = Guid.NewGuid();
                state = IUserInfoService.SaveSecondAgent(_ID, txtUserMUDID, txtUserName, txtDelegateUserMUDID, txtDelegateUserName, txtStartTime, txtEndTime, int.Parse(txtIsEnable), CurrentAdminUser.Email);
                var cont = "添加二线代理审批人:" + txtUserName;
                var num = operationAuditService.AddAudit("5", cont);
            }
            else
            {
                //修改代理人信息
                state = IUserInfoService.UpdateSecondAgent(Guid.Parse(ID), txtUserMUDID, txtUserName, txtDelegateUserMUDID, txtDelegateUserName, txtStartTime, txtEndTime, int.Parse(txtIsEnable), CurrentAdminUser.Email);
                var cont = "修改二线代理审批人:" + txtUserName;
                var num = operationAuditService.AddAudit("5", cont);
            }
            return Json(new { state });
        }
        #endregion

        #region 查询是否存在代理人信息
        public JsonResult ExistentAgent(string AgentMudid)
        {
            //判断当前用户是否是直线经理，是否有审批权限

            var a = IUserInfoService.isHaveApproval(AgentMudid);
            var b = PreApprovalService.HasApprove(AgentMudid);
            if (a.ApprovalCount != 0 || b)
            {
                var obj = IUserInfoService.isAgentBack(AgentMudid);
                return Json(new { state = 1, data = obj, isAgent = 1 });
            }
            return Json(new { state = 1, isAgent = 0 });

        }

        public JsonResult ExistentSecondAgent(string AgentMudid)
        {
            //判断当前用户是否是直线经理，是否有审批权限

            var a = IUserInfoService.isSecondApproval(AgentMudid);
            var b = PreApprovalService.HasApprove(AgentMudid);
            if (a.ApprovalCount != 0 || b)
            {
                var obj = IUserInfoService.isSecondAgentBack(AgentMudid);
                return Json(new { state = 1, data = obj, isAgent = 1 });
            }
            return Json(new { state = 1, isAgent = 0 });

        }
        #endregion

        #region 自动添加姓名
        [HttpPost]
        public JsonResult ShowName(string AgentMudid)
        {

            var res = IUserInfoService.Find(AgentMudid);
            if (res != null)
            {
                var userId = res.UserId;
                var isHaveApproval = IUserInfoService.isHaveApproval(userId);
                var has = PreApprovalService.HasApprove(userId);
                if (isHaveApproval.ApprovalCount > 0 || has)
                {
                    return Json(new { state = 1, ishave = 1, res });
                }
                else
                {
                    return Json(new { state = 1, ishave = 0, txt = "当前代理人不是直线经理，或没有审批权限，请重试！" });
                }
            }
            return Json(new { state = 1, ishave = 0, txt = "不存在此MUDID" });
        }

        [HttpPost]
        public JsonResult ShowSecondName(string AgentMudid)
        {

            var res = IUserInfoService.Find(AgentMudid);
            if (res != null)
            {
                var userId = res.UserId;
                var isHaveApproval = IUserInfoService.isSecondApproval(userId);
                var has = PreApprovalService.HasApprove(userId);
                if (isHaveApproval.ApprovalCount > 0 || has)
                {
                    return Json(new { state = 1, ishave = 1, res });
                }
                else
                {
                    return Json(new { state = 1, ishave = 0, txt = "当前代理人不是二线经理，或没有审批权限，请重试！" });
                }
            }
            return Json(new { state = 1, ishave = 0, txt = "不存在此MUDID" });
        }
        #endregion

        #region 自动添加姓名---其他页面使用
        [HttpPost]
        public JsonResult _ShowName(string AgentMudid)
        {

            var res = IUserInfoService.Find(AgentMudid);
            if (res != null)
            {
                return Json(new { state = 1, ishave = 1, res });
            }
            return Json(new { state = 1, ishave = 0, txt = "不存在此MUDID" });
        }
        #endregion

        public JsonResult LoadUpload(string HTCode, string ApplierMUDID, string BUHeadMUDID, string Type, string State, int rows, int page)
        {
            int total;
            var list = PreApprovalService.QueryLoad(HTCode, ApplierMUDID, BUHeadMUDID, Type, State, rows, page, out total
                ).Select(a => new
                {
                    c0 = a.c0,
                    c1 = FormatterNull(a.c1),
                    c2 = FormatterNull(a.c2),
                    c3 = FormatterNull(a.c3),
                    c4 = FormatterNull(a.c4),
                    c5 = FormatterNull(a.c5),
                    c6 = FormatterNull(a.c6),
                    c7 = FormatterNull(a.c7),
                    c8 = FormatterNull(a.c8),
                    c9 = FormatterNull(a.c9),
                    c10 = FormatterNull(a.c10),
                    c11 = FormatterNull(a.c11),
                    c12 = FormatterNull(a.c12),
                    c13 = FormatterNull(a.c13),
                    c14 = FormatterNull(a.c14)
                }).ToArray(); ;
            return Json(new { state = 1, rows = list, total = total });
        }

        #region 重新分配审批人页面
        /// <summary>
        /// 重新分配审批人页面
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult Pops(string ids, string hts)
        {
            ViewBag.IDS = ids;
            ViewBag.HTS = hts;
            return View();
        }
        #endregion

        #region 保存重新分配审批人
        /// <summary>
        /// 保存重新分配审批人
        /// </summary>
        /// <param name="UploadID"></param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public JsonResult UploadApprover(string UploadID, string htcode, string userId, string userName)
        {
            UploadID = UploadID.Replace(",", "','");
            var res = PreApprovalService.LoadByID(UploadID);
            var preList = new List<string>();
            var upoList = new List<string>();
            var preHTcodes = new List<string>();
            var upoHTcodes = new List<string>();
            foreach (var item in res)
            {

                if (item.c4 == "预申请")
                {
                    preList.Add(item.c0.ToString());
                    preHTcodes.Add(item.c1.ToString());
                    //var cont = "重新分配审批人-预申请:" + item.c1;
                    //var num = operationAuditService.AddAudit("3", cont);
                }
                else
                {
                    upoList.Add(item.c0.ToString());
                    upoHTcodes.Add(item.c1.ToString());
                    //var cont = "重新分配审批人-上传文件:" + item.c1;
                    //var num = operationAuditService.AddAudit("3", cont);
                }
            }

            if (preList.Count > 0)
            {
                var preIds = string.Join("','", preList);
                var preHTs = string.Join(",", preHTcodes);
                PreApprovalService.UpdatePreReAssign(preIds, CurrentAdminUser.Email, CurrentAdminUser.Name, userId, userName);
                var cont = "重新分配审批人-预申请:" + preHTs;
                var num = operationAuditService.AddAudit("3", cont);
                //发消息
                foreach (var preId in preList)
                {
                    var preApproval = PreApprovalService.GetPreApprovalByID(preId).FirstOrDefault();
                    if (preApproval != null)
                    {
                        string applicantMsg = preApproval.ModifyDate == preApproval.CreateDate ? $"{preApproval.HTCode}，您有需要审批的预申请。请<a href='{ConfigurationManager.AppSettings["MealH5SiteUrl"]}/P/PreApproval/Approval?id={preId}&from=0'>点击这里</a>进行审批。" : $"{preApproval.HTCode}，您有需要审批的预申请修改。请<a href='{ConfigurationManager.AppSettings["MealH5SiteUrl"]}/P/PreApproval/Approval?id={preId}&from=0'>点击这里</a>进行审批。";
                        var rtnVal = WxMessageClientChannelFactory.GetChannel().SendText(userId, applicantMsg);
                    }
                }
            }

            if (upoList.Count > 0)
            {
                var upoIds = string.Join("','", upoList);
                var upoHTs = string.Join(",", upoHTcodes);
                PreApprovalService.UpdatePuoReAssign(upoIds, CurrentAdminUser.Email, CurrentAdminUser.Name, userId, userName);
                var cont = "重新分配审批人-上传文件:" + upoHTs;
                var num = operationAuditService.AddAudit("3", cont);
                //发消息
                foreach (var uplId in upoList)
                {
                    var uploadFile = UploadFileQueryService.FindPreUploadFile(uplId);
                    if (uploadFile != null)
                    {
                        var messageBase = "该订单已上传会议支持文件";
                        switch (uploadFile.FileType)
                        {
                            case 1:
                                messageBase = "该订单已提交退单原因";
                                break;
                            case 2:
                                messageBase = "该订单已提交会议支持文件丢失原因";
                                break;
                            case 3:
                                messageBase = "该订单已提交未送达，会议未正常召开原因";
                                break;
                        }
                        var approverMsg = $"{uploadFile.HTCode}，{messageBase}，请<a href='{ConfigurationManager.AppSettings["MealH5SiteUrl"]}/P/Upload/Approval?id={uplId}&from=0'>点击这里</a>进行审批。";
                        var rtnVal = WxMessageClientChannelFactory.GetChannel().SendText(userId, approverMsg);
                    }
                }


            }
            return Json(new { state = 1 });
        }
        #endregion

        #region 根据ID查找
        /// <summary>
        /// 根据ID查找
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult FindById(string id)
        {
            var UserDelegate = IUserInfoService.FindById(Guid.Parse(id));

            return Json(new { state = 1, data = new { UserDelegate } });
        }

        /// <summary>
        /// 根据ID查找
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult FindSecondById(string id)
        {
            var UserDelegate = IUserInfoService.FindSecondById(Guid.Parse(id));

            return Json(new { state = 1, data = new { UserDelegate } });
        }
        #endregion

    }
}