using MealAdmin.Entity;
using MealAdmin.Entity.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Service
{
    public interface IEvaluateService
    {
        #region 根据订单ID查询评价
        /// <summary>
        /// 根据订单ID查询评价
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        P_EVALUATE LoadByOrderID(Guid OrderID);
        #endregion

        #region 根据1.0订单ID查询评价
        /// <summary>
        /// 根据1.0订单ID查询评价
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        P_EVALUATE LoadByOldOrderID(Guid OrderID);
        #endregion

        #region 根据订单ID查询评价
        /// <summary>
        /// 根据订单ID查询评价
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        P_EVALUATE LoadNonHTEvaluateByOrderID(Guid OrderID);
        #endregion

        #region 新建评价
        /// <summary>
        /// 新建评价
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int AddUnd(P_EVALUATE entity,string totalPrice);
        #endregion

        #region 新建评价
        /// <summary>
        /// 新建评价
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int Add(P_EVALUATE entity);
        #endregion

        #region 根据餐厅Id查询评价
        /// <summary>
        /// 根据餐厅Id查询评价
        /// </summary>
        /// <param name="resId"></param>
        /// <returns></returns>
        List<P_EVALUATE> LoadByResId(string resId);
        #endregion

        #region 载入餐厅评分
        /// <summary>
        /// 载入餐厅评分
        /// </summary>
        /// <param name="resIds"></param>
        /// <returns></returns>
        List<P_RESTAURANT_START_VIEW> LoadStarByResIds(string[] resIds);
        #endregion

        #region 添加审批记录
        /// <summary>
        /// 添加审批记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int AddOrderApprove(P_ORDER_APPROVE entity);
        #endregion
    }
}
