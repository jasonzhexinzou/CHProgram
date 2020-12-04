using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MealAdmin.Entity;
using XFramework.XInject.Attributes;
using MealAdmin.Dao;
using MealAdmin.Entity.View;

namespace MealAdmin.Service
{
    public class EvaluateService: IEvaluateService
    {
        [Bean("evaluateDao")]
        public IEvaluateDao evaluateDao { get; set; }

        [Bean("orderDao")]
        public IOrderDao orderDao { get; set; }

        [Bean("preApprovalDao")]
        public IPreApprovalDao preApprovalDao { get; set; }

        [Bean("groupMemberDao")]
        public IGroupMemberDao groupMemberDao { get; set; }

        #region 新建评价
        /// <summary>
        /// 新建评价
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int AddUnd(P_EVALUATE entity,string totalPrice)
        {
            var res= evaluateDao.Add(entity);
            return res;
        }
        #endregion

        #region 新建评价
        /// <summary>
        /// 新建评价
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Add(P_EVALUATE entity)
        {
            var res = evaluateDao.Add(entity);
            if (entity.OnTime == 0)
            {
                //写入用户确认金额
                orderDao.unUserPrice(entity.OrderID);
                preApprovalDao.FinishPreApproval(entity.OrderID);
                var order = orderDao.FindByID(entity.OrderID);
                var nowDate = DateTime.Now.AddHours(-48);
                var userId = order.UserId;
                if (order.IsTransfer == 1)
                {
                    userId = order.TransferUserMUDID;
                }
                var orderList = orderDao.LoadUserConfirmOrders(userId, nowDate);
                if (orderList.Count < 1)
                {
                    var group = groupMemberDao.FindByUser(userId);
                    if (group != null)
                    {
                        if (group.State == 0 && group.State2 == 0 && group.State3 == 0)
                        {
                            groupMemberDao.DelByState1(userId);
                        }
                        else
                        {
                            groupMemberDao.UpdateState1(userId);
                        }
                    }
                }
            }
            return res; 
        }
        #endregion

        #region 根据订单ID查询评价
        /// <summary>
        /// 根据订单ID查询评价
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public P_EVALUATE LoadByOrderID(Guid OrderID)
        {
            return evaluateDao.LoadByOrderID(OrderID);
        }
        #endregion

        #region 根据1.0订单ID查询评价
        /// <summary>
        /// 根据1.0订单ID查询评价
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public P_EVALUATE LoadByOldOrderID(Guid OrderID)
        {
            return evaluateDao.LoadByOldOrderID(OrderID);
        }
        #endregion

        #region 根据订单ID查询评价
        /// <summary>
        /// 根据订单ID查询评价
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public P_EVALUATE LoadNonHTEvaluateByOrderID(Guid OrderID)
        {
            return evaluateDao.LoadNonHTEvaluateByOrderID(OrderID);
        }
        #endregion

        #region 根据餐厅Id查询评价
        /// <summary>
        /// 根据餐厅Id查询评价
        /// </summary>
        /// <param name="resId"></param>
        /// <returns></returns>
        public List<P_EVALUATE> LoadByResId(string resId)
        {
            return evaluateDao.LoadByResId(resId);
        }
        #endregion

        #region 载入餐厅评分
        /// <summary>
        /// 载入餐厅评分
        /// </summary>
        /// <param name="resIds"></param>
        /// <returns></returns>
        public List<P_RESTAURANT_START_VIEW> LoadStarByResIds(string[] resIds)
        {
            var list = evaluateDao.LoadStarByResIds(resIds);

            foreach(var a in list)
            {
                a.Average = (a.Star * 1.0 / a.Count).ToString("0.0");
            }

            if (list.Count() < resIds.Length)
            {
                var otherResIds = resIds.Except(list.Select(a => a.RestaurantId).ToArray());
                list.AddRange(otherResIds.Select(a => new P_RESTAURANT_START_VIEW()
                {
                    RestaurantId = a,
                    Star = 0,
                    Count = 0,
                    Average = "5"
                }));
            }

            return list;
        }
        #endregion

        #region 添加审批记录
        /// <summary>
        /// 添加审批记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int AddOrderApprove(P_ORDER_APPROVE entity)
        {
            return evaluateDao.AddOrderApprove(entity);
        }
        #endregion
    }
}
