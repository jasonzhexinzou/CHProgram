using MealAdmin.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Service
{
    public interface IMarketService
    {
        List<P_MARKET> Load();
        List<P_TACode> LoadTAByUserId(string UserId);
        List<P_TACode> LoadTACodeByMarketAndUser(string Market, string UserId);
        P_TACode LoadRDCode(string Market, string UserId, string TCode);
        List<P_MARKET> LoadMarketByUserId(string UserId);
        List<P_TA> LoadTAByMarket(string marketName);
        List<P_TA> LoadTAByMarket(string marketName, string UserID);

        List<P_TA> LoadTAByMarketUserId(string marketName, string UserID, string TerritoryCode);
        List<D_COSTCENTERSELECT> LoadCostCenterByTA(string market, string ta);
        List<D_COSTCENTERSELECT> LoadCostCenterByMarketUserID(string market, string UserID, string TerritoryCode);
        P_MARKET FindByMarket(string marketName);
    }
}
