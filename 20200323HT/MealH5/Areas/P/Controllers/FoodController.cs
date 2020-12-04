using iPathFeast.API.Client;
using iPathFeast.ApiEntity;
using MealAdmin.Entity;
using MealAdmin.Entity.Enum;
using MealAdmin.Entity.Helper;
using MealAdminApiClient;
using MealH5.Areas.P.Filter;
using MealH5.Areas.P.Handler;
using MealH5.Filter;
using MealH5.Models;
using MeetingMealApiClient;
using MeetingMealEntity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XFramework.XInject.Attributes;
using XFramework.XUtil;
using MealH5.Handler;

namespace MealH5.Areas.P.Controllers
{
    #region 订餐主流程
    /// <summary>
    /// 订餐主流程
    /// </summary>
    [WxUserFilter]
    public class FoodController : BaseController
    {
        [Autowired]
        ApiV1Client apiClient { get; set; }

        void SetResponseFlag()
        {
            Response.Headers.Add("mic", "1");
        }


        #region 获取当前订单信息
        /// <summary>
        /// 获取当前订单信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult NowOrderInfo()
        {
            return Json(new { state = 1, data = OrderInfo });
        }
        #endregion

        #region 选择医院
        /// <summary>
        /// 选择医院
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [iPathOAuthFilter(MappingKey = "0x0013", CallBackUrl = true)]
        public ActionResult ChooseHospital()
        {
            SetResponseFlag();
            // 标记当前是新单
            ChangeOrderID = null;
            OrderInfo = null;

            var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
            var listMarket = baseDataChannel.LoadMarket();

            var channelUser = UserInfoClientChannelFactory.GetChannel();
            var channelBase = BaseDataClientChannelFactory.GetChannel();

            var userId = CurrentWxUser.UserId;
            var userInfo = channelUser.FindByUserId(userId);
            if (userInfo == null)
            {
                userInfo = new P_USERINFO()
                {
                    UserId = userId
                };
            }

            if (userInfo.IsCheckedStatement == 0)
            {
                return RedirectToAction("Statement", "Other");
            }

            var listGroup = channelBase.LoadUserGroup(userId).Select(a => a.GroupType).ToArray();

            if (listGroup.Contains(GroupType.ServPause))
            {
                userInfo.IsServPause = 1;
            }
            if (listGroup.Contains(GroupType.OutSideHT))
            {
                userInfo.IsOutSideHT = 1;
            }
            CurrentWxUser = userInfo;
            var market = CurrentWxUser.Market;
            market = market ?? "";
            ViewBag.Market = market;
            ViewBag.listMarket = listMarket.Where(a => a.Name == market).ToList();
            ViewBag.IsServPause = CurrentWxUser.IsServPause;
            ViewBag.IsOutSideHT = CurrentWxUser.IsOutSideHT;
            ViewBag.CurrentUserId = CurrentWxUser.UserId;
            return View();
        }
        #endregion

        #region 保存选择医院
        /// <summary>
        /// 保存选择医院
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ChooseHospital")]
        public JsonResult _ChooseHospital(P_ChooseHospital entity)
        {
            var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
            var listMarket = baseDataChannel.LoadMarket();
            var hospitalChannel = HospitalClientChannelFactory.GetChannel();
            var Hospital = hospitalChannel.FindByCode(entity.addressCode);
            var market = listMarket.First(a => a.Name == entity.market);

            entity.hospital = Hospital.GskHospital;
            entity.invoiceTitle = market.InvoiceTitle;
            entity.dutyParagraph = market.DutyParagraph;
            entity.isExternal = Hospital.External;
            entity.longitude = Hospital.Longitude;
            entity.latitude = Hospital.Latitude;
            entity.addressCode = Hospital.HospitalCode;
            HospitalInfo = entity;
            return Json(new { state = 1 });
        }
        #endregion

        #region 查省份
        /// <summary>
        /// 查省份
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LoadProvince(string Type, string TA)
        {
            var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
            var listProvince = baseDataChannel.LoadProvince(Type, TA);
            //if (CurrentWxUser.TerritoryCode != null && CurrentWxUser.TerritoryCode != "")
            //{
            //    listProvince = baseDataChannel.LoadProvinceByHos(Type, CurrentWxUser.UserId);
            //}
            return Json(new { state = 1, rows = listProvince });
        }
        #endregion

        #region 查城市
        /// <summary>
        /// 查城市
        /// </summary>
        /// <param name="provinceId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LoadCity(int provinceId, string Type, string TA)
        {
            var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
            var listCity = baseDataChannel.LoadCity(provinceId, Type, TA);
            //if (CurrentWxUser.TerritoryCode != null && CurrentWxUser.TerritoryCode != "")
            //{
            //    listCity = baseDataChannel.LoadCityByHos(provinceId, Type, CurrentWxUser.UserId, CurrentWxUser.TerritoryCode);
            //}
            return Json(new { state = 1, rows = listCity });
        }
        #endregion

        #region 查城市
        /// <summary>
        /// 查城市
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult FindCity(int cityId)
        {
            var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
            var city = baseDataChannel.FindCity(cityId);
            return Json(new { state = 1, data = city });
        }
        #endregion

        #region 查医院
        /// <summary>
        /// 查医院
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="market"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LoadHospital(int cityId, string market, string userid, string TA)
        {
            var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
            var listHospital = baseDataChannel.LoadHospital(cityId, market, TA);
            //if (CurrentWxUser.TerritoryCode != null && CurrentWxUser.TerritoryCode != "")
            //{
            //    listHospital = baseDataChannel.LoadHospitalByTaUser(cityId, market, CurrentWxUser.UserId, CurrentWxUser.TerritoryCode);
            //}
            return Json(new { state = 1, rows = listHospital });
        }
        #endregion

        #region 搜索医院
        /// <summary>
        /// 搜索医院
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="market"></param>
        /// <returns></returns>
        public JsonResult SearchHospital(string keyword, int province, int city, string market, string TA)
        {
            var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
            var listHospital = baseDataChannel.SearchHospital(keyword, province, city, market, TA);
            //if (CurrentWxUser.TerritoryCode != null && CurrentWxUser.TerritoryCode != "")
            //{
            //    listHospital = baseDataChannel.SearchHospitalByTA(keyword, province, city, market, CurrentWxUser.UserId, CurrentWxUser.TerritoryCode);
            //}
            return Json(new { state = 1, rows = listHospital });
        }
        #endregion

        #region 根据医院编码搜索医院
        /// <summary>
        /// 根据医院编码搜索医院
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="market"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public JsonResult SearchHospitalByCode(string keyword, int province, int city, string market, string TA)
        {
            var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
            var listHospital = baseDataChannel.SearchHospitalByCode(keyword, province, city, market, TA);
            //if (CurrentWxUser.TerritoryCode != null && CurrentWxUser.TerritoryCode != "")
            //{
            //    listHospital = baseDataChannel.SearchHospitalByCodeTA(keyword, province, city, market, CurrentWxUser.UserId, CurrentWxUser.TerritoryCode);
            //}
            return Json(new { state = 1, rows = listHospital });
        }
        #endregion

        #region 查TA
        /// <summary>
        /// 查TA
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="market"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LoadTA(string market)
        {
            var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
            var listTA = baseDataChannel.LoadTAByMarket(market);
            //if (CurrentWxUser.TerritoryCode != null && CurrentWxUser.TerritoryCode != "")
            //{
            //    listTA = baseDataChannel.LoadTAByMarketUserId(market, CurrentWxUser.UserId);
            //}
            return Json(new { state = 1, rows = listTA });
        }
        #endregion

        #region 选择CN号
        /// <summary>
        /// 选择CN号
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [OrderInfoFilter]
        public ActionResult ChooseCN()
        {
            var nowDate = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd HH:mm:ss");
            var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
            var listMeeting = baseDataChannel.LoadMeeting(CurrentWxUser.UserId, nowDate);

            ViewBag.listMeeting = listMeeting;
            ViewBag.P_ChooseHospital = HospitalInfo;
            return View();
        }
        #endregion

        #region 保存CN号
        /// <summary>
        /// 保存CN号
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ChooseCN")]
        [OrderInfoFilter]
        public JsonResult _ChooseCN(string code)
        {
            var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
            var meeting = baseDataChannel.FindMeeting(code);

            OrderInfo.CnCode = code;
            OrderInfo.meeting = new P_Meeting()
            {
                code = meeting.Code,
                title = meeting.Title,
                budgetTotal = meeting.BudgetTotal,
                submittedDate = meeting.SubmittedDate,
                approvedDate = meeting.ApprovedDate,
                status = meeting.Status,
                pendingWith = meeting.PendingWith,
                userId = meeting.UserId,
                userName = meeting.UserId
            };
            return Json(new { state = 1 });
        }
        #endregion        

        #region 查找会议信息
        /// <summary>
        /// 查找会议信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult FindMeeting(string code)
        {
            var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
            var listMeeting = baseDataChannel.FindMeeting(code);

            return Json(new { state = 1, data = listMeeting });
        }
        #endregion

        #region 转到下单页面
        /// <summary>
        /// 转到下单页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [OrderFilter]
        public ActionResult Order(string supplier, int? state, string sendTime)
        {
            //var code = OrderInfo.CnCode;
            //ChangeMeeting(code);
            ViewBag.supplier = supplier;
            ViewBag.state = state;
            ViewBag.sendTime = sendTime;
            return View();
        }
        #endregion

        #region 更新会议信息
        //更新会议信息
        public void ChangeMeeting(string code)
        {
            var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
            var meeting = baseDataChannel.FindMeeting(code);

            OrderInfo.meeting = new P_Meeting()
            {
                code = meeting.Code,
                title = meeting.Title,
                budgetTotal = meeting.BudgetTotal,
                submittedDate = meeting.SubmittedDate,
                approvedDate = meeting.ApprovedDate,
                status = meeting.Status,
                pendingWith = meeting.PendingWith,
                userId = meeting.UserId,
                userName = meeting.UserId
            };
        }
        #endregion

        #region 获取当前日期可选送饭时间区间
        /// <summary>
        /// 获取当前日期可选送饭时间区间
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LoadNextHoliday()
        {
            var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
            var holiday = baseDataChannel.FindNextRange(DateTime.Now);
            return Json(new { state = 1, data = new { holiday = holiday, now = DateTime.Now.Date } });
        }
        #endregion

        #region 保存订单配送等信息
        /// <summary>
        /// 保存订单配送等信息
        /// </summary>
        /// <param name="details"></param>
        /// <returns></returns>
        [HttpPost]
        [OrderInfoFilter]
        public JsonResult Details(P_OrderDetails details)
        {
            details.createTime = DateTime.Now;
            OrderInfo.details = details;
            int needApprove = -1;

            if (!IsNewOrder)
            {
                // 检查改单是否需要MMCoE审批
                var ID = ChangeOrderID.Value;
                var orderChannel = OrderApiClientChannelFactory.GetChannel();
                var oldOrder = orderChannel.FindByID(ID);
                if (oldOrder.MMCoEApproveState == MMCoEApproveState.APPROVESUCCESS)
                {
                    needApprove = 0;
                }
                else
                {
                    needApprove = 1;
                }
            }

            return Json(new { state = 1, needApprove = needApprove });
        }
        #endregion

        #region 从微信消息直接跳入修改MMCoE
        /// <summary>
        /// 从微信消息直接跳入修改MMCoE
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [iPathOAuthFilter(MappingKey = "0x0009", CallBackUrl = true)]
        public ActionResult MMCoEShell(Guid id)
        {
            var preApprovalChannel = PreApprovalClientChannelFactory.GetChannel();
            var preApproval = preApprovalChannel.LoadPreApprovalInfo(id);
            //if (int.Parse(preApproval.State) != MealAdmin.Entity.Enum.PreApprovalState.MMCOEREJECT)
            //{
            //    // 非预定成功并且非审批拒绝的状态，不可改单
            //    return Json(new { state = 0, txt = "当前预申请状态不允许修改", errCode = 9006 });
            //}
            PreApproval = preApproval;
            ViewBag.State = preApproval.State;
            return RedirectToAction("MMCoE");
        }
        #endregion

        #region 转到MMCoE申报页面
        /// <summary>
        /// 转到MMCoE申报页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MMCoE()
        {
            return View();
        }
        #endregion

        #region 保存MMCoE申报
        /// <summary>
        /// 保存MMCoE申报
        /// </summary>
        /// <param name="images"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("MMCoE")]
        [OrderInfoFilter]
        public JsonResult _MMCoE(string images)
        {
            OrderInfo.mmCoE = images;
            return Json(new { state = 1 });
        }
        #endregion

        #region 下单
        /// <summary>
        /// 下单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Order")]
        [OrderFilter]
        public JsonResult _Order()
        {
            if (IsNewOrder)
            {
                // 下新单
                return NewOrder();
            }
            else
            {
                // 改旧单
                return ChangeOrder();
            }
        }

        /// <summary>
        /// 下新单
        /// </summary>
        /// <returns></returns>
        private JsonResult NewOrder()
        {
            var ID = Guid.NewGuid();
            var customerID = ConfigurationManager.AppSettings["CustomerID"];
            var CreateDate = DateTime.Now;
            var XmsOrderId = string.Empty;
            var _orderInfo = OrderInfo;
            int RandKey = new Random().Next(1, 999);
            var two = (RandKey < 10 ? "00" + RandKey : (RandKey < 100 ? "0" + RandKey : RandKey.ToString()));
            var _channel = _orderInfo.supplier;
            var _date = DateTime.Now.ToString("yyMMddHHmmss");
            var _enterpriseOrderId = _channel.ToUpper() + "-" + _date + two;
            var foodList = _orderInfo.foods.foods.Select(a => a.ToFoodRequest()).ToArray();
            var foods = new List<iPathFeast.ApiEntity.Food>();
            foreach (var item in foodList)
            {
                foods.Add(new iPathFeast.ApiEntity.Food()
                {
                    foodId = item.foodId,
                    foodName = item.foodName,
                    count = Convert.ToInt32(item.count)
                });
            }

            if (string.IsNullOrEmpty(_orderInfo.mmCoE) && _orderInfo.details.attendCount < 60)
            {
                // 调用小秘书下单
                //var openApiChannel = OpenApiChannelFactory.GetChannel();
                //var _res = openApiChannel.createOrder2(ID.ToString(), string.Empty, "0", _orderInfo);
                var req = new CreateOrderReq()
                {
                    _Channels = _channel,
                    enterpriseOrderId = _enterpriseOrderId,
                    oldiPathOrderId = string.Empty,
                    CustomerID = Guid.Parse(customerID),
                    sendTime = _orderInfo.details.deliverTime.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                    foodFee = _orderInfo.foods.foodFee.ToString(),
                    packageFee = _orderInfo.foods.packageFee.ToString(),
                    sendFee = _orderInfo.foods.sendFee.ToString(),
                    totalFee = _orderInfo.foods.allPrice.ToString(),
                    invoiceTitle = _orderInfo.hospital.invoiceTitle + " - " + _orderInfo.hospital.dutyParagraph,
                    orderTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    remark = _orderInfo.details.remark,
                    dinnerName = _orderInfo.details.consignee,
                    dinnernum = _orderInfo.details.attendCount.ToString(),
                    phone = _orderInfo.details.phone,
                    address = _orderInfo.hospital.address + " - " + _orderInfo.details.deliveryAddress,
                    resId = _orderInfo.foods.resId,
                    longitude = _orderInfo.hospital.longitude,
                    latitude = _orderInfo.hospital.latitude,
                    hospitalId = _orderInfo.hospital.hospital,
                    foods = foods,
                    cityId = _orderInfo.hospital.city,
                    cn = _orderInfo.CnCode,
                    cnAmount = _orderInfo.meeting.budgetTotal.ToString(),
                    mudId = _orderInfo.meeting.userId,
                    typeId = "0"
                };
                var _res = apiClient.CreateOrder(req);
                if (_res.errorCode != "0")
                {
                    return Json(new { state = 0, txt = _res.errorMsg, errCode = _res.errorCode });
                }
                XmsOrderId = _res.iPathOrderId;
            }
            // 写入数据库订单记录
            var channel = OrderApiClientChannelFactory.GetChannel();
            //var res = channel.Add(ID, CurrentWxUser.UserId, XmsOrderId, _orderInfo, _enterpriseOrderId,_channel);
            //if (res > 0)
            //{
            //    OrderInfo = null;
            //    var order = channel.FindByID(ID);
            //    WxMessageHandler.GetInstance().SendMessageToUser(CurrentWxUser.UserId, order);
            //    return Json(new { state = 1, isChange = 0 });
            //}
            return Json(new { state = 0, txt = "下单失败", errCode = "9001" });
        }

        /// <summary>
        /// 改单流程
        /// </summary>
        /// <returns></returns>
        private JsonResult ChangeOrder()
        {
            var ID = ChangeOrderID.Value;
            var customerID = ConfigurationManager.AppSettings["CustomerID"];
            var ChangeID = Guid.NewGuid();
            var CreateDate = DateTime.Now;
            var XmsOrderId = string.Empty;
            var _orderInfo = OrderInfo;


            var orderChannel = OrderApiClientChannelFactory.GetChannel();
            var oldOrder = orderChannel.FindByID(ID);

            #region
            //CreateOrderResponse xmlOrderRes = null;
            //if (_orderInfo.details.attendCount < 60 || oldOrder.MMCoEApproveState == MMCoEApproveState.APPROVESUCCESS)
            //{
            //    // 调用小秘书改单
            //    var openApiChannel = OpenApiChannelFactory.GetChannel();
            //    var _res = openApiChannel.createOrder2(ChangeID.ToString(), oldOrder.XmsOrderId,
            //        string.IsNullOrEmpty(oldOrder.XmsOrderId) ? "0" : "1", _orderInfo);
            //    if (_res.code != "200")
            //    {
            //        return Json(new { state = 0, txt = "改单失败", errCode = _res.code });
            //    }
            //    xmlOrderRes = _res;
            //    if (string.IsNullOrEmpty(oldOrder.XmsOrderId))
            //    {
            //        XmsOrderId = xmlOrderRes.result.xmsOrderId;
            //    }

            //}
            #endregion

            var _enterpriseOrderId = oldOrder.EnterpriseOrderId;
            if (string.IsNullOrEmpty(_enterpriseOrderId))
            {
                int RandKey = new Random().Next(1, 999);
                var two = (RandKey < 10 ? "00" + RandKey : (RandKey < 100 ? "0" + RandKey : RandKey.ToString()));
                var _channel = _orderInfo.supplier;
                var _date = DateTime.Now.ToString("yyMMddHHmmss");
                _enterpriseOrderId = _channel.ToUpper() + "-" + _date + two;
            }

            var foodList = _orderInfo.foods.foods.Select(a => a.ToFoodRequest()).ToArray();
            var foods = new List<iPathFeast.ApiEntity.Food>();
            foreach (var item in foodList)
            {
                foods.Add(new iPathFeast.ApiEntity.Food()
                {
                    foodId = item.foodId,
                    foodName = item.foodName,
                    count = Convert.ToInt32(item.count)
                });
            }
            var type = string.IsNullOrEmpty(oldOrder.XmsOrderId) ? "0" : "1";
            if (_orderInfo.details.attendCount < 60 || oldOrder.MMCoEApproveState == MMCoEApproveState.APPROVESUCCESS)
            {
                // 调用小秘书下单
                //var openApiChannel = OpenApiChannelFactory.GetChannel();
                //var _res = openApiChannel.createOrder2(ID.ToString(), string.Empty, "0", _orderInfo);
                var req = new CreateOrderReq()
                {
                    _Channels = oldOrder.Channel,
                    enterpriseOrderId = _enterpriseOrderId,
                    oldiPathOrderId = oldOrder.XmsOrderId,
                    CustomerID = Guid.Parse(customerID),
                    sendTime = _orderInfo.details.deliverTime.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                    foodFee = _orderInfo.foods.foodFee.ToString(),
                    packageFee = _orderInfo.foods.packageFee.ToString(),
                    sendFee = _orderInfo.foods.sendFee.ToString(),
                    totalFee = _orderInfo.foods.allPrice.ToString(),
                    invoiceTitle = _orderInfo.hospital.invoiceTitle + " - " + _orderInfo.hospital.dutyParagraph,
                    orderTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    remark = _orderInfo.details.remark,
                    dinnerName = _orderInfo.details.consignee,
                    dinnernum = _orderInfo.details.attendCount.ToString(),
                    phone = _orderInfo.details.phone,
                    address = _orderInfo.hospital.address + " - " + _orderInfo.details.deliveryAddress,
                    resId = _orderInfo.foods.resId,
                    longitude = _orderInfo.hospital.longitude,
                    latitude = _orderInfo.hospital.latitude,
                    hospitalId = _orderInfo.hospital.hospital,
                    foods = foods,
                    cityId = _orderInfo.hospital.city,
                    cn = _orderInfo.CnCode,
                    cnAmount = _orderInfo.meeting.budgetTotal.ToString(),
                    mudId = _orderInfo.meeting.userId,
                    typeId = type
                };
                var _res = apiClient.CreateOrder(req);
                if (_res.errorCode != "0")
                {
                    return Json(new { state = 0, txt = _res.errorMsg, errCode = _res.errorCode });
                }
                if (string.IsNullOrEmpty(oldOrder.XmsOrderId))
                {
                    XmsOrderId = _res.iPathOrderId;
                }
            }

            // 写入改单后的新数据
            //var res = orderChannel.Change(ID, ChangeID, _orderInfo);
            //if (res > 0)
            //{
            //    OrderInfo = null;
            //    if (!string.IsNullOrEmpty(XmsOrderId))
            //    {
            //        // 从未在小秘书下单过的改单，下单成功后应写入小秘书单号
            //        orderChannel.SaveXmsOrderId(ID, XmsOrderId);
            //    }

            //    oldOrder = orderChannel.FindByID(ID);
            //    WxMessageHandler.GetInstance().SendMessageToUser(CurrentWxUser.UserId, oldOrder);
            //    return Json(new { state = 1, isChange = 1 });
            //}
            return Json(new { state = 0, txt = "改单失败", errCode = "9001" });
        }
        #endregion

        [HttpGet]
        [iPathOAuthFilter(MappingKey = "0x0010", CallBackUrl = true)]
        public ActionResult ChoosePreApproval()
        {
            WeChatOrderInfo = null;
            ChangeOrderID = null;
            var preApprovalClient = PreApprovalClientChannelFactory.GetChannel();
            var userId = CurrentWxUser.UserId;
            var channelBase = BaseDataClientChannelFactory.GetChannel();
            var channelUser = UserInfoClientChannelFactory.GetChannel();
            var userInfo = channelUser.FindByUserId(userId);

            var listGroup = channelBase.LoadUserGroup(userId).Select(a => a.GroupType).ToArray();
            if (listGroup.Contains(GroupType.ServPause))
            {
                userInfo.IsServPause = 1;
            }
            var listHTCode = preApprovalClient.LoadHTCode(userId);
            //判断当前时间是否在可订当日单时间范围
            var channel = BaseDataClientChannelFactory.GetChannel();
            var config = channel.GetTimeConfig();
            DateTime timeNow = DateTime.Now;
            if (!(DateTime.Compare(Convert.ToDateTime(config._4_2_ThatDayCanOrderMealTimeEnd), timeNow) >= 0))
            {
                listHTCode = listHTCode.Where(p => p.MeetingDate.Value.Day != timeNow.Day).ToList();
            }
            CurrentWxUser = userInfo;
            ViewBag.listHTCode = listHTCode;
            ViewBag.IsServPause = CurrentWxUser.IsServPause;
            return View();
        }

        #region 查找预申请信息

        [HttpPost]
        public JsonResult FindPreApproval(string HTCode)
        {
            var preApprovalClient = PreApprovalClientChannelFactory.GetChannel();
            var list = preApprovalClient.FindPreApprovalByHTCode(HTCode);
            var preApproval = new List<P_PreApproval_View>();
            foreach (var i in list)
            {
                preApproval.Add(GetDisplayObj(i));
            }
            return Json(new { state = 1, data = preApproval });
        }

        #endregion

        #region 预申请查询日期处理
        private P_PreApproval_View GetDisplayObj(P_PreApproval itm)
        {
            P_PreApproval_View rtnData = new MealAdmin.Entity.P_PreApproval_View();
            rtnData.ApplierMobile = itm.ApplierMobile;
            rtnData.ApplierMUDID = itm.ApplierMUDID;
            rtnData.ApplierName = itm.ApplierName;
            rtnData.AttendCount = itm.AttendCount;
            rtnData.BudgetTotal = itm.BudgetTotal.ToString("N");
            rtnData.BUHeadApproveDate = itm.BUHeadApproveDate != null ? itm.BUHeadApproveDate.Value.ToString("yyyy-MM-dd") : string.Empty;
            rtnData.BUHeadApproveTime = itm.BUHeadApproveDate != null ? itm.BUHeadApproveDate.Value.ToString("HH:mm:ss") : string.Empty;
            rtnData.BUHeadMUDID = itm.BUHeadMUDID;
            rtnData.BUHeadName = itm.BUHeadName;
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
            rtnData.Market = itm.Market;
            rtnData.VeevaMeetingID = itm.VeevaMeetingID;

            //rtnData.MeetingDate = itm.MeetingDate != null ? itm.MeetingDate.Value.ToString("") : string.Empty;
            rtnData.MeetingTime = itm.MeetingDate != null ? itm.MeetingDate.Value.ToString("yyyy-MM-dd HH:mm") : string.Empty;
            rtnData.MeetingName = itm.MeetingName;
            rtnData.ModifyDate = itm.ModifyDate != null ? itm.ModifyDate.Value.ToString("yyyy-MM-dd") : string.Empty;
            rtnData.ModifyTime = itm.ModifyDate != null ? itm.ModifyDate.Value.ToString("HH:mm:ss") : string.Empty;
            rtnData.Province = itm.Province;
            rtnData.ReAssignBUHeadApproveDate = itm.ReAssignBUHeadApproveDate != null ? itm.ReAssignBUHeadApproveDate.Value.ToString("yyyy-MM-dd") : string.Empty;
            rtnData.ReAssignBUHeadApproveTime = itm.ReAssignBUHeadApproveDate != null ? itm.ReAssignBUHeadApproveDate.Value.ToString("HH:mm:ss") : string.Empty;
            rtnData.ReAssignBUHeadMUDID = itm.ReAssignBUHeadMUDID;
            rtnData.ReAssignBUHeadName = itm.ReAssignBUHeadName;
            rtnData.TA = itm.TA;
            rtnData.HTType = itm.HTType;
            if (itm.State == "0" || itm.State == "1" || itm.State == "3")
            {
                rtnData.State = "提交成功";
            }
            if (itm.State == "5" || itm.State == "6")
            {
                rtnData.State = "审批通过";
            }
            if (itm.State == "2" || itm.State == "4")
            {
                rtnData.State = "审批被驳回";
            }
            rtnData.IsFreeSpeaker = itm.IsFreeSpeaker;

            rtnData.HospitalAddressCode = itm.HospitalAddressCode;
            return rtnData;
        }
        #endregion

        public JsonResult CheckPreApprovalState(string HTCode)
        {
            var preApprovalChannel = PreApprovalClientChannelFactory.GetChannel();
            var preAproval = preApprovalChannel.CheckPreApprovalState(HTCode);
            return Json(new { state = 1, data = preAproval });
        }


        #region 判断是否超出预算
        /// <summary>
        /// 判断是否超出预算
        /// </summary>
        /// <param name="cityID"></param>
        /// <returns></returns>
        public JsonResult IsSubmit(string hospitalId, decimal budget, int attendance, int state)
        {

            var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
            var res = baseDataChannel.FindCityBudget(hospitalId);
            var mss = "";
            //院内会
            if (res.IsOut == 0)
            {
                if (budget / attendance > 60)
                {
                    if (state == 1)
                    {
                        mss = "您的预申请已超出人均60元标准，请修改预申请";
                    }
                    else
                    {
                        mss = "您的订单已超出人均60元标准，请修改订单";
                    }
                    return Json(new { state = 1, data = false, txt = mss });
                }
                else if (budget > 0 && budget / attendance <= 1)
                {
                    mss = "您的订单人均<=1元，请修改订单";
                    return Json(new { state = 1, data = false, txt = mss });
                }
                return Json(new { state = 1, data = true });
            }
            //院外会
            else
            {
                //非一二线城市
                if (string.IsNullOrEmpty(res.CityName))
                {
                    if (budget / attendance > 100)
                    {
                        if (state == 1)
                        {
                            mss = "您的预申请已超出人均100元标准，请修改预申请";
                        }
                        else
                        {
                            mss = "您的订单已超出人均100元标准，请修改订单";
                        }
                        return Json(new { state = 1, data = false, txt = mss });
                    }
                    else if (budget > 0 && budget / attendance <= 1)
                    {
                        mss = "您的订单人均<=1元，请修改订单";
                        return Json(new { state = 1, data = false, txt = mss });
                    }
                    return Json(new { state = 1, data = true });
                }
                //一二线城市
                else
                {
                    if (budget / attendance > 150)
                    {
                        if (state == 1)
                        {
                            mss = "您的预申请已超出人均150元标准，请修改预申请";
                        }
                        else
                        {
                            mss = "您的订单已超出人均150元标准，请修改订单";
                        }
                        return Json(new { state = 1, data = false, txt = mss });
                    }
                    else if (budget > 0 && budget / attendance <= 1)
                    {
                        mss = "您的订单人均<=1元，请修改订单";
                        return Json(new { state = 1, data = false, txt = mss });
                    }
                    return Json(new { state = 1, data = true });
                }
            }
        }
        #endregion

        #region 判断是否超出预算
        /// <summary>
        /// 判断是否超出预算
        /// </summary>
        /// <param name="cityID"></param>
        /// <returns></returns>
        public JsonResult IsBindRes(string hospitalId)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("zh-cn");
            var channel = HospitalClientChannelFactory.GetChannel();
            var hospital = channel.FindByCode(hospitalId);

            if (hospital == null || string.IsNullOrEmpty(hospital.Name))
            {
                return Json(new { state = 1, data = false, txt = "该目标医院暂不支持送餐服务，是否继续？" });
            }

            //var openApiChannel = OpenApiChannelFactory.GetChannel();
            //var res = openApiChannel.queryRes(hospital.ID.ToString(), hospital.Address, hospital.Latitude, hospital.Longitude);
            //if (string.IsNullOrEmpty(supplier))
            //{
            //    supplier = "";
            //}

            var req = new SearchRestaurantReq()
            {
                _Channels = "",
                hospitalId = hospital.HospitalCode,
                address = hospital.Address,
                latitude = hospital.Latitude,
                longitude = hospital.Longitude,
                keyword = ""
            };
            var res = apiClient.SearchRestaurant(req);

            if (res == null)
            {
                return Json(new { state = 1, data = false, txt = "该目标医院暂不支持送餐服务，是否继续？" });
            }

            res = res.OrderBy(a => a.resName).ToList();

            #region 对接餐厅管理平台，排除黑名单餐厅
            //比对黑名单数据，对接餐厅管理平台
            if (WebConfigHandler.IsLoadRestaurantData == "1")
            {
                var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
                var resstatelist = baseDataChannel.LoadRestaurantState();
                var resstateResID = resstatelist.Select(x => x.ResId).ToList();
                res = res.Where(x => resstateResID.Contains(x.resId) || x.resName.Contains("堂食")).ToList();
                resstatelist = resstatelist.Where(p => p.Status == 0).ToList();
                foreach (var item in resstatelist)
                {
                    var inlist = res.FirstOrDefault(p => p.resId == item.ResId);
                    if (inlist != null)
                    {
                        res.Remove(inlist);
                    }
                }
            }
            #endregion

            //IComparer<SearchRestaurantRes> comparer = new Restaurant();
            //res.Sort(comparer);
            if (res.Count > 0)
            {
                return Json(new { state = 1, data = true });
            }
            else
            {
                return Json(new { state = 1, data = false, txt = "该目标医院暂不支持送餐服务，是否继续？" });
            }

        }
        #endregion

    }
    #endregion
}