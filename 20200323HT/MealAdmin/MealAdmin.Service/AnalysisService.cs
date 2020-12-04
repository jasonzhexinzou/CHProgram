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
    public class AnalysisService : IAnalysisService
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

        [Bean("analysisDao")]
        public IAnalysisDao analysisDao { get; set; }
        public List<P_TA> LoadTA()
        {
            return analysisDao.LoadTA();
        }


        public List<P_PreApproval> LoadRD(string sltTA)
        {
            return analysisDao.LoadRD(sltTA);
        }
        public List<P_PreApproval> LoadRM(string sltRD)
        {
            return analysisDao.LoadRM(sltRD);
        }
        public List<P_PreApproval> LoadDM(string sltRM)
        {
            return analysisDao.LoadDM(sltRM);
        }

        public List<P_MARKET_TA> LoadMarketTA(int hTType)
        {
            return analysisDao.LoadMarketTA(hTType);
        }
        
        public List<P_ORDER_OVERVIEW> LoadOrderview(string beginDate, string endDate, int hTType)
        {
            return analysisDao.LoadOrderview(beginDate, endDate, hTType);
        }
        public List<P_MARKET_TA_VIEW> LoadMarketTAData()
        {
            return analysisDao.LoadMarketTAData();
        }

        public int SaveGroupSetting(string name, List<string> taList, int hTType, int Cnt, string oldMarket)
        {
            return analysisDao.SaveGroupSetting(name, taList, hTType, Cnt, oldMarket);
        }

        public int DeleteGroupSetting(string market, int hTType)
        {
            return analysisDao.DeleteGroupSetting(market, hTType);
        }


        public List<P_PreApproval_TERRITORY> LoadCountChart(DateTime? _MTBegin, DateTime? _MTEnd, string PreAmount, string PreState, string sltTA, string htType, string sltRD, string sltRM, string sltDM)
        {
            return analysisDao.LoadCountChart(_MTBegin, _MTEnd, PreAmount, PreState, sltTA, htType, sltRD, sltRM, sltDM);
        }
        public List<P_PreApproval_CountAmount> ExportCountAmount(DateTime? _MTBegin, DateTime? _MTEnd, string PreAmount, string PreState, string sltTA, string htType, string sltRD, string sltRM, string sltDM)
        {
            return analysisDao.ExportCountAmount(_MTBegin, _MTEnd, PreAmount, PreState, sltTA, htType, sltRD, sltRM, sltDM);
        }
        public List<P_PreApproval_HospitalRanking> ExportHospitalRanking(DateTime? _MTBegin, DateTime? _MTEnd, string PreAmount, string PreState, string sltTA, string htType, string sltRD, string sltRM, string sltDM)
        {
            return analysisDao.ExportHospitalRanking(_MTBegin, _MTEnd, PreAmount, PreState, sltTA, htType, sltRD, sltRM, sltDM);
        }
        public List<P_PreApproval_LIST_VIEW> LoadCountList(string meetingdate, string HTType, string PreAmount, string PreState, string sltTA)
        {
            return analysisDao.LoadCountList(meetingdate, HTType, PreAmount, PreState, sltTA);
        }
        public List<P_PreApproval_Hospital_VIEW> LoadHospital(string meetingdate)
        {
            return analysisDao.LoadHospital(meetingdate);
        }
        public List<P_TA> LoadTAInOrderCost()
        {
            return analysisDao.LoadTAInOrderCost(); 
        }

        public List<P_ORDER_LIST_VIEW> LoadOrderList(string date, string hTType)
        {
            return analysisDao.LoadOrderList(date, hTType);
        }

        public List<Territory_Hospital> LoadTAHospitalList(string date)
        {
            return analysisDao.LoadTAHospitalList(date);
        }

        public List<P_RES_HOSPITAL> LoadResHospital(string v)
        {
            return analysisDao.LoadResHospital(v);
        }

        public List<Territory_Hospital> LoadTAHospitalListByTA(string date, string ta, string hTType)
        {
            return analysisDao.LoadTAHospitalListByTA(date, ta, hTType);
        }

        public List<P_ORDER_LIST_VIEW> LoadOrderListByTA(string date, string hTType, string sltTA)
        {
            return analysisDao.LoadOrderListByTA(date, hTType, sltTA);
        }

        public List<P_ORDER_TERRITORY> LoadOrderChart(DateTime? dTBegin, DateTime? dTEnd, string orderState, string hTType, string sltTA, string sltRD, string sltRM, string sltDM)
        {
            return analysisDao.LoadOrderChart(dTBegin, dTEnd, orderState, hTType, sltTA, sltRD, sltRM, sltDM);
        }

        public List<P_ORDER_TERRITORY> LoadCntAmoAttData(DateTime? dTBegin, DateTime? dTEnd, string orderState, string hTType, string sltTA, string sltRD, string sltRM, string sltDM)
        {
            return analysisDao.LoadCntAmoAttData(dTBegin, dTEnd, orderState, hTType, sltTA, sltRD, sltRM, sltDM);
        }

        public List<P_ORDER_HOSPITAL_RANKING> LoadHosRankingData(DateTime? dTBegin, DateTime? dTEnd, string orderState, string hTType, string sltTA, string sltRD, string sltRM, string sltDM)
        {
            return analysisDao.LoadHosRankingData(dTBegin, dTEnd, orderState, hTType, sltTA, sltRD, sltRM, sltDM);
        }

        public List<P_APPLIER_RANKING> LoadApplierRankingData(DateTime? dTBegin, DateTime? dTEnd, string orderState, string hTType, string sltTA)
        {
            return analysisDao.LoadApplierRankingData(dTBegin, dTEnd, orderState, hTType, sltTA);
        }

        public List<P_HOSPITAL_RANKING> LoadHospitalRankingData(DateTime? dTBegin, DateTime? dTEnd, string orderState, string hTType, string sltTA)
        {
            return analysisDao.LoadHospitalRankingData(dTBegin, dTEnd, orderState, hTType, sltTA);
        }

        public List<P_RESTAURANT_RANKING> LoadRestaurantRankingData(DateTime? dTBegin, DateTime? dTEnd, string orderState, string hTType)
        {
            return analysisDao.LoadRestaurantRankingData(dTBegin, dTEnd, orderState, hTType);
        }
        public List<P_TA> LoadRankingTA()
        {
            return analysisDao.LoadRankingTA();
        }
        public List<P_Order_RANKING> LoadOrderRankingChart(string deliverdate, string HTType, string sltTA)
        {
            return analysisDao.LoadOrderRankingChart(deliverdate, HTType, sltTA);
        }
        public List<P_Amount_RANKING> LoadAmountRankingChart(string deliverdate, string HTType, string sltTA)
        {
            return analysisDao.LoadAmountRankingChart(deliverdate, HTType, sltTA);
        }
        public List<P_Order_MRName> LoadMRName(string deliverdate, string HTType, string sltTA)
        {
            return analysisDao.LoadMRName(deliverdate, HTType, sltTA);
        }

        public List<Territory_Hospital> LoadTAHospitalOHList(string date)
        {
            return analysisDao.LoadTAHospitalOHList(date);
        }
    }
}
