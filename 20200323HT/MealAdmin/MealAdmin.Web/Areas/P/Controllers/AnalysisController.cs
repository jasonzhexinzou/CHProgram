using ICSharpCode.SharpZipLib.Zip;
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
    public class AnalysisController : AdminBaseController
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

        [Bean("analysisService")]
        public IAnalysisService analysisService { get; set; }


        #region 预申请分析
        public ActionResult PreAnalysis()
        {
            //加载TA
            var res = analysisService.LoadTA();
            List<string> list = new List<string>();
            foreach (var item in res)
            {
                list.Add(item.Name);
            }
            String join = String.Join(",", list);
            ViewBag.TAlist = join;
            return View();
        }

        //加载RD
        public JsonResult LoadRD(string TAOption)
        {
            string sltTA = "'" + TAOption.Replace(",", "','") + "'";
            var res = analysisService.LoadRD(sltTA);
            return Json(res);
        }
        //加载RM
        public JsonResult LoadRM(string RDOption)
        {
            string sltRD = "'" + RDOption + "'";
            var res = analysisService.LoadRM(sltRD);
            return Json(res);
        }
        //加载DM
        public JsonResult LoadDM(string RMOption)
        {
            string sltRM = "'" + RMOption + "'";
            var res = analysisService.LoadDM(sltRM);
            return Json(res);
        }
        //预申请分析1：数量金额分析
        public JsonResult LoadCountChart(string MeetingTimeBegin, string MeetingTimeEnd, string PreAmount, string PreState, string TA, string htType, string RD, string RM, string DM)
        {
            try
            {
                DateTime? _MTBegin, _MTEnd;
                DateTime _tmpTime;
                string sltTA = "";
                string sltRD = "";
                string sltRM = "";
                string sltDM = "";
                if (DateTime.TryParse(MeetingTimeBegin, out _tmpTime) == true)
                {
                    _MTBegin = _tmpTime;
                }
                else
                {
                    _MTBegin = null;
                }
                if (DateTime.TryParse(MeetingTimeEnd, out _tmpTime) == true)
                {
                    _MTEnd = _tmpTime.AddDays(1d);
                }
                else
                {
                    _MTEnd = null;
                }
                if (TA.Contains("ALL"))
                {
                    sltTA = null;
                }
                else
                {
                    sltTA = "'" + TA.Replace(",", "','") + "'";
                }
                if (RD == "ALL" || RD == "")
                {
                    sltRD = null;
                }
                else
                {
                    sltRD = "'" + RD + "'";
                }
                if (RM == "ALL" || RM == "")
                {
                    sltRM = null;
                }
                else
                {
                    sltRM = "'" + RM + "'";
                }
                if (DM == "ALL" || DM == "")
                {
                    sltDM = null;
                }
                else
                {
                    sltDM = "'" + DM + "'";
                }
                //抓取数据
                var res = analysisService.LoadCountChart(_MTBegin, _MTEnd, PreAmount, PreState, sltTA, htType, sltRD, sltRM, sltDM);
                var ChartRows = new List<P_PreApproval_Count_View>();
                var ChartAmountRows = new List<P_PreApproval_Count_View>();
                var resCount = from q in res
                               orderby q.TA, q.PreCount descending
                               select q;
                foreach (var item in resCount)
                {
                    if (TA.Contains("ALL"))
                    {
                        ChartRows.Add(new P_PreApproval_Count_View()
                        {
                            CodeandNAME = item.ConCode,
                            PreCount = item.PreCount,
                            PrePrice = item.PrePrice.ToString(),
                            PreAmount = item.PrePrice
                        });
                    }
                    else
                    {
                        ChartRows.Add(new P_PreApproval_Count_View()
                        {
                            CodeandNAME = item.ConCode + "-" + item.NAME,
                            PreCount = item.PreCount,
                            PrePrice = item.PrePrice.ToString(),
                            PreAmount = item.PrePrice
                        });
                    }
                }
                var resAmount = from q in res
                                orderby q.TA, q.PrePrice descending
                                select q;
                foreach (var item in resAmount)
                {
                    if (TA.Contains("ALL"))
                    {
                        ChartAmountRows.Add(new P_PreApproval_Count_View()
                        {
                            CodeandNAME = item.ConCode,
                            PreCount = item.PreCount,
                            PrePrice = item.PrePrice.ToString(),
                            PreAmount = item.PrePrice
                        });
                    }
                    else
                    {
                        ChartAmountRows.Add(new P_PreApproval_Count_View()
                        {
                            CodeandNAME = item.ConCode + "-" + item.NAME,
                            PreCount = item.PreCount,
                            PrePrice = item.PrePrice.ToString(),
                            PreAmount = item.PrePrice
                        });
                    }
                }

                return Json(new { state = 1, ChartCount = ChartRows, ChartAmount = ChartAmountRows });
            }
            catch (Exception ex)
            {
                LogHelper.Error("LoadCountChart" + ex.Message);
                return Json(new { state = 0 });
            }
        }
        //导出预申请分析1:数量金额及医院排行
        public void ExportPreApprovalAnalysis(string MeetingTimeBegin, string MeetingTimeEnd, string PreAmount, string PreState, string TA, string htType, string RD, string RM, string DM)
        {
            try
            {
                #region 抓取数据   
                DateTime? _MTBegin, _MTEnd;
                DateTime _tmpTime;
                string sltTA = "";
                string sltRD = "";
                string sltRM = "";
                string sltDM = "";
                if (DateTime.TryParse(MeetingTimeBegin, out _tmpTime) == true)
                {
                    _MTBegin = _tmpTime;
                }
                else
                {
                    _MTBegin = null;
                }
                if (DateTime.TryParse(MeetingTimeEnd, out _tmpTime) == true)
                {
                    _MTEnd = _tmpTime.AddDays(1d);
                }
                else
                {
                    _MTEnd = null;
                }
                if (TA.Contains("ALL"))
                {
                    sltTA = null;
                }
                else
                {
                    sltTA = "'" + TA.Replace(",", "','") + "'";
                }
                if (RD == "ALL" || RD == "" || RD == "null")
                {
                    sltRD = null;
                }
                else
                {
                    sltRD = "'" + RD + "'";
                }
                if (RM == "ALL" || RM == "" || RM == "null")
                {
                    sltRM = null;
                }
                else
                {
                    sltRM = "'" + RM + "'";
                }
                if (DM == "ALL" || DM == "" || DM == "null")
                {
                    sltDM = null;
                }
                else
                {
                    sltDM = "'" + DM + "'";
                }
                //符合条件的sheet1相关数据
                var list = analysisService.ExportCountAmount(_MTBegin, _MTEnd, PreAmount, PreState, sltTA, htType, sltRD, sltRM, sltDM);
                //符合条件的sheet2相关数据
                var hoslist = analysisService.ExportHospitalRanking(_MTBegin, _MTEnd, PreAmount, PreState, sltTA, htType, sltRD, sltRM, sltDM);

                #endregion

                #region 构建Excel
                FileStream file11 = new FileStream(Server.MapPath("~/Template/Template_HT_PreApprovalAnalysis.xls"), FileMode.Open, FileAccess.Read);
                HSSFWorkbook wk = new HSSFWorkbook(file11);

                ICellStyle cellstyle = wk.CreateCellStyle();
                cellstyle.VerticalAlignment = VerticalAlignment.Center;
                cellstyle.Alignment = HorizontalAlignment.Center;
                IDataFormat dataformat1 = wk.CreateDataFormat();
                cellstyle.DataFormat = dataformat1.GetFormat("#,###");

                ISheet sheet1 = wk.GetSheet("HT预申请数量金额");
                ISheet sheet2 = wk.GetSheet("HT医院排行");
                #endregion
                string zerocount = "";
                string nonzerocount = "";
                string totalcount = "";
                #region 制作表体 sheet1

                for (var i = 1; i <= list.Count; i++)
                {
                    var item = list[i - 1];
                    IRow row = sheet1.CreateRow(i);
                    ICell cell = null;
                    #region data cell
                    var a = 0;
                    cell = row.CreateCell(a);
                    cell.SetCellValue(item.ConCode);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(item.NAME);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(item.MUDID);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    if (PreAmount == "all")
                    {
                        zerocount = item.newZeroCount.ToString("N").Substring(0, item.newZeroCount.ToString("N").Length - 3);
                    }
                    //非0元
                    if (PreAmount == "1")
                    {
                        zerocount = "";
                    }
                    //0元
                    if (PreAmount == "0")
                    {
                        zerocount = item.newZeroCount.ToString("N").Substring(0, item.newZeroCount.ToString("N").Length - 3);
                    }
                    double zeroCount;
                    double.TryParse(zerocount, out zeroCount);
                    cell.SetCellValue(zeroCount);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    if (PreAmount == "all")
                    {
                        nonzerocount = item.newNonZeroCount.ToString("N").Substring(0, item.newNonZeroCount.ToString("N").Length - 3);
                    }
                    //非0元
                    if (PreAmount == "1")
                    {
                        nonzerocount = item.newNonZeroCount.ToString("N").Substring(0, item.newNonZeroCount.ToString("N").Length - 3);
                    }
                    //0元
                    if (PreAmount == "0")
                    {
                        nonzerocount = "";
                    }
                  
                    double nonzeroCount;
                    double.TryParse(nonzerocount, out nonzeroCount);
                    cell.SetCellValue(nonzeroCount);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    totalcount = item.TotalCount.ToString("N").Substring(0, item.TotalCount.ToString("N").Length - 3);
                    double totalCount;
                    double.TryParse(totalcount, out totalCount);
                    cell.SetCellValue(totalCount);

                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);

                    double totalprice;
                    double.TryParse(item.TotalPrice.ToString(), out totalprice);
                    cell.SetCellValue(totalprice);
                    //cell.CellStyle = cellstyle;
                    #endregion
                }
                #endregion

                string hoszerocount = "";
                string hosnonzerocount = "";
                string hostotalcount = "";
                #region 制作表体 sheet2

                for (var i = 1; i <= hoslist.Count; i++)
                {
                    var item = hoslist[i - 1];
                    IRow row = sheet2.CreateRow(i);
                    ICell cell = null;
                    #region data cell
                    var a = 0;
                    cell = row.CreateCell(a);
                    cell.SetCellValue(item.HospitalCode);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    cell.SetCellValue(item.HospitalName);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    if (PreAmount == "all")
                    {
                        hoszerocount = item.ZeroCount.ToString("N").Substring(0, item.ZeroCount.ToString("N").Length - 3);
                    }
                    //非0元
                    if (PreAmount == "1")
                    {
                        hoszerocount = "";
                    }
                    //0元
                    if (PreAmount == "0")
                    {
                        hoszerocount = item.ZeroCount.ToString("N").Substring(0, item.ZeroCount.ToString("N").Length - 3);
                    }
                   
                    double hoszeroCount;
                    double.TryParse(hoszerocount, out hoszeroCount);
                    cell.SetCellValue(hoszeroCount);

                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    if (PreAmount == "all")
                    {
                        hosnonzerocount = item.NonZeroCount.ToString("N").Substring(0, item.NonZeroCount.ToString("N").Length - 3);
                    }
                    //非0元
                    if (PreAmount == "1")
                    {
                        hosnonzerocount = item.NonZeroCount.ToString("N").Substring(0, item.NonZeroCount.ToString("N").Length - 3);
                    }
                    //0元
                    if (PreAmount == "0")
                    {
                        hosnonzerocount = "";
                    }
                    double hosnonzeroCount;
                    double.TryParse(hosnonzerocount, out hosnonzeroCount);
                    cell.SetCellValue(hosnonzeroCount);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    hostotalcount = item.newTotalCount.ToString("N").Substring(0, item.newTotalCount.ToString("N").Length - 3);
                    double hostotalCount;
                    double.TryParse(hostotalcount, out hostotalCount);
                    cell.SetCellValue(hostotalCount);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    double hostotalprice;
                    double.TryParse(item.newTotalPrice.ToString(), out hostotalprice);
                    cell.SetCellValue(hostotalprice);
                    //cell.CellStyle = cellstyle;
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
                LogHelper.Error("ExportPreApprovalAnalysis" + ex.Message);
            }
        }
        //导出预申请分析2
        public void ExportPreAnalysisReport(string Year, string Month, string HTType, string PreAmount, string PreState, string TA)
        {
            try
            {
                #region 抓取数据                   
                string sltTA = "";
                string meetingdate = "";
                if (Year != "" && Month != "")
                {
                    meetingdate = Year + "-" + Month + "-01";
                }
                else
                {
                    meetingdate = "";
                }
                if (TA.Contains("ALL"))
                {
                    sltTA = null;
                }
                else
                {
                    sltTA = "'" + TA.Replace(",", "','") + "'";
                }

                //预申请数据
                var preList = analysisService.LoadCountList(meetingdate, HTType, PreAmount, PreState, sltTA);
                //符合筛选条件的预申请TA
                var taViewList = preList.Select(i => new { i.TA }).Distinct().ToList();
                //全部预申请医院
                var Hospitallist = (from a in preList
                                    group a by new { a.TA, a.HospitalCode } into b
                                    select new
                                    {
                                        b.Key.TA,
                                        b.Key.HospitalCode
                                    }).ToList();
                //院内预申请医院
                var HospitalHTlist = (from a in preList
                                      where a.HTType == 1
                                      group a by new { a.TA, a.HospitalCode } into b
                                      select new
                                      {
                                          b.Key.TA,
                                          b.Key.HospitalCode
                                      }).ToList();
                //院外预申请医院
                var HospitalOHlist = (from a in preList
                                      where a.HTType == 2
                                      group a by new { a.TA, a.HospitalCode } into b
                                      select new
                                      {
                                          b.Key.TA,
                                          b.Key.HospitalCode
                                      }).ToList();
                //全部预申请代表
                var Applierlist = (from a in preList
                                   group a by new { a.TA, a.ApplierMUDID } into b
                                   select new
                                   {
                                       b.Key.TA,
                                       b.Key.ApplierMUDID
                                   }).ToList();
                //院内预申请代表
                var ApplierHTlist = (from a in preList
                                     where a.HTType == 1
                                     group a by new { a.TA, a.ApplierMUDID } into b
                                     select new
                                     {
                                         b.Key.TA,
                                         b.Key.ApplierMUDID
                                     }).ToList();
                //院外预申请代表
                var ApplierOHlist = (from a in preList
                                     where a.HTType == 2
                                     group a by new { a.TA, a.ApplierMUDID } into b
                                     select new
                                     {
                                         b.Key.TA,
                                         b.Key.ApplierMUDID
                                     }).ToList();

                //TA-医院 包含MR数据
                var taHospitalList = analysisService.LoadTAHospitalList(meetingdate);

                //TA-医院 按TA去重后的数据
                var taHospitalFilter = (from a in taHospitalList
                                        group a by new { a.TERRITORY_TA, a.HospitalCode } into b
                                        select new
                                        {
                                            b.Key.TERRITORY_TA,
                                            b.Key.HospitalCode
                                        }).ToList();
                //TA-院外医院 包含MR数据
                var taHospitalOHList = analysisService.LoadTAHospitalOHList(meetingdate);

                //目标医院数据
                var resHospital = analysisService.LoadHospital(meetingdate);
                //目标医院院外数据
                var resHospitalOH = (from a in resHospital
                                     where a.HospitalCode.Contains("-OH")
                                     group a by new { a.HospitalCode } into b
                                     select new
                                     {
                                         HospitalCode = b.Key.HospitalCode
                                     }).ToList();

                List<P_PreApproval_ANALYSIS_LIST> viewList = new List<P_PreApproval_ANALYSIS_LIST>();
                List<string> totalHosList = new List<string>(); //目标医院
                List<string> totalPreList = new List<string>(); //预申请医院
                List<string> totalMUDIDList = new List<string>(); //可订餐代表
                List<string> totalPreApplierList = new List<string>(); //预申请代表
                foreach (var ta in taViewList)
                {
                    string v1 = ta.TA;         //TA
                    int v2 = 0;                  //0元预申请
                    int v3 = 0;                  //非0元预申请
                    int v4 = 0;                  //预申请总数
                    int v5 = 0;                  //目标医院 
                    int v6 = 0;                  //预申请医院数 
                    int v7 = 0;                  //可订餐代表数
                    int v8 = 0;                  //预申请代表数
                    if (HTType == "all" || HTType == "0")
                    {
                        //可订餐MR
                        var MRList = taHospitalList.Where(c => c.TERRITORY_TA.Equals(ta.TA) && c.MUD_ID_MR != null && c.MUD_ID_MR != "").Select(i => new { i.MUD_ID_MR }).Distinct().ToList();
                        foreach (var item in MRList)
                        {
                            totalMUDIDList.Add(item.MUD_ID_MR);
                        }
                        //可订餐DM
                        var DMList = taHospitalList.Where(c => c.TERRITORY_TA.Equals(ta.TA) && c.MUD_ID_DM != null && c.MUD_ID_DM != "").Select(i => new { i.MUD_ID_DM }).Distinct().ToList();
                        foreach (var item in DMList)
                        {
                            totalMUDIDList.Add(item.MUD_ID_DM);
                        }
                        v7 = MRList.Count + DMList.Count;
                    }
                    else
                    {
                        //可订餐MR
                        var MRList = taHospitalOHList.Where(c => c.TERRITORY_TA.Equals(ta.TA) && c.MUD_ID_MR != null && c.MUD_ID_MR != "").Select(i => new { i.MUD_ID_MR }).Distinct().ToList();
                        foreach (var item in MRList)
                        {
                            totalMUDIDList.Add(item.MUD_ID_MR);
                        }
                        //可订餐DM
                        var DMList = taHospitalOHList.Where(c => c.TERRITORY_TA.Equals(ta.TA) && c.MUD_ID_DM != null && c.MUD_ID_DM != "").Select(i => new { i.MUD_ID_DM }).Distinct().ToList();
                        foreach (var item in DMList)
                        {
                            totalMUDIDList.Add(item.MUD_ID_DM);
                        }
                        v7 = MRList.Count + DMList.Count;
                    }
                    List<P_PreApproval_LIST_VIEW> oList = new List<P_PreApproval_LIST_VIEW>();
                    if (HTType == "all")
                    {
                        //0元预申请
                        var zeroList = preList.Where(c => c.TA.Equals(ta.TA) && c.BudgetTotal == 0).ToList();
                        //非0元预申请
                        var nonzeroList = preList.Where(c => c.TA.Equals(ta.TA) && c.BudgetTotal > 0).ToList();
                        //全部预申请
                        var countList = preList.Where(c => c.TA.Equals(ta.TA)).ToList();
                        //[Territory_Hospital_COST]中目标医院
                        var hosList = (from a in taHospitalFilter
                                       where a.TERRITORY_TA.Equals(ta.TA)
                                       select new
                                       {
                                           HospitalCode = a.HospitalCode
                                       }).ToList();
                        foreach (var item in hosList)
                        {
                            totalHosList.Add(item.HospitalCode);
                        }

                        //预申请医院数
                        var PrehosList = (from a in Hospitallist
                                          where a.TA.Equals(ta.TA)
                                          select new
                                          {
                                              HospitalCode = a.HospitalCode.Replace("-OH", "")
                                          }).Distinct().ToList();
                        foreach (var item in PrehosList)
                        {
                            totalPreList.Add(item.HospitalCode);
                        }
                        //预申请代表数
                        var PreApplierList = (from a in Applierlist
                                              where a.TA.Equals(ta.TA)
                                              select new
                                              {
                                                  ApplierMUDID = a.ApplierMUDID
                                              }).ToList();
                        foreach (var item in PreApplierList)
                        {
                            totalPreApplierList.Add(item.ApplierMUDID);
                        }
                        v2 = zeroList.Count;
                        v3 = nonzeroList.Count;
                        v4 = countList.Count;
                        v5 = hosList.Count;
                        v6 = PrehosList.Count;
                        v8 = PreApplierList.Count;

                    }
                    //院内
                    else if (HTType == "0")
                    {
                        //0元预申请
                        var htzeroList = preList.Where(c => c.TA.Equals(ta.TA) && c.BudgetTotal == 0).ToList();
                        //非0元预申请
                        var htnonzeroList = preList.Where(c => c.TA.Equals(ta.TA) && c.BudgetTotal > 0).ToList();
                        //全部预申请
                        var htcountList = preList.Where(c => c.TA.Equals(ta.TA)).ToList();
                        //[Territory_Hospital_COST]中目标医院
                        var hthosList = (from a in taHospitalFilter
                                         where a.TERRITORY_TA.Equals(ta.TA)
                                         select new
                                         {
                                             HospitalCode = a.HospitalCode
                                         }).ToList();
                        foreach (var item in hthosList)
                        {
                            totalHosList.Add(item.HospitalCode);
                        }

                        //预申请医院数
                        var htPrehosList = (from a in HospitalHTlist
                                            where a.TA.Equals(ta.TA)
                                            select new
                                            {
                                                HospitalCode = a.HospitalCode
                                            }).ToList();
                        foreach (var item in htPrehosList)
                        {
                            totalPreList.Add(item.HospitalCode);
                        }
                        //预申请代表数
                        var htPreApplierList = (from a in ApplierHTlist
                                                where a.TA.Equals(ta.TA)
                                                select new
                                                {
                                                    ApplierMUDID = a.ApplierMUDID
                                                }).ToList();
                        foreach (var item in htPreApplierList)
                        {
                            totalPreApplierList.Add(item.ApplierMUDID);
                        }
                        v2 = htzeroList.Count;
                        v3 = htnonzeroList.Count;
                        v4 = htcountList.Count;
                        v5 = hthosList.Count;
                        v6 = htPrehosList.Count;
                        v8 = htPreApplierList.Count;

                    }
                    //院外
                    else if (HTType == "1")
                    {
                        //0元预申请
                        var ohzeroList = preList.Where(c => c.TA.Equals(ta.TA) && c.BudgetTotal == 0).ToList();
                        //非0元预申请
                        var ohnonzeroList = preList.Where(c => c.TA.Equals(ta.TA) && c.BudgetTotal > 0).ToList();
                        //全部预申请
                        var ohcountList = preList.Where(c => c.TA.Equals(ta.TA)).ToList();
                        //[P_Hospital_COST]中目标医院
                        var ohhosList = (from a in taHospitalFilter
                                         where a.TERRITORY_TA.Equals(ta.TA)
                                         select new
                                         {
                                             HospitalCode = a.HospitalCode
                                         }).ToList();
                        var ohList = (from a in resHospitalOH
                                      join b in ohhosList on a.HospitalCode.Replace("-OH", "") equals b.HospitalCode
                                      select new
                                      {
                                          HospitalCode = a.HospitalCode
                                      }).ToList();
                        foreach (var item in ohList)
                        {
                            totalHosList.Add(item.HospitalCode);
                        }

                        //预申请医院数
                        var ohPrehosList = (from a in HospitalOHlist
                                            where a.TA.Equals(ta.TA)
                                            select new
                                            {
                                                HospitalCode = a.HospitalCode
                                            }).ToList();
                        foreach (var item in ohPrehosList)
                        {
                            totalPreList.Add(item.HospitalCode);
                        }
                        //预申请代表数
                        var ohPreApplierList = (from a in ApplierOHlist
                                                where a.TA.Equals(ta.TA)
                                                select new
                                                {
                                                    ApplierMUDID = a.ApplierMUDID
                                                }).ToList();
                        foreach (var item in ohPreApplierList)
                        {
                            totalPreApplierList.Add(item.ApplierMUDID);
                        }
                        v2 = ohzeroList.Count;
                        v3 = ohnonzeroList.Count;
                        v4 = ohcountList.Count;
                        v5 = ohList.Count;
                        v6 = ohPrehosList.Count;
                        v8 = ohPreApplierList.Count;

                    }

                    P_PreApproval_ANALYSIS_LIST list = new P_PreApproval_ANALYSIS_LIST
                    {
                        TA = v1,
                        ZeroCount = v2.ToString("N").Substring(0, v2.ToString("N").Length - 3),
                        NonZeroCount = v3.ToString("N").Substring(0, v3.ToString("N").Length - 3),
                        TotalCount = v4.ToString("N").Substring(0, v4.ToString("N").Length - 3),
                        HospitalCount = v5 == 0 ? "" : v5.ToString("N").Substring(0, v5.ToString("N").Length - 3),
                        PreHospitalCount = v6.ToString("N").Substring(0, v6.ToString("N").Length - 3),
                        MUDIDCount = v7 == 0 ? "" : v7.ToString("N").Substring(0, v7.ToString("N").Length - 3),
                        ApplierCount = v8.ToString("N").Substring(0, v8.ToString("N").Length - 3),
                    };
                    viewList.Add(list);
                }
                var totalHosCnt = totalHosList.Distinct().ToList().Count;
                var totalPreCnt = totalPreList.Distinct().ToList().Count;
                var totalMUDIDCnt = totalMUDIDList.Distinct().ToList().Count;
                var totalPreApplierCnt = totalPreApplierList.Distinct().ToList().Count;
                P_PreApproval_ANALYSIS_LIST exportlist = new P_PreApproval_ANALYSIS_LIST
                {
                    TA = "",
                    ZeroCount = "",
                    NonZeroCount = "",
                    TotalCount = "",
                    HospitalCount = totalHosCnt == 0 ? "" : "合计：" + totalHosCnt.ToString("N").Substring(0, totalHosCnt.ToString("N").Length - 3),
                    PreHospitalCount = totalPreCnt == 0 ? "" : "合计：" + totalPreCnt.ToString("N").Substring(0, totalPreCnt.ToString("N").Length - 3),
                    MUDIDCount = totalMUDIDCnt == 0 ? "" : "合计：" + totalMUDIDCnt.ToString("N").Substring(0, totalMUDIDCnt.ToString("N").Length - 3),
                    ApplierCount = totalPreApplierCnt == 0 ? "" : "合计：" + totalPreApplierCnt.ToString("N").Substring(0, totalPreApplierCnt.ToString("N").Length - 3),
                };
                viewList.Add(exportlist);
                #endregion

                #region 构建Excel
                FileStream file11 = new FileStream(Server.MapPath("~/Template/Template_HT_PreApprovalAnalysisReport.xls"), FileMode.Open, FileAccess.Read);
                HSSFWorkbook wk = new HSSFWorkbook(file11);

                ICellStyle cellstyle = wk.CreateCellStyle();
                cellstyle.VerticalAlignment = VerticalAlignment.Center;
                cellstyle.Alignment = HorizontalAlignment.Center;
                IDataFormat dataformat1 = wk.CreateDataFormat();
                cellstyle.DataFormat = dataformat1.GetFormat("#,###");


                ISheet sheet = wk.GetSheet("report");
                #endregion

                #region 制作表体

                for (var i = 1; i <= viewList.Count; i++)
                {
                    var item = viewList[i - 1];
                    IRow row = sheet.CreateRow(i);
                    ICell cell = null;
                    #region data cell
                    var a = 0;
                    cell = row.CreateCell(a);
                    cell.SetCellValue(item.TA);
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    if (i == viewList.Count)
                    {
                        cell.SetCellValue(item.ZeroCount);
                    }
                    else
                    {
                        if (item.ZeroCount.ToString() != "")
                        {
                            double zeroCount;
                            double.TryParse(item.ZeroCount.ToString(), out zeroCount);
                            cell.SetCellValue(zeroCount);
                        }
                        else
                        {
                            cell.SetCellValue("");
                        }

                    }
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    if (i == viewList.Count)
                    {
                        cell.SetCellValue(item.NonZeroCount);
                    }
                    else
                    {
                        if (item.NonZeroCount.ToString() != "")
                        {
                            double nonZeroCount;
                            double.TryParse(item.NonZeroCount.ToString(), out nonZeroCount);
                            cell.SetCellValue(nonZeroCount);
                        }
                        else
                        {
                            cell.SetCellValue("");
                        }
                    }
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    if (i == viewList.Count)
                    {
                        cell.SetCellValue(item.TotalCount);
                    }
                    else
                    {
                        if (item.TotalCount.ToString() != "")
                        {
                            double totalCount;
                            double.TryParse(item.TotalCount.ToString(), out totalCount);
                            cell.SetCellValue(totalCount);
                        }
                        else
                        {
                            cell.SetCellValue("");
                        }
                    }
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    if (i == viewList.Count)
                    {
                        cell.SetCellValue(item.HospitalCount);
                    }
                    else
                    {
                        if (item.HospitalCount.ToString() != "")
                        {
                            double hospitalCount;
                            double.TryParse(item.HospitalCount.ToString(), out hospitalCount);
                            cell.SetCellValue(hospitalCount);
                        }
                        else
                        {
                            cell.SetCellValue("");
                        }
                    }
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    if (i == viewList.Count)
                    {
                        cell.SetCellValue(item.PreHospitalCount);
                    }
                    else
                    {
                        if (item.PreHospitalCount.ToString() != "")
                        {
                            double preHospitalCount;
                            double.TryParse(item.PreHospitalCount.ToString(), out preHospitalCount);
                            cell.SetCellValue(preHospitalCount);
                        }
                        else
                        {
                            cell.SetCellValue("");
                        }
                    }
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    if (i == viewList.Count)
                    {
                        cell.SetCellValue(item.MUDIDCount);
                    }
                    else
                    {
                        if (item.MUDIDCount.ToString() != "")
                        {
                            double mUDIDCount;
                            double.TryParse(item.MUDIDCount.ToString(), out mUDIDCount);
                            cell.SetCellValue(mUDIDCount);
                        }
                        else
                        {
                            cell.SetCellValue("");
                        }
                    }
                    cell.CellStyle = cellstyle;
                    cell = row.CreateCell(++a);
                    if (i == viewList.Count)
                    {
                        cell.SetCellValue(item.ApplierCount);
                    }
                    else
                    {
                        if (item.ApplierCount.ToString() != "")
                        {
                            double applierCount;
                            double.TryParse(item.ApplierCount.ToString(), out applierCount);
                            cell.SetCellValue(applierCount);
                        }
                        else
                        {
                            cell.SetCellValue("");
                        }
                    }
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
                LogHelper.Error("ExportPreAnalysisReport" + ex.Message);
            }
        }
        #endregion

        #region HT订单分析-Summary
        public ActionResult OrderSummary()
        {
            DateTime dt = DateTime.Now.AddDays(1).AddMonths(1).AddDays(-1);
            return View();
        }

        public JsonResult LoadOrderSummaryData(string Year, string Month, int HTType)
        {
            try
            {

                string beginDate = string.Empty;
                string endDate = string.Empty;
                string tagetDate = Year + "-" + Month + "-01 23:59:59";
                if (Year == "2019")
                {
                    beginDate = "2019-11-01 00:00:00";
                }
                else
                {
                    beginDate = Year + "-01-01 00:00:00";
                }
                beginDate = Year + "-01-01 00:00:00";
                endDate = Convert.ToDateTime(tagetDate).AddDays(1 - Convert.ToDateTime(tagetDate).Day).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss");
                List<P_ORDER_SUMMARY_OVERVIEW> orderOverviewList = new List<P_ORDER_SUMMARY_OVERVIEW>();
                List<P_ORDER_SUMMARY_OVERVIEW> orderOverviewListPrice = new List<P_ORDER_SUMMARY_OVERVIEW>();
                var markerTA = analysisService.LoadMarketTA(HTType);
                var oderOverview = analysisService.LoadOrderview(beginDate, endDate, HTType);

                for (int i = 0; i < markerTA.Count; i++)
                {
                    string[] tas = markerTA[i].TAS.Split(',');
                    var queryResult = (from a in oderOverview
                                       where tas.Contains(a.TA)
                                       group a by new { a.Month } into b
                                       select new
                                       {
                                           Mon = b.Key.Month,
                                           OrderCnt = b.Sum(c => c.OrderCnt),
                                           OrderPrice = b.Sum(c => c.OrderPrice)
                                       }).ToList();
                    #region 拼接每个market月份数据
                    string jan = ""; string janP = "";
                    string feb = ""; string febP = "";
                    string mar = ""; string marP = "";
                    string apr = ""; string aprP = "";
                    string may = ""; string mayP = "";
                    string jun = ""; string junP = "";
                    string jul = ""; string julP = "";
                    string aug = ""; string augP = "";
                    string sep = ""; string sepP = "";
                    string oct = ""; string octP = "";
                    string nov = ""; string novP = "";
                    string dec = ""; string decP = "";
                    int totalCnt = 0; double totalPrice = 0;
                    for (int j = 1; j <= int.Parse(Month); j++)
                    {
                        switch (j)
                        {
                            case 1:
                                if (queryResult.Count > 0 && queryResult.FirstOrDefault(c => c.Mon.Equals(j)) != null)
                                {
                                    jan = queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderCnt != 0 ? queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderCnt.ToString() : "";
                                    janP = queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderPrice != 0 ? queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderPrice.ToString() : "";
                                    totalCnt += queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderCnt;
                                    totalPrice += queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderPrice;
                                }
                                else
                                {
                                    jan = "0";
                                    janP = "0";
                                    totalCnt += 0;
                                    totalPrice += 0;
                                }
                                break;
                            case 2:
                                if (queryResult.Count > 0 && queryResult.FirstOrDefault(c => c.Mon.Equals(j)) != null)
                                {
                                    feb = queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderCnt != 0 ? queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderCnt.ToString() : "";
                                    febP = queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderPrice != 0 ? queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderPrice.ToString() : "";
                                    totalCnt += queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderCnt;
                                    totalPrice += queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderPrice;
                                }
                                else
                                {
                                    feb = "0";
                                    febP = "0";
                                    totalCnt += 0;
                                    totalPrice += 0;
                                }
                                break;
                            case 3:
                                if (queryResult.Count > 0 && queryResult.FirstOrDefault(c => c.Mon.Equals(j)) != null)
                                {
                                    mar = queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderCnt != 0 ? queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderCnt.ToString() : "";
                                    marP = queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderPrice != 0 ? queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderPrice.ToString() : "";
                                    totalCnt += queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderCnt;
                                    totalPrice += queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderPrice;
                                }
                                else
                                {
                                    mar = "0";
                                    marP = "0";
                                    totalCnt += 0;
                                    totalPrice += 0;
                                }
                                break;
                            case 4:
                                if (queryResult.Count > 0 && queryResult.FirstOrDefault(c => c.Mon.Equals(j)) != null)
                                {
                                    apr = queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderCnt != 0 ? queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderCnt.ToString() : "";
                                    aprP = queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderPrice != 0 ? queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderPrice.ToString() : "";
                                    totalCnt += queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderCnt;
                                    totalPrice += queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderPrice;
                                }
                                else
                                {
                                    apr = "0";
                                    aprP = "0";
                                    totalCnt += 0;
                                    totalPrice += 0;
                                }
                                break;
                            case 5:
                                if (queryResult.Count > 0 && queryResult.FirstOrDefault(c => c.Mon.Equals(j)) != null)
                                {
                                    may = queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderCnt != 0 ? queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderCnt.ToString() : "";
                                    mayP = queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderPrice != 0 ? queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderPrice.ToString() : "";
                                    totalCnt += queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderCnt;
                                    totalPrice += queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderPrice;
                                }
                                else
                                {
                                    may = "0";
                                    mayP = "0";
                                    totalCnt += 0;
                                    totalPrice += 0;
                                }
                                break;
                            case 6:
                                if (queryResult.Count > 0 && queryResult.FirstOrDefault(c => c.Mon.Equals(j)) != null)
                                {
                                    jun = queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderCnt != 0 ? queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderCnt.ToString() : "";
                                    junP = queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderPrice != 0 ? queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderPrice.ToString() : "";
                                    totalCnt += queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderCnt;
                                    totalPrice += queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderPrice;
                                }
                                else
                                {
                                    jun = "0";
                                    junP = "0";
                                    totalCnt += 0;
                                    totalPrice += 0;
                                }
                                break;
                            case 7:
                                if (queryResult.Count > 0 && queryResult.FirstOrDefault(c => c.Mon.Equals(j)) != null)
                                {
                                    jul = queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderCnt != 0 ? queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderCnt.ToString() : "";
                                    julP = queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderPrice != 0 ? queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderPrice.ToString() : "";
                                    totalCnt += queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderCnt;
                                    totalPrice += queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderPrice;
                                }
                                else
                                {
                                    jul = "0";
                                    julP = "0";
                                    totalCnt += 0;
                                    totalPrice += 0;
                                }
                                break;
                            case 8:
                                if (queryResult.Count > 0 && queryResult.FirstOrDefault(c => c.Mon.Equals(j)) != null)
                                {
                                    aug = queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderCnt != 0 ? queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderCnt.ToString() : "";
                                    augP = queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderPrice != 0 ? queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderPrice.ToString() : "";
                                    totalCnt += queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderCnt;
                                    totalPrice += queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderPrice;
                                }
                                else
                                {
                                    aug = "0";
                                    augP = "0";
                                    totalCnt += 0;
                                    totalPrice += 0;
                                }
                                break;
                            case 9:
                                if (queryResult.Count > 0 && queryResult.FirstOrDefault(c => c.Mon.Equals(j)) != null)
                                {
                                    sep = queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderCnt != 0 ? queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderCnt.ToString() : "";
                                    sepP = queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderPrice != 0 ? queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderPrice.ToString() : "";
                                    totalCnt += queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderCnt;
                                    totalPrice += queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderPrice;
                                }
                                else
                                {
                                    sep = "0";
                                    sepP = "0";
                                    totalCnt += 0;
                                    totalPrice += 0;
                                }
                                break;
                            case 10:
                                if (queryResult.Count > 0 && queryResult.FirstOrDefault(c => c.Mon.Equals(j)) != null)
                                {
                                    oct = queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderCnt != 0 ? queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderCnt.ToString() : "";
                                    octP = queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderPrice != 0 ? queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderPrice.ToString() : "";
                                    totalCnt += queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderCnt;
                                    totalPrice += queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderPrice;
                                }
                                else
                                {
                                    oct = "0";
                                    octP = "0";
                                    totalCnt += 0;
                                    totalPrice += 0;
                                }
                                break;
                            case 11:
                                if (queryResult.Count > 0 && queryResult.FirstOrDefault(c => c.Mon.Equals(j)) != null)
                                {
                                    nov = queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderCnt != 0 ? queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderCnt.ToString() : "";
                                    novP = queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderPrice != 0 ? queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderPrice.ToString() : "";
                                    totalCnt += queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderCnt;
                                    totalPrice += queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderPrice;
                                }
                                else
                                {
                                    nov = "0";
                                    novP = "0";
                                    totalCnt += 0;
                                    totalPrice += 0;
                                }
                                break;
                            case 12:
                                if (queryResult.Count > 0 && queryResult.FirstOrDefault(c => c.Mon.Equals(j)) != null)
                                {
                                    dec = queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderCnt != 0 ? queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderCnt.ToString() : "";
                                    decP = queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderPrice != 0 ? queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderPrice.ToString() : "";
                                    totalCnt += queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderCnt;
                                    totalPrice += queryResult.FirstOrDefault(c => c.Mon.Equals(j)).OrderPrice;
                                }
                                else
                                {
                                    dec = "0";
                                    decP = "0";
                                    totalCnt += 0;
                                    totalPrice += 0;
                                }
                                break;
                        }
                    }
                    #endregion
                    P_ORDER_SUMMARY_OVERVIEW order_summary_count_add = new P_ORDER_SUMMARY_OVERVIEW
                    {
                        Market = markerTA[i].Market,
                        Jan = jan,
                        Feb = feb,
                        Mar = mar,
                        Apr = apr,
                        May = may,
                        Jun = jun,
                        Jul = jul,
                        Aug = aug,
                        Sep = sep,
                        Oct = oct,
                        Nov = nov,
                        Dec = dec,
                        YTD = totalCnt.ToString()
                    };
                    orderOverviewList.Add(order_summary_count_add);

                    P_ORDER_SUMMARY_OVERVIEW order_summary_pric = new P_ORDER_SUMMARY_OVERVIEW
                    {
                        Market = markerTA[i].Market,
                        Jan = janP,
                        Feb = febP,
                        Mar = marP,
                        Apr = aprP,
                        May = mayP,
                        Jun = junP,
                        Jul = julP,
                        Aug = augP,
                        Sep = sepP,
                        Oct = octP,
                        Nov = novP,
                        Dec = decP,
                        YTD = totalPrice.ToString()
                    };
                    orderOverviewListPrice.Add(order_summary_pric);
                }
                P_ORDER_SUMMARY_OVERVIEW order_summary_count = new P_ORDER_SUMMARY_OVERVIEW
                {
                    Market = "Total",
                    Jan = orderOverviewList.Sum(k => (k.Jan == "" ? 0 : int.Parse(k.Jan))).ToString(),
                    Feb = orderOverviewList.Sum(k => (k.Feb == "" ? 0 : int.Parse(k.Feb))).ToString(),
                    Mar = orderOverviewList.Sum(k => (k.Mar == "" ? 0 : int.Parse(k.Mar))).ToString(),
                    Apr = orderOverviewList.Sum(k => (k.Apr == "" ? 0 : int.Parse(k.Apr))).ToString(),
                    May = orderOverviewList.Sum(k => (k.May == "" ? 0 : int.Parse(k.May))).ToString(),
                    Jun = orderOverviewList.Sum(k => (k.Jun == "" ? 0 : int.Parse(k.Jun))).ToString(),
                    Jul = orderOverviewList.Sum(k => (k.Jul == "" ? 0 : int.Parse(k.Jul))).ToString(),
                    Aug = orderOverviewList.Sum(k => (k.Aug == "" ? 0 : int.Parse(k.Aug))).ToString(),
                    Sep = orderOverviewList.Sum(k => (k.Sep == "" ? 0 : int.Parse(k.Sep))).ToString(),
                    Oct = orderOverviewList.Sum(k => (k.Oct == "" ? 0 : int.Parse(k.Oct))).ToString(),
                    Nov = orderOverviewList.Sum(k => (k.Nov == "" ? 0 : int.Parse(k.Nov))).ToString(),
                    Dec = orderOverviewList.Sum(k => (k.Dec == "" ? 0 : int.Parse(k.Dec))).ToString(),
                    YTD = orderOverviewList.Sum(k => (k.YTD == "" ? 0 : int.Parse(k.YTD))).ToString()
                };
                orderOverviewList.Add(order_summary_count);
                P_ORDER_SUMMARY_OVERVIEW order_summary_price = new P_ORDER_SUMMARY_OVERVIEW
                {
                    Market = "Total",
                    Jan = orderOverviewListPrice.Sum(k => (k.Jan == "" ? 0 : float.Parse(k.Jan))).ToString(),
                    Feb = orderOverviewListPrice.Sum(k => (k.Feb == "" ? 0 : float.Parse(k.Feb))).ToString(),
                    Mar = orderOverviewListPrice.Sum(k => (k.Mar == "" ? 0 : float.Parse(k.Mar))).ToString(),
                    Apr = orderOverviewListPrice.Sum(k => (k.Apr == "" ? 0 : float.Parse(k.Apr))).ToString(),
                    May = orderOverviewListPrice.Sum(k => (k.May == "" ? 0 : float.Parse(k.May))).ToString(),
                    Jun = orderOverviewListPrice.Sum(k => (k.Jun == "" ? 0 : float.Parse(k.Jun))).ToString(),
                    Jul = orderOverviewListPrice.Sum(k => (k.Jul == "" ? 0 : float.Parse(k.Jul))).ToString(),
                    Aug = orderOverviewListPrice.Sum(k => (k.Aug == "" ? 0 : float.Parse(k.Aug))).ToString(),
                    Sep = orderOverviewListPrice.Sum(k => (k.Sep == "" ? 0 : float.Parse(k.Sep))).ToString(),
                    Oct = orderOverviewListPrice.Sum(k => (k.Oct == "" ? 0 : float.Parse(k.Oct))).ToString(),
                    Nov = orderOverviewListPrice.Sum(k => (k.Nov == "" ? 0 : float.Parse(k.Nov))).ToString(),
                    Dec = orderOverviewListPrice.Sum(k => (k.Dec == "" ? 0 : float.Parse(k.Dec))).ToString(),
                    YTD = orderOverviewListPrice.Sum(k => (k.YTD == "" ? 0 : float.Parse(k.YTD))).ToString()
                };
                orderOverviewListPrice.Add(order_summary_price);
                return Json(new { state = 1, orderOverviewData = orderOverviewList, feeOverviewData = orderOverviewListPrice });
            }
            catch (Exception ex)
            {
                LogHelper.Error("LoadOrderSummaryData", ex);
                return Json(new { state = 0 });
            }
        }

        public ActionResult GroupSetting()
        {
            return View();
        }

        public JsonResult LoadMarketTAData()
        {
            try
            {
                var markerTA = analysisService.LoadMarketTAData();
                var htList = (from a in markerTA
                              where a.HTType == 1
                              group a by new { a.Market, a.HTType, a.OrderIndex } into b
                              select new
                              {
                                  b.Key.Market,
                                  b.Key.HTType,
                                  OrderIndex = b.Max(c => c.OrderIndex)
                              }).OrderBy(o => o.OrderIndex).ToList();
                var ohList = (from a in markerTA
                              where a.HTType == 2
                              group a by new { a.Market, a.HTType, a.OrderIndex } into b
                              select new
                              {
                                  b.Key.Market,
                                  b.Key.HTType,
                                  OrderIndex = b.Max(c => c.OrderIndex)
                              }).OrderBy(o => o.OrderIndex).ToList();
                var nonList = (from a in markerTA
                               where a.HTType == 3
                               group a by new { a.Market, a.HTType, a.OrderIndex } into b
                               select new
                               {
                                   b.Key.Market,
                                   b.Key.HTType,
                                   OrderIndex = b.Max(c => c.OrderIndex)
                               }).OrderBy(o => o.OrderIndex).ToList();

                return Json(new { state = 1, htData = htList, ohData = ohList, nonData = nonList });
            }
            catch (Exception ex)
            {
                LogHelper.Error("LoadMarketTAData", ex);
                return Json(new { state = 0 });
            }
        }

        public ActionResult AddGroupSetting(int HTType)
        {
            var res = exportManagementService.LoadTAForGroup();
            List<string> list = new List<string>();
            foreach (var item in res)
            {
                list.Add(item.Name);
            }
            if (HTType == 3)
            {
                list.Add("R&D-Rx");
                list.Add("R&D-Vx");
            }
            string join = string.Join(",", list);
            ViewBag.HTType = HTType;
            ViewBag.TAS = join;
            return View();
        }

        public JsonResult SaveGroupSetting(string Name, string TAS, int HTType)
        {
            try
            {
                var markerTA = analysisService.LoadMarketTAData();
                int index = 0;
                if (HTType == 1)
                    index = (from a in markerTA
                             where a.HTType == 1
                             group a by new { a.Market, a.HTType, a.OrderIndex } into b
                             select new
                             {
                                 b.Key.Market,
                                 b.Key.HTType,
                                 OrderIndex = b.Max(c => c.OrderIndex)
                             }).OrderBy(o => o.OrderIndex).ToList().Count;
                else if (HTType == 2)
                    index = (from a in markerTA
                             where a.HTType == 2
                             group a by new { a.Market, a.HTType, a.OrderIndex } into b
                             select new
                             {
                                 b.Key.Market,
                                 b.Key.HTType,
                                 OrderIndex = b.Max(c => c.OrderIndex)
                             }).OrderBy(o => o.OrderIndex).ToList().Count;
                else if (HTType == 3)
                    index = (from a in markerTA
                             where a.HTType == 3
                             group a by new { a.Market, a.HTType, a.OrderIndex } into b
                             select new
                             {
                                 b.Key.Market,
                                 b.Key.HTType,
                                 OrderIndex = b.Max(c => c.OrderIndex)
                             }).OrderBy(o => o.OrderIndex).ToList().Count;

                List<string> taList = new List<string>(TAS.Split(','));
                var res = analysisService.SaveGroupSetting(Name, taList, HTType, index + 1, "");
                if (res > 0)
                {
                    return Json(new { state = 1, txt = "设置成功！" });
                }
                else
                {
                    return Json(new { state = 1, txt = "设置失败！" });
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error("SaveGroupSetting", ex);
                return Json(new { state = 0, txt = "设置失败！" });
            }
        }

        public JsonResult DeleteGroupSetting(string Market, int HTType)
        {
            try
            {
                string mar = HttpUtility.UrlDecode(Market);
                var res = analysisService.DeleteGroupSetting(mar, HTType);
                if (res > 0)
                {
                    return Json(new { state = 1, txt = "删除成功！" });
                }
                else
                {
                    return Json(new { state = 1, txt = "删除失败！" });
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error("DeleteGroupSetting", ex);
                return Json(new { state = 0, txt = "删除失败！" });
            }
        }

        public ActionResult ModifyGroupSetting(string Market, int HTType)
        {
            string mar = HttpUtility.UrlDecode(Market);
            var markerTA = analysisService.LoadMarketTAData();
            var chkList = markerTA.Where(c => c.Market.Equals(mar) && c.HTType == HTType).ToList();
            List<string> listChk = new List<string>();
            foreach (var item in chkList)
            {
                listChk.Add(item.TA);
            }
            string ckTA = string.Join(",", listChk);

            var res = exportManagementService.LoadTAForGroup();
            List<string> list = new List<string>();
            foreach (var item in res)
            {
                list.Add(item.Name);
            }
            if(HTType == 3)
            {
                list.Add("R&D-Rx");
                list.Add("R&D-Vx");
            }
            string tas = string.Join(",", list);
            ViewBag.HTType = HTType;
            ViewBag.TAS = tas;
            ViewBag.CHKTAS = ckTA;
            ViewBag.Market = mar;
            ViewBag.OrderIndex = chkList[0].OrderIndex;
            return View();
        }

        public JsonResult UpdateGroupSetting(string Name, string TAS, int HTType, int OrderIndex, string OldMarket)
        {
            try
            {
                List<string> taList = new List<string>(TAS.Split(','));
                var res = analysisService.SaveGroupSetting(Name, taList, HTType, OrderIndex, OldMarket);
                if (res > 0)
                {
                    return Json(new { state = 1, txt = "修改成功！" });
                }
                else
                {
                    return Json(new { state = 1, txt = "修改失败！" });
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error("UpdateGroupSetting", ex);
                return Json(new { state = 0, txt = "修改失败！" });
            }
        }
        #endregion

        #region HT订单分析1-金额数量分析
        public ActionResult AnalysisAmountCnt()
        {
            var res = analysisService.LoadTAInOrderCost();
            List<string> list = new List<string>();
            foreach (var item in res)
            {
                list.Add(item.Name);
            }
            string join = string.Join(",", list);
            ViewBag.TAList = join;
            return View();
        }

        public JsonResult LoadOrderChart(string DeliverTimeBegin, string DeliverTimeEnd, string OrderState, string HTType, string TA, string RD, string RM, string DM)
        {
            try
            {
                DateTime? _DTBegin, _DTEnd;
                DateTime _tmpTime;
                string sltTA = "";
                string sltRD = "";
                string sltRM = "";
                string sltDM = "";
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
                if (RD == "ALL" || RD == "")
                {
                    sltRD = null;
                }
                else
                {
                    sltRD = "'" + RD + "'";
                }
                if (RM == "ALL" || RM == "")
                {
                    sltRM = null;
                }
                else
                {
                    sltRM = "'" + RM + "'";
                }
                if (DM == "ALL" || DM == "")
                {
                    sltDM = null;
                }
                else
                {
                    sltDM = "'" + DM + "'";
                }
                var res = analysisService.LoadOrderChart(_DTBegin, _DTEnd, OrderState, HTType, sltTA, sltRD, sltRM, sltDM);
                var chartCount = new List<P_Order_Count_View>();
                var chartAmount = new List<P_Order_Count_View>();
                var chartAttend = new List<P_Order_Count_View>();
                #region 订单数量
                var resCount = from q in res
                               orderby q.TA, q.OrderCount descending
                               select q;
                foreach (var item in resCount)
                {
                    if (TA.Contains("ALL"))
                    {
                        chartCount.Add(new P_Order_Count_View()
                        {
                            CodeandNAME = item.ConCode,
                            OrderCount = item.OrderCount,
                            OrderPrice = item.OrderPrice.ToString(),
                            OrderAmount = item.OrderPrice,
                            PreAttendCount = item.PreAttendCount
                        });
                    }
                    else
                    {
                        chartCount.Add(new P_Order_Count_View()
                        {
                            CodeandNAME = item.ConCode + "-" + item.NAME,
                            OrderCount = item.OrderCount,
                            OrderPrice = item.OrderPrice.ToString(),
                            OrderAmount = item.OrderPrice,
                            PreAttendCount = item.PreAttendCount
                        });
                    }
                }
                #endregion
                #region 订单金额
                var resAmount = from q in res
                                orderby q.TA, q.OrderPrice descending
                                select q;
                foreach (var item in resAmount)
                {
                    if (TA.Contains("ALL"))
                    {
                        chartAmount.Add(new P_Order_Count_View()
                        {
                            CodeandNAME = item.ConCode,
                            OrderCount = item.OrderCount,
                            OrderPrice = item.OrderPrice.ToString(),
                            OrderAmount = item.OrderPrice,
                            PreAttendCount = item.PreAttendCount
                        });
                    }
                    else
                    {
                        chartAmount.Add(new P_Order_Count_View()
                        {
                            CodeandNAME = item.ConCode + "-" + item.NAME,
                            OrderCount = item.OrderCount,
                            OrderPrice = item.OrderPrice.ToString(),
                            OrderAmount = item.OrderPrice,
                            PreAttendCount = item.PreAttendCount
                        });
                    }
                }
                #endregion
                #region 参会人数
                var resAttend = from q in res
                                orderby q.TA, q.PreAttendCount descending
                                select q;
                foreach (var item in resAttend)
                {
                    if (TA.Contains("ALL"))
                    {
                        chartAttend.Add(new P_Order_Count_View()
                        {
                            CodeandNAME = item.ConCode,
                            OrderCount = item.OrderCount,
                            OrderPrice = item.OrderPrice.ToString(),
                            OrderAmount = item.OrderPrice,
                            PreAttendCount = item.PreAttendCount
                        });
                    }
                    else
                    {
                        chartAttend.Add(new P_Order_Count_View()
                        {
                            CodeandNAME = item.ConCode + "-" + item.NAME,
                            OrderCount = item.OrderCount,
                            OrderPrice = item.OrderPrice.ToString(),
                            OrderAmount = item.OrderPrice,
                            PreAttendCount = item.PreAttendCount
                        });
                    }
                }
                #endregion

                return Json(new { state = 1, ChartCount = chartCount, ChartAmount = chartAmount, ChartAttend = chartAttend });
            }
            catch (Exception ex)
            {
                LogHelper.Error("LoadOrderChart" + ex.Message);
                return Json(new { state = 0 });
            }
        }

        public void ExportCntAmoAttData(string DeliverTimeBegin, string DeliverTimeEnd, string OrderState, string HTType, string TA, string RD, string RM, string DM)
        {
            try
            {
                #region 抓取数据
                DateTime? _DTBegin, _DTEnd;
                DateTime _tmpTime;
                string sltTA = "";
                string sltRD = "";
                string sltRM = "";
                string sltDM = "";
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
                if (RD == "ALL" || RD == "")
                {
                    sltRD = null;
                }
                else
                {
                    sltRD = "'" + RD + "'";
                }
                if (RM == "ALL" || RM == "")
                {
                    sltRM = null;
                }
                else
                {
                    sltRM = "'" + RM + "'";
                }
                if (DM == "ALL" || DM == "")
                {
                    sltDM = null;
                }
                else
                {
                    sltDM = "'" + DM + "'";
                }
                var res = analysisService.LoadCntAmoAttData(_DTBegin, _DTEnd, OrderState, HTType, sltTA, sltRD, sltRM, sltDM);
                var resHos = analysisService.LoadHosRankingData(_DTBegin, _DTEnd, OrderState, HTType, sltTA, sltRD, sltRM, sltDM);
                var chartCount = new List<P_Order_Count_View_Export>();
                var chartAmount = new List<P_Order_Count_View_Export>();
                var chartAttend = new List<P_Order_Count_View_Export>();
                #region 订单数量
                var resCount = from q in res
                               orderby q.TA, q.OrderCount descending
                               select q;
                foreach (var item in resCount)
                {
                    chartCount.Add(new P_Order_Count_View_Export()
                    {
                        CodeandNAME = item.ConCode,
                        Name = item.NAME,
                        Mudid = item.MUDID,
                        OrderCount = item.OrderCount,
                        OrderPrice = Convert.ToDecimal(item.OrderPrice),
                        PreAttendCount = item.PreAttendCount
                    });
                }
                #endregion
                #region 订单金额
                var resAmount = from q in res
                                orderby q.TA, q.OrderPrice descending
                                select q;
                foreach (var item in resCount)
                {
                    chartAmount.Add(new P_Order_Count_View_Export()
                    {
                        CodeandNAME = item.ConCode,
                        Name = item.NAME,
                        Mudid = item.MUDID,
                        OrderCount = item.OrderCount,
                        OrderPrice = Convert.ToDecimal(item.OrderPrice),
                        PreAttendCount = item.PreAttendCount
                    });
                }
                #endregion
                #region 参会人数
                var resAttend = from q in res
                                orderby q.TA, q.PreAttendCount descending
                                select q;
                foreach (var item in resCount)
                {
                    chartAttend.Add(new P_Order_Count_View_Export()
                    {
                        CodeandNAME = item.ConCode,
                        Name = item.NAME,
                        Mudid = item.MUDID,
                        OrderCount = item.OrderCount,
                        OrderPrice = Convert.ToDecimal(item.OrderPrice),
                        PreAttendCount = item.PreAttendCount
                    });
                }
                #endregion
                #endregion

                #region 构建Excel
                FileStream file11 = new FileStream(Server.MapPath("~/Template/Template_HT_OrderCntAmoAttAnalysis.xls"), FileMode.Open, FileAccess.Read);
                HSSFWorkbook wk = new HSSFWorkbook(file11);

                ISheet sheet1 = wk.GetSheet("HT订单数量");
                ISheet sheet2 = wk.GetSheet("HT订单金额");
                ISheet sheet3 = wk.GetSheet("HT参会人数");
                ISheet sheet4 = wk.GetSheet("HT医院排行");

                #endregion

                #region 制作表体
                for (var i = 1; i <= chartCount.Count; i++)
                {
                    var item = chartCount[i - 1];
                    IRow row = sheet1.CreateRow(i);
                    ICell cell = null;
                    var j = 0;
                    if (item != null)
                    {
                        cell = row.CreateCell(j);
                        cell.SetCellValue(item.CodeandNAME);
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.Name);
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.Mudid);
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.OrderCount);
                    }
                }
                for (var i = 1; i <= chartAmount.Count; i++)
                {
                    var item = chartAmount[i - 1];
                    IRow row = sheet2.CreateRow(i);
                    ICell cell = null;
                    var j = 0;
                    if (item != null)
                    {
                        cell = row.CreateCell(j);
                        cell.SetCellValue(item.CodeandNAME);
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.Name);
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.Mudid);
                        cell = row.CreateCell(++j);
                        double orderPrice;
                        double.TryParse(item.OrderPrice.ToString(), out orderPrice);
                        cell.SetCellValue(orderPrice);
                    }
                }
                for (var i = 1; i <= chartAttend.Count; i++)
                {
                    var item = chartAttend[i - 1];
                    IRow row = sheet3.CreateRow(i);
                    ICell cell = null;
                    var j = 0;
                    if (item != null)
                    {
                        cell = row.CreateCell(j);
                        cell.SetCellValue(item.CodeandNAME);
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.Name);
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.Mudid);
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.PreAttendCount);
                    }
                }
                for (var i = 1; i <= resHos.Count; i++)
                {
                    var item = resHos[i - 1];
                    IRow row = sheet4.CreateRow(i);
                    ICell cell = null;
                    var j = 0;
                    if (item != null)
                    {
                        cell = row.CreateCell(j);
                        cell.SetCellValue(item.HospitalId);
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.HospitalName);
                        cell = row.CreateCell(++j);
                        double orderPrice;
                        double.TryParse(item.OrderPrice.ToString(), out orderPrice);
                        cell.SetCellValue(orderPrice);
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
                LogHelper.Error("" + ex.Message);
            }
        }
        #endregion

        #region HT订单分析2
        public void ExportResOrderData(string Year, string Month, string HTType, string TA)
        {
            try
            {
                #region 抓取数据 
                var res = analysisService.LoadTAInOrderCost();
                List<string> taList = new List<string>();
                List<string> taViewList = new List<string>();
                foreach (var item in res)
                {
                    taList.Add(item.Name);
                }

                string date = Year + "-" + Month + "-01";

                if (TA.Contains("ALL") || TA.Contains("null"))
                {
                    taViewList = taList;
                }
                else
                {
                    string[] str = TA.Split(',');
                    foreach (var item in taList)
                    {
                        if (str.Contains(item))
                            taViewList.Add(item);
                    }
                }
                //订单数据
                var orderList = analysisService.LoadOrderList(date, HTType);
                var orderListAl = (from a in orderList
                                   select new
                                   {
                                       GskHospital = a.HospitalId.Replace("-OH", ""),
                                       a.TA
                                   }).ToList();

                //TA-医院 包含MR数据
                var taHospitalList = analysisService.LoadTAHospitalList(date);

                //TA-院外医院 包含MR数据
                var taHospitalOHList = analysisService.LoadTAHospitalOHList(date);

                //TA-医院 按TA去重后的数据
                var taHospitalFilter = (from a in taHospitalList
                                        group a by new { a.TERRITORY_TA, a.HospitalCode } into b
                                        select new
                                        {
                                            b.Key.TERRITORY_TA,
                                            b.Key.HospitalCode
                                        }).ToList();

                //可送餐数据表数据
                var resHospital = analysisService.LoadResHospital(Year + "-" + Month);

                //可送餐（GskHospital替换掉-OH）的全部数据
                var resHosAllList = (from a in resHospital
                                     group a by new { a.GskHospital } into b
                                     select new
                                     {
                                         GskHospital = b.Key.GskHospital.Replace("-OH", "")
                                     }).ToList();
                //可送餐院内数据
                var resHospitalHT = (from a in resHospital
                                     where !a.GskHospital.Contains("-OH")
                                     group a by new { a.GskHospital } into b
                                     select new
                                     {
                                         GskHospital = b.Key.GskHospital.Replace("-OH", "")
                                     }).ToList();

                //可送餐院外数据
                var resHospitalOH = (from a in resHospital
                                     where a.GskHospital.Contains("-OH")
                                     group a by new { a.GskHospital } into b
                                     select new
                                     {
                                         GskHospital = b.Key.GskHospital.Replace("-OH", "")
                                     }).ToList();

                List<P_ORDER_ANALYSIS_2_EXPORT_LIST> viewList = new List<P_ORDER_ANALYSIS_2_EXPORT_LIST>();
                List<string> totalResList = new List<string>();
                List<string> totalHosList = new List<string>();
                List<string> totalDMMRList = new List<string>();
                List<string> totalMRList = new List<string>();
                foreach (var ta in taViewList)
                {
                    string v1 = ta;              //TA
                    string v2 = string.Empty;    //HT类型
                    int v3 = 0;    //可送餐医院数
                    int v4 = 0;                  //产生订单医院数 
                    int v5 = 0;    //可订餐代表数
                    int v6 = 0;                  //产生订单代表数
                    int v7 = 0;                  //订单数 
                    decimal v8 = 0;                  //订单金额

                    if (HTType == "0" || HTType == "1")
                    {
                        v5 = (taHospitalList.Where(c => c.TERRITORY_TA.Equals(ta) && c.MUD_ID_MR != null && c.MUD_ID_MR != "").Select(i => new { i.MUD_ID_MR }).Distinct().Count() + taHospitalList.Where(c => c.TERRITORY_TA.Equals(ta) && c.MUD_ID_DM != null && c.MUD_ID_DM != "").Select(i => new { i.MUD_ID_DM }).Distinct().Count());
                        foreach (var item in taHospitalList.Where(c => c.TERRITORY_TA.Equals(ta) && c.MUD_ID_MR != null && c.MUD_ID_MR != "").Select(i => new { i.MUD_ID_MR }).Distinct())
                        {
                            totalDMMRList.Add(item.MUD_ID_MR);
                        }
                        foreach (var item in taHospitalList.Where(c => c.TERRITORY_TA.Equals(ta) && c.MUD_ID_DM != null && c.MUD_ID_DM != "").Select(i => new { i.MUD_ID_DM }).Distinct())
                        {
                            totalDMMRList.Add(item.MUD_ID_DM);
                        }
                    }
                    else
                    {
                        v5 = (taHospitalOHList.Where(c => c.TERRITORY_TA.Equals(ta) && c.MUD_ID_MR != null && c.MUD_ID_MR != "").Select(i => new { i.MUD_ID_MR }).Distinct().Count() + taHospitalOHList.Where(c => c.TERRITORY_TA.Equals(ta) && c.MUD_ID_DM != null && c.MUD_ID_DM != "").Select(i => new { i.MUD_ID_DM }).Distinct().Count());
                        foreach (var item in taHospitalOHList.Where(c => c.TERRITORY_TA.Equals(ta) && c.MUD_ID_MR != null && c.MUD_ID_MR != "").Select(i => new { i.MUD_ID_MR }).Distinct())
                        {
                            totalDMMRList.Add(item.MUD_ID_MR);
                        }
                        foreach (var item in taHospitalOHList.Where(c => c.TERRITORY_TA.Equals(ta) && c.MUD_ID_DM != null && c.MUD_ID_DM != "").Select(i => new { i.MUD_ID_DM }).Distinct())
                        {
                            totalDMMRList.Add(item.MUD_ID_DM);
                        }
                        
                    }
                    List<P_ORDER_LIST_VIEW> oList = new List<P_ORDER_LIST_VIEW>();
                    if (HTType == "0")
                    {
                        v2 = "全部HT";
                        var oListQur = (from a in orderListAl
                                        where a.TA.Equals(ta)
                                        group a by new { a.GskHospital } into b
                                        select new
                                        {
                                            b.Key.GskHospital
                                        }).ToList();

                        oList = orderList.Where(c => c.TA.Equals(ta)).ToList();
                        var rListAll = (from a in taHospitalFilter
                                        join b in resHosAllList on a.HospitalCode equals b.GskHospital into c
                                        from v in c
                                        where a.TERRITORY_TA.Equals(ta)
                                        select new
                                        {
                                            HospitalID = v.GskHospital
                                        }).ToList();
                        foreach (var item in rListAll)
                        {
                            totalResList.Add(item.HospitalID);
                        }
                        v3 = rListAll.Select(i => new { i.HospitalID }).Distinct().Count();
                        v4 = oListQur.Count;
                        foreach (var item in oListQur)
                        {
                            totalHosList.Add(item.GskHospital);
                        }
                        var list = (from a in orderList
                                    where a.TA.Equals(ta)
                                    group a by new { a.UserId } into b
                                    select new
                                    {
                                        b.Key.UserId
                                    }).ToList();
                        v6 = list.Count;
                        foreach (var item in list)
                        {
                            totalMRList.Add(item.UserId);
                        }
                        v7 = oList.Count;
                        v8 = oList.Sum(c => c.GSKConfirmAmount);
                    }
                    else if (HTType == "1")
                    {
                        v2 = "院内HT";
                        var rListHT = (from a in taHospitalFilter
                                       join b in resHospitalHT on a.HospitalCode equals b.GskHospital into c
                                       from v in c
                                       where a.TERRITORY_TA.Equals(ta)
                                       select new
                                       {
                                           HospitalID = v.GskHospital
                                       }).ToList();
                        foreach (var item in rListHT)
                        {
                            totalResList.Add(item.HospitalID);
                        }
                        v3 = rListHT.Count;
                        oList = orderList.Where(c => c.TA.Equals(ta) && c.HTType.Equals(int.Parse(HTType))).ToList();
                        var hosList = orderList.Where(c => c.TA.Equals(ta) && c.HTType.Equals(int.Parse(HTType))).Select(i => new { i.HospitalId }).Distinct();
                        v4 = hosList.Count();
                        foreach (var item in hosList)
                        {
                            totalHosList.Add(item.HospitalId);
                        }
                        var list = (from a in orderList
                                    where a.TA.Equals(ta) && a.HTType.Equals(int.Parse(HTType))
                                    group a by new { a.UserId } into b
                                    select new
                                    {
                                        b.Key.UserId
                                    }).ToList();
                        v6 = list.Count;
                        foreach (var item in list)
                        {
                            totalMRList.Add(item.UserId);
                        }
                        v7 = oList.Count;
                        v8 = oList.Sum(c => c.GSKConfirmAmount);
                    }
                    else if (HTType == "2")
                    {
                        v2 = "院外HT";
                        oList = orderList.Where(c => c.TA.Equals(ta) && c.HTType.Equals(int.Parse(HTType))).ToList();
                        var rListOH = (from a in taHospitalFilter
                                       join b in resHospitalOH on a.HospitalCode equals b.GskHospital into c
                                       from v in c
                                       where a.TERRITORY_TA.Equals(ta)
                                       select new
                                       {
                                           HospitalID = v.GskHospital
                                       }).ToList();
                        foreach (var item in rListOH)
                        {
                            totalResList.Add(item.HospitalID);
                        }
                        v3 = rListOH.Count;
                        var hosList = orderList.Where(c => c.TA.Equals(ta) && c.HTType.Equals(int.Parse(HTType))).Select(i => new { i.HospitalId }).Distinct();
                        v4 = hosList.Count();
                        foreach (var item in hosList)
                        {
                            totalHosList.Add(item.HospitalId);
                        }
                        var list = (from a in orderList
                                    where a.TA.Equals(ta) && a.HTType.Equals(int.Parse(HTType))
                                    group a by new { a.UserId } into b
                                    select new
                                    {
                                        b.Key.UserId
                                    }).ToList();
                        v6 = list.Count;
                        foreach (var item in list)
                        {
                            totalMRList.Add(item.UserId);
                        }
                        v7 = oList.Count;
                        v8 = oList.Sum(c => c.GSKConfirmAmount);
                    }

                    P_ORDER_ANALYSIS_2_EXPORT_LIST p_ORDER_ANALYSIS_2_EXPORT = new P_ORDER_ANALYSIS_2_EXPORT_LIST
                    {
                        TA = v1,
                        HTType = v2,
                        ResHospitalCnt = v3 == 0 ? "" : v3.ToString("N").Substring(0, v3.ToString("N").Length - 3),
                        OrderHospitalCnt = v4.ToString(),
                        ResMRCnt = v5 == 0 ? "" : v5.ToString("N").Substring(0, v5.ToString("N").Length - 3),
                        OrderMRCnt = v6.ToString(),
                        OrderCnt = v7.ToString(),
                        OrderPrice = v8.ToString()
                    };
                    viewList.Add(p_ORDER_ANALYSIS_2_EXPORT);
                }
                var totalResCnt = totalResList.Distinct().ToList().Count();
                var totalHosCnt = totalHosList.Distinct().ToList().Count();
                var totalDMMRCnt = totalDMMRList.Distinct().Count();
                var totalMRCnt = totalMRList.Distinct().ToList().Count();
                P_ORDER_ANALYSIS_2_EXPORT_LIST p_ORDER_ANALYSIS_2_EXPORT_T = new P_ORDER_ANALYSIS_2_EXPORT_LIST
                {
                    TA = "",
                    HTType = "",
                    ResHospitalCnt = totalResCnt == 0 ? "" : "合计：" + totalResCnt.ToString("N").Substring(0, totalResCnt.ToString("N").Length - 3),
                    OrderHospitalCnt = totalHosCnt == 0 ? "" : "合计：" + totalHosCnt.ToString("N").Substring(0, totalHosCnt.ToString("N").Length - 3),
                    ResMRCnt = totalDMMRCnt == 0 ? "" : "合计：" + totalDMMRCnt.ToString("N").Substring(0, totalDMMRCnt.ToString("N").Length - 3),
                    OrderMRCnt = totalMRCnt == 0 ? "" : "合计：" + totalMRCnt.ToString("N").Substring(0, totalMRCnt.ToString("N").Length - 3),
                    OrderCnt = "",
                    OrderPrice = ""
                };
                viewList.Add(p_ORDER_ANALYSIS_2_EXPORT_T);
                #endregion

                #region 构建Excel
                FileStream file11 = new FileStream(Server.MapPath("~/Template/Template_HT_OrderAnalysis2.xls"), FileMode.Open, FileAccess.Read);
                HSSFWorkbook wk = new HSSFWorkbook(file11);

                ISheet sheet1 = wk.GetSheet("Export");
                ICellStyle cellstyle = wk.CreateCellStyle();
                cellstyle.VerticalAlignment = VerticalAlignment.Center;
                cellstyle.Alignment = HorizontalAlignment.Center;
                IDataFormat dataformat1 = wk.CreateDataFormat();
                cellstyle.DataFormat = dataformat1.GetFormat("#,###");

                #endregion

                #region 制作表体
                for (var i = 1; i <= viewList.Count; i++)
                {
                    var item = viewList[i - 1];
                    IRow row = sheet1.CreateRow(i);
                    ICell cell = null;
                    var j = 0;
                    if (item != null)
                    {
                        cell = row.CreateCell(j);
                        cell.SetCellValue(item.TA);
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.HTType);

                        cell = row.CreateCell(++j);
                        if (i == viewList.Count)
                        {
                            cell.SetCellValue(item.ResHospitalCnt);
                        }
                        else
                        {
                            if (item.ResHospitalCnt.ToString() != "")
                            {
                                double zeroCount;
                                double.TryParse(item.ResHospitalCnt.ToString(), out zeroCount);
                                cell.SetCellValue(zeroCount);
                            }
                            else
                            {
                                cell.SetCellValue("");
                            }

                        }
                        cell.CellStyle = cellstyle;
                        //cell.SetCellValue(item.ResHospitalCnt);

                        cell = row.CreateCell(++j);
                        if (i == viewList.Count)
                        {
                            cell.SetCellValue(item.OrderHospitalCnt);
                        }
                        else
                        {
                            if (item.OrderHospitalCnt.ToString() != "")
                            {
                                double zeroCount;
                                double.TryParse(item.OrderHospitalCnt.ToString(), out zeroCount);
                                cell.SetCellValue(zeroCount);
                            }
                            else
                            {
                                cell.SetCellValue("");
                            }

                        }
                        cell.CellStyle = cellstyle;
                        //cell.SetCellValue(item.OrderHospitalCnt);

                        cell = row.CreateCell(++j);
                        if (i == viewList.Count)
                        {
                            cell.SetCellValue(item.ResMRCnt);
                        }
                        else
                        {
                            if (item.ResMRCnt.ToString() != "")
                            {
                                double zeroCount;
                                double.TryParse(item.ResMRCnt.ToString(), out zeroCount);
                                cell.SetCellValue(zeroCount);
                            }
                            else
                            {
                                cell.SetCellValue("");
                            }

                        }
                        cell.CellStyle = cellstyle;
                        //cell.SetCellValue(item.ResMRCnt);

                        cell = row.CreateCell(++j);
                        if (i == viewList.Count)
                        {
                            cell.SetCellValue(item.OrderMRCnt);
                        }
                        else
                        {
                            if (item.OrderMRCnt.ToString() != "")
                            {
                                double zeroCount;
                                double.TryParse(item.OrderMRCnt.ToString(), out zeroCount);
                                cell.SetCellValue(zeroCount);
                            }
                            else
                            {
                                cell.SetCellValue("");
                            }

                        }
                        cell.CellStyle = cellstyle;
                        //cell.SetCellValue(item.OrderMRCnt);

                        cell = row.CreateCell(++j);
                        if (i == viewList.Count)
                        {
                            cell.SetCellValue(item.OrderCnt);
                        }
                        else
                        {
                            if (item.OrderCnt.ToString() != "")
                            {
                                double zeroCount;
                                double.TryParse(item.OrderCnt.ToString(), out zeroCount);
                                cell.SetCellValue(zeroCount);
                            }
                            else
                            {
                                cell.SetCellValue("");
                            }

                        }
                        cell.CellStyle = cellstyle;
                        //cell.SetCellValue(item.OrderCnt);

                        cell = row.CreateCell(++j);
                        if (i == viewList.Count)
                        {
                            cell.SetCellValue(item.OrderPrice);
                        }
                        else
                        {
                            double orderPrice;
                            double.TryParse(item.OrderPrice.ToString(), out orderPrice);
                            cell.SetCellValue(orderPrice);
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
                LogHelper.Error("ExportResOrderData" + ex.Message);
            }
        }
        #endregion

        #region HT订单分析4-餐厅订单
        public ActionResult AnalysisRestaurantOrder()
        {
            var res = analysisService.LoadTAInOrderCost();
            List<string> list = new List<string>();
            foreach (var item in res)
            {
                list.Add(item.Name);
            }
            string join = string.Join(",", list);
            ViewBag.TAList = join;
            return View();
        }

        public JsonResult LoadRestaurantOrderData(string Year, string Month, string HTType, string TA)
        {
            try
            {
                int s1_ht = 0; int s2_ht = 0; string s3_ht = string.Empty; int s4_ht = 0;
                int s1_oh = 0; int s2_oh = 0; string s3_oh = string.Empty; int s4_oh = 0;
                string sltTA = string.Empty;
                string date = Year + "-" + Month + "-01";
                if (TA.Contains("ALL"))
                {
                    sltTA = null;
                }
                else
                {
                    sltTA = "'" + TA.Replace(",", "','") + "'";
                }

                //订单数据
                var orderList = analysisService.LoadOrderListByTA(date, HTType, sltTA);
                //已订餐医院数
                var hospitalMealList = orderList.Select(i => new { i.HospitalId }).Distinct().ToList();

                //TA下所有医院数据(不包括门地址)
                var taHospitalList = analysisService.LoadTAHospitalListByTA(date, sltTA, HTType);
                //目标医院数
                var targetHospitalList = (from a in taHospitalList
                                          group a by new { a.GskHospitalView } into b
                                          select new
                                          {
                                              b.Key.GskHospitalView
                                          }).ToList();
                //TA下院外数据
                //var ohHospitalList = taHospitalList.Where(c => c.GskHospitalView!=null && c.GskHospitalView.Contains("-OH")).Select(i => new { i.GskHospitalView }).Distinct().ToList();

                //可送餐数据表数据--包含门地址覆盖（门地址去掉下划线）
                var resHospital = analysisService.LoadResHospital(Year + "-" + Month);
                var coveredHospitalList = (from a in targetHospitalList
                                           join b in resHospital on a.GskHospitalView equals b.GskHospital into c
                                           from v in c
                                           where v.ResIdCnt > 0
                                           select new
                                           {
                                               HospitalID = v.GskHospital
                                           }).ToList();
                //var corverOHHospitalList = (from a in ohHospitalList
                //                            join b in resHospital on a.GskHospitalView equals b.GskHospital into c
                //                            from v in c
                //                            where v.ResIdCnt > 0
                //                            select new
                //                            {
                //                                HospitalID = v.GskHospital
                //                            }).ToList();

                List<P_ORDER_ANALYSIS_CHART_COUNT_VIEW> chart_count_view_list = new List<P_ORDER_ANALYSIS_CHART_COUNT_VIEW>();
                P_ORDER_ANALYSIS_CHART_COUNT_VIEW chart_target = new P_ORDER_ANALYSIS_CHART_COUNT_VIEW
                {
                    CodeAndNAME = "Target Hospitals",
                    ChartCount = targetHospitalList.Count
                };
                P_ORDER_ANALYSIS_CHART_COUNT_VIEW chart_covered = new P_ORDER_ANALYSIS_CHART_COUNT_VIEW
                {
                    CodeAndNAME = "Covered Hospitals",
                    ChartCount = coveredHospitalList.Count
                };
                P_ORDER_ANALYSIS_CHART_COUNT_VIEW chart_mael = new P_ORDER_ANALYSIS_CHART_COUNT_VIEW
                {
                    CodeAndNAME = "Hospitals With Meal",
                    ChartCount = hospitalMealList.Count
                };
                chart_count_view_list.Add(chart_target);
                chart_count_view_list.Add(chart_covered);
                chart_count_view_list.Add(chart_mael);

                s1_ht = orderList.Where(c => c.HTType.Equals(1)).ToList().Count;
                s2_ht = orderList.Where(c => c.HTType.Equals(1)).Select(i => new { i.HospitalId }).Distinct().ToList().Count;
                s4_ht = coveredHospitalList.Count;
                
                s3_ht = s4_ht == 0 ? "0.00" : (Math.Round(((decimal)s2_ht / s4_ht), 4) * 100).ToString("#0.00");

                s1_oh = orderList.Where(c => c.HTType.Equals(2)).ToList().Count;
                s2_oh = orderList.Where(c => c.HTType.Equals(2)).Select(i => new { i.HospitalId }).Distinct().ToList().Count;
                s4_oh = coveredHospitalList.Count;
                s3_oh = s4_oh == 0 ? "0.00" : (Math.Round(((decimal)s2_oh / s4_oh), 4) * 100).ToString("#0.00");
                P_ORDER_ANALYSIS_SUMMARY_VIEW p_ORDER_ANALYSIS_SUMMARY_VIEW = new P_ORDER_ANALYSIS_SUMMARY_VIEW
                {
                    s1ht = s1_ht,
                    s2ht = s2_ht,
                    s3ht = s3_ht,
                    s4ht = s4_ht,
                    s1oh = s1_oh,
                    s2oh = s2_oh,
                    s3oh = s3_oh,
                    s4oh = s4_oh
                };

                List<P_ORDER_ANALYSIS_CHART_COUNT_VIEW> chart_count_view_list_sec = new List<P_ORDER_ANALYSIS_CHART_COUNT_VIEW>();
                //产生订单医院数--已关联餐厅覆盖数量
                var hosMealQur = (from a in hospitalMealList
                                  join b in resHospital on a.HospitalId equals b.GskHospital into c
                                  from v in c
                                  select new
                                  {
                                      HospitalID = v.GskHospital,
                                      v.ResIdCnt
                                  }).ToList();
                //订单数--已关联餐厅覆盖数量
                var orderQur = (from a in orderList
                                join b in resHospital on a.HospitalId equals b.GskHospital into c
                                from v in c
                                select new
                                {
                                    HospitalID = v.GskHospital,
                                    v.ResIdCnt
                                }).ToList();

                P_ORDER_ANALYSIS_CHART_COUNT_VIEW chart1 = new P_ORDER_ANALYSIS_CHART_COUNT_VIEW
                {
                    CodeAndNAME = "1 Restaurant",
                    ChartCount = hosMealQur.Where(c => c.ResIdCnt == 1).ToList().Count,
                    ChartCount1 = orderQur.Where(c => c.ResIdCnt == 1).ToList().Count
                };
                chart_count_view_list_sec.Add(chart1);
                P_ORDER_ANALYSIS_CHART_COUNT_VIEW chart2 = new P_ORDER_ANALYSIS_CHART_COUNT_VIEW
                {
                    CodeAndNAME = "2 Restaurants",
                    ChartCount = hosMealQur.Where(c => c.ResIdCnt == 2).ToList().Count,
                    ChartCount1 = orderQur.Where(c => c.ResIdCnt == 2).ToList().Count
                };
                chart_count_view_list_sec.Add(chart2);
                P_ORDER_ANALYSIS_CHART_COUNT_VIEW chart3 = new P_ORDER_ANALYSIS_CHART_COUNT_VIEW
                {
                    CodeAndNAME = "3 Restaurants",
                    ChartCount = hosMealQur.Where(c => c.ResIdCnt == 3).ToList().Count,
                    ChartCount1 = orderQur.Where(c => c.ResIdCnt == 3).ToList().Count
                };
                chart_count_view_list_sec.Add(chart3);
                P_ORDER_ANALYSIS_CHART_COUNT_VIEW chart4 = new P_ORDER_ANALYSIS_CHART_COUNT_VIEW
                {
                    CodeAndNAME = "4 Restaurants",
                    ChartCount = hosMealQur.Where(c => c.ResIdCnt == 4).ToList().Count,
                    ChartCount1 = orderQur.Where(c => c.ResIdCnt == 4).ToList().Count
                };
                chart_count_view_list_sec.Add(chart4);
                P_ORDER_ANALYSIS_CHART_COUNT_VIEW chart5 = new P_ORDER_ANALYSIS_CHART_COUNT_VIEW
                {
                    CodeAndNAME = ">=5 Restaurants",
                    ChartCount = hosMealQur.Where(c => c.ResIdCnt >= 5).ToList().Count,
                    ChartCount1 = orderQur.Where(c => c.ResIdCnt >= 5).ToList().Count
                };
                chart_count_view_list_sec.Add(chart5);

                return Json(new { state = 1, firstChartData = chart_count_view_list, secondChartData = chart_count_view_list_sec, summaryView = p_ORDER_ANALYSIS_SUMMARY_VIEW });
            }
            catch (Exception ex)
            {
                LogHelper.Error("LoadRestaurantOrderData" + ex.Message);
                return Json(new { state = 0 });
            }
        }
        #endregion

        #region 订单排名
        public void ExportOrderRanking(string DeliverTimeBegin, string DeliverTimeEnd, string OrderState, string HTType, string TA, bool ChkApplier, bool ChkHospital, bool ChkRestaurant)
        {
            try
            {
                #region 抓取数据
                DateTime? _DTBegin, _DTEnd;
                DateTime _tmpTime;
                string sltTA = "";
                string sltRD = "";
                string sltRM = "";
                string sltDM = "";
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
                //代表维度排名
                List<P_APPLIER_RANKING> resApplierList = new List<P_APPLIER_RANKING>();
                if (ChkApplier)
                    resApplierList = analysisService.LoadApplierRankingData(_DTBegin, _DTEnd, OrderState, HTType, sltTA);
                //医院维度排名
                List<P_HOSPITAL_RANKING> resHospitalList = new List<P_HOSPITAL_RANKING>();
                if (ChkHospital)
                    resHospitalList = analysisService.LoadHospitalRankingData(_DTBegin, _DTEnd, OrderState, HTType, sltTA);
                //餐厅维度排名
                List<P_RESTAURANT_RANKING> resRestaurantList = new List<P_RESTAURANT_RANKING>();
                if (ChkRestaurant)
                    resRestaurantList = analysisService.LoadRestaurantRankingData(_DTBegin, _DTEnd, OrderState, HTType);

                #endregion

                #region 构建Excel
                FileStream fileApplier = new FileStream(Server.MapPath("~/Template/Template_HT_OrderAppHosResRanking.xls"), FileMode.Open, FileAccess.Read);
                HSSFWorkbook wk = new HSSFWorkbook(fileApplier);
                ISheet sheet1 = wk.GetSheet("订单数量排名-代表");
                ISheet sheet2 = wk.GetSheet("订单金额排名-代表");

                ISheet sheet3 = wk.GetSheet("订单数量排名-医院");
                ISheet sheet4 = wk.GetSheet("订单金额排名-医院");

                ISheet sheet5 = wk.GetSheet("订单数量排名-餐厅");
                ISheet sheet6 = wk.GetSheet("订单金额排名-餐厅");

                #endregion

                #region 订单排名--代表
                if (resApplierList != null && resApplierList.Count > 0)
                {
                    //sheet1按订单数量降序
                    var rCnt = (from q in resApplierList
                                orderby q.OrderCount descending
                                select q).ToList();
                    for (var i = 1; i <= rCnt.Count; i++)
                    {
                        var item = rCnt[i - 1];
                        IRow row = sheet1.CreateRow(i);
                        ICell cell = null;
                        var j = 0;
                        if (item != null)
                        {
                            cell = row.CreateCell(j);
                            cell.SetCellValue(item.TA);
                            cell = row.CreateCell(++j);
                            cell.SetCellValue(item.ApplierName);
                            cell = row.CreateCell(++j);
                            cell.SetCellValue(item.ApplierMUDID);
                            cell = row.CreateCell(++j);
                            cell.SetCellValue(item.OrderCount);
                            cell = row.CreateCell(++j);
                            double orderPrice;
                            double.TryParse(item.OrderPrice.ToString(), out orderPrice);
                            cell.SetCellValue(orderPrice);
                            cell = row.CreateCell(++j);
                            cell.SetCellValue(item.RDTerritoryCode);
                            cell = row.CreateCell(++j);
                            cell.SetCellValue(item.RDName);
                            cell = row.CreateCell(++j);
                            cell.SetCellValue(item.MUD_ID_RD);
                        }
                    }
                    var totalCnt = rCnt.Sum(c => c.OrderCount) == 0 ? "" : "合计：" + rCnt.Sum(c => c.OrderCount).ToString("N").Substring(0, rCnt.Sum(c => c.OrderCount).ToString("N").Length - 3);
                    var totalAmount = rCnt.Sum(c => (decimal)c.OrderPrice) == 0 ? "¥0.00" : "合计：" + rCnt.Sum(c => c.OrderPrice).ToString("C").Replace('$', '¥');
                    IRow row1 = sheet1.CreateRow(rCnt.Count+1);
                    ICell cell1 = null;
                    var m = 0;
                    cell1 = row1.CreateCell(m);
                    cell1.SetCellValue("");
                    cell1 = row1.CreateCell(++m);
                    cell1.SetCellValue("");
                    cell1 = row1.CreateCell(++m);
                    cell1.SetCellValue("");
                    cell1 = row1.CreateCell(++m);
                    cell1.SetCellValue(totalCnt);
                    cell1 = row1.CreateCell(++m);
                    cell1.SetCellValue(totalAmount);
                    cell1 = row1.CreateCell(++m);
                    cell1.SetCellValue("");
                    cell1 = row1.CreateCell(++m);
                    cell1.SetCellValue("");
                    cell1 = row1.CreateCell(++m);
                    cell1.SetCellValue("");
                    //sheet2按订单金额降序
                    var rAmo = (from q in resApplierList
                                orderby q.OrderPrice descending
                                select q).ToList();
                    for (var i = 1; i <= rAmo.Count; i++)
                    {
                        var item = rAmo[i - 1];
                        IRow row = sheet2.CreateRow(i);
                        ICell cell = null;
                        var j = 0;
                        if (item != null)
                        {
                            cell = row.CreateCell(j);
                            cell.SetCellValue(item.TA);
                            cell = row.CreateCell(++j);
                            cell.SetCellValue(item.ApplierName);
                            cell = row.CreateCell(++j);
                            cell.SetCellValue(item.ApplierMUDID);
                            cell = row.CreateCell(++j);
                            double orderPrice;
                            double.TryParse(item.OrderPrice.ToString(), out orderPrice);
                            cell.SetCellValue(orderPrice);
                            cell = row.CreateCell(++j);
                            cell.SetCellValue(item.OrderCount);
                            cell = row.CreateCell(++j);
                            cell.SetCellValue(item.RDTerritoryCode);
                            cell = row.CreateCell(++j);
                            cell.SetCellValue(item.RDName);
                            cell = row.CreateCell(++j);
                            cell.SetCellValue(item.MUD_ID_RD);
                        }
                    }
                    IRow row2 = sheet2.CreateRow(rCnt.Count + 1);
                    ICell cell2 = null;
                    var n = 0;
                    cell2 = row2.CreateCell(n);
                    cell2.SetCellValue("");
                    cell2 = row2.CreateCell(++n);
                    cell2.SetCellValue("");
                    cell2 = row2.CreateCell(++n);
                    cell2.SetCellValue("");
                    cell2 = row2.CreateCell(++n);
                    cell2.SetCellValue(totalAmount);
                    cell2 = row2.CreateCell(++n);
                    cell2.SetCellValue(totalCnt);
                    cell2 = row2.CreateCell(++n);
                    cell2.SetCellValue("");
                    cell2 = row2.CreateCell(++n);
                    cell2.SetCellValue("");
                    cell2 = row2.CreateCell(++n);
                    cell2.SetCellValue("");
                }
                #endregion

                #region 订单排名--医院
                if (resHospitalList != null && resHospitalList.Count > 0)
                {
                    //sheet1按订单数量降序
                    var rCnt = (from q in resHospitalList
                                orderby q.OrderCount descending
                                select q).ToList();
                    for (var i = 1; i <= rCnt.Count; i++)
                    {
                        var item = rCnt[i - 1];
                        IRow row = sheet3.CreateRow(i);
                        ICell cell = null;
                        var j = 0;
                        if (item != null)
                        {
                            cell = row.CreateCell(j);
                            cell.SetCellValue(item.TA);
                            cell = row.CreateCell(++j);
                            cell.SetCellValue(item.HospitalId);
                            cell = row.CreateCell(++j);
                            cell.SetCellValue(item.HospitalName);
                            cell = row.CreateCell(++j);
                            cell.SetCellValue(item.OrderCount);
                            cell = row.CreateCell(++j);
                            double orderPrice;
                            double.TryParse(item.OrderPrice.ToString(), out orderPrice);
                            cell.SetCellValue(orderPrice);
                        }
                    }
                    var totalCnt = rCnt.Sum(c => c.OrderCount) == 0 ? "" : "合计：" + rCnt.Sum(c => c.OrderCount).ToString("N").Substring(0, rCnt.Sum(c => c.OrderCount).ToString("N").Length - 3);
                    var totalAmount = rCnt.Sum(c => (decimal)c.OrderPrice) == 0 ? "¥0.00" : "合计：" + rCnt.Sum(c => c.OrderPrice).ToString("C").Replace('$', '¥');
                    IRow row1 = sheet3.CreateRow(rCnt.Count + 1);
                    ICell cell1 = null;
                    var m = 0;
                    cell1 = row1.CreateCell(m);
                    cell1.SetCellValue("");
                    cell1 = row1.CreateCell(++m);
                    cell1.SetCellValue("");
                    cell1 = row1.CreateCell(++m);
                    cell1.SetCellValue("");
                    cell1 = row1.CreateCell(++m);
                    cell1.SetCellValue(totalCnt);
                    cell1 = row1.CreateCell(++m);
                    cell1.SetCellValue(totalAmount);
                    //sheet2按订单金额降序
                    var rAmo = (from q in resHospitalList
                                orderby q.OrderPrice descending
                                select q).ToList();
                    for (var i = 1; i <= rAmo.Count; i++)
                    {
                        var item = rAmo[i - 1];
                        IRow row = sheet4.CreateRow(i);
                        ICell cell = null;
                        var j = 0;
                        if (item != null)
                        {
                            cell = row.CreateCell(j);
                            cell.SetCellValue(item.TA);
                            cell = row.CreateCell(++j);
                            cell.SetCellValue(item.HospitalId);
                            cell = row.CreateCell(++j);
                            cell.SetCellValue(item.HospitalName);
                            cell = row.CreateCell(++j);
                            double orderPrice;
                            double.TryParse(item.OrderPrice.ToString(), out orderPrice);
                            cell.SetCellValue(orderPrice);
                            cell = row.CreateCell(++j);
                            cell.SetCellValue(item.OrderCount);
                        }
                    }
                    IRow row2 = sheet4.CreateRow(rCnt.Count + 1);
                    ICell cell2 = null;
                    var n = 0;
                    cell2 = row2.CreateCell(n);
                    cell2.SetCellValue("");
                    cell2 = row2.CreateCell(++n);
                    cell2.SetCellValue("");
                    cell2 = row2.CreateCell(++n);
                    cell2.SetCellValue("");
                    cell2 = row2.CreateCell(++n);
                    cell2.SetCellValue(totalAmount);
                    cell2 = row2.CreateCell(++n);
                    cell2.SetCellValue(totalCnt);
                }

                #endregion

                #region 订单排名--餐厅
                if (resRestaurantList != null && resRestaurantList.Count > 0)
                {
                    //sheet1按订单数量降序
                    var rCnt = (from q in resRestaurantList
                                orderby q.OrderCount descending
                                select q).ToList();
                    for (var i = 1; i <= rCnt.Count; i++)
                    {
                        var item = rCnt[i - 1];
                        IRow row = sheet5.CreateRow(i);
                        ICell cell = null;
                        var j = 0;
                        if (item != null)
                        {
                            cell = row.CreateCell(j);
                            cell.SetCellValue(item.Channel);
                            cell = row.CreateCell(++j);
                            cell.SetCellValue(item.RestaurantId);
                            cell = row.CreateCell(++j);
                            cell.SetCellValue(item.RestaurantName);
                            cell = row.CreateCell(++j);
                            cell.SetCellValue(item.OrderCount);
                            cell = row.CreateCell(++j);
                            double orderPrice;
                            double.TryParse(item.OrderPrice.ToString(), out orderPrice);
                            cell.SetCellValue(orderPrice);
                        }
                    }
                    var totalCnt = rCnt.Sum(c => c.OrderCount) == 0 ? "" : "合计：" + rCnt.Sum(c => c.OrderCount).ToString("N").Substring(0, rCnt.Sum(c => c.OrderCount).ToString("N").Length - 3);
                    var totalAmount = rCnt.Sum(c => (decimal)c.OrderPrice) == 0 ? "¥0.00" : "合计：" + rCnt.Sum(c => c.OrderPrice).ToString("C").Replace('$', '¥');
                    IRow row1 = sheet5.CreateRow(rCnt.Count + 1);
                    ICell cell1 = null;
                    var m = 0;
                    cell1 = row1.CreateCell(m);
                    cell1.SetCellValue("");
                    cell1 = row1.CreateCell(++m);
                    cell1.SetCellValue("");
                    cell1 = row1.CreateCell(++m);
                    cell1.SetCellValue("");
                    cell1 = row1.CreateCell(++m);
                    cell1.SetCellValue(totalCnt);
                    cell1 = row1.CreateCell(++m);
                    cell1.SetCellValue(totalAmount);
                    //sheet2按订单金额降序
                    var rAmo = (from q in resRestaurantList
                                orderby q.OrderPrice descending
                                select q).ToList();
                    for (var i = 1; i <= rAmo.Count; i++)
                    {
                        var item = rAmo[i - 1];
                        IRow row = sheet6.CreateRow(i);
                        ICell cell = null;
                        var j = 0;
                        if (item != null)
                        {
                            cell = row.CreateCell(j);
                            cell.SetCellValue(item.Channel);
                            cell = row.CreateCell(++j);
                            cell.SetCellValue(item.RestaurantId);
                            cell = row.CreateCell(++j);
                            cell.SetCellValue(item.RestaurantName);
                            cell = row.CreateCell(++j);
                            double orderPrice;
                            double.TryParse(item.OrderPrice.ToString(), out orderPrice);
                            cell.SetCellValue(orderPrice);
                            cell = row.CreateCell(++j);
                            cell.SetCellValue(item.OrderCount);
                        }
                    }
                    IRow row2 = sheet6.CreateRow(rCnt.Count + 1);
                    ICell cell2 = null;
                    var n = 0;
                    cell2 = row2.CreateCell(n);
                    cell2.SetCellValue("");
                    cell2 = row2.CreateCell(++n);
                    cell2.SetCellValue("");
                    cell2 = row2.CreateCell(++n);
                    cell2.SetCellValue("");
                    cell2 = row2.CreateCell(++n);
                    cell2.SetCellValue(totalAmount);
                    cell2 = row2.CreateCell(++n);
                    cell2.SetCellValue(totalCnt);
                }
                #endregion

                using (MemoryStream ms = new MemoryStream())
                {
                    wk.Write(ms);
                    Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xls", DateTime.Now.ToString("yyyyMMddHHmmssfff")));
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.BinaryWrite(ms.ToArray());

                }
                //#endregion
            }
            catch (Exception ex)
            {
                LogHelper.Error("" + ex.Message);
            }
        }
        #endregion

        #region 订单排名（图形）
        public ActionResult OrderRanking()
        {
            //加载TA
            var res = analysisService.LoadRankingTA();
            List<string> list = new List<string>();
            foreach (var item in res)
            {
                list.Add(item.Name);
            }
            String join = String.Join(",", list);
            ViewBag.TAlist = join;
            return View();
        }

        public JsonResult LoadOrderRankingChart(string Year, string Month, string HTType, string TA)
        {
            string deliverdate = "";
            string sltTA = "";
            if (Year != "" && Month != "")
            {
                deliverdate = Year + "-" + Month + "-01";
            }
            else
            {
                deliverdate = "";
            }

            if (TA.Contains("ALL"))
            {
                sltTA = null;
            }
            else
            {
                sltTA = "'" + TA.Replace(",", "','") + "'";
            }

            //抓取数据 根据订单数查代表数
            var res = analysisService.LoadOrderRankingChart(deliverdate, HTType, sltTA);
            //抓取数据 根据金额查代表数
            var list = analysisService.LoadAmountRankingChart(deliverdate, HTType, sltTA);
            //抓取数据 查代表姓名
            var order = analysisService.LoadMRName(deliverdate, HTType, sltTA);
            if (order != null && order.Count > 0)
            {
                var ChartRows = new List<P_Order_RANKING_Chart>();
                var ChartAmount = new List<P_Amount_RANKING_Chart>();
                var ChartTotal = new List<P_OrderAmount_RANKING_Chart>();
                double MRTotalcount = 0;
                double MRcount1 = 0;
                double MRcount2 = 0;
                double MRcount3 = 0;
                double MRcount4 = 0;
                double MRcount5 = 0;
                double MRcount6 = 0;

                double Totalcount = 0;
                double count1 = 0;
                double count2 = 0;
                double count3 = 0;
                double count4 = 0;
                double count5 = 0;
                double count6 = 0;

                string maxCount = "";
                string minCount = "";
                string maxAmountCount = "";
                string minAmountCount = "";
                string maxOrder = "";
                string maxAmount = "";
                string minAmount = "";

                #region 获取在人数区间范围和所选占比范围内的代表数据--订单数
                foreach (var item in res)
                {
                    MRTotalcount = MRTotalcount + item.MRCount;
                }
                if (MRTotalcount > 0)
                {
                    var MRCount1 = (from a in res
                                    where a.OrderCount == 1
                                    select new
                                    {
                                        a.OrderCount,
                                        a.MRCount
                                    }).ToList();
                    foreach (var item in MRCount1)
                    {
                        MRcount1 = MRcount1 + item.MRCount;
                    }
                    ChartRows.Add(new P_Order_RANKING_Chart() { MRCount = MRcount1, MRCountRate = Math.Round((MRcount1 / MRTotalcount) * 100, 2) });
                    var MRCount2 = (from a in res
                                    where a.OrderCount == 2
                                    select new
                                    {
                                        a.OrderCount,
                                        a.MRCount
                                    }).ToList();
                    foreach (var item in MRCount2)
                    {
                        MRcount2 = MRcount2 + item.MRCount;
                    }
                    ChartRows.Add(new P_Order_RANKING_Chart() { MRCount = MRcount2, MRCountRate = Math.Round((MRcount2 / MRTotalcount) * 100, 2) });
                    var MRCount3 = (from a in res
                                    where a.OrderCount >= 3 && a.OrderCount <= 4
                                    select new
                                    {
                                        a.OrderCount,
                                        a.MRCount
                                    }).ToList();
                    foreach (var item in MRCount3)
                    {
                        MRcount3 = MRcount3 + item.MRCount;
                    }
                    ChartRows.Add(new P_Order_RANKING_Chart() { MRCount = MRcount3, MRCountRate = Math.Round((MRcount3 / MRTotalcount) * 100, 2) });
                    var MRCount4 = (from a in res
                                    where a.OrderCount >= 5 && a.OrderCount <= 9
                                    select new
                                    {
                                        a.OrderCount,
                                        a.MRCount
                                    }).ToList();
                    foreach (var item in MRCount4)
                    {
                        MRcount4 = MRcount4 + item.MRCount;
                    }

                    ChartRows.Add(new P_Order_RANKING_Chart() { MRCount = MRcount4, MRCountRate = Math.Round((MRcount4 / MRTotalcount) * 100, 2) });

                    var MRCount5 = (from a in res
                                    where a.OrderCount >= 10 && a.OrderCount <= 14
                                    select new
                                    {
                                        a.OrderCount,
                                        a.MRCount
                                    }).ToList();
                    foreach (var item in MRCount5)
                    {
                        MRcount5 = MRcount5 + item.MRCount;
                    }

                    ChartRows.Add(new P_Order_RANKING_Chart() { MRCount = MRcount5, MRCountRate = Math.Round((MRcount5 / MRTotalcount) * 100, 2) });

                    var MRCount6 = (from a in res
                                    where a.OrderCount >= 15
                                    select new
                                    {
                                        a.OrderCount,
                                        a.MRCount
                                    }).ToList();
                    foreach (var item in MRCount6)
                    {
                        MRcount6 = MRcount6 + item.MRCount;
                    }

                    ChartRows.Add(new P_Order_RANKING_Chart() { MRCount = MRcount6, MRCountRate = Math.Round((MRcount6 / MRTotalcount) * 100, 2) });

                    foreach (var item in ChartRows)
                    {
                        ChartTotal.Add(new P_OrderAmount_RANKING_Chart() { MRCount = item.MRCount, MRCountRate = item.MRCountRate });
                    }
                }
                else
                {
                    for (var i = 0; i < 6; i++)
                    {
                        ChartTotal.Add(new P_OrderAmount_RANKING_Chart() { MRCount = 0, MRCountRate = 0.00 });
                    }
                }
                #endregion

                #region 获取在人数区间范围和所选占比范围内的代表数据--金额
                foreach (var item in list)
                {
                    Totalcount = Totalcount + item.MRCount;
                }
                if (Totalcount > 0)
                {
                    var Count1 = (from a in list
                                  where a.OrderPrice <= 999
                                  select new
                                  {
                                      a.OrderPrice,
                                      a.MRCount
                                  }).ToList();
                    foreach (var item in Count1)
                    {
                        count1 = count1 + item.MRCount;
                    }

                    ChartAmount.Add(new P_Amount_RANKING_Chart() { MRAmountCount = count1, MRAmountCountRate = Math.Round((count1 / Totalcount) * 100, 2) });

                    var Count2 = (from a in list
                                  where a.OrderPrice >= 1000 && a.OrderPrice <= 2499
                                  select new
                                  {
                                      a.OrderPrice,
                                      a.MRCount
                                  }).ToList();
                    foreach (var item in Count2)
                    {
                        count2 = count2 + item.MRCount;
                    }

                    ChartAmount.Add(new P_Amount_RANKING_Chart() { MRAmountCount = count2, MRAmountCountRate = Math.Round((count2 / Totalcount) * 100, 2) });

                    var Count3 = (from a in list
                                  where a.OrderPrice >= 2500 && a.OrderPrice <= 4999
                                  select new
                                  {
                                      a.OrderPrice,
                                      a.MRCount
                                  }).ToList();
                    foreach (var item in Count3)
                    {
                        count3 = count3 + item.MRCount;
                    }

                    ChartAmount.Add(new P_Amount_RANKING_Chart() { MRAmountCount = count3, MRAmountCountRate = Math.Round((count3 / Totalcount) * 100, 2) });

                    var Count4 = (from a in list
                                  where a.OrderPrice >= 5000 && a.OrderPrice <= 9999
                                  select new
                                  {
                                      a.OrderPrice,
                                      a.MRCount
                                  }).ToList();
                    foreach (var item in Count4)
                    {
                        count4 = count4 + item.MRCount;
                    }

                    ChartAmount.Add(new P_Amount_RANKING_Chart() { MRAmountCount = count4, MRAmountCountRate = Math.Round((count4 / Totalcount) * 100, 2) });

                    var Count5 = (from a in list
                                  where a.OrderPrice >= 10000 && a.OrderPrice <= 19999
                                  select new
                                  {
                                      a.OrderPrice,
                                      a.MRCount
                                  }).ToList();
                    foreach (var item in Count5)
                    {
                        count5 = count5 + item.MRCount;
                    }

                    ChartAmount.Add(new P_Amount_RANKING_Chart() { MRAmountCount = count5, MRAmountCountRate = Math.Round((count5 / Totalcount) * 100, 2) });

                    var Count6 = (from a in list
                                  where a.OrderPrice >= 20000
                                  select new
                                  {
                                      a.OrderPrice,
                                      a.MRCount
                                  }).ToList();
                    foreach (var item in Count6)
                    {
                        count6 = count6 + item.MRCount;
                    }

                    ChartAmount.Add(new P_Amount_RANKING_Chart() { MRAmountCount = count6, MRAmountCountRate = Math.Round((count6 / Totalcount) * 100, 2) });

                    foreach (var item in ChartAmount)
                    {
                        ChartTotal.Add(new P_OrderAmount_RANKING_Chart() { MRAmountCount = item.MRAmountCount, MRAmountCountRate = item.MRAmountCountRate });
                    }
                }
                else
                {
                    for (var j = 0; j < 6; j++)
                    {
                        ChartTotal.Add(new P_OrderAmount_RANKING_Chart() { MRAmountCount = 0, MRAmountCountRate = 0.00 });
                    }
                }
                #endregion

                #region 订单数取最大值
                var MaxCount = order.Max(p => p.OrderCount);
                var MaxMR = (from a in order
                             where a.OrderCount == MaxCount
                             select new
                             {
                                 a.OrderCount,
                                 a.ApplierMUDID,
                                 a.Name
                             }).ToList();
                maxOrder = MaxMR[0].OrderCount.ToString("N").Substring(0, MaxMR[0].OrderCount.ToString("N").Length - 3);
                if (MaxMR.Count > 1)
                {
                    maxCount = "No. of MR:" + MaxMR.Count.ToString("N").Substring(0, MaxMR.Count.ToString("N").Length - 3) + "人";
                }
                if (MaxMR.Count == 1)
                {
                    maxCount = MaxMR[0].ApplierMUDID + "(" + MaxMR[0].Name + ")";
                }
                #endregion

                #region 订单数取1              
                var MinMR = (from a in order
                             where a.OrderCount == 1
                             select new
                             {
                                 a.OrderCount,
                                 a.ApplierMUDID,
                                 a.Name
                             }).ToList();
                if (MinMR.Count > 1)
                {
                    minCount = "No. of MR:" + MinMR.Count.ToString("N").Substring(0, MinMR.Count.ToString("N").Length - 3) + "人";
                }
                if (MinMR.Count == 1)
                {
                    minCount = MaxMR[0].ApplierMUDID + "(" + MinMR[0].Name + ")";
                }
                #endregion

                #region 金额取最大值
                var MaxAmountCount = order.Max(p => p.OrderPrice);
                var MaxAmountMR = (from a in order
                                   where a.OrderPrice == MaxAmountCount
                                   select new
                                   {
                                       a.OrderPrice,
                                       a.ApplierMUDID,
                                       a.Name
                                   }).ToList();
                maxAmount = MaxAmountMR[0].OrderPrice.ToString("N");
                if (MaxAmountMR.Count > 1)
                {
                    maxAmountCount = "No. of MR:" + MaxAmountMR.Count.ToString("N").Substring(0, MaxAmountMR.Count.ToString("N").Length - 3) + "人";
                }
                if (MaxAmountMR.Count == 1)
                {
                    maxAmountCount = MaxAmountMR[0].ApplierMUDID + "(" + MaxAmountMR[0].Name + ")";
                }
                #endregion

                #region 金额取最小值
                var MinAmountCount = order.Min(p => p.OrderPrice);
                var MinAmountMR = (from a in order
                                   where a.OrderPrice == MinAmountCount
                                   select new
                                   {
                                       a.OrderPrice,
                                       a.ApplierMUDID,
                                       a.Name
                                   }).ToList();
                minAmount = MinAmountMR[0].OrderPrice.ToString("N");
                if (MinAmountMR.Count > 1)
                {
                    minAmountCount = "No. of MR:" + MinAmountMR.Count.ToString("N").Substring(0, MinAmountMR.Count.ToString("N").Length - 3) + "人";
                }
                if (MinAmountMR.Count == 1)
                {
                    minAmountCount = MinAmountMR[0].ApplierMUDID + "(" + MinAmountMR[0].Name + ")";
                }
                #endregion

                //代表平均订单数
                var CountSum = order.Sum(a => a.OrderCount);
                var MRCountSum = res.Sum(a => a.MRCount);
                string AverCount = (CountSum / MRCountSum).ToString("N");
                //代表平均金额
                var AmountSum = double.Parse(order.Sum(a => a.OrderPrice).ToString());
                var MRAmountSum = list.Sum(a => a.MRCount);
                string AverAmount = (AmountSum / MRAmountSum).ToString("N");
                ChartTotal.Add(new P_OrderAmount_RANKING_Chart()
                {
                    MRCountHighest = maxOrder,
                    HighestName = maxCount,
                    MRCountForLow = minCount,
                    MRCountAverage = AverCount,
                    MRAmountHighest = maxAmount,
                    HighestAmountName = maxAmountCount,
                    MRAmountLowest = minAmount,
                    LowestAmountName = minAmountCount,
                    MRAmountAverage = AverAmount
                });

                return Json(new { state = 1, data = ChartTotal });
            }
            else
            {
                return Json(new { state = 0, data = 0 });
            }

        }
        #endregion
    }
}