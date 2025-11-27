using Simulator.Domain.Enums;

namespace Simulator.Infrastructure.Services
{
    public interface ISimulatorEngine
    {
        (SimulatorState State, long Increment) Advance(
            double totalSecondsInCurrentState,
            double nominalSpeedPcsPerHour,
            double maxPhysicalSpeedPcsPerSec,
            double lowSpeedThresholdFactor,
            int maxCounterValue,
            double elapsedSecondsForIncrement);
    }
}
