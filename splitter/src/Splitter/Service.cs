using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Splitter
{
        public class Service
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq" };
        	var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
			channel.QueueDeclare(queue: "splitter", 
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
                
                var message = JsonConvert.DeserializeObject<Dictionary<string,dynamic>>(Encoding.UTF8.GetString(body));
                if (message != null)
                {
                    Console.WriteLine(" [x] Received {0}", message);
                    if (message.ContainsKey("task-list"))
                    {
                        Dictionary<int, int> taskList = JsonConvert.DeserializeObject<Dictionary<int,int>>(message["task-list"]);
                        foreach (KeyValuePair<int,int> task in taskList)
                        {
                            Dictionary<int, int> taskListPiece = new Dictionary<int, int>()
                            {
                                {task.Key, task.Value}
                            };
                            JObject messagePiece = new JObject(
                                new JProperty("task-list", JsonConvert.SerializeObject(taskListPiece)),
                                new JProperty("task-total", message["task-total"]),
                                new JProperty("body", message["body"]));
                            
                            channel.BasicPublish(
                                exchange: "",
                                routingKey: "main-router",
                                basicProperties: props,
                                body: body);
                        }
                    }
                }

            };
            channel.BasicConsume(queue: "splitter",
                autoAck: true,
                consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
