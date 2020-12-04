using MealAdmin.Entity;
using MealAdmin.Entity.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Service
{
    public interface IBaseDataService
    {
        #region 获取全部省份
        /// <summary>
        /// 加载全部省份
        /// </summary>
        /// <returns></returns>
        List<P_PROVINCE> LoadProvince(string Type, string TA);


        /// <summary>
        /// 加载全部省份
        /// </summary>
        /// <returns></returns>
        List<P_PROVINCE> LoadProvinceByHos(string Type, string Mudid, string TerritoryCode);
        #endregion

        #region 根据省份获取城市
        /// <summary>
        /// 根据省份获取城市
        /// </summary>
        /// <returns></returns>
        List<P_CITY> LoadCity(int provinceId, string Type, string TA);

        /// <summary>
        /// 根据省份获取城市
        /// </summary>
        /// <returns></returns>
        List<P_CITY> LoadCityByHos(int provinceId, string Type, string UserId, string TerritoryCode);
        #endregion

        #region 根据主键查城市
        /// <summary>
        /// 根据主键查城市
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        P_CITY FindCity(int id);
        #endregion

        #region 根据城市获取医院
        /// <summary>
        /// 根据城市获取医院
        /// </summary>
        /// <param name="ciytId"></param>
        /// <param name="market"></param>
        /// <returns></returns>
        List<P_HOSPITAL> LoadHospital(int ciytId, string market, string TA);

        List<P_HOSPITAL> LoadHospitalByTaUser(int ciytId, string market, string userid, string territoryCode);
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
        List<P_HOSPITAL> SearchHospital(string keyword, int province, int city, string market, string TA);

        List<P_HOSPITAL> SearchHospitalByTA(string keyword, int province, int city, string market, string userid, string TerritoryCode);
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
        List<P_HOSPITAL> SearchHospitalByCode(string keyword, int province, int city, string market, string TA);

        List<P_HOSPITAL> SearchHospitalByCodeTA(string keyword, int province, int city, string market, string userid, string TerritoryCode);
        #endregion

        #region 根据id查找医院
        /// <summary>
        /// 根据id查找医院
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <returns></returns>
        P_HOSPITAL FindHospital(string hospitalId);
        #endregion

        #region 同步基础数据
        /// <summary>
        /// 同步基础数据
        /// </summary>
        /// <returns></returns>
        int SyncBaseData();
        #endregion

        int UpdateJsApiTicket(string Signature, string Timestamp);
        P_JsApiTicket GetJsApiTicket();
        List<V_RestaurantState> LoadRestaurantState();

        //获取RM的TERRITORYCODE
        V_TerritoryRM FindTerritoryDM(string TerritoryCode);

        #region 判读一线城市
        /// <summary>
        /// 判读一线城市
        /// </summary>
        /// <param name="cityID"></param>
        /// <returns></returns>
        P_CITY_OUT FindCityBudget(string hospitalId);
        #endregion

        #region 分页查询城市列表
        /// <summary>
        /// 分页查询城市列表
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        List<P_CITY_LIST> LoadBiggestCity(string key,int rows, int page, out int total);
        #endregion

        #region 城市列表导入
        /// <summary>
        /// 导入城市列表
        /// </summary>
        /// <param name="list"></param>
        /// <param name="fails"></param>
        /// <returns></returns>
        int Import(List<P_CITY_LIST> list);
        #endregion

        #region 新增地址

        List<P_HOSPITAL_NEW> SearchHospitalByGskHospital(string gskHospital);

        int AddNewAddress(P_AddressApproval addressApproval);

        int AddressCancel(P_AddressApproval addressApproval);

        int AddressUpdate(P_AddressApproval addressApproval);

        List<P_HOSPITAL_NEW> SearchMainHospitalByGskHospital(string gskHospital);
        #endregion

    }
}
