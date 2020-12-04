using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SysPushOne
{
   public class Program
    {
        public static void Main(string[] args)
        {
            var url = ConfigurationManager.ConnectionStrings["WebUrl"].ToString();
            var url1 = url + "/P/TaskPlan/PushOne";                 //送餐后一小时未收餐
            var url2 = url + "/P/TaskPlan/PushEight";               //收餐后一小时未上传文件

            var url3 = url + "/P/TaskPlan/SysConfirmOrder";               //自动收餐


            var url4 = url + "/P/TaskPlan/LoadAutoChangeFail";               //自动返回预定失败、修改失败、按原订单配送失败
            var url5 = url + "/P/TaskPlan/LoadAutoChangeSuccess";            //自动返回退订成功


            using (var client = new WebClient())
            {
                client.DownloadData(url1);
                client.DownloadData(url2);
                client.DownloadData(url3);
                client.DownloadData(url4);
                client.DownloadData(url5);
            }

        }

    }
}