using MealApiInterface.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MealApiService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IOpenApi”。
    [ServiceContract]
    public interface IOpenApi
    {
        [OperationContract]
        List<Restaurant> LoadRestaurant(int channelCode, string keyword, string geo, string category_id, string order_by, string new_restaurant);

        [OperationContract]
        Restaurant FindRestaurant(int channelCode, string restaurant_id);

        [OperationContract]
        List<RestaurantFoodCategory> LoadRestaurantMenu(int channelCode, string restaurant_id);

        [OperationContract]
        Cart CreateCart(int channelCode, string phone, List<FoodCart> foods, string longitude, string latitude, string user_token, string device_id);

        [OperationContract]
        Order Order(int channelCode, string tp_order_id, string phones, string consignee, string address,
            string cart_id, string total, string longitude, string latitude, string ip,
            string description, string invoice, string deliver_time,
            List<FoodCart> food);

        [OperationContract]
        int Payment(int channelCode, string order_id, string tp_order_id);
    }
}
