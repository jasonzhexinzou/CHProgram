namespace MealAdmin.Entity
{
    using System;
    using System.Collections.Generic;

    public partial class WP_COSTCENTER
    {
        public  Guid ID { get; set; }
        public  string CompanyCode { get; set; }
        public  string CostCenterCode { get; set; }
        public  string Name { get; set; }
        public  string Description { get; set; }
        public  string ResponsibleUserID { get; set; }
        public  string ResponsibleUser { get; set; }
        public  DateTime CreateDate { get; set; }
        public  Guid Creator { get; set; }
        public  DateTime ModifyDate { get; set; }
        public  Guid Modifier { get; set; }
        public  int IsDel { get; set; }
    }
}
