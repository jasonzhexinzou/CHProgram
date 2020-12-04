

using System.Configuration;
using System.Net;

namespace UploadFileImage
{
    public class Program
    {
       public static void Main(string[] args)
        {
            var url = ConfigurationManager.ConnectionStrings["WebUrl"].ToString();
            var url1 = url + "P/UploadFileManagement/TimingExport";
            using (var client = new WebClient())
            {
                client.DownloadData(url1);
            }
        }
    }
}
