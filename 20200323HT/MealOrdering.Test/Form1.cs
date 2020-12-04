using LittleSecretary;
using MealAdminApiClient;
using MeetingMealApiClient;
using MeetingMealEntity;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MealOrdering.Test
{
    class little : LittleSecretaryAPI { }


    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                    (sender, certificate, chain, errors) => true;
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            var channel = OpenApiChannelFactory.GetChannel();
            var endTime = DateTime.Now.ToString("yyyy-MM-dd");
            var startTime = DateTime.Now.AddDays(-6).ToString("yyyy-MM-dd");
            var report = channel.GetReport(startTime, endTime, "1");



            var openApiChannel = OpenApiChannelFactory.GetChannel();

            var res = openApiChannel.GetReport("2017-05-18", "2017-05-24", "0");



            var appId = "1";
            var secret = "Z3NrdGVzdA==";

            //var little = ;
            var rel = little.getToken(appId, secret);
            //var rel0 = little.syncCity(rel.result.token, secret);
            //var rel2 = little.syncHospital(rel.result.token, secret, "0");
            //var rel3 = little.queryRes(rel.result.token, secret, rel2.result[0].hospitalId, rel2.result[0].address, "0", "0");
            //var rel4 = little.queryResFood(rel.result.token, secret, rel3.result[1].resId);
            //var foodlist = new FoodRequest[]
            //{
            //    new FoodRequest()
            //    {
            //        foodId = rel4.result.menuList[0].foods[2].foodId,
            //        foodName = string.Empty,
            //        count = "100"
            //    }
            //};
            //var rel5 = little.calculateFee(rel.result.token, secret, rel2.result[0].hospitalId, rel3.result[1].resId, foodlist);
            //var ID = Guid.NewGuid();

            //var rel6 = little.createOrder(rel.result.token, secret, ID.ToString(), string.Empty, "0", "2017-04-20 13:00:00", 
            //    rel5.result.foodFee.ToString(), rel5.result.packageFee.ToString(), rel5.result.sendFee.ToString(), "0", rel5.result.allPrice.ToString(), 
            //    "上海途径信息技术有限公司大连分公司", "2017-04-19 13:54:00",
            //    string.Empty, "xyz", "5", "100000", "1", "15641190204", "高新街腾讯大厦", rel3.result[1].resId, "", "0", "0", rel2.result[0].hospitalId,
            //    "cn0000001", "500", "abcdefg", "xaujqj", foodlist);

            //var rel7 = little.cancleOrder(rel.result.token, secret, rel6.result.xmsOrderId, "xasdqwqwe");
            //var rel8 = little.finishOrder(rel.result.token, secret, rel6.result.xmsOrderId, "0", "xxxxxxx");
            var rel9 = little.OrderQuery(rel.result.token, secret, "2017-05-18", "2017-05-24", "0");
            var rel10 = little.GetReport(rel.result.token, secret, "2017-05-18", "2017-05-24", "0");


            var client = OpenApiChannelFactory.GetChannel();

            //var _rel = client.calculateFee(rel2.result[0].hospitalId, rel3.result[1].resId, foodlist);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //var client = OrderApiClientChannelFactory.GetChannel();
            //int total = 0;
            //var rel = client.LoadByUserId("bingyin.jiang", Convert.ToDateTime("2017-01-01"), Convert.ToDateTime("2018-01-01"), "1,2,3", 10, 1, out total);



            
        }
    }
    
}
