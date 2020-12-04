using MealApiInterface.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealApiInterface
{
    public interface IMealAPI
    {
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
        List<Restaurant> LoadRestaurant(string keyword, string geo, string category_id, string order_by, string new_restaurant);
        #endregion

        #region 查询餐厅详情
        /// <summary>
        /// 查询餐厅详情
        /// </summary>
        /// <param name="restaurant_id"></param>
        /// <returns></returns>
        Restaurant FindRestaurant(string restaurant_id);
        #endregion

        #region 查询餐厅菜品
        /// <summary>
        /// 查询餐厅菜品
        /// </summary>
        /// <param name="restaurant_id"></param>
        /// <returns></returns>
        List<RestaurantFoodCategory> LoadRestaurantMenu(string restaurant_id);
        #endregion

        #region 查询餐厅评价综述
        /// <summary>
        /// 查询餐厅评价综述
        /// </summary>
        /// <param name="restaurant_id"></param>
        /// <returns></returns>
        List<RatingOverview> FindRestaurantRatingOverview(string restaurant_id);
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
        List<Rating> LoadRestaurantRating(string restaurant_id, int has_content, int record_type, int offset, int limit);
        #endregion

        #region 查询餐品评价综述
        /// <summary>
        /// 查询餐品评价综述
        /// </summary>
        /// <param name="food_id"></param>
        /// <returns></returns>
        RatingOverview FindFoodRatingOverview(string food_id);
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
        List<RatingFood> LoadFoodRating(string restaurant_id, string food_id, int has_content, int record_type, int offset, int limit);
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
        int RatingOrder(string order_id, int service_rating, string service_rating_text, string comment_time, string deliver_time);
        #endregion

        #region 提交餐品评价
        /// <summary>
        /// 提交餐品评价
        /// </summary>
        /// <param name="order_id"></param>
        /// <param name="food_ratings"></param>
        /// <returns></returns>
        int RatingFoods(string order_id, List<FoodRating> food_ratings);
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
        Cart CreateCart(string phone, List<FoodCart> foods, string longitude, string latitude, string user_token, string device_id);
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
        Order Order(string tp_order_id, string phones, string consignee, string address, 
            string cart_id, string total, string longitude, string latitude, string ip, 
            string description, string invoice, string deliver_time,
            List<FoodCart> foods);
        #endregion

        #region 支付
        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="order_id"></param>
        /// <param name="tp_order_id"></param>
        /// <returns></returns>
        int Payment(string order_id, string tp_order_id);
        #endregion

        #region 查询订单状态
        /// <summary>
        /// 查询订单状态
        /// </summary>
        /// <param name="order_id"></param>
        /// <returns></returns>
        OrderState FindOrderState(string order_id);
        #endregion

        #region 配送状态跟踪
        /// <summary>
        /// 配送状态跟踪
        /// </summary>
        /// <param name="order_id"></param>
        /// <returns></returns>
        OrderTracking FindOrderTracking(string order_id);
        #endregion

        #region 确认订单送达
        /// <summary>
        /// 确认订单送达
        /// </summary>
        /// <param name="order_id"></param>
        /// <returns></returns>
        int OrderOk(string order_id);
        #endregion

        #region 取消订单
        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="order_id"></param>
        /// <returns></returns>
        int OrderCancel(string order_id);
        #endregion

        #region 投诉餐厅
        /// <summary>
        /// 投诉餐厅
        /// </summary>
        /// <param name="restaurant_id"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        string Complaint(string restaurant_id, string content);
        #endregion

    }
}
