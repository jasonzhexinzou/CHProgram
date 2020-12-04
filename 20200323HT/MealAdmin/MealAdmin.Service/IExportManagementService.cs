using MealAdmin.Entity;
using MealAdmin.Entity.Helper;
using MealAdmin.Entity.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Service
{
    public interface IExportManagementService
    {
        List<P_SPECIAL_ORDER> ExportSpecialOrder(DateTime? _DTBegin, DateTime? _DTEnd, int SpecialOrderType);
        List<P_SPECIAL_ORDER_DETAIL> LoadSpecialOrderDetail(DateTime? _DTBegin, DateTime? _DTEnd, int SpecialOrderType);
        List<P_TA> LoadTA();
        List<P_SPECIAL_ORDER_PROPORTION> LoadSpecialOrderProportionSummary(string date, string ta, string htType, int specialOrderCnt, string resCnt, int proportion);
        List<P_SPECIAL_ORDER_DETAIL> LoadSpecialOrderProportionDetail(string date, string ta, string htType, int specialOrderCnt, string resCnt, int proportion);
        List<P_EVALUATE> ExportOrderEvaluate(DateTime? _DTBegin, DateTime? _DTEnd, string sltTA, string supplier);
        List<P_ORDER> ExportOrderCount(DateTime? _DTBegin, DateTime? _DTEnd, string sltTA, string supplier);
        List<P_EVALUATE> ExportNonHTOrderEvaluate(DateTime? _DTBegin, DateTime? _DTEnd, string sltTA, string supplier);
        List<P_ORDER> ExportNonHTOrderCount(DateTime? _DTBegin, DateTime? _DTEnd, string sltTA, string supplier);
        List<P_SPECIAL_INVOICE_ORDER> LoadSpecialInvoiceList(string date, string channel);
        List<P_SPECIAL_INVOICE_ORDER_NEW> LoadStarbucksList(string date, string channel);
        List<P_SPECIAL_INVOICE_ORDER_NEW> LoadNonHTList(string date, string channel);

        List<P_ORDER_UnfinishedOrder_View> ExportUnfinishedOrder(string startdate, string enddate, string sltTA, string HTType);
        List<P_ORDER_UnfinishedData_View> ExportUnfinishedData(string startdate, string enddate, string sltTA, string HTType);
        List<P_ORDER_UnfinishedDM_View> ExportUnfinishedDM(string startdate, string enddate, string sltTA, string HTType);
        List<P_ORDER_UnfinishedUser_View> ExportUnfinishedUser(string startdate, string enddate, string sltTA, string HTType);
        List<P_ORDER_Unfinished_VIEW> ExportUnfinished(string startdate, string enddate, string sltTA, string HTType);

        List<P_HOSPITAL_COVERAGE> LoadHospitalCoverageData(string date, string tA, string channel);
        List<P_HOSPITAL_COVERAGE_TOTAL> LoadHospitalCoverageRxData(string date, string tA, string channel);
        List<P_TA> LoadTAForGroup();

    }
}
