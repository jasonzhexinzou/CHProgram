using MealAdmin.Entity;
using MealAdmin.Entity.View;
using MealAdmin.Service;
using MealAdmin.Web.Filter;
using MeetingMealApiClient;
using MeetingMealEntity;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XFramework.XInject.Attributes;
using NPOI.XSSF.UserModel;

namespace MealAdmin.Web.Areas.P.Controllers
{

    public class RestaurantController : Controller
    {

        [Bean("restaurantService")]
        public IRestaurantService restaurantService { get; set; }

        #region 获取所有的可送餐列表
        /// <summary>
        /// 获取所有的可送餐列表
        /// </summary>
        /// <param name="provice"></param>
        /// <param name="city"></param>
        /// <returns></returns>
        public JsonResult SyncHospitalRes(string provice, string city)
        {
            // 获取当前时间

            var time = DateTime.Now;

            var channel = OpenApiChannelFactory.GetChannel();
            // XMS 获取的可送餐列表
            var res = channel.SyncHospitalRes(provice, city);
            // BDS 获取的可送餐列表
            var resBDS = channel.SyncHospitalResBDS(provice, city);

            // 整理数据
            var resList = new List<GetSyncHospitalResResult>();
            if (res.result != null && res.result.Count > 0)
            {
                resList = res.result;
            }

            var resBDSList = new List<GetSyncHospitalResResult>();
            if (resBDS.result != null && resBDS.result.Count > 0)
            {
                resBDSList = resBDS.result;
            }

            var hospitalRangeList = new List<P_HOSPITAL_RANGE_RESTAURANT>();
            var hospitalRangeCount = new List<P_HOSPITAL_RANGE_RESTAURANTCOUNT>();

            // 整理XMS返回的信息
            foreach (var oneHospitalRange in resList)
            {
                var ID = Guid.NewGuid();
                var dataSources = "XMS";
                var gskHospitalTemp = oneHospitalRange.gskHospital;

                var resCount = oneHospitalRange.resCount;

                hospitalRangeCount.Add(new P_HOSPITAL_RANGE_RESTAURANTCOUNT
                {
                    ID = Guid.NewGuid(),
                    DataSources = dataSources,
                    GskHospital = gskHospitalTemp,
                    TotalCount = resCount.totalCount,
                    BreakfastCount = resCount.breakfastCount,
                    LunchCount = resCount.lunchCount,
                    TeaCount = resCount.teaCount,
                    CreateDate = time
                });


                foreach (var oneRes in oneHospitalRange.resList)
                {
                    hospitalRangeList.Add(new P_HOSPITAL_RANGE_RESTAURANT
                    {
                        ID = Guid.NewGuid(),
                        DataSources = dataSources,
                        GskHospital = gskHospitalTemp,
                        ResId = oneRes.resId,
                        ResName = oneRes.resName,
                        CreateDate = time
                    });
                }
            }

            // 整理BDS返回的信息
            foreach (var oneHospitalRange in resBDSList)
            {
                var ID = Guid.NewGuid();
                var dataSources = "BDS";
                var gskHospitalTemp = oneHospitalRange.gskHospital;

                var resCount = oneHospitalRange.resCount;

                hospitalRangeCount.Add(new P_HOSPITAL_RANGE_RESTAURANTCOUNT
                {
                    ID = Guid.NewGuid(),
                    DataSources = dataSources,
                    GskHospital = gskHospitalTemp,
                    TotalCount = resCount.totalCount,
                    BreakfastCount = resCount.breakfastCount,
                    LunchCount = resCount.lunchCount,
                    TeaCount = resCount.teaCount,
                    CreateDate = time
                });

                foreach (var oneRes in oneHospitalRange.resList)
                {
                    hospitalRangeList.Add(new P_HOSPITAL_RANGE_RESTAURANT
                    {
                        ID = Guid.NewGuid(),
                        DataSources = dataSources,
                        GskHospital = gskHospitalTemp,
                        ResId = oneRes.resId,
                        ResName = oneRes.resName,
                        CreateDate = time
                    });
                }
            }

            // 写入数据
            int succCount = restaurantService.ImportRangeRestaurant(hospitalRangeList, hospitalRangeCount);

            return Json(new { state = 1, succCount });
        }
        #endregion

        #region 获取餐厅列表
        /// <summary>
        /// 获取餐厅列表
        /// </summary>
        /// <param name="provice"></param>
        /// <param name="city"></param>
        /// <returns></returns>
        public JsonResult SyncRes(string provice, string city)
        {
            var channel = OpenApiChannelFactory.GetChannel();

            // 获取当前时间
            var time = DateTime.Now;

            // XMS 获取的餐厅
            var res = channel.SyncRes(provice, city);
            // BDS 获取的餐厅
            var resBDS = channel.SyncResBDS(provice, city);

            var totleCount = 0;
            var resList = new List<GetSyncResResult>();
            if (res.result != null && res.result.Count > 0)
            {
                totleCount = res.result.Count;
                resList = res.result;
            }

            var resBDSList = new List<GetSyncResResult>();
            if (resBDS.result != null && resBDS.result.Count > 0)
            {
                totleCount = totleCount + resBDS.result.Count;
                resBDSList = resBDS.result;
            }

            var restaurantList = new List<P_RESTAURANT_LIST>();

            // 整理XMS返回的信息
            foreach (var oneRes in resList)
            {
                restaurantList.Add(new P_RESTAURANT_LIST
                {
                    ID = Guid.NewGuid(),
                    DataSources = "XMS",
                    ResId = oneRes.resId,
                    ResName = oneRes.resName,
                    ResType = oneRes.resType,
                    Province = oneRes.province,
                    City = oneRes.city,
                    CreateDate = time
                });
            }

            // 整理BDS返回的信息
            foreach (var oneRes in resBDSList)
            {
                restaurantList.Add(new P_RESTAURANT_LIST
                {
                    ID = Guid.NewGuid(),
                    DataSources = "BDS",
                    ResId = oneRes.resId,
                    ResName = oneRes.resName,
                    ResType = oneRes.resType,
                    Province = oneRes.province,
                    City = oneRes.city,
                    CreateDate = time
                });
            }

            // 放入数据
            int insCount = restaurantService.Import(restaurantList);

            return Json(new { state = 1, successCount = insCount, totleCount });
        }
        #endregion

        #region 导出区域关系
        /// <summary>
        /// 导出可送餐列表
        /// </summary>
        /// <returns></returns>
        //[PermissionFilter(Order = 2, Permission = "00000000-0000-0000-2000-000000000001")]
        //[OperationAuditFilter(Operation = "导出可送餐列表", OperationAuditTypeName = "导出可送餐列表")]
        public void ReportAreaExcel()
        {
            // 获取餐厅列表
            var restaurantList = restaurantService.QryAllArea();

            var titile = "医院代码,省份,城市,医院名称,医院地址,是否为主地址,Market,经度,纬度,区县,区县编码,客户默认性质,MUD_ID_MR,TerritoryCode_MR,MUD_ID_DM,TerritoryCode_DM,MUD_ID_RM,TerritoryCode_RM,MUD_ID_RD,TerritoryCodeRD,MUD_ID_TA,TERRITORY_TA".Split(',').ToList();

            XSSFWorkbook wk = new XSSFWorkbook();
            ISheet sheet = wk.CreateSheet("区域关系列表");
            IRow row = sheet.CreateRow(0);
            sheet.CreateFreezePane(0, 1, 0, 1);

            //ICellStyle style = wk.CreateCellStyle();
            //style.WrapText = true;
            //style.Alignment = HorizontalAlignment.Center;
            //IFont font = wk.CreateFont();
            //font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
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

            for (var i = 0; i < titile.Count; i++)
            {
                sheet.SetColumnWidth(i, 20 * 256);
            }

            for (var i = 0; i < titile.Count; i++)
            {
                ICell cell = row.CreateCell(i);
                cell.SetCellValue(titile[i]);
                cell.SetCellType(CellType.String);
                cell.CellStyle = style;
            }

            #region 组装Excel文档
            for (var i = 0; i < restaurantList.Count; i++)
            {
                //var y = -1;
                row = sheet.CreateRow(i + 1);
                ICell cell = null;

                cell = row.CreateCell(0);
                cell.SetCellValue(restaurantList[i].HospitalCode);
                cell = row.CreateCell(1);
                cell.SetCellValue(restaurantList[i].Province);
                cell = row.CreateCell(2);
                cell.SetCellValue(restaurantList[i].City);
                cell = row.CreateCell(3);
                cell.SetCellValue(restaurantList[i].HospitalName);
                cell = row.CreateCell(4);
                cell.SetCellValue(restaurantList[i].Address);
                cell = row.CreateCell(5);
                cell.SetCellValue(restaurantList[i].MainAddress);
                cell = row.CreateCell(6);
                cell.SetCellValue(restaurantList[i].Market);
                cell = row.CreateCell(7);
                cell.SetCellValue(restaurantList[i].Latitude);
                cell = row.CreateCell(8);
                cell.SetCellValue(restaurantList[i].Longitude);
                cell = row.CreateCell(9);
                cell.SetCellValue(restaurantList[i].District);
                cell = row.CreateCell(10);
                cell.SetCellValue(restaurantList[i].DistrictCode);
                cell = row.CreateCell(11);
                cell.SetCellValue(restaurantList[i].CustomerType);
                cell = row.CreateCell(12);
                cell.SetCellValue(restaurantList[i].MUD_ID_MR);
                cell = row.CreateCell(13);
                cell.SetCellValue(restaurantList[i].TERRITORY_MR);
                cell = row.CreateCell(14);
                cell.SetCellValue(restaurantList[i].MUD_ID_DM);
                cell = row.CreateCell(15);
                cell.SetCellValue(restaurantList[i].TERRITORY_DM);
                cell = row.CreateCell(16);
                cell.SetCellValue(restaurantList[i].MUD_ID_RM);
                cell = row.CreateCell(17);
                cell.SetCellValue(restaurantList[i].TERRITORY_RM);
                cell = row.CreateCell(18);
                cell.SetCellValue(restaurantList[i].MUD_ID_RD);
                cell = row.CreateCell(19);
                cell.SetCellValue(restaurantList[i].TERRITORY_RD);
                cell = row.CreateCell(20);
                cell.SetCellValue(restaurantList[i].MUD_ID_TA);
                cell = row.CreateCell(21);
                cell.SetCellValue(restaurantList[i].TERRITORY_TA);

            }
            #endregion

            // 写入到客户端  
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                wk.Write(ms);
                Response.AddHeader("Content-Disposition", string.Format("attachment; filename=Territory-List_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmm")));
                Response.ContentType = "application/vnd.ms-excel";
                Response.BinaryWrite(ms.ToArray());
            }
        }
        #endregion

        #region 导出可送餐列表
        /// <summary>
        /// 导出可送餐列表
        /// </summary>
        /// <returns></returns>
        //[PermissionFilter(Order = 2, Permission = "00000000-0000-0000-2000-000000000001")]
        //[OperationAuditFilter(Operation = "导出可送餐列表", OperationAuditTypeName = "导出可送餐列表")]
        public void ReportHospitalResExcel()
        {
            // 获取可送餐医院列表
            List<P_HOSPITAL_RANGE_RESTAURANT> listHospitalRestaurant = new List<P_HOSPITAL_RANGE_RESTAURANT>();
            List<P_HOSPITAL_INFO_VIEW> listHospital = new List<P_HOSPITAL_INFO_VIEW>();
            List<P_HOSPITAL_RANGE_RESTAURANTCOUNT> listHospitalRestaurantCount = new List<P_HOSPITAL_RANGE_RESTAURANTCOUNT>();

            restaurantService.QryAllRangeRestaurant(ref listHospitalRestaurant, ref listHospital, ref listHospitalRestaurantCount);

            // 整理数据
            var linq = from h in listHospital
                       join hr in listHospitalRestaurant on h.HospitalCode equals hr.GskHospital into _tmp
                       from hr in _tmp.DefaultIfEmpty()
                       join hc in listHospitalRestaurantCount on h.HospitalCode equals hc.GskHospital into _hc
                       from hc in _hc.DefaultIfEmpty()
                       select new
                       {
                           h,
                           hr = hr == null ? new P_HOSPITAL_RANGE_RESTAURANT() : hr,
                           hc = hc == null ? new P_HOSPITAL_RANGE_RESTAURANTCOUNT() : hc
                       };

            var _list = linq.ToList();

            var list = _list.GroupBy(a => a.h).Select(a => new
            {
                h = a.Key,
                listRestaurant = a.Select(b => b.hr) == null ? new List<P_HOSPITAL_RANGE_RESTAURANT>() : a.Select(b => b.hr).OrderBy(c => c.DataSources, new DataSourcesComparer()).ToList(),
                countRestaurant = a.Select(b => b.hc) == null ? new List<P_HOSPITAL_RANGE_RESTAURANTCOUNT>() : a.Select(b => b.hc).OrderBy(c => c.DataSources, new DataSourcesComparer()).ToList()
            }).ToList();

            //var maxCount = list.Count == 0 ? 0 : list.Max(a => a.listRestaurant.Count);
            //var maxCount = list.Max(a => (int?)a.listRestaurant.Count);
            var maxCount = listHospitalRestaurant.GroupBy(p => new { GskHospital = p.GskHospital }).Select(p => new { GskHospital = p.Key.GskHospital, ResCount = p.Count() }).OrderByDescending(p => p.ResCount).FirstOrDefault();
            var hospitalInfoList = list;

            var typeList = "Rx,TSKF,Vx,DDT,院外".Split(',').ToList();
            XSSFWorkbook wk = new XSSFWorkbook();

            // 循环 type 将数据放入EXCEL
            foreach (var type in typeList)
            {
                // 创建sheet
                ISheet sheet = wk.CreateSheet(type + "可送餐列表");
                IRow row = sheet.CreateRow(0);
                sheet.CreateFreezePane(0, 1, 0, 1);

                //ICellStyle style = wk.CreateCellStyle();
                //style.WrapText = true;
                //style.Alignment = HorizontalAlignment.Center;
                //IFont font = wk.CreateFont();
                //font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
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

                // 创建表头
                var titile = "医院ID,省,市,医院名称,医院地址,Market,备注,合计品牌数,XMS品牌数,BDS品牌数,meituan品牌数,XMS早餐数,XMS正餐数,XMS下午茶数,BDS早餐数,BDS正餐数,BDS下午茶数".Split(',').ToList();

                for (int i = 0; i < maxCount.ResCount; i++)
                {
                    titile.Add("餐厅" + (i + 1));
                }

                // 写入表头
                for (var i = 0; i < titile.Count; i++)
                {
                    sheet.SetColumnWidth(i, 20 * 256);
                }

                for (var i = 0; i < titile.Count; i++)
                {
                    ICell cell = row.CreateCell(i);
                    cell.SetCellValue(titile[i]);
                    cell.SetCellType(CellType.String);
                    cell.CellStyle = style;
                }

                // 取出对应type的医院信息
                var typehospitalInfoList = hospitalInfoList.Where(a => a.h.Type == type && a.h.External == 0).OrderBy(x => x.h.HospitalCode).ToList();
                if (type == "院外")
                {
                    typehospitalInfoList = hospitalInfoList.Where(a => a.h.External == 1).OrderBy(x => x.h.HospitalCode).ToList();
                }

                #region 组装Excel文档
                for (var i = 0; i < typehospitalInfoList.Count; i++)
                {
                    var resList = typehospitalInfoList[i].listRestaurant;
                    var resCount = resList.Count;
                    var y = -1;
                    row = sheet.CreateRow(i + 1);
                    ICell cell = null;

                    // 计算品牌数
                    var restaurantXMS = typehospitalInfoList[i].listRestaurant.Where(a => a.DataSources == "XMS").Distinct().ToList();
                    var restaurantBDS = typehospitalInfoList[i].listRestaurant.Where(a => a.DataSources == "BDS").Distinct().ToList();
                    var restaurantMT = typehospitalInfoList[i].listRestaurant.Where(a => a.DataSources == "MT").Distinct().ToList();
                    var countXMS = 0;
                    var countBDS = 0;
                    var countMT = 0;
                    var resCountXMS = typehospitalInfoList[i].countRestaurant.Where(a => a.DataSources == "XMS").Distinct().ToList();
                    var resCountBDS = typehospitalInfoList[i].countRestaurant.Where(a => a.DataSources == "BDS").Distinct().ToList();
                    var totalCountXMS = 0;
                    var breakfastCountXMS = 0;
                    var lunchCountXMS = 0;
                    var teaCountXMS = 0;
                    var totalCountBDS = 0;
                    var breakfastCountBDS = 0;
                    var lunchCountBDS = 0;
                    var teaCountBDS = 0;


                    if (restaurantXMS != null && restaurantXMS.Count > 0)
                    {
                        countXMS = restaurantXMS.
                          Select(b =>
                          new
                          {
                              resID = b.ResId.Substring(0, b.ResId.IndexOf("-"))
                          }).Distinct().Count();
                    }

                    if (restaurantBDS != null && restaurantBDS.Count > 0)
                    {
                        countBDS = restaurantBDS.
                          Select(b =>
                          new
                          {
                              resID = b.ResId.Substring(0, b.ResId.IndexOf("_"))
                          }).Distinct().Count();
                    }
                    if (restaurantMT != null && restaurantMT.Count > 0)
                    {
                        countMT = restaurantMT.
                          Select(b =>
                          new
                          {
                              resID = b.ResId.Substring(0, b.ResId.IndexOf("-"))
                          }).Distinct().Count();
                    }

                    if (resCountXMS != null && resCountXMS.Count > 0)
                    {
                        totalCountXMS = resCountXMS[0].TotalCount;
                        breakfastCountXMS = resCountXMS[0].BreakfastCount;
                        lunchCountXMS = resCountXMS[0].LunchCount;
                        teaCountXMS = resCountXMS[0].TeaCount;
                    }
                    if (resCountBDS != null && resCountBDS.Count > 0)
                    {
                        totalCountBDS = resCountBDS[0].TotalCount;
                        breakfastCountBDS = resCountBDS[0].BreakfastCount;
                        lunchCountBDS = resCountBDS[0].LunchCount;
                        teaCountBDS = resCountBDS[0].TeaCount;
                    }

                    cell = row.CreateCell(++y);
                    cell.SetCellValue(typehospitalInfoList[i].h.GskHospital);
                    cell = row.CreateCell(++y);
                    cell.SetCellValue(typehospitalInfoList[i].h.Provice);
                    cell = row.CreateCell(++y);
                    cell.SetCellValue(typehospitalInfoList[i].h.City);
                    cell = row.CreateCell(++y);
                    cell.SetCellValue(typehospitalInfoList[i].h.Name);
                    cell = row.CreateCell(++y);
                    cell.SetCellValue(typehospitalInfoList[i].h.GskHospital == typehospitalInfoList[i].h.HospitalCode ? typehospitalInfoList[i].h.Address : typehospitalInfoList[i].h.MainAddress + ":" + typehospitalInfoList[i].h.Address);
                    cell = row.CreateCell(++y);
                    cell.SetCellValue(typehospitalInfoList[i].h.Type);
                    cell = row.CreateCell(++y);
                    cell.SetCellValue(typehospitalInfoList[i].h.Remark);
                    cell = row.CreateCell(++y);
                    cell.SetCellValue(countXMS + countBDS + countMT);

                    cell = row.CreateCell(++y);
                    cell.SetCellValue(countXMS);
                    cell = row.CreateCell(++y);
                    cell.SetCellValue(countBDS);
                    cell = row.CreateCell(++y);
                    cell.SetCellValue(countMT);
                    //cell = row.CreateCell(++y);
                    //cell.SetCellValue(totalCountXMS);
                    cell = row.CreateCell(++y);
                    cell.SetCellValue(breakfastCountXMS);
                    cell = row.CreateCell(++y);
                    cell.SetCellValue(lunchCountXMS);
                    cell = row.CreateCell(++y);
                    cell.SetCellValue(teaCountXMS);
                    //cell = row.CreateCell(++y);
                    //cell.SetCellValue(totalCountBDS);
                    cell = row.CreateCell(++y);
                    cell.SetCellValue(breakfastCountBDS);
                    cell = row.CreateCell(++y);
                    cell.SetCellValue(lunchCountBDS);
                    cell = row.CreateCell(++y);
                    cell.SetCellValue(teaCountBDS);

                    var resInfoList = typehospitalInfoList[i].listRestaurant;
                    resInfoList = resInfoList.Distinct().ToList();
                    foreach (var restaurant in resInfoList)
                    {
                        if (restaurant.DataSources != null)
                        {
                            cell = row.CreateCell(++y);
                            cell.SetCellValue($"{restaurant.DataSources}-{restaurant.ResName}");
                        }
                    }
                }
                #endregion
            }

            // 写入到客户端  
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                wk.Write(ms);
                Response.AddHeader("Content-Disposition", string.Format("attachment; filename=Catering-Service-List_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmm")));
                Response.ContentType = "application/vnd.ms-excel";
                Response.BinaryWrite(ms.ToArray());
            }
        }
        #endregion

        #region 导出餐厅列表
        /// <summary>
        /// 导出餐厅列表
        /// </summary>
        /// <returns></returns>
        //[PermissionFilter(Order = 2, Permission = "00000000-0000-0000-2000-000000000001")]
        //[OperationAuditFilter(Operation = "导出餐厅列表", OperationAuditTypeName = "导出餐厅列表")]
        public void ReportResExcel()
        {
            // 获取餐厅列表
            var restaurantList = restaurantService.QryAllRestaurant();

            var titile = "编码,门店/区域,餐厅类型,供应商".Split(',').ToList();

            XSSFWorkbook wk = new XSSFWorkbook();
            ISheet sheet = wk.CreateSheet("餐厅列表");
            IRow row = sheet.CreateRow(0);
            sheet.CreateFreezePane(0, 1, 0, 1);

            //ICellStyle style = wk.CreateCellStyle();
            //style.WrapText = true;
            //style.Alignment = HorizontalAlignment.Center;
            //IFont font = wk.CreateFont();
            //font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
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

            for (var i = 0; i < titile.Count; i++)
            {
                sheet.SetColumnWidth(i, 20 * 256);
            }

            for (var i = 0; i < titile.Count; i++)
            {
                ICell cell = row.CreateCell(i);
                cell.SetCellValue(titile[i]);
                cell.SetCellType(CellType.String);
                cell.CellStyle = style;
            }

            #region 组装Excel文档
            for (var i = 0; i < restaurantList.Count; i++)
            {
                //var y = -1;
                row = sheet.CreateRow(i + 1);
                ICell cell = null;

                cell = row.CreateCell(0);
                cell.SetCellValue(restaurantList[i].ResId);
                cell = row.CreateCell(1);
                cell.SetCellValue(restaurantList[i].ResName);
                cell = row.CreateCell(2);
                cell.SetCellValue(restaurantList[i].ResType);
                cell = row.CreateCell(3);
                cell.SetCellValue(restaurantList[i].DataSources);

            }
            #endregion

            // 写入到客户端  
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                wk.Write(ms);
                Response.AddHeader("Content-Disposition", string.Format("attachment; filename=Restaurant-List_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmm")));
                Response.ContentType = "application/vnd.ms-excel";
                Response.BinaryWrite(ms.ToArray());
            }
        }
        #endregion

        #region 比较器
        /// <summary>
        /// 比较器
        /// </summary>
        /// <returns></returns>
        class DataSourcesComparer : IComparer<string>
        {
            Dictionary<string, int> order = new Dictionary<string, int>();

            public DataSourcesComparer()
            {
                order.Add("XMS", 0);
                order.Add("BDS", 1);
                order.Add("MT", 2);
            }

            int getOrderKey(string key)
            {
                if (string.IsNullOrEmpty(key))
                {
                    return -1;
                }
                if (order.ContainsKey(key))
                {
                    return order[key];
                }
                return key[0];
            }

            public int Compare(string x, string y)
            {
                return getOrderKey(x) - getOrderKey(y);
            }
        }
        #endregion

        #region 导出可送餐列表 废弃
        /// <summary>
        /// 导出可送餐列表 废弃
        /// </summary>
        /// <returns></returns>
        //public void ReportHospitalResExcelAbandoned()
        //{
        //    // 获取可送餐列表
        //    var hospitalRangeRTList = restaurantService.QryAllRangeRestaurant();


        //    var hospitalXMSList = hospitalRangeRTList.Where(a => a.DataSources == "XMS").ToList();
        //    var hospitalBDSList = hospitalRangeRTList.Where(a => a.DataSources == "BDS").ToList();

        //    // 可送餐餐厅最大数
        //    int maxCount = hospitalRangeRTList.Max(a => a.ResCount);

        //    // XMS
        //    int maxXMSCount = hospitalXMSList.Max(a => a.ResCount);
        //    // BDS
        //    int maxBDSCount = hospitalBDSList.Max(a => a.ResCount);

        //    var titile = "医院ID,省,市,医院名称,医院地址,Market,备注,合计品牌数,XMS品牌数,BDS品牌数,meituan品牌数".Split(',').ToList();

        //    for (int i = 0; i < maxXMSCount; i++)
        //    {
        //        titile.Add("餐厅" + (i + 1));
        //    }

        //    HSSFWorkbook wk = new HSSFWorkbook();
        //    ISheet sheet = wk.CreateSheet("小秘书可送餐列表");
        //    IRow row = sheet.CreateRow(0);

        //    ICellStyle style = wk.CreateCellStyle();
        //    style.WrapText = true;
        //    style.Alignment = HorizontalAlignment.Center;
        //    IFont font = wk.CreateFont();
        //    font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
        //    style.SetFont(font);

        //    for (var i = 0; i < titile.Count; i++)
        //    {
        //        sheet.SetColumnWidth(i, 15 * 256);
        //    }

        //    for (var i = 0; i < titile.Count; i++)
        //    {
        //        ICell cell = row.CreateCell(i);
        //        cell.SetCellValue(titile[i]);
        //        cell.SetCellType(CellType.String);
        //        cell.CellStyle = style;
        //    }

        //    #region 组装Excel文档
        //    for (var i = 0; i < hospitalXMSList.Count; i++)
        //    {
        //        var y = -1;
        //        row = sheet.CreateRow(i + 1);
        //        ICell cell = null;

        //        cell = row.CreateCell(++y);
        //        cell.SetCellValue(hospitalXMSList[i].GskHospital);
        //        cell = row.CreateCell(++y);
        //        cell.SetCellValue(hospitalXMSList[i].Province);
        //        cell = row.CreateCell(++y);
        //        cell.SetCellValue(hospitalXMSList[i].City);
        //        cell = row.CreateCell(++y);
        //        cell.SetCellValue(hospitalXMSList[i].Name);
        //        cell = row.CreateCell(++y);
        //        cell.SetCellValue(hospitalXMSList[i].Address);
        //        cell = row.CreateCell(++y);
        //        cell.SetCellValue(hospitalXMSList[i].Type);
        //        cell = row.CreateCell(++y);
        //        cell.SetCellValue(hospitalXMSList[i].Memo);
        //        cell = row.CreateCell(++y);
        //        cell.SetCellValue(hospitalXMSList[i].ResCount);
        //        cell = row.CreateCell(++y);
        //        cell.SetCellValue(hospitalXMSList[i].XMSCount);
        //        cell = row.CreateCell(++y);
        //        cell.SetCellValue(hospitalXMSList[i].BDSCount);
        //        cell = row.CreateCell(++y);
        //        cell.SetCellValue(hospitalXMSList[i].MTCount);

        //        // 写入范围内的每一家餐厅
        //        foreach (var restaurant in hospitalXMSList[i].resList)
        //        {
        //            cell = row.CreateCell(++y);
        //            cell.SetCellValue(restaurant.ResName);
        //        }
        //    }
        //    #endregion

        //    ISheet sheet2 = wk.CreateSheet("BDS可送餐列表");
        //    IRow row2 = sheet2.CreateRow(0);

        //    ICellStyle style2 = wk.CreateCellStyle();
        //    style2.WrapText = true;
        //    style2.Alignment = HorizontalAlignment.Center;
        //    IFont font2 = wk.CreateFont();
        //    font2.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
        //    style2.SetFont(font2);

        //    var titile2 = "医院ID,省,市,医院名称,医院地址,Market,备注,合计品牌数,XMS品牌数,BDS品牌数,meituan品牌数".Split(',').ToList();

        //    for (int i = 0; i < maxBDSCount; i++)
        //    {
        //        titile2.Add("餐厅" + (i + 1));
        //    }

        //    for (var i = 0; i < titile2.Count; i++)
        //    {
        //        sheet2.SetColumnWidth(i, 15 * 256);
        //    }

        //    for (var i = 0; i < titile2.Count; i++)
        //    {
        //        ICell cell = row2.CreateCell(i);
        //        cell.SetCellValue(titile2[i]);
        //        cell.SetCellType(CellType.String);
        //        cell.CellStyle = style;
        //    }

        //    #region 组装Excel文档
        //    for (var i = 0; i < hospitalBDSList.Count; i++)
        //    {
        //        var y = -1;
        //        row = sheet2.CreateRow(i + 1);
        //        ICell cell = null;

        //        cell = row.CreateCell(++y);
        //        cell.SetCellValue(hospitalBDSList[i].GskHospital);
        //        cell = row.CreateCell(++y);
        //        cell.SetCellValue(hospitalBDSList[i].Province);
        //        cell = row.CreateCell(++y);
        //        cell.SetCellValue(hospitalBDSList[i].City);
        //        cell = row.CreateCell(++y);
        //        cell.SetCellValue(hospitalBDSList[i].Name);
        //        cell = row.CreateCell(++y);
        //        cell.SetCellValue(hospitalBDSList[i].Address);
        //        cell = row.CreateCell(++y);
        //        cell.SetCellValue(hospitalBDSList[i].Type);
        //        cell = row.CreateCell(++y);
        //        cell.SetCellValue(hospitalBDSList[i].Memo);
        //        cell = row.CreateCell(++y);
        //        cell.SetCellValue(hospitalBDSList[i].ResCount);
        //        cell = row.CreateCell(++y);
        //        cell.SetCellValue(hospitalBDSList[i].XMSCount);
        //        cell = row.CreateCell(++y);
        //        cell.SetCellValue(hospitalBDSList[i].BDSCount);
        //        cell = row.CreateCell(++y);
        //        cell.SetCellValue(hospitalBDSList[i].MTCount);

        //        // 写入范围内的每一家餐厅
        //        foreach (var restaurant in hospitalBDSList[i].resList)
        //        {
        //            cell = row.CreateCell(++y);
        //            cell.SetCellValue(restaurant.ResName);
        //        }
        //    }
        //    #endregion


        //    // 写入到客户端  
        //    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
        //    {
        //        wk.Write(ms);
        //        Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xls", DateTime.Now.ToString("yyyyMMddHHmmssfff")));
        //        Response.BinaryWrite(ms.ToArray());
        //    }
        //}
        #endregion

    }
}