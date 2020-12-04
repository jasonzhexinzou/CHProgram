using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingMealEntity
{
    public class SyncCityResponse : ResponseBase
    {
        public List<ProvinceResult> result { get; set; }
        public int totleCount { get; set; }
    }


    public class ProvinceResult
    {
        public List<CityResult> citys { get; set; }
        public int deleted { get; set; }
        public string pinYin { get; set; }
        public int provinceId { get; set; }
        public string provinceName { get; set; }
    }

    public class CityResult
    {
        public int cityId { get; set; }
        public string cityName { get; set; }
        public int deleted { get; set; }
        public string pinYin { get; set; }
    }



}
