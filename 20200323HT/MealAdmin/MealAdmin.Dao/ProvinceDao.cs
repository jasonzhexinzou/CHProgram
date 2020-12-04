using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MealAdmin.Entity;
using XFramework.XInject.Attributes;
using XFramework.XDataBase;

namespace MealAdmin.Dao
{
    public class ProvinceDao : IProvinceDao
    {
        [Bean("sqlServerTemplFactory")]
        public DbTemplateFactory sqlServerTemplFactory { get; set; }

        public int Add()
        {
            return 0;
        }

        public int Delete(string provinceID)
        {
            throw new NotImplementedException();
        }

        private string selectSql = "SELECT * FROM P_PROVINCE";
        private string selectOrderBySql = " ORDER BY P_PROVINCE.ID ";

        public List<P_PROVINCE> Load(int rows, int page, out int total)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.LoadPages<P_PROVINCE>( rows, page, out total, selectSql, selectOrderBySql);
                return list;
            }
        }

    }
}
