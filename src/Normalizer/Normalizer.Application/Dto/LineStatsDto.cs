namespace Normalizer.Application.Dto
{
    public record LineStatsDto
    {
        public DateTime From { get; init; }
        public DateTime To { get; init; }
        public Dictionary<string, int> EventsCount { get; init; } = new();
    }
}
