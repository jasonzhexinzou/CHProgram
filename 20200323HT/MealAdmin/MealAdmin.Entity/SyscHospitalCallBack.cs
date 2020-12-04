using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity
{
    public class SyscHospitalCallBack
    {
        public SyscHospitalResult requestData { get; set; }

    }


    public class SyscHospitalResult
    {

        public string timestamp { get; set; }
        public string Sign { get; set; }
        public string supplier { get; set; }
        
    }
}
