using Normalizer.Domain.Enums;

namespace Normalizer.Domain.Events
{
    public record LineStatusChangedEvent
    {
        public required DateTime Timestamp { get; init; }
        public required LineStatus Status { get; init; }
    }
}
