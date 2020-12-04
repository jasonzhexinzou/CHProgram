using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity
{
    public class P_AddressApproval
    {
        public Guid ID { get; set; }
        public string ApplierName { get; set; }
        public string ApplierMUDID { get; set; }
        public string ApplierMobile { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string DACode { get; set; }
        public string Market { get; set; }
        public string TA { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public int ApprovalStatus { get; set; }
        public string RejectReason { get; set; }
        public int IsReAssign { get; set; }
        public string ReAssignBUHeadName { get; set; }
        public string ReAssignBUHeadMUDID { get; set; }
        public string ReAssignBUHeadApproveDate { get; set; }
        public string LineManagerName { get; set; }
        public string LineManagerMUDID { get; set; }
        public DateTime? LineManagerApproveDate { get; set; }
        public string GskHospital { get; set; }
        public string HospitalCode { get; set; }
        public string HospitalName { get; set; }
        public string AddAddress { get; set; }
        public string Distance { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string CurrentApproverName { get; set; }
        public string CurrentApproverMUDID { get; set; }
        public string AddressName { get; set; }
        public string OtherAddressDistance { get; set; }

        public string MAddress { get; set; }

        public int IsDel { get; set; }

        public int IsUpdate { get; set; }

        public string AddressNameDisplay { get; set; }
    }

    public class P_AddressApproval_View
    {
        public Guid ID { get; set; }
        public string ApplierName { get; set; }
        public string ApplierMUDID { get; set; }
        public string ApplierMobile { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string DACode { get; set; }
        public string Market { get; set; }
        public string TA { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public int ApprovalStatus { get; set; }
        public string RejectReason { get; set; }
        public int IsReAssign { get; set; }
        public string ReAssignBUHeadName { get; set; }
        public string ReAssignBUHeadMUDID { get; set; }
        public string ReAssignBUHeadApproveDate { get; set; }
        public string LineManagerName { get; set; }
        public string LineManagerMUDID { get; set; }
        public DateTime? LineManagerApproveDate { get; set; }
        public string GskHospital { get; set; }
        public string HospitalCode { get; set; }
        public string HospitalName { get; set; }
        public string AddAddress { get; set; }
        public string Distance { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string CurrentApproverName { get; set; }
        public string CurrentApproverMUDID { get; set; }

        public List<string> AddressList { get; set; }

        public string MainHospitalAddress { get; set; }

        public string AddressName { get; set; }

        public List<string> DistanceList { get; set; }

        public string MainAddress { get; set; }

        public string OtherAddressDistance { get; set; }

        public int CityId { get; set; }

        public int ProvinceId { get; set; }

        public string Type { get; set; }

        public string FirstLetters { get; set; }

        public int RejectViewResult { get; set; }

        public string RejectViewReason { get; set; }

        public string RejectViewLinemanagerMUDID { get; set; }

        public string RejectViewLinemanagerName { get; set; }

        public DateTime? RejectViewDate { get; set; }

        public List<P_AddressApproval> inProgressList { get; set; }

        public string MainLat { get; set; }

        public string MainLng { get; set; }

        public string Position { get; set; }

        public string Remark { get; set; }

        public string LineManagerApproveDateDisplay { get; set; }
        public string ApproveStatusDisplay { get; set; }

        public int IsDelete { get; set; }

        public string IsDeleteDisplay { get; set; }

        public string MAddress { get; set; }

        public int IsDeleteUpdate { get; set; }

        public string AddressNameDisplay { get; set; }

        public DateTime? DeleteDate { get; set; }
    }
}
