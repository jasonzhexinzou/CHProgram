using System.Configuration;
using System.Net;

namespace UpdateMasterData
{
    class Program
    {
        public static void Main(string[] args)
        {
            var url = ConfigurationManager.ConnectionStrings["WebUrl"].ToString();

            var url1 = url + "/P/TaskPlan/ReadRxTemp";                 //更新主数据Rx部分数据
            var url2 = url + "/P/TaskPlan/ReadVxTemp";                 //更新主数据Vx部分数据
            var url3 = url + "/P/TaskPlan/ReadDDTTemp";                //更新主数据DDT部分数据
            var url4 = url + "/P/TaskPlan/ReadTSKFTemp";               //更新主数据TSKF部分数据

            using (var client = new WebClient())
            {
                client.DownloadData(url1);
                client.DownloadData(url2);
                client.DownloadData(url3);
                client.DownloadData(url4);
            }

        }
    }
}
