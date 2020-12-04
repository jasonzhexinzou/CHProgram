namespace MealAdmin.Entity
{
    using System;
    using System.Collections.Generic;

    public partial class P_HOSPITAL_RANGE
    {
        public Guid ID { get; set; }
        public string DataSources { get; set; }
        public string GskHospital { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Type { get; set; }
        public string Memo { get; set; }
        public int ResCount { get; set; }
        public DateTime CreateDate { get; set; }

    }
}
