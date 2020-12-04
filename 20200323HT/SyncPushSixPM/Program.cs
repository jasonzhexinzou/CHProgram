using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SyncPushSixPM
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var url = ConfigurationManager.ConnectionStrings["WebUrl"].ToString();

            var url1 = url + "/P/TaskPlan/PushFive";                 //48小时未确认收餐
            var url2 = url + "/P/TaskPlan/PushSix";                  //确认收餐后7天未上传文件
            var url3 = url + "/P/TaskPlan/PushSeven";                //上传文件后7天未审批

            using (var client = new WebClient())
            {
                client.DownloadData(url1);
                client.DownloadData(url2);
                client.DownloadData(url3);
            }

        }
    }
}
