using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Gateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ParkingController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            try
            {
                var parkingClient = new ParkingClient();
                
                JObject request = new JObject(
                    new JProperty("location", "Aalborg 9000"));

                var response = parkingClient.Call(JsonConvert.SerializeObject(request));
                parkingClient.Close();
                return response;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }

    public class ParkingClient
    {
        private IConnection connection;
        private IModel channel;
        private string replyQueueName;
        private EventingBasicConsumer consumer;
        private BlockingCollection<string> replyQueue = new BlockingCollection<string>();
        private IBasicProperties props;
        
        public ParkingClient()
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq" };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            replyQueueName = channel.QueueDeclare().QueueName;
            consumer = new EventingBasicConsumer(channel);

            props = channel.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.ReplyTo = replyQueueName;

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var response = Encoding.UTF8.GetString(body);
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    replyQueue.Add(response);
                }
            };

            channel.BasicConsume(
                consumer: consumer,
                queue: replyQueueName,
                autoAck: true);
        }
        
        public string Call(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(
                exchange: "",
                routingKey: "parking-requests",
                basicProperties: props,
                body: messageBytes);

            return replyQueue.Take();
        }

        public void Close()
        {
            connection.Close();
        }
    }
}