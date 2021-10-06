using System;
using RestSharp;

namespace ParkingServiceConsumer
{
    public class Consumer
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Consume();
                System.Threading.Thread.Sleep(10000);
            }
        }

        public static IRestResponse Consume()
        {
            var client = new RestClient("http://psuparkingservice.fenris.ucn.dk");
            var request = new RestRequest("service", DataFormat.Json);
            var response = client.Get(request);
            Console.WriteLine(response.Content);
            return response;
        }
    }
}