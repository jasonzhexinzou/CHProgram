namespace MealAdmin.Entity
{
    using System;
    using System.Collections.Generic;

    public partial class P_RESTAURANT_LIST
    {
        public Guid ID { get; set; }
        public string DataSources { get; set; }
        public string ResId { get; set; }
        public string ResName { get; set; }
        public string ResType { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
