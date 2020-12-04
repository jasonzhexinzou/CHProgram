namespace MealAdmin.Entity
{
    using System;
    using System.Collections.Generic;

    public partial class P_CITY
    {
        public  int ID { get; set; }
        public  int ProvinceId { get; set; }
        public string Type { get; set; }
        public  string Name { get; set; }
        public  string PinYin { get; set; }
        public  DateTime CreateDate { get; set; }
    }
}
