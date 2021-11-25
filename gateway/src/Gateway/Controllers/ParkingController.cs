using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Gateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ParkingController : ControllerBase
    {
		private ConnectionFactory factory;
        private IConnection connection;
        private IModel channel;

		public ParkingController()
        {
            this.factory = new ConnectionFactory() { HostName = "rabbitmq" };
            this.connection = this.factory.CreateConnection();
            this.channel = this.connection.CreateModel();
        }

        [HttpGet]
        public string Get()
        {
            try
            {
                JObject request = new JObject(
                    new JProperty("location", "Aalborg 9000"));
            
                var body = Encoding.Default.GetBytes(JsonConvert.SerializeObject(request));
                this.channel.QueueDeclare(queue: "parking-requests",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
                this.channel.BasicPublish(exchange: "",
                    routingKey: "parking-requests",
                    basicProperties: null,
                    body: body);

                return "request sent";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}