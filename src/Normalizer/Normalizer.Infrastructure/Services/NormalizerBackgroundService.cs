using Microsoft.Extensions.Hosting;
using Normalizer.Domain.Enums;
using Normalizer.Infrastructure.Options;

namespace Normalizer.Infrastructure.Services
{
    public class NormalizerBackgroundService : BackgroundService, INormalizerService
    {
        private readonly INormalizerOrchestrator _orchestrator;
        private readonly NormalizerSettings _normalizerSettings;
        public LineStatus CurrentStatus => _orchestrator.CurrentStatus;
        public double CurrentSpeedPcsPerHour => _orchestrator.CurrentSpeedPcsPerHour;

        public NormalizerBackgroundService(INormalizerOrchestrator orchestrator, NormalizerSettings settings)
        {
            _orchestrator = orchestrator;
            _normalizerSettings = settings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var interval = TimeSpan.FromMilliseconds(_normalizerSettings.PollingIntervalMs);

            while (!stoppingToken.IsCancellationRequested)
            {
                await _orchestrator.ProcessAsync(stoppingToken);
                await Task.Delay(interval, stoppingToken);
            }
        }
    }
}
