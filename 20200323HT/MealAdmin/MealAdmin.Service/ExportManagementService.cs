using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MealAdmin.Entity;
using XFramework.XInject.Attributes;
using MealAdmin.Dao;
using MealAdmin.Entity.View;
using MealAdmin.Entity.Helper;
using MealAdmin.Service.Helper;
using Newtonsoft.Json;
using MealAdmin.Entity.Enum;
using MeetingMealApiClient;

namespace MealAdmin.Service
{
    public class ExportManagementService : IExportManagementService
    {
        [Bean("orderDao")]
        public IOrderDao orderDao { get; set; }

        [Bean("meetingDao")]
        public IMeetingDao meetingDao { get; set; }

        [Bean("preApprovalDao")]
        public IPreApprovalDao preApprovalDao { get; set; }

        [Bean("groupMemberDao")]
        public IGroupMemberDao groupMemberDao { get; set; }

        [Bean("exportManagementDao")]
        public IExportManagementDao exportManagementDao { get; set; }

        public List<P_SPECIAL_ORDER> ExportSpecialOrder(DateTime? dTBegin, DateTime? dTEnd, int specialOrderType)
        {
            return exportManagementDao.ExportSpecialOrder(dTBegin, dTEnd, specialOrderType);
        }

        public List<P_SPECIAL_ORDER_DETAIL> LoadSpecialOrderDetail(DateTime? dTBegin, DateTime? dTEnd, int specialOrderType)
        {
            return exportManagementDao.LoadSpecialOrderDetail(dTBegin, dTEnd, specialOrderType);
        }

        public List<P_EVALUATE> ExportOrderEvaluate(DateTime? _DTBegin, DateTime? _DTEnd, string TA, string supplier)
        {
            return exportManagementDao.ExportOrderEvaluate(_DTBegin, _DTEnd, TA, supplier);
        }

        public List<P_ORDER> ExportOrderCount(DateTime? _DTBegin, DateTime? _DTEnd, string TA, string supplier)
        {
            return exportManagementDao.ExportOrderCount(_DTBegin, _DTEnd, TA, supplier);
        }
        public List<P_EVALUATE> ExportNonHTOrderEvaluate(DateTime? _DTBegin, DateTime? _DTEnd, string TA, string supplier)
        {
            return exportManagementDao.ExportNonHTOrderEvaluate(_DTBegin, _DTEnd, TA, supplier);
        }

        public List<P_ORDER> ExportNonHTOrderCount(DateTime? _DTBegin, DateTime? _DTEnd, string TA, string supplier)
        {
            return exportManagementDao.ExportNonHTOrderCount(_DTBegin, _DTEnd, TA, supplier);
        }

        public List<P_TA> LoadTA()
        {
            return exportManagementDao.LoadTA();
        }

        public List<P_SPECIAL_ORDER_PROPORTION> LoadSpecialOrderProportionSummary(string date, string ta, string htType, int specialOrderCnt, string resCnt, int proportion)
        {
            return exportManagementDao.LoadSpecialOrderProportionSummary(date, ta, htType, specialOrderCnt, resCnt, proportion);
        }

        public List<P_SPECIAL_ORDER_DETAIL> LoadSpecialOrderProportionDetail(string date, string ta, string htType, int specialOrderCnt, string resCnt, int proportion)
        {
            return exportManagementDao.LoadSpecialOrderProportionDetail(date, ta, htType, specialOrderCnt, resCnt, proportion);
        }

        public List<P_SPECIAL_INVOICE_ORDER> LoadSpecialInvoiceList(string date, string channel)
        {
            return exportManagementDao.LoadSpecialInvoiceList(date, channel);
        }

        public List<P_SPECIAL_INVOICE_ORDER_NEW> LoadStarbucksList(string date, string channel)
        {
            return exportManagementDao.LoadStarbucksList(date, channel);
        }

        public List<P_SPECIAL_INVOICE_ORDER_NEW> LoadNonHTList(string date, string channel)
        {
            return exportManagementDao.LoadNonHTList(date, channel);
        }

        public List<P_ORDER_UnfinishedOrder_View> ExportUnfinishedOrder(string startdate, string enddate, string sltTA, string HTType)
        {
            return exportManagementDao.ExportUnfinishedOrder(startdate, enddate, sltTA, HTType);
        }
        public List<P_ORDER_UnfinishedData_View> ExportUnfinishedData(string startdate, string enddate, string sltTA, string HTType)
        {
            return exportManagementDao.ExportUnfinishedData(startdate, enddate, sltTA, HTType);
        }
        public List<P_ORDER_UnfinishedDM_View> ExportUnfinishedDM(string startdate, string enddate, string sltTA, string HTType)
        {
            return exportManagementDao.ExportUnfinishedDM(startdate, enddate, sltTA, HTType);
        }
        public List<P_ORDER_UnfinishedUser_View> ExportUnfinishedUser(string startdate, string enddate, string sltTA, string HTType)
        {
            return exportManagementDao.ExportUnfinishedUser(startdate, enddate, sltTA, HTType);
        }
        public List<P_ORDER_Unfinished_VIEW> ExportUnfinished(string startdate, string enddate, string sltTA, string HTType)
        {
            return exportManagementDao.ExportUnfinished(startdate, enddate, sltTA, HTType);
        }

        public List<P_HOSPITAL_COVERAGE> LoadHospitalCoverageData(string date, string ta, string channel)
        {
            return exportManagementDao.LoadHospitalCoverageData(date, ta, channel);
        }

        public List<P_HOSPITAL_COVERAGE_TOTAL> LoadHospitalCoverageRxData(string date, string ta, string channel)
        {
            return exportManagementDao.LoadHospitalCoverageRxData(date, ta, channel);
        }

        public List<P_TA> LoadTAForGroup()
        {
            return exportManagementDao.LoadTAForGroup();
        }
    }
}
