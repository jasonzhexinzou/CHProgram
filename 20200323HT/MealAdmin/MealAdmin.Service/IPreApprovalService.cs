using MealAdmin.Entity;
using MealAdmin.Entity.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Service
{
    public interface IPreApprovalService
    {
        //通过ID查询预申请信息
        List<P_PreApproval> GetPreApprovalByID(string PreApprovalID);
        //预申请查询
        List<P_PreApproval> Load(string srh_HTCode, string srh_HospitalCode, string srh_CostCenter, string srh_ApplierMUDID, string srh_ApplierTerritory, string srh_BUHeadMUDID, string srh_Market, string srh_TA, string srh_State, string srh_StartBUHeadApproveDate, string srh_EndBUHeadApproveDate, string srh_StartMeetingDate, string srh_EndMeetingDate, string srh_RD, int srh_BudgetTotal, int rows, int page, out int total);
        //预申请查询
        List<P_PreApproval> LoadFreeSpeakerFile(string srh_HTCode, string srh_HospitalCode, string srh_CostCenter, string srh_ApplierMUDID, string srh_ApplierTerritory, string srh_BUHeadMUDID, string srh_Market, string srh_TA, string srh_State, string srh_StartBUHeadApproveDate, string srh_EndBUHeadApproveDate, string srh_StartMeetingDate, string srh_EndMeetingDate, string srh_RD, int srh_BudgetTotal, int rows, int page, out int total);
        //导出预申请查询
        List<P_PreApproval> ExportPreApproval(string srh_HTCode, string srh_HospitalCode, string srh_CostCenter, string srh_ApplierMUDID, string srh_ApplierTerritory, string srh_BUHeadMUDID, string srh_Market, string srh_TA, string srh_State, string srh_StartBUHeadApproveDate, string srh_EndBUHeadApproveDate, string srh_StartMeetingDate, string srh_EndMeetingDate, string srh_RD, int srh_BudgetTotal);
      
        //预申请最高审批人
        List<D_COSTCENTER> CostCenterLoad(int rows, int page, out int total);
        //从V_TERRITORY_TA查询不属于成本中心表的[TERRITORY_TA]
        List<V_TERRITORY_TA> GetNewTERRITORY_TA();
        // 将newTERRITORY_TA插入成本中心表
        int InsertnewTERRITORY_TA(List<D_COSTCENTER> list);

        //预申请MMCoE审批记录
        List<P_PreApprovalApproveHistory_View> RecordsLoad(string srh_HTCode, string srh_ApplierMUDID, string srh_BUHeadMUDID, string srh_startMeetingDate, string srh_endMeetingDate, string srh_State, string srh_StartApproveDate, string srh_EndApproveDate, int rows, int page, out int total);
        //通过ID查询成本中心信息
        D_COSTCENTER GetCostCenterByID(string CostCenterID);

        //导出预申请最高审批人
        List<D_COSTCENTER> ExportCostCenterList();
        //成本中心导入
        int ImportCostCenter(List<D_COSTCENTER> excelRows, ref List<D_COSTCENTER> fails, string adminUser);
        //新增成本中心        
        int AddCostCenter(string sltMarket, string sltTA, string txtBUHeadName, string BUHeadMUDID, string Region, string RegionManagerName, string RegionManagerMUDID,string RDSDManagerName, string RDSDManagerMUDID, string CostCenter,string OldCostCenter, string CreateBy, DateTime CreateDateTIme);
        //删除成本中心
        int Del(Guid id);
        // 审批流程
        List<P_PreApprovalApproveHistory> GetApproval(string id);
        //预申请MMCoE查询
        List<P_PreApproval> MMCoELoad(string srh_HTCode, string srh_ApplierMUDID, string srh_startMeetingDate, string srh_endMeetingDate, int rows, int page, out int total);
        //预申请MMCoE审批--通过
        int Approved(Guid ID, string PID, string UserName, string UserId, int ActionType, DateTime ApproveDate, string txtComments, int type);

        //修改预申请状态--通过
        int UpdateStadeApproved(string ID, string PreApprovalState);
        //查询该成本中心是否存在未审批的预申请
        int _Exist(Guid id);
        int GetStateByTA(string TERRITORY_TA);
        //查询当前TERRITORY_TA
        List<D_COSTCENTER> GetTERRITORY_TAByID(Guid id);
        int UpdateUncompleteCostenterCodeByOldCostCenterCode(string CostCenterCode, string OldCostcenterCode);

        int Add(P_PreApproval entity);
        //获取上一级信息
        P_PreApproval GetNameUserId(string UserId);
        int Update(P_PreApproval entity);
        
        int UpdateCurrentPreApprova(P_PreApproval entity);
        //更新历史记录表删除标记
        int UpdateHisPreApprovaDelete(Guid PID);
        int Delete(Guid id);
        HTCode GetHTCode();
        D_COSTCENTER FindInfo(string ta, string region, string costCenter);
       
        int SaveChange(string ID, string txtTERRITORY_TA, string txtBUHeadName, string txtBUHeadMUDID, string name);

        int UpdatePreApprovalBUHead(Guid ID, string txtBUHeadMUDID, string txtBUHeadName);
        List<P_PreApproval> LoadHTCode(string UserID);
        P_PreApproval LoadPreApprovalInfo(Guid id);
        int BUHeadApprove(Guid id, int state, string reason);
        int BUHeadReject(Guid id, int state, string reason);
        int MMCoEApprove(Guid id, int state, string reason);
        int MMCoEReject(Guid id, int state, string reason);
        int AddPreApprovalApproveHistory(P_PreApprovalApproveHistory PreApprovalHistory);
        List<PreApprovalState> LoadMyPreApprovalUserId(string UserId, DateTime Begin, DateTime End,string State,string Budget, int rows, int page, out int total);
        List<P_PreApproval> LoadMyApprove(string UserId, DateTime Begin, DateTime End, string State,string Applicant, int rows, int page, out int total);
        List<P_PreApproval> LoadCurrentApprove(string UserId, DateTime Begin, DateTime End, string State, string Applicant, int rows, int page, out int total);
        List<P_PreApprovalApproveHistory> LoadApprovalRecords(Guid PID);
        List<P_PreApproval> LoadMyApproveAll(string UserId, string Applicant);
        List<P_PreApproval> FindPreApprovalByHTCode(string HTCode);
        bool HasApproveRights(string UserId);
        bool HasApprove(string UserId);
        bool HasApproveByTA(string UserId, string TA);
        bool HasFileApproveRights(string UserId);
        HTCode GetHTCodeByID(string htcodeId);
        List<P_PreApprovalApproveHistory> FindPreApprovalApproveHistory(Guid PID);
        P_PreApprovalApproveHistory LoadApproveHistoryInfo(Guid PID,int Type);
        P_PreApprovalApproveHistory LoadApproveHistory(Guid PID, int Type);
        P_PreApprovalApproveHistory LoadApproveHistoryRefused(Guid PID, int Type,string UserId);
        P_ORDER FindActivityOrderByHTCode(string HTCode);
        P_PreApproval CheckPreApprovalState(string HTCode);


        List<Approval_State> LoadPreApprovalState(int rows, int page, out int total);

        //审批状态查询
        List<Approval_State> QueryLoad(string srh_HTCode, string srh_ApplierMUDID, string srh_BUHeadMUDID, string srh_Category, string srh_Type, int rows, int page, out int total);


        List<Approval_State> LoadPreApprovalUpload(string MUDID, int rows, int page, out int total);

        List<P_PreApproval> LoadPreApprovalByCostCenter(string CostCenter);

        List<P_PreApproval> LoadPreApprovalByCurrentApprover(string CurrentApprover, string CostCenter);

        int UpdatePendingPreAPprovalBUHead(string HTCode, string BUHeadName, string BUHeadMUDID,string PreBUHeadMUDID);

        List<P_ORDER> GetMMCoEHisOrder();


        List<Approval_State> LoadUploadPage(string HTCode, string ApplierMUDID, string BUHeadMUDID, string Type, string State, int rows, int page, out int total);

        List<Approval_State> LoadByID(string ids);

        int UpdatePreReAssign(string ids, string userId, string name, string mudId, string headName);
        int UpdatePuoReAssign(string ids, string userId, string name, string mudId, string headName);
        int UpdatePuoReAssignByHTCode(string HtCode, string UserId, string Name, string MUDID, string HeadName);
        int InsertFileLink(string FilePath, string Email);
        
        string GetOldGskHospitalCodeByGskHospitalCode(string GskHospitalCode);
        string GetOldCostcenterByCostcenter(string Costcenter);

        #region 更新预申请医院地址
        /// <summary>
        /// 更新预申请医院地址
        /// </summary>
        /// <param name="preApprovalId"></param>
        /// <param name="hospitalAddress"></param>
        /// <returns></returns>
        int UpdateAddress(string preApprovalId, string hospitalAddress);
        #endregion

        //查询用户未审批的预申请
        List<P_PreApproval> GetPreApprovalByUser();

        List<P_PreApproval> GetPreApprovalByUser(string UserID);

        #region 新增地址
        List<P_AddressApproval> LoadMyAddressApprovalByUserId(string UserId, DateTime Begin, DateTime End, string State, string Budget, int rows, int page, out int total);

        List<P_AddressApproval> LoadMyAddressApprove(string UserId, DateTime Begin, DateTime End, string State, string Applicant, int rows, int page, out int total);

        P_AddressApproval_View LoadAddressApprovalInfo(Guid id);

        P_AddressApproval_View LoadAddressApprovalInfoForUpdate(Guid id);

        int AddressApprove(P_AddressApproval_View p_addressApproval_View, string reason);

        int AddAddressApproveHistory(P_AddressApproveHistory p_addressApproveHistory);

        List<P_AddressApproval_View> LoadMyAddressApproveAll(string UserId, string applicant);

        /// <summary>
        /// 每小时更新超过5个自然日未审批的地址申请数据
        /// </summary>
        /// <param name="nowDate"></param>
        /// <returns></returns>
        List<P_AddressApproval> LoadInvalidAddressApplication(DateTime nowDate);

        int InvalidAddressApplication(List<Guid> guids, int state);

        List<P_AddressApproval_View> LoadAddressApprove(string srh_DACode, string srh_ApplierMUDID, string srh_ApproverMUDID,  string srh_GskHospital, string srh_StartApplyDate, string srh_EndApplyDate, string srh_State, string srh_IsDelete, int rows, int page, out int total);
        List<WP_QYUSER> LoadWPQYUSER();
        List<P_AddressApproveHistory> LoadAddressApprovalHistory(Guid DA_ID);
        List<P_AddressApproval_View> ExportAddressApprovalList(string srh_DACode, string srh_ApplierMUDID, string srh_ApproverMUDID, string srh_GskHospital, string srh_StartApplyDate, string srh_EndApplyDate, string srh_State, string srh_IsDelete);

        List<P_AddressApproval> LoadAddressApprovalByDACode(string dA_CODE);

        List<P_AddressApproval> LoadMyAddressApproveCount(string userId, DateTime begin, DateTime end, string state, string applicant);
        #endregion

        #region 取消预申请
        int PreApprovalCancel(P_PreApproval p_preApproval);
        #endregion

        //根据成本中心表TERRITORY_TA查询预申请表待审批状态数据
        List<P_PreApproval> GetDataByTAAndState(string TERRITORY_TA);

        //预申请分析
        List<P_PreApproval_CountAmount_View> LoadPreApprovalData(string userId, string position, string territoryCode, string begin, string end);
        //预申请上层分析
        List<P_PreApproval_OwnBelongCountAmount> LoadPreApprovalUpData(string userId, string position, string territoryCode, string begin, string end);


        #region 同步预申请表
        int SyncPreApproval();
        #endregion

        V_COST_SUMMARY GetPreApprovalList(List<string> TerritoryStr, string StartDate, string EndDate);

    }
}
