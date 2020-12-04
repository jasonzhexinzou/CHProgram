using iPathFeast.API.Client;
using iPathFeast.ApiEntity;
using MealAdmin.Dao;
using MealAdmin.Entity;
using MealAdmin.Service;
using MealAdminApi;
using MealAdminApiClient;
using MeetingMealApiClient;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using XFramework.XInject.Attributes;
using XFramework.XUtil;

namespace MealAdmin.Web.Areas.P.Controllers
{
    public class TaskPlanController : AdminBaseController
    {
        [Bean("hospitalService")]
        public IHospitalService hospitalService { get; set; }
        [Bean("orderService")]
        public IOrderService orderService { get; set; }

        [Bean("groupMemberService")]
        public IGroupMemberService groupMemberService { get; set; }

        [Bean("preApprovalService")]
        public IPreApprovalService preApprovalService { get; set; }

        [Bean("groupMemberDao")]
        public IGroupMemberDao groupMemberDao { get; set; }

        [Bean("userInfoService")]
        public IUserInfoService userInfoService { get; set; }

        [Bean("uploadFileQueryService")]
        public IUploadFileQueryService uploadFileQueryService { get; set; }

        [Bean("uploadOrderService")]
        public IUploadOrderService uploadOrderService { get; set; }

        [Autowired]
        ApiV1Client apiClient { get; set; }

        // GET: P/TaskPlan
        public ActionResult Index()
        {
            return View();
        }

        #region 早九点到晚九点

        #region 送餐时间1小时后未确认收餐(每天)
        /// <summary>
        /// 送餐时间1小时后未确认收餐
        /// </summary>
        /// <returns></returns>
        public JsonResult PushOne()
        {

            var nowDate = DateTime.Now;
            nowDate = nowDate.AddHours(-1);
            var res = orderService.LoadConfirmOrders(nowDate);

            if (res.Count > 0)
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var channel = WxMessageClientChannelFactory.GetChannel();
                        var listTouser = new List<string>();
                        foreach (var item in res)
                        {
                            if (item.IsTransfer == 1)
                            {
                                listTouser.Add(item.TransferUserMUDID);
                            }
                            else
                            {
                                listTouser.Add(item.UserId);
                            }
                        }
                        listTouser = listTouser.Distinct().ToList();
                        channel.SendMessageForConfirm(listTouser, 1);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("送餐时间后1小时推送", ex);
                        throw ex;
                    }
                });

                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var resList = res.Select(a => a.XmsOrderId).ToList();
                        var xmsOrderIds = String.Join("','", resList);
                        orderService.UpdatePushOne(xmsOrderIds);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("更新送餐时间后1小时推送状态", ex);
                        throw ex;
                    }
                });
            }

            return Json(new { state = 1 });
        }
        #endregion

        #region 确认收餐后一小时未上传文件
        /// <summary>
        /// 确认收餐后一小时未上传文件
        /// </summary>
        /// <returns></returns>
        public JsonResult PushEight()
        {
            var nowDate = DateTime.Now;
            nowDate = nowDate.AddHours(-1);
            var res = orderService.LoadUploadFiles(nowDate);

            if (res.Count > 0)
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var channel = WxMessageClientChannelFactory.GetChannel();
                        var listTouser = new List<string>();
                        foreach (var item in res)
                        {
                            if (item.IsTransfer == 1)
                            {
                                listTouser.Add(item.TransferUserMUDID);
                            }
                            else
                            {
                                listTouser.Add(item.UserId);
                            }
                        }
                        listTouser = listTouser.Distinct().ToList();
                        channel.SendMessageForConfirm(listTouser, 8);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("收餐后一小时未上传文件", ex);
                        throw ex;
                    }
                });

                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var resList = res.Select(a => a.XmsOrderId).ToList();
                        var xmsOrderIds = String.Join("','", resList);
                        orderService.UpdatePushTwo(xmsOrderIds);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("收餐后一小时未上传文件，更新推送状态", ex);
                        throw ex;
                    }
                });
            }

            return Json(new { state = 1 });
        }
        #endregion

        #region 系统收餐
        /// <summary>
        /// 系统收餐
        /// </summary>
        /// <returns></returns>
        public JsonResult SysConfirmOrder()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var rtnVal = orderService.LoadOrders();
                    LogHelper.Info("auto sync SysConfirmOrder result:" + rtnVal);
                    //通知供应商确认收餐
                    foreach (var item in rtnVal)
                    {
                        var req = new FinishOrderReq()
                        {
                            _Channels = item.Channel,
                            iPathOrderId = item.XmsOrderId,
                            type = "1",
                            remark = string.Empty
                        };
                        apiClient.FinishOrder(req);
                        orderService.SystemConfirm(item.ID);
                        groupMemberService.UpdateServPauseDetail(item.CN, 1);
                    }
                }

                catch (Exception ex)
                {

                    LogHelper.Error("auto sync SysConfirmOrder  ERR", ex);
                    throw ex;
                }
            });
            return Json(new { state = 1 });
        }
        #endregion

        #endregion

        #region 每天晚10点

        #region 确认收餐
        /// <summary>
        /// 确认收餐
        /// </summary>
        /// <returns></returns>
        public JsonResult PushTwo()
        {
            var res = orderService.LoadConfirmOrders();
            if (res.Count > 0)
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var channel = WxMessageClientChannelFactory.GetChannel();
                        var listTouser = new List<string>();
                        foreach (var item in res)
                        {
                            if (item.IsTransfer == 1)
                            {
                                listTouser.Add(item.TransferUserMUDID);
                            }
                            else
                            {
                                listTouser.Add(item.UserId);
                            }
                        }
                        listTouser = listTouser.Distinct().ToList();
                        channel.SendMessageForConfirm(listTouser, 2);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("晚十点，确认收餐推送", ex);
                        throw ex;
                    }
                });
            }

            return Json(new { state = 1 });
        }
        #endregion

        #region 上传文件
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>
        public JsonResult PushThree()
        {
            var res = orderService.LoadUploadOrders();
            if (res.Count > 0)
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var channel = WxMessageClientChannelFactory.GetChannel();
                        var listTouser = new List<string>();
                        foreach (var item in res)
                        {
                            if (item.IsTransfer == 1)
                            {
                                listTouser.Add(item.TransferUserMUDID);
                            }
                            else
                            {
                                listTouser.Add(item.UserId);
                            }
                        }
                        listTouser = listTouser.Distinct().ToList();
                        channel.SendMessageForConfirm(listTouser, 3);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("晚十点，上传文件推送", ex);
                        throw ex;
                    }
                });
            }

            return Json(new { state = 1 });
        }
        #endregion

        #region 上传文件审批
        /// <summary>
        /// 上传文件审批
        /// </summary>
        /// <returns></returns>
        public JsonResult PushFour()
        {
            var res = orderService.LoadUploadFailOrders();
            if (res.Count > 0)
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var channel = WxMessageClientChannelFactory.GetChannel();
                        var listTouser = new List<string>();
                        foreach (var item in res)
                        {
                            if (item.IsTransfer == 1)
                            {
                                listTouser.Add(item.TransferUserMUDID);
                            }
                            else
                            {
                                listTouser.Add(item.UserId);
                            }
                        }
                        listTouser = listTouser.Distinct().ToList();
                        channel.SendMessageForConfirm(listTouser, 4);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("晚十点，上传文件审批推送", ex);
                        throw ex;
                    }
                });
            }

            return Json(new { state = 1 });
        }
        #endregion

        #endregion

        #region 每天晚6点

        #region 48小时未收餐
        /// <summary>
        /// 48小时未收餐
        /// </summary>
        /// <returns></returns>
        public JsonResult PushFive()
        {

            var nowDate = DateTime.Now;
            nowDate = nowDate.AddHours(-48);
            var res = orderService.LoadOrderConfirms(nowDate);

            if (res.Count > 0)
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var channel = WxMessageClientChannelFactory.GetChannel();
                        var listTouser = new List<string>();
                        foreach (var item in res)
                        {
                            if (item.IsTransfer == 1)
                            {
                                listTouser.Add(item.TransferUserMUDID);
                            }
                            else
                            {
                                listTouser.Add(item.UserId);
                            }
                        }
                        listTouser = listTouser.Distinct().ToList();
                        channel.SendMessageForConfirm(listTouser, 5);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("送餐时间后48小时没收餐", ex);
                        throw ex;
                    }
                });

                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var aryIds = new List<string>();
                        foreach (var item in res)
                        {
                            if (item.IsTransfer == 1)
                            {
                                aryIds.Add(item.TransferUserMUDID);
                                groupMemberService.AddServPauseDetail(item.TransferUserMUDID, item.TransferUserName, item.CN, 1, "未确认收餐");
                            }
                            else
                            {
                                aryIds.Add(item.UserId);
                                groupMemberService.AddServPauseDetail(item.UserId, item.Consignee, item.CN, 1, "未确认收餐");
                            }
                        }
                        aryIds = aryIds.Distinct().ToList();
                        var updCnt = groupMemberService.AddUser(aryIds, 1);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("送餐时间后48小时没收餐，暂停服务", ex);
                        throw ex;
                    }
                });
            }

            return Json(new { state = 1 });
        }
        #endregion

        #region 7天未上传文件
        /// <summary>
        /// 7天未上传文件
        /// </summary>
        /// <returns></returns>
        public JsonResult PushSix()
        {

            var nowDate = DateTime.Now;
            nowDate = nowDate.AddHours(-168);
            var res = orderService.LoadOrderUpload(nowDate);

            if (res.Count > 0)
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var channel = WxMessageClientChannelFactory.GetChannel();
                        var listTouser = new List<string>();
                        foreach (var item in res)
                        {
                            if (item.IsTransfer == 1)
                            {
                                listTouser.Add(item.TransferUserMUDID);
                            }
                            else
                            {
                                listTouser.Add(item.UserId);
                            }
                        }
                        listTouser = listTouser.Distinct().ToList();
                        channel.SendMessageForConfirm(listTouser, 6);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("收餐后7天未上传文件", ex);
                        throw ex;
                    }
                });

                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var aryIds = new List<string>();
                        foreach (var item in res)
                        {
                            if (item.IsTransfer == 1)
                            {
                                aryIds.Add(item.TransferUserMUDID);
                                groupMemberService.AddServPauseDetail(item.TransferUserMUDID, item.TransferUserName, item.CN, 2, "确认收餐后未上传文件");
                            }
                            else
                            {
                                aryIds.Add(item.UserId);
                                groupMemberService.AddServPauseDetail(item.UserId, item.Consignee, item.CN, 2, "确认收餐后未上传文件");
                            }
                        }
                        aryIds = aryIds.Distinct().ToList();
                        var updCnt = groupMemberService.AddUser(aryIds, 2);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("收餐后7天未上传文件，暂停服务", ex);
                        throw ex;
                    }
                });

            }

            return Json(new { state = 1 });
        }
        #endregion

        #region 7天未审批通过
        /// <summary>
        /// 7天未审批通过
        /// </summary>
        /// <returns></returns>
        public JsonResult PushSeven()
        {
            var nowDate = DateTime.Now;
            nowDate = nowDate.AddHours(-168);
            var res = orderService.LoadFailOrder(nowDate);
            var uploadRes = orderService.LoadFailUploadOrder(nowDate);
            if (res.Count > 0)
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var channel = WxMessageClientChannelFactory.GetChannel();
                        var listTouser = new List<string>();
                        foreach (var item in res)
                        {
                            if (item.IsTransfer == 1)
                            {
                                listTouser.Add(item.TransferUserMUDID);
                            }
                            else
                            {
                                listTouser.Add(item.UserId);
                            }
                        }
                        listTouser = listTouser.Distinct().ToList();
                        channel.SendMessageForConfirm(listTouser, 7);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("上传文件后7天未通过审批", ex);
                        throw ex;
                    }
                });

                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var aryIds = new List<string>();
                        foreach (var item in res)
                        {
                            var uploadOrder = uploadRes.Where(p => p.HTCode.Trim() == item.CN.Trim()).FirstOrDefault();
                            if (item.IsTransfer == 1)
                            {
                                aryIds.Add(item.TransferUserMUDID);
                                groupMemberService.AddServPauseDetail(item.TransferUserMUDID, item.TransferUserName, item.CN, uploadOrder.IsReopen == 1 ? 4 : 3, uploadOrder.IsReopen == 1 ? "Reopen后未重新上传文件" : "上传文件未审批");
                            }
                            else
                            {
                                aryIds.Add(item.UserId);
                                groupMemberService.AddServPauseDetail(item.UserId, item.Consignee, item.CN, uploadOrder.IsReopen == 1 ? 4 : 3, uploadOrder.IsReopen == 1 ? "Reopen后未重新上传文件" : "上传文件未审批");
                            }
                        }
                        aryIds = aryIds.Distinct().ToList();
                        var updCnt = groupMemberService.AddUser(aryIds, 3);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("上传文件后7天未通过审批，暂停服务", ex);
                        throw ex;
                    }
                });

            }

            return Json(new { state = 1 });
        }
        #endregion

        #endregion

        #region 简报发送
        /// <summary>
        /// 简报发送
        /// </summary>
        /// <returns></returns>
        public JsonResult SendBriefReport()
        {
            var arr = groupMemberService.GetGroupMembersByType(Entity.GroupTypeEnum.BriefReport).Select(s => s.UserId).ToArray();
            var touser = string.Join("|", arr);
            var brief = orderService.LoadBriefing(0);
            var cnt = "当日预申请审批通过数量:" + brief.TodayApprove + "(0元-" + brief.TodayApproveZero + "; 非0元-" + brief.TodayApproveNotZero + ")"
                + "\r\n明日配送订单量: " + brief.TomorrowDeliver + "(XMS-" + brief.TomorrowDeliverXms + ";\t BDS-" + brief.TomorrowDeliverBds + ")"
                + "\r\n明日配送订单金额: RMB" + brief.TomorrowDeliverTotal.ToString("n") + "(XMS- RMB" + brief.TomorrowDeliverTotalXms.ToString("n") + ";\t BDS- RMB" + brief.TomorrowDeliverTotalBds.ToString("n") + ")"
                + "\r\n明日配送订单,参会人数>=60: " + brief.TomorrowAttendCount60
                + "\r\n明日配送订单,预定金额>=1500元: " + brief.TomorrowExceed2000
                + "\r\n当日确认收餐数量:" + brief.TodayConfirmOrder
                + "\r\n当日上传文件审批通过数量:" + brief.TodayUpLoadThroughCount;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var rtnVal = WxMessageClientChannelFactory.GetChannel().SendText(touser, cnt);
                    LogHelper.Info("auto send BriefReport Content：【" + JsonConvert.SerializeObject(brief) + "】, | toUser: 【" + touser + "】，| result:" + rtnVal);
                }
                catch (Exception ex)
                {
                    LogHelper.Error("auto send BriefReport  ERR", ex);
                    throw ex;
                }
            });

            return Json(new { state = 1 });
        }
        #endregion


        #region 未在送餐时间范围内返回预定状态、改单状态、按原订单配送状态
        /// <summary>
        /// 未在送餐时间范围内返回预定状态、改单状态、按原订单配送状态
        /// </summary>
        /// <returns></returns>
        public JsonResult LoadAutoChangeFail()
        {

            var res = orderService.LoadAutoChangeFail();
            var channel = OrderApiClientChannelFactory.GetChannel();

            if (res.Count > 0)
            {
                foreach (var order in res)
                {
                    if (order.State == 3)
                    {
                        //预定状态未返回，修改状态未返回
                        Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                //预定失败
                                var oldXmsOrderId = string.Empty;
                                var remark = "供应商超时未返回，系统默认预订失败";
                                if (order.IsChange != 0)
                                {
                                    oldXmsOrderId = order.XmsOrderId;
                                    remark = "供应商超时未返回，系统默认修改失败";
                                }
                                var i = channel.ScheduledFail(order.XmsOrderId, remark, oldXmsOrderId);
                                if (i > 0)
                                {
                                    var _order = channel.FindByXmlOrderId(order.XmsOrderId);
                                    WxMessageClientChannelFactory.GetChannel().SendMessageToUser(_order.UserId, _order);
                                }
                            }
                            catch
                            {
                                LogHelper.Info("未在送餐时间范围内返回预定状态或修改状态,HTCode:" + order.CN);
                            }
                        });
                    }
                    else
                    {
                        //按原订单配送未返回
                        Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                var i = channel.OriginalOrderSendFail(order.XmsOrderId, "供应商超时未返回，系统默认按原订单配送请求失败");
                                if (i > 0)
                                {
                                    var _order = channel.FindByXmlOrderId(order.XmsOrderId);
                                    WxMessageClientChannelFactory.GetChannel().SendMessageToUser(_order.UserId, _order);
                                }
                            }
                            catch
                            {
                                LogHelper.Info("未在送餐时间范围内返回按原订单配送状态,HTCode:" + order.CN);
                            }
                        });
                    }
                }
            }

            return Json(new { state = 1 });
        }
        #endregion

        #region 发起退订的订单，未在点击退订18小时内返回退订状态
        /// <summary>
        /// 发起退订的订单，未在点击退订18小时内返回退订状态
        /// </summary>
        /// <returns></returns>
        public JsonResult LoadAutoChangeSuccess()
        {

            var res = orderService.LoadAutoChangeSuccess();
            var channel = OrderApiClientChannelFactory.GetChannel();
            if (res.Count > 0)
            {
                foreach (var order in res)
                {
                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            var i = channel.CancelOrderSuccess(order.XmsOrderId);
                            if (i > 0)
                            {
                                var _order = channel.FindByXmlOrderId(order.XmsOrderId);
                                WxMessageClientChannelFactory.GetChannel().SendMessageToUser(_order.UserId, _order);
                            }
                        }
                        catch
                        {
                            LogHelper.Info("未在退订时间18小时内返回退订状态,HTCode:" + order.CN);
                        }
                    });
                }

            }
            return Json(new { state = 1 });
        }
        #endregion

        #region 自动将离职员工的订单转交给其直线经理
        public void AutoTransfer()
        {
            var autoTransferList = userInfoService.LoadLeaveUserInfo();
            var leaveUserList = autoTransferList.Select(p => new { UserId = p.UserId, UserName = p.UserName, LineManagerId = p.LineManagerId }).Distinct();
            string htList = string.Empty;
            string htList1 = string.Empty;
            string htList2 = string.Empty;
            int htCount = 0;
            int htCount1 = 0;
            int htCount2 = 0;
            foreach (var user in leaveUserList)
            {
                //var Type = 1;
                htList = string.Empty;
                htList1 = string.Empty;
                htList2 = string.Empty;
                htCount = 0;
                htCount1 = 0;
                htCount2 = 0;
                List<string> listToUser = new List<string>();
                var lineManager = userInfoService.FindUserManagerInfo(user.LineManagerId);
                var managerManager = userInfoService.FindUserManagerInfo(lineManager.LineManagerId);
                listToUser.Add(lineManager.LineManagerId);
                var userAutoTransferList = autoTransferList.Where(p => p.UserId == user.UserId).ToList();
                foreach (var item in userAutoTransferList)
                {
                    if (item.IsOrderUpload == 1)
                    {
                        //转交上传文件
                        var transOrderResult = orderService.AutoTransferOrder(item.HTCode, lineManager.LineManagerId, lineManager.LineManagerName);
                        var transUploadResult = orderService.AutoTransferUpload(item.HTCode, lineManager.LineManagerId, lineManager.LineManagerName);
                        if (transUploadResult > 0 && transOrderResult > 0)
                        {
                            if (item.IsOrderUpload == 0)
                            {
                                htList += item.HTCode + "，";
                                htCount++;
                            }
                            else
                            {
                                if (item.UploadState == "1")
                                {
                                    htList2 += item.HTCode + "，";
                                    htCount2++;
                                }
                                else if (item.UploadState == "2" || item.UploadState == "3")
                                {
                                    htList1 += item.HTCode + "，";
                                    htCount1++;
                                }
                            }
                            //将上传文件转给DM的DM
                            if (preApprovalService.UpdatePuoReAssignByHTCode(item.HTCode, "user01.admin", "System", managerManager.LineManagerId, managerManager.LineManagerName) > 0)
                            {
                                var uploadFile = uploadFileQueryService.FindPreUploadFileByHTCode(item.HTCode);
                                if (uploadFile != null)
                                {
                                    var messageBase = "该订单已上传会议支持文件";
                                    switch (uploadFile.FileType)
                                    {
                                        case 1:
                                            messageBase = "该订单已提交退单原因";
                                            break;
                                        case 2:
                                            messageBase = "该订单已提交会议支持文件丢失原因";
                                            break;
                                        case 3:
                                            messageBase = "该订单已提交未送达，会议未正常召开原因";
                                            break;
                                    }
                                    var approverMsg = $"{uploadFile.HTCode}，{messageBase}，请<a href='{ConfigurationManager.AppSettings["MealH5SiteUrl"]}/P/Upload/Approval?id={uploadFile.ID}&from=0'>点击这里</a>进行审批。";
                                    var rtnVal = WxMessageClientChannelFactory.GetChannel().SendText(managerManager.LineManagerId, approverMsg);
                                }
                            }
                            orderService.AddAutoTransferHistory(item.HTCode, user.UserId, lineManager.LineManagerId, 2);
                        }
                    }
                    else
                    {
                        //转交订单
                        var transOrderResult = orderService.AutoTransferOrder(item.HTCode, lineManager.LineManagerId, lineManager.LineManagerName);
                        if (transOrderResult > 0)
                        {
                            if (item.IsOrderUpload == 0)
                            {
                                htList += item.HTCode + "，";
                                htCount++;
                            }
                            else
                            {
                                if (item.UploadState == "1")
                                {
                                    htList2 += item.HTCode + "，";
                                    htCount2++;
                                }
                                else if (item.UploadState == "2" || item.UploadState == "3")
                                {
                                    htList1 += item.HTCode + "，";
                                    htCount1++;
                                }
                            }
                            orderService.AddAutoTransferHistory(item.HTCode, user.UserId, lineManager.LineManagerId, 1);
                        }
                    }
                    // Start UpdateBy zhexin.zou at 20190104
                    //if(item.IsOrderUpload == 0)
                    //{
                    //    Type = 1;
                    //}
                    //else
                    //{
                    //    if(item.UploadState == "1")
                    //    {
                    //        Type = 3;
                    //    }
                    //    else if(item.UploadState == "2" || item.UploadState == "3")
                    //    {
                    //        Type = 2;
                    //    }
                    //}
                    // End UpdateBy zhexin.zou at 20190104
                }
                if (htList.Length > 1)
                {
                    htList = htList.Substring(0, htList.Length - 1);
                    // Start UpdateBy zhexin.zou at 20190104
                    WxMessageClientChannelFactory.GetChannel().SendMessageForAutoTransfer(listToUser, htList, user.UserName, user.UserId, htCount, 1);
                    // End UpdateBy zhexin.zou at 20190104
                }
                if (htList1.Length > 1)
                {
                    htList1 = htList1.Substring(0, htList1.Length - 1);
                    // Start UpdateBy zhexin.zou at 20190104
                    WxMessageClientChannelFactory.GetChannel().SendMessageForAutoTransfer(listToUser, htList1, user.UserName, user.UserId, htCount1, 2);
                }
                if (htList2.Length > 1)
                {
                    htList2 = htList2.Substring(0, htList2.Length - 1);
                    // Start UpdateBy zhexin.zou at 20190104
                    WxMessageClientChannelFactory.GetChannel().SendMessageForAutoTransfer(listToUser, htList2, user.UserName, user.UserId, htCount2, 3);
                }
            }

            //AutoTransferPre();
        }
        #endregion

        #region 自动将离职员工的订单转交给其直线经理
        public void AutoTransferPre()
        {
            string htList = string.Empty;
            var autoTransferList = preApprovalService.GetPreApprovalByUser();
            var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
            var userInfoChannel = UserInfoClientChannelFactory.GetChannel();
            var preApprovalChannel = PreApprovalClientChannelFactory.GetChannel();
            int htCount = 0;
            var leaveUserList = autoTransferList.Select(p => new { UserId = p.CurrentApproverMUDID, UserName = p.CurrentApproverName }).Distinct();
            foreach (var user in leaveUserList)
            {
                var userAutoTransferList = autoTransferList.Where(p => p.CurrentApproverMUDID == user.UserId).ToList();
                List<string> listToUser = new List<string>();
                var Pre = baseDataChannel.GetNameUserId(user.UserId);
                var delegateMUDID = userInfoChannel.isAgent(Pre.CurrentApproverMUDID);
                listToUser.Add(delegateMUDID == null ? Pre.CurrentApproverMUDID : delegateMUDID.DelegateUserMUDID);
                foreach (var list in autoTransferList)
                {

                    bool flag = preApprovalChannel.HasApprove(list.CurrentApproverMUDID);
                    if (list.BudgetTotal >= 1200 && list.BudgetTotal < 1500)
                    {
                        list.State = "7";
                        list.CurrentApproverMUDID = Pre.CurrentApproverMUDID;
                        list.CurrentApproverName = Pre.CurrentApproverName;
                        //PreApproval.CurrentApproverMUDID = Pre2.CurrentApproverMUDID;
                        //PreApproval.CurrentApproverName = Pre2.CurrentApproverName;

                    }
                    else if (list.BudgetTotal >= 1500)
                    {
                        list.State = "3";
                        list.CurrentApproverMUDID = Pre.CurrentApproverMUDID;
                        list.CurrentApproverName = Pre.CurrentApproverName;
                        //PreApproval.CurrentApproverMUDID = Pre.CurrentApproverMUDID;
                        list.IsOnc = 1;
                        //PreApproval.CurrentApproverName = Pre.CurrentApproverName;

                    }

                    baseDataChannel.UpdateCurrentPreApprova(list);
                    htList += list.HTCode + "，";
                    htCount++;
                }

                if (htList.Length > 1)
                {
                    htList = htList.Substring(0, htList.Length - 1);
                    // Start UpdateBy zhexin.zou at 20190104
                    WxMessageClientChannelFactory.GetChannel().SendMessageForAutoTransferPre(listToUser, htList, user.UserName, user.UserId, htCount);
                    // End UpdateBy zhexin.zou at 20190104
                }
            }

        }
        #endregion

        public JsonResult Test()
        {
            var brief = orderService.LoadBriefing(0);
            var cnt = "明日配送订单量: " + brief.TomorrowDeliver + "(MXS-" + brief.TomorrowDeliverXms + "; BDS-" + brief.TomorrowDeliverBds + ")"
                   + "\r\n明日配送订单总额: RMB" + brief.TomorrowDeliverTotal.ToString("n") + "(XMS- RMB" + brief.TomorrowDeliverTotalXms.ToString("n") + "; BDS- RMB" + brief.TomorrowDeliverTotalBds.ToString("n") + ")"
                   + "\r\n当日订单预订失败量: " + brief.TodayFail + "(XMS-" + brief.TodayFailXms + "; BDS-" + brief.TodayFailBds + ")"
                   + "\r\n当日退单成功量: " + brief.TodayCancelSuccess + "(XMS-" + brief.TodayCancelSuccessXms + "; BDS-" + brief.TodayCancelSuccessBds + ")"
                   + "\r\n当日退单失败量: " + brief.TodayCancelFail + "XMS-" + brief.TodayCancelFailXms + ";BDS" + brief.TodayCancelFailBds + ")"
                   + "\r\n明日配送订单:参会人数>=60, " + brief.TomorrowAttendCount60
                   + "\r\n明日配送订单:参会人数<60, 订单份数>=60, " + brief.TomorrowFoodCount60;
            var _brief = orderService.LoadBriefing(1);
            var _cnt = "明日配送订单量: " + brief.TomorrowDeliver + "(MXS-" + brief.TomorrowDeliverXms + ";  BDS-" + brief.TomorrowDeliverBds + ")"
                    + "\r\n明日配送订单总额: RMB" + brief.TomorrowDeliverTotal.ToString("n") + "(XMS- RMB" + brief.TomorrowDeliverTotalXms.ToString("n") + ";  BDS- RMB" + brief.TomorrowDeliverTotalBds.ToString("n") + ")"
                    + "\r\n当日订单预订失败量: " + brief.TodayFail + "(XMS-" + brief.TodayFailXms + "; BDS-" + brief.TodayFailBds + ")"
                    + "\r\n当日退单成功量: " + brief.TodayCancelSuccess + "(XMS-" + brief.TodayCancelSuccessXms + "; BDS-" + brief.TodayCancelSuccessBds + ")"
                    + "\r\n当日退单失败量: " + brief.TodayCancelFail + "XMS-" + brief.TodayCancelFailXms + ";BDS" + brief.TodayCancelFailBds + ")"
                    + "\r\n明日配送订单:参会人数>=60, " + _brief.TomorrowAttendCount60
                    + "\r\n明日配送订单:参会人数<60, 订单份数>=60, " + _brief.TomorrowFoodCount60;

            return Json(new { state = 1 });
        }


        #region 更新Rx数据
        /// <summary>
        /// 更新Rx数据
        /// </summary>
        /// <returns></returns>
        public JsonResult ReadRxTemp()
        {
            //获取Rx临时表数据
            var res = hospitalService.LoadRxTemp();
            if (res != null && res.Count > 0)
            {
                try
                {
                    //删除主数据表中Rx的主地址数据以及Detail表关联数据
                    hospitalService.DeleteMainAddressData("Rx");
                    //更新有效的门地址IsDelete=1，不更新原始数据中IsDelete=2或3的数据
                    hospitalService.UpdateIsDelete("Rx");
                    //更新院外IsDelete=1
                    hospitalService.UpdateOHIsDelete("Rx");
                    //将Rx临时表数据插入主数据表及Detail表
                    hospitalService.InsertRxData(res);
                    //将医院代码匹配的原门地址数据从IsDelete=1更改为IsDelete=0，不更新原始数据中IsDelete=2或3的数据
                    hospitalService.UpdateRxStatus("Rx");
                    //查询该Market的院外数据
                    var OHres = hospitalService.GetOHData("Rx");
                    if (OHres != null && OHres.Count > 0)
                    {
                        var sus = new List<P_HOSPITAL>();
                        foreach (var s in OHres)
                        {
                            string GskHospitalCode = s.GskHospital.Substring(0, s.GskHospital.Length - 3);
                            var Hres = hospitalService.GetHData(GskHospitalCode, "Rx");
                            if (Hres != null && Hres.Count > 0)
                            {
                                string OHName = "院外-" + Hres[0].Name;
                                sus.Add(new P_HOSPITAL()
                                {
                                    GskHospital = s.GskHospital,
                                    CityId = Hres[0].CityId,
                                    Name = OHName,
                                    FirstLetters = NPinyin.Pinyin.GetInitials(OHName).ToLower(),
                                    ProvinceId = Hres[0].ProvinceId,
                                    IsDelete = 0,

                                });
                            }
                        }
                        //更新院外数据与现有主数据匹配
                        hospitalService.UpdateOHData(sus);
                    }
                    //根据变量表更新门地址状态
                    //获取变量表中Rx且ACTION=6的主地址数据
                    var HosVar = hospitalService.GetHosVariables("Rx", "6");
                    if (HosVar != null && HosVar.Count > 0)
                    {
                        //主数据表中是否有与变量表删除主数据匹配的门地址
                        var notmain = hospitalService.Getnotmain("Rx", "6", "Rx");
                        if (notmain != null && notmain.Count > 0)
                        {
                            //更新失效门地址状态为IsDelete=2
                            hospitalService.Updatenotmain(notmain, HosVar);
                        }
                    }
                    //获取变量表中Rx且ACTION=5的主地址数据
                    var AHosVar = hospitalService.GetHosVariables("Rx", "5");
                    if (AHosVar != null && AHosVar.Count > 0)
                    {
                        //主数据表中是否有与变量表地址变更主数据匹配的门地址
                        var Anotmain = hospitalService.Getnotmain("Rx", "5", "Rx");
                        if (Anotmain != null && Anotmain.Count > 0)
                        {
                            //更新主地址地址变更的门地址状态为IsDelete=3
                            hospitalService.UpdateAnotmain(Anotmain, AHosVar);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error("更新主数据中Rx部分数据", ex);
                    throw ex;
                }
            }
            return Json(new { state = 1 });
        }
        #endregion

        private string GetPinYin(string txt)
        {
            var py = $";{NPinyin.Pinyin.GetPinyin(txt).Replace(" ", "")};{NPinyin.Pinyin.GetInitials(txt).ToLower()}";
            return py;
        }

        #region 更新Vx数据
        /// <summary>
        /// 更新Vx数据
        /// </summary>
        /// <returns></returns>
        public JsonResult ReadVxTemp()
        {
            //获取Vx临时表数据
            var res = hospitalService.LoadVxTemp();
            if (res != null && res.Count > 0)
            {
                try
                {
                    //删除主数据表中Vx的主地址数据以及Detail表关联数据
                    hospitalService.DeleteMainAddressData("Vx");
                    //更新有效的门地址IsDelete=1，不更新原始数据中IsDelete=2或3的数据
                    hospitalService.UpdateIsDelete("Vx");
                    //更新院外IsDelete=1
                    hospitalService.UpdateOHIsDelete("Vx");
                    //将Vx临时表数据插入主数据表及Detail表
                    hospitalService.InsertVxData(res);
                    //将医院代码匹配的原门地址数据从IsDelete=1更改为IsDelete=0，不更新原始数据中IsDelete=2或3的数据
                    hospitalService.UpdateVxStatus("Vx");
                    //更新院外数据与现有主数据匹配
                    var OHres = hospitalService.GetOHData("Vx");
                    if (OHres != null && OHres.Count > 0)
                    {
                        var sus = new List<P_HOSPITAL>();
                        foreach (var s in OHres)
                        {
                            string GskHospitalCode = s.GskHospital.Substring(0, s.GskHospital.Length - 3);
                            var Hres = hospitalService.GetHData(GskHospitalCode, "Vx");
                            if (Hres != null && Hres.Count > 0)
                            {
                                string OHName = "院外-" + Hres[0].Name;
                                sus.Add(new P_HOSPITAL()
                                {
                                    GskHospital = s.GskHospital,
                                    CityId = Hres[0].CityId,
                                    Name = OHName,
                                    FirstLetters = NPinyin.Pinyin.GetInitials(OHName).ToLower(),
                                    ProvinceId = Hres[0].ProvinceId,
                                    IsDelete = 0,

                                });
                            }
                        }
                        hospitalService.UpdateOHData(sus);
                    }
                    //根据变量表更新门地址状态
                    //获取变量表中Vx且ACTION=6的主地址数据
                    var HosVar = hospitalService.GetHosVariables("Vx", "6");
                    if (HosVar != null && HosVar.Count > 0)
                    {
                        //主数据表中是否有与变量表删除主数据匹配的门地址
                        var notmain = hospitalService.Getnotmain("Vx", "6", "Vx");
                        if (notmain != null && notmain.Count > 0)
                        {
                            //更新失效门地址状态为IsDelete=2
                            hospitalService.Updatenotmain(notmain, HosVar);
                        }
                    }
                    //获取变量表中Vx且ACTION=5的主地址数据
                    var AHosVar = hospitalService.GetHosVariables("Vx", "5");
                    if (AHosVar != null && AHosVar.Count > 0)
                    {
                        //主数据表中是否有与变量表地址变更主数据匹配的门地址
                        var Anotmain = hospitalService.Getnotmain("Vx", "5", "Vx");
                        if (Anotmain != null && Anotmain.Count > 0)
                        {
                            //更新主地址地址变更的门地址状态为IsDelete=3
                            hospitalService.UpdateAnotmain(Anotmain, AHosVar);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error("更新主数据中Vx部分数据", ex);
                    throw ex;
                }
            }
            return Json(new { state = 1 });
        }
        #endregion

        #region 更新DDT数据
        /// <summary>
        /// 更新DDT数据
        /// </summary>
        /// <returns></returns>
        public JsonResult ReadDDTTemp()
        {
            //获取DDT临时表数据
            var res = hospitalService.LoadDDTTemp();
            if (res != null && res.Count > 0)
            {
                try
                {
                    //删除主数据表中Vx的主地址数据以及Detail表关联数据
                    hospitalService.DeleteMainAddressData("DDT");
                    //更新有效的门地址IsDelete=1，不更新原始数据中IsDelete=2或3的数据
                    hospitalService.UpdateIsDelete("DDT");
                    //更新院外IsDelete=1
                    hospitalService.UpdateOHIsDelete("DDT");
                    //将DDT临时表数据插入主数据表及Detail表
                    hospitalService.InsertDDTData(res);
                    //将医院代码匹配的原门地址数据从IsDelete=1更改为IsDelete=0，不更新原始数据中IsDelete=2或3的数据
                    hospitalService.UpdateDDTStatus("DDT");
                    //更新院外数据与现有主数据匹配
                    var OHres = hospitalService.GetOHData("DDT");
                    if (OHres != null && OHres.Count > 0)
                    {
                        var sus = new List<P_HOSPITAL>();
                        foreach (var s in OHres)
                        {
                            string GskHospitalCode = s.GskHospital.Substring(0, s.GskHospital.Length - 3);
                            var Hres = hospitalService.GetHData(GskHospitalCode, "DDT");
                            if (Hres != null && Hres.Count > 0)
                            {
                                string OHName = "院外-" + Hres[0].Name;
                                sus.Add(new P_HOSPITAL()
                                {
                                    GskHospital = s.GskHospital,
                                    CityId = Hres[0].CityId,
                                    Name = OHName,
                                    FirstLetters = NPinyin.Pinyin.GetInitials(OHName).ToLower(),
                                    ProvinceId = Hres[0].ProvinceId,
                                    IsDelete = 0,

                                });
                            }
                        }
                        hospitalService.UpdateOHData(sus);
                    }
                    //根据变量表更新门地址状态
                    //获取变量表中DDT且ACTION=6的主地址数据
                    var HosVar = hospitalService.GetHosVariables("DDT", "6");
                    if (HosVar != null && HosVar.Count > 0)
                    {
                        //主数据表中是否有与变量表删除主数据匹配的门地址
                        var notmain = hospitalService.Getnotmain("DDT", "6", "DDT");
                        if (notmain != null && notmain.Count > 0)
                        {
                            //更新失效门地址状态为IsDelete=2
                            hospitalService.Updatenotmain(notmain, HosVar);
                        }
                    }
                    //获取变量表中DDT且ACTION=5的主地址数据
                    var AHosVar = hospitalService.GetHosVariables("DDT", "5");
                    if (AHosVar != null && AHosVar.Count > 0)
                    {
                        //主数据表中是否有与变量表地址变更主数据匹配的门地址
                        var Anotmain = hospitalService.Getnotmain("DDT", "5", "DDT");
                        if (Anotmain != null && Anotmain.Count > 0)
                        {
                            //更新主地址地址变更的门地址状态为IsDelete=3
                            hospitalService.UpdateAnotmain(Anotmain, AHosVar);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error("更新主数据中DDT部分数据", ex);
                    throw ex;
                }
            }
            return Json(new { state = 1 });
        }
        #endregion

        #region 更新TSKF数据
        /// <summary>
        /// 更新TSKF数据
        /// </summary>
        /// <returns></returns>
        public JsonResult ReadTSKFTemp()
        {
            //获取TSKF临时表数据
            var res = hospitalService.LoadTSKFTemp();
            if (res != null && res.Count > 0)
            {
                try
                {
                    //删除主数据表中Vx的主地址数据以及Detail表关联数据
                    hospitalService.DeleteMainAddressData("TSKF");
                    //更新有效的门地址IsDelete=1，不更新原始数据中IsDelete=2或3的数据
                    hospitalService.UpdateIsDelete("TSKF");
                    //更新院外IsDelete=1
                    hospitalService.UpdateOHIsDelete("TSKF");
                    //将DDT临时表数据插入主数据表及Detail表
                    hospitalService.InsertTSKFData(res);
                    //将医院代码匹配的原门地址数据从IsDelete=1更改为IsDelete=0，不更新原始数据中IsDelete=2或3的数据
                    hospitalService.UpdateTSKFStatus("TSKF");
                    //更新院外数据与现有主数据匹配
                    var OHres = hospitalService.GetOHData("TSKF");
                    if (OHres != null && OHres.Count > 0)
                    {
                        var sus = new List<P_HOSPITAL>();
                        foreach (var s in OHres)
                        {
                            string GskHospitalCode = s.GskHospital.Substring(0, s.GskHospital.Length - 3);
                            var Hres = hospitalService.GetHData(GskHospitalCode, "TSKF");
                            if (Hres != null && Hres.Count > 0)
                            {
                                string OHName = "院外-" + Hres[0].Name;
                                sus.Add(new P_HOSPITAL()
                                {
                                    GskHospital = s.GskHospital,
                                    CityId = Hres[0].CityId,
                                    Name = OHName,
                                    FirstLetters = NPinyin.Pinyin.GetInitials(OHName).ToLower(),
                                    ProvinceId = Hres[0].ProvinceId,
                                    IsDelete = 0,

                                });
                            }
                        }
                        hospitalService.UpdateOHData(sus);
                    }
                    //根据变量表更新门地址状态
                    //获取变量表中TSKF且ACTION=6的主地址数据
                    var HosVar = hospitalService.GetHosVariables("TSKF", "6");
                    if (HosVar != null && HosVar.Count > 0)
                    {
                        //主数据表中是否有与变量表删除主数据匹配的门地址
                        var notmain = hospitalService.Getnotmain("TSKF", "6", "TSKF");
                        if (notmain != null && notmain.Count > 0)
                        {
                            //更新失效门地址状态为IsDelete=2
                            hospitalService.Updatenotmain(notmain, HosVar);
                        }
                    }
                    //获取变量表中TSKF且ACTION=5的主地址数据
                    var AHosVar = hospitalService.GetHosVariables("TSKF", "5");
                    if (AHosVar != null && AHosVar.Count > 0)
                    {
                        //主数据表中是否有与变量表地址变更主数据匹配的门地址
                        var Anotmain = hospitalService.Getnotmain("TSKF", "5", "TSKF");
                        if (Anotmain != null && Anotmain.Count > 0)
                        {
                            //更新主地址地址变更的门地址状态为IsDelete=3
                            hospitalService.UpdateAnotmain(Anotmain, AHosVar);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error("更新主数据中TSKF部分数据", ex);
                    throw ex;
                }
            }
            return Json(new { state = 1 });
        }
        #endregion

        #region 每小时更新超过5个自然日未审批的地址申请数据
        public JsonResult Invalidate()
        {
            try
            {
                var nowDate = DateTime.Now;
                nowDate = nowDate.AddDays(-5);
                var res = preApprovalService.LoadInvalidAddressApplication(nowDate);
                LogHelper.Info(res.Count.ToString());

                if (res.Count > 0)
                {
                    Task.Factory.StartNew(() =>
                    {
                        var aryIds = new List<Guid>();
                        foreach (var item in res)
                        {
                            aryIds.Add(item.ID);
                        }
                        aryIds = aryIds.Distinct().ToList();
                        var updCnt = preApprovalService.InvalidAddressApplication(aryIds, 3);
                    });
                }

                return Json(new { state = 1 });
            }
            catch (Exception ex)
            {
                LogHelper.Error("Invalidate-每小时更新超过5个自然日未审批的地址申请数据", ex);
                return Json(new { state = 0 });
            }
        }
        #endregion

        #region 获取医院数据变量报告接口
        /// <summary>
        /// 获取医院数据变量报告接口
        /// </summary>
        /// <returns></returns>
        public JsonResult SyncHospitalChangedXMS()
        {
            try
            {
                LogHelper.Info("SyncHospitalChangedXMS---1");
                var time = DateTime.Now;
                var channel = OpenApiChannelFactory.GetChannel();
                // XMS 获取医院数据变量报告接口
                var resXMS = channel.SyncHospitalChangedXMS();
                LogHelper.Info(resXMS.result.ToString());
                return Json(new { state = 1, JsonRequestBehavior.AllowGet });
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
                return Json(new { state = 0, JsonRequestBehavior.AllowGet });
            }

        }

        #endregion

        #region 获取医院数据变量报告接口
        /// <summary>
        /// 获取医院数据变量报告接口
        /// </summary>
        /// <returns></returns>
        public JsonResult SyncHospitalChangedBDS()
        {
            try
            {
                LogHelper.Info("SyncHospitalChangedBDS---1");
                var time = DateTime.Now;
                var channel = OpenApiChannelFactory.GetChannel();
                // BDS 获取医院数据变量报告接口
                var resBDS = channel.SyncHospitalChanged();
                LogHelper.Info(resBDS.result.ToString());
                return Json(new { state = 1, JsonRequestBehavior.AllowGet });
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
                return Json(new { state = 0, JsonRequestBehavior.AllowGet });
            }
        }

        #endregion

        #region 每周一7:45将临时变量数据copy到P_Hospital_Variables_Data
        public JsonResult SyncCoypHospitalVariablesData()
        {
            try
            {
                List<CHECK_REPORT_LINE_RM> list = new List<CHECK_REPORT_LINE_RM>();
                list = hospitalService.LoadTerritoryRMVariablesData();
                if (list != null && list.Count > 0)
                {
                    LogHelper.Info("SyncCoypTerritoryRMVariablesData---1");
                    // 每周一将大区代码变量数据插入2.0系统对应的表中
                    var result = hospitalService.SyncCoypTerritoryRMVariablesData(list);
                }

                LogHelper.Info("SyncCoypHospitalVariablesData---1");
                // 每周一将临时变量表数据插入2.0系统对应的表中
                var res = hospitalService.SyncCoypHospitalVariablesData();
                // 每周一插入各BU数据&TA数据到P_Hospital_Variables_Count
                if (res > -1)
                {
                    var re = hospitalService.InsertHospitalVariablesCountData();
                    List<P_TERRITORY_TA> p_TAs = new List<P_TERRITORY_TA>();
                    p_TAs = hospitalService.LoadTerritoryTA();
                    var r = hospitalService.InsertHospitalVariablesCountDataTA(p_TAs);
                }
                return Json(new { state = 1, JsonRequestBehavior.AllowGet });
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
                return Json(new { state = 0, JsonRequestBehavior.AllowGet });
            }
        }
        #endregion

        #region 根据变量表同步医院数据
        /// <summary>
        /// 根据变量表同步医院数据
        /// </summary>
        /// <returns></returns>
        public JsonResult UpdateHospitalByVariables()
        {
            //获取变量临时表数据
            var temp = hospitalService.LoadVariablesTemp();
            if (temp != null && temp.Count > 0)
            {
                try
                {
                    //变量表非删除数据涉及省市转化为省市ID
                    var Nodeltemp = hospitalService.LoadVariablesTempNoDelete();
                    var addlist = new List<Temp_Hospital_Variables>();
                    var Updatelist = new List<Temp_Hospital_Variables>();
                    if (Nodeltemp != null && Nodeltemp.Count > 0)
                    {
                        var newidlist = hospitalService.LoadProvinceCityId(Nodeltemp);

                        foreach (var s in newidlist)
                        {
                            string action = s.action.ToString().Trim();
                            #region 根据action对同步数据进行不同操作
                            //action=1的新增数据         
                            if (action == "1")
                            {
                                addlist.Add(new Temp_Hospital_Variables()
                                {
                                    GskHospital = s.GskHospital,
                                    Province = s.Province,
                                    City = s.City,
                                    HospitalName = s.HospitalName,
                                    Address = s.Address,
                                    IsMainAdd = s.IsMainAdd,
                                    Market = s.Market,
                                    Longitude = s.Longitude,
                                    Latitude = s.Latitude,
                                    DistrictCode = s.DistrictCode,
                                    District = s.District,
                                    action = 1,
                                    createdate = s.createdate,
                                    createby = s.createby,
                                    Remarks = s.Remarks
                                });
                            }
                            //action=2,3,4,5的修改数据
                            if (action == "2" || action == "3" || action == "4" || action == "5")
                            {
                                Updatelist.Add(new Temp_Hospital_Variables()
                                {
                                    GskHospital = s.GskHospital,
                                    Province = s.Province,
                                    City = s.City,
                                    HospitalName = s.HospitalName,
                                    Address = s.Address,
                                    IsMainAdd = s.IsMainAdd,
                                    Market = s.Market,
                                    Longitude = s.Longitude,
                                    Latitude = s.Latitude,
                                    DistrictCode = s.DistrictCode,
                                    District = s.District,
                                    action = s.action,
                                    createdate = s.createdate,
                                    createby = s.createby,
                                    Remarks = s.Remarks
                                });
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        LogHelper.Info("[Temp_Hospital_Variables]变量表无action不为6的数据");
                    }
                    //获取action=6的删除数据
                    var deltemp = hospitalService.LoadVariablesTempAddData("6");
                    var dellist = new List<Temp_Hospital_Variables>();
                    if (deltemp != null && deltemp.Count > 0)
                    {
                        dellist = deltemp;
                    }
                    else
                    {
                        LogHelper.Info("[Temp_Hospital_Variables]变量表无action=6的数据");
                    }
                    //根据变量表数据同步更新主数据和detail表
                    //hospitalService.UpdateHospitalAndDetail(addlist, Updatelist,dellist);
                }
                catch (Exception ex)
                {
                    LogHelper.Error("更新主数据中Rx部分数据", ex);
                    throw ex;
                }
            }
            else
            {
                LogHelper.Info("[Temp_Hospital_Variables]变量表无数据");
            }
            return Json(new { state = 1 });
        }
        #endregion

        #region 发送覆盖变更信息
        public void SendRestaurantChangeMessage()
        {
            //获取未发送信息
            var CoverChangeList = hospitalService.FindHospitalCoverChange("1");

            if (CoverChangeList != null && CoverChangeList.Count > 0)
            {
                //院内医院由覆盖变为无覆盖
                var SendList1 = CoverChangeList.Where(p => p.Type == 1).Select(x => new { x.HospitalCode, x.HospitalName, x.Address }).Distinct().ToList();

                foreach (var item in SendList1)
                {

                    var userList = hospitalService.FindHospitalUser(item.HospitalCode);
                    if (userList != null && userList.Count > 0)
                    {
                        var ul = userList.Select(x => x.MUDID).Distinct().ToArray();
                        string user = string.Join("|", ul);
                        WxMessageClientChannelFactory.GetChannel().SendCoverChangeToUser(item.HospitalCode, item.HospitalName, item.Address, user, 1, "", "");
                    }
                }

                //院内医院由无覆盖变为覆盖
                var SendList2 = CoverChangeList.Where(p => p.Type == 2).Select(x => new { x.HospitalCode, x.HospitalName, x.Address }).Distinct().ToList();

                foreach (var item in SendList2)
                {

                    var userList = hospitalService.FindHospitalUser(item.HospitalCode);
                    if (userList != null && userList.Count > 0)
                    {
                        var ul = userList.Select(x => x.MUDID).Distinct().ToArray();
                        string user = string.Join("|", ul);
                        WxMessageClientChannelFactory.GetChannel().SendCoverChangeToUser(item.HospitalCode, item.HospitalName, item.Address, user, 2, "", "");
                    }
                }

                //院外餐厅上线
                var SendList3 = CoverChangeList.Where(p => p.Type == 3).Select(x => new { x.HospitalCode, x.HospitalName, x.Address, x.ResId, x.ResName }).Distinct().ToList();

                foreach (var item in SendList3)
                {

                    var userList = hospitalService.FindHospitalUser(item.HospitalCode.Replace("-OH", ""));
                    if (userList != null && userList.Count > 0)
                    {
                        var ul = userList.Select(x => x.MUDID).Distinct().ToArray();
                        string user = string.Join("|", ul);
                        WxMessageClientChannelFactory.GetChannel().SendCoverChangeToUser(item.HospitalCode, item.HospitalName, "", user, 3, item.ResId, item.ResName);
                    }
                }

                //院外餐厅下线，且还有其他覆盖餐厅
                var SendList4 = CoverChangeList.Where(p => p.Type == 4).Select(x => new { x.HospitalCode, x.HospitalName, x.Address, x.ResId, x.ResName }).Distinct().ToList();

                foreach (var item in SendList4)
                {

                    var userList = hospitalService.FindHospitalUser(item.HospitalCode.Replace("-OH", ""));
                    if (userList != null && userList.Count > 0)
                    {
                        var ul = userList.Select(x => x.MUDID).Distinct().ToArray();
                        string user = string.Join("|", ul);
                        WxMessageClientChannelFactory.GetChannel().SendCoverChangeToUser(item.HospitalCode, item.HospitalName, "", user, 4, item.ResId, item.ResName);
                    }
                }

                //院外餐厅下线，且没有其他覆盖餐厅
                var SendList5 = CoverChangeList.Where(p => p.Type == 5).Select(x => new { x.HospitalCode, x.HospitalName, x.Address, x.ResId, x.ResName }).Distinct().ToList();

                foreach (var item in SendList5)
                {

                    var userList = hospitalService.FindHospitalUser(item.HospitalCode.Replace("-OH", ""));
                    if (userList != null && userList.Count > 0)
                    {
                        var ul = userList.Select(x => x.MUDID).Distinct().ToArray();
                        string user = string.Join("|", ul);
                        WxMessageClientChannelFactory.GetChannel().SendCoverChangeToUser(item.HospitalCode, item.HospitalName, "", user, 5, item.ResId, item.ResName);
                    }
                }

                //更新发送状态
                hospitalService.UpdateMessageState();
            }
        }
        #endregion

        #region 同步数据分析数据
        public JsonResult SyncCostTable()
        {
            LogHelper.Info("Start Sync Cost Table");

            try
            {
                var syncPre = preApprovalService.SyncPreApproval();
                LogHelper.Info("Sync PreApproval Count:" + syncPre);

                var syncOrder = orderService.SyncOrder();
                LogHelper.Info("Sync Order Count:" + syncOrder);

                var syncUpload = uploadOrderService.SyncPreUploadOrder();
                LogHelper.Info("Sync Upload Count:" + syncUpload);

                LogHelper.Info("End Sync Cost Table");

                return Json(new { state = 1 });
            }
            catch(Exception e)
            {
                LogHelper.Error(e.Message);
                return Json(new { state = 0 });
            }
            
        }

        public JsonResult SyncHosTable()
        {
            LogHelper.Info("Start Sync Hos Table");

            try
            {
                var syncHospital = hospitalService.SyncHospital();
                LogHelper.Info("Sync Hospital Count:" + syncHospital);

                var syncHospitalDetail = hospitalService.SyncHospitalDetail();
                LogHelper.Info("Sync HospitalDetail Count:" + syncHospitalDetail);

                var syncTerritoryHospital = hospitalService.SyncTerritoryHospital();
                LogHelper.Info("Sync TerritoryHospital Count:" + syncTerritoryHospital);

                LogHelper.Info("End Sync Hos Table");

                return Json(new { state = 1 });
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
                return Json(new { state = 0 });
            }
        }


        public JsonResult SyncHospitalRange()
        {
            LogHelper.Info("Start Sync HosRange Table");

            try
            {
                var syncHospitalRange = hospitalService.SyncHospitalRange();
                LogHelper.Info("Sync Hospital Range Count:" + syncHospitalRange);

                LogHelper.Info("End Sync HosRange Table");

                return Json(new { state = 1 });
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
                return Json(new { state = 0 });
            }
        }

        #endregion
    }
}