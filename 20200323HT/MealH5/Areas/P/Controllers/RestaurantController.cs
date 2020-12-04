using iPathFeast.API.Client;
using iPathFeast.ApiEntity;
using MealAdmin.Entity.Enum;
using MealAdmin.Entity.Helper;
using MealAdminApiClient;
using MealH5.Areas.P.Filter;
using MealH5.Areas.P.Handler;
using MealH5.Handler;
using MealH5.Models;
using MeetingMealApiClient;
using MeetingMealEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XFramework.XInject.Attributes;
using XFramework.XUtil;

namespace MealH5.Areas.P.Controllers
{
    #region 餐厅
    /// <summary>
    /// 餐厅
    /// </summary>
    [WxUserFilter]
    public class RestaurantController : BaseController
    {
        [Autowired]
        ApiV1Client apiClient { get; set; }

        #region 浏览餐厅
        /// <summary>
        /// 浏览餐厅
        /// </summary>
        /// <param name="formenu"></param>
        /// <param name="supplier"></param>
        /// <param name="changeOrder"></param>
        /// <returns></returns>
        [HttpGet]
        [OrderInfoFilter]
        public ActionResult ResList(string formenu, string supplier, string changeOrder)
        {
            ViewBag.isForMenu = formenu == "1";
            //20200211 
            if (ViewBag.isForMenu == false)
            {
                ViewBag.sendTime = "";
            }
            
            ViewBag.hospitalId = HospitalInfo.addressCode;
            ViewBag.supplier = supplier;
            ViewBag.isChange = changeOrder == "1";
            //ViewBag.hospitalId = "64942";
            var channelBase = BaseDataClientChannelFactory.GetChannel();
            var channelUser = UserInfoClientChannelFactory.GetChannel();
            var userInfo = channelUser.FindByUserId(CurrentWxUser.UserId);
            var listGroup = channelBase.LoadUserGroup(CurrentWxUser.UserId).Select(a => a.GroupType).ToArray();
            if (listGroup.Contains(GroupType.ServPause))
            {
                userInfo.IsServPause = 1;
            }
            ViewBag.IsServPause = CurrentWxUser.IsServPause;
            return View();
        }
        #endregion


        #region 餐厅列表
        /// <summary>
        /// 餐厅列表
        /// </summary>
        /// <param name="formenu"></param>
        /// <returns></returns>
        [HttpGet]
        [OrderFilter]
        public ActionResult List(string formenu, string supplier, string changeOrder, string sendTime, string changeSendTime)
        {
            ViewBag.isForMenu = formenu == "1";
            //20200214
            if (ViewBag.isForMenu == false && (sendTime == "" || sendTime == null))
            {
                ViewBag.sendTime = "";
            }
            else
            {
                ViewBag.sendTime = sendTime;
            }
            ViewBag.hospitalId = PreApproval.HospitalAddressCode;
            ViewBag.supplier = supplier;
            ViewBag.isChange = changeOrder == "1";
            ViewBag.changeSendTime = changeSendTime;
            ViewBag.restaurantId = "";
            ViewBag.restaurantName = "";

            if (changeOrder == "1")
            {
                var channelOrder = OrderApiClientChannelFactory.GetChannel();
                var res = channelOrder.FindOrderByCN(PreApproval.HTCode);
                ViewBag.restaurantId = res.RestaurantId;
                ViewBag.restaurantName = res.RestaurantName;
            }
            //ViewBag.hospitalId = "64942";
            var channelBase = BaseDataClientChannelFactory.GetChannel();
            var channelUser = UserInfoClientChannelFactory.GetChannel();
            var userInfo = channelUser.FindByUserId(CurrentWxUser.UserId);
            var listGroup = channelBase.LoadUserGroup(CurrentWxUser.UserId).Select(a => a.GroupType).ToArray();
            if (listGroup.Contains(GroupType.ServPause))
            {
                userInfo.IsServPause = 1;
            }
            ViewBag.IsServPause = CurrentWxUser.IsServPause;
            return View();
        }
        #endregion

        #region 送餐时间
        /// <summary>
        /// 送餐时间
        /// </summary>
        /// <param name="formenu"></param>
        /// <returns></returns>
        [HttpGet]
        [OrderFilter]
        public ActionResult SendTime(string formenu, string supplier, string changeOrder, string restaurantId,
            string businessHourStart, string businessHourEnd, string eveningHourStart, string eveningHourEnd, int? dataState, string changeSendTime,string restaurantName, string orderCreateDate)
        {
            ViewBag.isForMenu = formenu == "1";
            ViewBag.hospitalId = PreApproval.HospitalAddressCode;
            ViewBag.meetingTime = String.Format("{0:yyyy-MM-dd HH:mm:ss}", PreApproval.MeetingDate);                           
            ViewBag.supplier = supplier;
            ViewBag.isChange = changeOrder == "1";
            ViewBag.restaurantId = restaurantId;
            ViewBag.businessHourStart = businessHourStart;
            ViewBag.businessHourEnd = businessHourEnd;
            ViewBag.eveningHourStart = eveningHourStart;
            ViewBag.eveningHourEnd = eveningHourEnd;
            ViewBag.changeSendTime = changeSendTime;
            ViewBag.dataState = dataState!=null? dataState:0;
            var channelBase = BaseDataClientChannelFactory.GetChannel();
            var channelUser = UserInfoClientChannelFactory.GetChannel();
            var userInfo = channelUser.FindByUserId(CurrentWxUser.UserId);
            var listGroup = channelBase.LoadUserGroup(CurrentWxUser.UserId).Select(a => a.GroupType).ToArray();
            if (listGroup.Contains(GroupType.ServPause))
            {
                userInfo.IsServPause = 1;
            }
            ViewBag.IsServPause = CurrentWxUser.IsServPause;
            ViewBag.restaurantName = restaurantName;
            return View();
        }
        #endregion

        #region 保存
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="resId"></param>
        /// <returns></returns>
        [HttpPost]
        [OrderFilter]
        public JsonResult ChoosedRestaurant(string resId)
        {
            var openApiChannel = OpenApiChannelFactory.GetChannel();
            var res = openApiChannel.queryResFood(resId);
            if (res.code == "200")
            {
                WeChatOrderInfo.foods = new P_Food()
                {
                    resId = resId,
                    resName = res.result.resName,
                    resAddress = res.result.address,
                    resTel = res.result.resTel,
                    resLogo = res.result.imagePath
                };
                return Json(new { state = 1 });
            }

            return Json(new { state = 0, txt = "网络接口错误，操作失败！", errCode = 9012 });
        }
        #endregion

        #region 载入餐厅信息
        /// <summary>
        /// 载入餐厅信息
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LoadRestaurant(string hospitalId, string supplier,string sendTime)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("zh-cn");
            var channel = HospitalClientChannelFactory.GetChannel();
            var hospital = channel.FindByCode(hospitalId);

            if (hospital == null || string.IsNullOrEmpty(hospital.Name))
            {
                return Json(new { state = 0, txt = "载入餐厅列表数据失败！", errCode = 9013 });
            }

            //var openApiChannel = OpenApiChannelFactory.GetChannel();
            //var res = openApiChannel.queryRes(hospital.ID.ToString(), hospital.Address, hospital.Latitude, hospital.Longitude);
            if (string.IsNullOrEmpty(supplier))
            {
                supplier = "";
            }
            if (string.IsNullOrEmpty(sendTime))
            {
                sendTime = "";
            }
            var req = new SearchRestaurantReq()
            {
                _Channels = supplier,
                hospitalId = hospital.HospitalCode,
                address = hospital.Address,
                latitude = hospital.Latitude,
                longitude = hospital.Longitude,
                sendTime= sendTime,
                keyword = ""
            };
            var res = apiClient.SearchRestaurant(req);

            if (res == null)
            {
                return Json(new { state = 0, txt = "载入餐厅列表数据失败！", errCode = 9013 });
            }

            res = res.OrderBy(a => a.resName).ToList();

            #region 对接餐厅管理平台，排除黑名单餐厅
            //比对黑名单数据，对接餐厅管理平台
            try
            {
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
            }
            catch (Exception e)
            {
                return Json(new { state = 1, rows = e.Message });
            }
            #endregion

            //IComparer<SearchRestaurantRes> comparer = new Restaurant();
            //res.Sort(comparer);

            //20200221 修改订单修改餐厅过滤原餐厅
            if (ChangeOrderID != null)
            {
                var channelOrder = OrderApiClientChannelFactory.GetChannel();
                var list = channelOrder.FindOrderByCN(PreApproval.HTCode);
                string restaurantId = list.RestaurantId;
                var re = res.Where(a => a.resId != restaurantId).ToList();              
                return Json(new { state = 1, rows = re });
            }
            return Json(new { state = 1, rows = res });
        }
        #endregion

        #region 载入餐厅评星
        /// <summary>
        /// 载入餐厅评星
        /// </summary>
        /// <param name="resIds"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LoadStar(string[] resIds)
        {
            var channel = EvaluateClientChannelFactory.GetChannel();
            var list = channel.LoadStarByResIds(resIds);

            return Json(new { state = 1, rows = list });
        }
        #endregion

        #region 餐厅菜单
        /// <summary>
        /// 餐厅菜单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [OrderFilter]
        public ActionResult Menu(string restaurantId, string supplier, int? State, string sendTime, string changeSendTime, string restaurantName)
        {
            if (string.IsNullOrEmpty(restaurantId))
            {
                restaurantId = string.Empty;
            }
            //更新会议信息
            //var code = OrderInfo.CnCode;
            //ChangeMeeting(code);

            ViewBag.restaurantId = restaurantId;
            ViewBag.hospitalId = PreApproval.HospitalAddressCode;
            ViewBag.supplier = supplier;
            ViewBag.State = State;
            ViewBag.sendTime = sendTime;
            ViewBag.changeSendTime = changeSendTime;
            ViewBag.restaurantName = restaurantName;
            return View();
        }
        #endregion

        #region 载入餐厅菜单
        /// <summary>
        /// 载入餐厅菜单
        /// </summary>
        /// <param name="restaurantId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LoadMenu(string restaurantId, string supplier, string hospitalId, string sendTime)
        {
            //var openApiChannel = OpenApiChannelFactory.GetChannel();
            //var menu = openApiChannel.queryResFood(restaurantId);
            var req = new LoadMenuReq()
            {
                _Channels = supplier,
                resId = restaurantId,
                resName = "",
                hospitalId = hospitalId,
                sendTime = sendTime
            };
            var res = apiClient.LoadMenu(req);
            if (res != null)
            {
                return Json(new { state = 1, data = res });
            }
            return Json(new { state = 0, txt = "载入餐品失败！", errCode = "9005" });
        }
        #endregion

        #region 查找餐厅详情
        /// <summary>
        /// 查找餐厅详情
        /// </summary>
        /// <param name="restaurantId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult FindRestaurant(string hospitalId, string restaurantId, string supplier, int? dataState, string sendTime)
        {
            //var openApiChannel = OpenApiChannelFactory.GetChannel();
            //var res = openApiChannel.queryResFood(restaurantId);
            if (dataState != OrderState.SCHEDULEDSUCCESS)
            {
                //先判断是否加载餐厅管理平台黑名单餐厅，再判断餐厅是否被拉黑
                if (WebConfigHandler.IsLoadRestaurantData == "1")
                {
                    var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
                    //var resLaheiLIst = baseDataChannel.LoadRestaurantState();
                    //var lahei = resLaheiLIst.Where(p => p.ResId == restaurantId);
                    string alertMessage = "此餐厅已下线，请您重新选择其他餐厅。";
                    var resLaheiLIst = baseDataChannel.LoadRestaurantState();
                    var lahei = resLaheiLIst.Where(p => p.ResId == restaurantId && p.Status == 0);
                    //if (lahei == null || lahei.Count() == 0)
                    //{
                    //    return Json(new { state = 0, txt = alertMessage });
                    //}
                    //else
                    //{
                    //    lahei = lahei.Where(x => x.Status == 0);
                    //}
                    if (lahei.Count() > 0)
                    {
                        return Json(new { state = 0, txt = alertMessage });
                    }
                }
            }
            var req = new LoadMenuReq()
            {
                _Channels = supplier,
                resId = restaurantId,
                resName = "",
                hospitalId = hospitalId,
                sendTime = sendTime
            };
            var res = apiClient.LoadMenu(req);
            if (res != null)
            {
                return Json(new { state = 1, data = res });
            }
            return Json(new { state = 0, txt = "操作失败！", errCode = 9008 });
        }
        #endregion

        #region 预算费用
        /// <summary>
        /// 预算费用
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <param name="resId"></param>
        /// <param name="foods"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CalculateFee(string hospitalId, string resId, FoodRequest[] foods, string supplier, int? dataState, string sendTime)
        {
            //var openApiChannel = OpenApiChannelFactory.GetChannel();
            //var res = openApiChannel.calculateFee(hospitalId, resId, foods);
            if (dataState != OrderState.SCHEDULEDSUCCESS)
            {
                //先判断是否加载餐厅管理平台黑名单餐厅，再判断餐厅是否被拉黑
                if (WebConfigHandler.IsLoadRestaurantData == "1")
                {
                    var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
                    //var resLaheiLIst = baseDataChannel.LoadRestaurantState();
                    //var lahei = resLaheiLIst.Where(p => p.ResId == resId);
                    string alertMessage = "此餐厅已下线，请您重新选择其他餐厅。";
                    var resLaheiLIst = baseDataChannel.LoadRestaurantState();
                    var lahei = resLaheiLIst.Where(p => p.ResId == resId && p.Status == 0);
                    //if (lahei == null || lahei.Count() == 0)
                    //{
                    //    return Json(new { state = 0, txt = alertMessage });
                    //}
                    //else
                    //{
                    //    lahei = lahei.Where(x => x.Status == 0);
                    //}
                    if (lahei.Count() > 0)
                    {
                        return Json(new { state = 0, txt = alertMessage });
                    }
                }
            }

            var foodList = new List<iPathFeast.ApiEntity.Food>();
            foreach (var item in foods)
            {
                foodList.Add(new iPathFeast.ApiEntity.Food()
                {
                    foodId = item.foodId,
                    foodName = item.foodName,
                    count = Convert.ToInt32(item.count),

                });
            }
            var req = new CalcFeeReq()
            {
                _Channels = supplier,
                resId = resId,
                hospitalId = PreApproval.HospitalAddressCode,
                longitude = WeChatOrderInfo.hospital.Latitude,
                latitude = WeChatOrderInfo.hospital.Longitude,
                phone = "",
                foods = foodList,
                sendTime = sendTime
            };
            var res = apiClient.CalcFee(req);
            if (res != null)
            {
                return Json(new { state = 1, data = res });
            }
            return Json(new { state = 0, txt = "预算餐费失败！", errCode = 9014 });
        }
        #endregion

        #region 保存选餐
        /// <summary>
        /// 保存选餐
        /// </summary>
        /// <param name="resId"></param>
        /// <param name="foods"></param>
        /// <returns></returns>
        [HttpPost]
        [OrderFilter]
        public JsonResult SaveFood(string resId, string resName, P_FoodItem[] foods, string supplier, string hospitalId, string sendTime)
        {
            //var openApiChannel = OpenApiChannelFactory.GetChannel();
            //var res = openApiChannel.calculateFee(HospitalInfo.hospital, resId, foods.Select(a=> a.ToFoodRequest()).ToArray());
            var foodList = new List<iPathFeast.ApiEntity.Food>();
            foreach (var item in foods)
            {
                var food = item.ToFoodRequest();
                foodList.Add(new iPathFeast.ApiEntity.Food()
                {
                    foodId = food.foodId,
                    foodName = food.foodName,
                    count = Convert.ToInt32(food.count),
                    describe = food.describe
                });
            }
            var req = new CalcFeeReq()
            {
                _Channels = supplier,
                resId = resId,
                hospitalId = PreApproval.HospitalAddressCode,
                longitude = "",
                latitude = "",
                phone = "",
                foods = foodList,
                sendTime = sendTime
            };
            var res = apiClient.CalcFee(req);
            if (res != null)
            {
                var _req = new LoadMenuReq()
                {
                    _Channels = supplier,
                    resId = resId,
                    resName = "",
                    hospitalId= hospitalId,
                    sendTime = sendTime
                };
                var restaurant = apiClient.LoadMenu(_req);


                var nowfoods = new P_Food()
                {
                    resId = resId,
                    resName = resName,
                    resAddress = restaurant.address,
                    resTel = restaurant.resTel,
                    resLogo = restaurant.imagePath,
                    foods = foods,
                    allPrice = Convert.ToDecimal(res.allPrice),
                    foodFee = Convert.ToDecimal(res.foodFee),
                    packageFee = Convert.ToDecimal(res.packageFee),
                    sendFee = Convert.ToDecimal(res.sendFee)
                };

                if (nowfoods.allPrice > WeChatOrderInfo.preApproval.BudgetTotal)
                {
                    return Json(new { state = 0, txt = "您的订单金额已超出预申请预算，请修改预申请预算或修改订单金额。", errCode = 3874 });
                }

                //if (WeChatOrderInfo.foods != null && WeChatOrderInfo.details != null
                //    && WeChatOrderInfo.foods.resId != nowfoods.resId)
                //{
                //    WeChatOrderInfo.details.oldDeliverTime = WeChatOrderInfo.details.deliverTime;
                //    WeChatOrderInfo.details.deliverTime = null;
                //}
                WeChatOrderInfo.foods = nowfoods;
                WeChatOrderInfo.supplier = supplier;
                return Json(new { state = 1 });
            }

            return Json(new { state = 0, txt = "网络接口错误，提交失败！", errCode = 9012 });
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

        public JsonResult IsOnLine(string RestaurantId)
        {
            var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
            var resstatelist = baseDataChannel.LoadRestaurantState();
            var isCurrentDisable = resstatelist.Any(p => p.ResId == RestaurantId);
            if (!isCurrentDisable)
            {
                return Json(new { state = 0 });
            }
            else
            {
                var status = resstatelist.Where(x => x.ResId == RestaurantId && x.Status == 0);
                if (status != null && status.Count() > 0)
                {
                    return Json(new { state = 0 });
                }
                else
                {
                    return Json(new { state = 1 });
                }
            }
        }

        #region 提交订单餐厅是否被拉黑
        [HttpPost]
        public JsonResult IsLahei(string resId, string supplier, int? dataState)
        {
            if (dataState != OrderState.SCHEDULEDSUCCESS)
            {
                //先判断是否加载餐厅管理平台黑名单餐厅，再判断餐厅是否被拉黑
                if (WebConfigHandler.IsLoadRestaurantData == "1")
                {
                    var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
                    string alertMessage = "此餐厅已下线，请您重新选择其他餐厅。";
                    var resLaheiLIst = baseDataChannel.LoadRestaurantState();
                    var lahei = resLaheiLIst.Where(p => p.ResId == resId && p.Status == 0);
                    //if (lahei == null || lahei.Count() == 0)
                    //{
                    //    return Json(new { state = 0, txt = alertMessage });
                    //}
                    //else
                    //{
                    //    lahei = lahei.Where(x => x.Status == 0);
                    //}
                    if (lahei.Count() > 0)
                    {
                        return Json(new { state = 0, txt = alertMessage });
                    }
                }
            }
            return Json(new { state = 1, data = "" });
        }
        #endregion
    }
    #endregion


    public class Restaurant : IComparer<SearchRestaurantRes>
    {
        public int Compare(SearchRestaurantRes x, SearchRestaurantRes y)
        {
            return x.resName.CompareTo(y.resName);
        }
    }
}