namespace MealAdmin.Entity
{
    using System;
    using System.Collections.Generic;

    public partial class P_HOSPITAL
    {
        public int ID { get; set; }
        public int CityId { get; set; }
        public string GskHospital { get; set; }
        public string OldGskHospital { get; set; }
        public string Name { get; set; }
        public string OldName { get; set; }
        public string FirstLetters { get; set; }
        public string Address { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Type { get; set; }
        public int External { get; set; }
        public DateTime CreateDate { get; set; }
        public int ProvinceId { get; set; }
        public string IsXMS { get; set; }
        public string IsBDS { get; set; }
        public string IsMT { get; set; }
        public string Remark { get; set; }
        public int IsDelete { get; set; }
        public string RelateUserList { get; set; }


        public string MainAddress { get; set; }              //是否为主地址
        public string HospitalCode { get; set; }            //实际使用的医院编码，由医院编码和是否为主地址拼凑而成

    }

    public class P_HOSPITAL_MNT_VIEW
    {
        public int ID { get; set; }
        public string GskHospital { get; set; }
        public string OldGskHospital { get; set; }
        public string Name { get; set; }
        public string OldName { get; set; }
        public string FirstLetters { get; set; }
        public string Address { get; set; }
        public string Type { get; set; }
        public string ProvinceName { get; set; }
        public string CityName { get; set; }
        public DateTime CreateDate { get; set; }
        public string RelateUserList { get; set; }

        public string MainAddress { get; set; }
        public string HospitalCode { get; set; }
    }



    public class P_HOSPITAL_CHANNEL
    {
        public string state { get; set; }
        public P_HOSPITALINFO result { get; set; }
    }


    public class P_HOSPITALINFO
    {
        public string GskHospital { get; set; }
        public string ProvinceName { get; set; }
        public string CityName { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Type { get; set; }
        public int External { get; set; }
    }

    public partial class P_HOSPITAL_NEW
    {
        public int ID { get; set; }
        public int CityId { get; set; }
        public string GskHospital { get; set; }
        public string OldGskHospital { get; set; }
        public string Name { get; set; }
        public string OldName { get; set; }
        public string FirstLetters { get; set; }
        public string Address { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Type { get; set; }
        public int External { get; set; }
        public DateTime CreateDate { get; set; }
        public int ProvinceId { get; set; }
        public string IsXMS { get; set; }
        public string IsBDS { get; set; }
        public string IsMT { get; set; }
        public string Remark { get; set; }
        public int IsDelete { get; set; }
        public string RelateUserList { get; set; }


        public string MainAddress { get; set; }              //是否为主地址
        public string HospitalCode { get; set; }            //实际使用的医院编码，由医院编码和是否为主地址拼凑而成

        public string CityName { get; set; }

        public string ProvinceName { get; set; }

        public int ApprovalStatus { get; set; }

        public string ApplierName { get; set; }

        public string ApplierMUDID { get; set; }

        public List<P_AddressApproval> inProgressList { get; set; }

        public Guid? DA_ID { get; set; }

    }

    public partial class P_HOSPITAL_DETAIL
    {
        public int ID { get; set; }
        public string GskHospital { get; set; }
        public string District { get; set; }
        public string DistrictCode { get; set; }
        public string CustomerType { get; set; }
        public string RESP { get; set; }
        public string HEP { get; set; }
        public string CNS { get; set; }
        public string HIV { get; set; }
        public string VOL { get; set; }
        public string MA { get; set; }
        public string Region { get; set; }
        public int IsDelete { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string IP { get; set; }
    }

    public class P_HOSPITAL_DATA_VIEW
    {
        public int ID { get; set; }
        public string GskHospital { get; set; }
        public string ProvinceName { get; set; }
        public string CityName { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string MainAddress { get; set; }
        public string Type { get; set; }
        public string OldGskHospital { get; set; }
        public string OldName { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string District { get; set; }
        public string DistrictCode { get; set; }
        public string CustomerType { get; set; }
        public string RESP { get; set; }
        public string HEP { get; set; }
        public string CNS { get; set; }
        public string HIV { get; set; }
        public string VOL { get; set; }
        public string MA { get; set; }
        public string Region { get; set; }
        public int XMSTotalCount { get; set; }
        public int XMSBreakfastCount { get; set; }
        public int XMSLunchCount { get; set; }
        public int XMSTeaCount { get; set; }
        public int BDSTotalCount { get; set; }
        public int BDSBreakfastCount { get; set; }
        public int BDSLunchCount { get; set; }
        public int BDSTeaCount { get; set; }
        public int TotalCount { get; set; }
        public int TotalBreakfastCount { get; set; }
        public int TotalLunchCount { get; set; }
        public int TotalTeaCount { get; set; }
        public string Remark { get; set; }
        public int Order_2017 { get; set; }
        public int Order_2018 { get; set; }
        public int Order_2019 { get; set; }
        public int Order_201719 { get; set; }
        public int Order_2020 { get; set; }
        public int Order_202001 { get; set; }
        public int Order_202002 { get; set; }
        public int Order_202003 { get; set; }
        public int Order_202004 { get; set; }
        public int Order_202005 { get; set; }
        public int Order_202006 { get; set; }
        public int Order_202007 { get; set; }
        public int Order_202008 { get; set; }
        public int Order_202009 { get; set; }
        public int Order_202010 { get; set; }
        public int Order_202011 { get; set; }
        public int Order_202012 { get; set; }
        public int External { get; set; }
        public string HospitalCode { get; set; }
        public string IP { get; set; }
    }

    public partial class P_HOSPITAL_RxTemp
    {
        public int ID { get; set; }
        public string GskHospital { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public Double Longitude { get; set; }
        public Double Latitude { get; set; }
        public string District { get; set; }
        public string DistrictCode { get; set; }
        public string CustomerType { get; set; }
        public string RESP { get; set; }
        public string HEP { get; set; }
        public string CNS { get; set; }
        public string HIV { get; set; }
        public string VOL { get; set; }
        public string MA { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string IP { get; set; }
    }

    public partial class P_HOSPITAL_VxTemp
    {
        public int ID { get; set; }
        public string GskHospital { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public Double Longitude { get; set; }
        public Double Latitude { get; set; }
        public string District { get; set; }
        public string DistrictCode { get; set; }
        public string CustomerType { get; set; }
        public string Region { get; set; }
        public DateTime? UpdateDate { get; set; }
    }

    public partial class P_HOSPITAL_DDTTemp
    {
        public int ID { get; set; }
        public string GskHospital { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public Double Longitude { get; set; }
        public Double Latitude { get; set; }
        public string District { get; set; }
        public string DistrictCode { get; set; }
        public string CustomerType { get; set; }
        public string Region { get; set; }
        public DateTime? UpdateDate { get; set; }
    }

    public partial class P_HOSPITAL_TSKFTemp
    {
        public int ID { get; set; }
        public string GskHospital { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public Double Longitude { get; set; }
        public Double Latitude { get; set; }
        public string District { get; set; }
        public string DistrictCode { get; set; }
        public string CustomerType { get; set; }
        public string Region { get; set; }
        public DateTime? UpdateDate { get; set; }
    }

    public partial class Temp_Hospital_Variables
    {
        public string GskHospital { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string HospitalName { get; set; }
        public string Address { get; set; }
        public string IsMainAdd { get; set; }
        public string Market { get; set; }
        public Double Longitude { get; set; }
        public Double Latitude { get; set; }
        public string DistrictCode { get; set; }
        public string District { get; set; }
        public int action { get; set; }       
        public DateTime? createdate { get; set; }
        public string createby { get; set; }
        public string Remarks { get; set; }
    }

    public class Territory_Hospital
    {
        public int ID { get; set; }
        public string Market { get; set; }
        public string TERRITORY_MR { get; set; }
        public string MUD_ID_MR { get; set; }
        public string TERRITORY_DM { get; set; }
        public string MUD_ID_DM { get; set; }
        public string TERRITORY_RM { get; set; }
        public string MUD_ID_RM { get; set; }
        public string TERRITORY_RD { get; set; }
        public string MUD_ID_RD { get; set; }
        public string TERRITORY_TA { get; set; }
        public string MUD_ID_TA { get; set; }
        public string HospitalCode { get; set; }

        public DateTime? CreateDate { get; set; }
        public string GskHospitalView { get; set; }
    }

}
