using MealAdmin.Entity;
using MealAdmin.Service;
using MealAdmin.Web.Filter;
using MealAdminApiClient;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using XFramework.XInject.Attributes;
using XFramework.XUtil;

namespace MealAdmin.Web.Areas.P.Controllers
{
    public class GroupController : AdminBaseController
    {

        [Bean("groupMemberService")]
        public IGroupMemberService groupMemberService { get; set; }
        [Bean("operationAuditService")]
        public IOperationAuditService operationAuditService { get; set; }

        [Bean("orderService")]
        public IOrderService orderService { get; set; }
        // GET: P/Group

        /// <summary>
        /// 投诉组
        /// </summary>
        /// <returns></returns>

        [AdminSessionFilter(Order = 1)]
        [PermissionFilter(Order = 2, Permission = "00000000-0000-0000-7000-000000000001")]
        public ActionResult Complaints()
        {
            ViewBag.PageTitle = "订餐投诉组";
            ViewBag.GroupType = GroupTypeEnum.Complaints.ToString("D");
            return View("GroupMember");
        }


        /// <summary>
        /// MMCoE
        /// </summary>
        /// <returns></returns>
        [AdminSessionFilter(Order = 1)]
        [PermissionFilter(Order = 2, Permission = "00000000-0000-0000-7000-000000000002")]
        public ActionResult MMCoE()
        {
            ViewBag.PageTitle = "MMCoE审批组";
            ViewBag.GroupType = GroupTypeEnum.MMCoE.ToString("D");
            return View("GroupMember");
        }

        /// <summary>
        /// 简报组
        /// </summary>
        /// <returns></returns>
        [AdminSessionFilter(Order = 1)]
        [PermissionFilter(Order = 2, Permission = "00000000-0000-0000-7000-000000000005")]
        public ActionResult BriefReport()
        {
            ViewBag.PageTitle = "简报发送组";
            ViewBag.GroupType = GroupTypeEnum.BriefReport.ToString("D");
            return View("GroupMember");
        }

        /// <summary>
        /// HT暂停服务组
        /// </summary>
        /// <returns></returns>
        [AdminSessionFilter(Order = 1)]
        [PermissionFilter(Order = 2, Permission = "00000000-0000-0000-7000-000000000003")]
        public ActionResult ServPause()
        {
            ViewBag.PageTitle = "HT暂停服务名单";
            ViewBag.GroupType = GroupTypeEnum.ServPause.ToString("D");
            return View("GroupMemberForServPause");
        }

        /// <summary>
        /// non-HT暂停服务组
        /// </summary>
        /// <returns></returns>
        [AdminSessionFilter(Order = 1)]
        [PermissionFilter(Order = 2, Permission = "00000000-0000-0000-7000-000000000009")]
        public ActionResult NServPause()
        {
            ViewBag.PageTitle = "non-HT暂停服务名单";
            ViewBag.GroupType = GroupTypeEnum.NServPause.ToString("D");
            return View("GroupMember");
        }


        /// <summary>
        /// 院外HT名单
        /// </summary>
        /// <returns></returns>
        [AdminSessionFilter(Order = 1)]
        [PermissionFilter(Order = 2, Permission = "00000000-0000-0000-7000-000000000004")]
        public ActionResult OutSideHT()
        {
            ViewBag.PageTitle = "院外HT名单";
            ViewBag.GroupType = GroupTypeEnum.OutSideHT.ToString("D");
            return View("GroupMember");
        }
        /// <summary>
        /// 变更审批人操作组
        /// </summary>
        /// <returns></returns>
        [AdminSessionFilter(Order = 1)]
        [PermissionFilter(Order = 2, Permission = "00000000-0000-0000-7000-000000000006")]
        public ActionResult ChangeAAg()
        {
            ViewBag.PageTitle = "变更审批人操作组";
            ViewBag.GroupType = GroupTypeEnum.ChangeAAG.ToString("D");
            return View("GroupMember");
        }
        /// <summary>
        /// 订单重新分配组
        /// </summary>
        /// <returns></returns>
        [AdminSessionFilter(Order = 1)]
        [PermissionFilter(Order = 2, Permission = "00000000-0000-0000-7000-000000000007")]
        public ActionResult RSGroup()
        {
            ViewBag.PageTitle = "订单重新分配组";
            ViewBag.GroupType = GroupTypeEnum.RSGroup.ToString("D");
            return View("GroupMember");
        }
        /// <summary>
        /// 成本中心管理组
        /// </summary>
        /// <returns></returns>
        [AdminSessionFilter(Order = 1)]
        [PermissionFilter(Order = 2, Permission = "00000000-0000-0000-7000-000000000008")]
        public ActionResult CCMGroup()
        {
            ViewBag.PageTitle = "成本中心管理组";
            ViewBag.GroupType = GroupTypeEnum.CCMGroup.ToString("D");
            return View("GroupMember");
        }

        private ActionResult GroupMember()
        {
            return View();
        }



        [AdminSessionFilter(Order = 1)]
        [HttpPost]
        public JsonResult Load(int GroupType, string UserId, string UserName, int rows, int page)
        {
            int total;
            var list = groupMemberService.LoadPage(GroupType, UserId, UserName, rows, page, out total);
            if (GroupType == 4)
            {
                string servPauseType = string.Empty;
                foreach (var item in list)
                {
                    servPauseType = string.Empty;
                    if (item.State1 == 1)
                    {
                        servPauseType = "未确认收餐，";
                    }
                    if (item.State2 == 1)
                    {
                        servPauseType += "确认收餐后未上传文件，";
                    }
                    if (item.State3 == 1)
                    {
                        servPauseType += "上传文件未审批，";
                    }
                    if (servPauseType.Length > 1)
                    {
                        servPauseType = servPauseType.Substring(0, servPauseType.Length - 1);
                    }
                    item.ServPauseType = servPauseType;
                }
            }
            return Json(new { state = 1, rows = list, total = total });
        }

        [AdminSessionFilter(Order = 1)]
        public ActionResult InputGroupMember(int GroupType)
        {
            ViewBag.GroupType = GroupType;
            return View();
        }

        [AdminSessionFilter(Order = 1)]
        public JsonResult SentBF()
        {
            try
            {
                var arr = groupMemberService.GetGroupMembersByType(Entity.GroupTypeEnum.BriefReport).Select(s => s.UserId).ToArray();
                var touser = string.Join("|", arr);
                var brief = orderService.LoadBriefing(0);
                LogHelper.Info("总金额：" + brief.TomorrowDeliverTotal + "XMS金额：" + brief.TomorrowDeliverTotalXms.ToString() + "BDS金额：" + brief.TomorrowDeliverTotalBds.ToString());
                var cnt = "当日预申请审批通过数量:" + brief.TodayApprove + "(0元-" + brief.TodayApproveZero + "; 非0元-" + brief.TodayApproveNotZero + ")"
                   + "\r\n明日配送订单量: " + brief.TomorrowDeliver + "(XMS-" + brief.TomorrowDeliverXms + ";\t BDS-" + brief.TomorrowDeliverBds + ")"
                   + "\r\n明日配送订单金额: RMB" + brief.TomorrowDeliverTotal.ToString("n") + "(XMS- RMB" + brief.TomorrowDeliverTotalXms.ToString("n") + ";\t BDS- RMB" + brief.TomorrowDeliverTotalBds.ToString("n") + ")"
                   + "\r\n明日配送订单,参会人数>=60人: " + brief.TomorrowAttendCount60
                   + "\r\n明日配送订单,预定金额>=1500元: " + brief.TomorrowExceed2000
                   + "\r\n当日确认收餐数量:" + brief.TodayConfirmOrder
                   + "\r\n当日上传文件审批通过数量:" + brief.TodayUpLoadThroughCount;
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var rtnVal = WxMessageClientChannelFactory.GetChannel().SendText(touser, cnt);
                        LogHelper.Info("manual send BriefReport Content：【" + JsonConvert.SerializeObject(brief) + "】, | toUser: 【" + touser + "】，| result:" + rtnVal);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("manual send BriefReport  ERR", ex);
                        throw ex;
                    }
                });

                return Json(new { state = 1 });

            }
            catch (Exception ex)
            {
                LogHelper.Error("P/Group/SentBF(0)  manual send BriefReport  ERR", ex);
                return Json(new { state = 1, txt = ex.Message });
            }
        }

        [HttpPost]
        [AdminSessionFilter(Order = 1)]
        public JsonResult SaveInputMember(int GroupType, string MudIDs)
        {
            var strIds = MudIDs.Trim();
            var aryIds = strIds.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            List<string> unSuccessIds;
            var updCnt = groupMemberService.Add(aryIds, GroupType, DateTime.Now, CurrentAdminUser.Email, 0, out unSuccessIds);

            if (unSuccessIds.Count == 0)
            {
                for (int i = 0; i < updCnt; i++)
                {
                    if (GroupType == 1)
                    {
                        var cont = "订单投诉组-添加人员:" + aryIds[i];
                        var num = operationAuditService.AddAudit("4", cont);
                    }
                    if (GroupType == 2)
                    {
                        var cont = "MMCoE审批组-添加人员:" + aryIds[i];
                        var num = operationAuditService.AddAudit("4", cont);
                    }
                    if (GroupType == 3)
                    {
                        var cont = "简报发送组-添加人员:" + aryIds[i];
                        var num = operationAuditService.AddAudit("4", cont);
                    }
                    if (GroupType == 4)
                    {
                        var cont = "HT暂停服务名单-添加人员:" + aryIds[i];
                        var num = operationAuditService.AddAudit("4", cont);
                    }
                    if (GroupType == 5)
                    {
                        var cont = "院外HT名单-添加人员:" + aryIds[i];
                        var num = operationAuditService.AddAudit("4", cont);
                    }
                    if (GroupType == 6)
                    {
                        var cont = "non-HT暂停服务名单-添加人员:" + aryIds[i];
                        var num = operationAuditService.AddAudit("4", cont);
                    }
                    if (GroupType == 7)
                    {
                        var cont = "变更审批人操作组-添加人员:" + aryIds[i];
                        var num = operationAuditService.AddAudit("4", cont);
                    }
                    if (GroupType == 8)
                    {
                        var cont = "订单重新分配租-添加人员:" + aryIds[i];
                        var num = operationAuditService.AddAudit("4", cont);
                    }
                    if (GroupType == 9)
                    {
                        var cont = "成本中心管理组-添加人员:" + aryIds[i];
                        var num = operationAuditService.AddAudit("4", cont);
                    }
                }
                return Json(new { state = 1 });
            }
            else
            {
                string _txt;
                if (updCnt == 0)
                {
                    _txt = "匹配导入失败，MUDID全未匹配成功！";
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
                    _txt = "匹配导入部分失败，MUDID " + unSuccessIds.Count + "条未匹配成功！" + "[" + res + "]";
                }

                return Json(new { state = 0, txt = _txt });
            }
        }

        [AdminSessionFilter(Order = 1)]
        public JsonResult ClearMembers(int GroupType)
        {
            var cnt = groupMemberService.DelByGroupType(GroupType);
            if (cnt > 0)
            {
                if (GroupType == 1)
                {
                    var cont = "订单投诉组清空";
                    var num = operationAuditService.AddAudit("4", cont);
                }
                if (GroupType == 2)
                {
                    var cont = "MMCoE审批组清空";
                    var num = operationAuditService.AddAudit("4", cont);
                }
                if (GroupType == 3)
                {
                    var cont = "简报发送组清空";
                    var num = operationAuditService.AddAudit("4", cont);
                }
                if (GroupType == 4)
                {
                    var cont = "HT暂停服务名单清空";
                    var num = operationAuditService.AddAudit("4", cont);
                }
                if (GroupType == 5)
                {
                    var cont = "院外HT名单清空";
                    var num = operationAuditService.AddAudit("4", cont);
                }
                if (GroupType == 6)
                {
                    var cont = "non-HT暂停服务名单清空";
                    var num = operationAuditService.AddAudit("4", cont);
                }
                if (GroupType == 7)
                {
                    var cont = "变更审批人操作组清空";
                    var num = operationAuditService.AddAudit("4", cont);
                }
                if (GroupType == 8)
                {
                    var cont = "订单重新分配租清空";
                    var num = operationAuditService.AddAudit("4", cont);
                }
                if (GroupType == 9)
                {
                    var cont = "成本中心管理组清空";
                    var num = operationAuditService.AddAudit("4", cont);
                }
            }
            return Json(new { state = 1 });
        }


        [AdminSessionFilter(Order = 1)]
        public JsonResult DeleteMember(Guid MemberID, int GroupType, string UserId)
        {
            var cnt = groupMemberService.DelByMemberID(MemberID, GroupType);
            if (cnt == 1)
            {
                if (GroupType == 1)
                {
                    var cont = "订单投诉组-删除人员:" + UserId;
                    var num = operationAuditService.AddAudit("4", cont);
                }
                if (GroupType == 2)
                {
                    var cont = "MMCoE审批组-删除人员:" + UserId;
                    var num = operationAuditService.AddAudit("4", cont);
                }
                if (GroupType == 3)
                {
                    var cont = "简报发送组-删除人员:" + UserId;
                    var num = operationAuditService.AddAudit("4", cont);
                }
                if (GroupType == 4)
                {
                    var cont = "HT暂停服务名单-删除人员:" + UserId;
                    var num = operationAuditService.AddAudit("4", cont);
                }
                if (GroupType == 5)
                {
                    var cont = "院外HT名单-删除人员:" + UserId;
                    var num = operationAuditService.AddAudit("4", cont);
                }
                if (GroupType == 6)
                {
                    var cont = "non-HT暂停服务名单-删除人员:" + UserId;
                    var num = operationAuditService.AddAudit("4", cont);
                }
                if (GroupType == 7)
                {
                    var cont = "变更审批人操作组-删除人员:" + UserId;
                    var num = operationAuditService.AddAudit("4", cont);
                }
                if (GroupType == 8)
                {
                    var cont = "订单重新分配租-删除人员:" + UserId;
                    var num = operationAuditService.AddAudit("4", cont);
                }
                if (GroupType == 9)
                {
                    var cont = "成本中心管理组-删除人员:" + UserId;
                    var num = operationAuditService.AddAudit("4", cont);
                }
                return Json(new { state = 1 });
            }
            else
            {
                return Json(new { state = 0 });
            }
        }


        #region 组别管理导出功能
        public ActionResult ExportGroupList(string MUDID, string Name, string GroupType)
        {
            int intGroupType = int.Parse(GroupType);
            if (intGroupType == 4)
            {
                var list = groupMemberService.ExportServPauseGroupList(MUDID, Name, intGroupType).OrderByDescending(p => p.ServPauseCreateDate).ToList();
                //FileStream file11 = new FileStream(Server.MapPath("/MealPlatformAdmin/Template/Template_HT_ServPauseReport.xls"), FileMode.Open, FileAccess.Read);
                FileStream file11 = new FileStream(Server.MapPath("~/Template/Template_HT_ServPauseReport.xls"), FileMode.Open, FileAccess.Read);
                HSSFWorkbook wk = new HSSFWorkbook(file11);

                ISheet sheet = wk.GetSheet("Report");
                int dataCnt = list.Count;
                for (int i = 1; i <= dataCnt; i++)
                {
                    IRow row = sheet.CreateRow(i);
                    ICell cell = null;

                    var disItm = list[i - 1];
                    row = sheet.CreateRow(i);
                    #region data cell
                    var a = 0;
                    cell = row.CreateCell(a);                   // 申请人姓名
                    cell.SetCellValue(disItm.ApplierName);
                    cell = row.CreateCell(++a);             //申请人MUDID
                    cell.SetCellValue(disItm.ApplierMUDID);
                    cell = row.CreateCell(++a);            //申请人职位
                    cell.SetCellValue(disItm.Position);
                    cell = row.CreateCell(++a);             //申请人手机号码
                    cell.SetCellValue(disItm.ApplierMobile);
                    cell = row.CreateCell(++a);             //HT编号
                    cell.SetCellValue(disItm.HTCode);
                    cell = row.CreateCell(++a);             //Market
                    cell.SetCellValue(disItm.Market);
                    cell = row.CreateCell(++a);             //TA
                    cell.SetCellValue(disItm.TA);
                    cell = row.CreateCell(++a);             //送餐日期
                    cell.SetCellValue(disItm.DeliverDate);
                    cell = row.CreateCell(++a);             //送餐时间
                    cell.SetCellValue(disItm.DeliverTime.ToString());
                    cell = row.CreateCell(++a);             //是否收餐/未送达
                    cell.SetCellValue(disItm.ReceiveState);
                    cell = row.CreateCell(++a);             //确认收餐日期
                    if (disItm.ReceiveDate.Ticks != 0)
                    {
                        cell.SetCellValue(disItm.ReceiveDate);
                    }
                    cell = row.CreateCell(++a);             //确认收餐时间
                    if (disItm.ReceiveTime.Ticks != 0)
                    {
                        cell.SetCellValue(disItm.ReceiveTime.ToString());
                    }
                    cell = row.CreateCell(++a);             //直线经理姓名
                    cell.SetCellValue(disItm.DMName);
                    cell = row.CreateCell(++a);             //直线经理MUDID
                    cell.SetCellValue(disItm.DMMUDID);
                    cell = row.CreateCell(++a);             //Level2 Manager 姓名
                    cell.SetCellValue(disItm.RMName);
                    cell = row.CreateCell(++a);             //Level2 Manager MUDID
                    cell.SetCellValue(disItm.RMMUDID);
                    cell = row.CreateCell(++a);             //Level3 Manager 姓名
                    cell.SetCellValue(disItm.RDName);
                    cell = row.CreateCell(++a);             //Level3 Manager MUDID
                    cell.SetCellValue(disItm.RDMUDID);
                    cell = row.CreateCell(++a);             //是否上传文件
                    cell.SetCellValue(disItm.IsOrderUpload);
                    cell = row.CreateCell(++a);             //上传文件提交日期
                    if (disItm.OrderUploadDate.Ticks != 0)
                    {
                        cell.SetCellValue(disItm.OrderUploadDate);
                    }
                    cell = row.CreateCell(++a);             //上传文件提交时间
                    if (disItm.OrderUploadTime.Ticks != 0)
                    {
                        cell.SetCellValue(disItm.OrderUploadTime.ToString());
                    }
                    cell = row.CreateCell(++a);             //上传文件审批直线经理姓名
                    cell.SetCellValue(disItm.BUHeadName);
                    cell = row.CreateCell(++a);             //上传文件审批直线经理MUDID
                    cell.SetCellValue(disItm.BUHeadMUDID);
                    cell = row.CreateCell(++a);             //上传文件审批日期
                    if (disItm.BUHeadApproveDate.Ticks != 0)
                    {
                        cell.SetCellValue(disItm.BUHeadApproveDate);
                    }
                    cell = row.CreateCell(++a);             //上传文件审批时间
                    if (disItm.BUHeadApproveTime.Ticks != 0)
                    {
                        cell.SetCellValue(disItm.BUHeadApproveTime.ToString());
                    }
                    cell = row.CreateCell(++a);             //上传文件审批状态
                    cell.SetCellValue(disItm.OrderUploadState);
                    cell = row.CreateCell(++a);             //上传文件是否Re-Open
                    cell.SetCellValue(disItm.IsReopen);
                    cell = row.CreateCell(++a);             //上传文件Re-Open日期
                    if (disItm.ReopenOperateDate.Ticks != 0)
                    {
                        cell.SetCellValue(disItm.ReopenOperateDate);
                    }
                    cell = row.CreateCell(++a);             //上传文件Re-Open时间
                    if (disItm.ReopenOperateTime.Ticks != 0)
                    {
                        cell.SetCellValue(disItm.ReopenOperateTime.ToString());
                    }
                    cell = row.CreateCell(++a);             //上传文件Re-Open审批状态
                    cell.SetCellValue(disItm.ReopenOrderUploadState);
                    cell = row.CreateCell(++a);             //暂停服务类型
                    cell.SetCellValue(disItm.ServPauseType);
                    cell = row.CreateCell(++a);             //暂停服务日期
                    if (disItm.ServPauseCreateDate.Ticks != 0)
                    {
                        cell.SetCellValue(disItm.ServPauseCreateDate);
                    }
                    cell = row.CreateCell(++a);             //暂停服务时间
                    if (disItm.ServPauseCreateTime.Ticks != 0)
                    {
                        cell.SetCellValue(disItm.ServPauseCreateTime.ToString());
                    }
                    cell = row.CreateCell(++a);             //暂停服务状态
                    cell.SetCellValue(disItm.ServPauseState);
                    cell = row.CreateCell(++a);             //暂停服务开通时间
                    if (disItm.ServPauseModifyDate.Ticks != 0 && disItm.ServPauseState == "已开通服务")
                    {
                        cell.SetCellValue(disItm.ServPauseModifyDate);

                    }
                    cell = row.CreateCell(++a);             //暂停服务开通时间
                    if (disItm.ServPauseModifyTime.Ticks != 0 && disItm.ServPauseState == "已开通服务")
                    {
                        cell.SetCellValue(disItm.ServPauseModifyTime.ToString());
                    }
                    #endregion
                }
                byte[] excelData;
                using (System.IO.MemoryStream _ms = new System.IO.MemoryStream())
                {
                    wk.Write(_ms);
                    excelData = _ms.ToArray();
                    //_ms.Close();
                }

                var cont = "HT暂停服务名单导出";
                var num = operationAuditService.AddAudit("4", cont);

                ViewBag.Msg = "导出成功！";
                return File(excelData, "application/vnd.ms-excel", HttpUtility.UrlEncode("Group_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xls", System.Text.Encoding.UTF8));
            }
            else
            {
                var list = groupMemberService.ExportGroupList(MUDID, Name, intGroupType);
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



                    var cell = row.CreateCell(0);
                    cell.SetCellValue("MUDID");
                    cell.CellStyle = headerStyle;
                    cell = row.CreateCell(1);
                    cell.SetCellValue("姓名");
                    cell.CellStyle = headerStyle;
                    cell = row.CreateCell(2);
                    cell.SetCellValue("创建人");
                    cell.CellStyle = headerStyle;
                    cell = row.CreateCell(3);
                    cell.SetCellValue("创建时间");
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
                    P_GROUP_MEMBER disItm;
                    int dataCnt = list.Count;
                    for (int i = 0; i < dataCnt; i++)
                    {
                        disItm = list[i];
                        row = sheet.CreateRow(1 + i);
                        #region data cell
                        cell = row.CreateCell(0);
                        cell.SetCellValue(disItm.UserId);// "MUDID");
                        cell = row.CreateCell(1);
                        cell.SetCellValue(disItm.UserName);//"姓名");
                        cell = row.CreateCell(2);
                        cell.SetCellValue(disItm.CreateUserId);// "创建人");
                        cell = row.CreateCell(3);
                        cell.SetCellValue(disItm.CreateDate == null ? "" : disItm.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"));// "创建时间");
                        #endregion
                    }

                    byte[] excelData;
                    using (System.IO.MemoryStream _ms = new System.IO.MemoryStream())
                    {
                        book.Write(_ms);
                        excelData = _ms.ToArray();
                        //_ms.Close();
                    }
                    if (GroupType == "1")
                    {
                        var cont = "订单投诉组导出";
                        var num = operationAuditService.AddAudit("4", cont);
                    }
                    if (GroupType == "2")
                    {
                        var cont = "MMCoE审批组导出";
                        var num = operationAuditService.AddAudit("4", cont);
                    }
                    if (GroupType == "3")
                    {
                        var cont = "简报发送组导出";
                        var num = operationAuditService.AddAudit("4", cont);
                    }

                    if (GroupType == "5")
                    {
                        var cont = "院外HT名单导出";
                        var num = operationAuditService.AddAudit("4", cont);
                    }
                    if (GroupType == "6")
                    {
                        var cont = "non-HT暂停服务名单导出";
                        var num = operationAuditService.AddAudit("4", cont);
                    }
                    if (GroupType == "7")
                    {
                        var cont = "变更审批人操作组导出";
                        var num = operationAuditService.AddAudit("4", cont);
                    }
                    if (GroupType == "8")
                    {
                        var cont = "订单重新分配租导出:";
                        var num = operationAuditService.AddAudit("4", cont);
                    }
                    if (GroupType == "9")
                    {
                        var cont = "成本中心管理组导出";
                        var num = operationAuditService.AddAudit("4", cont);
                    }
                    ViewBag.Msg = "导出成功！";
                    return File(excelData, "application/vnd.ms-excel", HttpUtility.UrlEncode("Group_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx", System.Text.Encoding.UTF8));
                }
                else
                {
                    ViewBag.Msg = "无符合条件数据！";
                    return View();
                }
            }
        }
        #endregion

    }
}