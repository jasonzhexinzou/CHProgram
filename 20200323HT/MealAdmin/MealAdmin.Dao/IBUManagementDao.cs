using MealAdmin.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Dao
{
    public interface IBUManagementDao
    {
        List<P_BUINFO> LoadBUInfo();

        List<P_TAINFOView> LoadTAInfo();

        P_BUINFO GetBUInfoByID(Guid ID);

        int DelBUInfoByID(Guid ID);

        int AddBUInfo(string BUName, string BUHead, string BUHeadMudid);

        int UpdateBUInfo(Guid ID, string BUName, string BUHead, string BUHeadMudid);

        P_TAINFO GetTAInfoByID(Guid ID);

        int DelTAInfoByID(Guid ID);

        int AddTAInfo();

        int UpdateTAInfo(Guid ID, string TerritoryTA, string TerritoryHead, string TerritoryHeadName, string BUID);

        P_BUINFO GetBUInfoByUserId(string UserId);

        P_TAINFO GetTAInfoByUserId(string UserId);

        List<P_TAINFO> GetTAInfoByBUID(Guid BUID);
    }
}
