using Amazon.S3;
using Amazon.S3.Model;
using IamPortal.AppLogin;
using iPathFeast.API.Client;
using iPathFeast.ApiEntity;
using MealAdmin.Entity;
using MealAdmin.Entity.Enum;
using MealAdmin.Entity.Helper;
using MealAdmin.Entity.View;
using MealAdmin.Service;
using MealAdmin.Util;
using MealAdmin.Web.Areas.P.Models;
using MealAdmin.Web.Filter;
using MealAdminApiClient;
using MeetingMealApiClient;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using XFramework.XInject.Attributes;
using XFramework.XUtil;

namespace MealAdmin.Web.Areas.P.Controllers
{
    public class OrderMntController : AdminBaseController
    {
        // GET: P/OrderMnt
        [Bean("orderService")]
        public IOrderService orderService { get; set; }

        [Bean("evaluateService")]
        public IEvaluateService evaluateService { get; set; }
        [Bean("operationAuditService")]
        public IOperationAuditService operationAuditService { get; set; }
        [Bean("preApprovalService")]
        public IPreApprovalService PreApprovalService { get; set; }
        [Autowired]
        ApiV1Client apiClient { get; set; }

        private static string awsAccessKey = "AKIAPJZX4QFWMMMA2RJA";
        private static string awsSecretKey = "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";
        private static string bucketName = "wechat";
        AmazonS3Config config = new AmazonS3Config()
        {
            ServiceURL = "http://s3.amazonaws.com",
            RegionEndpoint = Amazon.RegionEndpoint.CNNorth1
        };


        #region 特殊订单弹窗
        public ActionResult Pop(string HTCode, int State)
        {
            HTCode = "HT-" + HTCode.Substring(1);
            var ORDState = "";
            switch (State)
            {
                case 1:
                    ORDState = "订单待审批"; break;
                case 2:
                    ORDState = "订单审批被驳回"; break;
                case 3:
                    ORDState = "订单提交成功"; break;
                case 4:
                    ORDState = "预订成功"; break;
                case 5:
                    ORDState = "预订失败"; break;
                case 6:
                    ORDState = "已收餐"; break;
                case 7:
                    ORDState = "系统已收餐"; break;
                case 8:
                    ORDState = "未送达"; break;
                case 9:
                    ORDState = "已评价"; break;
                case 10:
                    ORDState = "申请退订"; break;
                case 11:
                    ORDState = "退订成功"; break;
                case 12:
                    ORDState = "退订失败"; break;
                default:
                    ORDState = string.Empty; break;
            }
            ViewBag.Stade = ORDState;
            ViewBag.HTCode = HTCode;
            return View();
        }
        //修改为特殊订单
        public JsonResult Save(string htCode, string state, int reason, string remark, string text)
        {
            AdminUser adminUser = Session[ConstantHelper.CurrentAdminUser] as AdminUser;
            var SpecialOrderOperatorName = adminUser.Name;
            var SpecialOrderOperatorMUDID = adminUser.Email;
            var ordState = ChangeState(state);
            var res = orderService.SaveChange(htCode, text, remark, SpecialOrderOperatorName, SpecialOrderOperatorMUDID, reason, ordState);
            if (res == 1)
            {
                if (text == "呼叫中心操作退单")
                {
                    var cont = "呼叫中心操作退单：" + htCode;
                    var num = operationAuditService.AddAudit("1", cont);
                }
                else
                {
                    var cont = "会议文件丢失：" + htCode;
                    var num = operationAuditService.AddAudit("2", cont);
                }
                return Json(new { state = 1 });
            }
            return Json(new { state = 0, txt = "操作失败，请重试！" });


        }
        #endregion

        public int ChangeState(string orderState)
        {
            int state = 0;
            switch (orderState)
            {
                case "订单待审批":
                    state = 1;
                    break;
                case "订单审批被驳回":
                    state = 2;
                    break;
                case "订单提交成功":
                    state = 3;
                    break;
                case "预订成功":
                    state = 4;
                    break;
                case "预订失败":
                    state = 5;
                    break;
                case "已收餐":
                    state = 6;
                    break;
                case "系统已收餐":
                    state = 7;
                    break;
                case "未送达":
                    state = 8;
                    break;
                case "已评价":
                    state = 9;
                    break;
                case "申请退订":
                    state = 10;
                    break;
                case "退订成功":
                    state = 11;
                    break;
                case "退订失败":
                    state = 12;
                    break;
            }
            return state;
        }


        #region ------------订单查询

        [AdminSessionFilter(Order = 1)]
        [PermissionFilter(Order = 2, Permission = "00000000-0000-0000-2000-000000000001")]
        public ActionResult SearchList()
        {
            return View();
        }

        [HttpPost]
        //[AdminSessionFilter(Order = 1)]
        //[PermissionFilter(Order = 2, Permission = "00000000-0000-0000-1000-000000000001")]
        public JsonResult LoadSearchList(string CN, string MUDID, string TACode, string HospitalCode, string RestaurantId, string CostCenter, string CreateTimeBegin, string CreateTimeEnd, string DeliverTimeBegin, string DeliverTimeEnd, string State, string Supplier, string IsSpecialOrder, string RD, int rows, int page)
        {
            LogHelper.Info(Request.RawUrl);
            //var res = WxMessageClientChannelFactory.GetChannel().SendText("zhongda.fang", "今天天不错啊！" + CN);
            //LogHelper.Info(res);
            DateTime _tmpTime;
            DateTime? _DTBegin, _DTEnd;
            if (DateTime.TryParse(CreateTimeBegin, out _tmpTime) == true)
            {
                _DTBegin = _tmpTime;
            }
            else
            {
                _DTBegin = null;
            }
            if (DateTime.TryParse(CreateTimeEnd, out _tmpTime) == true)
            {
                _DTEnd = _tmpTime.AddDays(1d);
            }
            else
            {
                _DTEnd = null;
            }
            DateTime tmpTime;
            DateTime? DTBegin, DTEnd;
            if (DateTime.TryParse(DeliverTimeBegin, out tmpTime) == true)
            {
                DTBegin = tmpTime;
            }
            else
            {
                DTBegin = null;
            }
            if (DateTime.TryParse(DeliverTimeEnd, out tmpTime) == true)
            {
                DTEnd = tmpTime.AddDays(1d);
            }
            else
            {
                DTEnd = null;
            }
            int tmpInt;
            int? stateFlg;
            if (int.TryParse(State, out tmpInt) == false)
            {
                stateFlg = null;
            }
            else
            {
                stateFlg = tmpInt;
            }
            int total;
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
            var list = orderService.LoadOrderReportPage(CN, MUDID, TACode, HospitalCode, RestaurantId, CostCenter, _DTBegin, _DTEnd, DTBegin, DTEnd, stateFlg, Supplier, IsSpecialOrder, RD, rows, page, out total);
            var rtnList = new List<HT_ORDER_REPORT_VIEW_EXT>();
            foreach (var i in list)
            {
                rtnList.Add(GetDisplayObj(i));
            }
            return Json(new { state = 1, rows = rtnList, total = total });
        }

        /// <summary>
        /// 订单列表
        /// </summary>
        /// <returns></returns>
        [AdminSessionFilter(Order = 1)]
        [PermissionFilter(Order = 2, Permission = "00000000-0000-0000-9000-000000000001")]
        public ActionResult NonSearchList()
        {
            return View();
        }

        /// <summary>
        /// 审批列表
        /// </summary>
        /// <returns></returns>
        [AdminSessionFilter(Order = 1)]
        [PermissionFilter(Order = 2, Permission = "00000000-0000-0000-9000-000000000002")]
        public ActionResult NonApproveList()
        {
            return View();
        }

        /// <summary>
        /// 评价列表
        /// </summary>
        /// <returns></returns>
        [AdminSessionFilter(Order = 1)]
        [PermissionFilter(Order = 2, Permission = "00000000-0000-0000-9000-000000000003")]
        public ActionResult NonEvaluateList()
        {
            return View();
        }

        //[AdminSessionFilter(Order = 1)]
        //[PermissionFilter(Order = 2, Permission = "00000000-0000-0000-1000-000000000001")]
        public void ExportSearchList(string CN, string MUDID, string TACode, string HospitalCode, string RestaurantId, string CostCenter, string CreateTimeBegin, string CreateTimeEnd, string DeliverTimeBegin, string DeliverTimeEnd, string State, string Supplier, string IsSpecialOrder, string RD)
        {
            try
            {
                LogHelper.Info(Request.RawUrl);
                //var res = WxMessageClientChannelFactory.GetChannel().SendText("zhongda.fang", "今天天不错啊！" + CN);
                //LogHelper.Info(res);
                DateTime _tmpTime;
                DateTime? _DTBegin, _DTEnd;
                if (DateTime.TryParse(CreateTimeBegin, out _tmpTime) == true)
                {
                    _DTBegin = _tmpTime;
                }
                else
                {
                    _DTBegin = null;
                }
                if (DateTime.TryParse(CreateTimeEnd, out _tmpTime) == true)
                {
                    _DTEnd = _tmpTime.AddDays(1d);
                }
                else
                {
                    _DTEnd = null;
                }
                DateTime tmpTime;
                DateTime? DTBegin, DTEnd;
                if (DateTime.TryParse(DeliverTimeBegin, out tmpTime) == true)
                {
                    DTBegin = tmpTime;
                }
                else
                {
                    DTBegin = null;
                }
                if (DateTime.TryParse(DeliverTimeEnd, out tmpTime) == true)
                {
                    DTEnd = tmpTime.AddDays(1d);
                }
                else
                {
                    DTEnd = null;
                }
                int tmpInt;
                int? stateFlg;
                if (int.TryParse(State, out tmpInt) == false)
                {
                    stateFlg = null;
                }
                else
                {
                    stateFlg = tmpInt;
                }
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
                var list = orderService.LoadOrderReport(CN, MUDID, TACode, HospitalCode, RestaurantId, CostCenter, _DTBegin, _DTEnd, DTBegin, DTEnd, stateFlg, Supplier, IsSpecialOrder, RD);
                if (list != null && list.Count > 0)
                {
                    FileStream file11 = new FileStream(Server.MapPath("~/Template/Template_HT_PlatFormOrderReports.xls"), FileMode.Open, FileAccess.Read);
                    //FileStream file11 = new FileStream(Server.MapPath("/Template/Template_HT_PlatFormOrderReports.xls"), FileMode.Open, FileAccess.Read);
                    HSSFWorkbook wk = new HSSFWorkbook(file11);

                    ISheet sheet = wk.GetSheet("Report");

                    HT_ORDER_REPORT_VIEW_EXT disItm;
                    int dataCnt = list.Count;
                    for (int i = 1; i <= dataCnt; i++)
                    {
                        IRow row = sheet.CreateRow(i);
                        ICell cell = null;

                        disItm = GetDisplayObj(list[i - 1]);
                        row = sheet.CreateRow(i);
                        #region data cell
                        var a = 0;
                        cell = row.CreateCell(a);                   // 申请人姓名
                        cell.SetCellValue(disItm.ApplierName);
                        cell = row.CreateCell(++a);             //申请人MUDID
                        cell.SetCellValue(disItm.ApplierMUDID);
                        cell = row.CreateCell(++a);             //Territory Code
                        cell.SetCellValue(disItm.MRTerritoryCode);
                        cell = row.CreateCell(++a);            //申请人职位
                        cell.SetCellValue(disItm.Position);
                        cell = row.CreateCell(++a);             //申请人手机号码
                        cell.SetCellValue(disItm.ApplierMobile);
                        cell = row.CreateCell(++a);             //预申请申请日期
                        cell.SetCellValue(disItm.PRECreateDate);
                        cell = row.CreateCell(++a);             //预申请申请时间
                        cell.SetCellValue(disItm.PRECreateTime);
                        cell = row.CreateCell(++a);             //预申请修改日期
                        cell.SetCellValue(disItm.PREModifyDate);
                        cell = row.CreateCell(++a);             //预申请修改时间
                        cell.SetCellValue(disItm.PREModifyTime);
                        cell = row.CreateCell(++a);             //HT编号
                        cell.SetCellValue(disItm.HTCode);
                        cell = row.CreateCell(++a);             //Market
                        cell.SetCellValue(disItm.Market);
                        cell = row.CreateCell(++a);
                        cell.SetCellValue(disItm.VeevaMeetingID);//VeevaMeetingID
                        cell = row.CreateCell(++a);             //TA
                        cell.SetCellValue(disItm.TA);

                        cell = row.CreateCell(++a);             //省份
                        cell.SetCellValue(disItm.Province);
                        cell = row.CreateCell(++a);             //城市
                        cell.SetCellValue(disItm.City);
                        cell = row.CreateCell(++a);             //医院编码
                        cell.SetCellValue(disItm.HospitalCode);
                        cell = row.CreateCell(++a);             //医院名称
                        cell.SetCellValue(disItm.HospitalName);
                        cell = row.CreateCell(++a);             //医院地址
                        cell.SetCellValue(disItm.HospitalAddress);
                        cell = row.CreateCell(++a);             //会议名称
                        cell.SetCellValue(disItm.MeetingName);
                        cell = row.CreateCell(++a);             //会议日期
                        cell.SetCellValue(disItm.MeetingDate);
                        cell = row.CreateCell(++a);             //会议时间
                        cell.SetCellValue(disItm.MeetingTime);
                        cell = row.CreateCell(++a);             //参会人数
                        cell.SetCellValue(disItm.PREAttendCount);
                        cell = row.CreateCell(++a);             //成本中心
                        cell.SetCellValue(disItm.CostCenter);

                        cell = row.CreateCell(++a);             //预算金额
                        cell.SetCellValue(disItm.BudgetTotal);
                        cell = row.CreateCell(++a);             //直线经理是否随访
                        cell.SetCellValue(disItm.IsDMFollow);
                        cell = row.CreateCell(++a);             //外部免费讲者来讲
                        cell.SetCellValue(disItm.IsFreeSpeaker);
                        //cell = row.CreateCell(++a);             //RDSD Name
                        //cell.SetCellValue(disItm.RDSDName);
                        //cell = row.CreateCell(++a);             //RDSD MUDID
                        //cell.SetCellValue(disItm.RDSDMUDID);
                        //20190122
                        //if (Convert.ToInt32(disItm.PREAttendCount) < 60 && Convert.ToDouble(disItm.BudgetTotal) < 1200)
                        //{
                        //    cell = row.CreateCell(++a);             //预申请审批人姓名
                        //    cell.SetCellValue("系统自动审批");
                        //    cell = row.CreateCell(++a);             //预申请审批人MUDID
                        //    cell.SetCellValue("系统自动审批");
                        //}
                        //    //else if (Convert.ToDouble(disItm.BudgetTotal) < 2000 && disItm.CurrentApproverMUDID == null)
                        //    //{
                        //    //    cell = row.CreateCell(++a);
                        //    //    cell.SetCellValue("系统自动审批");// "预申请审批人姓名");
                        //    //    cell = row.CreateCell(++a);
                        //    //    cell.SetCellValue("系统自动审批");// "预申请审批人MUDID");
                        //    //}
                        //    else if (Convert.ToInt32(disItm.PREAttendCount) >= 1200 && Convert.ToInt32(disItm.PREAttendCount) < 1500)
                        //    {

                        //        cell = row.CreateCell(++a);             //预申请审批人姓名
                        //        cell.SetCellValue(disItm.CurrentApproverName);
                        //        cell = row.CreateCell(++a);             //预申请审批人MUDID
                        //        cell.SetCellValue(disItm.CurrentApproverMUDID);
                        //    }
                        //    else if (Convert.ToInt32(disItm.PREAttendCount) > 1500 && disItm.PREState == "5")
                        //    {
                        //        cell = row.CreateCell(++a);             //预申请审批人姓名
                        //        cell.SetCellValue(disItm.CurrentApproverName);
                        //        cell = row.CreateCell(++a);             //预申请审批人MUDID
                        //        cell.SetCellValue(disItm.CurrentApproverMUDID);
                        //    }
                        //    else
                        //{
                        //    cell = row.CreateCell(++a);             //预申请审批人姓名
                        //    cell.SetCellValue(disItm.PREBUHeadName);
                        //    cell = row.CreateCell(++a);             //预申请审批人MUDID
                        //    cell.SetCellValue(disItm.PREBUHeadMUDID);
                        //}
                        cell = row.CreateCell(++a);             //RD Territory Code
                        cell.SetCellValue(disItm.RDTerritoryCode);
                        cell = row.CreateCell(++a);             //预申请审批人姓名
                        cell.SetCellValue(disItm.PREBUHeadName1);
                        cell = row.CreateCell(++a);             //预申请审批人MUDID
                        cell.SetCellValue(disItm.PREBUHeadMUDID1);
                        cell = row.CreateCell(++a);             //预申请审批日期
                        cell.SetCellValue(disItm.PREBUHeadApproveDate);
                        cell = row.CreateCell(++a);             //预申请审批时间
                        cell.SetCellValue(disItm.PREBUHeadApproveTime);
                        cell = row.CreateCell(++a);             //预申请审批状态
                        cell.SetCellValue(disItm.PREState);
                        cell = row.CreateCell(++a);             //预申请是否重新分配审批人
                        cell.SetCellValue(disItm.PREIsReAssign);
                        cell = row.CreateCell(++a);             //预申请重新分配审批人-操作人
                        cell.SetCellValue(disItm.PREReAssignOperatorName);
                        cell = row.CreateCell(++a);             //预申请重新分配审批人-操作人MUDID
                        cell.SetCellValue(disItm.PREReAssignOperatorMUDID);
                        cell = row.CreateCell(++a);             //预申请被重新分配审批人姓名
                        cell.SetCellValue(disItm.PREReAssignBUHeadName);
                        cell = row.CreateCell(++a);             //预申请被重新分配审批人MUDID
                        cell.SetCellValue(disItm.PREReAssignBUHeadMUDID);
                        cell = row.CreateCell(++a);             //预申请重新分配审批人日期
                        cell.SetCellValue(disItm.PREReAssignBUHeadApproveDate);
                        cell = row.CreateCell(++a);             //预申请重新分配审批人时间
                        cell.SetCellValue(disItm.PREReAssignBUHeadApproveTime);
                        cell = row.CreateCell(++a);             //供应商
                        cell.SetCellValue(disItm.Channel);
                        cell = row.CreateCell(++a);             //订单号
                        cell.SetCellValue(disItm.EnterpriseOrderId);
                        cell = row.CreateCell(++a);             //下单日期
                        cell.SetCellValue(disItm.ORDCreateDate);
                        cell = row.CreateCell(++a);             //下单时间
                        cell.SetCellValue(disItm.ORDCreateTime);
                        cell = row.CreateCell(++a);             //送餐日期
                        cell.SetCellValue(disItm.ORDDeliverDate);

                        cell = row.CreateCell(++a);             //送餐时间
                        cell.SetCellValue(disItm.ORDDeliverTime);
                        cell = row.CreateCell(++a);             //餐厅编码
                        cell.SetCellValue(disItm.RestaurantId);
                        cell = row.CreateCell(++a);             //餐厅名称
                        cell.SetCellValue(disItm.RestaurantName);
                        cell = row.CreateCell(++a);             //用餐人数
                        cell.SetCellValue(disItm.ORDAttendCount);
                        cell = row.CreateCell(++a);             //预订金额
                        cell.SetCellValue(disItm.TotalPrice);
                        cell = row.CreateCell(++a);             //实际金额
                        cell.SetCellValue(disItm.totalFee);
                        cell = row.CreateCell(++a);             //实际金额调整原因
                        cell.SetCellValue(disItm.feeModifyReason);
                        cell = row.CreateCell(++a);             //送餐地址
                        cell.SetCellValue(disItm.DeliveryAddress);
                        cell = row.CreateCell(++a);             //收餐人姓名
                        cell.SetCellValue(disItm.Consignee);

                        cell = row.CreateCell(++a);             //收餐人电话
                        cell.SetCellValue(disItm.Phone);
                        cell = row.CreateCell(++a);             //下单备注
                        cell.SetCellValue(disItm.Remark);
                        cell = row.CreateCell(++a);             //是否预定成功
                        cell.SetCellValue(disItm.bookState);
                        cell = row.CreateCell(++a);             //预定成功日期
                        cell.SetCellValue(disItm.ORDReceiveDate);
                        cell = row.CreateCell(++a);             //预定成功时间
                        cell.SetCellValue(disItm.ORDReceiveTime);
                        cell = row.CreateCell(++a);             //是否申请退单
                        cell.SetCellValue(disItm.IsRetuen);
                        cell = row.CreateCell(++a);             //是否退单成功
                        cell.SetCellValue(disItm.cancelState);
                        cell = row.CreateCell(++a);             //退单失败平台发起配送需求
                        cell.SetCellValue(disItm.IsRetuenSuccess);
                        cell = row.CreateCell(++a);             //退单失败反馈配送需求
                        cell.SetCellValue(disItm.cancelFeedback);
                        cell = row.CreateCell(++a);             //预定/退单失败原因
                        cell.SetCellValue(disItm.cancelFailReason);

                        cell = row.CreateCell(++a);             //是否收餐/未送达
                        cell.SetCellValue(disItm.ReceiveState);
                        cell = row.CreateCell(++a);             //确认收餐日期
                        cell.SetCellValue(disItm.ReceiveDate);
                        cell = row.CreateCell(++a);             //确认收餐时间
                        cell.SetCellValue(disItm.ReceiveTime);
                        cell = row.CreateCell(++a);             //用户确认金额
                        cell.SetCellValue(disItm.RealPrice);
                        cell = row.CreateCell(++a);             //是否与预定餐品一致
                        cell.SetCellValue(disItm.IsMealSame);
                        cell = row.CreateCell(++a);             //用户确认金额调整原因
                        cell.SetCellValue(disItm.RealPriceChangeReason);
                        cell = row.CreateCell(++a);             //用户确认金额调整备注
                        cell.SetCellValue(disItm.RealPriceChangeRemark);
                        cell = row.CreateCell(++a);             //实际用餐人数
                        cell.SetCellValue(disItm.RealCount);
                        cell = row.CreateCell(++a);             //实际用餐人数调整原因
                        cell.SetCellValue(disItm.RealCountChangeReason);
                        cell = row.CreateCell(++a);             //实际用餐人数调整备注
                        cell.SetCellValue(disItm.RealCountChangeRemrak);
                        cell = row.CreateCell(++a);             //未送达描述
                        cell.SetCellValue(disItm.EUnTimeDesc);

                        cell = row.CreateCell(++a);             //准点率
                        cell.SetCellValue(disItm.EOnTime);
                        cell = row.CreateCell(++a);             //准点率描述
                        cell.SetCellValue(disItm.EOnTimeDesc);
                        cell = row.CreateCell(++a);             //食品安全存在问题
                        cell.SetCellValue(disItm.EIsSafe);
                        cell = row.CreateCell(++a);             //食品安全问题描述
                        cell.SetCellValue(disItm.EIsSafeDesc);
                        cell = row.CreateCell(++a);             //餐品卫生及新鲜
                        cell.SetCellValue(disItm.EHealth);
                        cell = row.CreateCell(++a);             //餐品卫生描述
                        cell.SetCellValue(disItm.EHealthDesc);
                        cell = row.CreateCell(++a);             //餐品包装
                        cell.SetCellValue(disItm.EPack);
                        cell = row.CreateCell(++a);             //餐品包装描述
                        cell.SetCellValue(disItm.EPackDesc);
                        cell = row.CreateCell(++a);             //餐品性价比
                        cell.SetCellValue(disItm.ECost);
                        cell = row.CreateCell(++a);             //餐品性价比描述
                        cell.SetCellValue(disItm.ECostDesc);

                        cell = row.CreateCell(++a);             //其他评价
                        cell.SetCellValue(disItm.EOtherDesc);
                        cell = row.CreateCell(++a);             //在线评分
                        cell.SetCellValue(disItm.EStar);
                        cell = row.CreateCell(++a);             //评论日期
                        cell.SetCellValue(disItm.ECreateDate);
                        cell = row.CreateCell(++a);             //评论时间
                        cell.SetCellValue(disItm.ECreateTime);
                        cell = row.CreateCell(++a);             //1=同一医院当日多场
                        cell.SetCellValue(disItm.TYYYDRDC);
                        cell = row.CreateCell(++a);             //2=同一代表当日多场
                        cell.SetCellValue(disItm.TYDBDRDC);
                        cell = row.CreateCell(++a);             //3=同一餐厅当日多场
                        cell.SetCellValue(disItm.TYCTDRDC);
                        cell = row.CreateCell(++a);             //4=同一代表同一医院当日多场
                        cell.SetCellValue(disItm.TYDBTYYYDRDC);
                        cell = row.CreateCell(++a);             //5=同一代表同一餐厅当日多场
                        cell.SetCellValue(disItm.TYDBTYCTDRDC);
                        cell = row.CreateCell(++a);             //6=同一代表同一医院同一餐厅当日多场
                        cell.SetCellValue(disItm.TYDBTYYYTYCTDRDC);

                        cell = row.CreateCell(++a);             //7=参会人数>=60
                        cell.SetCellValue(disItm.CHRSDYLS);
                        cell = row.CreateCell(++a);             //8=代表自提
                        cell.SetCellValue(disItm.customerPickup);
                        //cell = row.CreateCell(++a);             //直线经理姓名
                        //cell.SetCellValue(disItm.PREBUHeadName);
                        //cell = row.CreateCell(++a);             //直线经理MUDID
                        //cell.SetCellValue(disItm.PREBUHeadMUDID);
                        //cell = row.CreateCell(++a);             //Level2 Manager 姓名
                        //cell.SetCellValue(disItm.PREBUHeadName);
                        //cell = row.CreateCell(++a);             //Level2 Manager MUDID
                        //cell.SetCellValue(disItm.PREBUHeadMUDID);
                        //cell = row.CreateCell(++a);             //Level3 Manager 姓名
                        //cell.SetCellValue(disItm.PREBUHeadName);
                        //cell = row.CreateCell(++a);             //Level3 Manager MUDID
                        //cell.SetCellValue(disItm.PREBUHeadMUDID);
                        cell = row.CreateCell(++a);             //是否上传文件
                        cell.SetCellValue(disItm.IsOrderUpload);

                        cell = row.CreateCell(++a);             //上传文件提交日期
                        cell.SetCellValue(disItm.PUOCreateDate);
                        cell = row.CreateCell(++a);             //上传文件提交时间
                        cell.SetCellValue(disItm.PUOCreateTime);
                        cell = row.CreateCell(++a);             //上传文件审批直线经理姓名
                        cell.SetCellValue(disItm.PUOBUHeadName);
                        cell = row.CreateCell(++a);             //上传文件审批直线经理MUDID
                        cell.SetCellValue(disItm.PUOBUHeadMUDID);
                        cell = row.CreateCell(++a);             //上传文件审批日期
                        cell.SetCellValue(disItm.ApproveDate);
                        cell = row.CreateCell(++a);             //上传文件审批时间
                        cell.SetCellValue(disItm.ApproveTime);
                        cell = row.CreateCell(++a);             //上传文件审批状态
                        cell.SetCellValue(disItm.PUOState);
                        cell = row.CreateCell(++a);             //签到人数是否等于实际用餐人数
                        cell.SetCellValue(disItm.IsAttentSame);
                        //cell = row.CreateCell(++a);               
                        //cell.SetCellValue(disItm.RealCount);
                        //cell.CellStyle = dataCellStyle;
                        cell = row.CreateCell(++a);             //签到人数调整原因
                        cell.SetCellValue(disItm.AttentSameReason);
                        cell = row.CreateCell(++a);             //是否与会议信息一致
                        cell.SetCellValue(disItm.IsMeetingInfoSame);
                        cell = row.CreateCell(++a);             //会议信息不一致原因
                        cell.SetCellValue(disItm.MeetingInfoSameReason);
                        cell = row.CreateCell(++a);             //上传文件是否Re-Open
                        cell.SetCellValue(disItm.IsReopen);
                        cell = row.CreateCell(++a);             //上传文件Re-Open操作人
                        cell.SetCellValue(disItm.ReopenOperatorName);
                        cell = row.CreateCell(++a);             //上传文件Re-Open操作人MUDID
                        cell.SetCellValue(disItm.ReopenOperatorMUDID);
                        cell = row.CreateCell(++a);             //上传文件Re-Open日期
                        cell.SetCellValue(disItm.ReopenOperateDate);
                        cell = row.CreateCell(++a);             //上传文件Re-Open时间
                        cell.SetCellValue(disItm.ReopenOperateTime);
                        cell = row.CreateCell(++a);             //上传文件Re-Open发起人姓名
                        cell.SetCellValue(disItm.ReopenOperatorName);
                        cell = row.CreateCell(++a);             //上传文件Re-Open发起人MUDID
                        cell.SetCellValue(disItm.ReopenOperatorMUDID);
                        cell = row.CreateCell(++a);             //上传文件Re-Open原因
                        cell.SetCellValue(disItm.ReopenReason);
                        cell = row.CreateCell(++a);             //上传文件Re-Open备注 
                        cell.SetCellValue(disItm.ReopenRemark);
                        cell = row.CreateCell(++a);             //上传文件Re-Open审批状态
                        cell.SetCellValue(disItm.ReopenState);
                        cell = row.CreateCell(++a);             //上传文件是否重新分配
                        cell.SetCellValue(disItm.IsTransfer);
                        cell = row.CreateCell(++a);             //上传文件重新分配操作人
                        cell.SetCellValue(disItm.TransferOperatorName);
                        cell = row.CreateCell(++a);             //上传文件重新分配操作人MUDID
                        cell.SetCellValue(disItm.TransferOperatorMUDID);

                        cell = row.CreateCell(++a);             //上传文件被重新分配人姓名
                        cell.SetCellValue(disItm.TransferUserName);
                        cell = row.CreateCell(++a);             //上传文件被重新分配人MUDID
                        cell.SetCellValue(disItm.TransferUserMUDID);
                        cell = row.CreateCell(++a);             //上传文件被重新分配日期
                        cell.SetCellValue(disItm.TransferOperateDate);
                        cell = row.CreateCell(++a);             //上传文件被重新分配时间
                        cell.SetCellValue(disItm.TransferOperateTime);
                        cell = row.CreateCell(++a);             //上传文件否重新分配审批人
                        cell.SetCellValue(disItm.IsReAssign);
                        cell = row.CreateCell(++a);             //上传文件重新分配审批人-操作人
                        cell.SetCellValue(disItm.ReAssignOperatorName);
                        cell = row.CreateCell(++a);             //上传文件重新分配审批人-操作人MUDID
                        cell.SetCellValue(disItm.ReAssignOperatorMUDID);
                        cell = row.CreateCell(++a);             //上传文件被重新分配审批人姓名
                        cell.SetCellValue(disItm.ReAssignBUHeadName);
                        cell = row.CreateCell(++a);             //上传文件被重新分配审批人MUDID
                        cell.SetCellValue(disItm.ReAssignBUHeadMUDID);
                        cell = row.CreateCell(++a);             //上传文件重新分配审批人日期
                        cell.SetCellValue(disItm.ReAssignBUHeadApproveDate);

                        cell = row.CreateCell(++a);             //上传文件重新分配审批人时间
                        cell.SetCellValue(disItm.ReAssignBUHeadApproveTime);
                        cell = row.CreateCell(++a);             //项目组特殊备注
                        cell.SetCellValue(disItm.SpecialReason);


                        cell = row.CreateCell(++a);
                        cell.SetCellValue(disItm.SupplierSpecialRemark);//供应商特殊备注
                        cell = row.CreateCell(++a);
                        cell.SetCellValue(disItm.IsCompleteDelivery);//是否完成送餐
                        cell = row.CreateCell(++a);
                        cell.SetCellValue(disItm.SupplierConfirmAmount);//与供应商确认订单金额
                        cell = row.CreateCell(++a);
                        cell.SetCellValue(disItm.GSKConfirmAmount);//GSK项目组确认金额
                        cell = row.CreateCell(++a);
                        cell.SetCellValue(disItm.GSKConAAReason);//GSK项目组确认金额调整原因

                        cell = row.CreateCell(++a);
                        cell.SetCellValue(string.Empty);

                        cell = row.CreateCell(++a);
                        cell.SetCellValue(disItm.MealPaymentAmount);//餐费付款金额
                        cell = row.CreateCell(++a);
                        cell.SetCellValue(disItm.MealPaymentPO);//餐费付款PO号码
                        cell = row.CreateCell(++a);
                        cell.SetCellValue(disItm.AccountingTime);//到账时间
                        cell = row.CreateCell(++a);
                        cell.SetCellValue(string.Empty);
                        cell = row.CreateCell(++a);
                        cell.SetCellValue(string.Empty);
                        cell = row.CreateCell(++a);
                        cell.SetCellValue(string.Empty);
                        cell = row.CreateCell(++a);
                        cell.SetCellValue(string.Empty);
                        #endregion
                    }
                    using (System.IO.MemoryStream _ms = new System.IO.MemoryStream())
                    {
                        wk.Write(_ms);
                        Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xls", DateTime.Now.ToString("yyyyMMddHHmmssfff")));
                        Response.ContentType = "application/vnd.ms-excel";
                        Response.BinaryWrite(_ms.GetBuffer());
                        Response.Charset = "";
                        Response.End();
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.Error("导出订单报表异常，如下信息：");
                LogHelper.Error(e.Message);
            }
        }

        #region 导入HT数据
        /// <summary>
        /// 导入HT数据
        /// </summary>
        /// <returns></returns>
        public JsonResult Import(HttpPostedFileBase file)
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

            var titleTemplate = "HT编号,订单号,供应商特殊备注,是否完成送餐,与供应商确认订单金额,GSK项目组确认金额,GSK项目组确认金额调整原因,餐费付款金额,餐费付款PO号码,到账时间".Split(',');
            var titleValues = new string[10];

            for (var i = 0; i < 10; i++)
            {
                titleValues[i] = row.GetCell(0) != null ? row.GetCell(i).StringCellValue : string.Empty;
                if (titleValues[i] != titleTemplate[i])
                {
                    return Json(new { state = 0, txt = "导入的文件格式不正确" }, "text/html", JsonRequestBehavior.AllowGet);
                }
            }
            #endregion           

            #region 读取表体
            var excelRows = new List<HT_ORDER_REPORT_VIEW_EXT>();
            string Failtxt = "";
            string strHTCode = "";
            string subHTCode = "";
            var sRows = new List<HT_ORDER_REPORT_VIEW>();
            string sql = "";
            string reportsql = "";
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                row = sheet.GetRow(i);
                if (row == null) continue;
                string celltxt = GetStringFromCell(row.GetCell(0)) + GetStringFromCell(row.GetCell(1)) + GetStringFromCell(row.GetCell(2)) + GetStringFromCell(row.GetCell(3)) + GetStringFromCell(row.GetCell(4))
                    + GetStringFromCell(row.GetCell(5)) + GetStringFromCell(row.GetCell(6)) + GetStringFromCell(row.GetCell(7)) + GetStringFromCell(row.GetCell(8)) + GetStringFromCell(row.GetCell(9));
                if (string.IsNullOrEmpty(celltxt)) break;
                if (string.IsNullOrEmpty(GetStringFromCell(row.GetCell(0))) || string.IsNullOrEmpty(GetStringFromCell(row.GetCell(1))))
                {
                    return Json(new { state = 0, txt = "发现有空值的列" + "</br>" + "请检查：HT编号或订单号" }, "text/html", JsonRequestBehavior.AllowGet);
                }
                string st = "";
                if (row.GetCell(9).ToString().Contains("月"))
                {
                    st = row.GetCell(9).ToString().Replace("月", "");
                }
                else
                {
                    st = row.GetCell(9).ToString();
                }
                //string strDate = string.Format("{0:yyyy-MM-dd HH:mm:ss}", st);
                //获取Excel中HTCode拼接字符串
                strHTCode += "'" + GetStringFromCell(row.GetCell(0)) + "',";
                excelRows.Add(new HT_ORDER_REPORT_VIEW_EXT()
                {
                    HTCode = GetStringFromCell(row.GetCell(0)),
                    EnterpriseOrderId = GetStringFromCell(row.GetCell(1)),
                    SupplierSpecialRemark = GetStringFromCell(row.GetCell(2)),
                    IsCompleteDelivery = GetStringFromCell(row.GetCell(3)),
                    SupplierConfirmAmount = GetStringFromCell(row.GetCell(4)),
                    GSKConfirmAmount = GetStringFromCell(row.GetCell(5)),
                    GSKConAAReason = GetStringFromCell(row.GetCell(6)),
                    MealPaymentAmount = GetStringFromCell(row.GetCell(7)),
                    MealPaymentPO = GetStringFromCell(row.GetCell(8)),
                    //AccountingTime = GetStringFromCell(row.GetCell(9))
                    AccountingTime = st
                });
            }
            //截取掉最后一个","得到的字符串
            subHTCode = strHTCode.Substring(0, strHTCode.Length - 1);
            //查询符合[P_Order]表中的HTCode
            var inHTCode = orderService.LoadDataByInHTCode(subHTCode);
            if (inHTCode != null && inHTCode.Count > 0)
            {

                foreach (var s in excelRows)
                {
                    var inData = inHTCode.Where(p => p.CN.Trim() == s.HTCode && p.EnterpriseOrderId.Trim() == s.EnterpriseOrderId);
                    if (inData == null || inData.Count() == 0)
                    {
                        Failtxt += s.HTCode + "/" + s.EnterpriseOrderId + "校验失败，请检查该项" + "</br>";
                    }
                    else
                    {
                        //组sql
                        sql += "update [P_ORDER] set ActionState='0',SupplierSpecialRemark= '" + s.SupplierSpecialRemark.Trim() + "' ,IsCompleteDelivery= '" + s.IsCompleteDelivery.Trim() + "'";
                        if (s.SupplierConfirmAmount == "")
                        {
                            sql += " ,SupplierConfirmAmount=NULL";
                        }
                        else
                        {
                            sql += " ,SupplierConfirmAmount=" + decimal.Parse(s.SupplierConfirmAmount);
                        }
                        if (s.GSKConfirmAmount == "")
                        {
                            sql += " ,GSKConfirmAmount=NULL";
                        }
                        else
                        {
                            sql += " ,GSKConfirmAmount=" + decimal.Parse(s.GSKConfirmAmount);
                        }
                        sql += " ,GSKConAAReason= '" + s.GSKConAAReason.Trim() + "'";
                        if (s.MealPaymentAmount == "")
                        {
                            sql += " ,MealPaymentAmount=NULL";
                        }
                        else
                        {
                            sql += " ,MealPaymentAmount=" + decimal.Parse(s.MealPaymentAmount);
                        }
                        sql += " ,MealPaymentPO= '" + s.MealPaymentPO.Trim() + "' ,AccountingTime= '" + s.AccountingTime.Trim() + "' where "
                        + " CN='" + s.HTCode.Trim() + "' and EnterpriseOrderId='" + s.EnterpriseOrderId.Trim() + "'      ";
                        sRows.Add(new HT_ORDER_REPORT_VIEW()
                        {
                            HTCode = s.HTCode
                        });
                        var reportData = orderService.LoadReportByHTOrder(s.HTCode, s.EnterpriseOrderId);
                        if (reportData != null && reportData.Count > 0)
                        {
                            //组reportsql
                            reportsql += "update [P_HT_Order_Report] set SupplierSpecialRemark= '" + s.SupplierSpecialRemark.Trim() + "' ,IsCompleteDelivery= '" + s.IsCompleteDelivery.Trim() + "'";
                            if (s.SupplierConfirmAmount == "")
                            {
                                reportsql += " ,SupplierConfirmAmount=NULL";
                            }
                            else
                            {
                                reportsql += " ,SupplierConfirmAmount=" + decimal.Parse(s.SupplierConfirmAmount);
                            }
                            if (s.GSKConfirmAmount == "")
                            {
                                reportsql += " ,GSKConfirmAmount=NULL";
                            }
                            else
                            {
                                reportsql += " ,GSKConfirmAmount=" + decimal.Parse(s.GSKConfirmAmount);
                            }
                            reportsql += " ,GSKConAAReason= '" + s.GSKConAAReason.Trim() + "'";
                            if (s.MealPaymentAmount == "")
                            {
                                reportsql += " ,MealPaymentAmount=NULL";
                            }
                            else
                            {
                                reportsql += " ,MealPaymentAmount=" + decimal.Parse(s.MealPaymentAmount);
                            }
                            reportsql += " ,MealPaymentPO= '" + s.MealPaymentPO.Trim() + "' ,AccountingTime= '" + s.AccountingTime.Trim() + "' where "
                            + " HTCode='" + s.HTCode.Trim() + "' and xmsOrderId='" + s.EnterpriseOrderId.Trim() + "'      ";

                        }
                    }
                }
            }
            else
            {
                //Excel中所有HTCode都不在P_Order表中
                foreach (var s in excelRows)
                {
                    Failtxt += s.HTCode + "/" + s.EnterpriseOrderId + "校验失败，请检查该项" + "</br>";
                }
            }
            if (Failtxt != "")
            {
                return Json(new { state = 0, txt = Failtxt }, "text/html", JsonRequestBehavior.AllowGet);
            }
            #endregion

            // 文件中是否有重复数据
            var listRepeat = sRows.GroupBy(a => a.HTCode).Where(a => a.Count() > 1).Select(a => a.Key).ToList();
            if (listRepeat.Count() > 0)
            {
                return Json(new { state = 0, txt = "Excel中发现HTCode重复数据", data = listRepeat }, "text/html", JsonRequestBehavior.AllowGet);
            }
            if (sql != "")
            {
                orderService.Import(sql);
            }
            if (reportsql != "")
            {
                orderService.ImportReport(reportsql);
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

        /// <summary>
        /// 导出订单列表
        /// </summary>
        /// <param name="CN"></param>
        /// <param name="MUDID"></param>
        /// <param name="CreateTimeBegin"></param>
        /// <param name="CreateTimeEnd"></param>
        /// <param name="DeliverTimeBegin"></param>
        /// <param name="DeliverTimeEnd"></param>
        /// <param name="State"></param>
        /// <param name="isNonHT"></param>
        /// <returns></returns>
        public ActionResult ExportNonSearchList(string CN, string MUDID, string HospitalCode, string RestaurantId, string CreateTimeBegin, string CreateTimeEnd, string DeliverTimeBegin, string DeliverTimeEnd, string State, string Supplier)
        {
            LogHelper.Info(Request.RawUrl);
            //var res = WxMessageClientChannelFactory.GetChannel().SendText("zhongda.fang", "今天天不错啊！" + CN);
            //LogHelper.Info(res);
            DateTime _tmpTime;
            DateTime? _DTBegin, _DTEnd;
            if (DateTime.TryParse(CreateTimeBegin, out _tmpTime) == true)
            {
                _DTBegin = _tmpTime;
            }
            else
            {
                _DTBegin = null;
            }
            if (DateTime.TryParse(CreateTimeEnd, out _tmpTime) == true)
            {
                _DTEnd = _tmpTime.AddDays(1d);
            }
            else
            {
                _DTEnd = null;
            }
            DateTime tmpTime;
            DateTime? DTBegin, DTEnd;
            if (DateTime.TryParse(DeliverTimeBegin, out tmpTime) == true)
            {
                DTBegin = tmpTime;
            }
            else
            {
                DTBegin = null;
            }
            if (DateTime.TryParse(DeliverTimeEnd, out tmpTime) == true)
            {
                DTEnd = tmpTime.AddDays(1d);
            }
            else
            {
                DTEnd = null;
            }
            int tmpInt;
            int? stateFlg;
            if (int.TryParse(State, out tmpInt) == false)
            {
                stateFlg = null;
            }
            else
            {
                stateFlg = tmpInt;
            }
            var list = orderService.LoadNonHTOrderMnt(CN, MUDID, HospitalCode, RestaurantId, _DTBegin, _DTEnd, DTBegin, DTEnd, stateFlg, Supplier, 1);
            if (list != null && list.Count > 0)
            {
                XSSFWorkbook book = new XSSFWorkbook();
                #region var headerStyle = book.CreateCellStyle();
                var headerStyle = book.CreateCellStyle();

                var headerFontStyle = book.CreateFont();
                headerFontStyle.Color = NPOI.HSSF.Util.HSSFColor.White.Index;
                headerFontStyle.Boldweight = short.MaxValue;
                headerFontStyle.FontHeightInPoints = 18;

                headerStyle.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;
                headerStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightOrange.Index;
                headerStyle.Alignment = HorizontalAlignment.Center;
                headerStyle.SetFont(headerFontStyle);
                #endregion
                var sheet = book.CreateSheet("report");
                var row = sheet.CreateRow(0);
                #region header
                var ci = 0;
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);
                sheet.SetColumnWidth(ci++, 20 * 256);


                var ri = 0;
                var cell = row.CreateCell(ri++);
                cell.SetCellValue("订餐人姓名");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("订餐人手机号");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("订餐人MUDID");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("用餐人Market");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("用餐人TA");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("Meeting Code");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("会议名称");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("PO No.");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("WBS");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("供应商");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("订单号");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("下单日期");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("下单时间");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("送餐日期");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("送餐时间");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("餐厅编码");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("预定餐厅");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("参会人数");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("总份数");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("预订金额");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("实际金额");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("金额调整原因");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("省份");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("城市");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("医院编码");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("医院名称");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("医院地址");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("送餐地址");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("收餐人姓名");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("收餐人电话");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("是否预定成功");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("预定成功日期");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("预定成功时间");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("是否收餐/未送达");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("未送达描述");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("准点率");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("准点率描述");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("食品安全存在问题");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("食品安全问题描述");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("餐品卫生及新鲜");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("餐品卫生描述");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("餐品包装");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("餐品包装描述");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("餐品性价比");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("餐品性价比描述");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("其他评价");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("在线评分");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("评论日期");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("评论时间");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("是否申请退单");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("是否退单成功");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("退单失败平台发起配送需求");// ("退单失败平台发起");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("退单失败线下反馈配送需求");// ("退单失败线下反馈");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("预定/退单失败原因");// ("退单失败原因");
                cell.CellStyle = headerStyle;

                ///////
                cell = row.CreateCell(ri++);
                cell.SetCellValue("XMS特殊备注");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("准点率");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("准点率描述");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("食品安全存在问题");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("食品安全问题描述");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("餐品卫生及新鲜");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("餐品卫生描述");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("餐品包装");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("餐品包装描述");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("餐品性价比");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("餐品性价比描述");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("其他评价");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("进线日期");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("进线时间");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("是否完成付款");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("付款金额");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("到帐时间");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("是否完成送餐");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(ri++);
                cell.SetCellValue("最终金额");
                cell.CellStyle = headerStyle;


                #endregion
                #region var dataCellStyle = book.CreateCellStyle();
                var dataCellStyle = book.CreateCellStyle();
                var dataFontStyle = book.CreateFont();
                dataFontStyle.Color = NPOI.HSSF.Util.HSSFColor.Black.Index;
                dataFontStyle.Boldweight = short.MaxValue;
                dataFontStyle.FontHeightInPoints = 16;

                dataCellStyle.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;
                dataCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.White.Index;
                //dataCellStyle.Alignment = HorizontalAlignment.Center;
                dataCellStyle.SetFont(dataFontStyle);
                #endregion
                P_ORDER_DAILY_VIEW_EXT disItm;
                int dataCnt = list.Count;
                for (int i = 0; i < dataCnt; i++)
                {
                    disItm = GetNonHTDisplayObj(list[i]);
                    row = sheet.CreateRow(1 + i);
                    #region data cell
                    var a = 0;
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.DCUserName);// "订餐人姓名");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.DCPhoneNum);//"订餐人手机号");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.MUDID);// "订餐人MUDID");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.Market);// "用餐人Market");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.TA);// "用餐人TA");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.MeetCode);// "POMeeting Code");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.MeetName);// "会议名称");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.PO);// "PO No.");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.WBS);// "WBS");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.Channel);// "供应商");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.XMSOrderID);// "订单号");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.OrderingDate);// "下单日期");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.OrderingTime);// "下单时间");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.SendDate);// "送餐日期");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.SendTime);// "送餐时间");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.RestaurantId);// "餐厅编码");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.RestName);// "预定餐厅");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.UserQuantity);// "参会人数");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.MealQuantity);// "总份数");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.TotalPrice);// "预订金额");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.XMSTotalPrice);// "实际金额");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.ChangePriceReason);// "金额调整原因");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.ProvinceName);// "省份");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.CityName);// "城市");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.GskHospital);// "医院编码");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.HospitalName);// "医院名称");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.HospitalAddr);// "医院地址");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.HospitalRoom);// "送餐地址");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.SCUserName);// "收餐人姓名");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.SCPhoneNum);// "收餐人电话");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.XMSBookState);// "是否预定成功");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.ReceiveDate);// "预订成功日期");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.ReceiveTime);// "预订成功时间");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.ReceiveState);// "是否收餐/未送达");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.EUnTimeDesc);// "未送达描述");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.EOnTime);// "准点率");

                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.EOnTimeDesc);// "准点率描述");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.EIsSafe);// "食品安全存在问题");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.EIsSafeDesc);// "食品安全问题描述");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.EHealth);// "餐品卫生及新鲜");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.EHealthDesc);// "餐品卫生描述");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.EPack);// "餐品包装");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.EPackDesc);// "餐品包装描述");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.ECost);// "餐品性价比");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.ECostDesc);// "餐品性价比描述");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.EOtherDesc);// "其他评价");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.EStar);// "在线评分");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.ECreateDate);// "评论日期");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.ECreateTime);// "评论时间");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.IsReturn);// "是否申请退单");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.XMSCancelState);// "是否退单成功");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.IsPlatformLaunch);// "退单失败平台发起");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.XMSCancelFeedback);// "退单失败线下反馈");
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(disItm.XMSOrderCancelReason);// "退单失败原因");

                    cell = row.CreateCell(a++);
                    cell.SetCellValue(string.Empty);
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(string.Empty);
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(string.Empty);
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(string.Empty);
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(string.Empty);
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(string.Empty);
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(string.Empty);
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(string.Empty);
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(string.Empty);
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(string.Empty);
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(string.Empty);
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(string.Empty);
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(string.Empty);
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(string.Empty);
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(string.Empty);
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(string.Empty);
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(string.Empty);
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(string.Empty);
                    cell = row.CreateCell(a++);
                    cell.SetCellValue(string.Empty);
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
                return File(excelData, "application/vnd.ms-excel", HttpUtility.UrlEncode("Report_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx", System.Text.Encoding.UTF8));
            }
            else
            {
                ViewBag.Msg = "无符合条件数据！";
                return View("ExportSearchList");
            }
        }

        //[AdminSessionFilter(Order = 1)]
        //[PermissionFilter(Order = 2, Permission = "00000000-0000-0000-1000-000000000001")]
        public JsonResult SyncXMSReport()
        {
            LogHelper.Info(Request.RawUrl);
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var channel = OrderApiClientChannelFactory.GetChannel();
                    var rtnVal = channel.SyncReport("xms");
                    LogHelper.Info("manual sync XMSReport result:" + rtnVal);
                }
                catch (Exception ex)
                {
                    LogHelper.Error("manual sync XMSReport ERR", ex);
                    throw ex;
                }
            });
            return Json(new { state = 1 }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SyncBDSReport()
        {
            LogHelper.Info(Request.RawUrl);
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var channel = OrderApiClientChannelFactory.GetChannel();
                    var rtnVal = channel.SyncReport("bds");
                    LogHelper.Info("manual sync BDSReport result:" + rtnVal);
                }
                catch (Exception ex)
                {
                    LogHelper.Error("manual sync BDSReport ERR", ex);
                    throw ex;
                }
            });
            return Json(new { state = 1 }, JsonRequestBehavior.AllowGet);
        }

        private HT_ORDER_REPORT_VIEW_EXT GetDisplayObj(HT_ORDER_REPORT_VIEW itm)
        {
            HT_ORDER_REPORT_VIEW_EXT rtnData = new HT_ORDER_REPORT_VIEW_EXT();
            rtnData.ApplierName = itm.ApplierName;
            rtnData.ApplierMUDID = itm.ApplierMUDID;
            rtnData.Position = itm.Position;
            rtnData.ApplierMobile = itm.ApplierMobile;
            rtnData.PRECreateDate = itm.PRECreateDate.ToString("yyyy/MM/dd");
            rtnData.PRECreateTime = itm.PRECreateDate.ToString("HH:mm:ss");
            rtnData.PREModifyDate = itm.PREModifyDate.ToString("yyyy/MM/dd");
            rtnData.PREModifyTime = itm.PREModifyDate.ToString("HH:mm:ss");
            rtnData.HTCode = itm.HTCode;
            rtnData.Market = itm.Market;
            rtnData.VeevaMeetingID = itm.VeevaMeetingID == null ? "" : itm.VeevaMeetingID;
            rtnData.TA = itm.TA;
            rtnData.Province = itm.Province;
            rtnData.City = itm.City;
            rtnData.HospitalCode = itm.HospitalCode;
            rtnData.HospitalName = itm.HospitalName;
            rtnData.HospitalAddress = itm.HospitalAddress;
            rtnData.MeetingName = itm.MeetingName;
            rtnData.MeetingDate = itm.MeetingDate.ToString("yyyy/MM/dd");
            rtnData.MeetingTime = itm.MeetingDate.ToString("HH:mm:ss");
            rtnData.PREAttendCount = itm.PREAttendCount.ToString();
            rtnData.CostCenter = itm.CostCenter;
            rtnData.BudgetTotal = itm.BudgetTotal.ToString("n");
            rtnData.IsDMFollow = itm.IsDMFollow == true ? "是" : "否";
            rtnData.IsFreeSpeaker = itm.IsFreeSpeaker == true ? "是" : "否";
            rtnData.RDSDName = itm.RDSDName;
            rtnData.RDSDMUDID = itm.RDSDMUDID;
            //20190122
            //if (itm.PREAttendCount < 60 && itm.BudgetTotal < 1200)
            //{
            //    rtnData.PREBUHeadName1 = "系统自动审批";
            //    rtnData.PREBUHeadMUDID1 = "系统自动审批";
            //}
            ////else if (itm.PREState == "6" && itm.CurrentApproverMUDID == null)
            ////{
            ////    rtnData.PREBUHeadName1 = "系统自动审批";
            ////    rtnData.PREBUHeadMUDID1 = "系统自动审批";
            ////}
            //else if (itm.BudgetTotal >= 1200 && itm.BudgetTotal < 1500)
            //{
            //    rtnData.PREBUHeadName1 = itm.CurrentApproverName;
            //    rtnData.PREBUHeadMUDID1 = itm.CurrentApproverMUDID;
            //}
            //else if (itm.BudgetTotal > 1500 && itm.PREState == "5")
            //{
            //    rtnData.PREBUHeadName1 = itm.CurrentApproverName;
            //    rtnData.PREBUHeadMUDID1 = itm.CurrentApproverMUDID;
            //}

            //else
            //{
            //    rtnData.PREBUHeadName1 = itm.PREBUHeadName != null ? itm.PREBUHeadName : string.Empty; ;
            //    rtnData.PREBUHeadMUDID1 = itm.PREBUHeadMUDID != null ? itm.PREBUHeadMUDID : string.Empty; ;
            //}
            rtnData.MRTerritoryCode = itm.MRTerritoryCode;
            rtnData.RDTerritoryCode = itm.RDTerritoryCode;
            rtnData.PREBUHeadName1 = itm.CurrentApproverName;
            rtnData.PREBUHeadMUDID1 = itm.CurrentApproverMUDID;
            rtnData.PREBUHeadName = itm.PREBUHeadName != null ? itm.PREBUHeadName : string.Empty; ;
            rtnData.PREBUHeadMUDID = itm.PREBUHeadMUDID != null ? itm.PREBUHeadMUDID : string.Empty; ;
            rtnData.PREBUHeadApproveDate = itm.PREBUHeadApproveDate != null ? itm.PREBUHeadApproveDate.Value.ToString("yyyy/MM/dd") : string.Empty;
            rtnData.PREBUHeadApproveTime = itm.PREBUHeadApproveDate != null ? itm.PREBUHeadApproveDate.Value.ToString("HH:mm:ss") : string.Empty;
            if (itm.PREState == "0" || itm.PREState == "1" || itm.PREState == "3" || itm.PREState == "4")
            {
                rtnData.PREState = "预申请提交成功";
            }
            else if (itm.PREState == "5" || itm.PREState == "6" || itm.PREState == "9")
            {
                rtnData.PREState = "预申请审批通过";
            }
            else if (itm.PREState == "2" || itm.PREState == "4" || itm.PREState == "8")
            {
                rtnData.PREState = "预申请审批被驳回";
            }
            else if (itm.PREState == "10")
            {
                rtnData.PREState = "预申请已取消";
            }
            else
            {
                rtnData.PREState = string.Empty;
            }
            rtnData.PREIsReAssign = itm.PREIsReAssign == true ? "是" : "否";
            rtnData.PREReAssignOperatorName = itm.PREReAssignOperatorName != null ? itm.PREReAssignOperatorName : string.Empty;
            rtnData.PREReAssignOperatorMUDID = itm.PREReAssignOperatorMUDID != null ? itm.PREReAssignOperatorMUDID : string.Empty;
            rtnData.PREReAssignBUHeadName = itm.PREReAssignBUHeadName != null ? itm.PREReAssignBUHeadName : string.Empty;
            rtnData.PREReAssignBUHeadMUDID = itm.PREReAssignBUHeadMUDID != null ? itm.PREReAssignBUHeadMUDID : string.Empty;
            rtnData.PREReAssignBUHeadApproveDate = itm.PREReAssignBUHeadApproveDate != null ? itm.PREReAssignBUHeadApproveDate.Value.ToString("yyyy/MM/dd") : string.Empty;
            rtnData.PREReAssignBUHeadApproveTime = itm.PREReAssignBUHeadApproveDate != null ? itm.PREReAssignBUHeadApproveDate.Value.ToString("HH:mm:ss") : string.Empty;
            rtnData.Channel = itm.Channel != null ? itm.Channel.ToUpper() : string.Empty;
            rtnData.EnterpriseOrderId = itm.EnterpriseOrderId;
            rtnData.ORDCreateDate = itm.ORDCreateDate.ToString("yyyy/MM/dd");
            rtnData.ORDCreateTime = itm.ORDCreateDate.ToString("HH:mm:ss");
            rtnData.ORDDeliverDate = itm.DeliverTime.ToString("yyyy/MM/dd");
            rtnData.ORDDeliverTime = itm.DeliverTime.ToString("HH:mm:ss");
            rtnData.RestaurantId = itm.RestaurantId != null ? itm.RestaurantId : string.Empty;
            rtnData.RestaurantName = itm.RestaurantName != null ? itm.RestaurantName : string.Empty;
            rtnData.ORDAttendCount = itm.ORDAttendCount.ToString();
            rtnData.FoodCount = itm.FoodCount.ToString();
            rtnData.TotalPrice = itm.TotalPrice.ToString("n");
            rtnData.totalFee = itm.totalFee.ToString("n");
            rtnData.feeModifyReason = itm.feeModifyReason != null ? itm.feeModifyReason : string.Empty;
            rtnData.DeliveryAddress = itm.DeliveryAddress != null ? itm.DeliveryAddress : string.Empty;
            rtnData.Consignee = itm.Consignee != null ? itm.Consignee : string.Empty;
            rtnData.Phone = itm.Phone != null ? itm.Phone : string.Empty;
            rtnData.Remark = itm.Remark != null ? itm.Remark : string.Empty;
            rtnData.bookState = itm.bookState != null ? itm.bookState : string.Empty;
            rtnData.ORDReceiveDate = itm.ReceiveTime != null ? itm.ReceiveTime.Value.ToString("yyyy/MM/dd") : string.Empty;
            rtnData.ORDReceiveTime = itm.ReceiveTime != null ? itm.ReceiveTime.Value.ToString("HH:mm:ss") : string.Empty;
            rtnData.IsRetuen = (itm.IsRetuen == 0) ? "否" : "是";
            rtnData.cancelState = itm.cancelState != null ? itm.cancelState : string.Empty;
            if (itm.IsRetuen != 0)
            {
                if (itm.IsRetuen == 4 || itm.IsRetuen == 5 || itm.IsRetuen == 6)
                {
                    rtnData.IsRetuenSuccess = "是";
                }
                else
                {
                    rtnData.IsRetuenSuccess = "否";
                }
            }
            else
            {
                rtnData.IsRetuenSuccess = string.Empty;
            }
            rtnData.cancelFeedback = itm.cancelFeedback != null ? itm.cancelFeedback : string.Empty;
            rtnData.cancelFailReason = itm.cancelFailReason != null ? itm.cancelFailReason : string.Empty;
            switch (itm.ReceiveState)
            {
                case 6:
                    rtnData.ReceiveState = "是"; break;
                case 7:
                    rtnData.ReceiveState = "自动"; break;
                case 8:
                    rtnData.ReceiveState = "未送达"; break;
                default:
                    rtnData.ReceiveState = "否"; break;
            }
            rtnData.ReceiveDate = itm.ReceiveDate != null ? itm.ReceiveDate.Value.ToString("yyyy/MM/dd") : string.Empty;
            rtnData.ReceiveTime = itm.ReceiveDate != null ? itm.ReceiveDate.Value.ToString("HH:mm:ss") : string.Empty;
            switch (itm.ORDState)
            {
                case 1:
                    rtnData.ORDState = "订单待审批"; break;
                case 2:
                    rtnData.ORDState = "订单审批被驳回"; break;
                case 3:
                    rtnData.ORDState = "订单提交成功"; break;
                case 4:
                    rtnData.ORDState = "预订成功"; break;
                case 5:
                    rtnData.ORDState = "预订失败"; break;
                case 6:
                    rtnData.ORDState = "已收餐"; break;
                case 7:
                    rtnData.ORDState = "系统已收餐"; break;
                case 8:
                    rtnData.ORDState = "未送达"; break;
                case 9:
                    rtnData.ORDState = "已评价"; break;
                case 10:
                    rtnData.ORDState = "申请退订"; break;
                case 11:
                    rtnData.ORDState = "退订成功"; break;
                case 12:
                    rtnData.ORDState = "退订失败"; break;
                default:
                    rtnData.ORDState = string.Empty; break;
            }
            rtnData.RealPrice = itm.RealPrice != null ? itm.RealPrice : string.Empty;
            rtnData.RealPriceChangeReason = itm.RealPriceChangeReason != null ? itm.RealPriceChangeReason : string.Empty;
            rtnData.RealPriceChangeRemark = itm.RealPriceChangeRemark != null ? itm.RealPriceChangeRemark : string.Empty;
            rtnData.RealCount = itm.RealCount != null ? itm.RealCount : string.Empty;
            rtnData.RealCountChangeReason = itm.RealCountChangeReason != null ? itm.RealCountChangeReason : string.Empty;
            rtnData.RealCountChangeRemrak = itm.RealCountChangeRemrak != null ? itm.RealCountChangeRemrak : string.Empty;
            rtnData.IsOrderUpload = itm.IsOrderUpload == 1 ? "是" : "否";
            if (itm.EStar != 99)
            {
                switch (itm.EOnTime)
                {
                    case 1:
                        rtnData.EOnTime = "迟到60分钟以上"; break;
                    case 2:
                        rtnData.EOnTime = "迟到60分钟以内"; break;
                    case 3:
                        rtnData.EOnTime = "早到"; break;
                    case 4:
                        rtnData.EOnTime = "准点"; break;
                    case 5:
                        rtnData.EOnTime = "迟到60分及以上"; break;
                    case 6:
                        rtnData.EOnTime = "迟到30-60分钟"; break;
                    case 7:
                        rtnData.EOnTime = "迟到30分钟以内"; break;
                    case 8:
                        rtnData.EOnTime = "早到30分钟及以上"; break;
                    case 9:
                        rtnData.EOnTime = "早到30分钟以内"; break;
                    case 10:
                        rtnData.EOnTime = "准点"; break;
                    default:
                        rtnData.EOnTime = string.Empty; break;
                }
            }
            else
            {
                rtnData.EOnTime = string.Empty;
            }
            rtnData.EUnTimeDesc = string.Empty;
            rtnData.EOnTimeDesc = string.Empty;
            if (itm.EOnTime == 0)
            {
                if (itm.EOnTimeDesc != null)
                {
                    rtnData.EUnTimeDesc = itm.EOnTimeDesc;
                }
            }
            else if (itm.EOnTime > 0)
            {
                rtnData.EOnTimeDesc = itm.EOnTimeDesc != null ? itm.EOnTimeDesc : string.Empty;
            }
            if (itm.EIsSafe == 1)
            {
                rtnData.EIsSafe = "是";
            }
            else if (itm.EIsSafe == 2)
            {
                rtnData.EIsSafe = "否";
            }
            else
            {
                rtnData.EIsSafe = "";
            }
            rtnData.EIsSafeDesc = itm.EIsSafeDesc != null ? itm.EIsSafeDesc : string.Empty;
            if (itm.EHealth == 1)
            {
                rtnData.EHealth = "好";
            }
            else if (itm.EHealth == 2)
            {
                rtnData.EHealth = "中";
            }
            else if (itm.EHealth == 3)
            {
                rtnData.EHealth = "差";
            }
            else
            {
                rtnData.EHealth = string.Empty;
            }
            rtnData.EHealthDesc = itm.EHealthDesc != null ? itm.EHealthDesc : string.Empty;
            if (itm.EPack == 1)
            {
                rtnData.EPack = "好";
            }
            else if (itm.EPack == 2)
            {
                rtnData.EPack = "中";
            }
            else if (itm.EPack == 3)
            {
                rtnData.EPack = "差";
            }
            else
            {
                rtnData.EPack = string.Empty;
            }
            rtnData.EPackDesc = itm.EPackDesc != null ? itm.EPackDesc : string.Empty;
            if (itm.ECost == 1)
            {
                rtnData.ECost = "好";
            }
            else if (itm.ECost == 2)
            {
                rtnData.ECost = "中";
            }
            else if (itm.ECost == 3)
            {
                rtnData.ECost = "差";
            }
            else
            {
                rtnData.ECost = string.Empty;
            }
            rtnData.ECostDesc = itm.ECostDesc != null ? itm.ECostDesc : string.Empty;
            rtnData.EOtherDesc = itm.EOtherDesc != null ? itm.EOtherDesc : string.Empty;
            rtnData.EStar = (itm.EStar != 99 && itm.EStar != 0) ? itm.EStar + "星" : string.Empty;
            rtnData.ECreateDate = itm.ECreateDate != null ? itm.ECreateDate.Value.ToString("yyyy/MM/dd") : string.Empty;
            rtnData.ECreateTime = itm.ECreateDate != null ? itm.ECreateDate.Value.ToString("HH:mm:ss") : string.Empty;
            rtnData.TYYYDRDC = itm.TYYYDRDC != null ? itm.TYYYDRDC : string.Empty;
            rtnData.TYDBDRDC = itm.TYDBDRDC != null ? itm.TYDBDRDC : string.Empty;
            rtnData.TYDBTYYYDRDC = itm.TYDBTYYYDRDC != null ? itm.TYDBTYYYDRDC : string.Empty;
            rtnData.CHRSDYLS = itm.CHRSDYLS != null ? itm.CHRSDYLS : string.Empty;
            rtnData.CHRSXYLSDDFSDYLS = itm.CHRSXYLSDDFSDYLS != null ? itm.CHRSXYLSDDFSDYLS : string.Empty;
            rtnData.TYCTDRDC = itm.TYCTDRDC != null ? itm.TYCTDRDC : string.Empty;
            rtnData.TYDBTYCTDRDC = itm.TYDBTYCTDRDC != null ? itm.TYDBTYCTDRDC : string.Empty;
            rtnData.TYDBTYYYTYCTDRDC = itm.TYDBTYYYTYCTDRDC != null ? itm.TYDBTYYYTYCTDRDC : string.Empty;
            rtnData.customerPickup = itm.customerPickup != null ? itm.customerPickup : string.Empty;
            rtnData.PUOCreateDate = itm.PUOCreateDate != null ? itm.PUOCreateDate.Value.ToString("yyyy/MM/dd") : string.Empty;
            rtnData.PUOCreateTime = itm.PUOCreateDate != null ? itm.PUOCreateDate.Value.ToString("HH:mm:ss") : string.Empty;
            rtnData.PUOBUHeadName = itm.PUOBUHeadName != null ? itm.PUOBUHeadName : string.Empty;
            rtnData.PUOBUHeadMUDID = itm.PUOBUHeadMUDID != null ? itm.PUOBUHeadMUDID : string.Empty;
            rtnData.ApproveDate = itm.ApproveDate != null ? itm.ApproveDate.Value.ToString("yyyy/MM/dd") : string.Empty;
            rtnData.ApproveTime = itm.ApproveDate != null ? itm.ApproveDate.Value.ToString("HH:mm:ss") : string.Empty;
            if (itm.PUOState == "2" || itm.PUOState == "3")
            {
                rtnData.PUOState = "上传文件审批驳回";
            }
            else if (itm.PUOState == "4")
            {
                rtnData.PUOState = "上传文件审批通过";
            }
            else
            {
                rtnData.PUOState = string.Empty;
            }
            rtnData.IsAttentSame = itm.IsAttentSame == 1 ? "是" : "否";
            rtnData.AttentSameReason = itm.AttentSameReason != null ? itm.AttentSameReason : string.Empty;
            rtnData.IsReopen = itm.IsReopen == 1 ? "是" : "否";
            rtnData.ReopenOperatorName = itm.ReopenOperatorName != null ? itm.ReopenOperatorName : string.Empty;
            rtnData.ReopenOperatorMUDID = itm.ReopenOperatorMUDID != null ? itm.ReopenOperatorMUDID : string.Empty;
            rtnData.ReopenOperateDate = itm.ReopenOperateDate != null ? itm.ReopenOperateDate.Value.ToString("yyyy/MM/dd") : string.Empty;
            rtnData.ReopenOperateTime = itm.ReopenOperateDate != null ? itm.ReopenOperateDate.Value.ToString("HH:mm:ss") : string.Empty;
            rtnData.ReopenReason = itm.ReopenReason != null ? itm.ReopenReason : string.Empty;
            if (itm.IsReopen == 1)
            {
                rtnData.ReopenState = rtnData.PUOState;
            }
            else
            {
                rtnData.ReopenState = string.Empty;
            }
            rtnData.IsTransfer = itm.IsTransfer == 1 ? "是" : "否";
            rtnData.TransferOperatorName = itm.TransferOperatorName != null ? itm.TransferOperatorName : string.Empty;
            rtnData.TransferOperatorMUDID = itm.TransferOperatorMUDID != null ? itm.TransferOperatorMUDID : string.Empty;
            rtnData.TransferUserName = itm.TransferUserName != null ? itm.TransferUserName : string.Empty;
            rtnData.TransferUserMUDID = itm.TransferUserMUDID != null ? itm.TransferUserMUDID : string.Empty;
            rtnData.TransferOperateDate = itm.TransferOperateDate != null ? itm.TransferOperateDate.Value.ToString("yyyy/MM/dd") : string.Empty;
            rtnData.TransferOperateTime = itm.TransferOperateDate != null ? itm.TransferOperateDate.Value.ToString("HH:mm:ss") : string.Empty;
            rtnData.IsReAssign = itm.IsReAssign ? "是" : "否";
            rtnData.ReAssignOperatorName = itm.ReAssignOperatorName != null ? itm.ReAssignOperatorName : string.Empty;
            rtnData.ReAssignOperatorMUDID = itm.ReAssignOperatorMUDID != null ? itm.ReAssignOperatorMUDID : string.Empty;
            rtnData.ReAssignBUHeadName = itm.ReAssignBUHeadName != null ? itm.ReAssignBUHeadName : string.Empty;
            rtnData.ReAssignBUHeadMUDID = itm.ReAssignBUHeadMUDID != null ? itm.ReAssignBUHeadMUDID : string.Empty;
            rtnData.ReAssignBUHeadApproveDate = itm.ReAssignBUHeadApproveDate != null ? itm.ReAssignBUHeadApproveDate.Value.ToString("yyyy/MM/dd") : string.Empty;
            rtnData.ReAssignBUHeadApproveTime = itm.ReAssignBUHeadApproveDate != null ? itm.ReAssignBUHeadApproveDate.Value.ToString("HH:mm:ss") : string.Empty;
            rtnData.SpecialReason = itm.SpecialReason != null ? itm.SpecialReason : string.Empty;

            if (itm.ORDState == 5 || itm.ORDState == 11)
            {
                rtnData.HtmlOrder = string.Empty;
            }
            else
            {
                if (itm.IsOrderUpload == 0)
                {
                    var htCode = "9" + itm.HTCode.Substring(3);
                    rtnData.HtmlOrder = "<a href='javascript:;' class='button button-small bg-blue' onclick='Pop(\"" + htCode + "\"," + itm.ORDState + ")' > 特殊订单 </ a > ";
                }
                else
                {
                    if (itm.PUOState == "2" || itm.PUOState == "3")
                    {
                        var htCode = "9" + itm.HTCode.Substring(3);
                        rtnData.HtmlOrder = "<a href='javascript:;' class='button button-small bg-blue' onclick='Pop(\"" + htCode + "\"," + itm.ORDState + ")' > 特殊订单 </ a > ";
                    }
                    else
                    {
                        rtnData.HtmlOrder = string.Empty;
                    }
                }
            }
            rtnData.Level2Name = itm.Level2Name != null ? itm.Level2Name : string.Empty;
            rtnData.Level2UserId = itm.Level2UserId != null ? itm.Level2UserId : string.Empty;
            rtnData.Level3Name = itm.Level3Name != null ? itm.Level3Name : string.Empty;
            rtnData.Level3UserId = itm.Level3UserId != null ? itm.Level3UserId : string.Empty;
            rtnData.IsMealSame = itm.IsMealSame == 1 ? "是" : "否"; ;
            rtnData.IsMeetingInfoSame = itm.IsMeetingInfoSame == 1 ? "是" : "否"; ;
            rtnData.MeetingInfoSameReason = itm.MeetingInfoSameReason;
            //20200324
            rtnData.SupplierSpecialRemark = itm.SupplierSpecialRemark != null ? itm.SupplierSpecialRemark : string.Empty;
            rtnData.IsCompleteDelivery = itm.IsCompleteDelivery != null ? itm.IsCompleteDelivery : string.Empty;
            rtnData.SupplierConfirmAmount = itm.SupplierConfirmAmount != null ? itm.SupplierConfirmAmount.ToString() : string.Empty;
            rtnData.GSKConfirmAmount = itm.GSKConfirmAmount != null ? itm.GSKConfirmAmount.ToString() : string.Empty;
            rtnData.GSKConAAReason = itm.GSKConAAReason != null ? itm.GSKConAAReason : string.Empty;
            rtnData.MealPaymentAmount = itm.MealPaymentAmount != null ? itm.MealPaymentAmount.ToString() : string.Empty;
            rtnData.MealPaymentPO = itm.MealPaymentPO != null ? itm.MealPaymentPO : string.Empty;
            rtnData.AccountingTime = itm.AccountingTime != null ? itm.AccountingTime : string.Empty;
            //rtnData.AccountingTime = DateTime.Parse(itm.AccountingTime);
            return rtnData;
        }

        private P_ORDER_DAILY_VIEW_EXT GetNonHTDisplayObj(P_ORDER_DAILY_VIEW itm)
        {
            P_ORDER_DAILY_VIEW_EXT rtnData = new P_ORDER_DAILY_VIEW_EXT();
            rtnData.ID = itm.ID;
            rtnData.DCUserName = string.IsNullOrEmpty(itm.DCUserName1) == false ? itm.DCUserName1 : (itm.DCUserName != null ? itm.DCUserName : string.Empty);
            rtnData.DCPhoneNum = string.IsNullOrEmpty(itm.DCPhoneNum1) == false ? itm.DCPhoneNum1 : (itm.DCPhoneNum != null ? itm.DCPhoneNum : string.Empty);
            rtnData.MUDID = itm.MUDID;
            rtnData.Market = itm.Market;
            rtnData.CN = itm.CN != null ? itm.CN : string.Empty;
            rtnData.PO = itm.PO != null ? itm.PO : string.Empty;
            rtnData.BudgetTotal = itm.BudgetTotal != null ? itm.BudgetTotal : string.Empty;
            rtnData.WBS = itm.WBS != null ? itm.WBS : string.Empty;
            rtnData.TA = itm.WBS != null ? itm.TA : string.Empty;
            rtnData.MeetCode = itm.MeetCode != null ? itm.MeetCode : string.Empty;
            rtnData.MeetName = itm.MeetName != null ? itm.MeetName : string.Empty;
            rtnData.XMSOrderID = itm.XMSOrderID != null ? itm.XMSOrderID : string.Empty;
            rtnData.OrderingDate = itm.OrderingTime != null ? itm.OrderingTime.Value.ToString("yyyy-MM-dd") : string.Empty;
            rtnData.OrderingTime = itm.OrderingTime != null ? itm.OrderingTime.Value.ToString("HH:mm:ss") : string.Empty;
            rtnData.SendDate = itm.SendTime != null ? itm.SendTime.Value.ToString("yyyy-MM-dd") : string.Empty;
            rtnData.SendTime = itm.SendTime != null ? itm.SendTime.Value.ToString("HH:mm:ss") : string.Empty;
            rtnData.ReceiveDate = itm.ReceiveTime != null ? itm.ReceiveTime.Value.ToString("yyyy-MM-dd") : string.Empty;
            rtnData.ReceiveTime = itm.ReceiveTime != null ? itm.ReceiveTime.Value.ToString("HH:mm:ss") : string.Empty;
            rtnData.RestaurantId = itm.RestaurantId != null ? itm.RestaurantId : string.Empty;
            rtnData.RestName = itm.RestName != null ? itm.RestName : string.Empty;
            rtnData.UserQuantity = itm.UserQuantity.ToString();
            rtnData.MealQuantity = itm.MealQuantity.ToString();
            rtnData.TotalPrice = itm.TotalPrice != null ? itm.TotalPrice.Value.ToString("n") : string.Empty;
            rtnData.XMSTotalPrice = (itm.XMSTotalPrice != null && itm.XMSTotalPrice.Value != -1m) ? itm.XMSTotalPrice.Value.ToString("n") : string.Empty;
            rtnData.ChangePriceReason = itm.ChangePriceReason != null ? itm.ChangePriceReason : string.Empty;
            rtnData.TYYYDRDC = itm.TYYYDRDC != null ? itm.TYYYDRDC : string.Empty;
            rtnData.TYDBDRDC = itm.TYDBDRDC != null ? itm.TYDBDRDC : string.Empty;
            rtnData.TYDBTYYYDRDC = itm.TYDBTYYYDRDC != null ? itm.TYDBTYYYDRDC : string.Empty;
            rtnData.CHRSDYLS = itm.CHRSDYLS != null ? itm.CHRSDYLS : string.Empty;
            rtnData.CHRSXYLSDDFSDYLS = itm.CHRSXYLSDDFSDYLS != null ? itm.CHRSXYLSDDFSDYLS : string.Empty;
            rtnData.TYCTDRDC = itm.TYCTDRDC != null ? itm.TYCTDRDC : string.Empty;
            rtnData.TYDBTYCTDRDC = itm.TYDBTYCTDRDC != null ? itm.TYDBTYCTDRDC : string.Empty;
            rtnData.TYDBTYYYTYCTDRDC = itm.TYDBTYYYTYCTDRDC != null ? itm.TYDBTYYYTYCTDRDC : string.Empty;
            rtnData.CustomerPickup = itm.CustomerPickup != null ? itm.CustomerPickup : string.Empty;
            rtnData.ProvinceName = itm.ProvinceName != null ? itm.ProvinceName : string.Empty;
            rtnData.CityName = itm.CityName != null ? itm.CityName : string.Empty;
            rtnData.GskHospital = itm.GskHospital != null ? itm.GskHospital : string.Empty;
            rtnData.HospitalName = itm.HospitalName != null ? itm.HospitalName : string.Empty;
            rtnData.HospitalAddr = itm.HospitalAddr != null ? itm.HospitalAddr : string.Empty;
            rtnData.HospitalRoom = itm.HospitalRoom != null ? itm.HospitalRoom : string.Empty;
            rtnData.SCUserName = itm.SCUserName != null ? itm.SCUserName : string.Empty;
            rtnData.SCPhoneNum = itm.SCPhoneNum != null ? itm.SCPhoneNum : string.Empty;
            rtnData.Remark = itm.Remark != null ? itm.Remark : string.Empty;
            rtnData.XMSBookState = itm.XMSBookState != null ? itm.XMSBookState : string.Empty;
            switch (itm.ReceiveState)
            {
                case 6:
                    rtnData.ReceiveState = "是"; break;
                case 7:
                    rtnData.ReceiveState = "自动"; break;
                case 8:
                    rtnData.ReceiveState = "未送达"; break;
                default:
                    rtnData.ReceiveState = "否"; break;
            }
            if (itm.EStar != 99)
            {
                switch (itm.EOnTime)
                {
                    case 1:
                        rtnData.EOnTime = "迟到60分钟以上"; break;
                    case 2:
                        rtnData.EOnTime = "迟到60分钟以内"; break;
                    case 3:
                        rtnData.EOnTime = "早到"; break;
                    case 4:
                        rtnData.EOnTime = "准点"; break;
                    case 5:
                        rtnData.EOnTime = "迟到60分及以上"; break;
                    case 6:
                        rtnData.EOnTime = "迟到30-60分钟"; break;
                    case 7:
                        rtnData.EOnTime = "迟到30分钟以内"; break;
                    case 8:
                        rtnData.EOnTime = "早到30分钟及以上"; break;
                    case 9:
                        rtnData.EOnTime = "早到30分钟以内"; break;
                    case 10:
                        rtnData.EOnTime = "准点"; break;
                    default:
                        rtnData.EOnTime = string.Empty; break;
                }
            }
            else
            {
                rtnData.EOnTime = string.Empty;
            }
            rtnData.Channel = itm.Channel != null ? itm.Channel.ToUpper() : string.Empty;
            rtnData.EUnTimeDesc = string.Empty;
            rtnData.EOnTimeDesc = string.Empty;
            if (itm.EOnTime == 0)
            {
                if (itm.EOnTimeDesc != null)
                {
                    rtnData.EUnTimeDesc = itm.EOnTimeDesc;
                }
            }
            else if (itm.EOnTime > 0)
            {
                rtnData.EOnTimeDesc = itm.EOnTimeDesc != null ? itm.EOnTimeDesc : string.Empty;
            }

            //rtnData.EIsSafe = (itm.EStar != 99 && itm.EStar != 0) ? (itm.EIsSafe == 1 ? "是" : "否") : string.Empty;
            if (itm.EIsSafe == 1)
            {
                rtnData.EIsSafe = "是";
            }
            else if (itm.EIsSafe == 2)
            {
                rtnData.EIsSafe = "否";
            }
            else
            {
                rtnData.EIsSafe = "";
            }

            rtnData.EIsSafeDesc = itm.EIsSafeDesc != null ? itm.EIsSafeDesc : string.Empty;
            if (itm.EHealth == 1)
            {
                rtnData.EHealth = "好";
            }
            else if (itm.EHealth == 2)
            {
                rtnData.EHealth = "中";
            }
            else if (itm.EHealth == 3)
            {
                rtnData.EHealth = "差";
            }
            else
            {
                rtnData.EHealth = string.Empty;
            }
            rtnData.EHealthDesc = itm.EHealthDesc != null ? itm.EHealthDesc : string.Empty;
            if (itm.EPack == 1)
            {
                rtnData.EPack = "好";
            }
            else if (itm.EPack == 2)
            {
                rtnData.EPack = "中";
            }
            else if (itm.EPack == 3)
            {
                rtnData.EPack = "差";
            }
            else
            {
                rtnData.EPack = string.Empty;
            }
            rtnData.EPackDesc = itm.EPackDesc != null ? itm.EPackDesc : string.Empty;
            if (itm.ECost == 1)
            {
                rtnData.ECost = "好";
            }
            else if (itm.ECost == 2)
            {
                rtnData.ECost = "中";
            }
            else if (itm.ECost == 3)
            {
                rtnData.ECost = "差";
            }
            else
            {
                rtnData.ECost = string.Empty;
            }
            rtnData.ECostDesc = itm.ECostDesc != null ? itm.ECostDesc : string.Empty;
            rtnData.EOtherDesc = itm.EOtherDesc != null ? itm.EOtherDesc : string.Empty;
            rtnData.EStar = (itm.EStar != 99 && itm.EStar != 0) ? itm.EStar + "星" : string.Empty;
            rtnData.ECreateDate = itm.ECreateDate != null ? itm.ECreateDate.Value.ToString("yyyy-MM-dd") : string.Empty;
            rtnData.ECreateTime = itm.ECreateDate != null ? itm.ECreateDate.Value.ToString("HH:mm:ss") : string.Empty;
            rtnData.IsReturn = (itm.XMSCancelState == null) ? "否" : itm.XMSCancelState == "" ? "否" : "是";
            //rtnData.IsReturn = (itm.TJIsReturn == 0) ? "否" : "是";
            rtnData.XMSCancelState = itm.XMSCancelState != null ? itm.XMSCancelState : string.Empty;

            if (itm.TJIsReturn == 1)
            {
                if (itm.TJIsDelivery == 1)
                {
                    rtnData.IsPlatformLaunch = "是";

                }
                else
                {
                    rtnData.IsPlatformLaunch = "否";

                }
            }
            else
            {
                rtnData.IsPlatformLaunch = string.Empty;


            }

            rtnData.XMSCancelFeedback = itm.XMSCancelFeedback != null ? itm.XMSCancelFeedback : string.Empty;
            rtnData.XMSOrderCancelReason = itm.XMSOrderCancelReason != null ? itm.XMSOrderCancelReason : string.Empty;
            switch (itm.OrderState)
            {
                case 1:
                    rtnData.OrderState = "订单待审批"; break;
                case 2:
                    rtnData.OrderState = "订单审批被驳回"; break;
                case 3:
                    rtnData.OrderState = "订单提交成功"; break;
                case 4:
                    rtnData.OrderState = "预订成功"; break;
                case 5:
                    rtnData.OrderState = "预订失败"; break;
                case 6:
                    rtnData.OrderState = "已收餐"; break;
                case 7:
                    rtnData.OrderState = "系统已收餐"; break;
                case 8:
                    rtnData.OrderState = "未送达"; break;
                case 9:
                    rtnData.OrderState = "已评价"; break;
                case 10:
                    rtnData.OrderState = "申请退订"; break;
                case 11:
                    rtnData.OrderState = "退订成功"; break;
                case 12:
                    rtnData.OrderState = "退订失败"; break;
                default:
                    rtnData.EOnTime = string.Empty; break;
            }
            return rtnData;
        }

        //[AdminSessionFilter(Order = 1)]
        //[PermissionFilter(Order = 2, Permission = "00000000-0000-0000-1000-000000000001")]
        public JsonResult UpdateXMSReport()
        {
            //var openApiChannel = OpenApiChannelFactory.GetChannel();
            //var res = openApiChannel.calculateFee(hospitalId, resId, foods);
            return Json("");
        }
        #endregion

        #region ------------审批列表
        //[AdminSessionFilter(Order = 1)]
        //[PermissionFilter(Order = 2, Permission = "00000000-0000-0000-1000-000000000002")]
        public ActionResult ApproveList()
        {
            return View();
        }

        [HttpPost]
        //[AdminSessionFilter(Order = 1)]
        //[PermissionFilter(Order = 2, Permission = "00000000-0000-0000-1000-000000000002")]
        public JsonResult LoadApproveList(string CN, string MUDID, string DeliverTimeBegin, string DeliverTimeEnd, string MMCoEApproveState, int isNonHt, int rows, int page)
        {

            LogHelper.Info(Request.RawUrl);
            DateTime tmpTime;
            DateTime? DTBegin, DTEnd;
            if (DateTime.TryParse(DeliverTimeBegin, out tmpTime) == true)
            {
                DTBegin = tmpTime;
            }
            else
            {
                DTBegin = null;
            }
            if (DateTime.TryParse(DeliverTimeEnd, out tmpTime) == true)
            {
                DTEnd = tmpTime.AddDays(1d);
            }
            else
            {
                DTEnd = null;
            }
            int stateFlg;
            if (int.TryParse(MMCoEApproveState, out stateFlg) == false)
            {
                stateFlg = Entity.Enum.MMCoEApproveState.WAITAPPROVE;
            }
            int total;
            var list = orderService.LoadOrderApprovePage(CN, MUDID, DTBegin, DTEnd, stateFlg, isNonHt, rows, page, out total);
            return Json(new { state = 1, rows = list, total = total });
        }

        //[AdminSessionFilter(Order = 1)]
        //[PermissionFilter(Order = 2, Permission = "00000000-0000-0000-1000-000000000002")]
        public ActionResult ApproveForm(Guid OrderID)
        {
            ViewBag.OrderID = OrderID;
            var odr = orderService.GetOrderInfo(OrderID);
            List<string> imgs = new List<string>();
            if (odr != null)
            {
                imgs = GetImgList(odr.MMCoEImage);
                if (odr.MMCoEApproveState == MMCoEApproveState.WAITAPPROVE)
                {
                    ViewBag.CanApprove = JsonConvert.True;
                }
                else
                {
                    ViewBag.CanApprove = JsonConvert.False;
                }

                ViewBag.MMCoEApproveState = odr.MMCoEApproveState;
                ViewBag.MMCoEReason = odr.MMCoEReason;
            }
            else
            {
                ViewBag.MMCoEApproveState = MMCoEApproveState.NO;
                ViewBag.MMCoEReason = string.Empty;
                ViewBag.CanApprove = JsonConvert.False;
            }
            ViewBag.Evidences = imgs;
            //WebConfig.MealH5SiteUrl;
            return View();
        }

        //[AdminSessionFilter(Order = 1)]
        //[PermissionFilter(Order = 2, Permission = "00000000-0000-0000-1000-000000000002")]
        public JsonResult ApproveOrder(Guid OrderID, bool IsPass, string Comment)
        {
            int state;
            if (IsPass == true)
            {
                state = MMCoEApproveState.APPROVESUCCESS;
            }
            else
            {
                state = MMCoEApproveState.APPROVEREJECT;
            }
            //-----

            var channel = OrderApiClientChannelFactory.GetChannel();
            var evaluateChannel = EvaluateClientChannelFactory.GetChannel();
            var order = channel.FindByID(OrderID);

            //审批驳回
            if (state == 2)
            {
                var res = channel.MMCoEResult(OrderID, state, Comment);
                if (res == 1)
                {
                    P_ORDER_APPROVE entity = new P_ORDER_APPROVE();
                    entity.ID = Guid.NewGuid();
                    entity.OrderID = OrderID;
                    if (order.IsNonHT == 0)
                    {
                        entity.CN = order.CN;
                    }
                    else
                    {
                        entity.CN = order.PO;
                    }
                    entity.UserId = order.UserId;
                    entity.OldState = order.State;
                    entity.NewState = OrderState.REJECT;
                    entity.Result = 0;
                    entity.Comment = Comment;
                    entity.IsWXClient = 0;
                    entity.CreateDate = DateTime.Now;
                    entity.CreateUserId = CurrentAdminUser.Email;
                    //添加审批记录
                    var evaluateRes = evaluateChannel.AddOrderApprove(entity);

                    if (evaluateRes == 0)
                    {
                        LogHelper.Error("保存审批记录未成功 0 evaluateChannel.AddOrderApprove(entity)：entity" + JsonConvert.SerializeObject(entity) + "| evaluateRes:" + evaluateRes);
                    }
                    // 发用户消息
                    if (order.IsNonHT == 0)
                    {
                        WxMessageClientChannelFactory.GetChannel().SendWxMessageByOrder(order.ID);
                    }
                    else
                    {
                        WMessageClientChannelFactory.GetChannel().SendWxMessageByOrder(order.ID);
                    }
                    return Json(new { state = 1, txt = "订单审批已驳回" });
                }
            }

            //审批通过
            else
            {
                //订单审批
                var _res = channel.MMCoEResult(OrderID, state, Comment);
                if (_res == 1)
                {
                    //var openApiChannel = OpenApiChannelFactory.GetChannel();
                    //下新单
                    if (order.IsChange == 0)
                    {
                        var json = order.Detail;
                        P_Order orderInfo = JsonConvert.DeserializeObject<P_Order>(json);
                        var foodList = orderInfo.foods.foods.Select(a => a.ToFoodRequest()).ToArray();
                        var foods = new List<iPathFeast.ApiEntity.Food>();
                        foreach (var item in foodList)
                        {
                            foods.Add(new iPathFeast.ApiEntity.Food()
                            {
                                foodId = item.foodId,
                                foodName = item.foodName,
                                count = Convert.ToInt32(item.count)
                            });
                        }
                        var req = new CreateOrderReq()
                        {
                            _Channels = order.Channel,
                            enterpriseOrderId = order.EnterpriseOrderId,
                            oldiPathOrderId = string.Empty,
                            sendTime = orderInfo.details.deliverTime.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                            foodFee = orderInfo.foods.foodFee.ToString(),
                            packageFee = orderInfo.foods.packageFee.ToString(),
                            sendFee = orderInfo.foods.sendFee.ToString(),
                            totalFee = orderInfo.foods.allPrice.ToString(),
                            orderTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            remark = orderInfo.details.remark,
                            dinnerName = orderInfo.details.consignee,
                            dinnernum = orderInfo.details.attendCount.ToString(),
                            phone = orderInfo.details.phone,
                            address = orderInfo.hospital.address + " - " + orderInfo.details.deliveryAddress,
                            resId = orderInfo.foods.resId,
                            longitude = orderInfo.hospital.longitude,
                            latitude = orderInfo.hospital.latitude,
                            hospitalId = orderInfo.hospital.hospital,
                            foods = foods,
                            cityId = orderInfo.hospital.city
                        };
                        if (order.IsNonHT == 0)
                        {
                            req.invoiceTitle = orderInfo.hospital.invoiceTitle;
                            req.cn = order.CN;
                            req.cnAmount = orderInfo.meeting.budgetTotal.ToString();
                            req.mudId = orderInfo.meeting.userId;
                            req.typeId = "0";
                        }
                        else
                        {
                            req.invoiceTitle = orderInfo.hospital.invoiceTitle + " - " + orderInfo.hospital.dutyParagraph;
                            req.cn = order.PO;
                            req.cnAmount = "99999";
                            req.mudId = orderInfo.userid;
                            req.typeId = "2";
                        }
                        var openApiRes = apiClient.CreateOrder(req);
                        if (openApiRes.errorCode == "0")
                        {
                            var _channel = OrderApiClientChannelFactory.GetChannel();
                            // 审批下单成功，保存小秘书单号
                            var res = channel.SaveXmsOrderId(OrderID, openApiRes.iPathOrderId);
                            if (res > 0)
                            {
                                var _order = channel.FindByID(OrderID);

                                P_ORDER_APPROVE entity = new P_ORDER_APPROVE();
                                entity.ID = Guid.NewGuid();
                                entity.OrderID = OrderID;
                                if (order.IsNonHT == 0)
                                {
                                    entity.CN = order.CN;
                                }
                                else
                                {
                                    entity.CN = order.PO;
                                }
                                entity.UserId = _order.UserId;
                                entity.OldState = order.State;
                                entity.NewState = _order.State;
                                entity.Result = 1;
                                entity.Comment = Comment;
                                entity.IsWXClient = 0;
                                entity.CreateDate = DateTime.Now;
                                entity.CreateUserId = CurrentAdminUser.Email;
                                //添加审批记录
                                var evaluateRes = evaluateChannel.AddOrderApprove(entity);
                                //发用户消息
                                if (order.IsNonHT == 0)
                                {
                                    WxMessageClientChannelFactory.GetChannel().SendWxMessageByOrder(order.ID);
                                }
                                else
                                {
                                    WMessageClientChannelFactory.GetChannel().SendWxMessageByOrder(order.ID);
                                }
                                if (evaluateRes > 0)
                                {
                                    return Json(new { state = 1, txt = "订单审批已通过" });
                                }
                                else
                                {
                                    LogHelper.Error("保存审批记录未成功 1 evaluateChannel.AddOrderApprove(entity)\r\n  entity:" + JsonConvert.SerializeObject(entity) + "|\r\n  evaluateRes:" + evaluateRes);
                                    return Json(new { state = 1, txt = "订单审批已通过，但审批数据未全部保存成功！" });
                                }
                            }
                        }
                        else
                        {
                            LogHelper.Info("xms interface createOrder2(OrderID.ToString():" + OrderID + ", string.Empty, \"0\", orderInfo:" + json + ") \r\n openApiRes:" + JsonConvert.SerializeObject(openApiRes));
                            //审批退回审批前状态
                            var res = channel._MMCoEResult(OrderID, 1, "");
                            if (res != 1)
                            {
                                LogHelper.Error("订单审批退回失败 1 channel._MMCoEResult(OrderID, 1, \"\") OrderID:" + OrderID + "| res:" + res);
                            }
                            return Json(new { state = 0, txt = "调用小秘书下单接口失败，订单审批失败。错误码：" + openApiRes.errorCode });
                        }
                    }
                    //改单
                    else
                    {
                        var json = order.ChangeDetail;
                        P_Order orderInfo = JsonConvert.DeserializeObject<P_Order>(json);
                        var ChangeID = Guid.NewGuid();
                        //调用小秘书下单接口
                        var foodList = orderInfo.foods.foods.Select(a => a.ToFoodRequest()).ToArray();
                        var foods = new List<iPathFeast.ApiEntity.Food>();
                        foreach (var item in foodList)
                        {
                            foods.Add(new iPathFeast.ApiEntity.Food()
                            {
                                foodId = item.foodId,
                                foodName = item.foodName,
                                count = Convert.ToInt32(item.count)
                            });
                        }
                        var _enterpriseOrderId = order.EnterpriseOrderId;
                        if (string.IsNullOrEmpty(_enterpriseOrderId))
                        {
                            int RandKey = new Random().Next(1, 999);
                            var two = (RandKey < 10 ? "00" + RandKey : (RandKey < 100 ? "0" + RandKey : RandKey.ToString()));
                            var _channel = order.Channel;
                            var _date = DateTime.Now.ToString("yyMMddHHmmss");
                            _enterpriseOrderId = _channel.ToUpper() + "-" + _date + two;
                        }
                        var type = "";
                        var req = new CreateOrderReq()
                        {
                            _Channels = order.Channel,
                            enterpriseOrderId = order.EnterpriseOrderId,
                            oldiPathOrderId = string.Empty,
                            sendTime = orderInfo.details.deliverTime.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                            foodFee = orderInfo.foods.foodFee.ToString(),
                            packageFee = orderInfo.foods.packageFee.ToString(),
                            sendFee = orderInfo.foods.sendFee.ToString(),
                            totalFee = orderInfo.foods.allPrice.ToString(),
                            orderTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            remark = orderInfo.details.remark,
                            dinnerName = orderInfo.details.consignee,
                            dinnernum = orderInfo.details.attendCount.ToString(),
                            phone = orderInfo.details.phone,
                            address = orderInfo.hospital.address + " - " + orderInfo.details.deliveryAddress,
                            resId = orderInfo.foods.resId,
                            longitude = orderInfo.hospital.longitude,
                            latitude = orderInfo.hospital.latitude,
                            hospitalId = orderInfo.hospital.hospital,
                            foods = foods,
                            cityId = orderInfo.hospital.city
                        };
                        if (order.IsNonHT == 0)
                        {
                            type = string.IsNullOrEmpty(order.XmsOrderId) ? "0" : "1";
                            req.invoiceTitle = orderInfo.hospital.invoiceTitle;
                            req.cn = order.CN;
                            req.cnAmount = orderInfo.meeting.budgetTotal.ToString();
                            req.mudId = orderInfo.meeting.userId;
                            req.typeId = type;
                        }
                        else
                        {
                            type = string.IsNullOrEmpty(order.XmsOrderId) ? "2" : "3";
                            req.invoiceTitle = orderInfo.hospital.invoiceTitle + " - " + orderInfo.hospital.dutyParagraph;
                            req.cn = order.PO;
                            req.cnAmount = "99999";
                            req.mudId = orderInfo.userid;
                            req.typeId = type;
                        }
                        var openApiRes = apiClient.CreateOrder(req);
                        if (openApiRes.errorCode == "0")
                        {
                            var _order = channel.FindByID(OrderID);

                            P_ORDER_APPROVE entity = new P_ORDER_APPROVE();
                            entity.ID = Guid.NewGuid();
                            entity.OrderID = OrderID;
                            if (order.IsNonHT == 0)
                            {
                                entity.CN = order.CN;
                            }
                            else
                            {
                                entity.CN = order.PO;
                            }
                            entity.UserId = _order.UserId;
                            entity.OldState = order.State;
                            entity.NewState = _order.State;
                            entity.Result = 1;
                            entity.Comment = Comment;
                            entity.IsWXClient = 0;
                            entity.CreateDate = DateTime.Now;
                            entity.CreateUserId = CurrentAdminUser.Email;
                            //添加审批记录
                            var evaluateRes = evaluateChannel.AddOrderApprove(entity);
                            //发用户消息
                            if (order.IsNonHT == 0)
                            {
                                WxMessageClientChannelFactory.GetChannel().SendWxMessageByOrder(order.ID);
                            }
                            else
                            {
                                WMessageClientChannelFactory.GetChannel().SendWxMessageByOrder(order.ID);
                            }

                            if (evaluateRes > 0)
                            {
                                return Json(new { state = 1, txt = "订单审批已通过" });
                            }
                            else
                            {
                                LogHelper.Error("保存审批记录未成功 2 evaluateChannel.AddOrderApprove(entity)\r\n entity:" + JsonConvert.SerializeObject(entity) + "| \r\n evaluateRes:" + evaluateRes);
                                return Json(new { state = 1, txt = "订单审批已通过，但审批数据未全部保存成功！" });
                            }
                        }
                        else
                        {
                            LogHelper.Info("xms interface createOrder2(OrderID.ToString():" + OrderID + ", string.Empty, \"0\", orderInfo:" + json + ") \r\n openApiRes:" + JsonConvert.SerializeObject(openApiRes));
                            //审批退回审批前状态
                            var res = channel._MMCoEResult(OrderID, 1, "");
                            if (res != 1)
                            {
                                LogHelper.Error("订单审批退回失败 2 channel._MMCoEResult(OrderID, 1, \"\") OrderID:" + OrderID + "| res:" + res);
                            }
                            return Json(new { state = 0, txt = "调用小秘书下单接口失败，订单审批失败。错误码：" + openApiRes.errorCode });
                        }
                    }
                }
            }
            LogHelper.Error("---background approve order unsuccessfull!\r\n| order:" + JsonConvert.SerializeObject(order) + "\r\n|IsPass:" + IsPass + "\r\n|Comment:" + Comment);
            return Json(new { state = 0, txt = "订单审批失败, 请重新打开页面尝试操作。" });
            //------------------------------------------------
        }

        #endregion

        [HttpPost]
        [AdminSessionFilter(Order = 1)]
        [PermissionFilter(Order = 2, Permission = "00000000-0000-0000-9000-000000000002")]
        public JsonResult LoadNonHTApproveList(string CN, string MUDID, string DeliverTimeBegin, string DeliverTimeEnd, string MMCoEApproveState, int isNonHt, int rows, int page)
        {

            LogHelper.Info(Request.RawUrl);
            DateTime tmpTime;
            DateTime? DTBegin, DTEnd;
            if (DateTime.TryParse(DeliverTimeBegin, out tmpTime) == true)
            {
                DTBegin = tmpTime;
            }
            else
            {
                DTBegin = null;
            }
            if (DateTime.TryParse(DeliverTimeEnd, out tmpTime) == true)
            {
                DTEnd = tmpTime.AddDays(1d);
            }
            else
            {
                DTEnd = null;
            }
            int stateFlg;
            if (int.TryParse(MMCoEApproveState, out stateFlg) == false)
            {
                stateFlg = Entity.Enum.MMCoEApproveState.WAITAPPROVE;
            }
            int total;
            var list = orderService.LoadNonHTOrderApprovePage(CN, MUDID, DTBegin, DTEnd, stateFlg, isNonHt, rows, page, out total);
            return Json(new { state = 1, rows = list, total = total });
        }

        #region ------------评价列表
        [AdminSessionFilter(Order = 1)]
        [PermissionFilter(Order = 2, Permission = "00000000-0000-0000-2000-000000000002")]
        public ActionResult EvaluateList()
        {
            return View();
        }

        [HttpPost]
        //[AdminSessionFilter(Order = 1)]
        //[PermissionFilter(Order = 2, Permission = "00000000-0000-0000-1000-000000000003")]
        public JsonResult LoadEvaluateList(string CN, string MUDID, string DeliverTimeBegin, string DeliverTimeEnd, bool Large60, bool UnSafe, bool UnSend, int star, string channel, int rows, int page)
        {

            LogHelper.Info(Request.RawUrl);
            DateTime tmpTime;
            DateTime? DTBegin, DTEnd;
            if (DateTime.TryParse(DeliverTimeBegin, out tmpTime) == true)
            {
                DTBegin = tmpTime;
            }
            else
            {
                DTBegin = null;
            }
            if (DateTime.TryParse(DeliverTimeEnd, out tmpTime) == true)
            {
                DTEnd = tmpTime.AddDays(1d);
            }
            else
            {
                DTEnd = null;
            }
            int total;
            var list = orderService.LoadOrderEvaluatePage(CN, MUDID, DTBegin, DTEnd, Large60, UnSafe, UnSend, 0, star, channel, rows, page, out total);
            return Json(new { state = 1, rows = list, total = total });
        }

        [HttpPost]
        [AdminSessionFilter(Order = 1)]
        [PermissionFilter(Order = 2, Permission = "00000000-0000-0000-9000-000000000003")]
        public JsonResult LoadNonHTEvaluateList(string CN, string MUDID, string DeliverTimeBegin, string DeliverTimeEnd, bool Large60, bool UnSafe, bool UnSend, int isNonHt, int rows, int page)
        {

            LogHelper.Info(Request.RawUrl);
            DateTime tmpTime;
            DateTime? DTBegin, DTEnd;
            if (DateTime.TryParse(DeliverTimeBegin, out tmpTime) == true)
            {
                DTBegin = tmpTime;
            }
            else
            {
                DTBegin = null;
            }
            if (DateTime.TryParse(DeliverTimeEnd, out tmpTime) == true)
            {
                DTEnd = tmpTime.AddDays(1d);
            }
            else
            {
                DTEnd = null;
            }
            int total;
            var list = orderService.LoadNonHTOrderEvaluatePage(CN, MUDID, DTBegin, DTEnd, Large60, UnSafe, UnSend, isNonHt, rows, page, out total);
            return Json(new { state = 1, rows = list, total = total });
        }

        //[AdminSessionFilter(Order = 1)]
        //[PermissionFilter(Order = 2, Permission = "00000000-0000-0000-1000-000000000003")]
        public void ExportEvaluateList(string CN, string MUDID, string DeliverTimeBegin, string DeliverTimeEnd, bool Large60, bool UnSafe, bool UnSend, int star, string channel)
        {
            LogHelper.Info(Request.RawUrl);
            DateTime tmpTime;
            DateTime? DTBegin, DTEnd;
            if (DateTime.TryParse(DeliverTimeBegin, out tmpTime) == true)
            {
                DTBegin = tmpTime;
            }
            else
            {
                DTBegin = null;
            }
            if (DateTime.TryParse(DeliverTimeEnd, out tmpTime) == true)
            {
                DTEnd = tmpTime.AddDays(1d);
            }
            else
            {
                DTEnd = null;
            }
            FileStream file11 = new FileStream(Server.MapPath("~/Template/Template_Evaluate.xls"), FileMode.Open, FileAccess.Read);
            HSSFWorkbook wk = new HSSFWorkbook(file11);
            ISheet sheet = wk.GetSheet("Report");

            var list = orderService.LoadOrderEvaluate(CN, MUDID, DTBegin, DTEnd, Large60, UnSafe, UnSend, 0, star, channel);
            if (list != null && list.Count > 0)
            {
                P_ORDER_EVALUATE_VIEW_EXT disItm;
                int dataCnt = list.Count;
                for (int i = 1; i <= dataCnt; i++)
                {
                    var item = list[i - 1];
                    IRow row = sheet.CreateRow(i);
                    ICell cell = null;
                    DateTime dateTimePara;
                    disItm = GetDisplay(list[i - 1]);

                    #region data cell
                    cell = row.CreateCell(0);
                    cell.SetCellValue(disItm.CN);// "预申请表CN号");
                    cell = row.CreateCell(1);
                    cell.SetCellValue(disItm.UserName);//"MUDID");
                    cell = row.CreateCell(2);
                    cell.SetCellValue(disItm.MUDID);// "订餐人姓名");
                    cell = row.CreateCell(3);
                    if (!string.IsNullOrEmpty(disItm.DeliverDate))
                    {
                        DateTime.TryParse(disItm.DeliverDate, out dateTimePara);
                        cell.SetCellValue(dateTimePara);
                    }
                    cell = row.CreateCell(4);
                    cell.SetCellValue(disItm.DeliverTime);// "餐厅名称");
                    cell = row.CreateCell(5);
                    cell.SetCellValue(disItm.Channel);// "评分");
                    cell = row.CreateCell(6);
                    cell.SetCellValue(disItm.RestCode);// "评分");
                    cell = row.CreateCell(7);
                    cell.SetCellValue(disItm.RestName);// "准点率");
                    cell = row.CreateCell(8);
                    cell.SetCellValue(disItm.UNOnTimeRemark);// "食品安全存在问题");
                    cell = row.CreateCell(9);
                    cell.SetCellValue(disItm.EOnTime);// "餐品卫生及新鲜");
                    cell = row.CreateCell(10);
                    cell.SetCellValue(disItm.EOnTimeRemark);// "餐品包装");
                    cell = row.CreateCell(11);
                    cell.SetCellValue(disItm.EIsSafe);// "餐品性价比");
                    cell = row.CreateCell(12);
                    cell.SetCellValue(disItm.EIsSafeRemark);// "餐品性价比");
                    cell = row.CreateCell(13);
                    cell.SetCellValue(disItm.EHealth);// "餐品性价比");
                    cell = row.CreateCell(14);
                    cell.SetCellValue(disItm.EHealthRemark);// "餐品性价比");
                    cell = row.CreateCell(15);
                    cell.SetCellValue(disItm.EPack);// "餐品性价比");
                    cell = row.CreateCell(16);
                    cell.SetCellValue(disItm.EPackRemark);// "餐品性价比");
                    cell = row.CreateCell(17);
                    cell.SetCellValue(disItm.ECost);// "餐品性价比");
                    cell = row.CreateCell(18);
                    cell.SetCellValue(disItm.ECostRemark);// "餐品性价比");
                    cell = row.CreateCell(19);
                    cell.SetCellValue(disItm.EOtherRemark);// "餐品性价比");
                    cell = row.CreateCell(20);
                    cell.SetCellValue(disItm.EStar);// "餐品性价比");
                    cell = row.CreateCell(21);
                    if (!string.IsNullOrEmpty(disItm.AppDate))
                    {
                        DateTime.TryParse(disItm.AppDate, out dateTimePara);
                        cell.SetCellValue(dateTimePara);
                    }
                    cell = row.CreateCell(22);
                    cell.SetCellValue(disItm.AppTime);// "餐品性价比");
                    #endregion
                }

                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    wk.Write(ms);
                    Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xls", DateTime.Now.ToString("yyyyMMddHHmmssfff")));
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.BinaryWrite(ms.ToArray());
                }
            }
        }

        public ActionResult ExportNonEvaluateList(string CN, string MUDID, string DeliverTimeBegin, string DeliverTimeEnd, bool Large60, bool UnSafe, bool UnSend)
        {
            LogHelper.Info(Request.RawUrl);
            DateTime tmpTime;
            DateTime? DTBegin, DTEnd;
            if (DateTime.TryParse(DeliverTimeBegin, out tmpTime) == true)
            {
                DTBegin = tmpTime;
            }
            else
            {
                DTBegin = null;
            }
            if (DateTime.TryParse(DeliverTimeEnd, out tmpTime) == true)
            {
                DTEnd = tmpTime.AddDays(1d);
            }
            else
            {
                DTEnd = null;
            }

            var list = orderService.LoadNonHTOrderEvaluate(CN, MUDID, DTBegin, DTEnd, Large60, UnSafe, UnSend, 1);
            if (list != null && list.Count > 0)
            {
                XSSFWorkbook book = new XSSFWorkbook();
                #region var headerStyle = book.CreateCellStyle();
                var headerStyle = book.CreateCellStyle();

                var headerFontStyle = book.CreateFont();
                headerFontStyle.Color = NPOI.HSSF.Util.HSSFColor.White.Index;
                headerFontStyle.Boldweight = short.MaxValue;
                headerFontStyle.FontHeightInPoints = 18;

                headerStyle.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;
                headerStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightOrange.Index;
                headerStyle.Alignment = HorizontalAlignment.Center;
                headerStyle.SetFont(headerFontStyle);
                #endregion
                var sheet = book.CreateSheet("report");
                var row = sheet.CreateRow(0);
                #region header
                sheet.SetColumnWidth(0, 20 * 256);
                sheet.SetColumnWidth(1, 20 * 256);
                sheet.SetColumnWidth(2, 20 * 256);
                sheet.SetColumnWidth(3, 20 * 256);
                sheet.SetColumnWidth(4, 30 * 256);
                sheet.SetColumnWidth(5, 15 * 256);
                sheet.SetColumnWidth(6, 30 * 256);
                sheet.SetColumnWidth(7, 25 * 256);
                sheet.SetColumnWidth(8, 20 * 256);
                sheet.SetColumnWidth(9, 20 * 256);
                sheet.SetColumnWidth(10, 20 * 256);

                var cell = row.CreateCell(0);
                cell.SetCellValue("PO No.");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(1);
                cell.SetCellValue("MUDID");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(2);
                cell.SetCellValue("订餐人姓名");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(3);
                cell.SetCellValue("送餐日期");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(4);
                cell.SetCellValue("餐厅名称");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(5);
                cell.SetCellValue("评分");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(6);
                cell.SetCellValue("准点率");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(7);
                cell.SetCellValue("食品安全存在问题");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(8);
                cell.SetCellValue("餐品卫生及新鲜");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(9);
                cell.SetCellValue("餐品包装");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(10);
                cell.SetCellValue("餐品性价比");
                cell.CellStyle = headerStyle;

                #endregion
                #region var dataCellStyle = book.CreateCellStyle();
                var dataCellStyle = book.CreateCellStyle();
                var dataFontStyle = book.CreateFont();
                dataFontStyle.Color = NPOI.HSSF.Util.HSSFColor.Black.Index;
                dataFontStyle.Boldweight = short.MaxValue;
                dataFontStyle.FontHeightInPoints = 16;

                dataCellStyle.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;
                dataCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.White.Index;
                dataCellStyle.SetFont(dataFontStyle);
                #endregion

                P_ORDER_EVALUATE_VIEW_EXT disItm;
                int dataCnt = list.Count;
                for (int i = 0; i < dataCnt; i++)
                {
                    disItm = GetDisplay(list[i]);
                    row = sheet.CreateRow(1 + i);
                    #region data cell
                    cell = row.CreateCell(0);
                    cell.SetCellValue(disItm.PO);// "预申请表CN号");
                    cell.CellStyle = dataCellStyle;
                    cell = row.CreateCell(1);
                    cell.SetCellValue(disItm.MUDID);//"MUDID");
                    cell.CellStyle = dataCellStyle;
                    cell = row.CreateCell(2);
                    cell.SetCellValue(disItm.UserName);// "订餐人姓名");
                    cell.CellStyle = dataCellStyle;
                    cell = row.CreateCell(3);
                    cell.SetCellValue(disItm.DeliverDate);// "送餐日期");
                    cell.CellStyle = dataCellStyle;
                    cell = row.CreateCell(4);
                    cell.SetCellValue(disItm.RestName);// "餐厅名称");
                    cell.CellStyle = dataCellStyle;
                    cell = row.CreateCell(5);
                    cell.SetCellValue(disItm.EStar);// "评分");
                    cell.CellStyle = dataCellStyle;
                    cell = row.CreateCell(6);
                    cell.SetCellValue(disItm.EOnTime);// "准点率");
                    cell.CellStyle = dataCellStyle;
                    cell = row.CreateCell(7);
                    cell.SetCellValue(disItm.EIsSafe);// "食品安全存在问题");
                    cell.CellStyle = dataCellStyle;
                    cell = row.CreateCell(8);
                    cell.SetCellValue(disItm.EHealth);// "餐品卫生及新鲜");
                    cell.CellStyle = dataCellStyle;
                    cell = row.CreateCell(9);
                    cell.SetCellValue(disItm.EPack);// "餐品包装");
                    cell.CellStyle = dataCellStyle;
                    cell = row.CreateCell(10);
                    cell.SetCellValue(disItm.ECost);// "餐品性价比");
                    cell.CellStyle = dataCellStyle;
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
                return File(excelData, "application/vnd.ms-excel", HttpUtility.UrlEncode("Report_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx", System.Text.Encoding.UTF8));
            }
            else
            {
                ViewBag.Msg = "无符合条件数据！";
                return View();
            }
        }

        private P_ORDER_EVALUATE_VIEW_EXT GetDisplay(P_ORDER_EVALUATE_VIEW itm)
        {
            P_ORDER_EVALUATE_VIEW_EXT rtnData = new P_ORDER_EVALUATE_VIEW_EXT();
            rtnData.ID = itm.ID;
            rtnData.CN = itm.CN;
            rtnData.PO = itm.PO;
            rtnData.MUDID = itm.MUDID;
            rtnData.UserName = string.IsNullOrEmpty(itm.DCUserName) == false ? itm.DCUserName : (itm.DCUserName != null ? itm.DCUserName : string.Empty);
            rtnData.DeliverDate = itm.SendTime != null ? itm.SendTime.ToString("yyyy-MM-dd") : string.Empty;
            rtnData.DeliverTime = itm.SendTime != null ? itm.SendTime.ToString("HH:mm:ss") : string.Empty;
            rtnData.RestName = itm.RestName != null ? itm.RestName : string.Empty;
            rtnData.EStar = (itm.EStar != 99 && itm.EStar != 0) ? (itm.EStar + "星") : string.Empty;

            if (itm.EStar != 99)
            {
                switch (itm.EOnTime)
                {
                    case 1:
                        rtnData.EOnTime = "迟到60分钟以上"; break;
                    case 2:
                        rtnData.EOnTime = "迟到60分钟以内"; break;
                    case 3:
                        rtnData.EOnTime = "早到"; break;
                    case 4:
                        rtnData.EOnTime = "准点"; break;
                    case 5:
                        rtnData.EOnTime = "迟到60分及以上"; break;
                    case 6:
                        rtnData.EOnTime = "迟到30-60分钟"; break;
                    case 7:
                        rtnData.EOnTime = "迟到30分钟以内"; break;
                    case 8:
                        rtnData.EOnTime = "早到30分钟及以上"; break;
                    case 9:
                        rtnData.EOnTime = "早到30分钟以内"; break;
                    case 10:
                        rtnData.EOnTime = "准点"; break;
                    default:
                        rtnData.EOnTime = string.Empty; break;
                }
            }
            else
            {
                rtnData.EOnTime = string.Empty;
            }

            rtnData.EIsSafe = (itm.EStar != 99 && itm.EStar != 0) ? (itm.EIsSafe == 1 ? "是" : "否") : string.Empty;

            if (itm.EHealth == 1)
            {
                rtnData.EHealth = "好";
            }
            else if (itm.EHealth == 2)
            {
                rtnData.EHealth = "中";
            }
            else if (itm.EHealth == 3)
            {
                rtnData.EHealth = "差";
            }
            else
            {
                rtnData.EHealth = string.Empty;
            }

            if (itm.EPack == 1)
            {
                rtnData.EPack = "好";
            }
            else if (itm.EPack == 2)
            {
                rtnData.EPack = "中";
            }
            else if (itm.EPack == 3)
            {
                rtnData.EPack = "差";
            }
            else
            {
                rtnData.EPack = string.Empty;
            }

            if (itm.ECost == 1)
            {
                rtnData.ECost = "好";
            }
            else if (itm.ECost == 2)
            {
                rtnData.ECost = "中";
            }
            else if (itm.ECost == 3)
            {
                rtnData.ECost = "差";
            }
            else
            {
                rtnData.ECost = string.Empty;
            }

            rtnData.RestCode = itm.ResCode;
            if (itm.State == 8)
            {
                rtnData.UNOnTimeRemark = itm.EOnTimeRemark;
                rtnData.EOnTimeRemark = string.Empty;
            }
            else
            {
                rtnData.UNOnTimeRemark = string.Empty;
                rtnData.EOnTimeRemark = itm.EOnTimeRemark;
            }
            rtnData.ECostRemark = itm.ECostRemark != null ? itm.ECostRemark : string.Empty;
            rtnData.EHealthRemark = itm.EHealthRemark != null ? itm.EHealthRemark : string.Empty;
            rtnData.EIsSafeRemark = itm.EIsSafeRemark != null ? itm.EIsSafeRemark : string.Empty;
            rtnData.EOtherRemark = itm.EOtherRemark != null ? itm.EOtherRemark : string.Empty;
            rtnData.EPackRemark = itm.EPackRemark != null ? itm.EPackRemark : string.Empty;
            rtnData.AppDate = itm.AppDate != null ? itm.AppDate.ToString("yyyy-MM-dd") : string.Empty;
            rtnData.AppTime = itm.AppDate != null ? itm.AppDate.ToString("HH:mm:ss") : string.Empty;
            rtnData.Channel = itm.Channel;

            return rtnData;
        }

        //[AdminSessionFilter(Order = 1)]
        //[PermissionFilter(Order = 2, Permission = "00000000-0000-0000-1000-000000000003")]
        public ActionResult EvaluateDetail(Guid OrderID)
        {
            var e = evaluateService.LoadByOrderID(OrderID);
            ViewBag.Evaluate = e;
            ViewBag.EvaluateSafeImage = GetImgList(e.SafeImage);
            ViewBag.EvaluatePackImage = GetImgList(e.PackImage);
            ViewBag.EvaluateHealthImage = GetImgList(e.HealthImage);
            ViewBag.EvaluateCostEffectiveImage = GetImgList(e.CostEffectiveImage);
            ViewBag.EvaluateOtherDiscrpionImage = GetImgList(e.OtherDiscrpionImage);
            return View();
        }
        #endregion

        public ActionResult EvaluateOldDetail(Guid OrderID)
        {
            var e = evaluateService.LoadByOldOrderID(OrderID);
            ViewBag.Evaluate = e;
            ViewBag.EvaluateSafeImage = GetImgList(e.SafeImage);
            ViewBag.EvaluatePackImage = GetImgList(e.PackImage);
            ViewBag.EvaluateHealthImage = GetImgList(e.HealthImage);
            ViewBag.EvaluateCostEffectiveImage = GetImgList(e.CostEffectiveImage);
            ViewBag.EvaluateOtherDiscrpionImage = GetImgList(e.OtherDiscrpionImage);
            return View();
        }

        //[AdminSessionFilter(Order = 1)]
        //[PermissionFilter(Order = 2, Permission = "00000000-0000-0000-1000-000000000003")]
        public ActionResult NonEvaluateDetail(Guid OrderID)
        {
            var e = evaluateService.LoadNonHTEvaluateByOrderID(OrderID);
            ViewBag.Evaluate = e;
            ViewBag.EvaluateSafeImage = GetImgList(e.SafeImage);
            ViewBag.EvaluatePackImage = GetImgList(e.PackImage);
            ViewBag.EvaluateHealthImage = GetImgList(e.HealthImage);
            ViewBag.EvaluateCostEffectiveImage = GetImgList(e.CostEffectiveImage);
            ViewBag.EvaluateOtherDiscrpionImage = GetImgList(e.OtherDiscrpionImage);
            return View();
        }

        private List<string> GetImgList(string StrImgs)
        {
            List<string> rtnData = new List<string>();
            if (string.IsNullOrEmpty(StrImgs) == false)
            {
                if (StrImgs.Contains(",") == true)
                {
                    var aryImg = StrImgs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    string img;
                    foreach (var i in aryImg)
                    {
                        img = WebConfig.AWSServiceUrl + i;
                        if (rtnData.Contains(img) == false)
                        {
                            rtnData.Add(img);
                        }
                    }
                }
                else
                {
                    rtnData.Add(WebConfig.AWSServiceUrl + StrImgs);
                }
            }
            return rtnData;
        }

        [HttpPost]
        //[AdminSessionFilter(Order = 1)]
        //[PermissionFilter(Order = 2, Permission = "00000000-0000-0000-1000-000000000001")]
        public JsonResult LoadNonHTSearchList(string CN, string MUDID, string HospitalCode, string RestaurantId, string CreateTimeBegin, string CreateTimeEnd, string DeliverTimeBegin, string DeliverTimeEnd, string State, string Supplier, int isNonHT, int rows, int page)
        {
            LogHelper.Info(Request.RawUrl);
            //var res = WxMessageClientChannelFactory.GetChannel().SendText("zhongda.fang", "今天天不错啊！" + CN);
            //LogHelper.Info(res);
            DateTime _tmpTime;
            DateTime? _DTBegin, _DTEnd;
            if (DateTime.TryParse(CreateTimeBegin, out _tmpTime) == true)
            {
                _DTBegin = _tmpTime;
            }
            else
            {
                _DTBegin = null;
            }
            if (DateTime.TryParse(CreateTimeEnd, out _tmpTime) == true)
            {
                _DTEnd = _tmpTime.AddDays(1d);
            }
            else
            {
                _DTEnd = null;
            }
            DateTime tmpTime;
            DateTime? DTBegin, DTEnd;
            if (DateTime.TryParse(DeliverTimeBegin, out tmpTime) == true)
            {
                DTBegin = tmpTime;
            }
            else
            {
                DTBegin = null;
            }
            if (DateTime.TryParse(DeliverTimeEnd, out tmpTime) == true)
            {
                DTEnd = tmpTime.AddDays(1d);
            }
            else
            {
                DTEnd = null;
            }
            int tmpInt;
            int? stateFlg;
            if (int.TryParse(State, out tmpInt) == false)
            {
                stateFlg = null;
            }
            else
            {
                stateFlg = tmpInt;
            }
            int total;
            string oldHospitalCode = PreApprovalService.GetOldGskHospitalCodeByGskHospitalCode(HospitalCode);
            if (!string.IsNullOrEmpty(oldHospitalCode))
            {
                HospitalCode += "," + oldHospitalCode;
            }
            var list = orderService.LoadNonHTOrderMntPage(CN, MUDID, HospitalCode, RestaurantId, _DTBegin, _DTEnd, DTBegin, DTEnd, stateFlg, Supplier, isNonHT, rows, page, out total);
            var rtnList = new List<P_ORDER_DAILY_VIEW_EXT>();
            foreach (var i in list)
            {
                rtnList.Add(GetNonHTDisplayObj(i));
            }
            return Json(new { state = 1, rows = rtnList, total = total });
        }

        #region 发送订单报表
        public JsonResult GetOrderCount(string CN, string MUDID, string TACode, string HospitalCode, string RestaurantId, string CostCenter,
            string CreateTimeBegin, string CreateTimeEnd, string DeliverTimeBegin, string DeliverTimeEnd, string State, string Supplier, string IsSpecialOrder, string RD)
        {
            try
            {
                int res = 0;
                DateTime _tmpTime;
                DateTime? _DTBegin, _DTEnd;
                if (DateTime.TryParse(CreateTimeBegin, out _tmpTime) == true)
                {
                    _DTBegin = _tmpTime;
                }
                else
                {
                    _DTBegin = null;
                }
                if (DateTime.TryParse(CreateTimeEnd, out _tmpTime) == true)
                {
                    _DTEnd = _tmpTime.AddDays(1d);
                }
                else
                {
                    _DTEnd = null;
                }
                DateTime tmpTime;
                DateTime? DTBegin, DTEnd;
                if (DateTime.TryParse(DeliverTimeBegin, out tmpTime) == true)
                {
                    DTBegin = tmpTime;
                }
                else
                {
                    DTBegin = null;
                }
                if (DateTime.TryParse(DeliverTimeEnd, out tmpTime) == true)
                {
                    DTEnd = tmpTime.AddDays(1d);
                }
                else
                {
                    DTEnd = null;
                }
                int tmpInt;
                int? stateFlg;
                if (int.TryParse(State, out tmpInt) == false)
                {
                    stateFlg = null;
                }
                else
                {
                    stateFlg = tmpInt;
                }
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
                res = orderService.GetOrderCount(CN, MUDID, TACode, HospitalCode, RestaurantId, CostCenter, _DTBegin, _DTEnd, DTBegin, DTEnd, stateFlg, Supplier, IsSpecialOrder, RD);
                return Json(new { state = 1, txt = res });
            }
            catch (Exception ex)
            {
                LogHelper.Error("订单报表获取数量失败---" + ex.Message);
                return Json(new { state = 0, txt = "邮件发送失败，请重试！" });
            }
        }
        public ActionResult AddRecipientOrder(string CN, string MUDID, string TACode, string HospitalCode, string RestaurantId, string CostCenter,
            string CreateTimeBegin, string CreateTimeEnd, string DeliverTimeBegin, string DeliverTimeEnd, string State, string Supplier, string IsSpecialOrder, string RD)
        {
            ViewBag.CN = CN;
            ViewBag.MUDID = MUDID;
            ViewBag.TACode = TACode;
            ViewBag.HospitalCode = HospitalCode;
            ViewBag.RestaurantId = RestaurantId;
            ViewBag.CostCenter = CostCenter;
            ViewBag.CreateTimeBegin = CreateTimeBegin;
            ViewBag.CreateTimeEnd = CreateTimeEnd;
            ViewBag.DeliverTimeBegin = DeliverTimeBegin;
            ViewBag.DeliverTimeEnd = DeliverTimeEnd;
            ViewBag.State = State;
            ViewBag.Supplier = Supplier;
            ViewBag.IsSpecialOrder = IsSpecialOrder;
            ViewBag.RD = RD;
            return View();
        }
        public JsonResult Send(string Recipient, string CCs, string CN, string MUDID, string TACode, string HospitalCode, string RestaurantId, string CostCenter,
            string CreateTimeBegin, string CreateTimeEnd, string DeliverTimeBegin, string DeliverTimeEnd, string State, string Supplier, string IsSpecialOrder, string RD)
        {
            try
            {
                DateTime _tmpTime;
                DateTime? _DTBegin, _DTEnd;
                if (DateTime.TryParse(CreateTimeBegin, out _tmpTime) == true)
                {
                    _DTBegin = _tmpTime;
                }
                else
                {
                    _DTBegin = null;
                }
                if (DateTime.TryParse(CreateTimeEnd, out _tmpTime) == true)
                {
                    _DTEnd = _tmpTime.AddDays(1d);
                }
                else
                {
                    _DTEnd = null;
                }
                DateTime tmpTime;
                DateTime? DTBegin, DTEnd;
                if (DateTime.TryParse(DeliverTimeBegin, out tmpTime) == true)
                {
                    DTBegin = tmpTime;
                }
                else
                {
                    DTBegin = null;
                }
                if (DateTime.TryParse(DeliverTimeEnd, out tmpTime) == true)
                {
                    DTEnd = tmpTime.AddDays(1d);
                }
                else
                {
                    DTEnd = null;
                }
                int tmpInt;
                int? stateFlg;
                if (int.TryParse(State, out tmpInt) == false)
                {
                    stateFlg = null;
                }
                else
                {
                    stateFlg = tmpInt;
                }
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
                var list = orderService.LoadOrderReport(CN, MUDID, TACode, HospitalCode, RestaurantId, CostCenter, _DTBegin, _DTEnd, DTBegin, DTEnd, stateFlg, Supplier, IsSpecialOrder, RD);
                var resList = list.Where(p => p.ORDState != 5 && p.ORDState != 11).ToList();
                if (resList != null && resList.Count > 0)
                {
                    #region 构建excel
                    FileStream file11 = new FileStream(Server.MapPath("~/Template/Template_OrderReports.xls"), FileMode.Open, FileAccess.Read);
                    //FileStream file11 = new FileStream(Server.MapPath("/Template/Template_HT_PlatFormOrderReports.xls"), FileMode.Open, FileAccess.Read);
                    //XSSFWorkbook wk = new XSSFWorkbook(file11);
                    HSSFWorkbook wk = new HSSFWorkbook(file11);

                    ISheet sheet = wk.GetSheet("Report");

                    HT_ORDER_REPORT_VIEW_EXT disItm;
                    int dataCnt = resList.Count;
                    for (int i = 1; i <= dataCnt; i++)
                    {
                        IRow row = sheet.CreateRow(i);
                        ICell cell = null;

                        disItm = GetDisplayObj(resList[i - 1]);
                        row = sheet.CreateRow(i);
                        #region data cell
                        var a = 0;
                        cell = row.CreateCell(a);                   // 申请人姓名
                        cell.SetCellValue(disItm.ApplierName);
                        cell = row.CreateCell(++a);             //申请人MUDID
                        cell.SetCellValue(disItm.ApplierMUDID);
                        cell = row.CreateCell(++a);             //Territory Code
                        cell.SetCellValue(disItm.MRTerritoryCode);
                        cell = row.CreateCell(++a);            //申请人职位
                        cell.SetCellValue(disItm.Position);
                        cell = row.CreateCell(++a);             //申请人手机号码
                        cell.SetCellValue(disItm.ApplierMobile);
                        cell = row.CreateCell(++a);             //预申请申请日期
                        cell.SetCellValue(disItm.PRECreateDate);
                        cell = row.CreateCell(++a);             //预申请申请时间
                        cell.SetCellValue(disItm.PRECreateTime);
                        cell = row.CreateCell(++a);             //预申请修改日期
                        cell.SetCellValue(disItm.PREModifyDate);
                        cell = row.CreateCell(++a);             //预申请修改时间
                        cell.SetCellValue(disItm.PREModifyTime);
                        cell = row.CreateCell(++a);             //HT编号
                        cell.SetCellValue(disItm.HTCode);
                        cell = row.CreateCell(++a);             //Market
                        cell.SetCellValue(disItm.Market);
                        cell = row.CreateCell(++a);
                        cell.SetCellValue(disItm.VeevaMeetingID);//VeevaMeetingID
                        cell = row.CreateCell(++a);             //TA
                        cell.SetCellValue(disItm.TA);

                        cell = row.CreateCell(++a);             //省份
                        cell.SetCellValue(disItm.Province);
                        cell = row.CreateCell(++a);             //城市
                        cell.SetCellValue(disItm.City);
                        cell = row.CreateCell(++a);             //医院编码
                        cell.SetCellValue(disItm.HospitalCode);
                        cell = row.CreateCell(++a);             //医院名称
                        cell.SetCellValue(disItm.HospitalName);
                        cell = row.CreateCell(++a);             //医院地址
                        cell.SetCellValue(disItm.HospitalAddress);
                        cell = row.CreateCell(++a);             //会议名称
                        cell.SetCellValue(disItm.MeetingName);
                        cell = row.CreateCell(++a);             //会议日期
                        cell.SetCellValue(disItm.MeetingDate);
                        cell = row.CreateCell(++a);             //会议时间
                        cell.SetCellValue(disItm.MeetingTime);
                        cell = row.CreateCell(++a);             //参会人数
                        cell.SetCellValue(disItm.PREAttendCount);
                        cell = row.CreateCell(++a);             //成本中心
                        cell.SetCellValue(disItm.CostCenter);

                        cell = row.CreateCell(++a);             //预算金额
                        double BudgetTotal;
                        double.TryParse(disItm.BudgetTotal.ToString(), out BudgetTotal);
                        cell.SetCellValue(BudgetTotal);
                        cell = row.CreateCell(++a);             //直线经理是否随访
                        cell.SetCellValue(disItm.IsDMFollow);
                        cell = row.CreateCell(++a);             //外部免费讲者来讲
                        cell.SetCellValue(disItm.IsFreeSpeaker);
                        //cell = row.CreateCell(++a);             //RDSD Name
                        //cell.SetCellValue(disItm.RDSDName);
                        //cell = row.CreateCell(++a);             //RDSD MUDID
                        //cell.SetCellValue(disItm.RDSDMUDID);
                        //20190122
                        //if (Convert.ToInt32(disItm.PREAttendCount) < 60 && Convert.ToDouble(disItm.BudgetTotal) < 1200)
                        //{
                        //    cell = row.CreateCell(++a);             //预申请审批人姓名
                        //    cell.SetCellValue("系统自动审批");
                        //    cell = row.CreateCell(++a);             //预申请审批人MUDID
                        //    cell.SetCellValue("系统自动审批");
                        //}
                        //    //else if (Convert.ToDouble(disItm.BudgetTotal) < 2000 && disItm.CurrentApproverMUDID == null)
                        //    //{
                        //    //    cell = row.CreateCell(++a);
                        //    //    cell.SetCellValue("系统自动审批");// "预申请审批人姓名");
                        //    //    cell = row.CreateCell(++a);
                        //    //    cell.SetCellValue("系统自动审批");// "预申请审批人MUDID");
                        //    //}
                        //    else if (Convert.ToInt32(disItm.PREAttendCount) >= 1200 && Convert.ToInt32(disItm.PREAttendCount) < 1500)
                        //    {

                        //        cell = row.CreateCell(++a);             //预申请审批人姓名
                        //        cell.SetCellValue(disItm.CurrentApproverName);
                        //        cell = row.CreateCell(++a);             //预申请审批人MUDID
                        //        cell.SetCellValue(disItm.CurrentApproverMUDID);
                        //    }
                        //    else if (Convert.ToInt32(disItm.PREAttendCount) > 1500 && disItm.PREState == "5")
                        //    {
                        //        cell = row.CreateCell(++a);             //预申请审批人姓名
                        //        cell.SetCellValue(disItm.CurrentApproverName);
                        //        cell = row.CreateCell(++a);             //预申请审批人MUDID
                        //        cell.SetCellValue(disItm.CurrentApproverMUDID);
                        //    }
                        //    else
                        //{
                        //    cell = row.CreateCell(++a);             //预申请审批人姓名
                        //    cell.SetCellValue(disItm.PREBUHeadName);
                        //    cell = row.CreateCell(++a);             //预申请审批人MUDID
                        //    cell.SetCellValue(disItm.PREBUHeadMUDID);
                        //}
                        cell = row.CreateCell(++a);             //RD Territory Code
                        cell.SetCellValue(disItm.RDTerritoryCode);
                        cell = row.CreateCell(++a);             //预申请审批人姓名
                        cell.SetCellValue(disItm.PREBUHeadName1);
                        cell = row.CreateCell(++a);             //预申请审批人MUDID
                        cell.SetCellValue(disItm.PREBUHeadMUDID1);
                        cell = row.CreateCell(++a);             //预申请审批日期
                        cell.SetCellValue(disItm.PREBUHeadApproveDate);
                        cell = row.CreateCell(++a);             //预申请审批时间
                        cell.SetCellValue(disItm.PREBUHeadApproveTime);
                        cell = row.CreateCell(++a);             //预申请审批状态
                        cell.SetCellValue(disItm.PREState);
                        cell = row.CreateCell(++a);             //预申请是否重新分配审批人
                        cell.SetCellValue(disItm.PREIsReAssign);
                        cell = row.CreateCell(++a);             //预申请重新分配审批人-操作人
                        cell.SetCellValue(disItm.PREReAssignOperatorName);
                        cell = row.CreateCell(++a);             //预申请重新分配审批人-操作人MUDID
                        cell.SetCellValue(disItm.PREReAssignOperatorMUDID);
                        cell = row.CreateCell(++a);             //预申请被重新分配审批人姓名
                        cell.SetCellValue(disItm.PREReAssignBUHeadName);
                        cell = row.CreateCell(++a);             //预申请被重新分配审批人MUDID
                        cell.SetCellValue(disItm.PREReAssignBUHeadMUDID);
                        cell = row.CreateCell(++a);             //预申请重新分配审批人日期
                        cell.SetCellValue(disItm.PREReAssignBUHeadApproveDate);
                        cell = row.CreateCell(++a);             //预申请重新分配审批人时间
                        cell.SetCellValue(disItm.PREReAssignBUHeadApproveTime);
                        cell = row.CreateCell(++a);             //供应商
                        cell.SetCellValue(disItm.Channel);
                        cell = row.CreateCell(++a);             //订单号
                        cell.SetCellValue(disItm.EnterpriseOrderId);
                        cell = row.CreateCell(++a);             //下单日期
                        cell.SetCellValue(disItm.ORDCreateDate);
                        cell = row.CreateCell(++a);             //下单时间
                        cell.SetCellValue(disItm.ORDCreateTime);
                        cell = row.CreateCell(++a);             //送餐日期
                        cell.SetCellValue(disItm.ORDDeliverDate);

                        cell = row.CreateCell(++a);             //送餐时间
                        cell.SetCellValue(disItm.ORDDeliverTime);
                        cell = row.CreateCell(++a);             //餐厅编码
                        cell.SetCellValue(disItm.RestaurantId);
                        cell = row.CreateCell(++a);             //餐厅名称
                        cell.SetCellValue(disItm.RestaurantName);
                        cell = row.CreateCell(++a);             //用餐人数
                        cell.SetCellValue(disItm.ORDAttendCount);
                        cell = row.CreateCell(++a);             //预订金额
                        double TotalPrice;
                        double.TryParse(disItm.TotalPrice.ToString(), out TotalPrice);
                        cell.SetCellValue(TotalPrice);
                        cell = row.CreateCell(++a);             //实际金额
                        double totalFee;
                        double.TryParse(disItm.totalFee.ToString(), out totalFee);
                        cell.SetCellValue(totalFee);
                        cell = row.CreateCell(++a);             //实际金额调整原因
                        cell.SetCellValue(disItm.feeModifyReason);
                        cell = row.CreateCell(++a);             //送餐地址
                        cell.SetCellValue(disItm.DeliveryAddress);
                        cell = row.CreateCell(++a);             //收餐人姓名
                        cell.SetCellValue(disItm.Consignee);

                        cell = row.CreateCell(++a);             //收餐人电话
                        cell.SetCellValue(disItm.Phone);
                        cell = row.CreateCell(++a);             //下单备注
                        cell.SetCellValue(disItm.Remark);
                        cell = row.CreateCell(++a);             //是否预定成功
                        cell.SetCellValue(disItm.bookState);
                        cell = row.CreateCell(++a);             //预定成功日期
                        cell.SetCellValue(disItm.ORDReceiveDate);
                        cell = row.CreateCell(++a);             //预定成功时间
                        cell.SetCellValue(disItm.ORDReceiveTime);
                        cell = row.CreateCell(++a);             //是否申请退单
                        cell.SetCellValue(disItm.IsRetuen);
                        cell = row.CreateCell(++a);             //是否退单成功
                        cell.SetCellValue(disItm.cancelState);
                        cell = row.CreateCell(++a);             //退单失败平台发起配送需求
                        cell.SetCellValue(disItm.IsRetuenSuccess);
                        cell = row.CreateCell(++a);             //退单失败反馈配送需求
                        cell.SetCellValue(disItm.cancelFeedback);
                        cell = row.CreateCell(++a);             //预定/退单失败原因
                        cell.SetCellValue(disItm.cancelFailReason);

                        cell = row.CreateCell(++a);             //是否收餐/未送达
                        cell.SetCellValue(disItm.ReceiveState);
                        cell = row.CreateCell(++a);             //确认收餐日期
                        cell.SetCellValue(disItm.ReceiveDate);
                        cell = row.CreateCell(++a);             //确认收餐时间
                        cell.SetCellValue(disItm.ReceiveTime);
                        cell = row.CreateCell(++a);             //用户确认金额
                        double RealPrice;
                        double.TryParse(disItm.RealPrice.ToString(), out RealPrice);
                        cell.SetCellValue(RealPrice);
                        cell = row.CreateCell(++a);             //是否与预定餐品一致
                        cell.SetCellValue(disItm.IsMealSame);
                        cell = row.CreateCell(++a);             //用户确认金额调整原因
                        cell.SetCellValue(disItm.RealPriceChangeReason);
                        cell = row.CreateCell(++a);             //用户确认金额调整备注
                        cell.SetCellValue(disItm.RealPriceChangeRemark);
                        cell = row.CreateCell(++a);             //实际用餐人数
                        cell.SetCellValue(disItm.RealCount);
                        cell = row.CreateCell(++a);             //实际用餐人数调整原因
                        cell.SetCellValue(disItm.RealCountChangeReason);
                        cell = row.CreateCell(++a);             //实际用餐人数调整备注
                        cell.SetCellValue(disItm.RealCountChangeRemrak);
                        //0326
                        //cell = row.CreateCell(++a);             //未送达描述
                        //cell.SetCellValue(disItm.EUnTimeDesc);

                        //cell = row.CreateCell(++a);             //准点率
                        //cell.SetCellValue(disItm.EOnTime);
                        //cell = row.CreateCell(++a);             //准点率描述
                        //cell.SetCellValue(disItm.EOnTimeDesc);
                        //cell = row.CreateCell(++a);             //食品安全存在问题
                        //cell.SetCellValue(disItm.EIsSafe);
                        //cell = row.CreateCell(++a);             //食品安全问题描述
                        //cell.SetCellValue(disItm.EIsSafeDesc);
                        //cell = row.CreateCell(++a);             //餐品卫生及新鲜
                        //cell.SetCellValue(disItm.EHealth);
                        //cell = row.CreateCell(++a);             //餐品卫生描述
                        //cell.SetCellValue(disItm.EHealthDesc);
                        //cell = row.CreateCell(++a);             //餐品包装
                        //cell.SetCellValue(disItm.EPack);
                        //cell = row.CreateCell(++a);             //餐品包装描述
                        //cell.SetCellValue(disItm.EPackDesc);
                        //cell = row.CreateCell(++a);             //餐品性价比
                        //cell.SetCellValue(disItm.ECost);
                        //cell = row.CreateCell(++a);             //餐品性价比描述
                        //cell.SetCellValue(disItm.ECostDesc);

                        //cell = row.CreateCell(++a);             //其他评价
                        //cell.SetCellValue(disItm.EOtherDesc);
                        //cell = row.CreateCell(++a);             //在线评分
                        //cell.SetCellValue(disItm.EStar);
                        //cell = row.CreateCell(++a);             //评论日期
                        //cell.SetCellValue(disItm.ECreateDate);
                        //cell = row.CreateCell(++a);             //评论时间
                        //cell.SetCellValue(disItm.ECreateTime);
                        //0326
                        cell = row.CreateCell(++a);             //1=同一医院当日多场
                        cell.SetCellValue(disItm.TYYYDRDC);
                        cell = row.CreateCell(++a);             //2=同一代表当日多场
                        cell.SetCellValue(disItm.TYDBDRDC);
                        cell = row.CreateCell(++a);             //3=同一餐厅当日多场
                        cell.SetCellValue(disItm.TYCTDRDC);
                        cell = row.CreateCell(++a);             //4=同一代表同一医院当日多场
                        cell.SetCellValue(disItm.TYDBTYYYDRDC);
                        cell = row.CreateCell(++a);             //5=同一代表同一餐厅当日多场
                        cell.SetCellValue(disItm.TYDBTYCTDRDC);
                        cell = row.CreateCell(++a);             //6=同一代表同一医院同一餐厅当日多场
                        cell.SetCellValue(disItm.TYDBTYYYTYCTDRDC);

                        cell = row.CreateCell(++a);             //7=参会人数>=60
                        cell.SetCellValue(disItm.CHRSDYLS);
                        cell = row.CreateCell(++a);             //8=代表自提
                        cell.SetCellValue(disItm.customerPickup);
                        //cell = row.CreateCell(++a);             //直线经理姓名
                        //cell.SetCellValue(disItm.PREBUHeadName);
                        //cell = row.CreateCell(++a);             //直线经理MUDID
                        //cell.SetCellValue(disItm.PREBUHeadMUDID);
                        //cell = row.CreateCell(++a);             //Level2 Manager 姓名
                        //cell.SetCellValue(disItm.PREBUHeadName);
                        //cell = row.CreateCell(++a);             //Level2 Manager MUDID
                        //cell.SetCellValue(disItm.PREBUHeadMUDID);
                        //cell = row.CreateCell(++a);             //Level3 Manager 姓名
                        //cell.SetCellValue(disItm.PREBUHeadName);
                        //cell = row.CreateCell(++a);             //Level3 Manager MUDID
                        //cell.SetCellValue(disItm.PREBUHeadMUDID);
                        cell = row.CreateCell(++a);             //是否上传文件
                        cell.SetCellValue(disItm.IsOrderUpload);

                        cell = row.CreateCell(++a);             //上传文件提交日期
                        cell.SetCellValue(disItm.PUOCreateDate);
                        cell = row.CreateCell(++a);             //上传文件提交时间
                        cell.SetCellValue(disItm.PUOCreateTime);
                        cell = row.CreateCell(++a);             //上传文件审批直线经理姓名
                        cell.SetCellValue(disItm.PUOBUHeadName);
                        cell = row.CreateCell(++a);             //上传文件审批直线经理MUDID
                        cell.SetCellValue(disItm.PUOBUHeadMUDID);
                        cell = row.CreateCell(++a);             //上传文件审批日期
                        cell.SetCellValue(disItm.ApproveDate);
                        cell = row.CreateCell(++a);             //上传文件审批时间
                        cell.SetCellValue(disItm.ApproveTime);
                        cell = row.CreateCell(++a);             //上传文件审批状态
                        cell.SetCellValue(disItm.PUOState);
                        cell = row.CreateCell(++a);             //签到人数是否等于实际用餐人数
                        cell.SetCellValue(disItm.IsAttentSame);
                        //cell = row.CreateCell(++a);               
                        //cell.SetCellValue(disItm.RealCount);
                        //cell.CellStyle = dataCellStyle;
                        cell = row.CreateCell(++a);             //签到人数调整原因
                        cell.SetCellValue(disItm.AttentSameReason);
                        cell = row.CreateCell(++a);             //是否与会议信息一致
                        cell.SetCellValue(disItm.IsMeetingInfoSame);
                        cell = row.CreateCell(++a);             //会议信息不一致原因
                        cell.SetCellValue(disItm.MeetingInfoSameReason);
                        cell = row.CreateCell(++a);             //上传文件是否Re-Open
                        cell.SetCellValue(disItm.IsReopen);


                        //0326
                        //cell = row.CreateCell(++a);             //上传文件Re-Open操作人
                        //cell.SetCellValue(disItm.ReopenOperatorName);
                        //cell = row.CreateCell(++a);             //上传文件Re-Open操作人MUDID
                        //cell.SetCellValue(disItm.ReopenOperatorMUDID);
                        //0326

                        cell = row.CreateCell(++a);             //上传文件Re-Open日期
                        cell.SetCellValue(disItm.ReopenOperateDate);

                        //0326
                        //cell = row.CreateCell(++a);             //上传文件Re-Open时间
                        //cell.SetCellValue(disItm.ReopenOperateTime);
                        //0326


                        cell = row.CreateCell(++a);             //上传文件Re-Open发起人姓名
                        cell.SetCellValue(disItm.ReopenOperatorName);
                        cell = row.CreateCell(++a);             //上传文件Re-Open发起人MUDID
                        cell.SetCellValue(disItm.ReopenOperatorMUDID);
                        cell = row.CreateCell(++a);             //上传文件Re-Open原因
                        cell.SetCellValue(disItm.ReopenReason);
                        cell = row.CreateCell(++a);             //上传文件Re-Open备注 
                        cell.SetCellValue(disItm.ReopenRemark);
                        cell = row.CreateCell(++a);             //上传文件Re-Open审批状态
                        cell.SetCellValue(disItm.ReopenState);
                        cell = row.CreateCell(++a);             //上传文件是否重新分配
                        cell.SetCellValue(disItm.IsTransfer);

                        //0326
                        //cell = row.CreateCell(++a);             //上传文件重新分配操作人
                        //cell.SetCellValue(disItm.TransferOperatorName);
                        //cell = row.CreateCell(++a);             //上传文件重新分配操作人MUDID
                        //cell.SetCellValue(disItm.TransferOperatorMUDID);
                        //0326

                        cell = row.CreateCell(++a);             //上传文件被重新分配人姓名
                        cell.SetCellValue(disItm.TransferUserName);
                        cell = row.CreateCell(++a);             //上传文件被重新分配人MUDID
                        cell.SetCellValue(disItm.TransferUserMUDID);
                        cell = row.CreateCell(++a);             //上传文件被重新分配日期
                        cell.SetCellValue(disItm.TransferOperateDate);


                        //0326
                        //cell = row.CreateCell(++a);             //上传文件被重新分配时间
                        //cell.SetCellValue(disItm.TransferOperateTime);
                        //0326

                        cell = row.CreateCell(++a);             //上传文件否重新分配审批人
                        cell.SetCellValue(disItm.IsReAssign);

                        //0326
                        //cell = row.CreateCell(++a);             //上传文件重新分配审批人-操作人
                        //cell.SetCellValue(disItm.ReAssignOperatorName);
                        //cell = row.CreateCell(++a);             //上传文件重新分配审批人-操作人MUDID
                        //cell.SetCellValue(disItm.ReAssignOperatorMUDID);
                        //0326
                        cell = row.CreateCell(++a);             //上传文件被重新分配审批人姓名
                        cell.SetCellValue(disItm.ReAssignBUHeadName);
                        cell = row.CreateCell(++a);             //上传文件被重新分配审批人MUDID
                        cell.SetCellValue(disItm.ReAssignBUHeadMUDID);
                        cell = row.CreateCell(++a);             //上传文件重新分配审批人日期
                        cell.SetCellValue(disItm.ReAssignBUHeadApproveDate);
                        //0326
                        //cell = row.CreateCell(++a);             //上传文件重新分配审批人时间
                        //cell.SetCellValue(disItm.ReAssignBUHeadApproveTime);
                        //0326
                        cell = row.CreateCell(++a);             //项目组特殊备注
                        cell.SetCellValue(disItm.SpecialReason);
                        cell = row.CreateCell(++a);             //是否完成送餐
                        cell.SetCellValue(disItm.IsCompleteDelivery);
                        cell = row.CreateCell(++a);             //GSK项目组确认金额
                        double confirmPrice;
                        double.TryParse(disItm.GSKConfirmAmount.ToString(), out confirmPrice);
                        cell.SetCellValue(confirmPrice);

                        //0326
                        //cell = row.CreateCell(++a);
                        //cell.SetCellValue(string.Empty);
                        //cell = row.CreateCell(++a);
                        //cell.SetCellValue(string.Empty);
                        //cell = row.CreateCell(++a);
                        //cell.SetCellValue(string.Empty);
                        //cell = row.CreateCell(++a);
                        //cell.SetCellValue(string.Empty);
                        //cell = row.CreateCell(++a);
                        //cell.SetCellValue(string.Empty);
                        //cell = row.CreateCell(++a);
                        //cell.SetCellValue(string.Empty);
                        //cell = row.CreateCell(++a);
                        //cell.SetCellValue(string.Empty);
                        //cell = row.CreateCell(++a);
                        //cell.SetCellValue(string.Empty);
                        //cell = row.CreateCell(++a);
                        //cell.SetCellValue(string.Empty);
                        //cell = row.CreateCell(++a);
                        //cell.SetCellValue(string.Empty);
                        //cell = row.CreateCell(++a);
                        //cell.SetCellValue(string.Empty);
                        //cell = row.CreateCell(++a);
                        //cell.SetCellValue(string.Empty);
                        //cell = row.CreateCell(++a);
                        //cell.SetCellValue(string.Empty);
                        //0326
                        #endregion
                    }
                    #endregion

                    #region 将报表上传S3，发送邮件
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    {
                        wk.Write(ms);
                        string url = string.Empty;
                        using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, config))
                        {
                            var key = string.Format("SendReport/{0}.xls", "Catering_Orders_Report_" + DateTime.Now.ToString("yyyyMMddHHmmssfff"));
                            var request = new PutObjectRequest()
                            {
                                BucketName = bucketName,
                                CannedACL = S3CannedACL.PublicRead,
                                Key = key,
                                InputStream = ms
                            };
                            client.PutObject(request);
                            //url = ConfigurationManager.AppSettings["AWSService"] + "/" + key;
                            url = "http://wechat.s3.cn-north-1.amazonaws.com.cn/" + key;
                        }
                        if (SendMail("Catering Orders Report", Recipient, CCs, url, ""))
                            return Json(new { state = 1, txt = "邮件已成功发送！" });
                        else
                            return Json(new { state = 0, txt = "邮件发送失败，请重试！" });
                    }
                    #endregion
                }
                else
                {
                    return Json(new { state = 1, txt = "无有效订单可发送！" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("订单报表发送失败---" + ex.Message);
                return Json(new { state = 0, txt = "邮件发送失败，请重试！" });
            }
        }
        public bool SendMail(string Subject, string mailTo, string mailCc, string Body, string FilePath)
        {
            string FROM = "cn.igsk@gsk.com";
            string FROMNAME = "Catering Service";
            string SMTP_USERNAME = "AKIAIHZIQK74DCRBRYMQ";
            string SMTP_PASSWORD = "Av567RSLSBZUtaDNe/oCeDdDGP/AaYpkSeAt7NtHPFGe";
            string HOST = "email-smtp.us-east-1.amazonaws.com";
            int PORT = 25;

            string SUBJECT = Subject;
            string BODY = "Dear,</br></br>"
                        + "&emsp;&emsp;Download fileptah as follow: " + Body + "</br></br></br>"
                        + "Best Regards</br></br>"
                        + "This is automatically sent by program. Please do not reply.";

            MailMessage message = new MailMessage();
            message.IsBodyHtml = true;
            message.From = new MailAddress(FROM, FROMNAME);
            //收件人
            string[] mailTos = mailTo.Split(';');
            foreach (string To in mailTos)
            {
                if (!string.IsNullOrEmpty(To))
                    message.To.Add(new MailAddress(To));
            }
            //抄送人
            string[] mailCcs = mailCc.Split(';');
            foreach (string Cc in mailCcs)
            {
                if (!string.IsNullOrEmpty(Cc))
                    message.CC.Add(new MailAddress(Cc));
            }
            message.Subject = SUBJECT;
            message.Body = BODY;

            //if (FilePath != "" && FilePath != null)
            //{
            //    message.Attachments.Add(new Attachment(FilePath));
            //}

            //MemoryStream attStream = ms;
            //attStream.Seek(0, SeekOrigin.Begin);
            //Attachment objAttachment = new Attachment(attStream, attName);
            //message.Attachments.Add(objAttachment);

            using (var client = new System.Net.Mail.SmtpClient(HOST, PORT))
            {
                // Pass SMTP credentials
                client.Credentials =
                    new NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);

                // Enable SSL encryption
                client.EnableSsl = true;

                try
                {
                    client.Send(message);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

    }
}