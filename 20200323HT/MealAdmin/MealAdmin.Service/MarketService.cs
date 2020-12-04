using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MealAdmin.Entity;
using MealAdmin.Dao;
using XFramework.XInject.Attributes;

namespace MealAdmin.Service
{
    public class MarketService : IMarketService
    {
        [Bean("marketDao")]
        public IMarketDao marketDao { get; set; }
        [Bean("userInfoDao")]
        public IUserInfoDao userInfoDao { get; set; }

        #region 载入全部发票抬头信息
        /// <summary>
        /// 载入全部发票抬头信息
        /// </summary>
        /// <returns></returns>
        public List<P_MARKET> Load()
        {
            return marketDao.Load();
        }
        #endregion

        #region 载入TACODE
        /// <summary>
        /// 载入TACODE
        /// </summary>
        /// <returns></returns>
        public List<P_TACode> LoadTAByUserId(string UserId)
        {
            return marketDao.LoadTAByUserId(UserId);
        }

        public List<P_TACode> LoadTACodeByMarketAndUser(string Market, string UserId)
        {
            return marketDao.LoadTACodeByMarketAndUser(Market, UserId);
        }

        public P_TACode LoadRDCode(string Market, string UserId, string TCode)
        {
            return marketDao.LoadRDCode(Market, UserId, TCode);
        }

        /// <summary>
        /// 载入TACODE
        /// </summary>
        /// <returns></returns>
        public List<P_MARKET> LoadMarketByUserId(string UserId)
        {
            return marketDao.LoadMarketByUserId(UserId);
        }
        #endregion

        #region 根据MarketName查询TA
        /// <summary>
        /// 根据MarketName查询TA
        /// </summary>
        /// <param name="marketName"></param>
        /// <returns></returns>
        public List<P_TA> LoadTAByMarket(string market, string UserID)
        {
            //var res = userInfoDao.Edit(userId,phoneNumber,market);
            return marketDao.LoadTAByMarketUser(market, UserID);
        }

        /// <summary>
        /// 根据MarketName查询TA
        /// </summary>
        /// <param name="marketName"></param>
        /// <returns></returns>
        public List<P_TA> LoadTAByMarketUserId(string market, string UserID, string TerritoryCode)
        {
            //var res = userInfoDao.Edit(userId,phoneNumber,market);
            return marketDao.LoadTAByMarketUserId(market, UserID, TerritoryCode);
        }
        #endregion


        #region 根据MarketName查询TA
        /// <summary>
        /// 根据MarketName查询TA
        /// </summary>
        /// <param name="marketName"></param>
        /// <returns></returns>
        public List<P_TA> LoadTAByMarket(string market)
        {
            //var res = userInfoDao.Edit(userId,phoneNumber,market);
            return marketDao.LoadTAByMarketName(market);
        }
        #endregion

        #region 根据TA查询成本中心
        /// <summary>
        /// 根据TA查询成本中心
        /// </summary>
        /// <param name="ta">TA</param>
        /// <returns></returns>
        public List<D_COSTCENTERSELECT> LoadCostCenterByTA(string market, string ta)
        {
            return marketDao.FindCostCenterByTA(market, ta);
        }
        #endregion

        #region 根据TA查询成本中心
        /// <summary>
        /// 根据TA查询成本中心
        /// </summary>
        /// <param name="ta">TA</param>
        /// <returns></returns>
        public List<D_COSTCENTERSELECT> LoadCostCenterByMarketUserID(string market, string UserID, string TerritoryCode)
        {
            return marketDao.LoadCostCenterByMarketUserID(market, UserID, TerritoryCode);
        }
        #endregion

        #region 根据名字查找market信息
        /// <summary>
        /// 根据名字查找market信息
        /// </summary>
        /// <param name="marketName"></param>
        /// <returns></returns>
        public P_MARKET FindByMarket(string marketName)
        {
            return marketDao.FindByMarket(marketName);
        }
        #endregion
    }
}
