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
    public interface IAnalysisService
    {

        List<P_TA> LoadTA();
        List<P_PreApproval> LoadRD(string sltTA);
        List<P_PreApproval> LoadRM(string sltRD);
        List<P_PreApproval> LoadDM(string sltRM);

        List<P_MARKET_TA> LoadMarketTA(int hTType);
        List<P_ORDER_OVERVIEW> LoadOrderview(string beginDate, string endDate, int hTType);
        List<P_MARKET_TA_VIEW> LoadMarketTAData();
        int SaveGroupSetting(string name, List<string> taList, int hTType, int Cnt, string oldMarket);
        int DeleteGroupSetting(string market, int hTType);
        List<P_TA> LoadTAInOrderCost();
        List<P_ORDER_LIST_VIEW> LoadOrderList(string date, string hTType);
        List<Territory_Hospital> LoadTAHospitalList(string date);
        List<P_PreApproval_TERRITORY> LoadCountChart(DateTime? _MTBegin, DateTime? _MTEnd, string PreAmount, string PreState, string sltTA, string htType, string sltRD, string sltRM, string sltDM);
        List<P_PreApproval_CountAmount> ExportCountAmount(DateTime? _MTBegin, DateTime? _MTEnd, string PreAmount, string PreState, string sltTA, string htType, string sltRD, string sltRM, string sltDM);
        List<P_PreApproval_HospitalRanking> ExportHospitalRanking(DateTime? _MTBegin, DateTime? _MTEnd, string PreAmount, string PreState, string sltTA, string htType, string sltRD, string sltRM, string sltDM);
        List<P_PreApproval_LIST_VIEW> LoadCountList(string meetingdate, string HTType, string PreAmount, string PreState, string sltTA);
        List<P_PreApproval_Hospital_VIEW> LoadHospital(string meetingdate);
        List<P_RES_HOSPITAL> LoadResHospital(string v);
        List<Territory_Hospital> LoadTAHospitalListByTA(string date, string ta, string hTType);
        List<P_ORDER_LIST_VIEW> LoadOrderListByTA(string date, string hTType, string sltTA);
        List<P_ORDER_TERRITORY> LoadOrderChart(DateTime? dTBegin, DateTime? dTEnd, string orderState, string hTType, string sltTA, string sltRD, string sltRM, string sltDM);
        List<P_ORDER_TERRITORY> LoadCntAmoAttData(DateTime? dTBegin, DateTime? dTEnd, string orderState, string hTType, string sltTA, string sltRD, string sltRM, string sltDM);
        List<P_ORDER_HOSPITAL_RANKING> LoadHosRankingData(DateTime? dTBegin, DateTime? dTEnd, string orderState, string hTType, string sltTA, string sltRD, string sltRM, string sltDM);
        List<P_APPLIER_RANKING> LoadApplierRankingData(DateTime? dTBegin, DateTime? dTEnd, string orderState, string hTType, string sltTA);
        List<P_HOSPITAL_RANKING> LoadHospitalRankingData(DateTime? dTBegin, DateTime? dTEnd, string orderState, string hTType, string sltTA);
        List<P_RESTAURANT_RANKING> LoadRestaurantRankingData(DateTime? dTBegin, DateTime? dTEnd, string orderState, string hTType);
        List<P_TA> LoadRankingTA();
        List<P_Order_RANKING> LoadOrderRankingChart(string deliverdate, string HTType, string sltTA);
        List<P_Amount_RANKING> LoadAmountRankingChart(string deliverdate, string HTType, string sltTA);
        List<P_Order_MRName> LoadMRName(string deliverdate, string HTType, string sltTA);
        List<Territory_Hospital> LoadTAHospitalOHList(string date);
    }
}
