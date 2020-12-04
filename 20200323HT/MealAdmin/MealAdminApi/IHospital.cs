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
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IHospital”。
    [ServiceContract]
    public interface IHospital
    {
        [OperationContract]
        List<P_HOSPITALINFO> LoadHospital(string channel);

        [OperationContract]
        P_HOSPITAL FindByCode(string hospitalCode);

        [OperationContract]
        List<P_HospitalCoverChange> FindHospitalCoverChange(string Type);

        [OperationContract]
        List<V_TerritoryHospitalMRDM> FindHospitalUser(string HospitalCode);

        #region 获取RM列表
        [OperationContract]
        List<V_TerritoryRM> LoadTerritoryRMList(string TerritoryStr);
        #endregion
    }
}
