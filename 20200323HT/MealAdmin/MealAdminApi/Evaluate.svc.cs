using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using MealAdmin.Entity;
using MealAdmin.Service;
using MealAdmin.Entity.View;

namespace MealAdminApi
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Evaluate”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Evaluate.svc 或 Evaluate.svc.cs，然后开始调试。
    public class Evaluate : IEvaluate
    {
        public IEvaluateService evaluateService = Global.applicationContext.GetBean("evaluateService") as IEvaluateService;
       
        #region 新建评价
        /// <summary>
        /// 新建评价
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int AddUnd(P_EVALUATE entity,string totalPrice)
        {
            return evaluateService.AddUnd(entity, totalPrice);
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
            return evaluateService.Add(entity);
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
            return evaluateService.LoadByOrderID(OrderID);
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
            return evaluateService.LoadByResId(resId);
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
            return evaluateService.LoadStarByResIds(resIds);
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
            return evaluateService.AddOrderApprove(entity);
        }
        #endregion
    }
}
