using MealAdmin.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Service
{
    public interface IBizConfigService
    {
        P_MARKET_INVOICE_OBJ GetAllMarkets();

        P_BIZ_CONF_OBJ GetConfig();

        int UpdateMarketsInvoice(P_MARKET_INVOICE_OBJ entity, out List<P_MARKET> unSuccessData);

        int UpdateConfig(P_BIZ_CONF_OBJ entity, out List<P_BIZ_CONF> unSuccessData);
    }
}
