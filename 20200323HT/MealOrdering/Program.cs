using MealApiInterface.Entity;
using MealApiService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MealOrdering
{
    class Program
    {
        static void Main(string[] args)
        {
            //var client = new OpenApiClient(new BasicHttpBinding(), new EndpointAddress("http://localhost:18346/OpenApi.svc"));
            //var res = client.LoadRestaurant("", "121.523744,38.8617", "", "", "");

            //var client = new System.ServiceModel.ChannelFactory<IOpenApi>(new BasicHttpBinding());
            //EndpointAddress ad = new EndpointAddress("http://localhost:18346/OpenApi.svc");
            //var apiOpen = client.CreateChannel(ad);
            //var res = apiOpen.LoadRestaurant("", "121.523744,38.8617", "", "", "");

            var channel = GetChannel();
            var res = channel.LoadRestaurant("", "121.523744,38.8617", "", "", "");
        }


        static IOpenApi GetChannel()
        {
            var client = new System.ServiceModel.ChannelFactory<IOpenApi>(new BasicHttpBinding());
            EndpointAddress ad = new EndpointAddress("http://localhost:18346/OpenApi.svc");
            return client.CreateChannel(ad);
        }
    }



}
