using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Normalizer.Application;
using Normalizer.Application.Dto;
using Normalizer.Domain.Enums;
using Normalizer.Infrastructure.Repositories;
using Normalizer.Infrastructure.Services;

namespace Normalizer.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NormalizerController : ControllerBase
    {
        private readonly INormalizerService _normalizer;
        private readonly IStatusEventRepository _eventRepo;

        public NormalizerController(INormalizerService normalizer, IStatusEventRepository eventRepo)
        {
            _normalizer = normalizer;
            _eventRepo = eventRepo;
        }

        /// <summary>
        /// Текущее состояние и скорость
        /// </summary>
        /// <returns></returns>
        [HttpGet("status")]
        public IActionResult GetStatus()
        {

            var tmp = _normalizer.CurrentStatus;

            return Ok(new LineStatusDto
            {
                Timestamp = DateTime.UtcNow,
                Status = _normalizer.CurrentStatus.ToString(),
                SpeedPcsPerHour = _normalizer.CurrentSpeedPcsPerHour
            });
        }

        /// <summary>
        /// Количество событий
        /// </summary>
        /// <param name="period"></param>
        /// <returns></returns>
        [HttpGet("stats")]
        public IActionResult GetStats([FromQuery] string period = "24h")
        {
            // можно задать не 24 часа
            var since = PeriodParser.ParsePeriod(period);

            var events = _eventRepo.GetEventsSince(since);
            var counts = Enum.GetValues<LineStatus>()
                .ToDictionary(
                    status => status.ToString(),
                    status => events.Count(e => e.Status == status)
                );

            return Ok(new LineStatsDto
            {
                From = since,
                To = DateTime.UtcNow,
                EventsCount = counts
            });
        }
    }
}
