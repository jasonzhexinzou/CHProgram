using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MealAdmin.Entity;
using MealAdmin.Dao;
using XFramework.XInject.Attributes;

namespace MealAdmin.Service
{
    public class DCostCenterService : IDCostCenterService
    {
        [Bean("dCostCenterDao")]
        public IDCostCenterDao centerDao { get; set; }

        #region 根据TA查询成本中心
        /// <summary>
        /// 根据TA查询成本中心
        /// </summary>
        /// <param name="ta">TA</param>
        /// <returns></returns>
        public List<D_COSTCENTER> LoadCostCenterByTA(string ta)
        {
            return centerDao.FindCostCenterByTA(ta);
        }
        #endregion

        #region 获取全部成本中心
        /// <summary>
        /// 获取全部成本中心
        /// </summary>
        /// <returns></returns>
        public List<D_COSTCENTER> GetAllCostCenter()
        {
            return centerDao.GetAllCostCenter();
        } 
        #endregion
    }
}
