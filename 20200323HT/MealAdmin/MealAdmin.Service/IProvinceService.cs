using MealAdmin.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Service
{
    public interface IProvinceService
    {
        List<P_PROVINCE> Load(int rows, int page, out int total);
    }
}
