using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PostHospitalVariables
{
    class Program
    {
        public static void Main(string[] args)
        {
            var url = ConfigurationManager.ConnectionStrings["WebUrl"].ToString();

            var url1 = url + "/P/TaskPlan/SyncHospitalChangedXMS";                 //Post医院变量数据至XMS
            var url2 = url + "/P/TaskPlan/SyncHospitalChangedBDS";                 //Post医院变量数据至BDS

            using (var client = new WebClient())
            {
                client.DownloadData(url1);
                client.DownloadData(url2);
            }

        }
    }
}