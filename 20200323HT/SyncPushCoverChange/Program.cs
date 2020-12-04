using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SyncPushCoverChange
{
    public class Program
    {
        static void Main(string[] args)
        {
            var url = ConfigurationManager.ConnectionStrings["WebUrl"].ToString();

            var url4 = url + "/P/TaskPlan/SendRestaurantChangeMessage";

            using (var client = new WebClient())
            {
                client.DownloadData(url4);
            }
        }
    }
}
