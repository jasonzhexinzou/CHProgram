namespace MealAdmin.Entity
{
    using System;
    using System.Collections.Generic;

    public partial class P_EVALUATE
    {
        public  Guid ID { get; set; }
        public  Guid OrderID { get; set; }
        public  string RestaurantId { get; set; }
        public  int Star { get; set; }
        public  int OnTime { get; set; }
        public  string OnTimeDiscrpion { get; set; }
        public  int IsSafe { get; set; }
        public  string SafeDiscrpion { get; set; }
        public  string SafeImage { get; set; }
        public  int Health { get; set; }
        public  string HealthDiscrpion { get; set; }
        public  string HealthImage { get; set; }
        public  int Pack { get; set; }
        public  string PackDiscrpion { get; set; }
        public  string PackImage { get; set; }
        public  int CostEffective { get; set; }
        public  string CostEffectiveDiscrpion { get; set; }
        public  string CostEffectiveImage { get; set; }
        public  string OtherDiscrpion { get; set; }
        public  string OtherDiscrpionImage { get; set; }
        public  int State { get; set; }
        public  DateTime CreateDate { get; set; }
        public int Normal { get; set; }
    }
}
