using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity.Helper
{
    public class P_WeChatOrder
    {
        public string inTime { get; set; }
        //供应商
        public string supplier { get; set; }
        //发票抬头
        public string invoiceTitle { get; set; }
        //纳税人识别号
        public string dutyParagraph { get; set; }
        //预申请信息
        public P_PreApproval preApproval { get; set; }
        //预购餐品
        public P_Food foods { get; set; }
        //
        public P_OrderDetails details { get; set; }
        //医院信息
        public P_HOSPITAL hospital { get; set; }
        //文件上传信息
        public P_PREUPLOADORDER orderInfo { get; set; }
    }
}
