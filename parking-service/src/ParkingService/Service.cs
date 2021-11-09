using System;
using RestSharp;
using RabbitMQ.Client;
using System.Text;

namespace ParkingService
{
    public class Service
    {
        private ConnectionFactory factory;
        private IConnection connection;
        private IModel channel;

        static void Main(string[] args)
        {
            var service = new Service();
            while (true)
            {
                service.Send();
                System.Threading.Thread.Sleep(10000);
            }
        }

        public Service(bool rabbitmq = true)
        {
            if (rabbitmq) 
            {
                this.factory = new ConnectionFactory() { HostName = "rabbitmq" };
                this.connection = this.factory.CreateConnection();
                this.channel = this.connection.CreateModel();
            }
        }

        public IRestResponse Serve()
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

        public IRestResponse Send()
        {
            Console.WriteLine("ok");
            var client = new RestClient("http://psuparkingservice.fenris.ucn.dk");
            // Set a timeout of 5 seconds
            client.Timeout = 5000;
            var request = new RestRequest("service", DataFormat.Json);
            var response = client.Get(request);

            var body = Encoding.UTF8.GetBytes(response.Content);
            this.channel.QueueDeclare(queue: "parking-spots",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            this.channel.BasicPublish(exchange: "",
                                 routingKey: "parking-spots",
                                 basicProperties: null,
                                 body: body);

            Console.WriteLine((int)response.StatusCode);
            return response;
        }
    }
}