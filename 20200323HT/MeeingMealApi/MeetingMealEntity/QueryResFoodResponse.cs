using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingMealEntity
{
    public class QueryResFoodResponse : ResponseBase
    {
        public QueryResFoodResult result { get; set; }

        public int totleCount { get; set; }
    }


    public class QueryResFoodResult
    {
        //public List<Menu> menuList { get; set; }
        //public string resId { get; set; }
        //public string resName { get; set; }
        public string address { get; set; }
        public long businessHourEnd { get; set; }
        public long businessHourStart { get; set; }
        public string cityId { get; set; }
        public string deliveryNotice { get; set; }
        public long eveningHourEnd { get; set; }
        public long eveningHourStart { get; set; }
        public string imagePath { get; set; }
        public string introduction { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public int minAmount { get; set; }
        public int minSendCount { get; set; }
        public string packageFeeNotice { get; set; }
        public string resId { get; set; }
        public string resName { get; set; }
        public string resTel { get; set; }
        public List<Menu> menuList { get; set; }



    }



    public class Food
    {
        public string describe { get; set; }
        public string foodId { get; set; }
        public string foodName { get; set; }
        public string picUrlBig { get; set; }
        public string picUrlSmall { get; set; }
        public string price { get; set; }
    }

    public class Menu
    {
        public List<Food> foods { get; set; }
        public int sort { get; set; }
        public string typeDetail { get; set; }
        public string typeId { get; set; }
        public string typeName { get; set; }
    }



}
