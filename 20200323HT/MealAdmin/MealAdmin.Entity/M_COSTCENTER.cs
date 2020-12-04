namespace MealAdmin.Entity
{
    using System;
    using System.Collections.Generic;

    public partial class M_COSTCENTER
    {
        public  Guid ID { get; set; }
        public  string Code { get; set; }
        public  string Name { get; set; }
        public  decimal MaxMoney { get; set; }
        public  decimal LimitMoney { get; set; }
        public  Guid LineManagerID { get; set; }
        public  Guid OwnerID { get; set; }
        public  int CostType { get; set; }
        public  int State { get; set; }
        public  Guid Creator { get; set; }
        public  DateTime CreateDate { get; set; }
        public  Guid Modifier { get; set; }
        public  DateTime ModifyDate { get; set; }
    }
}
