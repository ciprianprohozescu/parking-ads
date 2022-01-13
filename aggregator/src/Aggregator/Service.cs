using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Aggregator
{
    public class Service
    {
        private static Dictionary<string, JObject> storedMessages = new Dictionary<string, JObject>();

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() {HostName = "rabbitmq"};
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare(queue: "aggregator",
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

                var message =
                    JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(Encoding.UTF8.GetString(body));
                if (message != null)
                {
                    Console.WriteLine(" [x] Received {0}", message);

                    if (!storedMessages.ContainsKey(props.CorrelationId))
                    {
                        // Start a new message
                        JObject newMessage = new JObject(
                            new JProperty("task-total", (int) message["task-total"]),
                            new JProperty("task-done", 0),
                            new JProperty("body", new JObject()));
                        storedMessages[props.CorrelationId] = newMessage;
                    }

                    // Add the contents of this message's body to the aggregated message
                    foreach (JProperty property in ((JObject)message["body"]).Properties())
                    {
                        storedMessages[props.CorrelationId]["body"][property.Name] = property.Value;
                    }

                    // Mark new task as completed
                    storedMessages[props.CorrelationId]["task-done"] =
                        (int) storedMessages[props.CorrelationId]["task-done"] + 1;

                    Console.WriteLine("[x] {0} / {1} tasks completed",
                        storedMessages[props.CorrelationId]["task-done"],
                        storedMessages[props.CorrelationId]["task-total"]);

                    if ((int)storedMessages[props.CorrelationId]["task-done"] ==
                        (int)storedMessages[props.CorrelationId]["task-total"])
                    {
                        Console.WriteLine("[x] Sending message back");
                        // All tasks completed, send the message back and remove it
                        storedMessages[props.CorrelationId]["task-total"] = 1;
                        channel.BasicPublish(
                            exchange: "",
                            routingKey: "main-router",
                            basicProperties: props,
                            body: Encoding.UTF8.GetBytes(
                                JsonConvert.SerializeObject(storedMessages[props.CorrelationId])));
                        storedMessages.Remove(props.CorrelationId);
                    }
                }
            };
            channel.BasicConsume(queue: "aggregator",
                autoAck: true,
                consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}