using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SyncReport
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var url = ConfigurationManager.ConnectionStrings["WebUrl"].ToString();
            var url1 = url + "/P/OrderMnt/SyncXMSReport";                 //同步小秘书日报
            var url2 = url + "/P/OrderMnt/SyncBDSReport";                 //同步商宴通日报

            using (var client = new WebClient())
            {
                client.DownloadData(url1);
                client.DownloadData(url2);
            }
        }
    }
}
