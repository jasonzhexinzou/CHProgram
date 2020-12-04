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
    public class CostCenterManagerService : ICostCenterManagerService
    {
        [Bean("costCenterManagerDao")]
        public ICostCenterManagerDao costCenterManagerDao { get; set; }

        #region 新增一个审批人
        /// <summary>
        /// 新增一个审批人
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public int Add(List<M_COSTCENTER_MANAGER> entitys)
        {
            return costCenterManagerDao.Add(entitys);
        }
        #endregion

        #region 新增一群审批人
        /// <summary>
        /// 新增一群审批人
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Add(M_COSTCENTER_MANAGER entity)
        {
            return costCenterManagerDao.Add(entity);
        }
        #endregion

        #region 根据主键删除
        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public int Del(Guid ID)
        {
            return costCenterManagerDao.Del(ID);
        }
        #endregion

        #region 根据成本中心删除
        /// <summary>
        /// 根据成本中心删除
        /// </summary>
        /// <param name="CostID"></param>
        /// <returns></returns>
        public int DelByCostID(Guid CostID)
        {
            return costCenterManagerDao.DelByCostID(CostID);
        }
        #endregion

        #region 载入成本中心的审批人
        /// <summary>
        /// 载入成本中心的审批人
        /// </summary>
        /// <param name="CostID"></param>
        /// <returns></returns>
        public List<M_COSTCENTER_MANAGER> Load(Guid CostID)
        {
            return costCenterManagerDao.Load(CostID);
        }
        #endregion
    }
}
