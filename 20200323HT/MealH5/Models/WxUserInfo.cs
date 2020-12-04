using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MealH5.Models
{
    public class WxUserInfoRes
    {
        public int state { get; set; }
        public WxUserInfo userinfo { get; set; }
    }

    public class WxUserInfo
    {
        public int errcode { get; set; }
        public string deviceId { get; set; }
        public string openId { get; set; }
        public string userId { get; set; }
        public string ext { get; set; }
    }
}