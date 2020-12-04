using MealAdmin.Dao;
using MealAdmin.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XFramework.XInject.Attributes;

namespace MealAdmin.Service
{
    public class BUManagementService : IBUManagementService
    {
        [Bean("BUManagementDao")]
        public IBUManagementDao buManagementDao { get; set; }

        public List<P_BUINFO> LoadBUInfo()
        {
            return buManagementDao.LoadBUInfo();
        }

        public List<P_TAINFOView> LoadTAInfo()
        {
            buManagementDao.AddTAInfo();

            return buManagementDao.LoadTAInfo();
        }

        public P_BUINFO GetBUInfoByID(Guid ID)
        {
            return buManagementDao.GetBUInfoByID(ID);
        }

        public int DelBUInfoByID(Guid ID)
        {
            return buManagementDao.DelBUInfoByID(ID);
        }

        public int AddBUInfo(string BUName, string BUHead, string BUHeadMudid)
        {
            return buManagementDao.AddBUInfo(BUName, BUHead, BUHeadMudid);
        }

        public int UpdateBUInfo(Guid ID, string BUName, string BUHead, string BUHeadMudid)
        {
            return buManagementDao.UpdateBUInfo(ID, BUName, BUHead, BUHeadMudid);
        }

        public P_TAINFO GetTAInfoByID(Guid ID)
        {
            return buManagementDao.GetTAInfoByID(ID);
        }

        public int DelTAInfoByID(Guid ID)
        {
            return buManagementDao.DelTAInfoByID(ID);
        }

        public int UpdateTAInfo(Guid ID, string TerritoryTA, string TerritoryHead, string TerritoryHeadName, string BUID)
        {
            return buManagementDao.UpdateTAInfo(ID, TerritoryTA, TerritoryHead, TerritoryHeadName, BUID);
        }

        public P_BUINFO GetBUInfoByUserId(string UserId)
        {
            return buManagementDao.GetBUInfoByUserId(UserId);
        }

        public P_TAINFO GetTAInfoByUserId(string UserId)
        {
            return buManagementDao.GetTAInfoByUserId(UserId);
        }

        public List<P_TAINFO> GetTAInfoByBUID(Guid BUID)
        {
            return buManagementDao.GetTAInfoByBUID(BUID);
        }
    }
}
