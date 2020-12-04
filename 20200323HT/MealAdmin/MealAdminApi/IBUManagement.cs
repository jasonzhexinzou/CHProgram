using MealAdmin.Entity;
using MealAdmin.Entity.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MealAdminApi
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IBUManagement”。
    [ServiceContract]
    public interface IBUManagement
    {
        [OperationContract]
        P_BUINFO GetBUInfoByUserId(string UserId);

        [OperationContract]
        P_TAINFO GetTAInfoByUserId(string UserId);

        [OperationContract]
        List<P_TAINFO> GetTAInfoByBUID(Guid BUID);
    }
}
