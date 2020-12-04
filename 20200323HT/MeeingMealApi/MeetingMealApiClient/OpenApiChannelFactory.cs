using MeetingMealApiService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace MeetingMealApiClient
{
    public class OpenApiChannelFactory
    {
        public static IOpenApi GetChannel()
        {
            EndpointAddress addr = new EndpointAddress(System.Configuration.ConfigurationManager.AppSettings["IMeetingMealApi_Url"]);
            //return ChannelFactory.CreateChannel(addr);

            BasicHttpBinding binding = new BasicHttpBinding();
            //binding.MaxBufferPoolSize = 104857600;
            binding.MaxBufferPoolSize = 2147483647;
            binding.MaxReceivedMessageSize = 2147483647;
            binding.MaxBufferSize = 2147483647;
            //binding.MaxReceivedMessageSize = 104857600;
            binding.SendTimeout = new TimeSpan(0, 10, 0);
            return ChannelFactory<IOpenApi>.CreateChannel(binding, addr);
        }
    }
}
