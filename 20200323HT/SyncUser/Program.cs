using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SyncUser
{
    public class Program
    {
        static void Main(string[] args)
        {
            var url = ConfigurationManager.ConnectionStrings["WebUrl"].ToString();

            var url4 = url + "/P/Personnel/SyncQyUser";

            using (var client = new WebClient())
            {
                client.DownloadData(url4);
            }
        }
    }
}
