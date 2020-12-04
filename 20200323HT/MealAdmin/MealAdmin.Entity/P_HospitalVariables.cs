namespace MealAdmin.Entity
{
    using System;
    using System.Collections.Generic;

    public partial class P_HospitalVariables
    {
        public string GskHospital { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string HospitalName { get; set; }
        public string Address { get; set; }
        public string IsMainAdd { get; set; }
        public string Market { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string DistrictCode { get; set; }
        public string District { get; set; }
        public string Remarks { get; set; }
        public int Action { get; set; }
        public string ActionDisplay { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateBy { get; set; }

    }

    public class P_Hospital_Variables_Count
    {
        public int ID { get; set; }
        public DateTime? Date { get; set; }
        public int RxCount { get; set; }
        public int VxCount { get; set; }
        public int DDTCount { get; set; }
        public int TSKFCount { get; set; }
        public int AllCount { get; set; }

    }

    public class P_Brand_Coverage_Count
    {
        public string GskHospital { get; set; }
        public string Type { get; set; }
        public int TotalCount { get; set; }

    }

    public class P_Brand_Coverage_Return
    {
        public string Type { get; set; }
        public int TotalCount { get; set; }
        public int UnCovered { get; set; }
        public decimal Coverage { get; set; }

    }

    public class P_Hospital_Return
    {
        public string Type { get; set; }
        public int Rx { get; set; }
        public int Vx { get; set; }
        public int DDT { get; set; }
        public int TSKF { get; set; }

    }

    public class P_TERRITORY_TA
    {
        public string TERRITORY_TA { get; set; }
    }

    public class P_Hospital_Return_BY_TA
    {
        public string TA_HEAD { get; set; }
        public string HospitalCount { get; set; }
        public string AddressCount { get; set; }
        public string AllCount { get; set; }
        public string OHCount { get; set; }

    }

    public class P_TA_HOSPITAL
    {
        public string TERRITORY_TA { get; set; }
        public string Address { get; set; }
        public string MainAddress { get; set; }

    }

    public class P_Brand_Coverage_Count_TA
    {
        public string GskHospital { get; set; }
        public string TERRITORY_TA { get; set; }
        public int TotalCount { get; set; }

    }

    public class P_Brand_Coverage_Return_TA
    {
        public string TA_HEAD { get; set; }
        public string CoveredCount { get; set; }
        public string UnCoveredCount { get; set; }
        public string Coverage { get; set; }

    }

    public class CHECK_REPORT_LINE_RM
    {
        public string Market { get; set; }
        public string TERRITORY_TA { get; set; }
        public string TerritoryCode_RM { get; set; }
        public string Remarks { get; set; }
        public DateTime? createdate { get; set; }
        public int ACTION { get; set; }

    }

    public class P_CHECK_REPORT_LINE_RM
    {
        public string Market { get; set; }
        public string TERRITORY_TA { get; set; }
        public string TerritoryCode_RM { get; set; }
        public string Remarks { get; set; }
        public string createdate { get; set; }
        public int ACTION { get; set; }

    }

}
