using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ParkingUI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ParkingSpotsController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Available", "Unavailable"
        };

        private readonly ILogger<ParkingSpotsController> _logger;

        public ParkingSpotsController(ILogger<ParkingSpotsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<ParkingSpot> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new ParkingSpot
            {
                Date = DateTime.Now.AddDays(index),
                Name = "Spot " + rng.Next(1, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
