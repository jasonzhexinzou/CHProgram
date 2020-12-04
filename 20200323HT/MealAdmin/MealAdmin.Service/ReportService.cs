using MealAdmin.Dao;
using MealAdmin.Entity.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XFramework.XInject.Attributes;

namespace MealAdmin.Service
{
    public class ReportService : IReportService
    {
        [Bean("reportDao")]
        public IReportDao reportDao { get; set; }

        public List<v_caterreport> LoadCater(
            string CN, string MUID, string startDate, string endDate,
            string channel, string isOrderSuccess, string isReceived, string isReturn, string isReturnSuccess,string Supplier,
            int page, int rows, out int total)
        {
            DateTime? datetimeNull = null;
            DateTime? _startDate = string.IsNullOrEmpty(startDate)? datetimeNull:DateTime.Parse(startDate).AddDays(-1);
            DateTime? _endDate = string.IsNullOrEmpty(endDate) ? datetimeNull : DateTime.Parse(endDate).AddDays(1);
            return reportDao.LoadCater(
                CN, MUID, _startDate, _endDate,
                channel, isOrderSuccess, isReceived, isReturn, isReturnSuccess,Supplier,
                page, rows, out total);
        }

        public List<v_caterreport> LoadOldCater(
            string CN, string MUID, string startDate, string endDate,
            string channel, string isOrderSuccess, string isReceived, string isReturn, string isReturnSuccess, string Supplier,
            int page, int rows, out int total)
        {
            DateTime? datetimeNull = null;
            DateTime? _startDate = string.IsNullOrEmpty(startDate) ? datetimeNull : DateTime.Parse(startDate).AddDays(-1);
            DateTime? _endDate = string.IsNullOrEmpty(endDate) ? datetimeNull : DateTime.Parse(endDate).AddDays(1);
            return reportDao.LoadOldCater(
                CN, MUID, _startDate, _endDate,
                channel, isOrderSuccess, isReceived, isReturn, isReturnSuccess, Supplier,
                page, rows, out total);
        }

        public List<v_NonHT_caterreport> LoadCaterForNonHT(
            string CN, string MUID, string HospitalCode, string RestaurantId, string startDate, string endDate,
            string channel, string isOrderSuccess, string isReceived, string isReturn, string isReturnSuccess,string Supplier,
            int page, int rows, out int total)
        {
            DateTime? datetimeNull = null;
            DateTime? _startDate = string.IsNullOrEmpty(startDate) ? datetimeNull : DateTime.Parse(startDate).AddDays(-1);
            DateTime? _endDate = string.IsNullOrEmpty(endDate) ? datetimeNull : DateTime.Parse(endDate).AddDays(1);
            return reportDao.LoadCaterForNonHT(
                CN, MUID, HospitalCode, RestaurantId, _startDate, _endDate,
                channel, isOrderSuccess, isReceived, isReturn, isReturnSuccess,Supplier,
                page, rows, out total);
        }


        public List<HT_Order_Report> LoadOrderReport(string srh_CN, string srh_MUDID, DateTime? srh_CreateTimeBegin, DateTime? srh_CreateTimeEnd, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, int? srh_State, string Supplier, int page, int rows, out int total)
        {
            return reportDao.LoadOrderReport(srh_CN, srh_MUDID, srh_CreateTimeBegin, srh_CreateTimeEnd, srh_DeliverTimeBegin, srh_DeliverTimeEnd, srh_State, Supplier, page, rows, out total);
        }


        public List<HT_Order_Report> LoadReport(
              string CN, string MUID, string TACode, string HospitalCode, string RestaurantId, string CostCenter, string startDate, string endDate,
              string channel, string isOrderSuccess, string isReceived, string isReturn, string isReturnSuccess, string Supplier, string IsSpecialOrder, string OrderState,
              int page, int rows, out int total)
        {
            DateTime? datetimeNull = null;
            DateTime? _startDate = string.IsNullOrEmpty(startDate) ? datetimeNull : DateTime.Parse(startDate).AddDays(-1);
            DateTime? _endDate = string.IsNullOrEmpty(endDate) ? datetimeNull : DateTime.Parse(endDate).AddDays(1);
            return reportDao.LoadReport(
                CN, MUID, TACode, HospitalCode, RestaurantId, CostCenter, _startDate, _endDate,
                channel, isOrderSuccess, isReceived, isReturn, isReturnSuccess, Supplier, IsSpecialOrder, OrderState,
                page, rows, out total);
        }

    }
}
