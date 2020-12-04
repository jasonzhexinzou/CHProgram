using MealAdmin.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Dao
{
    public interface IBizConfigDao
    {
        List<P_MARKET> GetAllMarkets();

        List<P_BIZ_CONF> GetAllConfig();

        int UpdateMarkets(List<P_MARKET> entity, out List<P_MARKET> unSuccessData);

        int UpdateConfig(List<P_BIZ_CONF> entity, out List<P_BIZ_CONF> unSuccessData);
    }
}
