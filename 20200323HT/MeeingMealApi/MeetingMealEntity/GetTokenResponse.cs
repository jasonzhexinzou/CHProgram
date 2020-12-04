using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingMealEntity
{
    public class GetTokenResponse : ResponseBase
    {
        public GetTokenResult result { get; set; }
    }

    public class GetTokenResult
    {
        public string token { get; set; }
        public string expiresIn { get; set; }
    }

}
