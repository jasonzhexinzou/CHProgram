using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Util
{
    public class ConstantHelper
    {
        public const string CurrentOpenId = "CurrentOpenId";
        public const string CurrentAdminUser = "CurrentAdminUser";
        public const string CurrentPermission = "CurrentPermission";
        public const string CurrentUser = "CurrentUser";
        public const string CurrentUserRole = "CurrentUserRole";

        public readonly static Guid AdminID = Guid.Parse("00000000-0000-0000-0000-000000000001");
    }
}
