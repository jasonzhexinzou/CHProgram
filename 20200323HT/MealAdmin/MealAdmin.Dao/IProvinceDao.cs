using MealAdmin.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Dao
{
    public interface IProvinceDao
    {
        List<P_PROVINCE> Load(int rows, int page, out int total);

        int Add();

        int Delete(string provinceID);
    }
}
