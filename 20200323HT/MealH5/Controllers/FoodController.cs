using MealApiInterface.Entity;
using MealApiServiceClient;
using MealH5.Filter;
using MealH5.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Mvc;

namespace MealH5.Controllers
{
    [WxSessionFilter(MappingKey = "0x9060")]
    public class FoodController : Controller
    {
        #region 订餐主页
        /// <summary>
        /// 订餐主页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.currentuser = new
            {
                Name = "凯撒",
                PhoneNumber = "15641190204",
                ID = Guid.Empty
            };
            return View();
        }
        #endregion

        #region 载入常用地址
        /// <summary>
        /// 载入常用地址
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string LoadFrequentAddress(string district)
        {
            Response.AddHeader("Content-Type", "application/json; charset=utf-8");
            return "{\"state\":1,\"rows\":[{\"name\":\"普兰店站南山路9号\",\"geo\":\"121.960266,39.401707\",\"address\":\" \",\"phone\":\"15754021610\"},{\"name\":\"大连腾讯大厦敬贤街26号\",\"geo\":\"121.523812,38.861629\",\"address\":\"\",\"phone\":\"15754021610\"},{\"name\":\"大连腾讯大厦敬贤街26号\",\"geo\":\"121.523812,38.861629\",\"address\":\"505室\",\"phone\":\"15641190204\"}]}";
        }
        #endregion

        #region 载入坐标附近餐厅
        /// <summary>
        /// 载入坐标附近餐厅
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="geo"></param>
        /// <param name="category_id"></param>
        /// <param name="order_by"></param>
        /// <param name="new_restaurant"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LoadRestaurant(string keyword, string geo, string category_id, string order_by, string new_restaurant)
        {
            var openapi = OpenApiClientChannelFactory.GetChannel();
            var res = openapi.LoadRestaurant(MealChannelHandler.GetNowChannelCode(), keyword, geo, category_id, order_by, new_restaurant);

            return Json(new { state = 1, rows = res });
        }
        #endregion

        #region 载入餐厅菜单
        /// <summary>
        /// 载入餐厅菜单
        /// </summary>
        /// <param name="restaurant_id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LoadRestaurantMenu(string restaurant_id)
        {
            var openapi = OpenApiClientChannelFactory.GetChannel();
            var res = openapi.LoadRestaurantMenu(MealChannelHandler.GetNowChannelCode(), restaurant_id);

            return Json(new { state = 1, rows = res });
        }
        #endregion

        #region 载入餐厅信息
        /// <summary>
        /// 载入餐厅信息
        /// </summary>
        /// <param name="restaurant_id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult FindRestaurantInfo(string restaurant_id)
        {
            var openapi = OpenApiClientChannelFactory.GetChannel();
            var res = openapi.FindRestaurant(MealChannelHandler.GetNowChannelCode(), restaurant_id);

            return Json(new { state = 1, data = res });
        }
        #endregion

        #region 载入餐厅评价
        /// <summary>
        /// 载入餐厅评价
        /// </summary>
        /// <param name="restaurant_id"></param>
        /// <returns></returns>
        [HttpPost]
        public string LoadRestaurantRating(string restaurant_id)
        {
            Response.AddHeader("Content-Type", "application/json; charset=utf-8");
            return "{\"state\":1}";
        }
        #endregion

        #region 创建购物车
        /// <summary>
        /// 创建购物车
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <param name="food"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CreateCart(string phone, string longitude, string latitude, List<FoodCart> food)
        {
            var openapi = OpenApiClientChannelFactory.GetChannel();
            var res = openapi.CreateCart(MealChannelHandler.GetNowChannelCode(), phone, food, longitude, latitude, "", "0x9874");

            return Json(new { state = 1, data = res });
        }
        #endregion

        #region 下单
        /// <summary>
        /// 下单
        /// </summary>
        /// <param name="tp_order_id"></param>
        /// <param name="phones"></param>
        /// <param name="consignee"></param>
        /// <param name="address"></param>
        /// <param name="cart_id"></param>
        /// <param name="total"></param>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <param name="ip"></param>
        /// <param name="description"></param>
        /// <param name="invoice"></param>
        /// <param name="costcenter"></param>
        /// <param name="meetingcenter"></param>
        /// <param name="eat_description"></param>
        /// <param name="restaurant_image_url"></param>
        /// <param name="deliver_times"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Order(string tp_order_id, string phones, string consignee, string address, string cart_id,
            string total, string longitude, string latitude, string ip, string description, string invoice,
            Guid costcenter, Guid meetingcenter, string eat_description, string restaurant_image_url, string deliver_times,
            List<FoodCart> food)
        {
            var ID = Guid.NewGuid();
            tp_order_id = ID.ToString();

            var openapi = OpenApiClientChannelFactory.GetChannel();
            var order = openapi.Order(MealChannelHandler.GetNowChannelCode(), tp_order_id, phones, consignee, address, cart_id,
                total, longitude, latitude, ip, description, invoice, deliver_times, food);

            if (order != null)
            {
                if (order.error_code == 0 && !string.IsNullOrEmpty(order.order_id))
                {
                    // 下单成功
                    // 1.修改支付状态
                    var paystate = openapi.Payment(MealChannelHandler.GetNowChannelCode(), order.order_id, tp_order_id);
                    if (paystate == 200)
                    {
                        // 支付成功
                        order.error_msg = "下单成功，支付成功！";
                    }
                    else
                    {
                        // 支付失败
                        order.error_msg = "下单成功，支付失败！";
                        return Json(new { state = 0, txt = "下单支付失败！" });
                    }

                    

                }
                return Json(new { state = 1, data = order });
            }


            return Json(new { state = 1, data = new { error_code = 0} });
        }
        #endregion
    }
}