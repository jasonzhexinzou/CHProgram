using MealAdmin.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Dao
{
    /// <summary>
    /// 成本中心
    /// </summary>
    public interface ICostCenterDao
    {
        List<M_COSTCENTER> LoadPage(string code, string name, int rows, int page, out int total);
        int Add(M_COSTCENTER entity);
        int Upd(M_COSTCENTER entity);
        int Del(Guid id);
        M_COSTCENTER Find(Guid id);
        
    }
}
