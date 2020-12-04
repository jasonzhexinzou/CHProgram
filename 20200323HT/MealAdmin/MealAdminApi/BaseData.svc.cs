using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using MealAdmin.Entity;
using MealAdmin.Service;
using MealAdmin.Entity.View;

namespace MealAdminApi
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“BaseData”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 BaseData.svc 或 BaseData.svc.cs，然后开始调试。
    public class BaseData : IBaseData
    {
        public IBaseDataService baseDataService = Global.applicationContext.GetBean("baseDataService") as IBaseDataService;
        public IMarketService marketService = Global.applicationContext.GetBean("marketService") as IMarketService;
        public IMeetingService meetingService = Global.applicationContext.GetBean("meetingService") as IMeetingService;
        public IHolidayService holidayService = Global.applicationContext.GetBean("holidayService") as IHolidayService;
        public IBizConfigService bizConfig = Global.applicationContext.GetBean("bizConfigService") as IBizConfigService;
        public IGroupMemberService groupMemberService = Global.applicationContext.GetBean("groupMemberService") as IGroupMemberService;

        public IDCostCenterService dCostCenterService = Global.applicationContext.GetBean("dCostCenterService") as IDCostCenterService;
        public IPreApprovalService preApprovalService = Global.applicationContext.GetBean("preApprovalService") as IPreApprovalService;
        public IUploadOrderService uploadOrderService = Global.applicationContext.GetBean("uploadOrderService") as IUploadOrderService;


        #region 获取省
        /// <summary>
        /// 获取省
        /// </summary>
        /// <returns></returns>
        public List<P_PROVINCE> LoadProvince(string Type, string TA)
        {
            var list = baseDataService.LoadProvince(Type, TA);
            return list;
        }

        /// <summary>
        /// 获取省
        /// </summary>
        /// <returns></returns>
        public List<P_PROVINCE> LoadProvinceByHos(string Type, string Mudid, string TerritoryCode)
        {
            var list = baseDataService.LoadProvinceByHos(Type, Mudid, TerritoryCode);
            return list;
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
            var list = baseDataService.LoadCity(provinceId, Type, TA);
            return list;
        }

        /// <summary>
        /// 获取市
        /// </summary>
        /// <param name="provinceId"></param>
        /// <returns></returns>
        public List<P_CITY> LoadCityByHos(int provinceId, string Type, string UserId, string TerritoryCode)
        {
            var list = baseDataService.LoadCityByHos(provinceId, Type, UserId, TerritoryCode);
            return list;
        }
        #endregion

        #region 获取城市
        /// <summary>
        /// 获取城市
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public P_CITY FindCity(int id)
        {
            return baseDataService.FindCity(id);
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
            var list = baseDataService.LoadHospital(ciytId, market, TA);
            return list;
        }

        public List<P_HOSPITAL> LoadHospitalByTaUser(int ciytId, string market, string userid, string territoryCode)
        {
            var list = baseDataService.LoadHospitalByTaUser(ciytId, market, userid, territoryCode);
            return list;
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
            return baseDataService.SearchHospital(keyword, province, city, market, TA);
        }

        public List<P_HOSPITAL> SearchHospitalByTA(string keyword, int province, int city, string market, string userid, string TerritoryCode)
        {
            return baseDataService.SearchHospitalByTA(keyword, province, city, market, userid, TerritoryCode);
        }
        #endregion

        #region 根据Id查询医院信息
        /// <summary>
        /// 根据Id查询医院信息
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <returns></returns>
        public P_HOSPITAL FindHospital(string hospitalId)
        {
            return baseDataService.FindHospital(hospitalId);
        }
        #endregion

        #region 获取全部Market
        /// <summary>
        /// 获取全部Market
        /// </summary>
        /// <returns></returns>
        public List<P_MARKET> LoadMarket()
        {
            return marketService.Load();
        }

        /// <summary>
        /// 获取全部TACode
        /// </summary>
        /// <returns></returns>
        public List<P_MARKET> LoadMarketByUserId(string UserId)
        {
            return marketService.LoadMarketByUserId(UserId);
        }
        #endregion

        #region 获取全部TACode
        /// <summary>
        /// 获取全部TACode
        /// </summary>
        /// <returns></returns>
        public List<P_TACode> LoadTACode(string UserId)
        {
            return marketService.LoadTAByUserId(UserId);
        }
        #endregion

        #region 获取全部TACode
        /// <summary>
        /// 获取全部TACode
        /// </summary>
        /// <returns></returns>
        public List<P_TACode> LoadTACodeByMarketAndUser(string Market, string UserId)
        {
            return marketService.LoadTACodeByMarketAndUser(Market, UserId);
        }
        #endregion

        #region 获取全部TACode
        /// <summary>
        /// 获取全部TACode
        /// </summary>
        /// <returns></returns>
        public P_TACode LoadRDCode(string Market, string UserId, string TCode)
        {
            return marketService.LoadRDCode(Market, UserId, TCode);
        }
        #endregion

        #region 载入可用的CN号
        /// <summary>
        /// 载入可用的CN号
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<P_MEETING> LoadMeeting(string userId,string approvedDate)
        {
            return meetingService.LoadByUserId(userId, approvedDate);
        }
        #endregion
        #region 载入HT编号
        
        public List<P_PreApproval> LoadHTCode(string UserID)
        {
            return preApprovalService.LoadHTCode(UserID);
        }
        #endregion

        #region 查找会议信息
        /// <summary>
        /// 查找会议信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public P_MEETING_VIEW FindMeeting(string code)
        {
            return meetingService.FindByCode(code);
        }
        #endregion

        #region 找到距今最近的一个休假日区间
        /// <summary>
        /// 找到距今最近的一个休假日区间
        /// </summary>
        /// <param name="now"></param>
        /// <returns></returns>
        public P_HOLIDAY FindNextRange(DateTime now)
        {
            return holidayService.FindNextRange(now);
        }
        #endregion

        #region 获取P流程配置
        /// <summary>
        /// 获取P流程配置
        /// </summary>
        /// <returns></returns>
        public P_BIZ_CONF_OBJ GetTimeConfig()
        {
            return bizConfig.GetConfig();
        }
        #endregion

        #region 获取用户组
        /// <summary>
        /// 获取用户组
        /// </summary>
        /// <param name="groupType"></param>
        /// <returns></returns>
        public List<P_GROUP_MEMBER> GetGroupMembersByType(int groupType)
        {
            return groupMemberService.GetGroupMembersByType((GroupTypeEnum)groupType);
        }
        #endregion

        #region 载入用户所在组
        /// <summary>
        /// 载入用户所在组
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<P_GROUP_MEMBER> LoadUserGroup(string userId)
        {
            return groupMemberService.LoadUserGroup(userId);
        }
        #endregion


        #region 是否项目组成员
        /// <summary>
        /// 是否项目组成员
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsDevGroup(string userId)
        {
            return groupMemberService.IsDevGroup(userId);
        }
        #endregion

        #region 根据market查询TA
        /// <summary>
        /// 根据market查询TA
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="marketName"></param>
        /// <returns></returns>
        public List<P_TA> LoadTAByMarket(string market)
        {
            return marketService.LoadTAByMarket(market);
        }

        public List<P_TA> LoadTAByMarketUserId(string market, string UserID, string TerritoryCode)
        {
            return marketService.LoadTAByMarketUserId(market, UserID, TerritoryCode);
        }
        #endregion


        #region 根据TA查询成本中心
        /// <summary>
        /// 根据TA查询成本中心
        /// </summary>
        /// <param name="ta"></param>
        /// <returns></returns>
        public List<D_COSTCENTERSELECT> LoadCostCenterByTA(string market, string ta)
        {
            return marketService.LoadCostCenterByTA(market, ta);
        }
        #endregion


        #region 根据TA查询成本中心
        /// <summary>
        /// 根据TA查询成本中心
        /// </summary>
        /// <param name="ta"></param>
        /// <returns></returns>
        public List<D_COSTCENTERSELECT> LoadCostCenterByMarketUserID(string market, string UserID, string TerritoryCode)
        {
            return marketService.LoadCostCenterByMarketUserID(market, UserID, TerritoryCode);
        }
        #endregion

        #region 加载成本中心
        /// <summary>
        /// 加载成本中心
        /// </summary>
        public List<D_COSTCENTER> GetAllCostCenter()
        {
            return dCostCenterService.GetAllCostCenter();
        }
        #endregion

        #region 保存提交预申请信息
        /// <summary>
        /// 保存提交预申请信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Add(P_PreApproval entity)
        {
            return preApprovalService.Add(entity);
        }
        #endregion
        #region 获取直线经理和userid
        /// <summary>
        /// 获取直线经理和userid
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public P_PreApproval GetNameUserId(string UserId)
        {
            return preApprovalService.GetNameUserId(UserId);
        }
        #endregion
        #region 编辑提交预申请信息
        /// <summary>
        /// 编辑提交预申请信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Update(P_PreApproval entity)
        {
            return preApprovalService.Update(entity);
        }
        public int UpdateCurrentPreApprova(P_PreApproval entity)
        {
            return preApprovalService.UpdateCurrentPreApprova(entity);
        }
        #endregion
        //更新历史记录表删除标记
        public int UpdateHisPreApprovaDelete(Guid PID)
        {
            return preApprovalService.UpdateHisPreApprovaDelete(PID);
        }
        #region 获取HTCode编号方法
        /// <summary>
        /// 获取HTCode编号方法
        /// </summary>
        /// <returns></returns>
        public HTCode GetHTCode()
        {
            return preApprovalService.GetHTCode();
        }
        #endregion

        #region 查找成本中心审批人信息
        /// <summary>
        /// 查找成本中心审批人信息
        /// </summary>
        /// <param name="ta"></param>
        /// <param name="region"></param>
        /// <param name="costCenter"></param>
        /// <returns></returns>
        public D_COSTCENTER FindInfo(string ta, string region, string costCenter)
        {
            return preApprovalService.FindInfo(ta, region, costCenter);
        }
        #endregion

        #region 通过ID查询预申请信息
        /// <summary>
        /// 通过ID查询预申请信息
        /// </summary>
        /// <param name="PreApprovalID"></param>
        /// <returns></returns>
        public List<P_PreApproval> GetPreApprovalByID(string PreApprovalID)
        {
            return preApprovalService.GetPreApprovalByID(PreApprovalID);
        }
        #endregion

        public HTCode GetHTCodeByID(string htcodeId)
        {
            return preApprovalService.GetHTCodeByID(htcodeId);
        }

        #region 根据订单号查询订单详情
        /// <summary>
        /// 根据订单号查询订单详情
        /// </summary>
        /// <param name="HTCode"></param>
        /// <returns></returns>
        public P_ORDER FindOrderByHTCode(string HTCode)
        {
            return uploadOrderService.FindOrderByHTCode(HTCode);
        }
        #endregion

        #region 根据名字查找market信息
        /// <summary>
        /// 根据名字查找market信息
        /// </summary>
        /// <param name="marketName"></param>
        /// <returns></returns>
        public P_MARKET FindByMarket(string marketName)
        {
            return marketService.FindByMarket(marketName);
        }
        #endregion

        public int UpdateJsApiTicket(string Signature, string Timestamp)
        {
            return baseDataService.UpdateJsApiTicket(Signature, Timestamp);
        }

        public P_JsApiTicket GetJsApiTicket()
        {
            return baseDataService.GetJsApiTicket();
        }

        public List<V_RestaurantState> LoadRestaurantState()
        {
            return baseDataService.LoadRestaurantState();
        }

        #region 根据医院编码搜索医院
        /// <summary>
        /// 根据医院编码所搜医院
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="market"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<P_HOSPITAL> SearchHospitalByCode(string keyword, int province, int city, string market, string TA)
        {
            return baseDataService.SearchHospitalByCode(keyword,province,city,market, TA);
        }

        public List<P_HOSPITAL> SearchHospitalByCodeTA(string keyword, int province, int city, string market, string userid,string TerritoryCode)
        {
            return baseDataService.SearchHospitalByCodeTA(keyword, province, city, market, userid, TerritoryCode);
        }
        #endregion

        #region 判断一线城市
        /// <summary>
        /// 判断一线城市
        /// </summary>
        /// <param name="cityID"></param>
        /// <returns></returns>
        public P_CITY_OUT FindCityBudget(string hospitalId)
        {
            return baseDataService.FindCityBudget(hospitalId);
        }
        #endregion

        
        //获取RM的TERRITORYCODE
        public V_TerritoryRM FindTerritoryDM(string TerritoryCode)
        {
            return baseDataService.FindTerritoryDM(TerritoryCode);
        }

        #region 新增地址
        public List<P_HOSPITAL_NEW> SearchHospitalByGskHospital(string gskHospital)
        {
            return baseDataService.SearchHospitalByGskHospital(gskHospital);
        }

        public int AddNewAddress(P_AddressApproval addressApproval)
        {
            return baseDataService.AddNewAddress(addressApproval);
        }

        public int AddressCancel(P_AddressApproval addressApproval)
        {
            return baseDataService.AddressCancel(addressApproval);
        }

        public int AddressUpdate(P_AddressApproval addressApproval)
        {
            return baseDataService.AddressUpdate(addressApproval);
        }

        public List<P_HOSPITAL_NEW> SearchMainHospitalByGskHospital(string gskHospital)
        {
            return baseDataService.SearchMainHospitalByGskHospital(gskHospital);
        }
        #endregion
    }
}
