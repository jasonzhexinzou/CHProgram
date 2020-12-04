using MeetingMealEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MealAdmin.Entity.Helper
{
    public class P_FoodItem
    {
        public string foodId { get; set; }
        public string foodName { get; set; }
        public string count { get; set; }
        public string price { get; set; }
        public string describe { get; set; }

        public FoodRequest ToFoodRequest()
        {
            return new FoodRequest()
            {
                foodId = foodId,
                foodName = foodName,
                count = count,
                describe= describe
            };
        }
    }
}