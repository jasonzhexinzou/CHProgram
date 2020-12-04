using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MealAdmin.Entity;
using XFramework.XInject.Attributes;
using MealAdmin.Dao;

namespace MealAdmin.Service
{
    /// <summary>
    /// 成本中心
    /// </summary>
    public class CostCenterService : ICostCenterService
    {
        [Bean("costCenterDao")]
        public ICostCenterDao costCenterDao { get; set; }

        #region 新增成本中心
        /// <summary>
        /// 新增成本中心
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Add(M_COSTCENTER entity)
        {
            return costCenterDao.Add(entity);
        }
        #endregion

        #region 逻辑删除成本中心
        /// <summary>
        /// 逻辑删除成本中心
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Del(Guid id)
        {
            return costCenterDao.Del(id);
        }
        #endregion

        #region 用主键查找
        /// <summary>
        /// 用主键查找
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public M_COSTCENTER Find(Guid id)
        {
            return costCenterDao.Find(id);
        }
        #endregion

        #region 分页查询成本中心
        /// <summary>
        /// 分页查询成本中心
        /// </summary>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<M_COSTCENTER> LoadPage(string code, string name, int rows, int page, out int total)
        {
            code = string.IsNullOrEmpty(code) ? "%" : $"%{code}%";
            name = string.IsNullOrEmpty(name) ? "%" : $"%{name}%";

            return costCenterDao.LoadPage(code, name, rows, page, out total);
        }
        #endregion

        #region 保存修改
        /// <summary>
        /// 保存修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Save(M_COSTCENTER entity)
        {
            if (entity.ID == Guid.Empty)
            {
                entity.CreateDate = entity.ModifyDate;
                entity.Creator = entity.Modifier;
                return Add(entity);
            }
            else
            {
                return Upd(entity);
            }
        }
        #endregion

        #region 更新成本中心
        /// <summary>
        /// 更新成本中心
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Upd(M_COSTCENTER entity)
        {
            return costCenterDao.Upd(entity);
        }
        #endregion
        
    }
}
