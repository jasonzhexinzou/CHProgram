namespace MealAdmin.Entity
{
    using System;
    using System.Collections.Generic;

    public partial class P_ORDER_APPROVE
    {
        public Guid ID { get; set; }
        public Guid OrderID { get; set; }
        public string CN { get; set; }
        public string UserId { get; set; }
        public int OldState { get; set; }       
        public int NewState { get; set; }
        public int Result { get; set; }
        public string Comment { get; set; }       
        public int IsWXClient { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUserId { get; set; }
    }
}
