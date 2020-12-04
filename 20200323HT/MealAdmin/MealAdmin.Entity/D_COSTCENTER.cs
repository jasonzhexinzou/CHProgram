using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity
{
    public class D_COSTCENTER
    {
        public Guid ID { get; set; }
        public string Market { get; set; }
        public string TA { get; set; }
        public string BUHeadName { get; set; }
        public string Region { get; set; }
        public string RegionManagerName { get; set; }
        public string RegionManagerMUDID { get; set; }
        public string CostCenter { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string ModifiedBy { get; set; }
        public string BUHeadMUDID { get; set; }
        public string RDSDName { get; set; }
        public string RDSDMUDID { get; set; }
        public string OldCostCenter { get; set; }
        public string TERRITORY_TA { get; set; }
    }

    public class D_COSTCENTERSELECT
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
    }

    public class D_CostCenterCount
    {
        public int CostCenterCount { get; set; }
    }

    public class V_TERRITORY_TA
    {
        public int ID { get; set; }
        public string Market { get; set; }
        public string TERRITORY_MR { get; set; }
        public string MUD_ID_MR  { get; set; }
        public string TERRITORY_DM  { get; set; }
        public string MUD_ID_DM  { get; set; }
        public string TERRITORY_RM  { get; set; }
        public string MUD_ID_RM  { get; set; }
        public string TERRITORY_RD  { get; set; }
        public string MUD_ID_RD  { get; set; }
        public string TERRITORY_TA  { get; set; }
        public string MUD_ID_TA  { get; set; }
        public string HospitalCode  { get; set; }        
    }
}
