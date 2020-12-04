using MealAdmin.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Service
{
    public interface ICostCenterManagerService
    {
        List<M_COSTCENTER_MANAGER> Load(Guid CostID);
        int DelByCostID(Guid CostID);
        int Del(Guid ID);
        int Add(M_COSTCENTER_MANAGER entity);
        int Add(List<M_COSTCENTER_MANAGER> entitys);
    }
}
