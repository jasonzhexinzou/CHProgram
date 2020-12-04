using iPathFeast.API.Client;
using MealAdmin.Entity;
using MealAdmin.Entity.Helper;
using MealAdminApiClient;
using MealH5.Areas.P.Filter;
using MealH5.Areas.P.Handler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XFramework.XInject.Attributes;
using XFramework.XUtil;

namespace MealH5.Areas.P.Controllers
{
    [WxUserFilter]
    public class UploadController : BaseController
    {
        [Autowired]
        ApiV1Client apiClient { get; set; }

        #region 加载选择订单界面
        /// <summary>
        /// 加载选择订单界面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [iPathOAuthFilter(MappingKey = "0x0001", CallBackUrl = true)]
        public ActionResult UploadOrder()
        {
            //用户信息
            P_USERINFO userInfo = Session[MealH5.Util.ConstantHelper.CURRENTWXUSER] as P_USERINFO;
            var channel = UploadOrderApiClientChannelFactory.GetChannel();
            var listHTCode = channel.LoadHTCode(userInfo.UserId);
            ViewBag.listHTCode = listHTCode;
            return View(listHTCode);
        }

        #endregion

        #region 加载选择订单详细信息
        /// <summary>
        /// 加载选择订单详细信息
        /// </summary>
        /// <param name="HTCode"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult FindOrderByHTCode(string HTCode)
        {
            var orderClient = UploadOrderApiClientChannelFactory.GetChannel();
            var orderInfo = orderClient.FindOrderByHTCode(HTCode);
            return Json(new { state = 1, data = orderInfo });
        }
        #endregion

        #region 加载上传文件界面
        /// <summary>
        /// 加载上传文件界面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UploadFiles(string htCode, string fileType)
        {
            var channel = UploadOrderApiClientChannelFactory.GetChannel();
            //var listHTCode = channel.LoadHTCode();
            //ViewBag.listHTCode = listHTCode;
            var orderInfo = channel.FindOrderByHTCode(htCode);
            ViewBag.HTCode = htCode;
            ViewBag.Title = "上传文件";
            if (orderInfo.IsRetuen == 3 || orderInfo.IsRetuen == 4 || orderInfo.IsRetuen == 5 || orderInfo.IsRetuen == 6 || orderInfo.IsSpecialOrder == 1)
            {
                ViewBag.Title = "退单原因";
                fileType = "1";
            }
            if (orderInfo.IsSpecialOrder == 2)
            {
                ViewBag.Title = "会议支持文件丢失";
                fileType = "2";
            }
            if (orderInfo.IsSpecialOrder == 3)
            {
                ViewBag.Title = "未送达，会议未正常召开原因";
                fileType = "3";
            }
            ViewBag.FileType = fileType;
            ViewBag.IsOrderUpload = orderInfo.IsOrderUpload;
            return View();
        }
        #endregion

        [HttpGet]
        [iPathOAuthFilter(MappingKey = "0x0001", CallBackUrl = true)]
        public ActionResult TestInfo(string orderId, HttpPostedFileBase file)
        { 
            //var channel = UploadOrderApiClientChannelFactory.GetChannel();
            //ViewBag.ID = orderId;
            //string fileInfo = Request.Form["fileup"];
            var fileName = file.FileName;
            var filePath = Server.MapPath(string.Format("~/{0}", "File"));
            file.SaveAs(Path.Combine(filePath, fileName));
            return View();
        }

        #region 加载提示信息界面
        /// <summary>
        /// 加载提示信息界面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [iPathOAuthFilter(MappingKey = "0x0001", CallBackUrl = true)]
        public ActionResult InformationCue()
        {
            return View();
        }
        #endregion

        #region 加载上传文件状态界面
        /// <summary>
        /// 加载上传文件状态界面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [iPathOAuthFilter(MappingKey = "0x0001", CallBackUrl = true)]
        public ActionResult UploadFileStatus()
        {
            ViewBag.HasApproveRights = HasApproveRights();
            return View();
        }
        #endregion

        #region 加载自动转交订单状态界面
        /// <summary>
        /// 加载上传文件状态界面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [iPathOAuthFilter(MappingKey = "0x0001", CallBackUrl = true)]
        public ActionResult AutoTransferState()
        {
            return View();
        }

        public ActionResult AutoTransferOrderDetails(string HTCode)
        {
            ViewBag.HTCode = HTCode;
            return View();
        }
        #endregion

        #region 上传文件操作

        #region 获取当前订单信息
        /// <summary>
        /// 获取当前订单信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult NowOrder()
        {
            return Json(new { state = 1, data = WeChatOrderInfo });
        }
        #endregion

        #region 加载上传文件详细信息
        /// <summary>
        /// 加载上传文件详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult LoadOrderInfo(Guid id)
        {
            var orderChannel = UploadOrderApiClientChannelFactory.GetChannel();
            var order = orderChannel.LoadOrderInfo(id);
            return Json(new { state = 1, data = order });
        }
        #endregion

        public bool HasApproveRights()
        {
            var channel = PreApprovalClientChannelFactory.GetChannel();
            var res = channel.HasFileApproveRights(CurrentWxUser.UserId);
            return res;
        }

        #region 文件上传审批
        /// <summary>
        /// 文件上传审批
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public JsonResult OrderApprove(Guid id, int state, string reason)
        {
            var uploadChannel = UploadOrderApiClientChannelFactory.GetChannel();
            P_USERINFO userInfo = Session[MealH5.Util.ConstantHelper.CURRENTWXUSER] as P_USERINFO;
            //审批通过
            if (state == 3)
            {
                var res = uploadChannel.BUHeadApprove(id, state, reason);
                var order = uploadChannel.LoadPreUploadOrder(id);

                if (res == 1)
                {
                    P_OrderApproveHistory orderlHistory = new P_OrderApproveHistory()
                    {
                        ID = Guid.NewGuid(),
                        PID = id,
                        UserName = userInfo.Name,
                        UserId = userInfo.UserId,
                        ActionType = state,
                        ApproveDate = DateTime.Now,
                        Comments = reason,
                        type = 2
                    };
                    //添加审批记录
                    var historyRes = uploadChannel.AddOrderApproveHistory(orderlHistory);
                    if (historyRes == 0)
                    {
                        LogHelper.Info($"审批通过，添加审批记录失败 - [{ id}]");
                    }
                    // 发用户消息  修改SendOrderStateChangeMessageToUser
                    try
                    {
                        WxMessageHandler.GetInstance().SendOrderStateChangeMessageToUser(order);
                        string returnMessage = "上传文件已审批通过。";
                        switch (order.FileType)
                        {
                            case 1:
                                returnMessage = "退单原因已审批通过。"; break;
                            case 2:
                                returnMessage = "会议支持文件丢失原因已审批通过。"; break;
                            case 3:
                                returnMessage = "未送达，会议未正常召开原因已审批通过。"; break;
                        }
                        return Json(new { state = 1, txt = returnMessage });
                    }
                    catch (Exception ex)
                    {
                        string returnMessage = "上传文件失败。";
                        LogHelper.Info($"调用SendOrderStateChangeMessageToUser失败-[{ex.Message}]");
                        return Json(new { state = 0, txt = returnMessage });
                    }


                }
                else
                {
                    LogHelper.Info($"（审批通过）文件上传状态修改失败 - [{ id}]");
                }
            }
            else
            {
                var res = uploadChannel.BUHeadReject(id, state, reason);
                var orderInfo = uploadChannel.LoadPreUploadOrder(id);
                if (res == 1)
                {
                    P_OrderApproveHistory orderHistory = new P_OrderApproveHistory()
                    {
                        ID = Guid.NewGuid(),
                        PID = id,
                        UserName = userInfo.Name,
                        UserId = userInfo.UserId,
                        ActionType = state,
                        ApproveDate = DateTime.Now,
                        Comments = reason,
                        type = 2
                    };
                    //添加审批记录
                    var historyRes = uploadChannel.AddOrderApproveHistory(orderHistory);
                    if (historyRes == 0)
                    {
                        LogHelper.Info($"审批驳回，添加审批记录失败 - [{ id}]");
                    }
                    // 发用户消息
                    WxMessageHandler.GetInstance().SendOrderRejectMessageToUser(reason, orderInfo);
                    string returnMessage = "上传文件已审批被驳回。";
                    switch (orderInfo.FileType)
                    {
                        case 1:
                            returnMessage = "退单原因已审批被驳回。"; break;
                        case 2:
                            returnMessage = "会议支持文件丢失原因已审批被驳回。"; break;
                        case 3:
                            returnMessage = "未送达，会议未正常召开原因已审批被驳回。"; break;
                    }
                    return Json(new { state = 1, txt = returnMessage });
                }
                else
                {
                    LogHelper.Info($"（审批驳回）上传文件状态修改失败 - [{ id}]");
                }
            }
            return Json(new { state = 0, txt = "上传文件审批失败，请刷新页面后重试。", errCode = 9007 });
        }
        #endregion

        #region 文件上传审批
        /// <summary>
        /// 文件上传审批
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public JsonResult SaveMMCoEResult(Guid id, int state, string reason)
        {
            var orderChannel = UploadOrderApiClientChannelFactory.GetChannel();
            P_USERINFO userInfo = Session[MealH5.Util.ConstantHelper.CURRENTWXUSER] as P_USERINFO;
            //审批通过
            int pstate = 6;
            if (state == 3)
            {
                var preApproval = orderChannel.LoadPreUploadOrder(id);
                var res = orderChannel.MMCoEApprove(id, pstate, reason);

                if (res == 1)
                {
                    P_OrderApproveHistory orderHistory = new P_OrderApproveHistory()
                    {
                        ID = Guid.NewGuid(),
                        PID = id,
                        UserName = userInfo.Name,
                        UserId = userInfo.UserId,
                        ActionType = state,
                        ApproveDate = DateTime.Now,
                        Comments = reason,
                        type = 1
                    };
                    //添加审批记录
                    var historyRes = orderChannel.AddOrderApproveHistory(orderHistory);
                    if (historyRes == 0)
                    {
                        LogHelper.Info($"审批通过，添加审批记录失败 - [{ id}]");
                    }
                    // 发用户消息
                    WxMessageHandler.GetInstance().SendOrderStateChangeMessageToUser(preApproval);
                    return Json(new { state = 1, txt = "文件上传已审批通过。" });
                }
                else
                {
                    LogHelper.Info($"（审批通过）文件上传状态修改失败 - [{ id}]");
                }
            }
            else
            {
                pstate = 2;
                var res = orderChannel.MMCoEReject(id, pstate, reason);
                var orderInfo = orderChannel.LoadPreUploadOrder(id);
                orderInfo.State = "2";
                if (res == 1)
                {
                    P_OrderApproveHistory orderHistory = new P_OrderApproveHistory()
                    {
                        ID = Guid.NewGuid(),
                        PID = id,
                        UserName = userInfo.Name,
                        UserId = userInfo.UserId,
                        ActionType = state,
                        ApproveDate = DateTime.Now,
                        Comments = reason,
                        type = 1
                    };
                    //添加审批记录
                    var historyRes = orderChannel.AddOrderApproveHistory(orderHistory);
                    if (historyRes == 0)
                    {
                        LogHelper.Info($"审批驳回，添加审批记录失败 - [{ id}]");
                    }
                    // 发用户消息
                    WxMessageHandler.GetInstance().SendOrderRejectMessageToUser(reason, orderInfo);
                    return Json(new { state = 1, txt = "文件上传已审批驳回。" });
                }
                else
                {
                    LogHelper.Info($"（审批驳回）文件上传状态修改失败 - [{ id}]");
                }
            }
            return Json(new { state = 0, txt = "文件上传审批失败，请刷新页面后重试。", errCode = 9007 });
        }
        #endregion

        public JsonResult LoadMyUploadOrder(string end, string state, int year, int month)
        {
            var beginTime = "";
            var endTime = "";
            if (month == 0)
            {
                beginTime = year + "-01-01";
                year = year + 1;
                endTime = year + "-01-01";
            }
            else
            {
                beginTime = year + "-" + month + "-01";
                if (month == 12)
                {
                    year = year + 1;
                    month = 1;
                }
                else
                {
                    month = month + 1;
                }
                endTime = year + "-" + month + "-01";
            }
            var channel = UploadOrderApiClientChannelFactory.GetChannel();
            DateTime _begin = Convert.ToDateTime(beginTime);
            DateTime _end = Convert.ToDateTime(endTime);
            DateTime __end = Convert.ToDateTime(end);
            if (__end >= _begin && __end <= _end)
            {
                _end = __end;
            }
            int rows = 5;
            int page = 1;
            int total = 0;
            var list = channel.LoadMyOrderUserId(CurrentWxUser.UserId, _begin, _end, state, rows, page, out total);
            return Json(new { state = 1, rows = list });
        }

        public JsonResult LoadMyAutoTransfer(string end)
        {
            var channel = UploadOrderApiClientChannelFactory.GetChannel();
            int rows = 5;
            int page = 1;
            int total = 0;
            DateTime _end = Convert.ToDateTime(end);
            var list = channel.LoadMyAutoTransfer(CurrentWxUser.UserId, _end, rows, page, out total);
            return Json(new { state = 1, rows = list });
        }

        public JsonResult LoadMyApprove(string end, string state, string applicant, int year, int month)
        {
            var beginTime = "";
            var endTime = "";
            if (month == 0)
            {
                beginTime = year + "-01-01";
                year = year + 1;
                endTime = year + "-01-01";
            }
            else
            {
                beginTime = year + "-" + month + "-01";
                if (month == 12)
                {
                    year = year + 1;
                    month = 1;
                }
                else
                {
                    month = month + 1;
                }
                endTime = year + "-" + month + "-01";
            }
            var channel = UploadOrderApiClientChannelFactory.GetChannel();
            DateTime _begin = Convert.ToDateTime(beginTime);
            DateTime _end = Convert.ToDateTime(endTime);
            DateTime __end = Convert.ToDateTime(end);
            if (__end >= _begin && __end <= _end)
            {
                _end = __end;
            }
            int rows = 5;
            int page = 1;
            int total = 0;
            var list = channel.LoadMyApprove(CurrentWxUser.UserId, _begin, _end, state, applicant, rows, page, out total);
            return Json(new { state = 1, rows = list });
        }

        public JsonResult ApproveAll(string end, string state, string applicant, int year, int month)
        {
            var beginTime = "";
            var endTime = "";
            if (month == 0)
            {
                beginTime = year + "-01-01";
                year = year + 1;
                endTime = year + "-01-01";
            }
            else
            {
                beginTime = year + "-" + month + "-01";
                if (month == 12)
                {
                    year = year + 1;
                    month = 1;
                }
                else
                {
                    month = month + 1;
                }
                endTime = year + "-" + month + "-01";
            }
            var channel = UploadOrderApiClientChannelFactory.GetChannel();
            P_USERINFO userInfo = Session[MealH5.Util.ConstantHelper.CURRENTWXUSER] as P_USERINFO;
            DateTime _begin = Convert.ToDateTime(beginTime);
            DateTime _end = Convert.ToDateTime(endTime);
            DateTime __end = Convert.ToDateTime(end);
            if (__end >= _begin && __end <= _end)
            {
                _end = __end;
            }
            var list = channel.LoadMyApproveAll(CurrentWxUser.UserId, applicant);
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    var res = channel.BUHeadApprove(item.ID, 3, "");
                    item.State = "5";
                    if (res == 1)
                    {
                        P_OrderApproveHistory orderHistory = new P_OrderApproveHistory()
                        {
                            ID = Guid.NewGuid(),
                            PID = item.ID,
                            UserName = userInfo.Name,
                            UserId = userInfo.UserId,
                            ActionType = 3,
                            ApproveDate = DateTime.Now,
                            Comments = "",
                            type = 2
                        };
                        //添加审批记录
                        var historyRes = channel.AddOrderApproveHistory(orderHistory);
                        if (historyRes == 0)
                        {
                            LogHelper.Info($"审批通过，添加审批记录失败 - [{ item.ID}]");
                        }
                        // 发用户消息
                        WxMessageHandler.GetInstance().SendOrderStateChangeMessageToUser(item);
                    }
                    else
                    {
                        LogHelper.Info($"（审批通过）文件上传状态修改失败 - [{ item.ID}]");
                    }
                }
                return Json(new { state = 1, txt = "文件上传已全部审批通过。" });
            }
            else
            {
                return Json(new { state = 1, txt = "当前没有待审批文件上传数据。" });
            }
        }
        #endregion

        #region 上传文件详情
        /// <summary>
        /// 上传文件详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [iPathOAuthFilter(MappingKey = "0x0001", CallBackUrl = true)]
        public ActionResult Details(Guid id)
        {
            ViewBag.UploadOrderId = id;
            var channel = UploadOrderApiClientChannelFactory.GetChannel();
            var uploadOrder = channel.LoadPreUploadOrder(id);
            ViewBag.FileType = uploadOrder.FileType;
            return View(ViewBag);
        }
        #endregion

        #region 保存上传文件信息
        /// <summary>
        /// 保存上传文件信息
        /// </summary>
        /// <param name="details"></param>
        /// <returns></returns>
        [HttpPost]
        [OrderFilter]
        public JsonResult Details(P_OrderDetails details)
        {
            details.createTime = DateTime.Now;
            WeChatOrderInfo.details = details;
            int needApprove = -1;

            if (!IsNewOrder)
            {
                needApprove = 0;
            }

            return Json(new { state = 1, needApprove = needApprove });
        }
        #endregion

        public JsonResult LoadApproveHistoryInfo(Guid id)
        {
            var orderChannel = UploadOrderApiClientChannelFactory.GetChannel();
            var orderHistory = orderChannel.LoadApproveHistoryInfo(id);
            return Json(new { state = 1, data = orderHistory });
        }

        #region 保存MMCoE申报
        /// <summary>
        /// 保存MMCoE申报
        /// </summary>
        [HttpPost]
        [ActionName("UploadFiles")]
        [OrderFilter]
        public JsonResult _MMCoE(string MMCoEImageOne, string MMCoEImageTwo, string MMCoEImageThree)
        {
            orderUpload.MMCoEImageOne = MMCoEImageOne;
            orderUpload.MMCoEImageTwo = MMCoEImageTwo;
            orderUpload.MMCoEImageThree = MMCoEImageThree;
            return Json(new { state = 1 });
        }
        #endregion

        #region 新增上传文件
        /// <summary>
        /// 新增上传文件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult _Submit(string htCode, string MMCoEImageOne, string MMCoEImageTwo, string MMCoEImageThree, int FileType, int IsAttentSame, string AttentSameReason, string SpecialReason, int IsAddFile, int IsMeetingInfoSame, string MeetingInfoSameReason, string Memo)
        {
            //用户信息
            P_USERINFO userInfo = Session[MealH5.Util.ConstantHelper.CURRENTWXUSER] as P_USERINFO;
            var uploadChannel = UploadOrderApiClientChannelFactory.GetChannel();
            var userInfoChannel = UserInfoClientChannelFactory.GetChannel();
            var orderChannel = OrderApiClientChannelFactory.GetChannel();
            //DM
            var uploadInfo = uploadChannel.FindApproveInfo(userInfo.UserId);


            if (uploadInfo == null || uploadInfo.UserId == null)
            {
                return Json(new { state = 0, txt = "您的直线经理信息有误，请隔日再尝试提交。", errCode = 6666 });
            }

            var uploadInfoDMDelegate = userInfoChannel.isAgent(uploadInfo.UserId);
            //RM
            var uploaderRMInfo = new WP_QYUSER();
            var uploaderRDInfo = new WP_QYUSER();
            P_UserDelegate uploaderRDDelegate = null;

            if (uploadInfo != null && uploadInfo.UserId != null)
            {
                uploaderRMInfo = uploadChannel.FindApproveInfo(uploadInfo.UserId);

                if (uploaderRMInfo != null && uploaderRMInfo.UserId != null)
                {
                    //RD/SD
                    uploaderRDInfo = uploadChannel.FindApproveInfo(uploaderRMInfo.UserId);
                    uploaderRDDelegate = userInfoChannel.isAgent(uploaderRDInfo.UserId);
                }
            }

            var orderInfo = orderChannel.FindOrderByCN(htCode);
            var orderUpload = new P_PREUPLOADORDER();
            orderUpload.ID = Guid.NewGuid();
            orderUpload.ApplierName = orderInfo.IsTransfer == 0 ? userInfo.Name : orderInfo.Consignee;   //登录人姓名
            orderUpload.ApplierMUDID = orderInfo.IsTransfer == 0 ? userInfo.UserId : orderInfo.UserId;   //登录人ID
            orderUpload.CreateDate = DateTime.Now;   //创建日期
            orderUpload.ModifyDate = orderUpload.CreateDate;   //修改日期
            orderUpload.HTCode = htCode;   //HT编号
            orderUpload.BUHeadName = FileType == 2 ? (uploaderRDDelegate == null ? uploaderRDInfo.Name : uploaderRDDelegate.DelegateUserName) : (uploadInfoDMDelegate == null ? uploadInfo.Name : uploadInfoDMDelegate.DelegateUserName);   //审批人姓名
            orderUpload.BUHeadMUDID = FileType == 2 ? (uploaderRDDelegate == null ? uploaderRDInfo.UserId : uploaderRDDelegate.DelegateUserMUDID) : (uploadInfoDMDelegate == null ? uploadInfo.UserId : uploadInfoDMDelegate.DelegateUserMUDID);   //审批人ID
            orderUpload.IsReAssign = false;   //是否重新分配
            orderUpload.State = "1";   //状态
            orderUpload.MMCoEImageOne = MMCoEImageOne;   //上传文件1
            orderUpload.MMCoEImageTwo = MMCoEImageTwo;   //上传文件2
            orderUpload.MMCoEImageThree = MMCoEImageThree;   //上传文件2
            orderUpload.FileType = FileType;
            orderUpload.IsAttentSame = IsAttentSame;
            orderUpload.AttentSameReason = AttentSameReason;
            orderUpload.SpecialReason = SpecialReason;
            orderUpload.IsAddFile = IsAddFile;
            orderUpload.IsMeetingInfoSame = IsMeetingInfoSame;
            orderUpload.MeetingInfoSameReason = MeetingInfoSameReason;
            orderUpload.Memo = Memo;
            if (orderInfo.IsTransfer == 1)
            {
                orderUpload.IsTransfer = orderInfo.IsTransfer;
                orderUpload.TransferUserName = orderInfo.TransferUserName;
                orderUpload.TransferUserMUDID = orderInfo.TransferUserMUDID;
                orderUpload.TransferOperatorName = orderInfo.TransferOperatorName;
                orderUpload.TransferOperatorMUDID = orderInfo.TransferOperatorMUDID;
                orderUpload.TransferOperateDate = orderInfo.TransferOperateDate;
            }
            var res = uploadChannel.Add(orderUpload);
            if (res > 0)
            {
                P_OrderApproveHistory history = new P_OrderApproveHistory()
                {
                    ID = Guid.NewGuid(),
                    PID = orderUpload.ID,
                    UserName = userInfo.Name,
                    UserId = userInfo.UserId,
                    ActionType = 1,
                    ApproveDate = DateTime.Now,
                    type = 2
                };
                uploadChannel.AddOrderApproveHistory(history);
                WxMessageHandler.GetInstance().SendOrderStateChangeMessageToUser(orderUpload);
                return Json(new { state = 1 });
            }
            if (res == 0)
            {
                return Json(new { state = 0, txt = "您的文件已经提交过，请勿重复提交。", errCode = 5555 });
            }
            else
            {
                return Json(new { state = 0 });
            }
        }
        #endregion

        #region 编辑上传文件
        /// <summary>
        /// 编辑上传文件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult _Edit(Guid UploadOrderId, string htCode, string MMCoEImageOne, string MMCoEImageTwo, string MMCoEImageThree, int FileType, int IsAttentSame, string AttentSameReason, string SpecialReason, int IsAddFile, int IsMeetingInfoSame, string MeetingInfoSameReason, string Memo)
        {
            //用户信息
            P_USERINFO userInfo = Session[MealH5.Util.ConstantHelper.CURRENTWXUSER] as P_USERINFO;
            var uploadChannel = UploadOrderApiClientChannelFactory.GetChannel();
            var userInfoChannel = UserInfoClientChannelFactory.GetChannel();
            var orderChannel = OrderApiClientChannelFactory.GetChannel();
            //DM
            var uploadInfo = uploadChannel.FindApproveInfo(userInfo.UserId);

            if (uploadInfo == null || uploadInfo.UserId == null)
            {
                return Json(new { state = 0, txt = "您的直线经理信息有误，请隔日再尝试提交。", errCode = 6666 });
            }

            var uploadInfoDMDelegate = userInfoChannel.isAgent(uploadInfo.UserId);
            //RM
            var uploaderRMInfo = new WP_QYUSER();
            var uploaderRDInfo = new WP_QYUSER();
            P_UserDelegate uploaderRDDelegate = null;

            if (uploadInfo != null && uploadInfo.UserId != null)
            {
                uploaderRMInfo = uploadChannel.FindApproveInfo(uploadInfo.UserId);

                if (uploaderRMInfo != null && uploaderRMInfo.UserId != null)
                {
                    //RD/SD
                    uploaderRDInfo = uploadChannel.FindApproveInfo(uploaderRMInfo.UserId);
                    uploaderRDDelegate = userInfoChannel.isAgent(uploaderRDInfo.UserId);
                }
            }

            var uploadOrderInfo = uploadChannel.LoadPreUploadOrder(UploadOrderId);
            var orderInfo = orderChannel.FindOrderByCN(htCode);
            uploadOrderInfo.ModifyDate = DateTime.Now;   //修改日期
            uploadOrderInfo.State = "1";
            uploadOrderInfo.BUHeadName = FileType == 2 ? (uploaderRDDelegate == null ? uploaderRDInfo.Name : uploaderRDDelegate.DelegateUserName) : (uploadInfoDMDelegate == null ? uploadInfo.Name : uploadInfoDMDelegate.DelegateUserName);   //审批人姓名
            uploadOrderInfo.BUHeadMUDID = FileType == 2 ? (uploaderRDDelegate == null ? uploaderRDInfo.UserId : uploaderRDDelegate.DelegateUserMUDID) : (uploadInfoDMDelegate == null ? uploadInfo.UserId : uploadInfoDMDelegate.DelegateUserMUDID);   //审批人ID
            uploadOrderInfo.MMCoEImageOne = MMCoEImageOne;   //上传文件1
            uploadOrderInfo.MMCoEImageTwo = MMCoEImageTwo;   //上传文件2
            uploadOrderInfo.MMCoEImageThree = MMCoEImageThree;   //上传文件2
            uploadOrderInfo.IsAttentSame = IsAttentSame;
            uploadOrderInfo.AttentSameReason = AttentSameReason;
            uploadOrderInfo.SpecialReason = SpecialReason;
            uploadOrderInfo.IsAddFile = IsAddFile;
            uploadOrderInfo.IsMeetingInfoSame = IsMeetingInfoSame;
            uploadOrderInfo.MeetingInfoSameReason = MeetingInfoSameReason;
            uploadOrderInfo.Memo = Memo;
            if (orderInfo.IsTransfer == 1)
            {
                uploadOrderInfo.IsTransfer = orderInfo.IsTransfer;
                uploadOrderInfo.TransferUserName = orderInfo.TransferUserName;
                uploadOrderInfo.TransferUserMUDID = orderInfo.TransferUserMUDID;
                uploadOrderInfo.TransferOperatorName = orderInfo.TransferOperatorName;
                uploadOrderInfo.TransferOperatorMUDID = orderInfo.TransferOperatorMUDID;
                uploadOrderInfo.TransferOperateDate = orderInfo.TransferOperateDate;
            }
            uploadChannel.Update(uploadOrderInfo);
            P_OrderApproveHistory history = new P_OrderApproveHistory()
            {
                ID = Guid.NewGuid(),
                PID = UploadOrderId,
                UserName = CurrentWxUser.Name,
                UserId = CurrentWxUser.UserId,
                ActionType = 4,
                ApproveDate = DateTime.Now,
                type = 2
            };
            uploadChannel.AddOrderApproveHistory(history);

            WxMessageHandler.GetInstance().SendOrderStateChangeMessageToUser(uploadOrderInfo);
            return Json(new { state = 1 });
        }
        #endregion

        public JsonResult ApproveSelected(List<string> Ids)
        {
            var channel = UploadOrderApiClientChannelFactory.GetChannel();
            P_USERINFO userInfo = Session[MealH5.Util.ConstantHelper.CURRENTWXUSER] as P_USERINFO;
            foreach (var item in Ids)
            {
                var res = channel.BUHeadApprove(Guid.Parse(item), 3, "");
                if (res == 1)
                {
                    P_OrderApproveHistory orderHistory = new P_OrderApproveHistory()
                    {
                        ID = Guid.NewGuid(),
                        PID = Guid.Parse(item),
                        UserName = userInfo.Name,
                        UserId = userInfo.UserId,
                        ActionType = 3,
                        ApproveDate = DateTime.Now,
                        Comments = "",
                        type = 2
                    };
                    //添加审批记录
                    var historyRes = channel.AddOrderApproveHistory(orderHistory);
                    if (historyRes == 0)
                    {
                        LogHelper.Info($"审批通过，添加审批记录失败 - [{ item}]");
                    }
                    // 发用户消息
                    var orderInfo = channel.LoadPreUploadOrder(Guid.Parse(item));
                    WxMessageHandler.GetInstance().SendOrderStateChangeMessageToUser(orderInfo);
                }
                else
                {
                    LogHelper.Info($"（审批通过）上传文件状态修改失败 - [{ item}]");
                }
            }
            return Json(new { state = 1, txt = "上传文件已批量审批通过。" });
        }

        #region 保存上传文件
        /// <summary>
        /// 保存MMCoE申报
        /// </summary>
        /// <param name="images"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("UploadFiles")]
        [OrderFilter]
        public JsonResult _UploadFiles(string images, string images2)
        {
            orderUpload.MMCoEImageOne = images;
            orderUpload.MMCoEImageTwo = images2;
            return Json(new { state = 1 });
        }
        #endregion

        #region 保存选择的订单
        /// <summary>
        /// 保存选择的订单
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveUploadOrder(string OrderId)
        {

            return Json(new { state = 1 });
        }
        #endregion 

        #region 转到上传文件审批页面
        /// <summary>
        /// 转到上传文件审批页面
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpGet]
        [iPathOAuthFilter(MappingKey = "0x0011", CallBackUrl = true)]
        public ActionResult Approval(Guid id, int from)
        {
            ViewBag.From = from;
            ViewBag.UploadOrderId = id;
            var channel = UploadOrderApiClientChannelFactory.GetChannel();
            var uploadOrder = channel.LoadPreUploadOrder(id);
            var orderInfo = channel.FindOrderByHTCode(uploadOrder.HTCode);
            ViewBag.Title = "上传文件审批";
            if (orderInfo.IsRetuen == 3 || orderInfo.IsRetuen == 4 || orderInfo.IsRetuen == 5 || orderInfo.IsRetuen == 6 || orderInfo.IsSpecialOrder == 1)
            {
                ViewBag.Title = "退单原因审批";
            }
            if (orderInfo.IsSpecialOrder == 2)
            {
                ViewBag.Title = "会议支持文件丢失审批";
            }
            if (orderInfo.IsSpecialOrder == 3)
            {
                ViewBag.Title = "未送达，会议未正常召开审批";
            }
            return View();
        }
        #endregion

        #region 编辑提交上传文件信息加载
        /// <summary>
        /// 编辑提交上传文件信息加载
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet]
        [iPathOAuthFilter(MappingKey = "0x0001", CallBackUrl = true)]
        public ActionResult EditUploadFiles(Guid id)
        {
            var channel = UploadOrderApiClientChannelFactory.GetChannel();
            var uploadOrder = channel.LoadPreUploadOrder(id);
            ViewBag.HTCode = uploadOrder.HTCode;
            ViewBag.FileType = uploadOrder.FileType;
            ViewBag.UploadOrderID = id;
            var orderInfo = channel.FindOrderByHTCode(uploadOrder.HTCode);
            ViewBag.Title = "上传文件";
            if (orderInfo.IsRetuen == 3 || orderInfo.IsRetuen == 4 || orderInfo.IsRetuen == 5 || orderInfo.IsRetuen == 6 || orderInfo.IsSpecialOrder == 1)
            {
                ViewBag.Title = "退单原因";
            }
            if (orderInfo.IsSpecialOrder == 2)
            {
                ViewBag.Title = "会议支持文件丢失";
            }
            if (orderInfo.IsSpecialOrder == 3)
            {
                ViewBag.Title = "未送达，会议未正常召开";
            }
            return View(ViewBag);
        }
        #endregion

        #region 加载上传文件详细信息
        /// <summary>
        /// 加载上传文件详细信息
        /// </summary>
        /// <param name="UploadOrderId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult FindUploadOrderByID(Guid UploadOrderId)
        {
            var orderClient = UploadOrderApiClientChannelFactory.GetChannel();
            var uploadOrderInfo = orderClient.LoadPreUploadOrder(UploadOrderId);
            return Json(new { state = 1, data = uploadOrderInfo });
        }
        #endregion
    }
}