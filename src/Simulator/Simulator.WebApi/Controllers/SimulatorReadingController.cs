using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Simulator.Domain.Entities;
using Simulator.Infrastructure.Services;

namespace Simulator.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SimulatorReadingController : ControllerBase
    {
        private readonly ISimulatorStateService _simulator;

        public SimulatorReadingController(ISimulatorStateService simulator)
        {
            _simulator = simulator;
        }

        /// <summary>
        /// Возвращает текущее показание счётчика
        /// </summary>
        [HttpGet]
        public ActionResult<CounterReading> GetReading()
        {
            var reading = _simulator.GetCurrentReading();
            return Ok(reading);
        }
    }
}
