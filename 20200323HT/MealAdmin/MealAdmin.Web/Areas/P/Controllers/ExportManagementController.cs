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
    public class ExportManagementController : AdminBaseController
    {

        [Bean("hospitalService")]
        public IHospitalService hospitalService { get; set; }
        [Bean("operationAuditService")]
        public IOperationAuditService operationAuditService { get; set; }

        [Bean("baseDataService")]
        public IBaseDataService baseDataService { get; set; }
        [Bean("groupMemberService")]
        public IGroupMemberService groupMemberService { get; set; }

        [Bean("exportManagementService")]
        public IExportManagementService exportManagementService { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        #region 特殊订单
        public ActionResult SpecialOrder()
        {
            return View();
        }

        public void ExportSpecialOrder(string DeliverTimeBegin, string DeliverTimeEnd, int SpecialOrderType)
        {
            try
            {
                #region 抓取数据   
                DateTime? _DTBegin, _DTEnd;
                DateTime _tmpTime;
                if (DateTime.TryParse(DeliverTimeBegin, out _tmpTime) == true)
                {
                    _DTBegin = _tmpTime;
                }
                else
                {
                    _DTBegin = null;
                }
                if (DateTime.TryParse(DeliverTimeEnd, out _tmpTime) == true)
                {
                    _DTEnd = _tmpTime.AddDays(1d);
                }
                else
                {
                    _DTEnd = null;
                }
                var summaryList = exportManagementService.ExportSpecialOrder(_DTBegin, _DTEnd, SpecialOrderType);
                var detailList = exportManagementService.LoadSpecialOrderDetail(_DTBegin, _DTEnd, SpecialOrderType);
                #endregion

                #region 构建Excel
                FileStream file11 = new FileStream(Server.MapPath("~/Template/Template_HT_Special_Order.xls"), FileMode.Open, FileAccess.Read);
                HSSFWorkbook wk = new HSSFWorkbook(file11);

                ISheet sheet = wk.GetSheet("Summary");
                ISheet sheetDetail = wk.GetSheet("Detail");
                ICellStyle cellstyle = wk.CreateCellStyle();
                cellstyle.VerticalAlignment = VerticalAlignment.Center;
                cellstyle.Alignment = HorizontalAlignment.Center;
                IDataFormat dataformat1 = wk.CreateDataFormat();
                cellstyle.DataFormat = dataformat1.GetFormat("#,###");

                #endregion

                #region 制作表体 Summary
                for (var i = 1; i <= summaryList.Count; i++)
                {
                    var item = summaryList[i - 1];
                    IRow row = sheet.CreateRow(i);
                    ICell cell = null;
                    var j = 0;
                    if (item != null)
                    {
                        cell = row.CreateCell(j);
                        cell.SetCellValue(item.TA);
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.CountUser);
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.CountOrder);
                        cell = row.CreateCell(++j);
                        double budgetTotal;
                        double.TryParse(item.CountPrice.ToString(), out budgetTotal);
                        cell.SetCellValue(budgetTotal);
                    }

                }
                #endregion

                #region 制作表体 Detail
                P_SPECIAL_ORDER_DETAIL_VIEW disItm;
                for (var j = 1; j <= detailList.Count; j++)
                {
                    var item = detailList[j - 1];
                    disItm = GetDisplayObj(detailList[j - 1]);
                    IRow row = sheetDetail.CreateRow(j);
                    ICell cell = null;
                    var a = 0;
                    cell = row.CreateCell(a);                   // 申请人姓名
                    cell.SetCellValue(disItm.ApplierName);
                    cell = row.CreateCell(++a);             //申请人MUDID
                    cell.SetCellValue(disItm.ApplierMUDID);
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
                    cell = row.CreateCell(++a);             //大区区域编码
                    cell.SetCellValue(disItm.CostCenter);

                    cell = row.CreateCell(++a);             //供应商
                    cell.SetCellValue(disItm.Channel);
                    cell = row.CreateCell(++a);             //送餐日期
                    cell.SetCellValue(disItm.DeliverDate);
                    cell = row.CreateCell(++a);             //送餐时间
                    cell.SetCellValue(disItm.DeliverTime);
                    cell = row.CreateCell(++a);             //餐厅编码
                    cell.SetCellValue(disItm.RestaurantId);
                    cell = row.CreateCell(++a);             //餐厅名称
                    cell.SetCellValue(disItm.RestaurantName);
                    cell = row.CreateCell(++a);             //实际用餐人数
                    if (item.RealCount.ToString() != "")
                    {
                        double zeroCount;
                        double.TryParse(item.RealCount.ToString(), out zeroCount);
                        cell.SetCellValue(zeroCount);
                    }
                    else
                    {
                        cell.SetCellValue("");
                    }//实际用餐人数
                    cell.CellStyle = cellstyle;
                    //cell.SetCellValue(disItm.RealCount);
                    cell = row.CreateCell(++a);             //是否收餐/未送达
                    cell.SetCellValue(disItm.ReceiveState);

                    cell = row.CreateCell(++a);             //GSK项目组确认金额
                    double budgetTotal;
                    double.TryParse(disItm.GSKConfirmAmount, out budgetTotal);
                    cell.SetCellValue(budgetTotal);

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
            catch (Exception ex)
            {
                LogHelper.Error("ExportSpecialOrder_" + ex.Message);
            }
        }


        private P_SPECIAL_ORDER_DETAIL_VIEW GetDisplayObj(P_SPECIAL_ORDER_DETAIL itm)
        {
            P_SPECIAL_ORDER_DETAIL_VIEW rtnData = new P_SPECIAL_ORDER_DETAIL_VIEW();
            rtnData.ApplierName = itm.ApplierName;
            rtnData.ApplierMUDID = itm.ApplierMUDID;
            rtnData.HTCode = itm.HTCode;
            rtnData.Market = itm.Market;
            rtnData.VeevaMeetingID = itm.VeevaMeetingID == null ? "" : itm.VeevaMeetingID;
            rtnData.TA = itm.TA;
            rtnData.Province = itm.Province;
            rtnData.City = itm.City;
            rtnData.HospitalCode = itm.HospitalCode;
            rtnData.HospitalName = itm.HospitalName;
            rtnData.HospitalAddress = itm.HospitalAddress;
            rtnData.CostCenter = itm.CostCenter;
            rtnData.Channel = itm.Channel != null ? itm.Channel.ToUpper() : string.Empty;
            rtnData.DeliverDate = itm.DeliverTime.ToString("yyyy/MM/dd");
            rtnData.DeliverTime = itm.DeliverTime.ToString("HH:mm:ss");
            rtnData.RestaurantId = itm.RestaurantId != null ? itm.RestaurantId : string.Empty;
            rtnData.RestaurantName = itm.RestaurantName != null ? itm.RestaurantName : string.Empty;
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
            rtnData.RealCount = itm.RealCount != null ? itm.RealCount : string.Empty;
            rtnData.GSKConfirmAmount = itm.GSKConfirmAmount != null ? itm.GSKConfirmAmount.ToString() : string.Empty;
            return rtnData;
        }
        #endregion

        #region 订单评价
        public ActionResult OrderEvaluate()
        {
            //加载TA
            var res = exportManagementService.LoadTA();
            List<string> list = new List<string>();
            foreach (var item in res)
            {
                list.Add(item.Name);
            }
            String join = String.Join(",", list);
            ViewBag.TAlist = join + ",R&D-Vx,R&D-Rx";
            return View();
        }
        //导出订单评价
        public void ExportOrderEvaluate(string DeliverTimeBegin, string DeliverTimeEnd, string TA, string supplier)
        {
            try
            {
                #region 抓取数据   
                DateTime? _DTBegin, _DTEnd;
                DateTime _tmpTime;
                string sltTA = "";
                string listcount = "";
                string ordercount = "";
                string nonlistcount = "";
                string nonordercount = "";
                double Erate = 0;
                int A = 0;
                int B = 0;
                int C = 0;
                int D = 0;

                int NonA = 0;
                int NonB = 0;
                int NonC = 0;
                int NonD = 0;
                double ListCount = 0;
                double OrderCount = 0;
                double NonListCount = 0;
                double NonOrderCount = 0;
                if (DateTime.TryParse(DeliverTimeBegin, out _tmpTime) == true)
                {
                    _DTBegin = _tmpTime;
                }
                else
                {
                    _DTBegin = null;
                }
                if (DateTime.TryParse(DeliverTimeEnd, out _tmpTime) == true)
                {
                    _DTEnd = _tmpTime.AddDays(1d);
                }
                else
                {
                    _DTEnd = null;
                }
                if (TA.Contains("ALL"))
                {
                    sltTA = null;
                }
                else
                {
                    sltTA = "'" + TA.Replace(",", "','") + "'";
                }
                //HT符合条件的已评价订单
                var list = exportManagementService.ExportOrderEvaluate(_DTBegin, _DTEnd, sltTA, supplier);
                //HT符合条件的有效订单
                var order = exportManagementService.ExportOrderCount(_DTBegin, _DTEnd, sltTA, supplier);
                //NonHT符合条件的已评价订单
                var nonlist = exportManagementService.ExportNonHTOrderEvaluate(_DTBegin, _DTEnd, sltTA, supplier);
                //NonHT符合条件的有效订单
                var nonorder = exportManagementService.ExportNonHTOrderCount(_DTBegin, _DTEnd, sltTA, supplier);

                #region HT评价
                if (list != null && list.Count > 0)
                {
                    ListCount = list.Count;
                    listcount = list.Count.ToString("N").Substring(0, list.Count.ToString("N").Length - 3);
                    for (int j = 0; j < list.Count; j++)
                    {
                        int score = 0;
                        int star = list[j].Star;//基础星星打分： 每颗星对应1分， 满星5分
                        if (star == 1)
                        {
                            score = score + 1;
                        }
                        if (star == 2)
                        {
                            score = score + 2;
                        }
                        if (star == 3)
                        {
                            score = score + 3;
                        }
                        if (star == 4)
                        {
                            score = score + 4;
                        }
                        if (star == 5)
                        {
                            score = score + 5;
                        }
                        int ontime = list[j].OnTime;
                        if (ontime == 5)
                        {
                            score = score + 1; //迟到60分钟及以上 1分
                        }
                        if (ontime == 6 || ontime == 7)
                        {
                            score = score + 2;//迟到30-60分钟 /迟到30分钟以内容 2分   
                        }
                        if (ontime == 8 || ontime == 9)
                        {
                            score = score + 3;//早到30分钟及以上 / 早到30分钟以内 3分
                        }
                        if (ontime == 10)
                        {
                            score = score + 5;//准点  5分
                        }
                        int safe = list[j].IsSafe;
                        if (safe == 1)
                        {
                            score = score + 1;//有问题
                        }
                        if (safe == 2)
                        {
                            score = score + 5;//无问题
                        }
                        int health = list[j].Health;
                        if (health == 1)
                        {
                            score = score + 5;//好
                        }
                        if (health == 2)
                        {
                            score = score + 3;//中
                        }
                        if (health == 3)
                        {
                            score = score + 1;//差
                        }
                        int pack = list[j].Pack;
                        if (pack == 1)
                        {
                            score = score + 5;//好
                        }
                        if (pack == 2)
                        {
                            score = score + 3;//中
                        }
                        if (pack == 3)
                        {
                            score = score + 1;//差
                        }
                        int cost = list[j].CostEffective;
                        if (cost == 1)
                        {
                            score = score + 5;//好
                        }
                        if (cost == 2)
                        {
                            score = score + 3;//中
                        }
                        if (cost == 3)
                        {
                            score = score + 1;//差
                        }
                        if (score >= 17 && score <= 30)
                        {
                            A = A + 1;
                        }
                        if (score >= 14 && score <= 16)
                        {
                            B = B + 1;
                        }
                        if (score >= 10 && score <= 13)
                        {
                            C = C + 1;
                        }
                        if (score >= 6 && score <= 9)
                        {
                            D = D + 1;
                        }
                        //if (score >= 1 && score <= 5)
                        //{
                        //    F = F + 1;
                        //}
                    }
                }
                else
                {
                    listcount = "0";
                }
                if (order != null && order.Count > 0)
                {
                    OrderCount = order.Count;
                    ordercount = order.Count.ToString("N").Substring(0, order.Count.ToString("N").Length - 3);
                }
                else
                {
                    ordercount = "0";
                }
                #endregion

                #region NonHT评价
                if (nonlist != null && nonlist.Count > 0)
                {
                    NonListCount = nonlist.Count;
                    nonlistcount = nonlist.Count.ToString("N").Substring(0, nonlist.Count.ToString("N").Length - 3);
                    for (int m = 0; m < nonlist.Count; m++)
                    {
                        int score = 0;
                        int star = nonlist[m].Star;//基础星星打分： 每颗星对应1分， 满星5分
                        if (star == 1)
                        {
                            score = score + 1;
                        }
                        if (star == 2)
                        {
                            score = score + 2;
                        }
                        if (star == 3)
                        {
                            score = score + 3;
                        }
                        if (star == 4)
                        {
                            score = score + 4;
                        }
                        if (star == 5)
                        {
                            score = score + 5;
                        }
                        int ontime = nonlist[m].OnTime;
                        if (ontime == 5)
                        {
                            score = score + 1; //迟到60分钟及以上 1分
                        }
                        if (ontime == 6 || ontime == 7)
                        {
                            score = score + 2;//迟到30-60分钟 /迟到30分钟以内容 2分   
                        }
                        if (ontime == 8 || ontime == 9)
                        {
                            score = score + 3;//早到30分钟及以上 / 早到30分钟以内 3分
                        }
                        if (ontime == 10)
                        {
                            score = score + 5;//准点  5分
                        }
                        int safe = nonlist[m].IsSafe;
                        if (safe == 1)
                        {
                            score = score + 1;//有问题
                        }
                        if (safe == 2)
                        {
                            score = score + 5;//无问题
                        }
                        int health = nonlist[m].Health;
                        if (health == 1)
                        {
                            score = score + 5;//好
                        }
                        if (health == 2)
                        {
                            score = score + 3;//中
                        }
                        if (health == 3)
                        {
                            score = score + 1;//差
                        }
                        int pack = nonlist[m].Pack;
                        if (pack == 1)
                        {
                            score = score + 5;//好
                        }
                        if (pack == 2)
                        {
                            score = score + 3;//中
                        }
                        if (pack == 3)
                        {
                            score = score + 1;//差
                        }
                        int cost = nonlist[m].CostEffective;
                        if (cost == 1)
                        {
                            score = score + 5;//好
                        }
                        if (cost == 2)
                        {
                            score = score + 3;//中
                        }
                        if (cost == 3)
                        {
                            score = score + 1;//差
                        }
                        if (score >= 17 && score <= 30)
                        {
                            NonA = NonA + 1;
                        }
                        if (score >= 14 && score <= 16)
                        {
                            NonB = NonB + 1;
                        }
                        if (score >= 10 && score <= 13)
                        {
                            NonC = NonC + 1;
                        }
                        if (score >= 6 && score <= 9)
                        {
                            NonD = NonD + 1;
                        }
                    }
                }
                else
                {
                    nonlistcount = "0";
                }
                if (nonorder != null && nonorder.Count > 0)
                {
                    NonOrderCount = nonorder.Count;
                    nonordercount = nonorder.Count.ToString("N").Substring(0, nonorder.Count.ToString("N").Length - 3);
                }
                else
                {
                    nonordercount = "0";
                }
                //订单总数
                double HTnonHTcount = Double.Parse(ordercount) + Double.Parse(nonordercount);
                #endregion
                #endregion

                #region 构建Excel
                FileStream file11 = new FileStream(Server.MapPath("~/Template/Template_HT_OrderEvaluate.xls"), FileMode.Open, FileAccess.Read);
                HSSFWorkbook wk = new HSSFWorkbook(file11);

                ICellStyle cellstyle = wk.CreateCellStyle();
                cellstyle.VerticalAlignment = VerticalAlignment.Center;
                cellstyle.Alignment = HorizontalAlignment.Center;
                IDataFormat dataformat1 = wk.CreateDataFormat();
                cellstyle.DataFormat = dataformat1.GetFormat("#,###");
                ISheet sheet = wk.GetSheet("report");
                #endregion

                #region 制作表体 
                IRow row = sheet.GetRow(1);
                ICell cell = null;
                var i = 0;
                string a = A.ToString("n").Substring(0, A.ToString("N").Length - 3);
                string b = B.ToString("n").Substring(0, B.ToString("N").Length - 3);
                string c = C.ToString("n").Substring(0, C.ToString("N").Length - 3);
                string d = D.ToString("n").Substring(0, D.ToString("N").Length - 3);
               
                string total = (A + B + C + D).ToString("N").Substring(0, (A + B + C + D).ToString("N").Length - 3);

                string nona = NonA.ToString();
                string nonb = NonB.ToString();
                string nonc = NonC.ToString();
                string nond = NonD.ToString();
                string nontotal = (NonA + NonB + NonC + NonD).ToString();

                #region 第0行
                cell = row.CreateCell(i);
                //订单总数              
                cell.SetCellValue(HTnonHTcount);
                cell.CellStyle = cellstyle;
                cell = row.CreateCell(++i);
                //HT订单总数
                double htcountorder;
                double.TryParse(ordercount, out htcountorder);
                cell.SetCellValue(htcountorder);                
                cell.CellStyle = cellstyle;
                cell = row.CreateCell(++i);
                //NonHT订单总数
                double nonhtcountorder;
                double.TryParse(nonordercount, out nonhtcountorder);
                cell.SetCellValue(nonhtcountorder);
                cell.CellStyle = cellstyle;
                cell = row.CreateCell(++i);
                //HT已评价订单数
                double htcountlist;
                double.TryParse(listcount, out htcountlist);
                cell.SetCellValue(htcountlist);
                cell.CellStyle = cellstyle;
                cell = row.CreateCell(++i);
                //NonHT已评价订单数
                double nonhtcountlist;
                double.TryParse(nonlistcount, out nonhtcountlist);
                cell.SetCellValue(nonhtcountlist);
                cell.CellStyle = cellstyle;

                cell = row.CreateCell(++i);
                if ((OrderCount + NonOrderCount) > 0)
                {
                    Erate = (ListCount + NonListCount) / (OrderCount + NonOrderCount);
                    cell.SetCellValue(Erate);     //评价率
                }
                else
                {
                    cell.SetCellValue("-");
                    cell.CellStyle = cellstyle;
                }
                #endregion

                #region 第一行
                IRow row3 = sheet.GetRow(3);
                cell = row3.CreateCell(3);
                //订单数（17-30分）HT
                double ac;
                double.TryParse(a, out ac);
                cell.SetCellValue(ac);
                cell.CellStyle = cellstyle;
                cell = row3.CreateCell(4);
                //订单数（17-30分）NonHT
                double nonac;
                double.TryParse(nona, out nonac);
                cell.SetCellValue(nonac);
                cell.CellStyle = cellstyle;

                cell = row3.CreateCell(5);
                if ((ListCount + NonListCount) > 0)
                {
                    double arate = (ac + nonac) / (ListCount + NonListCount);
                    cell.SetCellValue(arate); //百分比
                }
                else
                {
                    cell.SetCellValue("-");
                    cell.CellStyle = cellstyle;
                }
                #endregion

                #region 第二行
                IRow row4 = sheet.GetRow(4);
                cell = row4.CreateCell(3);
                //订单数（14-16分）HT
                double bc;
                double.TryParse(b, out bc);
                cell.SetCellValue(bc);
                cell.CellStyle = cellstyle;
                //订单数（14-16分）NonHT
                cell = row4.CreateCell(4);
                double nonbc;
                double.TryParse(nonb, out nonbc);
                cell.SetCellValue(nonbc);
                cell.CellStyle = cellstyle;
                cell = row4.CreateCell(5);
                if ((ListCount + NonListCount) > 0)
                {
                    double brate = (bc + nonbc) / (ListCount + NonListCount);
                    cell.SetCellValue(brate); //百分比
                }
                else
                {
                    cell.SetCellValue("-");
                    cell.CellStyle = cellstyle;
                }
                #endregion

                #region 第三行
                IRow row5 = sheet.GetRow(5);
                cell = row5.CreateCell(3);
                //订单数（10-13分）HT
                double cc;
                double.TryParse(c, out cc);
                cell.SetCellValue(cc);
                cell.CellStyle = cellstyle;
                //订单数（10-13分）NonHT
                cell = row5.CreateCell(4);
                double noncc;
                double.TryParse(nonc, out noncc);
                cell.SetCellValue(noncc);
                cell.CellStyle = cellstyle;
                cell = row5.CreateCell(5);
                if ((ListCount + NonListCount) > 0)
                {
                    double crate = (cc + noncc) / (ListCount + NonListCount);
                    cell.SetCellValue(crate); //百分比
                }
                else
                {
                    cell.SetCellValue("-");
                    cell.CellStyle = cellstyle;
                }
                #endregion

                #region 第四行
                IRow row6 = sheet.GetRow(6);
                cell = row6.CreateCell(3);
                //订单数（6-9分）HT
                double ccc;
                double.TryParse(d, out ccc);
                cell.SetCellValue(ccc);
                cell.CellStyle = cellstyle;
                //订单数（6-9分）NonHT
                cell = row6.CreateCell(4);
                double nondc;
                double.TryParse(nond, out nondc);
                cell.SetCellValue(nondc);
                cell.CellStyle = cellstyle;
                cell = row6.CreateCell(5);
                if ((ListCount + NonListCount) > 0)
                {
                    double drate = (ccc + nondc) / (ListCount + NonListCount);
                    cell.SetCellValue(drate); //百分比
                }
                else
                {
                    cell.SetCellValue("-");
                    cell.CellStyle = cellstyle;
                }
                #endregion

                #region 第五行
                IRow row7 = sheet.GetRow(7);
                cell = row7.CreateCell(3);
                //Total HT
                double totalc;
                double.TryParse(total, out totalc);
                cell.SetCellValue(totalc);
                cell.CellStyle = cellstyle;
                cell = row7.CreateCell(4);
                //Total NonHT
                double nontotalc;
                double.TryParse(nontotal, out nontotalc);
                cell.SetCellValue(nontotalc);
                cell.CellStyle = cellstyle;
                cell = row7.CreateCell(5);
                if ((ListCount + NonListCount) > 0)
                {                  
                    double trate = (totalc + nontotalc) / (ListCount + NonListCount);
                    cell.SetCellValue(trate); //百分比
                }
                else
                {
                    cell.SetCellValue("-");
                    cell.CellStyle = cellstyle;
                }
                #endregion
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
            catch (Exception ex)
            {
                LogHelper.Error("ExportOrderEvaluate" + ex.Message);
            }
        }
        #endregion

        #region Special Order
        public ActionResult SpecialOrderProportion()
        {
            var res = exportManagementService.LoadTA();
            List<string> list = new List<string>();
            foreach (var item in res)
            {
                list.Add(item.Name);
            }
            string join = string.Join(",", list);
            ViewBag.TAList = join;
            return View();
        }

        public JsonResult LoadTa()
        {
            var res = exportManagementService.LoadTA();
            return Json(new { state = 1, data = res });
        }

        public void ExportSpecialOrderProportion(string Year, string Month, string TA, string HTType, int SpecialOrderCnt, string ResCnt, int Proportion)
        {
            try
            {
                #region 抓取数据   
                string sltTA = string.Empty;
                string date = Year + "-" + Month;
                if (TA.Contains("ALL") || TA.Contains("null"))
                {
                    sltTA = null;
                }
                else
                {
                    sltTA = "'" + TA.Replace(",", "','") + "'";
                }
                var summaryList = exportManagementService.LoadSpecialOrderProportionSummary(date, sltTA, HTType, SpecialOrderCnt, ResCnt, Proportion);
                var detailList = exportManagementService.LoadSpecialOrderProportionDetail(date, sltTA, HTType, SpecialOrderCnt, ResCnt, Proportion);
                #endregion

                #region 构建Excel
                FileStream file11 = new FileStream(Server.MapPath("~/Template/Template_HT_Special_Order_Proportion.xls"), FileMode.Open, FileAccess.Read);
                HSSFWorkbook wk = new HSSFWorkbook(file11);

                ISheet sheet = wk.GetSheet("Summary");
                ISheet sheetDetail = wk.GetSheet("Detail");
                ICellStyle cellstyle = wk.CreateCellStyle();
                cellstyle.VerticalAlignment = VerticalAlignment.Center;
                cellstyle.Alignment = HorizontalAlignment.Center;
                IDataFormat dataformat1 = wk.CreateDataFormat();
                cellstyle.DataFormat = dataformat1.GetFormat("#,###");

                #endregion

                #region 制作表体 Summary
                for (var i = 1; i <= summaryList.Count; i++)
                {
                    var item = summaryList[i - 1];
                    IRow row = sheet.CreateRow(i);
                    ICell cell = null;
                    var j = 0;
                    if (item != null)
                    {
                        cell = row.CreateCell(j);
                        cell.SetCellValue(item.ApplierName);
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.UserId);
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.TA);

                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.HospitalId);
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.RestaurantId);
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.RestaurantName);
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.SpecialCnt);
                        cell = row.CreateCell(++j);
                        double budgetTotal;
                        double.TryParse(item.PriceCnt.ToString(), out budgetTotal);
                        cell.SetCellValue(budgetTotal);
                        cell = row.CreateCell(++j);
                        if (item.ResIdCnt == -1)
                            cell.SetCellValue("");
                        else
                            cell.SetCellValue(item.ResIdCnt);
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.OrdCnt);
                        cell = row.CreateCell(++j);
                        double proportion;
                        double.TryParse(item.Proportion.ToString(), out proportion);
                        cell.SetCellValue(proportion / 100);
                    }

                }
                #endregion

                #region 制作表体 Detail
                P_SPECIAL_ORDER_DETAIL_VIEW disItm;
                for (var j = 1; j <= detailList.Count; j++)
                {
                    var item = detailList[j - 1];
                    disItm = GetDisplayObj(detailList[j - 1]);
                    IRow row = sheetDetail.CreateRow(j);
                    ICell cell = null;
                    var a = 0;
                    cell = row.CreateCell(a);                   // 申请人姓名
                    cell.SetCellValue(disItm.ApplierName);
                    cell = row.CreateCell(++a);             //申请人MUDID
                    cell.SetCellValue(disItm.ApplierMUDID);
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
                    cell = row.CreateCell(++a);             //大区区域编码
                    cell.SetCellValue(disItm.CostCenter);

                    cell = row.CreateCell(++a);             //供应商
                    cell.SetCellValue(disItm.Channel);
                    cell = row.CreateCell(++a);             //送餐日期
                    cell.SetCellValue(disItm.DeliverDate);
                    cell = row.CreateCell(++a);             //送餐时间
                    cell.SetCellValue(disItm.DeliverTime);
                    cell = row.CreateCell(++a);             //餐厅编码
                    cell.SetCellValue(disItm.RestaurantId);
                    cell = row.CreateCell(++a);             //餐厅名称
                    cell.SetCellValue(disItm.RestaurantName);
                    cell = row.CreateCell(++a);
                    if (item.RealCount.ToString() != "")
                    {
                        double zeroCount;
                        double.TryParse(item.RealCount.ToString(), out zeroCount);
                        cell.SetCellValue(zeroCount);
                    }
                    else
                    {
                        cell.SetCellValue("");
                    }//实际用餐人数
                    cell.CellStyle = cellstyle;
                    //cell.SetCellValue(disItm.RealCount);
                    cell = row.CreateCell(++a);             //是否收餐/未送达
                    cell.SetCellValue(disItm.ReceiveState);

                    cell = row.CreateCell(++a);             //GSK项目组确认金额
                    double budgetTotal;
                    double.TryParse(disItm.GSKConfirmAmount, out budgetTotal);
                    cell.SetCellValue(budgetTotal);

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
            catch (Exception ex)
            {
                LogHelper.Error("ExportSpecialOrderProportion" + ex.Message);
            }
        }
        #endregion

        #region 特殊发票订单
        public ActionResult SpecialInvoiceOrder()
        {
            return View();
        }

        public void ExportSpecialInvoiceOrder(string Year, string Month, string Channel)
        {
            try
            {
                #region 抓取数据   
                string date = Year + "-" + Month + "-01";
                var specialInvoiceList = exportManagementService.LoadSpecialInvoiceList(date, Channel);
                var starbucksList = exportManagementService.LoadStarbucksList(date, Channel);
                var nonHTList = exportManagementService.LoadNonHTList(date, Channel);
                #endregion

                #region 构建Excel
                FileStream file11 = new FileStream(Server.MapPath("~/Template/Template_HT_Special_Invoice_Order.xls"), FileMode.Open, FileAccess.Read);
                HSSFWorkbook wk = new HSSFWorkbook(file11);

                ISheet sheet1 = wk.GetSheet("Sheet1");
                ISheet sheet2 = wk.GetSheet("sheet2");
                ISheet sheet3 = wk.GetSheet("sheet3");

                #endregion

                #region 制作表体 Sheet1
                for (var i = 1; i <= specialInvoiceList.Count; i++)
                {
                    var item = specialInvoiceList[i - 1];
                    IRow row = sheet1.CreateRow(i);
                    ICell cell = null;
                    var j = 0;
                    if (item != null)
                    {
                        cell = row.CreateCell(j);
                        cell.SetCellValue(item.Channel);
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.SpecialCnt);
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.NonSpecialCnt);
                    }

                }
                #endregion

                #region 制作表体 sheet2
                for (var i = 1; i <= starbucksList.Count; i++)
                {
                    var item = starbucksList[i - 1];
                    IRow row = sheet2.CreateRow(i);
                    ICell cell = null;
                    var j = 0;
                    if (item != null)
                    {
                        cell = row.CreateCell(j);
                        cell.SetCellValue(item.Channel);
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.OrderCnt);
                        cell = row.CreateCell(++j);
                        double budgetTotal;
                        double.TryParse(item.OrderPrice.ToString(), out budgetTotal);
                        cell.SetCellValue(budgetTotal);
                    }

                }
                #endregion

                #region 制作表体 sheet3
                for (var i = 1; i <= nonHTList.Count; i++)
                {
                    var item = nonHTList[i - 1];
                    IRow row = sheet3.CreateRow(i);
                    ICell cell = null;
                    var j = 0;
                    if (item != null)
                    {
                        cell = row.CreateCell(j);
                        cell.SetCellValue(item.Channel);
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.OrderCnt);
                        cell = row.CreateCell(++j);
                        double budgetTotal;
                        double.TryParse(item.OrderPrice.ToString(), out budgetTotal);
                        cell.SetCellValue(budgetTotal);
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
            catch (Exception ex)
            {
                LogHelper.Error("ExportSpecialInvoiceOrder" + ex.Message);
            }
        }
        #endregion


        #region 未完成订单
        public ActionResult UnfinishedOrder()
        {
            //加载TA
            var res = exportManagementService.LoadTA();
            List<string> list = new List<string>();
            foreach (var item in res)
            {
                list.Add(item.Name);
            }
            String join = String.Join(",", list);
            ViewBag.TAlist = join;
            return View();
        }
        //导出未完成订单
        public void ExportUnfinishedOrder(string StartYear, string StartMonth, string EndYear, string EndMonth, string TA, string HTType)
        {
            try
            {
                #region 抓取数据   
                string sltTA = "";
                string startdate = "";
                string enddate = "";
                if (StartYear != "" && StartMonth != "")
                {
                    startdate = StartYear + "-" + StartMonth + "-01";
                }
                else
                {
                    startdate = "";
                }
                if (EndYear != "" && EndMonth != "")
                {
                    int Endmonth = Convert.ToInt32(EndMonth) + 1;
                    string endmonth = Endmonth.ToString();
                    enddate = EndYear + "-" + endmonth + "-01";
                }
                else
                {
                    enddate = "";
                }
                if (TA.Contains("ALL"))
                {
                    sltTA = null;
                }
                else
                {
                    sltTA = "'" + TA.Replace(",", "','") + "'";
                }
                //符合条件的未完成订单
                var list = exportManagementService.ExportUnfinishedOrder(startdate, enddate, sltTA, HTType);
                #endregion
                #region 构建Excel
                FileStream file11 = new FileStream(Server.MapPath("~/Template/Template_HT_UnfinishedOrder.xls"), FileMode.Open, FileAccess.Read);
                HSSFWorkbook wk = new HSSFWorkbook(file11);

                ICellStyle cellstyle = wk.CreateCellStyle();
                cellstyle.VerticalAlignment = VerticalAlignment.Center;
                cellstyle.Alignment = HorizontalAlignment.Center;
                IDataFormat dataformat1 = wk.CreateDataFormat();
                cellstyle.DataFormat = dataformat1.GetFormat("#,###");
                ISheet sheet = wk.GetSheet("report");
                #endregion
                #region 制作表体 

                for (var i = 1; i <= list.Count; i++)
                {
                    var item = list[i - 1];
                    IRow row = sheet.CreateRow(i);
                    ICell cell = null;
                    #region data cell
                    var a = 0;
                    cell = row.CreateCell(a);
                    cell.SetCellValue(item.TA);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(item.ApplierName);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(item.ApplierMUDID);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    string unfinishedCount = item.UnfinishedCount.ToString("N").Substring(0, item.UnfinishedCount.ToString("N").Length - 3);
                    double unfinishedcount;
                    double.TryParse(unfinishedCount, out unfinishedcount);
                    cell.SetCellValue(unfinishedcount);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(item.IsWorkdayQuit);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(item.TransferUserMUDID);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(item.ReAssignBUHeadMUDID);
                    cell.CellStyle = cellstyle;
                    #endregion
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
            catch (Exception ex)
            {
                LogHelper.Error("ExportUnfinishedOrder" + ex.Message);
            }
        }

        //导出分析数据
        public void ExportUnfinishedData(string StartYear, string StartMonth, string EndYear, string EndMonth, string TA, string HTType)
        {
            try
            {
                #region 抓取数据   
                string sltTA = "";
                string startdate = "";
                string enddate = "";
                if (StartYear != "" && StartMonth != "")
                {
                    startdate = StartYear + "-" + StartMonth + "-01";
                }
                else
                {
                    startdate = "";
                }
                if (EndYear != "" && EndMonth != "")
                {
                    int Endmonth = Convert.ToInt32(EndMonth) + 1;
                    string endmonth = Endmonth.ToString();
                    enddate = EndYear + "-" + endmonth + "-01";
                }
                else
                {
                    enddate = "";
                }
                if (TA.Contains("ALL"))
                {
                    sltTA = null;
                }
                else
                {
                    sltTA = "'" + TA.Replace(",", "','") + "'";
                }
                //符合条件的sheet1相关数据
                var list = exportManagementService.ExportUnfinishedData(startdate, enddate, sltTA, HTType);
                //符合条件的sheet2相关数据
                var DMlist = exportManagementService.ExportUnfinishedDM(startdate, enddate, sltTA, HTType);
                //符合条件的sheet3相关数据
                var UClist = exportManagementService.ExportUnfinishedUser(startdate, enddate, sltTA, HTType);
                //符合条件的sheet4相关数据
                var unlist = exportManagementService.ExportUnfinished(startdate, enddate, sltTA, HTType);
                #endregion

                #region 构建Excel
                FileStream file11 = new FileStream(Server.MapPath("~/Template/Template_HT_UnfinishedData.xls"), FileMode.Open, FileAccess.Read);
                HSSFWorkbook wk = new HSSFWorkbook(file11);

                ICellStyle cellstyle = wk.CreateCellStyle();
                cellstyle.VerticalAlignment = VerticalAlignment.Center;
                cellstyle.Alignment = HorizontalAlignment.Center;
                IDataFormat dataformat1 = wk.CreateDataFormat();
                cellstyle.DataFormat = dataformat1.GetFormat("#,###");

                ISheet sheet1 = wk.GetSheet("sheet1");
                ISheet sheet2 = wk.GetSheet("sheet2");
                ISheet sheet3 = wk.GetSheet("sheet3");
                ISheet sheet4 = wk.GetSheet("sheet4");
                #endregion

                #region 制作表体 sheet1

                for (var i = 1; i <= list.Count; i++)
                {
                    var item = list[i - 1];
                    IRow row = sheet1.CreateRow(i);
                    ICell cell = null;
                    #region data cell
                    var a = 0;
                    cell = row.CreateCell(a);
                    cell.SetCellValue(item.DeliverTime.ToString());
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    string neworder = item.newOrdersCount.ToString("N").Substring(0, item.newOrdersCount.ToString("N").Length - 3);
                    double ordernew;
                    double.TryParse(neworder, out ordernew);
                    cell.SetCellValue(ordernew);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    string newunfinished = item.newUnfinishedCount.ToString("N").Substring(0, item.newUnfinishedCount.ToString("N").Length - 3);
                    double unfinishednew;
                    double.TryParse(newunfinished, out unfinishednew);
                    cell.SetCellValue(unfinishednew);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    if (item.newOrdersCount > 0)
                    {
                        double rate = (item.newOrdersCount - item.newUnfinishedCount) / item.newOrdersCount;
                        cell.SetCellValue(rate); //Completed Ratio
                    }
                    else
                    {
                        cell.SetCellValue("-");
                        cell.CellStyle = cellstyle;
                    }

                    #endregion
                }
                #endregion

                #region 制作表体 sheet2

                for (var i = 1; i <= DMlist.Count; i++)
                {
                    var item = DMlist[i - 1];
                    IRow row = sheet2.CreateRow(i);
                    ICell cell = null;
                    #region data cell
                    var a = 0;
                    cell = row.CreateCell(a);
                    cell.SetCellValue(item.TA);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    string newDM = item.newTransferDM.ToString("N").Substring(0, item.newTransferDM.ToString("N").Length - 3);
                    double DMnew;
                    double.TryParse(newDM, out DMnew);
                    cell.SetCellValue(DMnew);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    string newquitorder = item.newUnfinishedCount.ToString("N").Substring(0, item.newUnfinishedCount.ToString("N").Length - 3);
                    double quitordernew;
                    double.TryParse(newquitorder, out quitordernew);
                    cell.SetCellValue(quitordernew);
                    cell.CellStyle = cellstyle;
                    #endregion
                }
                #endregion

                #region 制作表体 sheet3

                for (var i = 1; i <= UClist.Count; i++)
                {
                    var item = UClist[i - 1];
                    IRow row = sheet3.CreateRow(i);
                    ICell cell = null;
                    #region data cell
                    var a = 0;
                    cell = row.CreateCell(a);
                    cell.SetCellValue(item.TA);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    string newUser = item.UserCount.ToString("N").Substring(0, item.UserCount.ToString("N").Length - 3);
                    double Usernew;
                    double.TryParse(newUser, out Usernew);
                    cell.SetCellValue(Usernew);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    string newUNcount = item.UnfinishedCount.ToString("N").Substring(0, item.UnfinishedCount.ToString("N").Length - 3);
                    double UNcountnew;
                    double.TryParse(newUNcount, out UNcountnew);
                    cell.SetCellValue(UNcountnew);
                    cell.CellStyle = cellstyle;
                    #endregion
                }
                #endregion

                #region 制作表体 sheet4
                P_ORDER_Unfinished_VIEW_EXT disItm;
                for (var j = 1; j <= unlist.Count; j++)
                {
                    disItm = GetUnfinishedDisplayObj(unlist[j - 1]);
                    IRow row = sheet4.CreateRow(j);
                    ICell cell = null;
                    #region data cell
                    var a = 0;
                    cell = row.CreateCell(a);
                    cell.SetCellValue(disItm.ApplierName);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.ApplierMUDID);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.Position);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.HTCode);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.Market);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.TA);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.Province);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.City);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.HospitalCode);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.HospitalName);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.CostCenter);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.Channel);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.ORDDeliverDate);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.ORDDeliverTime);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.RestaurantId);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.RestaurantName);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    double ordAttendCount;
                    double.TryParse(disItm.ORDAttendCount, out ordAttendCount);
                    cell.SetCellValue(ordAttendCount);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    double totalfee;
                    double.TryParse(disItm.totalFee, out totalfee);
                    cell.SetCellValue(totalfee);
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.IsRetuen);//是否申请退单
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.cancelState);//是否退单成功
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.IsRetuenSuccess);//退单失败平台发起配送需求
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.cancelFeedback);//退单失败反馈配送需求
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.cancelFailReason);//预定/退单失败原因
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.ReceiveState);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    double realPrice;
                    double.TryParse(disItm.RealPrice, out realPrice);
                    cell.SetCellValue(realPrice);
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.MealSame);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.RealPriceChangeReason);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.RealPriceChangeRemark);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    if (disItm.RealCount.ToString() != "")
                    {
                        double realCount;
                        double.TryParse(disItm.RealCount, out realCount);
                        cell.SetCellValue(realCount);
                    }
                    else
                    {
                        cell.SetCellValue("");
                    }
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.RealCountChangeReason);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.RealCountChangeRemrak);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.IsOrderUpload);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.PUOCreateDate);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.PUOCreateTime);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.PUOBUHeadName);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.PUOBUHeadMUDID);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.ApproveDate);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.ApproveTime);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.PUOState);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.AttentSame);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.AttentSameReason);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.MeetingInfoSame);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.MeetingInfoSameReason);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.Reopen);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.ReopenOperatorName);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.ReopenOperatorMUDID);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.ReopenOperateDate);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.ReopenOperateTime);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.ReopenOriginatorName);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.ReopenOriginatorMUDID);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.ReopenReason);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.ReopenRemark);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.ReopenState);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.IsTransfer);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.TransferOperatorName);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.TransferOperatorMUDID);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.TransferUserName);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.TransferUserMUDID);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.TransferOperateDate);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.TransferOperateTime);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.ReAssign);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.ReAssignOperatorName);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.ReAssignOperatorMUDID);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.ReAssignBUHeadName);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.ReAssignBUHeadMUDID);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.ReAssignBUHeadApproveDate);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.ReAssignBUHeadApproveTime);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.SpecialOrderReason);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(disItm.IsWorkdayQuit);
                    cell.CellStyle = cellstyle;
                    #endregion
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
            catch (Exception ex)
            {
                LogHelper.Error("ExportUnfinishedData" + ex.Message);
            }
        }

        private P_ORDER_Unfinished_VIEW_EXT GetUnfinishedDisplayObj(P_ORDER_Unfinished_VIEW itm)
        {
            P_ORDER_Unfinished_VIEW_EXT rtnData = new P_ORDER_Unfinished_VIEW_EXT();
            rtnData.ApplierName = itm.ApplierName;
            rtnData.ApplierMUDID = itm.ApplierMUDID;
            rtnData.Position = itm.Position;
            rtnData.HTCode = itm.HTCode;
            rtnData.Market = itm.Market;
            rtnData.TA = itm.TA;
            rtnData.Province = itm.Province;
            rtnData.City = itm.City;
            rtnData.HospitalCode = itm.HospitalCode;
            rtnData.HospitalName = itm.HospitalName;
            rtnData.CostCenter = itm.CostCenter;
            rtnData.Channel = itm.Channel != null ? itm.Channel.ToUpper() : string.Empty;
            rtnData.ORDDeliverDate = itm.DeliverTime.ToString("yyyy/MM/dd");
            rtnData.ORDDeliverTime = itm.DeliverTime.ToString("HH:mm:ss");
            rtnData.RestaurantId = itm.RestaurantId != null ? itm.RestaurantId : string.Empty;
            rtnData.RestaurantName = itm.RestaurantName != null ? itm.RestaurantName : string.Empty;
            rtnData.ORDAttendCount = itm.ORDAttendCount.ToString();
            rtnData.totalFee = itm.totalFee.ToString();
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
            decimal realP = itm.RealPrice != null ? Decimal.Parse(itm.RealPrice) : 0;
            rtnData.RealPrice = realP.ToString();
            rtnData.RealPriceChangeReason = itm.RealPriceChangeReason != null ? itm.RealPriceChangeReason : string.Empty;
            rtnData.RealPriceChangeRemark = itm.RealPriceChangeRemark != null ? itm.RealPriceChangeRemark : string.Empty;
            rtnData.RealCount = itm.RealCount != null ? itm.RealCount : string.Empty;
            rtnData.RealCountChangeReason = itm.RealCountChangeReason != null ? itm.RealCountChangeReason : string.Empty;
            rtnData.RealCountChangeRemrak = itm.RealCountChangeRemrak != null ? itm.RealCountChangeRemrak : string.Empty;
            rtnData.IsOrderUpload = itm.IsOrderUpload == 1 ? "是" : "否";
            rtnData.PUOCreateDate = itm.PUOCreateDate != null ? itm.PUOCreateDate.Value.ToString("yyyy/MM/dd") : string.Empty;
            rtnData.PUOCreateTime = itm.PUOCreateDate != null ? itm.PUOCreateDate.Value.ToString("HH:mm:ss") : string.Empty;
            rtnData.PUOBUHeadName = itm.PUOBUHeadName != null ? itm.PUOBUHeadName : string.Empty;
            rtnData.PUOBUHeadMUDID = itm.PUOBUHeadMUDID != null ? itm.PUOBUHeadMUDID : string.Empty;
            rtnData.ApproveDate = itm.ApproveDate != null ? itm.ApproveDate.Value.ToString("yyyy/MM/dd") : string.Empty;
            rtnData.ApproveTime = itm.ApproveDate != null ? itm.ApproveDate.Value.ToString("HH:mm:ss") : string.Empty;
            if (itm.PUOState == "1")
            {
                rtnData.PUOState = "上传文件提交成功";
            }
            else if (itm.PUOState == "2" || itm.PUOState == "3")
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
            rtnData.AttentSame = itm.AttentSame != null ? itm.AttentSame : string.Empty;
            rtnData.AttentSameReason = itm.AttentSameReason != null ? itm.AttentSameReason : string.Empty;
            rtnData.Reopen = itm.Reopen != null ? itm.Reopen : string.Empty;
            rtnData.ReopenOperatorName = itm.ReopenOperatorName != null ? itm.ReopenOperatorName : string.Empty;
            rtnData.ReopenOperatorMUDID = itm.ReopenOperatorMUDID != null ? itm.ReopenOperatorMUDID : string.Empty;
            rtnData.ReopenOperateDate = itm.ReopenOperateDate != null ? itm.ReopenOperateDate.Value.ToString("yyyy/MM/dd") : string.Empty;
            rtnData.ReopenOperateTime = itm.ReopenOperateDate != null ? itm.ReopenOperateDate.Value.ToString("HH:mm:ss") : string.Empty;
            rtnData.ReopenReason = itm.ReopenReason != null ? itm.ReopenReason : string.Empty;
            rtnData.ReopenRemark = itm.ReopenRemark != null ? itm.ReopenRemark : string.Empty;
            rtnData.ReopenOriginatorName = itm.ReopenOriginatorName != null ? itm.ReopenOriginatorName : string.Empty;
            rtnData.ReopenOriginatorMUDID = itm.ReopenOriginatorMUDID != null ? itm.ReopenOriginatorMUDID : string.Empty;
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
            rtnData.ReAssign = itm.ReAssign != null ? itm.ReAssign : string.Empty;
            rtnData.ReAssignOperatorName = itm.ReAssignOperatorName != null ? itm.ReAssignOperatorName : string.Empty;
            rtnData.ReAssignOperatorMUDID = itm.ReAssignOperatorMUDID != null ? itm.ReAssignOperatorMUDID : string.Empty;
            rtnData.ReAssignBUHeadName = itm.ReAssignBUHeadName != null ? itm.ReAssignBUHeadName : string.Empty;
            rtnData.ReAssignBUHeadMUDID = itm.ReAssignBUHeadMUDID != null ? itm.ReAssignBUHeadMUDID : string.Empty;
            rtnData.ReAssignBUHeadApproveDate = itm.ReAssignBUHeadApproveDate != null ? itm.ReAssignBUHeadApproveDate.Value.ToString("yyyy/MM/dd") : string.Empty;
            rtnData.ReAssignBUHeadApproveTime = itm.ReAssignBUHeadApproveDate != null ? itm.ReAssignBUHeadApproveDate.Value.ToString("HH:mm:ss") : string.Empty;
            rtnData.SpecialOrderReason = itm.SpecialOrderReason != null ? itm.SpecialOrderReason : string.Empty;
            rtnData.MealSame = itm.MealSame != null ? itm.MealSame : string.Empty;
            rtnData.MeetingInfoSame = itm.MeetingInfoSame != null ? itm.MeetingInfoSame : string.Empty;
            rtnData.MeetingInfoSameReason = itm.MeetingInfoSameReason;
            rtnData.IsWorkdayQuit = itm.IsWorkdayQuit;

            return rtnData;
        }
        #endregion

        #region 医院覆盖率
        public ActionResult HospitalCoverage()
        {
            var res = exportManagementService.LoadTA();
            List<string> list = new List<string>();
            foreach (var item in res)
            {
                list.Add(item.Name);
            }
            String join = String.Join(",", list);
            ViewBag.TAList = join;
            return View();
        }

        public void ExportHospitalCoverage(string Year, string Month, string TA, string Channel)
        {
            try
            {
                #region 抓取数据   
                string date = Year + "-" + Month + "-01";
                string sltTA = string.Empty;
                if (TA.Contains("ALL") || TA.Contains("null"))
                {
                    sltTA = null;
                }
                else
                {
                    sltTA = "'" + TA.Replace(",", "','") + "'";
                }
                List<P_HOSPITAL_COVERAGE> xmsDataQuery = new List<P_HOSPITAL_COVERAGE>();
                List<P_HOSPITAL_COVERAGE> bdsDataQuery = new List<P_HOSPITAL_COVERAGE>();
                List<P_HOSPITAL_COVERAGE> totalDataQuery = new List<P_HOSPITAL_COVERAGE>();
                List<P_HOSPITAL_COVERAGE> xmsDataList = new List<P_HOSPITAL_COVERAGE>();
                List<P_HOSPITAL_COVERAGE> bdsDataList = new List<P_HOSPITAL_COVERAGE>();
                List<P_HOSPITAL_COVERAGE> totalDataList = new List<P_HOSPITAL_COVERAGE>();
                List<P_HOSPITAL_COVERAGE_TOTAL> xmsDataRxList = new List<P_HOSPITAL_COVERAGE_TOTAL>();
                List<P_HOSPITAL_COVERAGE_TOTAL> bdsDataRxList = new List<P_HOSPITAL_COVERAGE_TOTAL>();
                List<P_HOSPITAL_COVERAGE_TOTAL> totalDataRxList = new List<P_HOSPITAL_COVERAGE_TOTAL>();
                if (Channel == "全部")
                {
                    totalDataQuery = exportManagementService.LoadHospitalCoverageData(date, sltTA, "全部");
                    totalDataRxList = exportManagementService.LoadHospitalCoverageRxData(date, sltTA, "全部");
                }
                if (totalDataQuery != null && totalDataQuery.Count > 0)
                {
                    var RxList = totalDataQuery.Where(c => c.Market.Equals("Rx")).ToList();
                    if (RxList.Count > 0)
                    {
                        foreach (var item in RxList)
                        {
                            totalDataList.Add(item);
                        }

                        int AddressCnt = totalDataRxList.Count;
                        int BrandCnt1 = totalDataRxList.Where(c => c.TotalCount >= 1).Count();
                        int BrandCnt2 = totalDataRxList.Where(c => c.TotalCount >= 2).Count();
                        int BrandCnt3 = totalDataRxList.Where(c => c.TotalCount >= 3).Count();
                        int BrandCnt4 = totalDataRxList.Where(c => c.TotalCount >= 4).Count();
                        int BrandCnt5 = totalDataRxList.Where(c => c.TotalCount >= 5).Count();
                        int BreakfastCnt = totalDataRxList.Where(c => c.BreakfastCount >= 0).Count();
                        int TeaCnt = totalDataRxList.Where(c => c.TeaCount >= 0).Count();
                        P_HOSPITAL_COVERAGE p_hos_cov = new P_HOSPITAL_COVERAGE
                        {
                            Territory_TA = "Total",
                            AddressCnt = AddressCnt,
                            BrandCnt1 = BrandCnt1,
                            BrandCnt2 = BrandCnt2,
                            BrandCnt3 = BrandCnt3,
                            BrandCnt4 = BrandCnt4,
                            BrandCnt5 = BrandCnt5,
                            BreakfastCnt = BreakfastCnt,
                            TeaCnt = TeaCnt,
                            BrandCoverage1 = Math.Round(((decimal)BrandCnt1 / AddressCnt), 4),
                            BrandCoverage2 = Math.Round(((decimal)BrandCnt2 / AddressCnt), 4),
                            BrandCoverage3 = Math.Round(((decimal)BrandCnt3 / AddressCnt), 4),
                            BrandCoverage4 = Math.Round(((decimal)BrandCnt4 / AddressCnt), 4),
                            BrandCoverage5 = Math.Round(((decimal)BrandCnt5 / AddressCnt), 4),
                            BreakfastCoverage = Math.Round(((decimal)BreakfastCnt / AddressCnt), 4),
                            TeaCoverage = Math.Round(((decimal)TeaCnt / AddressCnt), 4)
                        };
                        totalDataList.Add(p_hos_cov);
                    }
                    var OtherList = totalDataQuery.Where(c => !c.Market.Equals("Rx")).ToList();
                    foreach (var item in OtherList)
                    {
                        totalDataList.Add(item);
                    }
                }
                if (Channel == "全部" || Channel == "XMS")
                {
                    xmsDataQuery = exportManagementService.LoadHospitalCoverageData(date, sltTA, "XMS");
                    xmsDataRxList = exportManagementService.LoadHospitalCoverageRxData(date, sltTA, "XMS");
                }
                if (xmsDataQuery != null && xmsDataQuery.Count > 0)
                {
                    var RxList = xmsDataQuery.Where(c => c.Market.Equals("Rx")).ToList();
                    if (RxList.Count > 0)
                    {
                        foreach (var item in RxList)
                        {
                            xmsDataList.Add(item);
                        }

                        int AddressCnt = xmsDataRxList.Count;
                        int BrandCnt1 = xmsDataRxList.Where(c => c.TotalCount >= 1).Count();
                        int BrandCnt2 = xmsDataRxList.Where(c => c.TotalCount >= 2).Count();
                        int BrandCnt3 = xmsDataRxList.Where(c => c.TotalCount >= 3).Count();
                        int BrandCnt4 = xmsDataRxList.Where(c => c.TotalCount >= 4).Count();
                        int BrandCnt5 = xmsDataRxList.Where(c => c.TotalCount >= 5).Count();
                        int BreakfastCnt = xmsDataRxList.Where(c => c.BreakfastCount >= 0).Count();
                        int TeaCnt = xmsDataRxList.Where(c => c.TeaCount >= 0).Count();
                        P_HOSPITAL_COVERAGE p_hos_cov = new P_HOSPITAL_COVERAGE
                        {
                            Territory_TA = "Total",
                            AddressCnt = AddressCnt,
                            BrandCnt1 = BrandCnt1,
                            BrandCnt2 = BrandCnt2,
                            BrandCnt3 = BrandCnt3,
                            BrandCnt4 = BrandCnt4,
                            BrandCnt5 = BrandCnt5,
                            BreakfastCnt = BreakfastCnt,
                            TeaCnt = TeaCnt,
                            BrandCoverage1 = Math.Round(((decimal)BrandCnt1 / AddressCnt), 4),
                            BrandCoverage2 = Math.Round(((decimal)BrandCnt2 / AddressCnt), 4),
                            BrandCoverage3 = Math.Round(((decimal)BrandCnt3 / AddressCnt), 4),
                            BrandCoverage4 = Math.Round(((decimal)BrandCnt4 / AddressCnt), 4),
                            BrandCoverage5 = Math.Round(((decimal)BrandCnt5 / AddressCnt), 4),
                            BreakfastCoverage = Math.Round(((decimal)BreakfastCnt / AddressCnt), 4),
                            TeaCoverage = Math.Round(((decimal)TeaCnt / AddressCnt), 4)
                        };
                        xmsDataList.Add(p_hos_cov);
                    }
                    var OtherList = xmsDataQuery.Where(c => !c.Market.Equals("Rx")).ToList();
                    foreach (var item in OtherList)
                    {
                        xmsDataList.Add(item);
                    }
                }
                if (Channel == "全部" || Channel == "BDS")
                {
                    bdsDataQuery = exportManagementService.LoadHospitalCoverageData(date, sltTA, "BDS");
                    bdsDataRxList = exportManagementService.LoadHospitalCoverageRxData(date, sltTA, "BDS");
                }

                if (bdsDataQuery != null && bdsDataQuery.Count > 0)
                {
                    var RxList = bdsDataQuery.Where(c => c.Market.Equals("Rx")).ToList();
                    if (RxList.Count > 0)
                    {
                        foreach (var item in RxList)
                        {
                            bdsDataList.Add(item);
                        }

                        int AddressCnt = bdsDataRxList.Count;
                        int BrandCnt1 = bdsDataRxList.Where(c => c.TotalCount >= 1).Count();
                        int BrandCnt2 = bdsDataRxList.Where(c => c.TotalCount >= 2).Count();
                        int BrandCnt3 = bdsDataRxList.Where(c => c.TotalCount >= 3).Count();
                        int BrandCnt4 = bdsDataRxList.Where(c => c.TotalCount >= 4).Count();
                        int BrandCnt5 = bdsDataRxList.Where(c => c.TotalCount >= 5).Count();
                        int BreakfastCnt = bdsDataRxList.Where(c => c.BreakfastCount >= 0).Count();
                        int TeaCnt = bdsDataRxList.Where(c => c.TeaCount >= 0).Count();
                        P_HOSPITAL_COVERAGE p_hos_cov = new P_HOSPITAL_COVERAGE
                        {
                            Territory_TA = "Total",
                            AddressCnt = AddressCnt,
                            BrandCnt1 = BrandCnt1,
                            BrandCnt2 = BrandCnt2,
                            BrandCnt3 = BrandCnt3,
                            BrandCnt4 = BrandCnt4,
                            BrandCnt5 = BrandCnt5,
                            BreakfastCnt = BreakfastCnt,
                            TeaCnt = TeaCnt,
                            BrandCoverage1 = Math.Round(((decimal)BrandCnt1 / AddressCnt), 4),
                            BrandCoverage2 = Math.Round(((decimal)BrandCnt2 / AddressCnt), 4),
                            BrandCoverage3 = Math.Round(((decimal)BrandCnt3 / AddressCnt), 4),
                            BrandCoverage4 = Math.Round(((decimal)BrandCnt4 / AddressCnt), 4),
                            BrandCoverage5 = Math.Round(((decimal)BrandCnt5 / AddressCnt), 4),
                            BreakfastCoverage = Math.Round(((decimal)BreakfastCnt / AddressCnt), 4),
                            TeaCoverage = Math.Round(((decimal)TeaCnt / AddressCnt), 4)
                        };
                        bdsDataList.Add(p_hos_cov);
                    }
                    var OtherList = bdsDataQuery.Where(c => !c.Market.Equals("Rx")).ToList();
                    foreach (var item in OtherList)
                    {
                        bdsDataList.Add(item);
                    }
                }
                #endregion

                #region 构建Excel
                FileStream file11 = new FileStream(Server.MapPath("~/Template/Template_HT_Hospital_Coverage.xls"), FileMode.Open, FileAccess.Read);
                HSSFWorkbook wk = new HSSFWorkbook(file11);

                ISheet sheetXMS = wk.GetSheet("XMS");
                ISheet sheetBDS = wk.GetSheet("BDS");
                ISheet sheetTotal = wk.GetSheet("TOTAL");

                ICellStyle headerStyle = wk.CreateCellStyle();
                headerStyle.Alignment = HorizontalAlignment.Center; //居中  
                headerStyle.VerticalAlignment = VerticalAlignment.Center;//垂直居中
                headerStyle.FillForegroundColor = IndexedColors.Orange.Index;
                headerStyle.FillPattern = FillPattern.SolidForeground;
                ICellStyle numStyle = wk.CreateCellStyle();
                IDataFormat dataformat = wk.CreateDataFormat();
                numStyle.DataFormat = dataformat.GetFormat("0.00%");

                ICellStyle numStyle1 = wk.CreateCellStyle();
                IDataFormat dataformat1 = wk.CreateDataFormat();
                numStyle1.DataFormat = dataformat1.GetFormat("#,###");
                #endregion

                #region 制作表体 XMS
                for (int i = 0; i < 16; i++)
                {
                    IRow row = sheetXMS.GetRow(i);
                    ICell cell = null;

                    for (int j = 1; j <= xmsDataList.Count; j++)
                    {
                        var item = xmsDataList[j - 1];
                        cell = row.CreateCell(j);
                        switch (i)
                        {
                            case 0:
                                cell.CellStyle = headerStyle;
                                cell.SetCellValue(item.Territory_TA);
                                break;
                            case 1:
                                cell.CellStyle = numStyle1;
                                cell.SetCellValue(item.AddressCnt);
                                break;
                            case 2:
                                cell.CellStyle = numStyle1;
                                cell.SetCellValue(item.BrandCnt1);
                                break;
                            case 3:
                                cell.CellStyle = numStyle1;
                                cell.SetCellValue(item.BrandCnt2);
                                break;
                            case 4:
                                cell.CellStyle = numStyle1;
                                cell.SetCellValue(item.BrandCnt3);
                                break;
                            case 5:
                                cell.CellStyle = numStyle1;
                                cell.SetCellValue(item.BrandCnt4);
                                break;
                            case 6:
                                cell.CellStyle = numStyle1;
                                cell.SetCellValue(item.BrandCnt5);
                                break;
                            case 7:
                                cell.CellStyle = numStyle1;
                                cell.SetCellValue(item.BreakfastCnt);
                                break;
                            case 8:
                                cell.CellStyle = numStyle1;
                                cell.SetCellValue(item.TeaCnt);
                                break;
                            case 9:
                                cell.CellStyle = numStyle;
                                if (item.BrandCoverage1 == 0)
                                    cell.SetCellValue("");
                                else
                                    cell.SetCellValue((double)item.BrandCoverage1);
                                break;
                            case 10:
                                cell.CellStyle = numStyle;
                                if (item.BrandCoverage2 == 0)
                                    cell.SetCellValue("");
                                else
                                    cell.SetCellValue((double)item.BrandCoverage2);
                                break;
                            case 11:
                                cell.CellStyle = numStyle;
                                if (item.BrandCoverage3 == 0)
                                    cell.SetCellValue("");
                                else
                                    cell.SetCellValue((double)item.BrandCoverage3);
                                break;
                            case 12:
                                cell.CellStyle = numStyle;
                                if (item.BrandCoverage4 == 0)
                                    cell.SetCellValue("");
                                else
                                    cell.SetCellValue((double)item.BrandCoverage4);
                                break;
                            case 13:
                                cell.CellStyle = numStyle;
                                if (item.BrandCoverage5 == 0)
                                    cell.SetCellValue("");
                                else
                                    cell.SetCellValue((double)item.BrandCoverage5);
                                break;
                            case 14:
                                cell.CellStyle = numStyle;
                                if (item.BreakfastCoverage == 0)
                                    cell.SetCellValue("");
                                else
                                    cell.SetCellValue((double)item.BreakfastCoverage);
                                break;
                            case 15:
                                cell.CellStyle = numStyle;
                                if (item.TeaCoverage == 0)
                                    cell.SetCellValue("");
                                else
                                    cell.SetCellValue((double)item.TeaCoverage);
                                break;
                        }
                    }
                }
                #endregion

                #region 制作表体 BDS
                for (int m = 0; m < 16; m++)
                {
                    IRow row = sheetBDS.GetRow(m);
                    ICell cell = null;
                    for (int n = 1; n <= bdsDataList.Count; n++)
                    {
                        var item = bdsDataList[n - 1];
                        cell = row.CreateCell(n);
                        switch (m)
                        {
                            case 0:
                                cell.CellStyle = headerStyle;
                                cell.SetCellValue(item.Territory_TA);
                                break;
                            case 1:
                                cell.CellStyle = numStyle1;
                                cell.SetCellValue(item.AddressCnt);
                                break;
                            case 2:
                                cell.CellStyle = numStyle1;
                                cell.SetCellValue(item.BrandCnt1);
                                break;
                            case 3:
                                cell.CellStyle = numStyle1;
                                cell.SetCellValue(item.BrandCnt2);
                                break;
                            case 4:
                                cell.CellStyle = numStyle1;
                                cell.SetCellValue(item.BrandCnt3);
                                break;
                            case 5:
                                cell.CellStyle = numStyle1;
                                cell.SetCellValue(item.BrandCnt4);
                                break;
                            case 6:
                                cell.CellStyle = numStyle1;
                                cell.SetCellValue(item.BrandCnt5);
                                break;
                            case 7:
                                cell.CellStyle = numStyle1;
                                cell.SetCellValue(item.BreakfastCnt);
                                break;
                            case 8:
                                cell.CellStyle = numStyle1;
                                cell.SetCellValue(item.TeaCnt);
                                break;
                            case 9:
                                cell.CellStyle = numStyle;
                                cell.SetCellValue((double)item.BrandCoverage1);
                                break;
                            case 10:
                                cell.CellStyle = numStyle;
                                cell.SetCellValue((double)item.BrandCoverage2);
                                break;
                            case 11:
                                cell.CellStyle = numStyle;
                                cell.SetCellValue((double)item.BrandCoverage3);
                                break;
                            case 12:
                                cell.CellStyle = numStyle;
                                cell.SetCellValue((double)item.BrandCoverage4);
                                break;
                            case 13:
                                cell.CellStyle = numStyle;
                                cell.SetCellValue((double)item.BrandCoverage5);
                                break;
                            case 14:
                                cell.CellStyle = numStyle;
                                cell.SetCellValue((double)item.BreakfastCoverage);
                                break;
                            case 15:
                                cell.CellStyle = numStyle;
                                cell.SetCellValue((double)item.TeaCoverage);
                                break;
                        }
                    }
                }
                #endregion

                #region 制作表体 Total
                for (int m = 0; m < 16; m++)
                {
                    IRow row = sheetTotal.GetRow(m);
                    ICell cell = null;
                    for (int n = 1; n <= totalDataList.Count; n++)
                    {
                        var item = totalDataList[n - 1];
                        cell = row.CreateCell(n);
                        switch (m)
                        {
                            case 0:
                                cell.CellStyle = headerStyle;
                                cell.SetCellValue(item.Territory_TA);
                                break;
                            case 1:
                                cell.CellStyle = numStyle1;
                                cell.SetCellValue(item.AddressCnt);
                                break;
                            case 2:
                                cell.CellStyle = numStyle1;
                                cell.SetCellValue(item.BrandCnt1);
                                break;
                            case 3:
                                cell.CellStyle = numStyle1;
                                cell.SetCellValue(item.BrandCnt2);
                                break;
                            case 4:
                                cell.CellStyle = numStyle1;
                                cell.SetCellValue(item.BrandCnt3);
                                break;
                            case 5:
                                cell.CellStyle = numStyle1;
                                cell.SetCellValue(item.BrandCnt4);
                                break;
                            case 6:
                                cell.CellStyle = numStyle1;
                                cell.SetCellValue(item.BrandCnt5);
                                break;
                            case 7:
                                cell.CellStyle = numStyle1;
                                cell.SetCellValue(item.BreakfastCnt);
                                break;
                            case 8:
                                cell.CellStyle = numStyle1;
                                cell.SetCellValue(item.TeaCnt);
                                break;
                            case 9:
                                cell.CellStyle = numStyle;
                                cell.SetCellValue((double)item.BrandCoverage1);
                                break;
                            case 10:
                                cell.CellStyle = numStyle;
                                cell.SetCellValue((double)item.BrandCoverage2);
                                break;
                            case 11:
                                cell.CellStyle = numStyle;
                                cell.SetCellValue((double)item.BrandCoverage3);
                                break;
                            case 12:
                                cell.CellStyle = numStyle;
                                cell.SetCellValue((double)item.BrandCoverage4);
                                break;
                            case 13:
                                cell.CellStyle = numStyle;
                                cell.SetCellValue((double)item.BrandCoverage5);
                                break;
                            case 14:
                                cell.CellStyle = numStyle;
                                cell.SetCellValue((double)item.BreakfastCoverage);
                                break;
                            case 15:
                                cell.CellStyle = numStyle;
                                cell.SetCellValue((double)item.TeaCoverage);
                                break;
                        }
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
            catch (Exception ex)
            {
                LogHelper.Error("ExportHospitalCoverage" + ex.Message);
            }
        }
        #endregion


    }
}