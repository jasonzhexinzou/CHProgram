using MealAdmin.Entity;
using MeetingMealApiClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MealOrdering.Test
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var bdClient = MealAdminApiClient.BaseDataClientChannelFactory.GetChannel();

            var res = bdClient.LoadProvince("Rx");

            var res1 = bdClient.LoadCity(15,"Rx");

            //var res2 = bdClient.LoadHospital(100000, "Rx");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var orderChannel = MealAdminApiClient.OrderApiClientChannelFactory.GetChannel();
            Guid _id = new Guid("6110151A-42C0-4874-A3B7-487A86B2DAF3");
            var order = orderChannel.FindByID(_id);

            

            var client = MealAdminApiClient.EvaluateClientChannelFactory.GetChannel();

            Guid id = new Guid("FC81BD4C-711C-4F8B-8E3B-C0FEA6EF4A66");
            var res = client.LoadByOrderID(id);
            P_EVALUATE entity = new P_EVALUATE();
            entity.ID = Guid.NewGuid();
            entity.OrderID = Guid.NewGuid();
            entity.RestaurantId = "10001";
            entity.State = 1;
            entity.CreateDate = DateTime.Now;

            var res1 = client.Add(entity);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var channel = MealAdminApiClient.UserInfoClientChannelFactory.GetChannel();

            var res2 = channel.Edit("TJ_011","18877776666","");



            P_USERINFO entity = new P_USERINFO();
            entity.ID = Guid.NewGuid();
            entity.UserId = "TJ_011";
            entity.Name = "韩利胜";
            entity.PhoneNumber = "18393911179";
            entity.CreateDate = DateTime.Now;
            var res = channel.Add(entity);


            var res1 = channel.FindByUserId("TJ_011");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var openApiChannel = OpenApiChannelFactory.GetChannel();
            var menu = openApiChannel.queryResFood("Delivery0028084");



            var oponapiChannel = OpenApiChannelFactory.GetChannel();
            var xmlres = oponapiChannel.orderDeliveryFailure("2017052015522374007", "饿死了");


            var channel = MealAdminApiClient.EvaluateClientChannelFactory.GetChannel();
            var res = channel.LoadByResId("Delivery0027612");



            var channel1 = MealAdminApiClient.OrderApiClientChannelFactory.GetChannel();
            Guid id = new Guid("E2B15FDA-8700-402A-9F0D-BCF5D3FDA998");
            var res1 = channel1.MMCoEResult(id,3, "");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var openApiChannel = OpenApiChannelFactory.GetChannel();

            var res = openApiChannel.GetReport("2017-05-18", "2017-05-24", "0");



            var appId = "1";
            var secret = "Z3NrdGVzdA==";

            //var little = ;
            var rel = little.getToken(appId, secret);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ////UrlMapping / OAuth2Wx
            //window.location.href = "https://catering-prod.igskapp.com/MealH5/UrlMapping/OAuth2Wx";
        }
    }
}
