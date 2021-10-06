using System;
using RestSharp;

namespace AdServiceConsumer
{
    public class Consumer
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        public static IRestResponse Consume()
        {
            var client = new RestClient("http://psuaddservice.fenris.ucn.dk");
            var request = new RestRequest("service", DataFormat.Json);
            var response = client.Get(request);
            return response;
        }
    }
}
