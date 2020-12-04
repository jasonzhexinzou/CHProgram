namespace MealAdmin.Entity
{
    using System;
    using System.Collections.Generic;

    public partial class WP_QYDEPT_USER
    {
        public  Guid ID { get; set; }
        public  Guid QyDeptID { get; set; }
        public  Guid QyUserID { get; set; }
        public  DateTime CreateDate { get; set; }
        public  Guid Creator { get; set; }
    }
}
