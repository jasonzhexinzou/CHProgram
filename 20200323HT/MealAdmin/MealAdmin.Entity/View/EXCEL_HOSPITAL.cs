using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity.View
{
    public class EXCEL_HOSPITAL
    {
        public string HospitalCode { get; set; }
        public string OldHospitalCode { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string HospitalName { get; set; }
        public string OldHospitelName { get; set; }
        public string HospitalAddress { get; set; }
        public string Market { get; set; }
        public string XMS { get; set; }
        public string BDS { get; set; }
        public string meituan { get; set; }
        public string Remark { get; set; }
        public int External { get; set; }
        public int ID { get; set; }
        public string RelateUserLIst { get; set; }
         
        public string MainAddress { get; set; }          //是否为主地址
        public string Code { get; set; }                 //实际使用的医院编码（医院编码加是否为主地址）

    }
}
