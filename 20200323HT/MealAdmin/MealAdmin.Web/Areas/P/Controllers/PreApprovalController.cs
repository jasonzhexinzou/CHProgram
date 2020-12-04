using Amazon.S3;
using Amazon.S3.Model;
using IamPortal.AppLogin;
using Ionic.Zip;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MealAdmin.Entity;
using MealAdmin.Service;
using MealAdmin.Util;
using MealAdmin.Web.Filter;
using MealAdmin.Web.Handler;
using MealAdminApiClient;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using XFramework.XException;
using XFramework.XInject.Attributes;
using XFramework.XUtil;

namespace MealAdmin.Web.Areas.P.Controllers
{
    public class PreApprovalController : AdminBaseController
    {
        [Bean("preApprovalService")]
        public IPreApprovalService PreApprovalService { get; set; }

        [Bean("baseDataService")]
        public IBaseDataService baseDataService { get; set; }

        [Bean("marketService")]
        public IMarketService marketService { get; set; }
        [Bean("operationAuditService")]
        public IOperationAuditService operationAuditService { get; set; }
        [Bean("userInfoService")]
        public IUserInfoService IUserInfoService { get; set; }

        private static string awsAccessKey = "AKIAPJZX4QFWMMMA2RJA";
        private static string awsSecretKey = "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";
        private static string bucketName = "wechat";
        AmazonS3Config config = new AmazonS3Config()
        {
            ServiceURL = "http://s3.amazonaws.com",
            RegionEndpoint = Amazon.RegionEndpoint.CNNorth1
        };

        [AdminSessionFilter(Order = 1)]
        [PermissionFilter(Order = 2, Permission = "00000000-0000-0000-1000-000000000001")]
        public ActionResult Index()
        {
            //var CurrentAdminUser = Session[MealAdmin.Util.ConstantHelper.CurrentAdminUser] as IamPortal.AppLogin.AdminUser;
            //AdminUser adminUser = Session[ConstantHelper.CurrentAdminUser] as AdminUser;
            return View();
        }
        [AdminSessionFilter(Order = 1)]
        [PermissionFilter(Order = 2, Permission = "00000000-0000-0000-1000-000000000002")]
        public ActionResult CostCenter()
        {
            return View();
        }
        [AdminSessionFilter(Order = 1)]
        [PermissionFilter(Order = 2, Permission = "00000000-0000-0000-1000-000000000003")]
        public ActionResult MMCoEApproval()
        {

            return View();
        }

        public ActionResult ApproveOrders(Guid id, string imageUrl, string BudgetTotal)
        {
            ViewBag.BudgetTotal = BudgetTotal;
            ViewBag.id = id;
            ViewBag.imageUrl = imageUrl;
            return View();
        }
        public ActionResult CostCenterDetails(Guid id)
        {
            ViewBag.id = id;
            return View();
        }

        [AdminSessionFilter(Order = 1)]
        [PermissionFilter(Order = 2, Permission = "00000000-0000-0000-1000-000000000004")]
        public ActionResult Records()
        {
            return View();
        }
        public ActionResult PreApplicationDetails(Guid id)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult EditCostCenter()
        {
            return View();
        }


        #region 预申请最高详情审批人详情（点击修改按钮进入详情）
        [OperationAuditFilter(Operation = "预申请最高详情审批人详情", OperationAuditTypeName = "预申请最高详情审批人详情")]
        public JsonResult CostCenterDetailsLoad(string id)
        {
            var costCenter = PreApprovalService.GetCostCenterByID(id);


            return Json(new { state = 1, data = new { costCenter } });
        }
        #endregion

        #region 预申请查询详情
        [OperationAuditFilter(Operation = "查看预申请详情", OperationAuditTypeName = "查看预申请详情")]
        public JsonResult DetailsLoad(string id)
        {
            var Approval = PreApprovalService.GetApproval(id);
            var a = PreApprovalService.GetPreApprovalByID(id);
            var p = new P_PreApproval_View();
            var obj = new List<P_PreApprovalApproveHistory_Time>();
            p = GetDisplayObj(a[0]);
            foreach (var i in Approval)
            {
                obj.Add(GetTimeObj(i));
            }

            return Json(new { state = 1, data = p, data1 = obj });
        }


        #endregion

        #region 审批流程时间处理
        private P_PreApprovalApproveHistory_Time GetTimeObj(P_PreApprovalApproveHistory itm)
        {
            P_PreApprovalApproveHistory_Time rtnData = new Entity.P_PreApprovalApproveHistory_Time();
            if (itm.ActionType == 1)
            {
                rtnData.ActionType = "预申请提交成功";
            }
            if (itm.ActionType == 2)
            {
                rtnData.ActionType = "预申请审批驳回";
            }
            if (itm.ActionType == 3)
            {
                rtnData.ActionType = "预申请审批通过";
            }
            if (itm.ActionType == 4)
            {
                rtnData.ActionType = "预申请修改成功";
            }
            if (itm.ActionType == 5)
            {
                rtnData.ActionType = "预申请取消成功";
            }

            rtnData.ApproveDate = itm.ApproveDate.Value.ToString("yyyy-MM-dd HH:mm");
            rtnData.Comments = itm.Comments;
            rtnData.ID = itm.ID;
            rtnData.PID = itm.PID;
            rtnData.type = itm.type;
            rtnData.UserId = itm.UserId;
            rtnData.UserName = itm.UserName;
            return rtnData;
        }
        #endregion

        #region 预申请查询日期处理
        private P_PreApproval_View GetDisplayObj(P_PreApproval itm)
        {
            P_PreApproval_View rtnData = new Entity.P_PreApproval_View();
            rtnData.ApplierMobile = itm.ApplierMobile;
            rtnData.ApplierMUDID = itm.ApplierMUDID;
            rtnData.ApplierName = itm.ApplierName;
            rtnData.AttendCount = itm.AttendCount;
            rtnData.BudgetTotal = itm.BudgetTotal.ToString("N");
            rtnData.BUHeadApproveDate = itm.BUHeadApproveDate != null ? itm.BUHeadApproveDate.Value.ToString("yyyy-MM-dd") : string.Empty;
            rtnData.BUHeadApproveTime = itm.BUHeadApproveDate != null ? itm.BUHeadApproveDate.Value.ToString("HH:mm:ss") : string.Empty;
            //if (itm.AttendCount < 60 && itm.BudgetTotal < 1200)
            //{
            //    rtnData.BUHeadName = "系统自动审批";
            //    rtnData.BUHeadMUDID = "系统自动审批";
            //}
            ////else if(itm.State =="6"  && itm.CurrentApproverMUDID == null)
            ////{
            ////    rtnData.BUHeadName = "系统自动审批";
            ////    rtnData.BUHeadMUDID = "系统自动审批";
            ////}
            //else if (itm.BudgetTotal >= 1200 && itm.BudgetTotal<1500)
            //{
            //    rtnData.BUHeadName = itm.CurrentApproverName;
            //    rtnData.BUHeadMUDID = itm.CurrentApproverMUDID;
            //}
            //else if (itm.BudgetTotal > 1500 && itm.State=="5")
            //{
            //    rtnData.BUHeadName = itm.CurrentApproverName;
            //    rtnData.BUHeadMUDID = itm.CurrentApproverMUDID;
            //}
            //else
            //{
            //    rtnData.BUHeadMUDID = itm.BUHeadMUDID;
            //    rtnData.BUHeadName = itm.BUHeadName;
            //}
            rtnData.BUHeadMUDID = itm.CurrentApproverMUDID;
            rtnData.BUHeadName = itm.CurrentApproverName;
            rtnData.City = itm.City;
            rtnData.CostCenter = itm.CostCenter;
            rtnData.CreateDate = itm.CreateDate != null ? itm.CreateDate.Value.ToString("yyyy-MM-dd") : string.Empty;
            rtnData.CreateTime = itm.CreateDate != null ? itm.CreateDate.Value.ToString("HH:mm:ss") : string.Empty;
            rtnData.HospitalAddress = itm.HospitalAddress;
            rtnData.HospitalCode = itm.HospitalCode;
            rtnData.HospitalName = itm.HospitalName;
            rtnData.HTCode = itm.HTCode;
            rtnData.ID = itm.ID;
            rtnData.IsDMFollow = itm.IsDMFollow;
            rtnData.IsReAssign = itm.IsReAssign;
            rtnData.IsBudgetChange = itm.IsBudgetChange;
            rtnData.IsMMCoEChange = itm.IsMMCoEChange;
            rtnData.IsUsed = itm.IsUsed;
            rtnData.IsFinished = itm.IsFinished;
            rtnData.IsFinished = itm.IsFinished;
            rtnData.IsFreeSpeaker = itm.IsFreeSpeaker;
            rtnData.Market = itm.Market;
            rtnData.VeevaMeetingID = itm.VeevaMeetingID;
            rtnData.MeetingDate = itm.MeetingDate != null ? itm.MeetingDate.Value.ToString("yyyy-MM-dd") : string.Empty;
            rtnData.MeetingTime = itm.MeetingDate != null ? itm.MeetingDate.Value.ToString("HH:mm:ss") : string.Empty;
            rtnData.MeetingName = itm.MeetingName;
            if (itm.ModifyDate == itm.CreateDate)
            {
                rtnData.ModifyDate = string.Empty;
                rtnData.ModifyTime = string.Empty;
            }
            else
            {
                rtnData.ModifyDate = itm.ModifyDate != null ? itm.ModifyDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                rtnData.ModifyTime = itm.ModifyDate != null ? itm.ModifyDate.Value.ToString("HH:mm:ss") : string.Empty;
            }
            rtnData.MMCoEImage = itm.MMCoEImage;
            rtnData.MMCoEApproveState = itm.MMCoEApproveState;
            rtnData.Province = itm.Province;
            rtnData.ReAssignBUHeadApproveDate = itm.ReAssignBUHeadApproveDate != null ? itm.ReAssignBUHeadApproveDate.Value.ToString("yyyy-MM-dd") : string.Empty;
            rtnData.ReAssignBUHeadApproveTime = itm.ReAssignBUHeadApproveDate != null ? itm.ReAssignBUHeadApproveDate.Value.ToString("HH:mm:ss") : string.Empty;
            rtnData.ReAssignBUHeadMUDID = itm.ReAssignBUHeadMUDID;
            rtnData.ReAssignBUHeadName = itm.ReAssignBUHeadName;
            rtnData.TA = itm.TA;
            rtnData.HTType = itm.HTType;
            if (itm.State == "0" || itm.State == "1" || itm.State == "3" || itm.State == "7")
            {
                rtnData.State = "待审批";
            }
            if (itm.State == "5" || itm.State == "6" || itm.State == "9")
            {
                rtnData.State = "审批通过";
            }
            if (itm.State == "2" || itm.State == "4" || itm.State == "8")
            {
                rtnData.State = "审批被驳回";
            }
            if (itm.State == "10")
            {
                rtnData.State = "预申请已取消";
            }
            rtnData.SpeakerServiceImage = itm.SpeakerServiceImage;
            rtnData.SpeakerBenefitImage = itm.SpeakerBenefitImage;
            rtnData.ReAssignOperatorMUDID = itm.ReAssignOperatorMUDID;
            rtnData.ReAssignOperatorName = itm.ReAssignOperatorName;
            //rtnData.RDSDMUDID = itm.RDSDMUDID;
            //rtnData.RDSDName = itm.RDSDName;
            rtnData.Position = itm.Position;
            rtnData.RDTerritoryCode = itm.RDTerritoryCode;
            rtnData.MRTerritoryCode = itm.MRTerritoryCode;
            string imageOne = string.Empty;
            string imageTwo = string.Empty;
            if (!string.IsNullOrEmpty(itm.SpeakerServiceImage))
            {
                foreach (var imgItem in itm.SpeakerServiceImage.Split(','))
                {
                    var imageOneSrc = ConfigurationManager.AppSettings["AWSService"].ToString() + imgItem;
                    imageOne += "<img src=" + imageOneSrc + " style='height:30px;' onclick=window.open('" + imageOneSrc + "') /> &nbsp;&nbsp;";   //签到表文件
                }
                itm.SpeakerServiceImage = imageOne;
                rtnData.SpeakerServiceImage = itm.SpeakerServiceImage;
            }
            else
            {
                rtnData.SpeakerServiceImage = string.Empty;
            }
            if (!string.IsNullOrEmpty(itm.SpeakerBenefitImage))
            {
                foreach (var imgItem in itm.SpeakerBenefitImage.Split(','))
                {
                    var imageTwoSrc = ConfigurationManager.AppSettings["AWSService"].ToString() + imgItem;
                    imageTwo += "<img src=" + imageTwoSrc + " style='height:30px;' onclick=window.open('" + imageTwoSrc + "') /> &nbsp;&nbsp;";   //签到表文件
                }
                itm.SpeakerBenefitImage = imageTwo;
                rtnData.SpeakerBenefitImage = itm.SpeakerBenefitImage;
            }
            else
            {
                rtnData.SpeakerBenefitImage = string.Empty;
            }
            return rtnData;
        }
        #endregion

        #region 预申请MMCoE审批记录日期处理
        private P_PreApprovalApproveHistory_Load GetRecordsObj(P_PreApprovalApproveHistory_View itm)
        {
            P_PreApprovalApproveHistory_Load rtnData = new Entity.P_PreApprovalApproveHistory_Load();
            if (itm.ActionType == 1)
            {
                rtnData.ActionType = "预申请提交成功";
            }
            if (itm.ActionType == 2)
            {
                rtnData.ActionType = "预申请审批驳回";
            }
            if (itm.ActionType == 3)
            {
                rtnData.ActionType = "预申请审批通过";
            }
            if (itm.ActionType == 4)
            {
                rtnData.ActionType = "预申请修改成功";
            }
            if (itm.ActionType == 5)
            {
                rtnData.ActionType = "预申请取消成功";
            }
            rtnData.ApplierMobile = itm.ApplierMobile;
            rtnData.ApplierMUDID = itm.ApplierMUDID;
            rtnData.ApplierName = itm.ApplierName;
            rtnData.ApproveDate = itm.ApproveDate != null ? itm.ApproveDate.Value.ToString("yyyy-MM-dd") : string.Empty;
            rtnData.ApproveTime = itm.ApproveDate != null ? itm.ApproveDate.Value.ToString("HH:mm:ss") : string.Empty;
            rtnData.AttendCount = itm.AttendCount;
            rtnData.BudgetTotal = itm.BudgetTotal;
            rtnData.BUHeadApproveDate = itm.BUHeadApproveDate != null ? itm.BUHeadApproveDate.Value.ToString("yyyy-MM-dd") : string.Empty;
            rtnData.BUHeadApproveTime = itm.BUHeadApproveDate != null ? itm.BUHeadApproveDate.Value.ToString("HH:mm:ss") : string.Empty;
            rtnData.BUHeadMUDID = itm.BUHeadMUDID;
            rtnData.BUHeadName = itm.BUHeadName;
            rtnData.City = itm.City;
            rtnData.Comments = itm.Comments;
            rtnData.CostCenter = itm.CostCenter;
            rtnData.CreateDate = itm.CreateDate != null ? itm.CreateDate.Value.ToString("yyyy-MM-dd") : string.Empty;
            rtnData.CreateTime = itm.CreateDate != null ? itm.CreateDate.Value.ToString("HH:mm:ss") : string.Empty;
            rtnData.HospitalAddress = itm.HospitalAddress;
            rtnData.HospitalCode = itm.HospitalCode;
            rtnData.HospitalName = itm.HospitalName;
            rtnData.HTCode = itm.HTCode;
            rtnData.ID = itm.ID;
            rtnData.IsDMFollow = itm.IsDMFollow;
            rtnData.IsReAssign = itm.IsReAssign;
            rtnData.IsFreeSpeaker = itm.IsFreeSpeaker;
            rtnData.Market = itm.Market;
            rtnData.MeetingDate = itm.MeetingDate != null ? itm.MeetingDate.Value.ToString("yyyy-MM-dd") : string.Empty;
            rtnData.MeetingTime = itm.MeetingDate != null ? itm.MeetingDate.Value.ToString("HH:mm:ss") : string.Empty;
            rtnData.MeetingName = itm.MeetingName;
            rtnData.ModifyDate = itm.ModifyDate != null ? itm.ModifyDate.Value.ToString("yyyy-MM-dd") : string.Empty;
            rtnData.ModifyTime = itm.ModifyDate != null ? itm.ModifyDate.Value.ToString("HH:mm:ss") : string.Empty;
            rtnData.PID = itm.PID;
            rtnData.Province = itm.Province;
            rtnData.ReAssignBUHeadApproveDate = itm.ReAssignBUHeadApproveDate != null ? itm.ReAssignBUHeadApproveDate.Value.ToString("yyyy-MM-dd") : string.Empty;
            rtnData.ReAssignBUHeadApproveTime = itm.ReAssignBUHeadApproveDate != null ? itm.ReAssignBUHeadApproveDate.Value.ToString("HH:mm:ss") : string.Empty;
            rtnData.ReAssignBUHeadMUDID = itm.ReAssignBUHeadMUDID;
            rtnData.ReAssignBUHeadName = itm.ReAssignBUHeadName;
            if (itm.State == "0" || itm.State == "1" || itm.State == "3")
            {
                rtnData.State = "待审批";
            }
            if (itm.State == "5" || itm.State == "6")
            {
                rtnData.State = "审批通过";
            }
            if (itm.State == "2" || itm.State == "4")
            {
                rtnData.State = "审批被驳回";
            }
            rtnData.TA = itm.TA;
            rtnData.UserId = itm.UserId;
            rtnData.UserName = itm.UserName;
            return rtnData;
        }
        #endregion

        #region 预申请查询
        [OperationAuditFilter(Operation = "预申请查询", OperationAuditTypeName = "预申请查询")]
        public JsonResult Load(string srh_HTCode, string srh_HospitalCode, string srh_CostCenter, string srh_ApplierMUDID, string srh_ApplierTerritory, string srh_BUHeadMUDID, string srh_Market, string srh_TA, string srh_State, string srh_StartBUHeadApproveDate, string srh_EndBUHeadApproveDate, string srh_StartMeetingDate, string srh_EndMeetingDate, string srh_RD, int srh_BudgetTotal, int rows, int page)
        {
            int total;
            string oldHospitalCode = PreApprovalService.GetOldGskHospitalCodeByGskHospitalCode(srh_HospitalCode);
            if (!string.IsNullOrEmpty(oldHospitalCode))
            {
                srh_HospitalCode += "," + oldHospitalCode;
            }
            string oldCostCenter = PreApprovalService.GetOldCostcenterByCostcenter(srh_CostCenter);
            if (!string.IsNullOrEmpty(oldCostCenter))
            {
                srh_CostCenter += "," + oldCostCenter;
            }
            var list = PreApprovalService.Load(srh_HTCode, srh_HospitalCode, srh_CostCenter, srh_ApplierMUDID, srh_ApplierTerritory, srh_BUHeadMUDID, srh_Market, srh_TA, srh_State, srh_StartBUHeadApproveDate, srh_EndBUHeadApproveDate, srh_StartMeetingDate, srh_EndMeetingDate, srh_RD, srh_BudgetTotal, rows, page, out total);
            var preApproval = new List<P_PreApproval_View>();
            foreach (var i in list)
            {
                preApproval.Add(GetDisplayObj(i));
            }
            return Json(new { state = 1, rows = preApproval, total = total });
        }

        #endregion

        #region 导出讲者服务协议、利益冲突声明
        /// <summary>
        /// 导出讲者服务协议、利益冲突声明
        /// </summary>
        /// <param name="srh_HTCode"></param>
        /// <param name="srh_ApplierMUDID"></param>
        /// <param name="srh_ApplierTerritory"></param>
        /// <param name="srh_BUHeadMUDID"></param>
        /// <param name="srh_Market"></param>
        /// <param name="srh_TA"></param>
        /// <param name="srh_State"></param>
        /// <param name="srh_StartBUHeadApproveDate"></param>
        /// <param name="srh_EndBUHeadApproveDate"></param>
        /// <param name="srh_StartMeetingDate"></param>
        /// <param name="srh_EndMeetingDate"></param>
        /// <returns></returns>
        [OperationAuditFilter(Operation = "导出讲者服务协议、利益冲突声明", OperationAuditTypeName = "导出讲者服务协议、利益冲突声明")]
        public JsonResult ExportServiceimage(string srh_HTCode, string srh_HospitalCode, string srh_CostCenter, string srh_ApplierMUDID, string srh_ApplierTerritory, string srh_BUHeadMUDID, string srh_Market, string srh_TA, string srh_State, string srh_StartBUHeadApproveDate, string srh_EndBUHeadApproveDate, string srh_StartMeetingDate, string srh_EndMeetingDate, string srh_RD, int srh_BudgetTotal)
        {
            var total = 0;
            string oldHospitalCode = PreApprovalService.GetOldGskHospitalCodeByGskHospitalCode(srh_HospitalCode);
            if (!string.IsNullOrEmpty(oldHospitalCode))
            {
                srh_HospitalCode += "," + oldHospitalCode;
            }
            //查询列表
            var list = PreApprovalService.LoadFreeSpeakerFile(srh_HTCode, srh_HospitalCode, srh_CostCenter, srh_ApplierMUDID, srh_ApplierTerritory, srh_BUHeadMUDID, srh_Market, srh_TA, srh_State, srh_StartBUHeadApproveDate, srh_EndBUHeadApproveDate, srh_StartMeetingDate, srh_EndMeetingDate, srh_RD, srh_BudgetTotal, int.MaxValue, 1, out total);
            AdminUser adminUser = Session[ConstantHelper.CurrentAdminUser] as AdminUser;
            PreApprovalDownloadImage preApprovalDownloadImage = new PreApprovalDownloadImage(list, Server.MapPath("~/Temp"), adminUser.Email, PreApprovalService);
            Thread thread = new Thread(preApprovalDownloadImage.InitImage);
            thread.Start();
            return Json(new
            {
                state = 1,
                message = "系统正在生成文件，稍后将以邮件方式发送下载链接，请注意查收邮件。"
            });
        }

        #endregion



        #region 预申请最高审批人列表
        [OperationAuditFilter(Operation = "预申请最高审批人", OperationAuditTypeName = "预申请最高审批人")]
        public JsonResult CostCenterLoad(int rows, int page)
        {
            //更新TERRITORY_TA至成本中心表
            var newTERRITORY_TA = PreApprovalService.GetNewTERRITORY_TA();
            if (newTERRITORY_TA != null && newTERRITORY_TA.Count > 0)
            {
                var sus = new List<D_COSTCENTER>();
                var CreateDate = DateTime.Now;
                foreach (var s in newTERRITORY_TA)
                {
                    Guid ID = Guid.NewGuid();
                    sus.Add(new D_COSTCENTER()
                    {
                        ID = ID,
                        Market = "",
                        TA = "",
                        BUHeadName = "",
                        Region = "",
                        CreateDate = CreateDate,
                        CreatedBy = "管理员",
                        BUHeadMUDID = "",
                        TERRITORY_TA = s.TERRITORY_TA.Trim()

                    });
                }
                PreApprovalService.InsertnewTERRITORY_TA(sus);
            }
            int total;
            var list = PreApprovalService.CostCenterLoad(rows, page, out total);
            return Json(new { state = 1, rows = list, total = total });
        }
        #endregion

        #region 新增成本中心
        [OperationAuditFilter(Operation = "新增成本中心", OperationAuditTypeName = "新增成本中心")]
        public JsonResult AddCostCenter(string sltMarket, string sltTA, string txtBUHeadName, string txtBUHeadMUDID, string txtRegion, string txtRegionManagerName, string txtRegionManagerMUDID, string RDSDManagerName, string RDSDManagerMUDID, string CostCenter, string OldCostCenter)
        {
            var user = CurrentAdminUser;
            string CreateBy = CurrentAdminUser.Name;
            DateTime CreateDateTIme = DateTime.Now;
            int State = PreApprovalService.AddCostCenter(sltMarket, sltTA, txtBUHeadName, txtBUHeadMUDID, txtRegion, txtRegionManagerName, txtRegionManagerMUDID, RDSDManagerName, RDSDManagerMUDID, CostCenter, OldCostCenter, CreateBy, CreateDateTIme);
            var txt = "添加成本中心成功！";
            if (State == 0)
            {
                txt = "成本中心已存在，请重试！";
            }
            return Json(new { state = State, data = 1, txt });
        }
        #endregion

        //#region 删除成本中心
        //[OperationAuditFilter(Operation = "删除成本中心", OperationAuditTypeName = "删除成本中心")]
        //public JsonResult DelById(Guid id, string CostCenter)
        //{
        //    var _Exist = PreApprovalService._Exist(id);
        //    if (_Exist > 0)
        //    {
        //        return Json(new { state = 0, txt = "该成本中心存在未审批订单，请审批后重试" });
        //    }
        //    var res = PreApprovalService.Del(id);
        //    if (res > 0)
        //    {
        //        string cont = "删除成本中心CostCenter:" + CostCenter;
        //        var num = operationAuditService.AddAudit("0", cont);
        //        return Json(new { state = 1 });
        //    }
        //    return Json(new { state = 0, txt = "操作失败" });
        //}
        //#endregion

        #region 删除当前TERRITORY_TA 
        [OperationAuditFilter(Operation = "删除当前TERRITORY_TA", OperationAuditTypeName = "删除当前TERRITORY_TA")]
        public JsonResult DelById(Guid id)
        {
            var TERRITORY_TA = PreApprovalService.GetTERRITORY_TAByID(id);
            string txtTERRITORY_TA = TERRITORY_TA[0].TERRITORY_TA;
            var getstate = PreApprovalService.GetStateByTA(txtTERRITORY_TA);
            if (getstate > 0)
            {
                return Json(new { state = 0, txt = "该TERRITORY_TA存在未审批订单，请审批后重试" });
            }
            var res = PreApprovalService.Del(id);
            if (res > 0)
            {
                string cont = "删除当前TERRITORY_TA:" + txtTERRITORY_TA;
                var num = operationAuditService.AddAudit("0", cont);
                return Json(new { state = 1 });
            }
            return Json(new { state = 0, txt = "操作失败" });
        }
        #endregion

        #region 预申请MMCoE审批记录
        public JsonResult RecordsLoad(string srh_HTCode, string srh_ApplierMUDID, string srh_BUHeadMUDID, string srh_startMeetingDate, string srh_endMeetingDate, string srh_State, string srh_StartApproveDate, string srh_EndApproveDate, int rows, int page)
        {
            int total;
            var list = PreApprovalService.RecordsLoad(srh_HTCode, srh_ApplierMUDID, srh_BUHeadMUDID, srh_startMeetingDate, srh_endMeetingDate, srh_State, srh_StartApproveDate, srh_EndApproveDate, rows, page, out total);
            var records = new List<P_PreApprovalApproveHistory_Load>();
            foreach (var i in list)
            {
                records.Add(GetRecordsObj(i));
            }
            return Json(new { state = 1, rows = records, total = total });
        }
        #endregion



        #region 修改预申请最高审批人（点击确定按钮）
        [OperationAuditFilter(Operation = "修改预申请最高审批人", OperationAuditTypeName = "修改预申请最高审批人")]
        public JsonResult Save(string ID, string txtTERRITORY_TA, string txtBUHeadName, string txtBUHeadMUDID)
        {
            AdminUser adminUser = Session[ConstantHelper.CurrentAdminUser] as AdminUser;
            var name = adminUser.Name;
            var res = 0;
            if (txtBUHeadMUDID == "")
            {
                txtBUHeadName = "";
                res = PreApprovalService.SaveChange(ID, txtTERRITORY_TA, txtBUHeadName, txtBUHeadMUDID, name);
            }
            else
            {
                var getname = IUserInfoService.Find(txtBUHeadMUDID);
                if (getname != null)
                {
                    txtBUHeadName = getname.Name;
                    res = PreApprovalService.SaveChange(ID, txtTERRITORY_TA, txtBUHeadName, txtBUHeadMUDID, name);
                }
                else
                {
                    return Json(new { state = 0, txt = "不存在此MUDID" });
                }
            }
            
            //判断是否同步更新待审批预申请BUHead
            var P_PreApproval = PreApprovalService.GetDataByTAAndState(txtTERRITORY_TA);
            if (P_PreApproval != null && P_PreApproval.Count > 0)
            {
                foreach (var s in P_PreApproval)
                {
                    Guid id = s.ID;
                    string CurrentApproverMUDID = s.CurrentApproverMUDID.Trim();
                    string BUHeadMUDID = s.BUHeadMUDID.Trim();
                    if (BUHeadMUDID.ToUpper() != CurrentApproverMUDID.ToUpper())
                    {
                        //同步更新预申请表BUHead
                        PreApprovalService.UpdatePreApprovalBUHead(id, txtBUHeadMUDID, txtBUHeadName);
                        LogHelper.Info("修改预申请最高审批人：HT编号为" + s.HTCode);
                    }
                }
            }
            switch (res)
            {
                case 1:
                    //var cont = "预申请最高审批人:" + txtBUHeadMUDID;
                    var cont = "修改TERRITORY_TA信息:" + txtTERRITORY_TA;
                    var num = operationAuditService.AddAudit("0", cont);
                    return Json(new { state = 1 });
            }
            return Json(new { state = 0, txt = "操作失败" });
        }
        #endregion

        #region 导出预申请查询
        [OperationAuditFilter(Operation = "导出预申请查询", OperationAuditTypeName = "导出预申请查询")]
        public void ExportPreApprovalList(string srh_HTCode, string srh_HospitalCode, string srh_CostCenter, string srh_ApplierMUDID, string srh_ApplierTerritory, string srh_BUHeadMUDID, string srh_Market, string srh_TA, string srh_State, string srh_StartBUHeadApproveDate, string srh_EndBUHeadApproveDate, string srh_StartMeetingDate, string srh_EndMeetingDate, string srh_RD, int srh_BudgetTotal)
        {
            #region 抓取数据            
            string oldHospitalCode = PreApprovalService.GetOldGskHospitalCodeByGskHospitalCode(srh_HospitalCode);
            if (!string.IsNullOrEmpty(oldHospitalCode))
            {
                srh_HospitalCode += "," + oldHospitalCode;
            }
            string oldCostCenter = PreApprovalService.GetOldCostcenterByCostcenter(srh_CostCenter);
            if (!string.IsNullOrEmpty(oldCostCenter))
            {
                srh_CostCenter += "," + oldCostCenter;
            }
            var list = PreApprovalService.ExportPreApproval(srh_HTCode, srh_HospitalCode, srh_CostCenter, srh_ApplierMUDID, srh_ApplierTerritory, srh_BUHeadMUDID, srh_Market, srh_TA, srh_State, srh_StartBUHeadApproveDate, srh_EndBUHeadApproveDate, srh_StartMeetingDate, srh_EndMeetingDate, srh_RD, srh_BudgetTotal);
            #endregion

            #region 构建Excel
            FileStream file11 = new FileStream(Server.MapPath("~/Template/Template_Pre-ApprovalReport.xls"), FileMode.Open, FileAccess.Read);
            HSSFWorkbook wk = new HSSFWorkbook(file11);

            ISheet sheet = wk.GetSheet("Report");

            #endregion

            #region 制作表体
            for (var i = 1; i <= list.Count; i++)
            {
                var item = list[i - 1];
                IRow row = sheet.CreateRow(i);
                ICell cell = null;
                var j = 0;
                if (item != null)
                {
                    cell = row.CreateCell(j);
                    cell.SetCellValue(item.ApplierName);// "申请人姓名");
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.ApplierMUDID);//"申请人MUDID");
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.MRTerritoryCode);//"申请人Territory Code");
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.Position);// "申请人职位");
                    cell = row.CreateCell(++j);
                    //cell.SetCellValue(item.ApplierMobile);// "申请人手机号码");
                    cell.SetCellValue("");
                    cell = row.CreateCell(++j);
                    if (item.CreateDate != null)
                    {
                        cell.SetCellValue(item.CreateDate.Value);// "预申请申请日期");
                    }
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.CreateDate == null ? "" : item.CreateDate.Value.ToString("HH:mm:ss"));// "预申请申请时间");
                    if (item.CreateDate == item.ModifyDate)
                    {
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(string.Empty);// "预申请修改日期");
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(string.Empty);// "预申请修改时间");
                    }
                    else
                    {
                        cell = row.CreateCell(++j);
                        if (item.ModifyDate != item.CreateDate)
                        {
                            cell.SetCellValue(item.ModifyDate.Value);// "预申请修改日期");
                        }
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.ModifyDate == null ? "" : item.ModifyDate.Value.ToString("HH:mm:ss"));// "预申请修改时间");
                    }
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.HTCode);// "HT编号");
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.Market);// "Market");
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.VeevaMeetingID == null ? "" : item.VeevaMeetingID.ToString());//VeevaMeetingID
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.TA);// "TA");
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.Province);// "省份");
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.City);// "城市");
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.HospitalCode);// "医院编码");
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.HospitalName);// "医院名称");
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.HospitalAddress);// "医院地址");
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.MeetingName);// "会议名称");
                    cell = row.CreateCell(++j);
                    if (item.MeetingDate != null)
                    {
                        cell.SetCellValue(item.MeetingDate.Value);// "会议日期");
                    }
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.MeetingDate == null ? "" : item.MeetingDate.Value.ToString("HH:mm:ss"));// "会议时间");
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.AttendCount);// "参会人数");
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.CostCenter);// "大区区域代码");
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.HTType == "1" ? "线上HT" : item.HTType == "0" ? "线下HT" : "");// "HT形式");
                    cell = row.CreateCell(++j);
                    double budgetTotal;
                    double.TryParse(item.BudgetTotal.ToString(), out budgetTotal);
                    cell.SetCellValue(budgetTotal);// "预算金额");
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.IsDMFollow == true ? "是" : "否");// "直线经理是否随访");
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.IsFreeSpeaker == true ? "是" : "否");//是否由外部免费讲者来讲
                    //cell = row.CreateCell(++j);
                    //cell.SetCellValue(item.RDSDName);
                    //cell = row.CreateCell(++j);
                    //cell.SetCellValue(item.RDSDMUDID);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.RDTerritoryCode);// "RD Territory Code");
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.CurrentApproverName);// "预申请审批人姓名");
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.CurrentApproverMUDID);// "预申请审批人MUDID");
                    //if (item.AttendCount < 60 && item.BudgetTotal < 1200)
                    //{
                    //    cell = row.CreateCell(++j);
                    //    cell.SetCellValue("系统自动审批");// "预申请审批人姓名");
                    //    cell = row.CreateCell(++j);
                    //    cell.SetCellValue("系统自动审批");// "预申请审批人MUDID");
                    //}
                    ////else if (item.State == "6" && item.CurrentApproverMUDID == null)
                    ////{
                    ////    cell = row.CreateCell(++j);
                    ////    cell.SetCellValue("系统自动审批");// "预申请审批人姓名");
                    ////    cell = row.CreateCell(++j);
                    ////    cell.SetCellValue("系统自动审批");// "预申请审批人MUDID");
                    ////}
                    //else if (item.BudgetTotal >= 1200 && item.BudgetTotal < 1500)
                    //{
                    //    cell = row.CreateCell(++j);
                    //    cell.SetCellValue(item.CurrentApproverName);// "预申请审批人姓名");
                    //    cell = row.CreateCell(++j);
                    //    cell.SetCellValue(item.CurrentApproverMUDID);// "预申请审批人MUDID");
                    //}
                    //else if (item.BudgetTotal > 1500 && item.State == "5")
                    //{
                    //    cell = row.CreateCell(++j);
                    //    cell.SetCellValue(item.CurrentApproverName);// "预申请审批人姓名");
                    //    cell = row.CreateCell(++j);
                    //    cell.SetCellValue(item.CurrentApproverMUDID);// "预申请审批人MUDID");
                    //}
                    //else
                    //{
                    //    cell = row.CreateCell(++j);
                    //    cell.SetCellValue(item.BUHeadName);// "预申请审批人姓名");
                    //    cell = row.CreateCell(++j);
                    //    cell.SetCellValue(item.BUHeadMUDID);// "预申请审批人MUDID");

                    //}

                    cell = row.CreateCell(++j);
                    if (item.BUHeadApproveDate != null)
                    {
                        cell.SetCellValue(item.BUHeadApproveDate.Value);// "预申请审批日期");
                    }

                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.BUHeadApproveDate == null ? "" : item.BUHeadApproveDate.Value.ToString("HH:mm:ss"));// "预申请审批时间");
                    cell = row.CreateCell(++j);
                    var numState = int.Parse(item.State);
                    string strState = "";
                    switch (numState)
                    {
                        case 0:
                            strState = "提交成功";
                            break;
                        case 1:
                            strState = "提交成功";
                            break;
                        case 2:
                            strState = "审批被驳回";
                            break;
                        case 3:
                            strState = "提交成功";
                            break;
                        case 4:
                            strState = "审批被驳回";
                            break;
                        case 5:
                            strState = "审批通过";
                            break;
                        case 6:
                            strState = "审批通过";
                            break;
                        case 7:
                            strState = "提交成功";
                            break;
                        case 8:
                            strState = "审批被驳回";
                            break;
                        case 9:
                            strState = "审批通过";
                            break;
                        case 10:
                            strState = "已取消";
                            break;
                        default:
                            break;
                    }
                    cell.SetCellValue(strState);// "预申请审批状态");
                    //cell = row.CreateCell(++j);
                    //cell.SetCellValue(item.IsReAssign == true ? "是" : "否");// "预申请是否重新分配审批人");
                    //cell = row.CreateCell(++j);
                    //cell.SetCellValue(item.ReAssignOperatorName);// "预申请重新分配审批人-操作人");
                    //cell = row.CreateCell(++j);
                    //cell.SetCellValue(item.ReAssignOperatorMUDID);// "预申请重新分配审批人-操作人MUDID");
                    //cell = row.CreateCell(++j);
                    //cell.SetCellValue(item.ReAssignBUHeadName);// "预申请被重新分配审批人姓名");
                    //cell = row.CreateCell(++j);
                    //cell.SetCellValue(item.ReAssignBUHeadMUDID);// "预申请被重新分配审批人MUDID");
                    //cell = row.CreateCell(++j);
                    //if (item.ReAssignBUHeadApproveDate != null)
                    //{
                    //    cell.SetCellValue(item.ReAssignBUHeadApproveDate.Value);// "预申请重新分配审批人日期");
                    //}
                    //cell = row.CreateCell(++j);
                    //cell.SetCellValue(item.ReAssignBUHeadApproveDate == null ? "" : item.ReAssignBUHeadApproveDate.Value.ToString("HH:mm:ss"));// "预申请重新分配审批人时间");
                }

            }
            #endregion

            #region 写入到客户端
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                wk.Write(ms);
                Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xls", DateTime.Now.ToString("yyyyMMddHHmmssfff")));
                Response.ContentType = "application/vnd.ms-excel";
                Response.BinaryWrite(ms.ToArray());
            }
            #endregion
        }
        #endregion        

        #region 导出预申请最高审批人
        public ActionResult ExportCostCenterList()
        {
            var list = PreApprovalService.ExportCostCenterList();
            if (list != null && list.Count > 0)
            {
                XSSFWorkbook book = new XSSFWorkbook();
                #region var headerStyle = book.CreateCellStyle();
                var headerStyle = book.CreateCellStyle();

                var headerFontStyle = book.CreateFont();
                headerFontStyle.Color = NPOI.HSSF.Util.HSSFColor.White.Index;
                headerFontStyle.Boldweight = short.MaxValue;
                headerFontStyle.FontHeightInPoints = 10;

                headerStyle.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;
                headerStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightOrange.Index;
                headerStyle.Alignment = HorizontalAlignment.Center;
                headerStyle.SetFont(headerFontStyle);
                #endregion
                var sheet = book.CreateSheet("report");
                var row = sheet.CreateRow(0);
                #region header
                sheet.SetColumnWidth(0, 20 * 256);
                sheet.SetColumnWidth(1, 25 * 256);
                sheet.SetColumnWidth(2, 25 * 256);

                var cell = row.CreateCell(0);
                cell.SetCellValue("TERRITORY_TA");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(1);
                cell.SetCellValue("BU Head(Rx/Vx)或销售总监(DDT/TSKF)姓名");
                cell.CellStyle = headerStyle;
                cell = row.CreateCell(2);
                cell.SetCellValue("BU Head(Rx/Vx)或销售总监(DDT/TSKF)MUDID");
                cell.CellStyle = headerStyle;

                #endregion
                #region var dataCellStyle = book.CreateCellStyle();
                var dataCellStyle = book.CreateCellStyle();
                var dataFontStyle = book.CreateFont();
                dataFontStyle.Color = NPOI.HSSF.Util.HSSFColor.Black.Index;
                dataFontStyle.Boldweight = short.MaxValue;
                dataFontStyle.FontHeightInPoints = 10;

                dataCellStyle.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;
                dataCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.White.Index;
                //dataCellStyle.Alignment = HorizontalAlignment.Center;
                dataCellStyle.SetFont(dataFontStyle);
                #endregion
                D_COSTCENTER disItm;
                int dataCnt = list.Count;
                for (int i = 0; i < dataCnt; i++)
                {
                    disItm = list[i];
                    row = sheet.CreateRow(1 + i);
                    #region data cell
                    cell = row.CreateCell(0);
                    cell.SetCellValue(disItm.TERRITORY_TA.Trim());// "TERRITORY_TA");
                    cell = row.CreateCell(1);
                    cell.SetCellValue(disItm.BUHeadName);//"BU Head(Rx/Vx)或销售总监(DDT/TSKF)姓名");
                    cell = row.CreateCell(2);
                    cell.SetCellValue(disItm.BUHeadMUDID);// "BU Head(Rx/Vx)或销售总监(DDT/TSKF)MUDID");

                    #endregion
                }

                byte[] excelData;
                using (System.IO.MemoryStream _ms = new System.IO.MemoryStream())
                {
                    book.Write(_ms);
                    excelData = _ms.ToArray();
                    //_ms.Close();
                }

                ViewBag.Msg = "导出成功！";
                return File(excelData, "application/vnd.ms-excel", HttpUtility.UrlEncode("TERRITORY_TA_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx", System.Text.Encoding.UTF8));
            }
            else
            {
                ViewBag.Msg = "无数据！";
                return View();
            }
        }
        #endregion

        #region 导出MMCoE审批文件图片
        /// <summary>
        /// 导出MMCoE审批文件图片
        /// </summary>
        /// <param name="srh_HTCode"></param>
        /// <param name="srh_ApplierMUDID"></param>
        /// <param name="srh_BUHeadMUDID"></param>
        /// <param name="srh_startMeetingDate"></param>
        /// <param name="srh_endMeetingDate"></param>
        /// <param name="srh_State"></param>
        /// <param name="srh_StartApproveDate"></param>
        /// <param name="srh_EndApproveDate"></param>
        /// <returns></returns>
        [OperationAuditFilter(Operation = "导出MMCoE审批文件图片", OperationAuditTypeName = "导出MMCoE审批文件图片")]
        public JsonResult ExportMMCoEImage(string srh_HTCode, string srh_ApplierMUDID, string srh_BUHeadMUDID, string srh_startMeetingDate, string srh_endMeetingDate, string srh_State, string srh_StartApproveDate, string srh_EndApproveDate)
        {
            using (ZipFile zip = new ZipFile(System.Text.Encoding.Default))
            {
                var total = 0;
                //查询列表
                var list = PreApprovalService.RecordsLoad(srh_HTCode, srh_ApplierMUDID, srh_BUHeadMUDID, srh_startMeetingDate, srh_endMeetingDate, srh_State, srh_StartApproveDate, srh_EndApproveDate, int.MaxValue, 1, out total);
                try
                {
                    //用于临时存储文件
                    var tempFilePath1 = Server.MapPath("~/Temp");
                    var tempFilePath2 = Server.MapPath("~/TempZip");
                    if (!Directory.Exists(tempFilePath1))//判断文件夹是否存在 
                    {
                        Directory.CreateDirectory(tempFilePath1);//不存在则创建文件夹 
                    }
                    if (!Directory.Exists(tempFilePath2))//判断文件夹是否存在 
                    {
                        Directory.CreateDirectory(tempFilePath2);//不存在则创建文件夹 
                    }
                    DirectoryInfo dir = new DirectoryInfo(tempFilePath1);
                    FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                    //遍历删除文件   防止临时文件过多。
                    foreach (FileSystemInfo i in fileinfo)
                    {
                        if (i is DirectoryInfo)            //判断是否文件夹
                        {
                            DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                            subdir.Delete(true);          //删除子目录和文件
                        }
                        else
                        {
                            System.IO.File.Delete(i.FullName);      //删除指定文件
                        }
                    }
                }
                catch (Exception e)
                {
                    LogHelper.Error("导出MMCoE审批文件图片-操作文件错误", e);
                }
                //s3 下载文件
                var s3Handler = new S3Handler();
                foreach (var record in list)
                {
                    //判断图片不为空
                    if (!string.IsNullOrEmpty(record.MMCoEImage))
                    {
                        //多个图片是用 , 拼接路径
                        var imgList = record.MMCoEImage.Split(',');
                        for (var i = 0; i < imgList.Length; i++)
                        {
                            var fileName = i + 1;
                            //删掉字符串中的 /
                            var s3key = imgList[i].Substring(1, imgList[i].Length - 1);
                            string extension = Path.GetExtension(imgList[i]);
                            string time = record.ModifyDate.Value.ToString("yyyy-MM");
                            s3Handler.S3Download(s3key, Server.MapPath("~/Temp/") + time + "/" + record.HTCode + "/" + "MMCoE" + fileName + extension);
                        }
                    }
                }
                //添加文件夹下的所有文件到压缩包里
                zip.AddDirectory(Server.MapPath("~/Temp/"));
                var fileId = Guid.NewGuid();
                zip.Save(Server.MapPath("~/TempZip/") + fileId + ".zip");
                //将zip路径返回  用于后面下载使用
                return Json(new
                {
                    state = 1,
                    filePath = Server.MapPath("~/TempZip/") + fileId + ".zip"
                });

            }
        }
        /// <summary>
        /// 结合上面方法使用
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public ActionResult DownloadByPath(string filePath)
        {
            return File(filePath, "application/zip", "ExportImg.zip");
        }
        #endregion

        #region 导出历史MMCoE审批文件图片
        /// <summary>
        /// 导出历史MMCoE审批文件图片
        /// </summary>
        /// <returns></returns>
        [OperationAuditFilter(Operation = "导出历史MMCoE审批文件图片", OperationAuditTypeName = "导出历史MMCoE审批文件图片")]
        public JsonResult ExportHistoryMMCoEImage()
        {
            using (ZipFile zip = new ZipFile(System.Text.Encoding.Default))
            {
                //查询列表
                var list = PreApprovalService.GetMMCoEHisOrder();
                try
                {
                    //用于临时存储文件
                    var tempFilePath1 = Server.MapPath("~/Temp");
                    var tempFilePath2 = Server.MapPath("~/TempZip");
                    if (!Directory.Exists(tempFilePath1))//判断文件夹是否存在 
                    {
                        Directory.CreateDirectory(tempFilePath1);//不存在则创建文件夹 
                    }
                    if (!Directory.Exists(tempFilePath2))//判断文件夹是否存在 
                    {
                        Directory.CreateDirectory(tempFilePath2);//不存在则创建文件夹 
                    }
                    DirectoryInfo dir = new DirectoryInfo(tempFilePath1);
                    FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                    //遍历删除文件   防止临时文件过多。
                    foreach (FileSystemInfo i in fileinfo)
                    {
                        if (i is DirectoryInfo)            //判断是否文件夹
                        {
                            DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                            subdir.Delete(true);          //删除子目录和文件
                        }
                        else
                        {
                            System.IO.File.Delete(i.FullName);      //删除指定文件
                        }
                    }
                }
                catch (Exception e)
                {
                    LogHelper.Error("导出MMCoE审批文件图片-操作文件错误", e);
                }
                //s3 下载文件
                var s3Handler = new S3Handler();
                foreach (var record in list)
                {
                    //判断图片不为空
                    if (!string.IsNullOrEmpty(record.MMCoEImage))
                    {
                        //多个图片是用 , 拼接路径
                        var imgList = record.MMCoEImage.Split(',');
                        for (var i = 0; i < imgList.Length; i++)
                        {
                            var fileName = i + 1;
                            //删掉字符串中的 /
                            var s3key = imgList[i].Substring(1, imgList[i].Length - 1);
                            string extension = Path.GetExtension(imgList[i]);
                            s3Handler.S3Download(s3key, Server.MapPath("~/Temp/") + record.CN + "/" + "MMCoE" + fileName + extension);
                        }
                    }
                }
                //添加文件夹下的所有文件到压缩包里
                zip.AddDirectory(Server.MapPath("~/Temp/"));
                var fileId = Guid.NewGuid();
                zip.Save(Server.MapPath("~/TempZip/") + fileId + ".zip");
                //将zip路径返回  用于后面下载使用
                return Json(new
                {
                    state = 1,
                    filePath = Server.MapPath("~/TempZip/") + fileId + ".zip"
                });

            }
        }
        #endregion

        public JsonResult SyncPreApproval()
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                var rtnVal = baseDataService.SyncBaseData();
            });
            return Json(new { state = 1 });
        }

        #region 导入成本中心
        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>        
        [OperationAuditFilter(Operation = "导入成本中心", OperationAuditTypeName = "导入成本中心")]
        public JsonResult ImportCostCenter(HttpPostedFileBase file)
        {
            //当前登录人信息
            AdminUser adminUser = Session[ConstantHelper.CurrentAdminUser] as AdminUser;
            #region 解析为Excel格式对象
            var workbook = new XSSFWorkbook(file.InputStream);

            var sheet = workbook.GetSheetAt(0);
            if (sheet == null)
            {
                return Json(new { state = 0, txt = "导入的文件格式不正确" });
            }
            #endregion

            #region 判断表头是否符合格式
            var row = sheet.GetRow(0);
            if (row == null)
            {
                return Json(new { state = 0, txt = "导入的文件格式不正确" });
            }

            var titleTemplate = "Market,TA,BU Head(Rx/Vx)或销售总监(DDT/TSKF)姓名,BU Head(Rx/Vx)或销售总监(DDT/TSKF)MUDID,Region,Regional Manager,Regional Manager MUDID,RD/SD Manager,RD/SD Manager MUDID,Regional Manager CostCenter,Old Regional Manager CostCenter".Split(',');
            var titleValues = new string[10];

            for (var i = 0; i < 10; i++)
            {
                titleValues[i] = row.GetCell(0) != null ? row.GetCell(i).StringCellValue : string.Empty;
                if (titleValues[i] != titleTemplate[i])
                {
                    return Json(new { state = 0, txt = "导入的文件格式不正确" });
                }
            }
            #endregion

            #region 读取表体
            var excelRows = new List<D_COSTCENTER>();
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                row = sheet.GetRow(i);
                if (row == null) continue;
                if (string.IsNullOrEmpty(GetStringFromCell(row.GetCell(0)))) break;
                excelRows.Add(new D_COSTCENTER()
                {
                    Market = GetStringFromCell(row.GetCell(0)).Replace("\r\n", string.Empty),
                    TA = GetStringFromCell(row.GetCell(1)).Replace("\r\n", string.Empty),
                    BUHeadName = GetStringFromCell(row.GetCell(2)).Replace("\r\n", string.Empty),
                    BUHeadMUDID = GetStringFromCell(row.GetCell(3)).Replace("\r\n", string.Empty),
                    Region = GetStringFromCell(row.GetCell(4)).Trim().Replace("\r\n", string.Empty),
                    RegionManagerName = GetStringFromCell(row.GetCell(5)).Replace("\r\n", string.Empty),
                    RegionManagerMUDID = GetStringFromCell(row.GetCell(6)).Replace("\r\n", string.Empty),
                    RDSDName = GetStringFromCell(row.GetCell(7)).Replace("\r\n", string.Empty),
                    RDSDMUDID = GetStringFromCell(row.GetCell(8)).Replace("\r\n", string.Empty),
                    CostCenter = GetStringFromCell(row.GetCell(9)).Replace("\r\n", string.Empty),
                    OldCostCenter = GetStringFromCell(row.GetCell(10)).Replace("\r\n", string.Empty),
                });
            }
            #endregion            

            var listRepeat = excelRows.GroupBy(a => new { TA = a.TA, Region = a.Region, CostCenter = a.CostCenter }).Where(a => a.Count() > 1).Select(a => a.Key).ToList();
            if (listRepeat.Count() > 0)
            {
                return Json(new { state = 0, txt = "Excel中发现成本中心重复数据" });
            }

            var fails = new List<D_COSTCENTER>();
            var listCos = new List<D_COSTCENTER>();
            foreach (var item in excelRows)
            {
                // var costCenter = PreApprovalService.FindInfo("", "", item.TA + "-" + item.Region + "(" + item.CostCenter + ")");
                var costCenter = PreApprovalService.FindInfo("", "", item.CostCenter);
                //var buheadOld = costCenter.BUHeadMUDID;
                listCos.Add(costCenter);
            }

            int state = PreApprovalService.ImportCostCenter(excelRows, ref fails, adminUser.Name);
            //foreach (var item in excelRows)
            //{
            //    //将旧成本中心换成新成本中心
            //    //var updOldRes = PreApprovalService.UpdateUncompleteCostenterCodeByOldCostCenterCode(item.TA + "-" + item.Region + "(" + item.CostCenter + ")", item.TA + "-" + item.Region + "(" + item.OldCostCenter + ")");
            //    var buheadOld = listCos.Find(x => x.ID == item.ID).BUHeadMUDID;
            //    //var pendingList = PreApprovalService.LoadPreApprovalByCostCenter(item.TA + "-" + item.Region + "(" + item.CostCenter + ")");
            //    var pendingList = PreApprovalService.LoadPreApprovalByCurrentApprover(buheadOld, item.TA + "-" + item.Region + "(" + item.CostCenter + ")");
            //    foreach (var pItem in pendingList)
            //    {
            //        //if (!pItem.CurrentApproverMUDID.Equals(item.BUHeadMUDID) || !pItem.Equals(item.BUHeadName))
            //        if (!pItem.CurrentApproverMUDID.Equals(item.BUHeadMUDID) || !pItem.CurrentApproverName.Equals(item.BUHeadName))
            //        {
            //            //成本中心审批人不一致，变更审批人
            //            var updRes = PreApprovalService.UpdatePendingPreAPprovalBUHead(pItem.HTCode, item.BUHeadName, item.BUHeadMUDID, pItem.BUHeadMUDID);
            //            //向新的审批人发送消息
            //            if (updRes == 1)
            //            {
            //                string applicantMsg = pItem.ModifyDate == pItem.CreateDate ? $"{pItem.HTCode}，您有需要审批的预申请。请<a href='{ConfigurationManager.AppSettings["MealH5SiteUrl"]}/P/PreApproval/Approval?id={pItem.ID}&from=0'>点击这里</a>进行审批。" : $"{pItem.HTCode}，您有需要审批的预申请修改。请<a href='{ConfigurationManager.AppSettings["MealH5SiteUrl"]}/P/PreApproval/Approval?id={pItem.ID}&from=0'>点击这里</a>进行审批。";
            //                var rtnVal = WxMessageClientChannelFactory.GetChannel().SendText(item.BUHeadMUDID, applicantMsg);
            //                //LogHelper.Info("auto send BriefReport Content：【" + JsonConvert.SerializeObject(brief) + "】, | toUser: 【" + touser + "】，| result:" + rtnVal);
            //            }
            //        }
            //    }
            //}
            if (state > 0)
            {
                var num = operationAuditService.AddAudit("0", "导入成本中心");
            }
            return Json(new
            {
                state
            }, "text/html", JsonRequestBehavior.AllowGet);
        }
        #endregion 
        private string GetStringFromCell(ICell cell)
        {
            if (cell == null)
            {
                return string.Empty;
            }

            switch (cell.CellType)
            {
                case CellType.Numeric:
                    return cell.NumericCellValue.ToString();
                case CellType.String:
                    return cell.StringCellValue;
                default:
                    return string.Empty;
            }
        }

        #region 预申请MMCoE审批
        public JsonResult MMCoELoad(string srh_HTCode, string srh_ApplierMUDID, string srh_startMeetingDate, string srh_endMeetingDate, int rows, int page)
        {
            int total;
            var list = PreApprovalService.MMCoELoad(srh_HTCode, srh_ApplierMUDID, srh_startMeetingDate, srh_endMeetingDate, rows, page, out total);
            var preApproval = new List<P_PreApproval_View>();
            foreach (var i in list)
            {
                preApproval.Add(GetDisplayObj(i));
            }
            return Json(new { state = 1, rows = preApproval, total = total });


        }

        #endregion

        #region 预申请MMCoE审批--通过
        public JsonResult Approved(string PID, string txtComments, string BudgetTotal)
        {
            //向预申请记录中插入数据
            //新建ID
            Guid ID = Guid.NewGuid();
            //当前登陆用户姓名
            AdminUser adminUser = Session[ConstantHelper.CurrentAdminUser] as AdminUser;
            string UserName = adminUser.Name;
            string UserId = adminUser.Email;
            int ActionType = 3;
            DateTime ApproveDate = DateTime.Now;
            int type = 1;



            //修改预申请状态，金额大于2000状态=6 小于两千状态=3
            string PreApprovalState = "6";
            if (double.Parse(BudgetTotal) >= 2000)
            {
                PreApprovalState = "3";
            }
            var state = PreApprovalService.UpdateStadeApproved(PID, PreApprovalState);
            if (state > 0)
            {
                var preList = PreApprovalService.GetPreApprovalByID(PID.ToString());
                var channel = WxMessageClientChannelFactory.GetChannel();
                channel.SendPreApprovalStateChangeMessageToUser(preList[0]);
            }

            int res = PreApprovalService.Approved(ID, PID, UserName, UserId, ActionType, ApproveDate, txtComments, type);

            return Json(new { state = 1, txt = "提交成功" });
        }

        #endregion

        #region 预申请MMCoE审批--驳回
        public JsonResult Reject(string PID, string txtComments, string BudgetTotal)
        {
            //向预申请记录中插入数据
            //新建ID
            Guid ID = Guid.NewGuid();
            //当前登陆用户姓名
            AdminUser adminUser = Session[ConstantHelper.CurrentAdminUser] as AdminUser;
            string UserName = adminUser.Name;
            string UserId = adminUser.Email;
            int ActionType = 2;
            DateTime ApproveDate = DateTime.Now;
            int type = 1;

            //修改预申请状态，状态=2
            string PreApprovalState = "2";
            var state = PreApprovalService.UpdateStadeApproved(PID, PreApprovalState);
            if (state > 0)
            {
                var preList = PreApprovalService.GetPreApprovalByID(PID.ToString());
                var channel = WxMessageClientChannelFactory.GetChannel();
                channel.SendPreApprovalRejectMessageToUser(txtComments, preList[0]);
            }

            int res = PreApprovalService.Approved(ID, PID, UserName, UserId, ActionType, ApproveDate, txtComments, type);

            return Json(new { state = 1, txt = "提交成功" });

        }
        #endregion

        #region 预申请详情导出PDF
        /// <summary>
        /// 预申请详情导出PDF
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationAuditFilter(Operation = "预申请详情导出PDF", OperationAuditTypeName = "预申请详情导出PDF")]
        public ActionResult PdfExport(string id)
        {
            //预申请详情导出查询
            var list = PreApprovalService.GetApproval(id);
            var a = PreApprovalService.GetPreApprovalByID(id);
            var p = new P_PreApproval_View();
            var obj = new List<P_PreApprovalApproveHistory_Time>();
            p = GetDisplayObj(a[0]);
            foreach (var i in list)
            {
                obj.Add(GetTimeObj(i));
            }

            #region 写入到客户端
            //using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            //{
            //    wk.Write(ms);
            //    Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xls", DateTime.Now.ToString("yyyyMMddHHmmssfff")));
            //    Response.ContentType = "application/vnd.ms-excel";
            //    Response.BinaryWrite(ms.ToArray());
            //}


            Document document = new Document(PageSize.A4);
            var path = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            if (!Directory.Exists(Server.MapPath("~/preApproval/")))
            {
                Directory.CreateDirectory(Server.MapPath("~/preApproval/"));
            }
            DirectoryInfo dir = new DirectoryInfo(Server.MapPath("~/preApproval/"));
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                                                                   //遍历删除文件   防止临时文件过多。
            foreach (FileSystemInfo i in fileinfo)
            {
                if (i is DirectoryInfo)            //判断是否文件夹
                {
                    DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                    subdir.Delete(true);          //删除子目录和文件
                }
                else
                {
                    System.IO.File.Delete(i.FullName);      //删除指定文件
                }
            }
            PdfWriter.GetInstance(document, new FileStream(Server.MapPath("~/preApproval/") + path + ".pdf", FileMode.Create));
            document.Open();
            BaseFont baseFont = BaseFont.CreateFont(@"c:\Windows\fonts\SIMSUN.TTC,1", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            Font font = new Font(baseFont);




            #endregion


            #region 文本
            PdfPTable table = new PdfPTable(4);
            PdfPCell cell1 = new PdfPCell(new Phrase($"申请人姓名:{p.ApplierName}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell1);
            PdfPCell cell2 = new PdfPCell(new Phrase($"申请人MUDID:{p.ApplierMUDID}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell2);
            PdfPCell cell3 = new PdfPCell(new Phrase($"申请人Territory Code:{p.MRTerritoryCode}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell3);
            PdfPCell cell4 = new PdfPCell(new Phrase($"申请人职位:{p.Position}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell4);
            //PdfPCell cell5 = new PdfPCell(new Phrase($"申请人手机号码:{p.ApplierMobile}", font))
            PdfPCell cell5 = new PdfPCell(new Phrase($"申请人手机号码:", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell5);
            PdfPCell cell6 = new PdfPCell(new Phrase($"预申请申请日期:{p.CreateDate}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell6);
            PdfPCell cell7 = new PdfPCell(new Phrase($"预申请申请时间:{p.CreateTime}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell7);
            PdfPCell cell8 = new PdfPCell(new Phrase($"预申请修改日期:{p.ModifyDate}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell8);
            PdfPCell cell9 = new PdfPCell(new Phrase($"预申请修改时间:{p.ModifyTime}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell9);
            PdfPCell cell10 = new PdfPCell(new Phrase($"HT编号:{p.HTCode}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell10);
            PdfPCell cell11 = new PdfPCell(new Phrase($"Market:{p.Market}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell11);
            PdfPCell cell143 = new PdfPCell(new Phrase($"VeevaMeetingID:{p.VeevaMeetingID}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell143);
            PdfPCell cell12 = new PdfPCell(new Phrase($"TA:{p.TA}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell12);
            PdfPCell cell13 = new PdfPCell(new Phrase($"省份:{p.Province}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell13);
            PdfPCell cell14 = new PdfPCell(new Phrase($"城市:{p.City}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell14);
            PdfPCell cell15 = new PdfPCell(new Phrase($"医院编码:{p.HospitalCode}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell15);
            PdfPCell cell16 = new PdfPCell(new Phrase($"医院名称:{p.HospitalName}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell16);
            PdfPCell cell17 = new PdfPCell(new Phrase($"医院地址:{p.HospitalAddress}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell17);
            PdfPCell cell18 = new PdfPCell(new Phrase($"会议名称:{p.MeetingName}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell18);
            PdfPCell cell19 = new PdfPCell(new Phrase($"会议日期:{p.MeetingDate}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell19);
            PdfPCell cell20 = new PdfPCell(new Phrase($"会议时间:{p.MeetingTime}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell20);
            PdfPCell cell21 = new PdfPCell(new Phrase($"参会人数:{p.AttendCount}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell21);
            PdfPCell cell22 = new PdfPCell(new Phrase($"大区区域代码:{p.CostCenter}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell22);
            var HTType = p.HTType == "1" ? "线上HT" : p.HTType == "0" ? "线下HT" : "";
            PdfPCell cell23 = new PdfPCell(new Phrase($"HT形式:{HTType}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell23);
            PdfPCell cell24 = new PdfPCell(new Phrase($"预算金额:{p.BudgetTotal}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell24);
            var IsDMFollow = p.IsDMFollow == true ? "是" : "否";
            PdfPCell cell25 = new PdfPCell(new Phrase($"直线经理是否随访:{IsDMFollow}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell25);
            var IsFreeSpeaker = p.IsFreeSpeaker == true ? "是" : "否";
            PdfPCell cell26 = new PdfPCell(new Phrase($"是否由外部免费讲者来讲:{IsFreeSpeaker}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell26);
            PdfPCell cell27 = new PdfPCell(new Phrase($"RD Territory Code:{p.RDTerritoryCode}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell27);
            PdfPCell cell28 = new PdfPCell(new Phrase($"预申请审批人姓名:{p.BUHeadName}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell28);
            PdfPCell cell29 = new PdfPCell(new Phrase($"预申请审批人MUDID:{p.BUHeadMUDID}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell29);
            PdfPCell cell30 = new PdfPCell(new Phrase($"预申请审批日期:{p.BUHeadApproveDate}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell30);
            PdfPCell cell31 = new PdfPCell(new Phrase($"预申请审批时间:{p.BUHeadApproveTime}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell31);
            PdfPCell cell32 = new PdfPCell(new Phrase($"预申请审批状态:{p.State}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell32);
            var isReAssign = p.IsReAssign == true ? "是" : "否";
            PdfPCell cell33 = new PdfPCell(new Phrase($"预申请是否重新分配审批人:{isReAssign}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell33);
            PdfPCell cell34 = new PdfPCell(new Phrase($"预申请重新分配审批人-操作人:{p.ReAssignOperatorName}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell34);
            PdfPCell cell35 = new PdfPCell(new Phrase($"预申请重新分配审批人-操作人MUDID:{p.ReAssignOperatorMUDID}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell35);
            PdfPCell cell36 = new PdfPCell(new Phrase($"预申请被重新分配审批人姓名:{p.ReAssignBUHeadName}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell36);
            PdfPCell cell37 = new PdfPCell(new Phrase($"预申请被重新分配审批人MUDID:{p.ReAssignBUHeadMUDID}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell37);
            PdfPCell cell38 = new PdfPCell(new Phrase($"预申请重新分配审批人日期:{p.ReAssignBUHeadApproveDate}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell38);
            PdfPCell cell39 = new PdfPCell(new Phrase($"预申请重新分配审批人时间:{p.ReAssignBUHeadApproveTime}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell39);
            PdfPCell cell40 = new PdfPCell(new Phrase($"***************************审批流程***************************", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell40);
            #endregion

            #region 表格
            table.SetWidths(new float[] { 1, 1, 2, 1 });
            PdfPCell cell_a1 = new PdfPCell(new Phrase("操作人", font));
            table.AddCell(cell_a1);
            PdfPCell cell_a2 = new PdfPCell(new Phrase("操作", font));
            table.AddCell(cell_a2);
            PdfPCell cell_a3 = new PdfPCell(new Phrase("审批意见", font));
            table.AddCell(cell_a3);
            PdfPCell cell_a4 = new PdfPCell(new Phrase("审批时间", font));
            table.AddCell(cell_a4);

            foreach (var item in obj)
            {
                table.AddCell(new PdfPCell(new Phrase(item.UserName, font)));
                table.AddCell(new PdfPCell(new Phrase(item.ActionType, font)));
                table.AddCell(new PdfPCell(new Phrase(item.Comments, font)));
                table.AddCell(new PdfPCell(new Phrase(item.ApproveDate, font)));
            }
            #endregion


            document.Add(table);
            document.Close();

            return File(Server.MapPath("~/preApproval/") + path + ".pdf", "application/pdf", path + ".pdf");

        }
        #endregion

        #region 根据Market获取TA
        /// <summary>
        /// 根据market获取TA
        /// </summary>
        /// <param name="marketName"></param>
        /// <returns></returns>
        public JsonResult LoadTa(string marketName)
        {
            var res = marketService.LoadTAByMarket(marketName);
            return Json(new { state = 1, data = res });
        }
        #endregion

        public class PreApprovalDownloadImage
        {
            private List<P_PreApproval> PreApprovalList;
            private string tempFilePath1;
            private string email;

            private static string awsAccessKey = "AKIAPJZX4QFWMMMA2RJA";
            private static string awsSecretKey = "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";
            private static string bucketName = "wechat";
            private IPreApprovalService PreApprovalService;
            AmazonS3Config config = new AmazonS3Config()
            {
                ServiceURL = "http://s3.amazonaws.com",
                RegionEndpoint = Amazon.RegionEndpoint.CNNorth1
            };
            public PreApprovalDownloadImage(List<P_PreApproval> PreApprovalList, string tempFilePath1, string email, IPreApprovalService PreApprovalService)
            {
                this.PreApprovalList = PreApprovalList;
                this.tempFilePath1 = tempFilePath1;
                this.email = email;
                this.PreApprovalService = PreApprovalService;
            }
            public void InitImage()
            {

                try
                {
                    if (!Directory.Exists(tempFilePath1))//判断文件夹是否存在 
                    {
                        Directory.CreateDirectory(tempFilePath1);//不存在则创建文件夹 
                    }
                }
                catch (Exception e)
                {
                    LogHelper.Error("导出讲者服务协议图片-操作文件错误", e);
                }
                //s3 下载文件
                var s3Handler = new S3Handler();
                var time = DateTime.Now.ToString("yyyyMMddhhmmss");
                foreach (var record in PreApprovalList)
                {
                    //判断图片不为空
                    if (!string.IsNullOrEmpty(record.SpeakerServiceImage))
                    {
                        //多个图片是用 , 拼接路径
                        var imgList = record.SpeakerServiceImage.Split(',');
                        for (var i = 0; i < imgList.Length; i++)
                        {
                            var fileName = i + 1;
                            //删掉字符串中的 /
                            var s3key = imgList[i].Substring(1, imgList[i].Length - 1);
                            string extension = Path.GetExtension(imgList[i]);
                            s3Handler.S3Download(s3key, tempFilePath1 + "/" + time + "/" + record.HTCode + "/Speaker Agreement/" + "Speaker Agreement" + fileName + extension);


                        }
                    }
                    //判断图片不为空
                    if (!string.IsNullOrEmpty(record.SpeakerBenefitImage))
                    {
                        //多个图片是用 , 拼接路径
                        var imgList = record.SpeakerBenefitImage.Split(',');
                        for (var i = 0; i < imgList.Length; i++)
                        {
                            var fileName = i + 1;
                            //删掉字符串中的 /
                            var s3key = imgList[i].Substring(1, imgList[i].Length - 1);
                            string extension = Path.GetExtension(imgList[i]);
                            s3Handler.S3Download(s3key, tempFilePath1 + "/" + time + "/" + record.HTCode + "/Speaker COI/" + "Speaker COI" + fileName + extension);
                        }
                    }
                }
                //添加文件夹下的所有文件到压缩包里
                var tempFilePathAfter = tempFilePath1 + "/" + time + "/";
                using (ZipFile zip = new ZipFile(System.Text.Encoding.Default))
                {
                    zip.AddDirectory(tempFilePathAfter);
                    zip.UseZip64WhenSaving = Zip64Option.AsNecessary;
                    var fileId = Guid.NewGuid();
                    if (!Directory.Exists(tempFilePath1 + "Zip/" + time + "/"))//判断文件夹是否存在 
                    {
                        Directory.CreateDirectory(tempFilePath1 + "Zip/" + time + "/");//不存在则创建文件夹 
                    }
                    zip.Save(tempFilePath1 + "Zip/" + time + "/" + fileId + ".zip");

                    //每周导出一次,导出当前日期前七天的
                    using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, config))
                    {
                        var key = string.Format("PreApprovalCache/") + fileId + ".zip";
                        //var request = new PutObjectRequest()
                        //{
                        //    BucketName = bucketName,
                        //    CannedACL = S3CannedACL.PublicRead,
                        //    Key = key,
                        //    InputStream = new FileStream(Server.MapPath("~/TempZip/") + fileId + ".zip", FileMode.Open, FileAccess.Read)
                        //};
                        //client.PutObject(request);

                        // Create list to store upload part responses.
                        List<UploadPartResponse> uploadResponses = new List<UploadPartResponse>();

                        // Setup information required to initiate the multipart upload.
                        InitiateMultipartUploadRequest initiateRequest = new InitiateMultipartUploadRequest
                        {
                            BucketName = bucketName,
                            Key = key,
                            CannedACL = S3CannedACL.PublicRead,
                        };

                        // Initiate the upload.
                        InitiateMultipartUploadResponse initResponse = client.InitiateMultipartUpload(initiateRequest);

                        // Upload parts.
                        string filePath = tempFilePath1 + "Zip/" + time + "/" + fileId + ".zip";
                        long contentLength = new FileInfo(filePath).Length;
                        long partSize = 5 * (long)Math.Pow(2, 20); // 5 MB

                        try
                        {
                            long filePosition = 0;
                            for (int ii = 1; filePosition < contentLength; ii++)
                            {
                                UploadPartRequest uploadRequest = new UploadPartRequest
                                {
                                    BucketName = bucketName,
                                    Key = key,
                                    UploadId = initResponse.UploadId,
                                    PartNumber = ii,
                                    PartSize = partSize,
                                    FilePosition = filePosition,
                                    FilePath = filePath
                                };

                                // Upload a part and add the response to our list.
                                uploadResponses.Add(client.UploadPart(uploadRequest));

                                filePosition += partSize;
                            }

                            // Setup to complete the upload.
                            CompleteMultipartUploadRequest completeRequest = new CompleteMultipartUploadRequest
                            {
                                BucketName = bucketName,
                                Key = key,
                                UploadId = initResponse.UploadId,
                            };
                            completeRequest.AddPartETags(uploadResponses);

                            // Complete the upload.
                            CompleteMultipartUploadResponse completeUploadResponse = client.CompleteMultipartUpload(completeRequest);

                            //发送邮件
                            PreApprovalService.InsertFileLink("http://wechat.s3.cn-north-1.amazonaws.com.cn/PreApprovalCache/" + fileId + ".zip", email);
                        }
                        catch (Exception exception)
                        {
                            // Abort the upload.
                            AbortMultipartUploadRequest abortMPURequest = new AbortMultipartUploadRequest
                            {
                                BucketName = bucketName,
                                Key = key,
                                UploadId = initResponse.UploadId
                            };
                            client.AbortMultipartUploadAsync(abortMPURequest);
                        }
                    }
                }
                DirectoryInfo dir = new DirectoryInfo(tempFilePath1 + "/" + time);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                                                                       //遍历删除文件   防止临时文件过多。
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)            //判断是否文件夹
                    {
                        DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                        subdir.Delete(true);          //删除子目录和文件
                    }
                    else
                    {
                        System.IO.File.Delete(i.FullName);      //删除指定文件
                    }
                }
                dir.Delete(true);
            }
        }

        #region 地址审批查询
        [OperationAuditFilter(Operation = "地址审批查询", OperationAuditTypeName = "地址审批查询")]
        public JsonResult LoadAddressApprove(string srh_DACode, string srh_ApplierMUDID, string srh_ApproverMUDID, string srh_GskHospital, string srh_StartApplyDate, string srh_EndApplyDate, string srh_State, string srh_IsDelete, int rows, int page)
        {
            try
            {
                int total;
                //List<WP_QYUSER> wp_users = new List<WP_QYUSER>();
                //wp_users = PreApprovalService.LoadWPQYUSER();

                var list = PreApprovalService.LoadAddressApprove(srh_DACode, srh_ApplierMUDID, srh_ApproverMUDID, srh_GskHospital, srh_StartApplyDate, srh_EndApplyDate, srh_State, srh_IsDelete, rows, page, out total);
                List<P_AddressApproval_View> addressApproval_View = new List<P_AddressApproval_View>();
                foreach (var i in list)
                {
                    addressApproval_View.Add(GetDisplayData(i));
                }
                return Json(new { state = 1, rows = addressApproval_View, total = total });
            }
            catch (Exception ex)
            {
                LogHelper.Error("LoadAddressApprove", ex);
                return Json(new { state = 0, rows = new List<P_AddressApproval_View>(), total = 0 });
            }
        }

        private P_AddressApproval_View GetDisplayData(P_AddressApproval_View item)
        {
            P_AddressApproval_View addressApproval_View = new P_AddressApproval_View();
            addressApproval_View.ID = item.ID;
            addressApproval_View.DACode = item.DACode;
            addressApproval_View.ApplierName = item.ApplierName;
            addressApproval_View.ApplierMUDID = item.ApplierMUDID;
            addressApproval_View.Position = item.Position;
            addressApproval_View.CreateDate = item.CreateDate;
            addressApproval_View.Market = item.Market;
            addressApproval_View.TA = item.TA;
            addressApproval_View.GskHospital = item.GskHospital;
            addressApproval_View.Province = item.Province;
            addressApproval_View.City = item.City;
            addressApproval_View.HospitalName = item.HospitalName;
            addressApproval_View.MainHospitalAddress = item.MainHospitalAddress;
            addressApproval_View.MainAddress = item.MainHospitalAddress;
            addressApproval_View.AddressName = item.AddressName;
            addressApproval_View.AddAddress = item.AddAddress;
            addressApproval_View.District = item.District;
            if (!string.IsNullOrEmpty(item.Distance) && item.Distance != "NaN")
                addressApproval_View.Distance = Math.Round((decimal.Parse(item.Distance) / 1000), 3).ToString();
            else
                addressApproval_View.Distance = decimal.Parse("0.000").ToString();
            //addressApproval_View.Distance = item.Distance;
            addressApproval_View.ApprovalStatus = item.ApprovalStatus;
            if (addressApproval_View.ApprovalStatus == 1 || addressApproval_View.ApprovalStatus == 5 || addressApproval_View.ApprovalStatus == 7)
                addressApproval_View.RejectReason = "";
            else
                addressApproval_View.RejectReason = item.RejectReason;
            addressApproval_View.LineManagerName = item.LineManagerName;
            addressApproval_View.LineManagerMUDID = item.LineManagerMUDID;
            addressApproval_View.LineManagerApproveDateDisplay = item.LineManagerApproveDate == null ? "" : item.LineManagerApproveDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
            addressApproval_View.Remark = item.Remark == null ? "" : item.Remark;
            addressApproval_View.AddressNameDisplay = item.AddressNameDisplay;
            var numState = item.ApprovalStatus;
            string strState = "";
            switch (numState)
            {
                case 0:
                case 9:
                case 10:
                    strState = "地址申请待审批";
                    break;
                case 1:
                case 5:
                case 7:
                    strState = "地址申请审批通过";
                    break;
                case 2:
                case 6:
                case 8:
                    strState = "地址申请审批驳回";
                    break;
                case 3:
                    strState = "地址申请已失效";
                    break;
                case 4:
                    strState = "地址申请已取消";
                    break;
                default:
                    break;

            }
            addressApproval_View.ApproveStatusDisplay = strState;
            if (item.IsDelete == 2 || item.IsDelete == 99)
            {
                addressApproval_View.IsDeleteDisplay = "该医院已被删除";
            }
            else if (item.IsDelete == 3)
            {
                addressApproval_View.IsDeleteDisplay = "该医院主地址已被修改";
            }
            else if (item.IsDelete == 1)
            {
                addressApproval_View.IsDeleteDisplay = "该外送地址项目组已删除";
            }
            else
                addressApproval_View.IsDeleteDisplay = "";
            addressApproval_View.DeleteDate = item.DeleteDate;
            return addressApproval_View;
        }

        public ActionResult AddressApplicationDetails(Guid id)
        {
            ViewBag.id = id;
            return View();
        }

        [OperationAuditFilter(Operation = "查看外送地址申请详情", OperationAuditTypeName = "查看外送地址申请详情")]
        public JsonResult AddressDetailsLoad(Guid id)
        {
            P_AddressApproval_View _AddressApproval = PreApprovalService.LoadAddressApprovalInfo(id);
            List<P_AddressApproveHistory> _AddressApproveHistory = PreApprovalService.LoadAddressApprovalHistory(id);

            return Json(new { state = 1, AddressData = _AddressApproval, HistoryData = _AddressApproveHistory });
        }

        #region 导出地址审批查询
        [OperationAuditFilter(Operation = "导出地址申请", OperationAuditTypeName = "导出地址申请")]
        public void ExportAddressApprovalList(string srh_DACode, string srh_ApplierMUDID, string srh_ApproverMUDID, string srh_GskHospital, string srh_StartApplyDate, string srh_EndApplyDate, string srh_State, string srh_IsDelete)
        {
            #region 抓取数据            
            var list = PreApprovalService.ExportAddressApprovalList(srh_DACode, srh_ApplierMUDID, srh_ApproverMUDID, srh_GskHospital, srh_StartApplyDate, srh_EndApplyDate, srh_State, srh_IsDelete);
            #endregion

            #region 构建Excel
            FileStream file11 = new FileStream(Server.MapPath("~/Template/Template_AddressApprove.xls"), FileMode.Open, FileAccess.Read);
            HSSFWorkbook wk = new HSSFWorkbook(file11);

            ISheet sheet = wk.GetSheet("Report");

            #endregion

            #region 制作表体
            for (var i = 1; i <= list.Count; i++)
            {
                var item = list[i - 1];
                IRow row = sheet.CreateRow(i);
                ICell cell = null;
                var j = 0;
                if (item != null)
                {
                    cell = row.CreateCell(j);
                    cell.SetCellValue(item.DACode);

                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.ApplierName);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.ApplierMUDID);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.Position);
                    cell = row.CreateCell(++j);
                    if (item.CreateDate != null)
                        cell.SetCellValue(item.CreateDate.Value);
                    else
                        cell.SetCellValue("");
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.CreateDate == null ? "" : item.CreateDate.Value.ToString("HH:mm:ss"));
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.Market);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.TA);

                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.GskHospital);

                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.Province);

                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.City);

                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.HospitalName);

                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.MainHospitalAddress);

                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.AddressName);

                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.AddAddress);

                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.Province);

                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.City);

                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.District);

                    cell = row.CreateCell(++j);
                    if (!string.IsNullOrEmpty(item.Distance) && item.Distance != "NaN")
                        cell.SetCellValue(Math.Round((decimal.Parse(item.Distance) / 1000), 3).ToString());
                    else
                        cell.SetCellValue(decimal.Parse("0.000").ToString());

                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.Remark);

                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.AddressNameDisplay);

                    cell = row.CreateCell(++j);
                    if (item.IsDelete == 2 || item.IsDelete == 99)
                        cell.SetCellValue("该医院已被删除");
                    else if (item.IsDelete == 3)
                        cell.SetCellValue("该医院主地址已被修改");
                    else if (item.IsDelete == 1)
                        cell.SetCellValue("该外送地址项目组已删除");
                    else
                        cell.SetCellValue("");

                    var numState = item.ApprovalStatus;
                    string strState = "";
                    switch (numState)
                    {
                        case 0:
                        case 9:
                        case 10:
                            strState = "地址申请待审批";
                            break;
                        case 1:
                        case 5:
                        case 7:
                            strState = "地址申请审批通过";
                            break;
                        case 2:
                        case 6:
                        case 8:
                            strState = "地址申请审批驳回";
                            break;
                        case 3:
                            strState = "地址申请已失效";
                            break;
                        case 4:
                            strState = "地址申请已取消";
                            break;
                        default:
                            break;
                    }
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(strState);
                    cell = row.CreateCell(++j);
                    if (item.ApprovalStatus == 1 || item.ApprovalStatus == 5 || item.ApprovalStatus == 7)
                        cell.SetCellValue("");
                    else
                        cell.SetCellValue(item.RejectReason);

                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.LineManagerName);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.LineManagerMUDID);
                    cell = row.CreateCell(++j);
                    //cell.SetCellValue(item.LineManagerApproveDate == null ? "" : item.LineManagerApproveDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                    if (item.LineManagerApproveDate != null)
                        cell.SetCellValue(item.LineManagerApproveDate.Value);
                    else
                        cell.SetCellValue("");
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.LineManagerApproveDate == null ? "" : item.LineManagerApproveDate.Value.ToString("HH:mm:ss"));

                    cell = row.CreateCell(++j);
                    //cell.SetCellValue(item.LineManagerApproveDate == null ? "" : item.LineManagerApproveDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                    if (item.DeleteDate != null)
                        cell.SetCellValue(item.DeleteDate.Value);
                    else
                        cell.SetCellValue("");
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.DeleteDate == null ? "" : item.DeleteDate.Value.ToString("HH:mm:ss"));

                }

            }
            #endregion

            #region 写入到客户端
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                wk.Write(ms);
                Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xls", "Add-Approval_" + DateTime.Now.ToString("yyyyMMddHHmm")));
                Response.ContentType = "application/vnd.ms-excel";
                Response.BinaryWrite(ms.ToArray());
            }
            #endregion
        }
        #endregion

        #endregion

        #region 发送预申请报表
        public ActionResult AddRecipient(string srh_HTCode, string srh_HospitalCode, string srh_CostCenter, string srh_ApplierMUDID, string srh_ApplierTerritory, string srh_BUHeadMUDID, string srh_Market, string srh_TA, string srh_State, string srh_StartBUHeadApproveDate, string srh_EndBUHeadApproveDate, string srh_StartMeetingDate, string srh_EndMeetingDate, string srh_RD, int srh_BudgetTotal)
        {
            ViewBag.HTCode = srh_HTCode;
            ViewBag.HospitalCode = srh_HospitalCode;
            ViewBag.CostCenter = srh_CostCenter;
            ViewBag.ApplierMUDID = srh_ApplierMUDID;
            ViewBag.ApplierTerritory = srh_ApplierTerritory;
            ViewBag.BUHeadMUDID = srh_BUHeadMUDID;
            ViewBag.Market = srh_Market;
            ViewBag.TA = srh_TA;
            ViewBag.State = srh_State;
            ViewBag.StartBUHeadApproveDate = srh_StartBUHeadApproveDate;
            ViewBag.EndBUHeadApproveDate = srh_EndBUHeadApproveDate;
            ViewBag.StartMeetingDate = srh_StartMeetingDate;
            ViewBag.EndMeetingDate = srh_EndMeetingDate;
            ViewBag.RDTerritoryCode = srh_RD;
            ViewBag.BudgetTotal = srh_BudgetTotal;
            return View();
        }

        public JsonResult Send(string Recipient, string CCs, string srh_HTCode, string srh_HospitalCode, string srh_CostCenter, string srh_ApplierMUDID, string srh_ApplierTerritory, string srh_BUHeadMUDID, string srh_Market, string srh_TA, string srh_State, string srh_StartBUHeadApproveDate, string srh_EndBUHeadApproveDate, string srh_StartMeetingDate, string srh_EndMeetingDate, string srh_RD, int srh_BudgetTotal)
        {
            try
            {
                #region 抓取数据            
                string oldHospitalCode = PreApprovalService.GetOldGskHospitalCodeByGskHospitalCode(srh_HospitalCode);
                if (!string.IsNullOrEmpty(oldHospitalCode))
                {
                    srh_HospitalCode += "," + oldHospitalCode;
                }
                string oldCostCenter = PreApprovalService.GetOldCostcenterByCostcenter(srh_CostCenter);
                if (!string.IsNullOrEmpty(oldCostCenter))
                {
                    srh_CostCenter += "," + oldCostCenter;
                }
                var list = PreApprovalService.ExportPreApproval(srh_HTCode, srh_HospitalCode, srh_CostCenter, srh_ApplierMUDID, srh_ApplierTerritory, srh_BUHeadMUDID, srh_Market, srh_TA, srh_State, srh_StartBUHeadApproveDate, srh_EndBUHeadApproveDate, srh_StartMeetingDate, srh_EndMeetingDate, srh_RD, srh_BudgetTotal);
                #endregion

                #region 构建Excel
                FileStream file11 = new FileStream(Server.MapPath("~/Template/Template_Pre-ApprovalReport.xls"), FileMode.Open, FileAccess.Read);
                HSSFWorkbook wk = new HSSFWorkbook(file11);

                ISheet sheet = wk.GetSheet("Report");

                #endregion

                #region 制作表体
                for (var i = 1; i <= list.Count; i++)
                {
                    var item = list[i - 1];
                    IRow row = sheet.CreateRow(i);
                    ICell cell = null;
                    var j = 0;
                    if (item != null)
                    {
                        cell = row.CreateCell(j);
                        cell.SetCellValue(item.ApplierName);// "申请人姓名");
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.ApplierMUDID);//"申请人MUDID");
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.MRTerritoryCode);//"申请人Territory Code");
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.Position);// "申请人职位");
                        cell = row.CreateCell(++j);
                        //cell.SetCellValue(item.ApplierMobile);// "申请人手机号码");
                        cell.SetCellValue("");
                        cell = row.CreateCell(++j);
                        if (item.CreateDate != null)
                        {
                            cell.SetCellValue(item.CreateDate.Value);// "预申请申请日期");
                        }
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.CreateDate == null ? "" : item.CreateDate.Value.ToString("HH:mm:ss"));// "预申请申请时间");
                        if (item.CreateDate == item.ModifyDate)
                        {
                            cell = row.CreateCell(++j);
                            cell.SetCellValue(string.Empty);// "预申请修改日期");
                            cell = row.CreateCell(++j);
                            cell.SetCellValue(string.Empty);// "预申请修改时间");
                        }
                        else
                        {
                            cell = row.CreateCell(++j);
                            if (item.ModifyDate != item.CreateDate)
                            {
                                cell.SetCellValue(item.ModifyDate.Value);// "预申请修改日期");
                            }
                            cell = row.CreateCell(++j);
                            cell.SetCellValue(item.ModifyDate == null ? "" : item.ModifyDate.Value.ToString("HH:mm:ss"));// "预申请修改时间");
                        }
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.HTCode);// "HT编号");
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.Market);// "Market");
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.VeevaMeetingID == null ? "" : item.VeevaMeetingID.ToString());//VeevaMeetingID
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.TA);// "TA");
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.Province);// "省份");
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.City);// "城市");
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.HospitalCode);// "医院编码");
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.HospitalName);// "医院名称");
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.HospitalAddress);// "医院地址");
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.MeetingName);// "会议名称");
                        cell = row.CreateCell(++j);
                        if (item.MeetingDate != null)
                        {
                            cell.SetCellValue(item.MeetingDate.Value);// "会议日期");
                        }
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.MeetingDate == null ? "" : item.MeetingDate.Value.ToString("HH:mm:ss"));// "会议时间");
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.AttendCount);// "参会人数");
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.CostCenter);// "大区区域代码");
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.HTType == "1" ? "线上HT" : item.HTType == "0" ? "线下HT" : "");// "HT形式");
                        cell = row.CreateCell(++j);
                        double budgetTotal;
                        double.TryParse(item.BudgetTotal.ToString(), out budgetTotal);
                        cell.SetCellValue(budgetTotal);// "预算金额");
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.IsDMFollow == true ? "是" : "否");// "直线经理是否随访");
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.IsFreeSpeaker == true ? "是" : "否");//是否由外部免费讲者来讲
                                                                                  //cell = row.CreateCell(++j);
                                                                                  //cell.SetCellValue(item.RDSDName);
                                                                                  //cell = row.CreateCell(++j);
                                                                                  //cell.SetCellValue(item.RDSDMUDID);
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.RDTerritoryCode);// "RD Territory Code");
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.CurrentApproverName);// "预申请审批人姓名");
                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.CurrentApproverMUDID);// "预申请审批人MUDID");
                                                                     //if (item.AttendCount < 60 && item.BudgetTotal < 1200)
                                                                     //{
                                                                     //    cell = row.CreateCell(++j);
                                                                     //    cell.SetCellValue("系统自动审批");// "预申请审批人姓名");
                                                                     //    cell = row.CreateCell(++j);
                                                                     //    cell.SetCellValue("系统自动审批");// "预申请审批人MUDID");
                                                                     //}
                                                                     ////else if (item.State == "6" && item.CurrentApproverMUDID == null)
                                                                     ////{
                                                                     ////    cell = row.CreateCell(++j);
                                                                     ////    cell.SetCellValue("系统自动审批");// "预申请审批人姓名");
                                                                     ////    cell = row.CreateCell(++j);
                                                                     ////    cell.SetCellValue("系统自动审批");// "预申请审批人MUDID");
                                                                     ////}
                                                                     //else if (item.BudgetTotal >= 1200 && item.BudgetTotal < 1500)
                                                                     //{
                                                                     //    cell = row.CreateCell(++j);
                                                                     //    cell.SetCellValue(item.CurrentApproverName);// "预申请审批人姓名");
                                                                     //    cell = row.CreateCell(++j);
                                                                     //    cell.SetCellValue(item.CurrentApproverMUDID);// "预申请审批人MUDID");
                                                                     //}
                                                                     //else if (item.BudgetTotal > 1500 && item.State == "5")
                                                                     //{
                                                                     //    cell = row.CreateCell(++j);
                                                                     //    cell.SetCellValue(item.CurrentApproverName);// "预申请审批人姓名");
                                                                     //    cell = row.CreateCell(++j);
                                                                     //    cell.SetCellValue(item.CurrentApproverMUDID);// "预申请审批人MUDID");
                                                                     //}
                                                                     //else
                                                                     //{
                                                                     //    cell = row.CreateCell(++j);
                                                                     //    cell.SetCellValue(item.BUHeadName);// "预申请审批人姓名");
                                                                     //    cell = row.CreateCell(++j);
                                                                     //    cell.SetCellValue(item.BUHeadMUDID);// "预申请审批人MUDID");

                        //}

                        cell = row.CreateCell(++j);
                        if (item.BUHeadApproveDate != null)
                        {
                            cell.SetCellValue(item.BUHeadApproveDate.Value);// "预申请审批日期");
                        }

                        cell = row.CreateCell(++j);
                        cell.SetCellValue(item.BUHeadApproveDate == null ? "" : item.BUHeadApproveDate.Value.ToString("HH:mm:ss"));// "预申请审批时间");
                        cell = row.CreateCell(++j);
                        var numState = int.Parse(item.State);
                        string strState = "";
                        switch (numState)
                        {
                            case 0:
                                strState = "提交成功";
                                break;
                            case 1:
                                strState = "提交成功";
                                break;
                            case 2:
                                strState = "审批被驳回";
                                break;
                            case 3:
                                strState = "提交成功";
                                break;
                            case 4:
                                strState = "审批被驳回";
                                break;
                            case 5:
                                strState = "审批通过";
                                break;
                            case 6:
                                strState = "审批通过";
                                break;
                            case 7:
                                strState = "提交成功";
                                break;
                            case 8:
                                strState = "审批被驳回";
                                break;
                            case 9:
                                strState = "审批通过";
                                break;
                            case 10:
                                strState = "已取消";
                                break;
                            default:
                                break;
                        }
                        cell.SetCellValue(strState);// "预申请审批状态");
                                                    //cell = row.CreateCell(++j);
                                                    //cell.SetCellValue(item.IsReAssign == true ? "是" : "否");// "预申请是否重新分配审批人");
                                                    //cell = row.CreateCell(++j);
                                                    //cell.SetCellValue(item.ReAssignOperatorName);// "预申请重新分配审批人-操作人");
                                                    //cell = row.CreateCell(++j);
                                                    //cell.SetCellValue(item.ReAssignOperatorMUDID);// "预申请重新分配审批人-操作人MUDID");
                                                    //cell = row.CreateCell(++j);
                                                    //cell.SetCellValue(item.ReAssignBUHeadName);// "预申请被重新分配审批人姓名");
                                                    //cell = row.CreateCell(++j);
                                                    //cell.SetCellValue(item.ReAssignBUHeadMUDID);// "预申请被重新分配审批人MUDID");
                                                    //cell = row.CreateCell(++j);
                                                    //if (item.ReAssignBUHeadApproveDate != null)
                                                    //{
                                                    //    cell.SetCellValue(item.ReAssignBUHeadApproveDate.Value);// "预申请重新分配审批人日期");
                                                    //}
                                                    //cell = row.CreateCell(++j);
                                                    //cell.SetCellValue(item.ReAssignBUHeadApproveDate == null ? "" : item.ReAssignBUHeadApproveDate.Value.ToString("HH:mm:ss"));// "预申请重新分配审批人时间");
                    }

                }
                #endregion

                //FileStream file = new FileStream(filePath, FileMode.Create);
                //workbook.Write(file); file.Close();
                //Response.AppendHeader("Content-Disposition", "attachment;filename=ErrorSongIds.xls");
                //Response.ContentType = "application/ms-excel";
                //Response.WriteFile(filePath);
                //Response.Flush();
                //Response.End();

                #region 将报表上传S3，发送邮件
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {

                    wk.Write(ms);
                    string url = string.Empty;
                    using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, config))
                    {
                        var key = string.Format("SendReport/{0}.xls", "PreApproval_Report_" + DateTime.Now.ToString("yyyyMMddHHmmssfff"));
                        var request = new PutObjectRequest()
                        {
                            BucketName = bucketName,
                            CannedACL = S3CannedACL.PublicRead,
                            Key = key,
                            InputStream = ms
                        };
                        client.PutObject(request);
                        url = "http://wechat.s3.cn-north-1.amazonaws.com.cn/" + key;
                        //url = ConfigurationManager.AppSettings["AWSService"] + "/" + key;
                    }
                    if (SendMail("PreApproval Report", Recipient, CCs, url, "", ms, "Pre-ApprovalReport.xls"))
                        return Json(new { state = 1, txt = "邮件已成功发送！" });
                    else
                        return Json(new { state = 0, txt = "邮件发送失败，请重试！" });
                }
                #endregion

                
            }
            catch(Exception ex)
            {
                LogHelper.Error("预申请报表发送失败---" + ex.Message);
                return Json(new { state = 0, txt = "邮件发送失败，请重试！" });
            }
        }

        public bool SendMail(string Subject, string mailTo, string mailCc, string Body, string FilePath, MemoryStream ms, string attName)
        {
            string FROM = "cn.igsk@gsk.com";
            string FROMNAME = "Catering Service";
            string SMTP_USERNAME = "AKIAIHZIQK74DCRBRYMQ";
            string SMTP_PASSWORD = "Av567RSLSBZUtaDNe/oCeDdDGP/AaYpkSeAt7NtHPFGe";
            string HOST = "email-smtp.us-east-1.amazonaws.com";
            int PORT = 25;

            string SUBJECT = Subject;
            string BODY = "Dear,</br></br>"
                        + "&emsp;&emsp;Download fileptah as follow: " + Body + "</br></br></br>"
                        + "Best Regards</br></br>"
                        + "This is automatically sent by program. Please do not reply.";

            MailMessage message = new MailMessage();
            message.IsBodyHtml = true;
            message.From = new MailAddress(FROM, FROMNAME);
            //收件人
            string[] mailTos = mailTo.Split(';');
            foreach (string To in mailTos)
            {
                if (!string.IsNullOrEmpty(To))
                    message.To.Add(new MailAddress(To));
            }
            //抄送人
            string[] mailCcs = mailCc.Split(';');
            foreach (string Cc in mailCcs)
            {
                if (!string.IsNullOrEmpty(Cc))
                    message.CC.Add(new MailAddress(Cc));
            }
            message.Subject = SUBJECT;
            message.Body = BODY;

            //if (FilePath != "" && FilePath != null)
            //{
            //    message.Attachments.Add(new Attachment(FilePath));
            //}

            //MemoryStream attStream = ms;
            //attStream.Seek(0, SeekOrigin.Begin);
            //Attachment objAttachment = new Attachment(attStream, attName);
            //message.Attachments.Add(objAttachment);

            using (var client = new System.Net.Mail.SmtpClient(HOST, PORT))
            {
                // Pass SMTP credentials
                client.Credentials =
                    new NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);

                // Enable SSL encryption
                client.EnableSsl = true;

                try
                {
                    client.Send(message);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion
    }

}