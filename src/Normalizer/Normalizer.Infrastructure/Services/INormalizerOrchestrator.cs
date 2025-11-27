using Normalizer.Domain.Enums;

namespace Normalizer.Infrastructure.Services
{
    public interface INormalizerOrchestrator
    {
        Task ProcessAsync(CancellationToken cancellationToken = default);
        LineStatus CurrentStatus { get; }
        double CurrentSpeedPcsPerHour { get; }
    }
}
