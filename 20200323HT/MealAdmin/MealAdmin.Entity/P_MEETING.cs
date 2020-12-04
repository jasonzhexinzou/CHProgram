namespace MealAdmin.Entity
{
    using System;
    using System.Collections.Generic;

    public partial class P_MEETING
    {
        public  Guid ID { get; set; }
        public  string Code { get; set; }
        public  string Title { get; set; }
        public  int BudgetTotal { get; set; }
        public  DateTime SubmittedDate { get; set; }
        public  DateTime ApprovedDate { get; set; }
        public  string Status { get; set; }
        public  string PendingWith { get; set; }
        public  string UserId { get; set; }
        public  int IsUsed { get; set; }
    }
}
