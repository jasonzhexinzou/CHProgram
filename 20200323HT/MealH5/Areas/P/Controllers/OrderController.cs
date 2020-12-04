using iPathFeast.API.Client;
using iPathFeast.ApiEntity;
using MealAdmin.Entity;
using MealAdmin.Entity.Enum;
using MealAdmin.Entity.Helper;
using MealAdminApiClient;
using MealH5.Areas.P.Filter;
using MealH5.Areas.P.Handler;
using MealH5.Models;
using MealH5.Util;
using MeetingMealApiClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using XFramework.XInject.Attributes;
using XFramework.XUtil;

namespace MealH5.Areas.P.Controllers
{
    /// <summary>
    /// 订单操作
    /// </summary>
    [WxUserFilter]
    public class OrderController : BaseController
    {
        [Autowired]
        ApiV1Client apiClient { get; set; }

        #region 转到个人订单首页
        /// <summary>
        /// 订单状态入口
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [iPathOAuthFilter(MappingKey = "0x0002", CallBackUrl = true)]
        public ActionResult Index1()
        {
            ViewBag.fromuri = "1";
            return View("Index");
        }

        /// <summary>6
        /// 确认收餐入口
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [iPathOAuthFilter(MappingKey = "0x0003", CallBackUrl = true)]
        public ActionResult Index2()
        {
            ViewBag.fromuri = "2";
            return View("Index");
        }

        /// <summary>
        /// 评价投诉入口
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [iPathOAuthFilter(MappingKey = "0x0004", CallBackUrl = true)]
        public ActionResult Index3()
        {
            ViewBag.fromuri = "3";
            return View("Index");
        }

        /// <summary>
        /// 未评价入口
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [iPathOAuthFilter(MappingKey = "NoNxyz", CallBackUrl = true)]
        public ActionResult Index4()
        {
            ViewBag.fromuri = "4";
            return View("Index");
        }
        #endregion

        #region 载入订单
        /// <summary>
        /// 载入订单
        /// </summary>
        /// <param name="end"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Load(string end, string state, int year, int month)
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
            var channel = OrderApiClientChannelFactory.GetChannel();
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
            if (state == "4,12")
            {
                var list = channel.LoadReceiveOrderByUserId(CurrentWxUser.UserId, _begin, _end, state, rows, page, out total);
                return Json(new { state = 1, rows = list });
            }
            else
            {
                if (year < 2018 || year == 2018 && month < 7)
                {
                    var list = channel.LoadOldOrderByUserId(CurrentWxUser.UserId, _begin, _end, state, rows, page, out total);
                    return Json(new { state = 1, rows = list });
                }
                else {
                    var list = channel.LoadByUserId(CurrentWxUser.UserId, _begin, _end, state, rows, page, out total);
                    return Json(new { state = 1, rows = list });
                }
            }
        }
        #endregion

        #region 订单详情
        /// <summary>
        /// 订单详情
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fromuri"></param>
        /// <returns></returns>
        [HttpGet]
        [iPathOAuthFilter(MappingKey = "0xNON", CallBackUrl = true)]
        public ActionResult Details(Guid id, string fromuri, string supplier)
        {
            ViewBag.orderId = id;
            ViewBag.fromuri = fromuri;
            ViewBag.supplier = supplier;
            return View();
        }
        #endregion

        #region 订单详情
        /// <summary>
        /// 订单详情
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fromuri"></param>
        /// <returns></returns>
        [HttpGet]
        [iPathOAuthFilter(MappingKey = "0xNON", CallBackUrl = true)]
        public ActionResult OrderDetails(Guid id, string fromuri, string supplier)
        {
            ViewBag.orderId = id;
            ViewBag.fromuri = fromuri;
            ViewBag.supplier = supplier;
            return View();
        }
        #endregion


        #region 转到评价页面
        /// <summary>
        /// 转到评价页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Evaluate(Guid id,string resId)
        {
            ViewBag.OrderId = id;
            ViewBag.resId = resId;
            return View();
        }
        #endregion

        #region 加载评价详情
        /// <summary>
        /// 加载评价详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult LoadEvaluate(Guid id)
        {
            var channel = EvaluateClientChannelFactory.GetChannel();
            var evalaute = channel.LoadByOrderID(id);
            return Json(new { state = 1, data = evalaute });
        }
        #endregion

        #region 评论
        /// <summary>
        /// 评论
        /// </summary>
        /// <returns></returns>
        public JsonResult Add(string orderID,string restaurantId,int star,int onTime,string onTimeDiscripion,
            int isSafe,string safeDiscepion,string safeImage,int health,string healthDiscrpion,string healthImage,
            int pack,string packDiscrpion,string packImage, int costEffective,string costEffectiveDiscrpion,string costEffectiveImage,string otherDiscrpion)
        {

            P_EVALUATE entity = new P_EVALUATE();
            entity.ID = Guid.NewGuid();
            entity.OrderID = new Guid(orderID);
            entity.RestaurantId = restaurantId == "" ? "" : restaurantId;
            entity.Star = star;
            entity.OnTime = onTime;
            entity.OnTimeDiscrpion = onTimeDiscripion;
            entity.IsSafe = isSafe;
            entity.SafeDiscrpion = safeDiscepion == "" ? "" : safeDiscepion;
            entity.SafeImage = safeImage == "" ? "" : safeImage;
            entity.Health = health;
            entity.HealthDiscrpion = healthDiscrpion == "" ? "" : healthDiscrpion;
            entity.HealthImage = healthImage == "" ? "" : healthImage;
            entity.Pack = pack;
            entity.PackDiscrpion = packDiscrpion == "" ? "" : packDiscrpion;
            entity.PackImage = packImage == "" ? "" : packImage;
            entity.CostEffective = costEffective;
            entity.CostEffectiveDiscrpion = costEffectiveDiscrpion == "" ? "" : costEffectiveDiscrpion;
            entity.CostEffectiveImage = costEffectiveImage == "" ? "" : costEffectiveImage;
            entity.OtherDiscrpion = otherDiscrpion == "" ? "" : otherDiscrpion;
            entity.State = 1;
            entity.CreateDate = DateTime.Now;

            var channel = EvaluateClientChannelFactory.GetChannel();
            int res = channel.Add(entity);
            if(res==1)
            {
                var _channel = OrderApiClientChannelFactory.GetChannel();
                Guid id = new Guid(orderID);
                var order = _channel.FindByID(id);
                // 发用户消息
                WxMessageHandler.GetInstance().SendMessageToUser(CurrentWxUser.UserId, order);

                return Json(new { state = 1 });
            }
            else
            {
                return Json(new { state = 0, errCode = 9008 });
            }
        }
        #endregion

        #region 转到审批页面
        /// <summary>
        /// 转到审批页面
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpGet]
        [iPathOAuthFilter(MappingKey = "0xNON", CallBackUrl = true)]
        public ActionResult OrderApproval(Guid id)
        {
            ViewBag.orderId = id;
            return View();
        }
        #endregion

        #region MMCoE审批
        /// <summary>
        /// MMCoE审批
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public JsonResult SaveMMCoEResult(Guid orderId, int state, string reason)
        {    
            var channel = OrderApiClientChannelFactory.GetChannel();
            var evaluateChannel = EvaluateClientChannelFactory.GetChannel();
            var order = channel.FindByID(orderId);
            var customerID = ConfigurationManager.AppSettings["CustomerID"];

            //审批驳回
            if (state==2)
            {
                var res = channel.MMCoEResult(orderId, state, reason);
                if(res==1)
                {
                    P_ORDER_APPROVE entity = new P_ORDER_APPROVE();
                    entity.ID = Guid.NewGuid();
                    entity.OrderID = orderId;
                    entity.CN = order.CN;
                    entity.UserId = order.UserId;
                    entity.OldState = order.State;
                    entity.NewState = OrderState.REJECT;
                    entity.Result =0;
                    entity.Comment = reason;
                    entity.IsWXClient = 1;
                    entity.CreateDate = DateTime.Now;
                    entity.CreateUserId = CurrentWxUser.UserId;
                    //添加审批记录
                    var evaluateRes = evaluateChannel.AddOrderApprove(entity);

                    if(evaluateRes==0)
                    {
                        LogHelper.Info($"订单状态修改成功，添加审批记录失败 - [{ orderId.ToString()}]");
                    }
                    // 发用户消息
                    WxMessageHandler.GetInstance().SendMessageToUser(order.UserId, order);
                    return Json(new { state = 1, txt = "订单审批已驳回" });
                }
                else
                {
                    LogHelper.Info($"（审批驳回）订单状态修改失败 - [{ orderId.ToString()}]");
                }
            }

            //审批通过
            else
            {
                //订单审批
                var _res = channel.MMCoEResult(orderId, state, reason);
                if(_res==1)
                {
                    //var openApiChannel = OpenApiChannelFactory.GetChannel();
                    //下新单
                    if (order.IsChange == 0)
                    {
                        var json = order.Detail;
                        P_Order orderInfo = JsonConvert.DeserializeObject<P_Order>(json);
                        var foodList = orderInfo.foods.foods.Select(a => a.ToFoodRequest()).ToArray();
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

                        //调用小秘书下单接口
                        //var openApiRes = openApiChannel.createOrder2(orderId.ToString(), string.Empty, "0", orderInfo);
                        var req = new CreateOrderReq()
                        {
                            _Channels = order.Channel,
                            enterpriseOrderId = order.EnterpriseOrderId,
                            oldiPathOrderId = string.Empty,
                            CustomerID = Guid.Parse(customerID),
                            sendTime = orderInfo.details.deliverTime.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                            foodFee = orderInfo.foods.foodFee.ToString(),
                            packageFee = orderInfo.foods.packageFee.ToString(),
                            sendFee = orderInfo.foods.sendFee.ToString(),
                            totalFee = orderInfo.foods.allPrice.ToString(),
                            invoiceTitle = orderInfo.hospital.invoiceTitle + " - " + orderInfo.hospital.dutyParagraph,
                            orderTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            remark = orderInfo.details.remark,
                            dinnerName = orderInfo.details.consignee,
                            dinnernum = orderInfo.details.attendCount.ToString(),
                            phone = orderInfo.details.phone,
                            address = orderInfo.hospital.address + " - " + orderInfo.details.deliveryAddress,
                            resId = orderInfo.foods.resId,
                            longitude = orderInfo.hospital.longitude,
                            latitude = orderInfo.hospital.latitude,
                            hospitalId = orderInfo.hospital.hospital,
                            foods = foods,
                            cityId = orderInfo.hospital.city,
                            cn = order.CN,
                            cnAmount = orderInfo.meeting.budgetTotal.ToString(),
                            mudId = orderInfo.meeting.userId,
                            typeId = "0"
                        };
                        var openApiRes = apiClient.CreateOrder(req);
                        if (openApiRes.errorCode == "0")
                        {
                            var _channel = OrderApiClientChannelFactory.GetChannel();
                            // 审批下单成功，保存小秘书单号
                            var res = channel.SaveXmsOrderId(orderId, openApiRes.iPathOrderId);
                            if (res > 0)
                            {
                                var _order = channel.FindByID(orderId);

                                P_ORDER_APPROVE entity = new P_ORDER_APPROVE();
                                entity.ID = Guid.NewGuid();
                                entity.OrderID = orderId;
                                entity.CN = _order.CN;
                                entity.UserId = _order.UserId;
                                entity.OldState = order.State;
                                entity.NewState = _order.State;
                                entity.Result = 1;
                                entity.Comment = reason;
                                entity.IsWXClient = 1;
                                entity.CreateDate = DateTime.Now;
                                entity.CreateUserId = CurrentWxUser.UserId;
                                //添加审批记录
                                var evaluateRes = evaluateChannel.AddOrderApprove(entity);

                                if(evaluateRes==0)
                                {
                                    LogHelper.Info($"审批通过，下单成功，保存小秘书单号成功，添加审批记录失败 - [{ orderId.ToString()}]");
                                }
                                //发用户消息
                                WxMessageHandler.GetInstance().SendMessageToUser(order.UserId, _order);
                                return Json(new { state = 1, txt = "订单审批已通过" });

                            }
                            else
                            {
                                LogHelper.Info("审批通过，下单成功，保存小秘书单号失败");
                            }
                        }
                        else
                        {
                            //审批退回审批前状态
                            var res = channel._MMCoEResult(orderId, 1, "");
                            if(res==1)
                            {
                                return Json(new { state = 0, txt = "调用小秘书下单接口失败，订单审批失败", errCode = openApiRes.errorCode });
                            }
                            else
                            {
                                LogHelper.Info($"订单状态修改成功，调用小秘书下单接口失败，订单审批退回失败 - [{ orderId.ToString()}]");
                            }
                        }
                    }
                    //改单
                    else
                    {
                        var json = order.ChangeDetail;
                        P_Order orderInfo = JsonConvert.DeserializeObject<P_Order>(json);
                        var ChangeID = Guid.NewGuid();
                        var XmsOrderId = string.Empty;
                        var foodList = orderInfo.foods.foods.Select(a => a.ToFoodRequest()).ToArray();
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
                        var _enterpriseOrderId = order.EnterpriseOrderId;
                        if (string.IsNullOrEmpty(_enterpriseOrderId))
                        {
                            int RandKey = new Random().Next(1, 999);
                            var two = (RandKey < 10 ? "00" + RandKey : (RandKey < 100 ? "0" + RandKey : RandKey.ToString()));
                            var _channel = orderInfo.supplier;
                            var _date = DateTime.Now.ToString("yyMMddHHmmss");
                            _enterpriseOrderId = _channel.ToUpper() + "-" + _date + two;
                        }
                        var type = string.IsNullOrEmpty(order.XmsOrderId) ? "0" : "1";
                        var req = new CreateOrderReq()
                        {
                            _Channels = order.Channel,
                            enterpriseOrderId = _enterpriseOrderId,
                            oldiPathOrderId = order.XmsOrderId,
                            CustomerID = Guid.Parse(customerID),
                            sendTime = orderInfo.details.deliverTime.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                            foodFee = orderInfo.foods.foodFee.ToString(),
                            packageFee = orderInfo.foods.packageFee.ToString(),
                            sendFee = orderInfo.foods.sendFee.ToString(),
                            totalFee = orderInfo.foods.allPrice.ToString(),
                            invoiceTitle = orderInfo.hospital.invoiceTitle + " - " + orderInfo.hospital.dutyParagraph,
                            orderTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            remark = orderInfo.details.remark,
                            dinnerName = orderInfo.details.consignee,
                            dinnernum = orderInfo.details.attendCount.ToString(),
                            phone = orderInfo.details.phone,
                            address = orderInfo.hospital.address + " - " + orderInfo.details.deliveryAddress,
                            resId = orderInfo.foods.resId,
                            longitude = orderInfo.hospital.longitude,
                            latitude = orderInfo.hospital.latitude,
                            hospitalId = orderInfo.hospital.hospital,
                            foods = foods,
                            cityId = orderInfo.hospital.city,
                            cn = orderInfo.CnCode,
                            cnAmount = orderInfo.meeting.budgetTotal.ToString(),
                            mudId = orderInfo.meeting.userId,
                            typeId = type
                        };
                        //调用小秘书下单接口
                        //var openApiRes = openApiChannel.createOrder2(order.ChangeID.ToString(), order.XmsOrderId,
                        //    string.IsNullOrEmpty(order.XmsOrderId) ? "0" : "1", orderInfo);
                        var openApiRes = apiClient.CreateOrder(req);

                        if (openApiRes.errorCode == "0")
                        {
                            if (string.IsNullOrEmpty(order.XmsOrderId))
                            {
                                XmsOrderId = openApiRes.iPathOrderId;
                            }
                            var _channel = OrderApiClientChannelFactory.GetChannel();
                            // 写入改单后的新数据
                            //var res = _channel.Change(orderId, ChangeID, orderInfo);
                            //if (res > 0)
                            //{
                            //    if (!string.IsNullOrEmpty(XmsOrderId))
                            //    {
                            //        // 从未在小秘书下单过的改单，下单成功后应写入小秘书单号
                            //        _channel.SaveXmsOrderId(orderId, XmsOrderId);
                            //    }
                            //}
                            OrderInfo = null;
                            var _order = channel.FindByID(orderId);

                            P_ORDER_APPROVE entity = new P_ORDER_APPROVE();
                            entity.ID = Guid.NewGuid();
                            entity.OrderID = orderId;
                            entity.CN = _order.CN;
                            entity.UserId = _order.UserId;
                            entity.OldState = order.State;
                            entity.NewState = _order.State;
                            entity.Result = 1;
                            entity.Comment = reason;
                            entity.IsWXClient = 1;
                            entity.CreateDate = DateTime.Now;
                            entity.CreateUserId = CurrentWxUser.UserId;
                            //添加审批记录
                            var evaluateRes = evaluateChannel.AddOrderApprove(entity);

                            if (evaluateRes == 0)
                            {
                                LogHelper.Info($"审批通过，（改单）下单成功，保存小秘书单号成功，添加审批记录失败 - [{ orderId.ToString()}]");
                            }

                            //发用户消息
                            WxMessageHandler.GetInstance().SendMessageToUser(order.UserId, _order);
                            return Json(new { state = 1, txt = "订单审批已通过" });
                        }
                        else
                        {
                            var res = channel._MMCoEResult(orderId, 1, "");
                            if (res == 1)
                            {
                                return Json(new { state = 0, txt = openApiRes.errorMsg, errCode = openApiRes.errorCode });
                            }
                            else
                            {
                                LogHelper.Info($"订单状态修改成功，调用供应商下单接口失败，订单审批退回失败 - [{ orderId.ToString()}]");
                            }
                        }
                    }
                }
                else
                {
                    LogHelper.Info($"（审批通过）订单状态修改失败 - [{ orderId.ToString()}]");
                }
            }

            return Json(new { state = 0, txt = "订单审批失败，请刷新页面后重试。", errCode = 9007 });
        }
        #endregion

        #region 载入订单的详情
        /// <summary>
        /// 载入订单的详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LoadOrderInfo(Guid id)
        {
            var channel = OrderApiClientChannelFactory.GetChannel();
            var orderInfo = channel.FindByID(id);
            var preApprovalChannel = PreApprovalClientChannelFactory.GetChannel();
            var preAproval = preApprovalChannel.CheckPreApprovalState(orderInfo.CN);
            //var json = orderInfo.Detail;
            //P_Order _orderInfo = JsonConvert.DeserializeObject<P_Order>(json);
            return Json(new { state = 1, data = orderInfo,preApproval= preAproval });
        }
        #endregion

        #region 载入订单的详情(HT)
        /// <summary>
        /// 载入订单的详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LoadHTOrderInfo(Guid id)
        {
            var channel = OrderApiClientChannelFactory.GetChannel();
            var orderInfo = channel.FindOldOrderByID(id);
            //var json = orderInfo.Detail;
            //P_Order _orderInfo = JsonConvert.DeserializeObject<P_Order>(json);
            return Json(new { state = 1, data = orderInfo });
        }
        #endregion

        #region 转到未送达页面
        /// <summary>
        /// 转到未送达页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Undelivered(Guid id, string resId,string supplier, string htCode)
        {
            ViewBag.OrderId = id;
            ViewBag.resId = resId;
            ViewBag.supplier = supplier;
            ViewBag.HTCode = htCode;
            var channel = OrderApiClientChannelFactory.GetChannel();
            var orderInfo = channel.FindByID(id);
            ViewBag.OrderState = orderInfo.State;
            return View();
        }
        #endregion

        #region 未送达
        /// <summary>
        /// 未送达
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="restaurantId"></param>
        /// <param name="onTimeDiscripion"></param>
        /// <returns></returns>
        public JsonResult AddUndelivered(string orderID,string restaurantId,string onTimeDiscripion,string supplier,int normal)
        {
            P_EVALUATE entity = new P_EVALUATE();
            entity.ID = Guid.NewGuid();
            entity.OrderID = new Guid(orderID);
            entity.RestaurantId = restaurantId;
            entity.OnTime = 0;
            entity.OnTimeDiscrpion = onTimeDiscripion;
            entity.State = 1;
            entity.CreateDate = DateTime.Now;
            entity.Normal = normal;
            var channel = EvaluateClientChannelFactory.GetChannel();
            int res = channel.Add(entity);

            if (res == 1)
            {
                var _channel = OrderApiClientChannelFactory.GetChannel();
                Guid id = new Guid(orderID);
                var order = _channel.FindByID(id);
                // 发用户消息
                WxMessageHandler.GetInstance().SendMessageToUser(CurrentWxUser.UserId, order);

                // 告诉小秘书没收到餐
                Task.Factory.StartNew(() =>
                {
                    //var oponapiChannel = OpenApiChannelFactory.GetChannel();
                    //var xmlres = oponapiChannel.orderDeliveryFailure(order.XmsOrderId, string.Empty);
                    var req = new OrderDeliveryFailureReq()
                    {
                        _Channels = supplier,
                        iPathOrderId = order.XmsOrderId,
                        remark = onTimeDiscripion
                    };
                    var iPathRes = apiClient.OrderDeliveryFailure(req);
                });
                
                return Json(new { state = 1 });
            }
            else
            {
                return Json(new { state = 0, errCode = 9008 });
            }
        }
        #endregion

        #region 确认收餐页面
        /// <summary>
        /// 确认收餐页面
        /// </summary>
        /// <param name="id"></param>
        /// <param name="supplier"></param>
        /// <param name="price"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Confirm(Guid id,string supplier,string price,string count,string htCode)
        {
            ViewBag.OrderID = id;
            ViewBag.Supplier = supplier;
            ViewBag.Price =string.Format("{0:N2}", decimal.Parse(price)) ;
            ViewBag.Count = count;
            ViewBag.HTCode = htCode;
            var channel = OrderApiClientChannelFactory.GetChannel();
            var orderInfo = channel.FindByID(id);
            ViewBag.OrderState = orderInfo.State;
            return View();
        }
        #endregion

        #region 确认收餐
        /// <summary>
        /// 确认收餐
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ConfirmOrder(Guid id,string supplier,string price,string reason,string remark,string count,string cReason,string cRemark,string isSame)
        {
            // 改数据库状态
            var channel = OrderApiClientChannelFactory.GetChannel();
            var res = channel.Confirm(id, price, reason, remark, count, cReason, cRemark, isSame);
            if (res > 0)
            {
                var order = channel.FindByID(id);
                // 发用户消息
                WxMessageHandler.GetInstance().SendMessageToUser(order.IsTransfer==0?CurrentWxUser.UserId:order.TransferUserMUDID, order);

                // 告诉小秘书已经收餐
                //var oponapiChannel = OpenApiChannelFactory.GetChannel();
                //var xmlres = oponapiChannel.finishOrder(order.XmsOrderId, "0", string.Empty);
                var req = new FinishOrderReq()
                {
                    _Channels = supplier,
                    iPathOrderId = order.XmsOrderId,
                    type = "0",
                    remark = string.Empty
                };
                var iPathRes = apiClient.FinishOrder(req);
                

                return Json(new { state = 1 });
            }
            return Json(new { state = 0, txt = "操作失败",errCode= 9008 });
        }
        #endregion

        #region 开始修改订单流程
        /// <summary>
        /// 开始修改订单流程
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult BeginChangeOrder(Guid id)
        {
            WeChatOrderInfo = null;
            ChangeOrderID = null;
            var orderChannel = OrderApiClientChannelFactory.GetChannel();
            var order = orderChannel.FindByID(id);
            if (order.State != OrderState.SCHEDULEDSUCCESS && order.State != OrderState.REJECT)
            {
                // 非预定成功并且非审批拒绝的状态，不可改单
                return Json(new { state = 0, txt = "当前订单状态不允许改单", errCode = 9006 });
            }
            ChangeOrderID = id;
            if (order.IsChange == OrderIsChange.YES)
            {
                WeChatOrderInfo = JsonConvert.DeserializeObject<P_WeChatOrder>(order.ChangeDetail);
            }
            else
            {
                WeChatOrderInfo = JsonConvert.DeserializeObject<P_WeChatOrder>(order.Detail);
            }
            ChangePreApproval(PreApproval.HTCode);
            return Json(new { state = 1});
        }
        #endregion

        #region 退单
        /// <summary>
        /// 退单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Refunds(Guid id,string supplier)
        {
            var orderChannel = OrderApiClientChannelFactory.GetChannel();
            var order = orderChannel.FindByID(id);

            //if (order.State != OrderState.SCHEDULEDSUCCESS)
            //{
            //    return Json(new { state = 0, txt = "当前订单状态不可执行退单操作！" ,errCode = 9009});
            //}

            //var channel = OpenApiChannelFactory.GetChannel();
            //var res = channel.cancleOrder(order.XmsOrderId, string.Empty);

            RefundOrderReq req = new RefundOrderReq()
            {
                _Channels= supplier,
                iPathOrderId = order.XmsOrderId
            };
            var res = apiClient.RefundOrder(req);

            if (res.errorCode == "0")
            {
                int cancelRes = orderChannel.CancelOrder(id);
                if (cancelRes > 0)
                {
                    // 发用户消息
                    WxMessageHandler.GetInstance().SendMessageToUser(CurrentWxUser.UserId, order);
                    return Json(new { state = 1 });
                }
                else
                {
                    return Json(new { state = 0, txt = "退单请求发送成功，但订单状态更改失败。", errCode = 9010 });
                }
                
            }

            return Json(new { state = 0, txt = "操作失败！", errCode = 9008 });
        }
        #endregion

        #region 转到原订单配送页面
        /// <summary>
        /// 转到原订单配送页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [iPathOAuthFilter(MappingKey = "0xNON", CallBackUrl = true)]
        public ActionResult OriginalOrder(Guid id,string supplier)
        {
            ViewBag.orderId = id;
            ViewBag.supplier = supplier;
            var orderChannel = OrderApiClientChannelFactory.GetChannel();
            var order = orderChannel.FindByID(id);
            var deliverTime=order.DeliverTime;
            var timeStamp = (double)(deliverTime - DateTime.Now).TotalHours;
            if (timeStamp > 1.0)
            {
                ViewBag.flag1 = true;
            }
            else
            {
                ViewBag.flag1 = false;
            }
            if (order.IsRetuen == 3)
            {
                ViewBag.flag = true;
            }else
            {
                ViewBag.flag = false;
            }
            return View();
        }
        #endregion

        #region 原单配送
        /// <summary>
        /// 原单配送
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult OriginalOrderSend(Guid id,string supplier)
        {
            var orderChannel = OrderApiClientChannelFactory.GetChannel();
            var order = orderChannel.FindByID(id);

            if (!(order.State == OrderState.RETURNFAIL && order.IsRetuen == OrderIsRetuen.FAIL))
            {
                return Json(new { state = 0, txt = "当前订单状态不可执行该操作！", errCode = 9011 });
            }

            //var channel = OpenApiChannelFactory.GetChannel();
            //var res = channel.cancleFailOrderFeedBack(order.XmsOrderId, "0", string.Empty);
            var req = new CancleFailOrderFeedBackReq()
            {
                _Channels = order.Channel,
                type = "0",
                iPathOrderId = order.XmsOrderId,
                remark = string.Empty
            };
            var res = apiClient.CancleFailOrderFeedBack(req);

            if (res.errorCode == "0")
            {
                int cancelRes = orderChannel.OriginalOrderSend(id);
                if (cancelRes > 0)
                {
                    // 发用户消息
                    WxMessageHandler.GetInstance().SendMessageToUser(CurrentWxUser.UserId, order);
                    return Json(new { state = 1 });
                }
                else
                {
                    return Json(new { state = 0, txt = "退单请求发送成功，但订单状态更改失败。", errCode = 9010 });
                }

            }
            return Json(new { state = 0, txt = "操作失败！", errCode = 9008 });
        }
        #endregion

        #region 重新获取预申请信息
        /// <summary>
        /// 重新获取预申请信息
        /// </summary>
        /// <param name="HTCode"></param>
        /// <returns></returns>
        public void ChangePreApproval(string code)
        {
            var preApprovalClient = PreApprovalClientChannelFactory.GetChannel();
            var res = preApprovalClient.FindPreApprovalByHTCode(code);
            WeChatOrderInfo.preApproval = res.FirstOrDefault();
        }
        #endregion

        #region 获取订单详情
        public JsonResult GetOrderInfoByHTCode(string HTCode)
        {
            var orderChannel = OrderApiClientChannelFactory.GetChannel();
            var res = orderChannel.GetOrderInfoByHTCode(HTCode);
            return Json(new { state = 1, data = res });
        }
        #endregion
    }
}