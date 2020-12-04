namespace MealAdmin.Entity
{
    using System;
    using System.Collections.Generic;

    public partial class P_SPECIAL_ORDER
    {
        public string TA { get; set; }
        public int CountUser { get; set; }
        public int CountOrder { get; set; }
        public decimal CountPrice { get; set; }
    }

    public class P_SPECIAL_ORDER_DETAIL
    {
        public string ApplierName { get; set; }
        public string ApplierMUDID { get; set; }
        public string HTCode { get; set; }
        public string Market { get; set; }
        public string VeevaMeetingID { get; set; }
        public string TA { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string HospitalCode { get; set; }
        public string HospitalName { get; set; }
        public string HospitalAddress { get; set; }
        public string CostCenter { get; set; }
        public string Channel { get; set; }
        public DateTime DeliverTime { get; set; }
        public string RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public string RealCount { get; set; }
        public int ReceiveState { get; set; }
        public decimal? GSKConfirmAmount { get; set; }

    }

    public class P_SPECIAL_ORDER_DETAIL_VIEW
    {
        public string ApplierName { get; set; }
        public string ApplierMUDID { get; set; }
        public string HTCode { get; set; }
        public string Market { get; set; }
        public string VeevaMeetingID { get; set; }
        public string TA { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string HospitalCode { get; set; }
        public string HospitalName { get; set; }
        public string HospitalAddress { get; set; }
        public string CostCenter { get; set; }
        public string Channel { get; set; }
        public string DeliverDate { get; set; }
        public string DeliverTime { get; set; }
        public string RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public string RealCount { get; set; }
        public string ReceiveState { get; set; }
        public string GSKConfirmAmount { get; set; }

    }

    public partial class P_SPECIAL_ORDER_PROPORTION
    {
        public string ApplierName { get; set; }
        public string UserId { get; set; }
        public string TA { get; set; }
        public string HospitalId { get; set; }
        public string RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public int SpecialCnt { get; set; }
        public decimal PriceCnt { get; set; }
        public int OrdCnt { get; set; }
        public int ResIdCnt { get; set; }
        public decimal Proportion { get; set; }
    }

    public partial class P_SPECIAL_INVOICE_ORDER
    {
        public string Channel { get; set; }
        public int SpecialCnt { get; set; }
        public int NonSpecialCnt { get; set; }
    }

    public partial class P_SPECIAL_INVOICE_ORDER_NEW
    {
        public string Channel { get; set; }
        public int OrderCnt { get; set; }
        public decimal OrderPrice { get; set; }
    }

    public partial class P_HOSPITAL_COVERAGE
    {
        public string Territory_TA { get; set; }
        public string Market { get; set; }
        public int AddressCnt { get; set; } 
        public int BrandCnt1 { get; set; }
        public int BrandCnt2 { get; set; }
        public int BrandCnt3 { get; set; }
        public int BrandCnt4 { get; set; }
        public int BrandCnt5 { get; set; }
        public int BreakfastCnt { get; set; }
        public int TeaCnt { get; set; }
        public decimal BrandCoverage1 { get; set; }
        public decimal BrandCoverage2 { get; set; }
        public decimal BrandCoverage3 { get; set; }
        public decimal BrandCoverage4 { get; set; }
        public decimal BrandCoverage5 { get; set; }
        public decimal BreakfastCoverage { get; set; }
        public decimal TeaCoverage { get; set; }
    }

    public partial class P_HOSPITAL_COVERAGE_TOTAL
    {
        public string HospitalCode { get; set; }
        public string Market { get; set; }
        public string Territory_TA { get; set; }
        public string DataSources { get; set; }
        public int TotalCount { get; set; }
        public int BreakfastCount { get; set; }
        public int LunchCount { get; set; }
        public int TeaCount { get; set; }
    }

}
