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
        List<Restaurant> LoadRestaurant(string keyword, string geo, string category_id, string order_by, string new_restaurant);

        [OperationContract]
        Restaurant FindRestaurant(string restaurant_id);

        [OperationContract]
        List<RestaurantFoodCategory> LoadRestaurantMenu(string restaurant_id);
    }
}
