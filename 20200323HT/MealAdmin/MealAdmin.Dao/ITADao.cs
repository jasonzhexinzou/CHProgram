using System;
using MealAdmin.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Dao
{
    public interface ITADao
    {
        List<P_TA> Load();

        List<D_COSTCENTER> LoadCostCenterByTA(string marketName);
    }
}
