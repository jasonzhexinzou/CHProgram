namespace MealAdmin.Entity
{
    using System;
    using System.Collections.Generic;

    public partial class P_USERINFO
    {
        public Guid ID { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Market { get; set; }
        public DateTime CreateDate { get; set; }
        public int IsCheckedStatement { get; set; }
        public int State { get; set; }
        public string DMUserId { get; set; }
        public string DMName { get; set; }
        public string Title { get; set; }
        public string TerritoryCode { get; set; }
        public string Role { get; set; }
    }
    public class P_Count
    {
        public int ApprovalCount { get; set; }
    }
}
