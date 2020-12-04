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
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IEvaluate”。
    [ServiceContract]
    public interface IEvaluate
    {
        [OperationContract]
        P_EVALUATE LoadByOrderID(Guid OrderID);

        [OperationContract]
        int AddUnd(P_EVALUATE entity,string totalPrice);

        [OperationContract]
        int Add(P_EVALUATE entity);

        [OperationContract]
        List<P_EVALUATE> LoadByResId(string resId);

        [OperationContract]
        List<P_RESTAURANT_START_VIEW> LoadStarByResIds(string[] resIds);

        [OperationContract]
        int AddOrderApprove(P_ORDER_APPROVE entity);
    }
}
