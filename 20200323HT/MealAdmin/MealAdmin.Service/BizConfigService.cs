using MealAdmin.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XFramework.XInject.Attributes;
using MealAdmin.Entity;

namespace MealAdmin.Service
{
    public class BizConfigService : IBizConfigService
    {
        [Bean("bizConfigDao")]
        public IBizConfigDao bizConfigDao { get; set; }

        public P_MARKET_INVOICE_OBJ GetAllMarkets()
        {
            var markets = bizConfigDao.GetAllMarkets();
            P_MARKET_INVOICE_OBJ obj = new P_MARKET_INVOICE_OBJ();
            if (markets.Count > 0)
            {
                foreach (var _v in markets)
                {
                    switch(_v.Name)
                    {
                        case "Rx":
                            obj.Rx = _v.InvoiceTitle;
                            obj.RxDutyParagraph = _v.DutyParagraph;
                            break;
                        case "Vx":
                            obj.Vx = _v.InvoiceTitle;
                            obj.VxDutyParagraph = _v.DutyParagraph;
                            break;
                        case "TSKF":
                            obj.TSKF = _v.InvoiceTitle;
                            obj.TSKFDutyParagraph = _v.DutyParagraph;
                            break;
                        case "DDT":
                            obj.DDT = _v.InvoiceTitle;
                            obj.DDTDutyParagraph = _v.DutyParagraph;
                            break;
                        case "R&D":
                            obj.RD = _v.InvoiceTitle;
                            obj.RDDutyParagraph = _v.DutyParagraph;
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                obj.Rx = string.Empty;
                obj.RxDutyParagraph = string.Empty;
                obj.Vx = string.Empty;
                obj.VxDutyParagraph = string.Empty;
                obj.TSKF = string.Empty;
                obj.TSKFDutyParagraph = string.Empty;
                obj.DDT = string.Empty;
                obj.DDTDutyParagraph = string.Empty;
                obj.RD = string.Empty;
                obj.RDDutyParagraph = string.Empty;
            }
            return obj;
        }

        public P_BIZ_CONF_OBJ GetConfig()
        {
            var configs = bizConfigDao.GetAllConfig();
            P_BIZ_CONF_OBJ obj = new P_BIZ_CONF_OBJ();
            BizConfID _key;
            if (configs.Count > 0)
            {
                foreach (var _v in configs)
                {
                    if (Enum.TryParse<BizConfID>(_v.ID.ToString(), out _key) == true)
                    {
                        switch (_key)
                        {
                            case BizConfID.ApprovePeopleNum:
                                obj._1_ApprovePeopleNum = int.Parse(_v.Val1);
                                break;
                            case BizConfID.ApprovePercapitaPrice:
                                obj._2_ApprovePercapitaPrice = decimal.Parse(_v.Val1);
                                break;
                            case BizConfID.SysWorkTimeSpan:
                                obj._3_1_SysWorkTimeBegin = _v.Val1;
                                obj._3_2_SysWorkTimeEnd = _v.Val2;
                                break;
                            case BizConfID.ThatDayCanOrderMealTimeSpan:
                                obj._4_1_ThatDayCanOrderMealTimeBegin = _v.Val1;
                                obj._4_2_ThatDayCanOrderMealTimeEnd = _v.Val2;
                                break;
                            case BizConfID.ThatDayCanSendMealTimeSpan:
                                obj._5_1_ThatDayCanSendMealTimeBegin = _v.Val1;
                                obj._5_2_ThatDayCanSendMealTimeEnd = _v.Val2;
                                break;
                            case BizConfID.NotThatDayCanSendMealTimeSpan:
                                obj._6_1_NotThatDayCanSendMealTimeBegin = _v.Val1;
                                obj._6_2_NotThatDayCanSendMealTimeEnd = _v.Val2;
                                break;
                            case BizConfID.OrderingBufferMin:
                                obj._7_OrderingBufferMin = int.Parse(_v.Val1);
                                break;
                            case BizConfID.SendBriefReportHour:
                                obj._8_SendBriefReportHour = int.Parse(_v.Val1);
                                break;
                            case BizConfID.SyncXMSReportTime:
                                obj._9_SyncXMSReportTime = _v.Val1;
                                break;
                            case BizConfID.SyncHospoitalIntervalHour:
                                obj._10_SyncHospoitalIntervalHour = int.Parse(_v.Val1);
                                break;
                            case BizConfID.SysOperableTime:
                                obj._11_1_SysOperableTimeBegin = _v.Val1;
                                obj._11_2_SysOperableTimeEnd = _v.Val2;
                                break;
                            case BizConfID.PreApprovalCanApplyMeetingTimeSpan:
                                obj._12_1_PreApprovalCanApplyMeetingTimeSpanBegin = _v.Val1;
                                obj._12_2_PreApprovalCanApplyMeetingTimeSpanEnd = _v.Val2;
                                break;
                            case BizConfID.PreApprovalNextWorkDaySendTimeSpan:
                                obj._13_1_PreApprovalNextWorkDaySendTimeSpanBegin = _v.Val1;
                                obj._13_2_PreApprovalNextWorkDaySendTimeSpanEnd = _v.Val2;
                                break;
                            case BizConfID.PreWorkTimeSpan:
                                obj._14_1_PreWorkTimeSpanBegin = _v.Val1;
                                obj._14_2_PreWorkTimeSpanEnd = _v.Val2;
                                break;
                            case BizConfID.PreOperableTimeSpan:
                                obj._15_1_PreOperableTimeSpanBegin = _v.Val1;
                                obj._15_2_PreOperableTimeSpanEnd = _v.Val2;
                                break;
                            case BizConfID.UploadWorkTimeSpan:
                                obj._16_1_UploadWorkTimeSpanBegin = _v.Val1;
                                obj._16_2_UploadWorkTimeSpanEnd = _v.Val2;
                                break;
                            case BizConfID.UploadOperableTimeSpan:
                                obj._17_1_UploadOperableTimeSpanBegin = _v.Val1;
                                obj._17_2_UploadOperableTimeSpanEnd = _v.Val2;
                                break;

                            case BizConfID.AddAddressTimeSpan:
                                obj._18_1_AddAddressTimeSpanBegin = _v.Val1;
                                obj._18_2_AddAddressTimeSpanEnd = _v.Val2;
                                break;
                            case BizConfID.ResubmitAddressTimeSpan:
                                obj._19_1_ResubmitAddressTimeSpanBegin = _v.Val1;
                                obj._19_2_ResubmitAddressTimeSpanEnd = _v.Val2;
                                break;
                            case BizConfID.UpdateAddressTimeSpan:
                                obj._20_1_UpdateAddressTimeSpanBegin = _v.Val1;
                                obj._20_2_UpdateAddressTimeSpanEnd = _v.Val2;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            else
            {
                obj._1_ApprovePeopleNum = 0;
                obj._2_ApprovePercapitaPrice = 0;
                obj._3_1_SysWorkTimeBegin = string.Empty;
                obj._3_2_SysWorkTimeEnd = string.Empty;
                obj._4_1_ThatDayCanOrderMealTimeBegin = string.Empty;
                obj._4_2_ThatDayCanOrderMealTimeEnd = string.Empty;
                obj._5_1_ThatDayCanSendMealTimeBegin = string.Empty;
                obj._5_2_ThatDayCanSendMealTimeEnd = string.Empty;
                obj._6_1_NotThatDayCanSendMealTimeBegin = string.Empty;
                obj._6_2_NotThatDayCanSendMealTimeEnd = string.Empty;
                obj._7_OrderingBufferMin = 0;
                obj._8_SendBriefReportHour = 0;
                obj._9_SyncXMSReportTime = string.Empty;
                obj._10_SyncHospoitalIntervalHour = 0;
                obj._11_1_SysOperableTimeBegin = string.Empty;
                obj._11_2_SysOperableTimeEnd = string.Empty;
                obj._12_1_PreApprovalCanApplyMeetingTimeSpanBegin = string.Empty;
                obj._12_2_PreApprovalCanApplyMeetingTimeSpanEnd = string.Empty;
                obj._13_1_PreApprovalNextWorkDaySendTimeSpanBegin = string.Empty;
                obj._13_2_PreApprovalNextWorkDaySendTimeSpanEnd = string.Empty;
                obj._14_1_PreWorkTimeSpanBegin = string.Empty;
                obj._14_2_PreWorkTimeSpanEnd = string.Empty;
                obj._15_1_PreOperableTimeSpanBegin = string.Empty;
                obj._15_2_PreOperableTimeSpanEnd = string.Empty;
                obj._16_1_UploadWorkTimeSpanBegin = string.Empty;
                obj._16_2_UploadWorkTimeSpanEnd = string.Empty;
                obj._17_1_UploadOperableTimeSpanBegin = string.Empty;
                obj._17_2_UploadOperableTimeSpanEnd = string.Empty;

                obj._18_1_AddAddressTimeSpanBegin = string.Empty;
                obj._18_2_AddAddressTimeSpanEnd = string.Empty;
                obj._19_1_ResubmitAddressTimeSpanBegin = string.Empty;
                obj._19_2_ResubmitAddressTimeSpanEnd = string.Empty;
                obj._20_1_UpdateAddressTimeSpanBegin = string.Empty;
                obj._20_2_UpdateAddressTimeSpanEnd = string.Empty;
            }
            return obj;
        }

        /// <summary>
        /// 检查指定格式时间区间是否合法 
        /// </summary>
        /// <param name="TimeBegin">8:00</param>
        /// <param name="TimeEnd">20:30</param>
        /// <returns></returns>
        private bool CheckTimeSpan(string TimeBegin, string TimeEnd)
        {
            bool rtnVal;
            if (string.IsNullOrEmpty(TimeBegin) == false && string.IsNullOrEmpty(TimeEnd) == false)
            {
                DateTime beginDate, endDate;
                string tmpStr1 = "2017-05-31 " + TimeBegin + ":00";
                string tmpStr2 = "2017-05-31 " + TimeEnd + ":00";
                if (DateTime.TryParse(tmpStr1, out beginDate) == true 
                    && DateTime.TryParse(tmpStr2, out endDate) == true
                    && endDate > beginDate)
                {
                    rtnVal = true;
                }
                else
                {
                    rtnVal = false;
                }
            }
            else
            {
                rtnVal = false;
            }

            return rtnVal;
        }

        public bool CheckTime(string Time)
        {
            bool rtnVal;
            if (string.IsNullOrEmpty(Time) == false)
            {
                string tmpStr1 = "2017-05-31 " + Time + ":00";
                DateTime date;
                rtnVal = DateTime.TryParse(tmpStr1, out date);
            }
            else
            {
                rtnVal = false;
            }

            return rtnVal;
        }

        public int UpdateConfig(P_BIZ_CONF_OBJ entity, out List<P_BIZ_CONF> unSuccessData)
        {
            int updCnt = 0;
            if (entity != null)
            {
                List<P_BIZ_CONF> saveList = new List<P_BIZ_CONF>();
                List<P_BIZ_CONF> notNeedSaveList = new List<P_BIZ_CONF>();
                P_BIZ_CONF cnf = new P_BIZ_CONF() { ID = (int)BizConfID.ApprovePeopleNum, Name = BizConfID.ApprovePeopleNum.ToString(), Val1 = entity._1_ApprovePeopleNum.ToString(), Val2 = string.Empty, Val3 = string.Empty };
                saveList.Add(cnf);
                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.ApprovePercapitaPrice, Name = BizConfID.ApprovePercapitaPrice.ToString(), Val1 = entity._2_ApprovePercapitaPrice.ToString("#0.00"), Val2 = string.Empty, Val3 = string.Empty };
                saveList.Add(cnf);
                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.SysWorkTimeSpan, Name = BizConfID.SysWorkTimeSpan.ToString(), Val1 = entity._3_1_SysWorkTimeBegin, Val2 = entity._3_2_SysWorkTimeEnd, Val3 = string.Empty };
                if (CheckTimeSpan(entity._3_1_SysWorkTimeBegin, entity._3_2_SysWorkTimeEnd) == true)
                {
                    saveList.Add(cnf);
                }
                else
                {
                    notNeedSaveList.Add(cnf);
                }

                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.ThatDayCanOrderMealTimeSpan, Name = BizConfID.ThatDayCanOrderMealTimeSpan.ToString(), Val1 = entity._4_1_ThatDayCanOrderMealTimeBegin, Val2 = entity._4_2_ThatDayCanOrderMealTimeEnd, Val3 = string.Empty };
                if (CheckTimeSpan(entity._4_1_ThatDayCanOrderMealTimeBegin, entity._4_2_ThatDayCanOrderMealTimeEnd) == true)
                {
                    saveList.Add(cnf);
                }
                else
                {
                    notNeedSaveList.Add(cnf);
                }

                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.ThatDayCanSendMealTimeSpan, Name = BizConfID.ThatDayCanSendMealTimeSpan.ToString(), Val1 = entity._5_1_ThatDayCanSendMealTimeBegin, Val2 = entity._5_2_ThatDayCanSendMealTimeEnd, Val3 = string.Empty };
                if (CheckTimeSpan(entity._5_1_ThatDayCanSendMealTimeBegin, entity._5_2_ThatDayCanSendMealTimeEnd) == true)
                {
                    saveList.Add(cnf);
                }
                else
                {
                    notNeedSaveList.Add(cnf);
                }

                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.NotThatDayCanSendMealTimeSpan, Name = BizConfID.NotThatDayCanSendMealTimeSpan.ToString(), Val1 = entity._6_1_NotThatDayCanSendMealTimeBegin, Val2 = entity._6_2_NotThatDayCanSendMealTimeEnd, Val3 = string.Empty };
                if (CheckTimeSpan(entity._6_1_NotThatDayCanSendMealTimeBegin, entity._6_2_NotThatDayCanSendMealTimeEnd) == true)
                {
                    saveList.Add(cnf);
                }
                else
                {
                    notNeedSaveList.Add(cnf);
                }

                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.OrderingBufferMin, Name = BizConfID.OrderingBufferMin.ToString(), Val1 = entity._7_OrderingBufferMin.ToString(), Val2 = string.Empty, Val3 = string.Empty };
                saveList.Add(cnf);
                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.SendBriefReportHour, Name = BizConfID.SendBriefReportHour.ToString(), Val1 = entity._8_SendBriefReportHour.ToString(), Val2 = string.Empty, Val3 = string.Empty };
                saveList.Add(cnf);

                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.SyncXMSReportTime, Name = BizConfID.SyncXMSReportTime.ToString(), Val1 = entity._9_SyncXMSReportTime.ToString(), Val2 = string.Empty, Val3 = string.Empty };
                if (CheckTime(entity._9_SyncXMSReportTime) == true)
                {
                    saveList.Add(cnf);
                }
                else
                {
                    notNeedSaveList.Add(cnf);
                }
                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.SyncHospoitalIntervalHour, Name = BizConfID.SyncHospoitalIntervalHour.ToString(), Val1 = entity._10_SyncHospoitalIntervalHour.ToString(), Val2 = string.Empty, Val3 = string.Empty };
                saveList.Add(cnf);

                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.SysOperableTime, Name = BizConfID.SysOperableTime.ToString(), Val1 = entity._11_1_SysOperableTimeBegin, Val2 = entity._11_2_SysOperableTimeEnd, Val3 = string.Empty };
                if (CheckTimeSpan(entity._11_1_SysOperableTimeBegin, entity._11_2_SysOperableTimeEnd) == true)
                {
                    saveList.Add(cnf);
                }
                else
                {
                    notNeedSaveList.Add(cnf);
                }
                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.PreApprovalCanApplyMeetingTimeSpan, Name = BizConfID.PreApprovalCanApplyMeetingTimeSpan.ToString(), Val1 = entity._12_1_PreApprovalCanApplyMeetingTimeSpanBegin, Val2 = entity._12_2_PreApprovalCanApplyMeetingTimeSpanEnd, Val3 = string.Empty };
                if (CheckTimeSpan(entity._12_1_PreApprovalCanApplyMeetingTimeSpanBegin, entity._12_2_PreApprovalCanApplyMeetingTimeSpanEnd) == true)
                {
                    saveList.Add(cnf);
                }
                else
                {
                    notNeedSaveList.Add(cnf);
                }
                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.PreApprovalNextWorkDaySendTimeSpan, Name = BizConfID.PreApprovalNextWorkDaySendTimeSpan.ToString(), Val1 = entity._13_1_PreApprovalNextWorkDaySendTimeSpanBegin, Val2 = entity._13_2_PreApprovalNextWorkDaySendTimeSpanEnd, Val3 = string.Empty };
                if (CheckTimeSpan(entity._13_1_PreApprovalNextWorkDaySendTimeSpanBegin, entity._13_2_PreApprovalNextWorkDaySendTimeSpanEnd) == true)
                {
                    saveList.Add(cnf);
                }
                else
                {
                    notNeedSaveList.Add(cnf);
                }                
                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.PreWorkTimeSpan, Name = BizConfID.PreWorkTimeSpan.ToString(), Val1 = entity._14_1_PreWorkTimeSpanBegin, Val2 = entity._14_2_PreWorkTimeSpanEnd, Val3 = string.Empty };
                if (CheckTimeSpan(entity._14_1_PreWorkTimeSpanBegin, entity._14_2_PreWorkTimeSpanEnd) == true)
                {
                    saveList.Add(cnf);
                }
                else
                {
                    notNeedSaveList.Add(cnf);
                }
                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.PreOperableTimeSpan, Name = BizConfID.PreOperableTimeSpan.ToString(), Val1 = entity._15_1_PreOperableTimeSpanBegin, Val2 = entity._15_2_PreOperableTimeSpanEnd, Val3 = string.Empty };
                if (CheckTimeSpan(entity._15_1_PreOperableTimeSpanBegin, entity._15_2_PreOperableTimeSpanEnd) == true)
                {
                    saveList.Add(cnf);
                }
                else
                {
                    notNeedSaveList.Add(cnf);
                }
                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.UploadWorkTimeSpan, Name = BizConfID.UploadWorkTimeSpan.ToString(), Val1 = entity._16_1_UploadWorkTimeSpanBegin, Val2 = entity._16_2_UploadWorkTimeSpanEnd, Val3 = string.Empty };
                if (CheckTimeSpan(entity._16_1_UploadWorkTimeSpanBegin, entity._16_2_UploadWorkTimeSpanEnd) == true)
                {
                    saveList.Add(cnf);
                }
                else
                {
                    notNeedSaveList.Add(cnf);
                }
                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.UploadOperableTimeSpan, Name = BizConfID.UploadOperableTimeSpan.ToString(), Val1 = entity._17_1_UploadOperableTimeSpanBegin, Val2 = entity._17_2_UploadOperableTimeSpanEnd, Val3 = string.Empty };
                if (CheckTimeSpan(entity._17_1_UploadOperableTimeSpanBegin, entity._17_2_UploadOperableTimeSpanEnd) == true)
                {
                    saveList.Add(cnf);
                }
                else
                {
                    notNeedSaveList.Add(cnf);
                }

                
                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.AddAddressTimeSpan, Name = BizConfID.AddAddressTimeSpan.ToString(), Val1 = entity._18_1_AddAddressTimeSpanBegin, Val2 = entity._18_2_AddAddressTimeSpanEnd, Val3 = string.Empty };
                if (CheckTimeSpan(entity._18_1_AddAddressTimeSpanBegin, entity._18_2_AddAddressTimeSpanEnd) == true)
                {
                    saveList.Add(cnf);
                }
                else
                {
                    notNeedSaveList.Add(cnf);
                }
                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.ResubmitAddressTimeSpan, Name = BizConfID.ResubmitAddressTimeSpan.ToString(), Val1 = entity._19_1_ResubmitAddressTimeSpanBegin, Val2 = entity._19_2_ResubmitAddressTimeSpanEnd, Val3 = string.Empty };
                if (CheckTimeSpan(entity._19_1_ResubmitAddressTimeSpanBegin, entity._19_2_ResubmitAddressTimeSpanEnd) == true)
                {
                    saveList.Add(cnf);
                }
                else
                {
                    notNeedSaveList.Add(cnf);
                }
                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.UpdateAddressTimeSpan, Name = BizConfID.UpdateAddressTimeSpan.ToString(), Val1 = entity._20_1_UpdateAddressTimeSpanBegin, Val2 = entity._20_2_UpdateAddressTimeSpanEnd, Val3 = string.Empty };
                if (CheckTimeSpan(entity._20_1_UpdateAddressTimeSpanBegin, entity._20_2_UpdateAddressTimeSpanEnd) == true)
                {
                    saveList.Add(cnf);
                }
                else
                {
                    notNeedSaveList.Add(cnf);
                }

                updCnt = bizConfigDao.UpdateConfig(saveList, out unSuccessData);
                if (notNeedSaveList.Count > 0)
                {
                    unSuccessData = unSuccessData.Concat(notNeedSaveList).ToList<P_BIZ_CONF>();
                }
            }
            else
            {
                unSuccessData = new List<P_BIZ_CONF>();

                P_BIZ_CONF cnf = new P_BIZ_CONF() { ID = (int)BizConfID.ApprovePeopleNum, Val1 = string.Empty, Val2 = string.Empty, Val3 = string.Empty };
                unSuccessData.Add(cnf);
                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.ApprovePercapitaPrice, Val1 = string.Empty, Val2 = string.Empty, Val3 = string.Empty };
                unSuccessData.Add(cnf);
                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.SysWorkTimeSpan, Val1 = string.Empty, Val2 = string.Empty, Val3 = string.Empty };
                unSuccessData.Add(cnf);
                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.ThatDayCanOrderMealTimeSpan, Val1 = string.Empty, Val2 = string.Empty, Val3 = string.Empty };
                unSuccessData.Add(cnf);
                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.ThatDayCanSendMealTimeSpan, Val1 = string.Empty, Val2 = string.Empty, Val3 = string.Empty };
                unSuccessData.Add(cnf);
                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.NotThatDayCanSendMealTimeSpan, Val1 = string.Empty, Val2 = string.Empty, Val3 = string.Empty };
                unSuccessData.Add(cnf);
                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.OrderingBufferMin, Val1 = string.Empty, Val2 = string.Empty, Val3 = string.Empty };
                unSuccessData.Add(cnf);
                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.SendBriefReportHour, Val1 = string.Empty, Val2 = string.Empty, Val3 = string.Empty };
                unSuccessData.Add(cnf);
                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.SyncXMSReportTime, Val1 = string.Empty, Val2 = string.Empty, Val3 = string.Empty };
                unSuccessData.Add(cnf);
                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.SyncHospoitalIntervalHour, Val1 = string.Empty, Val2 = string.Empty, Val3 = string.Empty };
                unSuccessData.Add(cnf);
                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.PreApprovalCanApplyMeetingTimeSpan, Val1 = string.Empty, Val2 = string.Empty, Val3 = string.Empty };
                unSuccessData.Add(cnf);
                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.PreApprovalCanApplyMeetingTimeSpan, Val1 = string.Empty, Val2 = string.Empty, Val3 = string.Empty };
                unSuccessData.Add(cnf);
                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.PreApprovalNextWorkDaySendTimeSpan, Val1 = string.Empty, Val2 = string.Empty, Val3 = string.Empty };
                unSuccessData.Add(cnf);
                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.PreWorkTimeSpan, Val1 = string.Empty, Val2 = string.Empty, Val3 = string.Empty };
                unSuccessData.Add(cnf);
                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.PreOperableTimeSpan, Val1 = string.Empty, Val2 = string.Empty, Val3 = string.Empty };
                unSuccessData.Add(cnf);
                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.UploadWorkTimeSpan, Val1 = string.Empty, Val2 = string.Empty, Val3 = string.Empty };
                unSuccessData.Add(cnf);
                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.UploadOperableTimeSpan, Val1 = string.Empty, Val2 = string.Empty, Val3 = string.Empty };
                unSuccessData.Add(cnf);

                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.AddAddressTimeSpan, Val1 = string.Empty, Val2 = string.Empty, Val3 = string.Empty };
                unSuccessData.Add(cnf);
                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.ResubmitAddressTimeSpan, Val1 = string.Empty, Val2 = string.Empty, Val3 = string.Empty };
                unSuccessData.Add(cnf);
                cnf = new P_BIZ_CONF() { ID = (int)BizConfID.UpdateAddressTimeSpan, Val1 = string.Empty, Val2 = string.Empty, Val3 = string.Empty };
                unSuccessData.Add(cnf);
            }
            return updCnt;
        }

        public int UpdateMarketsInvoice(P_MARKET_INVOICE_OBJ entity, out List<P_MARKET> unSuccessData)
        {
            int updCnt = 0;
            if (entity != null)
            {
                List<P_MARKET> saveList = new List<P_MARKET>();
                P_MARKET _market = new P_MARKET() { Name= "Rx", InvoiceTitle = entity.Rx,DutyParagraph=entity.RxDutyParagraph };
                saveList.Add(_market);
                _market = new P_MARKET() { Name = "Vx", InvoiceTitle = entity.Vx,DutyParagraph=entity.VxDutyParagraph };
                saveList.Add(_market);
                _market = new P_MARKET() { Name = "TSKF", InvoiceTitle = entity.TSKF,DutyParagraph=entity.TSKFDutyParagraph };
                saveList.Add(_market);
                _market = new P_MARKET() { Name = "DDT", InvoiceTitle = entity.DDT,DutyParagraph=entity.DDTDutyParagraph };
                saveList.Add(_market);
                _market = new P_MARKET() { Name = "RD", InvoiceTitle = entity.RD, DutyParagraph = entity.RDDutyParagraph };
                saveList.Add(_market);
                updCnt = bizConfigDao.UpdateMarkets(saveList, out unSuccessData);
            }
            else
            {
                unSuccessData = new List<P_MARKET>();
                P_MARKET _market = new P_MARKET() { Name = "Rx", InvoiceTitle = string.Empty };
                unSuccessData.Add(_market);
                _market = new P_MARKET() { Name = "Vx", InvoiceTitle = string.Empty };
                unSuccessData.Add(_market);
                _market = new P_MARKET() { Name = "TSKF", InvoiceTitle = string.Empty };
                unSuccessData.Add(_market);
                _market = new P_MARKET() { Name = "DDT", InvoiceTitle = string.Empty };
                unSuccessData.Add(_market);
                _market = new P_MARKET() { Name = "RD", InvoiceTitle = string.Empty };
                unSuccessData.Add(_market);
            }
            return updCnt;
        }
    }
}
