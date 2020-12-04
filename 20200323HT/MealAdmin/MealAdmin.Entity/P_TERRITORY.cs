namespace MealAdmin.Entity
{
    using System;
    using System.Collections.Generic;

    public partial class P_TERRITORY
    {
        public Guid ID { get; set; }
        public string HospitalCode { get; set; }
        public string HospitalName { get; set; }
        public string Address { get; set; }
        public string MainAddress { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Market { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string District { get; set; }
        public string DistrictCode { get; set; }
        public string CustomerType { get; set; }
        public string MUD_ID_MR { get; set; }
        public string TERRITORY_MR { get; set; }
        public string MUD_ID_DM { get; set; }
        public string TERRITORY_DM { get; set; }
        public string MUD_ID_RM { get; set; }
        public string TERRITORY_RM { get; set; }
        public string MUD_ID_RD { get; set; }
        public string TERRITORY_RD { get; set; }
        public string MUD_ID_TA { get; set; }
        public string TERRITORY_TA { get; set; }
    }

    public partial class P_TERRITORY_Hospital
    {
        public Guid ID { get; set; }
        public string HospitalCode { get; set; }      
        public string Market { get; set; }
        public string MUD_ID_MR { get; set; }
        public string TERRITORY_MR { get; set; }
        public string MUD_ID_DM { get; set; }
        public string TERRITORY_DM { get; set; }
        public string MUD_ID_RM { get; set; }
        public string TERRITORY_RM { get; set; }
        public string MUD_ID_RD { get; set; }
        public string TERRITORY_RD { get; set; }
        public string MUD_ID_TA { get; set; }
        public string TERRITORY_TA { get; set; }
    }
}
