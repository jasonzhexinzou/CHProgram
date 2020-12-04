using MealAdmin.Entity;
using MealAdmin.Entity.Enum;
using MealAdminApiClient;
using MealH5.Areas.P.Handler;
using MeetingMealEntity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XFramework.XUtil;

namespace MealH5.Areas.P.Controllers
{
    public class CallbackController : Controller
    {
        #region 订单状态回调
        /// <summary>
        /// 订单状态回调
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult OrderStateChange()
        {
            try
            {
                StreamReader reader = new StreamReader(Request.InputStream);
                var req = reader.ReadToEnd();
                LogHelper.Info(req);

                OrderStateChangeCallBack request = JsonConvert.DeserializeObject<OrderStateChangeCallBack>(req);
                var res = request.requestData;
                if (res.status == 2 && res.supplierOrderId == res.oldsupplierOrderId)
                {
                    // 迁就小秘书，把改单失败的状态码，从小秘书2改成我们需要的3
                    res.status = 3;
                }

                var channel = OrderApiClientChannelFactory.GetChannel();
                var orderId = string.IsNullOrEmpty(res.oldsupplierOrderId) ? res.supplierOrderId : res.oldsupplierOrderId;
                var order = channel.FindByXmlOrderId(orderId);

                if(order.State==5 || order.State == 11)
                {
                    LogHelper.Info("code=805，无效订单");
                    return Json(new { code = 805, message = "无效订单，无法继续返回订单状态" });
                }

                switch (res.status)
                {
                    case 2:
                        {
                            var timeNow = DateTime.Now;
                            if (timeNow > order.DeliverTime)
                            {
                                LogHelper.Info("code=804");
                                return Json(new { code = 804, message = "订单状态返回时间超过送餐时间" });
                            }
                            // 预定成功
                            var i = channel.ScheduledSuccess(res.supplierOrderId, res.CheckCode, res.remark, res.oldsupplierOrderId);
                            if (i > 0)
                            {
                                var _order = channel.FindByXmlOrderId(res.supplierOrderId);
                                if (_order.IsNonHT == 0)
                                {
                                    WxMessageHandler.GetInstance().SendMessageToUser(_order.UserId, _order);
                                }
                            }
                        }
                        break;
                    case 3:
                        {
                            var timeNow = DateTime.Now;
                            if (timeNow > order.DeliverTime)
                            {
                                LogHelper.Info("code=804");
                                return Json(new { code = 804, message = "订单状态返回时间超过送餐时间" });
                            }  
                            // 预定失败
                            var i = channel.ScheduledFail(res.supplierOrderId, res.remark, res.oldsupplierOrderId);
                            if (i > 0)
                            {
                                var _order = channel.FindByXmlOrderId(res.supplierOrderId);
                                if (_order.IsNonHT == 0)
                                {
                                    WxMessageHandler.GetInstance().SendMessageToUser(_order.UserId, _order);
                                }
                            }
                        }
                        break;
                    case 4:
                        {
                            // 配送成功
                            // 配送成功暂时无需进行数据库操作
                        }
                        break;
                    case 5:
                        {
                            
                            var timeNow = DateTime.Now;
                            var dataTime = order.ReturnOrderDate;
                            dataTime = dataTime.AddHours(18);
                            if (order.IsRetuen==4)
                            {
                                dataTime = order.DeliverTime;
                            }

                            if (timeNow > dataTime)
                            {
                                LogHelper.Info("code=804");
                                return Json(new { code = 804, message = "订单状态返回时间超过送餐时间" });
                            }

                            // 退订失败（只送小票 发票）
                            var i = -1;
                            //if (_order.State == OrderState.RETURNING)
                            //{
                            //    i = channel.CancelOrderFail(res.xmsOrderId, res.remark);
                            //}
                            //else if(_order.State == OrderState.RETURNFAIL && _order.IsRetuen== OrderIsRetuen.POSTFOOD)
                            //{
                            //    i = channel.OriginalOrderSendFail(res.xmsOrderId, res.remark);
                            //}

                            if (order.IsRetuen == OrderIsRetuen.POSTFOOD)
                            {
                                i = channel.OriginalOrderSendFail(res.supplierOrderId, res.remark);
                            }
                            else
                            {
                                i = channel.CancelOrderFail(res.supplierOrderId, res.remark);
                            }
                            if (i > 0)
                            {
                                var _order = channel.FindByXmlOrderId(res.supplierOrderId);
                                if (_order.IsNonHT == 0)
                                {
                                    WxMessageHandler.GetInstance().SendMessageToUser(_order.UserId, _order);
                                }
                            }

                        }
                        break;
                    case 6:
                        {
                            
                            var timeNow = DateTime.Now;
                            var dataTime = order.ReturnOrderDate;
                            dataTime = dataTime.AddHours(18);
                            if (order.IsRetuen == 4)
                            {
                                dataTime = order.DeliverTime;
                            }

                            if (timeNow > dataTime)
                            {
                                LogHelper.Info("code=804");
                                return Json(new { code = 804, message = "订单状态返回时间超过送餐时间" });
                            }

                            // 退订订单
                            var i = channel.CancelOrderSuccess(res.supplierOrderId);
                            if (i > 0)
                            {
                                var _order = channel.FindByXmlOrderId(res.supplierOrderId);
                                if (_order.IsNonHT == 0)
                                {
                                    WxMessageHandler.GetInstance().SendMessageToUser(_order.UserId, _order);
                                }
                            }
                        }
                        break;
                    case 7:
                        {
                            // 订单未送达
                            // 未送达暂时无需操作
                        }
                        break;
                    case 8:
                        {
                           
                            var timeNow = DateTime.Now;
                            var dataTime = order.ReturnOrderDate;
                            dataTime = dataTime.AddHours(18);
                            if (order.IsRetuen == 4)
                            {
                                dataTime = order.DeliverTime;
                            }

                            if (timeNow > dataTime)
                            {
                                LogHelper.Info("code=804");
                                return Json(new { code = 804, message = "订单状态返回时间超过送餐时间" });
                            }
                            var i = -1;
                            // 退订失败（继续配送）
                            if (order.IsRetuen == OrderIsRetuen.POSTFOOD && order.State == OrderState.RETURNFAIL)
                            {
                                i = channel.OriginalOrderSendSuccess(res.supplierOrderId,res.remark);
                            }
                            else
                            {
                                i= channel.OriginalOrderSendSuccess(res.supplierOrderId, res.remark);
                            }
                            //var i = channel.OriginalOrderSendFail(res.xmsOrderId);
                            if (i > 0)
                            {
                                var _order = channel.FindByXmlOrderId(res.supplierOrderId);
                                if (_order.IsNonHT == 0)
                                {
                                    WxMessageHandler.GetInstance().SendMessageToUser(_order.UserId, _order);
                                }
                            }


                        }
                        break;

                }

                return Json(new { code = 200, message = "ok" });
            }
            catch (Exception exp)
            {
                LogHelper.Error("Exception OrderStateChange", exp);
                return Json(new { code = 500, message = "error" });
            }

        }
        #endregion

        #region 订单金额改动回调
        /// <summary>
        /// 订单金额改动回调
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult OrderFeeChange()
        {
            try
            {
                StreamReader reader = new StreamReader(Request.InputStream);
                var req = reader.ReadToEnd();
                LogHelper.Info(req);

                OrderFeeChangeCallBack request = JsonConvert.DeserializeObject<OrderFeeChangeCallBack>(req);

                var channel = OrderApiClientChannelFactory.GetChannel();
                var res = request.requestData;
                var order = channel.FindByXmlOrderId(res.supplierOrderId);
                var detail = (JObject)JsonConvert.DeserializeObject(order.Detail);
                var timeNow = DateTime.Now;

                #region 判断是否超预算
                var baseDataChannel = BaseDataClientChannelFactory.GetChannel();
                var cBudget = baseDataChannel.FindCityBudget(order.HospitalId);
                if (cBudget.IsOut == 0)
                {
                    if (res.totalFee / order.AttendCount > 60)
                    {
                        LogHelper.Info("code=802");
                        return Json(new { code = 802, message = "人均实际金额超过60元" });
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(cBudget.CityName))
                    {
                        if (res.totalFee / order.AttendCount > 100)
                        {
                            LogHelper.Info("code=802");
                            return Json(new { code = 802, message = "人均实际金额超过100元" });
                        }
                    }
                    //一二线城市
                    else
                    {
                        if (res.totalFee / order.AttendCount > 150)
                        {
                            LogHelper.Info("code=802");
                            return Json(new { code = 802, message = "人均实际金额超过150元" });
                        }
                    }
                }
                #endregion

                if (timeNow >DateTime.Parse(detail["details"]["deliverTime"].ToString()))
                {
                    LogHelper.Info("code=803");
                    return Json(new { code = 803, message = "修改实际金额时间超过送餐时间" });
                }
                else if (res.totalFee > decimal.Parse(detail["preApproval"]["BudgetTotal"].ToString()))
                {
                    LogHelper.Info("code=801");
                    return Json(new { code = 801, message = "实际金额大于会议预算金额" });
                }
                else if (res.totalFee > order.TotalPrice)
                {
                    LogHelper.Info("code=804");
                    return Json(new { code = 804, message = "实际金额大于会议预定金额" });
                }
                //else if (res.totalFee / order.AttendCount > 60)
                //{
                //    LogHelper.Info("code=802");
                //    return Json(new { code = 802, message = "人均实际金额超过60元" });
                //}
                else
                {
                    int result = channel.XmsChangeTotalFee(res.supplierOrderId, res.totalFee, res.remark);
                    if(result > 0)
                    {
                        if (order.IsNonHT == 0)
                        {
                            WxMessageHandler.GetInstance().SendMessageToUserForChangeFee(res.supplierOrderId);
                        }
                        else
                        {
                            WxMessageHandler.GetInstance().SendNonHTMessageToUserForChangeFee(res.supplierOrderId);
                        }
                        LogHelper.Info("code=200");
                        return Json(new { code = 200, message = "ok" });
                    }
                    else
                    {
                        LogHelper.Info("数据库连接超时，更改订单金额失败，code=500");
                        return Json(new { code = 500, message = "fail" });
                    }
                    
                }
            }
            catch (Exception exp)
            {
                LogHelper.Error("Exception OrderFeeChange", exp);
                return Json(new { code = 500, message = "error" });
            }
        }
        #endregion

        #region 同步医院
        [HttpPost]
        public JsonResult SyncHospital()
        {
            try
            {
                StreamReader reader = new StreamReader(Request.InputStream);
                var req = reader.ReadToEnd();
                LogHelper.Info(req);
                SyscHospitalCallBack request = JsonConvert.DeserializeObject<SyscHospitalCallBack>(req);
                var res = request.requestData;
                var supplier = res.supplier.ToLower();
                switch (supplier)
                {
                    case "xms":
                        supplier = " P_HOSPITAL.IsXMS = '是' ";
                        break;
                    case "bds":
                        supplier = " P_HOSPITAL.IsBDS = '是' ";
                        break;

                }

                var channel = HospitalClientChannelFactory.GetChannel();

                var hospital = channel.LoadHospital(supplier);

                return LargeJson(new { code = 200, message = "ok", result = hospital });
            }
            catch (Exception exp)
            {
                LogHelper.Error("Exception SyncHospital", exp);
                return Json(new { code = 500, message = "error" });
            }
        }
        #endregion

        public JsonResult LargeJson(object data)
        {
            return new System.Web.Mvc.JsonResult()
            {
                Data = data,
                MaxJsonLength = Int32.MaxValue,
            };
        }

    }
}
