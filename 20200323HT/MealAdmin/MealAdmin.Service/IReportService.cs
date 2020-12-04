using MealAdmin.Entity.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Service
{
    public interface IReportService
    {
        List<v_caterreport> LoadCater(string CN, string MUID,
            string startDate, string endDate,
            string channel, string isOrderSuccess, string isReceived, string isReturn, string isReturnSuccess,string Supplier,
            int page, int rows, out int total);

        List<v_caterreport> LoadOldCater(
            string CN, string MUID, string startDate, string endDate,
            string channel, string isOrderSuccess, string isReceived, string isReturn, string isReturnSuccess, string Supplier,
            int page, int rows, out int total);

        List<v_NonHT_caterreport> LoadCaterForNonHT(string CN, string MUID, string HospitalCode, string RestaurantId,
            string startDate, string endDate,
            string channel, string isOrderSuccess, string isReceived, string isReturn, string isReturnSuccess,string Supplier,
            int page, int rows, out int total);

        List<HT_Order_Report> LoadOrderReport(string srh_CN, string srh_MUDID, DateTime? srh_CreateTimeBegin, DateTime? srh_CreateTimeEnd, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, int? srh_State, string Supplier, int page, int rows, out int total);

        List<HT_Order_Report> LoadReport(string CN, string MUID, string TACode, string HospitalCode, string RestaurantId, string CostCenter,
          string startDate, string endDate,
          string channel, string isOrderSuccess, string isReceived, string isReturn, string isReturnSuccess, string Supplier, string IsSpecialOrder, string OrderState,
          int page, int rows, out int total);
    }
}
