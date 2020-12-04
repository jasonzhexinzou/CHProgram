using MealAdminApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace MealAdminApiClient
{
    public class UserInfoClientChannelFactory
    {
        public static IUserInfo GetChannel()
        {
            EndpointAddress addr = new EndpointAddress(System.Configuration.ConfigurationManager.AppSettings["IUserInfo_Url"]);
            //return ChannelFactory.CreateChannel(addr);

            Binding binding = new BasicHttpBinding()
            {
                MaxBufferPoolSize = 10485760,
                MaxReceivedMessageSize = 10485760
            };
            return ChannelFactory<IUserInfo>.CreateChannel(binding, addr);
        }
    }
}
