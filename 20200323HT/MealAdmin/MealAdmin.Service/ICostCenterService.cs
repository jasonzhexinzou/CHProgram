using MealAdmin.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Service
{
    public interface ICostCenterService
    {
        List<M_COSTCENTER> LoadPage(string code, string name, int rows, int page, out int total);
        int Add(M_COSTCENTER entity);
        int Upd(M_COSTCENTER entity);
        int Del(Guid id);
        M_COSTCENTER Find(Guid id);
        int Save(M_COSTCENTER entity);
        
    }
}
