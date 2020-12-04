using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XFramework.WeChatAPI.SessionHandlers;

namespace iPathAuthService.Models
{
    public class WxQyConfigManager : IWxQyConfigManager
    {
        public WxQyConfigManager()
        {
            this.AgentId = System.Configuration.ConfigurationManager.AppSettings["WeChatAgentId"];
            this.CorpID = System.Configuration.ConfigurationManager.AppSettings["WeChatCorpID"];
            this.Secret = System.Configuration.ConfigurationManager.AppSettings["WeChatSecret"];
            this.Token = System.Configuration.ConfigurationManager.AppSettings["WeChatToken"];
        }

        public string AgentId { get; set; }
        public string CorpID { get; set; }
        public string Secret { get; set; }
        public string Token { get; set; }
    }
}