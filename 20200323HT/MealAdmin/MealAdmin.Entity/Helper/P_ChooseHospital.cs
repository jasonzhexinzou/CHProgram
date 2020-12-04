using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MealAdmin.Entity.Helper
{
    public class P_ChooseHospital
    {
        public string market { get; set; }
        public string invoiceTitle { get; set; }
        public string province { get; set; }
        public string provinceName { get; set; }
        public string city { get; set; }
        public string cityName { get; set; }
        public string hospital { get; set; }
        public string hospitalName { get; set; }
        public string address { get; set; }
        public int isExternal { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
        public string ta { get; set; }
        public string dutyParagraph { get; set; }

        public string addressCode { get; set; }
    }
}