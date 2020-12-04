namespace MealAdmin.Entity
{
    using System;
    using System.Collections.Generic;

    public partial class B_ROLE_PERMISSION
    {
        public  Guid ID { get; set; }
        public  Guid RoleID { get; set; }
        public  Guid PermissionID { get; set; }
        public  DateTime CreateDate { get; set; }
    }
}
