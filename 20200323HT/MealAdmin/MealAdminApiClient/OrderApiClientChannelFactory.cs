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
    public class OrderApiClientChannelFactory
    {
        public static IOrder GetChannel()
        {
            EndpointAddress addr = new EndpointAddress(System.Configuration.ConfigurationManager.AppSettings["IOrderApi_Url"]);
            //return ChannelFactory.CreateChannel(addr);

            Binding binding = new BasicHttpBinding()
            {
                MaxBufferPoolSize = 10485760,
                MaxReceivedMessageSize = 10485760
            };
            return ChannelFactory<IOrder>.CreateChannel(binding, addr);
        }
    }
}
