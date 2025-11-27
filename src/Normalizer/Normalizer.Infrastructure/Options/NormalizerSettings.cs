namespace Normalizer.Infrastructure.Options;

public class NormalizerSettings
{
    public const string SectionName = "Normalizer";
    public string SimulatorUrl { get; set; } = "https://localhost:7215";
    public double NominalSpeedPcsPerHour { get; set; } = 10_000;
    public double LowSpeedThresholdFactor { get; set; } = 0.95;
    public int StoppedThresholdSeconds { get; set; } = 10;
    public int NoDataThresholdSeconds { get; set; } = 5;
    public int PollingIntervalMs { get; set; } = 1500;
}