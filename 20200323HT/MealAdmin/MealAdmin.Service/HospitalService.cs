using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MealAdmin.Entity;
using XFramework.XInject.Attributes;
using MealAdmin.Dao;
using MealAdmin.Entity.View;

namespace MealAdmin.Service
{
    public class HospitalService : IHospitalService
    {
        [Bean("hospitalDao")]
        public IHospitalDao hospitalDao { get; set; }

        [Bean("baseDataDao")]
        public IBaseDataDao baseDataDao { get; set; }
        //public List<P_HOSPITAL_MNT_VIEW> Load(string srh_GskHospital, string srh_HospitalName, string srh_HospitalMarket, string srh_OHHospitalType)
        //{
        //    return hospitalDao.Load(srh_GskHospital, srh_HospitalName, srh_HospitalMarket, srh_OHHospitalType);
        //}

        public List<P_HOSPITAL_DATA_VIEW> Load(string srh_GskHospital, string srh_HospitalName, string srh_HospitalMarket, string srh_OHHospitalType)
        {
            return hospitalDao.Load(srh_GskHospital, srh_HospitalName, srh_HospitalMarket, srh_OHHospitalType);
        }

        public List<P_TERRITORY> LoadTA(string srh_GskHospital, string srh_HospitalName, string srh_MUDID, string srh_TerritoryCode, string srh_HospitalMarket, string srh_HospitalTA)
        {
            return hospitalDao.LoadTA(srh_GskHospital, srh_HospitalName, srh_MUDID, srh_TerritoryCode, srh_HospitalMarket, srh_HospitalTA);
        }

        //public List<P_HOSPITAL_MNT_VIEW> LoadPage(string srh_GskHospital, string srh_HospitalName, string srh_HospitalMarket, string srh_OHHospitalType, int rows, int page, out int total)
        //{
        //    return hospitalDao.LoadPage(srh_GskHospital, srh_HospitalName, srh_HospitalMarket, srh_OHHospitalType, rows, page, out total);
        //}

        public List<P_HOSPITAL_DATA_VIEW> LoadPage(string srh_GskHospital, string srh_HospitalName, string srh_HospitalMarket, string srh_OHHospitalType, int rows, int page, out int total)
        {
            return hospitalDao.LoadPage(srh_GskHospital, srh_HospitalName, srh_HospitalMarket, srh_OHHospitalType, rows, page, out total);
        }

        public List<P_TERRITORY> LoadTAPage(string srh_GskHospital, string srh_HospitalName, string srh_MUDID, string srh_TerritoryCode, string srh_HospitalMarket, string srh_HospitalTA, int rows, int page, out int total)
        {
            return hospitalDao.LoadTAPage(srh_GskHospital, srh_HospitalName, srh_MUDID, srh_TerritoryCode, srh_HospitalMarket, srh_HospitalTA, rows, page, out total);
        }

        public List<P_HOSPITAL_DATA_VIEW> LoadHData(string srh_GskHospital, string srh_HospitalName, string srh_HospitalMarket, string srh_OHHospitalType)
        {
            return hospitalDao.LoadHData(srh_GskHospital, srh_HospitalName, srh_HospitalMarket, srh_OHHospitalType);
        }

        #region 从Territory_Hospital根据Market查询[TERRITORY_TA]
        public List<Territory_Hospital> LoadTERRITORY_TAByMarket(string Market)
        {
            return hospitalDao.LoadTERRITORY_TAByMarket(Market);
        }
        #endregion
        public P_HOSPITAL GetHospitalByID(int HospitalID)
        {
            return hospitalDao.GetHospitalByID(HospitalID);
        }

        public List<P_HOSPITAL_ADDR> LoadHospitalAddr(int HospitalID)
        {
            return hospitalDao.LoadHospitalAddr(HospitalID);
        }

        public bool DeleteHospitalAddr(Guid AddrID)
        {
            return hospitalDao.DeleteHospitalAddr(AddrID);
        }

        public bool AddHospitalAddr(P_HOSPITAL_ADDR Data)
        {
            return hospitalDao.AddHospitalAddr(Data);
        }

        public List<P_HOSPITALINFO> LoadHospital(string channel)
        {
            var hosList = hospitalDao.LoadHospital(channel);
            return hosList;
        }

        //#region 导入医院
        ///// <summary>
        ///// 导入医院
        ///// </summary>
        ///// <param name="excelRows"></param>
        ///// <param name="fails"></param>
        ///// <returns></returns>
        //public int Import(List<EXCEL_HOSPITAL> excelRows, ref List<EXCEL_HOSPITAL> fails)
        //{
        //    // 本次导入涉及的省市
        //    var p_c = excelRows.Select(a => new { p = a.Province, c = a.City,t=a.Market }).Distinct().OrderBy(a => a.p).ToList();
        //    var listProvince = p_c.GroupBy(a => new { a.p,a.t}).Select(a => new { id = 0, name = a.Key.p,type=a.Key.t, citys = a.Select(b => new { id = 0, name = b.c,type=b.t }).ToList() }).ToList();
        //    var marketList = excelRows.Select(p => new { market = p.Market }).ToList().Distinct();
        //    string marketString = "(";
        //    foreach (var item in marketList)
        //    {
        //        marketString += "'"+item.market + "',";
        //    }
        //    marketString = marketString.Substring(0, marketString.Length - 1) + ")";
        //    // 数据库中现存的省市
        //    //var delProvince = hospitalDao.ClearProvinceByMarket(excelRows[0].Market);
        //    //var delCity= hospitalDao.ClearCityByMarket(excelRows[0].Market);
        //    var listDbProvince = baseDataDao.LoadProvinceByMarketList(marketString);
        //    var listDbCity = baseDataDao.LoadCityByMarketList(marketString);
        //    var _listDbProvince = listDbProvince.Select(a =>
        //    {
        //        return new
        //        {
        //            id = a.ID,
        //            name = a.Name,
        //            citys = listDbCity.Where(b => b.ProvinceId == a.ID ).Select(c => new { id = c.ID, name = c.Name,type=c.Type }).ToList(),
        //            type=a.Type
        //        };
        //    }).ToList();

        //    // 需要补录的省、市
        //    var listNonProvince
        //        = listProvince.Select(a =>new { a.name ,a.type}  ).ToList()
        //        .Except(_listDbProvince.Select(a =>new { a.name ,a.type} ).ToList())
        //        .Select(a => new NonPC
        //        {
        //            NonProvince = 1,
        //            name = a.name,
        //            citys = listProvince.Find(b => b.name == a.name).citys.Select(c =>new City { name=c.name,type=c.type} ).ToList(),
        //            type=a.type
        //        }).ToList();

        //    foreach (var p in listProvince)
        //    {
        //        if (_listDbProvince.Exists(a => a.name == p.name && a.type==p.type))
        //        {
        //            var _p = _listDbProvince.Find(a => a.name == p.name && a.type==p.type);
        //            var listNonCity = p.citys.Select(a => new { a.name, a.type }).ToList().Except(_p.citys.Select(a => new { a.name, a.type }).ToList()).ToList();
        //            if (listNonCity.Count > 0)
        //            {
        //                listNonProvince.Add(new NonPC
        //                {
        //                    name = p.name,
        //                    citys = listNonCity.Select(c => new City { name = c.name, type = c.type }).ToList(),
        //                    type=p.type
        //                });
        //            }
        //        }
        //    }

        //    if (listNonProvince.Count > 0)
        //    {
        //        // 补充省
        //        baseDataDao.AddProvince(listNonProvince.Where(a => a.NonProvince == 1).Select(a => new P_PROVINCE()
        //        {
        //            Name = a.name,
        //            PinYin = GetPinYin(a.name),
        //            Type=a.type
        //        }).ToList());
        //        listDbProvince = baseDataDao.LoadProvinceByMarketList(marketString);
        //        foreach (var item in listNonProvince)
        //        {
        //            var _item = listDbProvince.Find(a => a.Name == item.name && a.Type==item.type);
        //            if (_item != null)
        //            {
        //                // 补充市
        //                baseDataDao.AddCity(item.citys.Select(a => new P_CITY()
        //                {
        //                    ProvinceId = _item.ID,
        //                    Name = a.name,
        //                    PinYin = GetPinYin(a.name),
        //                    Type = a.type
        //                }).ToList());
        //            }
        //        }
        //        listDbCity = baseDataDao.LoadCityByMarketList(marketString);
        //    }

        //    _listDbProvince = listDbProvince.Select(a =>
        //    {
        //        return new
        //        {
        //            id = a.ID,
        //            name = a.Name,
        //            citys = listDbCity.Where(b => b.ProvinceId == a.ID).Select(c => new { id = c.ID, name = c.Name,type=c.Type }).ToList(),
        //            type=a.Type
        //        };
        //    }).ToList();

        //    // 导入医院数据
        //    var CreateDate = DateTime.Now;
        //    var listHospital = excelRows.Select(a =>
        //    {
        //        var p = _listDbProvince.Find(b => b.name == a.Province && b.type==a.Market);
        //        var c = p.citys.Find(b => b.name == a.City && b.type==a.Market);
        //        return new P_HOSPITAL()
        //        {
        //            CityId = c.id,
        //            GskHospital = a.HospitalCode,
        //            OldGskHospital=a.OldHospitalCode,
        //            Name = a.HospitalName,
        //            OldName=a.OldHospitelName,
        //            FirstLetters = NPinyin.Pinyin.GetInitials(a.HospitalName).ToLower(),
        //            //Address = a.HospitalAddress,
        //            Address = "院外",
        //            Latitude = "0",
        //            Longitude = "0",
        //            Type = a.Market,
        //            External = a.External,
        //            CreateDate = CreateDate,
        //            ProvinceId = p.id,
        //            IsXMS = a.XMS,
        //            IsBDS = a.BDS,
        //            IsMT = a.meituan,
        //            Remark = a.Remark,
        //            IsDelete = 0,
        //            //MainAddress=a.MainAddress,
        //            MainAddress = "主地址",
        //            HospitalCode =a.Code
        //        };
        //    }).ToList();
        //    ////校验导入数据
        //    //foreach(var s in listHospital)
        //    //{
        //    //    string Failtxt="";
        //    //    //校验医院代码
        //    //    string GskHospitalOHCode = s.GskHospital.Substring(0, s.GskHospital.Length - 3);
        //    //    //将截取医院代码后三位之后得到的Code与目标医院Code比对
        //    //    var OHcode = hospitalDao.GetDataByGskHospitalOH(GskHospitalOHCode);
        //    //    //若无匹配的目标医院Code
        //    //    if (OHcode == null)
        //    //    {
        //    //        Failtxt += s.GskHospital + " " + s.Name + " 是非目标医院 导入失败" ;
        //    //    }
        //    //    //校验医院名称
        //    //    string GskOHName = s.Name.Substring(3);
        //    //    //将截取医院名称前三位之后得到的名称与目标医院Name比对
        //    //    var OHName = hospitalDao.GetDataByGskOHName(GskOHName, GskHospitalOHCode);
        //    //    //若无匹配的目标医院Name
        //    //    if (OHName == null)
        //    //    {
        //    //        Failtxt += s.GskHospital + " " + s.Name + " 医院名称不一致 导入失败" ;
        //    //    }
        //    //}
        //    var fials = new List<P_HOSPITAL>();
        //    hospitalDao.Import(listHospital, marketString, ref fials);
        //    if (fials.Count > 0)
        //    {
        //        var failCodes = fials.Select(a => a.GskHospital).ToList();
        //        fails.AddRange(excelRows.Where(a => failCodes.Contains(a.HospitalCode)).ToList());
        //        return 0;
        //    }
        //    else
        //    {
        //        return 1;
        //    }
        //}
        //#endregion

        #region 导入医院
        /// <summary>
        /// 导入医院
        /// </summary>
        /// <param name="sus"></param>
        /// <param name="fails"></param>
        /// <returns></returns>
        public int Import(List<EXCEL_HOSPITAL> sus, ref List<EXCEL_HOSPITAL> fails)
        {
            // 本次导入涉及的省市
            var marketList = sus.Select(p => new { market = p.Market }).ToList().Distinct();
            string marketString = "(";
            foreach (var item in marketList)
            {
                marketString += "'" + item.market + "',";
            }
            marketString = marketString.Substring(0, marketString.Length - 1) + ")";

            // 导入医院数据
            var CreateDate = DateTime.Now;
            var listHospital = sus.Select(a =>
            {
                return new P_HOSPITAL()
                {
                    CityId = Convert.ToInt32(a.City),
                    GskHospital = a.HospitalCode,
                    Name = a.HospitalName,
                    FirstLetters = NPinyin.Pinyin.GetInitials(a.HospitalName).ToLower(),
                    Address = a.HospitalAddress,
                    Latitude = "0",
                    Longitude = "0",
                    Type = a.Market,
                    External = a.External,
                    CreateDate = CreateDate,
                    ProvinceId = Convert.ToInt32(a.Province),
                    IsXMS = a.XMS,
                    IsBDS = a.BDS,
                    IsMT = a.meituan,
                    IsDelete = 0,
                    MainAddress = a.MainAddress,
                    HospitalCode = a.HospitalCode
                };
            }).ToList();
            var fials = new List<P_HOSPITAL>();
            hospitalDao.Import(listHospital, marketString, ref fials);
            if (fials.Count > 0)
            {
                var failCodes = fials.Select(a => a.GskHospital).ToList();
                fails.AddRange(sus.Where(a => failCodes.Contains(a.HospitalCode)).ToList());
                return 0;
            }
            else
            {
                return 1;
            }
        }
        #endregion

        private string GetPinYin(string txt)
        {
            var py = $";{NPinyin.Pinyin.GetPinyin(txt).Replace(" ", "")};{NPinyin.Pinyin.GetInitials(txt).ToLower()}";
            return py;
        }

        #region 删除医院
        /// <summary>
        /// 删除医院
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public int Del(string[] ids)
        {
            return hospitalDao.Del(ids);
        }
        #endregion

        #region 保存修改
        /// <summary>
        /// 保存修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int SaveChange(EXCEL_HOSPITAL entity)
        {
            P_HOSPITAL _entity = new P_HOSPITAL();
            _entity.ID = entity.ID;
            _entity.CityId = Convert.ToInt32(entity.City);
            _entity.GskHospital = entity.HospitalCode;
            _entity.Name = entity.HospitalName;
            _entity.FirstLetters = NPinyin.Pinyin.GetInitials(entity.HospitalName).ToLower();
            _entity.Address = entity.HospitalAddress;
            _entity.Type = entity.Market;
            _entity.External = entity.External;
            _entity.ProvinceId = Convert.ToInt32(entity.Province);
            _entity.IsXMS = entity.XMS;
            _entity.IsBDS = entity.BDS;
            _entity.IsMT = entity.meituan;
            _entity.IsDelete = 0;
            _entity.HospitalCode = entity.HospitalCode;
            _entity.MainAddress = entity.MainAddress;

            if (_entity.ID > 0)
            {
                return hospitalDao.Change(_entity);
            }
            else
            {
                if (_entity.MainAddress != "主地址")
                {
                    var number = _entity.MainAddress.Substring(2);
                    _entity.HospitalCode = entity.HospitalCode + "_" + number;
                }
                else
                {
                    _entity.HospitalCode = entity.HospitalCode;
                }
                var hospital = hospitalDao.GetHospitalByGSKHospital(_entity.HospitalCode);
                if (hospital != null)
                {
                    return 2;
                }
                else
                {
                    _entity.Latitude = "0";
                    _entity.Longitude = "0";
                    _entity.CreateDate = DateTime.Now;
                    return hospitalDao.Add(_entity);
                }
            }

        }
        #endregion

        #region 批量删除医院
        /// <summary>
        /// 批量删除医院
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public int DelHospitals(List<string> ids, out List<string> unSuccesUserId)
        {
            return hospitalDao.DelHospitals(ids, out unSuccesUserId);
        }
        #endregion

        public void DeleteProvince()
        {
            hospitalDao.deleteCity();
            hospitalDao.deleteProvince();
        }

        #region 根据医院Code查询医院
        /// <summary>
        /// 根据医院Code查询医院
        /// </summary>
        /// <param name="hospitalCode"></param>
        /// <returns></returns>
        public P_HOSPITAL FindByCode(string hospitalCode)
        {
            return hospitalDao.FindByCode(hospitalCode);
        }
        #endregion


        #region 查询医院覆盖变化情况
        /// <summary>
        /// 查询医院覆盖变化情况
        /// </summary>
        /// <param name="Type">类型</param>
        /// <returns></returns>
        public List<P_HospitalCoverChange> FindHospitalCoverChange(string Type)
        {
            return hospitalDao.FindHospitalCoverChange(Type);
        }
        #endregion

        #region 查询医院对应代表及直线经理
        /// <summary>
        /// 查询医院对应代表及直线经理
        /// </summary>
        /// <param name="HospitalCode">类型</param>
        /// <returns></returns>
        public List<V_TerritoryHospitalMRDM> FindHospitalUser(string HospitalCode)
        {
            return hospitalDao.FindHospitalUser(HospitalCode);
        }
        #endregion

        #region 修改发送状态
        /// <summary>
        /// 修改发送状态
        /// </summary>
        /// <returns></returns>
        public void UpdateMessageState()
        {
            hospitalDao.UpdateMessageState();
        }
        #endregion

        #region 根据医院代码查询目标医院
        /// <summary>
        /// 根据医院代码查询目标医院 20190416
        /// </summary>
        /// <param name="GskHospitalOHCode"></param>
        /// <returns></returns>
        public List<P_HOSPITAL> GetDataByGskHospitalOH(string GskHospitalOHCode)
        {
            return hospitalDao.GetDataByGskHospitalOH(GskHospitalOHCode);
        }
        #endregion

        #region 根据医院代码名称查询目标医院
        /// <summary>
        /// 根据医院代码名称查询目标医院 20190416
        /// </summary>
        /// <param name="GskOHName"></param>
        /// <param name="GskHospitalOHCode"></param>
        /// <returns></returns>
        public P_HOSPITAL GetDataByGskOHName(string GskOHName, string GskHospitalOHCode)
        {
            return hospitalDao.GetDataByGskOHName(GskOHName, GskHospitalOHCode);
        }
        #endregion

        #region 根据医院代码名称查询目标医院省市Market
        public List<P_HOSPITAL_MNT_VIEW> GetDataByProvinceCityMarket(string GskOHName, string GskHospitalOHCode)
        {
            return hospitalDao.GetDataByProvinceCityMarket(GskOHName, GskHospitalOHCode);
        }
        #endregion

        #region 获取Rx临时表数据
        /// <summary>
        /// 获取Rx临时表数据
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public List<P_HOSPITAL_RxTemp> LoadRxTemp()
        {
            return hospitalDao.LoadRxTemp();
        }
        #endregion

        #region 根据Market删除主数据表中主地址数据
        public void DeleteMainAddressData(string Market)
        {
            hospitalDao.DeleteDetailData(Market);
            hospitalDao.DeleteMainAddressData(Market);
        }
        #endregion

        #region 更新门地址IsDelete=1
        /// <summary>
        /// 更新门地址IsDelete=1
        /// </summary>
        /// <param name="Market"></param>
        /// <returns></returns>
        public int UpdateIsDelete(string Market)
        {
            return hospitalDao.UpdateIsDelete(Market);
        }
        #endregion

        #region 更新院外IsDelete=1
        /// <summary>
        /// 更新院外IsDelete=1
        /// </summary>
        /// <param name="Market"></param>
        /// <returns></returns>
        public int UpdateOHIsDelete(string Market)
        {
            return hospitalDao.UpdateOHIsDelete(Market);
        }
        #endregion

        #region 插入Rx医院
        /// <summary>
        /// 插入医院
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        public int InsertRxData(List<P_HOSPITAL_RxTemp> res)
        {
            // 本次插入涉及的省市
            var p_c = res.Select(a => new { p = a.Province, c = a.City, t = "Rx" }).Distinct().OrderBy(a => a.p).ToList();
            var listProvince = p_c.GroupBy(a => new { a.p, a.t }).Select(a => new { id = 0, name = a.Key.p, type = a.Key.t, citys = a.Select(b => new { id = 0, name = b.c, type = b.t }).ToList() }).ToList();
            string marketString = "('Rx')";
            // 数据库中现存的省市
            var listDbProvince = baseDataDao.LoadProvinceByMarketList(marketString);
            var listDbCity = baseDataDao.LoadCityByMarketList(marketString);
            var _listDbProvince = listDbProvince.Select(a =>
            {
                return new
                {
                    id = a.ID,
                    name = a.Name,
                    citys = listDbCity.Where(b => b.ProvinceId == a.ID).Select(c => new { id = c.ID, name = c.Name, type = c.Type }).ToList(),
                    type = a.Type
                };
            }).ToList();

            // 需要补录的省、市
            var listNonProvince
                = listProvince.Select(a => new { a.name, a.type }).ToList()
                .Except(_listDbProvince.Select(a => new { a.name, a.type }).ToList())
                .Select(a => new NonPC
                {
                    NonProvince = 1,
                    name = a.name,
                    citys = listProvince.Find(b => b.name == a.name).citys.Select(c => new City { name = c.name, type = c.type }).ToList(),
                    type = a.type
                }).ToList();

            foreach (var p in listProvince)
            {
                if (_listDbProvince.Exists(a => a.name == p.name && a.type == p.type))
                {
                    var _p = _listDbProvince.Find(a => a.name == p.name && a.type == p.type);
                    var listNonCity = p.citys.Select(a => new { a.name, a.type }).ToList().Except(_p.citys.Select(a => new { a.name, a.type }).ToList()).ToList();
                    if (listNonCity.Count > 0)
                    {
                        listNonProvince.Add(new NonPC
                        {
                            name = p.name,
                            citys = listNonCity.Select(c => new City { name = c.name, type = c.type }).ToList(),
                            type = p.type
                        });
                    }
                }
            }

            if (listNonProvince.Count > 0)
            {
                // 补充省
                baseDataDao.AddProvince(listNonProvince.Where(a => a.NonProvince == 1).Select(a => new P_PROVINCE()
                {
                    Name = a.name,
                    PinYin = GetPinYin(a.name),
                    Type = a.type
                }).ToList());
                listDbProvince = baseDataDao.LoadProvinceByMarketList(marketString);
                foreach (var item in listNonProvince)
                {
                    var _item = listDbProvince.Find(a => a.Name == item.name && a.Type == item.type);
                    if (_item != null)
                    {
                        // 补充市
                        baseDataDao.AddCity(item.citys.Select(a => new P_CITY()
                        {
                            ProvinceId = _item.ID,
                            Name = a.name,
                            PinYin = GetPinYin(a.name),
                            Type = a.type
                        }).ToList());
                    }
                }
                listDbCity = baseDataDao.LoadCityByMarketList(marketString);
            }

            _listDbProvince = listDbProvince.Select(a =>
            {
                return new
                {
                    id = a.ID,
                    name = a.Name,
                    citys = listDbCity.Where(b => b.ProvinceId == a.ID).Select(c => new { id = c.ID, name = c.Name, type = c.Type }).ToList(),
                    type = a.Type
                };
            }).ToList();

            // 插入医院数据
            var CreateDate = DateTime.Now;
            var UpdateDate = DateTime.Now;
            var listHospital = res.Select(a =>
            {
                var p = _listDbProvince.Find(b => b.name == a.Province && b.type == "Rx");
                var c = p.citys.Find(b => b.name == a.City && b.type == "Rx");
                return new P_HOSPITAL()
                {
                    CityId = c.id,
                    GskHospital = a.GskHospital,
                    OldGskHospital = "",
                    Name = a.Name,
                    OldName = "",
                    FirstLetters = NPinyin.Pinyin.GetInitials(a.Name).ToLower(),
                    Address = a.Address.Trim(),
                    Latitude = a.Latitude.ToString().Trim(),
                    Longitude = a.Longitude.ToString().Trim(),
                    Type = "Rx",
                    External = 0,
                    CreateDate = CreateDate,
                    ProvinceId = p.id,
                    IsXMS = "是",
                    IsBDS = "是",
                    IsMT = "否",
                    Remark = "",
                    IsDelete = 0,
                    //RelateUserList = "",
                    MainAddress = "主地址",
                    HospitalCode = a.GskHospital
                };
            }).ToList();

            var listDetail = res.Select(a =>
            {
                return new P_HOSPITAL_DETAIL()
                {
                    GskHospital = a.GskHospital,
                    District = a.District,
                    DistrictCode = a.DistrictCode,
                    CustomerType = a.CustomerType,
                    RESP = a.RESP,
                    HEP = a.HEP,
                    CNS = a.CNS,
                    HIV = a.HIV,
                    VOL = a.VOL,
                    MA = a.MA,
                    Region = "",
                    IsDelete = 0,
                    CreateDate = CreateDate,
                    UpdateDate = UpdateDate,
                    IP = a.IP


                };
            }).ToList();
            hospitalDao.InsertHospitalData(listHospital);
            hospitalDao.InsertHospitalDetail(listDetail);

            return 1;
        }
        #endregion

        #region 更新门地址IsDelete=0
        /// <summary>
        /// 更新门地址IsDelete=0
        /// </summary>
        /// <param name="Market"></param>
        /// <returns></returns>
        public int UpdateRxStatus(string Market)
        {
            return hospitalDao.UpdateRxStatus(Market);
        }
        #endregion

        #region 根据Market查询主数据院外数据
        public List<P_HOSPITAL> GetOHData(string Market)
        {
            return hospitalDao.GetOHData(Market);
        }
        #endregion

        #region 根据Market院外Code查询主数据数据
        public List<P_HOSPITAL> GetHData(string GskHospitalCode, string Market)
        {
            return hospitalDao.GetHData(GskHospitalCode, Market);
        }
        #endregion

        #region 更新院外数据信息与目标医院一致
        public int UpdateOHData(List<P_HOSPITAL> sus)
        {
            return hospitalDao.UpdateOHData(sus);
        }
        #endregion

        #region 获取Vx临时表数据
        /// <summary>
        /// 获取Vx临时表数据
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public List<P_HOSPITAL_VxTemp> LoadVxTemp()
        {
            return hospitalDao.LoadVxTemp();
        }
        #endregion

        #region 插入Vx医院
        /// <summary>
        /// 插入医院
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        public int InsertVxData(List<P_HOSPITAL_VxTemp> res)
        {
            // 本次插入涉及的省市
            var p_c = res.Select(a => new { p = a.Province, c = a.City, t = "Vx" }).Distinct().OrderBy(a => a.p).ToList();
            var listProvince = p_c.GroupBy(a => new { a.p, a.t }).Select(a => new { id = 0, name = a.Key.p, type = a.Key.t, citys = a.Select(b => new { id = 0, name = b.c, type = b.t }).ToList() }).ToList();
            string marketString = "('Vx')";
            // 数据库中现存的省市
            var listDbProvince = baseDataDao.LoadProvinceByMarketList(marketString);
            var listDbCity = baseDataDao.LoadCityByMarketList(marketString);
            var _listDbProvince = listDbProvince.Select(a =>
            {
                return new
                {
                    id = a.ID,
                    name = a.Name,
                    citys = listDbCity.Where(b => b.ProvinceId == a.ID).Select(c => new { id = c.ID, name = c.Name, type = c.Type }).ToList(),
                    type = a.Type
                };
            }).ToList();

            // 需要补录的省、市
            var listNonProvince
                = listProvince.Select(a => new { a.name, a.type }).ToList()
                .Except(_listDbProvince.Select(a => new { a.name, a.type }).ToList())
                .Select(a => new NonPC
                {
                    NonProvince = 1,
                    name = a.name,
                    citys = listProvince.Find(b => b.name == a.name).citys.Select(c => new City { name = c.name, type = c.type }).ToList(),
                    type = a.type
                }).ToList();

            foreach (var p in listProvince)
            {
                if (_listDbProvince.Exists(a => a.name == p.name && a.type == p.type))
                {
                    var _p = _listDbProvince.Find(a => a.name == p.name && a.type == p.type);
                    var listNonCity = p.citys.Select(a => new { a.name, a.type }).ToList().Except(_p.citys.Select(a => new { a.name, a.type }).ToList()).ToList();
                    if (listNonCity.Count > 0)
                    {
                        listNonProvince.Add(new NonPC
                        {
                            name = p.name,
                            citys = listNonCity.Select(c => new City { name = c.name, type = c.type }).ToList(),
                            type = p.type
                        });
                    }
                }
            }

            if (listNonProvince.Count > 0)
            {
                // 补充省
                baseDataDao.AddProvince(listNonProvince.Where(a => a.NonProvince == 1).Select(a => new P_PROVINCE()
                {
                    Name = a.name,
                    PinYin = GetPinYin(a.name),
                    Type = a.type
                }).ToList());
                listDbProvince = baseDataDao.LoadProvinceByMarketList(marketString);
                foreach (var item in listNonProvince)
                {
                    var _item = listDbProvince.Find(a => a.Name == item.name && a.Type == item.type);
                    if (_item != null)
                    {
                        // 补充市
                        baseDataDao.AddCity(item.citys.Select(a => new P_CITY()
                        {
                            ProvinceId = _item.ID,
                            Name = a.name,
                            PinYin = GetPinYin(a.name),
                            Type = a.type
                        }).ToList());
                    }
                }
                listDbCity = baseDataDao.LoadCityByMarketList(marketString);
            }

            _listDbProvince = listDbProvince.Select(a =>
            {
                return new
                {
                    id = a.ID,
                    name = a.Name,
                    citys = listDbCity.Where(b => b.ProvinceId == a.ID).Select(c => new { id = c.ID, name = c.Name, type = c.Type }).ToList(),
                    type = a.Type
                };
            }).ToList();

            // 插入医院数据
            var CreateDate = DateTime.Now;
            var UpdateDate = DateTime.Now;
            var listHospital = res.Select(a =>
            {
                var p = _listDbProvince.Find(b => b.name == a.Province && b.type == "Vx");
                var c = p.citys.Find(b => b.name == a.City && b.type == "Vx");
                return new P_HOSPITAL()
                {
                    CityId = c.id,
                    GskHospital = a.GskHospital,
                    OldGskHospital = "",
                    Name = a.Name,
                    OldName = "",
                    FirstLetters = NPinyin.Pinyin.GetInitials(a.Name).ToLower(),
                    Address = a.Address.Trim(),
                    Latitude = a.Latitude.ToString().Trim(),
                    Longitude = a.Longitude.ToString().Trim(),
                    Type = "Vx",
                    External = 0,
                    CreateDate = CreateDate,
                    ProvinceId = p.id,
                    IsXMS = "是",
                    IsBDS = "是",
                    IsMT = "否",
                    Remark = "",
                    IsDelete = 0,
                    //RelateUserList = "",
                    MainAddress = "主地址",
                    HospitalCode = a.GskHospital
                };
            }).ToList();

            var listDetail = res.Select(a =>
            {
                return new P_HOSPITAL_DETAIL()
                {
                    GskHospital = a.GskHospital,
                    District = a.District,
                    DistrictCode = a.DistrictCode,
                    CustomerType = a.CustomerType,
                    RESP = "",
                    HEP = "",
                    CNS = "",
                    HIV = "",
                    VOL = "",
                    MA = "",
                    Region = a.Region,
                    IsDelete = 0,
                    CreateDate = CreateDate,
                    UpdateDate = UpdateDate

                };
            }).ToList();
            hospitalDao.InsertHospitalData(listHospital);
            hospitalDao.InsertHospitalDetail(listDetail);

            return 1;
        }
        #endregion

        #region 更新门地址IsDelete=0
        /// <summary>
        /// 更新门地址IsDelete=0
        /// </summary>
        /// <param name="Market"></param>
        /// <returns></returns>
        public int UpdateVxStatus(string Market)
        {
            return hospitalDao.UpdateVxStatus(Market);
        }
        #endregion

        #region 获取DDT临时表数据
        /// <summary>
        /// 获取DDT临时表数据
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public List<P_HOSPITAL_DDTTemp> LoadDDTTemp()
        {
            return hospitalDao.LoadDDTTemp();
        }
        #endregion

        #region 插入DDT医院
        /// <summary>
        /// 插入医院
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        public int InsertDDTData(List<P_HOSPITAL_DDTTemp> res)
        {
            // 本次插入涉及的省市
            var p_c = res.Select(a => new { p = a.Province, c = a.City, t = "DDT" }).Distinct().OrderBy(a => a.p).ToList();
            var listProvince = p_c.GroupBy(a => new { a.p, a.t }).Select(a => new { id = 0, name = a.Key.p, type = a.Key.t, citys = a.Select(b => new { id = 0, name = b.c, type = b.t }).ToList() }).ToList();
            string marketString = "('DDT')";
            // 数据库中现存的省市
            var listDbProvince = baseDataDao.LoadProvinceByMarketList(marketString);
            var listDbCity = baseDataDao.LoadCityByMarketList(marketString);
            var _listDbProvince = listDbProvince.Select(a =>
            {
                return new
                {
                    id = a.ID,
                    name = a.Name,
                    citys = listDbCity.Where(b => b.ProvinceId == a.ID).Select(c => new { id = c.ID, name = c.Name, type = c.Type }).ToList(),
                    type = a.Type
                };
            }).ToList();

            // 需要补录的省、市
            var listNonProvince
                = listProvince.Select(a => new { a.name, a.type }).ToList()
                .Except(_listDbProvince.Select(a => new { a.name, a.type }).ToList())
                .Select(a => new NonPC
                {
                    NonProvince = 1,
                    name = a.name,
                    citys = listProvince.Find(b => b.name == a.name).citys.Select(c => new City { name = c.name, type = c.type }).ToList(),
                    type = a.type
                }).ToList();

            foreach (var p in listProvince)
            {
                if (_listDbProvince.Exists(a => a.name == p.name && a.type == p.type))
                {
                    var _p = _listDbProvince.Find(a => a.name == p.name && a.type == p.type);
                    var listNonCity = p.citys.Select(a => new { a.name, a.type }).ToList().Except(_p.citys.Select(a => new { a.name, a.type }).ToList()).ToList();
                    if (listNonCity.Count > 0)
                    {
                        listNonProvince.Add(new NonPC
                        {
                            name = p.name,
                            citys = listNonCity.Select(c => new City { name = c.name, type = c.type }).ToList(),
                            type = p.type
                        });
                    }
                }
            }

            if (listNonProvince.Count > 0)
            {
                // 补充省
                baseDataDao.AddProvince(listNonProvince.Where(a => a.NonProvince == 1).Select(a => new P_PROVINCE()
                {
                    Name = a.name,
                    PinYin = GetPinYin(a.name),
                    Type = a.type
                }).ToList());
                listDbProvince = baseDataDao.LoadProvinceByMarketList(marketString);
                foreach (var item in listNonProvince)
                {
                    var _item = listDbProvince.Find(a => a.Name == item.name && a.Type == item.type);
                    if (_item != null)
                    {
                        // 补充市
                        baseDataDao.AddCity(item.citys.Select(a => new P_CITY()
                        {
                            ProvinceId = _item.ID,
                            Name = a.name,
                            PinYin = GetPinYin(a.name),
                            Type = a.type
                        }).ToList());
                    }
                }
                listDbCity = baseDataDao.LoadCityByMarketList(marketString);
            }

            _listDbProvince = listDbProvince.Select(a =>
            {
                return new
                {
                    id = a.ID,
                    name = a.Name,
                    citys = listDbCity.Where(b => b.ProvinceId == a.ID).Select(c => new { id = c.ID, name = c.Name, type = c.Type }).ToList(),
                    type = a.Type
                };
            }).ToList();

            // 插入医院数据
            var CreateDate = DateTime.Now;
            var UpdateDate = DateTime.Now;
            var listHospital = res.Select(a =>
            {
                var p = _listDbProvince.Find(b => b.name == a.Province && b.type == "DDT");
                var c = p.citys.Find(b => b.name == a.City && b.type == "DDT");
                return new P_HOSPITAL()
                {
                    CityId = c.id,
                    GskHospital = a.GskHospital,
                    OldGskHospital = "",
                    Name = a.Name,
                    OldName = "",
                    FirstLetters = NPinyin.Pinyin.GetInitials(a.Name).ToLower(),
                    Address = a.Address.Trim(),
                    Latitude = a.Latitude.ToString().Trim(),
                    Longitude = a.Longitude.ToString().Trim(),
                    Type = "DDT",
                    External = 0,
                    CreateDate = CreateDate,
                    ProvinceId = p.id,
                    IsXMS = "是",
                    IsBDS = "是",
                    IsMT = "否",
                    Remark = "",
                    IsDelete = 0,
                    //RelateUserList = "",
                    MainAddress = "主地址",
                    HospitalCode = a.GskHospital
                };
            }).ToList();

            var listDetail = res.Select(a =>
            {
                return new P_HOSPITAL_DETAIL()
                {
                    GskHospital = a.GskHospital,
                    District = a.District,
                    DistrictCode = a.DistrictCode,
                    CustomerType = a.CustomerType,
                    RESP = "",
                    HEP = "",
                    CNS = "",
                    HIV = "",
                    VOL = "",
                    MA = "",
                    Region = a.Region,
                    IsDelete = 0,
                    CreateDate = CreateDate,
                    UpdateDate = UpdateDate

                };
            }).ToList();
            hospitalDao.InsertHospitalData(listHospital);
            hospitalDao.InsertHospitalDetail(listDetail);

            return 1;
        }
        #endregion

        #region 更新门地址IsDelete=0
        /// <summary>
        /// 更新门地址IsDelete=0
        /// </summary>
        /// <param name="Market"></param>
        /// <returns></returns>
        public int UpdateDDTStatus(string Market)
        {
            return hospitalDao.UpdateDDTStatus(Market);
        }
        #endregion

        #region 获取TSKF临时表数据
        /// <summary>
        /// 获取TSKF临时表数据
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public List<P_HOSPITAL_TSKFTemp> LoadTSKFTemp()
        {
            return hospitalDao.LoadTSKFTemp();
        }
        #endregion

        #region 插入TSKF医院
        /// <summary>
        /// 插入医院
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        public int InsertTSKFData(List<P_HOSPITAL_TSKFTemp> res)
        {
            // 本次插入涉及的省市
            var p_c = res.Select(a => new { p = a.Province, c = a.City, t = "TSKF" }).Distinct().OrderBy(a => a.p).ToList();
            var listProvince = p_c.GroupBy(a => new { a.p, a.t }).Select(a => new { id = 0, name = a.Key.p, type = a.Key.t, citys = a.Select(b => new { id = 0, name = b.c, type = b.t }).ToList() }).ToList();
            string marketString = "('TSKF')";
            // 数据库中现存的省市
            var listDbProvince = baseDataDao.LoadProvinceByMarketList(marketString);
            var listDbCity = baseDataDao.LoadCityByMarketList(marketString);
            var _listDbProvince = listDbProvince.Select(a =>
            {
                return new
                {
                    id = a.ID,
                    name = a.Name,
                    citys = listDbCity.Where(b => b.ProvinceId == a.ID).Select(c => new { id = c.ID, name = c.Name, type = c.Type }).ToList(),
                    type = a.Type
                };
            }).ToList();

            // 需要补录的省、市
            var listNonProvince
                = listProvince.Select(a => new { a.name, a.type }).ToList()
                .Except(_listDbProvince.Select(a => new { a.name, a.type }).ToList())
                .Select(a => new NonPC
                {
                    NonProvince = 1,
                    name = a.name,
                    citys = listProvince.Find(b => b.name == a.name).citys.Select(c => new City { name = c.name, type = c.type }).ToList(),
                    type = a.type
                }).ToList();

            foreach (var p in listProvince)
            {
                if (_listDbProvince.Exists(a => a.name == p.name && a.type == p.type))
                {
                    var _p = _listDbProvince.Find(a => a.name == p.name && a.type == p.type);
                    var listNonCity = p.citys.Select(a => new { a.name, a.type }).ToList().Except(_p.citys.Select(a => new { a.name, a.type }).ToList()).ToList();
                    if (listNonCity.Count > 0)
                    {
                        listNonProvince.Add(new NonPC
                        {
                            name = p.name,
                            citys = listNonCity.Select(c => new City { name = c.name, type = c.type }).ToList(),
                            type = p.type
                        });
                    }
                }
            }

            if (listNonProvince.Count > 0)
            {
                // 补充省
                baseDataDao.AddProvince(listNonProvince.Where(a => a.NonProvince == 1).Select(a => new P_PROVINCE()
                {
                    Name = a.name,
                    PinYin = GetPinYin(a.name),
                    Type = a.type
                }).ToList());
                listDbProvince = baseDataDao.LoadProvinceByMarketList(marketString);
                foreach (var item in listNonProvince)
                {
                    var _item = listDbProvince.Find(a => a.Name == item.name && a.Type == item.type);
                    if (_item != null)
                    {
                        // 补充市
                        baseDataDao.AddCity(item.citys.Select(a => new P_CITY()
                        {
                            ProvinceId = _item.ID,
                            Name = a.name,
                            PinYin = GetPinYin(a.name),
                            Type = a.type
                        }).ToList());
                    }
                }
                listDbCity = baseDataDao.LoadCityByMarketList(marketString);
            }

            _listDbProvince = listDbProvince.Select(a =>
            {
                return new
                {
                    id = a.ID,
                    name = a.Name,
                    citys = listDbCity.Where(b => b.ProvinceId == a.ID).Select(c => new { id = c.ID, name = c.Name, type = c.Type }).ToList(),
                    type = a.Type
                };
            }).ToList();

            // 插入医院数据
            var CreateDate = DateTime.Now;
            var UpdateDate = DateTime.Now;
            var listHospital = res.Select(a =>
            {
                var p = _listDbProvince.Find(b => b.name == a.Province && b.type == "TSKF");
                var c = p.citys.Find(b => b.name == a.City && b.type == "TSKF");
                return new P_HOSPITAL()
                {
                    CityId = c.id,
                    GskHospital = a.GskHospital,
                    OldGskHospital = "",
                    Name = a.Name,
                    OldName = "",
                    FirstLetters = NPinyin.Pinyin.GetInitials(a.Name).ToLower(),
                    Address = a.Address.Trim(),
                    Latitude = a.Latitude.ToString().Trim(),
                    Longitude = a.Longitude.ToString().Trim(),
                    Type = "TSKF",
                    External = 0,
                    CreateDate = CreateDate,
                    ProvinceId = p.id,
                    IsXMS = "是",
                    IsBDS = "是",
                    IsMT = "否",
                    Remark = "",
                    IsDelete = 0,
                    //RelateUserList = "",
                    MainAddress = "主地址",
                    HospitalCode = a.GskHospital
                };
            }).ToList();

            var listDetail = res.Select(a =>
            {
                return new P_HOSPITAL_DETAIL()
                {
                    GskHospital = a.GskHospital,
                    District = a.District,
                    DistrictCode = a.DistrictCode,
                    CustomerType = a.CustomerType,
                    RESP = "",
                    HEP = "",
                    CNS = "",
                    HIV = "",
                    VOL = "",
                    MA = "",
                    Region = a.Region,
                    IsDelete = 0,
                    CreateDate = CreateDate,
                    UpdateDate = UpdateDate

                };
            }).ToList();
            hospitalDao.InsertHospitalData(listHospital);
            hospitalDao.InsertHospitalDetail(listDetail);

            return 1;
        }
        #endregion

        #region 更新门地址IsDelete=0
        /// <summary>
        /// 更新门地址IsDelete=0
        /// </summary>
        /// <param name="Market"></param>
        /// <returns></returns>
        public int UpdateTSKFStatus(string Market)
        {
            return hospitalDao.UpdateTSKFStatus(Market);
        }
        #endregion

        #region 根据Market及ACTION查询变量表数据
        public List<Temp_Hospital_Variables> GetHosVariables(string Market, string ACTION)
        {
            return hospitalDao.GetHosVariables(Market, ACTION);
        }
        #endregion

        #region 查询主数据表中与变量表匹配的门地址
        public List<P_HOSPITAL> Getnotmain(string Market, string ACTION, string Type)
        {
            return hospitalDao.Getnotmain(Market, ACTION, Type);
        }
        #endregion

        #region 更新失效门地址状态为IsDelete=2
        public int Updatenotmain(List<P_HOSPITAL> notmain, List<Temp_Hospital_Variables> varList)
        {
            return hospitalDao.Updatenotmain(notmain, varList);
        }
        #endregion

        #region 更新主地址地址变更的门地址状态为IsDelete=3
        public int UpdateAnotmain(List<P_HOSPITAL> Anotmain, List<Temp_Hospital_Variables> varList)
        {
            return hospitalDao.UpdateAnotmain(Anotmain, varList);
        }
        #endregion

        #region 变量记录
        public List<P_HospitalVariables> LoadHospitalVariables(string srh_GskHospital, string srh_HospitalName, string srh_HospitalMarket, bool srh_Add, bool srh_City, bool srh_UpdateHospitalName, bool srh_LatLong, bool srh_Address, bool srh_Delete, int rows, int page, out int total)
        {
            return hospitalDao.LoadHospitalVariables(srh_GskHospital, srh_HospitalName, srh_HospitalMarket, srh_Add, srh_City, srh_UpdateHospitalName, srh_LatLong, srh_Address, srh_Delete, rows, page, out total);
        }

        public List<P_HospitalVariables> ExportHospitalVariablesList(string srh_GskHospital, string srh_HospitalName, string srh_HospitalMarket, bool srh_Add, bool srh_City, bool srh_UpdateHospitalName, bool srh_LatLong, bool srh_Address, bool srh_Delete)
        {
            return hospitalDao.ExportHospitalVariablesList(srh_GskHospital, srh_HospitalName, srh_HospitalMarket, srh_Add, srh_City, srh_UpdateHospitalName, srh_LatLong, srh_Address, srh_Delete);
        }

        public int SyncCoypHospitalVariablesData()
        {
            return hospitalDao.SyncCoypHospitalVariablesData();
        }

        public int InsertHospitalVariablesCountData()
        {
            return hospitalDao.InsertHospitalVariablesCountData();
        }
        public List<P_Hospital_Variables_Count> LoadHospitalVariablesCount(int rows, int page, out int total)
        {
            return hospitalDao.LoadHospitalVariablesCount(rows, page, out total);
        }
        public List<P_CHECK_REPORT_LINE_RM> LoadTerritoryRMVariables(string srh_market, bool srh_Add, bool srh_Delete, int rows, int page, out int total)
        {
            return hospitalDao.LoadHospitalVariablesCount(srh_market, srh_Add, srh_Delete, rows, page, out total);
        }
        public List<P_CHECK_REPORT_LINE_RM> ExportTerritoryRMVariables(string srh_market, bool srh_Add, bool srh_Delete)
        {
            return hospitalDao.ExportTerritoryRMVariables(srh_market, srh_Add, srh_Delete);
        }
        public List<CHECK_REPORT_LINE_RM> LoadTerritoryRMVariablesData()
        {
            return hospitalDao.LoadTerritoryRMVariablesData();
        }
        public int SyncCoypTerritoryRMVariablesData(List<CHECK_REPORT_LINE_RM> list)
        {
            return hospitalDao.SyncCoypTerritoryRMVariablesData(list);
        }
        #endregion

        #region 获取变量临时表数据
        /// <summary>
        /// 获取变量临时表数据
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public List<Temp_Hospital_Variables> LoadVariablesTemp()
        {
            return hospitalDao.LoadVariablesTemp();
        }
        #endregion

        #region 获取变量临时表非删除数据
        /// <summary>
        /// 获取变量临时表非删除数据
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public List<Temp_Hospital_Variables> LoadVariablesTempNoDelete()
        {
            return hospitalDao.LoadVariablesTempNoDelete();
        }
        #endregion

        #region 转换及补录省市ID
        /// <summary>
        /// 转换及补录省市ID
        /// </summary>
        /// <param name="Nodeltemp"></param>
        /// <returns></returns>
        public List<Temp_Hospital_Variables> LoadProvinceCityId(List<Temp_Hospital_Variables> Nodeltemp)
        {
            // 本次导入涉及的省市
            var p_c = Nodeltemp.Select(a => new { p = a.Province, c = a.City, t = a.Market }).Distinct().OrderBy(a => a.p).ToList();
            var listProvince = p_c.GroupBy(a => new { a.p, a.t }).Select(a => new { id = 0, name = a.Key.p, type = a.Key.t, citys = a.Select(b => new { id = 0, name = b.c, type = b.t }).ToList() }).ToList();
            var marketList = Nodeltemp.Select(p => new { market = p.Market }).ToList().Distinct();
            string marketString = "(";
            foreach (var item in marketList)
            {
                marketString += "'" + item.market + "',";
            }
            marketString = marketString.Substring(0, marketString.Length - 1) + ")";
            // 数据库中现存的省市
            var listDbProvince = baseDataDao.LoadProvinceByMarketList(marketString);
            var listDbCity = baseDataDao.LoadCityByMarketList(marketString);
            var _listDbProvince = listDbProvince.Select(a =>
            {
                return new
                {
                    id = a.ID,
                    name = a.Name,
                    citys = listDbCity.Where(b => b.ProvinceId == a.ID).Select(c => new { id = c.ID, name = c.Name, type = c.Type }).ToList(),
                    type = a.Type
                };
            }).ToList();

            // 需要补录的省、市
            var listNonProvince
                = listProvince.Select(a => new { a.name, a.type }).ToList()
                .Except(_listDbProvince.Select(a => new { a.name, a.type }).ToList())
                .Select(a => new NonPC
                {
                    NonProvince = 1,
                    name = a.name,
                    citys = listProvince.Find(b => b.name == a.name).citys.Select(c => new City { name = c.name, type = c.type }).ToList(),
                    type = a.type
                }).ToList();

            foreach (var p in listProvince)
            {
                if (_listDbProvince.Exists(a => a.name == p.name && a.type == p.type))
                {
                    var _p = _listDbProvince.Find(a => a.name == p.name && a.type == p.type);
                    var listNonCity = p.citys.Select(a => new { a.name, a.type }).ToList().Except(_p.citys.Select(a => new { a.name, a.type }).ToList()).ToList();
                    if (listNonCity.Count > 0)
                    {
                        listNonProvince.Add(new NonPC
                        {
                            name = p.name,
                            citys = listNonCity.Select(c => new City { name = c.name, type = c.type }).ToList(),
                            type = p.type
                        });
                    }
                }
            }

            if (listNonProvince.Count > 0)
            {
                // 补充省
                baseDataDao.AddProvince(listNonProvince.Where(a => a.NonProvince == 1).Select(a => new P_PROVINCE()
                {
                    Name = a.name,
                    PinYin = GetPinYin(a.name),
                    Type = a.type
                }).ToList());
                listDbProvince = baseDataDao.LoadProvinceByMarketList(marketString);
                foreach (var item in listNonProvince)
                {
                    var _item = listDbProvince.Find(a => a.Name == item.name && a.Type == item.type);
                    if (_item != null)
                    {
                        // 补充市
                        baseDataDao.AddCity(item.citys.Select(a => new P_CITY()
                        {
                            ProvinceId = _item.ID,
                            Name = a.name,
                            PinYin = GetPinYin(a.name),
                            Type = a.type
                        }).ToList());
                    }
                }
                listDbCity = baseDataDao.LoadCityByMarketList(marketString);
            }

            _listDbProvince = listDbProvince.Select(a =>
            {
                return new
                {
                    id = a.ID,
                    name = a.Name,
                    citys = listDbCity.Where(b => b.ProvinceId == a.ID).Select(c => new { id = c.ID, name = c.Name, type = c.Type }).ToList(),
                    type = a.Type
                };
            }).ToList();


            var listHospital = Nodeltemp.Select(a =>
            {
                var p = _listDbProvince.Find(b => b.name == a.Province && b.type == a.Market);
                var c = p.citys.Find(b => b.name == a.City && b.type == a.Market);
                return new Temp_Hospital_Variables()
                {

                    GskHospital = a.GskHospital,
                    Province = p.id.ToString().Trim(),
                    City = c.id.ToString().Trim(),
                    HospitalName = a.HospitalName,
                    Address = a.Address,
                    IsMainAdd = a.IsMainAdd,
                    Market = a.Market,
                    Longitude = a.Longitude,
                    Latitude = a.Latitude,
                    DistrictCode = a.DistrictCode,
                    District = a.District,
                    action = a.action,
                    createdate = a.createdate,
                    createby = a.createby,
                    Remarks = a.Remarks
                };
            }).ToList();
            return listHospital;
        }
        #endregion

        #region 获取变量临时表不同数据
        public List<Temp_Hospital_Variables> LoadVariablesTempAddData(string ACTION)
        {
            return hospitalDao.LoadVariablesTempAddData(ACTION);
        }
        #endregion

        #region 删除新增地址
        public int DeleteAddress(P_HOSPITAL hospital)
        {
            return hospitalDao.DeleteAddress(hospital);
        }
        #endregion

        #region 医院summary
        public List<P_Brand_Coverage_Count> LoadBrandCoverageCount()
        {
            return hospitalDao.LoadBrandCoverageCount();
        }

        public List<P_Brand_Coverage_Count> LoadBrandCoverageCountOH()
        {
            return hospitalDao.LoadBrandCoverageCountOH();
        }

        public List<P_HOSPITAL> LoadHospital()
        {
            return hospitalDao.LoadHospital();
        }

        public List<P_TERRITORY_TA> LoadTerritoryTA()
        {
            return hospitalDao.LoadTerritoryTA();
        }

        public List<P_TA_HOSPITAL> LoadTAHospital()
        {
            return hospitalDao.LoadTAHospital();
        }

        public List<P_TA_HOSPITAL> LoadTAHospitalOH()
        {
            return hospitalDao.LoadTAHospitalOH();
        }

        public List<P_Brand_Coverage_Count_TA> LoadBrandCoverageCountTA()
        {
            return hospitalDao.LoadBrandCoverageCountTA();
        }

        public int InsertHospitalVariablesCountDataTA(List<P_TERRITORY_TA> p_TAs)
        {
            return hospitalDao.InsertHospitalVariablesCountDataTA(p_TAs);
        }

        public List<P_TERRITORY_TA> LoadAllTerritoryTA()
        {
            return hospitalDao.LoadAllTerritoryTA();
        }
        #endregion
        //#region 根据变量表数据同步更新主数据和detail表
        ///// <summary>
        ///// 根据变量表数据同步更新主数据和detail表
        ///// </summary>
        ///// <returns></returns>
        //public int UpdateHospitalAndDetail(List<Temp_Hospital_Variables> addlist, List<Temp_Hospital_Variables> Updatelist, List<Temp_Hospital_Variables> dellist)
        //{
        //    #region 处理新增数据
        //    // 插入医院主数据
        //    var CreateDate = DateTime.Now;
        //    var UpdateDate = DateTime.Now;
        //    var listHospital = addlist.Select(a =>
        //    { 
        //        return new P_HOSPITAL()
        //        {
        //            CityId = Convert.ToInt32(a.City),
        //            GskHospital = a.GskHospital,
        //            OldGskHospital = "",
        //            Name = a.HospitalName,
        //            OldName = "",
        //            FirstLetters = NPinyin.Pinyin.GetInitials(a.HospitalName).ToLower(),
        //            Address = a.Address,
        //            Latitude = a.Latitude.ToString().Trim(),
        //            Longitude = a.Longitude.ToString().Trim(),
        //            Type = a.Market,
        //            External = 0,
        //            CreateDate = CreateDate,
        //            ProvinceId = Convert.ToInt32(a.Province),
        //            IsXMS = "是",
        //            IsBDS = "是",
        //            IsMT = "否",
        //            Remark = "",
        //            IsDelete = 0,
        //            //RelateUserList = "",
        //            MainAddress = "主地址",
        //            HospitalCode = a.GskHospital
        //        };
        //    }).ToList();

        //    var listDetail = res.Select(a =>
        //    {
        //        return new P_HOSPITAL_DETAIL()
        //        {
        //            GskHospital = a.GskHospital,
        //            District = a.District,
        //            DistrictCode = a.DistrictCode,
        //            CustomerType = a.CustomerType,
        //            RESP = "",
        //            HEP = "",
        //            CNS = "",
        //            HIV = "",
        //            VOL = "",
        //            MA = "",
        //            Region = a.Region,
        //            IsDelete = 0,
        //            CreateDate = CreateDate,
        //            UpdateDate = UpdateDate

        //        };
        //    }).ToList();
        //    #endregion


        //    hospitalDao.InsertHospitalData(listHospital);
        //    hospitalDao.InsertHospitalDetail(listDetail);

        //    return 1;
        //}
        //#endregion



        #region 同步医院表
        public int SyncHospital()
        {
            return hospitalDao.SyncHospital();
        }

        public int SyncHospitalDetail()
        {
            return hospitalDao.SyncHospitalDetail();
        }

        public int SyncTerritoryHospital()
        {
            return hospitalDao.SyncTerritoryHospital();
        }

        public int SyncHospitalRange()
        {
            return hospitalDao.SyncHospitalRange();
        }

        #endregion

        #region 获取RM列表
        public List<V_TerritoryRM> LoadTerritoryRMList(string TerritoryStr)
        {
            return hospitalDao.LoadTerritoryRMList(TerritoryStr);
        }
        #endregion
    }

    public class NonPC
    {
        public int NonProvince { get; set; } = 0;
        public string name { get; set; }
        public List<City> citys { get; set; }
        public string type { get; set; }
    }
    public class City
    {
        public string name { get; set; }
        public string type { get; set; }
    }




}
