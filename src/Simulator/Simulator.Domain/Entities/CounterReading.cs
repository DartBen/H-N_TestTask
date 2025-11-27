namespace Simulator.Domain.Entities
{
    public record CounterReading
    {
        public required DateTime Timestamp { get; init; }
        public required long CounterValue { get; init; }
    }
}
