namespace MealAdmin.Entity
{
    using System;
    using System.Collections.Generic;

    public partial class M_COSTCENTER_MANAGER
    {
        public  Guid ID { get; set; }
        public  Guid CostCenterID { get; set; }
        public  Guid QyUserID { get; set; }
        public  DateTime CreateDate { get; set; }
    }
}
