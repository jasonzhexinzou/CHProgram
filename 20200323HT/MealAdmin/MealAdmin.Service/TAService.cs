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
    public class TAService : ITAService
    {
        [Bean("taDao")]
        public ITADao taDao { get; set; }
        [Bean("userInfoDao")]
        public IUserInfoDao userInfoDao { get; set; }

        #region 载入全部TA信息
        /// <summary>
        /// 载入全部TA信息
        /// </summary>
        /// <returns></returns>
        public List<P_TA> Load()
        {
            return taDao.Load();
        }
        #endregion

        #region 根据MarketName查询TA
        /// <summary>
        /// 根据MarketName查询TA
        /// </summary>
        /// <param name="marketName"></param>
        /// <returns></returns>
        public List<D_COSTCENTER> LoadCostCenterByTA(string ta)
        {
            return taDao.LoadCostCenterByTA(ta);


        }
        #endregion
    }
}
