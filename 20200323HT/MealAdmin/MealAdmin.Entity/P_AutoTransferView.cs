using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity
{
    public class P_AutoTransferView
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public Guid LineManagerId { get; set; }
        public string HTCode { get; set; }
        public string OrdUserId { get; set; }
        public int OrdIsTransfer { get; set; }
        public string OrdTransferMUDID { get; set; }
        public int IsOrderUpload { get; set; }
        public string UploadUserId { get; set; }
        public int UploadIsTransfer { get; set; }
        public string UploadTransferMUDID { get; set; }
        // Start UpdateBy zhexin.zou at 20190104
        public string UploadState { get; set; }
        // End UpdateBy zhexin.zou at 20190104
    }

    public class LineManager
    {
        public string LineManagerId { get; set; }
        public string LineManagerName { get; set; }
    }


    public class LineManagerUser
    {
        public string LineManagerId { get; set; }

        public string UserId { get; set; }

    }
}
