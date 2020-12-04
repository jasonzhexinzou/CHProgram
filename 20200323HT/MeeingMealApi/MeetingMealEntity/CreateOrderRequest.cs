using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingMealEntity
{
    public class CreateOrderRequest : RequestBase
    {
        public string enterpriseOrderId { get; set; }
        public string oldXMSOrderId { get; set; }
        public string typeId { get; set; }
        public string sendTime { get; set; }
        public string foodFee { get; set; }
        public string packageFee { get; set; }
        public string sendFee { get; set; }
        public string discountFee { get; set; }
        public string totalFee { get; set; }
        public string invoiceTitle { get; set; }
        public string orderTime { get; set; }
        public string remark { get; set; }
        public string dinnerName { get; set; }
        public string dinnerNum { get; set; }
        public string cityId { get; set; }
        public string sex { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public string resId { get; set; }
        public string resName { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
        public string hospitalId { get; set; }
        public string cn { get; set; }
        public string cnAmount { get; set; }
        public string department { get; set; }
        public string mudId { get; set; }
        public FoodRequest[] foods { get; set; }

    }
}
