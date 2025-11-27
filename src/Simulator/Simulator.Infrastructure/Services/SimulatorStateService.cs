using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Simulator.Domain.Entities;
using Simulator.Domain.Enums;
using Simulator.Infrastructure.Options;

namespace Simulator.Infrastructure.Services
{
    public class SimulatorStateService : BackgroundService, ISimulatorStateService
    {
        private readonly ISimulatorEngine _engine;
        private readonly IOptions<SimulatorOptions> _options;
        private readonly ILogger<SimulatorStateService> _logger;
        private readonly object _lock = new();

        private long _counter;
        private DateTime _lastUpdate = DateTime.UtcNow;
        private SimulatorState _currentState = SimulatorState.Normal;
        private DateTime _lastStateChange = DateTime.UtcNow;

        public SimulatorState CurrentState => _currentState;

        public SimulatorStateService(
            ISimulatorEngine engine,
            IOptions<SimulatorOptions> options,
            ILogger<SimulatorStateService> logger)
        {
            _engine = engine;
            _options = options;
            _logger = logger;
        }

        public CounterReading GetCurrentReading()
        {
            return new CounterReading() { Timestamp = _lastUpdate, CounterValue = _counter };

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var random = new Random();

            while (!stoppingToken.IsCancellationRequested)
            {
                var delayMs = random.Next(1000, 2001);
                await Task.Delay(delayMs, stoppingToken);

                var now = DateTime.UtcNow;
                var totalSecondsInState = (now - _lastStateChange).TotalSeconds;

                var (newState, increment) = _engine.Advance(
                    totalSecondsInState,
                    _options.Value.NominalSpeedPcsPerHour,
                    _options.Value.MaxPhysicalSpeedPcsPerSec,
                    _options.Value.LowSpeedThresholdFactor,
                    _options.Value.MaxCounterValue,
                    delayMs / 1000.0);


                // TO_DO проверка nodata только отключением симулятора, потому это условие скорее от лукавого
                if (newState == SimulatorState.NoData)
                {
                    _logger.LogInformation("Симулятор: состояние NoData (пауза 45 сек)");
                    await Task.Delay(45_000, stoppingToken);
                    _lastStateChange = DateTime.UtcNow;
                    continue;
                }

                lock (_lock)
                {
                    if (increment > 0)
                    {
                        _counter = (_counter + increment) % _options.Value.MaxCounterValue;
                        _lastUpdate = now;
                    }
                    _currentState = newState;
                }

                if (random.NextDouble() < 0.02)
                    _lastStateChange = now;
            }
        }
    }
}