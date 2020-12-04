using MealAdmin.Entity;
using MealAdmin.Entity.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Dao
{
    public interface IHospitalDao
    {
        //List<P_HOSPITAL_MNT_VIEW> LoadPage(string srh_GskHospital, string srh_HospitalName, string srh_HospitalMarket, string srh_OHHospitalType, int rows, int page, out int total);

        List<P_HOSPITAL_DATA_VIEW> LoadPage(string srh_GskHospital, string srh_HospitalName, string srh_HospitalMarket, string srh_OHHospitalType, int rows, int page, out int total);

        List<P_TERRITORY> LoadTAPage(string srh_GskHospital, string srh_HospitalName, string srh_MUDID, string srh_TerritoryCode, string srh_HospitalMarket, string srh_HospitalTA, int rows, int page, out int total);

        //List<P_HOSPITAL_MNT_VIEW> Load(string srh_GskHospital, string srh_HospitalName, string srh_HospitalMarket, string srh_OHHospitalType);
        List<P_HOSPITAL_DATA_VIEW> Load(string srh_GskHospital, string srh_HospitalName, string srh_HospitalMarket, string srh_OHHospitalType);

        List<P_TERRITORY> LoadTA(string srh_GskHospital, string srh_HospitalName, string srh_MUDID, string srh_TerritoryCode, string srh_HospitalMarket, string srh_HospitalTA);

        List<P_HOSPITAL_DATA_VIEW> LoadHData(string srh_GskHospital, string srh_HospitalName, string srh_HospitalMarket, string srh_OHHospitalType);
        List<Territory_Hospital> LoadTERRITORY_TAByMarket(string Market);
        P_HOSPITAL GetHospitalByID(int HospitalID);

        P_HOSPITAL GetHospitalByGSKHospital(string GSKHospital);

        List<P_HOSPITAL_ADDR> LoadHospitalAddr(int HospitalID);

        bool DeleteHospitalAddr(Guid AddrID);

        bool AddHospitalAddr(P_HOSPITAL_ADDR Data);

        List<P_HOSPITALINFO> LoadHospital(string channel);
        void deleteProvince();
        void deleteCity();

        #region 根据Market删除医院
        /// <summary>
        /// 根据Market删除医院
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        int ClearByMarket(string market);
        #endregion

        #region 根据Market删除省份
        /// <summary>
        /// 根据Market删除医院
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        int ClearProvinceByMarket(string market);
        #endregion

        #region 根据Market删除城市
        /// <summary>
        /// 根据Market删除医院
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        int ClearCityByMarket(string market);
        #endregion

        #region 导入医院
        /// <summary>
        /// 导入医院
        /// </summary>
        /// <param name="list"></param>
        /// <param name="fails"></param>
        /// <returns></returns>
        int Import(List<P_HOSPITAL> list, string MarketString, ref List<P_HOSPITAL> fails);
        #endregion

        #region 删除医院
        /// <summary>
        /// 删除医院
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        int Del(string[] ids);
        #endregion

        #region 批量删除医院
        /// <summary>
        /// 批量删除医院
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        int DelHospitals(List<string> ids, out List<string> unSuccesUserId);
        #endregion

        #region 修改医院
        /// <summary>
        /// 修改医院
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int Change(P_HOSPITAL entity);
        #endregion

        #region 新增医院
        /// <summary>
        /// 新增医院
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int Add(P_HOSPITAL entity);
        #endregion

        #region 根据医院Code查询医院
        /// <summary>
        /// 根据医院Code查询医院
        /// </summary>
        /// <param name="hospitalCode"></param>
        /// <returns></returns>
        P_HOSPITAL FindByCode(string hospitalCode);
        #endregion

        #region 查询医院覆盖变化情况
        /// <summary>
        /// 查询医院覆盖变化情况
        /// </summary>
        /// <param name="Type">类型</param>
        /// <returns></returns>
        List<P_HospitalCoverChange> FindHospitalCoverChange(string Type);
        #endregion

        #region 查询医院对应代表及直线经理
        /// <summary>
        /// 查询医院对应代表及直线经理
        /// </summary>
        /// <param name="HospitalCode">类型</param>
        /// <returns></returns>
        List<V_TerritoryHospitalMRDM> FindHospitalUser(string HospitalCode);
        #endregion

        #region 修改发送状态
        /// <summary>
        /// 修改发送状态
        /// </summary>
        /// <returns></returns>
        void UpdateMessageState();
        #endregion

        #region 根据医院代码查询目标医院
        /// <summary>
        /// 根据医院代码查询目标医院 20190416
        /// </summary>
        /// <param name="GskHospitalOHCode"></param>
        /// <returns></returns>
        List<P_HOSPITAL> GetDataByGskHospitalOH(string GskHospitalOHCode);
        #endregion

        #region 根据医院代码名称查询目标医院
        /// <summary>
        /// 根据医院代码名称查询目标医院 20190416
        /// </summary>
        /// <param name="GskOHName"></param>
        /// <param name="GskHospitalOHCode"></param>
        /// <returns></returns>
        P_HOSPITAL GetDataByGskOHName(string GskOHName, string GskHospitalOHCode);
        #endregion

        #region 根据医院代码名称查询目标医院省市Market
        List<P_HOSPITAL_MNT_VIEW> GetDataByProvinceCityMarket(string GskOHName, string GskHospitalOHCode);
        #endregion

        #region 获取Rx临时表数据
        /// <summary>
        /// 获取Rx临时表数据
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        List<P_HOSPITAL_RxTemp> LoadRxTemp();
        #endregion

        #region 根据Market删除Detail表中关联主数据表的数据
        void DeleteDetailData(string Market);
        #endregion

        #region 根据Market删除主数据表中主地址数据
        void DeleteMainAddressData(string Market);
        #endregion

        #region 更新门地址IsDelete=1
        /// <summary>
        /// 更新门地址IsDelete=1
        /// </summary>
        /// <param name="Market"></param>
        /// <returns></returns>
        int UpdateIsDelete(string Market);
        #endregion

        #region 更新院外IsDelete=1
        /// <summary>
        /// 更新院外IsDelete=1
        /// </summary>
        /// <param name="Market"></param>
        /// <returns></returns>
        int UpdateOHIsDelete(string Market);
        #endregion

        #region 插入医院
        /// <summary>
        /// 插入医院
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        int InsertHospitalData(List<P_HOSPITAL> list);
        #endregion

        #region 插入Detail
        /// <summary>
        /// 插入Detail
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        int InsertHospitalDetail(List<P_HOSPITAL_DETAIL> list);
        #endregion

        #region 更新门地址IsDelete=0
        /// <summary>
        /// 更新门地址IsDelete=0
        /// </summary>
        /// <param name="Market"></param>
        /// <returns></returns>
        int UpdateRxStatus(string Market);
        #endregion

        #region 根据Market查询主数据院外数据
        /// <summary>
        /// 根据Market查询主数据院外数据
        /// </summary>
        /// <param name="Market"></param>
        /// <returns></returns>
        List<P_HOSPITAL> GetOHData(string Market);
        #endregion

        #region 根据Market院外Code查询主数据数据
        /// <summary>
        /// 根据Market院外Code查询主数据数据
        /// </summary>
        /// <param name="GskHospitalCode"></param>
        /// <param name="Market"></param>
        /// <returns></returns>
        List<P_HOSPITAL> GetHData(string GskHospitalCode, string Market);
        #endregion

        #region 更新院外数据信息与目标医院一致

        int UpdateOHData(List<P_HOSPITAL> list);
        #endregion

        #region 获取Vx临时表数据
        /// <summary>
        /// 获取Vx临时表数据
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        List<P_HOSPITAL_VxTemp> LoadVxTemp();
        #endregion

        #region 更新门地址IsDelete=0
        /// <summary>
        /// 更新门地址IsDelete=0
        /// </summary>
        /// <param name="Market"></param>
        /// <returns></returns>
        int UpdateVxStatus(string Market);
        #endregion

        #region 获取DDT临时表数据
        /// <summary>
        /// 获取DDT临时表数据
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        List<P_HOSPITAL_DDTTemp> LoadDDTTemp();
        #endregion

        #region 更新门地址IsDelete=0
        /// <summary>
        /// 更新门地址IsDelete=0
        /// </summary>
        /// <param name="Market"></param>
        /// <returns></returns>
        int UpdateDDTStatus(string Market);
        #endregion

        #region 获取TSKF临时表数据
        /// <summary>
        /// 获取TSKF临时表数据
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        List<P_HOSPITAL_TSKFTemp> LoadTSKFTemp();
        #endregion

        #region 更新门地址IsDelete=0
        /// <summary>
        /// 更新门地址IsDelete=0
        /// </summary>
        /// <param name="Market"></param>
        /// <returns></returns>
        int UpdateTSKFStatus(string Market);
        #endregion

        #region 根据Market及ACTION查询变量表数据
        /// <summary>
        /// 根据Market及ACTION查询变量表数据
        /// </summary>
        /// <param name="Market"></param>
        /// <param name="ACTION"></param>
        /// <returns></returns>
        List<Temp_Hospital_Variables> GetHosVariables(string Market, string ACTION);
        #endregion

        #region 查询主数据表中与变量表匹配的门地址
        /// <summary>
        /// 查询主数据表中与变量表匹配的门地址
        /// </summary>
        /// <param name="Market"></param>
        /// <param name="ACTION"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        List<P_HOSPITAL> Getnotmain(string Market, string ACTION, string Type);
        #endregion

        #region 更新失效门地址状态为IsDelete=2
        int Updatenotmain(List<P_HOSPITAL> list, List<Temp_Hospital_Variables> varList);
        #endregion

        #region 更新主地址地址变更的门地址状态为IsDelete=3
        int UpdateAnotmain(List<P_HOSPITAL> list, List<Temp_Hospital_Variables> varList);
        #endregion

        #region 变量记录
        List<P_HospitalVariables> LoadHospitalVariables(string srh_GskHospital, string srh_HospitalName, string srh_HospitalMarket, bool srh_Add, bool srh_City, bool srh_UpdateHospitalName, bool srh_LatLong, bool srh_Address, bool srh_Delete, int rows, int page, out int total);
        List<P_HospitalVariables> ExportHospitalVariablesList(string srh_GskHospital, string srh_HospitalName, string srh_HospitalMarket, bool srh_Add, bool srh_City, bool srh_UpdateHospitalName, bool srh_LatLong, bool srh_Address, bool srh_Delete);
        int SyncCoypHospitalVariablesData();
        int InsertHospitalVariablesCountData();
        List<P_Hospital_Variables_Count> LoadHospitalVariablesCount(int rows, int page, out int total);
        List<P_CHECK_REPORT_LINE_RM> LoadHospitalVariablesCount(string srh_market, bool srh_Add, bool srh_Delete, int rows, int page, out int total);
        List<P_CHECK_REPORT_LINE_RM> ExportTerritoryRMVariables(string srh_market, bool srh_Add, bool srh_Delete);
        List<CHECK_REPORT_LINE_RM> LoadTerritoryRMVariablesData();
        int SyncCoypTerritoryRMVariablesData(List<CHECK_REPORT_LINE_RM> list);
        #endregion

        #region 获取变量临时表数据
        /// <summary>
        /// 获取变量临时表数据
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        List<Temp_Hospital_Variables> LoadVariablesTemp();
        #endregion

        #region 获取变量临时表非删除数据
        /// <summary>
        /// 获取变量临时表非删除数据
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        List<Temp_Hospital_Variables> LoadVariablesTempNoDelete();
        #endregion

        #region 获取变量临时表不同数据
        List<Temp_Hospital_Variables> LoadVariablesTempAddData(string ACTION);
        #endregion

        #region 删除新增地址
        int DeleteAddress(P_HOSPITAL hospital);
        #endregion

        #region 医院summary
        List<P_Brand_Coverage_Count> LoadBrandCoverageCount();
        List<P_Brand_Coverage_Count> LoadBrandCoverageCountOH();
        List<P_HOSPITAL> LoadHospital();
        List<P_TERRITORY_TA> LoadTerritoryTA();
        List<P_TA_HOSPITAL> LoadTAHospital();
        List<P_TA_HOSPITAL> LoadTAHospitalOH();
        List<P_Brand_Coverage_Count_TA> LoadBrandCoverageCountTA();
        int InsertHospitalVariablesCountDataTA(List<P_TERRITORY_TA> p_TAs);
        List<P_TERRITORY_TA> LoadAllTerritoryTA();
        #endregion

        #region 同步医院表
        int SyncHospital();

        int SyncHospitalDetail();

        int SyncTerritoryHospital();

        int SyncHospitalRange();
        #endregion

        #region 获取RM列表
        List<V_TerritoryRM> LoadTerritoryRMList(string TerritoryStr);
        #endregion
    }
}
