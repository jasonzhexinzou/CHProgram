using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SendBriefReport
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var url = ConfigurationManager.ConnectionStrings["WebUrl"].ToString();

            var url1 = url + "/P/TaskPlan/SendBriefReport";   
           
            using (var client = new WebClient())
            {
                client.DownloadData(url1);
            }
        }
    }
}
