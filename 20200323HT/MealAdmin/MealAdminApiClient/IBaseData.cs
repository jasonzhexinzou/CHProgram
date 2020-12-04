using MealAdmin.Entity;
using MealAdmin.Entity.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MealAdminApi
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IBaseData”。
    [ServiceContract]
    public interface IBaseData
    {
        [OperationContract]
        List<P_PROVINCE> LoadProvince(string Type, string TA);

        [OperationContract]
        List<P_PROVINCE> LoadProvinceByHos(string Type, string Mudid, string TerritoryCode);

        [OperationContract]
        List<P_CITY> LoadCity(int provinceId, string Type, string TA);

        [OperationContract]
        List<P_CITY> LoadCityByHos(int provinceId, string Type, string UserId, string TerritoryCode);

        [OperationContract]
        P_CITY FindCity(int id);

        [OperationContract]
        List<P_HOSPITAL> LoadHospital(int ciytId, string market, string TA);

        [OperationContract]
        List<P_HOSPITAL> LoadHospitalByTaUser(int ciytId, string market, string userid, string territoryCode);

        [OperationContract]
        P_HOSPITAL FindHospital(string hospitalId);

        [OperationContract]
        List<P_HOSPITAL> SearchHospital(string keyword, int province, int city, string market, string TA);

        [OperationContract]
        List<P_HOSPITAL> SearchHospitalByTA(string keyword, int province, int city, string market, string userid, string TerritoryCode);

        [OperationContract]
        List<P_MARKET> LoadMarket();

        [OperationContract]
        List<P_MARKET> LoadMarketByUserId(string UserId);

        [OperationContract]
        List<P_TACode> LoadTACode(string UserId);

        [OperationContract]
        List<P_TACode> LoadTACodeByMarketAndUser(string Market, string UserId);

        [OperationContract]
        P_TACode LoadRDCode(string Market,string UserId, string TCode);

        [OperationContract]
        List<P_MEETING> LoadMeeting(string userId,string approvedDate);

        
        [OperationContract]
        P_MEETING_VIEW FindMeeting(string code);

        [OperationContract]
        P_HOLIDAY FindNextRange(DateTime now);

        [OperationContract]
        P_BIZ_CONF_OBJ GetTimeConfig();

        [OperationContract]
        List<P_GROUP_MEMBER> GetGroupMembersByType(int groupType);

        [OperationContract]
        List<P_GROUP_MEMBER> LoadUserGroup(string userId);

        [OperationContract]
        bool IsDevGroup(string userId);

        [OperationContract]
        List<P_TA> LoadTAByMarket(string market);

        [OperationContract]
        List<P_TA> LoadTAByMarketUserId(string market, string UserID, string TerritoryCode);

        //[OperationContract]
        //List<P_TA> LoadTAByMarket(string market);

        [OperationContract]
        List<D_COSTCENTERSELECT> LoadCostCenterByTA(string market, string ta);

        [OperationContract]
        List<D_COSTCENTERSELECT> LoadCostCenterByMarketUserID(string market, string UserID, string TerritoryCode);

        [OperationContract]
        List<D_COSTCENTER> GetAllCostCenter();

        List<P_TA> LoadTAByMarket(string userId, string phoneNumber, string market);

        [OperationContract]
        int Add(P_PreApproval entity);
        [OperationContract]
        P_PreApproval GetNameUserId(string UserId);
        [OperationContract]
        int Update(P_PreApproval entity);
        [OperationContract]
        int UpdateCurrentPreApprova(P_PreApproval entity);
        [OperationContract]
        int UpdateHisPreApprovaDelete(Guid PID);

        [OperationContract]
        HTCode GetHTCode();

        [OperationContract]
        D_COSTCENTER FindInfo(string ta, string region, string costCenter);

        [OperationContract]
        List<P_PreApproval> GetPreApprovalByID(string PreApprovalID);

        [OperationContract]
        HTCode GetHTCodeByID(string htcodeId);

        [OperationContract]
        List<P_PreApproval> LoadHTCode(string UserID);

        [OperationContract]
        P_MARKET FindByMarket(string marketName);

        [OperationContract]
        List<P_ORDER> FindOrderByHTCode(string HTCoded);
        [OperationContract]
        int UpdateJsApiTicket(string Signature,string Timestamp);
        [OperationContract]
        P_JsApiTicket GetJsApiTicket();

        [OperationContract]
        List<V_RestaurantState> LoadRestaurantState();

        [OperationContract]
        //根据医院编码搜索医院
        List<P_HOSPITAL> SearchHospitalByCode(string keyword, int province, int city, string market, string TA);

        [OperationContract]
        //根据医院编码搜索医院
        List<P_HOSPITAL> SearchHospitalByCodeTA(string keyword, int province, int city, string market, string userid, string TerritoryCode);

        [OperationContract]
        //判断一线城市
        P_CITY_OUT FindCityBudget(string hospitalId);

        [OperationContract]
        //判断一线城市
        V_TerritoryRM FindTerritoryDM(string TerritoryCode);

        #region 新增地址
        [OperationContract]
        List<P_HOSPITAL_NEW> SearchHospitalByGskHospital(string gskHospital);

        [OperationContract]
        int AddNewAddress(P_AddressApproval addressApproval);

        [OperationContract]
        int AddressCancel(P_AddressApproval addressApproval);

        [OperationContract]
        int AddressUpdate(P_AddressApproval addressApproval);

        [OperationContract]
        List<P_HOSPITAL_NEW> SearchMainHospitalByGskHospital(string gskHospital);
        #endregion

    }
}
