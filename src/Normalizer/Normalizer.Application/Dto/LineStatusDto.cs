using Normalizer.Domain.Enums;

namespace Normalizer.Application.Dto
{
    public record LineStatusDto
    {
        public DateTime Timestamp { get; init; }
        public string Status { get; init; } = string.Empty;
        public double SpeedPcsPerHour { get; init; }
    }
}
