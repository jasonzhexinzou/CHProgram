using MealAdmin.Entity;
using MealAdmin.Entity.View;
using MealAdmin.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MealAdminApi
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Hospital”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Hospital.svc 或 Hospital.svc.cs，然后开始调试。
    public class BUManagement : IBUManagement
    {

        public IBUManagementService buManagementService = Global.applicationContext.GetBean("BUManagementService") as IBUManagementService;


        public P_BUINFO GetBUInfoByUserId(string UserId)
        {
            return buManagementService.GetBUInfoByUserId(UserId);
        }

        public P_TAINFO GetTAInfoByUserId(string UserId)
        {
            return buManagementService.GetTAInfoByUserId(UserId);
        }

        public List<P_TAINFO> GetTAInfoByBUID(Guid BUID)
        {
            return buManagementService.GetTAInfoByBUID(BUID);
        }

    }
}
