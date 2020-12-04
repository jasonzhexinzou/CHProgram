using MealAdmin.Entity;
using MealAdmin.Entity.View;
using MealAdmin.Service;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XFramework.XInject.Attributes;

namespace MealAdmin.Web.Areas.P.Controllers
{
    public class CityController : AdminBaseController
    {
        // GET: P/City
        [Bean("baseDataService")]
        public IBaseDataService baseDataService { get; set; }

        #region 城市列表首页
        /// <summary>
        /// 城市列表首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region 分页加载城市数据
        /// <summary>
        /// 分页加载城市数据
        /// </summary>
        /// <returns></returns>
        public JsonResult Load(string key,int rows, int page)
        {
            int total;
            var list = baseDataService.LoadBiggestCity(key,rows, page, out total);
            return Json(new { state = 1, rows = list, total = total });
        }
        #endregion

        #region 导入城市列表
        /// <summary>
        /// 导入城市列表
        /// </summary>
        /// <param name="file"></param>
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

            var titleTemplate = "所属省份,城市,城市等级划分".Split(',');
            var titleValues = new string[3];

            for (var i = 0; i < 3; i++)
            {
                titleValues[i] = row.GetCell(0) != null ? row.GetCell(i).StringCellValue : string.Empty;
                if (titleValues[i] != titleTemplate[i])
                {
                    return Json(new { state = 0, txt = "导入的文件格式不正确" }, "text/html", JsonRequestBehavior.AllowGet);
                }
            }
            #endregion

            #region 读取表体
            var excelRows = new List<P_CITY_LIST>();
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                row = sheet.GetRow(i);
                if (row == null) continue;
                if (string.IsNullOrEmpty(GetStringFromCell(row.GetCell(0)))) break;
                excelRows.Add(new P_CITY_LIST()
                {
                    ID=Guid.NewGuid(),
                    ProvinceName = GetStringFromCell(row.GetCell(0)),
                    CityName = GetStringFromCell(row.GetCell(1)),
                    Rank = GetStringFromCell(row.GetCell(2)),
                    CreateDate=DateTime.Now
                });
            }
            #endregion


            // 文件中是否有重复数据
            var listRepeat = excelRows.GroupBy(a => a.CityName).Where(a => a.Count() > 1).Select(a => a.Key).ToList();
            if (listRepeat.Count() > 0)
            {
                return Json(new { state = 0, txt = "Excel中发现重复数据", data = listRepeat }, "text/html", JsonRequestBehavior.AllowGet);
            }

            baseDataService.Import(excelRows);
            
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
    }
}