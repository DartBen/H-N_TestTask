using Simulator.Domain.Enums;

namespace Simulator.Infrastructure.Services
{
    public class SimulatorEngine : ISimulatorEngine
    {
        private readonly Random _random = new();

        public (SimulatorState State, long Increment) Advance(
            double totalSecondsInCurrentState,
            double nominalSpeedPcsPerHour,
            double maxPhysicalSpeedPcsPerSec,
            double lowSpeedThresholdFactor,
            int maxCounterValue,
            double elapsedSecondsForIncrement)
        {

            double speedPcsPerSec = 0;

            var state = DetermineState(totalSecondsInCurrentState);

            switch (state)
            {
                case SimulatorState.Normal:
                    speedPcsPerSec = maxPhysicalSpeedPcsPerSec * 0.93;
                    break;
                case SimulatorState.LowSpeed:
                    speedPcsPerSec = (nominalSpeedPcsPerHour * lowSpeedThresholdFactor) / 3600;
                    break;
                default:
                    speedPcsPerSec = 0;
                    break;
            }
            ;

            var increment = (long)(speedPcsPerSec * elapsedSecondsForIncrement);
            return (state, increment);
        }

        private SimulatorState DetermineState(double totalSeconds)
        {
            int cycle = ((int)(totalSeconds / 45)) % 4;
            return cycle switch
            {
                0 => SimulatorState.Normal,
                1 => SimulatorState.LowSpeed,
                2 => SimulatorState.Stopped,
                3 => SimulatorState.NoData,
                _ => SimulatorState.Normal
            };
        }
    }
}
