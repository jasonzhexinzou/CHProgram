using System.Collections.Generic;
using MealAdmin.Entity;
using XFramework.XInject.Attributes;
using MealAdmin.Dao;
using MealAdmin.Entity.View;
using System;

namespace MealAdmin.Service
{
    public class RestaurantService : IRestaurantService
    {
        [Bean("restaurantDao")]
        public IRestaurantDao restaurantDao { get; set; }
        
        public int Del()
        {
            return restaurantDao.Del();
        }
        
        public int DelRangeRestaurant()
        {
            return restaurantDao.DelRangeRestaurant();
        }

        public int Import(List<P_RESTAURANT_LIST> list)
        {
            return restaurantDao.Import(list);
        }

        public int ImportRangeRestaurant(List<P_HOSPITAL_RANGE_RESTAURANT> list, List<P_HOSPITAL_RANGE_RESTAURANTCOUNT> countList)
        {


            var copyRes = restaurantDao.CopyRangeRestaurant();

            var res = restaurantDao.ImportRangeRestaurant(list);

            var _res = restaurantDao.ImportRangeRestaurantCount(countList);

            var changeRes = restaurantDao.ImportRangeRestaurantChange();

            return 1;

        }

        public void QryAllRangeRestaurant(ref List<P_HOSPITAL_RANGE_RESTAURANT> listHospitalRestaurant, ref List<P_HOSPITAL_INFO_VIEW> listHospital, ref List<P_HOSPITAL_RANGE_RESTAURANTCOUNT> listHospitalRestaurantCount)
        {
            restaurantDao.QryAllRangeRestaurant(ref listHospitalRestaurant, ref listHospital, ref listHospitalRestaurantCount);
        }

        public List<P_RESTAURANT_LIST> QryAllRestaurant()
        {
            return restaurantDao.QryAllRestaurant();
        }

        public List<P_TERRITORY> QryAllArea()
        {
            return restaurantDao.QryAllArea();
        }

        public List<P_HOSPITAL_INFO_VIEW> QryAllHospitalInfoByGsk(List<string> gskHospitalList)
        {
            return restaurantDao.QryAllHospitalInfoByGsk(gskHospitalList);
        }
    }
}
