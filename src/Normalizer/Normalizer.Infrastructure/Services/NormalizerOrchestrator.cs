using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Normalizer.Domain.Entities;
using Normalizer.Domain.Enums;
using Normalizer.Domain.Events;
using Normalizer.Infrastructure.Options;
using Normalizer.Infrastructure.Repositories;
using System.Net.Http.Json;

namespace Normalizer.Infrastructure.Services
{
    public class NormalizerOrchestrator : INormalizerOrchestrator
    {
        private readonly HttpClient _httpClient;
        private readonly IStatusCalculator _calculator;
        private readonly IStatusEventRepository _eventRepo;
        private readonly NormalizerSettings _settings;
        private readonly ILogger<NormalizerOrchestrator> _logger;

        private CounterReading? _lastReading;
        private DateTime _lastFetchTime = DateTime.UtcNow;
        private LineStatus _currentStatus = LineStatus.NoData;
        private double _currentSpeed;

        private readonly object _lock = new();

        public LineStatus CurrentStatus => _currentStatus;
        public double CurrentSpeedPcsPerHour => _currentSpeed;

        public NormalizerOrchestrator(
            IHttpClientFactory httpClientFactory,
            IStatusCalculator calculator,
            IStatusEventRepository eventRepo,
            IOptions<NormalizerSettings> options, 
            ILogger<NormalizerOrchestrator> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _calculator = calculator;
            _eventRepo = eventRepo;
            _settings = new NormalizerSettings 
            {
                SimulatorUrl = options.Value.SimulatorUrl,
                NominalSpeedPcsPerHour = options.Value.NominalSpeedPcsPerHour,
                LowSpeedThresholdFactor = options.Value.LowSpeedThresholdFactor,
                StoppedThresholdSeconds = options.Value.StoppedThresholdSeconds,
                NoDataThresholdSeconds = options.Value.NoDataThresholdSeconds,
                PollingIntervalMs = options.Value.PollingIntervalMs
            };
            _logger = logger;
        }

        public async Task ProcessAsync(CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            CounterReading? reading = null;

            try
            {
                var response = await _httpClient.GetAsync($"{_settings.SimulatorUrl}/api/simulatorreading", ct);
                if (response.IsSuccessStatusCode)
                {
                    reading = await response.Content.ReadFromJsonAsync<CounterReading>(cancellationToken: ct);
                    _lastFetchTime = now;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Ошибка при опросе симулятора");
            }

            var (newStatus, speed) = _calculator.Calculate(
                reading, _lastReading, _lastFetchTime, now, _settings);

            if (newStatus != _currentStatus)
            {
                lock (_lock)
                {
                    _currentStatus = newStatus;
                    _currentSpeed = speed;
                }

                _eventRepo.Add(new LineStatusChangedEvent
                {
                    Timestamp = DateTime.UtcNow,
                    Status = newStatus
                });
                _logger.LogInformation("Статус изменён: {Old} => {New}", _currentStatus, newStatus);
            }
            else
            {
                lock (_lock) _currentSpeed = speed;
            }

            _lastReading = reading;
        }
    }
}
