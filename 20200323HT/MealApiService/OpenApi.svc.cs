using MealApiInterface.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MealApiService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“OpenApi”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 OpenApi.svc 或 OpenApi.svc.cs，然后开始调试。
    public class OpenApi : IOpenApi
    {
        protected static MealApiInterface.IMealAPI eleApi = new MealApiEleme.MealAPI();

        protected static MealApiInterface.IMealAPI GetMealApi(int channelCode)
        {
            switch(channelCode)
            {
                case 1:
                    return eleApi;
                default:
                    return null;
            }
        }

        public List<Restaurant> LoadRestaurant(int channelCode, string keyword, string geo, string category_id, string order_by, string new_restaurant)
        {
            return GetMealApi(channelCode).LoadRestaurant(keyword, geo, category_id, order_by, new_restaurant);
        }

        public Restaurant FindRestaurant(int channelCode, string restaurant_id)
        {
            return GetMealApi(channelCode).FindRestaurant(restaurant_id);
        }

        public List<RestaurantFoodCategory> LoadRestaurantMenu(int channelCode, string restaurant_id)
        {
            return GetMealApi(channelCode).LoadRestaurantMenu(restaurant_id);
        }

        public Cart CreateCart(int channelCode, string phone, List<FoodCart> foods, string longitude, string latitude, string user_token, string device_id)
        {
            return GetMealApi(channelCode).CreateCart(phone, foods, longitude, latitude, user_token, device_id);
        }

        public Order Order(int channelCode, string tp_order_id, string phones, string consignee, string address,
            string cart_id, string total, string longitude, string latitude, string ip,
            string description, string invoice, string deliver_time,
            List<FoodCart> food)
        {
            return GetMealApi(channelCode).Order(tp_order_id, phones, consignee, address,
                cart_id, total, longitude, latitude, ip,
                description, invoice, deliver_time, food);
        }

        public int Payment(int channelCode, string order_id, string tp_order_id)
        {
            return GetMealApi(channelCode).Payment(order_id, tp_order_id);
        }

    }
}
