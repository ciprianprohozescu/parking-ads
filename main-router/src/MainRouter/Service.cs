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
            {1, "parking-service"},
            {2, "ads-service"}
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
                if (message != null)
                {
                    Console.WriteLine(" [x] Received {0}", message);
                    if (message.ContainsKey("task-list"))
                    {
                        // This is a request message
                        Dictionary<int, int> taskList = JsonConvert.DeserializeObject<Dictionary<int,int>>(message["task-list"]);
                        if (taskList.Count > 1)
                        {
                            // Send it to the Splitter
                            channel.BasicPublish(
                                exchange: "",
                                routingKey: "splitter",
                                basicProperties: props,
                                body: body);
                        }
                        else
                        {
                            // Send it to the right end service
                            foreach (KeyValuePair<int,int> task in taskList)
                            {
                                channel.BasicPublish(
                                    exchange: "",
                                    routingKey: services[task.Key],
                                    basicProperties: props,
                                    body: body);
                            }
                        }
                    }
                    else
                    {
                        // This is a response message
                        int taskTotal = Int32.Parse(message["task-total"]);
                        if (taskTotal > 1)
                        {
                            // Send it to the Aggregator
                            channel.BasicPublish(
                                exchange: "",
                                routingKey: "aggregator",
                                basicProperties: props,
                                body: body);
                        }
                        else
                        {
                            // Send it to the Gateway
                            channel.BasicPublish(
                                exchange: "gateway",
                                routingKey: props.CorrelationId,
                                basicProperties: props,
                                body: body);
                        }
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
