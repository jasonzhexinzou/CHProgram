using MealAdmin.Entity;
using MealAdmin.Service;
using MealAdmin.Web.Filter;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XFramework.XInject.Attributes;

namespace MealAdmin.Web.Areas.P.Controllers
{
    /// <summary>
    /// 订单报表
    /// </summary>
    public class ReportController : Controller
    {
        [Bean("reportService")]
        public IReportService reportService { get; set; }
        [Bean("preApprovalService")]
        public IPreApprovalService PreApprovalService { get; set; }

        #region HT报表
        /// <summary>
        /// HT报表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Cater()
        {
            return View();
        }
        #endregion

        #region 1.0HT报表
        /// <summary>
        /// 1.0HT报表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult OldCater()
        {
            return View();
        }
        #endregion

        #region NonHT报表
        /// <summary>
        /// NonHT报表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CaterForNonHT()
        {
            return View();
        }
        #endregion

        #region 加载HT报表
        /// <summary>
        /// 加载HT报表
        /// </summary>
        /// <param name="CN"></param>
        /// <param name="MUID"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="channel"></param>
        /// <param name="isOrderSuccess"></param>
        /// <param name="isReceived"></param>
        /// <param name="isReturn"></param>
        /// <param name="isReturnSuccess"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [HttpPost]
        [OperationAuditFilter(Operation = "HT订单报表查询", OperationAuditTypeName = "HT订单报表查询")]
        public JsonResult LoadCater(string CN, string MUID, string TACode, string HospitalCode, string RestaurantId, string CostCenter,
            string startDate, string endDate,
            string channel, string isOrderSuccess, string isReceived, string isReturn, string isReturnSuccess, string Supplier, string IsSpecialOrder, string OrderState,
            int page, int rows)
        {
            int total = 0;
            string oldHospitalCode = PreApprovalService.GetOldGskHospitalCodeByGskHospitalCode(HospitalCode);
            if (!string.IsNullOrEmpty(oldHospitalCode))
            {
                HospitalCode += "," + oldHospitalCode;
            }
            string oldCostCenter = PreApprovalService.GetOldCostcenterByCostcenter(CostCenter);
            if (!string.IsNullOrEmpty(oldCostCenter))
            {
                CostCenter += "," + oldCostCenter;
            }
            var list = reportService.LoadReport(
                CN, MUID, TACode, HospitalCode, RestaurantId, CostCenter, startDate, endDate, channel,
                isOrderSuccess, isReceived, isReturn, isReturnSuccess, Supplier, IsSpecialOrder, OrderState,
                page, rows, out total)
                .Select(a => new
                {
                    c0 = FormatterNull(a.c0),
                    c1 = FormatterNull(a.c1),
                    c144 = FormatterNull(a.c144),
                    c132 = FormatterNull(a.c132),
                    c2 = FormatterNull(a.c2),
                    c3 = FormatterNull(a.c3),
                    c4 = FormatterNull(a.c4),
                    c5 = FormatterNull(a.c5),
                    c6 = FormatterNull(a.c6),
                    c7 = FormatterNull(a.c7),
                    c8 = FormatterNull(a.c8),
                    c143 = FormatterNull(a.c143),
                    c9 = FormatterNull(a.c9),
                    c10 = FormatterNull(a.c10),
                    c11 = FormatterNull(a.c11),
                    c12 = FormatterNull(a.c12),
                    c13 = FormatterNull(a.c13),
                    c14 = FormatterNull(a.c14),
                    c15 = FormatterNull(a.c15),
                    c16 = FormatterNull(a.c16),
                    c17 = FormatterNull(a.c17),
                    c18 = FormatterNull(a.c18),
                    c19 = FormatterNull(a.c19),
                    c20 = FormatterNull(a.c20),
                    c21 = FormatterNull(a.c21),
                    c22 = FormatterNull(a.c22),
                    c145 = FormatterNull(a.c145),
                    c23 = FormatterNull(a.c23),
                    c24 = FormatterNull(a.c24),
                    c25 = FormatterNull(a.c25),
                    c26 = FormatterNull(a.c26),
                    c27 = FormatterNull(a.c27),
                    c28 = FormatterNull(a.c28),
                    c29 = FormatterNull(a.c29),
                    c30 = FormatterNull(a.c30),
                    c31 = FormatterNull(a.c31),
                    c32 = FormatterNull(a.c32),
                    c33 = FormatterNull(a.c33),
                    c34 = FormatterNull(a.c34),
                    c35 = FormatterNull(a.c35),
                    c36 = FormatterNull(a.c36),
                    c37 = FormatterNull(a.c37),
                    c38 = FormatterNull(a.c38),
                    c39 = FormatterNull(a.c39),
                    c40 = FormatterNull(a.c40),
                    c41 = FormatterNull(a.c41),
                    c42 = FormatterNull(a.c42),
                    c43 = FormatterNull(a.c43),
                    c44 = FormatterNull(a.c44),
                    c45 = FormatterNull(a.c45),
                    c46 = FormatterNull(a.c46),
                    c47 = FormatterNull(a.c47),
                    c48 = FormatterNull(a.c48),
                    c49 = FormatterNull(a.c49),
                    c50 = FormatterNull(a.c50),
                    c51 = FormatterNull(a.c51),
                    c52 = FormatterNull(a.c52),
                    c53 = FormatterNull(a.c53),
                    c54 = FormatterNull(a.c54),
                    c55 = FormatterNull(a.c55),
                    c56 = FormatterNull(a.c56),
                    c57 = FormatterNull(a.c57),
                    c58 = FormatterNull(a.c58),
                    c59 = FormatterNull(a.c59),
                    c60 = FormatterNull(a.c60),
                    c61 = FormatterNull(a.c61),
                    c62 = FormatterNull(a.c62),
                    c63 = FormatterNull(a.c63),
                    c64 = FormatterNull(a.c64),
                    c65 = FormatterNull(a.c65),
                    c66 = FormatterNull(a.c66),
                    c67 = FormatterNull(a.c67),
                    c68 = FormatterNull(a.c68),
                    c69 = FormatterNull(a.c69),
                    c70 = FormatterNull(a.c70),
                    c71 = FormatterNull(a.c71),
                    c72 = FormatterNull(a.c72),
                    c73 = FormatterNull(a.c73),
                    c74 = FormatterNull(a.c74),
                    c75 = FormatterNull(a.c75),
                    c76 = FormatterNull(a.c76),
                    c77 = FormatterNull(a.c77),
                    c78 = FormatterNull(a.c78),
                    c79 = FormatterNull(a.c79),
                    c80 = FormatterNull(a.c80),
                    c81 = FormatterNull(a.c81),
                    c82 = FormatterNull(a.c82),
                    c83 = FormatterNull(a.c83),
                    c84 = FormatterNull(a.c84),
                    c85 = FormatterNull(a.c85),
                    c86 = FormatterNull(a.c86),
                    c87 = FormatterNull(a.c87),
                    c88 = FormatterNull(a.c88),
                    c89 = FormatterNull(a.c89),
                    c90 = FormatterNull(a.c90),
                    c91 = FormatterNull(a.c91),
                    c92 = FormatterNull(a.c92),
                    c93 = FormatterNull(a.c93),
                    c94 = FormatterNull(a.c94),
                    c95 = FormatterNull(a.c95),
                    c96 = FormatterNull(a.c96),
                    c97 = FormatterNull(a.c97),
                    c98 = FormatterNull(a.c98),
                    c99 = FormatterNull(a.c99),
                    c100 = FormatterNull(a.c100),
                    c101 = FormatterNull(a.c101),
                    c102 = FormatterNull(a.c102),
                    c103 = FormatterNull(a.c103),
                    c104 = FormatterNull(a.c104),
                    c105 = FormatterNull(a.c105),
                    c106 = FormatterNull(a.c106),
                    c107 = FormatterNull(a.c107),
                    c108 = FormatterNull(a.c108),
                    c109 = FormatterNull(a.c109),
                    c110 = FormatterNull(a.c110),
                    c111 = FormatterNull(a.c111),
                    c112 = FormatterNull(a.c112),
                    c113 = FormatterNull(a.c113),
                    c114 = FormatterNull(a.c114),
                    c115 = FormatterNull(a.c115),
                    c116 = FormatterNull(a.c116),
                    c117 = FormatterNull(a.c117),
                    c118 = FormatterNull(a.c118),
                    c119 = FormatterNull(a.c119),
                    c120 = FormatterNull(a.c120),
                    c121 = FormatterNull(a.c121),
                    c122 = FormatterNull(a.c122),
                    c123 = FormatterNull(a.c123),
                    c124 = FormatterNull(a.c124),
                    c125 = FormatterNull(a.c125),
                    c126 = FormatterNull(a.c126),
                    c127 = FormatterNull(a.c127),
                    c128 = FormatterNull(a.c128),
                    c129 = FormatterNull(a.c129),
                    c130 = FormatterNull(a.c130),
                    c131 = FormatterNull(a.c131),
                    c137 = FormatterNull(a.c137),
                    c138 = FormatterNull(a.c138),
                    c139 = FormatterNull(a.c139),
                    c140 = FormatterNull(a.c140),
                    c141 = FormatterNull(a.c141),
                    c142 = FormatterNull(a.c142),
                    //c144 = FormatterNull(a.c144),
                    //c145 = FormatterNull(a.c145),

                }).ToArray();

            var num = list.Count();

            return Json(new { state = 1, rows = list, total = total });
        }
        #endregion

        #region 加载NonHT报表
        /// <summary>
        /// 加载NonHT报表
        /// </summary>
        /// <param name="CN"></param>
        /// <param name="MUID"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="channel"></param>
        /// <param name="isOrderSuccess"></param>
        /// <param name="isReceived"></param>
        /// <param name="isReturn"></param>
        /// <param name="isReturnSuccess"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LoadCaterForNonHT(string CN, string MUID, string HospitalCode, string RestaurantId,
            string startDate, string endDate,
            string channel, string isOrderSuccess, string isReceived, string isReturn, string isReturnSuccess, string Supplier,
            int page, int rows)
        {
            int total = 0;
            string oldHospitalCode = PreApprovalService.GetOldGskHospitalCodeByGskHospitalCode(HospitalCode);
            if (!string.IsNullOrEmpty(oldHospitalCode))
            {
                HospitalCode += "," + oldHospitalCode;
            }
            var list = reportService.LoadCaterForNonHT(
                CN, MUID, HospitalCode, RestaurantId, startDate, endDate, channel,
                isOrderSuccess, isReceived, isReturn, isReturnSuccess, Supplier,
                page, rows, out total)
                .Select(a => new
                {
                    c0 = FormatterNull(a.c0),
                    c1 = FormatterNull(a.c1),
                    c2 = FormatterNull(a.c2),
                    c3 = FormatterNull(a.c3),
                    c4 = FormatterNull(a.c4),
                    c5 = FormatterNull(a.c5),
                    c6 = FormatterNull(a.c6),
                    c7 = FormatterNull(a.c7),
                    c8 = FormatterNull(a.c8),
                    c9 = FormatterNull(a.c9.ToUpper()),
                    c10 = FormatterNull(a.c10),
                    c11 = FormatterNull(a.c11),
                    c12 = FormatterNull(a.c12),
                    c13 = FormatterNull(a.c13),
                    c14 = FormatterNull(a.c14),
                    c55 = FormatterNull(a.c55),
                    c15 = FormatterNull(a.c15),
                    c16 = FormatterNull(a.c16),
                    c17 = FormatterNull(a.c17),
                    c18 = FormatterNull(a.c18),
                    c19 = FormatterNull(a.c19),
                    c20 = FormatterNull(a.c20),
                    c21 = FormatterNull(a.c21),
                    c22 = FormatterNull(a.c22),
                    c23 = FormatterNull(a.c23),
                    c24 = FormatterNull(a.c24),
                    c25 = FormatterNull(a.c25),
                    c26 = FormatterNull(a.c26),
                    c27 = FormatterNull(a.c27),
                    c28 = FormatterNull(a.c28),
                    c29 = FormatterNull(a.c29),
                    c30 = FormatterNull(a.c30),
                    c31 = FormatterNull(a.c31),
                    c32 = FormatterNull(a.c32),
                    c33 = FormatterNull(a.c33),
                    c34 = FormatterNull(a.c34),
                    c35 = FormatterNull(a.c35),
                    c36 = FormatterNull(a.c36),
                    c37 = FormatterNull(a.c37),
                    c38 = FormatterNull(a.c38),
                    c39 = FormatterNull(a.c39),
                    c40 = FormatterNull(a.c40),
                    c41 = FormatterNull(a.c41),
                    c42 = FormatterNull(a.c42),
                    c43 = FormatterNull(a.c43),
                    c44 = FormatterNull(a.c44),
                    c45 = FormatterNull(a.c45),
                    c46 = FormatterNull(a.c46),
                    c47 = FormatterNull(a.c47),
                    c48 = FormatterNull(a.c48),
                    c49 = FormatterNull(a.c49),
                    c50 = FormatterNull(a.c50),
                    c51 = FormatterNull(a.c51),
                    c52 = FormatterNull(a.c52),
                    c56 = FormatterNull(a.c56),
                }).ToArray();
            return Json(new { state = 1, rows = list, total = total });
        }
        #endregion

        #region 导出HT报表
        /// <summary>
        /// 导出HT报表
        /// </summary>
        /// <param name="CN"></param>
        /// <param name="MUID"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="channel"></param>
        /// <param name="isOrderSuccess"></param>
        /// <param name="isReceived"></param>
        /// <param name="isReturn"></param>
        /// <param name="isReturnSuccess"></param>
        public void ExcelCater(string CN, string MUID,string TACode, string HospitalCode, string RestaurantId, string CostCenter,
            string startDate, string endDate,
            string channel, string isOrderSuccess, string isReceived, string isReturn, string isReturnSuccess, string Supplier, string IsSpecialOrder, string OrderState)
        {
            #region 抓取数据
            int total = 0;
            string oldHospitalCode = PreApprovalService.GetOldGskHospitalCodeByGskHospitalCode(HospitalCode);
            if (!string.IsNullOrEmpty(oldHospitalCode))
            {
                HospitalCode += "," + oldHospitalCode;
            }
            string oldCostCenter = PreApprovalService.GetOldCostcenterByCostcenter(CostCenter);
            if (!string.IsNullOrEmpty(oldCostCenter))
            {
                CostCenter += "," + oldCostCenter;
            }
            var list = reportService.LoadReport(
                CN, MUID, TACode, HospitalCode, RestaurantId, CostCenter, startDate, endDate, channel,
                isOrderSuccess, isReceived, isReturn, isReturnSuccess, Supplier, IsSpecialOrder, OrderState,
                1, int.MaxValue, out total)
                .Select(a => new
                {
                    c0 = FormatterNull(a.c0),
                    c1 = FormatterNull(a.c1),
                    c2 = FormatterNull(a.c2),
                    c3 = FormatterNull(a.c3),
                    c4 = FormatterNull(a.c4),
                    c5 = FormatterNull(a.c5),
                    c6 = FormatterNull(a.c6),
                    c7 = FormatterNull(a.c7),
                    c8 = FormatterNull(a.c8),
                    c143 = FormatterNull(a.c143),
                    c9 = FormatterNull(a.c9),
                    c10 = FormatterNull(a.c10),
                    c11 = FormatterNull(a.c11),
                    c12 = FormatterNull(a.c12),
                    c13 = FormatterNull(a.c13),
                    c14 = FormatterNull(a.c14),
                    c15 = FormatterNull(a.c15),
                    c16 = FormatterNull(a.c16),
                    c17 = FormatterNull(a.c17),
                    c18 = FormatterNull(a.c18),
                    c19 = FormatterNull(a.c19),
                    c20 = FormatterNull(a.c20),
                    c21 = FormatterNull(a.c21),
                    c22 = FormatterNull(a.c22),
                    c23 = FormatterNull(a.c23),
                    c24 = FormatterNull(a.c24),
                    c25 = FormatterNull(a.c25),
                    c26 = FormatterNull(a.c26),
                    c27 = FormatterNull(a.c27),
                    c28 = FormatterNull(a.c28),
                    c29 = FormatterNull(a.c29),
                    c30 = FormatterNull(a.c30),
                    c31 = FormatterNull(a.c31),
                    c32 = FormatterNull(a.c32),
                    c33 = FormatterNull(a.c33),
                    c34 = FormatterNull(a.c34),
                    c35 = FormatterNull(a.c35),
                    c36 = FormatterNull(a.c36),
                    c37 = FormatterNull(a.c37),
                    c38 = FormatterNull(a.c38),
                    c39 = FormatterNull(a.c39),
                    c40 = FormatterNull(a.c40),
                    c41 = FormatterNull(a.c41),
                    c42 = FormatterNull(a.c42),
                    c43 = FormatterNull(a.c43),
                    c44 = FormatterNull(a.c44),
                    c45 = FormatterNull(a.c45),
                    c46 = FormatterNull(a.c46),
                    c47 = FormatterNull(a.c47),
                    c48 = FormatterNull(a.c48),
                    c49 = FormatterNull(a.c49),
                    c50 = FormatterNull(a.c50),
                    c51 = FormatterNull(a.c51),
                    c52 = FormatterNull(a.c52),
                    c53 = FormatterNull(a.c53),
                    c54 = FormatterNull(a.c54),
                    c55 = FormatterNull(a.c55),
                    c56 = FormatterNull(a.c56),
                    c57 = FormatterNull(a.c57),
                    c58 = FormatterNull(a.c58),
                    c59 = FormatterNull(a.c59),
                    c60 = FormatterNull(a.c60),
                    c61 = FormatterNull(a.c61),
                    c62 = FormatterNull(a.c62),
                    c63 = FormatterNull(a.c63),
                    c64 = FormatterNull(a.c64),
                    c65 = FormatterNull(a.c65),
                    c66 = FormatterNull(a.c66),
                    c67 = FormatterNull(a.c67),
                    c68 = FormatterNull(a.c68),
                    c69 = FormatterNull(a.c69),
                    c70 = FormatterNull(a.c70),
                    c71 = FormatterNull(a.c71),
                    c72 = FormatterNull(a.c72),
                    c73 = FormatterNull(a.c73),
                    c74 = FormatterNull(a.c74),
                    c75 = FormatterNull(a.c75),
                    c76 = FormatterNull(a.c76),
                    c77 = FormatterNull(a.c77),
                    c78 = FormatterNull(a.c78),
                    c79 = FormatterNull(a.c79),
                    c80 = FormatterNull(a.c80),
                    c81 = FormatterNull(a.c81),
                    c82 = FormatterNull(a.c82),
                    c83 = FormatterNull(a.c83),
                    c84 = FormatterNull(a.c84),
                    c85 = FormatterNull(a.c85),
                    c86 = FormatterNull(a.c86),
                    c87 = FormatterNull(a.c87),
                    c88 = FormatterNull(a.c88),
                    c89 = FormatterNull(a.c89),
                    c90 = FormatterNull(a.c90),
                    c91 = FormatterNull(a.c91),
                    c92 = FormatterNull(a.c92),
                    c93 = FormatterNull(a.c93),
                    c94 = FormatterNull(a.c94),
                    c95 = FormatterNull(a.c95),
                    c96 = FormatterNull(a.c96),
                    c97 = FormatterNull(a.c97),
                    c98 = FormatterNull(a.c98),
                    c99 = FormatterNull(a.c99),
                    c100 = FormatterNull(a.c100),
                    c101 = FormatterNull(a.c101),
                    c102 = FormatterNull(a.c102),
                    c103 = FormatterNull(a.c103),
                    c104 = FormatterNull(a.c104),
                    c105 = FormatterNull(a.c105),
                    c106 = FormatterNull(a.c106),
                    c107 = FormatterNull(a.c107),
                    c108 = FormatterNull(a.c108),
                    c109 = FormatterNull(a.c109),
                    c110 = FormatterNull(a.c110),
                    c111 = FormatterNull(a.c111),
                    c112 = FormatterNull(a.c112),
                    c113 = FormatterNull(a.c113),
                    c114 = FormatterNull(a.c114),
                    c115 = FormatterNull(a.c115),
                    c116 = FormatterNull(a.c116),
                    c117 = FormatterNull(a.c117),
                    c118 = FormatterNull(a.c118),
                    c119 = FormatterNull(a.c119),
                    c120 = FormatterNull(a.c120),
                    c121 = FormatterNull(a.c121),
                    c122 = FormatterNull(a.c122),
                    c123 = FormatterNull(a.c123),
                    c124 = FormatterNull(a.c124),
                    c125 = FormatterNull(a.c125),
                    c126 = FormatterNull(a.c126),
                    c127 = FormatterNull(a.c127),
                    c128 = FormatterNull(a.c128),
                    c129 = FormatterNull(a.c129),
                    c130 = FormatterNull(a.c130),
                    c131 = FormatterNull(a.c131),
                    c132 = FormatterNull(a.c132),
                    c133 = FormatterNull(a.c133),
                    c134 = FormatterNull(a.c134),
                    c135 = FormatterNull(a.c135),
                    c138 = FormatterNull(a.c138),
                    c139 = FormatterNull(a.c139),
                    c140 = FormatterNull(a.c140),
                    c141 = FormatterNull(a.c141),
                    c142 = FormatterNull(a.c142),
                    c144 = FormatterNull(a.c144),
                    c145 = FormatterNull(a.c145),
                    c146 = FormatterNull(a.c146),
                    c147 = FormatterNull(a.c147),
                    c148 = FormatterNull(a.c148),
                    c149 = FormatterNull(a.c149),
                    c150 = FormatterNull(a.c150),
                    c151 = FormatterNull(a.c151),
                    c152 = FormatterNull(a.c152),
                    c153 = FormatterNull(a.c153),
                }).ToArray();
            #endregion

            #region 构建Excel
            FileStream file11 = new FileStream(Server.MapPath("~/Template/Template_HT_PlatFormOrderReports.xls"), FileMode.Open, FileAccess.Read);

            //FileStream file11 = new FileStream(Server.MapPath("/Template/Template_HT_PlatFormOrderReports.xls"), FileMode.Open, FileAccess.Read);
            HSSFWorkbook wk = new HSSFWorkbook(file11);

            ISheet sheet = wk.GetSheet("Report");

            //ICellStyle style = wk.CreateCellStyle();
            //style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightOrange.Index;
            //style.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;
            //style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightOrange.Index;
            //style.Alignment = HorizontalAlignment.Center;
            //IFont font = wk.CreateFont();
            //font.Color = NPOI.HSSF.Util.HSSFColor.White.Index;
            //font.Boldweight = short.MaxValue;
            //font.FontHeightInPoints = 10;
            //style.SetFont(font);

            //NPOI.HSSF.UserModel.HSSFDataFormat format = (HSSFDataFormat)wk.CreateDataFormat();

            #endregion

            #region 生成表头
            var title = new string[] { "申请人姓名", "申请人MUDID","申请人Territory Code", "申请人职位","申请人手机号码", "预申请申请日期", "预申请申请时间","预申请修改日期", "预申请修改时间",
                "HT编号", "Market","VeevaMeetingID", "TA", "省份", "城市", "医院编码", "医院名称", "医院地址", "会议名称", "会议日期","会议时间", "参会人数", "大区区域代码",
                "预算金额", "直线经理是否随访", "是否由外部免费讲者来讲","RD Territory Code", "预申请审批人姓名", "预申请审批人MUDID", "预申请审批日期", "预申请审批时间",
                "预申请审批状态", "预申请是否重新分配审批人", "预申请重新分配审批人-操作人", "预申请重新分配审批人-操作人MUDID", "预申请被重新分配审批人姓名", "预申请被重新分配审批人MUDID",
                "预申请重新分配审批人日期", "预申请重新分配审批人时间", "供应商", "订单号","下单日期", "下单时间", "送餐日期", "送餐时间", "餐厅编码","预定餐厅", "用餐人数", "总份数",
                "预订金额", "实际金额", "金额调整原因", "送餐地址", "收餐人姓名", "收餐人电话", "下单备注", "是否预定成功", "预定成功日期", "预定成功时间",
                "是否申请退单", "是否退单成功", "退单失败平台发起配送需求", "退单失败线下反馈配送需求", "预定/退单失败原因", "是否收餐/未送达", "确认收餐日期", "确认收餐时间",
                "用户确认金额","是否与预定餐品一致", "用户确认金额调整原因", "用户确认金额调整备注", "实际用餐人数", "实际用餐人数调整原因", "实际用餐人数调整备注", "未送达描述", "准点率",
                "准点率描述", "食品安全存在问题", "食品安全问题描述", "餐品卫生及新鲜", "餐品卫生描述", "餐品包装", "餐品包装描述", "餐品性价比",
                "餐品性价比描述", "其他评价", "在线评分", "评论日期", "评论时间", "1=同一医院当日多场", "2=同一代表当日多场", "3=同一餐厅当日多场",
                "4=同一代表同一医院当日多场", "5=同一代表同一餐厅当日多场", "6=同一代表同一医院同一餐厅当日多场", "7=参会人数>=60", "8=参会人数<60,订单份数>=60", "9=代表自提",
                "直线经理姓名", "直线经理MUDID", "Level2 Manager 姓名", "Level2 Manager MUDID", "Level3 Manager 姓名", "Level3 Manager MUDID", "是否上传文件",
                "上传文件提交日期", "上传文件提交时间", "上传文件审批直线经理姓名", "上传文件审批直线经理MUDID", "上传文件审批日期", "上传文件审批时间", "上传文件审批状态",
                "签到人数是否等于实际用餐人数", "签到人数调整原因","是否与会议信息一致","会议信息不一致原因", "上传文件是否Re-Open", "上传文件Re-Open操作人", "上传文件Re-Open操作人MUDID", "上传文件Re-Open日期",
                "上传文件Re-Open时间", "上传文件Re-Open原因", "上传文件Re-Open审批状态", "上传文件是否重新分配", "上传文件重新分配操作人", "上传文件重新分配操作人MUDID", "上传文件被重新分配人姓名",
                "上传文件被重新分配人MUDID", "上传文件被重新分配日期", "上传文件被重新分配时间", "上传文件否重新分配审批人", "上传文件重新分配审批人-操作人",
                "上传文件重新分配审批人-操作人MUDID", "上传文件被重新分配审批人姓名", "上传文件被重新分配审批人MUDID", "上传文件重新分配审批人日期","上传文件重新分配审批人时间",
                "项目组特殊备注", "供应商特殊备注", "是否完成送餐", "与供应商确认订单金额", "GSK项目组确认金额", "GSK项目组确认金额调整原因", "餐费发票税点", "餐费付款金额",
                "餐费付款PO号码", "到账时间", "FFA 姓名", "FFA MUDUD", "SD助理", "SD助理MUDID" };

            //sheet.DefaultRowHeight = 200 * 2;

            //for (var i = 0; i < title.Length; i++)
            //{
            //    sheet.SetColumnWidth(i, 20 * 256);
            //}

            //for (var i = 0; i < title.Length; i++)
            //{
            //    ICell cell = row.CreateCell(i);
            //    cell.SetCellValue(title[i]);
            //    cell.SetCellType(CellType.String);
            //    //设置单元格格式
            //    cell.CellStyle = style;
            //}
            #endregion

            #region 制作表体
            for (var i = 1; i <= list.Length; i++)
            {
                var item = list[i - 1];
                IRow row = sheet.CreateRow(i);
                ICell cell = null;

                //2018-1-12 史强 注释掉序号列
                //cell = row.CreateCell(0);
                //cell.SetCellValue(i);



                var j = 0;
                if (item != null)
                {
                    //申请人姓名
                    cell = row.CreateCell(j);
                    cell.SetCellValue(item.c0);
                    //申请人MUDID
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c1);
                    //申请人Territory Code
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c144);
                    //申请人职位
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c132);
                    //申请人手机号码
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c2);
                    //预申请申请日期
                    cell = row.CreateCell(++j);
                    DateTime dateTimePara;
                    if (!string.IsNullOrEmpty(item.c3))
                    {
                        DateTime.TryParse(item.c3, out dateTimePara);
                        cell.SetCellValue(dateTimePara);
                    }
                    //预申请申请时间
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c4);
                    //预申请修改日期
                    cell = row.CreateCell(++j);
                    if (!string.IsNullOrEmpty(item.c5))
                    {
                        DateTime.TryParse(item.c5, out dateTimePara);
                        cell.SetCellValue(dateTimePara);
                    }
                    //预申请修改时间
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c6);
                    //HT编号
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c7);
                    //Market
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c8);
                    //VeevaMeetingID
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c143);
                    //TA
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c9);
                    //省份
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c10);
                    //城市
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c11);
                    //医院编码
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c12);
                    //医院名称
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c13);
                    //医院地址
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c14);
                    //会议名称
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c15);
                    //会议日期
                    cell = row.CreateCell(++j);
                    if (!string.IsNullOrEmpty(item.c16))
                    {
                        DateTime.TryParse(item.c16, out dateTimePara);
                        cell.SetCellValue(dateTimePara);
                    }
                    //会议时间
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c17);
                    //参会人数
                    cell = row.CreateCell(++j);
                    int attendCount;
                    int.TryParse(item.c18, out attendCount);
                    cell.SetCellValue(attendCount);
                    //成本中心
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c19);
                    //预算金额
                    cell = row.CreateCell(++j);
                    double budgetTotal;
                    double.TryParse(item.c20, out budgetTotal);
                    cell.SetCellValue(budgetTotal);
                    //直线经理是否随访
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c21);
                    //是否由外部免费讲者来讲
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c22);
                    ////RDSDName
                    //cell = row.CreateCell(++j);
                    //cell.SetCellValue(item.c141);
                    ////RDSDMUDID
                    //cell = row.CreateCell(++j);
                    //cell.SetCellValue(item.c142);
                    //RD Territory Code                    
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c145);
                    //if (item.c144 == "" && budgetTotal < 2000)
                    //{
                    //    cell = row.CreateCell(++j);
                    //    cell.SetCellValue("系统自动审批");
                    //}
                    //else
                    //{
                    //    cell = row.CreateCell(++j);
                    //    cell.SetCellValue(item.c23);
                    //}
                    //预申请审批人姓名
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c23);
                    //预申请审批人MUDID
                    //if (item.c144 == "" && budgetTotal < 2000)
                    //{
                    //    cell = row.CreateCell(++j);
                    //    cell.SetCellValue("系统自动审批");
                    //}
                    //else
                    //{
                    //    cell = row.CreateCell(++j);
                    //    cell.SetCellValue(item.c24);
                    //}
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c24);
                    //预申请审批日期
                    cell = row.CreateCell(++j);
                    if (!string.IsNullOrEmpty(item.c25))
                    {
                        DateTime.TryParse(item.c25, out dateTimePara);
                        cell.SetCellValue(dateTimePara);
                    }
                    //预申请审批时间
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c26);
                    //预申请审批状态
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c27);
                    //预申请是否重新分配审批人
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c28);
                    //预申请重新分配审批人-操作人
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c29);
                    //预申请重新分配审批人-操作人MUDID
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c30);
                    //预申请被重新分配审批人姓名
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c31);
                    //预申请被重新分配审批人MUDID
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c32);
                    //预申请重新分配审批人日期
                    cell = row.CreateCell(++j);
                    if (!string.IsNullOrEmpty(item.c33))
                    {
                        DateTime.TryParse(item.c33, out dateTimePara);
                        cell.SetCellValue(dateTimePara);
                    }
                    //预申请重新分配审批人时间
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c34);
                    //供应商
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c35);
                    //订单号
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c36);
                    //下单日期
                    cell = row.CreateCell(++j);
                    if (!string.IsNullOrEmpty(item.c37))
                    {
                        DateTime.TryParse(item.c37, out dateTimePara);
                        cell.SetCellValue(dateTimePara);
                    }
                    //下单时间
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c38);
                    //送餐日期
                    cell = row.CreateCell(++j);
                    if (!string.IsNullOrEmpty(item.c39))
                    {
                        DateTime.TryParse(item.c39, out dateTimePara);
                        cell.SetCellValue(dateTimePara);
                    }
                    //送餐时间
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c40);
                    //餐厅编码
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c41);
                    //预订餐厅
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c42);
                    //用餐人数
                    cell = row.CreateCell(++j);
                    int.TryParse(item.c43, out attendCount);
                    cell.SetCellValue(attendCount);
                    //总份数
                    //cell = row.CreateCell(++j);
                    //cell.SetCellValue(item.c44);
                    //预订金额
                    cell = row.CreateCell(++j);
                    double.TryParse(item.c45, out budgetTotal);
                    cell.SetCellValue(budgetTotal);
                    //实际金额
                    cell = row.CreateCell(++j);
                    double.TryParse(item.c46, out budgetTotal);
                    cell.SetCellValue(budgetTotal);
                    //金额调整原因
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c47);
                    //送餐地址
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c48);
                    //收餐人姓名
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c49);
                    //收餐人电话
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c50);
                    //下单备注
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c51);
                    //是否预定成功
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c52);
                    //预定成功日期
                    cell = row.CreateCell(++j);
                    if (!string.IsNullOrEmpty(item.c53))
                    {
                        DateTime.TryParse(item.c53, out dateTimePara);
                        cell.SetCellValue(dateTimePara);
                    }
                    //预定成功时间
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c54);
                    //是否申请退单
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c55);
                    //是否退单成功
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c56);
                    //预定/退单失败平台发起配送需求
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c57);
                    //退单失败线下反馈配送需求
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c58);
                    //预定/退单失败原因
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c59);
                    //是否收餐/未送达
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c60);
                    //确认收餐日期
                    cell = row.CreateCell(++j);
                    if (!string.IsNullOrEmpty(item.c61))
                    {
                        DateTime.TryParse(item.c61, out dateTimePara);
                        cell.SetCellValue(dateTimePara);
                    }
                    //确认收餐时间
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c62);
                    //用户确认金额
                    cell = row.CreateCell(++j);
                    double.TryParse(item.c63, out budgetTotal);
                    if (budgetTotal != 0)
                    {
                        cell.SetCellValue(budgetTotal);
                    }
                    //是否与预定餐品一致
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c138);
                    //用户确认金额调整原因
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c64);
                    //用户确认金额调整备注
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c65);
                    //实际用餐人数
                    cell = row.CreateCell(++j);
                    int.TryParse(item.c66, out attendCount);
                    if (attendCount != 0)
                    {
                        cell.SetCellValue(attendCount);
                    }
                    //实际用餐人数调整原因
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c67);
                    //实际用餐人数调整备注
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c68);
                    //未送达描述
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c69);
                    //准点率
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c70);
                    //准点率描述
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c71);
                    //食品安全存在问题
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c72);
                    //食品安全问题描述
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c73);
                    //餐品卫生及新鲜
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c74);
                    //餐品卫生描述
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c75);
                    //餐品包装
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c76);
                    //餐品包装描述
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c77);
                    //餐品性价比
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c78);
                    //餐品性价比描述
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c79);
                    //其他评价
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c80);
                    //在线评分
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c81);
                    //评论日期
                    cell = row.CreateCell(++j);
                    if (!string.IsNullOrEmpty(item.c82))
                    {
                        DateTime.TryParse(item.c82, out dateTimePara);
                        cell.SetCellValue(dateTimePara);
                    }
                    //评论时间
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c83);
                    //1=同一医院当日多场
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c84);
                    //2=同一代表当日多场
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c85);
                    //3=同一餐厅当日多场
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c86);
                    //4=同一代表同一医院当日多场
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c87);
                    //5=同一代表同一餐厅当日多场
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c88);
                    //6=同一代表同一医院同一餐厅当日多场
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c89);
                    //7=参会人数>=60
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c90);
                    //8=参会人数<60,订单分数>=60
                    //cell = row.CreateCell(++j);
                    //cell.SetCellValue(item.c91);
                    //9=代表自提
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c92);
                    ////直线经理姓名
                    //cell = row.CreateCell(++j);
                    //cell.SetCellValue(item.c93);
                    ////直线经理MUDID
                    //cell = row.CreateCell(++j);
                    //cell.SetCellValue(item.c94);
                    ////Level2 Manager 姓名
                    //cell = row.CreateCell(++j);
                    //cell.SetCellValue(item.c95);
                    ////Level2 Manager MUDID
                    //cell = row.CreateCell(++j);
                    //cell.SetCellValue(item.c96);
                    ////Level3 Manager 姓名
                    //cell = row.CreateCell(++j);
                    //cell.SetCellValue(item.c97);
                    ////Level3 Manager MUDID
                    //cell = row.CreateCell(++j);
                    //cell.SetCellValue(item.c98);
                    //是否上传文件
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c99);
                    //上传文件提交日期
                    cell = row.CreateCell(++j);
                    if (!string.IsNullOrEmpty(item.c100))
                    {
                        DateTime.TryParse(item.c100, out dateTimePara);
                        cell.SetCellValue(dateTimePara);
                    }
                    //上传文件提交时间
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c101);
                    //上传文件审批直线经理姓名
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c102);
                    //上传文件审批直线经理MUDID
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c103);
                    //上传文件审批日期
                    cell = row.CreateCell(++j);
                    if (!string.IsNullOrEmpty(item.c104))
                    {
                        DateTime.TryParse(item.c104, out dateTimePara);
                        cell.SetCellValue(dateTimePara);
                    }
                    //上传文件审批时间
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c105);
                    //上传文件审批状态
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c106);
                    //签到人数是否等于实际用餐人数
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c107);
                    //
                    //cell = row.CreateCell(++j);
                    //cell.SetCellValue(item.c108);
                    //签到人数调整原因
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c109);
                    //是否与会议信息一致
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c139);
                    //会议信息不一致原因
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c140);
                    //上传文件是否Re-Open
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c110);
                    //上传文件Re-Open操作人
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c111);
                    //上传文件Re-Open操作人MUDID
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c112);
                    //上传文件Re-Open日期
                    cell = row.CreateCell(++j);
                    if (!string.IsNullOrEmpty(item.c113))
                    {
                        DateTime.TryParse(item.c113, out dateTimePara);
                        cell.SetCellValue(dateTimePara);
                    }
                    //上传文件Re-Open时间
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c114);
                    //上传文件Re-Open发起人姓名
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c133);
                    //上传文件Re-Open发起人MUDID
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c134);
                    //上传文件Re-Open原因
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c115);
                    //上传文件Re-Open备注
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c135);
                    //上传文件Re-Open审批状态
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c116);
                    //上传文件是否重新分配
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c117);
                    //上传文件重新分配操作人
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c118);
                    //上传文件重新分配操作人MUDID
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c119);
                    //上传文件被重新分配人姓名
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c120);
                    //上传文件被重新分配人MUDID
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c121);
                    //上传文件被重新分配日期
                    cell = row.CreateCell(++j);
                    if (!string.IsNullOrEmpty(item.c122))
                    {
                        DateTime.TryParse(item.c122, out dateTimePara);
                        cell.SetCellValue(dateTimePara);
                    }
                    //上传文件被重新分配时间
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c123);
                    //上传文件否重新分配审批人
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c124);
                    //上传文件重新分配审批人-操作人
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c125);
                    //上传文件重新分配审批人-操作人MUDID
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c126);
                    //上传文件被重新分配审批人姓名
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c127);
                    //上传文件被重新分配审批人MUDID
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c128);
                    //上传文件重新分配审批人日期
                    cell = row.CreateCell(++j);
                    if (!string.IsNullOrEmpty(item.c129))
                    {
                        DateTime.TryParse(item.c129, out dateTimePara);
                        cell.SetCellValue(dateTimePara);
                    }
                    //上传文件重新分配审批人时间
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c130);
                    //项目组特殊备注 
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c131);
                    //供应商特殊备注
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c146);
                    //是否完成送餐
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c147);
                    //与供应商确认订单金额
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c148);
                    //GSK项目组确认金额
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c149);
                    //GSK项目组确认金额调整原因
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c150);
                    //
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(string.Empty);
                    //餐费付款金额
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c151);
                    //餐费付款PO号码
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c152);
                    //到账时间
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c153);
                    //
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(string.Empty);
                    //
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(string.Empty);
                    //
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(string.Empty);
                    //
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(string.Empty);
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

        #region 导出1.0HT报表
        /// <summary>
        /// 导出1.0HT报表
        /// </summary>
        /// <param name="CN"></param>
        /// <param name="MUID"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="channel"></param>
        /// <param name="isOrderSuccess"></param>
        /// <param name="isReceived"></param>
        /// <param name="isReturn"></param>
        /// <param name="isReturnSuccess"></param>
        public void ExcelOldCater(string CN, string MUID,
            string startDate, string endDate,
            string channel, string isOrderSuccess, string isReceived, string isReturn, string isReturnSuccess, string Supplier)
        {
            #region 抓取数据
            int total = 0;

            var list = reportService.LoadOldCater(
                CN, MUID, startDate, endDate, channel,
                isOrderSuccess, isReceived, isReturn, isReturnSuccess, Supplier,
                1, int.MaxValue, out total).Select(a => new
                {
                    c0 = FormatterNull(a.c0),
                    c1 = FormatterNull(a.c1),
                    c2 = FormatterNull(a.c2),
                    c3 = FormatterNull(a.c3),
                    c4 = FormatterNull(a.c4),
                    c5 = FormatterNull(a.c5.ToUpper()),
                    c6 = FormatterNull(a.c6),
                    c7 = FormatterNull(a.c7),
                    c8 = FormatterNull(a.c8),
                    c9 = FormatterNull(a.c9),
                    c10 = FormatterNull(a.c10),
                    c55 = FormatterNull(a.c55),
                    c11 = FormatterNull(a.c11),
                    c12 = FormatterNull(a.c12),
                    c13 = FormatterNull(a.c13),
                    c14 = FormatterNull(a.c14),
                    c15 = FormatterNull(a.c15),
                    c16 = FormatterNull(a.c16),
                    c17 = FormatterNull(a.c17),
                    c18 = FormatterNull(a.c18),
                    c19 = FormatterNull(a.c19),
                    c20 = FormatterNull(a.c20),
                    c21 = FormatterNull(a.c21),
                    c22 = FormatterNull(a.c22),
                    c23 = FormatterNull(a.c23),
                    c24 = FormatterNull(a.c24),
                    c25 = FormatterNull(a.c25),
                    c26 = FormatterNull(a.c26),
                    c27 = FormatterNull(a.c27),
                    c28 = FormatterNull(a.c28),
                    c29 = FormatterNull(a.c29),
                    c30 = FormatterNull(a.c30),
                    c31 = FormatterNull(a.c31),
                    c32 = FormatterNull(a.c32),
                    c33 = FormatterNull(a.c33),
                    c34 = FormatterNull(a.c34),
                    c35 = FormatterNull(a.c35),
                    c36 = FormatterNull(a.c36),
                    c37 = FormatterNull(a.c37),
                    c38 = FormatterNull(a.c38),
                    c39 = FormatterNull(a.c39),
                    c40 = FormatterNull(a.c40),
                    c41 = FormatterNull(a.c41),
                    c42 = FormatterNull(a.c42),
                    c43 = FormatterNull(a.c43),
                    c44 = FormatterNull(a.c44),
                    c45 = FormatterNull(a.c45),
                    c46 = FormatterNull(a.c46),
                    c47 = FormatterNull(a.c47),
                    c48 = FormatterNull(a.c48),
                    c49 = FormatterNull(a.c49),
                    c50 = FormatterNull(a.c50),
                    c51 = FormatterNull(a.c51),
                    c52 = FormatterNull(a.c52),
                    c53 = FormatterNull(a.c53),
                    c54 = FormatterNull(a.c54),
                    c57 = FormatterNull(a.c57),
                    c58 = FormatterNull(a.c58),
                    c59 = FormatterNull(a.c59),
                    c60 = FormatterNull(a.c60),
                }).ToArray();
            #endregion

            #region 构建Excel
            HSSFWorkbook wk = new HSSFWorkbook();
            ISheet sheet = wk.CreateSheet("Cater");
            IRow row = sheet.CreateRow(0);

            ICellStyle style = wk.CreateCellStyle();
            style.WrapText = true;
            style.Alignment = HorizontalAlignment.Center;
            IFont font = wk.CreateFont();
            font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
            style.SetFont(font);
            #endregion

            #region 生成表头
            var title = new string[] { "订餐人姓名", "订餐人手机号", "MUDID", "Market", "预申请表CN号", "预算金额", "供应商", "订单号", "下单日期", "下单时间", "送餐日期", "送餐时间", "餐厅编码", "预订餐厅", "参会人数", "总份数", "预订金额", "实际金额", "金额调整原因", "1=同一医院当日多场", "2=同一代表当日多场", "3=同一餐厅当日多场", "4=同一代表同一医院当日多场", "5=同一代表同一餐厅当日多场", "6=同一代表同一医院同一餐厅当日多场", "7=参会人数>=60", "8=参会人数<60,订单份数>=60", "9=代表自提", "省份", "城市", "医院编码", "医院名称", "医院地址", "送餐地址", "收餐人姓名", "收餐人电话", "是否预定成功", "预定成功日期", "预定成功时间", "是否收餐/未送达", "未送达描述", "准点率", "准点率描述", "食品安全存在问题", "食品安全问题描述", "餐品卫生及新鲜", "餐品卫生描述", "餐品包装", "餐品包装描述", "餐品性价比", "餐品性价比描述", "其他评价", "在线评分", "评论日期", "评论时间", "是否申请退单", "是否退单成功", "退单失败平台发起配送需求", "退单失败线下反馈配送需求", "预订/退单失败原因" };

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
                    cell.SetCellValue(item.c0);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c1);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c2);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c3);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c4);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c57);
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
                    cell.SetCellValue(item.c55);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c11);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c12);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c13);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c14);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c15);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c16);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c17);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c18);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c58);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c19);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c59);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c60);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c20);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c21);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c22);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c23);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c24);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c25);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c26);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c27);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c28);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c29);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c30);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c31);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c32);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c33);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c34);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c35);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c36);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c37);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c38);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c39);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c40);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c41);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c42);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c43);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c44);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c45);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c46);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c47);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c48);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c49);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c50);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c51);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c52);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c53);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c54);
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

        #region 导出NonHT报表
        /// <summary>
        /// 导出NonHT报表
        /// </summary>
        /// <param name="CN"></param>
        /// <param name="MUID"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="channel"></param>
        /// <param name="isOrderSuccess"></param>
        /// <param name="isReceived"></param>
        /// <param name="isReturn"></param>
        /// <param name="isReturnSuccess"></param>
        public void ExcelCaterForNonHT(string CN, string MUID, string HospitalCode, string RestaurantId,
            string startDate, string endDate,
            string channel, string isOrderSuccess, string isReceived, string isReturn, string isReturnSuccess, string Supplier)
        {
            #region 抓取数据
            int total = 0;

            var list = reportService.LoadCaterForNonHT(
                CN, MUID, HospitalCode, RestaurantId, startDate, endDate, channel,
                isOrderSuccess, isReceived, isReturn, isReturnSuccess, Supplier,
                1, int.MaxValue, out total)
                .Select(a => new
                {
                    c0 = FormatterNull(a.c0),
                    c1 = FormatterNull(a.c1),
                    c2 = FormatterNull(a.c2),
                    c3 = FormatterNull(a.c3),
                    c4 = FormatterNull(a.c4),
                    c5 = FormatterNull(a.c5),
                    c6 = FormatterNull(a.c6),
                    c7 = FormatterNull(a.c7),
                    c8 = FormatterNull(a.c8),
                    c9 = FormatterNull(a.c9.ToUpper()),
                    c10 = FormatterNull(a.c10),
                    c11 = FormatterNull(a.c11),
                    c12 = FormatterNull(a.c12),
                    c13 = FormatterNull(a.c13),
                    c14 = FormatterNull(a.c14),
                    c55 = FormatterNull(a.c55),
                    c15 = FormatterNull(a.c15),
                    c16 = FormatterNull(a.c16),
                    c17 = FormatterNull(a.c17),
                    c18 = FormatterNull(a.c18),
                    c19 = FormatterNull(a.c19),
                    c20 = FormatterNull(a.c20),
                    c21 = FormatterNull(a.c21),
                    c22 = FormatterNull(a.c22),
                    c23 = FormatterNull(a.c23),
                    c24 = FormatterNull(a.c24),
                    c25 = FormatterNull(a.c25),
                    c26 = FormatterNull(a.c26),
                    c27 = FormatterNull(a.c27),
                    c28 = FormatterNull(a.c28),
                    c29 = FormatterNull(a.c29),
                    c30 = FormatterNull(a.c30),
                    c31 = FormatterNull(a.c31),
                    c32 = FormatterNull(a.c32),
                    c33 = FormatterNull(a.c33),
                    c34 = FormatterNull(a.c34),
                    c35 = FormatterNull(a.c35),
                    c36 = FormatterNull(a.c36),
                    c37 = FormatterNull(a.c37),
                    c38 = FormatterNull(a.c38),
                    c39 = FormatterNull(a.c39),
                    c40 = FormatterNull(a.c40),
                    c41 = FormatterNull(a.c41),
                    c42 = FormatterNull(a.c42),
                    c43 = FormatterNull(a.c43),
                    c44 = FormatterNull(a.c44),
                    c45 = FormatterNull(a.c45),
                    c46 = FormatterNull(a.c46),
                    c47 = FormatterNull(a.c47),
                    c48 = FormatterNull(a.c48),
                    c49 = FormatterNull(a.c49),
                    c50 = FormatterNull(a.c50),
                    c51 = FormatterNull(a.c51),
                    c52 = FormatterNull(a.c52),
                }).ToArray();
            #endregion

            #region 构建Excel
            HSSFWorkbook wk = new HSSFWorkbook();
            ISheet sheet = wk.CreateSheet("Cater");
            IRow row = sheet.CreateRow(0);

            ICellStyle style = wk.CreateCellStyle();
            style.WrapText = true;
            style.Alignment = HorizontalAlignment.Center;
            IFont font = wk.CreateFont();
            font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
            style.SetFont(font);
            #endregion

            #region 生成表头
            var title = new string[] { "订餐人姓名", "订餐人手机号", "订餐人MUDID", "用餐人Market", "用餐人TA", "Meeting Code", "会议名称", "PO No.", "WBS", "供应商", "订单号", "下单日期", "下单时间", "送餐日期", "送餐时间", "餐厅编码", "预订餐厅", "参会人数", "总份数", "预订金额", "实际金额", "金额调整原因", "省份", "城市", "医院编码", "医院名称", "医院地址", "送餐地址", "收餐人姓名", "收餐人电话", "是否预定成功", "预定成功日期", "预定成功时间", "是否收餐/未送达", "未送达描述", "准点率", "准点率描述", "食品安全存在问题", "食品安全问题描述", "餐品卫生及新鲜", "餐品卫生描述", "餐品包装", "餐品包装描述", "餐品性价比", "餐品性价比描述", "其他评价", "在线评分", "评论日期", "评论时间", "是否申请退单", "是否退单成功", "退单失败平台发起配送需求", "退单失败线下反馈配送需求", "预订/退单失败原因" };

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

                //cell = row.CreateCell(0);
                //cell.SetCellValue(i);

                var j = 0;
                if (item != null)
                {
                    cell = row.CreateCell(j);
                    cell.SetCellValue(item.c0);
                    cell = row.CreateCell(++j);
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
                    cell.SetCellValue(item.c14);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c55);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c15);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c16);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c17);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c18);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c19);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c20);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c21);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c22);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c23);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c24);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c25);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c26);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c27);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c28);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c29);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c30);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c31);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c32);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c33);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c34);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c35);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c36);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c37);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c38);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c39);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c40);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c41);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c42);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c43);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c44);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c45);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c46);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c47);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c48);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c49);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c50);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c51);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.c52);
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

        static string FormatterNull(decimal? str)
        {
            return $"{str}";
        }

        #region 导出菜品详情
        public void ExportFoodDetails(string CN, string MUID,string TACode, string HospitalCode, string RestaurantId, string CostCenter,
            string startDate, string endDate,
            string channel, string isOrderSuccess, string isReceived, string isReturn, string isReturnSuccess, string Supplier, string IsSpecialOrder, string OrderState)
        {
            #region 抓取数据
            int total = 0;
            string oldHospitalCode = PreApprovalService.GetOldGskHospitalCodeByGskHospitalCode(HospitalCode);
            if (!string.IsNullOrEmpty(oldHospitalCode))
            {
                HospitalCode += "," + oldHospitalCode;
            }
            var list = reportService.LoadReport(
                CN, MUID, TACode, HospitalCode, RestaurantId, CostCenter, startDate, endDate, channel,
                isOrderSuccess, isReceived, isReturn, isReturnSuccess, Supplier, IsSpecialOrder, OrderState,
                1, int.MaxValue, out total)
                .Select(a => new
                {
                    c0 = FormatterNull(a.c0),
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
                    c14 = FormatterNull(a.c14),
                    c15 = FormatterNull(a.c15),
                    c16 = FormatterNull(a.c16),
                    c17 = FormatterNull(a.c17),
                    c18 = FormatterNull(a.c18),
                    c19 = FormatterNull(a.c19),
                    c20 = FormatterNull(a.c20),
                    c21 = FormatterNull(a.c21),
                    c22 = FormatterNull(a.c22),
                    c23 = FormatterNull(a.c23),
                    c24 = FormatterNull(a.c24),
                    c25 = FormatterNull(a.c25),
                    c26 = FormatterNull(a.c26),
                    c27 = FormatterNull(a.c27),
                    c28 = FormatterNull(a.c28),
                    c29 = FormatterNull(a.c29),
                    c30 = FormatterNull(a.c30),
                    c31 = FormatterNull(a.c31),
                    c32 = FormatterNull(a.c32),
                    c33 = FormatterNull(a.c33),
                    c34 = FormatterNull(a.c34),
                    c35 = FormatterNull(a.c35),
                    c36 = FormatterNull(a.c36),
                    c37 = FormatterNull(a.c37),
                    c38 = FormatterNull(a.c38),
                    c39 = FormatterNull(a.c39),
                    c40 = FormatterNull(a.c40),
                    c41 = FormatterNull(a.c41),
                    c42 = FormatterNull(a.c42),
                    c43 = FormatterNull(a.c43),
                    c44 = FormatterNull(a.c44),
                    c45 = FormatterNull(a.c45),
                    c46 = FormatterNull(a.c46),
                    c47 = FormatterNull(a.c47),
                    c48 = FormatterNull(a.c48),
                    c49 = FormatterNull(a.c49),
                    c50 = FormatterNull(a.c50),
                    c51 = FormatterNull(a.c51),
                    c52 = FormatterNull(a.c52),
                    c53 = FormatterNull(a.c53),
                    c54 = FormatterNull(a.c54),
                    c55 = FormatterNull(a.c55),
                    c56 = FormatterNull(a.c56),
                    c57 = FormatterNull(a.c57),
                    c58 = FormatterNull(a.c58),
                    c59 = FormatterNull(a.c59),
                    c60 = FormatterNull(a.c60),
                    c61 = FormatterNull(a.c61),
                    c62 = FormatterNull(a.c62),
                    c63 = FormatterNull(a.c63),
                    c64 = FormatterNull(a.c64),
                    c65 = FormatterNull(a.c65),
                    c66 = FormatterNull(a.c66),
                    c67 = FormatterNull(a.c67),
                    c68 = FormatterNull(a.c68),
                    c69 = FormatterNull(a.c69),
                    c70 = FormatterNull(a.c70),
                    c71 = FormatterNull(a.c71),
                    c72 = FormatterNull(a.c72),
                    c73 = FormatterNull(a.c73),
                    c74 = FormatterNull(a.c74),
                    c75 = FormatterNull(a.c75),
                    c76 = FormatterNull(a.c76),
                    c77 = FormatterNull(a.c77),
                    c78 = FormatterNull(a.c78),
                    c79 = FormatterNull(a.c79),
                    c80 = FormatterNull(a.c80),
                    c81 = FormatterNull(a.c81),
                    c82 = FormatterNull(a.c82),
                    c83 = FormatterNull(a.c83),
                    c84 = FormatterNull(a.c84),
                    c85 = FormatterNull(a.c85),
                    c86 = FormatterNull(a.c86),
                    c87 = FormatterNull(a.c87),
                    c88 = FormatterNull(a.c88),
                    c89 = FormatterNull(a.c89),
                    c90 = FormatterNull(a.c90),
                    c91 = FormatterNull(a.c91),
                    c92 = FormatterNull(a.c92),
                    c93 = FormatterNull(a.c93),
                    c94 = FormatterNull(a.c94),
                    c95 = FormatterNull(a.c95),
                    c96 = FormatterNull(a.c96),
                    c97 = FormatterNull(a.c97),
                    c98 = FormatterNull(a.c98),
                    c99 = FormatterNull(a.c99),
                    c100 = FormatterNull(a.c100),
                    c101 = FormatterNull(a.c101),
                    c102 = FormatterNull(a.c102),
                    c103 = FormatterNull(a.c103),
                    c104 = FormatterNull(a.c104),
                    c105 = FormatterNull(a.c105),
                    c106 = FormatterNull(a.c106),
                    c107 = FormatterNull(a.c107),
                    c108 = FormatterNull(a.c108),
                    c109 = FormatterNull(a.c109),
                    c110 = FormatterNull(a.c110),
                    c111 = FormatterNull(a.c111),
                    c112 = FormatterNull(a.c112),
                    c113 = FormatterNull(a.c113),
                    c114 = FormatterNull(a.c114),
                    c115 = FormatterNull(a.c115),
                    c116 = FormatterNull(a.c116),
                    c117 = FormatterNull(a.c117),
                    c118 = FormatterNull(a.c118),
                    c119 = FormatterNull(a.c119),
                    c120 = FormatterNull(a.c120),
                    c121 = FormatterNull(a.c121),
                    c122 = FormatterNull(a.c122),
                    c123 = FormatterNull(a.c123),
                    c124 = FormatterNull(a.c124),
                    c125 = FormatterNull(a.c125),
                    c126 = FormatterNull(a.c126),
                    c127 = FormatterNull(a.c127),
                    c128 = FormatterNull(a.c128),
                    c129 = FormatterNull(a.c129),
                    c130 = FormatterNull(a.c130),
                    c131 = FormatterNull(a.c131),
                    c132 = FormatterNull(a.c132),
                    c133 = FormatterNull(a.c133),
                    c134 = FormatterNull(a.c134),
                    c135 = FormatterNull(a.c135),
                    c136 = FormatterNull(a.c136),
                    c137 = FormatterNull(a.c137),
                }).ToArray();
            #endregion            
            #region 构建Excel
            FileStream file11 = new FileStream(Server.MapPath("~/Template/FoodDetail.xls"), FileMode.Open, FileAccess.Read);
            HSSFWorkbook wk = new HSSFWorkbook(file11);

            ISheet sheet = wk.GetSheet("Report");

            #endregion

            var rowIndex = 0;
            var row_ = 0;
            foreach (var _item in list)
            {
                row_++;
                IRow row = sheet.CreateRow(row_);
                string detail = _item.c136;
                FoodDetail foodDetail = JsonConvert.DeserializeObject<FoodDetail>(detail);
                rowIndex++;
                row = sheet.CreateRow(rowIndex);
                ICell cell = null;

                cell = row.CreateCell(0);
                cell.SetCellValue(foodDetail.preApproval.HTCode);

                cell = row.CreateCell(1);
                cell.SetCellValue(foodDetail.hospital.Name);

                cell = row.CreateCell(2);
                cell.SetCellValue(foodDetail.hospital.Address);

                cell = row.CreateCell(3);
                cell.SetCellValue(foodDetail.details.deliveryAddress);

                cell = row.CreateCell(4);
                cell.SetCellValue(foodDetail.details.consignee);

                cell = row.CreateCell(5);
                cell.SetCellValue(foodDetail.details.phone);

                cell = row.CreateCell(6);
                cell.SetCellValue(foodDetail.foods.resId);

                cell = row.CreateCell(7);
                cell.SetCellValue(foodDetail.foods.resName);

                DateTime deliveryDate;
                if (!string.IsNullOrEmpty(foodDetail.details.deliverTime))
                {
                    cell = row.CreateCell(8);
                    DateTime.TryParse(foodDetail.details.deliverTime, out deliveryDate);
                    cell.SetCellValue(deliveryDate);
                }
                DateTime deliveryTime;
                if (!string.IsNullOrEmpty(foodDetail.details.deliverTime))
                {
                    cell = row.CreateCell(9);
                    DateTime.TryParse(foodDetail.details.deliverTime, out deliveryTime);
                    cell.SetCellValue(deliveryTime);
                }


                double ReservePrice;
                double.TryParse(_item.c45, out ReservePrice);
                cell = row.CreateCell(10);
                cell.SetCellValue(ReservePrice);

                double ActualPrice;
                double.TryParse(_item.c46, out ActualPrice);
                cell = row.CreateCell(11);
                cell.SetCellValue(ActualPrice);

                cell = row.CreateCell(12);
                cell.SetCellValue(_item.c47);

                cell = row.CreateCell(13);
                cell.SetCellValue(_item.c137);

                cell = row.CreateCell(14);
                cell.SetCellValue(foodDetail.foods.foodFee);

                cell = row.CreateCell(15);
                cell.SetCellValue(foodDetail.foods.packageFee);

                cell = row.CreateCell(16);
                cell.SetCellValue(foodDetail.foods.sendFee);
                int _row = 16;
                foreach (var item in foodDetail.foods.foods)
                {
                    _row++;
                    cell = row.CreateCell(_row);
                    cell.SetCellValue(item.foodName);
                    _row++;
                    cell = row.CreateCell(_row);
                    cell.SetCellValue(item.describe);
                    _row++;
                    cell = row.CreateCell(_row);
                    cell.SetCellValue(item.count);
                    _row++;
                    cell = row.CreateCell(_row);
                    cell.SetCellValue(item.price);
                }

            }


            // 写入到客户端  
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                wk.Write(ms);
                Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xls", DateTime.Now.ToString("yyyyMMddHHmmssfff")));
                Response.BinaryWrite(ms.ToArray());
            }
        }
        #endregion

        #region 加载1.0HT报表
        /// <summary>
        /// 加载1.0HT报表
        /// </summary>
        /// <param name="CN"></param>
        /// <param name="MUID"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="channel"></param>
        /// <param name="isOrderSuccess"></param>
        /// <param name="isReceived"></param>
        /// <param name="isReturn"></param>
        /// <param name="isReturnSuccess"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LoadOldCater(string CN, string MUID,
            string startDate, string endDate,
            string channel, string isOrderSuccess, string isReceived, string isReturn, string isReturnSuccess, string Supplier,
            int page, int rows)
        {
            int total = 0;

            var list = reportService.LoadOldCater(
                CN, MUID, startDate, endDate, channel,
                isOrderSuccess, isReceived, isReturn, isReturnSuccess, Supplier,
                page, rows, out total)
                .Select(a => new
                {
                    c0 = FormatterNull(a.c0),
                    c1 = FormatterNull(a.c1),
                    c2 = FormatterNull(a.c2),
                    c3 = FormatterNull(a.c3),
                    c4 = FormatterNull(a.c4),
                    c5 = FormatterNull(a.c5.ToUpper()),
                    c6 = FormatterNull(a.c6),
                    c7 = FormatterNull(a.c7),
                    c8 = FormatterNull(a.c8),
                    c9 = FormatterNull(a.c9),
                    c10 = FormatterNull(a.c10),
                    c55 = FormatterNull(a.c55),
                    c11 = FormatterNull(a.c11),
                    c12 = FormatterNull(a.c12),
                    c13 = FormatterNull(a.c13),
                    c14 = FormatterNull(a.c14),
                    c15 = FormatterNull(a.c15),
                    c16 = FormatterNull(a.c16),
                    c17 = FormatterNull(a.c17),
                    c18 = FormatterNull(a.c18),
                    c19 = FormatterNull(a.c19),
                    c20 = FormatterNull(a.c20),
                    c21 = FormatterNull(a.c21),
                    c22 = FormatterNull(a.c22),
                    c23 = FormatterNull(a.c23),
                    c24 = FormatterNull(a.c24),
                    c25 = FormatterNull(a.c25),
                    c26 = FormatterNull(a.c26),
                    c27 = FormatterNull(a.c27),
                    c28 = FormatterNull(a.c28),
                    c29 = FormatterNull(a.c29),
                    c30 = FormatterNull(a.c30),
                    c31 = FormatterNull(a.c31),
                    c32 = FormatterNull(a.c32),
                    c33 = FormatterNull(a.c33),
                    c34 = FormatterNull(a.c34),
                    c35 = FormatterNull(a.c35),
                    c36 = FormatterNull(a.c36),
                    c37 = FormatterNull(a.c37),
                    c38 = FormatterNull(a.c38),
                    c39 = FormatterNull(a.c39),
                    c40 = FormatterNull(a.c40),
                    c41 = FormatterNull(a.c41),
                    c42 = FormatterNull(a.c42),
                    c43 = FormatterNull(a.c43),
                    c44 = FormatterNull(a.c44),
                    c45 = FormatterNull(a.c45),
                    c46 = FormatterNull(a.c46),
                    c47 = FormatterNull(a.c47),
                    c48 = FormatterNull(a.c48),
                    c49 = FormatterNull(a.c49),
                    c50 = FormatterNull(a.c50),
                    c51 = FormatterNull(a.c51),
                    c52 = FormatterNull(a.c52),
                    c53 = FormatterNull(a.c53),
                    c54 = FormatterNull(a.c54),
                    c56 = FormatterNull(a.c56),
                    c57 = FormatterNull(a.c57),
                    c58 = FormatterNull(a.c58),
                    c59 = FormatterNull(a.c59),
                    c60 = FormatterNull(a.c60),
                }).ToArray();
            return Json(new { state = 1, rows = list, total = total });
        }
        #endregion
    }
}