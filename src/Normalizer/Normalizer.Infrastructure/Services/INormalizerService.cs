using Normalizer.Domain.Enums;

namespace Normalizer.Infrastructure.Services
{
    public interface INormalizerService
    {
        LineStatus CurrentStatus { get; }
        double CurrentSpeedPcsPerHour { get; }
    }
}
