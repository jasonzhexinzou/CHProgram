namespace MealAdmin.Entity
{
    using System;
    using System.Collections.Generic;

    public partial class P_PROVINCE
    {
        public  int ID { get; set; }
        public  string Name { get; set; }
        public  string PinYin { get; set; }
        public string Type { get; set; }
        public  DateTime CreateDate { get; set; }
    }
}
