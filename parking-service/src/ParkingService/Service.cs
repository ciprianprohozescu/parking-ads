using System;
using System.Collections.Generic;
using RestSharp;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ParkingService
{
    public class Service
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq" };
        	var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
			channel.QueueDeclare(queue: "parking-service", 
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                // Decode the message
                var body = ea.Body.ToArray();
                var props = ea.BasicProperties;
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);
                var messageJson = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(message);
                var taskList = JsonConvert.DeserializeObject<Dictionary<int, int>>(messageJson["task-list"]);
                var taskTotal = messageJson["task-total"];
                var content = messageJson["body"];
                
                // Request a list of parking spots from the UCN API
                var client = new RestClient("http://psuparkingservice.fenris.ucn.dk");
                // Set a timeout of 5 seconds
                client.Timeout = 5000;
                var request = new RestRequest("service", DataFormat.Json);
                var response = client.Get(request);
                
                // Create response message
                JObject responseMessage = new JObject(
                    new JProperty("body", new JObject(
                        new JProperty("parkingSpots", response.Content))));
                
                // Something went wrong with the UCN API
                if (response.Content == "")
                {
                    responseMessage = new JObject(
                        new JProperty("body", new JObject(
                            new JProperty("parkingSpots", ""))));
                }
                
                // Add the total number of tasks to the response
                responseMessage.Add("task-total", taskTotal);
                
                // Send a response with the parking spots list
                channel.BasicPublish(exchange: "", 
                    routingKey: "main-router",
                    basicProperties: props, 
                    body: Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(responseMessage)));
            };
            channel.BasicConsume(queue: "parking-service",
                autoAck: true,
                consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}