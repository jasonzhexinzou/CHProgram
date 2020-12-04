using iPathFeast.API.Client;
using MealAdmin.Entity;
using MealAdminApiClient;
using MealH5.Areas.P.Filter;
using MealH5.Areas.P.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XFramework.XInject.Attributes;
using XFramework.XUtil;

namespace MealH5.Areas.P.Controllers
{
    /// <summary>
    /// 预申请状态控制器
    /// </summary>
    [WxUserFilter]
    public class PreApprovalStateController : BaseController
    {
        [Autowired]
        ApiV1Client apiClient { get; set; }
        /// <summary>
        /// 预申请状态
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [iPathOAuthFilter(MappingKey = "0x0001", CallBackUrl = true)]
        public ActionResult Index()
        {
            ViewBag.HasApproveRights = HasApproveRights();
            return View();
        }

        #region 预申请审批
        /// <summary>
        /// 预申请审批
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public JsonResult PreApprovalApprove(Guid id, int state, string reason)
        {
            var preApprovalChannel = PreApprovalClientChannelFactory.GetChannel();
            P_USERINFO userInfo = Session[MealH5.Util.ConstantHelper.CURRENTWXUSER] as P_USERINFO;
            var preApproval = preApprovalChannel.LoadPreApprovalInfo(id);
            // var userInfoChannel = UserInfoClientChannelFactory.GetChannel();
            bool flag = preApprovalChannel.HasApproveByTA(preApproval.CurrentApproverMUDID, preApproval.TA);
            bool second = false;
            //审批通过
            if (state == 3)
            {
                if (preApproval.BudgetTotal >= 1200 && preApproval.BudgetTotal < 1500)
                {
                    state = 7;
                    second = true;
                }
                else if (preApproval.BudgetTotal >= 1500)
                {
                    state = 3;
                }

                //预申请buhead审批通过
                if ((preApproval.BUHeadMUDID.ToUpper() == userInfo.UserId.ToUpper() || flag == true) && second == false)
                {
                    var res = preApprovalChannel.BUHeadApprove(id, 5, reason);
                    if (res == 1)
                    {
                        P_PreApprovalApproveHistory preApprovalHistory = new P_PreApprovalApproveHistory()
                        {
                            ID = Guid.NewGuid(),
                            PID = id,
                            UserName = userInfo.Name,
                            UserId = userInfo.UserId,
                            ActionType = 3,
                            ApproveDate = DateTime.Now,
                            Comments = reason,
                            type = 2
                        };
                        //添加审批记录
                        var historyRes = preApprovalChannel.AddPreApprovalApproveHistory(preApprovalHistory);
                        if (historyRes == 0)
                        {
                            LogHelper.Info($"审批通过，添加审批记录失败 - [{ id},{userInfo.UserId}]");
                        }
                        // 发用户消息
                        var preApproval1 = preApprovalChannel.LoadPreApprovalInfo(id);
                        WxMessageHandler.GetInstance().SendPreApprovalStateChangeMessageToUser(preApproval1);
                        LogHelper.Info($"BuHead审批通过 - [{ id},{userInfo.UserId}]");
                        return Json(new { state = 1, txt = "预申请已审批通过。" });
                    }
                    else
                    {
                        LogHelper.Info($"（审批通过）预申请状态修改失败 - [{id}]");
                    }

                }
                else if (state == 3)
                {
                    if (!CheckApproveStep(userInfo.UserId, preApproval.TA))
                    {
                        return Json(new { state = 0, txt = "当前预申请审批流程有误，请联系：技术支持热线: 0411-84898998或PMO邮箱cn.chinarx-pmo@gsk.com。" });
                    }

                    var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
                    var pre = baseDataChannel.GetNameUserId(preApproval.CurrentApproverMUDID);
                    //var delegateMUDID = userInfoChannel.isAgent(preApproval.CurrentApproverMUDID);
                    //preApproval.CurrentApproverMUDID = delegateMUDID == null ? preApproval.CurrentApproverMUDID : delegateMUDID.DelegateUserMUDID;
                    //preApproval.CurrentApproverName = delegateMUDID == null ? preApproval.CurrentApproverName : delegateMUDID.DelegateUserName;
                    preApproval.CurrentApproverMUDID = preApproval.CurrentApproverMUDID;
                    preApproval.CurrentApproverName = preApproval.CurrentApproverName;
                    P_PreApprovalApproveHistory preApprovalHistory = new P_PreApprovalApproveHistory()
                    {
                        ID = Guid.NewGuid(),
                        PID = id,
                        UserName = preApproval.CurrentApproverName,
                        UserId = preApproval.CurrentApproverMUDID,
                        ActionType = 3,
                        ApproveDate = DateTime.Now,
                        Comments = reason,
                        type = 2
                    };
                    //添加审批记录
                    var historyRes = preApprovalChannel.AddPreApprovalApproveHistory(preApprovalHistory);
                    if (historyRes == 0)
                    {
                        LogHelper.Info($"审批通过，添加审批记录失败 - [{ id}]");
                    }
                    else
                    {
                        preApproval.CurrentApproverMUDID = pre.CurrentApproverMUDID;
                        preApproval.CurrentApproverName = pre.CurrentApproverName;
                        var num = baseDataChannel.UpdateCurrentPreApprova(preApproval);
                        if (num == 0)
                        {
                            LogHelper.Info($"审批通过，跟新审批人失败 - [{ id}]");
                        }
                    }
                    // 发用户消息
                    //bool flag1 = preApprovalChannel.HasApprove(preApproval.CurrentApproverMUDID);
                    //if (flag1 == true)
                    //{
                    var preApproval3 = preApprovalChannel.LoadPreApprovalInfo(id);
                    preApproval3.IsOnc = 2;
                    WxMessageHandler.GetInstance().SendPreApprovalStateChangeMessageToUser(preApproval3);
                    LogHelper.Info($"审批通过 - [{ id},{pre.CurrentApproverMUDID}]");
                    //}

                    return Json(new { state = 1, txt = "预申请已审批通过。" });

                }
                else if (state == 7)
                {
                    var res = preApprovalChannel.BUHeadApprove(id, 9, reason);
                    if (res == 1)
                    {
                        P_PreApprovalApproveHistory preApprovalHistory = new P_PreApprovalApproveHistory()
                        {
                            ID = Guid.NewGuid(),
                            PID = id,
                            UserName = userInfo.Name,
                            UserId = userInfo.UserId,
                            ActionType = 3,
                            ApproveDate = DateTime.Now,
                            Comments = reason,
                            type = 2
                        };
                        //添加审批记录
                        var historyRes = preApprovalChannel.AddPreApprovalApproveHistory(preApprovalHistory);
                        if (historyRes == 0)
                        {
                            LogHelper.Info($"审批通过，添加审批记录失败 - [{ id}]");
                        }
                        // 发用户消息
                        var preApproval1 = preApprovalChannel.LoadPreApprovalInfo(id);
                        WxMessageHandler.GetInstance().SendPreApprovalStateChangeMessageToUser(preApproval1);
                        LogHelper.Info($"二线审批通过 - [{ id},{preApproval.CurrentApproverMUDID}]");
                        return Json(new { state = 1, txt = "预申请已审批通过。" });
                    }
                    else
                    {
                        LogHelper.Info($"（审批通过）预申请状态修改失败 - [{ id}]");
                    }
                }
            }

            else
            {
                if (preApproval.BudgetTotal >= 1200 && preApproval.BudgetTotal < 1500)
                {
                    state = 8;
                    preApproval.State = "8";
                }
                else if (preApproval.BudgetTotal >= 1500)
                {

                    if (!CheckApproveStep(userInfo.UserId, preApproval.TA))
                    {
                        return Json(new { state = 0, txt = "当前预申请审批流程有误，请联系：技术支持热线: 0411-84898998或PMO邮箱cn.chinarx-pmo@gsk.com。" });
                    }

                    state = 4;
                    preApproval.State = "4";
                    LogHelper.Info($"审批驳回 - [{ id},{preApproval.CurrentApproverMUDID}]");

                }
                var res = preApprovalChannel.BUHeadReject(id, state, reason);
                //var preApproval = preApprovalChannel.LoadPreApprovalInfo(id);
                if (res == 1)
                {
                    P_PreApprovalApproveHistory preApprovalHistory = new P_PreApprovalApproveHistory()
                    {
                        ID = Guid.NewGuid(),
                        PID = id,
                        UserName = userInfo.Name,
                        UserId = userInfo.UserId,
                        ActionType = 2,
                        ApproveDate = DateTime.Now,
                        Comments = reason,
                        type = 2
                    };
                    //添加审批记录
                    var historyRes = preApprovalChannel.AddPreApprovalApproveHistory(preApprovalHistory);

                    if (historyRes == 0)
                    {
                        LogHelper.Info($"审批驳回，添加审批记录失败 - [{ id}]");
                    }
                    // 发用户消息
                    if (state == 8 || flag == true)
                    {
                        var preApproval4 = preApprovalChannel.LoadPreApprovalInfo(id);
                        WxMessageHandler.GetInstance().SendPreApprovalRejectMessageToUser(reason, preApproval4);
                        LogHelper.Info($"审批拒绝 - [{ id},{preApproval.CurrentApproverMUDID}]");

                    }

                    return Json(new { state = 1, txt = "预申请已审批驳回。" });
                }
                else
                {
                    LogHelper.Info($"（审批驳回）预申请状态修改失败 - [{ id}]");
                }
            }
            return Json(new { state = 0, txt = "预申请审批失败，请刷新页面后重试。", errCode = 9007 });
        }
        #endregion

        #region 预申请MMCoE审批
        /// <summary>
        /// 预申请审批
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public JsonResult SaveMMCoEResult(Guid id, int state, string reason)
        {
            var preApprovalChannel = PreApprovalClientChannelFactory.GetChannel();
            P_USERINFO userInfo = Session[MealH5.Util.ConstantHelper.CURRENTWXUSER] as P_USERINFO;
            var preApproval = preApprovalChannel.LoadPreApprovalInfo(id);
            var hisList = preApprovalChannel.FindPreApprovalApproveHistory(id);
            P_PreApprovalApproveHistory lastBUHeadApprove = null;
            if (hisList.Count > 0)
            {
                lastBUHeadApprove = hisList.Where(p => p.PID == id && p.type == 2 && (p.ActionType == 2 || p.ActionType == 3)).OrderByDescending(p => p.ApproveDate).FirstOrDefault();
            }
            var isReject = true;
            if (lastBUHeadApprove != null)
            {
                if (lastBUHeadApprove.ActionType == 3)
                {
                    isReject = false;
                }
            }
            //审批通过
            if (state == 3)
            {
                //预算超02000并且（金额产生变化或上一次是审批拒绝）
                if (preApproval.BudgetTotal > 2000 && (preApproval.IsBudgetChange || (lastBUHeadApprove == null || isReject == true) || preApproval.ModifyDate == null))
                {
                    preApproval.State = "3";
                }
                else
                {
                    preApproval.State = "6";
                }
                var res = preApprovalChannel.MMCoEApprove(id, int.Parse(preApproval.State), reason);

                if (res == 1)
                {
                    P_PreApprovalApproveHistory preApprovalHistory = new P_PreApprovalApproveHistory()
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
                    // 发用户消息
                    WxMessageHandler.GetInstance().SendPreApprovalStateChangeMessageToUser(preApproval);
                    //添加审批记录
                    var historyRes = preApprovalChannel.AddPreApprovalApproveHistory(preApprovalHistory);
                    if (preApproval.State == "6")
                    {
                        P_PreApprovalApproveHistory autoHistory = new P_PreApprovalApproveHistory()
                        {
                            ID = Guid.NewGuid(),
                            PID = id,
                            UserName = "系统自动审批",
                            UserId = "系统自动审批",
                            ActionType = 3,
                            ApproveDate = DateTime.Now,
                            type = 1
                        };
                        preApprovalChannel.AddPreApprovalApproveHistory(autoHistory);
                    }
                    if (historyRes == 0)
                    {
                        LogHelper.Info($"审批通过，添加审批记录失败 - [{ id}]");
                    }
                    return Json(new { state = 1, txt = "预申请已审批通过。" });
                }
                else
                {
                    LogHelper.Info($"（审批通过）预申请状态修改失败 - [{ id}]");
                }
            }
            else
            {
                var res = preApprovalChannel.MMCoEReject(id, state, reason);
                preApproval.State = "2";
                if (res == 1)
                {
                    P_PreApprovalApproveHistory preApprovalHistory = new P_PreApprovalApproveHistory()
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
                    // 发用户消息
                    WxMessageHandler.GetInstance().SendPreApprovalRejectMessageToUser(reason, preApproval);
                    //添加审批记录
                    var historyRes = preApprovalChannel.AddPreApprovalApproveHistory(preApprovalHistory);
                    if (historyRes == 0)
                    {
                        LogHelper.Info($"审批驳回，添加审批记录失败 - [{ id}]");
                    }
                    return Json(new { state = 1, txt = "预申请已审批驳回。" });
                }
                else
                {
                    LogHelper.Info($"（审批驳回）预申请状态修改失败 - [{ id}]");
                }
            }
            return Json(new { state = 0, txt = "预申请审批失败，请刷新页面后重试。", errCode = 9007 });
        }
        #endregion

        public JsonResult LoadMyPreApproval(string end, string state, int year, int month, string budget)
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
            var channel = PreApprovalClientChannelFactory.GetChannel();
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
            //我的预申请
            var list = channel.LoadMyPreApprovalUserId(CurrentWxUser.UserId, _begin, _end, state, budget, rows, page, out total);
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
            //20190125获当前用户有没有待审批的人
            var channel = PreApprovalClientChannelFactory.GetChannel();
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
            var list = channel.LoadCurrentApprove(CurrentWxUser.UserId, _begin, _end, state, applicant, rows, page, out total);

            return Json(new { state = 1, rows = list });
        }

        public bool HasApproveRights()
        {
            var channel = PreApprovalClientChannelFactory.GetChannel();
            var res = channel.HasApproveRights(CurrentWxUser.UserId);
            return res;
        }
        public JsonResult ApproveAll(string end, string state, string applicant, int year, int month)
        {
            bool hasOverDate = false;
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
            var channel = PreApprovalClientChannelFactory.GetChannel();
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
                    if (item.MeetingDate < DateTime.Now)
                    {
                        hasOverDate = true;
                        continue;
                    }
                    var res = channel.BUHeadApprove(item.ID, 3, "");
                    item.State = "5";
                    if (res == 1)
                    {
                        P_PreApprovalApproveHistory preApprovalHistory = new P_PreApprovalApproveHistory()
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
                        var historyRes = channel.AddPreApprovalApproveHistory(preApprovalHistory);
                        if (historyRes == 0)
                        {
                            LogHelper.Info($"审批通过，添加审批记录失败 - [{ item.ID}]");
                        }
                        // 发用户消息
                        WxMessageHandler.GetInstance().SendPreApprovalStateChangeMessageToUser(item);
                    }
                    else
                    {
                        LogHelper.Info($"（审批通过）预申请状态修改失败 - [{ item.ID}]");
                    }
                }
                return Json(new { state = 1, txt = hasOverDate == true ? "预申请已全部审批通过，会议日期失效部分系统已忽略。" : "预申请已全部审批通过。" });
            }
            else
            {
                return Json(new { state = 1, txt = "当前没有待审批预申请数据。" });
            }
        }

        public JsonResult ApproveSelected(List<string> Ids)
        {
            bool hasOverDate = false;
            var channel = PreApprovalClientChannelFactory.GetChannel();
            P_USERINFO userInfo = Session[MealH5.Util.ConstantHelper.CURRENTWXUSER] as P_USERINFO;
            foreach (var item in Ids)
            {
                var preApproval = channel.LoadPreApprovalInfo(Guid.Parse(item));
                if (preApproval.MeetingDate < DateTime.Now)
                {
                    hasOverDate = true;
                    continue;
                }
                var res = channel.BUHeadApprove(Guid.Parse(item), 3, "");
                if (res == 1)
                {
                    P_PreApprovalApproveHistory preApprovalHistory = new P_PreApprovalApproveHistory()
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
                    var historyRes = channel.AddPreApprovalApproveHistory(preApprovalHistory);
                    if (historyRes == 0)
                    {
                        LogHelper.Info($"审批通过，添加审批记录失败 - [{ item}]");
                    }
                    // 发用户消息
                    //var preApproval = channel.LoadPreApprovalInfo(Guid.Parse(item));
                    WxMessageHandler.GetInstance().SendPreApprovalStateChangeMessageToUser(preApproval);
                }
                else
                {
                    LogHelper.Info($"（审批通过）预申请状态修改失败 - [{ item}]");
                }
            }
            return Json(new { state = 1, txt = hasOverDate == true ? "预申请已批量审批通过，会议日期失效部分系统已忽略。" : "预申请已批量审批通过。" });
        }

        public bool CheckApproveStep(string userid, string TA)
        {
            var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
            var preApprovalChannel = PreApprovalClientChannelFactory.GetChannel();
            bool result = false;
            bool flag = preApprovalChannel.HasApproveByTA(userid, TA);
            if (flag)
            {
                return true;
            }
            for (int i = 0; i < 6; i++)
            {
                var pre = baseDataChannel.GetNameUserId(userid);
                if (string.IsNullOrEmpty(pre.CurrentApproverMUDID) || pre == null)
                {
                    result = false;
                    break;
                }
                else
                {
                    userid = pre.CurrentApproverMUDID;
                }
                bool flag1 = preApprovalChannel.HasApproveByTA(userid, TA);

                if (flag1)
                {
                    result = true;
                    break;
                }
                else
                {
                    continue;
                }
            }

            return result;
        }

        #region 新增地址
        [HttpGet]
        [iPathOAuthFilter(MappingKey = "0x0001", CallBackUrl = true)]
        public ActionResult Address()
        {
            ViewBag.Authority = HasApproveRights();
            return View();
        }

        [HttpGet]
        public ActionResult ApplyAddress()
        {
            ViewBag.HasApproveRights = true;
            return View();
        }

        public JsonResult LoadMyAddressApproval(string end, string state, int year, int month, string budget)
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
            var channel = PreApprovalClientChannelFactory.GetChannel();
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
            //我的预申请
            var list = channel.LoadMyAddressApprovalByUserId(CurrentWxUser.UserId, _begin, _end, state, budget, rows, page, out total);
            return Json(new { state = 1, rows = list });
        }
        /// <summary>
        /// 获取当前待审批记录
        /// </summary>
        /// <param name="end"></param>
        /// <param name="state"></param>
        /// <param name="applicant"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public JsonResult LoadMyAddressApprove(string end, string state, string applicant, int year, int month)
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
            //20190125获当前用户有没有待审批的人
            var channel = PreApprovalClientChannelFactory.GetChannel();
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
            var list = channel.LoadMyAddressApprove(CurrentWxUser.UserId, _begin, _end, state, applicant, rows, page, out total);
            var totalList = channel.LoadMyAddressApproveCount(CurrentWxUser.UserId, _begin, _end, state, applicant);
            return Json(new { state = 1, rows = list, totalCount = totalList.Count });
        }

        public JsonResult LoadMyAddressApproveCount(string end, string state, string applicant, int year, int month)
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
            //20190125获当前用户有没有待审批的人
            var channel = PreApprovalClientChannelFactory.GetChannel();
            DateTime _begin = Convert.ToDateTime(beginTime);
            DateTime _end = Convert.ToDateTime(endTime);
            DateTime __end = Convert.ToDateTime(end);
            if (__end >= _begin && __end <= _end)
            {
                _end = __end;
            }
            var totalList = channel.LoadMyAddressApproveCount(CurrentWxUser.UserId, _begin, _end, state, applicant);
            return Json(new { state = 1, totalCount = totalList.Count });
        }

        public JsonResult AddressApprove(Guid id, int action, string reason)
        {
            P_AddressApproval addressApproval = new P_AddressApproval();
            addressApproval.ID = id;
            P_AddressApproval_View addressApproval_View = new P_AddressApproval_View();
            var preApprovalChannel = PreApprovalClientChannelFactory.GetChannel();
            var channel = PreApprovalClientChannelFactory.GetChannel();
            addressApproval_View = channel.LoadAddressApprovalInfo(id);
            P_USERINFO userInfo = Session[MealH5.Util.ConstantHelper.CURRENTWXUSER] as P_USERINFO;
            string msg = "";

            try
            {
                var res = preApprovalChannel.AddressApprove(addressApproval_View, reason);
                if (res == 1)
                {
                    P_AddressApproveHistory addressApproveHistory = new P_AddressApproveHistory()
                    {
                        ID = Guid.NewGuid(),
                        DA_ID = id,
                        UserName = userInfo.Name,
                        UserId = userInfo.UserId,
                        ActionType = action,
                        ApproveDate = DateTime.Now,
                        Comments = reason,
                        type = 0,
                        IsDelete = 0
                    };
                    //添加审批记录
                    //var historyRes = preApprovalChannel.AddAddressApproveHistory(addressApproveHistory);
                    //if (historyRes == 0)
                    //{
                    //    LogHelper.Info($"审批成功，添加审批记录失败 - [{ id},{userInfo.UserId}]");
                    //}
                    // 发用户消息
                    WxMessageHandler.GetInstance().SendAddressApprovalStateChangeMessageToUser(addressApproval);
                    LogHelper.Info($"地址申请审批通过 - [{ id},{userInfo.UserId}]");
                    if (reason == "")
                    {
                        if (addressApproval_View.ApprovalStatus == 0)
                            msg = "地址申请审批通过。";
                        else if (addressApproval_View.ApprovalStatus == 9)
                            msg = "地址申请修改审批通过。";
                        else if (addressApproval_View.ApprovalStatus == 10)
                            msg = "地址申请重新提交审批通过。";
                    }
                    else
                    {
                        if (addressApproval_View.ApprovalStatus == 0)
                            msg = "地址申请审批驳回。";
                        else if (addressApproval_View.ApprovalStatus == 9)
                            msg = "地址申请修改审批驳回。";
                        else if (addressApproval_View.ApprovalStatus == 10)
                            msg = "地址申请重新提交审批驳回。";
                    }
                    return Json(new { state = 1, txt = msg });
                }
                else
                {
                    LogHelper.Info($" 审批地址申请失败 - [{id}]");
                    return Json(new { state = 0, txt = "地址申请审批失败，请刷新页面后重试。", errCode = 9007 });
                }
            }catch(Exception ex)
            {
                LogHelper.Error("Exception AddressApprove", ex);
                return Json(new { state = 0, txt = "地址申请审批失败，请刷新页面后重试。", errCode = 9007 });
            }
        }

        public JsonResult AddressApproveAll(int action, string reason, string applicant)
        {
            var channel = PreApprovalClientChannelFactory.GetChannel();
            P_USERINFO userInfo = Session[MealH5.Util.ConstantHelper.CURRENTWXUSER] as P_USERINFO;

            try
            {
                var list = channel.LoadMyAddressApproveAll(CurrentWxUser.UserId, applicant);
                if (list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        var res = channel.AddressApprove(item, reason);
                        if (res == 1)
                        {
                            P_AddressApproveHistory addressApproveHistory = new P_AddressApproveHistory()
                            {
                                ID = Guid.NewGuid(),
                                DA_ID = item.ID,
                                UserName = userInfo.Name,
                                UserId = userInfo.UserId,
                                ActionType = action,
                                ApproveDate = DateTime.Now,
                                Comments = reason,
                                type = 0,
                                IsDelete = 0
                            };
                            //添加审批记录
                            //var historyRes = channel.AddAddressApproveHistory(addressApproveHistory);
                            //if (historyRes == 0)
                            //{
                            //    LogHelper.Info($"审批成功，添加审批记录失败 - [{ item.ID},{userInfo.UserId}]");
                            //}
                            P_AddressApproval addressApproval = new P_AddressApproval();
                            addressApproval.ID = item.ID;
                            // 发用户消息
                            WxMessageHandler.GetInstance().SendAddressApprovalStateChangeMessageToUser(addressApproval);
                        }
                        else
                        {
                            LogHelper.Info($" 审批地址申请失败 - [{item.ID}]");
                        }
                    }
                    return Json(new { state = 1, txt = "地址申请审批通过。" });
                }
                else
                {
                    return Json(new { state = 1, txt = "当前没有待审批地址申请数据。" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("Exception AddressApproveAll", ex);
                return Json(new { state = 0, txt = "地址申请审批失败，请刷新页面后重试。", errCode = 9007 });
            }
        }

        public JsonResult AddressApproveSelected(List<string> Ids)
        {
            var channel = PreApprovalClientChannelFactory.GetChannel();

            P_AddressApproval addressApproval = new P_AddressApproval();
            
            P_AddressApproval_View addressApproval_View = new P_AddressApproval_View();
            var preApprovalChannel = PreApprovalClientChannelFactory.GetChannel();
            
            P_USERINFO userInfo = Session[MealH5.Util.ConstantHelper.CURRENTWXUSER] as P_USERINFO;


            try
            {
                foreach (var id in Ids)
                {
                    addressApproval_View = channel.LoadAddressApprovalInfo(Guid.Parse(id));
                    var res = preApprovalChannel.AddressApprove(addressApproval_View, "");
                    if (res == 1)
                    {
                        P_AddressApproveHistory addressApproveHistory = new P_AddressApproveHistory()
                        {
                            ID = Guid.NewGuid(),
                            DA_ID = Guid.Parse(id),
                            UserName = userInfo.Name,
                            UserId = userInfo.UserId,
                            ActionType = 1,
                            ApproveDate = DateTime.Now,
                            Comments = "",
                            type = 0,
                            IsDelete = 0
                        };
                        //添加审批记录
                        //var historyRes = preApprovalChannel.AddAddressApproveHistory(addressApproveHistory);
                        //if (historyRes == 0)
                        //{
                        //    LogHelper.Info($"审批成功，添加审批记录失败 - [{ id},{userInfo.UserId}]");
                        //}
                        // 发用户消息
                        addressApproval.ID = Guid.Parse(id);
                        WxMessageHandler.GetInstance().SendAddressApprovalStateChangeMessageToUser(addressApproval);
                        LogHelper.Info($"地址申请审批通过 - [{ id},{userInfo.UserId}]");
                    }
                    else
                    {
                        LogHelper.Info($" 审批地址申请失败 - [{id}]");
                    }
                }
                return Json(new { state = 1, txt = "地址申请审批通过。" });
            }
            catch (Exception ex)
            {
                LogHelper.Error("Exception AddressApprove", ex);
                return Json(new { state = 0, txt = "地址申请审批失败，请刷新页面后重试。", errCode = 9007 });
            }

        }

        public JsonResult SendSpecialWxMessage(Guid id)
        {
            P_AddressApproval addressApproval = new P_AddressApproval();
            addressApproval.ID = id;
            P_AddressApproval_View addressApproval_View = new P_AddressApproval_View();
            var preApprovalChannel = PreApprovalClientChannelFactory.GetChannel();
            var channel = PreApprovalClientChannelFactory.GetChannel();
            addressApproval_View = channel.LoadAddressApprovalInfo(id);
            P_USERINFO userInfo = Session[MealH5.Util.ConstantHelper.CURRENTWXUSER] as P_USERINFO;
            string msg = "";
            try
            {
                var res = preApprovalChannel.AddressApprove(addressApproval_View, "该医院已被删除");
                if (res == 1)
                {
                    P_AddressApproveHistory addressApproveHistory = new P_AddressApproveHistory()
                    {
                        ID = Guid.NewGuid(),
                        DA_ID = id,
                        UserName = userInfo.Name,
                        UserId = userInfo.UserId,
                        ActionType = 2,
                        ApproveDate = DateTime.Now,
                        Comments = "该医院已被删除",
                        type = 0,
                        IsDelete = 0
                    };

                    WxMessageHandler.GetInstance().SendSpecialMessageToUser(addressApproval);
                    return Json(new { state = 1, txt = msg });
                }
                else
                {
                    LogHelper.Info($" 审批地址申请失败 - [{id}]");
                    return Json(new { state = 0, txt = "地址申请审批失败，请刷新页面后重试。", errCode = 9007 });
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("Exception SendWxMessage", ex);
                return Json(new { state = 0, txt = "", errCode = 9007 });
            }
        }
        #endregion
    }
}