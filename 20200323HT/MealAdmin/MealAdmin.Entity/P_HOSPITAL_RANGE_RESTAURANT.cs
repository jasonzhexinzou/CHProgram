namespace MealAdmin.Entity
{
    using System;
    using System.Collections.Generic;

    public partial class P_HOSPITAL_RANGE_RESTAURANT
    {
        public Guid ID { get; set; }
        public string GskHospital { get; set; }
        public string DataSources { get; set; }
        public string ResId { get; set; }
        public string ResName { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
