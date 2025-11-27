namespace Simulator.Infrastructure.Options
{
    public class SimulatorOptions
    {
        public const string SectionName = "Simulator";

        public int MaxCounterValue { get; set; } = 10_000;
        public double NominalSpeedPcsPerHour { get; set; } = 10_000;
        public double MaxPhysicalSpeedPcsPerSec { get; set; } = 3.0;
        public double LowSpeedThresholdFactor { get; set; } = 0.95;
    }
}
