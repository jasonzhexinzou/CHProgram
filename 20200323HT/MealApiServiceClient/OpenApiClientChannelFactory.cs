using MealApiService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace MealApiServiceClient
{
    public class OpenApiClientChannelFactory
    {
        //private static ChannelFactory<IOpenApi> ChannelFactory = new ChannelFactory<IOpenApi>(new BasicHttpBinding());
        public static IOpenApi GetChannel()
        {
            EndpointAddress addr = new EndpointAddress(System.Configuration.ConfigurationManager.AppSettings["IOpenApi_Url"]);
            //return ChannelFactory.CreateChannel(addr);

            Binding binding = new BasicHttpBinding();
            return ChannelFactory<IOpenApi>.CreateChannel(binding, addr);
        }
    }
}
