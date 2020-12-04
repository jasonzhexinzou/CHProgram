using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MealAdmin.Entity;
using XFramework.XInject.Attributes;
using MealAdmin.Dao;
using MeetingMealApiClient;
using XFramework.XUtil;
using MealAdmin.Entity.View;

namespace MealAdmin.Service
{
    public class BaseDataService : IBaseDataService
    {
        [Bean("baseDataDao")]
        public IBaseDataDao baseDataDao { get; set; }

        #region 获取省
        /// <summary>
        /// 获取省
        /// </summary>
        /// <returns></returns>
        public List<P_PROVINCE> LoadProvince(string Type, string TA)
        {
            return baseDataDao.LoadProvince(Type, TA);
        }

        /// <summary>
        /// 获取省
        /// </summary>
        /// <returns></returns>
        public List<P_PROVINCE> LoadProvinceByHos(string Type, string Mudid, string TerritoryCode)
        {
            return baseDataDao.LoadProvinceByHos(Type, Mudid, TerritoryCode);
        }
        #endregion

        #region 获取市
        /// <summary>
        /// 获取市
        /// </summary>
        /// <param name="provinceId"></param>
        /// <returns></returns>
        public List<P_CITY> LoadCity(int provinceId, string Type, string TA)
        {
            return baseDataDao.LoadCity(provinceId, Type, TA);
        }

        /// <summary>
        /// 获取市
        /// </summary>
        /// <param name="provinceId"></param>
        /// <returns></returns>
        public List<P_CITY> LoadCityByHos(int provinceId, string Type, string UserId, string TerritoryCode)
        {
            return baseDataDao.LoadCityByHos(provinceId, Type, UserId, TerritoryCode);
        }
        #endregion

        #region 根据主键查城市
        /// <summary>
        /// 根据主键查城市
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public P_CITY FindCity(int id)
        {
            return baseDataDao.FindCity(id);
        }
        #endregion

        #region 获取医院
        /// <summary>
        /// 获取医院
        /// </summary>
        /// <param name="ciytId"></param>
        /// <param name="market"></param>
        /// <returns></returns>
        public List<P_HOSPITAL> LoadHospital(int ciytId, string market, string TA)
        {
            return baseDataDao.LoadHospital(ciytId, market, TA);
        }

        public List<P_HOSPITAL> LoadHospitalByTaUser(int ciytId, string market, string userid, string territoryCode)
        {
            return baseDataDao.LoadHospitalByTaUser(ciytId, market, userid, territoryCode);
        }
        #endregion

        #region 根据关键字搜索医院
        /// <summary>
        /// 根据关键字搜索医院
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="market"></param>
        /// <returns></returns>
        public List<P_HOSPITAL> SearchHospital(string keyword, int province, int city, string market, string TA)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                keyword = "%";
            }
            else
            {
                keyword = $"%{keyword}%";
            }
            return baseDataDao.SearchHospital(keyword, province, city, market, TA);
        }

        /// <summary>
        /// 根据关键字搜索医院
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="market"></param>
        /// <returns></returns>
        public List<P_HOSPITAL> SearchHospitalByTA(string keyword, int province, int city, string market, string userid, string TerritoryCode)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                keyword = "%";
            }
            else
            {
                keyword = $"%{keyword}%";
            }
            return baseDataDao.SearchHospitalByTA(keyword, province, city, market, userid, TerritoryCode);
        }
        #endregion

        #region 根据医院编码搜索医院
        /// <summary>
        /// 根据医院编码搜索医院
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="market"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<P_HOSPITAL> SearchHospitalByCode(string keyword, int province, int city, string market, string TA)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                keyword = "%";
            }
            else
            {
                keyword = $"%{keyword}%";
            }
            return baseDataDao.SearchHospitalByCode(keyword, province, city, market, TA);
        }

        public List<P_HOSPITAL> SearchHospitalByCodeTA(string keyword, int province, int city, string market, string userid, string TerritoryCode)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                keyword = "%";
            }
            else
            {
                keyword = $"%{keyword}%";
            }
            return baseDataDao.SearchHospitalByCodeTA(keyword, province, city, market, userid, TerritoryCode);
        }
        #endregion

        #region 根据id查找医院
        /// <summary>
        /// 根据id查找医院
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <returns></returns>
        public P_HOSPITAL FindHospital(string hospitalId)
        {
            return baseDataDao.FindHospital(hospitalId);
        }
        #endregion

        #region 同步基础数据

        private object lockobj = new object();

        /// <summary>
        /// 同步基础数据
        /// </summary>
        /// <returns></returns>
        public int SyncBaseData()
        {
            int res = 0;
            LogHelper.Info("进入BaseDataService.SyncBaseData");
            lock(lockobj)
            {
                LogHelper.Info("进入BaseDataService.SyncBaseData => lock");

                var now = DateTime.Now;

                var channel = OpenApiChannelFactory.GetChannel();
                var citys = channel.syncCity();
                var hospitals = channel.syncHospital("0");

                var listProvince = new List<P_PROVINCE>();
                var listCity = new List<P_CITY>();
                var listHospital = hospitals.result.Select(a => new P_HOSPITAL()
                {
                    ID = Convert.ToInt32(a.hospitalId),
                    CityId = Convert.ToInt32(a.cityId),
                    GskHospital = string.IsNullOrEmpty(a.gskHospital) ? string.Empty : a.gskHospital,
                    Name = a.hospitalName,
                    FirstLetters = a.firstLetters,
                    Address = a.address,
                    Latitude = a.latitude,
                    Longitude = a.longitude,
                    Type = string.IsNullOrEmpty(a.type) ? string.Empty : a.type,
                    External = a.External,
                    CreateDate = now
                }).ToList();

                foreach (var province in citys.result)
                {
                    listProvince.Add(new P_PROVINCE()
                    {
                        ID = province.provinceId,
                        Name = province.provinceName,
                        PinYin = province.pinYin,
                        CreateDate = now
                    });

                    foreach (var city in province.citys)
                    {
                        listCity.Add(new P_CITY()
                        {
                            ID = city.cityId,
                            ProvinceId = province.provinceId,
                            Name = city.cityName,
                            PinYin = city.pinYin,
                            CreateDate = now
                        });
                    }

                }
                res = baseDataDao.SyncBaseData(listProvince, listCity, listHospital);
            }

            LogHelper.Info("完成BaseDataService.SyncBaseData");
            return res;
        }
        #endregion

        public int UpdateJsApiTicket(string Signature, string Timestamp)
        {
            return baseDataDao.UpdateJsApiTicket(Signature, Timestamp);
        }

        public P_JsApiTicket GetJsApiTicket()
        {
            return baseDataDao.GetJsApiTicket();
        }

        public List<V_RestaurantState> LoadRestaurantState()
        {
            return baseDataDao.LoadRestaurantState();
        }

        //获取RM的TERRITORYCODE
        public V_TerritoryRM FindTerritoryDM(string TerritoryCode)
        {
            return baseDataDao.FindTerritoryDM(TerritoryCode);
        }


        #region 判读一线城市
        /// <summary>
        /// 判读一线城市
        /// </summary>
        /// <param name="cityID"></param>
        /// <returns></returns>
        public P_CITY_OUT FindCityBudget(string hospitalId)
        {
            return baseDataDao.FindCityBudget(hospitalId);
        }
        #endregion

        #region 分页查询城市列表
        /// <summary>
        /// 分页查询城市列表
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<P_CITY_LIST> LoadBiggestCity(string key,int rows, int page, out int total)
        {
            return baseDataDao.LoadBiggestCity(key,rows, page, out total);
        }
        #endregion

        #region 城市列表导入
        /// <summary>
        /// 城市列表导入
        /// </summary>
        /// <param name="list"></param>
        /// <param name="fails"></param>
        /// <returns></returns>
        public int Import(List<P_CITY_LIST> excelRows)
        {
            return baseDataDao.Import(excelRows);
        }
        #endregion

        #region 新增地址
        public List<P_HOSPITAL_NEW> SearchHospitalByGskHospital(string gskHospital)
        {
            return baseDataDao.SearchHospitalByGskHospital(gskHospital);
        }

        public int AddNewAddress(P_AddressApproval addressApproval)
        {
            return baseDataDao.AddNewAddress(addressApproval);
        }

        public int AddressCancel(P_AddressApproval addressApproval)
        {
            return baseDataDao.AddressCancel(addressApproval);
        }

        public int AddressUpdate(P_AddressApproval addressApproval)
        {
            return baseDataDao.AddressUpdate(addressApproval);
        }

        public List<P_HOSPITAL_NEW> SearchMainHospitalByGskHospital(string gskHospital)
        {
            return baseDataDao.SearchMainHospitalByGskHospital(gskHospital);
        }
        #endregion

    }
}
