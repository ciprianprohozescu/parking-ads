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
            Console.WriteLine("ok");
            var client = new RestClient("http://psuparkingservice.fenris.ucn.dk");
            // Set a timeout of 5 seconds
            client.Timeout = 5000;
            var request = new RestRequest("service", DataFormat.Json);
            var response = client.Get(request);
            Console.WriteLine((int)response.StatusCode);
            return response;
        }
    }
}