using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SyncPushTenPM
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var url = ConfigurationManager.ConnectionStrings["WebUrl"].ToString();

            var url1 = url + "/P/TaskPlan/PushTwo";                 //确认收餐
            var url2 = url + "/P/TaskPlan/PushThree";               //上传文件
            var url3 = url + "/P/TaskPlan/PushFour";                //文件审批

            using (var client = new WebClient())
            {
                client.DownloadData(url1);
                client.DownloadData(url2);
                client.DownloadData(url3);
            }

        }
    }
}
