namespace MealAdmin.Entity
{
    using System;
    using System.Collections.Generic;

    public partial class P_BUINFO
    {
        public Guid ID { get; set; }
        public string BUName { get; set; }
        public string BUHead { get; set; }
        public string BUHeadMudid { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
    }
}
