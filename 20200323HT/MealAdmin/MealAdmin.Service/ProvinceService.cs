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
    public class ProvinceService:IProvinceService
    {
        [Bean("provinceDao")]
        public IProvinceDao provinceDao { get; set; }


        public List<P_PROVINCE> Load(int rows, int page, out int total)
        {
            return provinceDao.Load(rows, page, out total);
        }

    }
}
