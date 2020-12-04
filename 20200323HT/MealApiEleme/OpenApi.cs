using MealApiInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MealApiInterface.Entity;
using MealApiEleme.Entity;
using System.Configuration;
using XFramework.XUtil;
using Newtonsoft.Json;
using System.Diagnostics;

namespace MealApiEleme
{
    internal class OpenApi : OpenApiBase
    {
        readonly string domain = ConfigurationManager.AppSettings["eleme_api"];
        readonly string gpg_path = ConfigurationManager.AppSettings["gpg_path"];
        readonly string gpg_pwd = ConfigurationManager.AppSettings["gpg_pwd"];

        #region 线程槽WebClient资源Key
        /// <summary>
        /// 线程槽WebClient资源Key
        /// </summary>
        public override string WEBCLIENTTHREADKEY
        {
            get
            {
                return "elemeclient";
            }
        }
        #endregion

        #region 参数签名
        /// <summary>
        /// 参数签名
        /// </summary>
        /// <param name="pu"></param>
        /// <param name="url"></param>
        /// <param name="consumer_secret"></param>
        /// <returns></returns>
        public static string SignIt(Dictionary<string, string> pu, string url, string consumer_secret)
        {
            foreach (var key in pu.Keys.ToArray())
            {
                var value = pu[key];
                pu[key] = value.UrlEncodeToUpper();
            }

            pu = pu.OrderBy(a => a.Key).ToDictionary(a => a.Key, b => b.Value);
            var stringA = string.Join("&", pu.Select(a => a.Key + "=" + a.Value));
            var stringB = url + "?" + stringA + consumer_secret;
            var byteB = Encoding.UTF8.GetBytes(stringB);
            var sb = new StringBuilder(byteB.Length * 2);
            foreach (var b in byteB)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            var stringC = sb.ToString();
            var sig = SHA1Helper.SHA1_Hash(stringC);

            return sig;
        }
        #endregion

        #region 下单代付签名
        /// <summary>
        /// 下单代付签名
        /// </summary>
        /// <param name="cart_id"></param>
        /// <param name="ip"></param>
        /// <param name="is_online_paid"></param>
        /// <param name="phones"></param>
        /// <param name="total"></param>
        /// <param name="tp_order_id"></param>
        /// <returns></returns>
        public string OrderValidation(string cart_id, string ip, string is_online_paid,
            string phones, string total, string tp_order_id)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("cart_id", cart_id);
            dic.Add("ip", ip);
            dic.Add("is_online_paid", is_online_paid);
            dic.Add("phones", phones);
            dic.Add("total", total);
            dic.Add("tp_order_id", tp_order_id);

            dic = dic.OrderBy(a => a.Key).ToDictionary(a => a.Key, b => b.Value);
            var stringA = string.Join("&", dic.Select(a => a.Key + "=" + a.Value));
            var _tmp_Path = AppDomain.CurrentDomain.BaseDirectory + @"_tmp\";
            if (!System.IO.Directory.Exists(_tmp_Path))
            {
                System.IO.Directory.CreateDirectory(_tmp_Path);
            }
            var _file_path = _tmp_Path + Guid.NewGuid().ToString();
            var _filesig_path = _file_path + ".sig";

            System.IO.File.WriteAllText(_file_path, stringA);

            string rel = string.Empty;
            try
            {
                Process p = new Process();
                string fileargs = " --yes --batch -b --passphrase-fd 0 " + _file_path;
                p.StartInfo.FileName = gpg_path + @"gpg.exe";
                p.StartInfo.Arguments = fileargs;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardInput = true;

                p.Start();
                p.StandardInput.Write(gpg_pwd);
                p.StandardInput.Write(Environment.NewLine);
                p.StandardInput.Flush();
                p.WaitForExit();

                var bytes = System.IO.File.ReadAllBytes(_filesig_path);

                var sb = new StringBuilder(bytes.Length * 2);
                foreach (var b in bytes)
                {
                    sb.AppendFormat("{0:x2}", b);
                }
                rel = sb.ToString();
            }
            catch (Exception e)
            {
                LogHelper.Error("手动捕获异常", e);
                throw e;
            }
            finally
            {
                try
                {
                    System.IO.File.Delete(_file_path);
                    System.IO.File.Delete(_filesig_path);
                }
                catch { }
            }
            return rel;
        }
        #endregion

        #region 搜索餐厅
        /// <summary>
        /// 搜索餐厅
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="geo"></param>
        /// <param name="category_id"></param>
        /// <param name="order_by"></param>
        /// <param name="new_restaurant"></param>
        /// <param name="busy_level"></param>
        /// <param name="consumer_key"></param>
        /// <param name="consumer_secret"></param>
        /// <returns></returns>
        public RestaurantsRes Restaurants(string keyword, string geo, string category_id, string order_by, string new_restaurant,
            string busy_level, string consumer_key, string consumer_secret)
        {
            var url = domain + "/restaurants/";
            var timestamp = DateTimeHelper.NowToJavaTimeMillis().ToString();

            Dictionary<string, string> pu = new Dictionary<string, string>();
            pu.Add("keyword", keyword);
            pu.Add("geo", geo);
            //pu.Add("restaurant_ids", "790529");
            if (!string.IsNullOrEmpty(category_id))
            {
                pu.Add("category_id", category_id);
            }
            if (!string.IsNullOrEmpty(order_by))
            {
                pu.Add("order_by", order_by);
            }
            if (!string.IsNullOrEmpty(new_restaurant))
            {
                pu.Add("new_restaurant", new_restaurant);
            }

            pu.Add("payment", "1,3");
            //pu.Add("invoice", "0");
            pu.Add("busy_level", busy_level);
            pu.Add("limit", "100");

            pu.Add("timestamp", timestamp);
            pu.Add("consumer_key", consumer_key);

            pu = pu.Where(a => !string.IsNullOrEmpty(a.Value)).ToDictionary(a => a.Key, b => b.Value);
            var sig = SignIt(pu, url, consumer_secret);
            pu.Add("sig", sig);

            return GetObject<RestaurantsRes>(url, pu);
        }
        #endregion

        #region 获取餐厅详情
        /// <summary>
        /// 获取餐厅详情
        /// </summary>
        /// <param name="restaurant_id"></param>
        /// <param name="consumer_key"></param>
        /// <param name="consumer_secret"></param>
        /// <returns></returns>
        public RestaurantRes Restaurant(string restaurant_id, string consumer_key, string consumer_secret)
        {
            var url = domain + "/restaurant/" + restaurant_id + "/";
            var timestamp = DateTimeHelper.NowToJavaTimeMillis().ToString();

            Dictionary<string, string> pu = new Dictionary<string, string>();

            pu.Add("timestamp", timestamp);
            pu.Add("consumer_key", consumer_key);

            var sig = SignIt(pu, url, consumer_secret);
            pu.Add("sig", sig);

            return GetObject<RestaurantRes>(url, pu);
        }
        #endregion

        #region 取得餐厅类别
        /// <summary>
        /// 取得餐厅类别
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="consumer_key"></param>
        /// <param name="consumer_secret"></param>
        /// <returns></returns>
        public RestaurantsRes Restaurants_Categories(string latitude, string longitude,
            string consumer_key, string consumer_secret)
        {
            var url = domain + "restaurant/categories/";
            var timestamp = DateTimeHelper.NowToJavaTimeMillis().ToString();

            Dictionary<string, string> pu = new Dictionary<string, string>();
            pu.Add("latitude", latitude);
            pu.Add("longitude", longitude);

            pu.Add("timestamp", timestamp);
            pu.Add("consumer_key", consumer_key);

            var sig = SignIt(pu, url, consumer_secret);
            pu.Add("sig", sig);

            return GetObject<RestaurantsRes>(url, pu);
        }
        #endregion

        #region 取得餐厅菜品
        /// <summary>
        /// 取得餐厅菜品
        /// </summary>
        /// <param name="restaurant_id"></param>
        /// <param name="consumer_key"></param>
        /// <param name="consumer_secret"></param>
        /// <returns></returns>
        public Restaurants_MenuRes Restaurants_Menu(string restaurant_id, string consumer_key, string consumer_secret)
        {
            var url = domain + "/restaurant/" + restaurant_id + "/menu/";
            var timestamp = DateTimeHelper.NowToJavaTimeMillis().ToString();

            Dictionary<string, string> pu = new Dictionary<string, string>();

            pu.Add("timestamp", timestamp);
            pu.Add("consumer_key", consumer_key);

            var sig = SignIt(pu, url, consumer_secret);
            pu.Add("sig", sig);

            return GetObject<Restaurants_MenuRes>(url, pu);
        }
        #endregion

        #region 创建购物车
        /// <summary>
        /// 创建购物车
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="foods"></param>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <param name="user_token"></param>
        /// <param name="device_id"></param>
        /// <param name="consumer_key"></param>
        /// <param name="consumer_secret"></param>
        /// <returns></returns>
        public CartRes Cart(string phone, List<FoodCartEntity> foods, string longitude, string latitude, string user_token, string device_id,
            string consumer_key, string consumer_secret)
        {
            var url = domain + "/cart/";
            var timestamp = DateTimeHelper.NowToJavaTimeMillis().ToString();
            var is_online_paid = "1";

            Dictionary<string, string> pu = new Dictionary<string, string>();
            pu.Add("timestamp", timestamp);
            pu.Add("consumer_key", consumer_key);

            var food = JsonConvert.SerializeObject(new { group = new object[] { foods } });
            pu.Add("phone", phone);
            pu.Add("food", food);
            pu.Add("longitude", longitude);
            pu.Add("latitude", latitude);
            pu.Add("user_token", user_token);
            pu.Add("device_id", device_id);
            pu.Add("is_online_paid", is_online_paid);

            var sig = SignIt(pu, url, consumer_secret);
            pu.Add("sig", sig);

            return PostObject<CartRes>(url, pu);
        }
        #endregion

        #region 修改购物车手机号码
        /// <summary>
        /// 修改购物车手机号码
        /// </summary>
        /// <param name="cart_id"></param>
        /// <param name="phone"></param>
        /// <param name="foods"></param>
        /// <param name="consumer_key"></param>
        /// <param name="consumer_secret"></param>
        public BaseEntity CartInfo(string cart_id, string phone, List<FoodCartEntity> foods,
            string consumer_key, string consumer_secret)
        {
            var url = domain + "/cart/" + cart_id + "/";
            var timestamp = DateTimeHelper.NowToJavaTimeMillis().ToString();

            var food = JsonConvert.SerializeObject(new { group = new object[] { foods } });
            var is_online_paid = "1";

            Dictionary<string, string> pu = new Dictionary<string, string>();
            pu.Add("phone", phone);
            pu.Add("food", food);
            pu.Add("is_online_paid", is_online_paid);

            pu.Add("timestamp", timestamp);
            pu.Add("consumer_key", consumer_key);

            pu = pu.Where(a => !string.IsNullOrEmpty(a.Value)).ToDictionary(a => a.Key, b => b.Value);
            var sig = SignIt(pu, url, consumer_secret);
            pu.Add("sig", sig);

            var rel = PutObject<BaseEntity>(url, pu);
            return rel;
        }
        #endregion

        #region 创建订单
        /// <summary>
        /// 创建订单
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
        /// <returns></returns>
        public OrderRes Order(string tp_order_id, string phones, string consignee, string address, string cart_id,
            string total, string longitude, string latitude, string ip, string description, string invoice, string deliver_time,
            string consumer_key, string consumer_secret)
        {
            var url = domain + "/order/";
            var timestamp = DateTimeHelper.NowToJavaTimeMillis().ToString();
            var is_online_paid = "1";
            var validation = OrderValidation(cart_id, ip, is_online_paid, phones, total, tp_order_id);

            Dictionary<string, string> pu = new Dictionary<string, string>();
            pu.Add("tp_order_id", tp_order_id);
            pu.Add("phones", phones);
            pu.Add("consignee", consignee);
            pu.Add("address", address);
            pu.Add("cart_id", cart_id);
            pu.Add("total", total);
            pu.Add("longitude", longitude);
            pu.Add("latitude", latitude);
            pu.Add("ip", ip);
            pu.Add("description", description);
            pu.Add("deliver_time", deliver_time);

            pu.Add("is_online_paid", is_online_paid);
            pu.Add("validation", validation);
            pu.Add("show_detail", "1");

            pu.Add("timestamp", timestamp);
            pu.Add("consumer_key", consumer_key);

            pu = pu.Where(a => !string.IsNullOrEmpty(a.Value)).ToDictionary(a => a.Key, b => b.Value);
            var sig = SignIt(pu, url, consumer_secret);
            pu.Add("sig", sig);

            return PostObject<OrderRes>(url, pu);
        }
        #endregion

        #region 设置在线支付状态
        /// <summary>
        /// 设置在线支付状态
        /// </summary>
        /// <param name="eleme_order_id"></param>
        /// <param name="consumer_key"></param>
        /// <param name="consumer_secret"></param>
        public BaseEntity PaymentStatus(string eleme_order_id, string consumer_key, string consumer_secret)
        {
            var url = domain + "/order/" + eleme_order_id + "/payment_status/";
            var timestamp = DateTimeHelper.NowToJavaTimeMillis().ToString();

            Dictionary<string, string> pu = new Dictionary<string, string>();

            pu.Add("status", "1");

            pu.Add("timestamp", timestamp);
            pu.Add("consumer_key", consumer_key);

            pu = pu.Where(a => !string.IsNullOrEmpty(a.Value)).ToDictionary(a => a.Key, b => b.Value);
            var sig = SignIt(pu, url, consumer_secret);
            pu.Add("sig", sig);

            var rel = PutObject<BaseEntity>(url, pu);
            return rel;
        }
        #endregion

        #region 查询饿了么单号
        /// <summary>
        /// 查询饿了么单号
        /// </summary>
        /// <param name="tp_order_ids"></param>
        /// <param name="consumer_key"></param>
        /// <param name="consumer_secret"></param>
        /// <returns></returns>
        public BaseEntity OrderId(string tp_order_ids, string consumer_key, string consumer_secret)
        {
            var url = domain + "/orders/tp_order_id/";
            var timestamp = DateTimeHelper.NowToJavaTimeMillis().ToString();

            Dictionary<string, string> pu = new Dictionary<string, string>();
            pu.Add("tp_order_ids", tp_order_ids);

            pu.Add("timestamp", timestamp);
            pu.Add("consumer_key", consumer_key);

            pu = pu.Where(a => !string.IsNullOrEmpty(a.Value)).ToDictionary(a => a.Key, b => b.Value);
            var sig = SignIt(pu, url, consumer_secret);
            pu.Add("sig", sig);

            return GetObject<BaseEntity>(url, pu);
        }
        #endregion

        #region 送餐跟踪
        /// <summary>
        /// 送餐跟踪
        /// </summary>
        /// <param name="eleme_order_id"></param>
        /// <param name="consumer_key"></param>
        /// <param name="consumer_secret"></param>
        public OrderTrackingRes OrderTracking(string eleme_order_id, string consumer_key, string consumer_secret)
        {
            var url = domain + "/order/tracking-info/";
            var timestamp = DateTimeHelper.NowToJavaTimeMillis().ToString();

            Dictionary<string, string> pu = new Dictionary<string, string>();

            pu.Add("eleme_order_id", eleme_order_id);

            pu.Add("timestamp", timestamp);
            pu.Add("consumer_key", consumer_key);

            pu = pu.Where(a => !string.IsNullOrEmpty(a.Value)).ToDictionary(a => a.Key, b => b.Value);
            var sig = SignIt(pu, url, consumer_secret);
            pu.Add("sig", sig);

            return GetObject<OrderTrackingRes>(url, pu);
        }
        #endregion

        #region 添加订单评价
        /// <summary>
        /// 添加订单评价
        /// </summary>
        /// <param name="eleme_order_id"></param>
        /// <param name="service_rating"></param>
        /// <param name="service_rating_text"></param>
        /// <param name="comment_time"></param>
        /// <param name="deliver_time"></param>
        /// <param name="consumer_key"></param>
        /// <param name="consumer_secret"></param>
        /// <returns></returns>
        public BaseEntity RatingOrder(string eleme_order_id, string service_rating, string service_rating_text, string comment_time, string deliver_time,
            string consumer_key, string consumer_secret)
        {
            var url = domain + "/rating/order/" + eleme_order_id + "/";
            var timestamp = DateTimeHelper.NowToJavaTimeMillis().ToString();

            Dictionary<string, string> pu = new Dictionary<string, string>();
            pu.Add("service_rating", service_rating);
            pu.Add("service_rating_text", service_rating_text);
            pu.Add("comment_time", comment_time);
            pu.Add("deliver_time", deliver_time);

            pu.Add("timestamp", timestamp);
            pu.Add("consumer_key", consumer_key);

            pu = pu.Where(a => !string.IsNullOrEmpty(a.Value)).ToDictionary(a => a.Key, b => b.Value);
            var sig = SignIt(pu, url, consumer_secret);
            pu.Add("sig", sig);

            return PostObject<BaseEntity>(url, pu);
        }
        #endregion

        #region 获取餐厅评价
        /// <summary>
        /// 获取餐厅评价
        /// </summary>
        /// <param name="restaurant_id"></param>
        /// <param name="consumer_key"></param>
        /// <param name="consumer_secret"></param>
        /// <returns></returns>
        public RatingRestaurantRes LoadRatingRestaurant(string restaurant_id, int has_content, int record_type, int offset, int limit,
            string consumer_key, string consumer_secret)
        {
            var url = domain + "/rating/restaurant/" + restaurant_id + "/";
            var timestamp = DateTimeHelper.NowToJavaTimeMillis().ToString();

            Dictionary<string, string> pu = new Dictionary<string, string>();
            pu.Add("has_content", has_content.ToString());
            pu.Add("record_type", record_type.ToString());
            pu.Add("offset", offset.ToString());
            pu.Add("limit", limit.ToString());

            pu.Add("timestamp", timestamp);
            pu.Add("consumer_key", consumer_key);

            pu = pu.Where(a => !string.IsNullOrEmpty(a.Value)).ToDictionary(a => a.Key, b => b.Value);
            var sig = SignIt(pu, url, consumer_secret);
            pu.Add("sig", sig);

            return GetObject<RatingRestaurantRes>(url, pu);
        }
        #endregion

        #region 获取餐厅评价分类
        /// <summary>
        /// 获取餐厅评价分类
        /// </summary>
        /// <param name="restaurant_id"></param>
        /// <param name="consumer_key"></param>
        /// <param name="consumer_secret"></param>
        /// <returns></returns>
        public RatingCategoriesRes RatingCategories(string restaurant_id, string consumer_key, string consumer_secret)
        {
            var url = domain + "/rating/restaurant/" + restaurant_id + "/categories";
            var timestamp = DateTimeHelper.NowToJavaTimeMillis().ToString();

            Dictionary<string, string> pu = new Dictionary<string, string>();

            pu.Add("timestamp", timestamp);
            pu.Add("consumer_key", consumer_key);

            pu = pu.Where(a => !string.IsNullOrEmpty(a.Value)).ToDictionary(a => a.Key, b => b.Value);
            var sig = SignIt(pu, url, consumer_secret);
            pu.Add("sig", sig);

            return GetObject<RatingCategoriesRes>(url, pu);
        }
        #endregion

        #region 获取餐品评价
        /// <summary>
        /// 获取餐品评价
        /// </summary>
        /// <param name="restaurant_id"></param>
        /// <param name="food_id"></param>
        /// <param name="has_content"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="consumer_key"></param>
        /// <param name="consumer_secret"></param>
        /// <returns></returns>
        public RatingFoodRes LoadRatingFood(string restaurant_id, string food_id, int has_content, int offset, int limit,
            string consumer_key, string consumer_secret)
        {
            var url = domain + "/rating/food_ratings";
            var timestamp = DateTimeHelper.NowToJavaTimeMillis().ToString();

            Dictionary<string, string> pu = new Dictionary<string, string>();
            pu.Add("restaurant_id", restaurant_id);
            pu.Add("food_id", food_id);
            pu.Add("has_content", has_content.ToString());
            pu.Add("offset", offset.ToString());
            pu.Add("limit", limit.ToString());

            pu.Add("timestamp", timestamp);
            pu.Add("consumer_key", consumer_key);

            pu = pu.Where(a => !string.IsNullOrEmpty(a.Value)).ToDictionary(a => a.Key, b => b.Value);
            var sig = SignIt(pu, url, consumer_secret);
            pu.Add("sig", sig);

            return GetObject<RatingFoodRes>(url, pu);
        }
        #endregion

        #region 添加订单评价
        /// <summary>
        /// 添加订单评价
        /// </summary>
        /// <param name="eleme_order_id"></param>
        /// <param name="service_rating"></param>
        /// <param name="service_rating_text"></param>
        /// <param name="comment_time"></param>
        /// <param name="deliver_time"></param>
        /// <param name="consumer_key"></param>
        /// <param name="consumer_secret"></param>
        /// <returns></returns>
        public RatingOrderRes RatingOrder(string eleme_order_id, int service_rating, string service_rating_text, string comment_time, string deliver_time,
            string consumer_key, string consumer_secret)
        {
            var url = domain + $"/rating/order/{eleme_order_id}/";
            var timestamp = DateTimeHelper.NowToJavaTimeMillis().ToString();

            Dictionary<string, string> pu = new Dictionary<string, string>();
            pu.Add("service_rating", service_rating.ToString());
            pu.Add("service_rating_text", service_rating_text);
            pu.Add("comment_time", comment_time);
            pu.Add("deliver_time", deliver_time);

            pu.Add("timestamp", timestamp);
            pu.Add("consumer_key", consumer_key);

            pu = pu.Where(a => !string.IsNullOrEmpty(a.Value)).ToDictionary(a => a.Key, b => b.Value);
            var sig = SignIt(pu, url, consumer_secret);
            pu.Add("sig", sig);

            return PostObject<RatingOrderRes>(url, pu);
        }
        #endregion

        #region 提交餐品评价
        /// <summary>
        /// 提交餐品评价
        /// </summary>
        /// <param name="order_id"></param>
        /// <param name="food_ratings"></param>
        /// <param name="consumer_key"></param>
        /// <param name="consumer_secret"></param>
        /// <returns></returns>
        public SubmitRatingFoodRes SubmitRatingFoods(string order_id, List<Entity.FoodRating> food_ratings,
            string consumer_key, string consumer_secret)
        {
            var url = domain + "/rating/foods";
            var timestamp = DateTimeHelper.NowToJavaTimeMillis().ToString();

            Dictionary<string, string> pu = new Dictionary<string, string>();
            pu.Add("order_id", order_id);
            pu.Add("food_ratings", JsonConvert.SerializeObject(food_ratings));

            pu.Add("timestamp", timestamp);
            pu.Add("consumer_key", consumer_key);

            pu = pu.Where(a => !string.IsNullOrEmpty(a.Value)).ToDictionary(a => a.Key, b => b.Value);
            var sig = SignIt(pu, url, consumer_secret);
            pu.Add("sig", sig);

            return PostObject<SubmitRatingFoodRes>(url, pu);
        }
        #endregion

        #region 查询订单详情
        /// <summary>
        /// 查询订单详情
        /// </summary>
        /// <param name="eleme_order_id"></param>
        /// <param name="consumer_key"></param>
        /// <param name="consumer_secret"></param>
        /// <returns></returns>
        public FindOrderRes FindOrder(string eleme_order_id, string consumer_key, string consumer_secret)
        {
            var url = domain + "/order/" + eleme_order_id;
            var timestamp = DateTimeHelper.NowToJavaTimeMillis().ToString();

            Dictionary<string, string> pu = new Dictionary<string, string>();

            pu.Add("timestamp", timestamp);
            pu.Add("consumer_key", consumer_key);

            pu = pu.Where(a => !string.IsNullOrEmpty(a.Value)).ToDictionary(a => a.Key, b => b.Value);
            var sig = SignIt(pu, url, consumer_secret);
            pu.Add("sig", sig);

            return GetObject<FindOrderRes>(url, pu);
        }
        #endregion

        #region 查询订单状态
        /// <summary>
        /// 查询订单状态
        /// </summary>
        /// <param name="eleme_order_id"></param>
        /// <param name="consumer_key"></param>
        /// <param name="consumer_secret"></param>
        /// <returns></returns>
        public OrderStateRes OrderState(string eleme_order_id, string consumer_key, string consumer_secret)
        {
            var url = domain + $"/order/{eleme_order_id}/status";
            var timestamp = DateTimeHelper.NowToJavaTimeMillis().ToString();

            Dictionary<string, string> pu = new Dictionary<string, string>();

            pu.Add("timestamp", timestamp);
            pu.Add("consumer_key", consumer_key);

            pu = pu.Where(a => !string.IsNullOrEmpty(a.Value)).ToDictionary(a => a.Key, b => b.Value);
            var sig = SignIt(pu, url, consumer_secret);
            pu.Add("sig", sig);

            return GetObject<OrderStateRes>(url, pu);
        }
        #endregion

        #region 修改订单状态
        /// <summary>
        /// 修改订单状态
        /// </summary>
        /// <param name="eleme_order_id"></param>
        /// <param name="status"></param>
        /// <param name="consumer_key"></param>
        /// <param name="consumer_secret"></param>
        /// <returns></returns>
        public BaseEntity SetOrderState(string eleme_order_id, int status, string consumer_key, string consumer_secret)
        {
            var url = domain + $"/order/{eleme_order_id}/status";
            var timestamp = DateTimeHelper.NowToJavaTimeMillis().ToString();

            Dictionary<string, string> pu = new Dictionary<string, string>();
            pu.Add("status", status.ToString());
            if (status == -1)
            {
                // 取消订单时候应提供取消类型：用户消单
                pu.Add("type", "1");
            }


            pu.Add("timestamp", timestamp);
            pu.Add("consumer_key", consumer_key);

            pu = pu.Where(a => !string.IsNullOrEmpty(a.Value)).ToDictionary(a => a.Key, b => b.Value);
            var sig = SignIt(pu, url, consumer_secret);
            pu.Add("sig", sig);

            return PutObject<BaseEntity>(url, pu);
        }
        #endregion

    }
}
