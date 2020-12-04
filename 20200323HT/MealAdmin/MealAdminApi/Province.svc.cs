using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using MealAdmin.Entity;
using XFramework.XInject.Attributes;
using MealAdmin.Service;

namespace MealAdminApi
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Province”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Province.svc 或 Province.svc.cs，然后开始调试。
    public class Province : IProvince
    {
        [Bean("provinceService")]
        public IProvinceService provinceService { get; set; }

        public List<P_PROVINCE> Load(int rows, int page, out int total)
        {
            return provinceService.Load(rows, page, out total);
        }
    }
}
