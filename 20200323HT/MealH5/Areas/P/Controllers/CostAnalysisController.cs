using iPathFeast.API.Client;
using iPathFeast.ApiEntity;
using MealAdmin.Entity;
using MealAdmin.Entity.Enum;
using MealAdmin.Entity.Helper;
using MealAdminApiClient;
using MealH5.Areas.P.Filter;
using MealH5.Areas.P.Handler;
using MealH5.Models;
using MealH5.Util;
using MeetingMealApiClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using XFramework.XInject.Attributes;
using XFramework.XUtil;

namespace MealH5.Areas.P.Controllers
{
    /// <summary>
    /// 订单操作
    /// </summary>
    [WxUserFilter]
    public class CostAnalysisController : BaseController
    {
        [Autowired]
        ApiV1Client apiClient { get; set; }

        #region 预申请分析
        [iPathOAuthFilter(MappingKey = "0x0006", CallBackUrl = true)]
        [HttpGet]
        public ActionResult PreApprovalAnalysis()
        {
            var NowDate = DateTime.Now;
            string StartDate = "";
            string EndDate = "";
            if (NowDate.Day == 1)
            {
                StartDate = DateTime.Now.AddMonths(-1).ToString("yyyy-MM") + "-01";
                EndDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            }
            else
            {
                StartDate = DateTime.Now.ToString("yyyy-MM") + "-01";
                EndDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            }

            ViewBag.StartDate = StartDate;
            ViewBag.EndDate = EndDate;
            return View();
        }

        [iPathOAuthFilter(MappingKey = "0x0016", CallBackUrl = true)]
        [HttpGet]
        public ActionResult CostError()
        {
            var TerritoryCode = CurrentWxUser.TerritoryCode;
            if (TerritoryCode != null && TerritoryCode != "" && TerritoryCode.IndexOf("_RM") > -1)
            {
                CurrentWxUser.Role = "RM";
            }
            else if (TerritoryCode != null && TerritoryCode != "" && TerritoryCode.IndexOf("_SD") > -1)
            {
                CurrentWxUser.Role = "RD";
            }
            else
            {
                var UserId = CurrentWxUser.UserId;
                var channel = BUManagementClientChannelFactory.GetChannel();

                var BUINFO = channel.GetBUInfoByUserId(UserId);

                if (BUINFO != null && BUINFO.ID != null)
                {
                    CurrentWxUser.Role = "BU";
                }
                else
                {
                    var TAINFO = channel.GetTAInfoByUserId(UserId);
                    if (TAINFO != null && TAINFO.ID != null)
                    {
                        CurrentWxUser.Role = "TA";
                    }
                    else
                    {
                        CurrentWxUser.Role = "";
                    }
                }

            }

            return View();
        }

        //var preApprovalClient = PreApprovalClientChannelFactory.GetChannel();
        #region 预申请分析-按数量金额
        public JsonResult LoadPreApprovalData(string StartDate, string EndDate)
        {
            try
            {
                List<P_PreApproval_Count_Amount_View> countChart = new List<P_PreApproval_Count_Amount_View>();
                List<P_PreApproval_Count_Amount_View> suscountChart = new List<P_PreApproval_Count_Amount_View>();
                List<P_PreApproval_Count_Amount_View> refcountChart = new List<P_PreApproval_Count_Amount_View>();
                List<P_PreApproval_Count_Amount_View> pencountChart = new List<P_PreApproval_Count_Amount_View>();
                List<P_PreApproval_Count_Amount_View> cancountChart = new List<P_PreApproval_Count_Amount_View>();

                List<P_PreApproval_TotalCountAmount> alllab = new List<P_PreApproval_TotalCountAmount>();
                List<P_PreApproval_TotalCountAmount> suslab = new List<P_PreApproval_TotalCountAmount>();
                List<P_PreApproval_TotalCountAmount> reflab = new List<P_PreApproval_TotalCountAmount>();
                List<P_PreApproval_TotalCountAmount> penlab = new List<P_PreApproval_TotalCountAmount>();
                List<P_PreApproval_TotalCountAmount> canlab = new List<P_PreApproval_TotalCountAmount>();

                var PreChannel = PreApprovalClientChannelFactory.GetChannel();
                string position = "RM";
                string territoryCode = "RES_RM_N1";

                var res = PreChannel.LoadPreApprovalData(CurrentWxUser.UserId, position, territoryCode, StartDate, EndDate);

                if (res != null && res.Count > 0)
                {
                    #region 饼图
                    //全部
                    decimal NewTotalBudget = res.Sum(c => c.TotalPrice);
                    double ZeroCount = res.Sum(c => c.newZeroCount);
                    double NonZeroCount = res.Sum(c => c.newNonZeroCount);
                    double TotalCount = ZeroCount + NonZeroCount;
                    double ZeroCountPercent = Math.Round((ZeroCount / TotalCount) * 100, 2);
                    double NonZeroPercent = Math.Round((NonZeroCount / TotalCount) * 100, 2);
                    P_PreApproval_TotalCountAmount laball = new P_PreApproval_TotalCountAmount
                    {
                        NonZeroCount = NonZeroCount,
                        ZeroCount = ZeroCount,
                        TotalPrice = NewTotalBudget,
                        TotalCount = TotalCount
                    };
                    alllab.Add(laball);
                    P_PreApproval_Count_Amount_View ZeroChart = new P_PreApproval_Count_Amount_View
                    {
                        Name = "0元HT：\n" + ZeroCountPercent.ToString() + "%\n(" + ZeroCount + "单)",
                        PreCount = ZeroCount
                    };
                    countChart.Add(ZeroChart);
                    P_PreApproval_Count_Amount_View NonZeroChart = new P_PreApproval_Count_Amount_View
                    {
                        Name = "非0元HT：\n" + NonZeroPercent.ToString() + "%\n(" + NonZeroCount + "单)",
                        PreCount = NonZeroCount
                    };
                    countChart.Add(NonZeroChart);


                    //审批通过
                    var ApprovalSuccess = res.Where(c => c.PreState.Equals("审批通过")).ToList();
                    decimal susNewTotalBudget = ApprovalSuccess.Sum(c => c.TotalPrice);
                    double susZeroCount = ApprovalSuccess.Sum(c => c.newZeroCount);
                    double susNonZeroCount = ApprovalSuccess.Sum(c => c.newNonZeroCount);
                    double susTotalCount = susZeroCount + susNonZeroCount;
                    double susZeroCountPercent = Math.Round((susZeroCount / susTotalCount) * 100, 2);
                    double susNonZeroPercent = Math.Round((susNonZeroCount / susTotalCount) * 100, 2);
                    P_PreApproval_TotalCountAmount labsus = new P_PreApproval_TotalCountAmount
                    {
                        NonZeroCount = susNonZeroCount,
                        ZeroCount = susZeroCount,
                        TotalPrice = susNewTotalBudget,
                        TotalCount = susTotalCount
                    };
                    suslab.Add(laball);
                    P_PreApproval_Count_Amount_View susZeroChart = new P_PreApproval_Count_Amount_View
                    {
                        Name = "0元HT：\n" + susZeroCountPercent.ToString() + "%\n(" + susZeroCount + "单)",
                        PreCount = susZeroCount
                    };
                    suscountChart.Add(susZeroChart);
                    P_PreApproval_Count_Amount_View susNonZeroChart = new P_PreApproval_Count_Amount_View
                    {
                        Name = "非0元HT：\n" + susNonZeroPercent.ToString() + "%\n(" + susNonZeroCount + "单)",
                        PreCount = susNonZeroCount
                    };
                    suscountChart.Add(susNonZeroChart);

                    //审批驳回
                    var ApprovalRefuse = res.Where(c => c.PreState.Equals("审批驳回")).ToList();
                    decimal refNewTotalBudget = ApprovalRefuse.Sum(c => c.TotalPrice);
                    double refZeroCount = ApprovalRefuse.Sum(c => c.newZeroCount);
                    double refNonZeroCount = ApprovalRefuse.Sum(c => c.newNonZeroCount);
                    double refTotalCount = refZeroCount + refNonZeroCount;
                    double refZeroCountPercent = Math.Round((refZeroCount / refTotalCount) * 100, 2);
                    double refNonZeroPercent = Math.Round((refNonZeroCount / refTotalCount) * 100, 2);
                    P_PreApproval_TotalCountAmount labref = new P_PreApproval_TotalCountAmount
                    {
                        NonZeroCount = refNonZeroCount,
                        ZeroCount = refZeroCount,
                        TotalPrice = refNewTotalBudget,
                        TotalCount = refTotalCount
                    };
                    reflab.Add(labref);
                    P_PreApproval_Count_Amount_View refZeroChart = new P_PreApproval_Count_Amount_View
                    {
                        Name = "0元HT：\n" + refZeroCountPercent.ToString() + "%\n(" + refZeroCount + "单)",
                        PreCount = refZeroCount
                    };
                    refcountChart.Add(refZeroChart);
                    P_PreApproval_Count_Amount_View refNonZeroChart = new P_PreApproval_Count_Amount_View
                    {
                        Name = "非0元HT：\n" + refNonZeroPercent.ToString() + "%\n(" + refNonZeroCount + "单)",
                        PreCount = refNonZeroCount
                    };
                    refcountChart.Add(refNonZeroChart);

                    //待审批
                    var Pending = res.Where(c => c.PreState.Equals("待审批")).ToList();
                    decimal penNewTotalBudget = Pending.Sum(c => c.TotalPrice);
                    double penZeroCount = Pending.Sum(c => c.newZeroCount);
                    double penNonZeroCount = Pending.Sum(c => c.newNonZeroCount);
                    double penTotalCount = penZeroCount + penNonZeroCount;
                    double penZeroCountPercent = Math.Round((penZeroCount / penTotalCount) * 100, 2);
                    double penNonZeroPercent = Math.Round((penNonZeroCount / penTotalCount) * 100, 2);
                    P_PreApproval_TotalCountAmount labpen = new P_PreApproval_TotalCountAmount
                    {
                        NonZeroCount = penNonZeroCount,
                        ZeroCount = penZeroCount,
                        TotalPrice = penNewTotalBudget,
                        TotalCount = penTotalCount
                    };
                    penlab.Add(labpen);
                    P_PreApproval_Count_Amount_View penZeroChart = new P_PreApproval_Count_Amount_View
                    {
                        Name = "0元HT：\n" + penZeroCountPercent.ToString() + "%\n(" + penZeroCount + "单)",
                        PreCount = penZeroCount
                    };
                    pencountChart.Add(penZeroChart);
                    P_PreApproval_Count_Amount_View penNonZeroChart = new P_PreApproval_Count_Amount_View
                    {
                        Name = "非0元HT：\n" + penNonZeroPercent.ToString() + "%\n(" + penNonZeroCount + "单)",
                        PreCount = penNonZeroCount
                    };
                    pencountChart.Add(penNonZeroChart);

                    //已取消
                    var Cancel = res.Where(c => c.PreState.Equals("已取消")).ToList();
                    decimal canNewTotalBudget = Cancel.Sum(c => c.TotalPrice);
                    double canZeroCount = Cancel.Sum(c => c.newZeroCount);
                    double canNonZeroCount = Cancel.Sum(c => c.newNonZeroCount);
                    double canTotalCount = canZeroCount + canNonZeroCount;
                    double canZeroCountPercent = Math.Round((canZeroCount / canTotalCount) * 100, 2);
                    double canNonZeroPercent = Math.Round((canNonZeroCount / canTotalCount) * 100, 2);
                    P_PreApproval_TotalCountAmount labcan = new P_PreApproval_TotalCountAmount
                    {
                        NonZeroCount = canNonZeroCount,
                        ZeroCount = canZeroCount,
                        TotalPrice = canNewTotalBudget,
                        TotalCount = canTotalCount
                    };
                    canlab.Add(labcan);
                    P_PreApproval_Count_Amount_View canZeroChart = new P_PreApproval_Count_Amount_View
                    {
                        Name = "0元HT：\n" + canZeroCountPercent.ToString() + "%\n(" + canZeroCount + "单)",
                    };
                    cancountChart.Add(canZeroChart);
                    P_PreApproval_Count_Amount_View canNonZeroChart = new P_PreApproval_Count_Amount_View
                    {
                        Name = "非0元HT：\n" + canNonZeroPercent.ToString() + "%\n(" + canNonZeroCount + "单)",
                        TotalBudget = canNewTotalBudget
                    };
                    cancountChart.Add(canNonZeroChart);
                    #endregion

                    //折叠面板数据
                    var DataList = (from a in res
                                    group a by new { a.DMTerritoryCode, a.DMName, a.PreState } into b
                                    select new
                                    {
                                        b.Key.DMTerritoryCode,
                                        b.Key.DMName,
                                        b.Key.PreState,
                                        TotalPrice = b.Sum(c => c.TotalPrice),
                                        TotalCount = b.Sum(c => c.TotalCount),
                                        ZeroCount = b.Sum(c => c.newZeroCount),
                                        NonZeroCount = b.Sum(c => c.newNonZeroCount)
                                    }).ToList();
                    var ApprovalSuccesslist = DataList.Where(c => c.PreState.Equals("审批通过")).ToList();
                    var ApprovalRefuselist = DataList.Where(c => c.PreState.Equals("审批驳回")).ToList();
                    var Pendinglist = DataList.Where(c => c.PreState.Equals("待审批")).ToList();
                    var Cancellist = DataList.Where(c => c.PreState.Equals("已取消")).ToList();

                    List<P_PreApproval_TAB_VIEW> AllDatalist = new List<P_PreApproval_TAB_VIEW>();
                    List<P_PreApproval_TAB_VIEW> SuccessDatalist = new List<P_PreApproval_TAB_VIEW>();
                    List<P_PreApproval_TAB_VIEW> RefuseDatalist = new List<P_PreApproval_TAB_VIEW>();
                    List<P_PreApproval_TAB_VIEW> PendingDatalist = new List<P_PreApproval_TAB_VIEW>();
                    List<P_PreApproval_TAB_VIEW> CancelDatalist = new List<P_PreApproval_TAB_VIEW>();
                    #region 全部
                    foreach (var item in DataList)
                    {
                        List<P_PreApproval_DOWN_VIEW> downList = new List<P_PreApproval_DOWN_VIEW>();
                        if (position.ToUpper() == "BU" || position.ToUpper() == "RM")
                        {
                            //折叠面板内包含自己的下单数据
                            var dmDataList = res.Where(c => c.MRTerritoryCode == item.DMTerritoryCode)
                                .GroupBy(c => new { c.DMName, c.DMTerritoryCode })
                                .Select(b => new
                                {
                                    b.Key.DMName,
                                    b.Key.DMTerritoryCode,
                                    TotalPrice = b.Sum(c => c.TotalPrice),
                                    NonZeroCount = b.Sum(c => c.newNonZeroCount),
                                    ZeroCount = b.Sum(c => c.newZeroCount),
                                    NonZeroProportion = Math.Round((NonZeroCount / (NonZeroCount + ZeroCount)) * 100, 2),
                                    ZeroProportion = Math.Round((ZeroCount / (NonZeroCount + ZeroCount)) * 100, 2)
                                }).ToList();
                            foreach (var dmData in dmDataList)
                            {
                                P_PreApproval_DOWN_VIEW downView = new P_PreApproval_DOWN_VIEW
                                {
                                    Name = dmData.DMName,
                                    TerritoryCode = dmData.DMTerritoryCode,
                                    NonZeroCount = dmData.NonZeroCount,
                                    ZeroCount = dmData.ZeroCount,
                                    NonZeroProportion = dmData.NonZeroProportion,
                                    ZeroProportion = dmData.ZeroProportion,
                                    TotalPrice = dmData.TotalPrice
                                };
                                downList.Add(downView);
                            }
                            var mrDataList = res.Where(c => c.DMTerritoryCode == item.DMTerritoryCode && c.MRTerritoryCode != item.DMTerritoryCode)
                                .GroupBy(c => new { c.MRName, c.MRTerritoryCode })
                                .Select(b => new
                                {
                                    b.Key.MRName,
                                    b.Key.MRTerritoryCode,
                                    TotalPrice = b.Sum(c => c.TotalPrice),
                                    NonZeroCount = b.Sum(c => c.newNonZeroCount),
                                    ZeroCount = b.Sum(c => c.newZeroCount),
                                    NonZeroProportion = Math.Round((NonZeroCount / (NonZeroCount + ZeroCount)) * 100, 2),
                                    ZeroProportion = Math.Round((ZeroCount / (NonZeroCount + ZeroCount)) * 100, 2)
                                }).ToList();
                            foreach (var mrData in mrDataList)
                            {
                                P_PreApproval_DOWN_VIEW downView = new P_PreApproval_DOWN_VIEW
                                {
                                    Name = mrData.MRName,
                                    TerritoryCode = mrData.MRTerritoryCode,
                                    NonZeroCount = mrData.NonZeroCount,
                                    ZeroCount = mrData.ZeroCount,
                                    NonZeroProportion = mrData.NonZeroProportion,
                                    ZeroProportion = mrData.ZeroProportion,
                                    TotalPrice = mrData.TotalPrice
                                };
                                downList.Add(downView);
                            }
                        }

                        P_PreApproval_TAB_VIEW DataView = new P_PreApproval_TAB_VIEW
                        {
                            Name = item.DMName,
                            TerritoryCode = item.DMTerritoryCode,
                            NonZeroCount = item.NonZeroCount,
                            ZeroCount = item.ZeroCount,
                            NonZeroProportion = Math.Round((item.NonZeroCount / (item.NonZeroCount + item.ZeroCount)) * 100, 2),
                            ZeroProportion = Math.Round((item.ZeroCount / (item.NonZeroCount + item.ZeroCount)) * 100, 2),
                            TotalPrice = item.TotalPrice,
                            DownList = downList
                        };
                        AllDatalist.Add(DataView);
                    }
                    #endregion

                    #region 审批通过
                    foreach (var item in ApprovalSuccesslist)
                    {
                        List<P_PreApproval_DOWN_VIEW> downList = new List<P_PreApproval_DOWN_VIEW>();
                        if (position.ToUpper() == "BU" || position.ToUpper() == "RM")
                        {
                            //折叠面板内包含自己的下单数据
                            var dmDataList = res.Where(c => c.PreState.Equals("审批通过") && c.MRTerritoryCode == item.DMTerritoryCode)
                            .GroupBy(c => new { c.DMName, c.DMTerritoryCode, c.PreState })
                            .Select(b => new
                            {
                                b.Key.DMName,
                                b.Key.DMTerritoryCode,
                                TotalPrice = b.Sum(c => c.TotalPrice),
                                NonZeroCount = b.Sum(c => c.newNonZeroCount),
                                ZeroCount = b.Sum(c => c.newZeroCount),
                                NonZeroProportion = Math.Round((NonZeroCount / (NonZeroCount + ZeroCount)) * 100, 2),
                                ZeroProportion = Math.Round((ZeroCount / (NonZeroCount + ZeroCount)) * 100, 2)
                            }).ToList();
                            foreach (var dmData in dmDataList)
                            {
                                P_PreApproval_DOWN_VIEW downView = new P_PreApproval_DOWN_VIEW
                                {
                                    Name = dmData.DMName,
                                    TerritoryCode = dmData.DMTerritoryCode,
                                    NonZeroCount = dmData.NonZeroCount,
                                    ZeroCount = dmData.ZeroCount,
                                    NonZeroProportion = dmData.NonZeroProportion,
                                    ZeroProportion = dmData.ZeroProportion,
                                    TotalPrice = dmData.TotalPrice
                                };
                                downList.Add(downView);
                            }
                            var mrDataList = res.Where(c => c.PreState.Equals("审批通过") && c.DMTerritoryCode == item.DMTerritoryCode && c.MRTerritoryCode != item.DMTerritoryCode)
                                .GroupBy(c => new { c.MRName, c.MRTerritoryCode, c.PreState })
                                .Select(b => new
                                {
                                    b.Key.MRName,
                                    b.Key.MRTerritoryCode,
                                    TotalPrice = b.Sum(c => c.TotalPrice),
                                    NonZeroCount = b.Sum(c => c.newNonZeroCount),
                                    ZeroCount = b.Sum(c => c.newZeroCount),
                                    NonZeroProportion = Math.Round((NonZeroCount / (NonZeroCount + ZeroCount)) * 100, 2),
                                    ZeroProportion = Math.Round((ZeroCount / (NonZeroCount + ZeroCount)) * 100, 2)
                                }).ToList();
                            foreach (var mrData in mrDataList)
                            {
                                P_PreApproval_DOWN_VIEW downView = new P_PreApproval_DOWN_VIEW
                                {
                                    Name = mrData.MRName,
                                    TerritoryCode = mrData.MRTerritoryCode,
                                    NonZeroCount = mrData.NonZeroCount,
                                    ZeroCount = mrData.ZeroCount,
                                    NonZeroProportion = mrData.NonZeroProportion,
                                    ZeroProportion = mrData.ZeroProportion,
                                    TotalPrice = mrData.TotalPrice
                                };
                                downList.Add(downView);
                            }
                        }
                        P_PreApproval_TAB_VIEW DataView = new P_PreApproval_TAB_VIEW
                        {
                            Name = item.DMName,
                            TerritoryCode = item.DMTerritoryCode,
                            NonZeroCount = item.NonZeroCount,
                            ZeroCount = item.ZeroCount,
                            NonZeroProportion = Math.Round((item.NonZeroCount / (item.NonZeroCount + item.ZeroCount)) * 100, 2),
                            ZeroProportion = Math.Round((item.ZeroCount / (item.NonZeroCount + item.ZeroCount)) * 100, 2),
                            TotalPrice = item.TotalPrice,
                            DownList = downList
                        };
                        SuccessDatalist.Add(DataView);
                    }
                    #endregion

                    #region 审批驳回
                    foreach (var item in ApprovalRefuselist)
                    {
                        List<P_PreApproval_DOWN_VIEW> downList = new List<P_PreApproval_DOWN_VIEW>();
                        if (position.ToUpper() == "BU" || position.ToUpper() == "RM")
                        {
                            //折叠面板内包含自己的下单数据
                            var dmDataList = res.Where(c => c.PreState.Equals("审批驳回") && c.MRTerritoryCode == item.DMTerritoryCode)
                            .GroupBy(c => new { c.DMName, c.DMTerritoryCode, c.PreState })
                            .Select(b => new
                            {
                                b.Key.DMName,
                                b.Key.DMTerritoryCode,
                                TotalPrice = b.Sum(c => c.TotalPrice),
                                NonZeroCount = b.Sum(c => c.newNonZeroCount),
                                ZeroCount = b.Sum(c => c.newZeroCount),
                                NonZeroProportion = Math.Round((NonZeroCount / (NonZeroCount + ZeroCount)) * 100, 2),
                                ZeroProportion = Math.Round((ZeroCount / (NonZeroCount + ZeroCount)) * 100, 2)
                            }).ToList();
                            foreach (var dmData in dmDataList)
                            {
                                P_PreApproval_DOWN_VIEW downView = new P_PreApproval_DOWN_VIEW
                                {
                                    Name = dmData.DMName,
                                    TerritoryCode = dmData.DMTerritoryCode,
                                    NonZeroCount = dmData.NonZeroCount,
                                    ZeroCount = dmData.ZeroCount,
                                    NonZeroProportion = dmData.NonZeroProportion,
                                    ZeroProportion = dmData.ZeroProportion,
                                    TotalPrice = dmData.TotalPrice
                                };
                                downList.Add(downView);
                            }
                            var mrDataList = res.Where(c => c.PreState.Equals("审批驳回") && c.DMTerritoryCode == item.DMTerritoryCode && c.MRTerritoryCode != item.DMTerritoryCode)
                                .GroupBy(c => new { c.MRName, c.MRTerritoryCode, c.PreState })
                                .Select(b => new
                                {
                                    b.Key.MRName,
                                    b.Key.MRTerritoryCode,
                                    TotalPrice = b.Sum(c => c.TotalPrice),
                                    NonZeroCount = b.Sum(c => c.newNonZeroCount),
                                    ZeroCount = b.Sum(c => c.newZeroCount),
                                    NonZeroProportion = Math.Round((NonZeroCount / (NonZeroCount + ZeroCount)) * 100, 2),
                                    ZeroProportion = Math.Round((ZeroCount / (NonZeroCount + ZeroCount)) * 100, 2)
                                }).ToList();
                            foreach (var mrData in mrDataList)
                            {
                                P_PreApproval_DOWN_VIEW downView = new P_PreApproval_DOWN_VIEW
                                {
                                    Name = mrData.MRName,
                                    TerritoryCode = mrData.MRTerritoryCode,
                                    NonZeroCount = mrData.NonZeroCount,
                                    ZeroCount = mrData.ZeroCount,
                                    NonZeroProportion = mrData.NonZeroProportion,
                                    ZeroProportion = mrData.ZeroProportion,
                                    TotalPrice = mrData.TotalPrice
                                };
                                downList.Add(downView);
                            }
                        }
                        P_PreApproval_TAB_VIEW DataView = new P_PreApproval_TAB_VIEW
                        {
                            Name = item.DMName,
                            TerritoryCode = item.DMTerritoryCode,
                            NonZeroCount = item.NonZeroCount,
                            ZeroCount = item.ZeroCount,
                            NonZeroProportion = Math.Round((item.NonZeroCount / (item.NonZeroCount + item.ZeroCount)) * 100, 2),
                            ZeroProportion = Math.Round((item.ZeroCount / (item.NonZeroCount + item.ZeroCount)) * 100, 2),
                            TotalPrice = item.TotalPrice,
                            DownList = downList
                        };
                        RefuseDatalist.Add(DataView);
                    }
                    #endregion

                    #region 待审批
                    foreach (var item in Pendinglist)
                    {
                        List<P_PreApproval_DOWN_VIEW> downList = new List<P_PreApproval_DOWN_VIEW>();
                        if (position.ToUpper() == "BU" || position.ToUpper() == "RM")
                        {
                            //折叠面板内包含自己的下单数据
                            var dmDataList = res.Where(c => c.PreState.Equals("待审批") && c.MRTerritoryCode == item.DMTerritoryCode)
                            .GroupBy(c => new { c.DMName, c.DMTerritoryCode, c.PreState })
                            .Select(b => new
                            {
                                b.Key.DMName,
                                b.Key.DMTerritoryCode,
                                TotalPrice = b.Sum(c => c.TotalPrice),
                                NonZeroCount = b.Sum(c => c.newNonZeroCount),
                                ZeroCount = b.Sum(c => c.newZeroCount),
                                NonZeroProportion = Math.Round((NonZeroCount / (NonZeroCount + ZeroCount)) * 100, 2),
                                ZeroProportion = Math.Round((ZeroCount / (NonZeroCount + ZeroCount)) * 100, 2)
                            }).ToList();
                            foreach (var dmData in dmDataList)
                            {
                                P_PreApproval_DOWN_VIEW downView = new P_PreApproval_DOWN_VIEW
                                {
                                    Name = dmData.DMName,
                                    TerritoryCode = dmData.DMTerritoryCode,
                                    NonZeroCount = dmData.NonZeroCount,
                                    ZeroCount = dmData.ZeroCount,
                                    NonZeroProportion = dmData.NonZeroProportion,
                                    ZeroProportion = dmData.ZeroProportion,
                                    TotalPrice = dmData.TotalPrice
                                };
                                downList.Add(downView);
                            }
                            var mrDataList = res.Where(c => c.PreState.Equals("待审批") && c.DMTerritoryCode == item.DMTerritoryCode && c.MRTerritoryCode != item.DMTerritoryCode)
                                .GroupBy(c => new { c.MRName, c.MRTerritoryCode, c.PreState })
                                .Select(b => new
                                {
                                    b.Key.MRName,
                                    b.Key.MRTerritoryCode,
                                    TotalPrice = b.Sum(c => c.TotalPrice),
                                    NonZeroCount = b.Sum(c => c.newNonZeroCount),
                                    ZeroCount = b.Sum(c => c.newZeroCount),
                                    NonZeroProportion = Math.Round((NonZeroCount / (NonZeroCount + ZeroCount)) * 100, 2),
                                    ZeroProportion = Math.Round((ZeroCount / (NonZeroCount + ZeroCount)) * 100, 2)
                                }).ToList();
                            foreach (var mrData in mrDataList)
                            {
                                P_PreApproval_DOWN_VIEW downView = new P_PreApproval_DOWN_VIEW
                                {
                                    Name = mrData.MRName,
                                    TerritoryCode = mrData.MRTerritoryCode,
                                    NonZeroCount = mrData.NonZeroCount,
                                    ZeroCount = mrData.ZeroCount,
                                    NonZeroProportion = mrData.NonZeroProportion,
                                    ZeroProportion = mrData.ZeroProportion,
                                    TotalPrice = mrData.TotalPrice
                                };
                                downList.Add(downView);
                            }
                        }
                        P_PreApproval_TAB_VIEW DataView = new P_PreApproval_TAB_VIEW
                        {
                            Name = item.DMName,
                            TerritoryCode = item.DMTerritoryCode,
                            NonZeroCount = item.NonZeroCount,
                            ZeroCount = item.ZeroCount,
                            NonZeroProportion = Math.Round((item.NonZeroCount / (item.NonZeroCount + item.ZeroCount)) * 100, 2),
                            ZeroProportion = Math.Round((item.ZeroCount / (item.NonZeroCount + item.ZeroCount)) * 100, 2),
                            TotalPrice = item.TotalPrice,
                            DownList = downList
                        };
                        PendingDatalist.Add(DataView);
                    }
                    #endregion

                    #region 已取消
                    foreach (var item in Cancellist)
                    {
                        List<P_PreApproval_DOWN_VIEW> downList = new List<P_PreApproval_DOWN_VIEW>();
                        if (position.ToUpper() == "BU" || position.ToUpper() == "RM")
                        {
                            //折叠面板内包含自己的下单数据
                            var dmDataList = res.Where(c => c.PreState.Equals("已取消") && c.MRTerritoryCode == item.DMTerritoryCode)
                            .GroupBy(c => new { c.DMName, c.DMTerritoryCode, c.PreState })
                            .Select(b => new
                            {
                                b.Key.DMName,
                                b.Key.DMTerritoryCode,
                                TotalPrice = b.Sum(c => c.TotalPrice),
                                NonZeroCount = b.Sum(c => c.newNonZeroCount),
                                ZeroCount = b.Sum(c => c.newZeroCount),
                                NonZeroProportion = Math.Round((NonZeroCount / (NonZeroCount + ZeroCount)) * 100, 2),
                                ZeroProportion = Math.Round((ZeroCount / (NonZeroCount + ZeroCount)) * 100, 2)
                            }).ToList();
                            foreach (var dmData in dmDataList)
                            {
                                P_PreApproval_DOWN_VIEW downView = new P_PreApproval_DOWN_VIEW
                                {
                                    Name = dmData.DMName,
                                    TerritoryCode = dmData.DMTerritoryCode,
                                    NonZeroCount = dmData.NonZeroCount,
                                    ZeroCount = dmData.ZeroCount,
                                    NonZeroProportion = dmData.NonZeroProportion,
                                    ZeroProportion = dmData.ZeroProportion,
                                    TotalPrice = dmData.TotalPrice
                                };
                                downList.Add(downView);
                            }
                            var mrDataList = res.Where(c => c.PreState.Equals("已取消") && c.DMTerritoryCode == item.DMTerritoryCode && c.MRTerritoryCode != item.DMTerritoryCode)
                                .GroupBy(c => new { c.MRName, c.MRTerritoryCode, c.PreState })
                                .Select(b => new
                                {
                                    b.Key.MRName,
                                    b.Key.MRTerritoryCode,
                                    TotalPrice = b.Sum(c => c.TotalPrice),
                                    NonZeroCount = b.Sum(c => c.newNonZeroCount),
                                    ZeroCount = b.Sum(c => c.newZeroCount),
                                    NonZeroProportion = Math.Round((NonZeroCount / (NonZeroCount + ZeroCount)) * 100, 2),
                                    ZeroProportion = Math.Round((ZeroCount / (NonZeroCount + ZeroCount)) * 100, 2)
                                }).ToList();
                            foreach (var mrData in mrDataList)
                            {
                                P_PreApproval_DOWN_VIEW downView = new P_PreApproval_DOWN_VIEW
                                {
                                    Name = mrData.MRName,
                                    TerritoryCode = mrData.MRTerritoryCode,
                                    NonZeroCount = mrData.NonZeroCount,
                                    ZeroCount = mrData.ZeroCount,
                                    NonZeroProportion = mrData.NonZeroProportion,
                                    ZeroProportion = mrData.ZeroProportion,
                                    TotalPrice = mrData.TotalPrice
                                };
                                downList.Add(downView);
                            }
                        }
                        P_PreApproval_TAB_VIEW DataView = new P_PreApproval_TAB_VIEW
                        {
                            Name = item.DMName,
                            TerritoryCode = item.DMTerritoryCode,
                            NonZeroCount = item.NonZeroCount,
                            ZeroCount = item.ZeroCount,
                            NonZeroProportion = Math.Round((item.NonZeroCount / (item.NonZeroCount + item.ZeroCount)) * 100, 2),
                            ZeroProportion = Math.Round((item.ZeroCount / (item.NonZeroCount + item.ZeroCount)) * 100, 2),
                            TotalPrice = item.TotalPrice,
                            DownList = downList
                        };
                        CancelDatalist.Add(DataView);
                    }
                    #endregion

                    return Json(new
                    {
                        state = 1,
                        allLab = alllab,
                        dataCountChart = countChart,
                        LabSus = suslab,
                        dataCountChartSus = suscountChart,
                        LabRef = reflab,
                        dataCountChartRef = refcountChart,
                        LabPen = penlab,
                        dataCountChartPen = pencountChart,
                        LabCan = canlab,
                        dataCountChartCan = cancountChart,
                        AllList = AllDatalist,
                        SusList = SuccessDatalist,
                        RefList = RefuseDatalist,
                        PenList = PendingDatalist,
                        CanList = CancelDatalist
                    });
                }
                else
                {
                    return Json(new { state = 0, txt = "无符合筛选条件的预申请分析数据！" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("LoadPreApprovalData" + ex.Message);
                return Json(new { state = 0 });
            }

        }
        #endregion

        #region 预申请分析-按审批状态
        public JsonResult LoadPreApprovalDataByState(string StartDate, string EndDate)
        {
            try
            {
                List<P_PreApproval_Count_Amount_View> countChart = new List<P_PreApproval_Count_Amount_View>();
                List<P_PreApproval_Count_Amount_View> zerocountChart = new List<P_PreApproval_Count_Amount_View>();
                List<P_PreApproval_Count_Amount_View> nonzeroChart = new List<P_PreApproval_Count_Amount_View>();

                List<P_PreApproval_PreStateCount> alllab = new List<P_PreApproval_PreStateCount>();
                List<P_PreApproval_PreStateCount> zerolab = new List<P_PreApproval_PreStateCount>();
                List<P_PreApproval_PreStateCount> nonzerolab = new List<P_PreApproval_PreStateCount>();


                var PreChannel = PreApprovalClientChannelFactory.GetChannel();
                string position = "RM";
                string territoryCode = "RES_RM_N1";

                var res = PreChannel.LoadPreApprovalData(CurrentWxUser.UserId, position, territoryCode, StartDate, EndDate);

                if (res != null && res.Count > 0)
                {
                    #region 饼图
                    //全部
                    var SuccessList = res.Where(c => c.PreState.Equals("审批通过")).ToList();
                    var RefuseList = res.Where(c => c.PreState.Equals("审批驳回")).ToList();
                    var PendingList = res.Where(c => c.PreState.Equals("待审批")).ToList();
                    var CancelList = res.Where(c => c.PreState.Equals("已取消")).ToList();
                    double sustotal = SuccessList.Sum(c => c.TotalCount);
                    double reftotal = RefuseList.Sum(c => c.TotalCount);
                    double pentotal = PendingList.Sum(c => c.TotalCount);
                    double cantotal = CancelList.Sum(c => c.TotalCount);
                    double totalcount = res.Sum(c => c.TotalCount);
                    double susPercent = Math.Round((sustotal / totalcount) * 100, 2);
                    double refPercent = Math.Round((reftotal / totalcount) * 100, 2);
                    double penPercent = Math.Round((pentotal / totalcount) * 100, 2);
                    double canPercent = Math.Round((cantotal / totalcount) * 100, 2);

                    P_PreApproval_PreStateCount laball = new P_PreApproval_PreStateCount
                    {
                        SuccessCount = sustotal,
                        RefuseCount = reftotal,
                        PendingCount = pentotal,
                        CancelCount = cantotal,
                        TotalCount = totalcount
                    };
                    alllab.Add(laball);

                    P_PreApproval_Count_Amount_View susChart = new P_PreApproval_Count_Amount_View
                    {
                        Name = "审批通过：\n" + susPercent.ToString() + "%\n(" + sustotal + "单)",
                        PreCount = sustotal
                    };
                    countChart.Add(susChart);
                    P_PreApproval_Count_Amount_View refChart = new P_PreApproval_Count_Amount_View
                    {
                        Name = "审批驳回：\n" + refPercent.ToString() + "%\n(" + reftotal + "单)",
                        PreCount = reftotal
                    };
                    countChart.Add(refChart);
                    P_PreApproval_Count_Amount_View penChart = new P_PreApproval_Count_Amount_View
                    {
                        Name = "待审批：\n" + penPercent.ToString() + "%\n(" + pentotal + "单)",
                        PreCount = pentotal
                    };
                    countChart.Add(penChart);
                    P_PreApproval_Count_Amount_View canChart = new P_PreApproval_Count_Amount_View
                    {
                        Name = "已取消：\n" + canPercent.ToString() + "%\n(" + cantotal + "单)",
                        PreCount = cantotal
                    };
                    countChart.Add(canChart);
                    //0元
                    var zerores = res.Where(c => c.newZeroCount > 0).ToList();
                    var zeroSusList = zerores.Where(c => c.PreState.Equals("审批通过")).ToList();
                    var zeroRefList = zerores.Where(c => c.PreState.Equals("审批驳回")).ToList();
                    var zeroPenList = zerores.Where(c => c.PreState.Equals("待审批")).ToList();
                    var zeroCanList = zerores.Where(c => c.PreState.Equals("已取消")).ToList();
                    double zerosustotal = zeroSusList.Sum(c => c.newZeroCount);
                    double zeroreftotal = zeroRefList.Sum(c => c.newZeroCount);
                    double zeropentotal = zeroPenList.Sum(c => c.newZeroCount);
                    double zerocantotal = zeroCanList.Sum(c => c.newZeroCount);
                    double zerototalcount = zerores.Sum(c => c.newZeroCount);
                    double zerosusPercent = Math.Round((zerosustotal / zerototalcount) * 100, 2);
                    double zerorefPercent = Math.Round((zeroreftotal / zerototalcount) * 100, 2);
                    double zeropenPercent = Math.Round((zeropentotal / zerototalcount) * 100, 2);
                    double zerocanPercent = Math.Round((zerocantotal / zerototalcount) * 100, 2);

                    P_PreApproval_PreStateCount labzero = new P_PreApproval_PreStateCount
                    {
                        SuccessCount = zerosustotal,
                        RefuseCount = zeroreftotal,
                        PendingCount = zeropentotal,
                        CancelCount = zerocantotal,
                        TotalCount = zerototalcount
                    };
                    zerolab.Add(labzero);

                    P_PreApproval_Count_Amount_View zerosusChart = new P_PreApproval_Count_Amount_View
                    {
                        Name = "审批通过：\n" + zerosusPercent.ToString() + "%\n(" + zerosustotal + "单)",
                        PreCount = zerosustotal
                    };
                    zerocountChart.Add(zerosusChart);
                    P_PreApproval_Count_Amount_View zerorefChart = new P_PreApproval_Count_Amount_View
                    {
                        Name = "审批驳回：\n" + zerorefPercent.ToString() + "%\n(" + zeroreftotal + "单)",
                        PreCount = zeroreftotal
                    };
                    zerocountChart.Add(zerorefChart);
                    P_PreApproval_Count_Amount_View zeropenChart = new P_PreApproval_Count_Amount_View
                    {
                        Name = "待审批：\n" + zeropenPercent.ToString() + "%\n(" + zeropentotal + "单)",
                        PreCount = zeropentotal
                    };
                    zerocountChart.Add(zeropenChart);
                    P_PreApproval_Count_Amount_View zerocanChart = new P_PreApproval_Count_Amount_View
                    {
                        Name = "已取消：\n" + zerocanPercent.ToString() + "%\n(" + zerocantotal + "单)",
                        PreCount = zerocantotal
                    };
                    zerocountChart.Add(zerocanChart);
                    //非0元
                    var nonzerores = res.Where(c => c.newNonZeroCount > 0).ToList();
                    var nonzeroSusList = nonzerores.Where(c => c.PreState.Equals("审批通过")).ToList();
                    var nonzeroRefList = nonzerores.Where(c => c.PreState.Equals("审批驳回")).ToList();
                    var nonzeroPenList = nonzerores.Where(c => c.PreState.Equals("待审批")).ToList();
                    var nonzeroCanList = nonzerores.Where(c => c.PreState.Equals("已取消")).ToList();
                    double nonzerosustotal = nonzeroSusList.Sum(c => c.newNonZeroCount);
                    double nonzeroreftotal = nonzeroRefList.Sum(c => c.newNonZeroCount);
                    double nonzeropentotal = nonzeroPenList.Sum(c => c.newNonZeroCount);
                    double nonzerocantotal = nonzeroCanList.Sum(c => c.newNonZeroCount);
                    double nonzerototalcount = nonzerores.Sum(c => c.newNonZeroCount);
                    double nonzerosusPercent = Math.Round((nonzerosustotal / nonzerototalcount) * 100, 2);
                    double nonzerorefPercent = Math.Round((nonzeroreftotal / nonzerototalcount) * 100, 2);
                    double nonzeropenPercent = Math.Round((nonzeropentotal / nonzerototalcount) * 100, 2);
                    double nonzerocanPercent = Math.Round((nonzerocantotal / nonzerototalcount) * 100, 2);

                    P_PreApproval_PreStateCount labnonzero = new P_PreApproval_PreStateCount
                    {
                        SuccessCount = nonzerosustotal,
                        RefuseCount = nonzeroreftotal,
                        PendingCount = nonzeropentotal,
                        CancelCount = nonzerocantotal,
                        TotalCount = nonzerototalcount
                    };
                    nonzerolab.Add(labnonzero);

                    P_PreApproval_Count_Amount_View nonzerosusChart = new P_PreApproval_Count_Amount_View
                    {
                        Name = "审批通过：\n" + nonzerosusPercent.ToString() + "%\n(" + nonzerosustotal + "单)",
                        PreCount = nonzerosustotal
                    };
                    nonzeroChart.Add(nonzerosusChart);
                    P_PreApproval_Count_Amount_View nonzerorefChart = new P_PreApproval_Count_Amount_View
                    {
                        Name = "审批驳回：\n" + nonzerorefPercent.ToString() + "%\n(" + nonzeroreftotal + "单)",
                        PreCount = nonzeroreftotal
                    };
                    nonzeroChart.Add(nonzerorefChart);
                    P_PreApproval_Count_Amount_View nonzeropenChart = new P_PreApproval_Count_Amount_View
                    {
                        Name = "待审批：\n" + nonzeropenPercent.ToString() + "%\n(" + nonzeropentotal + "单)",
                        PreCount = nonzeropentotal
                    };
                    nonzeroChart.Add(nonzeropenChart);
                    P_PreApproval_Count_Amount_View nonzerocanChart = new P_PreApproval_Count_Amount_View
                    {
                        Name = "已取消：\n" + nonzerocanPercent.ToString() + "%\n(" + nonzerocantotal + "单)",
                        PreCount = nonzerocantotal
                    };
                    nonzeroChart.Add(nonzerocanChart);


                    #endregion

                    //折叠面板数据
                    var DataList = (from a in res
                                    group a by new { a.DMTerritoryCode, a.DMName, a.PreState } into b
                                    select new
                                    {
                                        b.Key.DMTerritoryCode,
                                        b.Key.DMName,
                                        b.Key.PreState,
                                        TotalPrice = b.Sum(c => c.TotalPrice),
                                        TotalCount = b.Sum(c => c.TotalCount),
                                        ZeroCount = b.Sum(c => c.newZeroCount),
                                        NonZeroCount = b.Sum(c => c.newNonZeroCount)
                                    }).ToList();
                    #region 全部
                    var ApprovalSuccesslist = DataList.Where(c => c.PreState.Equals("审批通过")).ToList();
                    var ApprovalRefuselist = DataList.Where(c => c.PreState.Equals("审批驳回")).ToList();
                    var Pendinglist = DataList.Where(c => c.PreState.Equals("待审批")).ToList();
                    var Cancellist = DataList.Where(c => c.PreState.Equals("已取消")).ToList();

                    List<P_PreApproval_TAB_State> SuccessDatalist = new List<P_PreApproval_TAB_State>();
                    List<P_PreApproval_TAB_State> RefuseDatalist = new List<P_PreApproval_TAB_State>();
                    List<P_PreApproval_TAB_State> PendingDatalist = new List<P_PreApproval_TAB_State>();
                    List<P_PreApproval_TAB_State> CancelDatalist = new List<P_PreApproval_TAB_State>();

                    #region 审批通过
                    foreach (var item in ApprovalSuccesslist)
                    {
                        List<P_PreApproval_DOWN_State> downList = new List<P_PreApproval_DOWN_State>();
                        if (position.ToUpper() == "BU" || position.ToUpper() == "RM")
                        {
                            //折叠面板内包含自己的下单数据
                            var dmDataList = res.Where(c => c.PreState.Equals("审批通过") && c.MRTerritoryCode == item.DMTerritoryCode)
                            .GroupBy(c => new { c.DMName, c.DMTerritoryCode, c.PreState })
                            .Select(b => new
                            {
                                b.Key.DMName,
                                b.Key.DMTerritoryCode,
                                b.Key.PreState,
                                TotalPrice = b.Sum(c => c.TotalPrice),
                                TotalCount = b.Sum(c => c.TotalCount)
                            }).ToList();
                            foreach (var dmData in dmDataList)
                            {
                                P_PreApproval_DOWN_State downView = new P_PreApproval_DOWN_State
                                {
                                    Name = dmData.DMName,
                                    TerritoryCode = dmData.DMTerritoryCode,
                                    PreState = dmData.PreState,
                                    PreCount = dmData.TotalCount,
                                    TotalBudget = dmData.TotalPrice
                                };
                                downList.Add(downView);
                            }
                            var mrDataList = res.Where(c => c.PreState.Equals("审批通过") && c.DMTerritoryCode == item.DMTerritoryCode && c.MRTerritoryCode != item.DMTerritoryCode)
                                .GroupBy(c => new { c.MRName, c.MRTerritoryCode, c.PreState })
                                .Select(b => new
                                {
                                    b.Key.MRName,
                                    b.Key.MRTerritoryCode,
                                    b.Key.PreState,
                                    TotalPrice = b.Sum(c => c.TotalPrice),
                                    TotalCount = b.Sum(c => c.TotalCount)
                                }).ToList();
                            foreach (var mrData in mrDataList)
                            {
                                P_PreApproval_DOWN_State downView = new P_PreApproval_DOWN_State
                                {
                                    Name = mrData.MRName,
                                    TerritoryCode = mrData.MRTerritoryCode,
                                    PreState = mrData.PreState,
                                    PreCount = mrData.TotalCount,
                                    TotalBudget = mrData.TotalPrice
                                };
                                downList.Add(downView);
                            }
                        }
                        P_PreApproval_TAB_State DataView = new P_PreApproval_TAB_State
                        {
                            Name = item.DMName,
                            TerritoryCode = item.DMTerritoryCode,
                            PreState = item.PreState,
                            PreCount = item.TotalCount,
                            TotalBudget = item.TotalPrice,
                            DownList = downList
                        };
                        SuccessDatalist.Add(DataView);
                    }
                    #endregion

                    #region 审批驳回
                    foreach (var item in ApprovalRefuselist)
                    {
                        List<P_PreApproval_DOWN_State> downList = new List<P_PreApproval_DOWN_State>();
                        if (position.ToUpper() == "BU" || position.ToUpper() == "RM")
                        {
                            //折叠面板内包含自己的下单数据
                            var dmDataList = res.Where(c => c.PreState.Equals("审批驳回") && c.MRTerritoryCode == item.DMTerritoryCode)
                            .GroupBy(c => new { c.DMName, c.DMTerritoryCode, c.PreState })
                            .Select(b => new
                            {
                                b.Key.DMName,
                                b.Key.DMTerritoryCode,
                                b.Key.PreState,
                                TotalPrice = b.Sum(c => c.TotalPrice),
                                TotalCount = b.Sum(c => c.TotalCount)
                            }).ToList();
                            foreach (var dmData in dmDataList)
                            {
                                P_PreApproval_DOWN_State downView = new P_PreApproval_DOWN_State
                                {
                                    Name = dmData.DMName,
                                    TerritoryCode = dmData.DMTerritoryCode,
                                    PreState = dmData.PreState,
                                    PreCount = dmData.TotalCount,
                                    TotalBudget = dmData.TotalPrice
                                };
                                downList.Add(downView);
                            }
                            var mrDataList = res.Where(c => c.PreState.Equals("审批驳回") && c.DMTerritoryCode == item.DMTerritoryCode && c.MRTerritoryCode != item.DMTerritoryCode)
                                .GroupBy(c => new { c.MRName, c.MRTerritoryCode, c.PreState })
                                .Select(b => new
                                {
                                    b.Key.MRName,
                                    b.Key.MRTerritoryCode,
                                    b.Key.PreState,
                                    TotalPrice = b.Sum(c => c.TotalPrice),
                                    TotalCount = b.Sum(c => c.TotalCount)
                                }).ToList();
                            foreach (var mrData in mrDataList)
                            {
                                P_PreApproval_DOWN_State downView = new P_PreApproval_DOWN_State
                                {
                                    Name = mrData.MRName,
                                    TerritoryCode = mrData.MRTerritoryCode,
                                    PreState = mrData.PreState,
                                    PreCount = mrData.TotalCount,
                                    TotalBudget = mrData.TotalPrice
                                };
                                downList.Add(downView);
                            }
                        }
                        P_PreApproval_TAB_State DataView = new P_PreApproval_TAB_State
                        {
                            Name = item.DMName,
                            TerritoryCode = item.DMTerritoryCode,
                            PreState = item.PreState,
                            PreCount = item.TotalCount,
                            TotalBudget = item.TotalPrice,
                            DownList = downList
                        };
                        RefuseDatalist.Add(DataView);
                    }
                    #endregion

                    #region 待审批
                    foreach (var item in Pendinglist)
                    {
                        List<P_PreApproval_DOWN_State> downList = new List<P_PreApproval_DOWN_State>();
                        if (position.ToUpper() == "BU" || position.ToUpper() == "RM")
                        {
                            //折叠面板内包含自己的下单数据
                            var dmDataList = res.Where(c => c.PreState.Equals("待审批") && c.MRTerritoryCode == item.DMTerritoryCode)
                            .GroupBy(c => new { c.DMName, c.DMTerritoryCode, c.PreState })
                            .Select(b => new
                            {
                                b.Key.DMName,
                                b.Key.DMTerritoryCode,
                                b.Key.PreState,
                                TotalPrice = b.Sum(c => c.TotalPrice),
                                TotalCount = b.Sum(c => c.TotalCount)
                            }).ToList();
                            foreach (var dmData in dmDataList)
                            {
                                P_PreApproval_DOWN_State downView = new P_PreApproval_DOWN_State
                                {
                                    Name = dmData.DMName,
                                    TerritoryCode = dmData.DMTerritoryCode,
                                    PreState = dmData.PreState,
                                    PreCount = dmData.TotalCount,
                                    TotalBudget = dmData.TotalPrice
                                };
                                downList.Add(downView);
                            }
                            var mrDataList = res.Where(c => c.PreState.Equals("待审批") && c.DMTerritoryCode == item.DMTerritoryCode && c.MRTerritoryCode != item.DMTerritoryCode)
                                .GroupBy(c => new { c.MRName, c.MRTerritoryCode, c.PreState })
                                .Select(b => new
                                {
                                    b.Key.MRName,
                                    b.Key.MRTerritoryCode,
                                    b.Key.PreState,
                                    TotalPrice = b.Sum(c => c.TotalPrice),
                                    TotalCount = b.Sum(c => c.TotalCount)
                                }).ToList();
                            foreach (var mrData in mrDataList)
                            {
                                P_PreApproval_DOWN_State downView = new P_PreApproval_DOWN_State
                                {
                                    Name = mrData.MRName,
                                    TerritoryCode = mrData.MRTerritoryCode,
                                    PreState = mrData.PreState,
                                    PreCount = mrData.TotalCount,
                                    TotalBudget = mrData.TotalPrice
                                };
                                downList.Add(downView);
                            }
                        }
                        P_PreApproval_TAB_State DataView = new P_PreApproval_TAB_State
                        {
                            Name = item.DMName,
                            TerritoryCode = item.DMTerritoryCode,
                            PreState = item.PreState,
                            PreCount = item.TotalCount,
                            TotalBudget = item.TotalPrice,
                            DownList = downList
                        };
                        PendingDatalist.Add(DataView);
                    }
                    #endregion

                    #region 已取消
                    foreach (var item in Cancellist)
                    {
                        List<P_PreApproval_DOWN_State> downList = new List<P_PreApproval_DOWN_State>();
                        if (position.ToUpper() == "BU" || position.ToUpper() == "RM")
                        {
                            //折叠面板内包含自己的下单数据
                            var dmDataList = res.Where(c => c.PreState.Equals("已取消") && c.MRTerritoryCode == item.DMTerritoryCode)
                            .GroupBy(c => new { c.DMName, c.DMTerritoryCode, c.PreState })
                            .Select(b => new
                            {
                                b.Key.DMName,
                                b.Key.DMTerritoryCode,
                                b.Key.PreState,
                                TotalPrice = b.Sum(c => c.TotalPrice),
                                TotalCount = b.Sum(c => c.TotalCount)
                            }).ToList();
                            foreach (var dmData in dmDataList)
                            {
                                P_PreApproval_DOWN_State downView = new P_PreApproval_DOWN_State
                                {
                                    Name = dmData.DMName,
                                    TerritoryCode = dmData.DMTerritoryCode,
                                    PreState = dmData.PreState,
                                    PreCount = dmData.TotalCount,
                                    TotalBudget = dmData.TotalPrice
                                };
                                downList.Add(downView);
                            }
                            var mrDataList = res.Where(c => c.PreState.Equals("已取消") && c.DMTerritoryCode == item.DMTerritoryCode && c.MRTerritoryCode != item.DMTerritoryCode)
                                .GroupBy(c => new { c.MRName, c.MRTerritoryCode, c.PreState })
                                .Select(b => new
                                {
                                    b.Key.MRName,
                                    b.Key.MRTerritoryCode,
                                    b.Key.PreState,
                                    TotalPrice = b.Sum(c => c.TotalPrice),
                                    TotalCount = b.Sum(c => c.TotalCount)
                                }).ToList();
                            foreach (var mrData in mrDataList)
                            {
                                P_PreApproval_DOWN_State downView = new P_PreApproval_DOWN_State
                                {
                                    Name = mrData.MRName,
                                    TerritoryCode = mrData.MRTerritoryCode,
                                    PreState = mrData.PreState,
                                    PreCount = mrData.TotalCount,
                                    TotalBudget = mrData.TotalPrice
                                };
                                downList.Add(downView);
                            }
                        }
                        P_PreApproval_TAB_State DataView = new P_PreApproval_TAB_State
                        {
                            Name = item.DMName,
                            TerritoryCode = item.DMTerritoryCode,
                            PreState = item.PreState,
                            PreCount = item.TotalCount,
                            TotalBudget = item.TotalPrice,
                            DownList = downList
                        };
                        CancelDatalist.Add(DataView);
                    }
                    #endregion
                    #endregion

                    #region 0元
                    var ZeroSuccesslist = DataList.Where(c => c.ZeroCount > 0 && c.PreState.Equals("审批通过")).ToList();
                    var ZeroRefuselist = DataList.Where(c => c.ZeroCount > 0 && c.PreState.Equals("审批驳回")).ToList();
                    var ZeroPendinglist = DataList.Where(c => c.ZeroCount > 0 && c.PreState.Equals("待审批")).ToList();
                    var ZeroCancellist = DataList.Where(c => c.ZeroCount > 0 && c.PreState.Equals("已取消")).ToList();

                    List<P_PreApproval_TAB_State> SuccessData = new List<P_PreApproval_TAB_State>();
                    List<P_PreApproval_TAB_State> RefuseData = new List<P_PreApproval_TAB_State>();
                    List<P_PreApproval_TAB_State> PendingData = new List<P_PreApproval_TAB_State>();
                    List<P_PreApproval_TAB_State> CancelData = new List<P_PreApproval_TAB_State>();

                    #region 审批通过
                    foreach (var item in ZeroSuccesslist)
                    {
                        List<P_PreApproval_DOWN_State> downList = new List<P_PreApproval_DOWN_State>();
                        if (position.ToUpper() == "BU" || position.ToUpper() == "RM")
                        {
                            //折叠面板内包含自己的下单数据
                            var dmDataList = res.Where(c => c.PreState.Equals("审批通过") && c.MRTerritoryCode == item.DMTerritoryCode)
                            .GroupBy(c => new { c.DMName, c.DMTerritoryCode, c.PreState })
                            .Select(b => new
                            {
                                b.Key.DMName,
                                b.Key.DMTerritoryCode,
                                b.Key.PreState,
                                TotalPrice = b.Sum(c => c.TotalPrice),
                                TotalCount = b.Sum(c => c.TotalCount)
                            }).ToList();
                            foreach (var dmData in dmDataList)
                            {
                                P_PreApproval_DOWN_State downView = new P_PreApproval_DOWN_State
                                {
                                    Name = dmData.DMName,
                                    TerritoryCode = dmData.DMTerritoryCode,
                                    PreState = dmData.PreState,
                                    PreCount = dmData.TotalCount,
                                    TotalBudget = dmData.TotalPrice
                                };
                                downList.Add(downView);
                            }
                            var mrDataList = res.Where(c => c.PreState.Equals("审批通过") && c.DMTerritoryCode == item.DMTerritoryCode && c.MRTerritoryCode != item.DMTerritoryCode)
                                .GroupBy(c => new { c.MRName, c.MRTerritoryCode, c.PreState })
                                .Select(b => new
                                {
                                    b.Key.MRName,
                                    b.Key.MRTerritoryCode,
                                    b.Key.PreState,
                                    TotalPrice = b.Sum(c => c.TotalPrice),
                                    TotalCount = b.Sum(c => c.TotalCount)
                                }).ToList();
                            foreach (var mrData in mrDataList)
                            {
                                P_PreApproval_DOWN_State downView = new P_PreApproval_DOWN_State
                                {
                                    Name = mrData.MRName,
                                    TerritoryCode = mrData.MRTerritoryCode,
                                    PreState = mrData.PreState,
                                    PreCount = mrData.TotalCount,
                                    TotalBudget = mrData.TotalPrice
                                };
                                downList.Add(downView);
                            }
                        }
                        P_PreApproval_TAB_State DataView = new P_PreApproval_TAB_State
                        {
                            Name = item.DMName,
                            TerritoryCode = item.DMTerritoryCode,
                            PreState = item.PreState,
                            PreCount = item.TotalCount,
                            TotalBudget = item.TotalPrice,
                            DownList = downList
                        };
                        SuccessData.Add(DataView);
                    }
                    #endregion

                    #region 审批驳回
                    foreach (var item in ZeroRefuselist)
                    {
                        List<P_PreApproval_DOWN_State> downList = new List<P_PreApproval_DOWN_State>();
                        if (position.ToUpper() == "BU" || position.ToUpper() == "RM")
                        {
                            //折叠面板内包含自己的下单数据
                            var dmDataList = res.Where(c => c.PreState.Equals("审批驳回") && c.MRTerritoryCode == item.DMTerritoryCode)
                            .GroupBy(c => new { c.DMName, c.DMTerritoryCode, c.PreState })
                            .Select(b => new
                            {
                                b.Key.DMName,
                                b.Key.DMTerritoryCode,
                                b.Key.PreState,
                                TotalPrice = b.Sum(c => c.TotalPrice),
                                TotalCount = b.Sum(c => c.TotalCount)
                            }).ToList();
                            foreach (var dmData in dmDataList)
                            {
                                P_PreApproval_DOWN_State downView = new P_PreApproval_DOWN_State
                                {
                                    Name = dmData.DMName,
                                    TerritoryCode = dmData.DMTerritoryCode,
                                    PreState = dmData.PreState,
                                    PreCount = dmData.TotalCount,
                                    TotalBudget = dmData.TotalPrice
                                };
                                downList.Add(downView);
                            }
                            var mrDataList = res.Where(c => c.PreState.Equals("审批驳回") && c.DMTerritoryCode == item.DMTerritoryCode && c.MRTerritoryCode != item.DMTerritoryCode)
                                .GroupBy(c => new { c.MRName, c.MRTerritoryCode, c.PreState })
                                .Select(b => new
                                {
                                    b.Key.MRName,
                                    b.Key.MRTerritoryCode,
                                    b.Key.PreState,
                                    TotalPrice = b.Sum(c => c.TotalPrice),
                                    TotalCount = b.Sum(c => c.TotalCount)
                                }).ToList();
                            foreach (var mrData in mrDataList)
                            {
                                P_PreApproval_DOWN_State downView = new P_PreApproval_DOWN_State
                                {
                                    Name = mrData.MRName,
                                    TerritoryCode = mrData.MRTerritoryCode,
                                    PreState = mrData.PreState,
                                    PreCount = mrData.TotalCount,
                                    TotalBudget = mrData.TotalPrice
                                };
                                downList.Add(downView);
                            }
                        }
                        P_PreApproval_TAB_State DataView = new P_PreApproval_TAB_State
                        {
                            Name = item.DMName,
                            TerritoryCode = item.DMTerritoryCode,
                            PreState = item.PreState,
                            PreCount = item.TotalCount,
                            TotalBudget = item.TotalPrice,
                            DownList = downList
                        };
                        RefuseData.Add(DataView);
                    }
                    #endregion

                    #region 待审批
                    foreach (var item in ZeroPendinglist)
                    {
                        List<P_PreApproval_DOWN_State> downList = new List<P_PreApproval_DOWN_State>();
                        if (position.ToUpper() == "BU" || position.ToUpper() == "RM")
                        {
                            //折叠面板内包含自己的下单数据
                            var dmDataList = res.Where(c => c.PreState.Equals("待审批") && c.MRTerritoryCode == item.DMTerritoryCode)
                            .GroupBy(c => new { c.DMName, c.DMTerritoryCode, c.PreState })
                            .Select(b => new
                            {
                                b.Key.DMName,
                                b.Key.DMTerritoryCode,
                                b.Key.PreState,
                                TotalPrice = b.Sum(c => c.TotalPrice),
                                TotalCount = b.Sum(c => c.TotalCount)
                            }).ToList();
                            foreach (var dmData in dmDataList)
                            {
                                P_PreApproval_DOWN_State downView = new P_PreApproval_DOWN_State
                                {
                                    Name = dmData.DMName,
                                    TerritoryCode = dmData.DMTerritoryCode,
                                    PreState = dmData.PreState,
                                    PreCount = dmData.TotalCount,
                                    TotalBudget = dmData.TotalPrice
                                };
                                downList.Add(downView);
                            }
                            var mrDataList = res.Where(c => c.PreState.Equals("待审批") && c.DMTerritoryCode == item.DMTerritoryCode && c.MRTerritoryCode != item.DMTerritoryCode)
                                .GroupBy(c => new { c.MRName, c.MRTerritoryCode, c.PreState })
                                .Select(b => new
                                {
                                    b.Key.MRName,
                                    b.Key.MRTerritoryCode,
                                    b.Key.PreState,
                                    TotalPrice = b.Sum(c => c.TotalPrice),
                                    TotalCount = b.Sum(c => c.TotalCount)
                                }).ToList();
                            foreach (var mrData in mrDataList)
                            {
                                P_PreApproval_DOWN_State downView = new P_PreApproval_DOWN_State
                                {
                                    Name = mrData.MRName,
                                    TerritoryCode = mrData.MRTerritoryCode,
                                    PreState = mrData.PreState,
                                    PreCount = mrData.TotalCount,
                                    TotalBudget = mrData.TotalPrice
                                };
                                downList.Add(downView);
                            }
                        }
                        P_PreApproval_TAB_State DataView = new P_PreApproval_TAB_State
                        {
                            Name = item.DMName,
                            TerritoryCode = item.DMTerritoryCode,
                            PreState = item.PreState,
                            PreCount = item.TotalCount,
                            TotalBudget = item.TotalPrice,
                            DownList = downList
                        };
                        PendingData.Add(DataView);
                    }
                    #endregion

                    #region 已取消
                    foreach (var item in ZeroCancellist)
                    {
                        List<P_PreApproval_DOWN_State> downList = new List<P_PreApproval_DOWN_State>();
                        if (position.ToUpper() == "BU" || position.ToUpper() == "RM")
                        {
                            //折叠面板内包含自己的下单数据
                            var dmDataList = res.Where(c => c.PreState.Equals("已取消") && c.MRTerritoryCode == item.DMTerritoryCode)
                            .GroupBy(c => new { c.DMName, c.DMTerritoryCode, c.PreState })
                            .Select(b => new
                            {
                                b.Key.DMName,
                                b.Key.DMTerritoryCode,
                                b.Key.PreState,
                                TotalPrice = b.Sum(c => c.TotalPrice),
                                TotalCount = b.Sum(c => c.TotalCount)
                            }).ToList();
                            foreach (var dmData in dmDataList)
                            {
                                P_PreApproval_DOWN_State downView = new P_PreApproval_DOWN_State
                                {
                                    Name = dmData.DMName,
                                    TerritoryCode = dmData.DMTerritoryCode,
                                    PreState = dmData.PreState,
                                    PreCount = dmData.TotalCount,
                                    TotalBudget = dmData.TotalPrice
                                };
                                downList.Add(downView);
                            }
                            var mrDataList = res.Where(c => c.PreState.Equals("已取消") && c.DMTerritoryCode == item.DMTerritoryCode && c.MRTerritoryCode != item.DMTerritoryCode)
                                .GroupBy(c => new { c.MRName, c.MRTerritoryCode, c.PreState })
                                .Select(b => new
                                {
                                    b.Key.MRName,
                                    b.Key.MRTerritoryCode,
                                    b.Key.PreState,
                                    TotalPrice = b.Sum(c => c.TotalPrice),
                                    TotalCount = b.Sum(c => c.TotalCount)
                                }).ToList();
                            foreach (var mrData in mrDataList)
                            {
                                P_PreApproval_DOWN_State downView = new P_PreApproval_DOWN_State
                                {
                                    Name = mrData.MRName,
                                    TerritoryCode = mrData.MRTerritoryCode,
                                    PreState = mrData.PreState,
                                    PreCount = mrData.TotalCount,
                                    TotalBudget = mrData.TotalPrice
                                };
                                downList.Add(downView);
                            }
                        }
                        P_PreApproval_TAB_State DataView = new P_PreApproval_TAB_State
                        {
                            Name = item.DMName,
                            TerritoryCode = item.DMTerritoryCode,
                            PreState = item.PreState,
                            PreCount = item.TotalCount,
                            TotalBudget = item.TotalPrice,
                            DownList = downList
                        };
                        CancelData.Add(DataView);
                    }
                    #endregion
                    #endregion

                    #region 非0元
                    var NZSuccesslist = DataList.Where(c => c.NonZeroCount > 0 && c.PreState.Equals("审批通过")).ToList();
                    var NZRefuselist = DataList.Where(c => c.NonZeroCount > 0 && c.PreState.Equals("审批驳回")).ToList();
                    var NZPendinglist = DataList.Where(c => c.NonZeroCount > 0 && c.PreState.Equals("待审批")).ToList();
                    var NZCancellist = DataList.Where(c => c.NonZeroCount > 0 && c.PreState.Equals("已取消")).ToList();

                    List<P_PreApproval_TAB_State> NZSuccessData = new List<P_PreApproval_TAB_State>();
                    List<P_PreApproval_TAB_State> NZRefuseData = new List<P_PreApproval_TAB_State>();
                    List<P_PreApproval_TAB_State> NZPendingData = new List<P_PreApproval_TAB_State>();
                    List<P_PreApproval_TAB_State> NZCancelData = new List<P_PreApproval_TAB_State>();

                    #region 审批通过
                    foreach (var item in NZSuccesslist)
                    {
                        List<P_PreApproval_DOWN_State> downList = new List<P_PreApproval_DOWN_State>();
                        if (position.ToUpper() == "BU" || position.ToUpper() == "RM")
                        {
                            //折叠面板内包含自己的下单数据
                            var dmDataList = res.Where(c => c.PreState.Equals("审批通过") && c.MRTerritoryCode == item.DMTerritoryCode)
                            .GroupBy(c => new { c.DMName, c.DMTerritoryCode, c.PreState })
                            .Select(b => new
                            {
                                b.Key.DMName,
                                b.Key.DMTerritoryCode,
                                b.Key.PreState,
                                TotalPrice = b.Sum(c => c.TotalPrice),
                                TotalCount = b.Sum(c => c.TotalCount)
                            }).ToList();
                            foreach (var dmData in dmDataList)
                            {
                                P_PreApproval_DOWN_State downView = new P_PreApproval_DOWN_State
                                {
                                    Name = dmData.DMName,
                                    TerritoryCode = dmData.DMTerritoryCode,
                                    PreState = dmData.PreState,
                                    PreCount = dmData.TotalCount,
                                    TotalBudget = dmData.TotalPrice
                                };
                                downList.Add(downView);
                            }
                            var mrDataList = res.Where(c => c.PreState.Equals("审批通过") && c.DMTerritoryCode == item.DMTerritoryCode && c.MRTerritoryCode != item.DMTerritoryCode)
                                .GroupBy(c => new { c.MRName, c.MRTerritoryCode, c.PreState })
                                .Select(b => new
                                {
                                    b.Key.MRName,
                                    b.Key.MRTerritoryCode,
                                    b.Key.PreState,
                                    TotalPrice = b.Sum(c => c.TotalPrice),
                                    TotalCount = b.Sum(c => c.TotalCount)
                                }).ToList();
                            foreach (var mrData in mrDataList)
                            {
                                P_PreApproval_DOWN_State downView = new P_PreApproval_DOWN_State
                                {
                                    Name = mrData.MRName,
                                    TerritoryCode = mrData.MRTerritoryCode,
                                    PreState = mrData.PreState,
                                    PreCount = mrData.TotalCount,
                                    TotalBudget = mrData.TotalPrice
                                };
                                downList.Add(downView);
                            }
                        }
                        P_PreApproval_TAB_State DataView = new P_PreApproval_TAB_State
                        {
                            Name = item.DMName,
                            TerritoryCode = item.DMTerritoryCode,
                            PreState = item.PreState,
                            PreCount = item.TotalCount,
                            TotalBudget = item.TotalPrice,
                            DownList = downList
                        };
                        SuccessData.Add(DataView);
                    }
                    #endregion

                    #region 审批驳回
                    foreach (var item in NZRefuselist)
                    {
                        List<P_PreApproval_DOWN_State> downList = new List<P_PreApproval_DOWN_State>();
                        if (position.ToUpper() == "BU" || position.ToUpper() == "RM")
                        {
                            //折叠面板内包含自己的下单数据
                            var dmDataList = res.Where(c => c.PreState.Equals("审批驳回") && c.MRTerritoryCode == item.DMTerritoryCode)
                            .GroupBy(c => new { c.DMName, c.DMTerritoryCode, c.PreState })
                            .Select(b => new
                            {
                                b.Key.DMName,
                                b.Key.DMTerritoryCode,
                                b.Key.PreState,
                                TotalPrice = b.Sum(c => c.TotalPrice),
                                TotalCount = b.Sum(c => c.TotalCount)
                            }).ToList();
                            foreach (var dmData in dmDataList)
                            {
                                P_PreApproval_DOWN_State downView = new P_PreApproval_DOWN_State
                                {
                                    Name = dmData.DMName,
                                    TerritoryCode = dmData.DMTerritoryCode,
                                    PreState = dmData.PreState,
                                    PreCount = dmData.TotalCount,
                                    TotalBudget = dmData.TotalPrice
                                };
                                downList.Add(downView);
                            }
                            var mrDataList = res.Where(c => c.PreState.Equals("审批驳回") && c.DMTerritoryCode == item.DMTerritoryCode && c.MRTerritoryCode != item.DMTerritoryCode)
                                .GroupBy(c => new { c.MRName, c.MRTerritoryCode, c.PreState })
                                .Select(b => new
                                {
                                    b.Key.MRName,
                                    b.Key.MRTerritoryCode,
                                    b.Key.PreState,
                                    TotalPrice = b.Sum(c => c.TotalPrice),
                                    TotalCount = b.Sum(c => c.TotalCount)
                                }).ToList();
                            foreach (var mrData in mrDataList)
                            {
                                P_PreApproval_DOWN_State downView = new P_PreApproval_DOWN_State
                                {
                                    Name = mrData.MRName,
                                    TerritoryCode = mrData.MRTerritoryCode,
                                    PreState = mrData.PreState,
                                    PreCount = mrData.TotalCount,
                                    TotalBudget = mrData.TotalPrice
                                };
                                downList.Add(downView);
                            }
                        }
                        P_PreApproval_TAB_State DataView = new P_PreApproval_TAB_State
                        {
                            Name = item.DMName,
                            TerritoryCode = item.DMTerritoryCode,
                            PreState = item.PreState,
                            PreCount = item.TotalCount,
                            TotalBudget = item.TotalPrice,
                            DownList = downList
                        };
                        NZRefuseData.Add(DataView);
                    }
                    #endregion

                    #region 待审批
                    foreach (var item in NZPendinglist)
                    {
                        List<P_PreApproval_DOWN_State> downList = new List<P_PreApproval_DOWN_State>();
                        if (position.ToUpper() == "BU" || position.ToUpper() == "RM")
                        {
                            //折叠面板内包含自己的下单数据
                            var dmDataList = res.Where(c => c.PreState.Equals("待审批") && c.MRTerritoryCode == item.DMTerritoryCode)
                            .GroupBy(c => new { c.DMName, c.DMTerritoryCode, c.PreState })
                            .Select(b => new
                            {
                                b.Key.DMName,
                                b.Key.DMTerritoryCode,
                                b.Key.PreState,
                                TotalPrice = b.Sum(c => c.TotalPrice),
                                TotalCount = b.Sum(c => c.TotalCount)
                            }).ToList();
                            foreach (var dmData in dmDataList)
                            {
                                P_PreApproval_DOWN_State downView = new P_PreApproval_DOWN_State
                                {
                                    Name = dmData.DMName,
                                    TerritoryCode = dmData.DMTerritoryCode,
                                    PreState = dmData.PreState,
                                    PreCount = dmData.TotalCount,
                                    TotalBudget = dmData.TotalPrice
                                };
                                downList.Add(downView);
                            }
                            var mrDataList = res.Where(c => c.PreState.Equals("待审批") && c.DMTerritoryCode == item.DMTerritoryCode && c.MRTerritoryCode != item.DMTerritoryCode)
                                .GroupBy(c => new { c.MRName, c.MRTerritoryCode, c.PreState })
                                .Select(b => new
                                {
                                    b.Key.MRName,
                                    b.Key.MRTerritoryCode,
                                    b.Key.PreState,
                                    TotalPrice = b.Sum(c => c.TotalPrice),
                                    TotalCount = b.Sum(c => c.TotalCount)
                                }).ToList();
                            foreach (var mrData in mrDataList)
                            {
                                P_PreApproval_DOWN_State downView = new P_PreApproval_DOWN_State
                                {
                                    Name = mrData.MRName,
                                    TerritoryCode = mrData.MRTerritoryCode,
                                    PreState = mrData.PreState,
                                    PreCount = mrData.TotalCount,
                                    TotalBudget = mrData.TotalPrice
                                };
                                downList.Add(downView);
                            }
                        }
                        P_PreApproval_TAB_State DataView = new P_PreApproval_TAB_State
                        {
                            Name = item.DMName,
                            TerritoryCode = item.DMTerritoryCode,
                            PreState = item.PreState,
                            PreCount = item.TotalCount,
                            TotalBudget = item.TotalPrice,
                            DownList = downList
                        };
                        NZPendingData.Add(DataView);
                    }
                    #endregion

                    #region 已取消
                    foreach (var item in NZCancellist)
                    {
                        List<P_PreApproval_DOWN_State> downList = new List<P_PreApproval_DOWN_State>();
                        if (position.ToUpper() == "BU" || position.ToUpper() == "RM")
                        {
                            //折叠面板内包含自己的下单数据
                            var dmDataList = res.Where(c => c.PreState.Equals("已取消") && c.MRTerritoryCode == item.DMTerritoryCode)
                            .GroupBy(c => new { c.DMName, c.DMTerritoryCode, c.PreState })
                            .Select(b => new
                            {
                                b.Key.DMName,
                                b.Key.DMTerritoryCode,
                                b.Key.PreState,
                                TotalPrice = b.Sum(c => c.TotalPrice),
                                TotalCount = b.Sum(c => c.TotalCount)
                            }).ToList();
                            foreach (var dmData in dmDataList)
                            {
                                P_PreApproval_DOWN_State downView = new P_PreApproval_DOWN_State
                                {
                                    Name = dmData.DMName,
                                    TerritoryCode = dmData.DMTerritoryCode,
                                    PreState = dmData.PreState,
                                    PreCount = dmData.TotalCount,
                                    TotalBudget = dmData.TotalPrice
                                };
                                downList.Add(downView);
                            }
                            var mrDataList = res.Where(c => c.PreState.Equals("已取消") && c.DMTerritoryCode == item.DMTerritoryCode && c.MRTerritoryCode != item.DMTerritoryCode)
                                .GroupBy(c => new { c.MRName, c.MRTerritoryCode, c.PreState })
                                .Select(b => new
                                {
                                    b.Key.MRName,
                                    b.Key.MRTerritoryCode,
                                    b.Key.PreState,
                                    TotalPrice = b.Sum(c => c.TotalPrice),
                                    TotalCount = b.Sum(c => c.TotalCount)
                                }).ToList();
                            foreach (var mrData in mrDataList)
                            {
                                P_PreApproval_DOWN_State downView = new P_PreApproval_DOWN_State
                                {
                                    Name = mrData.MRName,
                                    TerritoryCode = mrData.MRTerritoryCode,
                                    PreState = mrData.PreState,
                                    PreCount = mrData.TotalCount,
                                    TotalBudget = mrData.TotalPrice
                                };
                                downList.Add(downView);
                            }
                        }
                        P_PreApproval_TAB_State DataView = new P_PreApproval_TAB_State
                        {
                            Name = item.DMName,
                            TerritoryCode = item.DMTerritoryCode,
                            PreState = item.PreState,
                            PreCount = item.TotalCount,
                            TotalBudget = item.TotalPrice,
                            DownList = downList
                        };
                        NZCancelData.Add(DataView);
                    }
                    #endregion
                    #endregion
                    return Json(new
                    {
                        state = 1,
                        allLab = alllab,
                        dataCountChart = countChart,
                        LabZero = zerolab,
                        dataCountChartZero = zerocountChart,
                        LabNonZero = nonzerolab,
                        dataCountChartNonZero = nonzeroChart,

                        SusList = SuccessDatalist,
                        RefList = RefuseDatalist,
                        PenList = PendingDatalist,
                        CanList = CancelDatalist,

                        ZeroSusList = SuccessData,
                        ZeroRefList = RefuseData,
                        ZeroPenList = PendingData,
                        ZeroCanList = CancelData,

                        NZSuccessData = SuccessDatalist,
                        NZRefuseData = RefuseDatalist,
                        NZPendingData = PendingDatalist,
                        NZCancelData = CancelDatalist
                    });
                }
                else
                {
                    return Json(new { state = 0, txt = "无符合筛选条件的预申请分析数据！" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("LoadPreApprovalDataByState" + ex.Message);
                return Json(new { state = 0 });
            }

        }
        #endregion
        #endregion

        #region 有效预申请分析-向上
        [iPathOAuthFilter(MappingKey = "0x0006", CallBackUrl = true)]
        [HttpGet]
        public ActionResult PreApprovalAnalysisUp()
        {
            var NowDate = DateTime.Now;
            string StartDate = "";
            string EndDate = "";
            if (NowDate.Day == 1)
            {
                StartDate = DateTime.Now.AddMonths(-1).ToString("yyyy-MM") + "-01";
                EndDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            }
            else
            {
                StartDate = DateTime.Now.ToString("yyyy-MM") + "-01";
                EndDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            }

            ViewBag.StartDate = StartDate;
            ViewBag.EndDate = EndDate;
            return View();
        }
        [HttpPost]
        public JsonResult LoadPreApprovalUpData(string DataType, string StartDate, string EndDate)
        {
            try
            {
                List<P_PreApproval_Count_Amount_View> amountChartList = new List<P_PreApproval_Count_Amount_View>();
                List<P_PreApproval_Count_Amount_View> countChartList = new List<P_PreApproval_Count_Amount_View>();
                var PreChannel = PreApprovalClientChannelFactory.GetChannel();

                string position = "RM";
                string territoryCode = "RES_RM_N1";

                var res = PreChannel.LoadPreApprovalUpData(CurrentWxUser.UserId, position, territoryCode, StartDate, EndDate);

                if (res != null && res.Count > 0)
                {
                    decimal totalAmount = 0;
                    double totalCount = 0;
                    string belong = string.Empty;
                    #region 全部
                    if (DataType == "all")
                    {
                        totalAmount = res.Sum(c => c.TotalPrice);
                        totalCount = res.Sum(c => c.TotalCount);

                        if (position == "BU")
                        {
                            foreach (var item in res)
                            {
                                //金额占比
                                decimal proportionAmount = Math.Round((item.TotalPrice / totalAmount), 2) * 100;
                                P_PreApproval_Count_Amount_View pAmount = new P_PreApproval_Count_Amount_View
                                {
                                    Name = item.OwnTerritory.ToString() + "：\n" + proportionAmount.ToString() + "%\n(¥" + item.TotalPrice + ")",
                                    TotalBudget = item.TotalPrice
                                };
                                amountChartList.Add(pAmount);

                                //数量占比
                                double proportionCount = Math.Round((item.TotalCount / totalCount), 2) * 100;
                                P_PreApproval_Count_Amount_View pCount = new P_PreApproval_Count_Amount_View
                                {
                                    Name = item.OwnTerritory.ToString() + "：\n" + proportionCount.ToString() + "%\n(" + item.TotalCount + "单)",
                                    PreCount = item.TotalCount
                                };
                                countChartList.Add(pCount);
                            }
                        }
                        else
                        {
                            //own
                            var ownList = res.Where(c => c.OwnTerritory.Equals(territoryCode)).ToList();
                            decimal ownAmount = ownList.Sum(c => c.TotalPrice);
                            double ownCount = ownList.Sum(c => c.TotalCount);

                            //金额占比
                            decimal proportionAmount = Math.Round((ownAmount / totalAmount), 2) * 100;
                            P_PreApproval_Count_Amount_View pAmount = new P_PreApproval_Count_Amount_View
                            {
                                Name = territoryCode + "：\n" + proportionAmount.ToString() + "%\n(¥" + ownAmount + ")",
                                TotalBudget = ownAmount
                            };
                            amountChartList.Add(pAmount);
                            P_PreApproval_Count_Amount_View pAmount_Other = new P_PreApproval_Count_Amount_View
                            {
                                Name = "Other：\n" + (100 - proportionAmount).ToString() + "%\n(¥" + (totalAmount - ownAmount) + ")",
                                TotalBudget = totalAmount - ownAmount
                            };
                            amountChartList.Add(pAmount_Other);

                            //数量占比
                            double proportionCount = Math.Round((ownCount / totalCount), 2) * 100;
                            P_PreApproval_Count_Amount_View pCount = new P_PreApproval_Count_Amount_View
                            {
                                Name = territoryCode + "：\n" + proportionCount.ToString() + "%\n(" + ownCount + "单)",
                                PreCount = ownCount
                            };
                            countChartList.Add(pCount);
                            P_PreApproval_Count_Amount_View pCount_Other = new P_PreApproval_Count_Amount_View
                            {
                                Name = "Other：\n" + (100 - proportionCount).ToString() + "%(" + (totalCount - ownCount) + "单)",
                                PreCount = totalCount - ownCount
                            };
                            countChartList.Add(pCount_Other);
                        }

                        if (position.ToUpper() == "BU")
                            belong = "BU：" + res[0].BelongTerritory.ToString();
                        else
                            belong = "所属区域：" + res[0].BelongTerritory.ToString();

                    }
                    #endregion

                    #region 非0元
                    if (DataType == "1")
                    {
                        totalAmount = res.Sum(c => c.TotalPrice);
                        totalCount = res.Sum(c => c.newNonZeroCount);

                        if (position == "BU")
                        {
                            foreach (var item in res)
                            {
                                //金额占比
                                decimal proportionAmount = Math.Round((item.TotalPrice / totalAmount), 2) * 100;
                                P_PreApproval_Count_Amount_View pAmount = new P_PreApproval_Count_Amount_View
                                {
                                    Name = item.OwnTerritory.ToString() + "：\n" + proportionAmount.ToString() + "%\n(¥" + item.TotalPrice + ")",
                                    TotalBudget = item.TotalPrice
                                };
                                amountChartList.Add(pAmount);

                                //数量占比
                                double proportionCount = Math.Round((item.newNonZeroCount / totalCount), 2) * 100;
                                P_PreApproval_Count_Amount_View pCount = new P_PreApproval_Count_Amount_View
                                {
                                    Name = item.OwnTerritory.ToString() + "：\n" + proportionCount.ToString() + "%\n(" + item.newNonZeroCount + "单)",
                                    PreCount = item.newNonZeroCount
                                };
                                countChartList.Add(pCount);
                            }
                        }
                        else
                        {
                            //own
                            var ownList = res.Where(c => c.OwnTerritory.Equals(territoryCode)).ToList();
                            decimal ownAmount = ownList.Sum(c => c.TotalPrice);
                            double ownCount = ownList.Sum(c => c.newNonZeroCount);

                            //金额占比
                            decimal proportionAmount = Math.Round((ownAmount / totalAmount), 2) * 100;
                            P_PreApproval_Count_Amount_View pAmount = new P_PreApproval_Count_Amount_View
                            {
                                Name = territoryCode + "：\n" + proportionAmount.ToString() + "%\n(¥" + ownAmount + ")",
                                TotalBudget = ownAmount
                            };
                            amountChartList.Add(pAmount);
                            P_PreApproval_Count_Amount_View pAmount_Other = new P_PreApproval_Count_Amount_View
                            {
                                Name = "Other：\n" + (100 - proportionAmount).ToString() + "%\n(¥" + (totalAmount - ownAmount) + ")",
                                TotalBudget = totalAmount - ownAmount
                            };
                            amountChartList.Add(pAmount_Other);

                            //数量占比
                            double proportionCount = Math.Round((ownCount / totalCount), 2) * 100;
                            P_PreApproval_Count_Amount_View pCount = new P_PreApproval_Count_Amount_View
                            {
                                Name = territoryCode + "：\n" + proportionCount.ToString() + "%\n(" + ownCount + "单)",
                                PreCount = ownCount
                            };
                            countChartList.Add(pCount);
                            P_PreApproval_Count_Amount_View pCount_Other = new P_PreApproval_Count_Amount_View
                            {
                                Name = "Other：\n" + (100 - proportionCount).ToString() + "%(" + (totalCount - ownCount) + "单)",
                                PreCount = totalCount - ownCount
                            };
                            countChartList.Add(pCount_Other);
                        }

                        if (position.ToUpper() == "BU")
                            belong = "BU：" + res[0].BelongTerritory.ToString();
                        else
                            belong = "所属区域：" + res[0].BelongTerritory.ToString();

                    }
                    #endregion

                    #region 0元
                    if (DataType == "0")
                    {
                        totalAmount = 0;
                        totalCount = res.Sum(c => c.newZeroCount);

                        if (position == "BU")
                        {
                            foreach (var item in res)
                            {
                                //数量占比
                                double proportionCount = Math.Round((item.newZeroCount / totalCount), 2) * 100;
                                P_PreApproval_Count_Amount_View pCount = new P_PreApproval_Count_Amount_View
                                {
                                    Name = item.OwnTerritory.ToString() + "：\n" + proportionCount.ToString() + "%\n(" + item.newZeroCount + "单)",
                                    PreCount = item.newZeroCount
                                };
                                countChartList.Add(pCount);
                            }
                        }
                        else
                        {
                            //own
                            var ownList = res.Where(c => c.OwnTerritory.Equals(territoryCode)).ToList();
                            decimal ownAmount = ownList.Sum(c => c.TotalPrice);
                            double ownCount = ownList.Sum(c => c.newZeroCount);

                            //数量占比
                            double proportionCount = Math.Round((ownCount / totalCount), 2) * 100;
                            P_PreApproval_Count_Amount_View pCount = new P_PreApproval_Count_Amount_View
                            {
                                Name = territoryCode + "：\n" + proportionCount.ToString() + "%\n(" + ownCount + "单)",
                                PreCount = ownCount
                            };
                            countChartList.Add(pCount);
                            P_PreApproval_Count_Amount_View pCount_Other = new P_PreApproval_Count_Amount_View
                            {
                                Name = "Other：\n" + (100 - proportionCount).ToString() + "%(" + (totalCount - ownCount) + "单)",
                                PreCount = totalCount - ownCount
                            };
                            countChartList.Add(pCount_Other);
                        }

                        if (position.ToUpper() == "BU")
                            belong = "BU：" + res[0].BelongTerritory.ToString();
                        else
                            belong = "所属区域：" + res[0].BelongTerritory.ToString();

                    }
                    #endregion

                    return Json(new
                    {
                        state = 1,
                        dataAmountChart = amountChartList,
                        dataCountChart = countChartList,
                        TotalAmount = totalAmount,
                        TotalCount = totalCount,
                        Belong = belong,
                        position = CurrentWxUser.Role,
                        territoryCode = CurrentWxUser.TerritoryCode
                    });
                }
                else
                {
                    return Json(new { state = 0, txt = "无符合筛选条件的预申请分析数据！" });
                }


            }
            catch (Exception ex)
            {
                LogHelper.Error("LoadPreApprovalUpData" + ex.Message);
                return Json(new
                {
                    state = -1,
                    position = CurrentWxUser.Role,
                    territoryCode = CurrentWxUser.TerritoryCode
                });
            }

        }
        #endregion

        #region 订单分析-向上
        [iPathOAuthFilter(MappingKey = "0x0006", CallBackUrl = true)]
        [HttpGet]
        public ActionResult OrderAnalysisUp()
        {
            var NowDate = DateTime.Now;
            string StartDate = "";
            string EndDate = "";
            if (NowDate.Day == 1)
            {
                StartDate = DateTime.Now.AddMonths(-1).ToString("yyyy-MM") + "-01";
                EndDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            }
            else
            {
                StartDate = DateTime.Now.ToString("yyyy-MM") + "-01";
                EndDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            }

            ViewBag.StartDate = StartDate;
            ViewBag.EndDate = EndDate;
            return View();
        }
        [HttpPost]
        public JsonResult LoadUpOrderAnalysisData(string StartDate, string EndDate)
        {
            try
            {
                List<P_Order_Count_Amount_View> amountChartList = new List<P_Order_Count_Amount_View>();
                List<P_Order_Count_Amount_View> countChartList = new List<P_Order_Count_Amount_View>();
                var orderChannel = OrderApiClientChannelFactory.GetChannel();
                string position = "BU";
                string territoryCode = "RES";

                var res = orderChannel.LoadUpOrderAnalysisData(CurrentWxUser.UserId, position, territoryCode, StartDate, EndDate);

                if (res != null && res.Count > 0)
                {
                    var queryResult = (from a in res
                                       group a by new { a.OwnTerritory, a.BelongTerritory } into b
                                       select new
                                       {
                                           b.Key.OwnTerritory,
                                           b.Key.BelongTerritory,
                                           OrderAmount = b.Sum(c => c.OrderAmount),
                                           OrderCount = b.Count()
                                       }).ToList();
                    decimal totalAmount = queryResult.Sum(c => c.OrderAmount);
                    int totalCount = queryResult.Sum(c => c.OrderCount);

                    if (position == "BU")
                    {
                        foreach (var item in queryResult)
                        {
                            //金额占比
                            decimal proportionAmount = Math.Round(((decimal)item.OrderAmount / totalAmount), 2) * 100;
                            P_Order_Count_Amount_View p_Order_Amount = new P_Order_Count_Amount_View
                            {
                                Name = item.OwnTerritory.ToString() + "：\n" + proportionAmount.ToString() + "%\n(" + item.OrderAmount.ToString("C").Replace('$', '¥') + ")",
                                OrderAmount = item.OrderAmount
                            };
                            amountChartList.Add(p_Order_Amount);

                            //数量占比
                            decimal proportionCount = Math.Round(((decimal)item.OrderCount / totalCount), 2) * 100;
                            P_Order_Count_Amount_View p_Order_Count = new P_Order_Count_Amount_View
                            {
                                Name = item.OwnTerritory.ToString() + "：\n" + proportionCount.ToString() + "%\n(" + item.OrderCount.ToString("N").Substring(0, item.OrderCount.ToString("N").Length - 3) + "单)",
                                OrderCount = item.OrderCount
                            };
                            countChartList.Add(p_Order_Count);
                        }
                    }
                    else
                    {
                        //own
                        var ownList = queryResult.Where(c => c.OwnTerritory.Equals(territoryCode)).ToList();
                        decimal ownAmount = ownList.Sum(c => c.OrderAmount);
                        int ownCount = ownList.Sum(c => c.OrderCount);

                        //金额占比
                        decimal proportionAmount = Math.Round(((decimal)ownAmount / totalAmount), 2) * 100;
                        P_Order_Count_Amount_View p_Order_Amount = new P_Order_Count_Amount_View
                        {
                            Name = territoryCode + "：\n" + proportionAmount.ToString() + "%\n(" + ownAmount.ToString("C").Replace('$', '¥') + ")",
                            OrderAmount = ownList[0].OrderAmount
                        };
                        amountChartList.Add(p_Order_Amount);
                        P_Order_Count_Amount_View p_Order_Amount_Other = new P_Order_Count_Amount_View
                        {
                            Name = "Other：\n" + (100 - proportionAmount).ToString() + "%\n(" + (totalAmount - ownAmount).ToString("C").Replace('$', '¥') + ")",
                            OrderAmount = totalAmount - ownAmount
                        };
                        amountChartList.Add(p_Order_Amount_Other);

                        //数量占比
                        decimal proportionCount = Math.Round(((decimal)ownCount / totalCount), 2) * 100;
                        P_Order_Count_Amount_View p_Order_Count = new P_Order_Count_Amount_View
                        {
                            Name = territoryCode + "：\n" + proportionCount.ToString() + "%\n(" + ownCount.ToString("N").Substring(0, ownCount.ToString("N").Length - 3) + "单)",
                            OrderCount = ownList[0].OrderCount
                        };
                        countChartList.Add(p_Order_Count);
                        P_Order_Count_Amount_View p_Order_Count_Other = new P_Order_Count_Amount_View
                        {
                            Name = "Other：\n" + (100 - proportionCount).ToString() + "%(" + (totalCount - ownCount).ToString("N").Substring(0, (totalCount - ownCount).ToString("N").Length - 3) + "单)",
                            OrderCount = totalCount - ownCount
                        };
                        countChartList.Add(p_Order_Count_Other);
                    }
                    string belong = string.Empty;
                    if (position.ToUpper() == "BU")
                        belong = "BU：" + queryResult[0].BelongTerritory.ToString();
                    else
                        belong = "所属：" + queryResult[0].BelongTerritory.ToString();
                    return Json(new
                    {
                        state = 1,
                        dataAmountChart = amountChartList,
                        dataCountChart = countChartList,
                        TotalAmount = totalAmount,
                        TotalCount = totalCount,
                        Belong = belong,
                        role = CurrentWxUser.Role,
                        territoryCode = CurrentWxUser.TerritoryCode
                    });
                }
                else
                {
                    return Json(new { state = 0, txt = "无有效订单数据！" });
                }


            }
            catch (Exception ex)
            {
                LogHelper.Error("LoadUpOrderAnalysisData" + ex.Message);
                return Json(new { state = 0 });
            }

        }
        #endregion

        #region 订单分析-向下
        [iPathOAuthFilter(MappingKey = "0x0006", CallBackUrl = true)]
        [HttpGet]
        public ActionResult OrderAnalysis()
        {
            var NowDate = DateTime.Now;
            string StartDate = "";
            string EndDate = "";
            if (NowDate.Day == 1)
            {
                StartDate = DateTime.Now.AddMonths(-1).ToString("yyyy-MM") + "-01";
                EndDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            }
            else
            {
                StartDate = DateTime.Now.ToString("yyyy-MM") + "-01";
                EndDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            }

            ViewBag.StartDate = StartDate;
            ViewBag.EndDate = EndDate;
            return View();
        }
        [HttpPost]
        public JsonResult LoadDownOrderAnalysisData(string StartDate, string EndDate)
        {
            try
            {

                List<P_Order_Count_Amount_View> countChartList = new List<P_Order_Count_Amount_View>();
                var orderChannel = OrderApiClientChannelFactory.GetChannel();
                string position = "BU";
                string territoryCode = "RES";

                var res = orderChannel.LoadDownOrderAnalysisData(CurrentWxUser.UserId, position, territoryCode, StartDate, EndDate);

                if (res != null && res.Count > 0)
                {
                    #region 饼图
                    var reserveSuccessCnt = res.Where(c => c.OrderState.Equals("预定成功")).Count();
                    var reserveFailCnt = res.Where(c => c.OrderState.Equals("预定失败")).Count();
                    var returnSuccessCnt = res.Where(c => c.OrderState.Equals("退单成功")).Count();
                    var returnFailCnt = res.Where(c => c.OrderState.Equals("退单失败")).Count();
                    decimal proportionCount;
                    //预定成功
                    proportionCount = Math.Round(((decimal)reserveSuccessCnt / res.Count), 2) * 100;
                    P_Order_Count_Amount_View p_Order_Count = new P_Order_Count_Amount_View
                    {
                        Name = "预定成功：\n" + proportionCount.ToString() + "%\n(" + reserveSuccessCnt + "单)",
                        OrderCount = reserveSuccessCnt
                    };
                    countChartList.Add(p_Order_Count);
                    //预定失败
                    proportionCount = Math.Round(((decimal)reserveFailCnt / res.Count), 2) * 100;
                    P_Order_Count_Amount_View p_Order_Count1 = new P_Order_Count_Amount_View
                    {
                        Name = "预定失败：\n" + proportionCount.ToString() + "%\n(" + reserveFailCnt + "单)",
                        OrderCount = reserveFailCnt
                    };
                    countChartList.Add(p_Order_Count1);
                    //退单成功
                    proportionCount = Math.Round(((decimal)returnSuccessCnt / res.Count), 2) * 100;
                    P_Order_Count_Amount_View p_Order_Count2 = new P_Order_Count_Amount_View
                    {
                        Name = "退单成功：\n" + proportionCount.ToString() + "%\n(" + returnSuccessCnt + "单)",
                        OrderCount = returnSuccessCnt
                    };
                    countChartList.Add(p_Order_Count2);
                    //退单失败
                    proportionCount = Math.Round(((decimal)returnFailCnt / res.Count), 2) * 100;
                    P_Order_Count_Amount_View p_Order_Count3 = new P_Order_Count_Amount_View
                    {
                        Name = "退单失败：\n" + proportionCount.ToString() + "%\n(" + returnFailCnt + "单)",
                        OrderCount = returnFailCnt
                    };
                    countChartList.Add(p_Order_Count3);
                    #endregion

                    //折叠面板title数据
                    var tabTitleList = (from a in res
                                        group a by new { a.DMTerritoryCode, a.DMName, a.OrderState } into b
                                        select new
                                        {
                                            b.Key.DMTerritoryCode,
                                            b.Key.DMName,
                                            b.Key.OrderState,
                                            OrderAmount = b.Sum(c => c.OrderAmount),
                                            OrderCount = b.Count()
                                        }).ToList();
                    var reserveSuccess = tabTitleList.Where(c => c.OrderState.Equals("预定成功")).ToList();
                    var reserveFail = tabTitleList.Where(c => c.OrderState.Equals("预定失败")).ToList();
                    var returnSuccess = tabTitleList.Where(c => c.OrderState.Equals("退单成功")).ToList();
                    var returnFail = tabTitleList.Where(c => c.OrderState.Equals("退单失败")).ToList();

                    List<P_ORDER_TAB_VIEW> reserveSuccessList = new List<P_ORDER_TAB_VIEW>();
                    List<P_ORDER_TAB_VIEW> reserveFailList = new List<P_ORDER_TAB_VIEW>();
                    List<P_ORDER_TAB_VIEW> returnSuccessList = new List<P_ORDER_TAB_VIEW>();
                    List<P_ORDER_TAB_VIEW> returnFailList = new List<P_ORDER_TAB_VIEW>();
                    #region 预定成功
                    foreach (var item in reserveSuccess)
                    {
                        List<P_ORDER_DOWN_VIEW> downList = new List<P_ORDER_DOWN_VIEW>();
                        if (position.ToUpper() == "BU" || position.ToUpper() == "RM")
                        {
                            //折叠面板内包含自己的下单数据
                            var dmDataList = res.Where(c => c.OrderState.Equals("预定成功") && c.MRTerritoryCode == item.DMTerritoryCode)
                                .GroupBy(c => new { c.DMName, c.DMTerritoryCode, c.OrderState })
                                .Select(b => new
                                {
                                    b.Key.DMName,
                                    b.Key.DMTerritoryCode,
                                    b.Key.OrderState,
                                    OrderAmount = b.Sum(c => c.OrderAmount),
                                    OrderCount = b.Count()
                                }).ToList();
                            foreach (var dmData in dmDataList)
                            {
                                P_ORDER_DOWN_VIEW downView = new P_ORDER_DOWN_VIEW
                                {
                                    Name = dmData.DMName,
                                    TerritoryCode = dmData.DMTerritoryCode,
                                    OrderState = dmData.OrderState,
                                    OrderAmount = dmData.OrderAmount,
                                    OrderCount = dmData.OrderCount
                                };
                                downList.Add(downView);
                            }
                            var mrDataList = res.Where(c => c.OrderState.Equals("预定成功") && c.DMTerritoryCode == item.DMTerritoryCode && c.MRTerritoryCode != item.DMTerritoryCode)
                                .GroupBy(c => new { c.MRName, c.MRTerritoryCode, c.OrderState })
                                .Select(b => new
                                {
                                    b.Key.MRName,
                                    b.Key.MRTerritoryCode,
                                    b.Key.OrderState,
                                    OrderAmount = b.Sum(c => c.OrderAmount),
                                    OrderCount = b.Count()
                                }).ToList();
                            foreach (var mrData in mrDataList)
                            {
                                P_ORDER_DOWN_VIEW downView = new P_ORDER_DOWN_VIEW
                                {
                                    Name = mrData.MRName,
                                    TerritoryCode = mrData.MRTerritoryCode,
                                    OrderState = mrData.OrderState,
                                    OrderAmount = mrData.OrderAmount,
                                    OrderCount = mrData.OrderCount
                                };
                                downList.Add(downView);
                            }
                        }

                        P_ORDER_TAB_VIEW tab_VIEW = new P_ORDER_TAB_VIEW
                        {
                            Name = item.DMName,
                            TerritoryCode = item.DMTerritoryCode,
                            OrderState = item.OrderState,
                            OrderAmount = item.OrderAmount,
                            OrderCount = item.OrderCount,
                            DownList = downList
                        };
                        reserveSuccessList.Add(tab_VIEW);
                    }
                    #endregion
                    #region 预定失败
                    foreach (var item in reserveFail)
                    {
                        List<P_ORDER_DOWN_VIEW> downList = new List<P_ORDER_DOWN_VIEW>();
                        if (position.ToUpper() == "BU" || position.ToUpper() == "RM")
                        {
                            //折叠面板内包含自己的下单数据
                            var dmDataList = res.Where(c => c.OrderState.Equals("预定失败") && c.MRTerritoryCode == item.DMTerritoryCode)
                            .GroupBy(c => new { c.DMName, c.DMTerritoryCode, c.OrderState })
                            .Select(b => new
                            {
                                b.Key.DMName,
                                b.Key.DMTerritoryCode,
                                b.Key.OrderState,
                                OrderAmount = b.Sum(c => c.OrderAmount),
                                OrderCount = b.Count()
                            }).ToList();
                            foreach (var dmData in dmDataList)
                            {
                                P_ORDER_DOWN_VIEW downView = new P_ORDER_DOWN_VIEW
                                {
                                    Name = dmData.DMName,
                                    TerritoryCode = dmData.DMTerritoryCode,
                                    OrderState = dmData.OrderState,
                                    OrderAmount = dmData.OrderAmount,
                                    OrderCount = dmData.OrderCount
                                };
                                downList.Add(downView);
                            }
                            var mrDataList = res.Where(c => c.OrderState.Equals("预定失败") && c.DMTerritoryCode == item.DMTerritoryCode && c.MRTerritoryCode != item.DMTerritoryCode)
                                .GroupBy(c => new { c.MRName, c.MRTerritoryCode, c.OrderState })
                                .Select(b => new
                                {
                                    b.Key.MRName,
                                    b.Key.MRTerritoryCode,
                                    b.Key.OrderState,
                                    OrderAmount = b.Sum(c => c.OrderAmount),
                                    OrderCount = b.Count()
                                }).ToList();
                            foreach (var mrData in mrDataList)
                            {
                                P_ORDER_DOWN_VIEW downView = new P_ORDER_DOWN_VIEW
                                {
                                    Name = mrData.MRName,
                                    TerritoryCode = mrData.MRTerritoryCode,
                                    OrderState = mrData.OrderState,
                                    OrderAmount = mrData.OrderAmount,
                                    OrderCount = mrData.OrderCount
                                };
                                downList.Add(downView);
                            }
                        }
                        P_ORDER_TAB_VIEW tab_VIEW = new P_ORDER_TAB_VIEW
                        {
                            Name = item.DMName,
                            TerritoryCode = item.DMTerritoryCode,
                            OrderState = item.OrderState,
                            OrderAmount = item.OrderAmount,
                            OrderCount = item.OrderCount,
                            DownList = downList
                        };
                        reserveFailList.Add(tab_VIEW);
                    }
                    #endregion
                    #region 退单成功
                    foreach (var item in returnSuccess)
                    {
                        List<P_ORDER_DOWN_VIEW> downList = new List<P_ORDER_DOWN_VIEW>();
                        if (position.ToUpper() == "BU" || position.ToUpper() == "RM")
                        {
                            //折叠面板内包含自己的下单数据
                            var dmDataList = res.Where(c => c.OrderState.Equals("退单成功") && c.MRTerritoryCode == item.DMTerritoryCode)
                            .GroupBy(c => new { c.DMName, c.DMTerritoryCode, c.OrderState })
                            .Select(b => new
                            {
                                b.Key.DMName,
                                b.Key.DMTerritoryCode,
                                b.Key.OrderState,
                                OrderAmount = b.Sum(c => c.OrderAmount),
                                OrderCount = b.Count()
                            }).ToList();
                            foreach (var dmData in dmDataList)
                            {
                                P_ORDER_DOWN_VIEW downView = new P_ORDER_DOWN_VIEW
                                {
                                    Name = dmData.DMName,
                                    TerritoryCode = dmData.DMTerritoryCode,
                                    OrderState = dmData.OrderState,
                                    OrderAmount = dmData.OrderAmount,
                                    OrderCount = dmData.OrderCount
                                };
                                downList.Add(downView);
                            }
                            var mrDataList = res.Where(c => c.OrderState.Equals("退单成功") && c.DMTerritoryCode == item.DMTerritoryCode && c.MRTerritoryCode != item.DMTerritoryCode)
                                .GroupBy(c => new { c.MRName, c.MRTerritoryCode, c.OrderState })
                                .Select(b => new
                                {
                                    b.Key.MRName,
                                    b.Key.MRTerritoryCode,
                                    b.Key.OrderState,
                                    OrderAmount = b.Sum(c => c.OrderAmount),
                                    OrderCount = b.Count()
                                }).ToList();
                            foreach (var mrData in mrDataList)
                            {
                                P_ORDER_DOWN_VIEW downView = new P_ORDER_DOWN_VIEW
                                {
                                    Name = mrData.MRName,
                                    TerritoryCode = mrData.MRTerritoryCode,
                                    OrderState = mrData.OrderState,
                                    OrderAmount = mrData.OrderAmount,
                                    OrderCount = mrData.OrderCount
                                };
                                downList.Add(downView);
                            }
                        }

                        P_ORDER_TAB_VIEW tab_VIEW = new P_ORDER_TAB_VIEW
                        {
                            Name = item.DMName,
                            TerritoryCode = item.DMTerritoryCode,
                            OrderState = item.OrderState,
                            OrderAmount = item.OrderAmount,
                            OrderCount = item.OrderCount,
                            DownList = downList
                        };
                        returnSuccessList.Add(tab_VIEW);
                    }
                    #endregion
                    #region 退单失败
                    foreach (var item in returnFail)
                    {
                        List<P_ORDER_DOWN_VIEW> downList = new List<P_ORDER_DOWN_VIEW>();
                        if (position.ToUpper() == "BU" || position.ToUpper() == "RM")
                        {
                            //折叠面板内包含自己的下单数据
                            var dmDataList = res.Where(c => c.OrderState.Equals("退单失败") && c.MRTerritoryCode == item.DMTerritoryCode)
                            .GroupBy(c => new { c.DMName, c.DMTerritoryCode, c.OrderState })
                            .Select(b => new
                            {
                                b.Key.DMName,
                                b.Key.DMTerritoryCode,
                                b.Key.OrderState,
                                OrderAmount = b.Sum(c => c.OrderAmount),
                                OrderCount = b.Count()
                            }).ToList();
                            foreach (var dmData in dmDataList)
                            {
                                P_ORDER_DOWN_VIEW downView = new P_ORDER_DOWN_VIEW
                                {
                                    Name = dmData.DMName,
                                    TerritoryCode = dmData.DMTerritoryCode,
                                    OrderState = dmData.OrderState,
                                    OrderAmount = dmData.OrderAmount,
                                    OrderCount = dmData.OrderCount
                                };
                                downList.Add(downView);
                            }
                            var mrDataList = res.Where(c => c.OrderState.Equals("退单失败") && c.DMTerritoryCode == item.DMTerritoryCode && c.MRTerritoryCode != item.DMTerritoryCode)
                                .GroupBy(c => new { c.MRName, c.MRTerritoryCode, c.OrderState })
                                .Select(b => new
                                {
                                    b.Key.MRName,
                                    b.Key.MRTerritoryCode,
                                    b.Key.OrderState,
                                    OrderAmount = b.Sum(c => c.OrderAmount),
                                    OrderCount = b.Count()
                                }).ToList();
                            foreach (var mrData in mrDataList)
                            {
                                P_ORDER_DOWN_VIEW downView = new P_ORDER_DOWN_VIEW
                                {
                                    Name = mrData.MRName,
                                    TerritoryCode = mrData.MRTerritoryCode,
                                    OrderState = mrData.OrderState,
                                    OrderAmount = mrData.OrderAmount,
                                    OrderCount = mrData.OrderCount
                                };
                                downList.Add(downView);
                            }
                        }

                        P_ORDER_TAB_VIEW tab_VIEW = new P_ORDER_TAB_VIEW
                        {
                            Name = item.DMName,
                            TerritoryCode = item.DMTerritoryCode,
                            OrderState = item.OrderState,
                            OrderAmount = item.OrderAmount,
                            OrderCount = item.OrderCount,
                            DownList = downList
                        };
                        returnFailList.Add(tab_VIEW);
                    }
                    #endregion

                    return Json(new
                    {
                        state = 1,
                        ReserveSuccessCount = reserveSuccessCnt,
                        ReserveFailCount = reserveFailCnt,
                        ReturnSuccessCount = returnSuccessCnt,
                        ReturnFailCount = returnFailCnt,
                        ReserveSuccessList = reserveSuccessList,
                        ReserveFailList = reserveFailList,
                        ReturnSuccessList = returnSuccessList,
                        ReturnFailList = returnFailList,
                        TotalCount = res.Count,
                        dataCountChart = countChartList
                    });
                }
                else
                {
                    return Json(new { state = 0, txt = "无有效订单数据！" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("LoadUpOrderAnalysisData" + ex.Message);
                return Json(new { state = 0 });
            }

        }
        #endregion

        #region 统计汇总页面
        [iPathOAuthFilter(MappingKey = "0x0016", CallBackUrl = true)]
        [HttpGet]
        public ActionResult CostSummary()
        {
            var TerritoryCode = CurrentWxUser.TerritoryCode;
            List<string> TerritoryCodeList = new List<string>();
            string TerritoryCodeForRole = TerritoryCode;

            if (CurrentWxUser.Market == "DDT" || CurrentWxUser.Market == "TSKF")
            {
                return View("CostError");
            }

            #region 争议代码
            //if (TerritoryCode != null && TerritoryCode != "" && TerritoryCode.IndexOf("_RM") > -1)
            //{
            //    CurrentWxUser.Role = "RM";
            //}
            //else if (TerritoryCode != null && TerritoryCode != "" && TerritoryCode.IndexOf("_SD") > -1)
            //{
            //    CurrentWxUser.Role = "RD";
            //}
            //else
            //{
            //    var UserId = CurrentWxUser.UserId;
            //    var channel = BUManagementClientChannelFactory.GetChannel();

            //    var BUINFO = channel.GetBUInfoByUserId(UserId);

            //    if (BUINFO != null && BUINFO.ID != null)
            //    {
            //        CurrentWxUser.Role = "BU";
            //        CurrentWxUser.TerritoryCode = BUINFO.ID.ToString();
            //        TerritoryCodeForRole = BUINFO.ID.ToString();
            //    }
            //    else
            //    {
            //        var TAINFO = channel.GetTAInfoByUserId(UserId);
            //        if (TAINFO != null && TAINFO.ID != null)
            //        {
            //            CurrentWxUser.Role = "TA";
            //            TerritoryCodeForRole = TAINFO.TerritoryTA;
            //            CurrentWxUser.TerritoryCode = TAINFO.TerritoryTA;
            //        }
            //        else
            //        {
            //            CurrentWxUser.Role = "";
            //            return View("CostError");
            //        }
            //    }

            //}
            #endregion

            var UserId = CurrentWxUser.UserId;
            var channel = BUManagementClientChannelFactory.GetChannel();
            var userChannel = UserInfoClientChannelFactory.GetChannel();

            var BUINFO = channel.GetBUInfoByUserId(UserId);

            if (BUINFO != null && BUINFO.ID != null)
            {
                CurrentWxUser.Role = "BU";
                CurrentWxUser.TerritoryCode = BUINFO.ID.ToString();
                TerritoryCodeForRole = BUINFO.ID.ToString();
                goto GoOn;
            }
            else
            {
                var TAINFO = channel.GetTAInfoByUserId(UserId);
                if (TAINFO != null && TAINFO.ID != null)
                {
                    CurrentWxUser.Role = "TA";
                    TerritoryCodeForRole = TAINFO.TerritoryTA;
                    CurrentWxUser.TerritoryCode = TAINFO.TerritoryTA;
                    goto GoOn;
                }
                else
                {
                    var roleCountRD = userChannel.CheckUserRole("RD", UserId, CurrentWxUser.Market);

                    if (roleCountRD > 0)
                    {
                        CurrentWxUser.Role = "RD";
                        goto GoOn;
                    }
                    else
                    {
                        var roleCountRM = userChannel.CheckUserRole("RM", UserId, CurrentWxUser.Market);

                        if (roleCountRM > 0)
                        {
                            CurrentWxUser.Role = "RM";
                            goto GoOn;
                        }
                        else
                        {
                            CurrentWxUser.Role = "";
                            return View("CostError");
                        }
                    }
                }
            }


        GoOn: if (CurrentWxUser.Role != "")

            {
                TerritoryCodeList = GetTerritoryList(CurrentWxUser.Role, TerritoryCodeForRole);
            }
            else
            {
                TerritoryCodeList.Add(CurrentWxUser.TerritoryCode);
            }

            var NowDate = DateTime.Now;

            string StartDate = "";
            string EndDate = "";
            if (NowDate.Day == 1)
            {
                StartDate = DateTime.Now.AddMonths(-1).ToString("yyyy-MM") + "-01";
                EndDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            }
            else
            {
                StartDate = DateTime.Now.ToString("yyyy-MM") + "-01";
                EndDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            }


            ViewBag.StartDate = StartDate;
            ViewBag.EndDate = EndDate;
            ViewBag.Role = CurrentWxUser.Role;

            ViewBag.CostSummary = GetCostSummaryEntity(TerritoryCodeList, StartDate, EndDate);
            return View();
        }

        public ActionResult LoadSummaryData(string StartDate, string EndDate)
        {
            var TerritoryCode = CurrentWxUser.TerritoryCode;
            List<string> TerritoryCodeList = new List<string>();
            if (CurrentWxUser.Role != "")
            {
                TerritoryCodeList = GetTerritoryList(CurrentWxUser.Role, TerritoryCode);
            }
            else
            {
                TerritoryCodeList.Add(CurrentWxUser.TerritoryCode);
            }

            var CostSummary = GetCostSummaryEntity(TerritoryCodeList, StartDate, EndDate);

            return Json(new { state = 1, data = CostSummary });
        }


        /// <summary>
        /// 获取统计页面数据
        /// </summary>
        /// <param name="TerritoryList">TerritoryCode列表</param>
        /// <param name="StartDate">开始日期</param>
        /// <param name="EndDate">结束日期</param>
        /// <returns></returns>
        public V_COST_SUMMARY GetCostSummaryEntity(List<string> TerritoryList, string StartDate, string EndDate)
        {
            V_COST_SUMMARY costSummary = new V_COST_SUMMARY();

            var preChannel = PreApprovalClientChannelFactory.GetChannel();
            var orderChannel = OrderApiClientChannelFactory.GetChannel();

            var preList = preChannel.GetPreApprovalList(TerritoryList, StartDate, EndDate);
            var orderList = orderChannel.GetOrderList(TerritoryList, StartDate, EndDate);
            var unfinishedList = orderChannel.GetUnfinishedOrderList(TerritoryList, StartDate, EndDate);
            //var unfinishedList = new V_COST_SUMMARY();
            //unfinishedList.UnfinishedOrderApplierCount = "0";
            //unfinishedList.UnfinishedOrderCount = "0";
            var specialList = orderChannel.GetSpecialOrderList(TerritoryList, StartDate, EndDate);

            //int preCount = 0;
            //decimal prePrice = 0;
            //if (preList != null)
            //{
            //    preCount = preList.Select(p => p.HTCode).Distinct().Count();
            //    prePrice = preList.Select(p => p.BudgetTotal).Sum();
            //}

            //int orderCount = 0;
            //decimal orderPrice = 0;
            //if (orderList != null)
            //{
            //    orderCount = orderList.Select(p => p.CN).Distinct().Count();
            //    orderPrice = orderList.Select(p => p.XmsTotalPrice).Sum();
            //}

            //int unfinishedCount = 0;
            //int unfinishedApplier = 0;
            //if (unfinishedList != null)
            //{
            //    unfinishedCount = unfinishedList.Select(p => p.CN).Distinct().Count();
            //    unfinishedApplier = unfinishedList.Select(p => p.UserId).Distinct().Count();
            //}

            //int specialCount = 0;
            //int specialApplier = 0;
            //if (specialList != null)
            //{
            //    specialCount = specialList.Select(p => p.CN).Distinct().Count();
            //    specialApplier = specialList.Select(p => p.UserId).Distinct().Count();
            //}

            //costSummary.PreApprovalCount = preCount.ToString("N0");
            //costSummary.BudgetTotal = prePrice.ToString("N2");
            //costSummary.OrderCount = orderCount.ToString("N0");
            //costSummary.RealPrice = orderPrice.ToString("N2");
            //costSummary.SpecialOrderApplierCount = specialApplier.ToString("N0");
            //costSummary.SpecialOrderCount = specialCount.ToString("N0");
            //costSummary.UnfinishedOrderApplierCount = unfinishedApplier.ToString("N0");
            //costSummary.UnfinishedOrderCount = unfinishedCount.ToString("N0");

            costSummary.PreApprovalCount = preList.PreApprovalCount;
            costSummary.BudgetTotal = preList.BudgetTotal;
            costSummary.OrderCount = orderList.OrderCount;
            costSummary.RealPrice = orderList.RealPrice;
            costSummary.SpecialOrderApplierCount = specialList.SpecialOrderApplierCount;
            costSummary.SpecialOrderCount = specialList.SpecialOrderCount;
            costSummary.UnfinishedOrderApplierCount = unfinishedList.UnfinishedOrderApplierCount;
            costSummary.UnfinishedOrderCount = unfinishedList.UnfinishedOrderCount;

            return costSummary;
        }

        public List<string> GetTerritoryList(string Role, string TerritoryCode)
        {
            List<string> rtnData = new List<string>();
            var BUChannel = BUManagementClientChannelFactory.GetChannel();
            var HospitalChannel = HospitalClientChannelFactory.GetChannel();
            if (Role == "RM")
            {
                rtnData.Add(TerritoryCode);
            }
            else if (Role == "RD" || Role == "TA")
            {
                var TAList = HospitalChannel.LoadTerritoryRMList(TerritoryCode);
                var ListStr = TAList.Select(p => p.TA_CODE).Distinct();
                rtnData.AddRange(ListStr);
            }
            else
            {
                var TA = BUChannel.GetTAInfoByBUID(Guid.Parse(TerritoryCode));

                List<string> TAINFO = new List<string>();

                if (TA != null && TA.Count > 0)
                {
                    TAINFO.AddRange(TA.Select(p => p.TerritoryTA).Distinct());
                }

                var strTerritory = string.Join("','", TAINFO);

                var TAList = HospitalChannel.LoadTerritoryRMList(strTerritory);
                var ListStr = TAList.Select(p => p.TA_CODE).Distinct();
                rtnData.AddRange(ListStr);
            }

            return rtnData;
        }
        #endregion

        #region 有效预申请/订单分析
        [iPathOAuthFilter(MappingKey = "0x0006", CallBackUrl = true)]
        [HttpGet]
        public ActionResult PreOrderAnalysis()
        {
            var NowDate = DateTime.Now;
            string StartDate = "";
            string EndDate = "";
            if (NowDate.Day == 1)
            {
                StartDate = DateTime.Now.AddMonths(-1).ToString("yyyy-MM") + "-01";
                EndDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            }
            else
            {
                StartDate = DateTime.Now.ToString("yyyy-MM") + "-01";
                EndDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            }

            ViewBag.StartDate = StartDate;
            ViewBag.EndDate = EndDate;
            return View();
        }
        [HttpPost]
        public JsonResult LoadPreOrderAnalysisData(string StartDate, string EndDate, string Type)
        {
            try
            {
                List<P_Order_Count_Amount_View> orderAmountChartList = new List<P_Order_Count_Amount_View>();
                List<P_Order_Count_Amount_View> orderCountChartList = new List<P_Order_Count_Amount_View>();
                List<P_Pre_Count_Amount_View> preAmountChartList = new List<P_Pre_Count_Amount_View>();
                List<P_Pre_Count_Amount_View> preCountChartList = new List<P_Pre_Count_Amount_View>();
                List<P_PreORDER_VIEW> userRankingList = new List<P_PreORDER_VIEW>();
                List<P_PreOrder_Amount_Chart_View> barAmountList = new List<P_PreOrder_Amount_Chart_View>();
                List<P_PreOrder_Hospital_View> hospitalDataList = new List<P_PreOrder_Hospital_View>();
                var orderChannel = OrderApiClientChannelFactory.GetChannel();
                string position = "TA";
                string territoryCode = "RES_TAL_02";

                //有效订单数据
                var resOrder = orderChannel.LoadPreOrderAnalysisData(CurrentWxUser.UserId, position, territoryCode, StartDate, EndDate);

                //有预申请数据
                var resPreApproval = orderChannel.LoadPreAnalysisData(CurrentWxUser.UserId, position, territoryCode, StartDate, EndDate);

                //医院排行数据
                hospitalDataList  = orderChannel.LoadHospitalAnalysisData(CurrentWxUser.UserId, position, territoryCode, StartDate, EndDate);

                if (resOrder != null && resOrder.Count > 0 && resPreApproval != null && resPreApproval.Count > 0)
                {
                    var orderList = (from a in resOrder
                                     group a by new { a.DMTerritoryCode, a.DMName } into b
                                     select new
                                     {
                                         b.Key.DMTerritoryCode,
                                         b.Key.DMName,
                                         OrderAmount = b.Sum(c => c.OrderAmount),
                                         OrderCount = b.Sum(c => c.OrderAmount) == 0 ? 0 : b.Count()
                                     }).OrderByDescending(o => o.OrderAmount);
                    decimal totalOrderAmount = orderList.Sum(c => (decimal)c.OrderAmount);
                    int totalOrderCount = orderList.Sum(c => c.OrderCount);
                    if (totalOrderAmount != 0 || totalOrderCount != 0)
                    {
                        #region 订单饼图
                        foreach (var item in orderList)
                        {
                            if (Type == "1")
                            {
                                //金额占比
                                decimal proportionAmount = Math.Round((decimal)item.OrderAmount / totalOrderAmount, 2) * 100;
                                P_Order_Count_Amount_View p_Order_Amount = new P_Order_Count_Amount_View
                                {
                                    Name = item.DMName.ToString() + "：\n" + proportionAmount.ToString() + "%\n(" + ((decimal)item.OrderAmount).ToString("C").Replace('$', '¥') + ")",
                                    OrderAmount = (decimal)item.OrderAmount
                                };
                                orderAmountChartList.Add(p_Order_Amount);
                            }
                            else
                            {
                                //数量占比
                                decimal proportionCount = Math.Round((decimal)item.OrderCount / totalOrderCount, 2) * 100;
                                P_Order_Count_Amount_View p_Order_Count = new P_Order_Count_Amount_View
                                {
                                    Name = item.DMName.ToString() + "：\n" + proportionCount.ToString() + "%\n(" + item.OrderCount.ToString("N").Substring(0, item.OrderCount.ToString("N").Length - 3) + "单)",
                                    OrderCount = item.OrderCount
                                };
                                orderCountChartList.Add(p_Order_Count);
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        return Json(new { state = 0, txt = "无有效订单数据！" });
                    }

                    var preList = (from a in resPreApproval
                                   group a by new { a.DMTerritoryCode, a.DMName } into b
                                     select new
                                     {
                                         b.Key.DMTerritoryCode,
                                         b.Key.DMName,
                                         PreAmount = b.Sum(c => c.PreAmount),
                                         PreCount = b.Sum(c => c.PreAmount) == 0 ? 0 : b.Count()
                                     }).OrderByDescending(o => o.PreAmount);
                    decimal totalPreAmount = preList.Sum(c => (decimal)c.PreAmount);
                    int totalPreCount = preList.Sum(c => c.PreCount);
                    if (totalPreAmount != 0 || totalPreCount != 0)
                    {
                        #region 预申请饼图
                        foreach (var item in preList)
                        {
                            if (Type == "1")
                            {
                                //金额占比
                                decimal proportionAmount = Math.Round(((decimal)item.PreAmount / totalOrderAmount), 2) * 100;
                                P_Pre_Count_Amount_View p_Order_Amount = new P_Pre_Count_Amount_View
                                {
                                    Name = item.DMName.ToString() + "：\n" + proportionAmount.ToString() + "%\n(" + ((decimal)item.PreAmount).ToString("C").Replace('$', '¥') + ")",
                                    PreAmount = (decimal)item.PreAmount
                                };
                                preAmountChartList.Add(p_Order_Amount);
                            }
                            else
                            {
                                //数量占比
                                decimal proportionCount = Math.Round(((decimal)item.PreCount / totalOrderCount), 2) * 100;
                                P_Pre_Count_Amount_View p_Order_Count = new P_Pre_Count_Amount_View
                                {
                                    Name = item.DMName.ToString() + "：\n" + proportionCount.ToString() + "%\n(" + item.PreCount.ToString("N").Substring(0, item.PreCount.ToString("N").Length - 3) + "单)",
                                    PreCount = item.PreCount
                                };
                                preCountChartList.Add(p_Order_Count);
                            }
                        }
                    }
                    else
                    {
                        return Json(new { state = 0, txt = "无有效订单数据！" });
                    }
                    #endregion

                    #region 人员排行及数据
                    foreach (var item in orderList)
                    {
                        List<P_PreOrder_DOWN_VIEW> downList = new List<P_PreOrder_DOWN_VIEW>();
                        if (position.ToUpper() == "BU" || position.ToUpper() == "RM")
                        {
                            //展开折叠面板后，第一条显示DM自己的下单/预申请数据
                            #region 折叠面板内DM数据
                            var dmDataList = resOrder.Where(c => c.MRTerritoryCode == item.DMTerritoryCode)
                                .GroupBy(c => new { c.DMName, c.DMTerritoryCode })
                                .Select(b => new
                                {
                                    b.Key.DMName,
                                    b.Key.DMTerritoryCode,
                                    OrderAmount = b.Sum(c => c.OrderAmount),
                                    OrderCount = b.Count()
                                }).ToList();
                            var dmPreDataList = resPreApproval.Where(c => c.MRTerritoryCode == item.DMTerritoryCode)
                                .GroupBy(c => new { c.DMName, c.DMTerritoryCode })
                                .Select(b => new
                                {
                                    b.Key.DMName,
                                    b.Key.DMTerritoryCode,
                                    PreAmount = b.Sum(c => c.PreAmount),
                                    PreCount = b.Count()
                                }).ToList();
                            foreach (var dmData in dmDataList)
                            {
                                var dp = dmPreDataList.FirstOrDefault(c => c.DMTerritoryCode.Equals(dmData.DMTerritoryCode));
                                P_PreOrder_DOWN_VIEW downView = new P_PreOrder_DOWN_VIEW
                                {
                                    Name = dmData.DMName,
                                    TerritoryCode = dmData.DMTerritoryCode,
                                    OrderAmount = (decimal)dmData.OrderAmount,
                                    OrderCount = dmData.OrderCount,
                                    PreAmount = (decimal)dp.PreAmount,
                                    PreCount = dp.PreCount
                                };
                                downList.Add(downView);
                            }
                            #endregion
                            #region 折叠面板内MR数据
                            var mrDataList = resOrder.Where(c => c.DMTerritoryCode == item.DMTerritoryCode && c.MRTerritoryCode != item.DMTerritoryCode)
                                .GroupBy(c => new { c.MRName, c.MRTerritoryCode })
                                .Select(b => new
                                {
                                    b.Key.MRName,
                                    b.Key.MRTerritoryCode,
                                    OrderAmount = b.Sum(c => c.OrderAmount),
                                    OrderCount = b.Sum(c => c.OrderAmount) == 0 ? 0 : b.Count()
                                }).ToList();
                            var mrPreDataList = resPreApproval.Where(c => c.DMTerritoryCode == item.DMTerritoryCode && c.MRTerritoryCode != item.DMTerritoryCode)
                                .GroupBy(c => new { c.MRName, c.MRTerritoryCode })
                                .Select(b => new
                                {
                                    b.Key.MRName,
                                    b.Key.MRTerritoryCode,
                                    PreAmount = b.Sum(c => c.PreAmount),
                                    PreCount = b.Sum(c => c.PreAmount) == 0 ? 0 : b.Count()
                                }).ToList();
                            foreach (var mrData in mrDataList)
                            {
                                if (mrData.MRTerritoryCode == null)
                                    continue;
                                var dp = mrPreDataList.FirstOrDefault(c => c.MRTerritoryCode.Equals(mrData.MRTerritoryCode));
                                P_PreOrder_DOWN_VIEW downView = new P_PreOrder_DOWN_VIEW
                                {
                                    Name = mrData.MRName,
                                    TerritoryCode = mrData.MRTerritoryCode,
                                    OrderAmount = (decimal)mrData.OrderAmount,
                                    OrderCount = mrData.OrderCount,
                                    PreAmount = (decimal)dp.PreAmount,
                                    PreCount = dp.PreCount
                                };
                                downList.Add(downView);
                            }
                            #endregion
                        }
                        var dpv = preList.FirstOrDefault(c => c.DMTerritoryCode.Equals(item.DMTerritoryCode));
                        P_PreORDER_VIEW preORDER_VIEW = new P_PreORDER_VIEW
                        {
                            Name = item.DMName,
                            TerritoryCode = item.DMTerritoryCode,
                            OrderAmount = (decimal)item.OrderAmount,
                            OrderCount = item.OrderCount,
                            PreAmount = (decimal)dpv.PreAmount,
                            PreCount = dpv.PreCount,
                            DownList = downList
                        };
                        userRankingList.Add(preORDER_VIEW);
                        P_PreOrder_Amount_Chart_View chart = new P_PreOrder_Amount_Chart_View
                        {
                            CodeandNAME = item.DMName.ToString(),
                            OrderAmount = (decimal)item.OrderAmount,
                            PreAmount = (decimal)dpv.PreAmount
                        };
                        barAmountList.Add(chart);
                    }
                    #endregion

                    #region 医院排行及数据
                    #endregion

                    return Json(new
                    {
                        state = 1,
                        orderAmountChart = orderAmountChartList,
                        preAmountChart = preAmountChartList,
                        TotalOrderAmount = totalOrderAmount.ToString("C").Replace('$', '¥'),
                        TotalPreAmount = totalPreAmount.ToString("C").Replace('$', '¥'),

                        preCountChart = preCountChartList,
                        orderCountChart = orderCountChartList,
                        TotalOrderCount = totalOrderCount.ToString("N").Substring(0, totalOrderCount.ToString("N").Length - 3),
                        TotalPreCount = totalPreCount.ToString("N").Substring(0, totalPreCount.ToString("N").Length - 3),

                        BarAmountChart = barAmountList,

                        UserRankingList = userRankingList,
                        HospitalRankingList = hospitalDataList.Take(20),
                        //TotalHospitalOrderAmount = hospitalDataList.Sum(c=>c.OrderAmount).ToString("C").Replace('$', '¥'),
                        //TotalHospitalPreAmount = hospitalDataList.Sum(c => c.PreAmount).ToString("C").Replace('$', '¥'),
                        TotalHospitalCount = hospitalDataList.Count.ToString("N").Substring(0, hospitalDataList.Count.ToString("N").Length - 3),
                        TotalHospitalPreCount = hospitalDataList.Where(c => c.HosPre != null).Sum(c => c.PreCount).ToString("N").Substring(0, hospitalDataList.Where(c => c.HosPre != null).Sum(c => c.PreCount).ToString("N").Length - 3),
                        TotalHospitalOrderCount = hospitalDataList.Where(c => c.HosOrder != null).Sum(c => c.OrderCount).ToString("N").Substring(0, hospitalDataList.Where(c => c.HosOrder != null).Sum(c => c.OrderCount).ToString("N").Length - 3),
                        TotalPreProportion = "占比：" + Math.Round((hospitalDataList.Where(c => c.HosPre != null).Count() / (decimal)hospitalDataList.Count), 4).ToString("0.00%"),
                        TotalOrderProportion = "占比：" + Math.Round((hospitalDataList.Where(c => c.HosOrder != null).Count() / (decimal)hospitalDataList.Count), 4).ToString("0.00%")
                        //ReserveSuccessCount = reserveSuccessCnt,
                        //ReserveFailCount = reserveFailCnt,
                        //ReturnSuccessCount = returnSuccessCnt,
                        //ReturnFailCount = returnFailCnt,
                        //ReserveSuccessList = reserveSuccessList,
                        //ReserveFailList = reserveFailList,
                        //ReturnSuccessList = returnSuccessList,
                        //ReturnFailList = returnFailList,
                        //TotalCount = res.Count,
                        //dataCountChart = countChartList
                    });
                }
                else
                {
                    return Json(new { state = 0, txt = "无有效订单数据！" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("LoadUpOrderAnalysisData" + ex.Message);
                return Json(new { state = 0 });
            }
        }
        #endregion
    }
}