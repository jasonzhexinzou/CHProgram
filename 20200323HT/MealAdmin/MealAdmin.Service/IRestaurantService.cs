using MealAdmin.Entity;
using MealAdmin.Entity.View;
using System.Collections.Generic;

namespace MealAdmin.Service
{
    public interface IRestaurantService
    {

        #region 全量删除餐厅列表
        /// <summary>
        /// 全量删除餐厅列表
        /// </summary>
        /// <returns></returns>
        int Del();
        #endregion

        #region 全量删除医院可送餐餐厅关系表
        /// <summary>
        /// 全量删除医院可送餐餐厅关系表
        /// </summary>
        /// <returns></returns>
        int DelRangeRestaurant();
        #endregion

        #region 导入餐厅列表
        /// <summary>
        /// 导入餐厅列表
        /// </summary>
        /// <param name="list"></param>
        /// <param name="fails"></param>
        /// <returns></returns>
        int Import(List<P_RESTAURANT_LIST> list);
        #endregion

        #region 导入医院可送餐餐厅关系表
        /// <summary>
        /// 导入餐厅列表
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        int ImportRangeRestaurant(List<P_HOSPITAL_RANGE_RESTAURANT> list, List<P_HOSPITAL_RANGE_RESTAURANTCOUNT> countList);
        #endregion

        #region 查询所有可送餐列表
        /// <summary>
        /// 查询所有可送餐列表
        /// </summary>
        /// <returns></returns>
        void QryAllRangeRestaurant(ref List<P_HOSPITAL_RANGE_RESTAURANT> listHospitalRestaurant, ref List<P_HOSPITAL_INFO_VIEW> listHospital, ref List<P_HOSPITAL_RANGE_RESTAURANTCOUNT> listHospitalRestaurantCount);
        #endregion

        #region 查询所有餐厅列表
        /// <summary>
        /// 查询所有餐厅列表
        /// </summary>
        /// <returns></returns>
        List<P_RESTAURANT_LIST> QryAllRestaurant();
        #endregion

        List<P_TERRITORY> QryAllArea();

        #region 根据gsk查询所有医院信息
        /// <summary>
        /// 根据gsk查询所有医院信息
        /// </summary>
        /// <returns></returns>
        List<P_HOSPITAL_INFO_VIEW> QryAllHospitalInfoByGsk(List<string> gskHospitalList);
        #endregion

    }
}
