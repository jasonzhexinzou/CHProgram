namespace MealAdmin.Entity
{
    using System;
    using System.Collections.Generic;

    public partial class B_ADMINUSER
    {
        public  Guid ID { get; set; }
        public  string Email { get; set; }
        public  string Name { get; set; }
        public  string Pwd { get; set; }
        public  int State { get; set; }
        public  DateTime CreateDate { get; set; }
    }
}
