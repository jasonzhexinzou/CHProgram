using MealAdmin.Entity;
using MealAdminApiClient;
using MealH5.Areas.P.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MealH5.Areas.P.Controllers
{

    public class OtherController : BaseController
    {
        // GET: P/Other
        #region 转到个人中心页面
        /// <summary>
        /// 转到个人中心页面
        /// </summary>
        /// <returns></returns>
        [iPathOAuthFilter(MappingKey = "0x0017", CallBackUrl = true)]
        public ActionResult Personal()
        {
            var channel = MealAdminApiClient.UserInfoClientChannelFactory.GetChannel();
            var userId = CurrentWxUser.UserId;

            var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
            var listMarket = baseDataChannel.LoadMarket();
            ViewBag.listMarket = listMarket;

            //var isDevGroup = baseDataChannel.IsDevGroup(userId);

            var listTACode = baseDataChannel.LoadTACode(userId);
            var listMarketCode = baseDataChannel.LoadMarketByUserId(userId);

            if (listTACode != null && listTACode.Count > 0)
            {
                ViewBag.listMarket = listMarketCode;
                ViewBag.listTACode = listTACode;
            }else
            {
                ViewBag.listTACode = new List<P_TACode>();
            }
            

            var resta = channel.FindTAByUserId(userId);

            if(resta != null)
            {
                ViewBag.Name = resta.Name;
                ViewBag.UserId = resta.UserId;
                ViewBag.PhoneNumber = resta.PhoneNumber;
                ViewBag.Market = resta.Market;
                ViewBag.TACODE = resta.TerritoryCode;

                if (listMarketCode != null && listMarketCode.Count >0 && !listMarketCode.Select(x => x.Name).Contains(resta.Market))
                {
                    ViewBag.Market = listMarketCode.FirstOrDefault().Name;
                    //var MarketList = listMarket.Where(x => x.Name == resta.Market);
                    //if (MarketList != null && MarketList.Count() > 0)
                    //{
                    //    listMarketCode.Add(MarketList.FirstOrDefault());
                    //}
                }
                //else
                //{
                //    var MarketList = listMarket.Where(x => x.Name == resta.Market);
                //    if(MarketList != null && MarketList.Count() > 0)
                //    {
                //        listMarketCode.Add(MarketList.FirstOrDefault());
                //    }
                //}
            }
            else
            {
                //var res = channel.FindByUserId(userId);
                //if (res != null)
                //{
                //    ViewBag.Name = res.Name;
                //    ViewBag.UserId = res.UserId;
                //    ViewBag.PhoneNumber = res.PhoneNumber;
                //    ViewBag.Market = res.Market;
                //    ViewBag.TACODE = res.TerritoryCode;
                //}

                //else
                //{
                    var res1 = channel.Find(userId);

                    P_USERINFO entity = new P_USERINFO();
                    entity.ID = Guid.NewGuid();
                    entity.UserId = userId;
                    entity.Name = res1.Name;
                    entity.CreateDate = DateTime.Now;
                    var _res = channel.Add(entity);

                    ViewBag.Name = res1.Name;
                    ViewBag.UserId = userId;
                    if (string.IsNullOrEmpty(res1.PhoneNumber))
                    {
                        ViewBag.PhoneNumber = "";
                    }
                    else
                    {
                        ViewBag.PhoneNumber = res1.PhoneNumber;
                    }
                //}
            }

            

            return View();
        }
        #endregion

        /// <summary>
        /// 根据market获取TA
        /// </summary>
        /// <param name="marketName"></param>
        /// <returns></returns>
        public JsonResult LoadTa(string Market, string UserId)
        {
            var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
            var res = baseDataChannel.LoadTACodeByMarketAndUser(Market, UserId);
            return Json(new { state = 1, data = res });
        }

        public JsonResult ShowName(string AgentMudid, string UserId)
        {
            var channel = MealAdminApiClient.UserInfoClientChannelFactory.GetChannel();
            var pre = MealAdminApiClient.PreApprovalClientChannelFactory.GetChannel();
            var res = channel.Find(AgentMudid);
            if (res != null)//判断 MUDID是否存在
            {
                //var userId = res.UserId;
                var isHaveApproval = channel.isHaveApproval(AgentMudid);//判断是否是直线经理
                var has = pre.HasApprove(AgentMudid);//判断是否是BUHead
                //var Current = pre.HasApprove(UserId);//判断当前用户是否是BUHead
                //if (Current)
                //{
                //    return Json(new { state = 1, ishave = 1, res });//是BUHead无需验证代理人是否有权限
                //}                
                if (isHaveApproval.ApprovalCount > 0)//如果是直线经理则需要验证代理人是否是直线经理
                {
                    return Json(new { state = 1, ishave = 1, res });
                }
                else
                {
                    return Json(new { state = 1, ishave = 0, txt = "当前代理人不是直线经理,请重试！" });
                }
            }
            return Json(new { state = 1, ishave = 0, txt = "不存在此MUDID" });
        }


        public JsonResult ShowSecondName(string AgentMudid, string UserId)
        {
            var channel = MealAdminApiClient.UserInfoClientChannelFactory.GetChannel();
            var pre = MealAdminApiClient.PreApprovalClientChannelFactory.GetChannel();
            var res = channel.Find(AgentMudid);
            if (res != null)//判断 MUDID是否存在
            {
                //var userId = res.UserId;
                var isHaveApproval = channel.isSecondApproval(AgentMudid);//判断是否是直线经理
                var has = pre.HasApprove(AgentMudid);//判断是否是BUHead
                //var Current = pre.HasApprove(UserId);//判断当前用户是否是BUHead
                //if (Current)
                //{
                //    return Json(new { state = 1, ishave = 1, res });//是BUHead无需验证代理人是否有权限
                //}
                if (isHaveApproval.ApprovalCount > 0)//如果是直线经理则需要验证代理人是否是直线经理
                {
                    return Json(new { state = 1, ishave = 1, res });
                }
                else
                {
                    return Json(new { state = 1, ishave = 0, txt = "当前代理人不是二线经理,请重试！" });
                }
            }
            return Json(new { state = 1, ishave = 0, txt = "不存在此MUDID" });
        }

        #region 转到联系方式页面
        [iPathOAuthFilter(MappingKey = "0x0020", CallBackUrl = true)]
        public ActionResult Contact()
        {
            return View();
        }
        #endregion


        public ActionResult UserManual()
        {
            return View();
        }

        #region 修改手机号
        /// <summary>
        /// 修改手机号
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public JsonResult Edit(string userId, string userName, string phoneNumber, string delegateUserMUDID, string delegateUserName, string startTime, string endTime, int isEnable, string market, string DelegateID, string secondDelegateUserMUDID, string secondDelegateUserName, string startTimeSecond, string endTimeSecond, int isEnableSecond, string SecondDelegateID, string tacode)
        {
            var channel = MealAdminApiClient.UserInfoClientChannelFactory.GetChannel();
            var preChannel = MealAdminApiClient.PreApprovalClientChannelFactory.GetChannel();
            var DM = channel.isHaveApproval(userId);
            var BUHead = preChannel.HasApprove(userId);
            if (DM.ApprovalCount < 1 && !BUHead)
            {
                var res = channel.Edit(userId, phoneNumber, market, tacode);//修改手机号 
                return Json(new { state = 1 });
            }
            //var have = channel.AgentExist(Guid.Parse(DelegateID));//判断代理人信息是否存在
            //查询代理人是否存在 如果存在就修改 不存在新增

            if (!string.IsNullOrEmpty(startTimeSecond) && !string.IsNullOrEmpty(endTimeSecond))
            {
                if (SecondDelegateID == string.Empty)
                {
                    //创建代理人
                    Guid ID = Guid.NewGuid();
                    channel.SaveSecondAgent(ID, userId, userName, secondDelegateUserMUDID, secondDelegateUserName, Convert.ToDateTime(startTimeSecond), Convert.ToDateTime(endTimeSecond), isEnableSecond, userId);
                    //var res = channel.Edit(userId, phoneNumber, market);//修改手机号 
                }
                else
                {
                    //修改代理人信息
                    channel.UpdateSecondAgent(Guid.Parse(SecondDelegateID), userId, userName, secondDelegateUserMUDID, secondDelegateUserName, Convert.ToDateTime(startTimeSecond), Convert.ToDateTime(endTimeSecond), isEnableSecond, userId);
                    //var res = channel.Edit(userId, phoneNumber, market);//修改手机号 

                }
            }
            //else
            //{
            //    var res = channel.Edit(userId, phoneNumber, market);//修改手机号 
            //    //return Json(new { state = 1 });
            //}

            if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
            {
                if (DelegateID == string.Empty)
                {
                    //创建代理人
                    Guid ID = Guid.NewGuid();
                    channel.SaveAgent(ID, userId, userName, delegateUserMUDID, delegateUserName, Convert.ToDateTime(startTime), Convert.ToDateTime(endTime), isEnable, userId);
                    var res = channel.Edit(userId, phoneNumber, market, tacode);//修改手机号 
                }
                else
                {
                    //修改代理人信息
                    channel.UpdateAgent(Guid.Parse(DelegateID), userId, userName, delegateUserMUDID, delegateUserName, Convert.ToDateTime(startTime), Convert.ToDateTime(endTime), isEnable, userId);
                    var res = channel.Edit(userId, phoneNumber, market, tacode);//修改手机号 

                }
            }
            else
            {
                var res = channel.Edit(userId, phoneNumber, market, tacode);//修改手机号 
                return Json(new { state = 1 });
            }
            return Json(new { state = 1 });
        }
        #endregion

        #region 转到呼叫中心页面
        /// <summary>
        /// 转到呼叫中心页面
        /// </summary>
        /// <returns></returns>
        [iPathOAuthFilter(MappingKey = "0x0020", CallBackUrl = true)]
        public ActionResult CallCenter()
        {

            return View();
        }
        #endregion

        #region 转到中央订餐项目组页面
        /// <summary>
        /// 转到中央订餐项目组页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ProjectTeam()
        {
            return View();
        }
        #endregion

        #region 转到声明页面
        /// <summary>
        /// 转到声明页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Statement()
        {
            return View();
        }
        #endregion

        #region 保存同意声明动作
        /// <summary>
        /// 保存同意声明动作
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Statement")]
        public JsonResult _Statement()
        {
            var channelUser = UserInfoClientChannelFactory.GetChannel();
            channelUser.CheckedStatement(CurrentWxUser.UserId);

            return Json(new { state = 1 });
        }
        #endregion


        public JsonResult ExistentAgent(string UserId)
        {
            //判断当前用户是否是直线经理，是否有审批权限
            var channel = MealAdminApiClient.UserInfoClientChannelFactory.GetChannel();
            var preChannel = MealAdminApiClient.PreApprovalClientChannelFactory.GetChannel();
            var a = channel.isHaveApproval(UserId);
            var b = preChannel.HasApprove(UserId);
            if (a.ApprovalCount != 0 || b)
            {
                var obj = channel.isAgentBack(UserId);
                return Json(new { state = 1, data = obj, isAgent = 1 });
            }
            return Json(new { state = 1, isAgent = 0 });

        }

        public JsonResult ExistentSecondAgent(string UserId)
        {
            //判断当前用户是否是直线经理，是否有审批权限
            var channel = MealAdminApiClient.UserInfoClientChannelFactory.GetChannel();
            var preChannel = MealAdminApiClient.PreApprovalClientChannelFactory.GetChannel();
            var a = channel.isSecondApproval(UserId);
            var b = preChannel.HasApprove(UserId);
            if (a.ApprovalCount != 0 || b)
            {
                var obj = channel.isSecondAgentBack(UserId);
                return Json(new { state = 1, data = obj, isAgent = 1 });
            }
            return Json(new { state = 1, isAgent = 0 });

        }
    }
}