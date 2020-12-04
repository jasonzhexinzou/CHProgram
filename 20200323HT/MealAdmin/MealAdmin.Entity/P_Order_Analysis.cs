namespace MealAdmin.Entity
{
    using System;
    using System.Collections.Generic;

    public partial class P_ORDER_SUMMARY
    {
        public string TA { get; set; }
        public int CountUser { get; set; }
        public int CountOrder { get; set; }
        public decimal CountPrice { get; set; }
    }

    public partial class P_MARKET_TA
    {
        public string Market { get; set; }
        public string TAS { get; set; }
        public int OrderIndex { get; set; }
    }

    public partial class P_ORDER_OVERVIEW
    {
        public int Month { get; set; }
        public string TA { get; set; }
        public int OrderCnt { get; set; }
        public double OrderPrice { get; set; }
    }

    public partial class P_ORDER_SUMMARY_OVERVIEW
    {
        public string Market { get; set; }
        public string Jan { get; set; }
        public string Feb { get; set; }
        public string Mar { get; set; }
        public string Apr { get; set; }
        public string May { get; set; }
        public string Jun { get; set; }
        public string Jul { get; set; }
        public string Aug { get; set; }
        public string Sep { get; set; }
        public string Oct { get; set; }
        public string Nov { get; set; }
        public string Dec { get; set; }
        public string YTD { get; set; }
    }

    public partial class P_MONTH
    {
        public string Mon { get; set; }
    }

    public partial class P_MARKET_TA_VIEW
    {
        public int ID { get; set; }
        public string Market { get; set; }
        public string TA { get; set; }
        public int HTType { get; set; }
        public int OrderIndex { get; set; }
    }

    public partial class P_ORDER_LIST_VIEW
    {
        public Guid ID { get; set; }
        public string TA { get; set; }
        public string UserId { get; set; }
        public string HospitalId { get; set; }
        public int HTType { get; set; }
        public decimal GSKConfirmAmount { get; set; }
    }

    public partial class P_RES_HOSPITAL
    {
        public string GskHospital { get; set; }
        public int ResIdCnt { get; set; }
    }

    public partial class P_ORDER_ANALYSIS_2_EXPORT_LIST
    {
        public string TA { get; set; }
        public string HTType { get; set; }
        public string ResHospitalCnt { get; set; }
        public string OrderHospitalCnt { get; set; }
        public string ResMRCnt { get; set; }
        public string OrderMRCnt { get; set; }
        public string OrderCnt { get; set; }
        public string OrderPrice { get; set; }
    }

    public partial class P_ORDER_ANALYSIS_CHART_COUNT_VIEW
    {
        public string CodeAndNAME { get; set; }
        public double ChartCount { get; set; }
        public double ChartCount1 { get; set; }

    }

    public partial class P_ORDER_ANALYSIS_SUMMARY_VIEW
    {
        public int s1ht { get; set; }
        public int s2ht { get; set; }
        public string s3ht { get; set; }
        public int s4ht { get; set; }
        public int s1oh { get; set; }
        public int s2oh { get; set; }
        public string s3oh { get; set; }
        public int s4oh { get; set; }

    }

    public class P_ORDER_TERRITORY
    {

        public string TA { get; set; }
        public string RDTerritoryCode { get; set; }
        public string RMTerritoryCode { get; set; }
        public string DMTerritoryCode { get; set; }
        public string MRTerritoryCode { get; set; }
        public string TERRITORY_MR { get; set; }
        public string TERRITORY_RD { get; set; }
        public string TERRITORY_RM { get; set; }
        public string TERRITORY_DM { get; set; }
        public string MUD_ID_MR { get; set; }
        public string MUD_ID_DM { get; set; }
        public string MUD_ID_RM { get; set; }
        public string MUD_ID_RD { get; set; }
        public string NAME { get; set; }
        public double OrderCount { get; set; }
        public string ConCode { get; set; }
        public decimal OrderPrice { get; set; }
        public double PreAttendCount { get; set; }
        public string MUDID { get; set; }
    }

    public class P_Order_Count_View
    {
        public string CodeandNAME { get; set; }
        public double OrderCount { get; set; }
        public string OrderPrice { get; set; }
        public decimal OrderAmount { get; set; }
        public double PreAttendCount { get; set; }

    }

    public class P_Order_Count_View_Export
    {
        public string CodeandNAME { get; set; }
        public string Name { get; set; }
        public string Mudid { get; set; }
        public double OrderCount { get; set; }
        public decimal OrderPrice { get; set; }
        public double PreAttendCount { get; set; }

    }

    public class P_ORDER_HOSPITAL_RANKING
    {
        public string HospitalId { get; set; }
        public string HospitalName { get; set; }
        public decimal OrderPrice { get; set; }
    }

    public class P_APPLIER_RANKING
    {
        public string TA { get; set; }
        public string ApplierName { get; set; }
        public string ApplierMUDID { get; set; }
        public string RDTerritoryCode { get; set; }
        public string MUD_ID_RD { get; set; }
        public string RDName { get; set; }
        public double OrderCount { get; set; }
        public decimal OrderPrice { get; set; }
    }

    public class P_HOSPITAL_RANKING
    {
        public string TA { get; set; }
        public string HospitalId { get; set; }
        public string HospitalName { get; set; }
        public double OrderCount { get; set; }
        public decimal OrderPrice { get; set; }
    }

    public class P_RESTAURANT_RANKING
    {
        public string Channel { get; set; }
        public string RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public double OrderCount { get; set; }
        public decimal OrderPrice { get; set; }
    }

    public class P_Order_RANKING
    {
        public string ApplierMUDID { get; set; }
        public double OrderCount { get; set; }
        public double MRCount { get; set; }
    }
    public class P_Order_RANKING_Chart
    {
        public double MRCount { get; set; }
        public double MRCountRate { get; set; }
    }
    public class P_Amount_RANKING
    {
        public string ApplierMUDID { get; set; }
        public decimal OrderPrice { get; set; }
        public double MRCount { get; set; }
    }
    public class P_Amount_RANKING_Chart
    {
        public double MRAmountCount { get; set; }
        public double MRAmountCountRate { get; set; }
    }
    public class P_OrderAmount_RANKING_Chart
    {
        public double MRCount { get; set; }
        public double MRCountRate { get; set; }
        public double MRAmountCount { get; set; }
        public double MRAmountCountRate { get; set; }
        public string MRCountAverage { get; set; }
        public string MRAmountAverage { get; set; }
        public string MRCountForLow { get; set; }
        public string MRCountHighest { get; set; }
        public string HighestName { get; set; }
        public string HighestAmountName { get; set; }
        public string MRAmountHighest { get; set; }
        public string LowestAmountName { get; set; }
        public string MRAmountLowest { get; set; }
    }
    public class P_Order_MRName
    {
        public string ApplierMUDID { get; set; }
        public string Name { get; set; }
        public double OrderCount { get; set; }
        public decimal OrderPrice { get; set; }
    }

    public class P_Order_Count_Amount_View
    {
        public string Name { get; set; }
        public decimal OrderAmount { get; set; }
        public double OrderCount { get; set; }

    }

    public class P_Order_Count_Amount
    {
        public decimal OrderAmount { get; set; }
        public string OwnTerritory { get; set; }
        public string BelongTerritory { get; set; }

    }

    public class P_Order_By_State
    {
        public string DMTerritoryCode { get; set; }
        public string DMName { get; set; }
        public string MRTerritoryCode { get; set; }
        public string MRName { get; set; }
        public string OrderState { get; set; }
        public decimal OrderAmount { get; set; }
        public Guid ID { get; set; }

    }

    public class P_ORDER_TAB_VIEW
    {
        public string Name { get; set; }
        public string TerritoryCode { get; set; }
        public string OrderState { get; set; }
        public double OrderCount { get; set; }
        public decimal OrderAmount { get; set; }
        public List<P_ORDER_DOWN_VIEW> DownList { get; set; }
    }

    public class P_ORDER_DOWN_VIEW
    {
        public string Name { get; set; }
        public string TerritoryCode { get; set; }
        public string OrderState { get; set; }
        public double OrderCount { get; set; }
        public decimal OrderAmount { get; set; }
    }

    public class P_PreOrder_PreApproval
    {
        public string DMTerritoryCode { get; set; }
        public string DMName { get; set; }
        public string MRTerritoryCode { get; set; }
        public string MRName { get; set; }
        public decimal? PreAmount { get; set; }
        public Guid ID { get; set; }

    }

    public class P_PreOrder_Order
    {
        public string DMTerritoryCode { get; set; }
        public string DMName { get; set; }
        public string MRTerritoryCode { get; set; }
        public string MRName { get; set; }
        public string OrderState { get; set; }
        public decimal? OrderAmount { get; set; }
        public Guid ID { get; set; }

    }

    public class P_PreORDER_VIEW
    {
        public string Name { get; set; }
        public string TerritoryCode { get; set; }
        public double OrderCount { get; set; }
        public decimal OrderAmount { get; set; }
        public double PreCount { get; set; }
        public decimal PreAmount { get; set; }
        public List<P_PreOrder_DOWN_VIEW> DownList { get; set; }
    }

    public class P_PreOrder_DOWN_VIEW
    {
        public string Name { get; set; }
        public string TerritoryCode { get; set; }
        public double OrderCount { get; set; }
        public decimal OrderAmount { get; set; }
        public double PreCount { get; set; }
        public decimal PreAmount { get; set; }
    }

    public class P_Pre_Count_Amount_View
    {
        public string Name { get; set; }
        public decimal PreAmount { get; set; }
        public double PreCount { get; set; }

    }

    public class P_PreOrder_Amount_Chart_View
    {
        public string CodeandNAME { get; set; }
        public decimal OrderAmount { get; set; }
        public decimal PreAmount { get; set; }

    }
    public class P_PreOrder_Count_Chart_View
    {
        public string CodeandNAME { get; set; }
        public double OrderCount { get; set; }
        public double PreCount { get; set; }

    }

    public class P_PreOrder_Hospital_View
    {
        public string HospitalCode { get; set; }
        public string HospitalName { get; set; }
        public string HosOrder { get; set; }
        public double OrderCount { get; set; }
        public decimal OrderAmount { get; set; }
        public string HosPre { get; set; }
        public double PreCount { get; set; }
        public decimal PreAmount { get; set; }
    }

}
