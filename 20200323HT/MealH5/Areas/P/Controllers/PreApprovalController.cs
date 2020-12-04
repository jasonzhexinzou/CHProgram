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

namespace MealH5.Areas.P.Controllers
{
    /// <summary>
    /// 提交预申请操作控制器
    /// </summary>
    [WxUserFilter]
    public class PreApprovalController : BaseController
    {
        // GET: P/PreApproval
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
        public JsonResult NowOrder()
        {
            return Json(new { state = 1, data = WeChatOrderInfo });
        }
        #endregion

        #region 保存选择医院
        /// <summary>
        /// 保存选择医院
        /// </summary>
        /// <returns></returns>
        //[HttpPost]
        //[ActionName("Submit")]
        //public JsonResult _Submit(P_ChooseHospital entity)
        //{
        //    var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
        //    var listMarket = baseDataChannel.LoadMarket();
        //    var Hospital = baseDataChannel.FindHospital(entity.hospital);
        //    var market = listMarket.First(a => a.Name == entity.market);

        //    entity.hospital = Hospital.GskHospital;
        //    entity.invoiceTitle = market.InvoiceTitle;
        //    entity.dutyParagraph = market.DutyParagraph;
        //    entity.isExternal = Hospital.External;
        //    entity.longitude = Hospital.Longitude;
        //    entity.latitude = Hospital.Latitude;
        //    HospitalInfo = entity;

        //    return Json(new { state = 1 });
        //}
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
            var listProvince = new List<P_PROVINCE>();
            if (CurrentWxUser.TerritoryCode != null && CurrentWxUser.TerritoryCode != "")
            {
                listProvince = baseDataChannel.LoadProvinceByHos(Type, CurrentWxUser.UserId, CurrentWxUser.TerritoryCode);
            } else
            {
                listProvince = baseDataChannel.LoadProvince(Type, TA);
            }
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
            var listCity = new List<P_CITY>();
            if (CurrentWxUser.TerritoryCode != null && CurrentWxUser.TerritoryCode != "")
            {
                listCity = baseDataChannel.LoadCityByHos(provinceId, Type, CurrentWxUser.UserId, CurrentWxUser.TerritoryCode);
            } else
            {
                listCity = baseDataChannel.LoadCity(provinceId, Type, TA);
            }
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
        public JsonResult LoadHospital(int cityId, string market, string TA)
        {
            var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
            var listHospital = new List<P_HOSPITAL>();
            if (CurrentWxUser.TerritoryCode != null && CurrentWxUser.TerritoryCode != "")
            {
                listHospital = baseDataChannel.LoadHospitalByTaUser(cityId, market, CurrentWxUser.UserId, CurrentWxUser.TerritoryCode);
            } else
            {
                listHospital = baseDataChannel.LoadHospital(cityId, market, TA);
            }
            return Json(new { state = 1, rows = listHospital });
        }

        /// <summary>
        /// 查医院
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="market"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LoadHospitalByTaUser(int cityId, string market, string userid, string territoryCode)
        {
            var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
            var listHospital = baseDataChannel.LoadHospitalByTaUser(cityId, market, userid, territoryCode);
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
            var listHospital = new List<P_HOSPITAL>();
            if (CurrentWxUser.TerritoryCode != null && CurrentWxUser.TerritoryCode != "")
            {
                listHospital = baseDataChannel.SearchHospitalByTA(keyword, province, city, market, CurrentWxUser.UserId, CurrentWxUser.TerritoryCode);
            } else
            {
                listHospital = baseDataChannel.SearchHospital(keyword, province, city, market, TA);
            }
            return Json(new { state = 1, rows = listHospital });
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
        public ActionResult Order(string supplier)
        {
            //var code = OrderInfo.CnCode;
            //ChangeMeeting(code);
            ViewBag.supplier = supplier;
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
        [OrderFilter]
        public JsonResult Details(P_OrderDetails details)
        {
            details.createTime = DateTime.Now;
            WeChatOrderInfo.details = details;
            int needApprove = -1;

            if (!IsNewOrder)
            {
                //// 检查改单是否需要MMCoE审批
                //var ID = ChangeOrderID.Value;
                //var orderChannel = OrderApiClientChannelFactory.GetChannel();
                //var oldOrder = orderChannel.FindByID(ID);
                //if (oldOrder.MMCoEApproveState == MMCoEApproveState.APPROVESUCCESS)
                //{
                //    needApprove = 0;
                //}
                //else
                //{
                //    needApprove = 1;
                //}
                needApprove = 0;
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
        [iPathOAuthFilter(MappingKey = "xzy", CallBackUrl = true)]
        public ActionResult MMCoEShell(Guid id)
        {
            var orderChannel = OrderApiClientChannelFactory.GetChannel();
            var order = orderChannel.FindByID(id);
            if (order.State != OrderState.SCHEDULEDSUCCESS && order.State != OrderState.REJECT)
            {
                // 非预定成功并且非审批拒绝的状态，不可改单
                return Json(new { state = 0, txt = "当前订单状态不允许改单", errCode = 9006 });
            }
            ChangeOrderID = id;
            if (order.ChangeDetail == null)
            {
                OrderInfo = JsonConvert.DeserializeObject<P_Order>(order.Detail);
            }
            else
            {
                OrderInfo = JsonConvert.DeserializeObject<P_Order>(order.ChangeDetail);
            }

            return RedirectToAction("MMCoE");
        }
        #endregion

        #region 转到MMCoE申报页面
        /// <summary>
        /// 转到MMCoE申报页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [OrderFilter]
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
        [OrderFilter]
        public JsonResult _MMCoE(string images)
        {
            PreApproval.MMCoEImage = images;
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
            var _orderInfo = WeChatOrderInfo;
            //int RandKey = new Random().Next(1, 999);
            //var two = (RandKey < 10 ? "00" + RandKey : (RandKey < 100 ? "0" + RandKey : RandKey.ToString()));
            var _channel = _orderInfo.supplier;
            var _date = DateTime.Now.ToString("yyMMddHHmmssfff");
            var _enterpriseOrderId = _channel.ToUpper() + "-" + _date;
            //for (int i = 0; i < 2; i++)
            //{
            //    int RandKey = new Random().Next(0, 9);
            //    _enterpriseOrderId += RandKey;
            //}
            _enterpriseOrderId += GetOrderID(2);
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
            try
            {
                //2020 写入p_ordercache p_order 占用CN号 begin
                var channel = OrderApiClientChannelFactory.GetChannel();
                var res = channel.AddOrder(ID, CurrentWxUser.UserId, XmsOrderId, _orderInfo, _enterpriseOrderId, _channel);
                //2020 写入p_ordercache p_order 占用CN号 end
                if (res > 0)
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
                        invoiceTitle = _orderInfo.invoiceTitle + " - " + _orderInfo.dutyParagraph,
                        orderTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        remark = _orderInfo.details.remark,
                        dinnerName = _orderInfo.details.consignee,
                        dinnernum = _orderInfo.details.attendCount.ToString(),
                        phone = _orderInfo.details.phone,
                        address = _orderInfo.hospital.Address + " - " + _orderInfo.details.deliveryAddress,
                        resId = _orderInfo.foods.resId,
                        longitude = _orderInfo.hospital.Longitude,
                        latitude = _orderInfo.hospital.Latitude,
                        hospitalId = _orderInfo.hospital.HospitalCode,
                        foods = foods,
                        cityId = _orderInfo.hospital.CityId.ToString(),
                        cn = _orderInfo.preApproval.HTCode,
                        cnAmount = _orderInfo.preApproval.BudgetTotal.ToString(),
                        mudId = _orderInfo.preApproval.ApplierMUDID,
                        typeId = "0"
                    };
                    try
                    {
                        var _res = apiClient.CreateOrder(req);
                        if (_res == null)
                        {
                            LogHelper.Error("失败异常信息_RES:_res为空");
                        }
                        else
                        {
                            if (_res.errorCode == null)
                            {
                                LogHelper.Error("失败异常信息_RES:_res.errorCode为空");
                            }
                            else if (_res.iPathOrderId == null)
                            {
                                LogHelper.Error("失败异常信息_RES:_res.iPathOrderId为空");
                            }
                        }
                        //xms接口返回失败，删除2.0平台订单信息
                        if (_res.errorCode != "0")
                        {
                            LogHelper.Error("失败异常信息_RES:" + _res.errorCode);
                            try
                            {
                                var r = channel.DeleteOrder(ID, _orderInfo.preApproval.HTCode);
                                if (r > 0)
                                {
                                    return Json(new { state = 0, txt = _res.errorMsg, errCode = _res.errorCode });
                                }
                            }
                            catch (Exception ex)
                            {
                                var Touser = ConfigurationManager.AppSettings["OrderErrorUser"];
                                WxMessageHandler.GetInstance().SendOrderErrorMsgToUser(ID, _orderInfo.preApproval.HTCode, Touser, "", 1);
                                LogHelper.Error($"下单失败--删除2.0平台订单失败 - [{ID}]");
                                LogHelper.Error("失败异常信息:" + ex.Message);
                                return Json(new { state = 0, txt = "下单失败", errCode = "9001" });
                            }
                        }
                        //xms返回成功，将XmsOrderId更新到2.0平台数据库
                        if (_res.errorCode == "0")
                        {
                            try
                            {
                                XmsOrderId = _res.iPathOrderId;
                                LogHelper.Info("InterfaceSuccess:" + _res.iPathOrderId);

                                var r = channel.UpdateOrder(ID, XmsOrderId);
                                if (r > 0)
                                {
                                    WeChatOrderInfo = null;
                                    ChangeOrderID = null;
                                    var order = channel.FindByID(ID);
                                    WxMessageHandler.GetInstance().SendMessageToUser(CurrentWxUser.UserId, order);
                                    return Json(new { state = 1, isChange = 0 });
                                }
                                else
                                {
                                    var Touser = ConfigurationManager.AppSettings["OrderErrorUser"];
                                    WxMessageHandler.GetInstance().SendOrderErrorMsgToUser(ID, _orderInfo.preApproval.HTCode, Touser, XmsOrderId, 0);
                                    return Json(new { state = 1, isChange = 0 });
                                }
                            }
                            catch (Exception ex)
                            {
                                var Touser = ConfigurationManager.AppSettings["OrderErrorUser"];
                                WxMessageHandler.GetInstance().SendOrderErrorMsgToUser(ID, _orderInfo.preApproval.HTCode, Touser, XmsOrderId, 0);
                                LogHelper.Error($"下单失败--更新XMSOrderID失败-- [{ID}]");
                                LogHelper.Error("失败异常信息:" + ex.Message);
                                return Json(new { state = 1, isChange = 0 });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error($"下单失败 - [{ID}]");
                        LogHelper.Error("失败异常信息:" + ex.Message);
                        LogHelper.Error("CreateOrder下单返回异常，网络中断");
                        try
                        {
                            var r = channel.DeleteOrder(ID, _orderInfo.preApproval.HTCode);
                            if (r > 0)
                            {
                                var Touser = ConfigurationManager.AppSettings["OrderErrorUser"];
                                WxMessageHandler.GetInstance().SendOrderErrorMsgToUser(ID, _orderInfo.preApproval.HTCode, Touser, "", 6);
                            }
                        }
                        catch (Exception e)
                        {
                            var Touser = ConfigurationManager.AppSettings["OrderErrorUser"];
                            WxMessageHandler.GetInstance().SendOrderErrorMsgToUser(ID, _orderInfo.preApproval.HTCode, Touser, "", 1);
                            LogHelper.Error($"下单失败--删除2.0平台订单失败 - [{ID}]");
                            LogHelper.Error("失败异常信息:" + e.Message);
                            return Json(new { state = 0, txt = "下单失败", errCode = "9001" });
                        }
                        return Json(new { state = 0, txt = "下单失败", errCode = "9001" });
                    }

                    return Json(new { state = 0, txt = "下单失败", errCode = "9001" });
                }
                else
                {
                    var Touser = ConfigurationManager.AppSettings["OrderErrorUser"];
                    WxMessageHandler.GetInstance().SendOrderErrorMsgToUser(ID, _orderInfo.preApproval.HTCode, Touser, XmsOrderId, 5);
                    LogHelper.Error("新建订单失败HT单号：" + _orderInfo.preApproval.HTCode);
                    return Json(new { state = 0, txt = "下单失败", errCode = "9001" });
                }
            }
            catch(Exception ex)
            {
                var Touser = ConfigurationManager.AppSettings["OrderErrorUser"];
                WxMessageHandler.GetInstance().SendOrderErrorMsgToUser(ID, _orderInfo.preApproval.HTCode, Touser, XmsOrderId, 5);
                LogHelper.Error("新建订单失败HT单号：" + _orderInfo.preApproval.HTCode);
                LogHelper.Error("失败异常信息:" + ex.Message);
                return Json(new { state = 0, txt = "下单失败", errCode = "9001" });
            }
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
            var _orderInfo = WeChatOrderInfo;


            var orderChannel = OrderApiClientChannelFactory.GetChannel();
            var oldOrder = orderChannel.FindByID(ID);

            var _enterpriseOrderId = oldOrder.EnterpriseOrderId;
            if (string.IsNullOrEmpty(_enterpriseOrderId))
            {
                var _channel = _orderInfo.supplier;
                var _date = DateTime.Now.ToString("yyMMddHHmmss");
                _enterpriseOrderId = _channel.ToUpper() + "-" + _date;
                for (int i = 0; i < 3; i++)
                {
                    int RandKey = new Random().Next(0, 9);
                    _enterpriseOrderId += RandKey;
                }
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

            try
            {
                //写入改单后的新数据的到p_order p_ordercache
                var res = orderChannel.ChangeOrder(ID, ChangeID, _orderInfo);
                if (res > 0)
                {
                    // 调用小秘书下单
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
                        invoiceTitle = _orderInfo.invoiceTitle + " - " + _orderInfo.dutyParagraph,
                        orderTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        remark = _orderInfo.details.remark,
                        dinnerName = _orderInfo.details.consignee,
                        dinnernum = _orderInfo.details.attendCount.ToString(),
                        phone = _orderInfo.details.phone,
                        address = _orderInfo.hospital.Address + " - " + _orderInfo.details.deliveryAddress,
                        resId = _orderInfo.foods.resId,
                        longitude = _orderInfo.hospital.Longitude,
                        latitude = _orderInfo.hospital.Latitude,
                        hospitalId = _orderInfo.hospital.HospitalCode,
                        foods = foods,
                        cityId = _orderInfo.hospital.CityId.ToString(),
                        cn = _orderInfo.preApproval.HTCode,
                        cnAmount = _orderInfo.preApproval.BudgetTotal.ToString(),
                        mudId = _orderInfo.preApproval.ApplierMUDID,
                        typeId = type
                    };
                    try
                    {
                        var _res = apiClient.CreateOrder(req);
                        if (_res == null)
                        {
                            //从改单数据还原为原始订单数据
                            var r = orderChannel.RestoreOrder(ID, oldOrder);
                            LogHelper.Error("失败异常信息_RES:_res为空");
                        }
                        else
                        {
                            if (_res.errorCode != "0")
                            {
                                try
                                {
                                    //从改单数据还原为原始订单数据
                                    var re = orderChannel.RestoreOrder(ID, oldOrder);
                                    if (re > 0)
                                    {
                                        return Json(new { state = 0, txt = _res.errorMsg, errCode = _res.errorCode });
                                    }
                                    else
                                    {
                                        var Touser = ConfigurationManager.AppSettings["OrderErrorUser"];
                                        WxMessageHandler.GetInstance().SendOrderErrorMsgToUser(ID, _orderInfo.preApproval.HTCode, Touser, "", 2);
                                        LogHelper.Error($"还原订单信息失败 - [{ID}]");
                                        return Json(new { state = 0, txt = _res.errorMsg, errCode = _res.errorCode });
                                    }
                                }
                                catch (Exception ex)
                                {
                                    var Touser = ConfigurationManager.AppSettings["OrderErrorUser"];
                                    WxMessageHandler.GetInstance().SendOrderErrorMsgToUser(ID, _orderInfo.preApproval.HTCode, Touser, "", 2);
                                    LogHelper.Error($"还原订单信息失败 - [{ID}]");
                                    LogHelper.Error("失败异常信息:" + ex.Message);
                                    return Json(new { state = 0, txt = _res.errorMsg, errCode = _res.errorCode });
                                }

                            }
                            else
                            {
                                if (string.IsNullOrEmpty(oldOrder.XmsOrderId))
                                {
                                    XmsOrderId = _res.iPathOrderId;
                                }
                                WeChatOrderInfo = null;
                                ChangeOrderID = null;
                                try
                                {
                                    if (!string.IsNullOrEmpty(XmsOrderId))
                                    {
                                        // 从未在小秘书下单过的改单，下单成功后应写入小秘书单号
                                        orderChannel.SaveXmsOrderId(ID, XmsOrderId);
                                    }
                                    oldOrder = orderChannel.FindByID(ID);
                                    WxMessageHandler.GetInstance().SendMessageToUser(CurrentWxUser.UserId, oldOrder);
                                    return Json(new { state = 1, isChange = 1 });
                                }
                                catch (Exception ex)
                                {
                                    var Touser = ConfigurationManager.AppSettings["OrderErrorUser"];
                                    WxMessageHandler.GetInstance().SendOrderErrorMsgToUser(ID, _orderInfo.preApproval.HTCode, Touser, XmsOrderId, 4);
                                    LogHelper.Error($"改单失败--写入XMSOrderID失败-- [{ID}]");
                                    LogHelper.Error("失败异常信息:" + ex.Message);
                                    return Json(new { state = 1, isChange = 1 });
                                }
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        LogHelper.Error($"改单失败-- [{ID}]");
                        LogHelper.Error("失败异常信息:" + e.Message);
                        LogHelper.Error("修改订单返回异常，网络中断");
                        try
                        {
                            //从改单数据还原为原始订单数据
                            var re = orderChannel.RestoreOrder(ID, oldOrder);
                            if (re > 0)
                            {
                                var Touser = ConfigurationManager.AppSettings["OrderErrorUser"];
                                WxMessageHandler.GetInstance().SendOrderErrorMsgToUser(ID, _orderInfo.preApproval.HTCode, Touser, "", 7);
                            }
                        }
                        catch (Exception ex)
                        {
                            var Touser = ConfigurationManager.AppSettings["OrderErrorUser"];
                            WxMessageHandler.GetInstance().SendOrderErrorMsgToUser(ID, _orderInfo.preApproval.HTCode, Touser, "", 2);
                            LogHelper.Error($"还原订单信息失败 - [{ID}]");
                            LogHelper.Error("失败异常信息:" + ex.Message);
                            return Json(new { state = 0, txt = "改单失败", errCode = "9001" });
                        }
                        return Json(new { state = 0, txt = "改单失败", errCode = "9001" });
                    }
                    return Json(new { state = 0, txt = "改单失败", errCode = "9001" });
                }
                else
                {
                    LogHelper.Error("修改订单失败HT单号：" + _orderInfo.preApproval.HTCode);
                    return Json(new { state = 0, txt = "改单失败", errCode = "9001" });
                }
            }
            catch(Exception ex)
            {
                var Touser = ConfigurationManager.AppSettings["OrderErrorUser"];
                WxMessageHandler.GetInstance().SendOrderErrorMsgToUser(ID, _orderInfo.preApproval.HTCode, Touser, "", 3);
                LogHelper.Error($"改单失败-- [{ID}]");
                LogHelper.Error("失败异常信息:" + ex.Message);
                return Json(new { state = 0, txt = "改单失败", errCode = "9001" });
            }
            
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
            var listTA = new List<P_TA>();
            if (CurrentWxUser.TerritoryCode != null && CurrentWxUser.TerritoryCode != "")
            {
                listTA = baseDataChannel.LoadTAByMarketUserId(market, CurrentWxUser.UserId, CurrentWxUser.TerritoryCode);
            } else
            {
                listTA = baseDataChannel.LoadTAByMarket(market);
            }
            return Json(new { state = 1, rows = listTA });
        }
        #endregion

        #region 查询成本中心
        /// <summary>
        /// 查询成本中心
        /// </summary>
        /// <param name="ta"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LoadCostCenter(string market, string TA)
        {
            var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
            var listCostCenter = new List<D_COSTCENTERSELECT>();
            if (CurrentWxUser.TerritoryCode != null && CurrentWxUser.TerritoryCode != "")
            {
                listCostCenter = baseDataChannel.LoadCostCenterByMarketUserID(market, CurrentWxUser.UserId, CurrentWxUser.TerritoryCode);
            } else
            {
                listCostCenter = baseDataChannel.LoadCostCenterByTA(market, TA);
            }

            return Json(new { state = 1, rows = listCostCenter });
        }
        #endregion

        #region 加载成本中心
        /// <summary>
        /// 加载成本中心
        /// </summary>
        //[HttpPost]
        //public JsonResult GetAllCostCenter()
        //{
        //    var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
        //    var listCostCenter = baseDataChannel.GetAllCostCenter();
        //    return Json(new { state = 1, rows = listCostCenter });
        //}
        #endregion

        #region 保存提交
        /// <summary>
        /// 保存提交
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [iPathOAuthFilter(MappingKey = "0x0006", CallBackUrl = true)]
        public ActionResult Submit()
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
            var tacode = CurrentWxUser.TerritoryCode;
            tacode = tacode ?? "";
            ViewBag.Market = market;
            ViewBag.TACODE = tacode;
            ViewBag.GroupType = 1;
            if (CurrentWxUser.TerritoryCode == null || CurrentWxUser.TerritoryCode == "")
            {
                var isDevGroup = channelBase.IsDevGroup(userId);
                if (isDevGroup)
                {
                    ViewBag.GroupType = 3;
                }
                else
                {
                    ViewBag.GroupType = 2;
                }
            }
            ViewBag.listMarket = listMarket.Where(a => a.Name == market).ToList();
            ViewBag.IsServPause = CurrentWxUser.IsServPause;
            ViewBag.IsOutSideHT = CurrentWxUser.IsOutSideHT;
            ViewBag.CurrentUserId = CurrentWxUser.UserId;
            return View();
        }
        #endregion

        #region 数据校验方法
        //数据校验方法
        [HttpPost]
        public bool isValidate()
        {
            //TA
            if (String.IsNullOrEmpty(Request.Form["TA"]))
            {
                Response.Write("<script>alert('请选择TA');</script>");
                return false;
            }

            //省份
            if (String.IsNullOrEmpty(Request.Form["ProvinceName"]))
            {
                Response.Write("<script>alert('请选择省份');</script>");
                return false;
            }

            //城市
            if (String.IsNullOrEmpty(Request.Form["CityName"]))
            {
                Response.Write("<script>alert('请选择城市');</script>");
                return false;
            }

            //医院名称
            if (String.IsNullOrEmpty(Request.Form["HospitalName"]))
            {
                Response.Write("<script>alert('请选择医院名称');</script>");
                return false;
            }

            //会议名称
            if (String.IsNullOrEmpty(Request.Form["MeetingTitle"]))
            {
                Response.Write("<script>alert('请填写会议名称');</script>");
                return false;
            }

            //会议日期
            if (String.IsNullOrEmpty(Request.Form["MeetingDate"]))
            {
                Response.Write("<script>alert('请选择会议日期');</script>");
                return false;
            }

            //会议时间
            if (String.IsNullOrEmpty(Request.Form["MeetingTime"]))
            {
                Response.Write("<script>alert('请选择会议时间');</script>");
                return false;
            }

            //参会人数
            if (String.IsNullOrEmpty(Request.Form["Attendance"]))
            {
                Response.Write("<script>alert('请填写参会人数');</script>");
                return false;
            }

            //成本中心
            if (String.IsNullOrEmpty(Request.Form["CostCenter"]))
            {
                Response.Write("<script>alert('请选择成本中心');</script>");
                return false;
            }

            //预算金额
            if (String.IsNullOrEmpty(Request.Form["Budget"]))
            {
                Response.Write("<script>alert('请填写预算金额');</script>");
                return false;
            }

            //直线经理随访
            if (String.IsNullOrEmpty(Request.Form["FollowVisit"]))
            {
                Response.Write("<script>alert('请选择直线经理是否随访');</script>");
                return false;
            }
            return true;
        }
        #endregion

        #region 生成随机数字方法
        /// <summary>
        /// 生成随机数字方法
        /// </summary>
        /// <param name="no">生成位数</param>
        /// <returns></returns>
        public string GetHTCodeNo(int no)
        {
            Random rd = new Random();
            var result=rd.Next(0, 999);
            return result.ToString().PadLeft(no,'0');
        }
        #endregion

        #region 保存提交预申请信息
        /// <summary>
        /// 保存提交预申请信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [OrderFilter]
        [iPathOAuthFilter(MappingKey = "0x0001", CallBackUrl = true)]
        public JsonResult _Submit()
        {
            //用户信息
            P_USERINFO userInfo = Session[MealH5.Util.ConstantHelper.CURRENTWXUSER] as P_USERINFO;
            var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
            var preApprovalChannel = PreApprovalClientChannelFactory.GetChannel();
            //string result = "";
            var htCodeInfo = BaseDataClientChannelFactory.GetChannel();
            var userInfoChannel = UserInfoClientChannelFactory.GetChannel();
            PreApproval.ID = Guid.NewGuid();
            PreApproval.ApplierName = userInfo.Name;   //登录人姓名
            PreApproval.ApplierMUDID = userInfo.UserId;   //登录人ID
            if (userInfo.PhoneNumber == null || userInfo.PhoneNumber == "")
            {
                PreApproval.ApplierMobile = "";
            }
            else
            {
                PreApproval.ApplierMobile = userInfo.PhoneNumber;   //登录人手机号码
            }
            PreApproval.CreateDate = DateTime.Now;   //创建日期
            PreApproval.ModifyDate = PreApproval.CreateDate;   //修改日期
            Random randA = new Random(Guid.NewGuid().GetHashCode());
            int aa = randA.Next(0, 99);
            Random randB = new Random(Guid.NewGuid().GetHashCode());
            int bb= randB.Next(0, 99);
            //var codeA = randA.Next(0, 99).ToString().PadLeft(2, '0');
            var codeA = GetHtCodeNoNew(2);
            //var codeB = randB.Next(0, 99).ToString().PadLeft(2, '0');
            var codeB = GetHtCodeNoNew(2);
            PreApproval.HTCode = checkNew("HT-"+ codeA + DateTime.Now.ToString("ff") + codeB + DateTime.Now.ToString("ff"));
            //获取上一级
            var Pre = baseDataChannel.GetNameUserId(PreApproval.ApplierMUDID);
            var Pre2 = baseDataChannel.GetNameUserId(Pre.CurrentApproverMUDID);
            //if (PreApproval.AttendCount >= 60)
            //{
            //    PreApproval.State = "1";
            //}
            //else
            if (PreApproval.BudgetTotal >= 1200&& PreApproval.BudgetTotal < 1500)
            {
               
              
                PreApproval.State = "7";
                var delegateMUDID = userInfoChannel.isSecondAgent(Pre2.CurrentApproverMUDID);
                PreApproval.CurrentApproverMUDID = delegateMUDID == null ? Pre2.CurrentApproverMUDID : delegateMUDID.DelegateUserMUDID;
                PreApproval.CurrentApproverName = delegateMUDID == null ? Pre2.CurrentApproverName : delegateMUDID.DelegateUserName;
                //PreApproval.CurrentApproverMUDID = Pre2.CurrentApproverMUDID;
                //PreApproval.CurrentApproverName = Pre2.CurrentApproverName;

            }
            else if (PreApproval.BudgetTotal >= 1500)
            {
                PreApproval.State = "3";
                //var delegateMUDID = userInfoChannel.isAgent(Pre2.CurrentApproverMUDID);
                //PreApproval.CurrentApproverMUDID = delegateMUDID == null ? Pre2.CurrentApproverMUDID : delegateMUDID.DelegateUserMUDID;
                //PreApproval.CurrentApproverName = delegateMUDID == null ? Pre2.CurrentApproverName : delegateMUDID.DelegateUserName;
                PreApproval.CurrentApproverMUDID = Pre2.CurrentApproverMUDID;
                PreApproval.IsOnc = 1;
                PreApproval.CurrentApproverName = Pre2.CurrentApproverName;

                if (!CheckApproveStep(PreApproval.CurrentApproverMUDID, PreApproval.TA) || userInfo.UserId == PreApproval.BUHeadMUDID)
                {
                    WxMessageHandler.GetInstance().SendApproveStepErrorMessageToGroup(PreApproval);
                }

            }
            else
            {
                PreApproval.State = "6";
                PreApproval.BUHeadApproveDate = DateTime.Now;
                PreApproval.CurrentApproverMUDID = "系统自动审批";
                PreApproval.CurrentApproverName = "系统自动审批";
            }
            //20190125存储预申请记录
            PreApproval.IsBudgetChange = false;
            PreApproval.IsMMCoEChange = false;
            
            baseDataChannel.Add(PreApproval);
            //提交预申请记录信息
            P_PreApprovalApproveHistory history = new P_PreApprovalApproveHistory()
            {
                ID = Guid.NewGuid(),
                PID = PreApproval.ID,
                UserName = CurrentWxUser.Name,
                UserId = CurrentWxUser.UserId,
                ActionType = 1,
                ApproveDate = DateTime.Now,
                type = 2
            };
            preApprovalChannel.AddPreApprovalApproveHistory(history);
            if (PreApproval.State == "6")
            {
                P_PreApprovalApproveHistory autoHistory = new P_PreApprovalApproveHistory()
                {
                    ID = Guid.NewGuid(),
                    PID = PreApproval.ID,
                    UserName = "系统自动审批",
                    UserId = "系统自动审批",
                    ActionType = 3,
                    ApproveDate = DateTime.Now,
                    type = 2
                };
                preApprovalChannel.AddPreApprovalApproveHistory(autoHistory);
            }
            PreApproval.IsFirst = 1;
            WxMessageHandler.GetInstance().SendPreApprovalStateChangeMessageToUser(PreApproval);

            return Json(new { state = 1 });
        }
        #endregion

        #region 预申请审批记录查询
        public JsonResult LoadApprovalRecords(string PID)
        {

            var preApprovalChannel = PreApprovalClientChannelFactory.GetChannel();
            var userInfoChannel = UserInfoClientChannelFactory.GetChannel();
            var list = preApprovalChannel.LoadApprovalRecords(Guid.Parse(PID));
            var preApproval = preApprovalChannel.LoadPreApprovalInfo(Guid.Parse(PID));
            var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
            var userid = "";
            if (list.Count == 0)
            {
                var pre1 = baseDataChannel.GetNameUserId(preApproval.ApplierMUDID);
                userid = pre1.CurrentApproverMUDID;
                for (int i = 0; i < 6; i++)
                {
                    var pre = baseDataChannel.GetNameUserId(userid);
                    //var delegateMUDID =  .isAgent(pre.CurrentApproverMUDID);
                    //userid = delegateMUDID == null ? pre.CurrentApproverMUDID : delegateMUDID.DelegateUserMUDID;
                    if (string.IsNullOrEmpty(pre.CurrentApproverMUDID) || pre == null)
                    {
                        break;
                    }
                    userid = pre.CurrentApproverMUDID;

                    bool flag1 = preApprovalChannel.HasApproveByTA(userid, preApproval.TA);
                    if (flag1 == true)
                    {
                        P_PreApprovalApproveHistory preApprovalHistory1 = new P_PreApprovalApproveHistory()
                        {
                            UserName = pre.CurrentApproverName,
                            UserId = pre.CurrentApproverMUDID,
                            ActionType = 0,
                        };
                        list.Add(preApprovalHistory1);
                        break;
                    }

                    P_PreApprovalApproveHistory preApprovalHistory = new P_PreApprovalApproveHistory()
                    {
                        UserName = pre.CurrentApproverName,
                        UserId = pre.CurrentApproverMUDID,
                        ActionType = 0,
                    };
                    //userid = pre.CurrentApproverMUDID;
                    list.Add(preApprovalHistory);
                }

                return Json(new { state = 1, rows = list });
            }
            else
            {
                var prehis = list[list.Count - 1];
                userid = prehis.UserId;

                bool flag = preApprovalChannel.HasApproveByTA(userid, preApproval.TA);
                if (preApproval.State == "5" || preApproval.State == "6" || preApproval.State == "10")
                {
                    return Json(new { state = 1, rows = list });
                }
                else
                {
                    if (flag == true || prehis.ActionType == 2)
                    {
                        var pre1 = baseDataChannel.GetNameUserId(preApproval.ApplierMUDID);
                        userid = pre1.CurrentApproverMUDID;
                        // return Json(new { state = 1, rows = list });
                    }else
                    {
                        var pre1 = preApprovalChannel.LoadApproveHistoryInfo(Guid.Parse(PID), 2);
                        if(pre1.ActionType == 4)
                        {
                            var pre2 = baseDataChannel.GetNameUserId(preApproval.ApplierMUDID);
                            userid = pre2.CurrentApproverMUDID;
                        }
                    }

                    for (int i = 0; i < 6; i++)
                    {
                        var pre = baseDataChannel.GetNameUserId(userid);
                        //var delegateMUDID = userInfoChannel.isAgent(pre.CurrentApproverMUDID);
                        //userid = delegateMUDID == null ? pre.CurrentApproverMUDID : delegateMUDID.DelegateUserMUDID;
                        if (string.IsNullOrEmpty(pre.CurrentApproverMUDID) || pre == null)
                        {
                            break;
                        }
                        userid = pre.CurrentApproverMUDID;
                        bool flag1 = preApprovalChannel.HasApproveByTA(userid, preApproval.TA);
                        if (flag1 == true)
                        {
                            P_PreApprovalApproveHistory preApprovalHistory1 = new P_PreApprovalApproveHistory()
                            {
                                UserName = pre.CurrentApproverName,
                                UserId = pre.CurrentApproverMUDID,
                                ActionType = 0,
                            };
                            list.Add(preApprovalHistory1);
                            break;
                        }
                        P_PreApprovalApproveHistory preApprovalHistory = new P_PreApprovalApproveHistory()
                        {
                            UserName = pre.CurrentApproverName,
                            UserId = pre.CurrentApproverMUDID,
                            ActionType = 0,
                        };
                        //userid = pre.CurrentApproverMUDID;
                        list.Add(preApprovalHistory);
                    }

                    return Json(new { state = 1, rows = list });
                }
            }
        }
        #endregion

        #region 加载预申请详细信息
        /// <summary>
        /// 加载预申请详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult LoadPreApprovalInfo(Guid id)
        {
            var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
            var preApprovalChannel = PreApprovalClientChannelFactory.GetChannel();
            var preApproval = preApprovalChannel.LoadPreApprovalInfo(id);
            if(preApproval.BudgetTotal >= 1500)
            {
                if (preApproval.State == "5" || preApproval.State == "6")
                {
                    preApproval.BUHeadName = preApproval.CurrentApproverName;
                    preApproval.BUHeadMUDID = preApproval.CurrentApproverMUDID;
                    return Json(new { state = 1, data = preApproval });
                }else if(preApproval.State == "10")
                {
                    return Json(new { state = 1, data = preApproval });
                }
                var checkRes = CheckApproveStep(preApproval.CurrentApproverMUDID, preApproval.TA);
                if (checkRes)
                {
                    bool flag = preApprovalChannel.HasApproveByTA(preApproval.CurrentApproverMUDID, preApproval.TA);
                    if (flag)
                    {
                        preApproval.BUHeadName = preApproval.CurrentApproverName;
                        preApproval.BUHeadMUDID = preApproval.CurrentApproverMUDID;
                        return Json(new { state = 1, data = preApproval });
                    }

                    var userId = preApproval.CurrentApproverMUDID;
                    for (int i = 0; i < 6; i++)
                    {
                        var pre = baseDataChannel.GetNameUserId(userId);
                        bool flag1 = preApprovalChannel.HasApproveByTA(preApproval.CurrentApproverMUDID, preApproval.TA);
                        if (flag1)
                        {
                            preApproval.BUHeadName = pre.CurrentApproverName;
                            preApproval.BUHeadMUDID = pre.CurrentApproverMUDID;
                            return Json(new { state = 1, data = preApproval });
                        }
                        else
                        {
                            userId = pre.CurrentApproverMUDID;
                        }
                    }
                }else
                {
                    preApproval.BUHeadName = "";
                    preApproval.BUHeadMUDID = "";
                    return Json(new { state = 1, data = preApproval });
                }
            }
            return Json(new { state = 1, data = preApproval });
        }
        #endregion

        #region MyRegion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [iPathOAuthFilter(MappingKey = "0x0001", CallBackUrl = true)]
        public ActionResult Details(Guid id)
        {
            ViewBag.preApprovalId = id;
            var channel = PreApprovalClientChannelFactory.GetChannel();
            var preApproval = channel.LoadPreApprovalInfo(id);
            ViewBag.IsFinished = preApproval.IsFinished;
            ViewBag.IsUsed = preApproval.IsUsed;
            ViewBag.CurrentDay = DateTime.Now.Day;
            ViewBag.IsCrossMonth = ((preApproval.MeetingDate.Value.Month == DateTime.Now.Month - 1 && DateTime.Now.Day < 5) || preApproval.MeetingDate.Value.Month == DateTime.Now.Month);
            if (preApproval.IsUsed == 1)
            {
                var order = channel.FindActivityOrderByHTCode(preApproval.HTCode);
                ViewBag.OrderState = order.State;
            }

            if(CurrentWxUser.TerritoryCode != null && CurrentWxUser.TerritoryCode != "")
            {
                ViewBag.GroupType = 1;
            }else
            {
                ViewBag.GroupType = 3;
            }
            return View();
        }
        #endregion

        #region 编辑提交预申请信息加载
        /// <summary>
        /// 编辑提交预申请信息加载
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet]
        [iPathOAuthFilter(MappingKey = "0x0001", CallBackUrl = true)]
        public ActionResult Edit(Guid id)
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

            ViewBag.IsServPause = CurrentWxUser.IsServPause;
            ViewBag.IsOutSideHT = CurrentWxUser.IsOutSideHT;
            ViewBag.CurrentUserId = CurrentWxUser.UserId;

            ViewBag.TACODE = CurrentWxUser.TerritoryCode;
            ViewBag.GroupType = 1;
            if (CurrentWxUser.TerritoryCode == null || CurrentWxUser.TerritoryCode == "")
            {
                var isDevGroup = channelBase.IsDevGroup(userId);
                if (isDevGroup)
                {
                    ViewBag.GroupType = 3;
                }
                else
                {
                    ViewBag.GroupType = 2;
                }
            }
            //加载信息
            //id = Guid.Parse("137b3f04-5e83-4bc1-b5d0-f5a9bf721dd8");

            var channel = PreApprovalClientChannelFactory.GetChannel();
            var preApproval = channel.LoadPreApprovalInfo(id);
            PreApproval = preApproval;
            ViewBag.Market = preApproval.Market;
            ViewBag.listMarket = listMarket.Where(a => a.Name == preApproval.Market || a.Name == CurrentWxUser.Market).ToList();
            ViewBag.ID = preApproval.ID;
            ViewBag.ApplierName = preApproval.ApplierName;
            ViewBag.ApplierMUDID = preApproval.ApplierMUDID;
            ViewBag.ApplierMobile = preApproval.ApplierMobile;
            ViewBag.CreateDate = preApproval.CreateDate;
            ViewBag.ModifyDate = preApproval.ModifyDate;
            ViewBag.HTCode = preApproval.HTCode;
            ViewBag.VeevaMeetingID = preApproval.VeevaMeetingID;
            ViewBag.Market = preApproval.Market;
            ViewBag.TA = preApproval.TA;
            ViewBag.Province = preApproval.Province;
            ViewBag.City = preApproval.City;
            ViewBag.HospitalCode = preApproval.HospitalCode;
            ViewBag.HospitalName = preApproval.HospitalName;
            ViewBag.HospitalAddress = preApproval.HospitalAddress;
            ViewBag.MeetingName = preApproval.MeetingName;
            ViewBag.MeetingTime = preApproval.MeetingDate.Value.ToString("yyyy-MM-dd HH:mm");
            ViewBag.AttendCount = preApproval.AttendCount;
            ViewBag.CostCenter = preApproval.CostCenter;
            ViewBag.BudgetTotal = String.Format("{0:N}", preApproval.BudgetTotal);
            ViewBag.IsDMFollow = preApproval.IsDMFollow;
            ViewBag.BUHeadName = preApproval.BUHeadName;
            ViewBag.BUHeadMUDID = preApproval.BUHeadMUDID;
            ViewBag.BUHeadApproveDate = preApproval.BUHeadApproveDate;
            ViewBag.IsReAssign = preApproval.IsReAssign;
            ViewBag.ReAssignBUHeadName = preApproval.ReAssignBUHeadName;
            ViewBag.ReAssignBUHeadMUDID = preApproval.ReAssignBUHeadMUDID;
            ViewBag.ReAssignBUHeadApproveDate = preApproval.ReAssignBUHeadApproveDate;
            ViewBag.State = preApproval.State;
            ViewBag.IsFinished = preApproval.IsFinished;
            ViewBag.IsUsed= preApproval.IsUsed;
            ViewBag.IsFreeSpeaker = preApproval.IsFreeSpeaker;
            ViewBag.SpeakerServiceImage = preApproval.SpeakerServiceImage;
            ViewBag.SpeakerBenefitImage = preApproval.SpeakerBenefitImage;
            ViewBag.IsCrossMonth = ((preApproval.MeetingDate.Value.Month == DateTime.Now.Month -1 && DateTime.Now.Day < 5) || preApproval.MeetingDate.Value.Month == DateTime.Now.Month);
            if (preApproval.IsUsed==1)
            {
                var order = channel.FindActivityOrderByHTCode(preApproval.HTCode);
                ViewBag.OrderState = order.State;
            }
            if(preApproval.MRTerritoryCode != CurrentWxUser.TerritoryCode)
            {
                ViewBag.isTerritoryChange = 1;
            }else
            {
                ViewBag.isTerritoryChange = 0;
            }
            ViewBag.HTType = preApproval.HTType;
            return View(ViewBag);
        }
        #endregion

        #region 转到预申请审批页面
        /// <summary>
        /// 转到预申请审批页面
        /// </summary>
        /// <param name="preApprovalId"></param>
        /// <returns></returns>
        [HttpGet]
        [iPathOAuthFilter(MappingKey = "0x0007", CallBackUrl = true)]
        public ActionResult Approval(Guid id,int from)
        {
            ViewBag.From = from;
            ViewBag.preApprovalId = id;
            var preApprovalChannel = PreApprovalClientChannelFactory.GetChannel();
            var preApproval = preApprovalChannel.LoadPreApprovalInfo(id);
            if (preApproval.BudgetTotal>=1500)
            {
                ViewBag.Title = "预算金额>=1500元审批";
            }
            else if (preApproval.BudgetTotal >=1200 && preApproval.BudgetTotal<1500)
            {
                ViewBag.Title = "预算金额>=1200元审批";
            }
            return View();
        }
        public ActionResult ApprovalDetails(Guid id)
        {
            ViewBag.preApprovalId = id;
            return View();
        }
        #endregion

        #region 转到预申请审批页面
        /// <summary>
        /// 转到预申请审批页面
        /// </summary>
        /// <param name="preApprovalId"></param>
        /// <returns></returns>
        [HttpGet]
        [iPathOAuthFilter(MappingKey = "0x0008", CallBackUrl = true)]
        public ActionResult MMCoEApprove(Guid id)
        {
            ViewBag.preApprovalId = id;
            return View();
        }
        #endregion

        #region 编辑提交预申请信息
        /// <summary>
        /// 编辑提交预申请信息
        /// </summary>
        /// <param name="id">PreApprovalID</param>
        /// <returns></returns>
        [HttpPost]
        [OrderFilter]
        [iPathOAuthFilter(MappingKey = "0x0001", CallBackUrl = true)]
        public JsonResult _Edit()
        {
            var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
            var preApprovalChannel = PreApprovalClientChannelFactory.GetChannel();
            var userInfoChannel = UserInfoClientChannelFactory.GetChannel();
            var _oldPreApproval = preApprovalChannel.LoadPreApprovalInfo(PreApproval.ID);
            PreApproval.ModifyDate = DateTime.Now;   //修改日期
            var hisList = preApprovalChannel.FindPreApprovalApproveHistory(PreApproval.ID);
            var pre1 = baseDataChannel.GetNameUserId(PreApproval.ApplierMUDID);
            var pre2 = baseDataChannel.GetNameUserId(pre1.CurrentApproverMUDID);
            P_PreApprovalApproveHistory lastMMCoEApprove = null;
            P_PreApprovalApproveHistory lastBUHeadApprove = null;
            if (hisList.Count > 0)
            {
                lastMMCoEApprove = hisList.Where(p => p.PID == PreApproval.ID && p.type == 1).OrderByDescending(p => p.ApproveDate).FirstOrDefault();
                lastBUHeadApprove = hisList.Where(p => p.PID == PreApproval.ID && p.type == 2).OrderByDescending(p => p.ApproveDate).FirstOrDefault();
            }
            string alertMessage = "您的预申请修改已提交成功。";
            //参会人数改变或图片地址改变
            if (PreApproval.AttendCount != _oldPreApproval.AttendCount || PreApproval.MMCoEImage != _oldPreApproval.MMCoEImage)
            {
                PreApproval.IsMMCoEChange = true;
            }
            else
            {
                PreApproval.IsMMCoEChange = false;
            }
            //预算金额改变或成本中心改变
            if (PreApproval.BudgetTotal != _oldPreApproval.BudgetTotal || PreApproval.CostCenter != _oldPreApproval.CostCenter)
            {
                PreApproval.IsBudgetChange = true;
            }
            else
            {
                PreApproval.IsBudgetChange = false;
            }
            //会议时间或医院改变
            if (PreApproval.HospitalCode != _oldPreApproval.HospitalCode || PreApproval.MeetingDate != _oldPreApproval.MeetingDate)
            {
                PreApproval.IsHosOrMeetingTimeChange = true;
            }
            else
            {
                PreApproval.IsHosOrMeetingTimeChange = false;
            }
            //if (PreApproval.AttendCount >= 60 && (PreApproval.IsMMCoEChange == true || (lastMMCoEApprove == null || (lastMMCoEApprove != null && lastMMCoEApprove.ActionType == 2))))
            //{
            //    PreApproval.State = "1";
            //    alertMessage = "您的预申请修改已提交成功，正在等待中央订餐项目组审批。";
            //}
            //else 
            if (_oldPreApproval.BudgetTotal>=1500 && PreApproval.BudgetTotal < 1500 && PreApproval.BudgetTotal >= 1200)
            {
                var no = baseDataChannel.UpdateHisPreApprovaDelete(PreApproval.ID);
                if (no == 0)
                {
                    LogHelper.Info($"删除历史记录失败 - [{PreApproval.ID}]");
                }
            }
            if (PreApproval.BudgetTotal >= 1500 && _oldPreApproval.BudgetTotal < 1500 && _oldPreApproval.BudgetTotal >= 1200)
            {
                var no = baseDataChannel.UpdateHisPreApprovaDelete(PreApproval.ID);
                if (no == 0)
                {
                    LogHelper.Info($"删除历史记录失败 - [{PreApproval.ID}]");
                }
            }
            if (PreApproval.BudgetTotal >= 1500 && (PreApproval.IsBudgetChange == true || (lastBUHeadApprove == null || (lastBUHeadApprove != null && lastBUHeadApprove.ActionType == 2))))
            {
                PreApproval.State = "3";
                PreApproval.CurrentApproverMUDID = pre2.CurrentApproverMUDID;
                PreApproval.CurrentApproverName = pre2.CurrentApproverName;
                var num = baseDataChannel.UpdateCurrentPreApprova(PreApproval);
                if (num == 0)
                {
                    LogHelper.Info($"更改金额审批人更新失败 - [{ pre2.CurrentApproverMUDID}]");
                }
                //var no = baseDataChannel.UpdateHisPreApprovaDelete(PreApproval.ID);
                //if (no == 0)
                //{
                //    LogHelper.Info($"删除历史记录失败 - [{PreApproval.ID}]");
                //}
                alertMessage = "您的预申请修改已提交成功，正在等待BU Head（Rx/Vx）或销售总监（DDT/TSKF）审批。";

                if (!CheckApproveStep(PreApproval.CurrentApproverMUDID, PreApproval.TA) || PreApproval.ApplierMUDID == PreApproval.BUHeadMUDID)
                {
                    WxMessageHandler.GetInstance().SendApproveStepErrorMessageToGroup(PreApproval);
                }
            }
            else if (PreApproval.BudgetTotal < 1500&& PreApproval.BudgetTotal >= 1200&&(PreApproval.IsBudgetChange == true || (lastBUHeadApprove == null || (lastBUHeadApprove != null && lastBUHeadApprove.ActionType == 2))))
            {
                PreApproval.State = "7";
                var delegateMUDID = userInfoChannel.isSecondAgent(pre2.CurrentApproverMUDID);
                PreApproval.CurrentApproverMUDID = delegateMUDID == null ? pre2.CurrentApproverMUDID : delegateMUDID.DelegateUserMUDID;
                PreApproval.CurrentApproverName = delegateMUDID == null ? pre2.CurrentApproverName : delegateMUDID.DelegateUserName;
                //PreApproval.CurrentApproverMUDID = pre2.CurrentApproverMUDID;
                //PreApproval.CurrentApproverName = pre2.CurrentApproverName;
                var num = baseDataChannel.UpdateCurrentPreApprova(PreApproval);
                if (num == 0)
                {
                    LogHelper.Info($"更改金额审批人更新失败 - [{ pre2.CurrentApproverMUDID}]");
                }
                //var no = baseDataChannel.UpdateHisPreApprovaDelete(PreApproval.ID);
                //if (no==0)
                //{
                //    LogHelper.Info($"删除历史记录失败 - [{PreApproval.ID}]");
                //}
                alertMessage = "您的预申请修改已提交成功，正在等待二级经理审批。";
            }
            
            else
            {
                PreApproval.State = "6";
                PreApproval.IsFirst = 2;
                PreApproval.BUHeadApproveDate = DateTime.Now;
                if (PreApproval.IsBudgetChange)
                {
                    var num = baseDataChannel.UpdateCurrentPreApprova(PreApproval);
                    if (num == 0)
                    {
                        LogHelper.Info($"更改金额审批人更新失败 - [{ pre2.CurrentApproverMUDID}]");
                    }
                }
            }
            baseDataChannel.Update(PreApproval);
            P_PreApprovalApproveHistory history = new P_PreApprovalApproveHistory()
            {
                ID = Guid.NewGuid(),
                PID = PreApproval.ID,
                UserName = CurrentWxUser.Name,
                UserId = CurrentWxUser.UserId,
                ActionType = 4,
                ApproveDate = DateTime.Now,
                type = 2
            };
            preApprovalChannel.AddPreApprovalApproveHistory(history);
            if (PreApproval.State == "6")
            {
                P_PreApprovalApproveHistory autoHistory = new P_PreApprovalApproveHistory()
                {
                    ID = Guid.NewGuid(),
                    PID = PreApproval.ID,
                    UserName = "系统自动审批",
                    UserId = "系统自动审批",
                    ActionType = 3,
                    ApproveDate = DateTime.Now,
                    type = 2
                };
                
                preApprovalChannel.AddPreApprovalApproveHistory(autoHistory);

            }
            PreApproval.IsOnc = 1;
            
            WxMessageHandler.GetInstance().SendPreApprovalStateChangeMessageToUser(PreApproval);

            return Json(new { state = 1, txt = alertMessage });
        }
        #endregion

        public JsonResult SaveSession(P_PreApproval p)
        {
            var htCodeInfo = BaseDataClientChannelFactory.GetChannel();
            P_PreApproval preApprovalInfo = new P_PreApproval();
            var userInfoChannel = UserInfoClientChannelFactory.GetChannel();
            //用户信息
            P_USERINFO userInfo = Session[MealH5.Util.ConstantHelper.CURRENTWXUSER] as P_USERINFO;
            preApprovalInfo.Market = Request.Form["Market"];   //Market
            preApprovalInfo.TA = Request.Form["TA"];   //TA
            preApprovalInfo.VeevaMeetingID = Request.Form["VeevaMeetingID"];//VeevaMeetingID
            preApprovalInfo.Province = Request.Form["ProvinceName"];   //省份                                                              
            preApprovalInfo.City = Request.Form["CityName"];   //城市
            preApprovalInfo.HospitalName = Request.Form["HospitalName"];   //医院名称
            preApprovalInfo.HospitalCode = Request.Form["hospital"];   //医院编号
            preApprovalInfo.HospitalAddress = Request.Form["Address"];   //医院地址
            preApprovalInfo.MeetingName = Request.Form["MeetingTitle"];   //会议名称
            preApprovalInfo.HospitalAddressCode = Request.Form["AddressCode"];       //查修餐厅使用的医院编码
            preApprovalInfo.MeetingDate = Convert.ToDateTime(Request.Form["MeetingTime"]);
            preApprovalInfo.HTType = Request.Form["HTType"];
            if (Request.Form["Attendance"] != "")
            {
                preApprovalInfo.AttendCount = Convert.ToInt32(Request.Form["Attendance"]);   //参会人数
            }
            preApprovalInfo.CostCenter = Request.Form["CostCenter"];   //成本中心
            if (Request.Form["Budget"] != "")
            {
                preApprovalInfo.BudgetTotal = Convert.ToDecimal(Request.Form["Budget"]);   //预算金额
            }
            // 直线经理是否随访
            if (Request.Form["followVisit"] == "1")
            {
                preApprovalInfo.IsDMFollow = true;
            }
            else if (Request.Form["followVisit"] == "2")
            {
                preApprovalInfo.IsDMFollow = false;
            }
            if (!String.IsNullOrEmpty(Request.Form["CostCenter"]))
            {
                //string aa = Request.Form["CostCenter"].ToString();
                //string[] bb = aa.Split('(', ')');
                //string ta = bb[0].ToString();
                //string region = bb[1].ToString();
                //string costCenter = aa.Substring(aa.IndexOf("(") + 1, aa.LastIndexOf(")") - aa.IndexOf("(") - 1);
                string costCenter = preApprovalInfo.TA;

                D_COSTCENTER costInfos = htCodeInfo.FindInfo("", "", costCenter);
                var delegateBUHead = userInfoChannel.isSecondAgent(costInfos.BUHeadMUDID);
                preApprovalInfo.BUHeadName = delegateBUHead == null ? costInfos.BUHeadName : delegateBUHead.DelegateUserName;   //成本中心 审批人员名称
                preApprovalInfo.BUHeadMUDID = delegateBUHead == null ? costInfos.BUHeadMUDID : delegateBUHead.DelegateUserMUDID;   //
                preApprovalInfo.RDSDName=costInfos.RDSDName;
                preApprovalInfo.RDSDMUDID = costInfos.RDSDMUDID;
            }
            else
            {
                preApprovalInfo.BUHeadName = "";
                preApprovalInfo.BUHeadMUDID = "";
            }
            var RDUserInfo = htCodeInfo.LoadRDCode(preApprovalInfo.Market, CurrentWxUser.UserId, CurrentWxUser.TerritoryCode);
            preApprovalInfo.MRTerritoryCode = CurrentWxUser.TerritoryCode;
            if(RDUserInfo != null && RDUserInfo.Name != null)
            {
                preApprovalInfo.RDTerritoryCode = RDUserInfo.Name;
            }
            else
            {
                preApprovalInfo.RDTerritoryCode = "";
            }

            var DMUserInfo = htCodeInfo.FindTerritoryDM(CurrentWxUser.TerritoryCode);
            if (DMUserInfo != null && DMUserInfo.TA_CODE != null)
            {
                preApprovalInfo.DMTerritoryCode = DMUserInfo.TA_CODE;
            }
            else
            {
                preApprovalInfo.DMTerritoryCode = "";
            }


            preApprovalInfo.BUHeadApproveDate = null;   //批准日期
            preApprovalInfo.IsReAssign = false;   //是否重新分配
            preApprovalInfo.ReAssignBUHeadName = "";   //重新分配BUHeadName
            preApprovalInfo.ReAssignBUHeadMUDID = "";   //
            preApprovalInfo.ReAssignBUHeadApproveDate = null;   //
            preApprovalInfo.IsFreeSpeaker = Request.Form["IsFreeSpeaker"] == "1" ? true : false;
            preApprovalInfo.SpeakerServiceImage = p.SpeakerServiceImage;
            preApprovalInfo.SpeakerBenefitImage = p.SpeakerBenefitImage;
            PreApproval = preApprovalInfo;
            return Json(new { state = 1 });
        }

        public JsonResult UpdateSession(P_PreApproval p)
        {
            var htCodeInfo = BaseDataClientChannelFactory.GetChannel();
            var userInfoChannel = UserInfoClientChannelFactory.GetChannel();
            //用户信息
            P_USERINFO userInfo = Session[MealH5.Util.ConstantHelper.CURRENTWXUSER] as P_USERINFO;
            PreApproval.Market = Request.Form["Market"];   //Market
            PreApproval.VeevaMeetingID = Request.Form["VeevaMeetingID"];   //VeevaMeetingID
            PreApproval.TA = Request.Form["TA"];   //TA
            PreApproval.Province = Request.Form["ProvinceName"];   //省份
            PreApproval.City = Request.Form["CityName"];   //城市
            PreApproval.HospitalName = Request.Form["HospitalName"];   //医院名称
            PreApproval.HospitalCode = Request.Form["hospital"];   //医院编号
            var addressCode = Request.Form["AddressCode"];
            if (!string.IsNullOrEmpty(addressCode))
            {
                PreApproval.HospitalAddressCode = addressCode;
            }
            var address= Request.Form["Address"];//医院地址
            if (string.IsNullOrEmpty(address))
            {
                var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
                var hospital = baseDataChannel.FindHospital(PreApproval.HospitalCode);
                if (hospital != null)
                {
                    address = hospital.GskHospital==hospital.HospitalCode?hospital.Address:hospital.MainAddress+":"+hospital.Address;
                }
            }
            PreApproval.HospitalAddress = address;   
            PreApproval.MeetingName = Request.Form["MeetingTitle"];   //会议名称
            PreApproval.MeetingDate = Convert.ToDateTime(Request.Form["MeetingTime"]);   //会议日期
            if (Request.Form["Attendance"] != "")
            {
                PreApproval.AttendCount = Convert.ToInt32(Request.Form["Attendance"]);   //参会人数
            }
            if (Request.Form["HTType"] != "")
            {
                PreApproval.HTType = Request.Form["HTType"];   //HT形式
            }
            PreApproval.CostCenter = Request.Form["CostCenter"];   //成本中心
            if (Request.Form["Budget"] != "")
            {
                PreApproval.BudgetTotal = Convert.ToDecimal(Request.Form["Budget"]);   //预算金额
            }
            // 直线经理是否随访
            if (Request.Form["followVisit"] == "1")
            {
                PreApproval.IsDMFollow = true;
            }
            else if (Request.Form["followVisit"] == "2")
            {
                PreApproval.IsDMFollow = false;
            }
            if (!String.IsNullOrEmpty(Request.Form["CostCenter"]))
            {
                string costCenter = PreApproval.TA;

                D_COSTCENTER costInfos = htCodeInfo.FindInfo("", "", costCenter);
                if(costInfos != null)
                {
                    var delegateBUHead = userInfoChannel.isSecondAgent(costInfos.BUHeadMUDID);
                    PreApproval.BUHeadName = delegateBUHead == null ? costInfos.BUHeadName : delegateBUHead.DelegateUserName;   //成本中心 审批人员名称
                    PreApproval.BUHeadMUDID = delegateBUHead == null ? costInfos.BUHeadMUDID : delegateBUHead.DelegateUserMUDID;   //
                    PreApproval.RDSDName = costInfos.RDSDName;
                    PreApproval.RDSDMUDID = costInfos.RDSDMUDID;
                }
            }
            var RDUserInfo = htCodeInfo.LoadRDCode(PreApproval.Market, CurrentWxUser.UserId, PreApproval.MRTerritoryCode);
            //PreApproval.MRTerritoryCode = CurrentWxUser.TerritoryCode;
            if (RDUserInfo != null && RDUserInfo.Name != null)
            {
                PreApproval.RDTerritoryCode = RDUserInfo.Name;
            }
            else
            {
                PreApproval.RDTerritoryCode = "";
            }

            var DMUserInfo = htCodeInfo.FindTerritoryDM(CurrentWxUser.TerritoryCode);
            if (DMUserInfo != null && DMUserInfo.TA_CODE != null)
            {
                PreApproval.DMTerritoryCode = DMUserInfo.TA_CODE;
            }
            else
            {
                PreApproval.DMTerritoryCode = "";
            }

            PreApproval.IsFreeSpeaker = Request.Form["IsFreeSpeaker"] == "1" ? true : false;
            PreApproval.SpeakerServiceImage = p.SpeakerServiceImage;
            PreApproval.SpeakerBenefitImage = p.SpeakerBenefitImage;
            return Json(new { state = 1 });
        }

        public JsonResult LoadApproveHistoryInfo(Guid id, int Type)
        {
            var preApprovalChannel = PreApprovalClientChannelFactory.GetChannel();
            var preApprovalHistory = preApprovalChannel.LoadApproveHistoryInfo(id, Type);
            return Json(new { state = 1, data = preApprovalHistory });
        }
        public JsonResult LoadApproveHistory(Guid id, int Type)
        {
            var preApprovalChannel = PreApprovalClientChannelFactory.GetChannel();
            var preApprovalHistory = preApprovalChannel.LoadApproveHistory(id, Type);
            return Json(new { state = 1, data = preApprovalHistory });
        }
        public JsonResult LoadApproveHistoryRefused(Guid id, int Type,string UserId)
        {
            var preApprovalChannel = PreApprovalClientChannelFactory.GetChannel();
            var preApprovalHistory = preApprovalChannel.LoadApproveHistoryRefused(id, Type, UserId);
            return Json(new { state = 1, data = preApprovalHistory });
        }
        #region 保存选择的预申请
        /// <summary>
        /// 保存选择的预申请
        /// </summary>
        /// <param name="HTCode"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SavePreApproval(string code)
        {
            var preApprovalClient = PreApprovalClientChannelFactory.GetChannel();
            var res = preApprovalClient.FindPreApprovalByHTCode(code);
            PreApproval = res.FirstOrDefault();
            var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
            var hospitalChannel = HospitalClientChannelFactory.GetChannel();
            var hospital = hospitalChannel.FindByCode(PreApproval.HospitalAddressCode);
            WeChatOrderInfo.hospital = hospital;
            var market = baseDataChannel.FindByMarket(PreApproval.Market);
            WeChatOrderInfo.invoiceTitle = market.InvoiceTitle;
            WeChatOrderInfo.dutyParagraph = market.DutyParagraph;
            return Json(new { state = 1 });
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
            var listHospital = new List<P_HOSPITAL>();
            if(CurrentWxUser.TerritoryCode != null && CurrentWxUser.TerritoryCode != "")
            {
                listHospital = baseDataChannel.SearchHospitalByCodeTA(keyword, province, city, market, CurrentWxUser.UserId, CurrentWxUser.TerritoryCode);
            }else
            {
                listHospital = baseDataChannel.SearchHospitalByCode(keyword, province, city, market, TA);
            }
            return Json(new { state = 1, rows = listHospital });
        }
        #endregion

        private string check(string HTCode)
        {
            var preApprovalChannel = PreApprovalClientChannelFactory.GetChannel();
            var htcode = preApprovalChannel.GetHTCodeByID(HTCode);
            if (htcode != null)
            {
                return check("HT-" + GetHTCodeNo(8));
            }
            return HTCode;
        }


        private string checkNew(string HTCode)
        {
            var preApprovalChannel = PreApprovalClientChannelFactory.GetChannel();
            var htcode = preApprovalChannel.GetHTCodeByID(HTCode);
            if (htcode != null)
            {
                return checkNew("HT-" + GetHtCodeNoNew(8));
            }
            return HTCode;
        }


        #region 根据医院编码(医院编码+是否是主地址)查询医院地址
        /// <summary>
        /// 根据医院编码查询医院
        /// </summary>
        /// <param name="hospitalCode"></param>
        /// <returns></returns>
        public JsonResult FindHospital(string hospitalCode)
        {
            var hospitalChannel = HospitalClientChannelFactory.GetChannel();
            var hospital = hospitalChannel.FindByCode(hospitalCode);
            return Json(new { state=1,data=hospital});
        }
        #endregion

        #region 更新预申请医院地址
        /// <summary>
        /// 更新预申请医院地址
        /// </summary>
        /// <param name="preApprovalId"></param>
        /// <param name="hospitalAddress"></param>
        /// <returns></returns>
        public JsonResult UpDateAddress(string preApprovalId,string hospitalAddress)
        {
            var channel = PreApprovalClientChannelFactory.GetChannel();
            var res = channel.UpdateAddress(preApprovalId,hospitalAddress);
            return Json(new { state = 1 });
        }
        #endregion 


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

        public JsonResult CheckApproveStepByPreID(Guid PID)
        {
            var preApprovalChannel = PreApprovalClientChannelFactory.GetChannel();
            var preApproval = preApprovalChannel.LoadPreApprovalInfo(PID);

            if (!CheckApproveStep(preApproval.CurrentApproverMUDID, preApproval.TA) && (preApproval.State == "3" || preApproval.State == "4"))
            {
                return Json(new { state = 0, txt = "当前预申请审批流程有误，请联系：技术支持热线: 0411-84898998或PMO邮箱cn.chinarx-pmo@gsk.com。" });
            }

            return Json(new { state = 1, txt = "success" });
        }

        public string GetHtCodeNoNew(int No)
        {
            string str = @"0123456789abcdefghigklmnopqrstuvwxyzABCDEFGHIGKLMNOPQRSTUVWXYZ";
            //返回数字
            //return rnd.Next(10).ToString();

            //返回小写字母
            //return str.Substring(10 + rnd.Next(26), 1);

            //返回大写字母
            //return str.Substring(36 + rnd.Next(26), 1);

            //返回大小写字母混合
            //return str.Substring(10 + rnd.Next(52), 1);

            //返回小写字母和数字混合
            //return str.Substring(0 + rnd.Next(36), 1);

            //返回大写字母和数字混合
            // return str.Substring(0 + rnd.Next(36), 1).ToUpper();

            //返回大小写字母和数字混合
            //return str.Substring(0+ rnd.Next(61), 1);


            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            string ReturnStr = "";
            for(var i = 0; i < No; i++)
            {
                ReturnStr += str.Substring(0 + rnd.Next(36), 1).ToUpper();
            }

            return ReturnStr;
        }


        public string GetOrderID(int No)
        {
            string str = @"0123456789abcdefghigklmnopqrstuvwxyzABCDEFGHIGKLMNOPQRSTUVWXYZ";
            //返回数字
            //return rnd.Next(10).ToString();

            //返回小写字母
            //return str.Substring(10 + rnd.Next(26), 1);

            //返回大写字母
            //return str.Substring(36 + rnd.Next(26), 1);

            //返回大小写字母混合
            //return str.Substring(10 + rnd.Next(52), 1);

            //返回小写字母和数字混合
            //return str.Substring(0 + rnd.Next(36), 1);

            //返回大写字母和数字混合
            // return str.Substring(0 + rnd.Next(36), 1).ToUpper();

            //返回大小写字母和数字混合
            //return str.Substring(0+ rnd.Next(61), 1);


            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            string ReturnStr = "";
            for (var i = 0; i < No; i++)
            {
                ReturnStr += rnd.Next(10).ToString();
            }

            return ReturnStr;
        }

        #region 新增地址
        [HttpGet]
        [iPathOAuthFilter(MappingKey = "0x0006", CallBackUrl = true)]
        public ActionResult AddAddress()
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
            ViewBag.GroupType = 1;
            if (CurrentWxUser.TerritoryCode == null || CurrentWxUser.TerritoryCode == "")
            {
                var isDevGroup = channelBase.IsDevGroup(userId);
                if (isDevGroup)
                {
                    ViewBag.GroupType = 3;
                }
                else
                {
                    ViewBag.GroupType = 2;
                }
            }
            ViewBag.listMarket = listMarket.Where(a => a.Name == market).ToList();
            ViewBag.IsServPause = CurrentWxUser.IsServPause;
            ViewBag.IsOutSideHT = CurrentWxUser.IsOutSideHT;
            ViewBag.CurrentUserId = CurrentWxUser.UserId;
            return View();
        }

        [HttpGet]
        [iPathOAuthFilter(MappingKey = "0x0006", CallBackUrl = true)]
        public ActionResult Map()
        {
            if (CurrentWxUser.TerritoryCode == null || CurrentWxUser.TerritoryCode == "")
            {
                ViewBag.GroupType = 3;
            }else
            {
                ViewBag.GroupType = 1;
            }
            return View();
        }

        public JsonResult SearchHospitalByGskHospital(string gskHospital)
        {
            try
            {
                var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
                var listHospital = baseDataChannel.SearchHospitalByGskHospital(gskHospital);
                return Json(new { state = 1, rows = listHospital });
            }
            catch (Exception ex)
            {
                LogHelper.Error("Exception SearchHospitalByGskHospital", ex);
                return Json(new { state = 0, rows = new List<P_HOSPITAL_NEW>() });
            }
        }

        public JsonResult SearchHospitalByGskHospitalForShow(string gskHospital)
        {
            try
            {
                var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
                var listHospital = baseDataChannel.SearchHospitalByGskHospital(gskHospital);
                List<P_HOSPITAL_NEW> list = new List<P_HOSPITAL_NEW>();
                if (listHospital.Count > 0)
                {
                    foreach(P_HOSPITAL_NEW p in listHospital)
                    {
                        if(p.ApprovalStatus == 3 || p.ApprovalStatus == 4)
                        {
                            continue;
                        }
                        else
                        {
                            list.Add(p);
                        }
                    }
                }
                return Json(new { state = 1, rows = list });
            }
            catch (Exception ex)
            {
                LogHelper.Error("Exception SearchHospitalByGskHospital", ex);
                return Json(new { state = 0, rows = new List<P_HOSPITAL_NEW>() });
            }
        }
        protected string getRandrom(int num)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Random randrom = new Random((int)DateTime.Now.Ticks);
            string str = "";
            for (int i = 0; i < num; i++)
            {
                str += chars[randrom.Next(chars.Length)];
            }
            return str;
        }

        public JsonResult SaveNewAddress(string Market, string TA, string Province, string City, string District, string GskHospital, string AddAddress, string Distance, string Latitude, string Longitude, int Count, string HospitalName, string OtherAddressDistance,string MAddress,string AddressNameDisplay)
        {
            try
            {
                P_AddressApproval addressApproval = new P_AddressApproval();
                P_PreApproval preApprovalInfo = new P_PreApproval();
                //用户信息
                P_USERINFO userInfo = Session[MealH5.Util.ConstantHelper.CURRENTWXUSER] as P_USERINFO;
                var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
                //DM
                var userInfoChannel = UserInfoClientChannelFactory.GetChannel();
                var uploadChannel = UploadOrderApiClientChannelFactory.GetChannel();
                var uploadInfo = uploadChannel.FindApproveInfo(userInfo.UserId);
                var uploadInfoDMDelegate = userInfoChannel.isAgent(uploadInfo.UserId);

                addressApproval.ID = Guid.NewGuid();
                addressApproval.ApplierName = userInfo.Name;   //登录人姓名
                addressApproval.ApplierMUDID = userInfo.UserId;   //登录人ID
                if (userInfo.PhoneNumber == null || userInfo.PhoneNumber == "")
                {
                    addressApproval.ApplierMobile = "";
                }
                else
                {
                    addressApproval.ApplierMobile = userInfo.PhoneNumber;   //登录人手机号码
                }
                addressApproval.CreateDate = DateTime.Now;

                string DACode = string.Empty;
                string dacodeCheck = "DA-" + DateTime.Now.ToString("yyMMdd") + getRandrom(3);
                var preApprovalChannel = PreApprovalClientChannelFactory.GetChannel();
                var addressCheck = preApprovalChannel.LoadAddressApprovalByDACode(dacodeCheck);
                if(addressCheck.Count ==0)
                    DACode = dacodeCheck;
                else
                    DACode = "DA-" + DateTime.Now.ToString("yyMMdd") + getRandrom(3);

                addressApproval.DACode = DACode;
                addressApproval.Market = Market;
                addressApproval.TA = TA;
                addressApproval.Province = Province;
                addressApproval.City = City;
                addressApproval.District = District;
                addressApproval.ApprovalStatus = 0;
                addressApproval.LineManagerName = uploadInfoDMDelegate == null ? uploadInfo.Name : uploadInfoDMDelegate.DelegateUserName;   //审批人姓名
                LogHelper.Error("LineManagerName" + addressApproval.LineManagerName);
                addressApproval.LineManagerMUDID = uploadInfoDMDelegate == null ? uploadInfo.UserId : uploadInfoDMDelegate.DelegateUserMUDID;   //审批人ID
                addressApproval.GskHospital = GskHospital;
                addressApproval.HospitalCode = GskHospital + "_" + Count.ToString();
                addressApproval.HospitalName = HospitalName;
                addressApproval.AddAddress = AddAddress;
                addressApproval.Distance = Distance;
                addressApproval.Latitude = Latitude;
                addressApproval.Longitude = Longitude;
                addressApproval.AddressName = "地址" + Count.ToString();
                addressApproval.OtherAddressDistance = OtherAddressDistance;
                addressApproval.MAddress = MAddress;
                addressApproval.AddressNameDisplay = AddressNameDisplay;
                if (baseDataChannel.AddNewAddress(addressApproval) > 0)
                {
                    WxMessageHandler.GetInstance().SendAddressApprovalStateChangeMessageToUser(addressApproval);
                    return Json(new { state = 1 });
                }
                else
                    return Json(new { state = 0 });
            }
            catch (Exception ex)
            {
                LogHelper.Error("Exception SaveNewAddress", ex);
                return Json(new { state = 0 });
            }

        }

        [HttpGet]
        [iPathOAuthFilter(MappingKey = "0x0001", CallBackUrl = true)]
        public ActionResult AddressDetail(Guid id, int from)
        {
            ViewBag.preApprovalId = id;
            var channel = PreApprovalClientChannelFactory.GetChannel();
            var preApproval = channel.LoadAddressApprovalInfo(id);

            ViewBag.CurrentDay = DateTime.Now.Day;
            ViewBag.ApprovalStatus = preApproval.ApprovalStatus;

            ViewBag.TA = preApproval.TA;
            ViewBag.Market = preApproval.Market;
            ViewBag.GskHospital = preApproval.GskHospital;
            ViewBag.From = from;

            List<P_HOSPITAL_NEW> list_p_hospital_new = new List<P_HOSPITAL_NEW>();
            var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
            list_p_hospital_new = baseDataChannel.SearchMainHospitalByGskHospital(preApproval.GskHospital);
            //if (list_p_hospital_new != null && list_p_hospital_new.Count > 0)
            //{
            //    if (preApproval.MAddress != list_p_hospital_new[0].Address)
            //    {
            //        //医院主地址被修改
            //        ViewBag.IsDelUp = 3;
            //    }
            //}
            //else
            //{
            //    //医院驻地删除
            //    ViewBag.IsDelUp = 2;
            //}
            ViewBag.IsDelUp = preApproval.IsDeleteUpdate;

            //ViewBag.IsFinished = preApproval.IsFinished;
            //ViewBag.IsUsed = preApproval.IsUsed;
            //ViewBag.IsCrossMonth = (preApproval.MeetingDate.Value.Month != DateTime.Now.Month && DateTime.Now.Day < 5);
            //if (preApproval.IsUsed == 1)
            //{
            //    var order = channel.FindActivityOrderByHTCode(preApproval.HTCode);
            //    ViewBag.OrderState = order.State;
            //}
            return View();
        }

        public JsonResult LoadAddressApprovalInfo(Guid id)
        {
            var preApprovalChannel = PreApprovalClientChannelFactory.GetChannel();
            var preApproval = preApprovalChannel.LoadAddressApprovalInfo(id);
            return Json(new { state = 1, data = preApproval });
        }

        public JsonResult LoadAddressApprovalInfoForUpdate(Guid id)
        {
            var preApprovalChannel = PreApprovalClientChannelFactory.GetChannel();
            var preApproval = preApprovalChannel.LoadAddressApprovalInfoForUpdate(id);
            return Json(new { state = 1, data = preApproval });
        }

        public JsonResult AddressCancel(Guid id)
        {
            try
            {
                P_AddressApproval addressApproval = new P_AddressApproval();
                P_PreApproval preApprovalInfo = new P_PreApproval();
                //用户信息
                P_USERINFO userInfo = Session[MealH5.Util.ConstantHelper.CURRENTWXUSER] as P_USERINFO;
                var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
                //DM
                var userInfoChannel = UserInfoClientChannelFactory.GetChannel();
                var uploadChannel = UploadOrderApiClientChannelFactory.GetChannel();
                var uploadInfo = uploadChannel.FindApproveInfo(userInfo.UserId);
                var uploadInfoDMDelegate = userInfoChannel.isAgent(uploadInfo.UserId);

                //新增地址信息
                var preApprovalChannel = PreApprovalClientChannelFactory.GetChannel();
                var preApproval = preApprovalChannel.LoadAddressApprovalInfo(id);

                addressApproval.ID = id;
                addressApproval.ApplierMUDID = userInfo.UserId;
                addressApproval.DACode = preApproval.DACode;
                addressApproval.DACode = preApproval.DACode;
                addressApproval.HospitalName = preApproval.HospitalName;
                addressApproval.ApprovalStatus = 4;
                addressApproval.ApplierName = userInfo.Name;

                if (baseDataChannel.AddressCancel(addressApproval) > 0)
                {
                    WxMessageHandler.GetInstance().SendAddressApprovalStateChangeMessageToUser(addressApproval);
                    return Json(new { state = 1 });
                }
                else
                    return Json(new { state = 0 });
            }
            catch (Exception ex)
            {
                LogHelper.Error("Exception AddressCancel", ex);
                return Json(new { state = 0 });
            }
        }

        public JsonResult AddressUpdate(string AddAddress, string Distance, string Latitude, string Longitude, string OtherAddressDistance, string AddressApprovalID, string ActionType, int Count, string GskHospital, string MAddress, string AddressNameDisplay)
        {
            try
            {
                P_AddressApproval addressApproval = new P_AddressApproval();
                P_PreApproval preApprovalInfo = new P_PreApproval();
                //用户信息
                P_USERINFO userInfo = Session[MealH5.Util.ConstantHelper.CURRENTWXUSER] as P_USERINFO;
                var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
                //DM
                var userInfoChannel = UserInfoClientChannelFactory.GetChannel();
                var uploadChannel = UploadOrderApiClientChannelFactory.GetChannel();
                var uploadInfo = uploadChannel.FindApproveInfo(userInfo.UserId);
                var uploadInfoDMDelegate = userInfoChannel.isAgent(uploadInfo.UserId);

                //新增地址信息
                var preApprovalChannel = PreApprovalClientChannelFactory.GetChannel();
                var preApproval = preApprovalChannel.LoadAddressApprovalInfo(new Guid(AddressApprovalID));

                addressApproval.ID = new Guid(AddressApprovalID);
                addressApproval.AddAddress = AddAddress;
                addressApproval.Distance = Distance;
                addressApproval.Latitude = Latitude;
                addressApproval.Longitude = Longitude;
                addressApproval.OtherAddressDistance = OtherAddressDistance;
                addressApproval.ModifyDate = DateTime.Now;
                addressApproval.ApplierMUDID = userInfo.UserId;
                addressApproval.ApplierName = userInfo.Name;
                addressApproval.DACode = preApproval.DACode;
                addressApproval.HospitalName = preApproval.HospitalName;
                addressApproval.LineManagerMUDID = preApproval.LineManagerMUDID;

                addressApproval.AddressName = "地址" + Count.ToString();
                addressApproval.HospitalCode = GskHospital + "_" + Count.ToString();
                addressApproval.MAddress = MAddress;
                addressApproval.AddressNameDisplay = AddressNameDisplay;
                if (ActionType == "RESUBMIT")
                    addressApproval.ApprovalStatus = 10;
                else if(ActionType == "UPDATE")
                    addressApproval.ApprovalStatus = 9;


                if (baseDataChannel.AddressUpdate(addressApproval) > 0)
                {
                    WxMessageHandler.GetInstance().SendAddressApprovalStateChangeMessageToUser(addressApproval);
                    return Json(new { state = 1 });
                }
                else
                    return Json(new { state = 0 });
            }
            catch (Exception ex)
            {
                LogHelper.Error("Exception AddressUpdate", ex);
                return Json(new { state = 0 });
            }
        }
        [HttpGet]
        [iPathOAuthFilter(MappingKey = "0x0001", CallBackUrl = true)]
        public ActionResult AddressApprove(Guid id, int from)
        {
            ViewBag.preApprovalId = id;
            var channel = PreApprovalClientChannelFactory.GetChannel();
            var preApproval = channel.LoadAddressApprovalInfo(id);

            ViewBag.CurrentDay = DateTime.Now.Day;
            ViewBag.ApprovalStatus = preApproval.ApprovalStatus;

            ViewBag.TA = preApproval.TA;
            ViewBag.Market = preApproval.Market;
            ViewBag.GskHospital = preApproval.GskHospital;
            ViewBag.HospitalName = preApproval.HospitalName;
            ViewBag.DACode = preApproval.DACode;
            ViewBag.From = from;

            List<P_HOSPITAL_NEW> list_p_hospital_new = new List<P_HOSPITAL_NEW>();
            var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
            list_p_hospital_new = baseDataChannel.SearchMainHospitalByGskHospital(preApproval.GskHospital);
            //if (list_p_hospital_new != null && list_p_hospital_new.Count > 0)
            //{
            //    if(preApproval.MAddress != list_p_hospital_new[0].Address)
            //    {
            //        //医院主地址被修改
            //        ViewBag.IsDelUp = 3;
            //    }
            //}
            //else
            //{
            //    //医院驻地删除
            //    ViewBag.IsDelUp = 2;
            //}
            ViewBag.IsDelUp = preApproval.IsDeleteUpdate;
            ViewBag.Distance = preApproval.Distance == "" ? 0 : Int32.Parse(preApproval.Distance);

            //ViewBag.IsFinished = preApproval.IsFinished;
            //ViewBag.IsUsed = preApproval.IsUsed;
            //ViewBag.IsCrossMonth = (preApproval.MeetingDate.Value.Month != DateTime.Now.Month && DateTime.Now.Day < 5);
            //if (preApproval.IsUsed == 1)
            //{
            //    var order = channel.FindActivityOrderByHTCode(preApproval.HTCode);
            //    ViewBag.OrderState = order.State;
            //}
            return View();
        }
        #endregion

        #region 取消预申请
        [HttpPost]
        public JsonResult PreApprovalCancel(Guid id)
        {
            try
            {
                var channel = PreApprovalClientChannelFactory.GetChannel();
                var p_preApproval = channel.LoadPreApprovalInfo(id);

                if (channel.PreApprovalCancel(p_preApproval) > 0)
                {
                    p_preApproval.State = "10";
                    WxMessageHandler.GetInstance().SendPreApprovalRejectMessageToUser("", p_preApproval);
                    //WxMessageHandler.GetInstance().SendAddressApprovalStateChangeMessageToUser(addressApproval);
                    return Json(new { state = 1 });
                }
                else
                    return Json(new { state = 0 });

            }
            catch (Exception ex)
            {
                LogHelper.Error("Exception PreApprovalCancel", ex);
                return Json(new { state = 0 });
            }
        }
        #endregion
    }
}