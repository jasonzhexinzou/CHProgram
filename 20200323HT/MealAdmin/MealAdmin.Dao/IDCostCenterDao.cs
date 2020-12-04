using MealAdmin.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Dao
{
    /// <summary>
    /// 成本中心 接口
    /// </summary>
    public interface IDCostCenterDao
    {
        /// <summary>
        /// 新增成本中心
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int AddCostCenter(D_COSTCENTER entity);

        /// <summary>
        /// 查找成本中心
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        D_COSTCENTER FindCostCenter(Guid id);

        /// <summary>
        /// 根据TA查找成本中心
        /// </summary>
        /// <param name="ta"></param>
        /// <returns></returns>
        List<D_COSTCENTER> FindCostCenterByTA(string ta);

        /// <summary>
        /// 获取全部成本中心
        /// </summary>
        /// <returns></returns>
        List<D_COSTCENTER> GetAllCostCenter();
    }
}
