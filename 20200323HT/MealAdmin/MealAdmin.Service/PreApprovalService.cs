using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MealAdmin.Entity;
using XFramework.XInject.Attributes;
using MealAdmin.Dao;
using MealAdmin.Entity.View;

namespace MealAdmin.Service
{
    public class PreApprovalService : IPreApprovalService
    {
        [Bean("preApprovalDao")]
        public IPreApprovalDao PreApprovalDao { get; set; }

        [Bean("baseDataDao")]
        public IBaseDataDao baseDataDao { get; set; }
        #region 预申请查询
        public List<P_PreApproval> Load(string srh_HTCode, string srh_HospitalCode, string srh_CostCenter, string srh_ApplierMUDID, string srh_ApplierTerritory, string srh_BUHeadMUDID, string srh_Market, string srh_TA, string srh_State, string srh_StartBUHeadApproveDate, string srh_EndBUHeadApproveDate, string srh_StartMeetingDate, string srh_EndMeetingDate, string srh_RD, int srh_BudgetTotal, int rows, int page, out int total)
        {
            return PreApprovalDao.Load(srh_HTCode, srh_HospitalCode, srh_CostCenter, srh_ApplierMUDID, srh_ApplierTerritory, srh_BUHeadMUDID, srh_Market, srh_TA, srh_State, srh_StartBUHeadApproveDate, srh_EndBUHeadApproveDate, srh_StartMeetingDate, srh_EndMeetingDate, srh_RD, srh_BudgetTotal, rows, page, out total);
        }
        #endregion

        #region 预申请查询
        public List<P_PreApproval> LoadFreeSpeakerFile(string srh_HTCode, string srh_HospitalCode, string srh_CostCenter, string srh_ApplierMUDID, string srh_ApplierTerritory, string srh_BUHeadMUDID, string srh_Market, string srh_TA, string srh_State, string srh_StartBUHeadApproveDate, string srh_EndBUHeadApproveDate, string srh_StartMeetingDate, string srh_EndMeetingDate, string srh_RD, int srh_BudgetTotal, int rows, int page, out int total)
        {
            return PreApprovalDao.LoadFreeSpeakerFile(srh_HTCode, srh_HospitalCode, srh_CostCenter, srh_ApplierMUDID, srh_ApplierTerritory, srh_BUHeadMUDID, srh_Market, srh_TA, srh_State, srh_StartBUHeadApproveDate, srh_EndBUHeadApproveDate, srh_StartMeetingDate, srh_EndMeetingDate, srh_RD, srh_BudgetTotal, rows, page, out total);
        }
        #endregion

        #region 导出预申请查询
        public List<P_PreApproval> ExportPreApproval(string srh_HTCode, string srh_HospitalCode, string srh_CostCenter, string srh_ApplierMUDID, string srh_ApplierTerritory, string srh_BUHeadMUDID, string srh_Market, string srh_TA, string srh_State, string srh_StartBUHeadApproveDate, string srh_EndBUHeadApproveDate, string srh_StartMeetingDate, string srh_EndMeetingDate, string srh_RD, int srh_BudgetTotal)
        {
            return PreApprovalDao.ExportPreApproval(srh_HTCode, srh_HospitalCode, srh_CostCenter, srh_ApplierMUDID, srh_ApplierTerritory, srh_BUHeadMUDID, srh_Market, srh_TA, srh_State, srh_StartBUHeadApproveDate, srh_EndBUHeadApproveDate, srh_StartMeetingDate, srh_EndMeetingDate, srh_RD, srh_BudgetTotal);
        }
        #endregion
        

        #region 预申请最高审批人
        public List<D_COSTCENTER> CostCenterLoad(int rows, int page, out int total)
        {
            return PreApprovalDao.CostCenterLoad(rows, page, out total);
        }
        #endregion

        #region 从V_TERRITORY_TA查询不属于成本中心表的[TERRITORY_TA]
        public List<V_TERRITORY_TA> GetNewTERRITORY_TA()
        {
            return PreApprovalDao.GetNewTERRITORY_TA();
        }
        #endregion

        #region 将newTERRITORY_TA插入成本中心表       
        public int InsertnewTERRITORY_TA(List<D_COSTCENTER> sus)
        {
            return PreApprovalDao.InsertnewTERRITORY_TA(sus);
        }
        #endregion

        #region 新增成本中心
        public int AddCostCenter(string sltMarket, string sltTA, string txtBUHeadName, string BUHeadMUDID, string Region, string RegionManagerName, string RegionManagerMUDID, string RDSDManagerName, string RDSDManagerMUDID, string CostCenter, string OldCostCenter, string CreateBy, DateTime CreateDateTIme)

        {
            var list = PreApprovalDao.findCostCenter(sltTA, Region, CostCenter);//查询成本中心是否存在
            if (list.CostCenterCount >= 1)
            {
                return 0;
            }
            return PreApprovalDao.AddCostCenter(sltMarket, sltTA, txtBUHeadName, BUHeadMUDID, Region, RegionManagerName, RegionManagerMUDID, RDSDManagerName, RDSDManagerMUDID, CostCenter, OldCostCenter, CreateBy, CreateDateTIme);

        }
        #endregion
        

        #region 导出预申请最高审批人
        public List<D_COSTCENTER> ExportCostCenterList()
        {
            return PreApprovalDao.ExportCostCenterList();
        }
        #endregion

        #region 新增提交预申请信息
        /// <summary>
        /// 新增提交预申请信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Add(P_PreApproval entity)
        {
            return PreApprovalDao.Add(entity);
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
            return PreApprovalDao.GetNameUserId(UserId);
        }
        #endregion
        #region 更新提交预申请信息
        /// <summary>
        /// 更新提交预申请信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Update(P_PreApproval entity)
        {
            return PreApprovalDao.Update(entity);
        }
        #endregion

        #region 更新提交预申请信息
        /// <summary>
        /// 更新提交当前预申请信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int UpdateCurrentPreApprova(P_PreApproval entity)
        {
            return PreApprovalDao.UpdateCurrentPreApprova(entity);
        }
        #endregion
        //更新历史记录表删除标记
        public int UpdateHisPreApprovaDelete(Guid PID)
        {
            return PreApprovalDao.UpdateHisPreApprovaDelete(PID);
        }

        #region 逻辑删除提交预申请信息
        /// <summary>
        /// 逻辑删除提交预申请信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Delete(Guid id)
        {
            return PreApprovalDao.Delete(id);
        }
        #endregion

        #region 通过ID查询预申请信息
        public List<P_PreApproval> GetPreApprovalByID(string PreApprovalID)
        {
            return PreApprovalDao.GetPreApprovalID(PreApprovalID);
        }
        #endregion

        #region 通过ID查询成本中心信息
        public D_COSTCENTER GetCostCenterByID(string CostCenterID)
        {
            return PreApprovalDao.GetCostCenterByID(CostCenterID);
        }
        #endregion

        #region 保存预申请查询详情更改
        #region 获取HTCode编号方法
        /// <summary>
        /// 获取HTCode编号方法
        /// </summary>
        /// <returns></returns>
        public HTCode GetHTCode()
        {
            return PreApprovalDao.GetHTCode();
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
            return PreApprovalDao.FindInfo(ta, region, costCenter);
        }
        #endregion

        //public int SaveChange(string ID, string txtMarket, string txtTA, string txtBUHeadName, string txtBUHeadMUDID, string txtRegion, string txtRegionManagerName, string txtRegionManagerMUDID, string RDSDManagerName, string RDSDManagerMUDID, string txtCostCenter, string OldCostCenter, string name)
        //{
        //    return PreApprovalDao.SaveChange(ID, txtMarket, txtTA, txtBUHeadName, txtBUHeadMUDID, txtRegion, txtRegionManagerName, txtRegionManagerMUDID, RDSDManagerName, RDSDManagerMUDID, txtCostCenter, OldCostCenter, name);
        //}

        public int SaveChange(string ID, string txtTERRITORY_TA, string txtBUHeadName, string txtBUHeadMUDID, string name)
        {
            return PreApprovalDao.SaveChange(ID, txtTERRITORY_TA, txtBUHeadName, txtBUHeadMUDID, name);
        }

        public int UpdatePreApprovalBUHead(Guid ID, string txtBUHeadMUDID, string txtBUHeadName)
        {
            return PreApprovalDao.UpdatePreApprovalBUHead(ID, txtBUHeadMUDID, txtBUHeadName);
        }
        #endregion

        #region 预申请MMCoE审批记录
        public List<P_PreApprovalApproveHistory_View> RecordsLoad(string srh_HTCode, string srh_ApplierMUDID, string srh_BUHeadMUDID, string srh_startMeetingDate, string srh_endMeetingDate, string srh_State, string srh_StartApproveDate, string srh_EndApproveDate, int rows, int page, out int total)
        {
            return PreApprovalDao.RecordsLoad(srh_HTCode, srh_ApplierMUDID, srh_BUHeadMUDID, srh_startMeetingDate, srh_endMeetingDate, srh_State, srh_StartApproveDate, srh_EndApproveDate, rows, page, out total);
        }
        #endregion

        #region 预申请MMCoE审批
        public List<P_PreApproval> MMCoELoad(string srh_HTCode, string srh_ApplierMUDID, string srh_startMeetingDate, string srh_endMeetingDate, int rows, int page, out int total)
        {
            return PreApprovalDao.MMCoELoad(srh_HTCode, srh_ApplierMUDID, srh_startMeetingDate, srh_endMeetingDate, rows, page, out total);
        }
        #endregion

        #region 预申请MMCoE审批--通过
        public int Approved(Guid ID, string PID, string UserName, string UserId, int ActionType, DateTime ApproveDate, string txtComments, int type)
        {
            return PreApprovalDao.Approved(ID, PID, UserName, UserId, ActionType, ApproveDate, txtComments, type);
        }
        #endregion

        #region 修改预申请状态--通过
        public int UpdateStadeApproved(string ID, string PreApprovalState)
        {
            return PreApprovalDao.UpdateStadeApproved(ID, PreApprovalState);
        }
        #endregion

        #region 查询预申请详情
        /// <summary>
        /// 查询预申请详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public P_PreApproval LoadPreApprovalInfo(Guid id)
        {
            return PreApprovalDao.LoadPreApprovalInfo(id);
        }


        #endregion

        #region 预申请BUHead审批通过
        /// <summary>
        /// 预申请BUHead审批通过
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public int BUHeadApprove(Guid id, int state, string reason)
        {
            return PreApprovalDao.BUHeadApprove(id, state, reason);
        }
        #endregion

        #region 预申请MMCoE审批通过
        /// <summary>
        /// 预申请BUHead审批通过
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public int MMCoEApprove(Guid id, int state, string reason)
        {
            return PreApprovalDao.MMCoEApprove(id, state, reason);
        }
        #endregion

        #region 预申请BUHead审批驳回
        /// <summary>
        /// 预申请BUHead审批通过
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public int BUHeadReject(Guid id, int state, string reason)
        {
            return PreApprovalDao.BUHeadReject(id, state, reason);
        }
        #endregion

        #region 预申请MMCoE审批驳回
        /// <summary>
        /// 预申请BUHead审批通过
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public int MMCoEReject(Guid id, int state, string reason)
        {
            return PreApprovalDao.MMCoEReject(id, state, reason);
        }
        #endregion

        #region 成本中心导入
        public int ImportCostCenter(List<D_COSTCENTER> excelRows, ref List<D_COSTCENTER> fails, string adminUser)
        {
            foreach (var item in excelRows)
            {
                fails.Clear();
                var count = PreApprovalDao.findCostCenter(item.TA, item.Region, item.CostCenter).CostCenterCount;
                fails.Add(item);
                if (count > 0)
                {
                    PreApprovalDao.UpdateCostCenter(fails, adminUser, item.TA, item.Region, item.CostCenter);
                }
                else
                {
                    if (!string.IsNullOrEmpty(item.OldCostCenter))
                    {
                        count = PreApprovalDao.findCostCenter(item.TA, item.Region, item.OldCostCenter).CostCenterCount;
                        if (count > 0)
                        {
                            PreApprovalDao.UpdateCostCenter(fails, adminUser, item.TA, item.Region, item.OldCostCenter);
                        }
                        else
                        {
                            PreApprovalDao.ImportCostCenter(fails, adminUser);
                        }
                    }
                    else
                    {
                        PreApprovalDao.ImportCostCenter(fails, adminUser);
                    }
                }
            }
            return 1;


        }
        #endregion

        #region 将未完成的旧成本中心换成新成本中心
        public int UpdateUncompleteCostenterCodeByOldCostCenterCode(string CostCenterCode, string OldCostcenterCode)
        {
            return PreApprovalDao.UpdateUncompleteCostenterCodeByOldCostCenterCode(CostCenterCode, OldCostcenterCode);
        }
        #endregion

        #region 查询当前TERRITORY_TA
        public List<D_COSTCENTER> GetTERRITORY_TAByID(Guid id)
        {
            return PreApprovalDao.GetTERRITORY_TAByID(id);
        }
        #endregion

        #region 删除成本中心

        /// <summary>
        /// 删除成本中心
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public int Del(Guid id)
        {
            return PreApprovalDao.Del(id);
        }

        #endregion

        #region 查询该成本中心是否存在未审批的预申请
        public int _Exist(Guid ID)
        {
            var _Entity = PreApprovalDao.FindCostCenterByID(ID);
            string CostCenter = _Entity.TA + "-" + _Entity.Region + "(" + _Entity.CostCenter + ")";
            return PreApprovalDao._Exist(CostCenter);

        }
        #endregion

        #region 查询该TERRITORY_TA是否存在未审批的预申请
        public int GetStateByTA(string TERRITORY_TA)
        {           
            return PreApprovalDao.GetStateByTA(TERRITORY_TA);
        }
        #endregion

        public List<P_PreApprovalApproveHistory> GetApproval(string id)
        {
            return PreApprovalDao.GetApproval(id);
        }

        public HTCode GetHTCodeByID(string htcodeId)
        {
            return PreApprovalDao.GetHTCodeByID(htcodeId);
        }

        public int AddPreApprovalApproveHistory(P_PreApprovalApproveHistory PreApprovalHistory)
        {
            return PreApprovalDao.AddPreApprovalApproveHistory(PreApprovalHistory);
        }

        public List<PreApprovalState> LoadMyPreApprovalUserId(string UserId, DateTime Begin, DateTime End, string State, string Budget, int rows, int page, out int total)
        {
            return PreApprovalDao.LoadMyPreApprovalUserId(UserId, Begin, End, State, Budget, rows, page, out total);
        }

        public List<P_PreApproval> LoadMyApprove(string UserId, DateTime Begin, DateTime End, string State, string Applicant, int rows, int page, out int total)
        {
            return PreApprovalDao.LoadMyApprove(UserId, Begin, End, State, Applicant, rows, page, out total);
        }
        public List<P_PreApproval> LoadCurrentApprove(string UserId, DateTime Begin, DateTime End, string State, string Applicant, int rows, int page, out int total)
        {
            return PreApprovalDao.LoadCurrentApprove(UserId, Begin, End, State, Applicant, rows, page, out total);
        }
        public List<P_PreApprovalApproveHistory> LoadApprovalRecords(Guid PID)
        {
            return PreApprovalDao.LoadApprovalRecords(PID);
        }
        public List<P_PreApproval> LoadMyApproveAll(string UserId, string Applicant)
        {
            return PreApprovalDao.LoadMyApproveAll(UserId, Applicant);
        }

        public List<P_PreApproval> LoadHTCode(string UserID)
        {
            return PreApprovalDao.LoadHTCode(UserID);
        }
        public List<P_PreApproval> FindPreApprovalByHTCode(string HTCode)
        {
            return PreApprovalDao.FindPreApprovalByHTCode(HTCode);
        }
        public bool HasApproveRights(string HTCode)
        {
            return PreApprovalDao.HasApproveRights(HTCode);
        }
        public bool HasApprove(string HTCode)
        {
            return PreApprovalDao.HasApprove(HTCode);
        }
        public bool HasApproveByTA(string HTCode,string TA)
        {
            return PreApprovalDao.HasApproveByTA(HTCode, TA);
        }

        public bool HasFileApproveRights(string UserId)
        {
            return PreApprovalDao.HasFileApproveRights(UserId);
        }

        public List<P_PreApprovalApproveHistory> FindPreApprovalApproveHistory(Guid PID)
        {
            return PreApprovalDao.FindPreApprovalApproveHistory(PID);
        }

        public P_PreApprovalApproveHistory LoadApproveHistoryInfo(Guid PID, int Type)
        {
            return PreApprovalDao.LoadApproveHistoryInfo(PID, Type);
        }
        public P_PreApprovalApproveHistory LoadApproveHistory(Guid PID, int Type)
        {
            return PreApprovalDao.LoadApproveHistory(PID, Type);
        }
        public P_PreApprovalApproveHistory LoadApproveHistoryRefused(Guid PID, int Type, string UserId)
        {
            return PreApprovalDao.LoadApproveHistoryRefused(PID, Type,UserId);
        }
        public P_ORDER FindActivityOrderByHTCode(string HTCode)
        {
            return PreApprovalDao.FindActivityOrderByHTCode(HTCode);
        }

        public P_PreApproval CheckPreApprovalState(string HTCode)
        {
            return PreApprovalDao.CheckPreApprovalState(HTCode);
        }

        public List<Approval_State> LoadPreApprovalState(int rows, int page, out int total)
        {
            return PreApprovalDao.LoadPreApprovalState(rows, page, out total);
        }

        #region 审批状态查询
        public List<Approval_State> QueryLoad(string srh_HTCode, string srh_ApplierMUDID, string srh_BUHeadMUDID, string srh_Category, string srh_Type, int rows, int page, out int total)
        {
            return PreApprovalDao.QueryLoad(srh_HTCode, srh_ApplierMUDID, srh_BUHeadMUDID, srh_Category, srh_Type, rows, page, out total);
        }

        #endregion

        public List<Approval_State> LoadPreApprovalUpload(string MUDID, int rows, int page, out int total)
        {
            return PreApprovalDao.LoadPreApprovalUpload(MUDID, rows, page, out total);
        }

        public List<P_PreApproval> LoadPreApprovalByCostCenter(string CostCenter)
        {
            return PreApprovalDao.LoadPreApprovalByCostCenter(CostCenter);
        }


        public List<P_PreApproval> LoadPreApprovalByCurrentApprover(string CurrentApprover, string CostCenter)
        {
            return PreApprovalDao.LoadPreApprovalByCurrentApprover(CurrentApprover, CostCenter);
        }

        public int UpdatePendingPreAPprovalBUHead(string HTCode, string BUHeadName, string BUHeadMUDID, string PreBUHeadMUDID)
        {
            return PreApprovalDao.UpdatePendingPreAPprovalBUHead(HTCode, BUHeadName, BUHeadMUDID, PreBUHeadMUDID);
        }

        #region 查询MMCoE历史订单----1.0 CN字段以CN开头的
        /// <summary>
        /// 查询MMCoE历史订单----1.0 CN字段以CN开头的
        /// </summary>
        /// <returns></returns>
        public List<P_ORDER> GetMMCoEHisOrder()
        {
            return PreApprovalDao.GetMMCoEHisOrder();
        }
        #endregion

        public List<Approval_State> LoadUploadPage(string HTCode, string ApplierMUDID, string BUHeadMUDID, string Type, string State, int rows, int page, out int total)
        {
            return PreApprovalDao.LoadUploadPage(HTCode, ApplierMUDID, BUHeadMUDID, Type, State, rows, page, out total);
        }

        public List<Approval_State> LoadByID(string ids)
        {
            return PreApprovalDao.LoadByID(ids);
        }

        public int UpdatePreReAssign(string ids, string userId, string name, string mudId, string headName)
        {
            return PreApprovalDao.UpdatePreReAssign(ids, userId, name, mudId, headName);
        }
        public int UpdatePuoReAssign(string ids, string userId, string name, string mudId, string headName)
        {
            return PreApprovalDao.UpdatePuoReAssign(ids, userId, name, mudId, headName);
        }
        public int UpdatePuoReAssignByHTCode(string HtCode, string UserId, string Name, string MUDID, string HeadName)
        {
            return PreApprovalDao.UpdatePuoReAssignByHTCode(HtCode, UserId, Name, MUDID, HeadName);
        }

        public int InsertFileLink(string FilePath, string Email)
        {
            return PreApprovalDao.InsertFileLink(FilePath, Email);
        }

        public string GetOldGskHospitalCodeByGskHospitalCode(string GskHospitalCode)
        {
            return PreApprovalDao.GetOldGskHospitalCodeByGskHospitalCode(GskHospitalCode);
        }

        public string GetOldCostcenterByCostcenter(string Costcenter)
        {
            return PreApprovalDao.GetOldCostcenterByCostcenter(Costcenter);
        }

        #region 更新预申请医院地址
        /// <summary>
        /// 更新预申请医院地址
        /// </summary>
        /// <param name="preApprovalId"></param>
        /// <param name="hospitalAddress"></param>
        /// <returns></returns>
        public int UpdateAddress(string preApprovalId, string hospitalAddress)
        {
            return PreApprovalDao.UpdateAddress(preApprovalId, hospitalAddress);
        }
        #endregion


        #region 查询未审批的预申请
        public List<P_PreApproval> GetPreApprovalByUser()
        {
            return PreApprovalDao.GetPreApprovalByUser();
        }

        public List<P_PreApproval> GetPreApprovalByUser(string UserID)
        {
            return PreApprovalDao.GetPreApprovalByUser(UserID);
        }
        #endregion

        #region 新增地址
        public List<P_AddressApproval> LoadMyAddressApprovalByUserId(string UserId, DateTime Begin, DateTime End, string State, string Budget, int rows, int page, out int total)
        {
            return PreApprovalDao.LoadMyAddressApprovalByUserId(UserId, Begin, End, State, Budget, rows, page, out total);
        }

        public List<P_AddressApproval> LoadMyAddressApprove(string UserId, DateTime Begin, DateTime End, string State, string Applicant, int rows, int page, out int total)
        {
            return PreApprovalDao.LoadMyAddressApprove(UserId, Begin, End, State, Applicant, rows, page, out total);
        }

        public P_AddressApproval_View LoadAddressApprovalInfo(Guid id)
        {
            return PreApprovalDao.LoadAddressApprovalInfo(id);
        }

        public P_AddressApproval_View LoadAddressApprovalInfoForUpdate(Guid id)
        {
            return PreApprovalDao.LoadAddressApprovalInfoForUpdate(id);
        }

        public int AddressApprove(P_AddressApproval_View p_addressApproval_View, string reason)
        {
            return PreApprovalDao.AddressApprove(p_addressApproval_View, reason);
        }

        public int AddAddressApproveHistory(P_AddressApproveHistory p_addressApproveHistory)
        {
            return PreApprovalDao.AddAddressApproveHistory(p_addressApproveHistory);
        }

        public List<P_AddressApproval_View> LoadMyAddressApproveAll(string UserId, string applicant)
        {
            return PreApprovalDao.LoadMyAddressApproveAll(UserId, applicant);
        }

        public List<P_AddressApproval> LoadInvalidAddressApplication(DateTime nowDate)
        {
            return PreApprovalDao.LoadInvalidAddressApplication(nowDate);
        }

        public int InvalidAddressApplication(List<Guid> guids, int state)
        {
            return PreApprovalDao.InvalidAddressApplication(guids,state);
        }

        public List<P_AddressApproval_View> LoadAddressApprove(string srh_DACode, string srh_ApplierMUDID, string srh_ApproverMUDID, string srh_GskHospital, string srh_StartApplyDate, string srh_EndApplyDate, string srh_State, string srh_IsDelete, int rows, int page, out int total)
        {
            return PreApprovalDao.LoadAddressApprove(srh_DACode, srh_ApplierMUDID, srh_ApproverMUDID, srh_GskHospital, srh_StartApplyDate, srh_EndApplyDate, srh_State, srh_IsDelete, rows, page, out total);
        }

        public List<WP_QYUSER> LoadWPQYUSER()
        {
            return PreApprovalDao.LoadWPQYUSER();
        }

        public List<P_AddressApproveHistory> LoadAddressApprovalHistory(Guid DA_ID)
        {
            return PreApprovalDao.LoadAddressApprovalHistory(DA_ID);
        }

        public List<P_AddressApproval_View> ExportAddressApprovalList(string srh_DACode, string srh_ApplierMUDID, string srh_ApproverMUDID, string srh_GskHospital, string srh_StartApplyDate, string srh_EndApplyDate, string srh_State, string srh_IsDelete)
        {
            return PreApprovalDao.ExportAddressApprovalList(srh_DACode, srh_ApplierMUDID, srh_ApproverMUDID, srh_GskHospital, srh_StartApplyDate, srh_EndApplyDate, srh_State, srh_IsDelete);
        }

        public List<P_AddressApproval> LoadAddressApprovalByDACode(string dA_CODE)
        {
            return PreApprovalDao.LoadAddressApprovalByDACode(dA_CODE);
        }

        public List<P_AddressApproval> LoadMyAddressApproveCount(string userId, DateTime begin, DateTime end, string state, string applicant)
        {
            return PreApprovalDao.LoadMyAddressApproveCount(userId, begin, end, state, applicant);
        }
        #endregion

        #region 取消预申请
        public int PreApprovalCancel(P_PreApproval p_preApproval)
        {
            return PreApprovalDao.PreApprovalCancel(p_preApproval);
        }
        #endregion

        #region 根据成本中心表TERRITORY_TA查询预申请表待审批状态数据
        public List<P_PreApproval> GetDataByTAAndState(string TERRITORY_TA)
        {
            return PreApprovalDao.GetDataByTAAndState(TERRITORY_TA);
        }
        #endregion

        //预申请分析
        public List<P_PreApproval_CountAmount_View> LoadPreApprovalData(string userId, string position, string territoryCode, string begin, string end)
        {
            return PreApprovalDao.LoadPreApprovalData(userId, position, territoryCode, begin, end);
        }
        //预申请上层分析
        public List<P_PreApproval_OwnBelongCountAmount> LoadPreApprovalUpData(string userId, string position, string territoryCode, string begin, string end)
        {
            return PreApprovalDao.LoadPreApprovalUpData(userId, position, territoryCode, begin, end);
        }


        #region 同步预申请表
        public int SyncPreApproval()
        {
            return PreApprovalDao.SyncPreApproval();
        }
        #endregion

        public V_COST_SUMMARY GetPreApprovalList(List<string> TerritoryStr, string StartDate, string EndDate)
        {
            return PreApprovalDao.GetPreApprovalList(TerritoryStr, StartDate, EndDate);
        }

    }
}
