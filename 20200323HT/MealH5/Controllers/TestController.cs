using iPathFeast.API.Client;
using iPathFeast.ApiEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using XFramework.XInject.Attributes;

namespace MealH5.Controllers
{
    public class TestController : Controller
    {
        [Autowired]
        ApiV1Client apiClient { get; set; }

        // GET: Test
        public ContentResult Index()
        {

            #region 
            //var wc = XFramework.XUtil.ThreadWebClientFactory.GetWebClient();
            //wc.Encoding = Encoding.UTF8;
            //wc.TimeOut = 600000;
            //wc.Headers.Add("Content-Type", "application/json");

            //var param = "{\"requestData\":{\"supplier\":\"xms\",\"sign\":\"292e3aec6d6c32250a1f3ec5ea047a4d0b858259\",\"timestamp\":\"1510800715908\"}}";
            //var rel = wc.UploadData("https://wxm-dev.igskapp.com/NMealH5/P/CallBack/SyncHospital", "POST", Encoding.UTF8.GetBytes(param));

            //var json = Encoding.UTF8.GetString(rel);

            //var req = new GetReportReq()
            //{
            //    _Channels="bds",
            //    startTime="2017-11-13",
            //    endTime="2017-11-19",
            //    timeType="1"
            //};
            ////var guid = new Guid();

            //var res = apiClient.GetReport(req);
            #endregion

            var 请求 = new GetReportReq()
            {
                _Channels = "bds",
                startTime = "2017-11-27",
                endTime = "2017-12-03",
                timeType = "1"
            };
            var res = apiClient.GetReport(请求);
            

            return Content("ok");
        }

        //public JsonResult Test()
        //{
        //    var entity = new UserAddrInfoReq()
        //    {
        //        BNo = "123456789",
        //        UNo = "lisheng.han",
        //        UserName = "韩利胜",
        //        TelePhone = "17640426420",
        //        Address = "辽宁省大连市高新园区敬贤街26号",
        //        Latitude = "123.000",
        //        Longitude = "76.00",
        //        Type = 7,
        //        Extend = "{state = 1}"
        //    };

        //    var res = apiClient.SaveUserAddrInfo(entity);

        //    return Json(new { state = 0 });
        //}


    }
}