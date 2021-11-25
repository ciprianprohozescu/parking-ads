using System;
using RestSharp;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ParkingService
{
    public class Service
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq" };
        	var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
			channel.QueueDeclare(queue: "parking-requests",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);
            };
            channel.BasicConsume(queue: "parking-requests",
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        // public IRestResponse Serve()
        // {
        //     Console.WriteLine("ok");
        //     var client = new RestClient("http://psuparkingservice.fenris.ucn.dk");
        //     // Set a timeout of 5 seconds
        //     client.Timeout = 5000;
        //     var request = new RestRequest("service", DataFormat.Json);
        //     var response = client.Get(request);
        //
        //     Console.WriteLine((int)response.StatusCode);
        //     return response;
        // }
        //
        // public IRestResponse Send()
        // {
        //     Console.WriteLine("ok");
        //     var client = new RestClient("http://psuparkingservice.fenris.ucn.dk");
        //     // Set a timeout of 5 seconds
        //     client.Timeout = 5000;
        //     var request = new RestRequest("service", DataFormat.Json);
        //     var response = client.Get(request);
        //
        //     var body = Encoding.UTF8.GetBytes(response.Content);
        //     this.channel.QueueDeclare(queue: "parking-spots",
        //                          durable: false,
        //                          exclusive: false,
        //                          autoDelete: false,
        //                          arguments: null);
        //     this.channel.BasicPublish(exchange: "",
        //                          routingKey: "parking-spots",
        //                          basicProperties: null,
        //                          body: body);
        //
        //     Console.WriteLine((int)response.StatusCode);
        //     return response;
        // }
    }
}