namespace MealAdmin.Entity
{
    using System;
    using System.Collections.Generic;

    public partial class B_PERMISSION
    {
        public  Guid ID { get; set; }
        public  string Name { get; set; }
        public  Guid ParentID { get; set; }
        public  int Order { get; set; }
    }
}
