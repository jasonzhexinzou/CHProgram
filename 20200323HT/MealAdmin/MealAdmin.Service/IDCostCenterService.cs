using MealAdmin.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Service
{
    public interface IDCostCenterService
    {
        List<D_COSTCENTER> LoadCostCenterByTA(string ta);
        List<D_COSTCENTER> GetAllCostCenter();
    }
}
