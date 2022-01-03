using System;
using System.Collections.Generic;
using System.Linq;
using RestSharp;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;

namespace MainRouter
{
    public class Service
    {
        private static Dictionary<int, string> services = new Dictionary<int, string>()
        {
            {1, "parking-service"}
        };
        
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq" };
        	var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
			channel.QueueDeclare(queue: "main-router", 
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

                // Send the message to the first microservice in the task list
                var message = JsonConvert.DeserializeObject<Dictionary<string,dynamic>>(Encoding.UTF8.GetString(body));
                if (message != null && message.ContainsKey("task-list"))
                {
                    foreach (KeyValuePair<int,int> task in JsonConvert.DeserializeObject<Dictionary<int,int>>(message["task-list"]))
                    {
                        // Does the message have a task?
                        if (task.Value > 0)
                        {
                            channel.BasicPublish(
                                exchange: "",
                                routingKey: services[task.Key],
                                basicProperties: props,
                                body: body);
                        }
                        else
                        {
                            // Send it back to the gateway
                            channel.BasicPublish(
                                exchange: "gateway",
                                routingKey: props.CorrelationId,
                                basicProperties: props,
                                body: body);
                        }
                        break;
                    }
                }
                else
                {
                    Console.WriteLine(" [x] Received {0} (wrong format)", message);
                }
                
            };
            channel.BasicConsume(queue: "main-router",
                autoAck: true,
                consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
