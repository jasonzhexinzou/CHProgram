using MealAdmin.Entity;
using MealAdmin.Entity.View;
using MealAdmin.Service;
using MealAdmin.Web.Filter;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using XFramework.XInject.Attributes;
using XFramework.XUtil;

namespace MealAdmin.Web.Areas.P.Controllers
{
    public class HospitalController : AdminBaseController
    {

        [Bean("hospitalService")]
        public IHospitalService hospitalService { get; set; }
        [Bean("operationAuditService")]
        public IOperationAuditService operationAuditService { get; set; }

        [Bean("baseDataService")]
        public IBaseDataService baseDataService { get; set; }
        [Bean("groupMemberService")]
        public IGroupMemberService groupMemberService { get; set; }

        // GET: P/Hospital
        [AdminSessionFilter(Order = 1)]
        [PermissionFilter(Order = 2, Permission = "00000000-0000-0000-5000-000000000002")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        //[AdminSessionFilter(Order = 1)]
        //[PermissionFilter(Order = 2, Permission = "00000000-0000-0000-2000-000000000001")]
        [OperationAuditFilter(Operation = "查询医院数据", OperationAuditTypeName = "查询医院数据")]
        public JsonResult Load(string srh_GskHospital, string srh_HospitalName, string srh_HospitalMarket, string srh_OHHospitalType, int rows, int page)
        {
            int total;
            var list = hospitalService.LoadPage(srh_GskHospital, srh_HospitalName, srh_HospitalMarket, srh_OHHospitalType, rows, page, out total);
            return Json(new { state = 1, rows = list, total = total });
        }


        [HttpPost]
        //[AdminSessionFilter(Order = 1)]
        //[PermissionFilter(Order = 2, Permission = "00000000-0000-0000-2000-000000000001")]
        [OperationAuditFilter(Operation = "查询医院数据", OperationAuditTypeName = "查询医院数据")]
        public JsonResult LoadTerritory(string srh_GskHospital, string srh_HospitalName, string srh_MUDID, string srh_TerritoryCode, string srh_HospitalMarket, string srh_HospitalTA, int rows, int page)
        {
            int total;
            var list = hospitalService.LoadTAPage(srh_GskHospital, srh_HospitalName, srh_MUDID, srh_TerritoryCode, srh_HospitalMarket, srh_HospitalTA, rows, page, out total);
            return Json(new { state = 1, rows = list, total = total });
        }

        //[AdminSessionFilter(Order = 1)]
        //[PermissionFilter(Order = 2, Permission = "00000000-0000-0000-2000-000000000001")]
        [OperationAuditFilter(Operation = "导出医院列表", OperationAuditTypeName = "查询医院数据")]
        public ActionResult ExportHospitalList(string srh_GskHospital, string srh_HospitalName, string srh_HospitalMarket, string srh_OHHospitalType)
        {
            var list = hospitalService.Load(srh_GskHospital, srh_HospitalName, srh_HospitalMarket, srh_OHHospitalType);
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
                sheet.SetColumnWidth(0, 25 * 256);
                sheet.SetColumnWidth(1, 15 * 256);
                sheet.SetColumnWidth(2, 15 * 256);
                sheet.SetColumnWidth(3, 35 * 256);
                sheet.SetColumnWidth(4, 40 * 256);
                sheet.SetColumnWidth(5, 15 * 256);
                sheet.SetColumnWidth(6, 10 * 256);
                sheet.SetColumnWidth(7, 15 * 256);
                sheet.SetColumnWidth(8, 15 * 256);
                sheet.SetColumnWidth(9, 15 * 256);
                sheet.SetColumnWidth(10, 15 * 256);
                sheet.SetColumnWidth(11, 15 * 256);
                sheet.SetColumnWidth(12, 20 * 256);
                sheet.SetColumnWidth(13, 15 * 256);
                sheet.SetColumnWidth(14, 15 * 256);
                sheet.SetColumnWidth(15, 15 * 256);
                sheet.SetColumnWidth(16, 15 * 256);
                sheet.SetColumnWidth(17, 15 * 256);
                sheet.SetColumnWidth(18, 15 * 256);
                sheet.SetColumnWidth(19, 15 * 256);
                sheet.SetColumnWidth(20, 15 * 256);
                sheet.SetColumnWidth(21, 20 * 256);
                sheet.SetColumnWidth(22, 20 * 256);
                sheet.SetColumnWidth(23, 20 * 256);

                var cell = row.CreateCell(0);
                cell.SetCellValue("医院代码");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(1);
                cell.SetCellValue("省份");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(2);
                cell.SetCellValue("城市");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(3);
                cell.SetCellValue("医院名称");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(4);
                cell.SetCellValue("医院地址");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(5);
                cell.SetCellValue("是否为主地址");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(6);
                cell.SetCellValue("Market");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(7);
                cell.SetCellValue("经度");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(8);
                cell.SetCellValue("纬度");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(9);
                cell.SetCellValue("区县");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(10);
                cell.SetCellValue("区县编码");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(11);
                cell.SetCellValue("客户默认性质");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(12);
                cell.SetCellValue("Total品牌数");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(13);
                cell.SetCellValue("XMS品牌数");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(14);
                cell.SetCellValue("BDS品牌数");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(15);
                cell.SetCellValue("XMS早餐数");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(16);
                cell.SetCellValue("XMS正餐数");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(17);
                cell.SetCellValue("XMS下午茶数");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(18);
                cell.SetCellValue("BDS早餐数");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(19);
                cell.SetCellValue("BDS正餐数");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(20);
                cell.SetCellValue("BDS下午茶数");
                cell.CellStyle = headerStyle;                
                cell = row.CreateCell(21);
                cell.SetCellValue("Total早餐品牌数");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(22);
                cell.SetCellValue("Total正餐品牌数");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(23);
                cell.SetCellValue("Total下午茶品牌数");
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
                P_HOSPITAL_DATA_VIEW disItm;
                int dataCnt = list.Count;
                for (int i = 0; i < dataCnt; i++)
                {
                    disItm = list[i];
                    row = sheet.CreateRow(1 + i);
                    #region data cell
                    cell = row.CreateCell(0);
                    cell.SetCellValue(disItm.GskHospital);// "医院ID");
                    cell = row.CreateCell(1);
                    cell.SetCellValue(disItm.ProvinceName);//"省");
                    cell = row.CreateCell(2);
                    cell.SetCellValue(disItm.CityName);// "市");
                    cell = row.CreateCell(3);
                    cell.SetCellValue(disItm.Name);// "医院名称");
                    cell = row.CreateCell(4);
                    if (disItm.GskHospital == disItm.HospitalCode)
                    {
                        cell.SetCellValue(disItm.Address);// "医院地址");
                    }
                    else
                    {
                        var address = disItm.MainAddress + ":" + disItm.Address;
                        cell.SetCellValue(address);// "医院地址");
                    }
                    cell = row.CreateCell(5);
                    cell.SetCellValue(disItm.MainAddress);// "是否为主地址");
                    cell = row.CreateCell(6);
                    cell.SetCellValue(disItm.Type);// "Market");
                    cell = row.CreateCell(7);
                    cell.SetCellValue(disItm.Longitude);
                    cell = row.CreateCell(8);
                    cell.SetCellValue(disItm.Latitude);
                    cell = row.CreateCell(9);
                    cell.SetCellValue(disItm.District);
                    cell = row.CreateCell(10);
                    cell.SetCellValue(disItm.DistrictCode);
                    cell = row.CreateCell(11);
                    cell.SetCellValue(disItm.CustomerType);
                    cell = row.CreateCell(12);
                    cell.SetCellValue(disItm.TotalCount);
                    cell = row.CreateCell(13);
                    cell.SetCellValue(disItm.XMSTotalCount);
                    cell = row.CreateCell(14);
                    cell.SetCellValue(disItm.BDSTotalCount);
                    cell = row.CreateCell(15);
                    cell.SetCellValue(disItm.XMSBreakfastCount);
                    cell = row.CreateCell(16);
                    cell.SetCellValue(disItm.XMSLunchCount);
                    cell = row.CreateCell(17);
                    cell.SetCellValue(disItm.XMSTeaCount);
                    cell = row.CreateCell(18);
                    cell.SetCellValue(disItm.BDSBreakfastCount);
                    cell = row.CreateCell(19);
                    cell.SetCellValue(disItm.BDSLunchCount);
                    cell = row.CreateCell(20);
                    cell.SetCellValue(disItm.BDSTeaCount);                    
                    cell = row.CreateCell(21);
                    cell.SetCellValue(disItm.TotalBreakfastCount);
                    cell = row.CreateCell(22);
                    cell.SetCellValue(disItm.TotalLunchCount);
                    cell = row.CreateCell(23);
                    cell.SetCellValue(disItm.TotalTeaCount);
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
                return File(excelData, "application/vnd.ms-excel", HttpUtility.UrlEncode("Hospital_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".xlsx", System.Text.Encoding.UTF8));
            }
            else
            {
                ViewBag.Msg = "无符合条件数据！";
                return View();
            }
        }

        //[AdminSessionFilter(Order = 1)]
        //[PermissionFilter(Order = 2, Permission = "00000000-0000-0000-2000-000000000001")]
        [OperationAuditFilter(Operation = "导出医院数据", OperationAuditTypeName = "查询医院数据")]
        public ActionResult ExportHospitalData(string srh_GskHospital, string srh_HospitalName, string srh_HospitalMarket, string srh_OHHospitalType)
        {
            var Hlist = hospitalService.LoadHData(srh_GskHospital, srh_HospitalName, srh_HospitalMarket, srh_OHHospitalType);
            //获取Rx所有TERRITORY_TA
            var TERRITORY_TAlist = hospitalService.LoadTERRITORY_TAByMarket("Rx");
            if (Hlist != null && Hlist.Count > 0)
            {
                XSSFWorkbook book = new XSSFWorkbook();
                int yuanwai = 0;
                int DDT = 0;
                int Rx = 0;
                int Vx = 0;
                int TSKF = 0;

                #region 循环
                for (int i = 0; i < Hlist.Count; i++)
                {
                    //院外数据
                    if (Hlist[i].External == 1)
                    {
                        ISheet sheet;
                        IRow row;
                        ICell cell;

                        if (yuanwai == 0)
                        {
                            //创建表头
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
                            sheet = book.CreateSheet("院外");
                            row = sheet.CreateRow(0);
                            #region header
                            sheet.SetColumnWidth(0, 20 * 256);
                            sheet.SetColumnWidth(1, 15 * 256);
                            sheet.SetColumnWidth(2, 15 * 256);
                            sheet.SetColumnWidth(3, 40 * 256);
                            sheet.SetColumnWidth(4, 15 * 256);
                            sheet.SetColumnWidth(5, 15 * 256);
                            sheet.SetColumnWidth(6, 10 * 256);
                            sheet.SetColumnWidth(7, 20 * 256);
                            sheet.SetColumnWidth(8, 30 * 256);
                            sheet.SetColumnWidth(9, 15 * 256);
                            sheet.SetColumnWidth(10, 15 * 256);
                            sheet.SetColumnWidth(11, 15 * 256);
                            sheet.SetColumnWidth(12, 15 * 256);
                            sheet.SetColumnWidth(13, 15 * 256);
                            sheet.SetColumnWidth(14, 15 * 256);
                            sheet.SetColumnWidth(15, 15 * 256);
                            sheet.SetColumnWidth(16, 15 * 256);
                            sheet.SetColumnWidth(17, 15 * 256);
                            sheet.SetColumnWidth(18, 15 * 256);
                            sheet.SetColumnWidth(19, 15 * 256);
                            sheet.SetColumnWidth(20, 15 * 256);
                            sheet.SetColumnWidth(21, 15 * 256);
                            sheet.SetColumnWidth(22, 15 * 256);
                            sheet.SetColumnWidth(23, 15 * 256);
                            sheet.SetColumnWidth(24, 15 * 256);
                            sheet.SetColumnWidth(25, 15 * 256);
                            sheet.SetColumnWidth(26, 15 * 256);
                            sheet.SetColumnWidth(27, 25 * 256);
                            sheet.SetColumnWidth(28, 15 * 256);
                            sheet.SetColumnWidth(29, 15 * 256);
                            sheet.SetColumnWidth(30, 15 * 256);
                            sheet.SetColumnWidth(31, 20 * 256);
                            sheet.SetColumnWidth(32, 20 * 256);
                            sheet.SetColumnWidth(33, 20 * 256);
                            sheet.SetColumnWidth(34, 20 * 256);
                            sheet.SetColumnWidth(35, 20 * 256);
                            sheet.SetColumnWidth(36, 20 * 256);
                            sheet.SetColumnWidth(37, 20 * 256);
                            sheet.SetColumnWidth(38, 20 * 256);
                            sheet.SetColumnWidth(39, 20 * 256);
                            sheet.SetColumnWidth(40, 20 * 256);
                            sheet.SetColumnWidth(41, 20 * 256);
                            sheet.SetColumnWidth(42, 20 * 256);
                            sheet.SetColumnWidth(43, 20 * 256);
                            sheet.SetColumnWidth(44, 20 * 256);

                            cell = row.CreateCell(0);
                            cell.SetCellValue("医院代码");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(1);
                            cell.SetCellValue("省份");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(2);
                            cell.SetCellValue("城市");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(3);
                            cell.SetCellValue("医院名称");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(4);
                            cell.SetCellValue("医院地址");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(5);
                            cell.SetCellValue("是否为主地址");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(6);
                            cell.SetCellValue("Market");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(7);
                            cell.SetCellValue("原医院代码");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(8);
                            cell.SetCellValue("原医院名称");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(9);
                            cell.SetCellValue("经度");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(10);
                            cell.SetCellValue("纬度");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(11);
                            cell.SetCellValue("区县");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(12);
                            cell.SetCellValue("区县编码");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(13);
                            cell.SetCellValue("客户默认性质");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(14);
                            cell.SetCellValue("Region");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(15);
                            cell.SetCellValue("Total品牌数");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(16);
                            cell.SetCellValue("XMS品牌数");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(17);
                            cell.SetCellValue("BDS品牌数");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(18);
                            cell.SetCellValue("XMS早餐数");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(19);
                            cell.SetCellValue("XMS正餐数");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(20);
                            cell.SetCellValue("XMS下午茶数");
                            cell.CellStyle = headerStyle;                           
                            cell = row.CreateCell(21);
                            cell.SetCellValue("BDS早餐数");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(22);
                            cell.SetCellValue("BDS正餐数");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(23);
                            cell.SetCellValue("BDS下午茶数");
                            cell.CellStyle = headerStyle;                           
                            cell = row.CreateCell(24);
                            cell.SetCellValue("Total早餐品牌数");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(25);
                            cell.SetCellValue("Total正餐品牌数");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(26);
                            cell.SetCellValue("Total下午茶品牌数");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(27);
                            cell.SetCellValue("说明备注");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(28);
                            cell.SetCellValue("2017年订单数");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(29);
                            cell.SetCellValue("2018年订单数");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(30);
                            cell.SetCellValue("2019年订单数");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(31);
                            cell.SetCellValue("17-19年订单总量");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(32);
                            cell.SetCellValue("2020-YTD订单数");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(33);
                            cell.SetCellValue("2020年1月订单数");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(34);
                            cell.SetCellValue("2020年2月订单数");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(35);
                            cell.SetCellValue("2020年3月订单数");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(36);
                            cell.SetCellValue("2020年4月订单数");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(37);
                            cell.SetCellValue("2020年5月订单数");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(38);
                            cell.SetCellValue("2020年6月订单数");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(39);
                            cell.SetCellValue("2020年7月订单数");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(40);
                            cell.SetCellValue("2020年8月订单数");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(41);
                            cell.SetCellValue("2020年9月订单数");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(42);
                            cell.SetCellValue("2020年10月订单数");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(43);
                            cell.SetCellValue("2020年11月订单数");
                            cell.CellStyle = headerStyle;
                            cell = row.CreateCell(44);
                            cell.SetCellValue("2020年12月订单数");
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

                            //绑符合条件的第一行数据
                            P_HOSPITAL_DATA_VIEW disItm;
                            disItm = Hlist[i];
                            row = sheet.CreateRow(1 + yuanwai);
                            #region data cell
                            cell = row.CreateCell(0);
                            cell.SetCellValue(disItm.GskHospital);// "医院ID");
                            cell = row.CreateCell(1);
                            cell.SetCellValue(disItm.ProvinceName);//"省");
                            cell = row.CreateCell(2);
                            cell.SetCellValue(disItm.CityName);// "市");
                            cell = row.CreateCell(3);
                            cell.SetCellValue(disItm.Name);// "医院名称");
                            cell = row.CreateCell(4);
                            cell.SetCellValue(disItm.Address);// "医院地址");
                            cell = row.CreateCell(5);
                            cell.SetCellValue(disItm.MainAddress);// "是否为主地址");
                            cell = row.CreateCell(6);
                            cell.SetCellValue(disItm.Type);// "Market");
                            cell = row.CreateCell(7);
                            cell.SetCellValue(disItm.OldGskHospital);// "原医院代码");
                            cell = row.CreateCell(8);
                            cell.SetCellValue(disItm.OldName);// "原医院名称");
                            cell = row.CreateCell(9);
                            cell.SetCellValue(disItm.Longitude);
                            cell = row.CreateCell(10);
                            cell.SetCellValue(disItm.Latitude);
                            cell = row.CreateCell(11);
                            cell.SetCellValue(disItm.District);
                            cell = row.CreateCell(12);
                            cell.SetCellValue(disItm.DistrictCode);
                            cell = row.CreateCell(13);
                            cell.SetCellValue(disItm.CustomerType);
                            cell = row.CreateCell(14);
                            cell.SetCellValue(disItm.Region);
                            cell = row.CreateCell(15);
                            cell.SetCellValue(disItm.TotalCount);
                            cell = row.CreateCell(16);
                            cell.SetCellValue(disItm.XMSTotalCount);
                            cell = row.CreateCell(17);
                            cell.SetCellValue(disItm.BDSTotalCount);
                            cell = row.CreateCell(18);
                            cell.SetCellValue(disItm.XMSBreakfastCount);
                            cell = row.CreateCell(19);
                            cell.SetCellValue(disItm.XMSLunchCount);
                            cell = row.CreateCell(20);
                            cell.SetCellValue(disItm.XMSTeaCount);                            
                            cell = row.CreateCell(21);
                            cell.SetCellValue(disItm.BDSBreakfastCount);
                            cell = row.CreateCell(22);
                            cell.SetCellValue(disItm.BDSLunchCount);
                            cell = row.CreateCell(23);
                            cell.SetCellValue(disItm.BDSTeaCount);                            
                            cell = row.CreateCell(24);
                            cell.SetCellValue(disItm.TotalBreakfastCount);
                            cell = row.CreateCell(25);
                            cell.SetCellValue(disItm.TotalLunchCount);
                            cell = row.CreateCell(26);
                            cell.SetCellValue(disItm.TotalTeaCount);
                            cell = row.CreateCell(27);
                            cell.SetCellValue(disItm.Remark);
                            cell = row.CreateCell(28);
                            cell.SetCellValue(disItm.Order_2017);
                            cell = row.CreateCell(29);
                            cell.SetCellValue(disItm.Order_2018);
                            cell = row.CreateCell(30);
                            cell.SetCellValue(disItm.Order_2019);
                            cell = row.CreateCell(31);
                            cell.SetCellValue(disItm.Order_201719);
                            cell = row.CreateCell(32);
                            cell.SetCellValue(disItm.Order_2020);
                            cell = row.CreateCell(33);
                            cell.SetCellValue(disItm.Order_202001);
                            cell = row.CreateCell(34);
                            cell.SetCellValue(disItm.Order_202002);
                            cell = row.CreateCell(35);
                            cell.SetCellValue(disItm.Order_202003);
                            cell = row.CreateCell(36);
                            cell.SetCellValue(disItm.Order_202004);
                            cell = row.CreateCell(37);
                            cell.SetCellValue(disItm.Order_202005);
                            cell = row.CreateCell(38);
                            cell.SetCellValue(disItm.Order_202006);
                            cell = row.CreateCell(39);
                            cell.SetCellValue(disItm.Order_202007);
                            cell = row.CreateCell(40);
                            cell.SetCellValue(disItm.Order_202008);
                            cell = row.CreateCell(41);
                            cell.SetCellValue(disItm.Order_202009);
                            cell = row.CreateCell(42);
                            cell.SetCellValue(disItm.Order_202010);
                            cell = row.CreateCell(43);
                            cell.SetCellValue(disItm.Order_202011);
                            cell = row.CreateCell(44);
                            cell.SetCellValue(disItm.Order_202012);
                            #endregion
                        }
                        else
                        {
                            //绑除去第一行的其他符合条件数据
                            P_HOSPITAL_DATA_VIEW disItm;
                            disItm = Hlist[i];
                            sheet = book.GetSheet("院外");
                            row = sheet.CreateRow(1 + yuanwai);
                            #region data cell
                            cell = row.CreateCell(0);
                            cell.SetCellValue(disItm.GskHospital);// "医院ID");
                            cell = row.CreateCell(1);
                            cell.SetCellValue(disItm.ProvinceName);//"省");
                            cell = row.CreateCell(2);
                            cell.SetCellValue(disItm.CityName);// "市");
                            cell = row.CreateCell(3);
                            cell.SetCellValue(disItm.Name);// "医院名称");
                            cell = row.CreateCell(4);
                            cell.SetCellValue(disItm.Address);// "医院地址");
                            cell = row.CreateCell(5);
                            cell.SetCellValue(disItm.MainAddress);// "是否为主地址");
                            cell = row.CreateCell(6);
                            cell.SetCellValue(disItm.Type);// "Market");
                            cell = row.CreateCell(7);
                            cell.SetCellValue(disItm.OldGskHospital);// "原医院代码");
                            cell = row.CreateCell(8);
                            cell.SetCellValue(disItm.OldName);// "原医院名称");
                            cell = row.CreateCell(9);
                            cell.SetCellValue(disItm.Longitude);
                            cell = row.CreateCell(10);
                            cell.SetCellValue(disItm.Latitude);
                            cell = row.CreateCell(11);
                            cell.SetCellValue(disItm.District);
                            cell = row.CreateCell(12);
                            cell.SetCellValue(disItm.DistrictCode);
                            cell = row.CreateCell(13);
                            cell.SetCellValue(disItm.CustomerType);
                            cell = row.CreateCell(14);
                            cell.SetCellValue(disItm.Region);
                            cell = row.CreateCell(15);
                            cell.SetCellValue(disItm.TotalCount);
                            cell = row.CreateCell(16);
                            cell.SetCellValue(disItm.XMSTotalCount);
                            cell = row.CreateCell(17);
                            cell.SetCellValue(disItm.BDSTotalCount);
                            cell = row.CreateCell(18);
                            cell.SetCellValue(disItm.XMSBreakfastCount);
                            cell = row.CreateCell(19);
                            cell.SetCellValue(disItm.XMSLunchCount);
                            cell = row.CreateCell(20);
                            cell.SetCellValue(disItm.XMSTeaCount);
                            cell = row.CreateCell(21);
                            cell.SetCellValue(disItm.BDSBreakfastCount);
                            cell = row.CreateCell(22);
                            cell.SetCellValue(disItm.BDSLunchCount);
                            cell = row.CreateCell(23);
                            cell.SetCellValue(disItm.BDSTeaCount);
                            cell = row.CreateCell(24);
                            cell.SetCellValue(disItm.TotalBreakfastCount);
                            cell = row.CreateCell(25);
                            cell.SetCellValue(disItm.TotalLunchCount);
                            cell = row.CreateCell(26);
                            cell.SetCellValue(disItm.TotalTeaCount);
                            cell = row.CreateCell(27);
                            cell.SetCellValue(disItm.Remark);
                            cell = row.CreateCell(28);
                            cell.SetCellValue(disItm.Order_2017);
                            cell = row.CreateCell(29);
                            cell.SetCellValue(disItm.Order_2018);
                            cell = row.CreateCell(30);
                            cell.SetCellValue(disItm.Order_2019);
                            cell = row.CreateCell(31);
                            cell.SetCellValue(disItm.Order_201719);
                            cell = row.CreateCell(32);
                            cell.SetCellValue(disItm.Order_2020);
                            cell = row.CreateCell(33);
                            cell.SetCellValue(disItm.Order_202001);
                            cell = row.CreateCell(34);
                            cell.SetCellValue(disItm.Order_202002);
                            cell = row.CreateCell(35);
                            cell.SetCellValue(disItm.Order_202003);
                            cell = row.CreateCell(36);
                            cell.SetCellValue(disItm.Order_202004);
                            cell = row.CreateCell(37);
                            cell.SetCellValue(disItm.Order_202005);
                            cell = row.CreateCell(38);
                            cell.SetCellValue(disItm.Order_202006);
                            cell = row.CreateCell(39);
                            cell.SetCellValue(disItm.Order_202007);
                            cell = row.CreateCell(40);
                            cell.SetCellValue(disItm.Order_202008);
                            cell = row.CreateCell(41);
                            cell.SetCellValue(disItm.Order_202009);
                            cell = row.CreateCell(42);
                            cell.SetCellValue(disItm.Order_202010);
                            cell = row.CreateCell(43);
                            cell.SetCellValue(disItm.Order_202011);
                            cell = row.CreateCell(44);
                            cell.SetCellValue(disItm.Order_202012);
                            #endregion

                        }
                        yuanwai = yuanwai + 1;
                    }
                    else
                    {
                        //院内数据
                        //Rx
                        if (Hlist[i].Type == "Rx")
                        {
                            ISheet sheetRx;
                            IRow rowRx;
                            ICell cellRx;

                            if (Rx == 0)
                            {
                                //创建表头
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

                                sheetRx = book.CreateSheet("Rx");
                                rowRx = sheetRx.CreateRow(0);

                                #region header
                                sheetRx.SetColumnWidth(0, 20 * 256);
                                sheetRx.SetColumnWidth(1, 15 * 256);
                                sheetRx.SetColumnWidth(2, 15 * 256);
                                sheetRx.SetColumnWidth(3, 40 * 256);
                                sheetRx.SetColumnWidth(4, 40 * 256);
                                sheetRx.SetColumnWidth(5, 15 * 256);
                                sheetRx.SetColumnWidth(6, 10 * 256);
                                sheetRx.SetColumnWidth(7, 20 * 256);
                                sheetRx.SetColumnWidth(8, 30 * 256);
                                sheetRx.SetColumnWidth(9, 15 * 256);
                                sheetRx.SetColumnWidth(10, 15 * 256);
                                sheetRx.SetColumnWidth(11, 15 * 256);
                                sheetRx.SetColumnWidth(12, 15 * 256);
                                sheetRx.SetColumnWidth(13, 15 * 256);
                                //sheetRx.SetColumnWidth(14, 10 * 256);
                                //sheetRx.SetColumnWidth(15, 10 * 256);
                                //sheetRx.SetColumnWidth(16, 10 * 256);
                                //sheetRx.SetColumnWidth(17, 10 * 256);
                                //sheetRx.SetColumnWidth(18, 10 * 256);
                                //sheetRx.SetColumnWidth(19, 10 * 256);
                                //sheetRx.SetColumnWidth(20, 10 * 256);
                                sheetRx.SetColumnWidth(14, 15 * 256);
                                sheetRx.SetColumnWidth(15, 15 * 256);
                                sheetRx.SetColumnWidth(16, 15 * 256);
                                sheetRx.SetColumnWidth(17, 15 * 256);
                                sheetRx.SetColumnWidth(18, 15 * 256);
                                sheetRx.SetColumnWidth(19, 15 * 256);
                                sheetRx.SetColumnWidth(20, 15 * 256);
                                sheetRx.SetColumnWidth(21, 15 * 256);
                                sheetRx.SetColumnWidth(22, 15 * 256);
                                sheetRx.SetColumnWidth(23, 15 * 256);
                                sheetRx.SetColumnWidth(24, 15 * 256);
                                sheetRx.SetColumnWidth(25, 15 * 256);
                                sheetRx.SetColumnWidth(26, 20 * 256);
                                sheetRx.SetColumnWidth(27, 20 * 256);
                                sheetRx.SetColumnWidth(28, 20 * 256);
                                sheetRx.SetColumnWidth(29, 20 * 256);
                                sheetRx.SetColumnWidth(30, 20 * 256);
                                sheetRx.SetColumnWidth(31, 20 * 256);
                                sheetRx.SetColumnWidth(32, 20 * 256);
                                sheetRx.SetColumnWidth(33, 20 * 256);
                                sheetRx.SetColumnWidth(34, 20 * 256);
                                sheetRx.SetColumnWidth(35, 20 * 256);
                                sheetRx.SetColumnWidth(36, 20 * 256);
                                sheetRx.SetColumnWidth(37, 20 * 256);
                                sheetRx.SetColumnWidth(38, 20 * 256);
                                sheetRx.SetColumnWidth(39, 20 * 256);
                                sheetRx.SetColumnWidth(40, 20 * 256);
                                sheetRx.SetColumnWidth(41, 20 * 256);
                                sheetRx.SetColumnWidth(42, 20 * 256);
                                sheetRx.SetColumnWidth(43, 20 * 256);

                                cellRx = rowRx.CreateCell(0);
                                cellRx.SetCellValue("医院代码");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(1);
                                cellRx.SetCellValue("省份");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(2);
                                cellRx.SetCellValue("城市");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(3);
                                cellRx.SetCellValue("医院名称");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(4);
                                cellRx.SetCellValue("医院地址");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(5);
                                cellRx.SetCellValue("是否为主地址");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(6);
                                cellRx.SetCellValue("Market");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(7);
                                cellRx.SetCellValue("原医院代码");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(8);
                                cellRx.SetCellValue("原医院名称");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(9);
                                cellRx.SetCellValue("经度");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(10);
                                cellRx.SetCellValue("纬度");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(11);
                                cellRx.SetCellValue("区县");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(12);
                                cellRx.SetCellValue("区县编码");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(13);
                                cellRx.SetCellValue("客户默认性质代码");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(14);
                                cellRx.SetCellValue("Total品牌数");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(15);
                                cellRx.SetCellValue("XMS品牌数");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(16);
                                cellRx.SetCellValue("BDS品牌数");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(17);
                                cellRx.SetCellValue("XMS早餐数");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(18);
                                cellRx.SetCellValue("XMS正餐数");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(19);
                                cellRx.SetCellValue("XMS下午茶数");
                                cellRx.CellStyle = headerStyle;                                
                                cellRx = rowRx.CreateCell(20);
                                cellRx.SetCellValue("BDS早餐数");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(21);
                                cellRx.SetCellValue("BDS正餐数");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(22);
                                cellRx.SetCellValue("BDS下午茶数");
                                cellRx.CellStyle = headerStyle;                                
                                cellRx = rowRx.CreateCell(23);
                                cellRx.SetCellValue("Total早餐品牌数");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(24);
                                cellRx.SetCellValue("Total正餐品牌数");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(25);
                                cellRx.SetCellValue("Total下午茶品牌数");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(26);
                                cellRx.SetCellValue("说明备注");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(27);
                                cellRx.SetCellValue("2017年订单数");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(28);
                                cellRx.SetCellValue("2018年订单数");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(29);
                                cellRx.SetCellValue("2019年订单数");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(30);
                                cellRx.SetCellValue("17-19年订单总量");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(31);
                                cellRx.SetCellValue("2020-YTD订单数");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(32);
                                cellRx.SetCellValue("2020年1月订单数");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(33);
                                cellRx.SetCellValue("2020年2月订单数");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(34);
                                cellRx.SetCellValue("2020年3月订单数");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(35);
                                cellRx.SetCellValue("2020年4月订单数");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(36);
                                cellRx.SetCellValue("2020年5月订单数");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(37);
                                cellRx.SetCellValue("2020年6月订单数");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(38);
                                cellRx.SetCellValue("2020年7月订单数");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(39);
                                cellRx.SetCellValue("2020年8月订单数");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(40);
                                cellRx.SetCellValue("2020年9月订单数");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(41);
                                cellRx.SetCellValue("2020年10月订单数");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(42);
                                cellRx.SetCellValue("2020年11月订单数");
                                cellRx.CellStyle = headerStyle;
                                cellRx = rowRx.CreateCell(43);
                                cellRx.SetCellValue("2020年12月订单数");
                                cellRx.CellStyle = headerStyle;
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

                                //绑符合条件的第一行数据
                                P_HOSPITAL_DATA_VIEW disItm;
                                disItm = Hlist[i];
                                rowRx = sheetRx.CreateRow(1 + Rx);
                                #region data cell
                                cellRx = rowRx.CreateCell(0);
                                cellRx.SetCellValue(disItm.GskHospital);// "医院ID");
                                cellRx = rowRx.CreateCell(1);
                                cellRx.SetCellValue(disItm.ProvinceName);//"省");
                                cellRx = rowRx.CreateCell(2);
                                cellRx.SetCellValue(disItm.CityName);// "市");
                                cellRx = rowRx.CreateCell(3);
                                cellRx.SetCellValue(disItm.Name);// "医院名称");
                                cellRx = rowRx.CreateCell(4);
                                cellRx.SetCellValue(disItm.Address);// "医院地址");
                                cellRx = rowRx.CreateCell(5);
                                cellRx.SetCellValue(disItm.MainAddress);// "是否为主地址");
                                cellRx = rowRx.CreateCell(6);
                                cellRx.SetCellValue(disItm.Type);// "Market");
                                cellRx = rowRx.CreateCell(7);
                                cellRx.SetCellValue(disItm.OldGskHospital);// "原医院代码");
                                cellRx = rowRx.CreateCell(8);
                                cellRx.SetCellValue(disItm.OldName);// "原医院名称");
                                cellRx = rowRx.CreateCell(9);
                                cellRx.SetCellValue(disItm.Longitude);
                                cellRx = rowRx.CreateCell(10);
                                cellRx.SetCellValue(disItm.Latitude);
                                cellRx = rowRx.CreateCell(11);
                                cellRx.SetCellValue(disItm.District);
                                cellRx = rowRx.CreateCell(12);
                                cellRx.SetCellValue(disItm.DistrictCode);
                                cellRx = rowRx.CreateCell(13);
                                cellRx.SetCellValue(disItm.CustomerType);
                                cellRx = rowRx.CreateCell(14);
                                cellRx.SetCellValue(disItm.TotalCount);
                                cellRx = rowRx.CreateCell(15);
                                cellRx.SetCellValue(disItm.XMSTotalCount);
                                cellRx = rowRx.CreateCell(16);
                                cellRx.SetCellValue(disItm.BDSTotalCount);
                                cellRx = rowRx.CreateCell(17);
                                cellRx.SetCellValue(disItm.XMSBreakfastCount);
                                cellRx = rowRx.CreateCell(18);
                                cellRx.SetCellValue(disItm.XMSLunchCount);
                                cellRx = rowRx.CreateCell(19);
                                cellRx.SetCellValue(disItm.XMSTeaCount);                                
                                cellRx = rowRx.CreateCell(20);
                                cellRx.SetCellValue(disItm.BDSBreakfastCount);
                                cellRx = rowRx.CreateCell(21);
                                cellRx.SetCellValue(disItm.BDSLunchCount);
                                cellRx = rowRx.CreateCell(22);
                                cellRx.SetCellValue(disItm.BDSTeaCount);
                                cellRx = rowRx.CreateCell(23);
                                cellRx.SetCellValue(disItm.TotalBreakfastCount);
                                cellRx = rowRx.CreateCell(24);
                                cellRx.SetCellValue(disItm.TotalLunchCount);
                                cellRx = rowRx.CreateCell(25);
                                cellRx.SetCellValue(disItm.TotalTeaCount);
                                cellRx = rowRx.CreateCell(26);
                                cellRx.SetCellValue(disItm.Remark);
                                cellRx = rowRx.CreateCell(27);
                                cellRx.SetCellValue(disItm.Order_2017);
                                cellRx = rowRx.CreateCell(28);
                                cellRx.SetCellValue(disItm.Order_2018);
                                cellRx = rowRx.CreateCell(29);
                                cellRx.SetCellValue(disItm.Order_2019);
                                cellRx = rowRx.CreateCell(30);
                                cellRx.SetCellValue(disItm.Order_201719);
                                cellRx = rowRx.CreateCell(31);
                                cellRx.SetCellValue(disItm.Order_2020);
                                cellRx = rowRx.CreateCell(32);
                                cellRx.SetCellValue(disItm.Order_202001);
                                cellRx = rowRx.CreateCell(33);
                                cellRx.SetCellValue(disItm.Order_202002);
                                cellRx = rowRx.CreateCell(34);
                                cellRx.SetCellValue(disItm.Order_202003);
                                cellRx = rowRx.CreateCell(35);
                                cellRx.SetCellValue(disItm.Order_202004);
                                cellRx = rowRx.CreateCell(36);
                                cellRx.SetCellValue(disItm.Order_202005);
                                cellRx = rowRx.CreateCell(37);
                                cellRx.SetCellValue(disItm.Order_202006);
                                cellRx = rowRx.CreateCell(38);
                                cellRx.SetCellValue(disItm.Order_202007);
                                cellRx = rowRx.CreateCell(39);
                                cellRx.SetCellValue(disItm.Order_202008);
                                cellRx = rowRx.CreateCell(40);
                                cellRx.SetCellValue(disItm.Order_202009);
                                cellRx = rowRx.CreateCell(41);
                                cellRx.SetCellValue(disItm.Order_202010);
                                cellRx = rowRx.CreateCell(42);
                                cellRx.SetCellValue(disItm.Order_202011);
                                cellRx = rowRx.CreateCell(43);
                                cellRx.SetCellValue(disItm.Order_202012);
                                #endregion
                            }
                            else
                            {
                                //绑除去第一行的其他符合条件数据
                                P_HOSPITAL_DATA_VIEW disItm;
                                disItm = Hlist[i];
                                sheetRx = book.GetSheet("Rx");
                                rowRx = sheetRx.CreateRow(1 + Rx);

                                #region data cell
                                cellRx = rowRx.CreateCell(0);
                                cellRx.SetCellValue(disItm.GskHospital);// "医院ID");
                                cellRx = rowRx.CreateCell(1);
                                cellRx.SetCellValue(disItm.ProvinceName);//"省");
                                cellRx = rowRx.CreateCell(2);
                                cellRx.SetCellValue(disItm.CityName);// "市");
                                cellRx = rowRx.CreateCell(3);
                                cellRx.SetCellValue(disItm.Name);// "医院名称");
                                cellRx = rowRx.CreateCell(4);
                                cellRx.SetCellValue(disItm.Address);// "医院地址");
                                cellRx = rowRx.CreateCell(5);
                                cellRx.SetCellValue(disItm.MainAddress);// "是否为主地址");
                                cellRx = rowRx.CreateCell(6);
                                cellRx.SetCellValue(disItm.Type);// "Market");
                                cellRx = rowRx.CreateCell(7);
                                cellRx.SetCellValue(disItm.OldGskHospital);// "原医院代码");
                                cellRx = rowRx.CreateCell(8);
                                cellRx.SetCellValue(disItm.OldName);// "原医院名称");
                                cellRx = rowRx.CreateCell(9);
                                cellRx.SetCellValue(disItm.Longitude);
                                cellRx = rowRx.CreateCell(10);
                                cellRx.SetCellValue(disItm.Latitude);
                                cellRx = rowRx.CreateCell(11);
                                cellRx.SetCellValue(disItm.District);
                                cellRx = rowRx.CreateCell(12);
                                cellRx.SetCellValue(disItm.DistrictCode);
                                cellRx = rowRx.CreateCell(13);
                                cellRx.SetCellValue(disItm.CustomerType);
                                cellRx = rowRx.CreateCell(14);
                                cellRx.SetCellValue(disItm.TotalCount);
                                cellRx = rowRx.CreateCell(15);
                                cellRx.SetCellValue(disItm.XMSTotalCount);
                                cellRx = rowRx.CreateCell(16);
                                cellRx.SetCellValue(disItm.BDSTotalCount);
                                cellRx = rowRx.CreateCell(17);
                                cellRx.SetCellValue(disItm.XMSBreakfastCount);
                                cellRx = rowRx.CreateCell(18);
                                cellRx.SetCellValue(disItm.XMSLunchCount);
                                cellRx = rowRx.CreateCell(19);
                                cellRx.SetCellValue(disItm.XMSTeaCount);
                                cellRx = rowRx.CreateCell(20);
                                cellRx.SetCellValue(disItm.BDSBreakfastCount);
                                cellRx = rowRx.CreateCell(21);
                                cellRx.SetCellValue(disItm.BDSLunchCount);
                                cellRx = rowRx.CreateCell(22);
                                cellRx.SetCellValue(disItm.BDSTeaCount);
                                cellRx = rowRx.CreateCell(23);
                                cellRx.SetCellValue(disItm.TotalBreakfastCount);
                                cellRx = rowRx.CreateCell(24);
                                cellRx.SetCellValue(disItm.TotalLunchCount);
                                cellRx = rowRx.CreateCell(25);
                                cellRx.SetCellValue(disItm.TotalTeaCount);
                                cellRx = rowRx.CreateCell(26);
                                cellRx.SetCellValue(disItm.Remark);
                                cellRx = rowRx.CreateCell(27);
                                cellRx.SetCellValue(disItm.Order_2017);
                                cellRx = rowRx.CreateCell(28);
                                cellRx.SetCellValue(disItm.Order_2018);
                                cellRx = rowRx.CreateCell(29);
                                cellRx.SetCellValue(disItm.Order_2019);
                                cellRx = rowRx.CreateCell(30);
                                cellRx.SetCellValue(disItm.Order_201719);
                                cellRx = rowRx.CreateCell(31);
                                cellRx.SetCellValue(disItm.Order_2020);
                                cellRx = rowRx.CreateCell(32);
                                cellRx.SetCellValue(disItm.Order_202001);
                                cellRx = rowRx.CreateCell(33);
                                cellRx.SetCellValue(disItm.Order_202002);
                                cellRx = rowRx.CreateCell(34);
                                cellRx.SetCellValue(disItm.Order_202003);
                                cellRx = rowRx.CreateCell(35);
                                cellRx.SetCellValue(disItm.Order_202004);
                                cellRx = rowRx.CreateCell(36);
                                cellRx.SetCellValue(disItm.Order_202005);
                                cellRx = rowRx.CreateCell(37);
                                cellRx.SetCellValue(disItm.Order_202006);
                                cellRx = rowRx.CreateCell(38);
                                cellRx.SetCellValue(disItm.Order_202007);
                                cellRx = rowRx.CreateCell(39);
                                cellRx.SetCellValue(disItm.Order_202008);
                                cellRx = rowRx.CreateCell(40);
                                cellRx.SetCellValue(disItm.Order_202009);
                                cellRx = rowRx.CreateCell(41);
                                cellRx.SetCellValue(disItm.Order_202010);
                                cellRx = rowRx.CreateCell(42);
                                cellRx.SetCellValue(disItm.Order_202011);
                                cellRx = rowRx.CreateCell(43);
                                cellRx.SetCellValue(disItm.Order_202012);
                                #endregion

                            }
                            Rx = Rx + 1;
                        }
                        //Vx
                        else if (Hlist[i].Type == "Vx")
                        {
                            ISheet sheetVx;
                            IRow rowVx;
                            ICell cellVx;

                            if (Vx == 0)
                            {
                                //创建表头
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

                                sheetVx = book.CreateSheet("Vx");
                                rowVx = sheetVx.CreateRow(0);

                                #region header
                                sheetVx.SetColumnWidth(0, 20 * 256);
                                sheetVx.SetColumnWidth(1, 15 * 256);
                                sheetVx.SetColumnWidth(2, 15 * 256);
                                sheetVx.SetColumnWidth(3, 40 * 256);
                                sheetVx.SetColumnWidth(4, 40 * 256);
                                sheetVx.SetColumnWidth(5, 15 * 256);
                                sheetVx.SetColumnWidth(6, 10 * 256);
                                sheetVx.SetColumnWidth(7, 20 * 256);
                                sheetVx.SetColumnWidth(8, 30 * 256);
                                sheetVx.SetColumnWidth(9, 15 * 256);
                                sheetVx.SetColumnWidth(10, 15 * 256);
                                sheetVx.SetColumnWidth(11, 15 * 256);
                                sheetVx.SetColumnWidth(12, 15 * 256);
                                sheetVx.SetColumnWidth(13, 15 * 256);
                                sheetVx.SetColumnWidth(14, 15 * 256);
                                sheetVx.SetColumnWidth(15, 15 * 256);
                                sheetVx.SetColumnWidth(16, 15 * 256);
                                sheetVx.SetColumnWidth(17, 15 * 256);
                                sheetVx.SetColumnWidth(18, 15 * 256);
                                sheetVx.SetColumnWidth(19, 15 * 256);
                                sheetVx.SetColumnWidth(20, 15 * 256);
                                sheetVx.SetColumnWidth(21, 15 * 256);
                                sheetVx.SetColumnWidth(22, 15 * 256);
                                sheetVx.SetColumnWidth(23, 15 * 256);
                                sheetVx.SetColumnWidth(24, 15 * 256);
                                sheetVx.SetColumnWidth(25, 15 * 256);
                                sheetVx.SetColumnWidth(26, 15 * 256);
                                sheetVx.SetColumnWidth(27, 25 * 256);
                                sheetVx.SetColumnWidth(28, 15 * 256);
                                sheetVx.SetColumnWidth(29, 15 * 256);
                                sheetVx.SetColumnWidth(30, 15 * 256);
                                sheetVx.SetColumnWidth(31, 20 * 256);
                                sheetVx.SetColumnWidth(32, 20 * 256);
                                sheetVx.SetColumnWidth(33, 20 * 256);
                                sheetVx.SetColumnWidth(34, 20 * 256);
                                sheetVx.SetColumnWidth(35, 20 * 256);
                                sheetVx.SetColumnWidth(36, 20 * 256);
                                sheetVx.SetColumnWidth(37, 20 * 256);
                                sheetVx.SetColumnWidth(38, 20 * 256);
                                sheetVx.SetColumnWidth(39, 20 * 256);
                                sheetVx.SetColumnWidth(40, 20 * 256);
                                sheetVx.SetColumnWidth(41, 20 * 256);
                                sheetVx.SetColumnWidth(42, 20 * 256);
                                sheetVx.SetColumnWidth(43, 20 * 256);
                                sheetVx.SetColumnWidth(44, 20 * 256);

                                cellVx = rowVx.CreateCell(0);
                                cellVx.SetCellValue("医院代码");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(1);
                                cellVx.SetCellValue("省份");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(2);
                                cellVx.SetCellValue("城市");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(3);
                                cellVx.SetCellValue("医院名称");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(4);
                                cellVx.SetCellValue("医院地址");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(5);
                                cellVx.SetCellValue("是否为主地址");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(6);
                                cellVx.SetCellValue("Market");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(7);
                                cellVx.SetCellValue("原医院代码");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(8);
                                cellVx.SetCellValue("原医院名称");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(9);
                                cellVx.SetCellValue("经度");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(10);
                                cellVx.SetCellValue("纬度");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(11);
                                cellVx.SetCellValue("区县");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(12);
                                cellVx.SetCellValue("区县编码");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(13);
                                cellVx.SetCellValue("客户默认性质");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(14);
                                cellVx.SetCellValue("Region");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(15);
                                cellVx.SetCellValue("Total品牌数");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(16);
                                cellVx.SetCellValue("XMS品牌数");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(17);
                                cellVx.SetCellValue("BDS品牌数");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(18);
                                cellVx.SetCellValue("XMS早餐数");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(19);
                                cellVx.SetCellValue("XMS正餐数");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(20);
                                cellVx.SetCellValue("XMS下午茶数");
                                cellVx.CellStyle = headerStyle;                                
                                cellVx = rowVx.CreateCell(21);
                                cellVx.SetCellValue("BDS早餐数");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(22);
                                cellVx.SetCellValue("BDS正餐数");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(23);
                                cellVx.SetCellValue("BDS下午茶数");
                                cellVx.CellStyle = headerStyle;                               
                                cellVx = rowVx.CreateCell(24);
                                cellVx.SetCellValue("Total早餐品牌数");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(25);
                                cellVx.SetCellValue("Total正餐品牌数");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(26);
                                cellVx.SetCellValue("Total下午茶品牌数");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(27);
                                cellVx.SetCellValue("说明备注");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(28);
                                cellVx.SetCellValue("2017年订单数");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(29);
                                cellVx.SetCellValue("2018年订单数");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(30);
                                cellVx.SetCellValue("2019年订单数");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(31);
                                cellVx.SetCellValue("17-19年订单总量");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(32);
                                cellVx.SetCellValue("2020-YTD订单数");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(33);
                                cellVx.SetCellValue("2020年1月订单数");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(34);
                                cellVx.SetCellValue("2020年2月订单数");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(35);
                                cellVx.SetCellValue("2020年3月订单数");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(36);
                                cellVx.SetCellValue("2020年4月订单数");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(37);
                                cellVx.SetCellValue("2020年5月订单数");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(38);
                                cellVx.SetCellValue("2020年6月订单数");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(39);
                                cellVx.SetCellValue("2020年7月订单数");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(40);
                                cellVx.SetCellValue("2020年8月订单数");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(41);
                                cellVx.SetCellValue("2020年9月订单数");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(42);
                                cellVx.SetCellValue("2020年10月订单数");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(43);
                                cellVx.SetCellValue("2020年11月订单数");
                                cellVx.CellStyle = headerStyle;
                                cellVx = rowVx.CreateCell(44);
                                cellVx.SetCellValue("2020年12月订单数");
                                cellVx.CellStyle = headerStyle;
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

                                //绑符合条件的第一行数据
                                P_HOSPITAL_DATA_VIEW disItm;
                                disItm = Hlist[i];
                                rowVx = sheetVx.CreateRow(1 + Vx);

                                #region data cell
                                cellVx = rowVx.CreateCell(0);
                                cellVx.SetCellValue(disItm.GskHospital);// "医院ID");
                                cellVx = rowVx.CreateCell(1);
                                cellVx.SetCellValue(disItm.ProvinceName);//"省");
                                cellVx = rowVx.CreateCell(2);
                                cellVx.SetCellValue(disItm.CityName);// "市");
                                cellVx = rowVx.CreateCell(3);
                                cellVx.SetCellValue(disItm.Name);// "医院名称");
                                cellVx = rowVx.CreateCell(4);
                                cellVx.SetCellValue(disItm.Address);// "医院地址");
                                cellVx = rowVx.CreateCell(5);
                                cellVx.SetCellValue(disItm.MainAddress);// "是否为主地址");
                                cellVx = rowVx.CreateCell(6);
                                cellVx.SetCellValue(disItm.Type);// "Market");
                                cellVx = rowVx.CreateCell(7);
                                cellVx.SetCellValue(disItm.OldGskHospital);// "原医院代码");
                                cellVx = rowVx.CreateCell(8);
                                cellVx.SetCellValue(disItm.OldName);// "原医院名称");
                                cellVx = rowVx.CreateCell(9);
                                cellVx.SetCellValue(disItm.Longitude);
                                cellVx = rowVx.CreateCell(10);
                                cellVx.SetCellValue(disItm.Latitude);
                                cellVx = rowVx.CreateCell(11);
                                cellVx.SetCellValue(disItm.District);
                                cellVx = rowVx.CreateCell(12);
                                cellVx.SetCellValue(disItm.DistrictCode);
                                cellVx = rowVx.CreateCell(13);
                                cellVx.SetCellValue(disItm.CustomerType);
                                cellVx = rowVx.CreateCell(14);
                                cellVx.SetCellValue(disItm.Region);
                                cellVx = rowVx.CreateCell(15);
                                cellVx.SetCellValue(disItm.TotalCount);
                                cellVx = rowVx.CreateCell(16);
                                cellVx.SetCellValue(disItm.XMSTotalCount);
                                cellVx = rowVx.CreateCell(17);
                                cellVx.SetCellValue(disItm.BDSTotalCount);
                                cellVx = rowVx.CreateCell(18);
                                cellVx.SetCellValue(disItm.XMSBreakfastCount);
                                cellVx = rowVx.CreateCell(19);
                                cellVx.SetCellValue(disItm.XMSLunchCount);
                                cellVx = rowVx.CreateCell(20);
                                cellVx.SetCellValue(disItm.XMSTeaCount);
                                cellVx = rowVx.CreateCell(21);
                                cellVx.SetCellValue(disItm.BDSBreakfastCount);
                                cellVx = rowVx.CreateCell(22);
                                cellVx.SetCellValue(disItm.BDSLunchCount);
                                cellVx = rowVx.CreateCell(23);
                                cellVx.SetCellValue(disItm.BDSTeaCount);                                
                                cellVx = rowVx.CreateCell(24);
                                cellVx.SetCellValue(disItm.TotalBreakfastCount);
                                cellVx = rowVx.CreateCell(25);
                                cellVx.SetCellValue(disItm.TotalLunchCount);
                                cellVx = rowVx.CreateCell(26);
                                cellVx.SetCellValue(disItm.TotalTeaCount);
                                cellVx = rowVx.CreateCell(27);
                                cellVx.SetCellValue(disItm.Remark);
                                cellVx = rowVx.CreateCell(28);
                                cellVx.SetCellValue(disItm.Order_2017);
                                cellVx = rowVx.CreateCell(29);
                                cellVx.SetCellValue(disItm.Order_2018);
                                cellVx = rowVx.CreateCell(30);
                                cellVx.SetCellValue(disItm.Order_2019);
                                cellVx = rowVx.CreateCell(31);
                                cellVx.SetCellValue(disItm.Order_201719);
                                cellVx = rowVx.CreateCell(32);
                                cellVx.SetCellValue(disItm.Order_2020);
                                cellVx = rowVx.CreateCell(33);
                                cellVx.SetCellValue(disItm.Order_202001);
                                cellVx = rowVx.CreateCell(34);
                                cellVx.SetCellValue(disItm.Order_202002);
                                cellVx = rowVx.CreateCell(35);
                                cellVx.SetCellValue(disItm.Order_202003);
                                cellVx = rowVx.CreateCell(36);
                                cellVx.SetCellValue(disItm.Order_202004);
                                cellVx = rowVx.CreateCell(37);
                                cellVx.SetCellValue(disItm.Order_202005);
                                cellVx = rowVx.CreateCell(38);
                                cellVx.SetCellValue(disItm.Order_202006);
                                cellVx = rowVx.CreateCell(39);
                                cellVx.SetCellValue(disItm.Order_202007);
                                cellVx = rowVx.CreateCell(40);
                                cellVx.SetCellValue(disItm.Order_202008);
                                cellVx = rowVx.CreateCell(41);
                                cellVx.SetCellValue(disItm.Order_202009);
                                cellVx = rowVx.CreateCell(42);
                                cellVx.SetCellValue(disItm.Order_202010);
                                cellVx = rowVx.CreateCell(43);
                                cellVx.SetCellValue(disItm.Order_202011);
                                cellVx = rowVx.CreateCell(44);
                                cellVx.SetCellValue(disItm.Order_202012);
                                #endregion
                            }
                            else
                            {
                                //绑除去第一行的其他符合条件数据
                                P_HOSPITAL_DATA_VIEW disItm;
                                disItm = Hlist[i];
                                sheetVx = book.GetSheet("Vx");
                                rowVx = sheetVx.CreateRow(1 + Vx);

                                #region data cell
                                cellVx = rowVx.CreateCell(0);
                                cellVx.SetCellValue(disItm.GskHospital);// "医院ID");
                                cellVx = rowVx.CreateCell(1);
                                cellVx.SetCellValue(disItm.ProvinceName);//"省");
                                cellVx = rowVx.CreateCell(2);
                                cellVx.SetCellValue(disItm.CityName);// "市");
                                cellVx = rowVx.CreateCell(3);
                                cellVx.SetCellValue(disItm.Name);// "医院名称");
                                cellVx = rowVx.CreateCell(4);
                                cellVx.SetCellValue(disItm.Address);// "医院地址");
                                cellVx = rowVx.CreateCell(5);
                                cellVx.SetCellValue(disItm.MainAddress);// "是否为主地址");
                                cellVx = rowVx.CreateCell(6);
                                cellVx.SetCellValue(disItm.Type);// "Market");
                                cellVx = rowVx.CreateCell(7);
                                cellVx.SetCellValue(disItm.OldGskHospital);// "原医院代码");
                                cellVx = rowVx.CreateCell(8);
                                cellVx.SetCellValue(disItm.OldName);// "原医院名称");
                                cellVx = rowVx.CreateCell(9);
                                cellVx.SetCellValue(disItm.Longitude);
                                cellVx = rowVx.CreateCell(10);
                                cellVx.SetCellValue(disItm.Latitude);
                                cellVx = rowVx.CreateCell(11);
                                cellVx.SetCellValue(disItm.District);
                                cellVx = rowVx.CreateCell(12);
                                cellVx.SetCellValue(disItm.DistrictCode);
                                cellVx = rowVx.CreateCell(13);
                                cellVx.SetCellValue(disItm.CustomerType);
                                cellVx = rowVx.CreateCell(14);
                                cellVx.SetCellValue(disItm.Region);
                                cellVx = rowVx.CreateCell(15);
                                cellVx.SetCellValue(disItm.TotalCount);
                                cellVx = rowVx.CreateCell(16);
                                cellVx.SetCellValue(disItm.XMSTotalCount);
                                cellVx = rowVx.CreateCell(17);
                                cellVx.SetCellValue(disItm.BDSTotalCount);
                                cellVx = rowVx.CreateCell(18);
                                cellVx.SetCellValue(disItm.XMSBreakfastCount);
                                cellVx = rowVx.CreateCell(19);
                                cellVx.SetCellValue(disItm.XMSLunchCount);
                                cellVx = rowVx.CreateCell(20);
                                cellVx.SetCellValue(disItm.XMSTeaCount);
                                cellVx = rowVx.CreateCell(21);
                                cellVx.SetCellValue(disItm.BDSBreakfastCount);
                                cellVx = rowVx.CreateCell(22);
                                cellVx.SetCellValue(disItm.BDSLunchCount);
                                cellVx = rowVx.CreateCell(23);
                                cellVx.SetCellValue(disItm.BDSTeaCount);
                                cellVx = rowVx.CreateCell(24);
                                cellVx.SetCellValue(disItm.TotalBreakfastCount);
                                cellVx = rowVx.CreateCell(25);
                                cellVx.SetCellValue(disItm.TotalLunchCount);
                                cellVx = rowVx.CreateCell(26);
                                cellVx.SetCellValue(disItm.TotalTeaCount);
                                cellVx = rowVx.CreateCell(27);
                                cellVx.SetCellValue(disItm.Remark);
                                cellVx = rowVx.CreateCell(28);
                                cellVx.SetCellValue(disItm.Order_2017);
                                cellVx = rowVx.CreateCell(29);
                                cellVx.SetCellValue(disItm.Order_2018);
                                cellVx = rowVx.CreateCell(30);
                                cellVx.SetCellValue(disItm.Order_2019);
                                cellVx = rowVx.CreateCell(31);
                                cellVx.SetCellValue(disItm.Order_201719);
                                cellVx = rowVx.CreateCell(32);
                                cellVx.SetCellValue(disItm.Order_2020);
                                cellVx = rowVx.CreateCell(33);
                                cellVx.SetCellValue(disItm.Order_202001);
                                cellVx = rowVx.CreateCell(34);
                                cellVx.SetCellValue(disItm.Order_202002);
                                cellVx = rowVx.CreateCell(35);
                                cellVx.SetCellValue(disItm.Order_202003);
                                cellVx = rowVx.CreateCell(36);
                                cellVx.SetCellValue(disItm.Order_202004);
                                cellVx = rowVx.CreateCell(37);
                                cellVx.SetCellValue(disItm.Order_202005);
                                cellVx = rowVx.CreateCell(38);
                                cellVx.SetCellValue(disItm.Order_202006);
                                cellVx = rowVx.CreateCell(39);
                                cellVx.SetCellValue(disItm.Order_202007);
                                cellVx = rowVx.CreateCell(40);
                                cellVx.SetCellValue(disItm.Order_202008);
                                cellVx = rowVx.CreateCell(41);
                                cellVx.SetCellValue(disItm.Order_202009);
                                cellVx = rowVx.CreateCell(42);
                                cellVx.SetCellValue(disItm.Order_202010);
                                cellVx = rowVx.CreateCell(43);
                                cellVx.SetCellValue(disItm.Order_202011);
                                cellVx = rowVx.CreateCell(44);
                                cellVx.SetCellValue(disItm.Order_202012);
                                #endregion

                            }
                            Vx = Vx + 1;
                        }
                        //TSKF
                        else if (Hlist[i].Type == "TSKF")
                        {
                            ISheet sheetTSKF;
                            IRow rowTSKF;
                            ICell cellTSKF;

                            if (TSKF == 0)
                            {
                                //创建表头
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

                                sheetTSKF = book.CreateSheet("TSKF");
                                rowTSKF = sheetTSKF.CreateRow(0);

                                #region header
                                sheetTSKF.SetColumnWidth(0, 20 * 256);
                                sheetTSKF.SetColumnWidth(1, 15 * 256);
                                sheetTSKF.SetColumnWidth(2, 15 * 256);
                                sheetTSKF.SetColumnWidth(3, 40 * 256);
                                sheetTSKF.SetColumnWidth(4, 40 * 256);
                                sheetTSKF.SetColumnWidth(5, 15 * 256);
                                sheetTSKF.SetColumnWidth(6, 10 * 256);
                                sheetTSKF.SetColumnWidth(7, 20 * 256);
                                sheetTSKF.SetColumnWidth(8, 30 * 256);
                                sheetTSKF.SetColumnWidth(9, 15 * 256);
                                sheetTSKF.SetColumnWidth(10, 15 * 256);
                                sheetTSKF.SetColumnWidth(11, 15 * 256);
                                sheetTSKF.SetColumnWidth(12, 15 * 256);
                                sheetTSKF.SetColumnWidth(13, 15 * 256);
                                sheetTSKF.SetColumnWidth(14, 15 * 256);
                                sheetTSKF.SetColumnWidth(15, 15 * 256);
                                sheetTSKF.SetColumnWidth(16, 15 * 256);
                                sheetTSKF.SetColumnWidth(17, 15 * 256);
                                sheetTSKF.SetColumnWidth(18, 15 * 256);
                                sheetTSKF.SetColumnWidth(19, 15 * 256);
                                sheetTSKF.SetColumnWidth(20, 15 * 256);
                                sheetTSKF.SetColumnWidth(21, 15 * 256);
                                sheetTSKF.SetColumnWidth(22, 15 * 256);
                                sheetTSKF.SetColumnWidth(23, 15 * 256);
                                sheetTSKF.SetColumnWidth(24, 15 * 256);
                                sheetTSKF.SetColumnWidth(25, 15 * 256);
                                sheetTSKF.SetColumnWidth(26, 15 * 256);
                                sheetTSKF.SetColumnWidth(27, 25 * 256);
                                sheetTSKF.SetColumnWidth(28, 15 * 256);
                                sheetTSKF.SetColumnWidth(29, 15 * 256);
                                sheetTSKF.SetColumnWidth(30, 15 * 256);
                                sheetTSKF.SetColumnWidth(31, 20 * 256);
                                sheetTSKF.SetColumnWidth(32, 20 * 256);
                                sheetTSKF.SetColumnWidth(33, 20 * 256);
                                sheetTSKF.SetColumnWidth(34, 20 * 256);
                                sheetTSKF.SetColumnWidth(35, 20 * 256);
                                sheetTSKF.SetColumnWidth(36, 20 * 256);
                                sheetTSKF.SetColumnWidth(37, 20 * 256);
                                sheetTSKF.SetColumnWidth(38, 20 * 256);
                                sheetTSKF.SetColumnWidth(39, 20 * 256);
                                sheetTSKF.SetColumnWidth(40, 20 * 256);
                                sheetTSKF.SetColumnWidth(41, 20 * 256);
                                sheetTSKF.SetColumnWidth(42, 20 * 256);
                                sheetTSKF.SetColumnWidth(43, 20 * 256);
                                sheetTSKF.SetColumnWidth(44, 20 * 256);

                                cellTSKF = rowTSKF.CreateCell(0);
                                cellTSKF.SetCellValue("医院代码");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(1);
                                cellTSKF.SetCellValue("省份");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(2);
                                cellTSKF.SetCellValue("城市");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(3);
                                cellTSKF.SetCellValue("医院名称");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(4);
                                cellTSKF.SetCellValue("医院地址");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(5);
                                cellTSKF.SetCellValue("是否为主地址");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(6);
                                cellTSKF.SetCellValue("Market");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(7);
                                cellTSKF.SetCellValue("原医院代码");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(8);
                                cellTSKF.SetCellValue("原医院名称");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(9);
                                cellTSKF.SetCellValue("经度");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(10);
                                cellTSKF.SetCellValue("纬度");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(11);
                                cellTSKF.SetCellValue("区县");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(12);
                                cellTSKF.SetCellValue("区县编码");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(13);
                                cellTSKF.SetCellValue("客户默认性质");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(14);
                                cellTSKF.SetCellValue("Region");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(15);
                                cellTSKF.SetCellValue("Total品牌数");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(16);
                                cellTSKF.SetCellValue("XMS品牌数");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(17);
                                cellTSKF.SetCellValue("BDS品牌数");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(18);
                                cellTSKF.SetCellValue("XMS早餐数");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(19);
                                cellTSKF.SetCellValue("XMS正餐数");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(20);
                                cellTSKF.SetCellValue("XMS下午茶数");
                                cellTSKF.CellStyle = headerStyle;                               
                                cellTSKF = rowTSKF.CreateCell(21);
                                cellTSKF.SetCellValue("BDS早餐数");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(22);
                                cellTSKF.SetCellValue("BDS正餐数");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(23);
                                cellTSKF.SetCellValue("BDS下午茶数");
                                cellTSKF.CellStyle = headerStyle;                                
                                cellTSKF = rowTSKF.CreateCell(24);
                                cellTSKF.SetCellValue("Total早餐品牌数");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(25);
                                cellTSKF.SetCellValue("Total正餐品牌数");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(26);
                                cellTSKF.SetCellValue("Total下午茶品牌数");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(27);
                                cellTSKF.SetCellValue("说明备注");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(28);
                                cellTSKF.SetCellValue("2017年订单数");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(29);
                                cellTSKF.SetCellValue("2018年订单数");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(30);
                                cellTSKF.SetCellValue("2019年订单数");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(31);
                                cellTSKF.SetCellValue("17-19年订单总量");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(32);
                                cellTSKF.SetCellValue("2020-YTD订单数");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(33);
                                cellTSKF.SetCellValue("2020年1月订单数");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(34);
                                cellTSKF.SetCellValue("2020年2月订单数");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(35);
                                cellTSKF.SetCellValue("2020年3月订单数");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(36);
                                cellTSKF.SetCellValue("2020年4月订单数");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(37);
                                cellTSKF.SetCellValue("2020年5月订单数");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(38);
                                cellTSKF.SetCellValue("2020年6月订单数");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(39);
                                cellTSKF.SetCellValue("2020年7月订单数");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(40);
                                cellTSKF.SetCellValue("2020年8月订单数");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(41);
                                cellTSKF.SetCellValue("2020年9月订单数");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(42);
                                cellTSKF.SetCellValue("2020年10月订单数");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(43);
                                cellTSKF.SetCellValue("2020年11月订单数");
                                cellTSKF.CellStyle = headerStyle;
                                cellTSKF = rowTSKF.CreateCell(44);
                                cellTSKF.SetCellValue("2020年12月订单数");
                                cellTSKF.CellStyle = headerStyle;
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

                                //绑符合条件的第一行数据
                                P_HOSPITAL_DATA_VIEW disItm;
                                disItm = Hlist[i];
                                rowTSKF = sheetTSKF.CreateRow(1 + TSKF);

                                #region data cell
                                cellTSKF = rowTSKF.CreateCell(0);
                                cellTSKF.SetCellValue(disItm.GskHospital);// "医院ID");
                                cellTSKF = rowTSKF.CreateCell(1);
                                cellTSKF.SetCellValue(disItm.ProvinceName);//"省");
                                cellTSKF = rowTSKF.CreateCell(2);
                                cellTSKF.SetCellValue(disItm.CityName);// "市");
                                cellTSKF = rowTSKF.CreateCell(3);
                                cellTSKF.SetCellValue(disItm.Name);// "医院名称");
                                cellTSKF = rowTSKF.CreateCell(4);
                                cellTSKF.SetCellValue(disItm.Address);// "医院地址");
                                cellTSKF = rowTSKF.CreateCell(5);
                                cellTSKF.SetCellValue(disItm.MainAddress);// "是否为主地址");
                                cellTSKF = rowTSKF.CreateCell(6);
                                cellTSKF.SetCellValue(disItm.Type);// "Market");
                                cellTSKF = rowTSKF.CreateCell(7);
                                cellTSKF.SetCellValue(disItm.OldGskHospital);// "原医院代码");
                                cellTSKF = rowTSKF.CreateCell(8);
                                cellTSKF.SetCellValue(disItm.OldName);// "原医院名称");
                                cellTSKF = rowTSKF.CreateCell(9);
                                cellTSKF.SetCellValue(disItm.Longitude);
                                cellTSKF = rowTSKF.CreateCell(10);
                                cellTSKF.SetCellValue(disItm.Latitude);
                                cellTSKF = rowTSKF.CreateCell(11);
                                cellTSKF.SetCellValue(disItm.District);
                                cellTSKF = rowTSKF.CreateCell(12);
                                cellTSKF.SetCellValue(disItm.DistrictCode);
                                cellTSKF = rowTSKF.CreateCell(13);
                                cellTSKF.SetCellValue(disItm.CustomerType);
                                cellTSKF = rowTSKF.CreateCell(14);
                                cellTSKF.SetCellValue(disItm.Region);
                                cellTSKF = rowTSKF.CreateCell(15);
                                cellTSKF.SetCellValue(disItm.TotalCount);
                                cellTSKF = rowTSKF.CreateCell(16);
                                cellTSKF.SetCellValue(disItm.XMSTotalCount);
                                cellTSKF = rowTSKF.CreateCell(17);
                                cellTSKF.SetCellValue(disItm.BDSTotalCount);
                                cellTSKF = rowTSKF.CreateCell(18);
                                cellTSKF.SetCellValue(disItm.XMSBreakfastCount);
                                cellTSKF = rowTSKF.CreateCell(19);
                                cellTSKF.SetCellValue(disItm.XMSLunchCount);
                                cellTSKF = rowTSKF.CreateCell(20);
                                cellTSKF.SetCellValue(disItm.XMSTeaCount);                                
                                cellTSKF = rowTSKF.CreateCell(21);
                                cellTSKF.SetCellValue(disItm.BDSBreakfastCount);
                                cellTSKF = rowTSKF.CreateCell(22);
                                cellTSKF.SetCellValue(disItm.BDSLunchCount);
                                cellTSKF = rowTSKF.CreateCell(23);
                                cellTSKF.SetCellValue(disItm.BDSTeaCount);                                
                                cellTSKF = rowTSKF.CreateCell(24);
                                cellTSKF.SetCellValue(disItm.TotalBreakfastCount);
                                cellTSKF = rowTSKF.CreateCell(25);
                                cellTSKF.SetCellValue(disItm.TotalLunchCount);
                                cellTSKF = rowTSKF.CreateCell(26);
                                cellTSKF.SetCellValue(disItm.TotalTeaCount);
                                cellTSKF = rowTSKF.CreateCell(27);
                                cellTSKF.SetCellValue(disItm.Remark);
                                cellTSKF = rowTSKF.CreateCell(28);
                                cellTSKF.SetCellValue(disItm.Order_2017);
                                cellTSKF = rowTSKF.CreateCell(29);
                                cellTSKF.SetCellValue(disItm.Order_2018);
                                cellTSKF = rowTSKF.CreateCell(30);
                                cellTSKF.SetCellValue(disItm.Order_2019);
                                cellTSKF = rowTSKF.CreateCell(31);
                                cellTSKF.SetCellValue(disItm.Order_201719);
                                cellTSKF = rowTSKF.CreateCell(32);
                                cellTSKF.SetCellValue(disItm.Order_2020);
                                cellTSKF = rowTSKF.CreateCell(33);
                                cellTSKF.SetCellValue(disItm.Order_202001);
                                cellTSKF = rowTSKF.CreateCell(34);
                                cellTSKF.SetCellValue(disItm.Order_202002);
                                cellTSKF = rowTSKF.CreateCell(35);
                                cellTSKF.SetCellValue(disItm.Order_202003);
                                cellTSKF = rowTSKF.CreateCell(36);
                                cellTSKF.SetCellValue(disItm.Order_202004);
                                cellTSKF = rowTSKF.CreateCell(37);
                                cellTSKF.SetCellValue(disItm.Order_202005);
                                cellTSKF = rowTSKF.CreateCell(38);
                                cellTSKF.SetCellValue(disItm.Order_202006);
                                cellTSKF = rowTSKF.CreateCell(39);
                                cellTSKF.SetCellValue(disItm.Order_202007);
                                cellTSKF = rowTSKF.CreateCell(40);
                                cellTSKF.SetCellValue(disItm.Order_202008);
                                cellTSKF = rowTSKF.CreateCell(41);
                                cellTSKF.SetCellValue(disItm.Order_202009);
                                cellTSKF = rowTSKF.CreateCell(42);
                                cellTSKF.SetCellValue(disItm.Order_202010);
                                cellTSKF = rowTSKF.CreateCell(43);
                                cellTSKF.SetCellValue(disItm.Order_202011);
                                cellTSKF = rowTSKF.CreateCell(44);
                                cellTSKF.SetCellValue(disItm.Order_202012);
                                #endregion
                            }
                            else
                            {
                                //绑除去第一行的其他符合条件数据
                                P_HOSPITAL_DATA_VIEW disItm;
                                disItm = Hlist[i];
                                sheetTSKF = book.GetSheet("TSKF");
                                rowTSKF = sheetTSKF.CreateRow(1 + TSKF);

                                #region data cell
                                cellTSKF = rowTSKF.CreateCell(0);
                                cellTSKF.SetCellValue(disItm.GskHospital);// "医院ID");
                                cellTSKF = rowTSKF.CreateCell(1);
                                cellTSKF.SetCellValue(disItm.ProvinceName);//"省");
                                cellTSKF = rowTSKF.CreateCell(2);
                                cellTSKF.SetCellValue(disItm.CityName);// "市");
                                cellTSKF = rowTSKF.CreateCell(3);
                                cellTSKF.SetCellValue(disItm.Name);// "医院名称");
                                cellTSKF = rowTSKF.CreateCell(4);
                                cellTSKF.SetCellValue(disItm.Address);// "医院地址");
                                cellTSKF = rowTSKF.CreateCell(5);
                                cellTSKF.SetCellValue(disItm.MainAddress);// "是否为主地址");
                                cellTSKF = rowTSKF.CreateCell(6);
                                cellTSKF.SetCellValue(disItm.Type);// "Market");
                                cellTSKF = rowTSKF.CreateCell(7);
                                cellTSKF.SetCellValue(disItm.OldGskHospital);// "原医院代码");
                                cellTSKF = rowTSKF.CreateCell(8);
                                cellTSKF.SetCellValue(disItm.OldName);// "原医院名称");
                                cellTSKF = rowTSKF.CreateCell(9);
                                cellTSKF.SetCellValue(disItm.Longitude);
                                cellTSKF = rowTSKF.CreateCell(10);
                                cellTSKF.SetCellValue(disItm.Latitude);
                                cellTSKF = rowTSKF.CreateCell(11);
                                cellTSKF.SetCellValue(disItm.District);
                                cellTSKF = rowTSKF.CreateCell(12);
                                cellTSKF.SetCellValue(disItm.DistrictCode);
                                cellTSKF = rowTSKF.CreateCell(13);
                                cellTSKF.SetCellValue(disItm.CustomerType);
                                cellTSKF = rowTSKF.CreateCell(14);
                                cellTSKF.SetCellValue(disItm.Region);
                                cellTSKF = rowTSKF.CreateCell(15);
                                cellTSKF.SetCellValue(disItm.TotalCount);
                                cellTSKF = rowTSKF.CreateCell(16);
                                cellTSKF.SetCellValue(disItm.XMSTotalCount);
                                cellTSKF = rowTSKF.CreateCell(17);
                                cellTSKF.SetCellValue(disItm.BDSTotalCount);
                                cellTSKF = rowTSKF.CreateCell(18);
                                cellTSKF.SetCellValue(disItm.XMSBreakfastCount);
                                cellTSKF = rowTSKF.CreateCell(19);
                                cellTSKF.SetCellValue(disItm.XMSLunchCount);
                                cellTSKF = rowTSKF.CreateCell(20);
                                cellTSKF.SetCellValue(disItm.XMSTeaCount);
                                cellTSKF = rowTSKF.CreateCell(21);
                                cellTSKF.SetCellValue(disItm.BDSBreakfastCount);
                                cellTSKF = rowTSKF.CreateCell(22);
                                cellTSKF.SetCellValue(disItm.BDSLunchCount);
                                cellTSKF = rowTSKF.CreateCell(23);
                                cellTSKF.SetCellValue(disItm.BDSTeaCount);
                                cellTSKF = rowTSKF.CreateCell(24);
                                cellTSKF.SetCellValue(disItm.TotalBreakfastCount);
                                cellTSKF = rowTSKF.CreateCell(25);
                                cellTSKF.SetCellValue(disItm.TotalLunchCount);
                                cellTSKF = rowTSKF.CreateCell(26);
                                cellTSKF.SetCellValue(disItm.TotalTeaCount);
                                cellTSKF = rowTSKF.CreateCell(27);
                                cellTSKF.SetCellValue(disItm.Remark);
                                cellTSKF = rowTSKF.CreateCell(28);
                                cellTSKF.SetCellValue(disItm.Order_2017);
                                cellTSKF = rowTSKF.CreateCell(29);
                                cellTSKF.SetCellValue(disItm.Order_2018);
                                cellTSKF = rowTSKF.CreateCell(30);
                                cellTSKF.SetCellValue(disItm.Order_2019);
                                cellTSKF = rowTSKF.CreateCell(31);
                                cellTSKF.SetCellValue(disItm.Order_201719);
                                cellTSKF = rowTSKF.CreateCell(32);
                                cellTSKF.SetCellValue(disItm.Order_2020);
                                cellTSKF = rowTSKF.CreateCell(33);
                                cellTSKF.SetCellValue(disItm.Order_202001);
                                cellTSKF = rowTSKF.CreateCell(34);
                                cellTSKF.SetCellValue(disItm.Order_202002);
                                cellTSKF = rowTSKF.CreateCell(35);
                                cellTSKF.SetCellValue(disItm.Order_202003);
                                cellTSKF = rowTSKF.CreateCell(36);
                                cellTSKF.SetCellValue(disItm.Order_202004);
                                cellTSKF = rowTSKF.CreateCell(37);
                                cellTSKF.SetCellValue(disItm.Order_202005);
                                cellTSKF = rowTSKF.CreateCell(38);
                                cellTSKF.SetCellValue(disItm.Order_202006);
                                cellTSKF = rowTSKF.CreateCell(39);
                                cellTSKF.SetCellValue(disItm.Order_202007);
                                cellTSKF = rowTSKF.CreateCell(40);
                                cellTSKF.SetCellValue(disItm.Order_202008);
                                cellTSKF = rowTSKF.CreateCell(41);
                                cellTSKF.SetCellValue(disItm.Order_202009);
                                cellTSKF = rowTSKF.CreateCell(42);
                                cellTSKF.SetCellValue(disItm.Order_202010);
                                cellTSKF = rowTSKF.CreateCell(43);
                                cellTSKF.SetCellValue(disItm.Order_202011);
                                cellTSKF = rowTSKF.CreateCell(44);
                                cellTSKF.SetCellValue(disItm.Order_202012);
                                #endregion

                            }
                            TSKF = TSKF + 1;
                        }
                        //DDT
                        else if (Hlist[i].Type == "DDT")
                        {
                            ISheet sheetDDT;
                            IRow rowDDT;
                            ICell cellDDT;

                            if (DDT == 0)
                            {
                                //创建表头
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

                                sheetDDT = book.CreateSheet("DDT");
                                rowDDT = sheetDDT.CreateRow(0);

                                #region header
                                sheetDDT.SetColumnWidth(0, 20 * 256);
                                sheetDDT.SetColumnWidth(1, 15 * 256);
                                sheetDDT.SetColumnWidth(2, 15 * 256);
                                sheetDDT.SetColumnWidth(3, 40 * 256);
                                sheetDDT.SetColumnWidth(4, 40 * 256);
                                sheetDDT.SetColumnWidth(5, 15 * 256);
                                sheetDDT.SetColumnWidth(6, 10 * 256);
                                sheetDDT.SetColumnWidth(7, 20 * 256);
                                sheetDDT.SetColumnWidth(8, 30 * 256);
                                sheetDDT.SetColumnWidth(9, 15 * 256);
                                sheetDDT.SetColumnWidth(10, 15 * 256);
                                sheetDDT.SetColumnWidth(11, 15 * 256);
                                sheetDDT.SetColumnWidth(12, 15 * 256);
                                sheetDDT.SetColumnWidth(13, 15 * 256);
                                sheetDDT.SetColumnWidth(14, 15 * 256);
                                sheetDDT.SetColumnWidth(15, 15 * 256);
                                sheetDDT.SetColumnWidth(16, 15 * 256);
                                sheetDDT.SetColumnWidth(17, 15 * 256);
                                sheetDDT.SetColumnWidth(18, 15 * 256);
                                sheetDDT.SetColumnWidth(19, 15 * 256);
                                sheetDDT.SetColumnWidth(20, 15 * 256);
                                sheetDDT.SetColumnWidth(21, 15 * 256);
                                sheetDDT.SetColumnWidth(22, 15 * 256);
                                sheetDDT.SetColumnWidth(23, 15 * 256);
                                sheetDDT.SetColumnWidth(24, 15 * 256);
                                sheetDDT.SetColumnWidth(25, 15 * 256);
                                sheetDDT.SetColumnWidth(26, 15 * 256);
                                sheetDDT.SetColumnWidth(27, 25 * 256);
                                sheetDDT.SetColumnWidth(28, 15 * 256);
                                sheetDDT.SetColumnWidth(29, 15 * 256);
                                sheetDDT.SetColumnWidth(30, 15 * 256);
                                sheetDDT.SetColumnWidth(31, 20 * 256);
                                sheetDDT.SetColumnWidth(32, 20 * 256);
                                sheetDDT.SetColumnWidth(33, 20 * 256);
                                sheetDDT.SetColumnWidth(34, 20 * 256);
                                sheetDDT.SetColumnWidth(35, 20 * 256);
                                sheetDDT.SetColumnWidth(36, 20 * 256);
                                sheetDDT.SetColumnWidth(37, 20 * 256);
                                sheetDDT.SetColumnWidth(38, 20 * 256);
                                sheetDDT.SetColumnWidth(39, 20 * 256);
                                sheetDDT.SetColumnWidth(40, 20 * 256);
                                sheetDDT.SetColumnWidth(41, 20 * 256);
                                sheetDDT.SetColumnWidth(42, 20 * 256);
                                sheetDDT.SetColumnWidth(43, 20 * 256);
                                sheetDDT.SetColumnWidth(44, 20 * 256);

                                cellDDT = rowDDT.CreateCell(0);
                                cellDDT.SetCellValue("医院代码");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(1);
                                cellDDT.SetCellValue("省份");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(2);
                                cellDDT.SetCellValue("城市");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(3);
                                cellDDT.SetCellValue("医院名称");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(4);
                                cellDDT.SetCellValue("医院地址");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(5);
                                cellDDT.SetCellValue("是否为主地址");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(6);
                                cellDDT.SetCellValue("Market");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(7);
                                cellDDT.SetCellValue("原医院代码");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(8);
                                cellDDT.SetCellValue("原医院名称");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(9);
                                cellDDT.SetCellValue("经度");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(10);
                                cellDDT.SetCellValue("纬度");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(11);
                                cellDDT.SetCellValue("区县");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(12);
                                cellDDT.SetCellValue("区县编码");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(13);
                                cellDDT.SetCellValue("客户默认性质");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(14);
                                cellDDT.SetCellValue("Region");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(15);
                                cellDDT.SetCellValue("Total品牌数");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(16);
                                cellDDT.SetCellValue("XMS品牌数");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(17);
                                cellDDT.SetCellValue("BDS品牌数");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(18);
                                cellDDT.SetCellValue("XMS早餐数");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(19);
                                cellDDT.SetCellValue("XMS正餐数");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(20);
                                cellDDT.SetCellValue("XMS下午茶数");
                                cellDDT.CellStyle = headerStyle;                                
                                cellDDT = rowDDT.CreateCell(21);
                                cellDDT.SetCellValue("BDS早餐数");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(22);
                                cellDDT.SetCellValue("BDS正餐数");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(23);
                                cellDDT.SetCellValue("BDS下午茶数");
                                cellDDT.CellStyle = headerStyle;                               
                                cellDDT = rowDDT.CreateCell(24);
                                cellDDT.SetCellValue("Total早餐品牌数");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(25);
                                cellDDT.SetCellValue("Total正餐品牌数");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(26);
                                cellDDT.SetCellValue("Total下午茶品牌数");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(27);
                                cellDDT.SetCellValue("说明备注");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(28);
                                cellDDT.SetCellValue("2017年订单数");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(29);
                                cellDDT.SetCellValue("2018年订单数");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(30);
                                cellDDT.SetCellValue("2019年订单数");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(31);
                                cellDDT.SetCellValue("17-19年订单总量");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(32);
                                cellDDT.SetCellValue("2020-YTD订单数");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(33);
                                cellDDT.SetCellValue("2020年1月订单数");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(34);
                                cellDDT.SetCellValue("2020年2月订单数");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(35);
                                cellDDT.SetCellValue("2020年3月订单数");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(36);
                                cellDDT.SetCellValue("2020年4月订单数");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(37);
                                cellDDT.SetCellValue("2020年5月订单数");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(38);
                                cellDDT.SetCellValue("2020年6月订单数");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(39);
                                cellDDT.SetCellValue("2020年7月订单数");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(40);
                                cellDDT.SetCellValue("2020年8月订单数");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(41);
                                cellDDT.SetCellValue("2020年9月订单数");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(42);
                                cellDDT.SetCellValue("2020年10月订单数");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(43);
                                cellDDT.SetCellValue("2020年11月订单数");
                                cellDDT.CellStyle = headerStyle;
                                cellDDT = rowDDT.CreateCell(44);
                                cellDDT.SetCellValue("2020年12月订单数");
                                cellDDT.CellStyle = headerStyle;
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

                                //绑符合条件的第一行数据
                                P_HOSPITAL_DATA_VIEW disItm;
                                disItm = Hlist[i];
                                rowDDT = sheetDDT.CreateRow(1 + DDT);

                                #region data cell
                                cellDDT = rowDDT.CreateCell(0);
                                cellDDT.SetCellValue(disItm.GskHospital);// "医院ID");
                                cellDDT = rowDDT.CreateCell(1);
                                cellDDT.SetCellValue(disItm.ProvinceName);//"省");
                                cellDDT = rowDDT.CreateCell(2);
                                cellDDT.SetCellValue(disItm.CityName);// "市");
                                cellDDT = rowDDT.CreateCell(3);
                                cellDDT.SetCellValue(disItm.Name);// "医院名称");
                                cellDDT = rowDDT.CreateCell(4);
                                cellDDT.SetCellValue(disItm.Address);// "医院地址");
                                cellDDT = rowDDT.CreateCell(5);
                                cellDDT.SetCellValue(disItm.MainAddress);// "是否为主地址");
                                cellDDT = rowDDT.CreateCell(6);
                                cellDDT.SetCellValue(disItm.Type);// "Market");
                                cellDDT = rowDDT.CreateCell(7);
                                cellDDT.SetCellValue(disItm.OldGskHospital);// "原医院代码");
                                cellDDT = rowDDT.CreateCell(8);
                                cellDDT.SetCellValue(disItm.OldName);// "原医院名称");
                                cellDDT = rowDDT.CreateCell(9);
                                cellDDT.SetCellValue(disItm.Longitude);
                                cellDDT = rowDDT.CreateCell(10);
                                cellDDT.SetCellValue(disItm.Latitude);
                                cellDDT = rowDDT.CreateCell(11);
                                cellDDT.SetCellValue(disItm.District);
                                cellDDT = rowDDT.CreateCell(12);
                                cellDDT.SetCellValue(disItm.DistrictCode);
                                cellDDT = rowDDT.CreateCell(13);
                                cellDDT.SetCellValue(disItm.CustomerType);
                                cellDDT = rowDDT.CreateCell(14);
                                cellDDT.SetCellValue(disItm.Region);
                                cellDDT = rowDDT.CreateCell(15);
                                cellDDT.SetCellValue(disItm.TotalCount);
                                cellDDT = rowDDT.CreateCell(16);
                                cellDDT.SetCellValue(disItm.XMSTotalCount);
                                cellDDT = rowDDT.CreateCell(17);
                                cellDDT.SetCellValue(disItm.BDSTotalCount);
                                cellDDT = rowDDT.CreateCell(18);
                                cellDDT.SetCellValue(disItm.XMSBreakfastCount);
                                cellDDT = rowDDT.CreateCell(19);
                                cellDDT.SetCellValue(disItm.XMSLunchCount);
                                cellDDT = rowDDT.CreateCell(20);
                                cellDDT.SetCellValue(disItm.XMSTeaCount);                                
                                cellDDT = rowDDT.CreateCell(21);
                                cellDDT.SetCellValue(disItm.BDSBreakfastCount);
                                cellDDT = rowDDT.CreateCell(22);
                                cellDDT.SetCellValue(disItm.BDSLunchCount);
                                cellDDT = rowDDT.CreateCell(23);
                                cellDDT.SetCellValue(disItm.BDSTeaCount);                               
                                cellDDT = rowDDT.CreateCell(24);
                                cellDDT.SetCellValue(disItm.TotalBreakfastCount);
                                cellDDT = rowDDT.CreateCell(25);
                                cellDDT.SetCellValue(disItm.TotalLunchCount);
                                cellDDT = rowDDT.CreateCell(26);
                                cellDDT.SetCellValue(disItm.TotalTeaCount);
                                cellDDT = rowDDT.CreateCell(27);
                                cellDDT.SetCellValue(disItm.Remark);
                                cellDDT = rowDDT.CreateCell(28);
                                cellDDT.SetCellValue(disItm.Order_2017);
                                cellDDT = rowDDT.CreateCell(29);
                                cellDDT.SetCellValue(disItm.Order_2018);
                                cellDDT = rowDDT.CreateCell(30);
                                cellDDT.SetCellValue(disItm.Order_2019);
                                cellDDT = rowDDT.CreateCell(31);
                                cellDDT.SetCellValue(disItm.Order_201719);
                                cellDDT = rowDDT.CreateCell(32);
                                cellDDT.SetCellValue(disItm.Order_2020);
                                cellDDT = rowDDT.CreateCell(33);
                                cellDDT.SetCellValue(disItm.Order_202001);
                                cellDDT = rowDDT.CreateCell(34);
                                cellDDT.SetCellValue(disItm.Order_202002);
                                cellDDT = rowDDT.CreateCell(35);
                                cellDDT.SetCellValue(disItm.Order_202003);
                                cellDDT = rowDDT.CreateCell(36);
                                cellDDT.SetCellValue(disItm.Order_202004);
                                cellDDT = rowDDT.CreateCell(37);
                                cellDDT.SetCellValue(disItm.Order_202005);
                                cellDDT = rowDDT.CreateCell(38);
                                cellDDT.SetCellValue(disItm.Order_202006);
                                cellDDT = rowDDT.CreateCell(39);
                                cellDDT.SetCellValue(disItm.Order_202007);
                                cellDDT = rowDDT.CreateCell(40);
                                cellDDT.SetCellValue(disItm.Order_202008);
                                cellDDT = rowDDT.CreateCell(41);
                                cellDDT.SetCellValue(disItm.Order_202009);
                                cellDDT = rowDDT.CreateCell(42);
                                cellDDT.SetCellValue(disItm.Order_202010);
                                cellDDT = rowDDT.CreateCell(43);
                                cellDDT.SetCellValue(disItm.Order_202011);
                                cellDDT = rowDDT.CreateCell(44);
                                cellDDT.SetCellValue(disItm.Order_202012);
                                #endregion
                            }
                            else
                            {
                                //绑除去第一行的其他符合条件数据
                                P_HOSPITAL_DATA_VIEW disItm;
                                disItm = Hlist[i];
                                sheetDDT = book.GetSheet("DDT");
                                rowDDT = sheetDDT.CreateRow(1 + DDT);

                                #region data cell
                                cellDDT = rowDDT.CreateCell(0);
                                cellDDT.SetCellValue(disItm.GskHospital);// "医院ID");
                                cellDDT = rowDDT.CreateCell(1);
                                cellDDT.SetCellValue(disItm.ProvinceName);//"省");
                                cellDDT = rowDDT.CreateCell(2);
                                cellDDT.SetCellValue(disItm.CityName);// "市");
                                cellDDT = rowDDT.CreateCell(3);
                                cellDDT.SetCellValue(disItm.Name);// "医院名称");
                                cellDDT = rowDDT.CreateCell(4);
                                cellDDT.SetCellValue(disItm.Address);// "医院地址");
                                cellDDT = rowDDT.CreateCell(5);
                                cellDDT.SetCellValue(disItm.MainAddress);// "是否为主地址");
                                cellDDT = rowDDT.CreateCell(6);
                                cellDDT.SetCellValue(disItm.Type);// "Market");
                                cellDDT = rowDDT.CreateCell(7);
                                cellDDT.SetCellValue(disItm.OldGskHospital);// "原医院代码");
                                cellDDT = rowDDT.CreateCell(8);
                                cellDDT.SetCellValue(disItm.OldName);// "原医院名称");
                                cellDDT = rowDDT.CreateCell(9);
                                cellDDT.SetCellValue(disItm.Longitude);
                                cellDDT = rowDDT.CreateCell(10);
                                cellDDT.SetCellValue(disItm.Latitude);
                                cellDDT = rowDDT.CreateCell(11);
                                cellDDT.SetCellValue(disItm.District);
                                cellDDT = rowDDT.CreateCell(12);
                                cellDDT.SetCellValue(disItm.DistrictCode);
                                cellDDT = rowDDT.CreateCell(13);
                                cellDDT.SetCellValue(disItm.CustomerType);
                                cellDDT = rowDDT.CreateCell(14);
                                cellDDT.SetCellValue(disItm.Region);
                                cellDDT = rowDDT.CreateCell(15);
                                cellDDT.SetCellValue(disItm.TotalCount);
                                cellDDT = rowDDT.CreateCell(16);
                                cellDDT.SetCellValue(disItm.XMSTotalCount);
                                cellDDT = rowDDT.CreateCell(17);
                                cellDDT.SetCellValue(disItm.BDSTotalCount);
                                cellDDT = rowDDT.CreateCell(18);
                                cellDDT.SetCellValue(disItm.XMSBreakfastCount);
                                cellDDT = rowDDT.CreateCell(19);
                                cellDDT.SetCellValue(disItm.XMSLunchCount);
                                cellDDT = rowDDT.CreateCell(20);
                                cellDDT.SetCellValue(disItm.XMSTeaCount);
                                cellDDT = rowDDT.CreateCell(21);
                                cellDDT.SetCellValue(disItm.BDSBreakfastCount);
                                cellDDT = rowDDT.CreateCell(22);
                                cellDDT.SetCellValue(disItm.BDSLunchCount);
                                cellDDT = rowDDT.CreateCell(23);
                                cellDDT.SetCellValue(disItm.BDSTeaCount);
                                cellDDT = rowDDT.CreateCell(24);
                                cellDDT.SetCellValue(disItm.TotalBreakfastCount);
                                cellDDT = rowDDT.CreateCell(25);
                                cellDDT.SetCellValue(disItm.TotalLunchCount);
                                cellDDT = rowDDT.CreateCell(26);
                                cellDDT.SetCellValue(disItm.TotalTeaCount);
                                cellDDT = rowDDT.CreateCell(27);
                                cellDDT.SetCellValue(disItm.Remark);
                                cellDDT = rowDDT.CreateCell(28);
                                cellDDT.SetCellValue(disItm.Order_2017);
                                cellDDT = rowDDT.CreateCell(29);
                                cellDDT.SetCellValue(disItm.Order_2018);
                                cellDDT = rowDDT.CreateCell(30);
                                cellDDT.SetCellValue(disItm.Order_2019);
                                cellDDT = rowDDT.CreateCell(31);
                                cellDDT.SetCellValue(disItm.Order_201719);
                                cellDDT = rowDDT.CreateCell(32);
                                cellDDT.SetCellValue(disItm.Order_2020);
                                cellDDT = rowDDT.CreateCell(33);
                                cellDDT.SetCellValue(disItm.Order_202001);
                                cellDDT = rowDDT.CreateCell(34);
                                cellDDT.SetCellValue(disItm.Order_202002);
                                cellDDT = rowDDT.CreateCell(35);
                                cellDDT.SetCellValue(disItm.Order_202003);
                                cellDDT = rowDDT.CreateCell(36);
                                cellDDT.SetCellValue(disItm.Order_202004);
                                cellDDT = rowDDT.CreateCell(37);
                                cellDDT.SetCellValue(disItm.Order_202005);
                                cellDDT = rowDDT.CreateCell(38);
                                cellDDT.SetCellValue(disItm.Order_202006);
                                cellDDT = rowDDT.CreateCell(39);
                                cellDDT.SetCellValue(disItm.Order_202007);
                                cellDDT = rowDDT.CreateCell(40);
                                cellDDT.SetCellValue(disItm.Order_202008);
                                cellDDT = rowDDT.CreateCell(41);
                                cellDDT.SetCellValue(disItm.Order_202009);
                                cellDDT = rowDDT.CreateCell(42);
                                cellDDT.SetCellValue(disItm.Order_202010);
                                cellDDT = rowDDT.CreateCell(43);
                                cellDDT.SetCellValue(disItm.Order_202011);
                                cellDDT = rowDDT.CreateCell(44);
                                cellDDT.SetCellValue(disItm.Order_202012);
                                #endregion

                            }
                            DDT = DDT + 1;
                        }
                    }
                }
                #endregion

                if (TERRITORY_TAlist != null && TERRITORY_TAlist.Count > 0)
                {
                    string txt = "";
                    string TAcolumn = "";
                    foreach (var s in TERRITORY_TAlist)
                    {
                        TAcolumn += ", ["+s.TERRITORY_TA.Trim()+"] ";
                        txt += ", max(case [TERRITORY_TA] when '" + s.TERRITORY_TA.Trim() + "' then 'Y' else '' end) [" + s.TERRITORY_TA.Trim() + "]";
                    }
                    if (txt != "")
                    {
                        string Condition = "";
                        //选择全部Market
                        if (srh_HospitalMarket == "")
                        {
                            //选择全部Type
                            if (srh_OHHospitalType == "")
                            {
                                Condition = " AND b.GskHospital LIKE '%" + srh_GskHospital + "%' AND b.[Name] LIKE '%" + srh_HospitalName + "%' AND (b.[Type] = 'Rx') OR('Rx' = '') ";
                            }
                            //选择院内
                            else if (srh_OHHospitalType == "院内")
                            {
                                Condition = " AND b.GskHospital LIKE '%" + srh_GskHospital + "%' AND b.[Name] LIKE '%" + srh_HospitalName + "%' AND (b.[Type] = 'Rx') OR('Rx' = '') AND b.[External] = 0 ";
                            }
                            //选择院外
                            else
                            {
                                Condition = " AND b.GskHospital LIKE '%" + srh_GskHospital + "%' AND b.[Name] LIKE '%" + srh_HospitalName + "%' AND (b.[Type] = 'Rx') OR('Rx' = '')  AND b.[External] = 1 ";
                            }
                        }
                        else
                        {
                            //选择全部Type
                            if (srh_OHHospitalType == "")
                            {
                                Condition = " AND b.GskHospital LIKE '%" + srh_GskHospital + "%' AND b.[Name] LIKE '%" + srh_HospitalName + "%' AND (b.[Type] = '" + srh_HospitalMarket + "') OR('" + srh_HospitalMarket + "' = '') ";
                            }
                            //选择院内
                            else if (srh_OHHospitalType == "院内")
                            {
                                Condition = " AND b.GskHospital LIKE '%" + srh_GskHospital + "%' AND b.[Name] LIKE '%" + srh_HospitalName + "%' AND (b.[Type] = '" + srh_HospitalMarket + "') OR('" + srh_HospitalMarket + "' = '') AND b.[External] = 0 ";
                            }
                            //选择院外
                            else
                            {
                                Condition = " AND b.GskHospital LIKE '%" + srh_GskHospital + "%' AND b.[Name] LIKE '%" + srh_HospitalName + "%' AND (b.[Type] = '" + srh_HospitalMarket + "') OR('" + srh_HospitalMarket + "' = '')  AND b.[External] = 1 ";
                            }
                        }
                        //将Rx的TERRITORY_TA组成datatable
                        DataTable dt = new DataTable();
                        dt = LoadTERRITORY_TA(TAcolumn,txt, "Rx", Condition);
                        if (dt.Rows.Count > 0)
                        {
                            ISheet sheet;
                            IRow row;
                            ICell cell;
                            // int TA = 0;

                            //创建表头
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
                            sheet = book.CreateSheet("Rx医院TA对应关系");
                            row = sheet.CreateRow(0);
                            #region header
                            for (int i = 0; i < dt.Columns.Count; i++)
                            {
                                sheet.SetColumnWidth(i, 20 * 256);

                                cell = row.CreateCell(i);                               
                                cell.SetCellValue(dt.Columns[i].ToString().Trim());
                                cell.CellStyle = headerStyle;
                            }
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

                            //绑符合条件的数据
                            //row = sheet.CreateRow(1 + TA);
                            #region data cell
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                row = sheet.CreateRow(1 + i);
                                for (int j = 0; j < dt.Columns.Count; j++)
                                {
                                    cell = row.CreateCell(j);
                                    cell.SetCellValue(dt.Rows[i][j].ToString().Trim());// "医院ID");

                                }
                            }
                            #endregion
                        }
                    }
                }
                byte[] excelData;
                using (System.IO.MemoryStream _ms = new System.IO.MemoryStream())
                {
                    book.Write(_ms);
                    excelData = _ms.ToArray();
                    //_ms.Close();
                }

                ViewBag.Msg = "导出成功！";
                return File(excelData, "application/vnd.ms-excel", HttpUtility.UrlEncode("HospitalDataReport_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".xlsx", System.Text.Encoding.UTF8));
            }
            else
            {
                ViewBag.Msg = "无符合条件数据！";
                return View();
            }
        }

        [OperationAuditFilter(Operation = "导出医院数据", OperationAuditTypeName = "查询医院数据")]
        public ActionResult ExportTerritoryData(string srh_GskHospital, string srh_HospitalName, string srh_MUDID, string srh_TerritoryCode, string srh_HospitalMarket, string srh_HospitalTA)
        {
            var list = hospitalService.LoadTA(srh_GskHospital, srh_HospitalName, srh_MUDID, srh_TerritoryCode, srh_HospitalMarket, srh_HospitalTA);
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
                sheet.SetColumnWidth(0, 20 * 256);
                sheet.SetColumnWidth(1, 15 * 256);
                sheet.SetColumnWidth(2, 15 * 256);
                sheet.SetColumnWidth(3, 35 * 256);
                sheet.SetColumnWidth(4, 40 * 256);
                sheet.SetColumnWidth(5, 20 * 256);
                sheet.SetColumnWidth(6, 35 * 256);
                sheet.SetColumnWidth(7, 45 * 256);
                sheet.SetColumnWidth(8, 45 * 256);
                sheet.SetColumnWidth(9, 45 * 256);
                sheet.SetColumnWidth(10, 45 * 256);
                sheet.SetColumnWidth(11, 45 * 256);
                sheet.SetColumnWidth(12, 45 * 256);
                sheet.SetColumnWidth(13, 45 * 256);
                sheet.SetColumnWidth(14, 45 * 256);
                sheet.SetColumnWidth(15, 45 * 256);
                sheet.SetColumnWidth(16, 45 * 256);
                sheet.SetColumnWidth(17, 45 * 256);
                sheet.SetColumnWidth(18, 45 * 256);
                sheet.SetColumnWidth(19, 45 * 256);
                sheet.SetColumnWidth(20, 45 * 256);
                sheet.SetColumnWidth(21, 45 * 256);

                var cell = row.CreateCell(0);
                cell.SetCellValue("医院代码");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(1);
                cell.SetCellValue("省份");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(2);
                cell.SetCellValue("城市");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(3);
                cell.SetCellValue("医院名称");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(4);
                cell.SetCellValue("医院地址");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(5);                  //增加列"是否为主地址" 20190326 liuxiaomeng
                cell.SetCellValue("是否为主地址");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(6);
                cell.SetCellValue("Market");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(7);
                cell.SetCellValue("经度");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(8);
                cell.SetCellValue("纬度");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(9);
                cell.SetCellValue("区县");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(10);
                cell.SetCellValue("区县编码");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(11);
                cell.SetCellValue("客户默认性质");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(12);
                cell.SetCellValue("MUD_ID_MR");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(13);
                cell.SetCellValue("TerritoryCode_MR");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(14);
                cell.SetCellValue("MUD_ID_DM");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(15);
                cell.SetCellValue("TerritoryCode_DM");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(16);
                cell.SetCellValue("MUD_ID_RM");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(17);
                cell.SetCellValue("TerritoryCode_RM");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(18);
                cell.SetCellValue("MUD_ID_RD");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(19);
                cell.SetCellValue("TerritoryCode_RD");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(20);
                cell.SetCellValue("MUD_ID_TA");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(21);
                cell.SetCellValue("TERRITORY_TA");
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
                P_TERRITORY disItm;
                int dataCnt = list.Count;
                for (int i = 0; i < dataCnt; i++)
                {
                    disItm = list[i];
                    row = sheet.CreateRow(1 + i);
                    #region data cell
                    cell = row.CreateCell(0);
                    cell.SetCellValue(disItm.HospitalCode);// "医院ID");
                    cell = row.CreateCell(1);
                    cell.SetCellValue(disItm.Province);//"省");
                    cell = row.CreateCell(2);
                    cell.SetCellValue(disItm.City);// "市");
                    cell = row.CreateCell(3);
                    cell.SetCellValue(disItm.HospitalName);// "医院名称");
                    cell = row.CreateCell(4);
                    cell.SetCellValue(disItm.Address);// "医院地址");
                    cell = row.CreateCell(5);       //增加列"是否为主地址" 20190326 liuxiaomeng
                    cell.SetCellValue(disItm.MainAddress);// "是否为主地址");
                    cell = row.CreateCell(6);
                    cell.SetCellValue(disItm.Market);// "Market");
                    cell = row.CreateCell(7);
                    cell.SetCellValue(disItm.Longitude);// "原医院代码");
                    cell = row.CreateCell(8);
                    cell.SetCellValue(disItm.Latitude);// "原医院名称");
                    cell = row.CreateCell(9);
                    cell.SetCellValue(disItm.District);// "院外代表");
                    cell = row.CreateCell(10);
                    cell.SetCellValue(disItm.DistrictCode);// "院外代表");
                    cell = row.CreateCell(11);
                    cell.SetCellValue(disItm.CustomerType);// "院外代表");
                    cell = row.CreateCell(12);
                    cell.SetCellValue(disItm.MUD_ID_MR);// "院外代表");
                    cell = row.CreateCell(13);
                    cell.SetCellValue(disItm.TERRITORY_MR);// "院外代表");
                    cell = row.CreateCell(14);
                    cell.SetCellValue(disItm.MUD_ID_DM);// "院外代表");
                    cell = row.CreateCell(15);
                    cell.SetCellValue(disItm.TERRITORY_DM);// "院外代表");
                    cell = row.CreateCell(16);
                    cell.SetCellValue(disItm.MUD_ID_RM);// "院外代表");
                    cell = row.CreateCell(17);
                    cell.SetCellValue(disItm.TERRITORY_RM);// "院外代表");
                    cell = row.CreateCell(18);
                    cell.SetCellValue(disItm.MUD_ID_RD);// "院外代表");
                    cell = row.CreateCell(19);
                    cell.SetCellValue(disItm.TERRITORY_RD);// "院外代表");
                    cell = row.CreateCell(20);
                    cell.SetCellValue(disItm.MUD_ID_TA);// "院外代表");
                    cell = row.CreateCell(21);
                    cell.SetCellValue(disItm.TERRITORY_TA);// "院外代表");
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
                return File(excelData, "application/vnd.ms-excel", HttpUtility.UrlEncode("Territory_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".xlsx", System.Text.Encoding.UTF8));
            }
            else
            {
                ViewBag.Msg = "无符合条件数据！";
                return View();
            }
        }

        readonly static string sql = ConfigurationManager.AppSettings["sqlconnection"];
        public static DataTable LoadTERRITORY_TA(string TAcolumn,string txt, string Market, string Condition)
        {
            //查询语句
            //string commandString = " SELECT a.[HospitalCode]  "
            //                   + txt + "  FROM [Territory_Hospital] a  "
            //                   + " left join V_HospitalDataReport b on b.GskHospital=a.HospitalCode "
            //                   + " where a.MARKET = '" + Market + "' " + Condition + " group by  a.[HospitalCode] order by a.[HospitalCode]  ";

            string commandString = " SELECT HospitalCode as 医院代码, MainAddress as 是否为主地址  "
                               + TAcolumn + "  FROM  (SELECT a.[HospitalCode], b.MainAddress, b.HospitalCode BBB "
                               + txt + " FROM[Territory_Hospital] a  "
                               + " left join V_HospitalDataReport b on b.GskHospital=a.HospitalCode "
                               + " where a.MARKET = '" + Market + "' " + Condition + " group by  a.[HospitalCode], b.MainAddress, b.HospitalCode) AAA "
                               + " ORDER BY HospitalCode, BBB  ";
            //創建SqlDataAdapter對象  并執行sql
            SqlDataAdapter dataAdapter = new SqlDataAdapter(commandString, sql);
            //創建數據集dataSet
            DataSet dataSet = new DataSet();
            //將數據添加到數據集中
            dataAdapter.Fill(dataSet);
            //將數據表添加到數據集中
            DataTable dataTable = dataSet.Tables[0];

            return dataTable;
        }
        //[AdminSessionFilter(Order = 1)]
        //[PermissionFilter(Order = 2, Permission = "00000000-0000-0000-2000-000000000001")]
        public JsonResult SyncHospital()
        {
            Task.Factory.StartNew(() =>
            {
                var rtnVal = baseDataService.SyncBaseData();
            });
            return Json(new { state = 1 });
        }

        //[AdminSessionFilter(Order = 1)]
        //[PermissionFilter(Order = 2, Permission = "00000000-0000-0000-2000-000000000001")]
        public ActionResult HospitalDetail(int HospitalID)
        {
            var h = hospitalService.GetHospitalByID(HospitalID);
            var hadr = hospitalService.LoadHospitalAddr(HospitalID);
            ViewBag.Hospital = h;
            ViewBag.HospitalAddr = hadr;
            return View();
        }

        //[AdminSessionFilter(Order = 1)]
        //[PermissionFilter(Order = 2, Permission = "00000000-0000-0000-2000-000000000001")]
        public JsonResult SaveNewHospitalAddr(int HospitalID, string Addr)
        {
            P_HOSPITAL_ADDR addr = new P_HOSPITAL_ADDR() { ID = Guid.NewGuid(), HospitalId = HospitalID, Address = Addr, CreateDate = DateTime.Now };
            if (hospitalService.AddHospitalAddr(addr) == true)
            {
                return Json(new { state = 1, data = addr });
            }
            else
            {
                return Json(new { state = 0 });
            }
        }

        //[AdminSessionFilter(Order = 1)]
        //[PermissionFilter(Order = 2, Permission = "00000000-0000-0000-2000-000000000001")]
        [OperationAuditFilter(Operation = "删除医院数据", OperationAuditTypeName = "删除医院数据")]
        public JsonResult DelHospitalAddr(Guid AddrID)
        {
            if (hospitalService.DeleteHospitalAddr(AddrID) == true)
            {
                return Json(new { state = 1 });
            }
            else
            {
                return Json(new { state = 0 });
            }
        }

        public ActionResult Index2()
        {

            return View();
        }

        public ActionResult Territory()
        {

            return View();
        }        

        #region 导入
        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [OperationAuditFilter(Operation = "导入医院数据", OperationAuditTypeName = "查询医院数据")]
        public JsonResult Import(HttpPostedFileBase file, int isExternal, string importMarket)
        {
            #region 解析为Excel格式对象
            var workbook = new XSSFWorkbook(file.InputStream);

            var sheet = workbook.GetSheetAt(0);
            if (sheet == null)
            {
                return Json(new { state = 0, txt = "导入的文件格式不正确" }, "text/html", JsonRequestBehavior.AllowGet);
            }
            #endregion

            #region 判断表头是否符合格式
            var row = sheet.GetRow(0);
            if (row == null)
            {
                return Json(new { state = 0, txt = "导入的文件格式不正确" }, "text/html", JsonRequestBehavior.AllowGet);
            }
            var titleTemplate = "医院代码";
            var titleValues = new string[1];

            for (var i = 0; i < 1; i++)
            {
                titleValues[i] = row.GetCell(0) != null ? row.GetCell(i).StringCellValue : string.Empty;
                if (titleValues[i] != titleTemplate)
                {
                    return Json(new { state = 0, txt = "导入的文件格式不正确" }, "text/html", JsonRequestBehavior.AllowGet);
                }
            }
            #endregion

            #region 读取表体
            var excelRows = new List<EXCEL_HOSPITAL>();
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                row = sheet.GetRow(i);
                if (row == null) continue;
                if (string.IsNullOrEmpty(GetStringFromCell(row.GetCell(0)))) break;

                excelRows.Add(new EXCEL_HOSPITAL()
                {
                    HospitalCode = GetStringFromCell(row.GetCell(0)).Trim()
                });

            }
            #endregion


            // 文件中是否有重复数据
            var listRepeat = excelRows.GroupBy(a => a.HospitalCode).Where(a => a.Count() > 1).Select(a => a.Key).ToList();
            if (listRepeat.Count() > 0)
            {
                return Json(new { state = 0, txt = "Excel中发现医院代码重复数据", data = listRepeat }, "text/html", JsonRequestBehavior.AllowGet);
            }
            string Failtxt = "";
            int num = 0;
            var sus = new List<EXCEL_HOSPITAL>();
            //校验导入数据
            foreach (var s in excelRows)
            {
                //校验医院代码
                string GskHospitalOHCode = s.HospitalCode;
                //将医院代码与现有目标医院Code比对
                var OHlist = hospitalService.GetDataByGskHospitalOH(GskHospitalOHCode);
                if (OHlist != null && OHlist.Count > 0)
                {
                    num++;
                    string OHName = "院外-" + OHlist[0].Name;
                    sus.Add(new EXCEL_HOSPITAL()
                    {
                        HospitalCode = s.HospitalCode + "-OH",
                        Province = OHlist[0].ProvinceId.ToString().Trim(),
                        City = OHlist[0].CityId.ToString().Trim(),
                        HospitalName = OHName,
                        HospitalAddress = "院外",
                        Market = OHlist[0].Type,
                        External = 1,
                        XMS = "是",
                        BDS = "否",
                        meituan = "否",
                        MainAddress = "主地址"
                    });
                }
                else
                {
                    //若无匹配的目标医院Code
                    Failtxt += s.HospitalCode + "-OH 医院代码不一致 导入失败" + "</br>";
                    continue;
                }
            }
            if (num > 0)
            {
                var cont = "导入院外";
                var num1 = operationAuditService.AddAudit("6", cont);
            }
            else
            {
                //排除无数据和只有一条错误数据
                if (Failtxt == "")
                {
                    return Json(new { state = 0, txt = "导入院外失败" + "</br>" + "导入文件中无数据" }, "text/html", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { state = 0, txt = "导入院外失败" + "</br>" + Failtxt }, "text/html", JsonRequestBehavior.AllowGet);
                }
            }
            var fails = new List<EXCEL_HOSPITAL>();
            //hospitalService.Import(excelRows, ref fails);
            hospitalService.Import(sus, ref fails);
            if (fails.Count > 0)
            {
                // 导入失败
                return Json(new { state = 0, txt = "数据库中已经存在医院代码相同数据", data = fails.Select(a => a.HospitalCode).ToList() }, "text/html", JsonRequestBehavior.AllowGet);
            }
            //返回报错信息  待修改20190423
            if (Failtxt != "")
            {
                return Json(new { state = 0, txt = "导入院外失败" + "</br>" + Failtxt }, "text/html", JsonRequestBehavior.AllowGet);
                //return Json(new { tate = 1 });
            }

            return Json(new { state = 1 }, "text/html", JsonRequestBehavior.AllowGet);
        }
        #endregion

        private string GetStringFromCell(ICell cell)
        {
            if (cell == null)
            {
                return string.Empty;
            }

            switch (cell.CellType)
            {
                case CellType.Numeric:
                    return cell.NumericCellValue.ToString();
                case CellType.String:
                    return cell.StringCellValue;
                default:
                    return string.Empty;
            }
        }

        #region 删除一条
        /// <summary>
        /// 删除一条
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [OperationAuditFilter(Operation = "删除医院数据", OperationAuditTypeName = "删除医院数据")]
        public JsonResult DelById(string id)
        {
            var res = hospitalService.Del(new string[] { id });
            if (res > 0)
            {
                var cont = "删除医院：" + id;
                var num = operationAuditService.AddAudit("6", cont);
                return Json(new { state = 1 });
            }
            return Json(new { state = 0, txt = "操作失败" });
        }
        #endregion

        #region 删除多条
        /// <summary>
        /// 删除多条
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        [OperationAuditFilter(Operation = "删除多条医院数据", OperationAuditTypeName = "删除多条医院数据")]
        public JsonResult DelByIds(string ids)
        {
            var res = hospitalService.Del(ids.Split(',').Select(a => a).ToArray());
            if (res > 0)
            {
                return Json(new { state = 1 });
            }
            return Json(new { state = 0, txt = "操作失败" });
        }
        #endregion

        #region 删除多条
        /// <summary>
        /// 删除多条
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        [OperationAuditFilter(Operation = "删除多条医院数据", OperationAuditTypeName = "删除多条医院数据")]
        public JsonResult DelHosByIds(string HosIDs)
        {
            var strIds = HosIDs.Trim();
            var aryIds = strIds.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            List<string> unSuccessIds;
            var updCnt = hospitalService.DelHospitals(aryIds, out unSuccessIds);
            if (unSuccessIds.Count == 0)
            {
                return Json(new { state = 1 });
            }
            else
            {
                string _txt;
                if (updCnt == 0)
                {
                    _txt = "批量删除失败，医院ID全未匹配成功！";
                }
                else
                {
                    int i = 1;
                    string res = "";
                    foreach (var item in unSuccessIds)
                    {
                        if (i == unSuccessIds.Count)
                        {
                            res += "\"" + item + "\"";
                        }
                        else
                        {
                            res += "\"" + item + "\",\t";
                        }
                        i++;
                    }
                    _txt = "批量删除部分失败，医院ID " + unSuccessIds.Count + "条未删除成功！" + "[" + res + "]";
                }

                return Json(new { state = 0, txt = _txt });
            }
        }
        #endregion

        public ActionResult Delete()
        {
            return View();
        }
        #region 编辑医院
        /// <summary>
        /// 编辑医院
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]

        public ActionResult Edit(string id)
        {
            ViewBag.id = id;
            return View();
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
            var hospital = hospitalService.FindByCode(id);
            return Json(new { state = 1, data = new { hospital } });
        }
        #endregion

        #region 保存
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="GskHospital"></param>        
        /// <returns></returns>
        public JsonResult Save(
            string ID, string GskHospital)
        {
            #region 手动添加院外数据校验 20190813
            //校验医院代码
            string GskHospitalOHCode = "";
            GskHospitalOHCode = GskHospital;
            //将填写的Code与目标医院Code比对
            var OHcode = hospitalService.GetDataByGskHospitalOH(GskHospitalOHCode);
            //若无匹配的目标医院Code
            if (ID == "" && OHcode.Count == 0)
            {
                return Json(new { state = 0, txt = "添加医院失败" + "</br>" + GskHospital + "-OH 医院代码不一致 添加失败" });
            }
            if (ID != "" && OHcode.Count == 0)
            {
                return Json(new { state = 0, txt = "修改医院失败" + "</br>" + GskHospital + "-OH 医院代码不一致 修改失败" });
            }
            //若有对应的目标医院Code
            else
            {
                var entity = new EXCEL_HOSPITAL
                {
                    ID = string.IsNullOrEmpty(ID) ? 0 : Convert.ToInt32(ID),
                    HospitalCode = GskHospital + "-OH",
                    Province = OHcode[0].ProvinceId.ToString().Trim(),
                    City = OHcode[0].CityId.ToString().Trim(),
                    HospitalName = "院外-" + OHcode[0].Name,
                    HospitalAddress = "院外",
                    Market = OHcode[0].Type,
                    External = 1,
                    XMS = "是",
                    BDS = "否",
                    meituan = "否",
                    MainAddress = "主地址"
                };

                var res = hospitalService.SaveChange(entity);
                DeleteProvince();
                switch (res)
                {
                    case 1:
                        if (ID == "")
                        {
                            var cont = "添加医院：" + GskHospital + "-OH";
                            var num = operationAuditService.AddAudit("6", cont);
                        }
                        else
                        {
                            var cont = "修改医院：" + GskHospital + "-OH";
                            var num = operationAuditService.AddAudit("6", cont);
                        }

                        return Json(new { state = 1 });

                    case 2:
                        return Json(new { state = 0, txt = "已经存在相同的医院代码" });
                }

                return Json(new { state = 0, txt = "操作失败" });
            }

            #endregion
        }
        #endregion

        #region 删除无用省份、城市
        /// <summary>
        /// 判断修改前省份是否存在
        /// </summary>
        /// <param name="provinceNmae"></param>
        public void DeleteProvince()
        {
            hospitalService.DeleteProvince();
        }
        #endregion

        #region 新增地址
        public ActionResult AddressApprove()
        {

            return View();
        }
        #endregion

        #region 变量记录
        public ActionResult HospitalVariables()
        {

            return View();
        }
        #region 医院变量
        [OperationAuditFilter(Operation = "变量记录查询", OperationAuditTypeName = "变量记录查询")]
        public JsonResult LoadHospitalVariables(string srh_GskHospital, string srh_HospitalName, string srh_HospitalMarket, bool srh_Add, bool srh_City, bool srh_UpdateHospitalName, bool srh_LatLong, bool srh_Address, bool srh_Delete, int rows, int page)
        {
            try
            {
                int total;
                List<P_HospitalVariables> p_HospitalVariables = new List<P_HospitalVariables>();

                p_HospitalVariables = hospitalService.LoadHospitalVariables(srh_GskHospital, srh_HospitalName, srh_HospitalMarket, srh_Add, srh_City, srh_UpdateHospitalName, srh_LatLong, srh_Address, srh_Delete, rows, page, out total);
                return Json(new { state = 1, rows = p_HospitalVariables, total = total });
            }
            catch (Exception ex)
            {
                LogHelper.Error("LoadHospitalVariables", ex);
                return Json(new { state = 0, rows = new List<P_HospitalVariables>(), total = 0 });
            }
        }
        #endregion

        #region 大区代码变量
        [OperationAuditFilter(Operation = "大区代码变量记录查询", OperationAuditTypeName = "大区代码变量记录查询")]
        public JsonResult LoadTerritoryRMVariables(string srh_market, bool srh_Add, bool srh_Delete, int rows, int page)
        {
            try
            {
                int total=0;
                List<P_CHECK_REPORT_LINE_RM> cHECK_REPORT_LINE_RMs = new List<P_CHECK_REPORT_LINE_RM>();

                cHECK_REPORT_LINE_RMs = hospitalService.LoadTerritoryRMVariables(srh_market, srh_Add, srh_Delete, rows, page, out total);
                return Json(new { state = 1, rows = cHECK_REPORT_LINE_RMs, total = total });
            }
            catch (Exception ex)
            {
                LogHelper.Error("LoadHospitalVariables", ex);
                return Json(new { state = 0, rows = new List<P_HospitalVariables>(), total = 0 });
            }
        }
        [OperationAuditFilter(Operation = "导出大区代码变量记录", OperationAuditTypeName = "导出大区代码变量记录")]
        public void ExportTerritoryRMVariables(string srh_market, bool srh_Add, bool srh_Delete)
        {
            #region 抓取数据     
            List<P_CHECK_REPORT_LINE_RM> list = new List<P_CHECK_REPORT_LINE_RM>();
            list = hospitalService.ExportTerritoryRMVariables(srh_market, srh_Add, srh_Delete);
            #endregion

            #region 构建Excel
            FileStream file11 = new FileStream(Server.MapPath("~/Template/TerritoryCode_RM Changes.xls"), FileMode.Open, FileAccess.Read);
            HSSFWorkbook wk = new HSSFWorkbook(file11);

            ISheet sheet = wk.GetSheet("Report");

            #endregion

            #region 制作表体
            for (var i = 1; i <= list.Count; i++)
            {
                var item = list[i - 1];
                IRow row = sheet.CreateRow(i);
                ICell cell = null;
                var j = 0;
                if (item != null)
                {
                    cell = row.CreateCell(j);
                    cell.SetCellValue(item.Market);

                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.TERRITORY_TA);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.TerritoryCode_RM);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.Remarks);

                    cell = row.CreateCell(++j);
                    if (!string.IsNullOrEmpty(item.createdate))
                        cell.SetCellValue(item.createdate);
                    else
                        cell.SetCellValue("");
                }

            }
            #endregion

            #region 写入到客户端
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                wk.Write(ms);
                Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xls", "TerritoryRM-Variables_" + DateTime.Now.ToString("yyyyMMddHHmm")));
                Response.ContentType = "application/vnd.ms-excel";
                Response.BinaryWrite(ms.ToArray());
            }
            #endregion
        }
        #endregion

        #region 导出变量记录
        [OperationAuditFilter(Operation = "导出变量记录", OperationAuditTypeName = "导出变量记录")]
        public void ExportAddressApprovalList(string srh_GskHospital, string srh_HospitalName, string srh_HospitalMarket, bool srh_Add, bool srh_City, bool srh_UpdateHospitalName, bool srh_LatLong, bool srh_Address, bool srh_Delete)
        {
            #region 抓取数据     
            List<P_HospitalVariables> list = new List<P_HospitalVariables>();
            list = hospitalService.ExportHospitalVariablesList(srh_GskHospital, srh_HospitalName, srh_HospitalMarket, srh_Add, srh_City, srh_UpdateHospitalName, srh_LatLong, srh_Address, srh_Delete);
            #endregion

            #region 构建Excel
            FileStream file11 = new FileStream(Server.MapPath("~/Template/Template_HospitalVariables.xls"), FileMode.Open, FileAccess.Read);
            HSSFWorkbook wk = new HSSFWorkbook(file11);

            ISheet sheet = wk.GetSheet("Report");

            #endregion

            #region 制作表体
            for (var i = 1; i <= list.Count; i++)
            {
                var item = list[i - 1];
                IRow row = sheet.CreateRow(i);
                ICell cell = null;
                var j = 0;
                if (item != null)
                {
                    cell = row.CreateCell(j);
                    cell.SetCellValue(item.GskHospital);

                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.Province);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.City);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.HospitalName);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.Address);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.IsMainAdd);

                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.Market);

                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.Longitude);

                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.Latitude);

                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.DistrictCode);

                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.District);

                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.ActionDisplay);

                    cell = row.CreateCell(++j);
                    if (item.CreateDate != null)
                        cell.SetCellValue(item.CreateDate.Value);
                    else
                        cell.SetCellValue("");

                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.CreateDate == null ? "" : item.CreateDate.Value.ToString("HH:mm:ss"));

                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.Remarks);


                }

            }
            #endregion

            #region 写入到客户端
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                wk.Write(ms);
                Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xls", "Hospital-Variables_" + DateTime.Now.ToString("yyyyMMddHHmm")));
                Response.ContentType = "application/vnd.ms-excel";
                Response.BinaryWrite(ms.ToArray());
            }
            #endregion
        }
        #endregion
        #endregion

        #region 删除新增地址
        [HttpPost]
        [OperationAuditFilter(Operation = "删除医院数据", OperationAuditTypeName = "删除医院数据")]
        public JsonResult DeleteAddress(int id)
        {
            try
            {
                var hospital = hospitalService.GetHospitalByID(id);

                //增加审计记录
                string content = "删除外送地址：医院编码" + hospital.GskHospital;
                var num = operationAuditService.AddAudit("6", content);
                if (num > 0)
                {
                    //删除医院
                    var res = hospitalService.DeleteAddress(hospital);
                    if (res > 0)
                        return Json(new { state = 1 });
                    else
                        return Json(new { state = 0 });
                }
                return Json(new { state = 0, txt = "操作失败" });
            }
            catch (Exception ex)
            {
                LogHelper.Error("DeleteAddress", ex);
                return Json(new { state = 0, txt = "操作失败" });
            }
        }
        #endregion

        #region 医院数据summary
        public ActionResult HospitalSummary()
        {
            return View();
        }

        public JsonResult LoadHospitalVariablesCount(int rows, int page)
        {
            try
            {
                int total;
                List<P_Hospital_Variables_Count> p_Hospital_Variables_Count = new List<P_Hospital_Variables_Count>();

                p_Hospital_Variables_Count = hospitalService.LoadHospitalVariablesCount(rows, page, out total);
                return Json(new { state = 1, rows = p_Hospital_Variables_Count, total = total });
            }
            catch (Exception ex)
            {
                LogHelper.Error("LoadHospitalVariables", ex);
                return Json(new { state = 0, rows = new List<P_HospitalVariables>(), total = 0 });
            }
        }

        public JsonResult LoadHospitalBrandCoverageCount()
        {
            try
            {
                #region By Market
                //当前医院数
                List<P_HOSPITAL> p_HOSPITALs = new List<P_HOSPITAL>();
                p_HOSPITALs = hospitalService.LoadHospital();

                List<P_Hospital_Return> p_Hospital_Returns = new List<P_Hospital_Return>
                {
                    new P_Hospital_Return
                    {
                        Type = "目标医院数",
                        Rx =p_HOSPITALs.Where(p => p.Type == "Rx" && p.IsDelete==0 && p.MainAddress == "主地址" && p.Address != "院外").ToList().Count(),
                        Vx =p_HOSPITALs.Where(p => p.Type == "Vx" && p.IsDelete==0 && p.MainAddress == "主地址" && p.Address != "院外").ToList().Count(),
                        DDT =p_HOSPITALs.Where(p => p.Type == "DDT" && p.IsDelete==0 && p.MainAddress == "主地址" && p.Address != "院外").ToList().Count(),
                        TSKF =p_HOSPITALs.Where(p => p.Type == "TSKF" && p.IsDelete==0 && p.MainAddress == "主地址" && p.Address != "院外").ToList().Count()
                    },
                    new P_Hospital_Return
                    {
                        Type = "外送地址数",
                        Rx =p_HOSPITALs.Where(p => p.Type == "Rx" && p.IsDelete==0 && p.MainAddress != "主地址").ToList().Count(),
                        Vx =p_HOSPITALs.Where(p => p.Type == "Vx" && p.IsDelete==0 && p.MainAddress != "主地址").ToList().Count(),
                        DDT =p_HOSPITALs.Where(p => p.Type == "DDT" && p.IsDelete==0 && p.MainAddress != "主地址").ToList().Count(),
                        TSKF =p_HOSPITALs.Where(p => p.Type == "TSKF" && p.IsDelete==0 && p.MainAddress != "主地址").ToList().Count()
                    },
                    new P_Hospital_Return
                    {
                        Type = "目标地址数",
                        Rx =p_HOSPITALs.Where(p => p.Type == "Rx" && p.IsDelete==0 && p.Address != "院外").ToList().Count(),
                        Vx =p_HOSPITALs.Where(p => p.Type == "Vx" && p.IsDelete==0 && p.Address != "院外").ToList().Count(),
                        DDT =p_HOSPITALs.Where(p => p.Type == "DDT" && p.IsDelete==0 && p.Address != "院外").ToList().Count(),
                        TSKF =p_HOSPITALs.Where(p => p.Type == "TSKF" && p.IsDelete==0 && p.Address != "院外").ToList().Count()
                    },
                    new P_Hospital_Return
                    {
                        Type = "院外医院数",
                        Rx =p_HOSPITALs.Where(p => p.Type == "Rx" && p.IsDelete==0 && p.Address == "院外").ToList().Count(),
                        Vx =p_HOSPITALs.Where(p => p.Type == "Vx" && p.IsDelete==0&& p.Address == "院外").ToList().Count(),
                        DDT =p_HOSPITALs.Where(p => p.Type == "DDT" && p.IsDelete==0 && p.Address == "院外").ToList().Count(),
                        TSKF =p_HOSPITALs.Where(p => p.Type == "TSKF" && p.IsDelete==0 && p.Address == "院外").ToList().Count()
                    }
                };

                //品牌覆盖数
                List<P_Brand_Coverage_Count> p_Brand_Coverage_Count = new List<P_Brand_Coverage_Count>();
                p_Brand_Coverage_Count = hospitalService.LoadBrandCoverageCount();

                int RxCovered = p_Brand_Coverage_Count.Where(a => a.Type == "Rx" && a.TotalCount > 0).ToList().Count();
                int RxUncovered = p_Brand_Coverage_Count.Where(a => a.Type == "Rx").ToList().Count() - RxCovered;
                decimal rx = Math.Round(((decimal)RxCovered / (RxCovered + RxUncovered)), 4) * 100;

                int VxCovered = p_Brand_Coverage_Count.Where(a => a.Type == "Vx" && a.TotalCount > 0).ToList().Count();
                int VxUncovered = p_Brand_Coverage_Count.Where(a => a.Type == "Vx").ToList().Count() - VxCovered;
                decimal vx = Math.Round(((decimal)VxCovered / (VxCovered + VxUncovered)), 4) * 100;

                int DDTCovered = p_Brand_Coverage_Count.Where(a => a.Type == "DDT" && a.TotalCount > 0).ToList().Count();
                int DDTUncovered = p_Brand_Coverage_Count.Where(a => a.Type == "DDT").ToList().Count() - DDTCovered;
                decimal ddt = Math.Round(((decimal)DDTCovered / (DDTCovered + DDTUncovered)), 4) * 100;

                int TSKFCovered = p_Brand_Coverage_Count.Where(a => a.Type == "TSKF" && a.TotalCount > 0).ToList().Count();
                int TSKFUncovered = p_Brand_Coverage_Count.Where(a => a.Type == "TSKF").ToList().Count() - TSKFCovered;
                decimal tskf = Math.Round(((decimal)TSKFCovered / (TSKFCovered + TSKFUncovered)), 4) * 100;

                List<P_Brand_Coverage_Return> p_Brand_Coverage_Returns = new List<P_Brand_Coverage_Return> {
                    new P_Brand_Coverage_Return
                    {
                        Type = "Rx",
                        TotalCount = RxCovered,
                        UnCovered = RxUncovered,
                        Coverage = rx
                    },
                    new P_Brand_Coverage_Return
                    {
                        Type = "Vx",
                        TotalCount = VxCovered,
                        UnCovered = VxUncovered,
                        Coverage = vx
                    },
                    new P_Brand_Coverage_Return
                    {
                        Type = "DDT",
                        TotalCount = DDTCovered,
                        UnCovered = DDTUncovered,
                        Coverage = ddt
                    },
                    new P_Brand_Coverage_Return
                    {
                        Type = "TSKF",
                        TotalCount = TSKFCovered,
                        UnCovered = TSKFUncovered,
                        Coverage = tskf
                    }
                };

                //品牌覆盖数-院外
                List<P_Brand_Coverage_Count> p_Brand_Coverage_Count_OH = new List<P_Brand_Coverage_Count>();
                p_Brand_Coverage_Count_OH = hospitalService.LoadBrandCoverageCountOH();

                decimal rxOH;
                int RxCoveredOH = p_Brand_Coverage_Count_OH.Where(a => a.Type == "Rx" && a.TotalCount > 0).ToList().Count();
                int RxUncoveredOH = p_Brand_Coverage_Count_OH.Where(a => a.Type == "Rx").ToList().Count() - RxCoveredOH;
                if ((RxCoveredOH + RxUncoveredOH) == 0)
                    rxOH = 0;
                else
                    rxOH = Math.Round(((decimal)RxCoveredOH / (RxCoveredOH + RxUncoveredOH)), 4) * 100;

                decimal vxOH;
                int VxCoveredOH = p_Brand_Coverage_Count_OH.Where(a => a.Type == "Vx" && a.TotalCount > 0).ToList().Count();
                int VxUncoveredOH = p_Brand_Coverage_Count_OH.Where(a => a.Type == "Vx").ToList().Count() - VxCoveredOH;
                if ((VxCoveredOH + VxUncoveredOH) == 0)
                    vxOH = 0;
                else
                    vxOH = Math.Round(((decimal)VxCoveredOH / (VxCoveredOH + VxUncoveredOH)), 4) * 100;

                decimal ddtOH;
                int DDTCoveredOH = p_Brand_Coverage_Count_OH.Where(a => a.Type == "DDT" && a.TotalCount > 0).ToList().Count();
                int DDTUncoveredOH = p_Brand_Coverage_Count_OH.Where(a => a.Type == "DDT").ToList().Count() - DDTCoveredOH;
                if ((DDTCoveredOH + DDTUncoveredOH) == 0)
                    ddtOH = 0;
                else
                    ddtOH = Math.Round(((decimal)DDTCoveredOH / (DDTCoveredOH + DDTUncoveredOH)), 4) * 100;

                decimal tskfOH;
                int TSKFCoveredOH = p_Brand_Coverage_Count_OH.Where(a => a.Type == "TSKF" && a.TotalCount > 0).ToList().Count();
                int TSKFUncoveredOH = p_Brand_Coverage_Count_OH.Where(a => a.Type == "TSKF").ToList().Count() - TSKFCoveredOH;
                if ((TSKFCoveredOH + TSKFUncoveredOH) == 0)
                    tskfOH = 0;
                else
                    tskfOH = Math.Round(((decimal)TSKFCoveredOH / (TSKFCoveredOH + TSKFUncoveredOH)), 4) * 100;

                List<P_Brand_Coverage_Return> p_Brand_Coverage_Returns_OH = new List<P_Brand_Coverage_Return> {
                    new P_Brand_Coverage_Return
                    {
                        Type = "Rx",
                        TotalCount = RxCoveredOH,
                        UnCovered = RxUncoveredOH,
                        Coverage = rxOH
                    },
                    new P_Brand_Coverage_Return
                    {
                        Type = "Vx",
                        TotalCount = VxCoveredOH,
                        UnCovered = VxUncoveredOH,
                        Coverage = vxOH
                    },
                    new P_Brand_Coverage_Return
                    {
                        Type = "DDT",
                        TotalCount = DDTCoveredOH,
                        UnCovered = DDTUncoveredOH,
                        Coverage = ddtOH
                    },
                    new P_Brand_Coverage_Return
                    {
                        Type = "TSKF",
                        TotalCount = TSKFCoveredOH,
                        UnCovered = TSKFUncoveredOH,
                        Coverage = tskfOH
                    }
                };
                #endregion

                #region By TA
                //当前医院数
                List<P_TERRITORY_TA> p_TAs = new List<P_TERRITORY_TA>();
                p_TAs = hospitalService.LoadTerritoryTA(); //当前TA数
                List<P_TA_HOSPITAL> p_TA_HOSPITALs = new List<P_TA_HOSPITAL>();
                p_TA_HOSPITALs = hospitalService.LoadTAHospital(); //医院数by ta
                List<P_TA_HOSPITAL> p_TA_OH = hospitalService.LoadTAHospitalOH(); //院外数by ta

                List<P_Hospital_Return_BY_TA> p_Hospital_Return_BY_TAs = new List<P_Hospital_Return_BY_TA>();
                p_Hospital_Return_BY_TAs.Add(new P_Hospital_Return_BY_TA
                {
                    TA_HEAD = "数据",
                    HospitalCount = "目标医院数",
                    AddressCount = "外送地址数",
                    AllCount = "目标地址数",
                    OHCount = "院外医院数"
                });
                foreach (P_TERRITORY_TA ta in p_TAs)
                {
                    P_Hospital_Return_BY_TA p_hospital_return_by_ta = new P_Hospital_Return_BY_TA
                    {
                        TA_HEAD = ta.TERRITORY_TA.Trim(),
                        HospitalCount = p_TA_HOSPITALs.Where(p => p.TERRITORY_TA.Trim() == ta.TERRITORY_TA.Trim() && p.MainAddress == "主地址" && p.Address != "院外").ToList().Count().ToString(),
                        AddressCount = p_TA_HOSPITALs.Where(p => p.TERRITORY_TA.Trim() == ta.TERRITORY_TA.Trim() && p.MainAddress != "主地址").ToList().Count().ToString(),
                        AllCount = p_TA_HOSPITALs.Where(p => p.TERRITORY_TA.Trim() == ta.TERRITORY_TA.Trim() && p.Address != "院外").ToList().Count().ToString(),
                        OHCount = p_TA_OH.Where(p => p.TERRITORY_TA.Trim() == ta.TERRITORY_TA.Trim() && p.Address == "院外").ToList().Count().ToString()
                    };
                    p_Hospital_Return_BY_TAs.Add(p_hospital_return_by_ta);
                }

                //品牌覆盖数
                List<P_Brand_Coverage_Count_TA> p_Brand_Coverage_Count_TAs = new List<P_Brand_Coverage_Count_TA>();
                p_Brand_Coverage_Count_TAs = hospitalService.LoadBrandCoverageCountTA();

                List<P_Brand_Coverage_Return_TA> p_Brand_Coverage_Return_TAs = new List<P_Brand_Coverage_Return_TA>();
                p_Brand_Coverage_Return_TAs.Add(new P_Brand_Coverage_Return_TA
                {
                    TA_HEAD = "数据",
                    CoveredCount = "一品牌上线数",
                    UnCoveredCount = "未覆盖医院数",
                    Coverage = "一品牌覆盖率"
                });
                foreach (P_TERRITORY_TA ta in p_TAs)
                {
                    int total = p_Brand_Coverage_Count_TAs.Where(a => a.TERRITORY_TA.Trim() == ta.TERRITORY_TA.Trim()).ToList().Count();
                    int cover = p_Brand_Coverage_Count_TAs.Where(a => a.TERRITORY_TA.Trim() == ta.TERRITORY_TA.Trim() && a.TotalCount > 0).ToList().Count();
                    decimal coverage = 0;
                    if (total != 0)
                        coverage = Math.Round(((decimal)cover / total), 4) * 100;
                    P_Brand_Coverage_Return_TA p_Brand_Coverage_Return_ta = new P_Brand_Coverage_Return_TA
                    {
                        TA_HEAD = ta.TERRITORY_TA.Trim(),
                        CoveredCount = cover.ToString(),
                        UnCoveredCount = (total - cover).ToString(),
                        Coverage = coverage.ToString("G0")
                    };
                    p_Brand_Coverage_Return_TAs.Add(p_Brand_Coverage_Return_ta);
                }
                #endregion

                return Json(new { state = 1, hospitalData = p_Hospital_Returns, brandData = p_Brand_Coverage_Returns, hospitalDataTA = p_Hospital_Return_BY_TAs, brandDataTA = p_Brand_Coverage_Return_TAs, brandDataOH = p_Brand_Coverage_Returns_OH });
            }
            catch (Exception ex)
            {
                LogHelper.Error("LoadBrandCoverageCount", ex);
                return Json(new { state = 0, hospitalData = new List<P_Hospital_Return>(), brandData = new List<P_Brand_Coverage_Return>() });
            }
        }

        public JsonResult LoadHospitalVariablesCountTA(int rows, int page)
        {

            try
            {
                string connectionString = System.Web.Configuration.WebConfigurationManager.AppSettings["sqlconnection"];
                List<P_TERRITORY_TA> p_TAs = new List<P_TERRITORY_TA>();
                p_TAs = hospitalService.LoadAllTerritoryTA(); //当前TA数
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < p_TAs.Count; i++)
                {
                    sb.Append("[");
                    sb.Append(p_TAs[i].TERRITORY_TA.Trim());
                    if (i == p_TAs.Count - 1)
                        sb.Append("]");
                    else
                        sb.Append("], ");
                }
                string commandString = "select * from (select Date, hospitalCount, ta from[P_Hospital_Variables_Count_TA]) test PIVOT(sum(hospitalCount) for ta in ( " + sb.ToString() + " ) ) pvt order by Date Desc";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(commandString, connectionString);
                //創建數據集dataSet
                DataSet dataSet = new DataSet();
                //將數據添加到數據集中
                dataAdapter.Fill(dataSet);
                //將數據表添加到數據集中
                DataTable dt = dataSet.Tables[0];

                List<object> list = new List<object>();
                foreach (DataRow dr in dt.Rows)
                {
                    string p = "Date=" + Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd HH:mm:ss") + ",";
                    int sum = 0;
                    for (int m = 0; m < p_TAs.Count; m++)
                    {
                        if (dr[p_TAs[m].TERRITORY_TA.Trim()].ToString() == "")
                            p += p_TAs[m].TERRITORY_TA.Trim() + "=0,";
                        else
                            p += p_TAs[m].TERRITORY_TA.Trim() + "=" + dr[p_TAs[m].TERRITORY_TA.Trim()].ToString() + ",";
                        //bool res = int.TryParse(dr[p_TAs[m].TERRITORY_TA.Trim()].ToString(), out count);

                        sum += int.Parse(dr[p_TAs[m].TERRITORY_TA.Trim()].ToString() == "" ? "0" : dr[p_TAs[m].TERRITORY_TA.Trim()].ToString());
                    }
                    p += "ALL=" + sum.ToString() + ",";
                    var o = new { p };
                    list.Add(o);
                }

                return Json(new { state = 99, rows = list, total = list.Count });
            }
            catch (Exception ex)
            {
                LogHelper.Error("LoadHospitalVariablesCountTA", ex);
                return Json(new { state = 0, rows = new List<P_HospitalVariables>(), total = 0 });
            }
        }

        public JsonResult LoadTAHospitalVariablesHeader()
        {

            try
            {
                List<P_TERRITORY_TA> p_TAs = new List<P_TERRITORY_TA>();
                p_TAs = hospitalService.LoadAllTerritoryTA(); //当前TA数
                return Json(new { state = 1, rows = p_TAs, total = 0 });
            }
            catch (Exception ex)
            {
                LogHelper.Error("LoadHospitalVariables", ex);
                return Json(new { state = 0, rows = new List<P_HospitalVariables>(), total = 0 });
            }
        }
        #endregion
    }
}