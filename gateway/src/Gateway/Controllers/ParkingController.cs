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
        private Dictionary<string, int> services = new Dictionary<string, int>()
        {
            {"parking-service", 1}
        };
        
        private Dictionary<string, int> tasks = new Dictionary<string, int>()
        {
            {"list-parking-lots", 1}
        };

        private IModel channel;

        public ParkingController()
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq" };
            var connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange: "gateway", type: "direct");
        }
        
        [HttpGet]
        public async Task<string> Get()
        {
            try
            {
                Dictionary<int, int> taskList = new Dictionary<int, int>()
                {
                    {services["parking-service"], tasks["list-parking-lots"]}
                };

                JObject request = new JObject(
                    new JProperty("task-list", JsonConvert.SerializeObject(taskList)),
                    new JProperty("location", "Aalborg 9000"));
                
                // Create new message with a Correlation ID
                var props = channel.CreateBasicProperties();
                var correlationId = Guid.NewGuid().ToString();
                props.CorrelationId = correlationId;
                
                // Create new queue and bind it to the exchange
                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName,
                    exchange: "gateway",
                    routingKey: correlationId);
                
                // Send the request message
                var messageBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request));
                channel.BasicPublish(
                    exchange: "",
                    routingKey: "main-router",
                    basicProperties: props,
                    body: messageBytes);
                
                // Create new consumer and wait for the response
                var response = await Task.Run(() =>
                {
                    BasicGetResult result = null;
                    // TODO: maybe add a timeout to not wait infinitely
                    while (result == null)
                    {
                        result = channel.BasicGet(queueName, true);
                    }
                    return Encoding.UTF8.GetString(result.Body.ToArray());
                });
                
                // Destroy the queue
                channel.QueueDelete(queueName);
                
                return response;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        [Route("test")]
        public async Task<string> Test()
        {
            return "ok";
        }
    }
}