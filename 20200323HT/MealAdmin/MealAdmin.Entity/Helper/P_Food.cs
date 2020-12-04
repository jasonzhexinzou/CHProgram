using MeetingMealEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MealAdmin.Entity.Helper
{
    public class P_Food
    {
        public string resId { get; set; }
        public string resName { get; set; }
        public string resLogo { get; set; }
        public string resAddress { get; set; }
        public string resTel { get; set; }
        public P_FoodItem[] foods { get; set; }
        public decimal allPrice { get; set; }
        public decimal foodFee { get; set; }
        public decimal packageFee { get; set; }
        public decimal sendFee { get; set; }
    }
}