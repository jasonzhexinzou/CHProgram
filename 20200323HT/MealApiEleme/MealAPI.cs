using MealApiInterface;
using MealApiInterface.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XFramework.XUtil;

namespace MealApiEleme
{
    /// <summary>
    /// 订餐对外API
    /// </summary>
    public class MealAPI : IMealAPI
    {
        readonly string consumer_key = ConfigurationManager.AppSettings["eleme_consumer_key"];
        readonly string consumer_secret = ConfigurationManager.AppSettings["eleme_consumer_secret"];
        OpenApi openApi = new OpenApi();

        #region 初始化参数
        public static Dictionary<int, string> order_err_msg = new Dictionary<int, string>();
        static MealAPI()
        {
            order_err_msg.Add(0, "表示没有错误");
            order_err_msg.Add(1, "未知错误");
            order_err_msg.Add(2, "描述中不能有电话号码存在");
            order_err_msg.Add(3, "收货人不能有电话号码存在");
            order_err_msg.Add(4, "收货地址不能有电话号码存在");
            order_err_msg.Add(5, "地址无效");
            order_err_msg.Add(6, "预订时间无效");
            order_err_msg.Add(7, "电话号码无效");
            order_err_msg.Add(8, "订单金额过低");
            order_err_msg.Add(9, "餐厅仅支持在线支付");
            order_err_msg.Add(10, "无效的餐厅");
            order_err_msg.Add(11, "抱歉，不巧餐厅已打烊了，换家餐厅试试吧");
            order_err_msg.Add(12, "餐厅不接受此时段的预订");
            order_err_msg.Add(13, "餐厅不接受优惠券");
            order_err_msg.Add(14, "餐厅不接受网上订单");
            order_err_msg.Add(15, "餐厅不支持开发票");
            order_err_msg.Add(16, "餐厅不接受在线支付");
            order_err_msg.Add(17, "食物库存不足");
            order_err_msg.Add(18, "同一设备和手机号24小时内只能下7张订单");
            order_err_msg.Add(19, "生成订单失败");
            order_err_msg.Add(20, "购物车没有添加任何食物");
            order_err_msg.Add(21, "食物不存在");
            order_err_msg.Add(22, "红包不存在");
            order_err_msg.Add(23, "无法使用该红包");
            order_err_msg.Add(24, "红包仅限在线支付使用");
            order_err_msg.Add(25, "食物无效");
            order_err_msg.Add(26, "订单总价太少了，再去选点好吃的吧");
            order_err_msg.Add(27, "起送价增加，首页定位换成收货地址试试");
            order_err_msg.Add(28, "食物已经不再售卖");
            order_err_msg.Add(29, "购物车和订单支付方式不一致");
        }
        #endregion

        #region 查询餐厅
        /// <summary>
        /// 查询餐厅
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="geo"></param>
        /// <param name="category_id"></param>
        /// <param name="order_by"></param>
        /// <param name="new_restaurant"></param>
        /// <returns></returns>
        public List<Restaurant> LoadRestaurant(string keyword, string geo, string category_id, string order_by, string new_restaurant)
        {
            var apirel = openApi.Restaurants(keyword, geo, category_id, order_by, new_restaurant, 
                "0", consumer_key, consumer_secret);
            if (apirel.code != 200 || apirel.data == null
                || apirel.data.restaurants == null
                || apirel.data.restaurants.Count < 1)
            {
                return null;
            }
            var list = apirel.data.restaurants.Select(a => new Restaurant()
            {
                restaurant_id = a.restaurant_id.ToString(),
                agent_fee = a.agent_fee.ToString(),
                deliver_amount = a.deliver_amount.ToString(),
                image_url = a.image_url,
                deliver_spent = a.deliver_spent,
                num_rating = average(a.num_ratings),
                restaurant_name = a.restaurant_name,
                recent_order_num = a.recent_order_num,
                distances = 0
            }).ToList();

            return list;
        }
        /// <summary>
        /// 计算平均数
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private float average(List<int> list)
        {
            int sum = 0, count = 0;
            for (var i = 0; i < list.Count; i++)
            {
                count += list[i];
                sum += list[i] * (i + 1);
            }
            if (count == 0 || sum == 0)
                return 0;
            return sum * 1.0f / count;
        }
        #endregion

        #region 查询餐厅详情
        /// <summary>
        /// 查询餐厅详情
        /// </summary>
        /// <param name="restaurant_id"></param>
        /// <returns></returns>
        public Restaurant FindRestaurant(string restaurant_id)
        {
            var apirel = openApi.Restaurant(restaurant_id, consumer_key, consumer_secret);
            if (apirel.code != 200 || apirel.data == null)
            {
                return null;
            }

            var restaurant = new Restaurant();

            restaurant.restaurant_id = apirel.data.restaurant.restaurant_id.ToString();
            restaurant.agent_fee = apirel.data.restaurant.agent_fee.ToString();
            restaurant.deliver_amount = apirel.data.restaurant.deliver_amount.ToString();
            restaurant.image_url = apirel.data.restaurant.image_url;
            restaurant.deliver_spent = apirel.data.restaurant.deliver_spent;
            restaurant.num_rating = average(apirel.data.restaurant.num_ratings);
            restaurant.restaurant_name = apirel.data.restaurant.restaurant_name;
            restaurant.recent_order_num = apirel.data.restaurant.recent_order_num;
            restaurant.distances = 10;
            restaurant.serving_time = apirel.data.restaurant.serving_time;
            restaurant.is_bookable = apirel.data.restaurant.is_bookable;
            restaurant.deliver_times = apirel.data.restaurant.deliver_times;
            restaurant.mobile = apirel.data.restaurant.mobile;
            restaurant.phone_list = apirel.data.restaurant.phone_list;
            restaurant.promotion_info = apirel.data.restaurant.promotion_info;
            restaurant.address_text = apirel.data.restaurant.address_text;
            return restaurant;
        }
        #endregion

        #region 查询餐厅菜品
        /// <summary>
        /// 查询餐厅菜品
        /// </summary>
        /// <param name="restaurant_id"></param>
        /// <returns></returns>
        public List<RestaurantFoodCategory> LoadRestaurantMenu(string restaurant_id)
        {
            var apirel = openApi.Restaurants_Menu(restaurant_id, consumer_key, consumer_secret);
            if (apirel.code != 200 || apirel.data == null
                || apirel.data.restaurant_menu == null
                || apirel.data.restaurant_menu.Count < 1)
            {
                return null;
            }

            var list = apirel.data.restaurant_menu.Select(a => new RestaurantFoodCategory()
            {
                id = a.category_id.ToString(),
                name = a.category,
                description = a.description,
                foods = a.foods.Select(b => new RestaurantFood()
                {
                    id = b.id.ToString(),
                    name = b.name,
                    description = b.description,
                    image_url = b.image_url,
                    price = b.price,
                    rating = b.rating,
                    sales = b.sales
                }).ToList()
            }).ToList();

            return list;
        }
        #endregion

        #region 查询餐厅评价综述
        /// <summary>
        /// 查询餐厅评价综述
        /// </summary>
        /// <param name="restaurant_id"></param>
        /// <returns></returns>
        public List<RatingOverview> FindRestaurantRatingOverview(string restaurant_id)
        {
            var apirel = openApi.RatingCategories(restaurant_id, consumer_key, consumer_secret);
            if (apirel.code == 200 && apirel.data != null)
            {
                apirel.data.categories.Select(
                    a => new RatingOverview()
                    {
                        amount = a.amount,
                        name = a.name,
                        record_type = a.record_type
                    }).ToList();
            }
            return null;
        }
        #endregion

        #region 载入餐厅评价详情
        /// <summary>
        /// 载入餐厅评价详情
        /// </summary>
        /// <param name="restaurant_id"></param>
        /// <param name="has_content"></param>
        /// <param name="record_type"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public List<Rating> LoadRestaurantRating(string restaurant_id, int has_content, int record_type, int offset, int limit)
        {
            var apirel = openApi.LoadRatingRestaurant(restaurant_id, has_content, record_type, offset, limit, consumer_key, consumer_secret);
            if (apirel.code == 200 && apirel.data != null)
            {
                List<Rating> ratings = new List<Rating>();
                foreach(var item in apirel.data.ratings)
                {
                    var rating = new Rating();
                    rating.id = item.rating_id.ToString();
                    rating.from = item.is_from_eleme ? "eleme" : "other";
                    rating.rated_at = item.rated_at;
                    rating.rating_star = item.rating_star;
                    rating.rating_text = item.rating_text;
                    rating.user_name = item.user_name;
                    rating.foods = item.foods.Select(a => new RatingFood()
                    {
                        food_id = a.food_id.ToString(),
                        rating = a.rating,
                        rating_text = a.rating_text,
                        food_name = a.food_name,
                        image_url = a.image_url,
                        rated_at = a.rated_at
                    }).ToList();
                }

                return ratings;
            }
            return null;
        }
        #endregion

        #region 查询餐品评价综述
        /// <summary>
        /// 查询餐品评价综述
        /// </summary>
        /// <param name="food_id"></param>
        /// <returns></returns>
        public RatingOverview FindFoodRatingOverview(string food_id)
        {
            return null;
        }
        #endregion

        #region 载入餐品评价详情
        /// <summary>
        /// 载入餐品评价详情
        /// </summary>
        /// <param name="restaurant_id"></param>
        /// <param name="food_id"></param>
        /// <param name="has_content"></param>
        /// <param name="record_type"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public List<RatingFood> LoadFoodRating(string restaurant_id, string food_id, int has_content, int record_type, int offset, int limit)
        {
            var apirel = openApi.LoadRatingFood(restaurant_id, food_id, has_content, offset, limit, consumer_key, consumer_secret);
            if (apirel.code == 200 && apirel.data != null)
            {
                return apirel.data.Select(a => new RatingFood()
                {
                    food_id = a.food_id.ToString(),
                    rating = a.rating,
                    rating_text = a.rating_text,
                    food_name = a.food_name,
                    image_url = a.image_url,
                    rated_at = a.rated_at
                }).ToList();
            }
            return null;
        }
        #endregion

        #region 提交订单评价
        /// <summary>
        /// 提交订单评价
        /// </summary>
        /// <param name="order_id"></param>
        /// <param name="service_rating"></param>
        /// <param name="service_rating_text"></param>
        /// <param name="comment_time"></param>
        /// <param name="deliver_time"></param>
        /// <returns></returns>
        public int RatingOrder(string order_id, int service_rating, string service_rating_text, string comment_time, string deliver_time)
        {
            var apirel = openApi.RatingOrder(order_id, service_rating, service_rating_text, comment_time, deliver_time, 
                consumer_key, consumer_secret);

            return apirel != null && apirel.code == 200 ? 1 : 0;
        }
        #endregion

        #region 提交餐品评价
        /// <summary>
        /// 提交餐品评价
        /// </summary>
        /// <param name="order_id"></param>
        /// <param name="food_ratings"></param>
        /// <returns></returns>
        public int RatingFoods(string order_id, List<FoodRating> food_ratings)
        {
            var apirel = openApi.SubmitRatingFoods(order_id, food_ratings.Select(a => new Entity.FoodRating()
            {
                food_id = Convert.ToInt32(a.food_id),
                rating = a.rating,
                rating_text = a.rating_text,
                image_url = a.image_url,
                image_ext = a.image_ext
            }).ToList(),
                consumer_key, consumer_secret);

            return apirel != null && apirel.code == 200 ? 1 : 0;
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
        /// <returns></returns>
        public Cart CreateCart(string phone, List<FoodCart> foods, string longitude, string latitude, string user_token, string device_id)
        {
            var _foods = foods.Select(a => new Entity.FoodCartEntity() { id =Convert.ToInt32(a.id), quantity = a.quantity }).ToList();
            var apirel = openApi.Cart(phone, _foods, longitude, latitude, user_token, device_id, consumer_key, consumer_secret);

            if (apirel.code != 200 || apirel.data == null
                || string.IsNullOrEmpty(apirel.data.id))
            {
                return null;
            }

            var cart = new Cart();

            cart.id = apirel.data.id;
            cart.total = Convert.ToDecimal(apirel.data.total);
            cart.deliver_amount = Convert.ToDecimal(apirel.data.deliver_amount);
            cart.restaurant_id = apirel.data.restaurant;
            cart.create_time = apirel.data.create_time;
            cart.error_code = apirel.data.error_code;
            cart.phone = apirel.data.phone;
            var _group = new List<Entity.GroupItem>();
            foreach (var g in apirel.data.group)
            {
                if (g.Count > 0)
                {
                    _group.AddRange(g);
                }
            }
            cart.foods = _group.Select(a => new FoodCart()
            {
                id = a.id.ToString(),
                quantity = a.quantity,
                name = a.name,
                price = Convert.ToDecimal(a.price)
            }).ToList();
            cart.extras = apirel.data.extra.Select(a => new FoodCartExtra()
            {
                description = a.description,
                price = Convert.ToDecimal(a.price),
                name = a.name,
                quantity = a.quantity
            }).ToList();

            return cart;
        }
        #endregion

        #region 下单和支付
        /// <summary>
        /// 下单和支付
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
        /// <param name="deliver_time"></param>
        /// <returns></returns>
        public Order Order(string tp_order_id, string phones, string consignee, string address,
            string cart_id, string total, string longitude, string latitude, string ip,
            string description, string invoice, string deliver_time,
            List<FoodCart> foods)
        {
            var _foods = foods.Select(a => new Entity.FoodCartEntity() { id = Convert.ToInt32(a.id), quantity = a.quantity }).ToList();
            var order = new Order();

            // 1.修改购物车手机号码
            var phone = phones.Contains(',') ? phones.Split(',')[0] : phones;
            var changephonerel = openApi.CartInfo(cart_id, phone, _foods, consumer_key, consumer_secret);
            if (changephonerel.code != 200)
            {
                order.error_msg = "下单失败";
            }

            // 2.正式下单
            var apirel = openApi.Order(tp_order_id, phones, consignee, address, cart_id,
                total, longitude, latitude, ip, description, invoice, deliver_time,
                consumer_key, consumer_secret);

            try
            {
                if (apirel.data != null)
                {
                    LogHelper.Info("eleme:error_code=" + apirel.data.error_code);
                }
            }
            catch { }

            if (apirel.code == 200)
            {
                order.error_msg = "下单成功，还未支付！";
                order.error_code = apirel.data.error_code;
                order.order_id = apirel.data.order_id;
                order.original_price = Convert.ToDecimal(apirel.data.original_price);
                order.total_price = Convert.ToDecimal(apirel.data.total_price);
                order.deliver_fee = Convert.ToDecimal(apirel.data.deliver_fee);
                order.deliver_time = apirel.data.deliver_time;
                order.status_code = apirel.data.status_code;
                order.restaurant_id = apirel.data.restaurant_id;
                order.consignee = apirel.data.consignee;
                order.invoice = apirel.data.invoice;
                order.restaurant_name = apirel.data.restaurant_name;
                order.address = apirel.data.address;
                order.is_online_paid = apirel.data.is_online_paid;
                order.description = apirel.data.description;
                order.delivery_poi_address = apirel.data.delivery_poi_address;
                order.created_at = apirel.data.created_at;
                order.delivery_geo = apirel.data.delivery_geo;
                order.phones = string.Join(",", apirel.data.phone_list);
                order.is_book = apirel.data.is_book;
                var _group = new List<Entity.GroupItem>();
                foreach (var g in apirel.data.detail.group)
                {
                    if (g.Count > 0)
                    {
                        _group.AddRange(g);
                    }
                }
                if (_group.Count > 0)
                {
                    order.foods = _group.Select(a => new FoodCart()
                    {
                        id = a.id.ToString(),
                        quantity = a.quantity,
                        name = a.name,
                        price = Convert.ToDecimal(a.price)
                    }).ToList();
                }

                if (apirel.data.detail.extra != null && apirel.data.detail.extra.Count > 0)
                {
                    order.extras = apirel.data.detail.extra.Select(a => new FoodCartExtra()
                    {
                        description = a.description,
                        price = Convert.ToDecimal(a.price),
                        name = a.name,
                        quantity = a.quantity
                    }).ToList();
                }
            }
            else if (apirel.code == 1022)
            {
                // 下单失败
                order.error_code = apirel.data.error_code;
                order.error_msg = order_err_msg[apirel.data.error_code];
            }
            else
            {
                order.error_code = -1;
                order.error_msg = "下单失败";
            }
            return order;
        }
        #endregion

        #region 支付
        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="order_id"></param>
        /// <param name="tp_order_id"></param>
        /// <returns></returns>
        public int Payment(string order_id, string tp_order_id)
        {
            var _apirel = openApi.PaymentStatus(order_id, consumer_key, consumer_secret);
            return _apirel.code;
        }
        #endregion

        #region 查询订单状态
        /// <summary>
        /// 查询订单状态
        /// </summary>
        /// <param name="order_id"></param>
        /// <returns></returns>
        public OrderState FindOrderState(string order_id)
        {
            var apirel = openApi.OrderState(order_id, consumer_key, consumer_secret);

            if (apirel != null && apirel.code == 200 )
            {
                return new OrderState()
                {
                    status_code = (MealApiInterface.Enum.OrderState)apirel.data.status_code
                };
            }
            return null;
        }
        #endregion

        #region 配送状态跟踪
        /// <summary>
        /// 配送状态跟踪
        /// </summary>
        /// <param name="order_id"></param>
        /// <returns></returns>
        public OrderTracking FindOrderTracking(string order_id)
        {
            var apirel = openApi.OrderTracking(order_id, consumer_key, consumer_secret);
            OrderTracking orderTracking = null;
            if (apirel.code == 200 && apirel.data != null)
            {
                orderTracking = new OrderTracking()
                {
                    state_code = apirel.code.ToString(),
                    last_updated_at = apirel.data.last_updated_at.ToString()
                };

                if (apirel.data.deliveryman_info != null)
                {
                    orderTracking.deliveryman_info = new DeliveryMan()
                    {
                        name = apirel.data.deliveryman_info.name,
                        phone = apirel.data.deliveryman_info.phone
                    };
                }

                if (apirel.data.tracking_info != null)
                {
                    orderTracking.tracking_info = new Tracking()
                    {
                        latitude = apirel.data.tracking_info.latitude,
                        longitude = apirel.data.tracking_info.longitude
                    };
                }
            }

            return orderTracking;
        }
        #endregion

        #region 确认订单送达
        /// <summary>
        /// 确认订单送达
        /// </summary>
        /// <param name="order_id"></param>
        /// <returns></returns>
        public int OrderOk(string order_id)
        {
            var apirel = openApi.SetOrderState(order_id, 11, consumer_key, consumer_secret);
            return apirel != null && apirel.code == 200 ? 1 : 0;
        }
        #endregion

        #region 取消订单
        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="order_id"></param>
        /// <returns></returns>
        public int OrderCancel(string order_id)
        {
            var apirel = openApi.SetOrderState(order_id, -1, consumer_key, consumer_secret);
            return apirel != null && apirel.code == 200 ? 1 : 0;
        }
        #endregion

        #region 投诉餐厅
        /// <summary>
        /// 投诉餐厅
        /// </summary>
        /// <param name="restaurant_id"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public string Complaint(string restaurant_id, string content)
        {
            return null;
        }
        #endregion
    }
}
