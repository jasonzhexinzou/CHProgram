using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;

namespace SyncInvalidateAnHour
{
    class Program
    {
        public static void Main(string[] args)
        {
            var url = ConfigurationManager.ConnectionStrings["WebUrl"].ToString();

            var url1 = url + "/P/TaskPlan/Invalidate";                 //每小时更新超过5个自然日未审批的地址申请数据

            using (var client = new WebClient())
            {
                client.DownloadData(url1);
            }
        }
    }
}
