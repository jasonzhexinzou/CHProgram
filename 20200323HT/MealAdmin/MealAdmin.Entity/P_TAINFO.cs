namespace MealAdmin.Entity
{
    using System;
    using System.Collections.Generic;

    public partial class P_TAINFO
    {
        public Guid ID { get; set; }
        public string TerritoryTA { get; set; }
        public string TerritoryShotName { get; set; }
        public string TerritoryHead { get; set; }
        public string TerritoryHeadName { get; set; }
        public Guid BUID { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
    }


    public partial class P_TAINFOView
    {
        public Guid ID { get; set; }
        public string TerritoryTA { get; set; }
        public string TerritoryShotName { get; set; }
        public string TerritoryHead { get; set; }
        public string TerritoryHeadName { get; set; }
        public string BUName { get; set; }
        public Guid BUID { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
    }
}
